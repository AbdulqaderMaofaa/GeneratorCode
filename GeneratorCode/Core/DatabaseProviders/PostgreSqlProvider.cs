using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using System.Linq;

namespace GeneratorCode.Core.DatabaseProviders
{
    /// <summary>
    /// موفر PostgreSQL
    /// </summary>
    public class PostgreSqlProvider : IDatabaseProvider
    {
        public DatabaseType DatabaseType => DatabaseType.PostgreSql;
        public string Name => "PostgreSQL";
        
        public List<TableInfo> GetTables(string connectionString)
        {
            var tables = new List<TableInfo>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT table_name, table_schema 
                    FROM information_schema.tables 
                    WHERE table_schema NOT IN ('pg_catalog', 'information_schema') 
                    AND table_type = 'BASE TABLE'
                    ORDER BY table_schema, table_name";

                using (var command = new NpgsqlCommand(sql, connection))
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
            using (var connection = new NpgsqlConnection(connectionString))
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
                    ORDER BY ordinal_position";

                using (var command = new NpgsqlCommand(sql, connection))
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
                                MaxLength = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2),
                                Precision = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3),
                                Scale = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4),
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
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT kcu.column_name
                    FROM information_schema.table_constraints tc
                    JOIN information_schema.key_column_usage kcu
                        ON tc.constraint_name = kcu.constraint_name
                        AND tc.table_schema = kcu.table_schema
                    WHERE tc.constraint_type = 'PRIMARY KEY'
                        AND tc.table_name = @tableName
                    ORDER BY kcu.ordinal_position";

                using (var command = new NpgsqlCommand(sql, connection))
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
                    con.conname as FK_NAME,
                    att.attname as COLUMN_NAME,
                    cl2.relname as REFERENCED_TABLE,
                    att2.attname as REFERENCED_COLUMN,
                    CASE con.confdeltype
                        WHEN 'a' THEN 'NO ACTION'
                        WHEN 'r' THEN 'RESTRICT'
                        WHEN 'c' THEN 'CASCADE'
                        WHEN 'n' THEN 'SET NULL'
                        WHEN 'd' THEN 'SET DEFAULT'
                    END as DELETE_ACTION,
                    CASE con.confupdtype
                        WHEN 'a' THEN 'NO ACTION'
                        WHEN 'r' THEN 'RESTRICT'
                        WHEN 'c' THEN 'CASCADE'
                        WHEN 'n' THEN 'SET NULL'
                        WHEN 'd' THEN 'SET DEFAULT'
                    END as UPDATE_ACTION
                FROM pg_constraint con
                JOIN pg_class cl ON cl.oid = con.conrelid
                JOIN pg_class cl2 ON cl2.oid = con.confrelid
                JOIN pg_attribute att ON att.attrelid = con.conrelid AND att.attnum = con.conkey[1]
                JOIN pg_attribute att2 ON att2.attrelid = con.confrelid AND att2.attnum = con.confkey[1]
                WHERE con.contype = 'f' AND cl.relname = @tableName";
            
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand(query, connection))
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
                "integer" or "int4" => "int",
                "bigint" or "int8" => "long",
                "smallint" or "int2" => "short",
                "numeric" or "decimal" => "decimal",
                "real" or "float4" => "float",
                "double precision" or "float8" => "double",
                "boolean" or "bool" => "bool",
                "character varying" or "varchar" or "text" => "string",
                "character" or "char" => "string",
                "timestamp" or "timestamp without time zone" => "DateTime",
                "timestamptz" or "timestamp with time zone" => "DateTimeOffset",
                "date" => "DateTime",
                "time" or "time without time zone" => "TimeSpan",
                "timetz" or "time with time zone" => "DateTimeOffset",
                "interval" => "TimeSpan",
                "uuid" => "Guid",
                "bytea" => "byte[]",
                "json" or "jsonb" => "string",
                "xml" => "string",
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
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = server,
                Database = database,
                Username = username,
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
                using (var connection = new NpgsqlConnection(connectionString))
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

        private List<IndexInfo> GetTableIndexes(NpgsqlConnection connection, string tableName)
        {
            var indexes = new List<IndexInfo>();
            var query = @"
                SELECT
                    i.relname as INDEX_NAME,
                    am.amname as INDEX_TYPE,
                    array_to_string(array_agg(a.attname ORDER BY k.n), ', ') as COLUMN_NAMES,
                    ix.indisunique as IS_UNIQUE,
                    ix.indisprimary as IS_PRIMARY,
                    obj_description(i.oid, 'pg_class') as INDEX_COMMENT
                FROM pg_index ix
                JOIN pg_class i ON i.oid = ix.indexrelid
                JOIN pg_class t ON t.oid = ix.indrelid
                JOIN pg_am am ON i.relam = am.oid
                JOIN pg_namespace n ON n.oid = i.relnamespace
                JOIN pg_attribute a ON a.attrelid = t.oid
                JOIN generate_series(1, array_length(ix.indkey, 1)) k(n) ON a.attnum = ix.indkey[k.n-1]
                WHERE t.relname = @tableName
                GROUP BY i.relname, am.amname, ix.indisunique, ix.indisprimary, i.oid";

            using (var command = new NpgsqlCommand(query, connection))
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
                            Description = reader["INDEX_COMMENT"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return indexes;
        }

        private List<TriggerInfo> GetTableTriggers(NpgsqlConnection connection, string tableName)
        {
            var triggers = new List<TriggerInfo>();
            var query = @"
                SELECT 
                    t.tgname as TRIGGER_NAME,
                    CASE 
                        WHEN t.tgtype & 2 > 0 THEN 'BEFORE'
                        WHEN t.tgtype & 16 > 0 THEN 'AFTER'
                        ELSE 'INSTEAD OF'
                    END as TIMING,
                    CASE
                        WHEN t.tgtype & 4 > 0 THEN 'INSERT'
                        WHEN t.tgtype & 8 > 0 THEN 'DELETE'
                        WHEN t.tgtype & 16 > 0 THEN 'UPDATE'
                        ELSE 'TRUNCATE'
                    END as EVENT,
                    p.proname as FUNCTION_NAME,
                    obj_description(t.oid, 'pg_trigger') as TRIGGER_COMMENT
                FROM pg_trigger t
                JOIN pg_class c ON c.oid = t.tgrelid
                JOIN pg_proc p ON p.oid = t.tgfoid
                WHERE NOT t.tgisinternal AND c.relname = @tableName";

            using (var command = new NpgsqlCommand(query, connection))
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
                            Definition = reader["FUNCTION_NAME"].ToString(),
                            Description = reader["TRIGGER_COMMENT"]?.ToString() ?? ""
                        });
                    }
                }
            }
            return triggers;
        }
    }
} 