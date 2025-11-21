using GeneratorCode.Forms;
using System;
using System.Windows.Forms;

namespace GeneratorCode.GeneratorCode.Helpers
{
    /// <summary>
    /// فئة مساعدة لفتح واجهة عرض السجلات من أي مكان في التطبيق
    /// </summary>
    public static class LogViewerHelper
    {
        /// <summary>
        /// فتح واجهة عرض السجلات
        /// </summary>
        /// <param name="owner">النموذج الأب (اختياري)</param>
        public static void ShowLogViewer(Form owner = null)
        {
            try
            {
                var logViewer = new FrmLogViewer();
                if (owner != null && !owner.IsDisposed)
                {
                    logViewer.ShowDialog(owner);
                }
                else
                {
                    logViewer.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"حدث خطأ أثناء فتح واجهة عرض السجلات:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}

