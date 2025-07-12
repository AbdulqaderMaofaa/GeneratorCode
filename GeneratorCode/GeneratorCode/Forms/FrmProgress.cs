using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
        private Label lblStatus;
        private int totalFiles = 0;
        private int completedFiles = 0;
        private string projectPath = "";

        public FrmProgress()
        {
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
            txtOutput.BackColor = Color.Black;
            txtOutput.ForeColor = Color.Lime;
            txtOutput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            txtOutput.Location = new Point(12, 12);
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtOutput.Size = new Size(760, 400);
            txtOutput.TabIndex = 0;
            txtOutput.Text = "";

            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 430);
            progressBar.Size = new Size(530, 23);
            progressBar.TabIndex = 1;
            progressBar.Style = ProgressBarStyle.Continuous;

            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 460);
            lblStatus.Size = new Size(100, 20);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "جاري التوليد...";

            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Enabled = false;
            btnOpenFolder.Location = new Point(550, 430);
            btnOpenFolder.Size = new Size(100, 50);
            btnOpenFolder.TabIndex = 3;
            btnOpenFolder.Text = "فتح المجلد";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += BtnOpenFolder_Click;

            // 
            // btnClose
            // 
            btnClose.Enabled = false;
            btnClose.Location = new Point(670, 430);
            btnClose.Size = new Size(100, 50);
            btnClose.TabIndex = 4;
            btnClose.Text = "إغلاق";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;

            // 
            // FrmProgress
            // 
            ClientSize = new Size(784, 491);
            Controls.Add(btnClose);
            Controls.Add(btnOpenFolder);
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
            // إضافة نص ترحيبي
            AppendText("=== مولد الكود التلقائي ===", Color.Yellow);
            AppendText("بدء عملية توليد الكود...", Color.White);
            AppendText("", Color.White);
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
            AppendText(text, Color.Lime);
        }

        public void ReportFileGenerated(string fileName, string operation)
        {
            completedFiles++;
            progressBar.Value = completedFiles;
            
            AppendText($"✓ {operation}: {fileName}", Color.Lime);
            UpdateStatus();
        }

        public void ReportError(string error)
        {
            AppendText($"✗ خطأ: {error}", Color.Red);
        }

        public void ReportWarning(string warning)
        {
            AppendText($"⚠ تحذير: {warning}", Color.Yellow);
        }

        public void ReportInfo(string info)
        {
            AppendText($"ℹ {info}", Color.Cyan);
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
                AppendText("=== تم الانتهاء من التوليد بنجاح ===", Color.Green);
                AppendText($"تم توليد {totalFiles} ملف بنجاح!", Color.Green);
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
            UpdateStatus();
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
            AppendText($">>> {stepName}", Color.Cyan);
        }

        public void ShowTable(string tableName)
        {
            AppendText($"📊 معالجة الجدول: {tableName}", Color.Yellow);
        }

        public void ShowSuccess(string message)
        {
            AppendText($"✅ {message}", Color.Green);
        }
    }
} 