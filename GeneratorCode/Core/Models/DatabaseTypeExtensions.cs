using System;

namespace GeneratorCode.Core.Models
{
    public static class DatabaseTypeExtensions
    {
        public static DatabaseType ParseDatabaseType(string databaseType)
        {
            return databaseType?.ToLower() switch
            {
                "sqlserver" or "sql server" => DatabaseType.SqlServer,
                "mysql" => DatabaseType.MySql,
                "postgresql" or "postgres" => DatabaseType.PostgreSql,
                "oracle" => DatabaseType.Oracle,
                "sqlite" => DatabaseType.SQLite,
                _ => throw new ArgumentException($"نوع قاعدة البيانات غير مدعوم: {databaseType}")
            };
        }
    }
} 