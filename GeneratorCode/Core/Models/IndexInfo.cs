using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    public class IndexInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Columns { get; set; }
        public bool IsUnique { get; set; }
        public bool IsPrimary { get; set; }
        public string Description { get; set; }
    }
} 