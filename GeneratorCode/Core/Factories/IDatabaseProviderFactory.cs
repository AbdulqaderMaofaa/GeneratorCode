using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// Factory لموفري قواعد البيانات
    /// </summary>
    public interface IDatabaseProviderFactory
    {
        /// <summary>
        /// إنشاء موفر قاعدة بيانات بناءً على النوع
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <returns>موفر قاعدة البيانات أو null إذا لم يوجد</returns>
        IDatabaseProvider CreateProvider(DatabaseType databaseType);
        
        /// <summary>
        /// الحصول على جميع موفري قواعد البيانات المدعومة
        /// </summary>
        /// <returns>قائمة الموفرين</returns>
        List<IDatabaseProvider> GetSupportedProviders();
        
        /// <summary>
        /// الحصول على أنواع قواعد البيانات المدعومة
        /// </summary>
        /// <returns>قائمة الأنواع</returns>
        List<DatabaseType> GetSupportedDatabaseTypes();
        
        /// <summary>
        /// التحقق من دعم نوع قاعدة بيانات معين
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <returns>true إذا كان مدعوماً</returns>
        bool IsDatabaseTypeSupported(DatabaseType databaseType);
        
        /// <summary>
        /// تسجيل موفر قاعدة بيانات جديد
        /// </summary>
        /// <param name="provider">موفر قاعدة البيانات</param>
        void RegisterProvider(IDatabaseProvider provider);
        
        /// <summary>
        /// إلغاء تسجيل موفر قاعدة بيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        void UnregisterProvider(DatabaseType databaseType);
    }
} 