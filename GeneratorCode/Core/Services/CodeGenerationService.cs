using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.DependencyInjection;

namespace GeneratorCode.Core.Services
{
    /// <summary>
    /// خدمة توليد الكود الرئيسية
    /// </summary>
    public class CodeGenerationService
    {
        private readonly IArchitecturePatternFactory _patternFactory;
        private readonly IDatabaseProviderFactory _databaseFactory;
        private readonly IDIProviderFactory _diProviderFactory;
        private readonly ITemplateEngine _templateEngine;
        
        public CodeGenerationService(
            IArchitecturePatternFactory patternFactory,
            IDatabaseProviderFactory databaseFactory,
            IDIProviderFactory diProviderFactory,
            ITemplateEngine templateEngine)
        {
            _patternFactory = patternFactory ?? throw new ArgumentNullException(nameof(patternFactory));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
            _diProviderFactory = diProviderFactory ?? throw new ArgumentNullException(nameof(diProviderFactory));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        }
        
        /// <summary>
        /// توليد الكود
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>نتيجة التوليد</returns>
        public async Task<CodeGenerationResult> GenerateCodeAsync(CodeGenerationContext context)
        {
            // التحقق من صحة السياق
            var validationResult = ValidateContext(context);
            if (!validationResult.Success)
                return validationResult;

            var result = new CodeGenerationResult();
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            
            try
            {
                // Generate the code using the selected pattern
                result = await pattern.Generate(context);

                // Generate DI Configuration if enabled
                if (result.Success && context.DIOptions.EnableDI)
                {
                    var diProvider = _diProviderFactory.CreateProvider(context.DIOptions.PreferredContainer);
                    if (diProvider != null)
                    {
                        var diResult = diProvider.GenerateConfiguration(context);
                        if (diResult.Success)
                        {
                            result.GeneratedFiles.AddRange(diResult.ConfigurationFiles);
                            result.Message += $" مع تكوين {diProvider.Name}";
                        }
                        else
                        {
                            result.Errors.AddRange(diResult.Errors);
                            result.Warnings.AddRange(diResult.Warnings);
                        }
                    }
                    else
                    {
                        result.Warnings.Add($"موفر DI غير مدعوم: {context.DIOptions.PreferredContainer}");
                    }
                }

                // Save all generated files
                if (result.Success)
                {
                    await SaveGeneratedFilesAsync(result);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error generating code: {ex.Message}";
                result.Errors.Add(ex.ToString());
                return result;
            }
        }
        
        /// <summary>
        /// الحصول على الأنماط المعمارية المدعومة
        /// </summary>
        /// <returns>قائمة الأنماط المدعومة</returns>
        public string[] GetSupportedPatterns()
        {
            return _patternFactory.GetSupportedPatternNames().ToArray();
        }
        
        /// <summary>
        /// الحصول على أنواع قواعد البيانات المدعومة
        /// </summary>
        /// <returns>قائمة أنواع قواعد البيانات</returns>
        public DatabaseType[] GetSupportedDatabaseTypes()
        {
            return _databaseFactory.GetSupportedDatabaseTypes().ToArray();
        }

        /// <summary>
        /// اختبار الاتصال بقاعدة البيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <param name="connectionString">نص الاتصال</param>
        /// <returns>true إذا نجح الاتصال</returns>
        public bool TestDatabaseConnection(DatabaseType databaseType, string connectionString)
        {
            var provider = _databaseFactory.CreateProvider(databaseType);
            return provider?.TestConnection(connectionString) ?? false;
        }

        /// <summary>
        /// الحصول على قائمة الجداول من قاعدة البيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <param name="connectionString">نص الاتصال</param>
        /// <returns>قائمة الجداول</returns>
        public List<TableInfo> GetTables(DatabaseType databaseType, string connectionString)
        {
            var provider = _databaseFactory.CreateProvider(databaseType);
            return provider?.GetTables(connectionString) ?? new List<TableInfo>();
        }

        /// <summary>
        /// الحصول على أنواع DI Containers المدعومة
        /// </summary>
        /// <returns>قائمة أنواع Containers</returns>
        public DIContainerType[] GetSupportedDIContainerTypes()
        {
            return _diProviderFactory.GetSupportedContainerTypes().ToArray();
        }
        
        /// <summary>
        /// الحصول على التبعيات المطلوبة لـ DI Container معين
        /// </summary>
        /// <param name="containerType">نوع Container</param>
        /// <returns>قائمة التبعيات</returns>
        public List<string> GetRequiredPackagesForDI(DIContainerType containerType)
        {
            var provider = _diProviderFactory.CreateProvider(containerType);
            return provider?.GetRequiredPackages() ?? new List<string>();
        }

        public List<ColumnInfo> GetTableColumns(DatabaseType databaseType, string connectionString, string tableName)
        {
            var provider = _databaseFactory.CreateProvider(databaseType);
            return provider?.GetColumns(connectionString, tableName) ?? new List<ColumnInfo>();
        }

        public PreviewResult GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            var result = new PreviewResult();
            
            // التحقق من المدخلات
            if (table == null)
            {
                result.Success = false;
                result.Error = "معلومات الجدول مطلوبة";
                return result;
            }

            if (context == null)
            {
                result.Success = false;
                result.Error = "سياق توليد الكود مطلوب";
                return result;
            }

            if (string.IsNullOrEmpty(context.ArchitecturePattern))
            {
                result.Success = false;
                result.Error = "النمط المعماري مطلوب";
                return result;
            }

            // إنشاء النمط المعماري
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            if (pattern == null)
            {
                result.Success = false;
                result.Error = $"النمط المعماري '{context.ArchitecturePattern}' غير مدعوم";
                return result;
            }

            // توليد المعاينة
            result.Files = pattern.GeneratePreview(table, context);
            result.Success = true;
            
            return result;
        }

