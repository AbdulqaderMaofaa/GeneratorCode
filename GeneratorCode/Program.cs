using GeneratorCode.CLI;
using GeneratorCode.Core.CodeFirst;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Services;
using GeneratorCode.Core.TemplateEngine;
using GeneratorCode.Core.Logging;
using GeneratorCode.Forms;
using System;
using System.CommandLine;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GeneratorCode
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            // تهيئة Logger أولاً
            var logger = LoggerFactory.Default;
            
            // تهيئة Global Exception Handlers
            ExceptionMiddleware.Initialize(logger);
            
            try
            {
                logger.LogInfo("Application started", "GeneratorCode.Program");
                
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                // التحقق من وجود ملف الإعدادات وإنشائه إذا لم يكن موجوداً
             
                
                // إظهار رسالة ترحيب
                //ShowWelcomeMessage();
                
                // تهيئة الخدمات
                var templateEngine = new SimpleTemplateEngine(logger);
                var patternFactory = new ArchitecturePatternFactory(templateEngine);
                var databaseFactory = new DatabaseProviderFactory();
                var diProviderFactory = new DIProviderFactory();
                
                var codeGenerationService = new CodeGenerationService(
                    patternFactory,
                    databaseFactory,
                    diProviderFactory,
                    templateEngine,
                    logger
                );
                
                logger.LogInfo("Services initialized successfully", "GeneratorCode.Program");

                // التحقق من وجود معاملات سطر الأوامر
                if (args.Length > 0)
                {
                    logger.LogInfo($"Running CLI mode with {args.Length} arguments", "GeneratorCode.Program");
                    // تشغيل واجهة سطر الأوامر (async)
                    var cli = new CommandLineInterface(codeGenerationService);
                    var result = cli.BuildRootCommand().InvokeAsync(args).GetAwaiter().GetResult();
                    logger.LogInfo($"CLI execution completed with exit code: {result}", "GeneratorCode.Program");
                    return result;
                }
                else
                {
                    logger.LogInfo("Running GUI mode", "GeneratorCode.Program");
                    // تشغيل واجهة المستخدم الرسومية
                    Application.Run(new FrmConnection());
                    logger.LogInfo("Application closed", "GeneratorCode.Program");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Fatal error during application startup", ex, "GeneratorCode.Program");
                MessageBox.Show(
                    $"حدث خطأ فادح أثناء بدء التطبيق:\n{ex.Message}\n\nيرجى مراجعة ملفات السجل في:\n{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GeneratorCode", "Logs")}",
                    "خطأ فادح",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
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
