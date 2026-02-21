using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class DomainDrivenDesignPattern : BaseArchitecturePattern
    {
        public override string Name => "Domain-Driven Design";
        public override string Description => "نمط التصميم الموجه بالنطاق";

        public override async Task<CodeGenerationResult> Generate(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var options = context.Options ?? new GenerationOptions();

            try
            {
                if (context.TableInfo == null)
                {
                    result.Success = false;
                    result.Message = "TableInfo is required for generation.";
                    result.Errors.Add("TableInfo is null.");
                    return result;
                }

                var ns = context.Namespace ?? "GeneratedCode";
                var entityName = context.EntityName ?? context.TableInfo.Name ?? "Entity";
                var outputPath = context.OutputPath ?? ".";

                if (options.GenerateDomainLayer || options.GenerateEntities)
                {
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Entities", "Entity.cs", GenerateBaseEntity(ns), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Entities", $"{entityName}.cs", GenerateDomainEntity(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/ValueObjects", $"{entityName}Id.cs", GenerateEntityIdValueObject(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Events", "IDomainEvent.cs", GenerateIDomainEvent(ns), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Events", $"{entityName}CreatedEvent.cs", GenerateCreatedEvent(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Events", $"{entityName}UpdatedEvent.cs", GenerateUpdatedEvent(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Events", $"{entityName}DeletedEvent.cs", GenerateDeletedEvent(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Repositories", $"I{entityName}Repository.cs", GenerateDomainRepositoryInterface(context), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Specifications", "ISpecification.cs", GenerateISpecification(ns, entityName), "Domain"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Domain/Specifications", $"{entityName}Specifications.cs", GenerateEntitySpecifications(context), "Domain"));
                }

                if (options.GenerateApplicationLayer)
                {
                    if (options.GenerateDTOs)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/DTOs", $"{entityName}Dto.cs", GenerateEntityDto(context), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/DTOs", $"Create{entityName}Dto.cs", GenerateCreateDto(context), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/DTOs", $"Update{entityName}Dto.cs", GenerateUpdateDto(context), "Application"));
                    }
                    if (options.GenerateServices)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/Services", $"I{entityName}AppService.cs", GenerateAppServiceInterface(context), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/Services", $"{entityName}AppService.cs", GenerateAppService(context), "Application"));
                    }
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/Mappers", $"{entityName}Mapper.cs", GenerateMapper(context), "Application"));
                    if (options.GenerateValidators)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Application/Validators", $"{entityName}Validator.cs", GenerateValidator(context), "Application"));
                    }
                }

                if (options.GenerateInfrastructureLayer)
                {
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Infrastructure/Data", $"{ns}DbContext.cs", GenerateDbContext(context), "Infrastructure"));
                    if (options.GenerateRepositories)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Infrastructure/Repositories", $"{entityName}Repository.cs", GenerateRepositoryImpl(context), "Infrastructure"));
                    }
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Infrastructure/UnitOfWork", "IUnitOfWork.cs", GenerateIUnitOfWork(context), "Infrastructure"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Infrastructure/UnitOfWork", "UnitOfWork.cs", GenerateUnitOfWork(context), "Infrastructure"));
                }

                if (options.GeneratePresentationLayer && options.GenerateControllers)
                {
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "API/Controllers", $"{entityName}Controller.cs", GenerateController(context), "API"));
                }

                result.Success = true;
                result.Message = "Domain-Driven Design architecture generated successfully.";
                result.TotalSizeInBytes = result.GeneratedFiles.Sum(f => f.SizeInBytes);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Code generation failed: {ex.Message}";
                result.Errors.Add(ex.ToString());
            }

            stopwatch.Stop();
            result.GenerationTime = stopwatch.Elapsed;
            return result;
        }

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            if (context == null) context = new CodeGenerationContext();
            context.TableInfo = table;
            context.TableName = table?.Name ?? "Table";
            context.EntityName = context.EntityName ?? table?.Name ?? "Entity";
            context.Namespace = context.Namespace ?? "GeneratedCode";
            context.Options = context.Options ?? new GenerationOptions();

            var entityName = context.EntityName;
            var list = new List<PreviewFile>
            {
                new() { FileName = $"{entityName}.cs", Content = GenerateDomainEntity(context), Language = "csharp" },
                new() { FileName = $"{entityName}Id.cs", Content = GenerateEntityIdValueObject(context), Language = "csharp" },
                new() { FileName = $"{entityName}CreatedEvent.cs", Content = GenerateCreatedEvent(context), Language = "csharp" },
                new() { FileName = $"I{entityName}Repository.cs", Content = GenerateDomainRepositoryInterface(context), Language = "csharp" },
                new() { FileName = $"{entityName}Dto.cs", Content = GenerateEntityDto(context), Language = "csharp" },
                new() { FileName = $"I{entityName}AppService.cs", Content = GenerateAppServiceInterface(context), Language = "csharp" },
                new() { FileName = $"{entityName}AppService.cs", Content = GenerateAppService(context), Language = "csharp" },
                new() { FileName = $"{context.Namespace}DbContext.cs", Content = GenerateDbContext(context), Language = "csharp" },
                new() { FileName = $"{entityName}Repository.cs", Content = GenerateRepositoryImpl(context), Language = "csharp" },
                new() { FileName = $"{entityName}Controller.cs", Content = GenerateController(context), Language = "csharp" }
            };
            return list;
        }

        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new() { "Domain", "Application", "Infrastructure", "API" };
        public override List<string> GetRequiredDependencies() => new() { "MediatR", "FluentValidation", "Microsoft.EntityFrameworkCore" };

        private static GeneratedFile CreateGeneratedFile(string outputPath, string layerFolder, string fileName, string content, string layer)
        {
            var relativePath = Path.Combine(layerFolder, fileName);
            var fullPath = Path.Combine(outputPath, relativePath);
            return new GeneratedFile
            {
                FileName = fileName,
                RelativePath = relativePath,
                FullPath = fullPath,
                Content = content,
                FileType = "cs",
                Layer = layer,
                SizeInBytes = System.Text.Encoding.UTF8.GetByteCount(content ?? ""),
                CreatedDate = DateTime.Now
            };
        }

        private string GetPrimaryKeyName(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.Name ?? "Id";
        }

        private string GetPrimaryKeyCSharpType(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.CSharpType ?? "int";
        }

        private string GenerateBaseEntity(string ns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine("    public abstract class Entity<TId>");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly List<object> _domainEvents = new List<object>();");
            sb.AppendLine("        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();");
            sb.AppendLine();
            sb.AppendLine("        public TId Id { get; protected set; }");
            sb.AppendLine();
            sb.AppendLine("        public void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);");
            sb.AppendLine("        public void ClearDomainEvents() => _domainEvents.Clear();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateIDomainEvent(string ns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine("    public interface IDomainEvent");
            sb.AppendLine("    {");
            sb.AppendLine("        DateTime OccurredOn { get; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDomainEntity(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);
            var pkType = GetPrimaryKeyCSharpType(context);
            var tableName = context.TableName ?? "Table";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName} : Entity<{entityName}Id>");
            sb.AppendLine("    {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col.Name == pkName) continue;
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {col.Name} {{ get; private set; }}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"        private {entityName}() {{ }}");
            sb.AppendLine();

            var createParams = new List<string>();
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns)
                {
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    var paramName = char.ToLowerInvariant(col.Name[0]) + col.Name.Substring(1);
                    createParams.Add($"{csharpType}{nullable} {paramName}");
                }
            sb.AppendLine($"        public static {entityName} Create({string.Join(", ", createParams)})");
            sb.AppendLine("        {");
            var firstRequired = context.TableInfo?.Columns?.FirstOrDefault(c => c.Name != pkName && (c.CSharpType == "string" || c.CSharpType == "String") && !c.IsNullable);
            if (firstRequired != null)
            {
                var pname = char.ToLowerInvariant(firstRequired.Name[0]) + firstRequired.Name.Substring(1);
                sb.AppendLine($"            if (string.IsNullOrWhiteSpace({pname})) throw new ArgumentException(\"Required field {firstRequired.Name} cannot be empty.\", nameof({pname}));");
            }
            sb.AppendLine($"            var entity = new {entityName}();");
            var idParamName = context.TableInfo?.Columns?.FirstOrDefault(c => c.Name == pkName) != null ? char.ToLowerInvariant(pkName[0]) + pkName.Substring(1) : "id";
            sb.AppendLine($"            entity.Id = {entityName}Id.Create({idParamName});");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns.Where(c => c.Name != pkName))
                {
                    var paramName = char.ToLowerInvariant(col.Name[0]) + col.Name.Substring(1);
                    sb.AppendLine($"            entity.{col.Name} = {paramName};");
                }
            sb.AppendLine($"            entity.AddDomainEvent(new {ns}.Domain.Events.{entityName}CreatedEvent(entity.Id, DateTime.UtcNow));");
            sb.AppendLine("            return entity;");
            sb.AppendLine("        }");
            sb.AppendLine();

            var updateParams = (context.TableInfo?.Columns ?? new List<ColumnInfo>()).Where(c => !c.IsPrimaryKey).Select(c => $"{c.CSharpType ?? "object"}{(c.IsNullable && c.CSharpType != "string" ? "?" : "")} {char.ToLowerInvariant(c.Name[0]) + c.Name.Substring(1)}").ToList();
            sb.AppendLine($"        public void Update({string.Join(", ", updateParams)})");
            sb.AppendLine("        {");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns.Where(c => !c.IsPrimaryKey))
                {
                    var paramName = char.ToLowerInvariant(col.Name[0]) + col.Name.Substring(1);
                    sb.AppendLine($"            {col.Name} = {paramName};");
                }
            sb.AppendLine($"            AddDomainEvent(new {ns}.Domain.Events.{entityName}UpdatedEvent(Id, DateTime.UtcNow));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Delete()");
            sb.AppendLine("        {");
            sb.AppendLine($"            AddDomainEvent(new {ns}.Domain.Events.{entityName}DeletedEvent(Id, DateTime.UtcNow));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateEntityIdValueObject(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.ValueObjects");
            sb.AppendLine("{");
            sb.AppendLine($"    public sealed class {entityName}Id : IEquatable<{entityName}Id>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} Value {{ get; }}");
            sb.AppendLine();
            sb.AppendLine($"        private {entityName}Id({pkType} value) {{ Value = value; }}");
            sb.AppendLine();
            sb.AppendLine($"        public static {entityName}Id Create({pkType} value) => new {entityName}Id(value);");
            sb.AppendLine($"        public static {entityName}Id CreateNew() => new {entityName}Id(default);");
            sb.AppendLine();
            sb.AppendLine($"        public bool Equals({entityName}Id other) => other != null && System.Collections.Generic.EqualityComparer<{pkType}>.Default.Equals(Value, other.Value);");
            sb.AppendLine("        public override bool Equals(object obj) => obj is {entityName}Id other && Equals(other);");
            sb.AppendLine($"        public override int GetHashCode() => System.Collections.Generic.EqualityComparer<{pkType}>.Default.GetHashCode(Value);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateCreatedEvent(CodeGenerationContext context)
        {
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}CreatedEvent : IDomainEvent");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}Id EntityId {{ get; }}");
            sb.AppendLine("        public DateTime OccurredOn { get; }");
            sb.AppendLine($"        public {entityName}CreatedEvent({entityName}Id entityId, DateTime occurredOn) {{ EntityId = entityId; OccurredOn = occurredOn; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateUpdatedEvent(CodeGenerationContext context)
        {
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}UpdatedEvent : IDomainEvent");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}Id EntityId {{ get; }}");
            sb.AppendLine("        public DateTime OccurredOn { get; }");
            sb.AppendLine($"        public {entityName}UpdatedEvent({entityName}Id entityId, DateTime occurredOn) {{ EntityId = entityId; OccurredOn = occurredOn; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDeletedEvent(CodeGenerationContext context)
        {
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}DeletedEvent : IDomainEvent");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}Id EntityId {{ get; }}");
            sb.AppendLine("        public DateTime OccurredOn { get; }");
            sb.AppendLine($"        public {entityName}DeletedEvent({entityName}Id entityId, DateTime occurredOn) {{ EntityId = entityId; OccurredOn = occurredOn; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDomainRepositoryInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}> GetByIdAsync({entityName}Id id);");
            sb.AppendLine($"        Task<IReadOnlyList<{entityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}> AddAsync({entityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({entityName} entity);");
            sb.AppendLine($"        Task DeleteAsync({entityName}Id id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateISpecification(string ns, string entityName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Linq;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Specifications");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface ISpecification<T> where T : class");
            sb.AppendLine("    {");
            sb.AppendLine("        System.Linq.Expressions.Expression<System.Func<T, bool>> Criteria { get; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateEntitySpecifications(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq.Expressions;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Specifications");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {entityName}Specifications");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static Expression<Func<{entityName}, bool>> ById({entityName}Id id) => e => e.Id.Equals(id);");
            sb.AppendLine($"        public static Expression<Func<{entityName}, bool>> All => e => true;");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateEntityDto(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Dto");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns)
                {
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {col.Name} {{ get; set; }}");
                }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateCreateDto(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{entityName}Dto");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns.Where(c => !c.IsPrimaryKey || !c.IsAutoIncrement))
                {
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {col.Name} {{ get; set; }}");
                }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateUpdateDto(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{entityName}Dto");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns)
                {
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {col.Name} {{ get; set; }}");
                }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateAppServiceInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Services");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}AppService");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}Dto> GetByIdAsync({entityName}Id id);");
            sb.AppendLine($"        Task<IReadOnlyList<{entityName}Dto>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}Dto> CreateAsync(Create{entityName}Dto dto);");
            sb.AppendLine($"        Task UpdateAsync({entityName}Id id, Update{entityName}Dto dto);");
            sb.AppendLine($"        Task DeleteAsync({entityName}Id id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateAppService(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Application.Mappers;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Services");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}AppService : I{entityName}AppService");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine($"        private readonly {entityName}Mapper _mapper;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}AppService(I{entityName}Repository repository, {entityName}Mapper mapper)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("            _mapper = mapper;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}Dto> GetByIdAsync({entityName}Id id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(id);");
            sb.AppendLine($"            return entity == null ? null : _mapper.ToDto(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IReadOnlyList<{entityName}Dto>> GetAllAsync()");
            sb.AppendLine("        {");
            sb.AppendLine($"            var list = await _repository.GetAllAsync();");
            sb.AppendLine($"            return list.Select(_mapper.ToDto).ToList();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}Dto> CreateAsync(Create{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = _mapper.ToEntity(dto);");
            sb.AppendLine($"            entity = await _repository.AddAsync(entity);");
            sb.AppendLine($"            foreach (var evt in entity.DomainEvents) {{ }}");
            sb.AppendLine($"            return _mapper.ToDto(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({entityName}Id id, Update{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(id);");
            sb.AppendLine($"            if (entity == null) return;");
            sb.AppendLine($"            _mapper.Apply(entity, dto);");
            sb.AppendLine($"            await _repository.UpdateAsync(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task DeleteAsync({entityName}Id id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _repository.DeleteAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateMapper(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Mappers");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Mapper");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}Dto ToDto({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (entity == null) return null;");
            sb.AppendLine($"            return new {entityName}Dto");
            sb.AppendLine("            {");
            sb.AppendLine($"                {pkName} = entity.Id.Value,");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns.Where(c => c.Name != pkName))
                    sb.AppendLine($"                {col.Name} = entity.{col.Name},");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName} ToEntity(Create{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (dto == null) return null;");
            var createArgs = new List<string>();
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns)
                    createArgs.Add(col.Name == pkName ? "0" : $"dto.{col.Name}");
            sb.AppendLine($"            return {entityName}.Create({string.Join(", ", createArgs)});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public void Apply({entityName} entity, Update{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (entity == null || dto == null) return;");
            var updateArgs = context.TableInfo?.Columns?.Where(c => !c.IsPrimaryKey).Select(c => $"dto.{c.Name}").ToList() ?? new List<string>();
            sb.AppendLine($"            entity.Update({string.Join(", ", updateArgs)});");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateValidator(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using FluentValidation;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Validators");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Validator : AbstractValidator<Create{entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}Validator()");
            sb.AppendLine("        {");
            if (context.TableInfo?.Columns != null)
                foreach (var col in context.TableInfo.Columns.Where(c => !c.IsPrimaryKey || !c.IsAutoIncrement))
                {
                    if (col.CSharpType == "string" || col.CSharpType == "String")
                    {
                        if (!col.IsNullable)
                            sb.AppendLine($"            RuleFor(x => x.{col.Name}).NotEmpty();");
                        if (col.MaxLength.HasValue)
                            sb.AppendLine($"            RuleFor(x => x.{col.Name}).MaximumLength({col.MaxLength.Value});");
                    }
                }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDbContext(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var tableName = context.TableName ?? "Table";

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Infrastructure.Data");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {ns}DbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public DbSet<{entityName}> {entityName}s {{ get; set; }}");
            sb.AppendLine();
            sb.AppendLine($"        public {ns}DbContext(DbContextOptions<{ns}DbContext> options) : base(options) {{ }}");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine($"            modelBuilder.Entity<{entityName}>(e =>");
            sb.AppendLine("            {");
            sb.AppendLine($"                e.ToTable(\"{tableName}\");");
            sb.AppendLine($"                e.Property(x => x.Id).HasConversion(id => id.Value, value => {entityName}Id.Create(value));");
            sb.AppendLine($"                e.HasKey(x => x.Id);");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateRepositoryImpl(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine($"using {ns}.Infrastructure.Data;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Infrastructure.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Repository : I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {ns}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Repository({ns}DbContext context) {{ _context = context; }}");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> GetByIdAsync({entityName}Id id)");
            sb.AppendLine($"            => await _context.Set<{entityName}>().FirstOrDefaultAsync(e => e.Id.Equals(id));");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IReadOnlyList<{entityName}>> GetAllAsync()");
            sb.AppendLine($"            => await _context.Set<{entityName}>().ToListAsync();");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> AddAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.Set<{entityName}>().Add(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("            return entity;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.Set<{entityName}>().Update(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task DeleteAsync({entityName}Id id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await GetByIdAsync(id);");
            sb.AppendLine($"            if (entity != null) {{ _context.Set<{entityName}>().Remove(entity); await _context.SaveChangesAsync(); }}");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateIUnitOfWork(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Infrastructure.UnitOfWork");
            sb.AppendLine("{");
            sb.AppendLine("    public interface IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine("        Task<int> SaveChangesAsync();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateUnitOfWork(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";

            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Infrastructure.Data;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Infrastructure.UnitOfWork");
            sb.AppendLine("{");
            sb.AppendLine($"    public class UnitOfWork : IUnitOfWork");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {ns}DbContext _context;");
            sb.AppendLine($"        public UnitOfWork({ns}DbContext context) {{ _context = context; }}");
            sb.AppendLine($"        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateController(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Application.Services;");
            sb.AppendLine($"using {ns}.Domain.ValueObjects;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.API.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("[ApiController]");
            sb.AppendLine("[Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {entityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}AppService _appService;");
            sb.AppendLine($"        public {entityName}Controller(I{entityName}AppService appService) {{ _appService = appService; }}");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}Dto>> Get({entityName}Id id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var dto = await _appService.GetByIdAsync(id);");
            sb.AppendLine($"            return dto == null ? NotFound() : Ok(dto);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("[HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<IReadOnlyList<{entityName}Dto>>> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            var list = await _appService.GetAllAsync();");
            sb.AppendLine($"            return Ok(list);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("[HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}Dto>> Create([FromBody] Create{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = await _appService.CreateAsync(dto);");
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine($"            return CreatedAtAction(nameof(Get), new {{ id = result.{pkName} }}, result);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"[HttpPut(\"{{id}}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Update({entityName}Id id, [FromBody] Update{entityName}Dto dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _appService.UpdateAsync(id, dto);");
            sb.AppendLine($"            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"[HttpDelete(\"{{id}}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Delete({entityName}Id id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _appService.DeleteAsync(id);");
            sb.AppendLine($"            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
