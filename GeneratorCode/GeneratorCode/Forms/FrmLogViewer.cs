using GeneratorCode.Core.Logging;
using GeneratorCode.Helpers;
using GeneratorCode.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace GeneratorCode.Forms
{
    /// <summary>
    /// نافذة عرض الـ Logs
    /// </summary>
    public partial class FrmLogViewer : Form
    {
        #region Private Fields

        private readonly LogReaderService _logReader;
        private readonly ILogger _logger;
        private List<LogEntry> _allLogs;
        private List<LogEntry> _filteredLogs;

        #endregion

        #region Constructors

        public FrmLogViewer()
        {
            _logger = LoggerFactory.Default;
            _logReader = new LogReaderService();
            _allLogs = new List<LogEntry>();
            _filteredLogs = new List<LogEntry>();

            InitializeComponent();
            SetupEventHandlers();
            SetupTooltips();
            LoadLogs();
        }

        #endregion

        #region Initialization Methods

        private void SetupTooltips()
        {
            var tip = AppTheme.CreateTooltipProvider();
            tip.SetToolTip(btnRefresh, "تحديث السجلات (F5)");
            tip.SetToolTip(btnClearFilters, "مسح جميع الفلاتر");
            tip.SetToolTip(btnViewDetails, "عرض تفاصيل السجل المحدد");
            tip.SetToolTip(btnExport, "تصدير السجلات إلى ملف");
            tip.SetToolTip(btnClose, "إغلاق النافذة (Escape)");
            tip.SetToolTip(cmbLevelFilter, "تصفية حسب مستوى السجل");
            tip.SetToolTip(txtSearch, "بحث في الرسائل والمصادر");
            tip.SetToolTip(dgvTableView, "انقر مرتين لعرض التفاصيل");
        }

        private void SetupEventHandlers()
        {
            rdoFormattedView.CheckedChanged += RdoViewMode_CheckedChanged;
            rdoTableView.CheckedChanged += RdoViewMode_CheckedChanged;

            cmbLevelFilter.SelectedIndexChanged += FilterChanged;
            dtpDateFilter.ValueChanged += FilterChanged;
            txtSearch.TextChanged += FilterChanged;
            txtSourceFilter.TextChanged += FilterChanged;

            btnRefresh.Click += BtnRefresh_Click;
            btnViewDetails.Click += BtnViewDetails_Click;
            btnExport.Click += BtnExport_Click;
            btnClose.Click += (s, e) => Close();
            btnClearFilters.Click += BtnClearFilters_Click;

            dgvTableView.CellDoubleClick += DgvTableView_CellDoubleClick;

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
                if (e.KeyCode == Keys.F5) { LoadLogs(); e.SuppressKeyPress = true; }
            };

            ApplyViewMode();
        }

        #endregion

        #region Event Handlers

        private void RdoViewMode_CheckedChanged(object sender, EventArgs e)
        {
            ApplyViewMode();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            cmbLevelFilter.SelectedIndex = 0;
            dtpDateFilter.Checked = false;
            txtSearch.Clear();
            txtSourceFilter.Clear();
        }

        private void DgvTableView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvTableView.Rows[e.RowIndex];
            if (row.DataBoundItem != null)
            {
                var logEntry = ((dynamic)row.DataBoundItem).LogEntry as LogEntry;
                if (logEntry != null)
                {
                    var detailsForm = new FrmLogDetails(logEntry);
                    detailsForm.ShowDialog();
                }
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            LogEntry selectedLog = null;

            if (rdoTableView.Checked && dgvTableView.SelectedRows.Count > 0)
            {
                var row = dgvTableView.SelectedRows[0];
                if (row.DataBoundItem != null)
                {
                    selectedLog = ((dynamic)row.DataBoundItem).LogEntry as LogEntry;
                }
            }

            if (selectedLog == null)
            {
                MessageBox.Show(
                    "الرجاء اختيار سجل لعرض تفاصيله",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            var detailsForm = new FrmLogDetails(selectedLog);
            detailsForm.ShowDialog();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (_filteredLogs.Count == 0)
            {
                MessageBox.Show(
                    "لا توجد سجلات للتصدير",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON Files (*.json)|*.json|CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt";
                saveDialog.FilterIndex = rdoFormattedView.Checked ? 3 : 1;
                saveDialog.FileName = $"logs_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportLogs(saveDialog.FileName, saveDialog.FilterIndex);
                        MessageBox.Show(
                            "تم التصدير بنجاح",
                            "نجاح",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error exporting logs", ex, "FrmLogViewer.BtnExport_Click");
                        MessageBox.Show(
                            $"حدث خطأ أثناء التصدير:\n{ex.Message}",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        #endregion

        #region Business Logic Methods

        private void LoadLogs()
        {
            try
            {
                _logger.LogInfo("Loading logs in LogViewer", "FrmLogViewer.LoadLogs");
                _allLogs = _logReader.GetAllLogs();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading logs", ex, "FrmLogViewer.LoadLogs");
                MessageBox.Show(
                    $"حدث خطأ أثناء تحميل السجلات:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// تطبيق جميع الفلترات بشكل متراكم على السجلات
        /// الترتيب: Level -> التاريخ -> Source -> البحث
        /// كل فلتر يطبق على النتائج المفلترة من الفلترات السابقة
        /// </summary>
        private void ApplyFilters()
        {
            // البدء من جميع السجلات
            _filteredLogs = _allLogs.ToList();

            // 1. تصفية حسب Level (إذا تم اختيار level معين)
            // تطبق أولاً لأنها تقلل عدد السجلات بشكل كبير
            if (cmbLevelFilter.SelectedIndex > 0)
            {
                // Mapping صحيح: ComboBox Index -> LogLevel
                // Index 1 = "Error" -> LogLevel.Error (3)
                // Index 2 = "Warning" -> LogLevel.Warning (2)
                // Index 3 = "Info" -> LogLevel.Info (1)
                // Index 4 = "Debug" -> LogLevel.Debug (0)
                LogLevel selectedLevel = cmbLevelFilter.SelectedIndex switch
                {
                    1 => LogLevel.Error,    // "Error"
                    2 => LogLevel.Warning,  // "Warning"
                    3 => LogLevel.Info,      // "Info"
                    4 => LogLevel.Debug,    // "Debug"
                    _ => LogLevel.Debug     // Default (لا يجب الوصول هنا)
                };
                _filteredLogs = _filteredLogs.Where(l => l.Level == selectedLevel).ToList();
            }

            // 2. تصفية حسب التاريخ - تطبيق على _filteredLogs الحالية (بعد فلترة Level)
            // مثال: إذا تم اختيار Level=Info وتاريخ=اليوم، ستظهر فقط سجلات Info في اليوم المحدد
            if (dtpDateFilter.Checked)
            {
                var startDate = dtpDateFilter.Value.Date;
                var endDate = startDate.AddDays(1);
                _filteredLogs = _filteredLogs
                    .Where(l => l.Timestamp >= startDate && l.Timestamp < endDate)
                    .ToList();
            }

            // 3. تصفية حسب Source - تطبيق على _filteredLogs الحالية (بعد فلترة Level والتاريخ)
            if (!string.IsNullOrWhiteSpace(txtSourceFilter.Text))
            {
                var sourceFilter = txtSourceFilter.Text.ToLower();
                _filteredLogs = _filteredLogs
                    .Where(l => l.Source?.ToLower().Contains(sourceFilter) ?? false)
                    .ToList();
            }

            // 4. البحث عن نص - تطبيق على _filteredLogs الحالية (بعد جميع الفلترات السابقة)
            // مثال: بحث عن "test" في Level=Info وتاريخ=اليوم
            // النتيجة: سجلات Info فقط في اليوم المحدد والتي تحتوي على "test" في Message أو Source أو Exception
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var searchText = txtSearch.Text.ToLower();
                _filteredLogs = _filteredLogs
                    .Where(l =>
                        (l.Message?.ToLower().Contains(searchText) ?? false) ||
                        (l.Source?.ToLower().Contains(searchText) ?? false) ||
                        (l.Exception?.Message?.ToLower().Contains(searchText) ?? false))
                    .ToList();
            }

            // تحديث العرض بعد تطبيق جميع الفلترات
            UpdateDisplay();
            UpdateLogCount();
        }

        private void UpdateDisplay()
        {
            if (rdoFormattedView.Checked)
            {
                DisplayFormattedView();
            }
            else
            {
                DisplayTableView();
            }
        }

        private void DisplayFormattedView()
        {
            rtbFormattedView.Clear();

            foreach (var log in _filteredLogs)
            {
                var formattedText = _logReader.FormatLogEntry(log);
                var startIndex = rtbFormattedView.TextLength;

                rtbFormattedView.AppendText(formattedText);
                rtbFormattedView.AppendText(Environment.NewLine);

                // تطبيق الألوان حسب Level
                var color = GetLevelColor(log.Level);
                rtbFormattedView.Select(startIndex, formattedText.Length);
                rtbFormattedView.SelectionColor = color;
                rtbFormattedView.SelectionLength = 0;
            }

            rtbFormattedView.SelectionStart = 0;
            rtbFormattedView.ScrollToCaret();
        }

        private void DisplayTableView()
        {
            dgvTableView.DataSource = null;
            dgvTableView.Rows.Clear();

            var bindingSource = new BindingSource
            {
                DataSource = _filteredLogs.Select(l => new
                {
                    Timestamp = l.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    Level = l.Level.ToString(),
                    Message = l.Message ?? "",
                    Source = l.Source ?? "",
                    HasException = l.Exception != null ? "نعم" : "لا",
                    LogEntry = l
                }).ToList()
            };

            dgvTableView.DataSource = bindingSource;

            // تطبيق الألوان على الصفوف
            foreach (DataGridViewRow row in dgvTableView.Rows)
            {
                var logEntry = ((dynamic)row.DataBoundItem).LogEntry as LogEntry;
                if (logEntry != null)
                {
                    var color = GetLevelColor(logEntry.Level);
                    row.DefaultCellStyle.BackColor = Color.FromArgb(50, color.R, color.G, color.B);
                    row.DefaultCellStyle.ForeColor = color;
                }
            }
        }

        private Color GetLevelColor(LogLevel level)
        {
            return AppTheme.GetLogLevelColor(level.ToString());
        }

        private void UpdateLogCount()
        {
            lblLogCount.Text = $"عدد السجلات: {_filteredLogs.Count} من {_allLogs.Count}";
        }

        private void ApplyViewMode()
        {
            rtbFormattedView.Visible = rdoFormattedView.Checked;
            dgvTableView.Visible = rdoTableView.Checked;
            UpdateDisplay();
        }

        private void ExportLogs(string filePath, int filterIndex)
        {
            if (filterIndex == 1) // JSON
            {
                var json = JsonSerializer.Serialize(_filteredLogs, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, json);
            }
            else if (filterIndex == 2) // CSV
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Timestamp,Level,Message,Source,HasException");
                foreach (var log in _filteredLogs)
                {
                    csv.AppendLine($"{log.Timestamp:yyyy-MM-dd HH:mm:ss},{log.Level},\"{log.Message?.Replace("\"", "\"\"")}\",{log.Source},{log.Exception != null}");
                }
                System.IO.File.WriteAllText(filePath, csv.ToString());
            }
            else // TXT (Formatted)
            {
                var text = new System.Text.StringBuilder();
                foreach (var log in _filteredLogs)
                {
                    text.AppendLine(_logReader.FormatLogEntry(log));
                    text.AppendLine();
                }
                System.IO.File.WriteAllText(filePath, text.ToString());
            }
        }

        #endregion

        #region Form Events

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // حفظ إعدادات العرض
            var settings = Settings.Default;
            settings.LogViewerDefaultViewMode = rdoFormattedView.Checked ? "Formatted" : "Table";
            settings.Save();

            base.OnFormClosing(e);
        }

        #endregion
    }
}
