using GeneratorCode.GeneratorCode.Helpers;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Logging;
using GeneratorCode.Helpers;
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
        private readonly ILogger _logger;
        public string ConnectionString { get; private set; }
        public string DatabaseType { get; private set; }

        public FrmConnection()
        {
            _logger = LoggerFactory.Default;
            _logger.LogDebug("Initializing FrmConnection", "FrmConnection");
            
            InitializeComponent();
            ApplyTheme();
            _settings = Settings.Default;
            _dbHelper = new DatabaseHelper();
            SetupInitialState();
            InitializeEvents();
            LoadDefaultSettings();
            
            _logger.LogInfo("FrmConnection initialized successfully", "FrmConnection");
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
            btnCodeFirst.Click += BtnCodeFirst_Click;
            btnCancel.Click += BtnCancel_Click;
            btnViewLogs.Click += BtnViewLogs_Click;

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

        private void ApplyTheme()
        {
            AppTheme.StyleButton(btnTestConnection, AppTheme.Primary);
            AppTheme.StyleButton(btnConnect, AppTheme.Success);
            AppTheme.StyleButton(btnCancel, AppTheme.Danger);
            AppTheme.StyleButton(btnViewLogs, AppTheme.Purple);
            AppTheme.StyleButton(btnCodeFirst, AppTheme.PurpleDark, large: true);
            AppTheme.StyleGroupBox(grpDatabaseType, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpConnectionDetails, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpAuthentication, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpActions, AppTheme.PrimaryDark);
        }

        private void SetupUI()
        {
            AppTheme.StyleForm(this);
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            UpdateDatabaseIcon("default");
            UpdateStatusIcon(StatusType.Info);
            SetupTooltips();
            chkSaveCredentials.CheckedChanged += ChkSaveCredentials_CheckedChanged;
        }

        private void SetupTooltips()
        {
            var toolTip = AppTheme.CreateTooltipProvider();
            toolTip.SetToolTip(cmbDatabaseType, "اختر نوع قاعدة البيانات التي تريد الاتصال بها");
            toolTip.SetToolTip(cmbServer, "أدخل اسم السيرفر أو عنوان IP");
            toolTip.SetToolTip(txtPort, "رقم المنفذ (Port) الخاص بقاعدة البيانات");
            toolTip.SetToolTip(btnTestConnection, "اختبر الاتصال قبل المتابعة (Ctrl+T)");
            toolTip.SetToolTip(chkSaveCredentials, "حفظ بيانات الدخول للاستخدام التالي");
            toolTip.SetToolTip(btnConnect, "الاتصال بقاعدة البيانات والمتابعة");
            toolTip.SetToolTip(btnCancel, "إغلاق النافذة (Escape)");
            toolTip.SetToolTip(btnViewLogs, "عرض سجلات النظام");
            toolTip.SetToolTip(btnCodeFirst, "الانتقال إلى وضع Code First لتصميم الكيانات");
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
            picStatus.BackColor = statusType switch
            {
                StatusType.Success => AppTheme.StatusLedGreen,
                StatusType.Error => AppTheme.StatusLedRed,
                StatusType.Warning => AppTheme.StatusLedOrange,
                _ => AppTheme.StatusLedBlue,
            };

        }

        private void UpdateStatus(string message, StatusType statusType = StatusType.Info)
        {
            lblStatus.Text = message;
            UpdateStatusIcon(statusType);

            // تحديث لون النص حسب النوع
            lblStatus.ForeColor = statusType switch
            {
                StatusType.Success => AppTheme.StatusSuccess,
                StatusType.Error => AppTheme.StatusError,
                StatusType.Warning => AppTheme.StatusWarning,
                _ => AppTheme.StatusInfo,
            };

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
                new("PostgreSQL", "PostgreSQL"),
                new("SQL Server", "SQLServer"),
                new("MySQL", "MySQL"),
                new("Oracle", "Oracle"),
                new("SQLite", "SQLite")
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
                    txtPassword.Text = Core.Helpers.PasswordEncryption.Decrypt(_settings.PostgreSqlDefaultPassword);
                    txtPort.Text = _settings.PostgreSqlDefaultPort;
                    break;
                case "SQLServer":
                    txtUsername.Text = _settings.SqlServerDefaultUsername;
                    txtPassword.Text = Core.Helpers.PasswordEncryption.Decrypt(_settings.SqlServerDefaultPassword);
                    break;
                case "MySQL":
                    txtUsername.Text = _settings.MySqlDefaultUsername;
                    txtPassword.Text = Core.Helpers.PasswordEncryption.Decrypt(_settings.MySqlDefaultPassword);
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

            _logger.LogInfo("Testing database connection", "FrmConnection.BtnTestConnection_Click",
                new Dictionary<string, object> { { "DatabaseType", cmbDatabaseType.Text }, { "Server", cmbServer.Text } });

            UpdateStatus("جاري اختبار الاتصال...", StatusType.Info);
            progressBar.Visible = true;
            btnTestConnection.Enabled = false;
            
            // تأثير بصري لمدة قصيرة
            btnTestConnection.BackColor = AppTheme.PrimaryLight;

            try
            {
                bool isConnected = await DatabaseHelper.TestConnection(
                    cmbDatabaseType.Text,
                    cmbServer.Text,
                    txtUsername.Text,
                    txtPassword.Text
                );

                if (isConnected)
                {
                    _logger.LogInfo("Database connection test successful", "FrmConnection.BtnTestConnection_Click");
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
                    _logger.LogWarning("Database connection test failed", null, "FrmConnection.BtnTestConnection_Click",
                        new Dictionary<string, object> { { "DatabaseType", cmbDatabaseType.Text }, { "Server", cmbServer.Text } });
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
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during database connection test", ex, "FrmConnection.BtnTestConnection_Click");
                UpdateStatus("❌ فشل في الاتصال بالسيرفر", StatusType.Error);
                MessageBox.Show(
                    $"حدث خطأ أثناء اختبار الاتصال:\n{ex.Message}",
                    "خطأ في الاتصال",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
            finally
            {
                progressBar.Visible = false;
                btnTestConnection.Enabled = true;
                btnTestConnection.BackColor = AppTheme.Primary;
            }
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
                
                _logger.LogInfo($"Loaded {databases.Count} databases successfully", "FrmConnection.LoadDatabases");
                UpdateStatus($"تم تحميل {databases.Count} قاعدة بيانات", StatusType.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load databases", ex, "FrmConnection.LoadDatabases",
                    new Dictionary<string, object> { { "DatabaseType", cmbDatabaseType.Text }, { "Server", cmbServer.Text } });
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

        private void BtnCodeFirst_Click(object sender, EventArgs e)
        {
            _logger.LogInfo("Opening Code First Entity Designer", "FrmConnection");
            var designer = new FrmEntityDesigner();
            designer.ShowDialog(this);
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInputs() || cmbDatabase.SelectedIndex == -1) return;

            UpdateStatus("جاري إنشاء الاتصال...", StatusType.Info);
            progressBar.Visible = true;
            
            // تأثير بصري للزر
            btnConnect.BackColor = AppTheme.SuccessDark;
            
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
                
                _logger.LogError("Failed to establish database connection", ex, "FrmConnection.BtnConnect_Click",
                    new Dictionary<string, object>
                    {
                        { "DatabaseType", DatabaseType ?? "N/A" },
                        { "Server", cmbServer.Text ?? "N/A" },
                        { "Database", cmbDatabase.Text ?? "N/A" },
                        { "Username", txtUsername.Text ?? "N/A" }
                    });
                
                var errorMessage = new System.Text.StringBuilder();
                errorMessage.AppendLine($"حدث خطأ أثناء إنشاء الاتصال:");
                errorMessage.AppendLine(ex.Message);
                
                if (ex.InnerException != null)
                {
                    errorMessage.AppendLine();
                    errorMessage.AppendLine($"تفاصيل: {ex.InnerException.Message}");
                }
                
                // رسائل مساعدة حسب نوع الخطأ
                if (ex.Message.Contains("network") || ex.Message.Contains("connection"))
                {
                    errorMessage.AppendLine();
                    errorMessage.AppendLine("تأكد من:");
                    errorMessage.AppendLine("• أن السيرفر يعمل");
                    errorMessage.AppendLine("• أن اسم السيرفر صحيح");
                    errorMessage.AppendLine("• أن الجدار الناري يسمح بالاتصال");
                }
                else if (ex.Message.Contains("login") || ex.Message.Contains("authentication"))
                {
                    errorMessage.AppendLine();
                    errorMessage.AppendLine("تأكد من:");
                    errorMessage.AppendLine("• أن اسم المستخدم وكلمة المرور صحيحة");
                    errorMessage.AppendLine("• أن المستخدم لديه صلاحيات الوصول");
                }
                
                MessageBox.Show(
                    errorMessage.ToString(),
                    "خطأ في الاتصال",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
            finally
            {
                progressBar.Visible = false;
                btnConnect.BackColor = AppTheme.Success;
            }
        }

        private void SaveCredentials()
        {
            var selectedType = cmbDatabaseType.SelectedItem as DbType;
            if (selectedType == null) return;
            
            // استخدام التشفير لحفظ كلمات المرور
            switch (selectedType.Value)
            {
                case "PostgreSQL":
                    _settings.PostgreSqlDefaultUsername = txtUsername.Text;
                    _settings.PostgreSqlDefaultPassword = Core.Helpers.PasswordEncryption.Encrypt(txtPassword.Text);
                    _settings.PostgreSqlDefaultPort = txtPort.Text;
                    break;
                case "SQLServer":
                    _settings.SqlServerDefaultUsername = txtUsername.Text;
                    _settings.SqlServerDefaultPassword = Core.Helpers.PasswordEncryption.Encrypt(txtPassword.Text);
                    break;
                case "MySQL":
                    _settings.MySqlDefaultUsername = txtUsername.Text;
                    _settings.MySqlDefaultPassword = Core.Helpers.PasswordEncryption.Encrypt(txtPassword.Text);
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

        private void BtnViewLogs_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.LogInfo("Opening Log Viewer", "FrmConnection.BtnViewLogs_Click");
                LogViewerHelper.ShowLogViewer(this);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error opening Log Viewer", ex, "FrmConnection.BtnViewLogs_Click");
                MessageBox.Show(
                    $"حدث خطأ أثناء فتح عارض السجلات:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
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
            // التحقق من نوع قاعدة البيانات
            if (cmbDatabaseType.SelectedIndex == -1)
            {
                UpdateStatus("الرجاء اختيار نوع قاعدة البيانات", StatusType.Warning);
                return false;
            }

            // التحقق من السيرفر
            if (cmbServer.SelectedIndex == -1 || string.IsNullOrWhiteSpace(cmbServer.Text))
            {
                UpdateStatus("الرجاء اختيار السيرفر", StatusType.Warning);
                return false;
            }

            // التحقق من اسم المستخدم
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                UpdateStatus("الرجاء إدخال اسم المستخدم", StatusType.Warning);
                return false;
            }

            // التحقق من كلمة المرور
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                UpdateStatus("الرجاء إدخال كلمة المرور", StatusType.Warning);
                return false;
            }

            // التحقق من المنفذ (لـ PostgreSQL و MySQL)
            var selectedType = cmbDatabaseType.SelectedItem as DbType;
            bool isPostgres = selectedType?.Value == "PostgreSQL";
            bool isMySql = selectedType?.Value == "MySQL";
            
            if ((isPostgres || isMySql) && !string.IsNullOrWhiteSpace(txtPort.Text))
            {
                if (!int.TryParse(txtPort.Text, out int port) || port <= 0 || port > 65535)
                {
                    UpdateStatus("المنفذ يجب أن يكون رقماً بين 1 و 65535", StatusType.Warning);
                    return false;
                }
            }

            return true;
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

                // استخدام الفئة الموحدة لبناء Connection String
                var dbType = DatabaseTypeExtensions.ParseDatabaseType(selectedType.Value);
                int? port = null;
                if (!string.IsNullOrWhiteSpace(txtPort.Text) && int.TryParse(txtPort.Text, out int portValue))
                {
                    port = portValue;
                }

                ConnectionString = Core.Helpers.ConnectionStringBuilder.Build(
                    dbType,
                    cmbServer.Text,
                    cmbDatabase.Text,
                    txtUsername.Text,
                    txtPassword.Text,
                    port,
                    useIntegratedSecurity: false,
                    trustServerCertificate: true
                );
            }
        }
    }
}