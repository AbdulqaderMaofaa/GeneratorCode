using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class MicroservicesArchitecturePattern : BaseArchitecturePattern
    {
        public override string Name => "Microservices";
        public override string Description => "نمط الخدمات المصغرة";

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

                var outputPath = context.OutputPath ?? ".";
                var entityName = context.EntityName ?? context.TableInfo.Name ?? "Entity";
                var serviceName = $"{entityName}Service";
                var ns = context.Namespace ?? "GeneratedCode";

                if (options.GenerateDomainLayer && options.GenerateEntities)
                {
                    var domainEntitiesPath = Path.Combine(serviceName, "Domain", "Entities");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, domainEntitiesPath, $"{entityName}.cs", GenerateDomainEntity(context, serviceName), "Domain"));

                    var domainReposPath = Path.Combine(serviceName, "Domain", "Repositories");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, domainReposPath, $"I{entityName}Repository.cs", GenerateRepositoryInterface(context, serviceName), "Domain"));

                    var domainEventsPath = Path.Combine(serviceName, "Domain", "Events");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, domainEventsPath, $"{entityName}CreatedEvent.cs", GenerateIntegrationEvent(context, serviceName), "Domain"));
                }

                if (options.GenerateApplicationLayer)
                {
                    var appDtosPath = Path.Combine(serviceName, "Application", "DTOs");
                    if (options.GenerateDTOs)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appDtosPath, $"{entityName}Dto.cs", GenerateEntityDto(context, serviceName), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appDtosPath, $"Create{entityName}Request.cs", GenerateCreateRequest(context, serviceName), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appDtosPath, $"Update{entityName}Request.cs", GenerateUpdateRequest(context, serviceName), "Application"));
                    }

                    var appCommandsPath = Path.Combine(serviceName, "Application", "Commands");
                    if (options.GenerateServices)
                    {
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appCommandsPath, $"Create{entityName}Command.cs", GenerateCreateCommandAndHandler(context, serviceName), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appCommandsPath, $"Update{entityName}Command.cs", GenerateUpdateCommandAndHandler(context, serviceName), "Application"));
                        result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appCommandsPath, $"Delete{entityName}Command.cs", GenerateDeleteCommandAndHandler(context, serviceName), "Application"));
                    }

                    var appQueriesPath = Path.Combine(serviceName, "Application", "Queries");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appQueriesPath, $"Get{entityName}ByIdQuery.cs", GenerateGetByIdQueryAndHandler(context, serviceName), "Application"));
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, appQueriesPath, $"GetAll{entityName}Query.cs", GenerateGetAllQueryAndHandler(context, serviceName), "Application"));
                }

                if (options.GenerateInfrastructureLayer && options.GenerateRepositories)
                {
                    var infraDataPath = Path.Combine(serviceName, "Infrastructure", "Data");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, infraDataPath, $"{entityName}DbContext.cs", GenerateDbContext(context, serviceName), "Infrastructure"));

                    var infraReposPath = Path.Combine(serviceName, "Infrastructure", "Repositories");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, infraReposPath, $"{entityName}Repository.cs", GenerateRepositoryImplementation(context, serviceName), "Infrastructure"));
                }

                if (options.GenerateControllers)
                {
                    var apiControllersPath = Path.Combine(serviceName, "API", "Controllers");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, apiControllersPath, $"{entityName}Controller.cs", GenerateController(context, serviceName), "API"));
                }

                if (options.GenerateProgram)
                {
                    var apiPath = Path.Combine(serviceName, "API");
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, apiPath, "Program.cs", GenerateServiceProgram(context, serviceName), "API"));
                }

                var gatewayPath = "ApiGateway";
                result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, gatewayPath, "ocelot.json", GenerateOcelotConfig(context, entityName, serviceName), "Gateway"));
                result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, gatewayPath, "Program.cs", GenerateGatewayProgram(context), "Gateway"));

                result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, serviceName, "Dockerfile", GenerateServiceDockerfile(context, serviceName), "Infrastructure"));
                result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "", "docker-compose.yml", GenerateDockerCompose(context, entityName, serviceName), "Infrastructure"));

                result.Success = true;
                result.Message = "Microservices architecture generated successfully.";
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
            return await Task.FromResult(result);
        }

        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new() { "API", "Domain", "Infrastructure", "Gateway" };
        public override List<string> GetRequiredDependencies() => new() { "Ocelot", "RabbitMQ.Client" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            if (context == null) context = new CodeGenerationContext();
            context.TableInfo = table;
            context.TableName = table?.Name ?? "Table";
            context.EntityName = context.EntityName ?? table?.Name ?? "Entity";
            context.Namespace = context.Namespace ?? "GeneratedCode";
            var serviceName = $"{context.EntityName}Service";

            return new List<PreviewFile>
            {
                new() { FileName = $"{context.EntityName}.cs", Content = GenerateDomainEntity(context, serviceName), Language = "csharp" },
                new() { FileName = $"I{context.EntityName}Repository.cs", Content = GenerateRepositoryInterface(context, serviceName), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Dto.cs", Content = GenerateEntityDto(context, serviceName), Language = "csharp" },
                new() { FileName = $"Create{context.EntityName}Command.cs", Content = GenerateCreateCommandAndHandler(context, serviceName), Language = "csharp" },
                new() { FileName = $"Get{context.EntityName}ByIdQuery.cs", Content = GenerateGetByIdQueryAndHandler(context, serviceName), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Controller.cs", Content = GenerateController(context, serviceName), Language = "csharp" },
                new() { FileName = $"{context.EntityName}DbContext.cs", Content = GenerateDbContext(context, serviceName), Language = "csharp" },
                new() { FileName = "Program.cs", Content = GenerateServiceProgram(context, serviceName), Language = "csharp" },
                new() { FileName = "ocelot.json", Content = GenerateOcelotConfig(context, context.EntityName, serviceName), Language = "json" },
                new() { FileName = "docker-compose.yml", Content = GenerateDockerCompose(context, context.EntityName, serviceName), Language = "yaml" }
            };
        }

        private static GeneratedFile CreateGeneratedFile(string outputPath, string layerFolder, string fileName, string content, string layer)
        {
            var relativePath = string.IsNullOrEmpty(layerFolder) ? fileName : Path.Combine(layerFolder, fileName);
            var fullPath = Path.Combine(outputPath, relativePath);
            return new GeneratedFile
            {
                FileName = fileName,
                RelativePath = relativePath,
                FullPath = fullPath,
                Content = content,
                FileType = Path.GetExtension(fileName).TrimStart('.'),
                Layer = layer,
                SizeInBytes = System.Text.Encoding.UTF8.GetByteCount(content ?? ""),
                CreatedDate = DateTime.Now
            };
        }

        private static string GetPrimaryKeyName(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.Name ?? "Id";
        }

        private static string GetPrimaryKeyCSharpType(CodeGenerationContext context)
        {
            var col = context.TableInfo?.Columns?.FirstOrDefault(c => c.IsPrimaryKey);
            return col?.CSharpType ?? "int";
        }

        private static string GenerateDomainEntity(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name)) continue;
                    var csharpType = col.CSharpType ?? "object";
                    var nullable = col.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateRepositoryInterface(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {serviceName}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Domain.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}> GetByIdAsync({pkType} {pkName});");
            sb.AppendLine($"        Task<IReadOnlyList<{entityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}> AddAsync({entityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({entityName} entity);");
            sb.AppendLine($"        Task DeleteAsync({pkType} {pkName});");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateIntegrationEvent(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}CreatedEvent");
            sb.AppendLine("    {");
            sb.AppendLine($"        public Guid EventId {{ get; set; }} = Guid.NewGuid();");
            sb.AppendLine($"        public DateTime OccurredOn {{ get; set; }} = DateTime.UtcNow;");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateEntityDto(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Dto");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateCreateRequest(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{entityName}Request");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name)) continue;
                    if (col.IsPrimaryKey && col.IsAutoIncrement) continue;
                    var nameLower = col.Name.ToLowerInvariant();
                    if (nameLower == "id") continue;
                    sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateUpdateRequest(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{entityName}Request");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name) || col.IsPrimaryKey) continue;
                    sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateCreateCommandAndHandler(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Application.DTOs;");
            sb.AppendLine($"using {serviceName}.Domain.Entities;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.Commands");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Create{entityName}Command : IRequest<{entityName}Dto>");
            sb.AppendLine("    {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name)) continue;
                    if (col.IsPrimaryKey && col.IsAutoIncrement) continue;
                    if (col.Name.ToLowerInvariant() == "id") continue;
                    sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, {entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Create{entityName}CommandHandler(I{entityName}Repository repository) => _repository = repository;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}Dto> Handle(Create{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = new {entityName}();");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name)) continue;
                    if (col.IsPrimaryKey && col.IsAutoIncrement) continue;
                    if (col.Name.ToLowerInvariant() == "id") continue;
                    sb.AppendLine($"            entity.{col.Name} = request.{col.Name};");
                }
            }
            sb.AppendLine($"            var created = await _repository.AddAsync(entity);");
            sb.AppendLine($"            return new {entityName}Dto");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"                {col.Name} = created.{col.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateUpdateCommandAndHandler(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Application.DTOs;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.Commands");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Update{entityName}Command : IRequest<{entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {GetPrimaryKeyCSharpType(context)} {pkName} {{ get; set; }}");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name) || col.IsPrimaryKey) continue;
                    sb.AppendLine($"        public {col.CSharpType ?? "object"} {col.Name} {{ get; set; }}");
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public class Update{entityName}CommandHandler : IRequestHandler<Update{entityName}Command, {entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Update{entityName}CommandHandler(I{entityName}Repository repository) => _repository = repository;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}Dto> Handle(Update{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(request.{pkName});");
            sb.AppendLine("            if (entity == null) return null;");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col == null || string.IsNullOrEmpty(col.Name) || col.IsPrimaryKey) continue;
                    sb.AppendLine($"            entity.{col.Name} = request.{col.Name};");
                }
            }
            sb.AppendLine("            await _repository.UpdateAsync(entity);");
            sb.AppendLine($"            return new {entityName}Dto");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"                {col.Name} = entity.{col.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateDeleteCommandAndHandler(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.Commands");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Delete{entityName}Command : IRequest<Unit>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public class Delete{entityName}CommandHandler : IRequestHandler<Delete{entityName}Command, Unit>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Delete{entityName}CommandHandler(I{entityName}Repository repository) => _repository = repository;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<Unit> Handle(Delete{entityName}Command request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _repository.DeleteAsync(request.{pkName});");
            sb.AppendLine("            return Unit.Value;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetByIdQueryAndHandler(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Application.DTOs;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.Queries");
            sb.AppendLine("{");
            sb.AppendLine($"    public class Get{entityName}ByIdQuery : IRequest<{entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {pkType} {pkName} {{ get; set; }}");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public class Get{entityName}ByIdQueryHandler : IRequestHandler<Get{entityName}ByIdQuery, {entityName}Dto>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public Get{entityName}ByIdQueryHandler(I{entityName}Repository repository) => _repository = repository;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}Dto> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _repository.GetByIdAsync(request.{pkName});");
            sb.AppendLine("            if (entity == null) return null;");
            sb.AppendLine($"            return new {entityName}Dto");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"                {col.Name} = entity.{col.Name},");
                }
            }
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGetAllQueryAndHandler(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Application.DTOs;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Application.Queries");
            sb.AppendLine("{");
            sb.AppendLine($"    public class GetAll{entityName}Query : IRequest<IReadOnlyList<{entityName}Dto>>");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    public class GetAll{entityName}QueryHandler : IRequestHandler<GetAll{entityName}Query, IReadOnlyList<{entityName}Dto>>");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public GetAll{entityName}QueryHandler(I{entityName}Repository repository) => _repository = repository;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IReadOnlyList<{entityName}Dto>> Handle(GetAll{entityName}Query request, CancellationToken cancellationToken)");
            sb.AppendLine("        {");
            sb.AppendLine("            var entities = await _repository.GetAllAsync();");
            sb.AppendLine($"            return entities.Select(e => new {entityName}Dto");
            sb.AppendLine("            {");
            if (context.TableInfo?.Columns != null)
            {
                foreach (var col in context.TableInfo.Columns)
                {
                    if (col != null && !string.IsNullOrEmpty(col.Name))
                        sb.AppendLine($"                {col.Name} = e.{col.Name},");
                }
            }
            sb.AppendLine("            }).ToList();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateDbContext(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var tableName = context.TableName ?? context.TableInfo?.Name ?? "Table";
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {serviceName}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Infrastructure.Data");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}DbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {entityName}DbContext(DbContextOptions<{entityName}DbContext> options) : base(options) {{ }}");
            sb.AppendLine($"        public DbSet<{entityName}> {entityName}s {{ get; set; }}");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine($"            modelBuilder.Entity<{entityName}>().ToTable(\"{tableName}\");");
            if (context.TableInfo?.PrimaryKeys != null && context.TableInfo.PrimaryKeys.Count > 0)
            {
                if (context.TableInfo.PrimaryKeys.Count == 1)
                    sb.AppendLine($"            modelBuilder.Entity<{entityName}>().HasKey(e => e.{context.TableInfo.PrimaryKeys[0]});");
                else
                    sb.AppendLine($"            modelBuilder.Entity<{entityName}>().HasKey(e => new {{ {string.Join(", ", context.TableInfo.PrimaryKeys.Select(pk => $"e.{pk}"))} }});");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateRepositoryImplementation(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {serviceName}.Domain.Entities;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine($"using {serviceName}.Infrastructure.Data;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.Infrastructure.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Repository : I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {entityName}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Repository({entityName}DbContext context) => _context = context;");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> GetByIdAsync({pkType} {pkName})");
            sb.AppendLine($"            => await _context.{entityName}s.FindAsync({pkName});");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IReadOnlyList<{entityName}>> GetAllAsync()");
            sb.AppendLine($"            => await _context.{entityName}s.ToListAsync();");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> AddAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.{entityName}s.Add(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("            return entity;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.{entityName}s.Update(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task DeleteAsync({pkType} {pkName})");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await _context.{entityName}s.FindAsync({pkName});");
            sb.AppendLine("            if (entity != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _context.{entityName}s.Remove(entity);");
            sb.AppendLine("                await _context.SaveChangesAsync();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateController(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            var pkName = GetPrimaryKeyName(context);
            var pkType = GetPrimaryKeyCSharpType(context);
            var routeKey = pkName.Length > 0 ? char.ToLowerInvariant(pkName[0]) + pkName.Substring(1) : "id";
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Application.Commands;");
            sb.AppendLine($"using {serviceName}.Application.Queries;");
            sb.AppendLine($"using {serviceName}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {serviceName}.API.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {entityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IMediator _mediator;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Controller(IMediator mediator) => _mediator = mediator;");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<IReadOnlyList<{entityName}Dto>>> GetAll()");
            sb.AppendLine($"            => Ok(await _mediator.Send(new GetAll{entityName}Query()));");
            sb.AppendLine();
            sb.AppendLine($"        [HttpGet(\"{{{routeKey}}}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}Dto>> GetById([FromRoute] {pkType} {routeKey})");
            sb.AppendLine("        {");
            sb.AppendLine($"            var result = await _mediator.Send(new Get{entityName}ByIdQuery {{ {pkName} = {routeKey} }});");
            sb.AppendLine("            if (result == null) return NotFound();");
            sb.AppendLine("            return result;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}Dto>> Create([FromBody] Create{entityName}Command command)");
            sb.AppendLine($"            => await _mediator.Send(command);");
            sb.AppendLine();
            sb.AppendLine($"        [HttpPut(\"{{{routeKey}}}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}Dto>> Update([FromRoute] {pkType} {routeKey}, [FromBody] Update{entityName}Command command)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if ({routeKey} != command.{pkName}) return BadRequest();");
            sb.AppendLine($"            return await _mediator.Send(command);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        [HttpDelete(\"{{{routeKey}}}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Delete([FromRoute] {pkType} {routeKey})");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _mediator.Send(new Delete{entityName}Command {{ {pkName} = {routeKey} }});");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateServiceProgram(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var entityName = context.EntityName ?? "Entity";
            sb.AppendLine("using System;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {serviceName}.Domain.Repositories;");
            sb.AppendLine($"using {serviceName}.Infrastructure.Data;");
            sb.AppendLine($"using {serviceName}.Infrastructure.Repositories;");
            sb.AppendLine();
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine("builder.Services.AddControllers();");
            sb.AppendLine("builder.Services.AddEndpointsApiExplorer();");
            sb.AppendLine("builder.Services.AddSwaggerGen();");
            sb.AppendLine($"builder.Services.AddDbContext<{entityName}DbContext>(options =>");
            sb.AppendLine("    options.UseSqlServer(builder.Configuration.GetConnectionString(\"DefaultConnection\")));");
            sb.AppendLine($"builder.Services.AddScoped<I{entityName}Repository, {entityName}Repository>();");
            sb.AppendLine("builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));");
            sb.AppendLine();
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine("if (app.Environment.IsDevelopment()) app.UseSwagger().UseSwaggerUI();");
            sb.AppendLine("app.UseAuthorization();");
            sb.AppendLine("app.MapControllers();");
            sb.AppendLine("app.Run();");
            return sb.ToString();
        }

        private static string GenerateOcelotConfig(CodeGenerationContext context, string entityName, string serviceName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"Routes\": [");
            sb.AppendLine("    {");
            sb.AppendLine($"      \"DownstreamPathTemplate\": \"/api/{entityName}/{{everything}}\",");
            sb.AppendLine($"      \"DownstreamScheme\": \"http\",");
            sb.AppendLine($"      \"DownstreamHostAndPorts\": [ {{ \"Host\": \"localhost\", \"Port\": 5001 }} ],");
            sb.AppendLine($"      \"UpstreamPathTemplate\": \"/api/{entityName}/{{everything}}\",");
            sb.AppendLine("      \"UpstreamHttpMethod\": [ \"GET\", \"POST\", \"PUT\", \"DELETE\" ]");
            sb.AppendLine("    }");
            sb.AppendLine("  ]");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateGatewayProgram(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Ocelot.DependencyInjection;");
            sb.AppendLine("using Ocelot.Middleware;");
            sb.AppendLine();
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine("builder.Configuration.AddJsonFile(\"ocelot.json\", optional: false, reloadOnChange: true);");
            sb.AppendLine("builder.Services.AddOcelot(builder.Configuration);");
            sb.AppendLine();
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine("await app.UseOcelot();");
            sb.AppendLine("app.Run();");
            return sb.ToString();
        }

        private static string GenerateServiceDockerfile(CodeGenerationContext context, string serviceName)
        {
            var sb = new StringBuilder();
            var framework = context.TargetFramework ?? "net8.0";
            sb.AppendLine($"FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base");
            sb.AppendLine("WORKDIR /app");
            sb.AppendLine("EXPOSE 80");
            sb.AppendLine();
            sb.AppendLine($"FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build");
            sb.AppendLine("WORKDIR /src");
            sb.AppendLine("COPY [\"API/API.csproj\", \"API/\"]");
            sb.AppendLine("RUN dotnet restore \"API/API.csproj\"");
            sb.AppendLine("COPY . .");
            sb.AppendLine("WORKDIR \"/src/API\"");
            sb.AppendLine("RUN dotnet build \"API.csproj\" -c Release -o /app/build");
            sb.AppendLine();
            sb.AppendLine("FROM build AS publish");
            sb.AppendLine("RUN dotnet publish \"API.csproj\" -c Release -o /app/publish");
            sb.AppendLine();
            sb.AppendLine("FROM base AS final");
            sb.AppendLine("WORKDIR /app");
            sb.AppendLine("COPY --from=publish /app/publish .");
            sb.AppendLine("ENTRYPOINT [\"dotnet\", \"API.dll\"]");
            return sb.ToString();
        }

        private static string GenerateDockerCompose(CodeGenerationContext context, string entityName, string serviceName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("version: '3.8'");
            sb.AppendLine("services:");
            sb.AppendLine($"  {serviceName.ToLowerInvariant()}:");
            sb.AppendLine("    build:");
            sb.AppendLine($"      context: ./{serviceName}");
            sb.AppendLine("      dockerfile: Dockerfile");
            sb.AppendLine("    ports:");
            sb.AppendLine("      - \"5001:80\"");
            sb.AppendLine("    environment:");
            sb.AppendLine("      - ASPNETCORE_ENVIRONMENT=Development");
            sb.AppendLine("      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MicroserviceDb;User Id=sa;Password=YourStrong@Passw0rd;");
            sb.AppendLine();
            sb.AppendLine("  apigateway:");
            sb.AppendLine("    image: mcr.microsoft.com/dotnet/aspnet:8.0");
            sb.AppendLine("    working_dir: /app");
            sb.AppendLine("    ports:");
            sb.AppendLine("      - \"5000:80\"");
            sb.AppendLine("    depends_on:");
            sb.AppendLine($"      - {serviceName.ToLowerInvariant()}");
            sb.AppendLine();
            sb.AppendLine("  sqlserver:");
            sb.AppendLine("    image: mcr.microsoft.com/mssql/server:2022-latest");
            sb.AppendLine("    environment:");
            sb.AppendLine("      - ACCEPT_EULA=Y");
            sb.AppendLine("      - MSSQL_SA_PASSWORD=YourStrong@Passw0rd");
            sb.AppendLine("    ports:");
            sb.AppendLine("      - \"1433:1433\"");
            return sb.ToString();
        }
    }
}
