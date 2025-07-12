using System;
using System.Collections.Generic;
using System.Linq;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.ArchitecturePatterns;

namespace GeneratorCode.Core.Factories
{
    /// <summary>
    /// Factory للأنماط المعمارية
    /// </summary>
    public class ArchitecturePatternFactory : IArchitecturePatternFactory
    {
        private readonly Dictionary<string, Func<IArchitecturePattern>> _patterns;
        
        public ArchitecturePatternFactory()
        {
            _patterns = new Dictionary<string, Func<IArchitecturePattern>>(StringComparer.OrdinalIgnoreCase);
            
            // تسجيل الأنماط المعمارية المدعومة
            RegisterDefaultPatterns();
        }
        
        /// <summary>
        /// تسجيل الأنماط المعمارية الافتراضية
        /// </summary>
        private void RegisterDefaultPatterns()
        {
            _patterns["CleanArchitecture"] = () => new CleanArchitecturePattern();
            _patterns["LayeredArchitecture"] = () => new LayeredArchitecturePattern();
            _patterns["MicroservicesArchitecture"] = () => new MicroservicesArchitecturePattern();
            _patterns["DDD"] = () => new DomainDrivenDesignPattern();
            _patterns["CQRS"] = () => new CQRSPattern();
            _patterns["SimpleArchitecture"] = () => new SimpleArchitecturePattern();
        }
        
        /// <summary>
        /// إنشاء نمط معماري بناءً على الاسم
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        /// <returns>النمط المعماري أو null إذا لم يوجد</returns>
        public IArchitecturePattern CreatePattern(string patternName)
        {
            if (string.IsNullOrEmpty(patternName))
                return null;
                
            if (_patterns.TryGetValue(patternName.Trim().Replace(" ",""), out var factory))
            {
                return factory();
            }
            
            return null;
        }
        
        /// <summary>
        /// الحصول على جميع الأنماط المعمارية المدعومة
        /// </summary>
        /// <returns>قائمة الأنماط المعمارية</returns>
        public List<IArchitecturePattern> GetSupportedPatterns()
        {
            return _patterns.Values.Select(factory => factory()).ToList();
        }
        
        /// <summary>
        /// الحصول على أسماء الأنماط المعمارية المدعومة
        /// </summary>
        /// <returns>قائمة الأسماء</returns>
        public List<string> GetSupportedPatternNames()
        {
            return _patterns.Keys.ToList();
        }
        
        /// <summary>
        /// التحقق من دعم نمط معماري معين
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        /// <returns>true إذا كان مدعوماً</returns>
        public bool IsPatternSupported(string patternName)
        {
            return !string.IsNullOrEmpty(patternName) && _patterns.ContainsKey(patternName);
        }
        
        /// <summary>
        /// تسجيل نمط معماري جديد
        /// </summary>
        /// <param name="pattern">النمط المعماري</param>
        public void RegisterPattern(IArchitecturePattern pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
                
            _patterns[pattern.Name] = () => pattern;
        }
        
        /// <summary>
        /// إلغاء تسجيل نمط معماري
        /// </summary>
        /// <param name="patternName">اسم النمط المعماري</param>
        public void UnregisterPattern(string patternName)
        {
            if (!string.IsNullOrEmpty(patternName))
            {
                _patterns.Remove(patternName);
            }
        }
    }
} 