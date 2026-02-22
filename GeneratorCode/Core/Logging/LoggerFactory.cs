namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// Factory لإنشاء Logger instances (Singleton Pattern)
    /// </summary>
    public static class LoggerFactory
    {
        private static ILogger? _defaultLogger;
        private static readonly object _lock = new();

        /// <summary>
        /// الحصول على Logger الافتراضي (Singleton)
        /// </summary>
        public static ILogger Default
        {
            get
            {
                if (_defaultLogger == null)
                {
                    lock (_lock)
                    {
                        if (_defaultLogger == null)
                        {
                            _defaultLogger = CreateLogger();
                        }
                    }
                }
                return _defaultLogger;
            }
        }

        /// <summary>
        /// إنشاء Logger جديد
        /// </summary>
        /// <param name="fileManager">LogFileManager (اختياري)</param>
        /// <returns>Logger instance</returns>
        public static ILogger CreateLogger(LogFileManager? fileManager = null)
        {
            return new FileLogger(fileManager);
        }

        /// <summary>
        /// إعادة تعيين Logger الافتراضي (للاستخدام في الاختبارات)
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _defaultLogger = null;
            }
        }
    }
}

