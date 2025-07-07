using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    public class DIConfigurationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<GeneratedFile> ConfigurationFiles { get; set; } = new List<GeneratedFile>();
    }
} 