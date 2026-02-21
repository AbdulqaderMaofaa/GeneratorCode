using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    public class PackagesGenerator
    {
        private readonly Dictionary<string, string> _packageVersions;

        public PackagesGenerator()
        {
            _packageVersions = LoadPackageVersions();
        }

        private static Dictionary<string, string> LoadPackageVersions()
        {
            var defaults = GetDefaultPackageVersions();

            try
            {
                var configPath = Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "package-versions.json");

                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    var loaded = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (loaded != null)
                    {
                        foreach (var kvp in loaded)
                            defaults[kvp.Key] = kvp.Value;
                    }
                }
            }
            catch { }

            return defaults;
        }

        private static Dictionary<string, string> GetDefaultPackageVersions()
        {
            return new Dictionary<string, string>
            {
                ["AutoMapper"] = "13.0.1",
                ["FluentValidation"] = "11.9.0",
                ["MediatR"] = "12.2.0",
                ["Microsoft.EntityFrameworkCore"] = "8.0.0",
                ["Microsoft.EntityFrameworkCore.Tools"] = "8.0.0",
                ["Microsoft.EntityFrameworkCore.SqlServer"] = "8.0.0",
                ["Pomelo.EntityFrameworkCore.MySql"] = "8.0.0",
                ["Npgsql.EntityFrameworkCore.PostgreSQL"] = "8.0.0",
                ["Swashbuckle.AspNetCore"] = "6.5.0",
                ["Microsoft.AspNetCore.Mvc.NewtonsoftJson"] = "8.0.0",
                ["Microsoft.NET.Test.Sdk"] = "17.9.0",
                ["xunit"] = "2.7.0",
                ["xunit.runner.visualstudio"] = "2.5.7",
                ["Moq"] = "4.20.70",
                ["Microsoft.AspNetCore.Mvc.Testing"] = "8.0.0",
                ["Microsoft.Extensions.DependencyInjection"] = "8.0.0",
                ["Microsoft.Extensions.Configuration"] = "8.0.0",
                ["Autofac"] = "8.0.0",
                ["Autofac.Extensions.DependencyInjection"] = "9.0.0",
                ["AutoMapper.Extensions.Microsoft.DependencyInjection"] = "12.0.1",
                ["MediatR.Extensions.Microsoft.DependencyInjection"] = "11.1.0",
                ["FluentValidation.DependencyInjectionExtensions"] = "11.9.0",
                ["Oracle.EntityFrameworkCore"] = "8.23.50",
                ["Microsoft.EntityFrameworkCore.Sqlite"] = "8.0.0"
            };
        }

        private string V(string packageName)
        {
            return _packageVersions.TryGetValue(packageName, out var version) ? version : "8.0.0";
        }

        public string GenerateProjectFile(CodeGenerationContext context, List<string> diPackages = null, string projectType = "Application")
        {
            var sb = new StringBuilder();
            var targetFramework = context.TargetFramework ?? "net8.0";

            string sdkType = projectType switch
            {
                "API" => "Microsoft.NET.Sdk.Web",
                _ => "Microsoft.NET.Sdk"
            };
            
            sb.AppendLine($"<Project Sdk=\"{sdkType}\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine($"    <TargetFramework>{targetFramework}</TargetFramework>");
            sb.AppendLine("    <Nullable>enable</Nullable>");
            sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
            sb.AppendLine($"    <RootNamespace>{context.Namespace}.{projectType}</RootNamespace>");
            sb.AppendLine($"    <AssemblyName>{context.Namespace}.{projectType}</AssemblyName>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            
            // إضافة المراجع حسب نوع المشروع
            switch (projectType)
            {
                case "Domain":
                    // Domain layer has minimal dependencies
                    break;
                    
                case "Application":
                    sb.AppendLine($"    <PackageReference Include=\"AutoMapper\" Version=\"{V("AutoMapper")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"FluentValidation\" Version=\"{V("FluentValidation")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"MediatR\" Version=\"{V("MediatR")}\" />");
                    // Add reference to Domain project
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Domain\\{context.Namespace}.Domain.csproj\" />");
                    break;
                    
                case "Infrastructure":
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"{V("Microsoft.EntityFrameworkCore")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"{V("Microsoft.EntityFrameworkCore.Tools")}\" />");
                    switch (context.DatabaseType)
                    {
                        case DatabaseType.SqlServer:
                            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"{V("Microsoft.EntityFrameworkCore.SqlServer")}\" />");
                            break;
                        case DatabaseType.MySql:
                            sb.AppendLine($"    <PackageReference Include=\"Pomelo.EntityFrameworkCore.MySql\" Version=\"{V("Pomelo.EntityFrameworkCore.MySql")}\" />");
                            break;
                        case DatabaseType.PostgreSql:
                            sb.AppendLine($"    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"{V("Npgsql.EntityFrameworkCore.PostgreSQL")}\" />");
                            break;
                        case DatabaseType.Oracle:
                            sb.AppendLine($"    <PackageReference Include=\"Oracle.EntityFrameworkCore\" Version=\"{V("Oracle.EntityFrameworkCore")}\" />");
                            break;
                        case DatabaseType.SQLite:
                            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Sqlite\" Version=\"{V("Microsoft.EntityFrameworkCore.Sqlite")}\" />");
                            break;
                    }
                    // Add references to Domain and Application projects
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Domain\\{context.Namespace}.Domain.csproj\" />");
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Application\\{context.Namespace}.Application.csproj\" />");
                    break;
                    
                case "API":
                    sb.AppendLine($"    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"{V("Swashbuckle.AspNetCore")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Mvc.NewtonsoftJson\" Version=\"{V("Microsoft.AspNetCore.Mvc.NewtonsoftJson")}\" />");
                    // Add references to all other projects
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Domain\\{context.Namespace}.Domain.csproj\" />");
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Application\\{context.Namespace}.Application.csproj\" />");
                    sb.AppendLine($"    <ProjectReference Include=\"..\\{context.Namespace}.Infrastructure\\{context.Namespace}.Infrastructure.csproj\" />");
                    break;
                    
                case "UnitTests":
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"{V("Microsoft.NET.Test.Sdk")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"xunit\" Version=\"{V("xunit")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"{V("xunit.runner.visualstudio")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"Moq\" Version=\"{V("Moq")}\" />");
                    // Add references to projects being tested
                    sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{context.Namespace}.Domain\\{context.Namespace}.Domain.csproj\" />");
                    sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{context.Namespace}.Application\\{context.Namespace}.Application.csproj\" />");
                    break;
                    
                case "IntegrationTests":
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"{V("Microsoft.NET.Test.Sdk")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"xunit\" Version=\"{V("xunit")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"{V("xunit.runner.visualstudio")}\" />");
                    sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Mvc.Testing\" Version=\"{V("Microsoft.AspNetCore.Mvc.Testing")}\" />");
                    // Add references to projects being tested
                    sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{context.Namespace}.API\\{context.Namespace}.API.csproj\" />");
                    break;
            }
            
            // Add any additional DI packages if specified
            if (diPackages != null)
            {
                foreach (var package in diPackages)
                {
                    sb.AppendLine($"    <PackageReference Include=\"{package}\" Version=\"{GetPackageVersion(package)}\" />");
                }
            }
            
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// توليد ملف appsettings.json
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>محتوى ملف appsettings.json</returns>
        public string GenerateAppSettings(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("{");
            sb.AppendLine("  \"ConnectionStrings\": {");
            
            switch (context.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    sb.AppendLine("    \"DefaultConnection\": \"Server=(localdb)\\\\mssqllocaldb;Database=YourDatabaseName;Trusted_Connection=true;MultipleActiveResultSets=true\"");
                    break;
                case DatabaseType.MySql:
                    sb.AppendLine("    \"DefaultConnection\": \"Server=localhost;Database=YourDatabaseName;Uid=root;Pwd=yourpassword;\"");
                    break;
                case DatabaseType.PostgreSql:
                    sb.AppendLine("    \"DefaultConnection\": \"Host=localhost;Database=YourDatabaseName;Username=postgres;Password=yourpassword\"");
                    break;
                default:
                    sb.AppendLine("    \"DefaultConnection\": \"Your connection string here\"");
                    break;
            }
            
            sb.AppendLine("  },");
            sb.AppendLine("  \"Logging\": {");
            sb.AppendLine("    \"LogLevel\": {");
            sb.AppendLine("      \"Default\": \"Information\",");
            sb.AppendLine("      \"Microsoft.AspNetCore\": \"Warning\"");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"AllowedHosts\": \"*\",");
            sb.AppendLine("  \"HealthChecks\": {");
            sb.AppendLine("    \"Enabled\": true");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// توليد ملف launchSettings.json
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>محتوى ملف launchSettings.json</returns>
        public string GenerateLaunchSettings(CodeGenerationContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("{");
            sb.AppendLine("  \"$schema\": \"https://json.schemastore.org/launchsettings.json\",");
            sb.AppendLine("  \"iisSettings\": {");
            sb.AppendLine("    \"windowsAuthentication\": false,");
            sb.AppendLine("    \"anonymousAuthentication\": true,");
            sb.AppendLine("    \"iisExpress\": {");
            sb.AppendLine("      \"applicationUrl\": \"http://localhost:5000\",");
            sb.AppendLine("      \"sslPort\": 44300");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"profiles\": {");
            sb.AppendLine($"    \"{context.EntityName}.API\": {{");
            sb.AppendLine("      \"commandName\": \"Project\",");
            sb.AppendLine("      \"dotnetRunMessages\": true,");
            sb.AppendLine("      \"launchBrowser\": true,");
            sb.AppendLine("      \"launchUrl\": \"swagger\",");
            sb.AppendLine("      \"applicationUrl\": \"https://localhost:7000;http://localhost:5000\",");
            sb.AppendLine("      \"environmentVariables\": {");
            sb.AppendLine("        \"ASPNETCORE_ENVIRONMENT\": \"Development\"");
            sb.AppendLine("      }");
            sb.AppendLine("    },");
            sb.AppendLine("    \"IIS Express\": {");
            sb.AppendLine("      \"commandName\": \"IISExpress\",");
            sb.AppendLine("      \"launchBrowser\": true,");
            sb.AppendLine("      \"launchUrl\": \"swagger\",");
            sb.AppendLine("      \"environmentVariables\": {");
            sb.AppendLine("        \"ASPNETCORE_ENVIRONMENT\": \"Development\"");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        private string GetPackageVersion(string packageName) => V(packageName);
    }
} 