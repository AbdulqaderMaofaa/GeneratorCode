using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    public class PreviewResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public List<PreviewFile> Files { get; set; } = new List<PreviewFile>();
    }

    public class PreviewFile
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string Language { get; set; }
    }
} 