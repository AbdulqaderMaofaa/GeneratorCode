using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// موفر Microsoft Dependency Injection
    /// </summary>
    public class MicrosoftDIProvider : IDependencyInjectionProvider
    {
        public string Name => "Microsoft Dependency Injection";
        public DIContainerType ContainerType => DIContainerType.MicrosoftDI;
        
        public DIConfigurationResult GenerateConfiguration(CodeGenerationContext context)
        {
            var result = new DIConfigurationResult { Success = true };
            
            // توليد Service Extensions
                var serviceExtensions = GenerateServiceExtensions(context);
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}ServiceExtensions.cs",
                    RelativePath = $"Extensions/{context.EntityName}ServiceExtensions.cs",
                    FullPath = System.IO.Path.Combine(context.OutputPath, "Extensions", $"{context.EntityName}ServiceExtensions.cs"),
                    Content = serviceExtensions,
                    FileType = "cs",
                    Layer = "Infrastructure"
                });
                
                // توليد Startup Configuration
                var startupConfig = GenerateStartupConfiguration(context);
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = "Startup.cs",
                    RelativePath = "Startup.cs",
                    FullPath = System.IO.Path.Combine(context.OutputPath, "Startup.cs"),
                    Content = startupConfig,
                    FileType = "cs",
                    Layer = "Presentation"
                });
                
                // توليد ملفات التكوين الإضافية
                var packagesGenerator = new PackagesGenerator();
                
                // توليد ملف csproj
                var projectFile = packagesGenerator.GenerateProjectFile(context, GetRequiredPackages());
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = $"{context.EntityName}.csproj",
                    RelativePath = $"{context.EntityName}.csproj",
                    FullPath = System.IO.Path.Combine(context.OutputPath, $"{context.EntityName}.csproj"),
                    Content = projectFile,
                    FileType = "xml",
                    Layer = "Root"
                });
                
                // توليد ملف appsettings.json
                var appSettings = packagesGenerator.GenerateAppSettings(context);
                result.ConfigurationFiles.Add(new GeneratedFile
                {
                    FileName = "appsettings.json",
                    RelativePath = "appsettings.json",
                    FullPath = System.IO.Path.Combine(context.OutputPath, "appsettings.json"),
                    Content = appSettings,
                    FileType = "json",
                    Layer = "Root"
                });
                
                result.Message = "تم توليد تكوين Microsoft DI بنجاح مع جميع ملفات التكوين";
            
            return result;
        }
        
        public List<string> GetRequiredPackages()
        {
            return new List<string>
            {
                "Microsoft.Extensions.DependencyInjection",
                "Microsoft.Extensions.Configuration",
                "Microsoft.EntityFrameworkCore.SqlServer",
                "AutoMapper.Extensions.Microsoft.DependencyInjection",
                "MediatR.Extensions.Microsoft.DependencyInjection"
            };
        }
        
        public List<string> GetRequiredUsings()
        {
            return new List<string>
            {
                "Microsoft.Extensions.DependencyInjection",
                "Microsoft.Extensions.Configuration",
                "Microsoft.AspNetCore.Builder"
            };
        }
        
        // باقي الدوال في ملف منفصل للطول
        public string GenerateServiceExtensions(CodeGenerationContext context) => 
            new ServiceExtensionsGenerator().Generate(context);
            
        public string GenerateStartupConfiguration(CodeGenerationContext context) => 
            new StartupConfigurationGenerator().Generate(context);
    }
} 