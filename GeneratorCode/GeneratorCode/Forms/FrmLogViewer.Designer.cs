using GeneratorCode.Properties;

namespace GeneratorCode.Forms
{
    partial class FrmLogViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Panels
            pnlFilters = new System.Windows.Forms.Panel();
            pnlViewMode = new System.Windows.Forms.Panel();
            pnlContent = new System.Windows.Forms.Panel();
            pnlActions = new System.Windows.Forms.Panel();

            // Filter Controls
            lblLevelFilter = new System.Windows.Forms.Label();
            cmbLevelFilter = new System.Windows.Forms.ComboBox();
            lblDateFilter = new System.Windows.Forms.Label();
            dtpDateFilter = new System.Windows.Forms.DateTimePicker();
            lblSearch = new System.Windows.Forms.Label();
            txtSearch = new System.Windows.Forms.TextBox();
            lblSourceFilter = new System.Windows.Forms.Label();
            txtSourceFilter = new System.Windows.Forms.TextBox();
            btnRefresh = new System.Windows.Forms.Button();

            // View Mode Controls
            lblViewMode = new System.Windows.Forms.Label();
            rdoFormattedView = new System.Windows.Forms.RadioButton();
            rdoTableView = new System.Windows.Forms.RadioButton();

            // Content Controls
            rtbFormattedView = new System.Windows.Forms.RichTextBox();
            dgvTableView = new System.Windows.Forms.DataGridView();

            // Action Controls
            lblLogCount = new System.Windows.Forms.Label();
            btnViewDetails = new System.Windows.Forms.Button();
            btnExport = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();

