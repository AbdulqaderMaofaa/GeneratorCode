using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    /// <summary>
    /// Layered Architecture: Data Access Layer, Business Logic Layer, Presentation/Service Layer.
    /// </summary>
    public class LayeredArchitecturePattern : BaseArchitecturePattern
    {
        public override string Name => "Layered Architecture";
        public override string Description => "3-layer architecture: Data Access, Business Logic, Presentation/Service";

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
                var tableName = context.TableName ?? context.TableInfo.Name ?? "Table";
                var outputPath = context.OutputPath ?? ".";

                // 1. Models/{EntityName}.cs
                if (options.GenerateModels)
                {
                    var modelContent = GenerateEntityModel(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Models", $"{entityName}.cs", modelContent, "Models"));
                }

                // 2. DataAccess/I{EntityName}Repository.cs
                if (options.GenerateRepositories)
                {
                    var repoInterfaceContent = GenerateRepositoryInterface(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "DataAccess", $"I{entityName}Repository.cs", repoInterfaceContent, "DataAccess"));

                    // 3. DataAccess/{EntityName}Repository.cs
                    var repoImplContent = GenerateRepositoryImplementation(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "DataAccess", $"{entityName}Repository.cs", repoImplContent, "DataAccess"));

                    // 4. DataAccess/{Namespace}DbContext.cs
                    var dbContextContent = GenerateDbContext(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "DataAccess", $"{ns}DbContext.cs", dbContextContent, "DataAccess"));
                }

                // 5. BusinessLogic/I{EntityName}Service.cs & 6. BusinessLogic/{EntityName}Service.cs
                if (options.GenerateServices)
                {
                    var serviceInterfaceContent = GenerateServiceInterface(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "BusinessLogic", $"I{entityName}Service.cs", serviceInterfaceContent, "BusinessLogic"));

                    var serviceImplContent = GenerateServiceImplementation(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "BusinessLogic", $"{entityName}Service.cs", serviceImplContent, "BusinessLogic"));
                }

                // 7. Presentation/{EntityName}Controller.cs
                if (options.GenerateControllers)
                {
                    var controllerContent = GenerateApiController(context);
                    result.GeneratedFiles.Add(CreateGeneratedFile(outputPath, "Presentation", $"{entityName}Controller.cs", controllerContent, "Presentation"));
                }

                result.Success = true;
                result.Message = "Layered architecture generated successfully.";
                result.TotalSizeInBytes = result.GeneratedFiles.Sum(f => (long)(f.Content?.Length ?? 0));
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

        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;

        public override List<string> GetRequiredLayers() => new() { "DataAccess", "BusinessLogic", "Presentation" };

        public override List<string> GetRequiredDependencies() => new()
        {
            "Microsoft.EntityFrameworkCore",
            "Microsoft.EntityFrameworkCore.SqlServer",
            "Microsoft.AspNetCore.Mvc.Core",
            "System.ComponentModel.DataAnnotations"
        };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            if (context == null) context = new CodeGenerationContext();
            context.TableInfo = table;
            context.TableName = table?.Name ?? "Table";
            context.EntityName = context.EntityName ?? table?.Name ?? "Entity";
            context.Namespace = context.Namespace ?? "GeneratedCode";

            var previews = new List<PreviewFile>
            {
                new() { FileName = $"{context.EntityName}.cs", Content = GenerateEntityModel(context), Language = "csharp" },
                new() { FileName = $"I{context.EntityName}Repository.cs", Content = GenerateRepositoryInterface(context), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Repository.cs", Content = GenerateRepositoryImplementation(context), Language = "csharp" },
                new() { FileName = $"{context.Namespace}DbContext.cs", Content = GenerateDbContext(context), Language = "csharp" },
                new() { FileName = $"I{context.EntityName}Service.cs", Content = GenerateServiceInterface(context), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Service.cs", Content = GenerateServiceImplementation(context), Language = "csharp" },
                new() { FileName = $"{context.EntityName}Controller.cs", Content = GenerateApiController(context), Language = "csharp" }
            };
            return previews;
        }

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
                SizeInBytes = content?.Length ?? 0
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

        private string GenerateEntityModel(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";

            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Models");
            sb.AppendLine("{");
            sb.AppendLine($"    [Table(\"{context.TableName ?? "Table"}\")]");
            sb.AppendLine($"    public class {entityName}");
            sb.AppendLine("    {");

            if (context.TableInfo?.Columns != null)
            {
                foreach (var column in context.TableInfo.Columns)
                {
                    if (column.IsPrimaryKey)
                        sb.AppendLine("        [Key]");
                    if (column.IsAutoIncrement)
                        sb.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                    if (!column.IsNullable && (column.CSharpType == "string" || column.CSharpType == "String"))
                        sb.AppendLine("        [Required]");
                    if (column.MaxLength.HasValue && (column.CSharpType == "string" || column.CSharpType == "String"))
                        sb.AppendLine($"        [StringLength({column.MaxLength.Value})]");
                    if (!string.IsNullOrEmpty(column.Description))
                        sb.AppendLine($"        [Display(Name = \"{column.Description}\")]");

                    var csharpType = column.CSharpType ?? "object";
                    var nullable = column.IsNullable && csharpType != "string" && csharpType != "String" ? "?" : "";
                    sb.AppendLine($"        public {csharpType}{nullable} {column.Name} {{ get; set; }}");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateRepositoryInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.DataAccess");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}> GetByIdAsync({pkType} id);");
            sb.AppendLine($"        Task<IEnumerable<{entityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}> AddAsync({entityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({entityName} entity);");
            sb.AppendLine($"        Task DeleteAsync({pkType} id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateRepositoryImplementation(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.DataAccess");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Repository : I{entityName}Repository");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly {ns}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Repository({ns}DbContext context)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = context;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> GetByIdAsync({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _context.Set<{entityName}>().FindAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IEnumerable<{entityName}>> GetAllAsync()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _context.Set<{entityName}>().ToListAsync();");
            sb.AppendLine("        }");
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
            sb.AppendLine($"        public async Task DeleteAsync({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var entity = await GetByIdAsync(id);");
            sb.AppendLine("            if (entity != null)");
            sb.AppendLine("            {");
            sb.AppendLine($"                _context.Set<{entityName}>().Remove(entity);");
            sb.AppendLine("                await _context.SaveChangesAsync();");
            sb.AppendLine("            }");
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
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.DataAccess");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {ns}DbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public DbSet<{entityName}> {entityName}s {{ get; set; }}");
            sb.AppendLine();
            sb.AppendLine($"        public {ns}DbContext(DbContextOptions<{ns}DbContext> options)");
            sb.AppendLine("            : base(options)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnModelCreating(modelBuilder);");
            sb.AppendLine($"            modelBuilder.Entity<{entityName}>().ToTable(\"{tableName}\");");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateServiceInterface(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.BusinessLogic");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface I{entityName}Service");
            sb.AppendLine("    {");
            sb.AppendLine($"        Task<{entityName}> GetByIdAsync({pkType} id);");
            sb.AppendLine($"        Task<IEnumerable<{entityName}>> GetAllAsync();");
            sb.AppendLine($"        Task<{entityName}> CreateAsync({entityName} entity);");
            sb.AppendLine($"        Task UpdateAsync({entityName} entity);");
            sb.AppendLine($"        Task DeleteAsync({pkType} id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateServiceImplementation(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine($"using {ns}.DataAccess;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.BusinessLogic");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entityName}Service : I{entityName}Service");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Repository _repository;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Service(I{entityName}Repository repository)");
            sb.AppendLine("        {");
            sb.AppendLine("            _repository = repository;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> GetByIdAsync({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _repository.GetByIdAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<IEnumerable<{entityName}>> GetAllAsync()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _repository.GetAllAsync();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task<{entityName}> CreateAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return await _repository.AddAsync(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task UpdateAsync({entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _repository.UpdateAsync(entity);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public async Task DeleteAsync({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _repository.DeleteAsync(id);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateApiController(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var ns = context.Namespace ?? "GeneratedCode";
            var entityName = context.EntityName ?? "Entity";
            var pkType = GetPrimaryKeyCSharpType(context);

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine($"using {ns}.Models;");
            sb.AppendLine($"using {ns}.BusinessLogic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Presentation");
            sb.AppendLine("{");
            sb.AppendLine("    [ApiController]");
            sb.AppendLine("    [Route(\"api/[controller]\")]");
            sb.AppendLine($"    public class {entityName}Controller : ControllerBase");
            sb.AppendLine("    {");
            sb.AppendLine($"        private readonly I{entityName}Service _service;");
            sb.AppendLine();
            sb.AppendLine($"        public {entityName}Controller(I{entityName}Service service)");
            sb.AppendLine("        {");
            sb.AppendLine("            _service = service;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine($"        public async Task<ActionResult<IEnumerable<{entityName}>>> GetAll()");
            sb.AppendLine("        {");
            sb.AppendLine($"            var items = await _service.GetAllAsync();");
            sb.AppendLine("            return Ok(items);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        [HttpGet(\"{{id}}\")]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}>> GetById({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var item = await _service.GetByIdAsync(id);");
            sb.AppendLine("            if (item == null) return NotFound();");
            sb.AppendLine("            return Ok(item);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine($"        public async Task<ActionResult<{entityName}>> Create([FromBody] {entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var created = await _service.CreateAsync(entity);");
            var pkName = GetPrimaryKeyName(context);
            sb.AppendLine($"            return CreatedAtAction(nameof(GetById), new {{ id = created.{pkName} }}, created);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPut(\"{id}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Update({pkType} id, [FromBody] {entityName} entity)");
            sb.AppendLine("        {");
            sb.AppendLine($"            if (entity.{pkName} != id) return BadRequest();");
            sb.AppendLine($"            await _service.UpdateAsync(entity);");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        [HttpDelete(\"{{id}}\")]");
            sb.AppendLine($"        public async Task<IActionResult> Delete({pkType} id)");
            sb.AppendLine("        {");
            sb.AppendLine($"            await _service.DeleteAsync(id);");
            sb.AppendLine("            return NoContent();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
