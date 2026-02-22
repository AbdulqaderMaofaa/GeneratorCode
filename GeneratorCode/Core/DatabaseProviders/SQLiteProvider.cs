using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using Microsoft.Data.Sqlite;

namespace GeneratorCode.Core.DatabaseProviders
{
    /// <summary>
    /// موفر SQLite
    /// </summary>
    public class SQLiteProvider : IDatabaseProvider
    {
        public DatabaseType DatabaseType => DatabaseType.SQLite;
        public string Name => "SQLite";

        public List<TableInfo> GetTables(string connectionString)
        {
            var tables = new List<TableInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                const string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
                using (var command = new SqliteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tableName = reader.GetString(0);
                        var indexes = GetTableIndexes(connection, tableName);
                        var triggers = GetTableTriggers(connection, tableName);
                        long rowCount = 0;
                        try
                        {
                            using (var countCmd = new SqliteCommand($"SELECT COUNT(*) FROM \"{EscapeIdentifier(tableName)}\"", connection))
                            {
                                var obj = countCmd.ExecuteScalar();
                                if (obj != null && obj != DBNull.Value)
                                    rowCount = Convert.ToInt64(obj);
                            }
                        }
                        catch { /* ignore */ }

                        var table = new TableInfo
                        {
                            Schema = "main",
                            Name = tableName,
                            Description = "",
                            RowCount = rowCount,
                            Size = "0 B",
                            CreatedDate = default,
                            ModifiedDate = default,
                            HasIndexes = indexes.Count > 0,
                            HasTriggers = triggers.Count > 0,
                            Type = "table",
                            Engine = "SQLite",
                            Collation = "",
                            Columns = GetColumns(connectionString, tableName),
                            PrimaryKeys = GetPrimaryKeys(connectionString, tableName),
                            ForeignKeys = GetForeignKeys(connectionString, tableName),
                            Indexes = indexes,
                            Triggers = triggers
                        };
                        tables.Add(table);
                    }
                }
            }
            return tables;
        }

        public List<ColumnInfo> GetColumns(string connectionString, string tableName)
        {
            var columns = new List<ColumnInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var primaryKeys = GetPrimaryKeys(connectionString, tableName);
                var foreignKeys = GetForeignKeys(connectionString, tableName);
                var fkColumns = new HashSet<string>(foreignKeys.Select(fk => fk.LocalColumn));

                using (var command = new SqliteCommand($"PRAGMA table_info(\"{EscapeIdentifier(tableName)}\")", connection))
                using (var reader = command.ExecuteReader())
                {
                    int ordinal = 0;
                    while (reader.Read())
                    {
                        var name = reader["name"].ToString();
                        var type = reader["type"].ToString();
                        var notnull = Convert.ToInt32(reader["notnull"]) != 0;
                        var dflt = reader["dflt_value"];
                        var pk = Convert.ToInt32(reader["pk"]) != 0;

                        var column = new ColumnInfo
                        {
                            Name = name,
                            DataType = type,
                            IsNullable = !notnull,
                            DefaultValue = dflt == DBNull.Value || dflt == null ? null : dflt.ToString(),
                            MaxLength = null,
                            Precision = null,
                            Scale = null,
                            OrdinalPosition = ordinal++,
                            IsPrimaryKey = primaryKeys.Contains(name),
                            IsForeignKey = fkColumns.Contains(name),
                            IsAutoIncrement = pk && type.Equals("INTEGER", StringComparison.OrdinalIgnoreCase),
                            Description = ""
                        };
                        column.CSharpType = MapDataType(column.DataType, column.IsNullable);
                        columns.Add(column);
                    }
                }
            }
            return columns;
        }

        public List<string> GetPrimaryKeys(string connectionString, string tableName)
        {
            var primaryKeys = new List<string>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand($"PRAGMA table_info(\"{EscapeIdentifier(tableName)}\")", connection))
                using (var reader = command.ExecuteReader())
                {
                    var pkColumns = new List<(int keyOrder, string name)>();
                    while (reader.Read())
                    {
                        var pk = Convert.ToInt32(reader["pk"]);
                        if (pk > 0)
                            pkColumns.Add((pk, reader["name"].ToString()));
                    }
                    primaryKeys = pkColumns.OrderBy(x => x.keyOrder).Select(x => x.name).ToList();
                }
            }
            return primaryKeys;
        }

        public List<ForeignKeyInfo> GetForeignKeys(string connectionString, string tableName)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand($"PRAGMA foreign_key_list(\"{EscapeIdentifier(tableName)}\")", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader["id"];
                        var from = reader["from"].ToString();
                        var table = reader["table"].ToString();
                        var to = reader["to"].ToString();
                        var onUpdate = reader["on_update"].ToString();
                        var onDelete = reader["on_delete"].ToString();

                        foreignKeys.Add(new ForeignKeyInfo
                        {
                            Name = $"FK_{tableName}_{from}_{table}_{to}",
                            LocalColumn = from,
                            ReferencedTable = table,
                            ReferencedColumn = to,
                            DeleteAction = string.IsNullOrEmpty(onDelete) ? "NO ACTION" : onDelete.ToUpperInvariant(),
                            UpdateAction = string.IsNullOrEmpty(onUpdate) ? "NO ACTION" : onUpdate.ToUpperInvariant()
                        });
                    }
                }
            }
            return foreignKeys;
        }

        public string MapDataType(string dbType, bool isNullable)
        {
            var csharpType = (dbType ?? "").ToUpperInvariant() switch
            {
                "TEXT" or "CHAR" or "VARCHAR" or "CLOB" => "string",
                "INTEGER" or "INT" => "int",
                "BIGINT" => "long",
                "REAL" or "FLOAT" or "DOUBLE" => "double",
                "BLOB" => "byte[]",
                "NUMERIC" or "DECIMAL" => "decimal",
                "BOOLEAN" => "bool",
                "DATE" or "DATETIME" => "DateTime",
                _ => "object"
            };

            if (isNullable && csharpType != "string" && csharpType != "byte[]" && csharpType != "object")
                csharpType += "?";
            return csharpType;
        }

        public string BuildConnectionString(string server, string database, string username, string password, Dictionary<string, string> additionalParams = null)
        {
            var cs = $"Data Source={database ?? ""}";
            if (additionalParams != null)
            {
                foreach (var p in additionalParams)
                    cs += $";{p.Key}={p.Value}";
            }
            return cs;
        }

        public bool TestConnection(string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                return true;
            }
        }

        private static string EscapeIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return name.Replace("\"", "\"\"");
        }

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{Math.Round(size, 2)} {sizes[order]}";
        }

        private List<IndexInfo> GetTableIndexes(SqliteConnection connection, string tableName)
        {
            var indexes = new List<IndexInfo>();
            var indexNames = new List<(string name, int unique)>();
            using (var cmd = new SqliteCommand($"PRAGMA index_list(\"{EscapeIdentifier(tableName)}\")", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    indexNames.Add((reader["name"].ToString(), Convert.ToInt32(reader["unique"])));
            }

            foreach (var (indexName, isUnique) in indexNames)
            {
                var columns = new List<string>();
                try
                {
                    using (var cmd = new SqliteCommand($"PRAGMA index_info(\"{EscapeIdentifier(indexName)}\")", connection))
                    using (var r = cmd.ExecuteReader())
                    {
                        var ordered = new List<(int seq, string name)>();
                        while (r.Read())
                            ordered.Add((Convert.ToInt32(r["seqno"]), r["name"].ToString()));
                        columns = ordered.OrderBy(x => x.seq).Select(x => x.name).ToList();
                    }
                }
                catch { /* ignore */ }

                indexes.Add(new IndexInfo
                {
                    Name = indexName,
                    Type = "index",
                    Columns = columns,
                    IsUnique = isUnique != 0,
                    IsPrimary = indexName.StartsWith("sqlite_", StringComparison.OrdinalIgnoreCase) && indexName.Contains("primary"),
                    Description = ""
                });
            }
            return indexes;
        }

        private List<TriggerInfo> GetTableTriggers(SqliteConnection connection, string tableName)
        {
            var triggers = new List<TriggerInfo>();
            using (var command = new SqliteCommand("SELECT name, sql FROM sqlite_master WHERE type='trigger' AND tbl_name=@tblName", connection))
            {
                command.Parameters.AddWithValue("@tblName", tableName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["name"].ToString();
                        var sql = reader["sql"]?.ToString() ?? "";
                        ParseTriggerSql(sql, name, out var timing, out var eventType);
                        triggers.Add(new TriggerInfo
                        {
                            Name = name,
                            Timing = timing,
                            Event = eventType,
                            Definition = sql,
                            Description = ""
                        });
                    }
                }
            }
            return triggers;
        }

        private static void ParseTriggerSql(string sql, string defaultName, out string timing, out string eventType)
        {
            timing = "AFTER";
            eventType = "UNKNOWN";
            if (string.IsNullOrWhiteSpace(sql)) return;
            var u = sql.Trim().ToUpperInvariant();
            if (u.Contains("BEFORE "))
                timing = "BEFORE";
            else if (u.Contains("AFTER "))
                timing = "AFTER";
            else if (u.Contains("INSTEAD OF "))
                timing = "INSTEAD OF";

            if (u.Contains(" INSERT ")) eventType = "INSERT";
            else if (u.Contains(" DELETE ")) eventType = "DELETE";
            else if (u.Contains(" UPDATE ")) eventType = "UPDATE";
        }
    }
}
