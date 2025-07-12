using GeneratorCode.GeneratorCode.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using GeneratorCode.Properties;

namespace GeneratorCode.Forms
{
    public enum StatusType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public partial class FrmConnection : Form
    {
        private DatabaseHelper _dbHelper;
        private Settings _settings;
        public string ConnectionString { get; private set; }
        public string DatabaseType { get; private set; }

        public FrmConnection()
        {

            InitializeComponent();
            _settings = Settings.Default;
            _dbHelper = new DatabaseHelper();
            SetupInitialState();
            InitializeEvents();
            LoadDefaultSettings();

        }

        private void InitializeEvents()
        {
            // أحداث تغيير الاختيار
            cmbDatabaseType.SelectedIndexChanged += CmbDatabaseType_SelectedIndexChanged;
            cmbServer.SelectedIndexChanged += CmbServer_SelectedIndexChanged;
            cmbDatabase.SelectedIndexChanged += CmbDatabase_SelectedIndexChanged;

            // أحداث الأزرار
            btnTestConnection.Click += BtnTestConnection_Click;
            btnConnect.Click += BtnConnect_Click;
            btnCancel.Click += BtnCancel_Click;

            // أحداث التحقق من المدخلات
            txtUsername.TextChanged += ValidateInputs;
            txtPassword.TextChanged += ValidateInputs;
        }

        private void SetupInitialState()
        {
            // إعداد المظهر العام
            SetupUI();
            
            // إعداد الحالة الأولية
            UpdateStatus("اختر نوع قاعدة البيانات للبدء...", StatusType.Info);
            
            // تعطيل العناصر حتى يتم اختيار نوع قاعدة البيانات
            EnableServerControls(false);
            EnableAuthenticationControls(false);
            EnableDatabaseControls(false);
            
            // تعطيل أزرار العمليات
            btnTestConnection.Enabled = false;
            btnConnect.Enabled = false;
        }

        private void SetupUI()
        {
            // إعداد الأيقونات
            UpdateDatabaseIcon("default");
            UpdateStatusIcon(StatusType.Info);
            
            // إعداد تلميحات الأدوات
            SetupTooltips();
            
            // إعداد الأحداث الإضافية
            chkSaveCredentials.CheckedChanged += ChkSaveCredentials_CheckedChanged;
        }

        private void SetupTooltips()
        {
            var toolTip = new ToolTip();
            toolTip.SetToolTip(cmbDatabaseType, "اختر نوع قاعدة البيانات التي تريد الاتصال بها");
            toolTip.SetToolTip(cmbServer, "أدخل اسم السيرفر أو عنوان IP");
            toolTip.SetToolTip(txtPort, "رقم المنفذ (Port) الخاص بقاعدة البيانات");
            toolTip.SetToolTip(btnTestConnection, "اختبر الاتصال قبل المتابعة");
            toolTip.SetToolTip(chkSaveCredentials, "حفظ بيانات الدخول للاستخدام التالي");
        }

        private void UpdateDatabaseIcon(string databaseType)
        {
            // يمكن إضافة أيقونات مختلفة حسب نوع قاعدة البيانات
            // في الوقت الحالي سنستخدم نص بسيط
            switch (databaseType.ToLower())
            {
                case "sqlserver":
                    // SQL Server icon placeholder
                    break;
                case "postgresql":
                    // PostgreSQL icon placeholder
                    break;
                case "mysql":
                    // MySQL icon placeholder
                    break;
                default:
                    // Default database icon
                    break;
            }
        }

        private void UpdateStatusIcon(StatusType statusType)
        {
            // تحديث أيقونة الحالة حسب النوع
            switch (statusType)
            {
                case StatusType.Success:
                    picStatus.BackColor = Color.Green;
                    break;
                case StatusType.Error:
                    picStatus.BackColor = Color.Red;
                    break;
                case StatusType.Warning:
                    picStatus.BackColor = Color.Orange;
                    break;
                case StatusType.Info:
                default:
                    picStatus.BackColor = Color.Blue;
                    break;
            }
        }

