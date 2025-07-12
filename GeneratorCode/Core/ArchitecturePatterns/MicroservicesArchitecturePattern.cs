using System;
using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class MicroservicesArchitecturePattern : BaseArchitecturePattern
    {
        public override string Name => "Microservices";
        public override string Description => "نمط الخدمات المصغرة";
        
        public override CodeGenerationResult Generate(CodeGenerationContext context)
        {
            return new CodeGenerationResult { Success = true, Message = "قيد التطوير" };
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new List<string> { "API", "Domain", "Infrastructure", "Gateway" };
        public override List<string> GetRequiredDependencies() => new List<string> { "Ocelot", "RabbitMQ.Client" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            return new List<PreviewFile>
            {
                new PreviewFile
                {
                    FileName = "Preview.txt",
                    Content = "Microservices pattern preview is under development.",
                    Language = "text"
                }
            };
        }
    }
} 