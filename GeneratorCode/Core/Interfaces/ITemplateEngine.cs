using System.Collections.Generic;
using GeneratorCode.Core.Models;
using System.Threading.Tasks;

namespace GeneratorCode.Core.Interfaces
{
    /// <summary>
    /// واجهة لمحرك القوالب
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// تحميل قالب من ملف
        /// </summary>
        /// <param name="templatePath">مسار القالب</param>
        /// <returns>محتوى القالب</returns>
        string LoadTemplate(string templatePath);
        
        /// <summary>
        /// معالجة القالب بالبيانات
        /// </summary>
        /// <param name="template">القالب</param>
        /// <param name="data">البيانات</param>
        /// <returns>النتيجة النهائية</returns>
        string ProcessTemplate(string template, Dictionary<string, object> data);
        
        /// <summary>
        /// الحصول على القوالب المتاحة لنمط معماري معين
        /// </summary>
        /// <param name="architecturePattern">النمط المعماري</param>
        /// <returns>قائمة القوالب</returns>
        List<TemplateInfo> GetAvailableTemplates(string architecturePattern);
        
        /// <summary>
        /// إنشاء قالب جديد
        /// </summary>
        /// <param name="templateInfo">معلومات القالب</param>
        /// <param name="templateContent">محتوى القالب</param>
        /// <returns>true إذا نجح الإنشاء</returns>
        bool CreateTemplate(TemplateInfo templateInfo, string templateContent);
        
        /// <summary>
        /// حذف قالب
        /// </summary>
        /// <param name="templatePath">مسار القالب</param>
        /// <returns>true إذا نجح الحذف</returns>
        bool DeleteTemplate(string templatePath);
        
        /// <summary>
        /// التحقق من صحة القالب
        /// </summary>
        /// <param name="template">القالب</param>
        /// <returns>نتيجة التحقق</returns>
        TemplateValidationResult ValidateTemplate(string template);

        Task<string> LoadTemplateAsync(string templateName);
        string RenderTemplate(string template, object data);
    }
} 