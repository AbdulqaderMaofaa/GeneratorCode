using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public class DomainDrivenDesignPattern : BaseArchitecturePattern
    {
        public override string Name => "Domain-Driven Design";
        public override string Description => "نمط التصميم الموجه بالنطاق";
        
        public override async Task<CodeGenerationResult> Generate(CodeGenerationContext context)
        {
            await Task.CompletedTask;
            return new CodeGenerationResult { Success = true, Message = "قيد التطوير" };
        }
        
        public override bool SupportsDatabaseType(DatabaseType databaseType) => true;
        public override List<string> GetRequiredLayers() => new() { "Domain", "Application", "Infrastructure", "Presentation" };
        public override List<string> GetRequiredDependencies() => new() { "MediatR", "FluentValidation" };

        public override List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context)
        {
            return new List<PreviewFile>
            {
                new() {
                    FileName = "Preview.txt",
                    Content = "DDD pattern preview is under development.",
                    Language = "text"
                }
            };
        }
    }
} 