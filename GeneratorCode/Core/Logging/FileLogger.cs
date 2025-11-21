using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// Logger لحفظ السجلات في ملفات JSON
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly LogFileManager _fileManager;
        private readonly SemaphoreSlim _semaphore;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileLogger(LogFileManager? fileManager = null)
        {
            _fileManager = fileManager ?? new LogFileManager();
            _semaphore = new SemaphoreSlim(1, 1);
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // تنظيف الملفات القديمة عند التهيئة
            _fileManager.CleanOldLogs();
        }

        /// <summary>
        /// تسجيل خطأ
        /// </summary>
        public void LogError(string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null)
        {
            Log(LogLevel.Error, message, exception, source, additionalData);
        }

        /// <summary>
        /// تسجيل تحذير
        /// </summary>
        public void LogWarning(string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null)
        {
            Log(LogLevel.Warning, message, exception, source, additionalData);
        }

        /// <summary>
        /// تسجيل معلومات
        /// </summary>
        public void LogInfo(string message, string? source = null, Dictionary<string, object>? additionalData = null)
        {
            Log(LogLevel.Info, message, null, source, additionalData);
        }

        /// <summary>
        /// تسجيل معلومات تفصيلية (Debug)
        /// </summary>
        public void LogDebug(string message, string? source = null, Dictionary<string, object>? additionalData = null)
        {
            Log(LogLevel.Debug, message, null, source, additionalData);
        }

        /// <summary>
        /// تسجيل حسب المستوى
        /// </summary>
        public void Log(LogLevel level, string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null)
        {
            try
            {
                var logEntry = CreateLogEntry(level, message, exception, source, additionalData);
                WriteLogEntryAsync(logEntry, level).ConfigureAwait(false);
            }
            catch
            {
                // تجاهل الأخطاء في عملية التسجيل لتجنب الحلقات اللانهائية
            }
        }

        /// <summary>
        /// إنشاء LogEntry
        /// </summary>
        private LogEntry CreateLogEntry(LogLevel level, string message, Exception? exception, string? source, Dictionary<string, object>? additionalData)
        {
            var entry = new LogEntry
            {
                Level = level,
                Message = message,
                Source = source ?? GetCallingSource(),
                Timestamp = DateTime.UtcNow,
                AdditionalData = additionalData
            };

            if (exception != null)
            {
                entry.Exception = CreateExceptionDetails(exception);
                entry.StackTrace = exception.StackTrace;
            }

            return entry;
        }

        /// <summary>
        /// إنشاء تفاصيل الاستثناء
        /// </summary>
        private ExceptionDetails CreateExceptionDetails(Exception exception)
        {
            var details = new ExceptionDetails
            {
                Type = exception.GetType().FullName ?? exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            };

            if (exception.Data != null && exception.Data.Count > 0)
            {
                details.Data = new Dictionary<string, object>();
                foreach (var key in exception.Data.Keys)
                {
                    if (key != null)
                    {
                        details.Data[key.ToString()!] = exception.Data[key] ?? string.Empty;
                    }
                }
            }

            if (exception.InnerException != null)
            {
                details.InnerException = CreateExceptionDetails(exception.InnerException);
            }

            return details;
        }

        /// <summary>
        /// الحصول على المصدر من StackTrace
        /// </summary>
        private string GetCallingSource()
        {
            try
            {
                var stackTrace = new System.Diagnostics.StackTrace(skipFrames: 3);
                var frame = stackTrace.GetFrame(0);
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                    {
                        var declaringType = method.DeclaringType;
                        if (declaringType != null)
                        {
                            return $"{declaringType.FullName}.{method.Name}";
                        }
                    }
                }
            }
            catch
            {
                // تجاهل الأخطاء
            }

            return "Unknown";
        }

        /// <summary>
        /// كتابة LogEntry في الملف (Thread-safe)
        /// </summary>
        private async Task WriteLogEntryAsync(LogEntry entry, LogLevel level)
        {
            await _semaphore.WaitAsync();
            try
            {
                var filePath = _fileManager.GetLogFilePath(level);
                var json = JsonSerializer.Serialize(entry, _jsonOptions);
                var line = json + Environment.NewLine;

                // استخدام AppendAllTextAsync للكتابة في نهاية الملف
                await File.AppendAllTextAsync(filePath, line, Encoding.UTF8);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