            // Suspend layouts
            pnlFilters.SuspendLayout();
            pnlViewMode.SuspendLayout();
            pnlContent.SuspendLayout();
            pnlActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTableView).BeginInit();
            SuspendLayout();

            // 
            // pnlFilters
            // 
            pnlFilters.Controls.Add(btnClearFilters);
            pnlFilters.Controls.Add(btnRefresh);
            pnlFilters.Controls.Add(txtSourceFilter);
            pnlFilters.Controls.Add(lblSourceFilter);
            pnlFilters.Controls.Add(txtSearch);
            pnlFilters.Controls.Add(lblSearch);
            pnlFilters.Controls.Add(dtpDateFilter);
            pnlFilters.Controls.Add(lblDateFilter);
            pnlFilters.Controls.Add(cmbLevelFilter);
            pnlFilters.Controls.Add(lblLevelFilter);
            pnlFilters.Dock = System.Windows.Forms.DockStyle.Top;
            pnlFilters.Location = new System.Drawing.Point(0, 0);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Padding = new System.Windows.Forms.Padding(10);
            pnlFilters.Size = new System.Drawing.Size(1200, 80);
            pnlFilters.TabIndex = 0;

            // 
            // lblLevelFilter
            // 
            lblLevelFilter.AutoSize = true;
            lblLevelFilter.Location = new System.Drawing.Point(10, 15);
            lblLevelFilter.Name = "lblLevelFilter";
            lblLevelFilter.Size = new System.Drawing.Size(50, 15);
            lblLevelFilter.TabIndex = 0;
            lblLevelFilter.Text = "المستوى:";

            // 
            // cmbLevelFilter
            // 
            cmbLevelFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLevelFilter.FormattingEnabled = true;
            cmbLevelFilter.Location = new System.Drawing.Point(70, 12);
            cmbLevelFilter.Name = "cmbLevelFilter";
            cmbLevelFilter.Size = new System.Drawing.Size(120, 23);
            cmbLevelFilter.TabIndex = 1;

            // 
            // lblDateFilter
            // 
            lblDateFilter.AutoSize = true;
            lblDateFilter.Location = new System.Drawing.Point(210, 15);
            lblDateFilter.Name = "lblDateFilter";
            lblDateFilter.Size = new System.Drawing.Size(40, 15);
            lblDateFilter.TabIndex = 2;
            lblDateFilter.Text = "التاريخ:";

            // 
            // dtpDateFilter
            // 
            dtpDateFilter.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpDateFilter.Location = new System.Drawing.Point(260, 12);
            dtpDateFilter.Name = "dtpDateFilter";
            dtpDateFilter.Size = new System.Drawing.Size(150, 23);
            dtpDateFilter.TabIndex = 3;

            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Location = new System.Drawing.Point(430, 15);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new System.Drawing.Size(40, 15);
            lblSearch.TabIndex = 4;
            lblSearch.Text = "البحث:";

            // 
            // txtSearch
            // 
            txtSearch.Location = new System.Drawing.Point(480, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new System.Drawing.Size(200, 23);
            txtSearch.TabIndex = 5;

            // 
            // lblSourceFilter
            // 
            lblSourceFilter.AutoSize = true;
            lblSourceFilter.Location = new System.Drawing.Point(10, 50);
            lblSourceFilter.Name = "lblSourceFilter";
            lblSourceFilter.Size = new System.Drawing.Size(45, 15);
            lblSourceFilter.TabIndex = 6;
            lblSourceFilter.Text = "المصدر:";

            // 
            // txtSourceFilter
            // 
            txtSourceFilter.Location = new System.Drawing.Point(70, 47);
            txtSourceFilter.Name = "txtSourceFilter";
            txtSourceFilter.Size = new System.Drawing.Size(200, 23);
            txtSourceFilter.TabIndex = 7;

            // 
            // btnRefresh
            // 
            Helpers.AppTheme.StyleButton(btnRefresh, Helpers.AppTheme.Primary);
            btnRefresh.Location = new System.Drawing.Point(700, 12);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(100, 30);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "تحديث";

            // 
            // btnClearFilters
            // 
            btnClearFilters = new System.Windows.Forms.Button();
            Helpers.AppTheme.StyleButton(btnClearFilters, Helpers.AppTheme.Gray);
            btnClearFilters.Location = new System.Drawing.Point(810, 12);
            btnClearFilters.Name = "btnClearFilters";
            btnClearFilters.Size = new System.Drawing.Size(110, 30);
            btnClearFilters.TabIndex = 9;
            btnClearFilters.Text = "مسح الفلاتر";

            // 
            // pnlViewMode
            // 
            pnlViewMode.Controls.Add(rdoTableView);
            pnlViewMode.Controls.Add(rdoFormattedView);
            pnlViewMode.Controls.Add(lblViewMode);
            pnlViewMode.Dock = System.Windows.Forms.DockStyle.Top;
            pnlViewMode.Location = new System.Drawing.Point(0, 80);
            pnlViewMode.Name = "pnlViewMode";
            pnlViewMode.Padding = new System.Windows.Forms.Padding(10);
            pnlViewMode.Size = new System.Drawing.Size(1200, 50);
            pnlViewMode.TabIndex = 1;

            // 
            // lblViewMode
            // 
            lblViewMode.AutoSize = true;
            lblViewMode.Font = Helpers.AppTheme.DefaultFontBold;
            lblViewMode.Location = new System.Drawing.Point(10, 15);
            lblViewMode.Name = "lblViewMode";
            lblViewMode.Size = new System.Drawing.Size(70, 15);
            lblViewMode.TabIndex = 0;
            lblViewMode.Text = "وضع العرض:";

            // 
            // rdoFormattedView
            // 
            rdoFormattedView.AutoSize = true;
            rdoFormattedView.Location = new System.Drawing.Point(90, 13);
            rdoFormattedView.Name = "rdoFormattedView";
            rdoFormattedView.Size = new System.Drawing.Size(85, 19);
            rdoFormattedView.TabIndex = 1;
            rdoFormattedView.TabStop = true;
            rdoFormattedView.Text = "عرض منسق";
            rdoFormattedView.UseVisualStyleBackColor = true;

            // 
            // rdoTableView
            // 
            rdoTableView.AutoSize = true;
            rdoTableView.Location = new System.Drawing.Point(190, 13);
            rdoTableView.Name = "rdoTableView";
            rdoTableView.Size = new System.Drawing.Size(85, 19);
            rdoTableView.TabIndex = 2;
            rdoTableView.Text = "عرض جدولي";
            rdoTableView.UseVisualStyleBackColor = true;

            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(dgvTableView);
            pnlContent.Controls.Add(rtbFormattedView);
            pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlContent.Location = new System.Drawing.Point(0, 130);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new System.Windows.Forms.Padding(10);
            pnlContent.Size = new System.Drawing.Size(1200, 470);
            pnlContent.TabIndex = 2;

            // 
            // rtbFormattedView
            // 
            rtbFormattedView.Dock = System.Windows.Forms.DockStyle.Fill;
            rtbFormattedView.Location = new System.Drawing.Point(10, 10);
            rtbFormattedView.Name = "rtbFormattedView";
            rtbFormattedView.Size = new System.Drawing.Size(1180, 450);
            rtbFormattedView.TabIndex = 0;
            rtbFormattedView.Text = "";

            // 
            // dgvTableView
            // 
            dgvTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvTableView.Location = new System.Drawing.Point(10, 10);
            dgvTableView.Name = "dgvTableView";
            dgvTableView.Size = new System.Drawing.Size(1180, 450);
            dgvTableView.TabIndex = 1;

            // 
            // pnlActions
            // 
            pnlActions.Controls.Add(btnClose);
            pnlActions.Controls.Add(btnExport);
            pnlActions.Controls.Add(btnViewDetails);
            pnlActions.Controls.Add(lblLogCount);
            pnlActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            pnlActions.Location = new System.Drawing.Point(0, 600);
            pnlActions.Name = "pnlActions";
            pnlActions.Padding = new System.Windows.Forms.Padding(10);
            pnlActions.Size = new System.Drawing.Size(1200, 50);
            pnlActions.TabIndex = 3;

            // 
            // lblLogCount
            // 
            lblLogCount.AutoSize = true;
            lblLogCount.Location = new System.Drawing.Point(10, 15);
            lblLogCount.Name = "lblLogCount";
            lblLogCount.Size = new System.Drawing.Size(80, 15);
            lblLogCount.TabIndex = 0;
            lblLogCount.Text = "عدد السجلات: 0";

            // 
            // btnViewDetails
            // 
            Helpers.AppTheme.StyleButton(btnViewDetails, Helpers.AppTheme.Primary);
            btnViewDetails.Location = new System.Drawing.Point(900, 10);
            btnViewDetails.Name = "btnViewDetails";
            btnViewDetails.Size = new System.Drawing.Size(100, 30);
            btnViewDetails.TabIndex = 1;
            btnViewDetails.Text = "عرض التفاصيل";

            // 
            // btnExport
            // 
            Helpers.AppTheme.StyleButton(btnExport, Helpers.AppTheme.Success);
            btnExport.Location = new System.Drawing.Point(1010, 10);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(80, 30);
            btnExport.TabIndex = 2;
            btnExport.Text = "تصدير";

            // 
            // btnClose
            // 
            Helpers.AppTheme.StyleButton(btnClose, Helpers.AppTheme.Danger);
            btnClose.Location = new System.Drawing.Point(1100, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(80, 30);
            btnClose.TabIndex = 3;
            btnClose.Text = "إغلاق";

            // 
            // FrmLogViewer
            // 
            this.BackColor = Helpers.AppTheme.FormBackground;
            this.Font = Helpers.AppTheme.DefaultFont;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 650);
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlViewMode);
            this.Controls.Add(pnlActions);
            this.Controls.Add(pnlFilters);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmLogViewer";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "عارض السجلات (Log Viewer)";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            // Resume layouts
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            pnlViewMode.ResumeLayout(false);
            pnlViewMode.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlActions.ResumeLayout(false);
            pnlActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTableView).EndInit();
            ResumeLayout(false);
            
            // استدعاء InitializeForm بعد إنشاء جميع العناصر
            InitializeForm();
        }

        /// <summary>
        /// تهيئة النموذج بعد إنشاء جميع العناصر
        /// </summary>
        private void InitializeForm()
        {
            SetupViewMode();
            SetupFilters();
            SetupDataGridView();
            SetupRichTextBox();
        }

        private void SetupViewMode()
        {
            var settings = Settings.Default;
            var defaultViewMode = settings.LogViewerDefaultViewMode ?? "Formatted";
            rdoFormattedView.Checked = defaultViewMode == "Formatted";
            rdoTableView.Checked = defaultViewMode == "Table";
        }

        private void SetupFilters()
        {
            // إعداد ComboBox Level Filter
            cmbLevelFilter.Items.AddRange(new[] { "الكل", "Error", "Warning", "Info", "Debug" });
            cmbLevelFilter.SelectedIndex = 0;

            // إعداد DateTimePicker
            dtpDateFilter.Value = System.DateTime.Now;
            dtpDateFilter.Checked = false;
        }

        private void SetupDataGridView()
        {
            Helpers.AppTheme.StyleDataGridView(dgvTableView);
            dgvTableView.AutoGenerateColumns = false;
            dgvTableView.MultiSelect = false;

            dgvTableView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Timestamp",
                HeaderText = "التاريخ والوقت",
                DataPropertyName = "Timestamp",
                Width = 150
            });

            dgvTableView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Level",
                HeaderText = "المستوى",
                DataPropertyName = "Level",
                Width = 80
            });

            dgvTableView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Message",
                HeaderText = "الرسالة",
                DataPropertyName = "Message",
                Width = 300
            });

            dgvTableView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "Source",
                HeaderText = "المصدر",
                DataPropertyName = "Source",
                Width = 200
            });

            dgvTableView.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                Name = "HasException",
                HeaderText = "يحتوي على استثناء",
                DataPropertyName = "HasException",
                Width = 100
            });
        }

        private void SetupRichTextBox()
        {
            Helpers.AppTheme.StyleRichTextBoxConsole(rtbFormattedView);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Panel pnlViewMode;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Label lblLevelFilter;
        private System.Windows.Forms.ComboBox cmbLevelFilter;
        private System.Windows.Forms.Label lblDateFilter;
        private System.Windows.Forms.DateTimePicker dtpDateFilter;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSourceFilter;
        private System.Windows.Forms.TextBox txtSourceFilter;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.Label lblViewMode;
        private System.Windows.Forms.RadioButton rdoFormattedView;
        private System.Windows.Forms.RadioButton rdoTableView;
        private System.Windows.Forms.RichTextBox rtbFormattedView;
        private System.Windows.Forms.DataGridView dgvTableView;
        private System.Windows.Forms.Label lblLogCount;
        private System.Windows.Forms.Button btnViewDetails;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
    }
}

