using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using Oracle.ManagedDataAccess.Client;

namespace GeneratorCode.Core.DatabaseProviders
{
    public class OracleProvider : IDatabaseProvider
    {
        public DatabaseType DatabaseType => DatabaseType.Oracle;
        public string Name => "Oracle";

        public List<TableInfo> GetTables(string connectionString)
        {
            var tables = new List<TableInfo>();
            using var connection = new OracleConnection(connectionString);
            connection.Open();

            var query = @"
                SELECT 
                    t.TABLE_NAME,
                    NVL(c.COMMENTS, '') AS TABLE_COMMENT,
                    NVL(t.NUM_ROWS, 0) AS ROW_COUNT
                FROM USER_TABLES t
                LEFT JOIN USER_TAB_COMMENTS c ON c.TABLE_NAME = t.TABLE_NAME
                ORDER BY t.TABLE_NAME";

            using var command = new OracleCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var tableName = reader["TABLE_NAME"].ToString();
                var indexes = GetTableIndexes(connection, tableName);
                var triggers = GetTableTriggers(connection, tableName);

                var table = new TableInfo
                {
                    Schema = GetCurrentSchema(connection),
                    Name = tableName,
                    Description = reader["TABLE_COMMENT"].ToString(),
                    RowCount = reader["ROW_COUNT"] is DBNull ? 0 : Convert.ToInt64(reader["ROW_COUNT"]),
                    Size = "0 B",
                    CreatedDate = default,
                    ModifiedDate = default,
                    HasIndexes = indexes.Count > 0,
                    HasTriggers = triggers.Count > 0,
                    Type = "TABLE",
                    Engine = "Oracle",
                    Collation = "",
                    Columns = new List<ColumnInfo>(),
                    PrimaryKeys = new List<string>(),
                    ForeignKeys = new List<ForeignKeyInfo>(),
                    Indexes = indexes,
                    Triggers = triggers
                };

                tables.Add(table);
            }

            foreach (var table in tables)
            {
                table.Columns = GetColumns(connectionString, table.Name);
                table.PrimaryKeys = GetPrimaryKeys(connectionString, table.Name);
                table.ForeignKeys = GetForeignKeys(connectionString, table.Name);
            }

            return tables;
        }

        public List<ColumnInfo> GetColumns(string connectionString, string tableName)
        {
            var columns = new List<ColumnInfo>();
            using var connection = new OracleConnection(connectionString);
            connection.Open();

            var primaryKeys = GetPrimaryKeys(connectionString, tableName);
            var foreignKeys = GetForeignKeys(connectionString, tableName);
            var fkColumns = new HashSet<string>(foreignKeys.Select(fk => fk.LocalColumn));

            var query = @"
                SELECT 
                    c.COLUMN_NAME,
                    c.DATA_TYPE,
                    c.NULLABLE,
                    c.DATA_DEFAULT,
                    c.CHAR_LENGTH,
                    c.DATA_PRECISION,
                    c.DATA_SCALE,
                    c.COLUMN_ID,
                    NVL(cc.COMMENTS, '') AS COLUMN_COMMENT
                FROM USER_TAB_COLUMNS c
                LEFT JOIN USER_COL_COMMENTS cc ON cc.TABLE_NAME = c.TABLE_NAME AND cc.COLUMN_NAME = c.COLUMN_NAME
                WHERE c.TABLE_NAME = :tableName
                ORDER BY c.COLUMN_ID";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add(new OracleParameter("tableName", tableName));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader["COLUMN_NAME"].ToString();
                var dataType = reader["DATA_TYPE"].ToString();
                var isNullable = reader["NULLABLE"].ToString() == "Y";

                var column = new ColumnInfo
                {
                    Name = name,
                    DataType = dataType,
                    IsNullable = isNullable,
                    DefaultValue = reader["DATA_DEFAULT"] is DBNull ? null : reader["DATA_DEFAULT"]?.ToString()?.Trim(),
                    MaxLength = reader["CHAR_LENGTH"] is DBNull ? null : Convert.ToInt32(reader["CHAR_LENGTH"]),
                    Precision = reader["DATA_PRECISION"] is DBNull ? null : Convert.ToInt32(reader["DATA_PRECISION"]),
                    Scale = reader["DATA_SCALE"] is DBNull ? null : Convert.ToInt32(reader["DATA_SCALE"]),
                    OrdinalPosition = reader["COLUMN_ID"] is DBNull ? 0 : Convert.ToInt32(reader["COLUMN_ID"]),
                    IsPrimaryKey = primaryKeys.Contains(name),
                    IsForeignKey = fkColumns.Contains(name),
                    IsAutoIncrement = false,
                    Description = reader["COLUMN_COMMENT"].ToString()
                };

                column.CSharpType = MapDataType(column.DataType, column.IsNullable);
                columns.Add(column);
            }

