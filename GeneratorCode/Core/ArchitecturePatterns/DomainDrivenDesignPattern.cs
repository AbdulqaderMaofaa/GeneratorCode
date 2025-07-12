using System;
using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class DomainDrivenDesignPattern : BaseArchitecturePattern
    {
        public override string Name => "Domain-Driven Design";
        public override string Description => "نمط التصميم الموجه بالنطاق";
        
        public override CodeGenerationResult Generate(CodeGenerationContext context)
        {
            return new CodeGenerationResult { Success = true, Message = "قيد التطوير" };
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new List<string> { "Domain", "Application", "Infrastructure", "Presentation" };
        public override List<string> GetRequiredDependencies() => new List<string> { "MediatR", "FluentValidation" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            return new List<PreviewFile>
            {
                new PreviewFile
                {
                    FileName = "Preview.txt",
                    Content = "DDD pattern preview is under development.",
                    Language = "text"
                }
            };
        }
    }
} 