using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Services;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.TemplateEngine;
using System.Linq;
using GeneratorCode.Forms;

namespace GeneratorCode.Examples
{
    /// <summary>
    /// مثال لاختبار الواجهة الجديدة
    /// </summary>
    public class UITestExample
    {
        /// <summary>
        /// اختبار فتح النموذج الرئيسي بشكل برمجي
        /// </summary>
        public static void TestMainForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var form = new FrmConnection();
            form.ShowDialog();
        }
        
        /// <summary>
        /// اختبار فتح نموذج الجداول مع سياق محدد
        /// </summary>
        public static void TestTablesFormWithContext()
        {
            var context = CreateTestContext();
            var codeGenerationService = CreateCodeGenerationService();
            
            var tablesForm = new FrmTabls(context.ConnectionString, context.DatabaseType.ToString());
            tablesForm.ShowDialog();
        }
        
        /// <summary>
        /// اختبار توليد الكود كاملاً
        /// </summary>
        public static async Task TestCompleteCodeGeneration()
        {
            var context = CreateTestContext();
            var codeGenerationService = CreateCodeGenerationService();
            
            // اختبار الاتصال
            var canConnect = codeGenerationService.TestDatabaseConnection(
                context.DatabaseType, context.ConnectionString);
            
            if (!canConnect)
            {
                MessageBox.Show("فشل في الاتصال بقاعدة البيانات للاختبار", "خطأ", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // الحصول على الجداول
            var tables = codeGenerationService.GetTables(
                context.DatabaseType, context.ConnectionString);
            
            if (tables.Count == 0)
            {
                MessageBox.Show("لا توجد جداول في قاعدة البيانات", "تحذير", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // اختيار أول جدول للاختبار
            var firstTable = tables[0];
            context.TableName = firstTable.Name;
            context.EntityName = firstTable.Name;
            context.ClassName = firstTable.Name;
            context.TableInfo = firstTable;
            
            // توليد الكود
            var result = await codeGenerationService.GenerateCodeAsync(context);
            
            if (result.Success)
            {
                MessageBox.Show(
                    $"تم توليد الكود بنجاح!\n\n" +
                    $"عدد الملفات المولدة: {result.GeneratedFiles.Count}\n" +
                    $"الحجم الإجمالي: {result.TotalSizeInBytes} بايت\n" +
                    $"الرسالة: {result.Message}\n\n" +
                    $"الملفات المولدة:\n" +
                    string.Join("\n", result.GeneratedFiles.Select(f => $"- {f.FileName} ({f.Layer})")),
                    "نجح الاختبار",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"فشل في توليد الكود:\n{result.Message}\n\n" +
                    $"الأخطاء:\n{string.Join("\n", result.Errors)}",
                    "فشل الاختبار",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// اختبار الميزات المختلفة للنظام
        /// </summary>
        public static void TestSystemFeatures()
        {
            var codeGenerationService = CreateCodeGenerationService();

            var features = new System.Text.StringBuilder();
            features.AppendLine("اختبار ميزات النظام:");
            features.AppendLine();

            // اختبار الأنماط المعمارية المدعومة
            var patterns = codeGenerationService.GetSupportedPatterns();
            features.AppendLine($"الأنماط المعمارية المدعومة ({patterns.Length}):");
            foreach (var pattern in patterns)
            {
                features.AppendLine($"  ✓ {pattern}");
            }
            features.AppendLine();

            // اختبار أنواع قواعد البيانات المدعومة
            var dbTypes = codeGenerationService.GetSupportedDatabaseTypes();
            features.AppendLine($"أنواع قواعد البيانات المدعومة ({dbTypes.Length}):");
            foreach (var dbType in dbTypes)
            {
                features.AppendLine($"  ✓ {dbType}");
            }
            features.AppendLine();

            // اختبار أنواع DI Containers المدعومة
            var diTypes = codeGenerationService.GetSupportedDIContainerTypes();
            features.AppendLine($"أنواع DI Containers المدعومة ({diTypes.Length}):");
            foreach (var diType in diTypes)
            {
                features.AppendLine($"  ✓ {diType}");
            }
            features.AppendLine();

            // اختبار التبعيات المطلوبة لـ Microsoft DI
            var packages = codeGenerationService.GetRequiredPackagesForDI(DIContainerType.MicrosoftDI);
            features.AppendLine($"التبعيات المطلوبة لـ Microsoft DI ({packages.Count}):");
            foreach (var package in packages)
            {
                features.AppendLine($"  ✓ {package}");
            }

            MessageBox.Show(features.ToString(), "ميزات النظام",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static CodeGenerationContext CreateTestContext()
        {
            return new CodeGenerationContext
            {
                ConnectionString = "(localdb)\\MSSQLLocalDB;Database=TestDB;Integrated Security=true;",
                DatabaseType = DatabaseType.SqlServer,
                Namespace = "TestApp",
                ArchitecturePattern = "CleanArchitecture",
                TargetLanguage = ProgrammingLanguage.CSharp,
                OutputPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "GeneratedCode"),
                
                DIOptions = new DIOptions
                {
                    EnableDI = true,
                    PreferredContainer = DIContainerType.MicrosoftDI,
                    GenerateServiceExtensions = true,
                    EnableLogging = true,
                    EnableHealthChecks = true,
                    EnableValidation = true
                },
                
                Options = new GenerationOptions
                {
                    GenerateControllers = true,
                    GenerateServices = true,
                    GenerateRepositories = true,
                    GenerateModels = true,
                    GenerateDTOs = true,
                    GenerateValidators = true,
                    GenerateUnitTests = false,
                    EnableDependencyInjection = true,
                    EnableAsyncOperations = true,
                    EnableLogging = true
                }
            };
        }
        
        private static CodeGenerationService CreateCodeGenerationService()
        {
            var patternFactory = new ArchitecturePatternFactory();
            var databaseFactory = new DatabaseProviderFactory();
            var diProviderFactory = new DIProviderFactory();
            var templateEngine = new SimpleTemplateEngine();
            
            return new CodeGenerationService(
                patternFactory,
                databaseFactory,
                diProviderFactory,
                templateEngine
            );
        }
    }
} 