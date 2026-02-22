using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.DependencyInjection;
using GeneratorCode.Core.CodeGenerators;
using GeneratorCode.Core.Logging;

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
        private readonly ILogger _logger;
        
        public CodeGenerationService(
            IArchitecturePatternFactory patternFactory,
            IDatabaseProviderFactory databaseFactory,
            IDIProviderFactory diProviderFactory,
            ITemplateEngine templateEngine,
            ILogger? logger = null)
        {
            _patternFactory = patternFactory ?? throw new ArgumentNullException(nameof(patternFactory));
            _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));
            _diProviderFactory = diProviderFactory ?? throw new ArgumentNullException(nameof(diProviderFactory));
            _templateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
            _logger = logger ?? LoggerFactory.Default;
        }
        
        /// <summary>
        /// توليد الكود
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>نتيجة التوليد</returns>
        public async Task<CodeGenerationResult> GenerateCodeAsync(CodeGenerationContext context)
        {
            if (context?.Mode == Models.GenerationMode.CodeFirst)
            {
                return await GenerateCodeFirstAsync(context);
            }

            _logger.LogInfo($"Starting code generation for table: {context?.TableName}, pattern: {context?.ArchitecturePattern}", 
                "CodeGenerationService.GenerateCodeAsync",
                new Dictionary<string, object> { { "EntityName", context?.EntityName ?? "N/A" }, { "Namespace", context?.Namespace ?? "N/A" } });

            // التحقق من صحة السياق
            var validationResult = ValidateContext(context);
            if (!validationResult.Success)
            {
                _logger.LogWarning($"Code generation validation failed: {validationResult.Message}", 
                    null, "CodeGenerationService.GenerateCodeAsync");
                return validationResult;
            }

            var result = new CodeGenerationResult();
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);

            if (pattern == null)
            {
                result.Success = false;
                result.Message = $"النمط المعماري '{context.ArchitecturePattern}' غير مدعوم";
                _logger.LogError($"Unsupported architecture pattern: {context.ArchitecturePattern}", null, "CodeGenerationService.GenerateCodeAsync");
                return result;
            }

            try
            {
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
                            _logger.LogInfo($"DI configuration generated successfully using {diProvider.Name}", 
                                "CodeGenerationService.GenerateCodeAsync");
                        }
                        else
                        {
                            result.Errors.AddRange(diResult.Errors);
                            result.Warnings.AddRange(diResult.Warnings);
                            _logger.LogWarning($"DI configuration generation had issues: {string.Join(", ", diResult.Errors)}", 
                                null, "CodeGenerationService.GenerateCodeAsync");
                        }
                    }
                    else
                    {
                        result.Warnings.Add($"موفر DI غير مدعوم: {context.DIOptions.PreferredContainer}");
                        _logger.LogWarning($"Unsupported DI provider: {context.DIOptions.PreferredContainer}", 
                            null, "CodeGenerationService.GenerateCodeAsync");
                    }
                }

                if (result.Success)
                {
                    AppendLanguageSpecificFiles(context, result);
                }

                if (result.Success)
                {
                    await SaveGeneratedFilesAsync(result);
                    _logger.LogInfo($"Code generation completed successfully. Generated {result.GeneratedFiles.Count} files, Total size: {result.TotalSizeInBytes} bytes", 
                        "CodeGenerationService.GenerateCodeAsync");
                }
                else
                {
                    _logger.LogError($"Code generation failed: {result.Message}", null, 
                        "CodeGenerationService.GenerateCodeAsync",
                        new Dictionary<string, object> { { "Errors", string.Join("; ", result.Errors) } });
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error generating code: {ex.Message}";
                result.Errors.Add(ex.ToString());
                _logger.LogError("Error generating code", ex, "CodeGenerationService.GenerateCodeAsync",
                    new Dictionary<string, object> { { "TableName", context?.TableName ?? "N/A" }, { "Pattern", context?.ArchitecturePattern ?? "N/A" } });
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
            try
            {
                _logger.LogDebug($"Testing database connection for {databaseType}", "CodeGenerationService.TestDatabaseConnection");
                var provider = _databaseFactory.CreateProvider(databaseType);
                var result = provider?.TestConnection(connectionString) ?? false;
                if (result)
                {
                    _logger.LogInfo($"Database connection test successful for {databaseType}", "CodeGenerationService.TestDatabaseConnection");
                }
                else
                {
                    _logger.LogWarning($"Database connection test failed for {databaseType}", null, "CodeGenerationService.TestDatabaseConnection");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error testing database connection for {databaseType}", ex, "CodeGenerationService.TestDatabaseConnection");
                return false;
            }
        }

        /// <summary>
        /// الحصول على قائمة الجداول من قاعدة البيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <param name="connectionString">نص الاتصال</param>
        /// <returns>قائمة الجداول</returns>
        public List<TableInfo> GetTables(DatabaseType databaseType, string connectionString)
        {
            try
            {
                _logger.LogDebug($"Getting tables from {databaseType}", "CodeGenerationService.GetTables");
                var provider = _databaseFactory.CreateProvider(databaseType);
                var tables = provider?.GetTables(connectionString) ?? new List<TableInfo>();
                _logger.LogInfo($"Retrieved {tables.Count} tables from {databaseType}", "CodeGenerationService.GetTables");
                return tables;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting tables from {databaseType}", ex, "CodeGenerationService.GetTables");
                return new List<TableInfo>();
            }
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
            try
            {
                _logger.LogDebug($"Getting columns for table {tableName} from {databaseType}", "CodeGenerationService.GetTableColumns");
                var provider = _databaseFactory.CreateProvider(databaseType);
                if (provider == null)
                {
                    _logger.LogWarning($"Database provider not found for {databaseType}", null, "CodeGenerationService.GetTableColumns");
                    return new List<ColumnInfo>();
                }
                var columns = provider.GetColumns(connectionString, tableName) ?? new List<ColumnInfo>();
                _logger.LogInfo($"Retrieved {columns.Count} columns for table {tableName}", "CodeGenerationService.GetTableColumns");
                return columns;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting columns for table {tableName}", ex, "CodeGenerationService.GetTableColumns");
                return new List<ColumnInfo>();
            }
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

        private void AppendLanguageSpecificFiles(CodeGenerationContext context, CodeGenerationResult result)
        {
            try
            {
                switch (context.TargetLanguage)
                {
                    case ProgrammingLanguage.TypeScript:
                        var tsFiles = TypeScriptGenerator.Generate(context);
                        result.GeneratedFiles.AddRange(tsFiles);
                        result.TotalSizeInBytes += tsFiles.Sum(f => f.SizeInBytes);
                        _logger.LogInfo($"Generated {tsFiles.Count} TypeScript files", "CodeGenerationService.AppendLanguageSpecificFiles");
                        break;

                    case ProgrammingLanguage.AspNetCore:
                        var razorFiles = AspNetCoreViewGenerator.Generate(context);
                        result.GeneratedFiles.AddRange(razorFiles);
                        result.TotalSizeInBytes += razorFiles.Sum(f => f.SizeInBytes);
                        _logger.LogInfo($"Generated {razorFiles.Count} ASP.NET Core Razor Pages", "CodeGenerationService.AppendLanguageSpecificFiles");
                        break;

                    case ProgrammingLanguage.CSharp:
                    case ProgrammingLanguage.AspNetWebForms:
                    case ProgrammingLanguage.AspNetMvc:
                        break;
                }
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"Language-specific generation warning: {ex.Message}");
                _logger.LogWarning($"Language-specific generation failed: {ex.Message}", ex, "CodeGenerationService.AppendLanguageSpecificFiles");
            }
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

        private async Task SaveGeneratedFilesAsync(CodeGenerationResult result)
        {
            var savedCount = 0;
            foreach (var file in result.GeneratedFiles)
            {
                try
                {
                    var directory = Path.GetDirectoryName(file.FullPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    await File.WriteAllTextAsync(file.FullPath, file.Content);
                    file.SizeInBytes = System.Text.Encoding.UTF8.GetByteCount(file.Content);
                    savedCount++;
                }
                catch (UnauthorizedAccessException ex)
                {
                    result.Warnings.Add($"لا توجد صلاحيات كافية لحفظ الملف: {file.FileName} - {ex.Message}");
                    _logger.LogWarning($"Access denied saving file: {file.FullPath}", ex, "CodeGenerationService.SaveGeneratedFilesAsync");
                }
                catch (DirectoryNotFoundException ex)
                {
                    result.Warnings.Add($"المسار غير موجود: {file.FileName} - {ex.Message}");
                    _logger.LogWarning($"Directory not found for file: {file.FullPath}", ex, "CodeGenerationService.SaveGeneratedFilesAsync");
                }
                catch (IOException ex)
                {
                    result.Warnings.Add($"خطأ في حفظ الملف: {file.FileName} - {ex.Message}");
                    _logger.LogError($"IO error saving file: {file.FullPath}", ex, "CodeGenerationService.SaveGeneratedFilesAsync");
                }
            }

            result.TotalSizeInBytes = result.GeneratedFiles.Sum(f => f.SizeInBytes);
            _logger.LogInfo($"Saved {savedCount}/{result.GeneratedFiles.Count} files successfully", "CodeGenerationService.SaveGeneratedFilesAsync");
        }

        private static string GetTemplateDirectory(string architecturePattern)
        {
            // تحويل اسم النمط إلى اسم المجلد
            return architecturePattern.Replace(" ", "");
        }

        public async Task GenerateStartupFile(CodeGenerationContext context, string filePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern ?? string.Empty);
            var templatePath = $"{patternDir}/Infrastructure/Startup.template";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateProgramFile(CodeGenerationContext context, string filePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern ?? string.Empty);
            var templatePath = $"{patternDir}/Infrastructure/Program.template";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateGitignore(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                
            var template = await _templateEngine.LoadTemplateAsync("CleanArchitecture/.gitignore.template");
            var content = _templateEngine.RenderTemplate(template, null);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateReadme(CodeGenerationContext context, string filePath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
                
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern ?? string.Empty);
            var templatePath = $"{patternDir}/README.template.md";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            await File.WriteAllTextAsync(filePath, content);
        }

        public async Task GenerateSolutionFile(CodeGenerationContext context, string projectPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(projectPath))
                throw new ArgumentException("Project path cannot be null or empty", nameof(projectPath));
                
            try
            {
                // Create namespace directory only if projectPath doesn't end with namespace
                string namespacePath;
                if (!string.IsNullOrEmpty(context.Namespace) && !projectPath.EndsWith(context.Namespace))
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
                // Using rollForward: latestMajor without specific version allows using any compatible SDK version
                var globalJsonContent = @"{
  ""sdk"": {
    ""rollForward"": ""latestMajor"",
    ""allowPrerelease"": false
  }
}";
                await File.WriteAllTextAsync(Path.Combine(namespacePath, "global.json"), globalJsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating solution file", ex, "CodeGenerationService.GenerateSolutionFile",
                    new Dictionary<string, object> { { "ProjectPath", projectPath }, { "Namespace", context.Namespace } });
                throw new Exception($"Error generating solution file: {ex.Message}", ex);
            }
        }

        public async Task GenerateInfrastructureLayer(CodeGenerationContext context, string directoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));
                
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!string.IsNullOrEmpty(context.Namespace) && !directoryPath.EndsWith(context.Namespace))
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
                _logger.LogError("Error generating infrastructure layer", ex, "CodeGenerationService.GenerateInfrastructureLayer",
                    new Dictionary<string, object> { { "DirectoryPath", directoryPath }, { "Namespace", context.Namespace } });
                throw new Exception($"Error generating infrastructure layer: {ex.Message}", ex);
            }
        }

        public async Task GenerateApplicationLayer(CodeGenerationContext context, string directoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));
                
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!string.IsNullOrEmpty(context.Namespace) && !directoryPath.EndsWith(context.Namespace))
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
                _logger.LogError("Error generating application layer", ex, "CodeGenerationService.GenerateApplicationLayer",
                    new Dictionary<string, object> { { "DirectoryPath", directoryPath }, { "Namespace", context.Namespace } });
                throw new Exception($"Error generating application layer: {ex.Message}", ex);
            }
        }

        public async Task GenerateDomainLayer(CodeGenerationContext context, string directoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));
                
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!string.IsNullOrEmpty(context.Namespace) && !directoryPath.EndsWith(context.Namespace))
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
                _logger.LogError("Error generating domain layer", ex, "CodeGenerationService.GenerateDomainLayer",
                    new Dictionary<string, object> { { "DirectoryPath", directoryPath }, { "Namespace", context.Namespace } });
                throw new Exception($"Error generating domain layer: {ex.Message}", ex);
            }
        }

        public async Task GeneratePresentationLayer(CodeGenerationContext context, string directoryPath)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Directory path cannot be null or empty", nameof(directoryPath));
                
            try
            {
                // Check if directoryPath already ends with namespace
                string namespacePath;
                if (!string.IsNullOrEmpty(context.Namespace) && !directoryPath.EndsWith(context.Namespace))
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
                _logger.LogError("Error generating presentation layer", ex, "CodeGenerationService.GeneratePresentationLayer",
                    new Dictionary<string, object> { { "DirectoryPath", directoryPath }, { "Namespace", context.Namespace } });
                throw new Exception($"Error generating presentation layer: {ex.Message}", ex);
            }
        }

        private async Task<CodeGenerationResult> GenerateCodeFirstAsync(CodeGenerationContext context)
        {
            _logger.LogInfo("Starting Code First generation", "CodeGenerationService.GenerateCodeFirstAsync");

            if (context.DomainModel == null)
            {
                return new CodeGenerationResult
                {
                    Success = false,
                    Message = "نموذج المجال مطلوب لوضع Code First"
                };
            }

            var validator = new Core.DomainModel.DomainModelValidator();
            var validation = validator.Validate(context.DomainModel);
            if (!validation.IsValid)
            {
                return new CodeGenerationResult
                {
                    Success = false,
                    Message = "أخطاء في نموذج المجال",
                    Errors = validation.Errors
                };
            }

            var generator = new Core.CodeFirst.EfCoreCodeFirstGenerator(_logger);
            var result = await generator.GenerateFullProjectAsync(context.DomainModel, context);

            if (result.Success && context.ApplyMigration && !string.IsNullOrWhiteSpace(context.MigrationName))
            {
                var migrationService = new EfCoreMigrationService(_logger);
                var migResult = await migrationService.AddMigrationAsync(
                    context.MigrationName, context.OutputPath);

                if (migResult.Success)
                {
                    var updateResult = await migrationService.UpdateDatabaseAsync(context.OutputPath);
                    if (!updateResult.Success)
                    {
                        result.Warnings ??= new List<string>();
                        result.Warnings.Add($"تم توليد الكود بنجاح لكن فشل تحديث قاعدة البيانات: {updateResult.ErrorOutput}");
                    }
                }
                else
                {
                    result.Warnings ??= new List<string>();
                    result.Warnings.Add($"تم توليد الكود بنجاح لكن فشل إنشاء Migration: {migResult.ErrorOutput}");
                }
            }

            return result;
        }
    }
} 