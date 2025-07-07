using System.Linq;
using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.DependencyInjection
{
    /// <summary>
    /// خدمة تكامل Dependency Injection مع الأنماط المعمارية
    /// </summary>
    public class DIIntegrationService
    {
        private readonly IDIProviderFactory _diProviderFactory;
        
        public DIIntegrationService(IDIProviderFactory diProviderFactory)
        {
            _diProviderFactory = diProviderFactory;
        }
        
        /// <summary>
        /// تحديث النمط المعماري ليدعم DI
        /// </summary>
        /// <param name="pattern">النمط المعماري</param>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>النمط المحدث</returns>
        public IArchitecturePattern IntegrateDIWithPattern(IArchitecturePattern pattern, CodeGenerationContext context)
        {
            if (!context.DIOptions.EnableDI)
                return pattern;
                
            var diProvider = _diProviderFactory.CreateProvider(context.DIOptions.PreferredContainer);
            if (diProvider == null)
                return pattern;
            
            // إضافة Using statements مطلوبة للـ DI
            var requiredUsings = diProvider.GetRequiredUsings();
            if (context.AdditionalData.ContainsKey("RequiredUsings"))
            {
                var existingUsings = (string[])context.AdditionalData["RequiredUsings"];
                context.AdditionalData["RequiredUsings"] = existingUsings.Concat(requiredUsings).Distinct().ToArray();
            }
            else
            {
                context.AdditionalData["RequiredUsings"] = requiredUsings.ToArray();
            }
            
            // إضافة التبعيات المطلوبة
            var requiredPackages = diProvider.GetRequiredPackages();
            context.AdditionalData["RequiredPackages"] = requiredPackages;
            
            // إضافة معلومات DI للسياق
            context.AdditionalData["DIProvider"] = diProvider;
            context.AdditionalData["DIContainerType"] = context.DIOptions.PreferredContainer.ToString();
            
            return pattern;
        }
        
        /// <summary>
        /// توليد كود DI للنمط المعماري
        /// </summary>
        /// <param name="context">سياق توليد الكود</param>
        /// <returns>كود DI</returns>
        public string GenerateDICodeForPattern(CodeGenerationContext context)
        {
            if (!context.DIOptions.EnableDI)
                return string.Empty;
                
            var diProvider = _diProviderFactory.CreateProvider(context.DIOptions.PreferredContainer);
            if (diProvider == null)
                return string.Empty;
                
            switch (context.ArchitecturePattern)
            {
                case "CleanArchitecture":
                    return GenerateCleanArchitectureDI(diProvider, context);
                case "LayeredArchitecture":
                    return GenerateLayeredArchitectureDI(diProvider, context);
                case "CQRS":
                    return GenerateCQRSDI(diProvider, context);
                default:
                    return diProvider.GenerateServiceExtensions(context);
            }
        }
        
        private string GenerateCleanArchitectureDI(IDependencyInjectionProvider diProvider, CodeGenerationContext context)
        {
            // تخصيص DI للـ Clean Architecture
            return diProvider.GenerateServiceExtensions(context);
        }
        
        private string GenerateLayeredArchitectureDI(IDependencyInjectionProvider diProvider, CodeGenerationContext context)
        {
            // تخصيص DI للـ Layered Architecture
            return diProvider.GenerateServiceExtensions(context);
        }
        
        private string GenerateCQRSDI(IDependencyInjectionProvider diProvider, CodeGenerationContext context)
        {
            // تخصيص DI للـ CQRS
            return diProvider.GenerateServiceExtensions(context);
        }
    }
} 