using System.Collections.Generic;
using System.Data;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Interfaces
{
    /// <summary>
    /// واجهة لموفري قواعد البيانات المختلفة
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// نوع قاعدة البيانات
        /// </summary>
        DatabaseType DatabaseType { get; }
        
        /// <summary>
        /// اسم موفر قاعدة البيانات
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// الحصول على معلومات الجداول
        /// </summary>
        /// <param name="connectionString">نص الاتصال</param>
        /// <returns>قائمة الجداول</returns>
        List<TableInfo> GetTables(string connectionString);
        
        /// <summary>
        /// الحصول على معلومات الأعمدة لجدول محدد
        /// </summary>
        /// <param name="connectionString">نص الاتصال</param>
        /// <param name="tableName">اسم الجدول</param>
        /// <returns>قائمة الأعمدة</returns>
        List<ColumnInfo> GetColumns(string connectionString, string tableName);
        
        /// <summary>
        /// الحصول على المفاتيح الأساسية للجدول
        /// </summary>
        /// <param name="connectionString">نص الاتصال</param>
        /// <param name="tableName">اسم الجدول</param>
        /// <returns>قائمة المفاتيح الأساسية</returns>
        List<string> GetPrimaryKeys(string connectionString, string tableName);
        
        /// <summary>
        /// الحصول على المفاتيح الخارجية للجدول
        /// </summary>
        /// <param name="connectionString">نص الاتصال</param>
        /// <param name="tableName">اسم الجدول</param>
        /// <returns>قائمة المفاتيح الخارجية</returns>
        List<ForeignKeyInfo> GetForeignKeys(string connectionString, string tableName);
        
        /// <summary>
        /// تحويل نوع البيانات من قاعدة البيانات إلى C#
        /// </summary>
        /// <param name="dbType">نوع البيانات في قاعدة البيانات</param>
        /// <param name="isNullable">هل النوع يسمح بالقيم الفارغة</param>
        /// <returns>نوع البيانات في C#</returns>
        string MapDataType(string dbType, bool isNullable);
        
        /// <summary>
        /// بناء نص الاتصال
        /// </summary>
        /// <param name="server">اسم الخادم</param>
        /// <param name="database">اسم قاعدة البيانات</param>
        /// <param name="username">اسم المستخدم</param>
        /// <param name="password">كلمة المرور</param>
        /// <param name="additionalParams">معاملات إضافية</param>
        /// <returns>نص الاتصال</returns>
        string BuildConnectionString(string server, string database, string username, string password, Dictionary<string, string> additionalParams = null);
        
        /// <summary>
        /// اختبار الاتصال بقاعدة البيانات
        /// </summary>
        /// <param name="connectionString">نص الاتصال</param>
        /// <returns>true إذا نجح الاتصال</returns>
        bool TestConnection(string connectionString);
    }
} 