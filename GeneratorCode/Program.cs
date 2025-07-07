using GeneratorCode.CLI;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Services;
using GeneratorCode.Core.TemplateEngine;
using GeneratorCode.Forms;
using System;
using System.CommandLine;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneratorCode.Properties;
using System.IO;

namespace GeneratorCode
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task<int> Main(string[] args)
        {
            try
            {
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // التحقق من وجود ملف الإعدادات وإنشائه إذا لم يكن موجوداً
             
                
                // إظهار رسالة ترحيب
                //ShowWelcomeMessage();
                
                // تهيئة الخدمات
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

                // التحقق من وجود معاملات سطر الأوامر
                if (args.Length > 0)
                {
                    // تشغيل واجهة سطر الأوامر
                    var cli = new CommandLineInterface(codeGenerationService);
                    return await cli.BuildRootCommand().InvokeAsync(args);
                }
                else
                {
                    // تشغيل واجهة المستخدم الرسومية
                    Application.Run(new FrmConnection());
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"حدث خطأ أثناء بدء تشغيل البرنامج: {ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
                return 1;
            }
        }
        
        private static void ShowWelcomeMessage()
        {
            var result = MessageBox.Show(
                "مرحباً بك في مولد الكود المتطور!\n\n" +
                "الميزات الجديدة:\n" +
                "✓ دعم قواعد بيانات متعددة (SQL Server, MySQL, PostgreSQL)\n" +
                "✓ أنماط معمارية متطورة (Clean Architecture, CQRS, Layered)\n" +
                "✓ دعم Dependency Injection (Microsoft DI, Autofac)\n" +
                "✓ توليد ملفات التكوين والتبعيات تلقائياً\n" +
                "✓ كود حديث مع أفضل الممارسات\n" +
                "✓ دعم العمليات غير المتزامنة\n" +
                "✓ Health Checks والتسجيل المدمج\n\n" +
                "هل تريد المتابعة؟",
                "مولد الكود المتطور - الإصدار 2.0",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );
            
            if (result == DialogResult.No)
            {
                Environment.Exit(0);
            }
        }
    }
}
