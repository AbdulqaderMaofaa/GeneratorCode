using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using System.Linq;

namespace GeneratorCode.Core.DatabaseProviders
{
    /// <summary>
    /// موفر MySQL
    /// </summary>
    public class MySqlProvider : IDatabaseProvider
    {
        public DatabaseType DatabaseType => DatabaseType.MySql;
        public string Name => "MySQL";
        
        public List<TableInfo> GetTables(string connectionString)
        {
            var tables = new List<TableInfo>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT table_name, table_schema 
                    FROM information_schema.tables 
                    WHERE table_schema = DATABASE()
                    AND table_type = 'BASE TABLE'
                    ORDER BY table_name";

                using (var command = new MySqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(new TableInfo
                        {
                            Name = reader.GetString(0),
                            Schema = reader.GetString(1)
                        });
                    }
                }
            }
            return tables;
        }
        
        public List<ColumnInfo> GetColumns(string connectionString, string tableName)
        {
            var columns = new List<ColumnInfo>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT 
                        column_name,
                        data_type,
                        character_maximum_length,
                        numeric_precision,
                        numeric_scale,
                        is_nullable,
                        column_default,
                        ordinal_position
                    FROM information_schema.columns
                    WHERE table_name = @tableName
                    AND table_schema = DATABASE()
                    ORDER BY ordinal_position";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(new ColumnInfo
                            {
                                Name = reader.GetString(0),
                                DataType = reader.GetString(1),
                                MaxLength = reader.IsDBNull(2) ? null : (int?)reader.GetInt64(2),
                                Precision = reader.IsDBNull(3) ? null : (int?)reader.GetInt64(3),
                                Scale = reader.IsDBNull(4) ? null : (int?)reader.GetInt64(4),
                                IsNullable = reader.GetString(5) == "YES",
                                DefaultValue = reader.IsDBNull(6) ? null : reader.GetString(6),
                                OrdinalPosition = reader.GetInt32(7)
                            });
                        }
                    }
                }
            }
            return columns;
        }
        
        public List<string> GetPrimaryKeys(string connectionString, string tableName)
        {
            var primaryKeys = new List<string>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT column_name
                    FROM information_schema.key_column_usage
                    WHERE table_name = @tableName
                    AND constraint_name = 'PRIMARY'
                    AND table_schema = DATABASE()
                    ORDER BY ordinal_position";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            primaryKeys.Add(reader.GetString(0));
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
                    CONSTRAINT_NAME as FK_NAME,
                    COLUMN_NAME,
                    REFERENCED_TABLE_NAME,
                    REFERENCED_COLUMN_NAME
                FROM information_schema.KEY_COLUMN_USAGE
                WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = @tableName
                    AND REFERENCED_TABLE_NAME IS NOT NULL";
            
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableName", tableName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var foreignKey = new ForeignKeyInfo
                            {
                                Name = reader["FK_NAME"].ToString(),
                                LocalColumn = reader["COLUMN_NAME"].ToString(),
                                ReferencedTable = reader["REFERENCED_TABLE_NAME"].ToString(),
                                ReferencedColumn = reader["REFERENCED_COLUMN_NAME"].ToString(),
                                DeleteAction = "NO ACTION", // MySQL لا يوفر هذه المعلومات بسهولة
                                UpdateAction = "NO ACTION"
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
                "int" or "integer" or "mediumint" => "int",
                "bigint" => "long",
                "smallint" or "tinyint" => "short",
                "bit" => "bool",
                "decimal" or "numeric" => "decimal",
                "float" => "float",
                "double" => "double",
                "datetime" or "timestamp" => "DateTime",
                "date" => "DateTime",
                "time" => "TimeSpan",
                "char" or "varchar" or "tinytext" or "text" or "mediumtext" or "longtext" => "string",
                "binary" or "varbinary" or "tinyblob" or "blob" or "mediumblob" or "longblob" => "byte[]",
                "enum" or "set" => "string",
                "json" => "string",
                _ => "object"
            };
            
            if (isNullable && csharpType != "string" && csharpType != "byte[]" && csharpType != "object")
            {
                csharpType += "?";
            }
            
            return csharpType;
        }

        public string BuildConnectionString(string server, string database, string username, string password, Dictionary<string, string> additionalParams = null)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = username,
                Password = password
            };

            if (additionalParams != null)
            {
                foreach (var param in additionalParams)
                {
                    builder[param.Key] = param.Value;
                }
            }

            return builder.ToString();
        }

        public bool TestConnection(string connectionString)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
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

        private List<IndexInfo> GetTableIndexes(MySqlConnection connection, string tableName)
        {
            var indexes = new List<IndexInfo>();
            var query = @"
                SELECT 
                    INDEX_NAME,
                    INDEX_TYPE,
                    GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) as COLUMN_NAMES,
                    NON_UNIQUE = 0 as IS_UNIQUE,
                    INDEX_NAME = 'PRIMARY' as IS_PRIMARY,
                    INDEX_COMMENT
                FROM information_schema.STATISTICS
                WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = @tableName
                GROUP BY INDEX_NAME, INDEX_TYPE, NON_UNIQUE, INDEX_COMMENT";

            using (var command = new MySqlCommand(query, connection))
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

        private List<TriggerInfo> GetTableTriggers(MySqlConnection connection, string tableName)
        {
            var triggers = new List<TriggerInfo>();
            var query = @"
                SELECT 
                    TRIGGER_NAME,
                    ACTION_TIMING as TIMING,
                    EVENT_MANIPULATION as EVENT,
                    ACTION_STATEMENT as DEFINITION,
                    '' as TRIGGER_COMMENT
                FROM information_schema.TRIGGERS
                WHERE EVENT_OBJECT_SCHEMA = DATABASE()
                    AND EVENT_OBJECT_TABLE = @tableName";

            using (var command = new MySqlCommand(query, connection))
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