using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GeneratorCode.Core.DomainModel;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Services
{
    public class DomainModelService : IDomainModelService
    {
        private readonly DomainModelValidator _validator = new();
        private readonly Dictionary<string, (DomainModel.DomainModel Model, DateTime LoadedAt)> _modelCache = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public DomainModel.DomainModel CurrentModel { get; private set; } = new();

        public void CreateNewModel(string projectName, string defaultNamespace, DatabaseType databaseType, string targetFramework = "net8.0")
        {
            CurrentModel = new DomainModel.DomainModel
            {
                ProjectName = projectName,
                DefaultNamespace = defaultNamespace,
                TargetDatabaseType = databaseType,
                TargetFramework = targetFramework
            };
        }

        public void AddEntity(DomainEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (string.IsNullOrWhiteSpace(entity.Name)) throw new ArgumentException("اسم الكيان مطلوب");

            if (CurrentModel.Entities.Any(e => string.Equals(e.Name, entity.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"الكيان '{entity.Name}' موجود بالفعل");

            if (string.IsNullOrWhiteSpace(entity.TableName))
                entity.TableName = entity.Name;

            CurrentModel.Entities.Add(entity);
        }

        public void RemoveEntity(string entityName)
        {
            var entity = FindEntity(entityName);
            CurrentModel.Entities.Remove(entity);

            foreach (var other in CurrentModel.Entities)
            {
                other.Relations?.RemoveAll(r =>
                    string.Equals(r.TargetEntity, entityName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r.SourceEntity, entityName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public void UpdateEntity(string oldName, DomainEntity entity)
        {
            var existing = FindEntity(oldName);
            var index = CurrentModel.Entities.IndexOf(existing);

            if (!string.Equals(oldName, entity.Name, StringComparison.OrdinalIgnoreCase) &&
                CurrentModel.Entities.Any(e => string.Equals(e.Name, entity.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"الكيان '{entity.Name}' موجود بالفعل");
            }

            if (!string.Equals(oldName, entity.Name, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var other in CurrentModel.Entities)
                {
                    if (other.Relations == null) continue;
                    foreach (var rel in other.Relations)
                    {
                        if (string.Equals(rel.TargetEntity, oldName, StringComparison.OrdinalIgnoreCase))
                            rel.TargetEntity = entity.Name;
                        if (string.Equals(rel.SourceEntity, oldName, StringComparison.OrdinalIgnoreCase))
                            rel.SourceEntity = entity.Name;
                    }
                }
            }

            CurrentModel.Entities[index] = entity;
        }

        public void AddProperty(string entityName, DomainProperty property)
        {
            var entity = FindEntity(entityName);
            if (entity.Properties.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"الخاصية '{property.Name}' موجودة بالفعل في الكيان '{entityName}'");

            property.Order = entity.Properties.Count;
            if (string.IsNullOrWhiteSpace(property.ColumnName))
                property.ColumnName = property.Name;

            entity.Properties.Add(property);
        }

        public void RemoveProperty(string entityName, string propertyName)
        {
            var entity = FindEntity(entityName);
            var prop = entity.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException($"الخاصية '{propertyName}' غير موجودة في الكيان '{entityName}'");
            entity.Properties.Remove(prop);
        }

        public void UpdateProperty(string entityName, string oldPropertyName, DomainProperty property)
        {
            var entity = FindEntity(entityName);
            var existing = entity.Properties.FirstOrDefault(p =>
                string.Equals(p.Name, oldPropertyName, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException($"الخاصية '{oldPropertyName}' غير موجودة في الكيان '{entityName}'");
            if (!string.Equals(oldPropertyName, property.Name, StringComparison.OrdinalIgnoreCase) &&
                entity.Properties.Any(p => string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"الخاصية '{property.Name}' موجودة بالفعل في الكيان '{entityName}'");
            }

            var index = entity.Properties.IndexOf(existing);
            entity.Properties[index] = property;
        }

        public void AddRelation(string entityName, DomainRelation relation)
        {
            var entity = FindEntity(entityName);
            relation.SourceEntity = entityName;

            if (string.IsNullOrWhiteSpace(relation.NavigationPropertyName))
                relation.NavigationPropertyName = relation.RelationType == RelationType.OneToMany
                    ? relation.TargetEntity + "s"
                    : relation.TargetEntity;

            entity.Relations ??= new List<DomainRelation>();
            entity.Relations.Add(relation);
        }

        public void RemoveRelation(string entityName, string targetEntity)
        {
            var entity = FindEntity(entityName);
            entity.Relations?.RemoveAll(r =>
                string.Equals(r.TargetEntity, targetEntity, StringComparison.OrdinalIgnoreCase));
        }

        public ValidationResult ValidateModel()
        {
            return _validator.Validate(CurrentModel);
        }

        public async Task SaveModelAsync(string path)
        {
            var json = JsonSerializer.Serialize(CurrentModel, JsonOptions);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(path, json);
        }

        public async Task<DomainModel.DomainModel> LoadModelAsync(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"ملف النموذج غير موجود: {path}");

            var fullPath = Path.GetFullPath(path);
            var lastWrite = File.GetLastWriteTimeUtc(fullPath);

            if (_modelCache.TryGetValue(fullPath, out var cached) && cached.LoadedAt >= lastWrite)
            {
                CurrentModel = cached.Model;
                return CurrentModel;
            }

            var json = await File.ReadAllTextAsync(path);
            CurrentModel = JsonSerializer.Deserialize<DomainModel.DomainModel>(json, JsonOptions)
                           ?? new DomainModel.DomainModel();

            _modelCache[fullPath] = (CurrentModel, DateTime.UtcNow);
            return CurrentModel;
        }

        public string ExportToJson()
        {
            return JsonSerializer.Serialize(CurrentModel, JsonOptions);
        }

        public void ImportFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("محتوى JSON مطلوب");

            CurrentModel = JsonSerializer.Deserialize<DomainModel.DomainModel>(json, JsonOptions)
                           ?? new DomainModel.DomainModel();
        }

        public DomainEntity ConvertFromTableInfo(TableInfo tableInfo, DatabaseType databaseType)
        {
            if (tableInfo == null) throw new ArgumentNullException(nameof(tableInfo));

            var entity = new DomainEntity
            {
                Name = ToPascalCase(tableInfo.Name),
                TableName = tableInfo.Name,
                Schema = tableInfo.Schema ?? "dbo",
                Description = tableInfo.Description
            };

            if (tableInfo.Columns != null)
            {
                int order = 0;
                foreach (var col in tableInfo.Columns)
                {
                    var prop = new DomainProperty
                    {
                        Name = ToPascalCase(col.Name),
                        ColumnName = col.Name,
                        Type = MapCSharpTypeToDomainType(col.CSharpType ?? col.DataType),
                        IsRequired = !col.IsNullable,
                        IsPrimaryKey = col.IsPrimaryKey,
                        IsIdentity = col.IsAutoIncrement,
                        MaxLength = col.MaxLength > 0 ? col.MaxLength : null,
                        Precision = col.Precision > 0 ? col.Precision : null,
                        Scale = col.Scale > 0 ? col.Scale : null,
                        DefaultValue = col.DefaultValue,
                        Description = col.Description,
                        Order = order++
                    };
                    entity.Properties.Add(prop);
                }
            }

            return entity;
        }

        private DomainEntity FindEntity(string entityName)
        {
            var entity = CurrentModel.Entities.FirstOrDefault(e =>
                string.Equals(e.Name, entityName, StringComparison.OrdinalIgnoreCase));

            return entity ?? throw new InvalidOperationException($"الكيان '{entityName}' غير موجود");
        }

        private static DomainPropertyType MapCSharpTypeToDomainType(string csharpType)
        {
            var normalized = (csharpType ?? "string").TrimEnd('?').ToLowerInvariant();
            return normalized switch
            {
                "int" or "int32" or "integer" => DomainPropertyType.Int,
                "long" or "int64" or "bigint" => DomainPropertyType.Long,
                "short" or "int16" or "smallint" => DomainPropertyType.Short,
                "byte" or "tinyint" => DomainPropertyType.Byte,
                "bool" or "boolean" or "bit" => DomainPropertyType.Bool,
                "decimal" or "numeric" or "money" => DomainPropertyType.Decimal,
                "double" or "float8" => DomainPropertyType.Double,
                "float" or "single" or "real" or "float4" => DomainPropertyType.Float,
                "datetime" or "datetime2" or "timestamp" or "smalldatetime" or "date" => DomainPropertyType.DateTime,
                "dateonly" => DomainPropertyType.DateOnly,
                "timeonly" or "time" => DomainPropertyType.TimeOnly,
                "guid" or "uniqueidentifier" or "uuid" => DomainPropertyType.Guid,
                "byte[]" or "binary" or "varbinary" or "bytea" or "blob" => DomainPropertyType.ByteArray,
                _ => DomainPropertyType.String
            };
        }

        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;

            var parts = name.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Select(p =>
                char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p[1..] : "")));
        }
    }
}
