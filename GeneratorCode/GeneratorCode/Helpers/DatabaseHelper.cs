using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Npgsql;
using GeneratorCode.Core.Models;

namespace GeneratorCode.GeneratorCode.Helpers
{

    public class DbType
    {
        public string Display { get; set; }
        public string Value { get; set; }

        public DbType(string display, string value)
        {
            Display = display;
            Value = value;
        }
    }

    public class DatabaseHelper
    {
        public static async Task<bool> TestConnection(string dbType, string server, string username, string password)
        {
            var connectionString = BuildConnectionString(dbType, server, username, password);
            
            switch (dbType)
            {
                case "SQL Server":
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                    }
                    break;

                case "MySQL":
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                    }
                    break;

                case "PostgreSQL":
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                    }
                    break;

                default:
                    throw new ArgumentException("نوع قاعدة البيانات غير مدعوم");
            }

            return true;
        }

        public static async Task<List<string>> GetDatabases(string dbType, string server, string username, string password)
        {
            var databases = new List<string>();
            var connectionString = BuildConnectionString(dbType, server, username, password);

            switch (dbType)
            {
                case "SQL Server":
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using var command = new SqlCommand(
                            @"SELECT name 
                            FROM sys.databases 
                            WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')
                            ORDER BY name", connection);
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                    break;

                case "MySQL":
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using var command = new MySqlCommand(
                            @"SHOW DATABASES 
                            WHERE `Database` NOT IN ('information_schema', 'mysql', 'performance_schema', 'sys')",
                            connection);
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                    break;

                case "PostgreSQL":
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        using var command = new NpgsqlCommand(
                            @"SELECT datname 
                            FROM pg_database 
                            WHERE datistemplate = false 
                            AND datname NOT IN ('postgres', 'template0', 'template1')
                            ORDER BY datname", connection);
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            databases.Add(reader.GetString(0));
                        }
                    }
                    break;

                default:
                    throw new ArgumentException("نوع قاعدة البيانات غير مدعوم");
            }

            return databases;
        }

        public static string BuildConnectionString(string dbType, string server, string username, string password, string database = "")
        {
            // استخدام الفئة الموحدة لبناء Connection String
            var databaseType = DatabaseTypeExtensions.ParseDatabaseType(dbType);
            return Core.Helpers.ConnectionStringBuilder.Build(
                databaseType,
                server,
                database,
                username,
                password
            );
        }
    }
} 