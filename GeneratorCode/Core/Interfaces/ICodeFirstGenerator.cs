using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    public interface ICodeFirstGenerator
    {
        Task<CodeGenerationResult> GenerateEntitiesAsync(DomainModel.DomainModel model, CodeGenerationContext context);
        Task<GeneratedFile> GenerateDbContextAsync(DomainModel.DomainModel model, CodeGenerationContext context);
        Task<List<GeneratedFile>> GenerateFluentConfigurationAsync(DomainModel.DomainModel model, CodeGenerationContext context);
        Task<GeneratedFile> GenerateProjectFileAsync(DomainModel.DomainModel model, CodeGenerationContext context);
        Task<CodeGenerationResult> GenerateFullProjectAsync(DomainModel.DomainModel model, CodeGenerationContext context);
    }
}
