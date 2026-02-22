using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GeneratorCode.Core.Helpers;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Logging;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Services
{
    public class EfCoreMigrationService : IMigrationService
    {
        private readonly ILogger _logger;
        private static readonly Regex ValidMigrationName = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public EfCoreMigrationService(ILogger logger = null)
        {
            _logger = logger ?? LoggerFactory.Default;
        }

        public async Task<bool> IsToolInstalledAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await ProcessRunner.RunAsync("dotnet", "ef --version", timeoutMs: 15000, cancellationToken: cancellationToken);
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<MigrationResult> AddMigrationAsync(string name, string projectPath,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            ValidateMigrationName(name);
            ValidateProjectPath(projectPath);

            _logger.LogInfo($"Adding migration '{name}' in '{projectPath}'", "EfCoreMigrationService");

            var result = await RunEfCommandAsync($"migrations add {name}", projectPath, progress, cancellationToken);
            return result;
        }

        public async Task<MigrationResult> UpdateDatabaseAsync(string projectPath, string connectionString = null,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            ValidateProjectPath(projectPath);

            _logger.LogInfo($"Updating database from '{projectPath}'", "EfCoreMigrationService");

            var args = "database update";
            if (!string.IsNullOrWhiteSpace(connectionString))
                args += $" --connection \"{connectionString}\"";

            return await RunEfCommandAsync(args, projectPath, progress, cancellationToken);
        }

        public async Task<MigrationResult> RollbackAsync(string migrationName, string projectPath,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            ValidateProjectPath(projectPath);

            _logger.LogInfo($"Rolling back to '{migrationName}' in '{projectPath}'", "EfCoreMigrationService");

            return await RunEfCommandAsync($"database update {migrationName}", projectPath, progress, cancellationToken);
        }

        public async Task<MigrationResult> GenerateScriptAsync(string from, string to, string projectPath,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            ValidateProjectPath(projectPath);

            _logger.LogInfo("Generating SQL script", "EfCoreMigrationService");

            var args = "migrations script --idempotent";
            if (!string.IsNullOrWhiteSpace(from)) args += $" {from}";
            if (!string.IsNullOrWhiteSpace(to)) args += $" {to}";

            var result = await RunEfCommandAsync(args, projectPath, progress, cancellationToken);
            if (result.Success)
                result.ScriptContent = result.Output;

            return result;
        }

        public async Task<MigrationResult> RemoveLastMigrationAsync(string projectPath,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            ValidateProjectPath(projectPath);

            _logger.LogInfo("Removing last migration", "EfCoreMigrationService");

            return await RunEfCommandAsync("migrations remove --force", projectPath, progress, cancellationToken);
        }

        public async Task<List<MigrationInfo>> ListMigrationsAsync(string projectPath,
            CancellationToken cancellationToken = default)
        {
            ValidateProjectPath(projectPath);

            var result = await RunEfCommandAsync("migrations list", projectPath, cancellationToken: cancellationToken);
            var migrations = new List<MigrationInfo>();

            if (!result.Success) return migrations;

            var lines = result.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("Build") || trimmed.StartsWith("info")) continue;

                var isApplied = !trimmed.Contains("(Pending)");
                var name = trimmed.Replace("(Pending)", "").Trim();

                migrations.Add(new MigrationInfo
                {
                    Name = name,
                    Id = name,
                    IsApplied = isApplied
                });
            }

            return migrations;
        }

        private async Task<MigrationResult> RunEfCommandAsync(string efArgs, string projectPath,
            IProgress<string> progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var processResult = await ProcessRunner.RunAsync(
                    "dotnet",
                    $"ef {efArgs}",
                    workingDirectory: projectPath,
                    timeoutMs: 120000,
                    progress: progress,
                    cancellationToken: cancellationToken);

                return new MigrationResult
                {
                    Success = processResult.Success,
                    Output = processResult.StandardOutput,
                    ErrorOutput = processResult.StandardError
                };
            }
            catch (TimeoutException ex)
            {
                _logger.LogError("Migration command timed out", ex, "EfCoreMigrationService");
                return new MigrationResult
                {
                    Success = false,
                    ErrorOutput = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Migration command failed", ex, "EfCoreMigrationService");
                return new MigrationResult
                {
                    Success = false,
                    ErrorOutput = ex.Message
                };
            }
        }

        private static void ValidateMigrationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("اسم الـ Migration مطلوب");

            if (!ValidMigrationName.IsMatch(name))
                throw new ArgumentException($"اسم الـ Migration غير صالح: '{name}'. يجب أن يبدأ بحرف أو _ ويحتوي فقط على حروف وأرقام و _");
        }

        private static void ValidateProjectPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("مسار المشروع مطلوب");

            if (path.Contains(".."))
                throw new ArgumentException("مسار المشروع يحتوي على أحرف غير مسموح بها");
        }
    }
}
