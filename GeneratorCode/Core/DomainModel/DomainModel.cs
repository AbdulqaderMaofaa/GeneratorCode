using System.Collections.Generic;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DomainModel
{
    public class DomainModel
    {
        public string ProjectName { get; set; } = string.Empty;

        public string DefaultNamespace { get; set; } = string.Empty;

        public string TargetFramework { get; set; } = "net8.0";

        public DatabaseType TargetDatabaseType { get; set; } = DatabaseType.SqlServer;

        public List<DomainEntity> Entities { get; set; } = new();

        public string ConnectionString { get; set; } = string.Empty;

        public string OutputPath { get; set; } = string.Empty;
    }
}
