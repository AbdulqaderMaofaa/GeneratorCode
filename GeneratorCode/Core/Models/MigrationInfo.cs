using System;

namespace GeneratorCode.Core.Models
{
    public class MigrationInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public DateTime? AppliedDate { get; set; }
        public bool IsApplied { get; set; }
    }
}
