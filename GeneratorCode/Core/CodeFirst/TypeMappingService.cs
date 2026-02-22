using GeneratorCode.Core.DomainModel;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.CodeFirst
{
    public class TypeMappingService
    {
        public static string MapToDatabaseType(DomainPropertyType type, DatabaseType dbType, int? maxLength = null, int? precision = null, int? scale = null)
        {
            return dbType switch
            {
                DatabaseType.SqlServer => MapToSqlServer(type, maxLength, precision, scale),
                DatabaseType.MySql => MapToMySql(type, maxLength, precision, scale),
                DatabaseType.PostgreSql => MapToPostgreSql(type, maxLength, precision, scale),
                DatabaseType.Oracle => MapToOracle(type, maxLength, precision, scale),
                DatabaseType.SQLite => MapToSQLite(type),
                _ => MapToSqlServer(type, maxLength, precision, scale)
            };
        }

        public static DomainPropertyType MapFromDatabaseType(string dbColumnType, DatabaseType dbType)
        {
            var normalized = (dbColumnType ?? "").Trim().ToLowerInvariant();

            if (normalized.Contains("int") && !normalized.Contains("interval"))
            {
                if (normalized.Contains("big")) return DomainPropertyType.Long;
                if (normalized.Contains("small")) return DomainPropertyType.Short;
                if (normalized.Contains("tiny")) return DomainPropertyType.Byte;
                return DomainPropertyType.Int;
            }

            if (normalized.Contains("bool") || normalized == "bit" || normalized == "tinyint(1)")
                return DomainPropertyType.Bool;

            if (normalized.Contains("decimal") || normalized.Contains("numeric") || normalized.Contains("money"))
                return DomainPropertyType.Decimal;

            if (normalized.Contains("double") || normalized == "float8" || normalized.Contains("binary_double"))
                return DomainPropertyType.Double;

            if (normalized == "real" || normalized == "float4" || normalized == "float" || normalized.Contains("binary_float"))
                return DomainPropertyType.Float;

            if (normalized.Contains("datetime") || normalized.Contains("timestamp") || normalized == "date")
                return DomainPropertyType.DateTime;

            if (normalized.Contains("uniqueidentifier") || normalized == "uuid" || normalized == "raw(16)")
                return DomainPropertyType.Guid;

            if (normalized.Contains("binary") || normalized.Contains("blob") || normalized == "bytea" || normalized.Contains("image"))
                return DomainPropertyType.ByteArray;

            if (normalized.Contains("time") && !normalized.Contains("stamp"))
                return DomainPropertyType.TimeOnly;

            return DomainPropertyType.String;
        }

        public static string MapToCSharpType(DomainPropertyType type, bool isNullable)
        {
            var baseType = type switch
            {
                DomainPropertyType.Int => "int",
                DomainPropertyType.Long => "long",
                DomainPropertyType.Short => "short",
                DomainPropertyType.Byte => "byte",
                DomainPropertyType.Bool => "bool",
                DomainPropertyType.String => "string",
                DomainPropertyType.Decimal => "decimal",
                DomainPropertyType.Double => "double",
                DomainPropertyType.Float => "float",
                DomainPropertyType.DateTime => "DateTime",
                DomainPropertyType.DateOnly => "DateOnly",
                DomainPropertyType.TimeOnly => "TimeOnly",
                DomainPropertyType.Guid => "Guid",
                DomainPropertyType.ByteArray => "byte[]",
                _ => "string"
            };

            if (isNullable && type != DomainPropertyType.String && type != DomainPropertyType.ByteArray)
                return baseType + "?";

            return baseType;
        }

        public static string GetColumnTypeAnnotation(DomainPropertyType type, DatabaseType dbType, int? maxLength = null, int? precision = null, int? scale = null)
        {
            var dbTypeName = MapToDatabaseType(type, dbType, maxLength, precision, scale);
            return $"[Column(TypeName = \"{dbTypeName}\")]";
        }

        private static string MapToSqlServer(DomainPropertyType type, int? maxLength, int? precision, int? scale)
        {
            return type switch
            {
                DomainPropertyType.Int => "int",
                DomainPropertyType.Long => "bigint",
                DomainPropertyType.Short => "smallint",
                DomainPropertyType.Byte => "tinyint",
                DomainPropertyType.Bool => "bit",
                DomainPropertyType.String => maxLength.HasValue ? $"nvarchar({maxLength})" : "nvarchar(max)",
                DomainPropertyType.Decimal => $"decimal({precision ?? 18},{scale ?? 2})",
                DomainPropertyType.Double => "float",
                DomainPropertyType.Float => "real",
                DomainPropertyType.DateTime => "datetime2",
                DomainPropertyType.DateOnly => "date",
                DomainPropertyType.TimeOnly => "time",
                DomainPropertyType.Guid => "uniqueidentifier",
                DomainPropertyType.ByteArray => "varbinary(max)",
                _ => "nvarchar(max)"
            };
        }

        private static string MapToMySql(DomainPropertyType type, int? maxLength, int? precision, int? scale)
        {
            return type switch
            {
                DomainPropertyType.Int => "int",
                DomainPropertyType.Long => "bigint",
                DomainPropertyType.Short => "smallint",
                DomainPropertyType.Byte => "tinyint unsigned",
                DomainPropertyType.Bool => "tinyint(1)",
                DomainPropertyType.String => maxLength.HasValue ? $"varchar({maxLength})" : "longtext",
                DomainPropertyType.Decimal => $"decimal({precision ?? 18},{scale ?? 2})",
                DomainPropertyType.Double => "double",
                DomainPropertyType.Float => "float",
                DomainPropertyType.DateTime => "datetime",
                DomainPropertyType.DateOnly => "date",
                DomainPropertyType.TimeOnly => "time",
                DomainPropertyType.Guid => "char(36)",
                DomainPropertyType.ByteArray => "longblob",
                _ => "longtext"
            };
        }

        private static string MapToPostgreSql(DomainPropertyType type, int? maxLength, int? precision, int? scale)
        {
            return type switch
            {
                DomainPropertyType.Int => "integer",
                DomainPropertyType.Long => "bigint",
                DomainPropertyType.Short => "smallint",
                DomainPropertyType.Byte => "smallint",
                DomainPropertyType.Bool => "boolean",
                DomainPropertyType.String => maxLength.HasValue ? $"varchar({maxLength})" : "text",
                DomainPropertyType.Decimal => $"numeric({precision ?? 18},{scale ?? 2})",
                DomainPropertyType.Double => "double precision",
                DomainPropertyType.Float => "real",
                DomainPropertyType.DateTime => "timestamp",
                DomainPropertyType.DateOnly => "date",
                DomainPropertyType.TimeOnly => "time",
                DomainPropertyType.Guid => "uuid",
                DomainPropertyType.ByteArray => "bytea",
                _ => "text"
            };
        }

        private static string MapToOracle(DomainPropertyType type, int? maxLength, int? precision, int? scale)
        {
            return type switch
            {
                DomainPropertyType.Int => "NUMBER(10)",
                DomainPropertyType.Long => "NUMBER(19)",
                DomainPropertyType.Short => "NUMBER(5)",
                DomainPropertyType.Byte => "NUMBER(3)",
                DomainPropertyType.Bool => "NUMBER(1)",
                DomainPropertyType.String => maxLength.HasValue ? $"NVARCHAR2({maxLength})" : "NCLOB",
                DomainPropertyType.Decimal => $"NUMBER({precision ?? 18},{scale ?? 2})",
                DomainPropertyType.Double => "BINARY_DOUBLE",
                DomainPropertyType.Float => "BINARY_FLOAT",
                DomainPropertyType.DateTime => "DATE",
                DomainPropertyType.DateOnly => "DATE",
                DomainPropertyType.TimeOnly => "INTERVAL DAY TO SECOND",
                DomainPropertyType.Guid => "RAW(16)",
                DomainPropertyType.ByteArray => "BLOB",
                _ => "NCLOB"
            };
        }

        private static string MapToSQLite(DomainPropertyType type)
        {
            return type switch
            {
                DomainPropertyType.Int or DomainPropertyType.Long or DomainPropertyType.Short
                    or DomainPropertyType.Byte or DomainPropertyType.Bool => "INTEGER",
                DomainPropertyType.Decimal or DomainPropertyType.Double or DomainPropertyType.Float => "REAL",
                DomainPropertyType.ByteArray => "BLOB",
                _ => "TEXT"
            };
        }
    }
}