        private static CodeGenerationResult ValidateContext(CodeGenerationContext context)
        {
            var result = new CodeGenerationResult { Success = true };
            
            if (context == null)
            {
                result.Success = false;
                result.Message = "سياق توليد الكود مطلوب";
                return result;
            }
            
            if (string.IsNullOrEmpty(context.ConnectionString))
            {
                result.Success = false;
                result.Message = "نص الاتصال مطلوب";
                return result;
            }
            
            if (string.IsNullOrEmpty(context.TableName))
            {
                result.Success = false;
                result.Message = "اسم الجدول مطلوب";
                return result;
            }
            
            if (string.IsNullOrEmpty(context.EntityName))
            {
                result.Success = false;
                result.Message = "اسم الكيان مطلوب";
                return result;
            }
            
            if (string.IsNullOrEmpty(context.Namespace))
            {
                result.Success = false;
                result.Message = "مساحة الأسماء مطلوبة";
                return result;
            }
            
            if (string.IsNullOrEmpty(context.OutputPath))
            {
                result.Success = false;
                result.Message = "مسار الحفظ مطلوب";
                return result;
            }
            
            return result;
        }

        private static TableInfo LoadTableInfo(IDatabaseProvider provider, CodeGenerationContext context)
        {
            var tables = provider.GetTables(context.ConnectionString);
            var table = tables.Find(t => t.Name.Equals(context.TableName, StringComparison.OrdinalIgnoreCase));

            if (table != null)
            {
                table.Columns = provider.GetColumns(context.ConnectionString, context.TableName);
                table.PrimaryKeys = provider.GetPrimaryKeys(context.ConnectionString, context.TableName);
                table.ForeignKeys = provider.GetForeignKeys(context.ConnectionString, context.TableName);
            }

            return table;
        }

        private static async Task SaveGeneratedFilesAsync(CodeGenerationResult result)
        {
            foreach (var file in result.GeneratedFiles)
            {
                var directory = System.IO.Path.GetDirectoryName(file.FullPath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                await System.IO.File.WriteAllTextAsync(file.FullPath, file.Content);
                file.SizeInBytes = System.Text.Encoding.UTF8.GetByteCount(file.Content);
            }
            
            result.TotalSizeInBytes = result.GeneratedFiles.Sum(f => f.SizeInBytes);
        }

        private static string GetTemplateDirectory(string architecturePattern)
        {
            // تحويل اسم النمط إلى اسم المجلد
            return architecturePattern.Replace(" ", "");
        }

        public async Task GenerateStartupFile(CodeGenerationContext context, string filePath)
        {
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern);
            var templatePath = $"{patternDir}/Infrastructure/Startup.template";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateProgramFile(CodeGenerationContext context, string filePath)
        {
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern);
            var templatePath = $"{patternDir}/Infrastructure/Program.template";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateGitignore(string filePath)
        {
            var template = await _templateEngine.LoadTemplateAsync("CleanArchitecture/.gitignore.template");
            var content = _templateEngine.RenderTemplate(template, null);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateReadme(CodeGenerationContext context, string filePath)
        {
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern);
            var templatePath = $"{patternDir}/README.template.md";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateSolutionFile(CodeGenerationContext context, string projectPath)
        {
            try
            {
                // Create namespace directory only if projectPath doesn't end with namespace
                string namespacePath;
                if (!projectPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(projectPath, context.Namespace);
                    if (!Directory.Exists(namespacePath))
                    {
                        Directory.CreateDirectory(namespacePath);
                    }
                }
                else
                {
                    namespacePath = projectPath;
                }

                // Create src directory if it doesn't exist
                var srcPath = Path.Combine(namespacePath, "src");
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }

                // Create tests directory if it doesn't exist
                var testsPath = Path.Combine(namespacePath, "tests");
                if (!Directory.Exists(testsPath))
                {
                    Directory.CreateDirectory(testsPath);
                }

                var patternDir = GetTemplateDirectory(context.ArchitecturePattern);
                var templatePath = $"{patternDir}/Solution.template";
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
                
                var content = _templateEngine.RenderTemplate(template, templateData);
                var slnPath = Path.Combine(namespacePath, $"{context.Namespace}.sln");
                await File.WriteAllTextAsync(slnPath, content);

                // Generate global.json in the namespace directory
                var globalJsonContent = @"{
  ""sdk"": {
    ""version"": ""6.0.100"",
    ""rollForward"": ""latestFeature""
  }
}";
                await File.WriteAllTextAsync(Path.Combine(namespacePath, "global.json"), globalJsonContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating solution file: {ex.Message}", ex);
            }
        }

        public async Task GenerateInfrastructureLayer(CodeGenerationContext context, string directoryPath)
        {
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!directoryPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(directoryPath, context.Namespace);
                    if (!Directory.Exists(namespacePath))
                    {
                        Directory.CreateDirectory(namespacePath);
                    }
                }
                else
                {
                    namespacePath = directoryPath;
                }

                // Create src directory if it doesn't exist
                var srcPath = Path.Combine(namespacePath, "src");
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }

