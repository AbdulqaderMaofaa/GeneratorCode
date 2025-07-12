using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.DependencyInjection;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    /// <summary>
    /// نمط Clean Architecture
    /// </summary>
    public class CleanArchitecturePattern : BaseArchitecturePattern
    {
        private readonly ITemplateEngine _templateEngine;

        public CleanArchitecturePattern(ITemplateEngine templateEngine)
        {
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        }

        public override string Name => "Clean Architecture";

        public override string Description => "نمط معماري يركز على فصل الاهتمامات وجعل التطبيق مستقلاً عن التفاصيل الخارجية";

        public override async Task<CodeGenerationResult> Generate(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Determine the base path (check if it already ends with namespace)
                string namespacePath;
                if (!context.OutputPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(context.OutputPath, context.Namespace);
                }
                else
                {
                    namespacePath = context.OutputPath;
                }

                // Create base directory if it doesn't exist
                if (!Directory.Exists(namespacePath))
                {
                    Directory.CreateDirectory(namespacePath);
                }

                // Create src and tests directories
                var srcPath = Path.Combine(namespacePath, "src");
                var testsPath = Path.Combine(namespacePath, "tests");
                
                // Only create directories if they don't exist
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }
                if (!Directory.Exists(testsPath))
                {
                    Directory.CreateDirectory(testsPath);
                }

                // Create project directories inside src folder with full names
                var domainPath = Path.Combine(srcPath, $"{context.Namespace}.Domain");
                var applicationPath = Path.Combine(srcPath, $"{context.Namespace}.Application");
                var infrastructurePath = Path.Combine(srcPath, $"{context.Namespace}.Infrastructure");
                var apiPath = Path.Combine(srcPath, $"{context.Namespace}.API");
                var unitTestsPath = Path.Combine(testsPath, $"{context.Namespace}.UnitTests");
                var integrationTestsPath = Path.Combine(testsPath, $"{context.Namespace}.IntegrationTests");

                // Create project directories only if they don't exist
                CreateDirectoryIfNotExists(domainPath);
                CreateDirectoryIfNotExists(applicationPath);
                CreateDirectoryIfNotExists(infrastructurePath);
                CreateDirectoryIfNotExists(apiPath);
                CreateDirectoryIfNotExists(unitTestsPath);
                CreateDirectoryIfNotExists(integrationTestsPath);

                // Create subdirectories for each project
                CreateProjectStructure(domainPath);
                CreateProjectStructure(applicationPath);
                CreateProjectStructure(infrastructurePath);
                CreateProjectStructure(apiPath);
                CreateTestProjectStructure(unitTestsPath);
                CreateTestProjectStructure(integrationTestsPath);

                // Generate project files first
                var packagesGenerator = new PackagesGenerator();
                
                // Generate project files
                var domainProjectFile = packagesGenerator.GenerateProjectFile(context, null, "Domain");
                var applicationProjectFile = packagesGenerator.GenerateProjectFile(context, null, "Application");
                var infrastructureProjectFile = packagesGenerator.GenerateProjectFile(context, null, "Infrastructure");
                var apiProjectFile = packagesGenerator.GenerateProjectFile(context, null, "API");
                var unitTestsProjectFile = packagesGenerator.GenerateProjectFile(context, null, "UnitTests");
                var integrationTestsProjectFile = packagesGenerator.GenerateProjectFile(context, null, "IntegrationTests");

                // Add project files with correct relative paths
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.Domain.csproj",
                    RelativePath = $"src/{context.Namespace}.Domain/{context.Namespace}.Domain.csproj",
                    FullPath = Path.Combine(domainPath, $"{context.Namespace}.Domain.csproj"),
                    Content = domainProjectFile,
                    FileType = "csproj",
                    Layer = "Domain"
                });

                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.Application.csproj",
                    RelativePath = $"src/{context.Namespace}.Application/{context.Namespace}.Application.csproj",
                    FullPath = Path.Combine(applicationPath, $"{context.Namespace}.Application.csproj"),
                    Content = applicationProjectFile,
                    FileType = "csproj",
                    Layer = "Application"
                });

                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.Infrastructure.csproj",
                    RelativePath = $"src/{context.Namespace}.Infrastructure/{context.Namespace}.Infrastructure.csproj",
                    FullPath = Path.Combine(infrastructurePath, $"{context.Namespace}.Infrastructure.csproj"),
                    Content = infrastructureProjectFile,
                    FileType = "csproj",
                    Layer = "Infrastructure"
                });

                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.API.csproj",
                    RelativePath = $"src/{context.Namespace}.API/{context.Namespace}.API.csproj",
                    FullPath = Path.Combine(apiPath, $"{context.Namespace}.API.csproj"),
                    Content = apiProjectFile,
                    FileType = "csproj",
                    Layer = "API"
                });

                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.UnitTests.csproj",
                    RelativePath = $"tests/{context.Namespace}.UnitTests/{context.Namespace}.UnitTests.csproj",
                    FullPath = Path.Combine(unitTestsPath, $"{context.Namespace}.UnitTests.csproj"),
                    Content = unitTestsProjectFile,
                    FileType = "csproj",
                    Layer = "UnitTests"
                });

                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.IntegrationTests.csproj",
                    RelativePath = $"tests/{context.Namespace}.IntegrationTests/{context.Namespace}.IntegrationTests.csproj",
                    FullPath = Path.Combine(integrationTestsPath, $"{context.Namespace}.IntegrationTests.csproj"),
                    Content = integrationTestsProjectFile,
                    FileType = "csproj",
                    Layer = "IntegrationTests"
                });

                // Generate Domain Layer
                GenerateDomainLayer(context, result, domainPath);
                
                // Generate Application Layer
                GenerateApplicationLayer(context, result, applicationPath);
                
                // Generate Infrastructure Layer
                GenerateInfrastructureLayer(context, result, infrastructurePath);
                
                // Generate API Layer
                GeneratePresentationLayer(context, result, apiPath);

                // Generate Test Projects
                GenerateTestProjects(context, result, unitTestsPath, integrationTestsPath);

                // Generate global.json
                var globalJsonContent = @"{
  ""sdk"": {
    ""version"": ""6.0.100"",
    ""rollForward"": ""latestFeature""
  }
}";
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = "global.json",
                    RelativePath = "global.json",
                    FullPath = Path.Combine(namespacePath, "global.json"),
                    Content = globalJsonContent,
                    FileType = "json",
                    Layer = "Root"
                });

                // Generate solution file last
                var templatePath = "CleanArchitecture/Solution.template";
                var template = await _templateEngine.LoadTemplateAsync(templatePath);
                
                // Create template data with all necessary GUIDs
                var templateData = new Dictionary<string, object>
                {
                    { "namespace", context.Namespace },
                    { "solutionGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "srcFolderGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "testsFolderGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "domainProjectGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "applicationProjectGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "infrastructureProjectGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "apiProjectGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "unitTestsProjectGuid", Guid.NewGuid().ToString("B").ToUpper() },
                    { "integrationTestsProjectGuid", Guid.NewGuid().ToString("B").ToUpper() }
                };
                
                var solutionContent = _templateEngine.RenderTemplate(template, templateData);
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.Namespace}.sln",
                    RelativePath = $"{context.Namespace}.sln",
                    FullPath = Path.Combine(namespacePath, $"{context.Namespace}.sln"),
                    Content = solutionContent,
                    FileType = "sln",
                    Layer = "Root"
                });

                result.Success = true;
                result.Message = "تم توليد كود Clean Architecture بنجاح";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"حدث خطأ أثناء توليد الكود: {ex.Message}";
                result.Errors.Add(ex.ToString());
            }

            stopwatch.Stop();
            result.GenerationTime = stopwatch.Elapsed;

            return result;
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CreateProjectStructure(string projectPath)
        {
            // Create common folders for each project
            Directory.CreateDirectory(Path.Combine(projectPath, "Interfaces"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Models"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Services"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Extensions"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Exceptions"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Constants"));

            // Create specific folders based on project type
            if (projectPath.EndsWith(".Domain"))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, "Entities"));
                Directory.CreateDirectory(Path.Combine(projectPath, "ValueObjects"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Events"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Repositories"));
            }
            else if (projectPath.EndsWith(".Application"))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, "DTOs"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Mappings"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Validators"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Features"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Common"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Behaviors"));
            }
            else if (projectPath.EndsWith(".Infrastructure"))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, "Data"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Repositories"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Services"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Persistence"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Identity"));
            }
            else if (projectPath.EndsWith(".API"))
            {
                Directory.CreateDirectory(Path.Combine(projectPath, "Controllers"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Filters"));
                Directory.CreateDirectory(Path.Combine(projectPath, "Middleware"));
                Directory.CreateDirectory(Path.Combine(projectPath, "ViewModels"));
            }
        }

        private void CreateTestProjectStructure(string testProjectPath)
        {
            Directory.CreateDirectory(Path.Combine(testProjectPath, "Common"));
            Directory.CreateDirectory(Path.Combine(testProjectPath, "Fixtures"));
            Directory.CreateDirectory(Path.Combine(testProjectPath, "Helpers"));
            Directory.CreateDirectory(Path.Combine(testProjectPath, "Mocks"));
            
            if (testProjectPath.EndsWith(".UnitTests"))
            {
                Directory.CreateDirectory(Path.Combine(testProjectPath, "Domain"));
                Directory.CreateDirectory(Path.Combine(testProjectPath, "Application"));
                Directory.CreateDirectory(Path.Combine(testProjectPath, "Infrastructure"));
            }
            else if (testProjectPath.EndsWith(".IntegrationTests"))
            {
                Directory.CreateDirectory(Path.Combine(testProjectPath, "API"));
                Directory.CreateDirectory(Path.Combine(testProjectPath, "Persistence"));
            }
        }

        public override bool SupportsDatabaseType(DatabaseType databaseType)
        {
            // Clean Architecture يدعم جميع قواعد البيانات
            return true;
        }

        public override List<string> GetRequiredLayers()
        {
            return new List<string>
            {
                "Domain",
                "Application",
                "Infrastructure",
                "Presentation"
            };
        }

        public override List<string> GetRequiredDependencies()
        {
            return new List<string>
            {
                "MediatR",
                "FluentValidation",
                "AutoMapper",
                "Microsoft.Extensions.DependencyInjection"
            };
        }

        private void GenerateDomainLayer(CodeGenerationContext context, CodeGenerationResult result, string basePath)
        {
            // توليد Entity
            var entityContent = GenerateEntity(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}.cs",
                RelativePath = $"src/{context.Namespace}.Domain/Entities/{context.EntityName}.cs",
                FullPath = Path.Combine(basePath, "Entities", $"{context.EntityName}.cs"),
                Content = entityContent,
                FileType = "cs",
                Layer = "Domain"
            });

            // توليد Repository Interface
            var repositoryInterfaceContent = GenerateRepositoryInterface(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"I{context.EntityName}Repository.cs",
                RelativePath = $"src/{context.Namespace}.Domain/Repositories/I{context.EntityName}Repository.cs",
                FullPath = Path.Combine(basePath, "Repositories", $"I{context.EntityName}Repository.cs"),
                Content = repositoryInterfaceContent,
                FileType = "cs",
                Layer = "Domain"
            });
        }

        private void GenerateApplicationLayer(CodeGenerationContext context, CodeGenerationResult result, string basePath)
        {
            // توليد Commands و Queries
            GenerateCommands(context, result);
            GenerateQueries(context, result);
            GenerateValidators(context, result);
            GenerateMappers(context, result);
        }

        private void GenerateInfrastructureLayer(CodeGenerationContext context, CodeGenerationResult result, string basePath)
        {
            // توليد Repository Implementation
            var repositoryContent = GenerateRepositoryImplementation(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}Repository.cs",
                RelativePath = $"src/{context.Namespace}.Infrastructure/Repositories/{context.EntityName}Repository.cs",
                FullPath = Path.Combine(basePath, "Repositories", $"{context.EntityName}Repository.cs"),
                Content = repositoryContent,
                FileType = "cs",
                Layer = "Infrastructure"
            });

            // توليد DbContext Configuration
            var dbContextContent = GenerateDbContextConfiguration(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}Configuration.cs",
                RelativePath = $"src/{context.Namespace}.Infrastructure/Data/Configurations/{context.EntityName}Configuration.cs",
                FullPath = Path.Combine(basePath, "Data", "Configurations", $"{context.EntityName}Configuration.cs"),
                Content = dbContextContent,
                FileType = "cs",
                Layer = "Infrastructure"
            });
        }

        private void GeneratePresentationLayer(CodeGenerationContext context, CodeGenerationResult result, string basePath)
        {
            // توليد Controller
            var controllerContent = GenerateController(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}Controller.cs",
                RelativePath = $"src/{context.Namespace}.API/Controllers/{context.EntityName}Controller.cs",
                FullPath = Path.Combine(basePath, "Controllers", $"{context.EntityName}Controller.cs"),
                Content = controllerContent,
                FileType = "cs",
                Layer = "Presentation"
            });
        }

        private string GenerateEntity(CodeGenerationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}");
            sb.AppendLine("    {");

            // إضافة الخصائص من معلومات الجدول
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine($"        public {column.CSharpType} {column.Name} {{ get; set; }}");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRepositoryInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Domain.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{context.EntityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{context.EntityName}> GetByIdAsync(int id);");
            sb.AppendLine($"        Task<List<{context.EntityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{context.EntityName}> AddAsync({context.EntityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({context.EntityName} entity);");
            sb.AppendLine($"        Task DeleteAsync(int id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateRepositoryImplementation(CodeGenerationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Infrastructure.Repositories");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Repository : I{context.EntityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName}Repository(DbContext context)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = context;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{context.EntityName}> GetByIdAsync(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _context.Set<{context.EntityName}>().FindAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<List<{context.EntityName}>> GetAllAsync()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _context.Set<{context.EntityName}>().ToListAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{context.EntityName}> AddAsync({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.Set<{context.EntityName}>().Add(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("            return entity;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({context.EntityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            _context.Set<{context.EntityName}>().Update(entity);");
            sb.AppendLine("            await _context.SaveChangesAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task DeleteAsync(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await GetByIdAsync(id);");
            sb.AppendLine("            if (entity != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _context.Set<{context.EntityName}>().Remove(entity);");
            sb.AppendLine("                await _context.SaveChangesAsync();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateDbContextConfiguration(CodeGenerationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Infrastructure.Data.Configurations");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Configuration : IEntityTypeConfiguration<{context.EntityName}>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public void Configure(EntityTypeBuilder<{context.EntityName}> builder)");
            sb.AppendLine("        {");
            sb.AppendLine($"            builder.ToTable(\"{context.TableName}\");");
            sb.AppendLine();

            // إضافة تكوين الأعمدة
            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column.IsPrimaryKey)
                    {
                        sb.AppendLine($"            builder.HasKey(x => x.{column.Name});");
                    }

                    sb.AppendLine($"            builder.Property(x => x.{column.Name})");
                    sb.AppendLine($"                .HasColumnName(\"{column.Name}\")");

                    if (column.MaxLength.HasValue)
                    {
                        sb.AppendLine($"                .HasMaxLength({column.MaxLength.Value})");
                    }

                    if (!column.IsNullable)
                    {
                        sb.AppendLine("                .IsRequired()");
                    }

                    sb.AppendLine("                ;");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateController(CodeGenerationContext context)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using MediatR;");
            sb.AppendLine($"using {context.Namespace}.Application.Features.{context.EntityName}.Commands;");
            sb.AppendLine($"using {context.Namespace}.Application.Features.{context.EntityName}.Queries;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Presentation.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine($"    [Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {context.EntityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly IMediator _mediator;");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName}Controller(IMediator mediator)");
            sb.AppendLine("        {");
            sb.AppendLine("            _mediator = mediator;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<List<{context.EntityName}>>> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _mediator.Send(new Get{context.EntityName}ListQuery());");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet(\"{id}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{context.EntityName}>> GetById(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _mediator.Send(new Get{context.EntityName}ByIdQuery {{ Id = id }});");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{context.EntityName}>> Create(Create{context.EntityName}Command command)");
            sb.AppendLine("        {");
            sb.AppendLine("            return await _mediator.Send(command);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Update(int id, Update{context.EntityName}Command command)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (id != command.Id) return BadRequest();");
            sb.AppendLine("            await _mediator.Send(command);");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpDelete(\"{id}\")]");
            sb.AppendLine("        public async Task<IActionResult> Delete(int id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _mediator.Send(new Delete{context.EntityName}Command {{ Id = id }});");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GenerateCommands(CodeGenerationContext context, CodeGenerationResult result)
        {
            // سيتم إضافة توليد Commands لاحقاً
        }

        private void GenerateQueries(CodeGenerationContext context, CodeGenerationResult result)
        {
            // سيتم إضافة توليد Queries لاحقاً
        }

        private void GenerateValidators(CodeGenerationContext context, CodeGenerationResult result)
        {
            // سيتم إضافة توليد Validators لاحقاً
        }

        private void GenerateMappers(CodeGenerationContext context, CodeGenerationResult result)
        {
            // سيتم إضافة توليد Mappers لاحقاً
        }

        private void GenerateTestProjects(CodeGenerationContext context, CodeGenerationResult result, string unitTestsPath, string integrationTestsPath)
        {
            // Generate Unit Tests
            var unitTestContent = GenerateUnitTests(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}Tests.cs",
                RelativePath = $"tests/{context.Namespace}.UnitTests/{context.EntityName}Tests.cs",
                FullPath = Path.Combine(unitTestsPath, $"{context.EntityName}Tests.cs"),
                Content = unitTestContent,
                FileType = "cs",
                Layer = "UnitTests"
            });

            // Generate Integration Tests
            var integrationTestContent = GenerateIntegrationTests(context);
            result.GeneratedFiles.Add(new GeneratedFile
            {
                FileName = $"{context.EntityName}IntegrationTests.cs",
                RelativePath = $"tests/{context.Namespace}.IntegrationTests/{context.EntityName}IntegrationTests.cs",
                FullPath = Path.Combine(integrationTestsPath, $"{context.EntityName}IntegrationTests.cs"),
                Content = integrationTestContent,
                FileType = "cs",
                Layer = "IntegrationTests"
            });
        }

        private string GenerateUnitTests(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Xunit;");
            sb.AppendLine("using Moq;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Application.Services;");
            sb.AppendLine();
            
            sb.AppendLine($"namespace {context.Namespace}.UnitTests");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Tests");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly Mock<I{context.EntityName}Repository> _mockRepository;");
            sb.AppendLine($"        private readonly I{context.EntityName}Service _service;");
            sb.AppendLine();
            
            sb.AppendLine($"        public {context.EntityName}Tests()");
            sb.AppendLine("        {");
            sb.AppendLine($"            _mockRepository = new Mock<I{context.EntityName}Repository>();");
            sb.AppendLine($"            _service = new {context.EntityName}Service(_mockRepository.Object);");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            sb.AppendLine("        [Fact]");
            sb.AppendLine("        public async Task GetById_ShouldReturnEntity_WhenEntityExists()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Arrange");
            sb.AppendLine($"            var entity = new {context.EntityName}();");
            sb.AppendLine("            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))");
            sb.AppendLine("                .ReturnsAsync(entity);");
            sb.AppendLine();
            sb.AppendLine("            // Act");
            sb.AppendLine("            var result = await _service.GetByIdAsync(1);");
            sb.AppendLine();
            sb.AppendLine("            // Assert");
            sb.AppendLine("            Assert.NotNull(result);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private string GenerateIntegrationTests(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Net;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc.Testing;");
            sb.AppendLine("using Xunit;");
            sb.AppendLine($"using {context.Namespace}.API;");
            sb.AppendLine();
            
            sb.AppendLine($"namespace {context.Namespace}.IntegrationTests");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}IntegrationTests : IClassFixture<WebApplicationFactory<Program>>");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly WebApplicationFactory<Program> _factory;");
            sb.AppendLine();
            
            sb.AppendLine($"        public {context.EntityName}IntegrationTests(WebApplicationFactory<Program> factory)");
            sb.AppendLine("        {");
            sb.AppendLine("            _factory = factory;");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            sb.AppendLine("        [Fact]");
            sb.AppendLine("        public async Task GetAll_ReturnsSuccessStatusCode()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Arrange");
            sb.AppendLine("            var client = _factory.CreateClient();");
            sb.AppendLine();
            sb.AppendLine("            // Act");
            sb.AppendLine($"            var response = await client.GetAsync(\"/api/{context.EntityName}\");");
            sb.AppendLine();
            sb.AppendLine("            // Assert");
            sb.AppendLine("            Assert.Equal(HttpStatusCode.OK, response.StatusCode);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            var files = new List<PreviewFile>();
            context.TableInfo = table;
            context.TableName = table.Name;
            context.EntityName = table.Name;

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}.cs",
                Content = GenerateEntity(context),
                Language = "csharp"
            });

            files.Add(new PreviewFile
            {
                FileName = $"I{context.EntityName}Repository.cs",
                Content = GenerateRepositoryInterface(context),
                Language = "csharp"
            });

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}Repository.cs",
                Content = GenerateRepositoryImplementation(context),
                Language = "csharp"
            });

            files.Add(new PreviewFile
            {
                FileName = $"{context.EntityName}Controller.cs",
                Content = GenerateController(context),
                Language = "csharp"
            });

            return files;
        }

        public override async Task GenerateInfrastructureLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            // توليد Repository Implementation
            var repoContent = GenerateRepositoryImplementation(context);
            var repoPath = Path.Combine(directoryPath, "Repositories", $"{context.EntityName}Repository.cs");
            await CreateFileAsync(repoPath, repoContent);

            // توليد DbContext Configuration
            var configContent = GenerateDbContextConfiguration(context);
            var configPath = Path.Combine(directoryPath, "Data", "Configurations", $"{context.EntityName}Configuration.cs");
            await CreateFileAsync(configPath, configContent);

            // توليد DbContext
            var dbContextContent = GenerateDbContext(context);
            var dbContextPath = Path.Combine(directoryPath, "Data", $"{context.Namespace}DbContext.cs");
            await CreateFileAsync(dbContextPath, dbContextContent);

            // توليد DI Registration
            var diContent = GenerateDIRegistration(context);
            var diPath = Path.Combine(directoryPath, "DependencyInjection", "DependencyInjection.cs");
            await CreateFileAsync(diPath, diContent);
        }

        public override async Task GenerateApplicationLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            // توليد DTOs
            var dtoContent = GenerateDTO(context);
            var dtoPath = Path.Combine(directoryPath, "DTOs", $"{context.EntityName}DTO.cs");
            await CreateFileAsync(dtoPath, dtoContent);

            // توليد Interfaces
            var interfaceContent = GenerateServiceInterface(context);
            var interfacePath = Path.Combine(directoryPath, "Interfaces", $"I{context.EntityName}Service.cs");
            await CreateFileAsync(interfacePath, interfaceContent);

            // توليد Services
            var serviceContent = GenerateService(context);
            var servicePath = Path.Combine(directoryPath, "Services", $"{context.EntityName}Service.cs");
            await CreateFileAsync(servicePath, serviceContent);

            // توليد Validators
            if (context.Options.GenerateValidators)
            {
                var validatorContent = GenerateValidator(context);
                var validatorPath = Path.Combine(directoryPath, "Validators", $"{context.EntityName}Validator.cs");
                await CreateFileAsync(validatorPath, validatorContent);
            }

            // توليد AutoMapper Profile
            var mapperContent = GenerateAutoMapperProfile(context);
            var mapperPath = Path.Combine(directoryPath, "Mappings", $"{context.EntityName}Profile.cs");
            await CreateFileAsync(mapperPath, mapperContent);

            // توليد CQRS
            var commandsPath = Path.Combine(directoryPath, "Commands", context.EntityName);
            var queriesPath = Path.Combine(directoryPath, "Queries", context.EntityName);
            await GenerateCommandsAndQueries(context, commandsPath, queriesPath);
        }

        public override async Task GenerateDomainLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            // توليد Entity
            var entityContent = GenerateEntity(context);
            var entityPath = Path.Combine(directoryPath, "Entities", $"{context.EntityName}.cs");
            await CreateFileAsync(entityPath, entityContent);

            // توليد Repository Interface
            var repoInterfaceContent = GenerateRepositoryInterface(context);
            var repoInterfacePath = Path.Combine(directoryPath, "Repositories", $"I{context.EntityName}Repository.cs");
            await CreateFileAsync(repoInterfacePath, repoInterfaceContent);

            // توليد Domain Events
            var eventContent = GenerateDomainEvent(context);
            var eventPath = Path.Combine(directoryPath, "Events", $"{context.EntityName}Event.cs");
            await CreateFileAsync(eventPath, eventContent);

            // توليد Domain Exceptions
            var exceptionContent = GenerateDomainException(context);
            var exceptionPath = Path.Combine(directoryPath, "Exceptions", $"{context.EntityName}Exception.cs");
            await CreateFileAsync(exceptionPath, exceptionContent);

            // توليد Value Objects
            var voContent = GenerateValueObjects(context);
            var voPath = Path.Combine(directoryPath, "ValueObjects", $"{context.EntityName}ValueObjects.cs");
            await CreateFileAsync(voPath, voContent);
        }

        public override async Task GeneratePresentationLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            // توليد Controller
            var controllerContent = GenerateController(context);
            var controllerPath = Path.Combine(directoryPath, "Controllers", $"{context.EntityName}Controller.cs");
            await CreateFileAsync(controllerPath, controllerContent);

            // توليد Program.cs
            var programContent = GenerateProgram(context);
            var programPath = Path.Combine(directoryPath, "Program.cs");
            await CreateFileAsync(programPath, programContent);

            // توليد appsettings.json
            var settingsContent = GenerateAppSettings(context);
            var settingsPath = Path.Combine(directoryPath, "appsettings.json");
            await CreateFileAsync(settingsPath, settingsContent);

            // توليد Startup.cs
            var startupContent = GenerateStartup(context);
            var startupPath = Path.Combine(directoryPath, "Startup.cs");
            await CreateFileAsync(startupPath, startupContent);

            if (context.Options.GeneratePresentationLayer)
            {
                // توليد Views
                var views = new[] { "Index", "Create", "Edit", "Details", "Delete" };
                foreach (var view in views)
                {
                    var viewContent = GenerateView(context, view);
                    var viewPath = Path.Combine(directoryPath, "Views", context.EntityName, $"{view}.cshtml");
                    await CreateFileAsync(viewPath, viewContent);
                }

                // توليد View Models
                var viewModelContent = GenerateViewModel(context);
                var viewModelPath = Path.Combine(directoryPath, "ViewModels", $"{context.EntityName}ViewModel.cs");
                await CreateFileAsync(viewModelPath, viewModelContent);
            }
        }

        private async Task CreateFileAsync(string path, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;

            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            await File.WriteAllTextAsync(path, content);
        }

        private string GenerateDIRegistration(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine($"using {context.Namespace}.Application.Interfaces;");
            sb.AppendLine($"using {context.Namespace}.Application.Services;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine($"using {context.Namespace}.Infrastructure.Repositories;");
            sb.AppendLine();
            sb.AppendLine("namespace Microsoft.Extensions.DependencyInjection");
            sb.AppendLine("{");
            sb.AppendLine("    public static class DependencyInjection");
            sb.AppendLine("    {");
            sb.AppendLine("        public static IServiceCollection AddInfrastructure(this IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine($"            services.AddScoped<I{context.EntityName}Repository, {context.EntityName}Repository>();");
            sb.AppendLine("            return services;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private async Task GenerateCommandsAndQueries(CodeGenerationContext context, string commandsPath, string queriesPath)
        {
            // Generate Commands
            var createCommandContent = GenerateCreateCommand(context);
            await CreateFileAsync(Path.Combine(commandsPath, $"Create{context.EntityName}Command.cs"), createCommandContent);

            var updateCommandContent = GenerateUpdateCommand(context);
            await CreateFileAsync(Path.Combine(commandsPath, $"Update{context.EntityName}Command.cs"), updateCommandContent);

            var deleteCommandContent = GenerateDeleteCommand(context);
            await CreateFileAsync(Path.Combine(commandsPath, $"Delete{context.EntityName}Command.cs"), deleteCommandContent);

            // Generate Queries
            var getByIdQueryContent = GenerateGetByIdQuery(context);
            await CreateFileAsync(Path.Combine(queriesPath, $"Get{context.EntityName}ByIdQuery.cs"), getByIdQueryContent);

            var getAllQueryContent = GenerateGetAllQuery(context);
            await CreateFileAsync(Path.Combine(queriesPath, $"GetAll{context.EntityName}Query.cs"), getAllQueryContent);
        }

        private string GenerateProgram(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Hosting;");
            sb.AppendLine();
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine();
            sb.AppendLine("// Add services to the container");
            sb.AppendLine("builder.Services.AddControllers();");
            sb.AppendLine("builder.Services.AddEndpointsApiExplorer();");
            sb.AppendLine("builder.Services.AddSwaggerGen();");
            sb.AppendLine("builder.Services.AddInfrastructure();");
            sb.AppendLine();
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine();
            sb.AppendLine("// Configure the HTTP request pipeline");
            sb.AppendLine("if (app.Environment.IsDevelopment())");
            sb.AppendLine("{");
            sb.AppendLine("    app.UseSwagger();");
            sb.AppendLine("    app.UseSwaggerUI();");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("app.UseHttpsRedirection();");
            sb.AppendLine("app.UseAuthorization();");
            sb.AppendLine("app.MapControllers();");
            sb.AppendLine();
            sb.AppendLine("app.Run();");
            return sb.ToString();
        }

        private string GenerateAppSettings(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"ConnectionStrings\": {");
            sb.AppendLine("    \"DefaultConnection\": \"YOUR_CONNECTION_STRING\"");
            sb.AppendLine("  },");
            sb.AppendLine("  \"Logging\": {");
            sb.AppendLine("    \"LogLevel\": {");
            sb.AppendLine("      \"Default\": \"Information\",");
            sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"AllowedHosts\": \"*\"");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateStartup(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Hosting;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.API");
            sb.AppendLine("{");
            sb.AppendLine("    public class Startup");
            sb.AppendLine("    {");
            sb.AppendLine("        public Startup(IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            Configuration = configuration;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public IConfiguration Configuration { get; }");
            sb.AppendLine();
            sb.AppendLine("        public void ConfigureServices(IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine("            services.AddControllers();");
            sb.AppendLine("            services.AddEndpointsApiExplorer();");
            sb.AppendLine("            services.AddSwaggerGen();");
            sb.AppendLine("            services.AddInfrastructure();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (env.IsDevelopment())");
            sb.AppendLine("            {");
            sb.AppendLine("                app.UseSwagger();");
            sb.AppendLine("                app.UseSwaggerUI();");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            app.UseHttpsRedirection();");
            sb.AppendLine("            app.UseRouting();");
            sb.AppendLine("            app.UseAuthorization();");
            sb.AppendLine("            app.UseEndpoints(endpoints =>");
            sb.AppendLine("            {");
            sb.AppendLine("                endpoints.MapControllers();");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateValueObjects(CodeGenerationContext context)
        {
            // Add implementation for generating value objects
            return string.Empty;
        }

        private string GenerateCreateCommand(CodeGenerationContext context)
        {
            // Add implementation for generating create command
            return string.Empty;
        }

        private string GenerateUpdateCommand(CodeGenerationContext context)
        {
            // Add implementation for generating update command
            return string.Empty;
        }

        private string GenerateDeleteCommand(CodeGenerationContext context)
        {
            // Add implementation for generating delete command
            return string.Empty;
        }

        private string GenerateGetByIdQuery(CodeGenerationContext context)
        {
            // Add implementation for generating get by id query
            return string.Empty;
        }

        private string GenerateGetAllQuery(CodeGenerationContext context)
        {
            // Add implementation for generating get all query
            return string.Empty;
        }

        private string GenerateDbContext(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Infrastructure.Data");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.Namespace}DbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public DbSet<{context.EntityName}> {context.EntityName}s {{ get; set; }}");
            sb.AppendLine();
            sb.AppendLine($"        public {context.Namespace}DbContext(DbContextOptions<{context.Namespace}DbContext> options)");
            sb.AppendLine("            : base(options)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnModelCreating(modelBuilder);");
            sb.AppendLine($"            modelBuilder.ApplyConfiguration(new {context.EntityName}Configuration());");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDTO(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Application.DTOs");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}DTO");
            sb.AppendLine("    {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine($"        public {column.CSharpType} {column.Name} {{ get; set; }}");
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateServiceInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {context.Namespace}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Application.Interfaces");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{context.EntityName}Service");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{context.EntityName}DTO> GetByIdAsync(int id);");
            sb.AppendLine($"        Task<List<{context.EntityName}DTO>> GetAllAsync();");
            sb.AppendLine($"        Task<{context.EntityName}DTO> CreateAsync({context.EntityName}DTO dto);");
            sb.AppendLine($"        Task UpdateAsync({context.EntityName}DTO dto);");
            sb.AppendLine("        Task DeleteAsync(int id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateService(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using {context.Namespace}.Application.DTOs;");
            sb.AppendLine($"using {context.Namespace}.Application.Interfaces;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine($"using {context.Namespace}.Domain.Repositories;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Application.Services");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Service : I{context.EntityName}Service");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{context.EntityName}Repository _repository;");
            sb.AppendLine("        private readonly IMapper _mapper;");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName}Service(I{context.EntityName}Repository repository, IMapper mapper)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("            _mapper = mapper;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{context.EntityName}DTO> GetByIdAsync(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            var entity = await _repository.GetByIdAsync(id);");
            sb.AppendLine("            return _mapper.Map<{context.EntityName}DTO>(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<List<{context.EntityName}DTO>> GetAllAsync()");
            sb.AppendLine("        {");
            sb.AppendLine("            var entities = await _repository.GetAllAsync();");
            sb.AppendLine($"            return _mapper.Map<List<{context.EntityName}DTO>>(entities);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{context.EntityName}DTO> CreateAsync({context.EntityName}DTO dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = _mapper.Map<{context.EntityName}>(dto);");
            sb.AppendLine("            var result = await _repository.AddAsync(entity);");
            sb.AppendLine("            return _mapper.Map<{context.EntityName}DTO>(result);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({context.EntityName}DTO dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = _mapper.Map<{context.EntityName}>(dto);");
            sb.AppendLine("            await _repository.UpdateAsync(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task DeleteAsync(int id)");
            sb.AppendLine("        {");
            sb.AppendLine("            await _repository.DeleteAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerateValidator(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using FluentValidation;");
            sb.AppendLine($"using {context.Namespace}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Application.Validators");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Validator : AbstractValidator<{context.EntityName}DTO>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {context.EntityName}Validator()");
            sb.AppendLine("        {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (!column.IsNullable)
                    {
                        sb.AppendLine($"            RuleFor(x => x.{column.Name}).NotEmpty();");
                    }
                    if (column.MaxLength.HasValue)
                    {
                        sb.AppendLine($"            RuleFor(x => x.{column.Name}).MaximumLength({column.MaxLength.Value});");
                    }
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateAutoMapperProfile(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using {context.Namespace}.Domain.Entities;");
            sb.AppendLine($"using {context.Namespace}.Application.DTOs;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Application.Mappings");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}Profile : Profile");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {context.EntityName}Profile()");
            sb.AppendLine("        {");
            sb.AppendLine($"            CreateMap<{context.EntityName}, {context.EntityName}DTO>().ReverseMap();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDomainEvent(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {context.Namespace}.Domain.Common;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Domain.Events");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}CreatedEvent : DomainEvent");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {context.EntityName}CreatedEvent({context.EntityName} item)");
            sb.AppendLine("        {");
            sb.AppendLine("            Item = item;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public {context.EntityName} Item {{ get; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateDomainException(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Domain.Exceptions");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}NotFoundException : Exception");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {context.EntityName}NotFoundException(int id)");
            sb.AppendLine($"            : base($\"{context.EntityName} with ID {{id}} was not found.\")");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateViewModel(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {context.Namespace}.Presentation.ViewModels");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {context.EntityName}ViewModel");
            sb.AppendLine("    {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (!column.IsNullable)
                    {
                        sb.AppendLine("        [Required]");
                    }
                    if (column.MaxLength.HasValue)
                    {
                        sb.AppendLine($"        [MaxLength({column.MaxLength.Value})]");
                    }
                    sb.AppendLine($"        public {column.CSharpType} {column.Name} {{ get; set; }}");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateView(CodeGenerationContext context, string viewName)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"@model {context.Namespace}.Presentation.ViewModels.{context.EntityName}ViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine($"    ViewData[\"Title\"] = \"{viewName} {context.EntityName}\";");
            sb.AppendLine("}");
            sb.AppendLine();

            switch (viewName)
            {
                case "Index":
                    GenerateIndexView(context, sb);
                    break;
                case "Create":
                case "Edit":
                    GenerateFormView(context, sb, viewName);
                    break;
                case "Details":
                    GenerateDetailsView(context, sb);
                    break;
                case "Delete":
                    GenerateDeleteView(context, sb);
                    break;
            }

            return sb.ToString();
        }

        private void GenerateIndexView(CodeGenerationContext context, StringBuilder sb)
        {
            sb.AppendLine("<p>");
            sb.AppendLine($"    <a asp-action=\"Create\" class=\"btn btn-primary\">Create New {context.EntityName}</a>");
            sb.AppendLine("</p>");
            sb.AppendLine("<table class=\"table\">");
            sb.AppendLine("    <thead>");
            sb.AppendLine("        <tr>");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine($"            <th>{column.Name}</th>");
                }
            }

            sb.AppendLine("            <th>Actions</th>");
            sb.AppendLine("        </tr>");
            sb.AppendLine("    </thead>");
            sb.AppendLine("    <tbody>");
            sb.AppendLine("    @foreach (var item in Model) {");
            sb.AppendLine("        <tr>");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine($"            <td>@item.{column.Name}</td>");
                }
            }

            sb.AppendLine("            <td>");
            sb.AppendLine("                <a asp-action=\"Edit\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-warning\">Edit</a> |");
            sb.AppendLine("                <a asp-action=\"Details\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-info\">Details</a> |");
            sb.AppendLine("                <a asp-action=\"Delete\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-danger\">Delete</a>");
            sb.AppendLine("            </td>");
            sb.AppendLine("        </tr>");
            sb.AppendLine("    }");
            sb.AppendLine("    </tbody>");
            sb.AppendLine("</table>");
        }

        private void GenerateFormView(CodeGenerationContext context, StringBuilder sb, string viewName)
        {
            sb.AppendLine("<div class=\"row\">");
            sb.AppendLine("    <div class=\"col-md-6\">");
            sb.AppendLine("        <form asp-action=\"" + viewName + "\">");
            sb.AppendLine("            <div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column.Name.ToLower() != "id")
                    {
                        sb.AppendLine("            <div class=\"form-group\">");
                        sb.AppendLine($"                <label asp-for=\"{column.Name}\" class=\"control-label\"></label>");
                        sb.AppendLine($"                <input asp-for=\"{column.Name}\" class=\"form-control\" />");
                        sb.AppendLine($"                <span asp-validation-for=\"{column.Name}\" class=\"text-danger\"></span>");
                        sb.AppendLine("            </div>");
                    }
                }
            }

            sb.AppendLine("            <div class=\"form-group\">");
            sb.AppendLine($"                <input type=\"submit\" value=\"{viewName}\" class=\"btn btn-primary\" />");
            sb.AppendLine("                <a asp-action=\"Index\" class=\"btn btn-secondary\">Back to List</a>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </form>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
        }

        private void GenerateDetailsView(CodeGenerationContext context, StringBuilder sb)
        {
            sb.AppendLine("<div>");
            sb.AppendLine("    <dl class=\"row\">");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine("        <dt class=\"col-sm-2\">");
                    sb.AppendLine($"            {column.Name}");
                    sb.AppendLine("        </dt>");
                    sb.AppendLine("        <dd class=\"col-sm-10\">");
                    sb.AppendLine($"            @Model.{column.Name}");
                    sb.AppendLine("        </dd>");
                }
            }

            sb.AppendLine("    </dl>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div>");
            sb.AppendLine("    <a asp-action=\"Edit\" asp-route-id=\"@Model.Id\" class=\"btn btn-warning\">Edit</a> |");
            sb.AppendLine("    <a asp-action=\"Index\" class=\"btn btn-secondary\">Back to List</a>");
            sb.AppendLine("</div>");
        }

        private void GenerateDeleteView(CodeGenerationContext context, StringBuilder sb)
        {
            sb.AppendLine("<h3>Are you sure you want to delete this?</h3>");
            sb.AppendLine("<div>");
            sb.AppendLine("    <dl class=\"row\">");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    sb.AppendLine("        <dt class=\"col-sm-2\">");
                    sb.AppendLine($"            {column.Name}");
                    sb.AppendLine("        </dt>");
                    sb.AppendLine("        <dd class=\"col-sm-10\">");
                    sb.AppendLine($"            @Model.{column.Name}");
                    sb.AppendLine("        </dd>");
                }
            }

            sb.AppendLine("    </dl>");
            sb.AppendLine("    <form asp-action=\"Delete\">");
            sb.AppendLine("        <input type=\"hidden\" asp-for=\"Id\" />");
            sb.AppendLine("        <input type=\"submit\" value=\"Delete\" class=\"btn btn-danger\" />");
            sb.AppendLine("        <a asp-action=\"Index\" class=\"btn btn-secondary\">Back to List</a>");
            sb.AppendLine("    </form>");
            sb.AppendLine("</div>");
        }
    }
}