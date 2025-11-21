using GeneratorCode.Core.Logging;
using System;
using System.Text;
using System.Windows.Forms;

namespace GeneratorCode.Forms
{
    /// <summary>
    /// نافذة عرض تفاصيل السجل الكاملة
    /// </summary>
    public partial class FrmLogDetails : Form
    {
        #region Private Fields

        private readonly LogEntry _logEntry;

        #endregion

        #region Constructors

        public FrmLogDetails(LogEntry logEntry)
        {
            _logEntry = logEntry ?? throw new ArgumentNullException(nameof(logEntry));
            InitializeComponent();
            LoadLogDetails();
        }

        #endregion

        #region Business Logic Methods

        private void LoadLogDetails()
        {
            var sb = new StringBuilder();

            // معلومات أساسية
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine("تفاصيل السجل");
            sb.AppendLine("═══════════════════════════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine($"المعرف: {_logEntry.Id}");
            sb.AppendLine($"التاريخ والوقت: {_logEntry.Timestamp:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"المستوى: {_logEntry.Level}");
            sb.AppendLine($"المصدر: {_logEntry.Source ?? "غير محدد"}");
            sb.AppendLine();
            sb.AppendLine("───────────────────────────────────────────────────────────");
            sb.AppendLine("الرسالة:");
            sb.AppendLine("───────────────────────────────────────────────────────────");
            sb.AppendLine(_logEntry.Message ?? "");
            sb.AppendLine();

            // تفاصيل الاستثناء
            if (_logEntry.Exception != null)
            {
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                sb.AppendLine("تفاصيل الاستثناء");
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"النوع: {_logEntry.Exception.Type}");
                sb.AppendLine($"الرسالة: {_logEntry.Exception.Message}");
                sb.AppendLine();

                if (!string.IsNullOrEmpty(_logEntry.Exception.StackTrace))
                {
                    sb.AppendLine("───────────────────────────────────────────────────────────");
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine("───────────────────────────────────────────────────────────");
                    sb.AppendLine(_logEntry.Exception.StackTrace);
                    sb.AppendLine();
                }

                // البيانات الإضافية للاستثناء
                if (_logEntry.Exception.Data != null && _logEntry.Exception.Data.Count > 0)
                {
                    sb.AppendLine("───────────────────────────────────────────────────────────");
                    sb.AppendLine("بيانات الاستثناء:");
                    sb.AppendLine("───────────────────────────────────────────────────────────");
                    foreach (var kvp in _logEntry.Exception.Data)
                    {
                        sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                    }
                    sb.AppendLine();
                }

                // الاستثناء الداخلي
                if (_logEntry.Exception.InnerException != null)
                {
                    sb.AppendLine("═══════════════════════════════════════════════════════════");
                    sb.AppendLine("الاستثناء الداخلي");
                    sb.AppendLine("═══════════════════════════════════════════════════════════");
                    AppendExceptionDetails(sb, _logEntry.Exception.InnerException, 1);
                }
            }

            // StackTrace العام
            if (!string.IsNullOrEmpty(_logEntry.StackTrace))
            {
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                sb.AppendLine("Stack Trace العام:");
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                sb.AppendLine(_logEntry.StackTrace);
                sb.AppendLine();
            }

            // البيانات الإضافية
            if (_logEntry.AdditionalData != null && _logEntry.AdditionalData.Count > 0)
            {
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                sb.AppendLine("البيانات الإضافية:");
                sb.AppendLine("═══════════════════════════════════════════════════════════");
                foreach (var kvp in _logEntry.AdditionalData)
                {
                    sb.AppendLine($"{kvp.Key}: {kvp.Value}");
                }
            }

            rtbDetails.Text = sb.ToString();
            rtbDetails.SelectionStart = 0;
            rtbDetails.ScrollToCaret();
        }

        private static void AppendExceptionDetails(StringBuilder sb, ExceptionDetails exception, int depth)
        {
            var indent = new string(' ', depth * 2);
            sb.AppendLine($"{indent}النوع: {exception.Type}");
            sb.AppendLine($"{indent}الرسالة: {exception.Message}");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(exception.StackTrace))
            {
                sb.AppendLine($"{indent}Stack Trace:");
                sb.AppendLine(exception.StackTrace);
                sb.AppendLine();
            }

            if (exception.InnerException != null)
            {
                sb.AppendLine($"{indent}الاستثناء الداخلي:");
                AppendExceptionDetails(sb, exception.InnerException, depth + 1);
            }
        }

        #endregion
    }
}
