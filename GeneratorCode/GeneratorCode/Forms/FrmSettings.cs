using System;
using System.Windows.Forms;
using GeneratorCode.Properties;
using System.IO;

namespace GeneratorCode.Forms
{
    public partial class FrmSettings : Form
    {
        private Settings _settings;
        private static FrmSettings _instance;
        public static event Action SettingsUpdated;

        public static FrmSettings Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new FrmSettings();
                }
                return _instance;
            }
        }

        public FrmSettings()
        {
            InitializeComponent();
            _settings = Settings.Default;
            
            // تعيين الحد الأدنى لحجم النموذج
            this.MinimumSize = new System.Drawing.Size(500, 600);
            
            LoadSettings();
            ShowDatabaseGroup(_settings.DatabaseType);
        }

        private void ShowDatabaseGroup(string databaseType)
        {
            // إخفاء جميع المجموعات أولاً
            grpPostgres.Visible = false;
            grpSqlServer.Visible = false;
            grpMySql.Visible = false;

            // إظهار المجموعة المناسبة فقط
            switch (databaseType?.ToLower())
            {
                case "postgresql":
                    grpPostgres.Visible = true;
                    break;
                case "sqlserver":
                    grpSqlServer.Visible = true;
                    break;
                case "mysql":
                    grpMySql.Visible = true;
                    break;
            }

            // إعادة تنظيم المجموعات المرئية
            ReorganizeGroups();
        }

        private void ReorganizeGroups()
        {
            int topPosition = txtOutputPath.Bottom + 20;
            int maxBottom = topPosition;

            // تنظيم المجموعات
            if (grpPostgres.Visible)
            {
                grpPostgres.Top = topPosition;
                maxBottom = grpPostgres.Bottom;
                topPosition = grpPostgres.Bottom + 10;
            }

            if (grpSqlServer.Visible)
            {
                grpSqlServer.Top = topPosition;
                maxBottom = grpSqlServer.Bottom;
                topPosition = grpSqlServer.Bottom + 10;
            }

            if (grpMySql.Visible)
            {
                grpMySql.Top = topPosition;
                maxBottom = grpMySql.Bottom;
            }

            // تحديث موضع الأزرار لتكون دائماً في الأسفل
            int buttonMargin = 20;
            btnCancel.Top = maxBottom + buttonMargin;
            btnSave.Top = maxBottom + buttonMargin;
            
            // محاذاة الأزرار لليمين
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - buttonMargin;
            btnSave.Left = btnCancel.Left - btnSave.Width - 10;

            // تحديث ارتفاع النموذج
            int newHeight = btnSave.Bottom + buttonMargin;
            if (newHeight > this.MinimumSize.Height)
            {
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, newHeight);
            }
        }

        private void LoadSettings()
        {
            // إعدادات عامة
            chkEnableDI.Checked = _settings.EnableDI;
            chkEnableValidation.Checked = _settings.EnableValidation;
            chkEnableTesting.Checked = _settings.EnableTesting;
            txtDefaultNamespace.Text = _settings.DefaultNamespace;
            txtOutputPath.Text = _settings.DefaultOutputPath;

            // إعدادات PostgreSQL
            txtPostgresUsername.Text = _settings.PostgreSqlDefaultUsername;
            txtPostgresPassword.Text = _settings.PostgreSqlDefaultPassword;
            txtPostgresPort.Text = _settings.PostgreSqlDefaultPort;

            // إعدادات SQL Server
            txtSqlServerUsername.Text = _settings.SqlServerDefaultUsername;
            txtSqlServerPassword.Text = _settings.SqlServerDefaultPassword;

            // إعدادات MySQL
            txtMySqlUsername.Text = _settings.MySqlDefaultUsername;
            txtMySqlPassword.Text = _settings.MySqlDefaultPassword;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // إعدادات عامة
            _settings.EnableDI = chkEnableDI.Checked;
            _settings.EnableValidation = chkEnableValidation.Checked;
            _settings.EnableTesting = chkEnableTesting.Checked;
            _settings.DefaultNamespace = txtDefaultNamespace.Text;
            _settings.DefaultOutputPath = txtOutputPath.Text;

            // إعدادات PostgreSQL
            if (grpPostgres.Visible)
            {
                _settings.PostgreSqlDefaultUsername = txtPostgresUsername.Text;
                _settings.PostgreSqlDefaultPassword = txtPostgresPassword.Text;
                _settings.PostgreSqlDefaultPort = txtPostgresPort.Text;
            }

            // إعدادات SQL Server
            if (grpSqlServer.Visible)
            {
                _settings.SqlServerDefaultUsername = txtSqlServerUsername.Text;
                _settings.SqlServerDefaultPassword = txtSqlServerPassword.Text;
            }

            // إعدادات MySQL
            if (grpMySql.Visible)
            {
                _settings.MySqlDefaultUsername = txtMySqlUsername.Text;
                _settings.MySqlDefaultPassword = txtMySqlPassword.Text;
            }

            _settings.Save();
            SettingsUpdated?.Invoke();  // إطلاق الحدث عند تحديث الإعدادات
            DialogResult = DialogResult.OK;
            Hide();  // إخفاء النموذج بدلاً من إغلاقه
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();  // إخفاء النموذج بدلاً من إغلاقه
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                Title = "اختر المجلد الافتراضي لحفظ الملفات",
                InitialDirectory = !string.IsNullOrEmpty(txtOutputPath.Text) ? txtOutputPath.Text : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "اختر هذا المجلد",
                Filter = "مجلد|*.",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string selectedPath = Path.GetDirectoryName(dialog.FileName);
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        txtOutputPath.Text = selectedPath;
                    }
                }
        }
    }
} 