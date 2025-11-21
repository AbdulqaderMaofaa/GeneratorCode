namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// مستويات التسجيل (Logging Levels)
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// معلومات تفصيلية للتطوير والتصحيح
        /// </summary>
        Debug = 0,
        
        /// <summary>
        /// معلومات عامة عن سير العمل
        /// </summary>
        Info = 1,
        
        /// <summary>
        /// تحذيرات قد تحتاج انتباه
        /// </summary>
        Warning = 2,
        
        /// <summary>
        /// أخطاء تحتاج معالجة فورية
        /// </summary>
        Error = 3
    }
}

