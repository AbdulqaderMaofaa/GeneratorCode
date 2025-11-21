using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// Service لقراءة الـ logs من ملفات JSON
    /// </summary>
    public class LogReaderService
    {
        private readonly LogFileManager _fileManager;
        private readonly JsonSerializerOptions _jsonOptions;

        public LogReaderService(LogFileManager? fileManager = null)
        {
            _fileManager = fileManager ?? new LogFileManager();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// قراءة جميع الـ logs من جميع الملفات
        /// </summary>
        public List<LogEntry> GetAllLogs()
        {
            var allLogs = new List<LogEntry>();
            var logsDirectory = _fileManager.LogsDirectory;

            if (!Directory.Exists(logsDirectory))
                return allLogs;

            var logFiles = Directory.GetFiles(logsDirectory, "*.json");

            foreach (var file in logFiles)
            {
                try
                {
                    var logs = ReadLogsFromFile(file);
                    allLogs.AddRange(logs);
                }
                catch
                {
                    // تجاهل الملفات التالفة
                }
            }

            return allLogs.OrderByDescending(l => l.Timestamp).ToList();
        }

        /// <summary>
        /// تصفية حسب Level
        /// </summary>
        public List<LogEntry> GetLogsByLevel(LogLevel level)
        {
            return GetAllLogs().Where(l => l.Level == level).ToList();
        }

        /// <summary>
        /// تصفية حسب التاريخ
        /// </summary>
        public List<LogEntry> GetLogsByDate(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            
            return GetAllLogs()
                .Where(l => l.Timestamp >= startDate && l.Timestamp < endDate)
                .ToList();
        }

        /// <summary>
        /// تصفية حسب نطاق تاريخي
        /// </summary>
        public List<LogEntry> GetLogsByDateRange(DateTime start, DateTime end)
        {
            return GetAllLogs()
                .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                .ToList();
        }

        /// <summary>
        /// بحث في الرسائل والمصادر
        /// </summary>
        public List<LogEntry> SearchLogs(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllLogs();

            var searchLower = searchText.ToLower();
            return GetAllLogs()
                .Where(l => 
                    (l.Message?.ToLower().Contains(searchLower) ?? false) ||
                    (l.Source?.ToLower().Contains(searchLower) ?? false) ||
                    (l.Exception?.Message?.ToLower().Contains(searchLower) ?? false))
                .ToList();
        }

        /// <summary>
        /// الحصول على قائمة ملفات الـ logs
        /// </summary>
        public List<string> GetLogFiles()
        {
            var logsDirectory = _fileManager.LogsDirectory;
            if (!Directory.Exists(logsDirectory))
                return new List<string>();

            return Directory.GetFiles(logsDirectory, "*.json")
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .ToList();
        }

        /// <summary>
        /// تنسيق السجل للعرض المنسق
        /// </summary>
        public string FormatLogEntry(LogEntry entry, string? format = null)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = @"[{{timestamp}}] [{{level}}] {{source}}
Message: {{message}}
{{#if exception}}
Exception: {{exception.type}}: {{exception.message}}
{{#if exception.stackTrace}}
StackTrace:
{{exception.stackTrace}}
{{/if}}
{{/if}}
---";
            }

            var sb = new StringBuilder(format);

            // استبدال المتغيرات البسيطة
            sb.Replace("{{timestamp}}", entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Replace("{{level}}", entry.Level.ToString().ToUpper());
            sb.Replace("{{source}}", entry.Source ?? "Unknown");
            sb.Replace("{{message}}", entry.Message ?? "");

            // معالجة الاستثناء
            if (entry.Exception != null)
            {
                sb.Replace("{{exception.type}}", entry.Exception.Type ?? "Exception");
                sb.Replace("{{exception.message}}", entry.Exception.Message ?? "");
                
                if (!string.IsNullOrEmpty(entry.Exception.StackTrace))
                {
                    sb.Replace("{{exception.stackTrace}}", entry.Exception.StackTrace);
                }
                else
                {
                    // إزالة قسم StackTrace إذا لم يكن موجوداً
                    var stackTraceStart = sb.ToString().IndexOf("StackTrace:");
                    if (stackTraceStart >= 0)
                    {
                        var stackTraceEnd = sb.ToString().IndexOf("\n", stackTraceStart);
                        if (stackTraceEnd >= 0)
                        {
                            sb.Remove(stackTraceStart, stackTraceEnd - stackTraceStart + 1);
                        }
                    }
                }
            }
            else
            {
                // إزالة قسم الاستثناء بالكامل
                var exceptionStart = sb.ToString().IndexOf("{{#if exception}}");
                if (exceptionStart >= 0)
                {
                    var exceptionEnd = sb.ToString().IndexOf("{{/if}}", exceptionStart);
                    if (exceptionEnd >= 0)
                    {
                        sb.Remove(exceptionStart, exceptionEnd - exceptionStart + 7);
                    }
                }
            }

            // إزالة علامات الشرطية المتبقية
            sb.Replace("{{#if exception}}", "");
            sb.Replace("{{#if exception.stackTrace}}", "");
            sb.Replace("{{/if}}", "");

            return sb.ToString();
        }

        /// <summary>
        /// قراءة الـ logs من ملف واحد
        /// </summary>
        private List<LogEntry> ReadLogsFromFile(string filePath)
        {
            var logs = new List<LogEntry>();

            if (!File.Exists(filePath))
                return logs;

            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var logEntry = JsonSerializer.Deserialize<LogEntry>(line, _jsonOptions);
                        if (logEntry != null)
                        {
                            logs.Add(logEntry);
                        }
                    }
                    catch
                    {
                        // تجاهل السطور التالفة
                    }
                }
            }
            catch
            {
                // تجاهل الملفات التالفة
            }

            return logs;
        }
    }
}

