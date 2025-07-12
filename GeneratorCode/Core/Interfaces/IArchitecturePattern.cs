using System.Collections.Generic;
using System.Threading.Tasks;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    /// <summary>
    /// واجهة للأنماط المعمارية المختلفة
    /// </summary>
    public interface IArchitecturePattern
    {
        /// <summary>
        /// اسم النمط المعماري
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// وصف النمط المعماري
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// توليد الكود وفقاً للنمط المعماري
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>نتيجة توليد الكود</returns>
        Task<CodeGenerationResult> Generate(CodeGenerationContext context);
        
        /// <summary>
        /// التحقق من دعم قاعدة البيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <returns>true إذا كانت مدعومة</returns>
        bool SupportsDatabaseType(DatabaseType dbType);
        
        /// <summary>
        /// الحصول على الطبقات المطلوبة لهذا النمط
        /// </summary>
        /// <returns>قائمة الطبقات</returns>
        List<string> GetRequiredLayers();
        
        /// <summary>
        /// الحصول على التبعيات المطلوبة
        /// </summary>
        /// <returns>قائمة التبعيات</returns>
        List<string> GetRequiredDependencies();

        List<PreviewFile> GeneratePreview(TableInfo table, CodeGenerationContext context);

        Task GenerateInfrastructureLayerAsync(CodeGenerationContext context, string directoryPath);
        Task GenerateApplicationLayerAsync(CodeGenerationContext context, string directoryPath);
        Task GenerateDomainLayerAsync(CodeGenerationContext context, string directoryPath);
        Task GeneratePresentationLayerAsync(CodeGenerationContext context, string directoryPath);
    }
} 