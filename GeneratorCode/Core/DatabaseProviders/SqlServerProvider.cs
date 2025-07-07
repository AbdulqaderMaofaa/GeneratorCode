using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using System.Linq;

namespace GeneratorCode.Core.DatabaseProviders
{
    /// <summary>
    /// موفر SQL Server
    /// </summary>
    public class SqlServerProvider : IDatabaseProvider
    {
        public DatabaseType DatabaseType => DatabaseType.SqlServer;
        public string Name => "SQL Server";
        
        public List<TableInfo> GetTables(string connectionString)
        {
            var tables = new List<TableInfo>();
            
            var query = @"
                SELECT 
                    t.TABLE_SCHEMA,
                    t.TABLE_NAME,
                    ISNULL(ep.value, '') as TABLE_COMMENT,
                    p.rows as ROW_COUNT,
                    SUM(a.total_pages) * 8 * 1024 as TABLE_SIZE,
                    t.CREATE_DATE,
                    t.MODIFY_DATE,
                    CASE WHEN EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = t.object_id) THEN 1 ELSE 0 END as HAS_INDEXES,
                    CASE WHEN EXISTS (SELECT 1 FROM sys.triggers tr WHERE tr.parent_id = t.object_id) THEN 1 ELSE 0 END as HAS_TRIGGERS,
                    t.type_desc as TABLE_TYPE,
                    'SQL Server' as ENGINE,
                    COLLATION_NAME
                FROM sys.tables t
                LEFT JOIN sys.extended_properties ep ON ep.major_id = t.object_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
                LEFT JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0, 1)
                LEFT JOIN sys.allocation_units a ON a.container_id = p.partition_id
                LEFT JOIN sys.objects o ON o.object_id = t.object_id
                GROUP BY t.TABLE_SCHEMA, t.TABLE_NAME, ep.value, p.rows, t.object_id, t.type_desc, t.CREATE_DATE, t.MODIFY_DATE, COLLATION_NAME
                ORDER BY t.TABLE_SCHEMA, t.TABLE_NAME";
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableName = reader["TABLE_NAME"].ToString();
                            var table = new TableInfo
                            {
                                Schema = reader["TABLE_SCHEMA"].ToString(),
                                Name = tableName,
                                Description = reader["TABLE_COMMENT"].ToString(),
                                RowCount = Convert.ToInt64(reader["ROW_COUNT"]),
                                Size = FormatSize(Convert.ToInt64(reader["TABLE_SIZE"])),
                                CreatedDate = Convert.ToDateTime(reader["CREATE_DATE"]),
                                ModifiedDate = Convert.ToDateTime(reader["MODIFY_DATE"]),
                                HasIndexes = Convert.ToBoolean(reader["HAS_INDEXES"]),
                                HasTriggers = Convert.ToBoolean(reader["HAS_TRIGGERS"]),
                                Type = reader["TABLE_TYPE"].ToString(),
                                Engine = reader["ENGINE"].ToString(),
                                Collation = reader["COLLATION_NAME"].ToString(),
                                Columns = new List<ColumnInfo>(),
                                PrimaryKeys = new List<string>(),
                                ForeignKeys = new List<ForeignKeyInfo>(),
                                Indexes = GetTableIndexes(connection, tableName),
                                Triggers = GetTableTriggers(connection, tableName)
                            };
                            
