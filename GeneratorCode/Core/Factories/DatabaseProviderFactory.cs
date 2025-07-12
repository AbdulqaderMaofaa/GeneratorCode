using System;
using System.Collections.Generic;
using System.Linq;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.DatabaseProviders;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// Factory لموفري قواعد البيانات
    /// </summary>
    public class DatabaseProviderFactory : IDatabaseProviderFactory
    {
        private readonly Dictionary<DatabaseType, Func<IDatabaseProvider>> _providers;
        
        public DatabaseProviderFactory()
        {
            _providers = new Dictionary<DatabaseType, Func<IDatabaseProvider>>();
            
            // تسجيل موفري قواعد البيانات الافتراضية
            RegisterDefaultProviders();
        }
        
        /// <summary>
        /// تسجيل موفري قواعد البيانات الافتراضية
        /// </summary>
        private void RegisterDefaultProviders()
        {
            _providers[DatabaseType.SqlServer] = () => new SqlServerProvider();
            _providers[DatabaseType.PostgreSql] = () => new PostgreSqlProvider();
            _providers[DatabaseType.MySql] = () => new MySqlProvider();
        }
        
        /// <summary>
        /// إنشاء موفر قاعدة بيانات بناءً على النوع
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <returns>موفر قاعدة البيانات أو null إذا لم يوجد</returns>
        public IDatabaseProvider CreateProvider(DatabaseType databaseType)
        {
            if (_providers.TryGetValue(databaseType, out var factory))
            {
                return factory();
            }
            
            return null;
        }
        
        /// <summary>
        /// الحصول على جميع موفري قواعد البيانات المدعومة
        /// </summary>
        /// <returns>قائمة الموفرين</returns>
        public List<IDatabaseProvider> GetSupportedProviders()
        {
            return _providers.Values.Select(factory => factory()).ToList();
        }
        
        /// <summary>
        /// الحصول على أنواع قواعد البيانات المدعومة
        /// </summary>
        /// <returns>قائمة الأنواع</returns>
        public List<DatabaseType> GetSupportedDatabaseTypes()
        {
            return _providers.Keys.ToList();
        }
        
        /// <summary>
        /// التحقق من دعم نوع قاعدة بيانات معين
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        /// <returns>true إذا كان مدعوماً</returns>
        public bool IsDatabaseTypeSupported(DatabaseType databaseType)
        {
            return _providers.ContainsKey(databaseType);
        }
        
        /// <summary>
        /// تسجيل موفر قاعدة بيانات جديد
        /// </summary>
        /// <param name="provider">موفر قاعدة البيانات</param>
        public void RegisterProvider(IDatabaseProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
                
            _providers[provider.DatabaseType] = () => provider;
        }
        
        /// <summary>
        /// إلغاء تسجيل موفر قاعدة بيانات
        /// </summary>
        /// <param name="databaseType">نوع قاعدة البيانات</param>
        public void UnregisterProvider(DatabaseType databaseType)
        {
            _providers.Remove(databaseType);
        }
    }
} 