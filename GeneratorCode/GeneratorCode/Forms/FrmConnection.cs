using GeneratorCode.GeneratorCode.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using GeneratorCode.Properties;

namespace GeneratorCode.Forms
{
  
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
            // إخفاء حقل البورت في البداية
            lblPort.Visible = false;
            txtPort.Visible = false;

            // تعبئة قائمة أنواع قواعد البيانات

            // تعطيل العناصر حتى يتم اختيار نوع قاعدة البيانات
            EnableServerControls(false);
            EnableAuthenticationControls(false);
            EnableDatabaseControls(false);
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
            EnableServerControls(true);
            cmbServer.Items.Clear();

            try
            {
                if (selectedType != null)
                {
                    string selectedDb = selectedType.Value;
                    // تحديث خاصية DatabaseType
                    DatabaseType = selectedDb;
                    
                    lblPort.Visible = selectedDb == "PostgreSQL";
                    txtPort.Visible = selectedDb == "PostgreSQL";

                    switch (selectedDb)
                    {
                        case "SQLServer":
                            await LoadSqlServers();
                            break;
                        case "MySQL":
                            cmbServer.Items.Add("localhost");
                            SettAutomaticSettings(selectedDb);
                            break;
                        case "PostgreSQL":
                            cmbServer.Items.Add("localhost");
                            SettAutomaticSettings(selectedDb);
                            break;
                    }
                }

                // حفظ نوع قاعدة البيانات المختار في الإعدادات
                Settings.Default.DatabaseType = selectedType.Value;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                ValidateInputs(sender, e);
            }
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInputs()) return;

            progressBar.Visible = true;
            btnTestConnection.Enabled = false;

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
                    MessageBox.Show(
                        "تم الاتصال بالسيرفر بنجاح!",
                        "نجاح",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RightAlign
                    );
                    await LoadDatabases();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"فشل الاتصال: {ex.Message}",
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
                btnTestConnection.Enabled = true;
            }
        }

        private async Task LoadDatabases()
        {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"حدث خطأ أثناء تحميل قواعد البيانات: {ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
        }

        private void CmbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnConnect.Enabled = cmbDatabase.SelectedIndex != -1;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (!ValidateConnectionInputs() || cmbDatabase.SelectedIndex == -1) return;

            try
            {
                BuildConnectionString();
                DialogResult = DialogResult.OK;
                new FrmTabls(ConnectionString, DatabaseType).ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"حدث خطأ: {ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign
                );
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ValidateInputs(object sender, EventArgs e)
        {
            btnTestConnection.Enabled = ValidateConnectionInputs();
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
        }

        private void EnableAuthenticationControls(bool enable)
        {
            txtUsername.Enabled = enable;
            txtPassword.Enabled = enable;
        }

        private void EnableDatabaseControls(bool enable)
        {
            cmbDatabase.Enabled = enable;
            lblDatabase.Enabled = enable;
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