        private void UpdateStatus(string message, StatusType statusType = StatusType.Info)
        {
            lblStatus.Text = message;
            UpdateStatusIcon(statusType);
            
            // تحديث لون النص حسب النوع
            switch (statusType)
            {
                case StatusType.Success:
                    lblStatus.ForeColor = Color.DarkGreen;
                    break;
                case StatusType.Error:
                    lblStatus.ForeColor = Color.DarkRed;
                    break;
                case StatusType.Warning:
                    lblStatus.ForeColor = Color.DarkOrange;
                    break;
                case StatusType.Info:
                default:
                    lblStatus.ForeColor = Color.DarkBlue;
                    break;
            }
        }

        private void ChkSaveCredentials_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSaveCredentials.Checked)
            {
                var result = MessageBox.Show(
                    "هل أنت متأكد من حفظ بيانات الدخول؟\nسيتم حفظها محلياً على جهازك.",
                    "تأكيد حفظ البيانات",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.RightAlign
                );
                
                if (result == DialogResult.No)
                {
                    chkSaveCredentials.Checked = false;
                }
            }
        }

        private void LoadDatabaseTypes(string databaseType)
        {
            var databaseTypes = new List<DbType>
            {
                new DbType("PostgreSQL", "PostgreSQL"),
                new DbType("SQL Server", "SQLServer"),
                new DbType("MySQL", "MySQL")
            };

            cmbDatabaseType.DataSource = databaseTypes;
            cmbDatabaseType.DisplayMember = "Display";
            cmbDatabaseType.ValueMember = "Value";

            // اختيار نوع قاعدة البيانات من الإعدادات
            cmbDatabaseType.SelectedValue = databaseType;
        }

        private void LoadDefaultSettings()
        {

            var defaultDatabaseType = _settings.DatabaseType ?? "PostgreSQL";
            if (cmbDatabaseType.SelectedItem is DbType selectedType)
            {
                defaultDatabaseType = selectedType.Value;
            }
            LoadDatabaseTypes(defaultDatabaseType);


        }

        private async void CmbDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDatabaseType.SelectedIndex == -1) return;
            var selectedType = cmbDatabaseType.SelectedItem as DbType;
            
            if (selectedType != null)
            {
                string selectedDb = selectedType.Value;
                DatabaseType = selectedDb;
                
                UpdateStatus($"جاري تحميل إعدادات {selectedType.Display}...", StatusType.Info);
                UpdateDatabaseIcon(selectedDb);
                
                EnableServerControls(true);
                cmbServer.Items.Clear();

                // إظهار/إخفاء حقل البورت حسب نوع قاعدة البيانات
                bool showPort = selectedDb == "PostgreSQL" || selectedDb == "MySQL";
                grpConnectionDetails.Controls["lblPort"].Visible = showPort;
                grpConnectionDetails.Controls["txtPort"].Visible = showPort;

                switch (selectedDb)
                {
                    case "SQLServer":
                        await LoadSqlServers();
                        txtPort.Text = "1433";
                        break;
                    case "MySQL":
                        cmbServer.Items.Add("localhost");
                        txtPort.Text = "3306";
                        SettAutomaticSettings(selectedDb);
                        break;
                    case "PostgreSQL":
                        cmbServer.Items.Add("localhost");
                        txtPort.Text = "5432";
                        SettAutomaticSettings(selectedDb);
                        break;
                }
                
                UpdateStatus($"تم تحميل إعدادات {selectedType.Display} بنجاح", StatusType.Success);
            }

            // حفظ نوع قاعدة البيانات المختار في الإعدادات
            if (selectedType != null)
            {
                Settings.Default.DatabaseType = selectedType.Value;
                Settings.Default.Save();
            }
        }
        private void SettAutomaticSettings(string databaseType)
        {

            switch (databaseType)
            {
                case "PostgreSQL":
                    txtUsername.Text = _settings.PostgreSqlDefaultUsername;
                    txtPassword.Text = _settings.PostgreSqlDefaultPassword;
                    txtPort.Text = _settings.PostgreSqlDefaultPort;
                    break;
                case "SQLServer":
                    txtUsername.Text = _settings.SqlServerDefaultUsername;
                    txtPassword.Text = _settings.SqlServerDefaultPassword;
                    break;
                case "MySQL":
                    txtUsername.Text = _settings.MySqlDefaultUsername;
                    txtPassword.Text = _settings.MySqlDefaultPassword;
                    break;
            }

        }
        private async Task LoadSqlServers()
        {
            await Task.Run(() =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // إضافة السيرفرات المحلية الشائعة
                    cmbServer.Items.Add("localhost");
                    cmbServer.Items.Add(".");
                    cmbServer.Items.Add("(local)");
                    cmbServer.Items.Add(@".\SQLEXPRESS");
                });
            });
        }

        private void CmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbServer.SelectedIndex != -1)
            {
                EnableAuthenticationControls(true);
                UpdateStatus("أدخل بيانات المصادقة للمتابعة...", StatusType.Info);
                ValidateInputs(sender, e);
            }
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInputs()) return;

            UpdateStatus("جاري اختبار الاتصال...", StatusType.Info);
            progressBar.Visible = true;
            btnTestConnection.Enabled = false;
            
            // تأثير بصري لمدة قصيرة
            btnTestConnection.BackColor = Color.FromArgb(41, 128, 185);

            bool isConnected = await DatabaseHelper.TestConnection(
                cmbDatabaseType.Text,
                cmbServer.Text,
                txtUsername.Text,
                txtPassword.Text
            );

            if (isConnected)
            {
                UpdateStatus("✅ تم الاتصال بالسيرفر بنجاح!", StatusType.Success);
                MessageBox.Show(
                    "تم الاتصال بالسيرفر بنجاح!\nسيتم الآن تحميل قواعد البيانات المتاحة.",
                    "نجاح الاتصال",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
                await LoadDatabases();
            }
            else
            {
                UpdateStatus("❌ فشل في الاتصال بالسيرفر", StatusType.Error);
                MessageBox.Show(
                    "فشل في الاتصال بالسيرفر.\nتأكد من صحة البيانات المدخلة.",
                    "خطأ في الاتصال",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
            
            progressBar.Visible = false;
            btnTestConnection.Enabled = true;
            btnTestConnection.BackColor = Color.FromArgb(52, 152, 219);
        }

        private async Task LoadDatabases()
        {
            UpdateStatus("جاري تحميل قواعد البيانات...", StatusType.Info);
            progressBar.Visible = true;
            
            try
            {
                var databases = await DatabaseHelper.GetDatabases(
                    cmbDatabaseType.Text,
                    cmbServer.Text,
                    txtUsername.Text,
                    txtPassword.Text
                );

                cmbDatabase.Items.Clear();
                cmbDatabase.Items.AddRange(databases.ToArray());
                EnableDatabaseControls(true);
                
                UpdateStatus($"تم تحميل {databases.Count} قاعدة بيانات", StatusType.Success);
            }
            catch (Exception ex)
            {
                UpdateStatus("فشل في تحميل قواعد البيانات", StatusType.Error);
                MessageBox.Show(
                    $"حدث خطأ أثناء تحميل قواعد البيانات:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private void CmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool databaseSelected = cmbDatabase.SelectedIndex != -1;
            btnConnect.Enabled = databaseSelected;
            
            if (databaseSelected)
            {
                UpdateStatus($"جاهز للاتصال بقاعدة البيانات: {cmbDatabase.Text}", StatusType.Success);
                
                // حفظ بيانات الدخول إذا تم تفعيل الخيار
                if (chkSaveCredentials.Checked)
                {
                    SaveCredentials();
                }
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInputs() || cmbDatabase.SelectedIndex == -1) return;

            UpdateStatus("جاري إنشاء الاتصال...", StatusType.Info);
            progressBar.Visible = true;
            
            // تأثير بصري للزر
            btnConnect.BackColor = Color.FromArgb(39, 174, 96);
            
            try
            {
                BuildConnectionString();
                UpdateStatus("✅ تم إنشاء الاتصال بنجاح!", StatusType.Success);
                
                DialogResult = DialogResult.OK;
                new FrmTabls(ConnectionString, DatabaseType).ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                UpdateStatus("❌ فشل في إنشاء الاتصال", StatusType.Error);
                MessageBox.Show(
                    $"حدث خطأ أثناء إنشاء الاتصال:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
            finally
            {
                progressBar.Visible = false;
                btnConnect.BackColor = Color.FromArgb(46, 204, 113);
            }
        }

        private void SaveCredentials()
        {
            var selectedType = cmbDatabaseType.SelectedItem as DbType;
            if (selectedType == null) return;
            
            switch (selectedType.Value)
            {
                case "PostgreSQL":
                    _settings.PostgreSqlDefaultUsername = txtUsername.Text;
                    _settings.PostgreSqlDefaultPassword = txtPassword.Text;
                    _settings.PostgreSqlDefaultPort = txtPort.Text;
                    break;
                case "SQLServer":
                    _settings.SqlServerDefaultUsername = txtUsername.Text;
                    _settings.SqlServerDefaultPassword = txtPassword.Text;
                    break;
                case "MySQL":
                    _settings.MySqlDefaultUsername = txtUsername.Text;
                    _settings.MySqlDefaultPassword = txtPassword.Text;
                    break;
            }
            
            _settings.Save();
            UpdateStatus("تم حفظ بيانات الدخول", StatusType.Info);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            UpdateStatus("تم إلغاء العملية", StatusType.Warning);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            bool isValid = ValidateConnectionInputs();
            btnTestConnection.Enabled = isValid;
            
            if (isValid)
            {
                UpdateStatus("جاهز لاختبار الاتصال", StatusType.Success);
            }
            else
            {
                UpdateStatus("أكمل البيانات المطلوبة...", StatusType.Warning);
            }
        }

        private bool ValidateConnectionInputs()
        {
            bool isPostgres = cmbDatabaseType.SelectedValue?.ToString() == "PostgreSQL";
            bool isPortValid = !isPostgres || (!string.IsNullOrWhiteSpace(txtPort.Text) && int.TryParse(txtPort.Text, out _));

            return cmbDatabaseType.SelectedIndex != -1 &&
                   cmbServer.SelectedIndex != -1 &&
                   !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                   !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                   isPortValid;
        }

        private void EnableServerControls(bool enable)
        {
            cmbServer.Enabled = enable;
            lblServer.Enabled = enable;
            if (enable)
            {
                UpdateStatus("اختر السيرفر للمتابعة...", StatusType.Info);
            }
        }

        private void EnableAuthenticationControls(bool enable)
        {
            txtUsername.Enabled = enable;
            txtPassword.Enabled = enable;
            chkSaveCredentials.Enabled = enable;
        }

        private void EnableDatabaseControls(bool enable)
        {
            cmbDatabase.Enabled = enable;
            lblDatabase.Enabled = enable;
            if (enable)
            {
                UpdateStatus("اختر قاعدة البيانات للمتابعة...", StatusType.Info);
            }
        }

        private void BuildConnectionString()
        {
            if (cmbDatabaseType.SelectedItem is DbType selectedType)
            {
                DatabaseType = selectedType.Value;  // تأكيد تحديث نوع قاعدة البيانات

                switch (selectedType.Value)
                {
                    case "PostgreSQL":
                        //Host=localhost;Port=5432;Database=tabweebdbstage;Username=postgres;Password=4oh70*w8QT
                        ConnectionString = $"Host={cmbServer.Text};Port={txtPort.Text};Database={cmbDatabase.Text};Username={txtUsername.Text};Password={txtPassword.Text};";
                        break;
                    case "SQLServer":
                        ConnectionString = $"Data Source={cmbServer.Text};Initial Catalog={cmbDatabase.Text};User ID={txtUsername.Text};Password={txtPassword.Text};TrustServerCertificate=True;";
                        break;
                    case "MySQL":
                        ConnectionString = $"Server={cmbServer.Text};Database={cmbDatabase.Text};Uid={txtUsername.Text};Pwd={txtPassword.Text};";
                        break;
                }
            }
        }
    }
}