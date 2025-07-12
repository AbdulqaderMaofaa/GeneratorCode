using System;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Services;

namespace GeneratorCode.CLI
{
    public class CommandLineInterface
    {
        private readonly CodeGenerationService _codeGenerationService;

        public CommandLineInterface(CodeGenerationService codeGenerationService)
        {
            _codeGenerationService = codeGenerationService;
        }

        public RootCommand BuildRootCommand()
        {
            var serverOption = new Option<string>("--server", "اسم السيرفر (مثال: localhost)");
            var databaseOption = new Option<string>("--database", "اسم قاعدة البيانات");
            var dbTypeOption = new Option<DatabaseType>("--db-type", () => DatabaseType.SqlServer, "نوع قاعدة البيانات (SqlServer, MySQL, PostgreSQL)");
            var namespaceOption = new Option<string>("--namespace", () => "GeneratedCode", "مساحة الأسماء للكود المولد");
            var patternOption = new Option<string>("--pattern", () => "CleanArchitecture", "نمط البنية المعمارية (CleanArchitecture, CQRS, etc)");
            var outputOption = new Option<string>("--output", "مسار حفظ الملفات المولدة");
            var enableDiOption = new Option<bool>("--enable-di", () => true, "تفعيل Dependency Injection");
            var asyncOption = new Option<bool>("--async", () => true, "توليد عمليات غير متزامنة");
            var testsOption = new Option<bool>("--tests", () => false, "توليد اختبارات وحدة");

            var rootCommand = new RootCommand("مولد الكود المتطور - واجهة سطر الأوامر");
            rootCommand.AddOption(serverOption);
            rootCommand.AddOption(databaseOption);
            rootCommand.AddOption(dbTypeOption);
            rootCommand.AddOption(namespaceOption);
            rootCommand.AddOption(patternOption);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(enableDiOption);
            rootCommand.AddOption(asyncOption);
            rootCommand.AddOption(testsOption);

            rootCommand.SetHandler(
                (ctx) => HandleCommand(
                    ctx.ParseResult.GetValueForOption(serverOption),
                    ctx.ParseResult.GetValueForOption(databaseOption),
                    ctx.ParseResult.GetValueForOption(dbTypeOption),
                    ctx.ParseResult.GetValueForOption(namespaceOption),
                    ctx.ParseResult.GetValueForOption(patternOption),
                    ctx.ParseResult.GetValueForOption(outputOption),
                    ctx.ParseResult.GetValueForOption(enableDiOption),
                    ctx.ParseResult.GetValueForOption(asyncOption),
                    ctx.ParseResult.GetValueForOption(testsOption)
                ));

            return rootCommand;
        }

        private async Task HandleCommand(
            string server,
            string database,
            DatabaseType dbType,
            string @namespace,
            string pattern,
            string output,
            bool enableDi,
            bool async,
            bool tests)
        {
            var context = new CodeGenerationContext
            {
                ConnectionString = BuildConnectionString(server, database, dbType),
                DatabaseType = dbType,
                Namespace = @namespace,
                ArchitecturePattern = pattern,
                OutputPath = output,
                TargetLanguage = ProgrammingLanguage.CSharp,
                
                DIOptions = new DIOptions
                {
                    EnableDI = enableDi,
                    PreferredContainer = DIContainerType.MicrosoftDI,
                    GenerateServiceExtensions = true
                },
                
                Options = new GenerationOptions
                {
                    GenerateControllers = true,
                    GenerateServices = true,
                    GenerateRepositories = true,
                    GenerateModels = true,
                    GenerateDTOs = true,
                    GenerateValidators = true,
                    GenerateUnitTests = tests,
                    EnableDependencyInjection = enableDi,
                    EnableAsyncOperations = async
                }
            };

            var result = await _codeGenerationService.GenerateCodeAsync(context);
            if (result.Success)
            {
                Console.WriteLine("✅ تم توليد الكود بنجاح");
                Console.WriteLine($"المسار: {output}");
            }
            else
            {
                Console.WriteLine($"❌ فشل في توليد الكود: {result.Message}");
            }
        }

        private string BuildConnectionString(string server, string database, DatabaseType dbType)
        {
            return dbType switch
            {
                DatabaseType.SqlServer => $"Server={server};Database={database};Trusted_Connection=True;",
                DatabaseType.MySql => $"Server={server};Database={database};Uid=root;Pwd=;",
                DatabaseType.PostgreSql => $"Host={server};Database={database};Username=postgres;Password=;",
                _ => throw new ArgumentException($"نوع قاعدة البيانات غير مدعوم: {dbType}")
            };
        }
    }
} 