                            tables.Add(table);
                        }
                    }
                }

                // تحميل معلومات الأعمدة والمفاتيح لكل جدول
                foreach (var table in tables)
                {
                    table.Columns = GetColumns(connectionString, table.Name);
                    table.PrimaryKeys = GetPrimaryKeys(connectionString, table.Name);
                    table.ForeignKeys = GetForeignKeys(connectionString, table.Name);
                }
            }
            
            return tables;
        }
        
        public List<ColumnInfo> GetColumns(string connectionString, string tableName)
        {
            var columns = new List<ColumnInfo>();
            
            var query = @"
                SELECT 
                    c.COLUMN_NAME,
                    c.DATA_TYPE,
                    c.IS_NULLABLE,
                    c.COLUMN_DEFAULT,
                    c.CHARACTER_MAXIMUM_LENGTH,
                    c.NUMERIC_PRECISION,
                    c.NUMERIC_SCALE,
                    c.ORDINAL_POSITION,
                    CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY,
                    CASE WHEN fk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_FOREIGN_KEY,
                    CASE WHEN ic.is_identity = 1 THEN 1 ELSE 0 END AS IS_IDENTITY,
                    ISNULL(ep.value, '') as COLUMN_DESCRIPTION
                FROM INFORMATION_SCHEMA.COLUMNS c
                LEFT JOIN (
                    SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
                LEFT JOIN (
                    SELECT ku.TABLE_NAME, ku.COLUMN_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    WHERE tc.CONSTRAINT_TYPE = 'FOREIGN KEY'
                ) fk ON c.TABLE_NAME = fk.TABLE_NAME AND c.COLUMN_NAME = fk.COLUMN_NAME
                LEFT JOIN sys.tables st ON st.name = c.TABLE_NAME
                LEFT JOIN sys.columns sc ON sc.object_id = st.object_id AND sc.name = c.COLUMN_NAME
                LEFT JOIN sys.identity_columns ic ON ic.object_id = sc.object_id AND ic.column_id = sc.column_id
                LEFT JOIN sys.extended_properties ep ON ep.major_id = sc.object_id AND ep.minor_id = sc.column_id AND ep.name = 'MS_Description'
                WHERE c.TABLE_NAME = @TableName
                ORDER BY c.ORDINAL_POSITION";
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var column = new ColumnInfo
                            {
                                Name = reader["COLUMN_NAME"].ToString(),
                                DataType = reader["DATA_TYPE"].ToString(),
                                IsNullable = reader["IS_NULLABLE"].ToString().ToUpper() == "YES",
                                DefaultValue = reader["COLUMN_DEFAULT"]?.ToString(),
                                MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"] as int?,
                                Precision = reader["NUMERIC_PRECISION"] as int?,
                                Scale = reader["NUMERIC_SCALE"] as int?,
                                OrdinalPosition = Convert.ToInt32(reader["ORDINAL_POSITION"]),
                                IsPrimaryKey = Convert.ToBoolean(reader["IS_PRIMARY_KEY"]),
                                IsForeignKey = Convert.ToBoolean(reader["IS_FOREIGN_KEY"]),
                                IsAutoIncrement = Convert.ToBoolean(reader["IS_IDENTITY"]),
                                Description = reader["COLUMN_DESCRIPTION"].ToString()
                            };
                            
                            column.CSharpType = MapDataType(column.DataType, column.IsNullable);
                            columns.Add(column);
                        }
                    }
                }
            }
            
            return columns;
        }
        
        public List<string> GetPrimaryKeys(string connectionString, string tableName)
        {
            var primaryKeys = new List<string>();
            
            var query = @"
                SELECT ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                WHERE tc.TABLE_NAME = @TableName AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                ORDER BY ku.ORDINAL_POSITION";
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            primaryKeys.Add(reader["COLUMN_NAME"].ToString());
                        }
                    }
                }
            }
            
            return primaryKeys;
        }
        
        public List<ForeignKeyInfo> GetForeignKeys(string connectionString, string tableName)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            
            var query = @"
                SELECT 
                    fk.name AS FK_NAME,
                    cp.name AS COLUMN_NAME,
                    rt.name AS REFERENCED_TABLE,
                    rp.name AS REFERENCED_COLUMN,
                    fk.delete_referential_action_desc AS DELETE_ACTION,
                    fk.update_referential_action_desc AS UPDATE_ACTION
                FROM sys.foreign_keys fk
                INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                INNER JOIN sys.tables t ON t.object_id = fk.parent_object_id
                INNER JOIN sys.columns cp ON cp.object_id = t.object_id AND cp.column_id = fkc.parent_column_id
                INNER JOIN sys.tables rt ON rt.object_id = fk.referenced_object_id
                INNER JOIN sys.columns rp ON rp.object_id = rt.object_id AND rp.column_id = fkc.referenced_column_id
                WHERE t.name = @TableName";
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var foreignKey = new ForeignKeyInfo
                            {
                                Name = reader["FK_NAME"].ToString(),
                                LocalColumn = reader["COLUMN_NAME"].ToString(),
                                ReferencedTable = reader["REFERENCED_TABLE"].ToString(),
                                ReferencedColumn = reader["REFERENCED_COLUMN"].ToString(),
                                DeleteAction = reader["DELETE_ACTION"].ToString(),
                                UpdateAction = reader["UPDATE_ACTION"].ToString()
                            };
                            
                            foreignKeys.Add(foreignKey);
                        }
                    }
                }
            }
            
            return foreignKeys;
        }
        
        public string MapDataType(string dbType, bool isNullable)
        {
            var csharpType = dbType.ToLower() switch
            {
                "int" or "integer" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
                "float" => "double",
                "real" => "float",
                "datetime" or "datetime2" or "smalldatetime" => "DateTime",
                "date" => "DateTime",
                "time" => "TimeSpan",
                "datetimeoffset" => "DateTimeOffset",
                "char" or "nchar" or "varchar" or "nvarchar" or "text" or "ntext" => "string",
                "uniqueidentifier" => "Guid",
                "binary" or "varbinary" or "image" => "byte[]",
                "xml" => "string",
                "sql_variant" => "object",
                _ => "object"
            };
            
            // إضافة ? للأنواع القابلة للإرجاع null
            if (isNullable && csharpType != "string" && csharpType != "byte[]" && csharpType != "object")
            {
                csharpType += "?";
            }
            
            return csharpType;
        }
        
        public string BuildConnectionString(string server, string database, string username, string password, Dictionary<string, string> additionalParams = null)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                UserID = username,
                Password = password,
                IntegratedSecurity = string.IsNullOrEmpty(username),
                TrustServerCertificate = true
            };
            
            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    builder[param.Key] = param.Value;
                }
            }
            
            return builder.ConnectionString;
        }
        
        public bool TestConnection(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }
            
            return $"{Math.Round(size, 2)} {sizes[order]}";
        }

        private List<IndexInfo> GetTableIndexes(SqlConnection connection, string tableName)
        {
            var indexes = new List<IndexInfo>();
            var query = @"
                SELECT 
                    i.name as INDEX_NAME,
                    i.type_desc as INDEX_TYPE,
                    STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) as COLUMN_NAMES,
                    i.is_unique as IS_UNIQUE,
                    i.is_primary_key as IS_PRIMARY,
                    ISNULL(ep.value, '') as INDEX_COMMENT
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                LEFT JOIN sys.extended_properties ep ON ep.major_id = i.object_id AND ep.minor_id = i.index_id AND ep.name = 'MS_Description'
                WHERE t.name = @tableName
                GROUP BY i.name, i.type_desc, i.is_unique, i.is_primary_key, ep.value";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        indexes.Add(new IndexInfo
                        {
                            Name = reader["INDEX_NAME"].ToString(),
                            Type = reader["INDEX_TYPE"].ToString(),
                            Columns = reader["COLUMN_NAMES"].ToString().Split(',').Select(c => c.Trim()).ToList(),
                            IsUnique = Convert.ToBoolean(reader["IS_UNIQUE"]),
                            IsPrimary = Convert.ToBoolean(reader["IS_PRIMARY"]),
                            Description = reader["INDEX_COMMENT"].ToString()
                        });
                    }
                }
            }
            return indexes;
        }

        private List<TriggerInfo> GetTableTriggers(SqlConnection connection, string tableName)
        {
            var triggers = new List<TriggerInfo>();
            var query = @"
                SELECT 
                    tr.name as TRIGGER_NAME,
                    CASE 
                        WHEN is_instead_of_trigger = 1 THEN 'INSTEAD OF'
                        ELSE 'AFTER'
                    END as TIMING,
                    CASE
                        WHEN is_insert_trigger = 1 THEN 'INSERT'
                        WHEN is_update_trigger = 1 THEN 'UPDATE'
                        WHEN is_delete_trigger = 1 THEN 'DELETE'
                        ELSE 'UNKNOWN'
                    END as EVENT,
                    OBJECT_DEFINITION(tr.object_id) as DEFINITION,
                    ISNULL(ep.value, '') as TRIGGER_COMMENT
                FROM sys.triggers tr
                INNER JOIN sys.tables t ON tr.parent_id = t.object_id
                LEFT JOIN sys.extended_properties ep ON ep.major_id = tr.object_id AND ep.minor_id = 0 AND ep.name = 'MS_Description'
                WHERE t.name = @tableName";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        triggers.Add(new TriggerInfo
                        {
                            Name = reader["TRIGGER_NAME"].ToString(),
                            Timing = reader["TIMING"].ToString(),
                            Event = reader["EVENT"].ToString(),
                            Definition = reader["DEFINITION"].ToString(),
                            Description = reader["TRIGGER_COMMENT"].ToString()
                        });
                    }
                }
            }
            return triggers;
        }
    }
} 