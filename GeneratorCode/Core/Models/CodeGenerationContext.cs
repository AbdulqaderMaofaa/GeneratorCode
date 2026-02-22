using System.Collections.Generic;
using GeneratorCode.Core.DomainModel;

namespace GeneratorCode.Core.Models
{
    /// <summary>
    /// سياق توليد الكود
    /// </summary>
    public class CodeGenerationContext
    {
        /// <summary>
        /// نص الاتصال بقاعدة البيانات
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// نوع قاعدة البيانات
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
        
        /// <summary>
        /// اسم الجدول
        /// </summary>
        public string TableName { get; set; }
        
        /// <summary>
        /// اسم الكيان
        /// </summary>
        public string EntityName { get; set; }
        
        /// <summary>
        /// اسم الكلاس
        /// </summary>
        public string ClassName { get; set; }
        
        /// <summary>
        /// مساحة الأسماء
        /// </summary>
        public string Namespace { get; set; }
        
        /// <summary>
        /// مسار الحفظ
        /// </summary>
        public string OutputPath { get; set; }
        
        /// <summary>
        /// النمط المعماري المحدد
        /// </summary>
        public string ArchitecturePattern { get; set; }
        
        /// <summary>
        /// لغة البرمجة المستهدفة
        /// </summary>
        public ProgrammingLanguage TargetLanguage { get; set; }
        
        /// <summary>
        /// معلومات الجدول
        /// </summary>
        public TableInfo TableInfo { get; set; }
        
        /// <summary>
        /// الطبقات المطلوب توليدها
        /// </summary>
        public List<string> RequiredLayers { get; set; } = new List<string>();
        
        /// <summary>
        /// خيارات التوليد
        /// </summary>
        public GenerationOptions Options { get; set; } = new GenerationOptions();
        
        /// <summary>
        /// بيانات إضافية
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// خيارات Dependency Injection
        /// </summary>
        public DIOptions DIOptions { get; set; } = new DIOptions();
        
        /// <summary>
        /// خيارات التحقق
        /// </summary>
        public ValidationOptions ValidationOptions { get; set; } = new ValidationOptions();
        
        /// <summary>
        /// خيارات الاختبار
        /// </summary>
        public TestingOptions TestingOptions { get; set; } = new TestingOptions();

        /// <summary>
        /// إطار العمل المستهدف (مثل net6.0, net7.0, net8.0)
        /// </summary>
        public string TargetFramework { get; set; } = "net8.0";

        /// <summary>
        /// وضع التوليد (Database First أو Code First)
        /// </summary>
        public GenerationMode Mode { get; set; } = GenerationMode.DatabaseFirst;

        /// <summary>
        /// نموذج المجال (يستخدم في وضع Code First)
        /// </summary>
        public DomainModel.DomainModel DomainModel { get; set; }

        /// <summary>
        /// اسم الـ Migration (يستخدم في وضع Code First)
        /// </summary>
        public string MigrationName { get; set; }

        /// <summary>
        /// تطبيق الـ Migration تلقائياً
        /// </summary>
        public bool ApplyMigration { get; set; }
    }
} 