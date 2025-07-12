using System;
using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class LayeredArchitecturePattern : BaseArchitecturePattern
    {
        public override string Name => "Layered Architecture";
        public override string Description => "نمط العمارة الطبقية";
        
        public override CodeGenerationResult Generate(CodeGenerationContext context)
        {
            return new CodeGenerationResult { Success = true, Message = "قيد التطوير" };
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new List<string> { "Data", "Business", "Service", "Presentation" };
        public override List<string> GetRequiredDependencies() => new List<string>();

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            return new List<PreviewFile>
            {
                new PreviewFile
                {
                    FileName = "Preview.txt",
                    Content = "Layered pattern preview is under development.",
                    Language = "text"
                }
            };
        }
    }
} 