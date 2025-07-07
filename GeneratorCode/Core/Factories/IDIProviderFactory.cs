using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// واجهة Factory لموفري Dependency Injection
    /// </summary>
    public interface IDIProviderFactory
    {
        /// <summary>
        /// إنشاء موفر DI بناءً على النوع
        /// </summary>
        /// <param name="containerType">نوع Container</param>
        /// <returns>موفر DI</returns>
        IDependencyInjectionProvider CreateProvider(DIContainerType containerType);
        
        /// <summary>
        /// الحصول على جميع موفري DI المدعومة
        /// </summary>
        /// <returns>قائمة الموفرين</returns>
        List<IDependencyInjectionProvider> GetSupportedProviders();
        
        /// <summary>
        /// الحصول على أنواع Containers المدعومة
        /// </summary>
        /// <returns>قائمة الأنواع</returns>
        List<DIContainerType> GetSupportedContainerTypes();
        
        /// <summary>
        /// التحقق من دعم نوع Container معين
        /// </summary>
        /// <param name="containerType">نوع Container</param>
        /// <returns>true إذا كان مدعوماً</returns>
        bool IsContainerTypeSupported(DIContainerType containerType);
        
        /// <summary>
        /// تسجيل موفر DI جديد
        /// </summary>
        /// <param name="provider">موفر DI</param>
        void RegisterProvider(IDependencyInjectionProvider provider);
    }
} 