using System;
using GeneratorCode.Core.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;

namespace GeneratorCode.Core.Helpers
{
    /// <summary>
    /// فئة موحدة لبناء Connection Strings لأنواع قواعد البيانات المختلفة
    /// </summary>
    public static class ConnectionStringBuilder
    {
        /// <summary>
        /// بناء Connection String بناءً على نوع قاعدة البيانات والمعاملات
        /// </summary>
        /// <param name="dbType">نوع قاعدة البيانات</param>
        /// <param name="server">اسم الخادم</param>
        /// <param name="database">اسم قاعدة البيانات</param>
        /// <param name="username">اسم المستخدم (اختياري)</param>
        /// <param name="password">كلمة المرور (اختياري)</param>
        /// <param name="port">المنفذ (اختياري)</param>
        /// <param name="useIntegratedSecurity">استخدام المصادقة المتكاملة (لـ SQL Server فقط)</param>
        /// <param name="trustServerCertificate">الثقة في شهادة الخادم (لـ SQL Server فقط)</param>
        /// <returns>Connection String</returns>
        public static string Build(
            DatabaseType dbType,
            string server,
            string database = "",
            string username = "",
            string password = "",
            int? port = null,
            bool useIntegratedSecurity = false,
            bool trustServerCertificate = true)
        {
            if (string.IsNullOrWhiteSpace(server))
                throw new ArgumentException("Server name cannot be null or empty", nameof(server));

            return dbType switch
            {
                DatabaseType.SqlServer => BuildSqlServerConnectionString(
                    server, database, username, password, useIntegratedSecurity, trustServerCertificate),
                DatabaseType.MySql => BuildMySqlConnectionString(
                    server, database, username, password, port),
                DatabaseType.PostgreSql => BuildPostgreSqlConnectionString(
                    server, database, username, password, port),
                _ => throw new ArgumentException($"نوع قاعدة البيانات غير مدعوم: {dbType}")
            };
        }

        /// <summary>
        /// بناء Connection String لـ SQL Server
        /// </summary>
        private static string BuildSqlServerConnectionString(
            string server,
            string database,
            string username,
            string password,
            bool useIntegratedSecurity,
            bool trustServerCertificate)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                TrustServerCertificate = trustServerCertificate
            };

            if (!string.IsNullOrWhiteSpace(database))
            {
                builder.InitialCatalog = database;
            }

            if (useIntegratedSecurity || string.IsNullOrWhiteSpace(username))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = username;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    builder.Password = password;
                }
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// بناء Connection String لـ MySQL
        /// </summary>
        private static string BuildMySqlConnectionString(
            string server,
            string database,
            string username,
            string password,
            int? port)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = server
            };

            if (port.HasValue)
            {
                builder.Port = (uint)port.Value;
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                builder.UserID = username;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                builder.Password = password;
            }

            if (!string.IsNullOrWhiteSpace(database))
            {
                builder.Database = database;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// بناء Connection String لـ PostgreSQL
        /// </summary>
        private static string BuildPostgreSqlConnectionString(
            string server,
            string database,
            string username,
            string password,
            int? port)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = server
            };

            if (port.HasValue)
            {
                builder.Port = port.Value;
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                builder.Username = username;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                builder.Password = password;
            }

            if (!string.IsNullOrWhiteSpace(database))
            {
                builder.Database = database;
            }

            return builder.ConnectionString;
        }

        /// <summary>
        /// بناء Connection String من string (للتوافق مع الكود القديم)
        /// </summary>
        public static string Build(
            string dbType,
            string server,
            string database = "",
            string username = "",
            string password = "",
            int? port = null,
            bool useIntegratedSecurity = false,
            bool trustServerCertificate = true)
        {
            var databaseType = ParseDatabaseType(dbType);
            return Build(databaseType, server, database, username, password, port, useIntegratedSecurity, trustServerCertificate);
        }

        /// <summary>
        /// تحويل string إلى DatabaseType
        /// </summary>
        private static DatabaseType ParseDatabaseType(string dbType)
        {
            if (string.IsNullOrWhiteSpace(dbType))
                throw new ArgumentException("Database type cannot be null or empty", nameof(dbType));

            return dbType.Trim().ToLower() switch
            {
                "sqlserver" or "sql server" or "mssql" => DatabaseType.SqlServer,
                "mysql" => DatabaseType.MySql,
                "postgresql" or "postgres" or "pgsql" => DatabaseType.PostgreSql,
                _ => throw new ArgumentException($"نوع قاعدة البيانات غير مدعوم: {dbType}")
            };
        }
    }
}

