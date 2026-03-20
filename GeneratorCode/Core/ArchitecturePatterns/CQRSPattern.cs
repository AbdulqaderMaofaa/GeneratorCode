using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class CQRSPattern : BaseArchitecturePattern
    {
        public override string Name => "CQRS";
        public override string Description => "نمط فصل القراءة عن الكتابة (Command Query Responsibility Segregation) باستخدام MediatR";

        public override async Task<CodeGenerationResult> Generate(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                string basePath;
                if (!context.OutputPath.EndsWith(context.Namespace ?? ""))
                    basePath = Path.Combine(context.OutputPath, context.Namespace ?? "GeneratedCode");
                else
                    basePath = context.OutputPath;

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                var domainPath = Path.Combine(basePath, "Domain");
                var applicationPath = Path.Combine(basePath, "Application");
                var apiPath = Path.Combine(basePath, "API");

                CreateDirectoryIfNotExists(domainPath);
                CreateDirectoryIfNotExists(Path.Combine(domainPath, "Entities"));
                CreateDirectoryIfNotExists(Path.Combine(domainPath, "Repositories"));
                CreateDirectoryIfNotExists(applicationPath);
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "DTOs"));
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "Commands", $"Create{context.EntityName}"));
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "Commands", $"Update{context.EntityName}"));
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "Commands", $"Delete{context.EntityName}"));
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "Queries", $"Get{context.EntityName}ById"));
                CreateDirectoryIfNotExists(Path.Combine(applicationPath, "Queries", $"GetAll{context.EntityName}"));
                CreateDirectoryIfNotExists(Path.Combine(apiPath, "Controllers"));

                var ns = context.Namespace ?? "GeneratedCode";
                var entityName = context.EntityName ?? "Entity";

                // Domain
                AddFile(result, Path.Combine(domainPath, "Entities", $"{entityName}.cs"), $"Domain/Entities/{entityName}.cs", GenerateEntity(context), "Domain");
                AddFile(result, Path.Combine(domainPath, "Repositories", $"I{entityName}Repository.cs"), $"Domain/Repositories/I{entityName}Repository.cs", GenerateRepositoryInterface(context), "Domain");

                // Application DTOs
                AddFile(result, Path.Combine(applicationPath, "DTOs", $"{entityName}DTO.cs"), $"Application/DTOs/{entityName}DTO.cs", GenerateEntityDto(context), "Application");

                // Commands
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Create{entityName}", $"Create{entityName}Command.cs"), $"Application/Commands/Create{entityName}/Create{entityName}Command.cs", GenerateCreateCommand(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Create{entityName}", $"Create{entityName}CommandHandler.cs"), $"Application/Commands/Create{entityName}/Create{entityName}CommandHandler.cs", GenerateCreateCommandHandler(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Update{entityName}", $"Update{entityName}Command.cs"), $"Application/Commands/Update{entityName}/Update{entityName}Command.cs", GenerateUpdateCommand(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Update{entityName}", $"Update{entityName}CommandHandler.cs"), $"Application/Commands/Update{entityName}/Update{entityName}CommandHandler.cs", GenerateUpdateCommandHandler(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Delete{entityName}", $"Delete{entityName}Command.cs"), $"Application/Commands/Delete{entityName}/Delete{entityName}Command.cs", GenerateDeleteCommand(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Commands", $"Delete{entityName}", $"Delete{entityName}CommandHandler.cs"), $"Application/Commands/Delete{entityName}/Delete{entityName}CommandHandler.cs", GenerateDeleteCommandHandler(context), "Application");

                // Queries
                AddFile(result, Path.Combine(applicationPath, "Queries", $"Get{entityName}ById", $"Get{entityName}ByIdQuery.cs"), $"Application/Queries/Get{entityName}ById/Get{entityName}ByIdQuery.cs", GenerateGetByIdQuery(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Queries", $"Get{entityName}ById", $"Get{entityName}ByIdQueryHandler.cs"), $"Application/Queries/Get{entityName}ById/Get{entityName}ByIdQueryHandler.cs", GenerateGetByIdQueryHandler(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Queries", $"GetAll{entityName}", $"GetAll{entityName}Query.cs"), $"Application/Queries/GetAll{entityName}/GetAll{entityName}Query.cs", GenerateGetAllQuery(context), "Application");
                AddFile(result, Path.Combine(applicationPath, "Queries", $"GetAll{entityName}", $"GetAll{entityName}QueryHandler.cs"), $"Application/Queries/GetAll{entityName}/GetAll{entityName}QueryHandler.cs", GenerateGetAllQueryHandler(context), "Application");

                // API
                AddFile(result, Path.Combine(apiPath, "Controllers", $"{entityName}Controller.cs"), $"API/Controllers/{entityName}Controller.cs", GenerateController(context), "API");

                result.Success = true;
                result.Message = "تم توليد كود CQRS بنجاح";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"حدث خطأ أثناء توليد الكود: {ex.Message}";
                result.Errors.Add(ex.ToString());
            }

            stopwatch.Stop();
            result.GenerationTime = stopwatch.Elapsed;
            return await Task.FromResult(result);
        }

        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;

        public override List<string> GetRequiredLayers() => new() { "Commands", "Queries", "Handlers" };

        public override List<string> GetRequiredDependencies() => new() { "MediatR" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            if (context == null) context = new CodeGenerationContext();
            context.TableInfo = table;
            context.TableName = table?.Name ?? "Entity";
            context.EntityName = context.EntityName ?? table?.Name ?? "Entity";
            context.Namespace = context.Namespace ?? "GeneratedCode";

            return new List<PreviewFile>
            {
                new() { FileName = $"{context.EntityName}.cs", Content = GenerateEntity(context), Language = "csharp" },
                new() { FileName = $"Create{context.EntityName}Command.cs", Content = GenerateCreateCommand(context), Language = "csharp" },
                new() { FileName = $"Get{context.EntityName}ByIdQuery.cs", Content = GenerateGetByIdQuery(context), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Controller.cs", Content = GenerateController(context), Language = "csharp" }
            };
        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        private static void AddFile(CodeGenerationResult result, string fullPath, string relativePath, string content, string layer)
        {
            if (string.IsNullOrEmpty(content)) return;
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = Path.GetFileName(fullPath),
                RelativePath = relativePath,
                FullPath = fullPath,
                Content = content,
                FileType = "cs",
                Layer = layer
            });
        }

        private static string GetPrimaryKeyType(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.CSharpType ?? "int";
        }

        private static string GetPrimaryKeyName(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.Name ?? "Id";
        }

        private static string GenerateEntity(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}");
            sb.AppendLine("    {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"        public {column.CSharpType ?? "object"} {column.Name} {{ get; set; }}");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateRepositoryInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyType(context);
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Domain.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}> GetByIdAsync({pkType} {pkName});");
            sb.AppendLine($"        Task<List<{entityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}> AddAsync({entityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({entityName} entity);");
            sb.AppendLine($"        Task DeleteAsync({pkType} {pkName});");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateEntityDto(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}DTO");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"        public {column.CSharpType ?? "object"} {column.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateCreateCommand(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Create{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{entityName}Command : IRequest<{entityName}DTO>");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column == null || string.IsNullOrEmpty(column.Name)) continue;
                    if (column.IsPrimaryKey && column.IsAutoIncrement) continue;
                    var nameLower = column.Name.ToLowerInvariant();
                    if (nameLower == "id") continue;
                    sb.AppendLine($"        public {column.CSharpType ?? "object"} {column.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateCreateCommandHandler(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Create{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, {entityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Create{entityName}CommandHandler(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}DTO> Handle(Create{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = new {entityName}();");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column == null || string.IsNullOrEmpty(column.Name)) continue;
                    if (column.IsPrimaryKey && column.IsAutoIncrement) continue;
                    var nameLower = column.Name.ToLowerInvariant();
                    if (nameLower == "id") continue;
                    sb.AppendLine($"            entity.{column.Name} = request.{column.Name};");
                }
            }
            sb.AppendLine($"            var created = await _repository.AddAsync(entity);");
            sb.AppendLine($"            return new {entityName}DTO");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"                {column.Name} = created.{column.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateUpdateCommand(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyType(context);
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Update{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{entityName}Command : IRequest<{entityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column == null || string.IsNullOrEmpty(column.Name) || column.IsPrimaryKey) continue;
                    sb.AppendLine($"        public {column.CSharpType ?? "object"} {column.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateUpdateCommandHandler(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.Entities;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Update{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{entityName}CommandHandler : IRequestHandler<Update{entityName}Command, {entityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Update{entityName}CommandHandler(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}DTO> Handle(Update{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(request.{pkName});");
            sb.AppendLine("            if (entity == null) return null;");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column == null || string.IsNullOrEmpty(column.Name) || column.IsPrimaryKey) continue;
                    sb.AppendLine($"            entity.{column.Name} = request.{column.Name};");
                }
            }
            sb.AppendLine("            await _repository.UpdateAsync(entity);");
            sb.AppendLine($"            return new {entityName}DTO");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"                {column.Name} = entity.{column.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateDeleteCommand(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyType(context);
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using MediatR;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Delete{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Delete{entityName}Command : IRequest<Unit>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateDeleteCommandHandler(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Commands.Delete{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Delete{entityName}CommandHandler : IRequestHandler<Delete{entityName}Command, Unit>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Delete{entityName}CommandHandler(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<Unit> Handle(Delete{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _repository.DeleteAsync(request.{GetPrimaryKeyName(context)});");
            sb.AppendLine("            return Unit.Value;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetByIdQuery(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyType(context);
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Queries.Get{entityName}ById");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{entityName}ByIdQuery : IRequest<{entityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetByIdQueryHandler(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Queries.Get{entityName}ById");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{entityName}ByIdQueryHandler : IRequestHandler<Get{entityName}ByIdQuery, {entityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Get{entityName}ByIdQueryHandler(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}DTO> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(request.{pkName});");
            sb.AppendLine("            if (entity == null) return null;");
            sb.AppendLine($"            return new {entityName}DTO");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"                {column.Name} = entity.{column.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetAllQuery(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Queries.GetAll{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class GetAll{entityName}Query : IRequest<List<{entityName}DTO>>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetAllQueryHandler(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine($"using {ns}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Application.Queries.GetAll{entityName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class GetAll{entityName}QueryHandler : IRequestHandler<GetAll{entityName}Query, List<{entityName}DTO>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public GetAll{entityName}QueryHandler(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<List<{entityName}DTO>> Handle(GetAll{entityName}Query request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine("            var entities = await _repository.GetAllAsync();");
            sb.AppendLine($"            return entities.Select(e => new {entityName}DTO");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column != null && !string.IsNullOrEmpty(column.Name))
                        sb.AppendLine($"                {column.Name} = e.{column.Name},");
                }
            }
            sb.AppendLine("            }).ToList();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateController(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {ns}.Application.Commands.Create{entityName};");
            sb.AppendLine($"using {ns}.Application.Commands.Update{entityName};");
            sb.AppendLine($"using {ns}.Application.Commands.Delete{entityName};");
            sb.AppendLine($"using {ns}.Application.Queries.Get{entityName}ById;");
            sb.AppendLine($"using {ns}.Application.Queries.GetAll{entityName};");
            sb.AppendLine($"using {ns}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.API.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {entityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IMediator _mediator;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Controller(IMediator mediator)");
            sb.AppendLine("        {");
            sb.AppendLine("            _mediator = mediator;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<List<{entityName}DTO>>> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _mediator.Send(new GetAll{entityName}Query());");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}DTO>> GetById([FromRoute] {GetPrimaryKeyType(context)} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = await _mediator.Send(new Get{entityName}ByIdQuery {{ {pkName} = id }});");
            sb.AppendLine("            if (result == null) return NotFound();");
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}DTO>> Create([FromBody] Create{entityName}Command command)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _mediator.Send(command);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}DTO>> Update([FromRoute] {GetPrimaryKeyType(context)} id, [FromBody] Update{entityName}Command command)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (id != command.{pkName}) return BadRequest();");
            sb.AppendLine($"            return await _mediator.Send(command);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpDelete(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Delete([FromRoute] {GetPrimaryKeyType(context)} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _mediator.Send(new Delete{entityName}Command {{ {pkName} = id }});");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
