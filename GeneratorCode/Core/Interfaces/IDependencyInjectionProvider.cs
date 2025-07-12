using System.Collections.Generic;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    /// <summary>
    /// واجهة موفر Dependency Injection
    /// </summary>
    public interface IDependencyInjectionProvider
    {
        /// <summary>
        /// اسم موفر DI
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// نوع DI Container
        /// </summary>
        DIContainerType ContainerType { get; }
        
        /// <summary>
        /// توليد كود تكوين DI
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>كود تكوين DI</returns>
        DIConfigurationResult GenerateConfiguration(CodeGenerationContext context);
        
        /// <summary>
        /// توليد Service Extensions
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>كود Service Extensions</returns>
        string GenerateServiceExtensions(CodeGenerationContext context);
        
        /// <summary>
        /// توليد Startup Configuration
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>كود Startup Configuration</returns>
        string GenerateStartupConfiguration(CodeGenerationContext context);
        
        /// <summary>
        /// الحصول على التبعيات المطلوبة لـ DI
        /// </summary>
        /// <returns>قائمة التبعيات</returns>
        List<string> GetRequiredPackages();
        
        /// <summary>
        /// الحصول على Using Statements المطلوبة
        /// </summary>
        /// <returns>قائمة Using Statements</returns>
        List<string> GetRequiredUsings();
    }
} 