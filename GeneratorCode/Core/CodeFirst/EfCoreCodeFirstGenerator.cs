using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorCode.Core.DomainModel;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Logging;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.CodeFirst
{
    public class EfCoreCodeFirstGenerator : ICodeFirstGenerator
    {
        private readonly ILogger _logger;
        private readonly TypeMappingService _typeMapper = new();

        public EfCoreCodeFirstGenerator(ILogger logger = null)
        {
            _logger = logger ?? LoggerFactory.Default;
        }

        public async Task<CodeGenerationResult> GenerateFullProjectAsync(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var result = new CodeGenerationResult { Success = true, GeneratedFiles = new List<GeneratedFile>() };
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInfo("Starting Code First project generation", "EfCoreCodeFirstGenerator");

                var entitiesResult = await GenerateEntitiesAsync(model, context);
                if (entitiesResult.GeneratedFiles != null)
                    result.GeneratedFiles.AddRange(entitiesResult.GeneratedFiles);

                var dbContext = await GenerateDbContextAsync(model, context);
                result.GeneratedFiles.Add(dbContext);

                var configs = await GenerateFluentConfigurationAsync(model, context);
                result.GeneratedFiles.AddRange(configs);

                var csproj = await GenerateProjectFileAsync(model, context);
                result.GeneratedFiles.Add(csproj);

                var programFile = GenerateProgramFile(model, context);
                result.GeneratedFiles.Add(programFile);

                var appSettings = GenerateAppSettings(model, context);
                result.GeneratedFiles.Add(appSettings);

                if (model.Entities.Any(e => e.UseBaseEntity))
                {
                    var baseEntity = GenerateBaseEntityClass(model, context);
                    result.GeneratedFiles.Add(baseEntity);
                }

                foreach (var entity in model.Entities.Where(e => e.SeedData?.Count > 0))
                {
                    var seedConfig = GenerateSeedConfiguration(entity, model, context);
                    result.GeneratedFiles.Add(seedConfig);
                }

                var snapshot = GenerateSnapshot(model);
                result.GeneratedFiles.Add(snapshot);

                await SaveFilesAsync(result.GeneratedFiles, context.OutputPath);

                sw.Stop();
                result.Message = $"تم توليد المشروع بنجاح ({result.GeneratedFiles.Count} ملف) في {sw.ElapsedMilliseconds}ms";
                result.GenerationTime = sw.Elapsed;
                _logger.LogInfo(result.Message, "EfCoreCodeFirstGenerator");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"فشل التوليد: {ex.Message}";
                result.Errors = new List<string> { ex.Message };
                _logger.LogError("Code First generation failed", ex, "EfCoreCodeFirstGenerator");
            }

            return result;
        }

        public Task<CodeGenerationResult> GenerateEntitiesAsync(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var result = new CodeGenerationResult { Success = true, GeneratedFiles = new List<GeneratedFile>() };

            foreach (var entity in model.Entities)
            {
                var content = GenerateEntityClass(entity, model, context);
                result.GeneratedFiles.Add(new GeneratedFile
                {
                    FileName = $"{entity.Name}.cs",
                    RelativePath = Path.Combine("Entities", $"{entity.Name}.cs"),
                    Content = content,
                    FileType = "Entity",
                    Layer = "Domain"
                });
            }

            return Task.FromResult(result);
        }

        public Task<GeneratedFile> GenerateDbContextAsync(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var ns = context.Namespace ?? model.DefaultNamespace;
            var sb = new StringBuilder();

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {ns}.Entities;");
            sb.AppendLine($"using {ns}.Configurations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Data");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {model.ProjectName}DbContext : DbContext");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {model.ProjectName}DbContext(DbContextOptions<{model.ProjectName}DbContext> options)");
            sb.AppendLine("            : base(options) { }");
            sb.AppendLine();

            foreach (var entity in model.Entities)
            {
                sb.AppendLine($"        public DbSet<{entity.Name}> {entity.TableName ?? entity.Name}s {{ get; set; }}");
            }

            sb.AppendLine();
            sb.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            base.OnModelCreating(modelBuilder);");
            sb.AppendLine();

            foreach (var entity in model.Entities)
            {
                sb.AppendLine($"            modelBuilder.ApplyConfiguration(new {entity.Name}Configuration());");
            }

            sb.AppendLine("        }");

            sb.AppendLine();
            sb.AppendLine("        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!optionsBuilder.IsConfigured)");
            sb.AppendLine("            {");

            var configMethod = GetProviderConfigurationMethod(model.TargetDatabaseType);
            sb.AppendLine($"                optionsBuilder.{configMethod};");

            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return Task.FromResult(new GeneratedFile
            {
                FileName = $"{model.ProjectName}DbContext.cs",
                RelativePath = Path.Combine("Data", $"{model.ProjectName}DbContext.cs"),
                Content = sb.ToString(),
                FileType = "DbContext",
                Layer = "Infrastructure"
            });
        }

        public Task<List<GeneratedFile>> GenerateFluentConfigurationAsync(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var files = new List<GeneratedFile>();
            var ns = context.Namespace ?? model.DefaultNamespace;

            foreach (var entity in model.Entities)
            {
                var sb = new StringBuilder();
                sb.AppendLine("using Microsoft.EntityFrameworkCore;");
                sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
                sb.AppendLine($"using {ns}.Entities;");
                sb.AppendLine();
                sb.AppendLine($"namespace {ns}.Configurations");
                sb.AppendLine("{");
                sb.AppendLine($"    public class {entity.Name}Configuration : IEntityTypeConfiguration<{entity.Name}>");
                sb.AppendLine("    {");
                sb.AppendLine($"        public void Configure(EntityTypeBuilder<{entity.Name}> builder)");
                sb.AppendLine("        {");

                var tableName = entity.TableName ?? entity.Name;
                var schema = entity.Schema ?? "dbo";
                if (model.TargetDatabaseType == DatabaseType.SqlServer)
                    sb.AppendLine($"            builder.ToTable(\"{tableName}\", \"{schema}\");");
                else
                    sb.AppendLine($"            builder.ToTable(\"{tableName}\");");

                sb.AppendLine();

                var pks = entity.Properties.Where(p => p.IsPrimaryKey).ToList();
                if (pks.Count == 1)
                {
                    sb.AppendLine($"            builder.HasKey(e => e.{pks[0].Name});");
                }
                else if (pks.Count > 1)
                {
                    var pkNames = string.Join(", ", pks.Select(p => $"e.{p.Name}"));
                    sb.AppendLine($"            builder.HasKey(e => new {{ {pkNames} }});");
                }

                sb.AppendLine();

                foreach (var prop in entity.Properties.OrderBy(p => p.Order))
                {
                    sb.Append($"            builder.Property(e => e.{prop.Name})");

                    if (prop.IsRequired && prop.Type == DomainPropertyType.String)
                        sb.Append(".IsRequired()");

                    if (prop.MaxLength.HasValue && prop.Type == DomainPropertyType.String)
                        sb.Append($".HasMaxLength({prop.MaxLength})");

                    if (prop.Type == DomainPropertyType.Decimal)
                    {
                        var p = prop.Precision ?? 18;
                        var s = prop.Scale ?? 2;
                        sb.Append($".HasPrecision({p}, {s})");
                    }

                    var dbColType = TypeMappingService.MapToDatabaseType(prop.Type, model.TargetDatabaseType, prop.MaxLength, prop.Precision, prop.Scale);
                    sb.Append($".HasColumnType(\"{dbColType}\")");

                    if (!string.IsNullOrWhiteSpace(prop.ColumnName) && prop.ColumnName != prop.Name)
                        sb.Append($".HasColumnName(\"{prop.ColumnName}\")");

                    if (!string.IsNullOrWhiteSpace(prop.DefaultValue))
                        sb.Append($".HasDefaultValueSql(\"{prop.DefaultValue}\")");

                    if (prop.IsIdentity)
                        sb.Append(".ValueGeneratedOnAdd()");

                    sb.AppendLine(";");
                }

                if (entity.Relations != null && entity.Relations.Count > 0)
                {
                    sb.AppendLine();
                    foreach (var rel in entity.Relations)
                    {
                        GenerateRelationConfiguration(sb, entity, rel);
                    }
                }

                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                files.Add(new GeneratedFile
                {
                    FileName = $"{entity.Name}Configuration.cs",
                    RelativePath = Path.Combine("Configurations", $"{entity.Name}Configuration.cs"),
                    Content = sb.ToString(),
                    FileType = "Configuration",
                    Layer = "Infrastructure"
                });
            }

            return Task.FromResult(files);
        }

        public Task<GeneratedFile> GenerateProjectFileAsync(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            var targetFramework = model.TargetFramework ?? context.TargetFramework ?? "net8.0";

            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine($"    <TargetFramework>{targetFramework}</TargetFramework>");
            sb.AppendLine("    <Nullable>enable</Nullable>");
            sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"{GetEfCoreVersion(targetFramework)}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"{GetEfCoreVersion(targetFramework)}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Design\" Version=\"{GetEfCoreVersion(targetFramework)}\" />");

            var providerPackage = GetProviderPackageName(model.TargetDatabaseType);
            var providerVersion = GetProviderVersion(model.TargetDatabaseType, targetFramework);
            sb.AppendLine($"    <PackageReference Include=\"{providerPackage}\" Version=\"{providerVersion}\" />");

            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");

            return Task.FromResult(new GeneratedFile
            {
                FileName = $"{model.ProjectName}.csproj",
                RelativePath = $"{model.ProjectName}.csproj",
                Content = sb.ToString(),
                FileType = "Project",
                Layer = "Root"
            });
        }

        private string GenerateEntityClass(DomainEntity entity, DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var ns = context.Namespace ?? model.DefaultNamespace;
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Entities");
            sb.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(entity.Description))
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {entity.Description}");
                sb.AppendLine($"    /// </summary>");
            }

            sb.AppendLine($"    [Table(\"{entity.TableName ?? entity.Name}\")]");
            var inheritance = entity.UseBaseEntity ? " : BaseEntity" : "";
            sb.AppendLine($"    public class {entity.Name}{inheritance}");
            sb.AppendLine("    {");

            foreach (var prop in entity.Properties.OrderBy(p => p.Order))
            {
                var attrs = new List<string>();
                if (prop.IsPrimaryKey) attrs.Add("[Key]");
                if (prop.IsIdentity) attrs.Add("[DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                if (prop.IsRequired && prop.Type == DomainPropertyType.String) attrs.Add("[Required]");
                if (prop.MaxLength.HasValue && prop.Type == DomainPropertyType.String) attrs.Add($"[MaxLength({prop.MaxLength})]");

                if (prop.Type == DomainPropertyType.Decimal || prop.Type == DomainPropertyType.DateTime
                    || prop.Type == DomainPropertyType.DateOnly || prop.Type == DomainPropertyType.TimeOnly
                    || prop.Type == DomainPropertyType.Guid)
                {
                    var colType = TypeMappingService.MapToDatabaseType(prop.Type, model.TargetDatabaseType, prop.MaxLength, prop.Precision, prop.Scale);
                    attrs.Add($"[Column(TypeName = \"{colType}\")]");
                }

                if (!string.IsNullOrWhiteSpace(prop.ColumnName) && prop.ColumnName != prop.Name)
                    attrs.Add($"[Column(\"{prop.ColumnName}\")]");

                foreach (var attr in attrs)
                    sb.AppendLine($"        {attr}");

                var isNullable = !prop.IsRequired && prop.Type != DomainPropertyType.String && prop.Type != DomainPropertyType.ByteArray;
                var csharpType = TypeMappingService.MapToCSharpType(prop.Type, isNullable);

                if (!string.IsNullOrWhiteSpace(prop.Description))
                    sb.AppendLine($"        /// <summary>{prop.Description}</summary>");

                sb.AppendLine($"        public {csharpType} {prop.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            if (entity.Relations != null)
            {
                foreach (var rel in entity.Relations)
                {
                    var navName = string.IsNullOrWhiteSpace(rel.NavigationPropertyName)
                        ? (rel.RelationType == RelationType.OneToMany || rel.RelationType == RelationType.ManyToMany
                            ? rel.TargetEntity + "s"
                            : rel.TargetEntity)
                        : rel.NavigationPropertyName;

                    if (rel.RelationType == RelationType.OneToMany || rel.RelationType == RelationType.ManyToMany)
                    {
                        sb.AppendLine($"        public virtual ICollection<{rel.TargetEntity}> {navName} {{ get; set; }} = new HashSet<{rel.TargetEntity}>();");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rel.ForeignKeyName))
                            sb.AppendLine($"        public {TypeMappingService.MapToCSharpType(DomainPropertyType.Int, true)} {rel.ForeignKeyName} {{ get; set; }}");
                        sb.AppendLine($"        public virtual {rel.TargetEntity} {navName} {{ get; set; }}");
                    }
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GenerateRelationConfiguration(StringBuilder sb, DomainEntity entity, DomainRelation rel)
        {
            switch (rel.RelationType)
            {
                case RelationType.OneToOne:
                    sb.AppendLine($"            builder.HasOne(e => e.{rel.NavigationPropertyName ?? rel.TargetEntity})");
                    if (!string.IsNullOrWhiteSpace(rel.InverseNavigationPropertyName))
                        sb.AppendLine($"                .WithOne(e => e.{rel.InverseNavigationPropertyName})");
                    else
                        sb.AppendLine("                .WithOne()");
                    if (!string.IsNullOrWhiteSpace(rel.ForeignKeyName))
                        sb.AppendLine($"                .HasForeignKey<{entity.Name}>(e => e.{rel.ForeignKeyName})");
                    sb.AppendLine($"                .OnDelete({MapDeleteBehavior(rel.DeleteBehavior)});");
                    break;

                case RelationType.OneToMany:
                    sb.AppendLine($"            builder.HasMany(e => e.{rel.NavigationPropertyName ?? rel.TargetEntity + "s"})");
                    if (!string.IsNullOrWhiteSpace(rel.InverseNavigationPropertyName))
                        sb.AppendLine($"                .WithOne(e => e.{rel.InverseNavigationPropertyName})");
                    else
                        sb.AppendLine("                .WithOne()");
                    if (!string.IsNullOrWhiteSpace(rel.ForeignKeyName))
                        sb.AppendLine($"                .HasForeignKey(e => e.{rel.ForeignKeyName})");
                    sb.AppendLine($"                .OnDelete({MapDeleteBehavior(rel.DeleteBehavior)});");
                    break;

                case RelationType.ManyToMany:
                    sb.AppendLine($"            builder.HasMany(e => e.{rel.NavigationPropertyName ?? rel.TargetEntity + "s"})");
                    sb.AppendLine($"                .WithMany()");
                    sb.AppendLine($"                .UsingEntity(\"{entity.Name}{rel.TargetEntity}\");");
                    break;
            }
            sb.AppendLine();
        }

        private GeneratedFile GenerateProgramFile(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var ns = context.Namespace ?? model.DefaultNamespace;
            var sb = new StringBuilder();

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine($"using {ns}.Data;");
            sb.AppendLine();
            sb.AppendLine("var builder = WebApplication.CreateBuilder(args);");
            sb.AppendLine();
            sb.AppendLine($"builder.Services.AddDbContext<{model.ProjectName}DbContext>(options =>");

            var connPlaceholder = "builder.Configuration.GetConnectionString(\"DefaultConnection\")";
            switch (model.TargetDatabaseType)
            {
                case DatabaseType.SqlServer:
                    sb.AppendLine($"    options.UseSqlServer({connPlaceholder}));");
                    break;
                case DatabaseType.MySql:
                    sb.AppendLine($"    options.UseMySql({connPlaceholder}, ServerVersion.AutoDetect({connPlaceholder})));");
                    break;
                case DatabaseType.PostgreSql:
                    sb.AppendLine($"    options.UseNpgsql({connPlaceholder}));");
                    break;
                case DatabaseType.Oracle:
                    sb.AppendLine($"    options.UseOracle({connPlaceholder}));");
                    break;
                case DatabaseType.SQLite:
                    sb.AppendLine($"    options.UseSqlite({connPlaceholder}));");
                    break;
            }

            sb.AppendLine();
            sb.AppendLine("var app = builder.Build();");
            sb.AppendLine();
            sb.AppendLine("app.Run();");

            return new GeneratedFile
            {
                FileName = "Program.cs",
                RelativePath = "Program.cs",
                Content = sb.ToString(),
                FileType = "Program",
                Layer = "Root"
            };
        }

        private GeneratedFile GenerateAppSettings(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var connStr = !string.IsNullOrWhiteSpace(model.ConnectionString)
                ? model.ConnectionString
                : GetDefaultConnectionString(model.TargetDatabaseType);

            var content = $@"{{
  ""ConnectionStrings"": {{
    ""DefaultConnection"": ""{connStr}""
  }},
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }},
  ""AllowedHosts"": ""*""
}}";

            return new GeneratedFile
            {
                FileName = "appsettings.json",
                RelativePath = "appsettings.json",
                Content = content,
                FileType = "Configuration",
                Layer = "Root"
            };
        }

        private GeneratedFile GenerateBaseEntityClass(DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var ns = context.Namespace ?? model.DefaultNamespace;
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Entities");
            sb.AppendLine("{");
            sb.AppendLine("    public abstract class BaseEntity");
            sb.AppendLine("    {");
            sb.AppendLine("        public int Id { get; set; }");
            sb.AppendLine("        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;");
            sb.AppendLine("        public DateTime? UpdatedAt { get; set; }");
            sb.AppendLine("        public bool IsDeleted { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new GeneratedFile
            {
                FileName = "BaseEntity.cs",
                RelativePath = Path.Combine("Entities", "BaseEntity.cs"),
                Content = sb.ToString(),
                FileType = "Entity",
                Layer = "Domain"
            };
        }

        private GeneratedFile GenerateSeedConfiguration(DomainEntity entity, DomainModel.DomainModel model, CodeGenerationContext context)
        {
            var ns = context.Namespace ?? model.DefaultNamespace;
            var sb = new StringBuilder();

            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine($"using {ns}.Entities;");
            sb.AppendLine();
            sb.AppendLine($"namespace {ns}.Configurations");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {entity.Name}SeedConfiguration : IEntityTypeConfiguration<{entity.Name}>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public void Configure(EntityTypeBuilder<{entity.Name}> builder)");
            sb.AppendLine("        {");
            sb.AppendLine("            builder.HasData(");

            for (int i = 0; i < entity.SeedData.Count; i++)
            {
                var row = entity.SeedData[i];
                var props = new List<string>();
                foreach (var kvp in row)
                {
                    var value = kvp.Value;
                    if (value is string s)
                        props.Add($"{kvp.Key} = \"{s}\"");
                    else if (value is bool b)
                        props.Add($"{kvp.Key} = {(b ? "true" : "false")}");
                    else
                        props.Add($"{kvp.Key} = {value}");
                }

                var separator = i < entity.SeedData.Count - 1 ? "," : "";
                sb.AppendLine($"                new {entity.Name} {{ {string.Join(", ", props)} }}{separator}");
            }

            sb.AppendLine("            );");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new GeneratedFile
            {
                FileName = $"{entity.Name}SeedConfiguration.cs",
                RelativePath = Path.Combine("Configurations", $"{entity.Name}SeedConfiguration.cs"),
                Content = sb.ToString(),
                FileType = "SeedConfiguration",
                Layer = "Infrastructure"
            };
        }

        private GeneratedFile GenerateSnapshot(DomainModel.DomainModel model)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(model, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            });

            return new GeneratedFile
            {
                FileName = $"snapshot_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json",
                RelativePath = Path.Combine("Snapshots", $"snapshot_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json"),
                Content = json,
                FileType = "Snapshot",
                Layer = "Root"
            };
        }

        private static async Task SaveFilesAsync(List<GeneratedFile> files, string outputPath)
        {
            foreach (var file in files)
            {
                var fullPath = Path.Combine(outputPath, file.RelativePath);
                file.FullPath = fullPath;

                var dir = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(fullPath, file.Content, Encoding.UTF8);
                file.SizeInBytes = Encoding.UTF8.GetByteCount(file.Content);
            }
        }

        private static string GetProviderConfigurationMethod(DatabaseType dbType)
        {
            return dbType switch
            {
                DatabaseType.SqlServer => "UseSqlServer(\"Server=(localdb)\\\\mssqllocaldb;Database=MyDb;Trusted_Connection=true;\")",
                DatabaseType.MySql => "UseMySql(\"Server=localhost;Database=MyDb;Uid=root;Pwd=;\", ServerVersion.AutoDetect(\"Server=localhost;Database=MyDb;Uid=root;Pwd=;\"))",
                DatabaseType.PostgreSql => "UseNpgsql(\"Host=localhost;Database=MyDb;Username=postgres;Password=postgres\")",
                DatabaseType.Oracle => "UseOracle(\"Data Source=localhost:1521/XEPDB1;User Id=system;Password=oracle;\")",
                DatabaseType.SQLite => "UseSqlite(\"Data Source=app.db\")",
                _ => "UseSqlServer(\"Server=(localdb)\\\\mssqllocaldb;Database=MyDb;Trusted_Connection=true;\")"
            };
        }

        private static string GetProviderPackageName(DatabaseType dbType)
        {
            return dbType switch
            {
                DatabaseType.SqlServer => "Microsoft.EntityFrameworkCore.SqlServer",
                DatabaseType.MySql => "Pomelo.EntityFrameworkCore.MySql",
                DatabaseType.PostgreSql => "Npgsql.EntityFrameworkCore.PostgreSQL",
                DatabaseType.Oracle => "Oracle.EntityFrameworkCore",
                DatabaseType.SQLite => "Microsoft.EntityFrameworkCore.Sqlite",
                _ => "Microsoft.EntityFrameworkCore.SqlServer"
            };
        }

        private static string GetEfCoreVersion(string targetFramework)
        {
            return targetFramework switch
            {
                "net6.0" => "6.0.36",
                "net7.0" => "7.0.20",
                "net8.0" => "8.0.12",
                "net9.0" => "9.0.2",
                _ => "8.0.12"
            };
        }

        private static string GetProviderVersion(DatabaseType dbType, string targetFramework)
        {
            return (dbType, targetFramework) switch
            {
                (DatabaseType.SqlServer, _) => GetEfCoreVersion(targetFramework),
                (DatabaseType.MySql, "net6.0") => "6.0.3",
                (DatabaseType.MySql, "net7.0") => "7.0.0",
                (DatabaseType.MySql, "net8.0") => "8.0.2",
                (DatabaseType.MySql, "net9.0") => "9.0.0",
                (DatabaseType.PostgreSql, "net6.0") => "6.0.29",
                (DatabaseType.PostgreSql, "net7.0") => "7.0.18",
                (DatabaseType.PostgreSql, "net8.0") => "8.0.11",
                (DatabaseType.PostgreSql, "net9.0") => "9.0.3",
                (DatabaseType.Oracle, "net6.0") => "6.21.150",
                (DatabaseType.Oracle, "net7.0") => "7.21.13",
                (DatabaseType.Oracle, "net8.0") => "8.23.60",
                (DatabaseType.Oracle, "net9.0") => "9.23.60",
                (DatabaseType.SQLite, _) => GetEfCoreVersion(targetFramework),
                _ => GetEfCoreVersion(targetFramework)
            };
        }

        private static string GetDefaultConnectionString(DatabaseType dbType)
        {
            return dbType switch
            {
                DatabaseType.SqlServer => "Server=(localdb)\\\\mssqllocaldb;Database=MyDb;Trusted_Connection=true;",
                DatabaseType.MySql => "Server=localhost;Database=MyDb;Uid=root;Pwd=;",
                DatabaseType.PostgreSql => "Host=localhost;Database=MyDb;Username=postgres;Password=postgres",
                DatabaseType.Oracle => "Data Source=localhost:1521/XEPDB1;User Id=system;Password=oracle;",
                DatabaseType.SQLite => "Data Source=app.db",
                _ => "Server=(localdb)\\\\mssqllocaldb;Database=MyDb;Trusted_Connection=true;"
            };
        }

        private static string MapDeleteBehavior(DeleteBehavior behavior)
        {
            return behavior switch
            {
                DeleteBehavior.Cascade => "DeleteBehavior.Cascade",
                DeleteBehavior.Restrict => "DeleteBehavior.Restrict",
                DeleteBehavior.SetNull => "DeleteBehavior.SetNull",
                DeleteBehavior.NoAction => "DeleteBehavior.NoAction",
                _ => "DeleteBehavior.Cascade"
            };
        }
    }
}
