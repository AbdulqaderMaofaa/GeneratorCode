using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

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
            try
            {
                // التحقق من صحة السياق
                var validationResult = ValidateContext(context);
                if (!validationResult.Success)
                    return validationResult;
                
                // الحصول على النمط المعماري
                var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
                if (pattern == null)
                {
                    return new CodeGenerationResult
                    {
                        Success = false,
                        Message = $"النمط المعماري غير مدعوم: {context.ArchitecturePattern}"
                    };
                }
                
                // التحقق من دعم قاعدة البيانات
                if (!pattern.SupportsDatabaseType(context.DatabaseType))
                {
                    return new CodeGenerationResult
                    {
                        Success = false,
                        Message = $"النمط المعماري {pattern.Name} لا يدعم قاعدة البيانات {context.DatabaseType}"
                    };
                }
                
                // الحصول على موفر قاعدة البيانات
                var databaseProvider = _databaseFactory.CreateProvider(context.DatabaseType);
                if (databaseProvider == null)
                {
                    return new CodeGenerationResult
                    {
                        Success = false,
                        Message = $"موفر قاعدة البيانات غير مدعوم: {context.DatabaseType}"
                    };
                }
                
                // تحميل معلومات الجدول إذا لم تكن متوفرة
                context.TableInfo ??= LoadTableInfo(databaseProvider, context);
                
                // توليد الكود باستخدام النمط المعماري
                var result = pattern.Generate(context);
                
                // توليد DI Configuration إذا كان مفعل
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
                
                // حفظ الملفات المولدة
                if (result.Success)
                {
                    await SaveGeneratedFilesAsync(result);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return new CodeGenerationResult
                {
                    Success = false,
                    Message = $"خطأ في توليد الكود: {ex.Message}",
                    Errors = { ex.Message }
                };
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
                var provider = _databaseFactory.CreateProvider(databaseType);
                return provider?.TestConnection(connectionString) ?? false;
            }
            catch
            {
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
                var provider = _databaseFactory.CreateProvider(databaseType);
                return provider?.GetTables(connectionString) ?? new List<TableInfo>();
            }
            catch
            {
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
                var provider = _databaseFactory.CreateProvider(databaseType);
                return provider?.GetColumns(connectionString, tableName) ?? new List<ColumnInfo>();
            }
            catch
            {
                return new List<ColumnInfo>();
            }
        }

        public PreviewResult GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            var result = new PreviewResult();
            
            try
            {
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
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Error = $"حدث خطأ أثناء توليد المعاينة: {ex.Message}";
            }
            
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
                try
                {
                    var directory = System.IO.Path.GetDirectoryName(file.FullPath);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                    
                    await System.IO.File.WriteAllTextAsync(file.FullPath, file.Content);
                    file.SizeInBytes = System.Text.Encoding.UTF8.GetByteCount(file.Content);
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"خطأ في حفظ الملف {file.FileName}: {ex.Message}");
                }
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

        public async Task GenerateInfrastructureLayer(CodeGenerationContext context, string directoryPath)
        {
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            await pattern.GenerateInfrastructureLayerAsync(context, directoryPath);
        }

        public async Task GenerateApplicationLayer(CodeGenerationContext context, string directoryPath)
        {
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            await pattern.GenerateApplicationLayerAsync(context, directoryPath);
        }

        public async Task GenerateDomainLayer(CodeGenerationContext context, string directoryPath)
        {
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            await pattern.GenerateDomainLayerAsync(context, directoryPath);
        }

        public async Task GeneratePresentationLayer(CodeGenerationContext context, string directoryPath)
        {
            var pattern = _patternFactory.CreatePattern(context.ArchitecturePattern);
            await pattern.GeneratePresentationLayerAsync(context, directoryPath);
        }

        public async Task GenerateSolutionFile(CodeGenerationContext context, string projectPath)
        {
            var patternDir = GetTemplateDirectory(context.ArchitecturePattern);
            var templatePath = $"{patternDir}/Solution.template";
            var template = await _templateEngine.LoadTemplateAsync(templatePath);
            var content = _templateEngine.RenderTemplate(template, context);
            var slnPath = Path.Combine(projectPath, $"{context.Namespace}.sln");
            await File.WriteAllTextAsync(slnPath, content);
        }
    }
} 