using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    public class MigrationResult
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
        public string ErrorOutput { get; set; } = string.Empty;
        public string ScriptContent { get; set; }
        public DatabaseType TargetDatabase { get; set; }
        public string ProviderName { get; set; }
        public List<string> AppliedMigrations { get; set; } = new();
    }
}
