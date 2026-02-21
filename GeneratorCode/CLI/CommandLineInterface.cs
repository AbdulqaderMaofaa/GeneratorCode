using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using GeneratorCode.Core.Helpers;
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
            var serverOption = new Option<string>("--server", "اسم السيرفر (مثال: localhost)") { IsRequired = true };
            var databaseOption = new Option<string>("--database", "اسم قاعدة البيانات") { IsRequired = true };
            var dbTypeOption = new Option<DatabaseType>("--db-type", () => DatabaseType.SqlServer, "نوع قاعدة البيانات (SqlServer, MySQL, PostgreSQL)");
            var usernameOption = new Option<string>("--username", () => "", "اسم المستخدم لقاعدة البيانات");
            var passwordOption = new Option<string>("--password", () => "", "كلمة المرور لقاعدة البيانات");
            var portOption = new Option<int?>("--port", () => null, "رقم المنفذ (افتراضي حسب نوع قاعدة البيانات)");
            var tableOption = new Option<string>("--table", () => "", "اسم جدول محدد (إذا لم يُحدد يتم التوليد لجميع الجداول)");
            var namespaceOption = new Option<string>("--namespace", () => "GeneratedCode", "مساحة الأسماء للكود المولد");
            var patternOption = new Option<string>("--pattern", () => "CleanArchitecture", "نمط البنية المعمارية (CleanArchitecture, CQRS, etc)");
            var outputOption = new Option<string>("--output", "مسار حفظ الملفات المولدة") { IsRequired = true };
            var enableDiOption = new Option<bool>("--enable-di", () => true, "تفعيل Dependency Injection");
            var asyncOption = new Option<bool>("--async", () => true, "توليد عمليات غير متزامنة");
            var testsOption = new Option<bool>("--tests", () => false, "توليد اختبارات وحدة");

            var rootCommand = new RootCommand("مولد الكود المتطور - واجهة سطر الأوامر");
            rootCommand.AddOption(serverOption);
            rootCommand.AddOption(databaseOption);
            rootCommand.AddOption(dbTypeOption);
            rootCommand.AddOption(usernameOption);
            rootCommand.AddOption(passwordOption);
            rootCommand.AddOption(portOption);
            rootCommand.AddOption(tableOption);
            rootCommand.AddOption(namespaceOption);
            rootCommand.AddOption(patternOption);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(enableDiOption);
            rootCommand.AddOption(asyncOption);
            rootCommand.AddOption(testsOption);

            rootCommand.SetHandler(async (ctx) =>
            {
                var result = ctx.ParseResult;
                await HandleCommand(
                    result.GetValueForOption(serverOption),
                    result.GetValueForOption(databaseOption),
                    result.GetValueForOption(dbTypeOption),
                    result.GetValueForOption(usernameOption),
                    result.GetValueForOption(passwordOption),
                    result.GetValueForOption(portOption),
                    result.GetValueForOption(tableOption),
                    result.GetValueForOption(namespaceOption),
                    result.GetValueForOption(patternOption),
                    result.GetValueForOption(outputOption),
                    result.GetValueForOption(enableDiOption),
                    result.GetValueForOption(asyncOption),
                    result.GetValueForOption(testsOption)
                );
            });

            var codeFirstCommand = BuildCodeFirstCommand();
            rootCommand.AddCommand(codeFirstCommand);

            return rootCommand;
        }

        private async Task HandleCommand(
            string server,
            string database,
            DatabaseType dbType,
            string username,
            string password,
            int? port,
            string table,
            string @namespace,
            string pattern,
            string output,
            bool enableDi,
            bool useAsync,
            bool tests)
        {
            try
            {
                var connectionString = ConnectionStringBuilder.Build(
                    dbType, server, database, username, password, port,
                    useIntegratedSecurity: dbType == DatabaseType.SqlServer && string.IsNullOrEmpty(username));

                Console.WriteLine($"جاري الاتصال بقاعدة البيانات {dbType}...");

                if (!_codeGenerationService.TestDatabaseConnection(dbType, connectionString))
                {
                    Console.WriteLine("فشل الاتصال بقاعدة البيانات. تحقق من بيانات الاتصال.");
                    return;
                }

                Console.WriteLine("تم الاتصال بنجاح.");

                var allTables = _codeGenerationService.GetTables(dbType, connectionString);
                if (allTables == null || allTables.Count == 0)
                {
                    Console.WriteLine("لم يتم العثور على أي جداول في قاعدة البيانات.");
                    return;
                }

                List<TableInfo> tablesToGenerate;
                if (!string.IsNullOrEmpty(table))
                {
                    var matched = allTables.FirstOrDefault(t =>
                        t.Name.Equals(table, StringComparison.OrdinalIgnoreCase));
                    if (matched == null)
                    {
                        Console.WriteLine($"الجدول '{table}' غير موجود. الجداول المتاحة:");
                        foreach (var t in allTables)
                            Console.WriteLine($"  - {t.Name}");
                        return;
                    }
                    tablesToGenerate = new List<TableInfo> { matched };
                }
                else
                {
                    tablesToGenerate = allTables;
                }

                Console.WriteLine($"سيتم توليد الكود لـ {tablesToGenerate.Count} جدول...");

                int successCount = 0;
                int failCount = 0;

                foreach (var tbl in tablesToGenerate)
                {
                    Console.WriteLine($"  توليد: {tbl.Name}...");

                    var context = new CodeGenerationContext
                    {
                        ConnectionString = connectionString,
                        DatabaseType = dbType,
                        TableName = tbl.Name,
                        EntityName = tbl.Name,
                        ClassName = tbl.Name,
                        Namespace = @namespace,
                        ArchitecturePattern = pattern,
                        OutputPath = output,
                        TargetLanguage = ProgrammingLanguage.CSharp,
                        TableInfo = tbl,
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
                            EnableAsyncOperations = useAsync
                        }
                    };

                    var result = await _codeGenerationService.GenerateCodeAsync(context);
                    if (result.Success)
                    {
                        successCount++;
                        Console.WriteLine($"    تم بنجاح ({result.GeneratedFiles.Count} ملف)");
                    }
                    else
                    {
                        failCount++;
                        Console.WriteLine($"    فشل: {result.Message}");
                        foreach (var err in result.Errors)
                            Console.WriteLine($"      - {err}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"اكتمل التوليد: {successCount} نجح، {failCount} فشل");
                Console.WriteLine($"المسار: {output}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ: {ex.Message}");
            }
        }

        private Command BuildCodeFirstCommand()
        {
            var modelPathOption = new Option<string>("--model-path", "مسار ملف نموذج المجال (domain-model.json)") { IsRequired = true };
            var outputOption = new Option<string>("--output", "مسار حفظ الملفات المولدة") { IsRequired = true };
            var migrationNameOption = new Option<string>("--migration-name", () => "", "اسم الـ Migration");
            var applyMigrationOption = new Option<bool>("--apply-migration", () => false, "تطبيق الـ Migration تلقائياً");
            var targetFrameworkOption = new Option<string>("--target-framework", () => "net8.0", "إطار العمل المستهدف (net6.0, net7.0, net8.0, net9.0)");

            var cmd = new Command("codefirst", "توليد مشروع EF Core من نموذج المجال (Code First)");
            cmd.AddOption(modelPathOption);
            cmd.AddOption(outputOption);
            cmd.AddOption(migrationNameOption);
            cmd.AddOption(applyMigrationOption);
            cmd.AddOption(targetFrameworkOption);

            cmd.SetHandler(async (ctx) =>
            {
                var parseResult = ctx.ParseResult;
                await HandleCodeFirstCommand(
                    parseResult.GetValueForOption(modelPathOption),
                    parseResult.GetValueForOption(outputOption),
                    parseResult.GetValueForOption(migrationNameOption),
                    parseResult.GetValueForOption(applyMigrationOption),
                    parseResult.GetValueForOption(targetFrameworkOption)
                );
            });

            return cmd;
        }

        private async Task HandleCodeFirstCommand(string modelPath, string output, string migrationName, bool applyMigration, string targetFramework)
        {
            try
            {
                if (!System.IO.File.Exists(modelPath))
                {
                    Console.WriteLine($"ملف النموذج غير موجود: {modelPath}");
                    return;
                }

                Console.WriteLine("جاري تحميل نموذج المجال...");
                var modelService = new Core.Services.DomainModelService();
                var model = await modelService.LoadModelAsync(modelPath);

                var validation = modelService.ValidateModel();
                if (!validation.IsValid)
                {
                    Console.WriteLine("أخطاء في نموذج المجال:");
                    foreach (var err in validation.Errors)
                        Console.WriteLine($"  - {err}");
                    return;
                }

                model.TargetFramework = targetFramework;

                var context = new CodeGenerationContext
                {
                    Mode = GenerationMode.CodeFirst,
                    DomainModel = model,
                    OutputPath = output,
                    Namespace = model.DefaultNamespace,
                    TargetFramework = targetFramework,
                    MigrationName = migrationName,
                    ApplyMigration = applyMigration,
                    DatabaseType = model.TargetDatabaseType
                };

                Console.WriteLine($"جاري توليد مشروع Code First ({model.Entities.Count} كيان)...");
                var result = await _codeGenerationService.GenerateCodeAsync(context);

                if (result.Success)
                {
                    Console.WriteLine($"تم التوليد بنجاح: {result.GeneratedFiles?.Count ?? 0} ملف");
                    Console.WriteLine($"المسار: {output}");
                    if (result.Warnings?.Count > 0)
                    {
                        Console.WriteLine("تحذيرات:");
                        foreach (var w in result.Warnings)
                            Console.WriteLine($"  - {w}");
                    }
                }
                else
                {
                    Console.WriteLine($"فشل التوليد: {result.Message}");
                    if (result.Errors?.Count > 0)
                        foreach (var err in result.Errors)
                            Console.WriteLine($"  - {err}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ: {ex.Message}");
            }
        }
    }
}
