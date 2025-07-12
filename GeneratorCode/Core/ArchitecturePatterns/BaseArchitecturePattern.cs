using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.ArchitecturePatterns
{
    public abstract class BaseArchitecturePattern : IArchitecturePattern
    {
        public abstract string Name { get; }
        public virtual string Description => $"{Name} pattern implementation";
        public abstract bool SupportsDatabaseType(DatabaseType dbType);
        public abstract Task<CodeGenerationResult> Generate(CodeGenerationContext context);
        public abstract List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context);

        public virtual List<string> GetRequiredLayers()
        {
            return new List<string> { "Infrastructure", "Application", "Domain", "Presentation" };
        }

        public virtual List<string> GetRequiredDependencies()
        {
            return new List<string>();
        }

        public virtual async Task GenerateInfrastructureLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            await Task.CompletedTask;
        }

        public virtual async Task GenerateApplicationLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            await Task.CompletedTask;
        }

        public virtual async Task GenerateDomainLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            await Task.CompletedTask;
        }

        public virtual async Task GeneratePresentationLayerAsync(CodeGenerationContext context, string directoryPath)
        {
            await Task.CompletedTask;
        }
    }
} 