using System;
using System.IO;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// مدير ملفات السجلات
    /// </summary>
    public class LogFileManager
    {
        private readonly string _logsDirectory;
        private readonly int _retentionDays;

        public LogFileManager(int retentionDays = 30)
        {
            _retentionDays = retentionDays;
            _logsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GeneratorCode",
                "Logs"
            );

            EnsureLogsDirectoryExists();
        }

        /// <summary>
        /// التأكد من وجود مجلد السجلات
        /// </summary>
        private void EnsureLogsDirectoryExists()
        {
            if (!Directory.Exists(_logsDirectory))
            {
                Directory.CreateDirectory(_logsDirectory);
            }
        }

        /// <summary>
        /// الحصول على مسار ملف السجل لليوم المحدد
        /// </summary>
        /// <param name="level">مستوى السجل</param>
        /// <param name="date">التاريخ (افتراضي: اليوم)</param>
        /// <returns>مسار الملف</returns>
        public string GetLogFilePath(LogLevel level, DateTime? date = null)
        {
            var logDate = date ?? DateTime.UtcNow;
            var levelName = level.ToString().ToLower();
            var fileName = $"{levelName}-{logDate:yyyy-MM-dd}.json";
            return Path.Combine(_logsDirectory, fileName);
        }

        /// <summary>
        /// تنظيف الملفات القديمة
        /// </summary>
        public void CleanOldLogs()
        {
            try
            {
                if (!Directory.Exists(_logsDirectory))
                    return;

                var cutoffDate = DateTime.UtcNow.AddDays(-_retentionDays);
                var logFiles = Directory.GetFiles(_logsDirectory, "*.json");

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTimeUtc < cutoffDate)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                            // تجاهل الأخطاء عند حذف الملفات القديمة
                        }
                    }
                }
            }
            catch
            {
                // تجاهل الأخطاء في عملية التنظيف
            }
        }

        /// <summary>
        /// الحصول على مجلد السجلات
        /// </summary>
        public string LogsDirectory => _logsDirectory;
    }
}

