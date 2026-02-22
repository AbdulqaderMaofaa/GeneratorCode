using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    public interface IMigrationService
    {
        Task<MigrationResult> AddMigrationAsync(string name, string projectPath, IProgress<string> progress = null, CancellationToken cancellationToken = default);
        Task<MigrationResult> UpdateDatabaseAsync(string projectPath, string connectionString = null, IProgress<string> progress = null, CancellationToken cancellationToken = default);
        Task<MigrationResult> RollbackAsync(string migrationName, string projectPath, IProgress<string> progress = null, CancellationToken cancellationToken = default);
        Task<MigrationResult> GenerateScriptAsync(string from, string to, string projectPath, IProgress<string> progress = null, CancellationToken cancellationToken = default);
        Task<MigrationResult> RemoveLastMigrationAsync(string projectPath, IProgress<string> progress = null, CancellationToken cancellationToken = default);
        Task<List<MigrationInfo>> ListMigrationsAsync(string projectPath, CancellationToken cancellationToken = default);
        Task<bool> IsToolInstalledAsync(CancellationToken cancellationToken = default);
    }
}
