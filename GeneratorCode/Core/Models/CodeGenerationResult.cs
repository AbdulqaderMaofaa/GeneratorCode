using System.Collections.Generic;

namespace GeneratorCode.Core.Models
{
    /// <summary>
    /// نتيجة توليد الكود
    /// </summary>
    public class CodeGenerationResult
    {
        /// <summary>
        /// هل نجح التوليد
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// رسالة النتيجة
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// الملفات المولدة
        /// </summary>
        public List<GeneratedFile> GeneratedFiles { get; set; } = new List<GeneratedFile>();
        
        /// <summary>
        /// الأخطاء
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>
        /// التحذيرات
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();
        
        /// <summary>
        /// معلومات إضافية
        /// </summary>
        public Dictionary<string, object> AdditionalInfo { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// وقت التوليد
        /// </summary>
        public System.TimeSpan GenerationTime { get; set; }
        
        /// <summary>
        /// حجم الكود المولد (بالبايت)
        /// </summary>
        public long TotalSizeInBytes { get; set; }
    }
    
    /// <summary>
    /// ملف مولد
    /// </summary>
    public class GeneratedFile
    {
        /// <summary>
        /// اسم الملف
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// المسار النسبي
        /// </summary>
        public string RelativePath { get; set; }
        
        /// <summary>
        /// المسار الكامل
        /// </summary>
        public string FullPath { get; set; }
        
        /// <summary>
        /// محتوى الملف
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// نوع الملف
        /// </summary>
        public string FileType { get; set; }
        
        /// <summary>
        /// الطبقة التي ينتمي إليها
        /// </summary>
        public string Layer { get; set; }
        
        /// <summary>
        /// حجم الملف (بالبايت)
        /// </summary>
        public long SizeInBytes { get; set; }
        
        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public System.DateTime CreatedDate { get; set; } = System.DateTime.Now;
        
        /// <summary>
        /// معلومات إضافية
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// معلومات القالب
    /// </summary>
    public class TemplateInfo
    {
        /// <summary>
        /// اسم القالب
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// وصف القالب
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// مسار القالب
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// النمط المعماري
        /// </summary>
        public string ArchitecturePattern { get; set; }
        
        /// <summary>
        /// الطبقة
        /// </summary>
        public string Layer { get; set; }
        
        /// <summary>
        /// نوع الملف
        /// </summary>
        public string FileType { get; set; }
        
        /// <summary>
        /// الإصدار
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// المؤلف
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// تاريخ آخر تعديل
        /// </summary>
        public System.DateTime LastModifiedDate { get; set; }
    }
    
    /// <summary>
    /// نتيجة التحقق من صحة القالب
    /// </summary>
    public class TemplateValidationResult
    {
        /// <summary>
        /// هل القالب صحيح
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// الأخطاء
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        
        /// <summary>
        /// التحذيرات
        /// </summary>
        public List<string> Warnings { get; set; } = new List<string>();
        
        /// <summary>
        /// المتغيرات المطلوبة
        /// </summary>
        public List<string> RequiredVariables { get; set; } = new List<string>();
        
        /// <summary>
        /// المتغيرات الاختيارية
        /// </summary>
        public List<string> OptionalVariables { get; set; } = new List<string>();
    }
} 