            return columns;
        }

        public List<string> GetPrimaryKeys(string connectionString, string tableName)
        {
            var primaryKeys = new List<string>();
            using var connection = new OracleConnection(connectionString);
            connection.Open();

            var query = @"
                SELECT cc.COLUMN_NAME
                FROM USER_CONSTRAINTS c
                JOIN USER_CONS_COLUMNS cc ON c.CONSTRAINT_NAME = cc.CONSTRAINT_NAME
                WHERE c.TABLE_NAME = :tableName AND c.CONSTRAINT_TYPE = 'P'
                ORDER BY cc.POSITION";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add(new OracleParameter("tableName", tableName));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                primaryKeys.Add(reader["COLUMN_NAME"].ToString());
            }

            return primaryKeys;
        }

        public List<ForeignKeyInfo> GetForeignKeys(string connectionString, string tableName)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            using var connection = new OracleConnection(connectionString);
            connection.Open();

            var query = @"
                SELECT 
                    c.CONSTRAINT_NAME AS FK_NAME,
                    cc.COLUMN_NAME AS LOCAL_COLUMN,
                    rc.TABLE_NAME AS REFERENCED_TABLE,
                    rcc.COLUMN_NAME AS REFERENCED_COLUMN,
                    c.DELETE_RULE
                FROM USER_CONSTRAINTS c
                JOIN USER_CONS_COLUMNS cc ON c.CONSTRAINT_NAME = cc.CONSTRAINT_NAME
                JOIN USER_CONSTRAINTS rc ON c.R_CONSTRAINT_NAME = rc.CONSTRAINT_NAME
                JOIN USER_CONS_COLUMNS rcc ON rc.CONSTRAINT_NAME = rcc.CONSTRAINT_NAME AND cc.POSITION = rcc.POSITION
                WHERE c.TABLE_NAME = :tableName AND c.CONSTRAINT_TYPE = 'R'";

            using var command = new OracleCommand(query, connection);
            command.Parameters.Add(new OracleParameter("tableName", tableName));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                foreignKeys.Add(new ForeignKeyInfo
                {
                    Name = reader["FK_NAME"].ToString(),
                    LocalColumn = reader["LOCAL_COLUMN"].ToString(),
                    ReferencedTable = reader["REFERENCED_TABLE"].ToString(),
                    ReferencedColumn = reader["REFERENCED_COLUMN"].ToString(),
                    DeleteAction = reader["DELETE_RULE"]?.ToString() ?? "NO ACTION",
                    UpdateAction = "NO ACTION"
                });
            }

            return foreignKeys;
        }

        public string MapDataType(string dbType, bool isNullable)
        {
            var csharpType = (dbType ?? "").ToUpperInvariant() switch
            {
                "NUMBER" => "decimal",
                "FLOAT" or "BINARY_FLOAT" => "float",
                "BINARY_DOUBLE" => "double",
                "VARCHAR2" or "NVARCHAR2" or "CHAR" or "NCHAR" or "CLOB" or "NCLOB" or "LONG" => "string",
                "DATE" or "TIMESTAMP" => "DateTime",
                var t when t.StartsWith("TIMESTAMP") => "DateTime",
                "BLOB" or "RAW" or "LONG RAW" => "byte[]",
                "XMLTYPE" => "string",
                _ => "object"
            };

            if (isNullable && csharpType != "string" && csharpType != "byte[]" && csharpType != "object")
                csharpType += "?";

            return csharpType;
        }

        public string BuildConnectionString(string server, string database, string username, string password, Dictionary<string, string> additionalParams = null)
        {
            var port = 1521;
            if (additionalParams != null && additionalParams.TryGetValue("port", out var portStr))
                _ = int.TryParse(portStr, out port);

            return $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={server})(PORT={port}))(CONNECT_DATA=(SID={database})));User Id={username};Password={password};";
        }

        public bool TestConnection(string connectionString)
        {
            using var connection = new OracleConnection(connectionString);
            connection.Open();
            return true;
        }

        private static string GetCurrentSchema(OracleConnection connection)
        {
            try
            {
                using var cmd = new OracleCommand("SELECT USER FROM DUAL", connection);
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static List<IndexInfo> GetTableIndexes(OracleConnection connection, string tableName)
        {
            var indexes = new List<IndexInfo>();
            try
            {
                var query = @"
                    SELECT 
                        i.INDEX_NAME,
                        i.INDEX_TYPE,
                        i.UNIQUENESS,
                        LISTAGG(ic.COLUMN_NAME, ', ') WITHIN GROUP (ORDER BY ic.COLUMN_POSITION) AS COLUMN_NAMES
                    FROM USER_INDEXES i
                    JOIN USER_IND_COLUMNS ic ON i.INDEX_NAME = ic.INDEX_NAME
                    WHERE i.TABLE_NAME = :tableName
                    GROUP BY i.INDEX_NAME, i.INDEX_TYPE, i.UNIQUENESS";

                using var cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(new OracleParameter("tableName", tableName));

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    indexes.Add(new IndexInfo
                    {
                        Name = reader["INDEX_NAME"].ToString(),
                        Type = reader["INDEX_TYPE"].ToString(),
                        Columns = reader["COLUMN_NAMES"].ToString().Split(',').Select(c => c.Trim()).ToList(),
                        IsUnique = reader["UNIQUENESS"].ToString() == "UNIQUE",
                        IsPrimary = false,
                        Description = ""
                    });
                }
            }
            catch { }

            return indexes;
        }

        private static List<TriggerInfo> GetTableTriggers(OracleConnection connection, string tableName)
        {
            var triggers = new List<TriggerInfo>();
            try
            {
                var query = @"
                    SELECT 
                        TRIGGER_NAME,
                        TRIGGER_TYPE,
                        TRIGGERING_EVENT,
                        TRIGGER_BODY
                    FROM USER_TRIGGERS
                    WHERE TABLE_NAME = :tableName";

                using var cmd = new OracleCommand(query, connection);
                cmd.Parameters.Add(new OracleParameter("tableName", tableName));

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var triggerType = reader["TRIGGER_TYPE"].ToString();
                    var timing = triggerType.Contains("BEFORE") ? "BEFORE" :
                                 triggerType.Contains("AFTER") ? "AFTER" :
                                 triggerType.Contains("INSTEAD") ? "INSTEAD OF" : "AFTER";

                    triggers.Add(new TriggerInfo
                    {
                        Name = reader["TRIGGER_NAME"].ToString(),
                        Timing = timing,
                        Event = reader["TRIGGERING_EVENT"].ToString(),
                        Definition = reader["TRIGGER_BODY"]?.ToString() ?? "",
                        Description = ""
                    });
                }
            }
            catch { }

            return triggers;
        }
    }
}
