using System.Collections.Generic;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Models;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// Factory للأنماط المعمارية
    /// </summary>
    public interface IArchitecturePatternFactory
    {
        /// <summary>
        /// إنشاء نمط معماري بناءً على الاسم
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        /// <returns>النمط المعماري أو null إذا لم يوجد</returns>
        IArchitecturePattern CreatePattern(string patternName);
        
        /// <summary>
        /// الحصول على جميع الأنماط المعمارية المدعومة
        /// </summary>
        /// <returns>قائمة الأنماط المعمارية</returns>
        List<IArchitecturePattern> GetSupportedPatterns();
        
        /// <summary>
        /// الحصول على أسماء الأنماط المعمارية المدعومة
        /// </summary>
        /// <returns>قائمة الأسماء</returns>
        List<string> GetSupportedPatternNames();
        
        /// <summary>
        /// التحقق من دعم نمط معماري معين
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        /// <returns>true إذا كان مدعوماً</returns>
        bool IsPatternSupported(string patternName);
        
        /// <summary>
        /// تسجيل نمط معماري جديد
        /// </summary>
        /// <param name="pattern">النمط المعماري</param>
        void RegisterPattern(IArchitecturePattern pattern);
        
        /// <summary>
        /// إلغاء تسجيل نمط معماري
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        void UnregisterPattern(string patternName);
    }
} 