using System;
using System.Threading.Tasks;
using GeneratorCode.Core.Services;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.DependencyInjection;
using GeneratorCode.Core.TemplateEngine;

namespace GeneratorCode.Examples
{
    /// <summary>
    /// مثال لاستخدام نظام Dependency Injection الجديد
    /// </summary>
    public class DIUsageExample
    {
        public async Task<CodeGenerationResult> GenerateWithDI()
        {
            // إنشاء السياق
            var context = new CodeGenerationContext
            {
                ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=true;",
                DatabaseType = DatabaseType.SqlServer,
                TableName = "Users",
                EntityName = "User",
                ClassName = "User",
                Namespace = "MyApp",
                OutputPath = @"C:\Output\MyApp",
                ArchitecturePattern = "CleanArchitecture",
                TargetLanguage = ProgrammingLanguage.CSharp,
                
                // خيارات DI الجديدة
                DIOptions = new DIOptions
                {
                    EnableDI = true,
                    PreferredContainer = DIContainerType.MicrosoftDI,
                    GenerateServiceExtensions = true,
                    EnableLogging = true,
                    EnableConfiguration = true,
                    EnableHealthChecks = true
                },
                
                Options = new GenerationOptions
                {
                    GenerateControllers = true,
                    GenerateServices = true,
                    GenerateRepositories = true,
                    GenerateModels = true,
                    GenerateDTOs = true,
                    GenerateValidators = true,
                    EnableDependencyInjection = true,
                    EnableAsyncOperations = true,
                    EnableLogging = true
                }
            };
            
            // إنشاء الخدمات
            var patternFactory = new ArchitecturePatternFactory();
            var databaseFactory = new DatabaseProviderFactory();
            var diProviderFactory = new DIProviderFactory();
            var templateEngine = new SimpleTemplateEngine();
            
            var codeGenerationService = new CodeGenerationService(
                patternFactory,
                databaseFactory,
                diProviderFactory,
                templateEngine
            );
            
            // توليد الكود مع DI
            var result = await codeGenerationService.GenerateCodeAsync(context);
            
            if (result.Success)
            {
                Console.WriteLine($"تم توليد {result.GeneratedFiles.Count} ملف بنجاح");
                Console.WriteLine($"الحجم الإجمالي: {result.TotalSizeInBytes} بايت");
                Console.WriteLine($"رسالة: {result.Message}");
                
                // عرض الملفات المولدة
                foreach (var file in result.GeneratedFiles)
                {
                    Console.WriteLine($"- {file.FileName} ({file.Layer})");
                }
                
                // عرض التبعيات المطلوبة لـ DI
                var diTypes = codeGenerationService.GetSupportedDIContainerTypes();
                Console.WriteLine($"\nأنواع DI Containers المدعومة: {string.Join(", ", diTypes)}");
                
                var packages = codeGenerationService.GetRequiredPackagesForDI(DIContainerType.MicrosoftDI);
                Console.WriteLine($"\nالتبعيات المطلوبة لـ Microsoft DI:");
                foreach (var package in packages)
                {
                    Console.WriteLine($"- {package}");
                }
            }
            else
            {
                Console.WriteLine($"فشل في التوليد: {result.Message}");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"خطأ: {error}");
                }
            }
            
            return result;
        }
        
        public async Task<CodeGenerationResult> GenerateWithAutofac()
        {
            // مثال لاستخدام Autofac بدلاً من Microsoft DI
            var context = new CodeGenerationContext
            {
                ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=TestDB;Trusted_Connection=true;",
                DatabaseType = DatabaseType.SqlServer,
                TableName = "Products",
                EntityName = "Product",
                ClassName = "Product",
                Namespace = "MyApp",
                OutputPath = @"C:\Output\MyApp",
                ArchitecturePattern = "CleanArchitecture",
                TargetLanguage = ProgrammingLanguage.CSharp,
                
                // استخدام Autofac
                DIOptions = new DIOptions
                {
                    EnableDI = true,
                    PreferredContainer = DIContainerType.Autofac,
                    GenerateModuleClasses = true,
                    EnableAutoRegistration = false
                }
            };
            
            var patternFactory = new ArchitecturePatternFactory();
            var databaseFactory = new DatabaseProviderFactory();
            var diProviderFactory = new DIProviderFactory();
            var templateEngine = new SimpleTemplateEngine();
            
            var codeGenerationService = new CodeGenerationService(
                patternFactory,
                databaseFactory,
                diProviderFactory,
                templateEngine
            );
            
            return await codeGenerationService.GenerateCodeAsync(context);
        }
    }
} 