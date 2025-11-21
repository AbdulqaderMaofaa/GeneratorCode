using System;
using System.Collections.Generic;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// واجهة Logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// تسجيل خطأ
        /// </summary>
        /// <param name="message">الرسالة</param>
        /// <param name="exception">الاستثناء (اختياري)</param>
        /// <param name="source">المصدر (اختياري)</param>
        /// <param name="additionalData">بيانات إضافية (اختياري)</param>
        void LogError(string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null);

        /// <summary>
        /// تسجيل تحذير
        /// </summary>
        /// <param name="message">الرسالة</param>
        /// <param name="exception">الاستثناء (اختياري)</param>
        /// <param name="source">المصدر (اختياري)</param>
        /// <param name="additionalData">بيانات إضافية (اختياري)</param>
        void LogWarning(string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null);

        /// <summary>
        /// تسجيل معلومات
        /// </summary>
        /// <param name="message">الرسالة</param>
        /// <param name="source">المصدر (اختياري)</param>
        /// <param name="additionalData">بيانات إضافية (اختياري)</param>
        void LogInfo(string message, string? source = null, Dictionary<string, object>? additionalData = null);

        /// <summary>
        /// تسجيل معلومات تفصيلية (Debug)
        /// </summary>
        /// <param name="message">الرسالة</param>
        /// <param name="source">المصدر (اختياري)</param>
        /// <param name="additionalData">بيانات إضافية (اختياري)</param>
        void LogDebug(string message, string? source = null, Dictionary<string, object>? additionalData = null);

        /// <summary>
        /// تسجيل حسب المستوى
        /// </summary>
        /// <param name="level">مستوى السجل</param>
        /// <param name="message">الرسالة</param>
        /// <param name="exception">الاستثناء (اختياري)</param>
        /// <param name="source">المصدر (اختياري)</param>
        /// <param name="additionalData">بيانات إضافية (اختياري)</param>
        void Log(LogLevel level, string message, Exception? exception = null, string? source = null, Dictionary<string, object>? additionalData = null);
    }
}

