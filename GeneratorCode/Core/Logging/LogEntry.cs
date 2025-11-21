using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// نموذج لإدخال السجل (Log Entry)
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// الوقت والتاريخ
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// مستوى السجل
        /// </summary>
        [JsonPropertyName("level")]
        public LogLevel Level { get; set; }

        /// <summary>
        /// الرسالة
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// المصدر (اسم الكلاس/الطريقة)
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// تفاصيل الاستثناء (إذا كان موجود)
        /// </summary>
        [JsonPropertyName("exception")]
        public ExceptionDetails? Exception { get; set; }

        /// <summary>
        /// Stack Trace (للأخطاء)
        /// </summary>
        [JsonPropertyName("stackTrace")]
        public string? StackTrace { get; set; }

        /// <summary>
        /// بيانات إضافية
        /// </summary>
        [JsonPropertyName("additionalData")]
        public Dictionary<string, object>? AdditionalData { get; set; }

        /// <summary>
        /// معرف فريد للسجل
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// تفاصيل الاستثناء
    /// </summary>
    public class ExceptionDetails
    {
        /// <summary>
        /// نوع الاستثناء
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// رسالة الاستثناء
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Stack Trace
        /// </summary>
        [JsonPropertyName("stackTrace")]
        public string? StackTrace { get; set; }

        /// <summary>
        /// الاستثناء الداخلي
        /// </summary>
        [JsonPropertyName("innerException")]
        public ExceptionDetails? InnerException { get; set; }

        /// <summary>
        /// البيانات الإضافية للاستثناء
        /// </summary>
        [JsonPropertyName("data")]
        public Dictionary<string, object>? Data { get; set; }
    }
}

