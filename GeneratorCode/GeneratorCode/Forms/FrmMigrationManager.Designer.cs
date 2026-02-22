using Helpers = GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    partial class FrmMigrationManager
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            _cts?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            var headerColor = Helpers.AppTheme.PrimaryDark;
            var accentBlue = Helpers.AppTheme.Primary;
            var accentGreen = Helpers.AppTheme.Success;
            var accentRed = Helpers.AppTheme.Danger;
            var accentOrange = Helpers.AppTheme.Warning;
            var accentPurple = Helpers.AppTheme.Purple;
            var bgColor = Helpers.AppTheme.SurfaceLight;
            var panelBg = Helpers.AppTheme.PanelBackground;
            var defaultFont = Helpers.AppTheme.DefaultFontSmall;

            // Top info panel
            panelInfo = new System.Windows.Forms.Panel();
            lblProjectName = new System.Windows.Forms.Label();
            lblDatabaseInfo = new System.Windows.Forms.Label();
            lblOutputPath = new System.Windows.Forms.Label();

            // Split main content
            splitMain = new System.Windows.Forms.SplitContainer();

            // Left: Migrations list + actions
            grpMigrations = new System.Windows.Forms.GroupBox();
            gridMigrations = new System.Windows.Forms.DataGridView();
            panelActions = new System.Windows.Forms.Panel();
            lblMigrationName = new System.Windows.Forms.Label();
            txtMigrationName = new System.Windows.Forms.TextBox();
            btnGenerateCode = new System.Windows.Forms.Button();
            btnAddMigration = new System.Windows.Forms.Button();
            btnUpdateDatabase = new System.Windows.Forms.Button();
            btnRollback = new System.Windows.Forms.Button();
            btnGenerateScript = new System.Windows.Forms.Button();
            btnRemoveLast = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();

            // Right: Output
            grpOutput = new System.Windows.Forms.GroupBox();
            txtOutput = new System.Windows.Forms.RichTextBox();

            // Bottom
            panelBottom = new System.Windows.Forms.Panel();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblStatusBar = new System.Windows.Forms.Label();
            btnClose = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridMigrations).BeginInit();
            SuspendLayout();

            // ===== panelInfo =====
            panelInfo.BackColor = headerColor;
            panelInfo.Dock = System.Windows.Forms.DockStyle.Top;
            panelInfo.Height = 75;
            panelInfo.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);

            lblProjectName.Text = "المشروع: -";
            lblProjectName.ForeColor = System.Drawing.Color.White;
            lblProjectName.Font = Helpers.AppTheme.HeaderFontSemibold;
            lblProjectName.Location = new System.Drawing.Point(15, 10);
            lblProjectName.AutoSize = true;

            lblDatabaseInfo.Text = "قاعدة البيانات: -";
            lblDatabaseInfo.ForeColor = Helpers.AppTheme.TextSecondary;
            lblDatabaseInfo.Font = defaultFont;
            lblDatabaseInfo.Location = new System.Drawing.Point(15, 35);
            lblDatabaseInfo.AutoSize = true;

            lblOutputPath.Text = "المسار: -";
            lblOutputPath.ForeColor = Helpers.AppTheme.TextSecondary;
            lblOutputPath.Font = defaultFont;
            lblOutputPath.Location = new System.Drawing.Point(15, 53);
            lblOutputPath.AutoSize = true;

            panelInfo.Controls.AddRange(new System.Windows.Forms.Control[] { lblProjectName, lblDatabaseInfo, lblOutputPath });

            // ===== splitMain =====
            splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitMain.SplitterDistance = 340;
            splitMain.Orientation = System.Windows.Forms.Orientation.Vertical;

            // ===== Migrations Panel (Panel1) =====
            grpMigrations.Text = "  الـ Migrations  ";
            grpMigrations.Font = Helpers.AppTheme.DefaultFontBold;
            grpMigrations.ForeColor = Helpers.AppTheme.TextPrimary;
            grpMigrations.Dock = System.Windows.Forms.DockStyle.Fill;
            grpMigrations.Padding = new System.Windows.Forms.Padding(8);

            gridMigrations.BackgroundColor = System.Drawing.Color.White;
            gridMigrations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            gridMigrations.Dock = System.Windows.Forms.DockStyle.Fill;
            gridMigrations.RowHeadersVisible = false;
            gridMigrations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridMigrations.AllowUserToAddRows = false;
            gridMigrations.AllowUserToDeleteRows = false;
            gridMigrations.ReadOnly = true;
            gridMigrations.ColumnHeadersDefaultCellStyle.BackColor = Helpers.AppTheme.PrimaryDark;
            gridMigrations.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            gridMigrations.ColumnHeadersDefaultCellStyle.Font = Helpers.AppTheme.DefaultFontSmall;
            gridMigrations.ColumnHeadersHeight = 34;
            gridMigrations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridMigrations.EnableHeadersVisualStyles = false;
            gridMigrations.RowTemplate.Height = 28;
            gridMigrations.DefaultCellStyle.Font = defaultFont;
            gridMigrations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            gridMigrations.Columns.Add("colMigName", "الاسم");
            gridMigrations.Columns.Add("colMigId", "المعرف");
            gridMigrations.Columns.Add("colMigDate", "التاريخ");
            gridMigrations.Columns.Add("colMigStatus", "الحالة");

            // Actions panel
            panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelActions.Height = 260;
            panelActions.BackColor = panelBg;
            panelActions.Padding = new System.Windows.Forms.Padding(10);

            lblMigrationName.Text = "اسم الـ Migration:";
            lblMigrationName.Font = defaultFont;
            lblMigrationName.Location = new System.Drawing.Point(15, 12);
            lblMigrationName.AutoSize = true;

            txtMigrationName.Font = defaultFont;
            txtMigrationName.Location = new System.Drawing.Point(15, 35);
            txtMigrationName.Width = 280;

            SetupActionButton(btnGenerateCode, "توليد كود EF Core", accentPurple, 15, 70, 280, 36);
            SetupActionButton(btnAddMigration, "إنشاء Migration", accentBlue, 15, 112, 135, 36);
            SetupActionButton(btnUpdateDatabase, "تحديث القاعدة", accentGreen, 160, 112, 135, 36);
            SetupActionButton(btnRollback, "تراجع", accentOrange, 15, 154, 90, 36);
            SetupActionButton(btnGenerateScript, "توليد SQL", accentBlue, 110, 154, 90, 36);
            SetupActionButton(btnRemoveLast, "حذف آخر", accentRed, 205, 154, 90, 36);
            SetupActionButton(btnRefresh, "تحديث القائمة", Helpers.AppTheme.GrayDark, 15, 200, 280, 34);

            panelActions.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblMigrationName, txtMigrationName,
                btnGenerateCode, btnAddMigration, btnUpdateDatabase,
                btnRollback, btnGenerateScript, btnRemoveLast, btnRefresh
            });

            grpMigrations.Controls.Add(gridMigrations);
            grpMigrations.Controls.Add(panelActions);
            splitMain.Panel1.Controls.Add(grpMigrations);

            // ===== Output Panel (Panel2) =====
            grpOutput.Text = "  المخرجات  ";
            grpOutput.Font = Helpers.AppTheme.DefaultFontBold;
            grpOutput.ForeColor = Helpers.AppTheme.TextPrimary;
            grpOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            grpOutput.Padding = new System.Windows.Forms.Padding(8);

            txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            txtOutput.Font = Helpers.AppTheme.ConsoleFont;
            txtOutput.BackColor = Helpers.AppTheme.ConsoleBg;
            txtOutput.ForeColor = Helpers.AppTheme.ConsoleFg;
            txtOutput.ReadOnly = true;
            txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtOutput.WordWrap = true;

            grpOutput.Controls.Add(txtOutput);
            splitMain.Panel2.Controls.Add(grpOutput);

            // ===== panelBottom =====
            panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            panelBottom.Height = 45;
            panelBottom.BackColor = panelBg;

            progressBar.Location = new System.Drawing.Point(10, 12);
            progressBar.Size = new System.Drawing.Size(200, 20);
            progressBar.Visible = false;
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;

            lblStatusBar.Text = "جاهز";
            lblStatusBar.Font = defaultFont;
            lblStatusBar.Location = new System.Drawing.Point(220, 13);
            lblStatusBar.AutoSize = true;

            SetupActionButton(btnClose, "إغلاق", Helpers.AppTheme.Gray, 850, 7, 90, 30);

            SetupActionButton(btnCancel, "إلغاء", Helpers.AppTheme.Danger, 10, 7, 90, 30);
            btnCancel.Visible = false;

            panelBottom.Controls.AddRange(new System.Windows.Forms.Control[] { progressBar, lblStatusBar, btnClose, btnCancel });

            // ===== Form =====
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1000, 650);
            Controls.Add(splitMain);
            Controls.Add(panelBottom);
            Controls.Add(panelInfo);
            Text = "إدارة الـ Migrations";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = defaultFont;
            BackColor = bgColor;
            MinimumSize = new System.Drawing.Size(750, 500);

            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridMigrations).EndInit();
            ResumeLayout(false);
        }

        private void SetupActionButton(System.Windows.Forms.Button btn, string text, System.Drawing.Color bgColor, int x, int y, int w, int h)
        {
            btn.Text = text;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = bgColor;
            btn.ForeColor = System.Drawing.Color.White;
            btn.Font = Helpers.AppTheme.ButtonFont;
            btn.Location = new System.Drawing.Point(x, y);
            btn.Size = new System.Drawing.Size(w, h);
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
        }

        #endregion

        private System.Windows.Forms.Panel panelInfo;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.Label lblDatabaseInfo;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpMigrations;
        private System.Windows.Forms.DataGridView gridMigrations;
        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Label lblMigrationName;
        private System.Windows.Forms.TextBox txtMigrationName;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.Button btnAddMigration;
        private System.Windows.Forms.Button btnUpdateDatabase;
        private System.Windows.Forms.Button btnRollback;
        private System.Windows.Forms.Button btnGenerateScript;
        private System.Windows.Forms.Button btnRemoveLast;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox grpOutput;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatusBar;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancel;
    }
}
