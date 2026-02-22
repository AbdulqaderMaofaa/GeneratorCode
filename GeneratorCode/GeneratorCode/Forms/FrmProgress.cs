using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    /// <summary>
    /// نافذة إظهار التقدم بنمط CMD
    /// </summary>
    public partial class FrmProgress : Form
    {
        private RichTextBox txtOutput;
        private ProgressBar progressBar;
        private Button btnClose;
        private Button btnOpenFolder;
        private Button btnCancel;
        private Button btnCopyLog;
        private Label lblStatus;
        private CancellationTokenSource _cts;
        private int totalFiles = 0;
        private int completedFiles = 0;
        private string projectPath = "";

        public CancellationToken CancellationToken => _cts?.Token ?? CancellationToken.None;

        public FrmProgress()
        {
            _cts = new CancellationTokenSource();
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            txtOutput = new RichTextBox();
            progressBar = new ProgressBar();
            btnClose = new Button();
            btnOpenFolder = new Button();
            lblStatus = new Label();
            SuspendLayout();

            // 
            // txtOutput
            // 
            txtOutput.BackColor = AppTheme.ConsoleBg;
            txtOutput.ForeColor = AppTheme.ConsoleSuccess;
            txtOutput.Font = AppTheme.ConsoleFontSmall;
            txtOutput.Location = new Point(12, 12);
            txtOutput.ReadOnly = true;
            txtOutput.RightToLeft = RightToLeft.No;
            txtOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtOutput.Size = new Size(760, 400);
            txtOutput.TabIndex = 0;
            txtOutput.Text = "";

            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 420);
            progressBar.Size = new Size(410, 23);
            progressBar.TabIndex = 1;
            progressBar.Style = ProgressBarStyle.Continuous;

            // 
            // btnCopyLog
            // 
            btnCopyLog = new Button();
            AppTheme.StyleButton(btnCopyLog, AppTheme.Primary);
            btnCopyLog.Location = new Point(430, 416);
            btnCopyLog.Size = new Size(100, 30);
            btnCopyLog.TabIndex = 5;
            btnCopyLog.Text = "نسخ السجل";
            btnCopyLog.UseVisualStyleBackColor = false;
            btnCopyLog.Click += BtnCopyLog_Click;

            // 
            // btnOpenFolder
            // 
            AppTheme.StyleButton(btnOpenFolder, AppTheme.Success);
            btnOpenFolder.Enabled = false;
            btnOpenFolder.Location = new Point(550, 416);
            btnOpenFolder.Size = new Size(100, 35);
            btnOpenFolder.TabIndex = 3;
            btnOpenFolder.Text = "فتح المجلد";
            btnOpenFolder.Click += BtnOpenFolder_Click;

            // 
            // btnClose
            // 
            AppTheme.StyleButton(btnClose, AppTheme.PrimaryDark);
            btnClose.Enabled = false;
            btnClose.Location = new Point(670, 416);
            btnClose.Size = new Size(100, 35);
            btnClose.TabIndex = 4;
            btnClose.Text = "إغلاق";
            btnClose.Click += BtnClose_Click;

            // 
            // btnCancel
            // 
            btnCancel = new Button();
            AppTheme.StyleButton(btnCancel, AppTheme.Danger);
            btnCancel.Location = new Point(430, 452);
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "إلغاء";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Visible = true;
            btnCancel.Click += (s, e) =>
            {
                _cts?.Cancel();
                btnCancel.Enabled = false;
                AppendText("تم طلب الإلغاء...", AppTheme.ConsoleWarning);
            };

            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 452);
            lblStatus.Size = new Size(100, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "جاري التوليد...";

            // 
            // FrmProgress
            // 
            ClientSize = new Size(784, 491);
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && btnClose.Enabled)
                    Close();
            };
            Controls.Add(btnClose);
            Controls.Add(btnOpenFolder);
            Controls.Add(btnCancel);
            Controls.Add(btnCopyLog);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Controls.Add(txtOutput);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmProgress";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            Text = "تقدم توليد الكود";
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeCustomComponents()
        {
            Font = AppTheme.DefaultFont;
            lblStatus.Font = AppTheme.DefaultFontBold;
            lblStatus.ForeColor = AppTheme.TextPrimary;
            AppendText("=== مولد الكود التلقائي ===", AppTheme.ConsoleWarning);
            AppendText("بدء عملية توليد الكود...", Color.White);
            AppendText("", Color.White);
        }

        private void BtnCopyLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                Clipboard.SetText(txtOutput.Text);
                btnCopyLog.Text = "تم النسخ!";
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (ts, te) => { btnCopyLog.Text = "نسخ السجل"; timer.Stop(); timer.Dispose(); };
                timer.Start();
            }
        }

        public void SetTotalFiles(int total)
        {
            totalFiles = total;
            completedFiles = 0;
            progressBar.Maximum = total;
            progressBar.Value = 0;
            UpdateStatus();
        }

        public void AppendText(string text, Color color)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new Action<string, Color>(AppendText), text, color);
                return;
            }

            txtOutput.SelectionStart = txtOutput.TextLength;
            txtOutput.SelectionLength = 0;
            txtOutput.SelectionColor = color;
            txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
            txtOutput.SelectionColor = txtOutput.ForeColor;
            txtOutput.ScrollToCaret();
        }

        public void AppendText(string text)
        {
            AppendText(text, AppTheme.ConsoleSuccess);
        }

        public void ReportFileGenerated(string fileName, string operation)
        {
            completedFiles++;
            progressBar.Value = completedFiles;
            
            AppendText($"✓ {operation}: {fileName}", AppTheme.ConsoleSuccess);
            UpdateStatus();
        }

        public void ReportError(string error)
        {
            AppendText($"✗ خطأ: {error}", AppTheme.ConsoleError);
        }

        public void ReportWarning(string warning)
        {
            AppendText($"⚠ تحذير: {warning}", AppTheme.ConsoleWarning);
        }

        public void ReportInfo(string info)
        {
            AppendText($"ℹ {info}", AppTheme.ConsoleInfo);
        }

        private void UpdateStatus()
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action(UpdateStatus));
                return;
            }

            lblStatus.Text = $"تم إنجاز {completedFiles} من {totalFiles} ملف";
            
            if (completedFiles >= totalFiles && totalFiles > 0)
            {
                lblStatus.Text = "تم إنجاز جميع الملفات بنجاح!";
                btnClose.Enabled = true;
                btnOpenFolder.Enabled = true;
                AppendText("", Color.White);
                AppendText("=== تم الانتهاء من التوليد بنجاح ===", AppTheme.ConsoleComplete);
                AppendText($"تم توليد {totalFiles} ملف بنجاح!", AppTheme.ConsoleComplete);
            }
        }

        public void CompleteProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CompleteProgress));
                return;
            }

            progressBar.Value = progressBar.Maximum;
            completedFiles = totalFiles; // تأكيد إنجاز جميع الملفات
            btnCancel.Visible = false;
            UpdateStatus();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Dispose();
            base.OnFormClosing(e);
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(projectPath) && Directory.Exists(projectPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", projectPath);
            }
        }

        public void SetProjectPath(string path)
        {
            projectPath = path;
        }

        public void ShowStep(string stepName)
        {
            AppendText($">>> {stepName}", AppTheme.ConsoleInfo);
        }

        public void ShowTable(string tableName)
        {
            AppendText($"📊 معالجة الجدول: {tableName}", AppTheme.ConsoleWarning);
        }

        public void ShowSuccess(string message)
        {
            AppendText($"✅ {message}", AppTheme.ConsoleComplete);
        }
    }
} 