using System;
using System.Threading;
using System.Windows.Forms;

namespace GeneratorCode.Core.Logging
{
    /// <summary>
    /// Middleware لالتقاط ومعالجة الاستثناءات تلقائياً
    /// </summary>
    public static class ExceptionMiddleware
    {
        private static ILogger? _logger;

        /// <summary>
        /// تهيئة Global Exception Handlers
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // معالج الاستثناءات للـ UI Thread
            Application.ThreadException += Application_ThreadException;

            // معالج الاستثناءات للـ Non-UI Threads
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// معالج الاستثناءات للـ UI Thread
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception, "UI Thread Exception");
        }

        /// <summary>
        /// معالج الاستثناءات للـ Non-UI Threads
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                var additionalData = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "IsTerminating", e.IsTerminating }
                };
                HandleException(exception, "Unhandled Exception", additionalData);
            }
        }

        /// <summary>
        /// معالجة الاستثناء
        /// </summary>
        private static void HandleException(Exception exception, string context, System.Collections.Generic.Dictionary<string, object>? additionalData = null)
        {
            try
            {
                _logger?.LogError(
                    $"Unhandled exception in {context}: {exception.Message}",
                    exception,
                    context,
                    additionalData
                );
            }
            catch
            {
                // إذا فشل التسجيل، تجاهل لتجنب الحلقات اللانهائية
            }
        }

        /// <summary>
        /// إزالة المعالجات (للتنظيف)
        /// </summary>
        public static void Dispose()
        {
            Application.ThreadException -= Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }
    }
}