                // Create Infrastructure project directory
                var infrastructurePath = Path.Combine(srcPath, $"{context.Namespace}.Infrastructure");
                if (!Directory.Exists(infrastructurePath))
                {
                    Directory.CreateDirectory(infrastructurePath);
                }

                var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
                await pattern.GenerateInfrastructureLayerAsync(context, infrastructurePath);

                // Generate project file for Infrastructure layer
                var packagesGenerator = new PackagesGenerator();
                var projectFile = packagesGenerator.GenerateProjectFile(context, null, "Infrastructure");
                var projectPath = Path.Combine(infrastructurePath, $"{context.Namespace}.Infrastructure.csproj");
                await File.WriteAllTextAsync(projectPath, projectFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating infrastructure layer: {ex.Message}", ex);
            }
        }

        public async Task GenerateApplicationLayer(CodeGenerationContext context, string directoryPath)
        {
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!directoryPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(directoryPath, context.Namespace);
                    if (!Directory.Exists(namespacePath))
                    {
                        Directory.CreateDirectory(namespacePath);
                    }
                }
                else
                {
                    namespacePath = directoryPath;
                }

                // Create src directory if it doesn't exist
                var srcPath = Path.Combine(namespacePath, "src");
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }

                // Create Application project directory
                var applicationPath = Path.Combine(srcPath, $"{context.Namespace}.Application");
                if (!Directory.Exists(applicationPath))
                {
                    Directory.CreateDirectory(applicationPath);
                }

                var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
                await pattern.GenerateApplicationLayerAsync(context, applicationPath);

                // Generate project file for Application layer
                var packagesGenerator = new PackagesGenerator();
                var projectFile = packagesGenerator.GenerateProjectFile(context, null, "Application");
                var projectPath = Path.Combine(applicationPath, $"{context.Namespace}.Application.csproj");
                await File.WriteAllTextAsync(projectPath, projectFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating application layer: {ex.Message}", ex);
            }
        }

        public async Task GenerateDomainLayer(CodeGenerationContext context, string directoryPath)
        {
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!directoryPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(directoryPath, context.Namespace);
                    if (!Directory.Exists(namespacePath))
                    {
                        Directory.CreateDirectory(namespacePath);
                    }
                }
                else
                {
                    namespacePath = directoryPath;
                }

                // Create src directory if it doesn't exist
                var srcPath = Path.Combine(namespacePath, "src");
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }

                // Create Domain project directory
                var domainPath = Path.Combine(srcPath, $"{context.Namespace}.Domain");
                if (!Directory.Exists(domainPath))
                {
                    Directory.CreateDirectory(domainPath);
                }

                var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
                await pattern.GenerateDomainLayerAsync(context, domainPath);

                // Generate project file for Domain layer
                var packagesGenerator = new PackagesGenerator();
                var projectFile = packagesGenerator.GenerateProjectFile(context, null, "Domain");
                var projectPath = Path.Combine(domainPath, $"{context.Namespace}.Domain.csproj");
                await File.WriteAllTextAsync(projectPath, projectFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating domain layer: {ex.Message}", ex);
            }
        }

        public async Task GeneratePresentationLayer(CodeGenerationContext context, string directoryPath)
        {
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!directoryPath.EndsWith(context.Namespace))
                {
                    namespacePath = Path.Combine(directoryPath, context.Namespace);
                    if (!Directory.Exists(namespacePath))
                    {
                        Directory.CreateDirectory(namespacePath);
                    }
                }
                else
                {
                    namespacePath = directoryPath;
                }

                // Create src directory if it doesn't exist
                var srcPath = Path.Combine(namespacePath, "src");
                if (!Directory.Exists(srcPath))
                {
                    Directory.CreateDirectory(srcPath);
                }

                // Create API project directory
                var apiPath = Path.Combine(srcPath, $"{context.Namespace}.API");
                if (!Directory.Exists(apiPath))
                {
                    Directory.CreateDirectory(apiPath);
                }

                var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
                await pattern.GeneratePresentationLayerAsync(context, apiPath);

                // Generate project file for API layer
                var packagesGenerator = new PackagesGenerator();
                var projectFile = packagesGenerator.GenerateProjectFile(context, null, "API");
                var projectPath = Path.Combine(apiPath, $"{context.Namespace}.API.csproj");
                await File.WriteAllTextAsync(projectPath, projectFile);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating presentation layer: {ex.Message}", ex);
            }
        }
    }
} 