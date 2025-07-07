using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// مولد ملفات التبعيات
    /// </summary>
    public class PackagesGenerator
    {
        /// <summary>
        /// توليد ملف csproj للتبعيات
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <param name="diPackages">التبعيات الخاصة بـ DI</param>
        /// <returns>محتوى ملف csproj</returns>
        public string GenerateProjectFile(CodeGenerationContext context, List<string> diPackages = null)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>net6.0</TargetFramework>");
            sb.AppendLine("    <Nullable>enable</Nullable>");
            sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            
            // تبعيات أساسية
            sb.AppendLine("    <PackageReference Include=\"Microsoft.AspNetCore.OpenApi\" Version=\"6.0.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"6.2.3\" />");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore\" Version=\"6.0.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Tools\" Version=\"6.0.0\" />");
            
            // تبعيات قاعدة البيانات
            switch (context.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    sb.AppendLine("    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"6.0.0\" />");
                    break;
                case DatabaseType.MySql:
                    sb.AppendLine("    <PackageReference Include=\"Pomelo.EntityFrameworkCore.MySql\" Version=\"6.0.0\" />");
                    break;
                case DatabaseType.PostgreSql:
                    sb.AppendLine("    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"6.0.0\" />");
                    break;
            }
            
            // تبعيات النمط المعماري
            if (context.ArchitecturePattern == "CleanArchitecture" || context.ArchitecturePattern == "CQRS")
            {
                sb.AppendLine("    <PackageReference Include=\"MediatR.Extensions.Microsoft.DependencyInjection\" Version=\"10.0.0\" />");
                sb.AppendLine("    <PackageReference Include=\"AutoMapper.Extensions.Microsoft.DependencyInjection\" Version=\"11.0.0\" />");
            }
            
            // تبعيات التحقق من الصحة
            if (context.Options.GenerateValidators)
            {
                sb.AppendLine("    <PackageReference Include=\"FluentValidation.DependencyInjectionExtensions\" Version=\"11.0.0\" />");
            }
            
            // تبعيات DI
            if (context.DIOptions.EnableDI && diPackages != null)
            {
                foreach (var package in diPackages.Distinct())
                {
                    var version = GetPackageVersion(package);
                    sb.AppendLine($"    <PackageReference Include=\"{package}\" Version=\"{version}\" />");
                }
            }
            
            // تبعيات اختيارية
            sb.AppendLine("    <PackageReference Include=\"Microsoft.AspNetCore.Diagnostics.HealthChecks\" Version=\"2.2.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Serilog.AspNetCore\" Version=\"6.0.0\" />");
            
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
        
        private string GetPackageVersion(string packageName)
        {
            var versions = new Dictionary<string, string>
            {
                ["Microsoft.Extensions.DependencyInjection"] = "6.0.0",
                ["Microsoft.Extensions.Configuration"] = "6.0.0",
                ["Autofac"] = "6.4.0",
                ["Autofac.Extensions.DependencyInjection"] = "8.0.0",
                ["Microsoft.EntityFrameworkCore"] = "6.0.0",
                ["AutoMapper.Extensions.Microsoft.DependencyInjection"] = "11.0.0",
                ["MediatR.Extensions.Microsoft.DependencyInjection"] = "10.0.0",
                ["FluentValidation.DependencyInjectionExtensions"] = "11.0.0"
            };
            
            return versions.TryGetValue(packageName, out var version) ? version : "6.0.0";
        }
    }
} 