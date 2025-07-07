using System;
using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class CQRSPattern : BaseArchitecturePattern
    {
        public override string Name => "CQRS";
        public override string Description => "نمط فصل القراءة عن الكتابة";
        
        public override CodeGenerationResult Generate(CodeGenerationContext context)
        {
            return new CodeGenerationResult { Success = true, Message = "قيد التطوير" };
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new List<string> { "Commands", "Queries", "Handlers" };
        public override List<string> GetRequiredDependencies() => new List<string> { "MediatR" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            return new List<PreviewFile>
            {
                new()
                {
                    FileName = "Preview.txt",
                    Content = "CQRS pattern preview is under development.",
                    Language = "text"
                }
            };
        }
    }
} 