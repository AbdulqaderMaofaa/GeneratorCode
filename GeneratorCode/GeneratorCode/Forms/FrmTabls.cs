using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Services;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeneratorCode.Forms
{
    /// <summary>
    /// نموذج اختيار الجداول وتوليد الكود
    /// </summary>
    public partial class FrmTabls : Form
    {
        #region Private Fields

        private readonly CodeGenerationContext _context;
        private readonly CodeGenerationService _codeGenerationService;
        private List<TableInfo> _availableTables;

        #endregion

        #region Constructors

        public FrmTabls(string connectionString, string databaseType)
        {
            InitializeComponent();

            // تهيئة أعمدة الجدول
            gridColumns.Columns.Add("ColumnName", "اسم العمود");
            gridColumns.Columns.Add("DataType", "نوع البيانات");
            gridColumns.Columns.Add("IsNullable", "يقبل Null");
            gridColumns.Columns.Add("IsPrimaryKey", "مفتاح رئيسي");
            gridColumns.Columns.Add("IsForeignKey", "مفتاح خارجي");
            gridColumns.Columns.Add("ReferencedTable", "الجدول المرجعي");

            // إنشاء سياق افتراضي
            _context = new CodeGenerationContext
            {
                ConnectionString = connectionString,
                DatabaseType = DatabaseTypeExtensions.ParseDatabaseType(databaseType),
                Namespace = "GeneratedCode",
                OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            // إنشاء خدمة توليد الكود
            var patternFactory = new ArchitecturePatternFactory();
            var databaseFactory = new DatabaseProviderFactory();
            var diProviderFactory = new DIProviderFactory();
            var templateEngine = new Core.TemplateEngine.SimpleTemplateEngine();

            _codeGenerationService = new CodeGenerationService(
                patternFactory, databaseFactory, diProviderFactory, templateEngine);

            InitializeFormWithContext();
            SetupEventHandlers();

            // إضافة مستمع لحدث تحديث الإعدادات
            FrmSettings.SettingsUpdated += OnSettingsUpdated;
        }

        #endregion

        #region Initialization Methods

        private void InitializeFormWithContext()
        {
            try
            {
                UpdateStatusLabel("جاري تهيئة النموذج...");

                // تحديث معلومات النمط والقاعدة
                UpdateDatabaseInfo();

                // تحميل الأنماط المعمارية
                LoadArchitecturePatterns();

                // تحميل خيارات اللغات
                LoadProgrammingLanguages();

                // تحميل الجداول
                LoadTables();

                // تعيين القيم الافتراضية
                SetDefaultValues();

                UpdateStatusLabel("تم تهيئة النموذج بنجاح");
            }
            catch (Exception ex)
            {
                ShowError("خطأ في تهيئة النموذج", ex);
            }
        }

        private void SetupEventHandlers()
        {
            // أحداث تغيير الجدول
            lstTables.SelectedIndexChanged += LstTables_SelectedIndexChanged;

            // أحداث النمط المعماري
            cmbArchitecture.SelectedIndexChanged += CmbArchitecture_SelectedIndexChanged;

            // أحداث اللغة
            cmbLanguage.SelectedIndexChanged += CmbLanguage_SelectedIndexChanged;

            // أحداث الأزرار
            btnGenerate.Click += BtnGenerate_Click;
            btnPreview.Click += BtnPreview_Click;
            btnBrowse.Click += BtnBrowse_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        private void LoadArchitecturePatterns()
        {
            cmbArchitecture.Items.Clear();
            cmbArchitecture.Items.AddRange(new string[] {
                "Clean Architecture",
                "Layered Architecture",
                "Microservices",
                "DDD (Domain-Driven Design)",
                "CQRS"
            });
            cmbArchitecture.SelectedIndex = 0;
        }

        private void LoadProgrammingLanguages()
        {
            cmbLanguage.Items.Clear();
            cmbLanguage.Items.AddRange(new string[] {
                "C#",
                "ASP.NET Web Forms",
                "ASP.NET MVC",
                "ASP.NET Core",
                "TypeScript"
            });
            cmbLanguage.SelectedIndex = 0;
        }

        private void UpdateDatabaseInfo()
        {
            lblDatabaseType.Text = $"نوع قاعدة البيانات: {_context.DatabaseType}";
            lblConnectionInfo.Text = "متصل بنجاح";
            lblConnectionInfo.ForeColor = Color.Green;
        }

        private void LoadTables()
        {
            try
            {
                UpdateStatusLabel("جاري تحميل الجداول...");
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Visible = true;

                _availableTables = _codeGenerationService.GetTables(
                    _context.DatabaseType, _context.ConnectionString);

                if (_availableTables == null || !_availableTables.Any())
                {
                    throw new Exception("لم يتم العثور على أي جداول في قاعدة البيانات");
                }

                lstTables.Items.Clear();
                foreach (var table in _availableTables)
                {
                    if (!string.IsNullOrEmpty(table.Name))
                    {
                        lstTables.Items.Add(table.Name);
                    }
                }

                if (lstTables.Items.Count > 0)
                {
                    lstTables.SelectedIndex = 0;
                }

                UpdateStatusLabel($"تم تحميل {_availableTables.Count} جدول");
            }
            catch (Exception ex)
            {
                ShowError("خطأ في تحميل الجداول", ex);
                UpdateStatusLabel("فشل تحميل الجداول");
            }
            finally
            {
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Visible = false;
            }
        }

        private void SetDefaultValues()
        {
            // إعدادات عامة
            txtNamespace.Text = _context.Namespace;
            txtOutputPath.Text = _context.OutputPath;

            // خيارات توليد الكود
            chkEntities.Checked = true;
            chkDTOs.Checked = true;
            chkRepositories.Checked = true;
            chkServices.Checked = true;
            chkControllers.Checked = true;

            // خيارات إضافية
            chkUnitTests.Checked = true;
            chkValidation.Checked = true;
            chkSwagger.Checked = true;
            chkDependencyInjection.Checked = true;

            // خيارات CRUD
            chkGenerateCreate.Checked = true;
            chkGenerateRead.Checked = true;
            chkGenerateUpdate.Checked = true;
            chkGenerateDelete.Checked = true;
        }

        #endregion

        #region Event Handlers

        private void LstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTables.SelectedIndex != -1)
            {
                var selectedTable = _availableTables[lstTables.SelectedIndex];
                LoadTableDetails(selectedTable);
            }
        }

        private void CmbArchitecture_SelectedIndexChanged(object sender, EventArgs e)
        {
            _context.ArchitecturePattern = cmbArchitecture.Text;
            UpdateArchitectureOptions();
        }

        private void CmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            _context.TargetLanguage = (ProgrammingLanguage)cmbLanguage.SelectedIndex;
            UpdateLanguageOptions();
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    UpdateStatusLabel("جاري توليد المشروع...");
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.Visible = true;
                    if (lstTables.SelectedIndex != -1)
                    {
                        var selectedTable = _availableTables[lstTables.SelectedIndex];
                        UpdateGenerationContext(selectedTable);

                        // إنشاء مجلد المشروع
                        var projectPath = Path.Combine(_context.OutputPath, _context.Namespace);
                        Directory.CreateDirectory(projectPath);

                        // توليد هيكل المشروع
                        if (chkGenerateStartup.Checked)
                            await GenerateStartupFile(projectPath);

                        if (chkGenerateProgram.Checked)
                            await GenerateProgramFile(projectPath);

                        if (chkGenerateGitignore.Checked)
                            await GenerateGitignore(projectPath);

                        if (chkGenerateReadme.Checked)
                            await GenerateReadme(projectPath);

                        // توليد الطبقات المعمارية
                        if (chkInfrastructureLayer.Checked)
                            await GenerateInfrastructureLayer(projectPath);
                        if (chkApplicationLayer.Checked)
                            await GenerateApplicationLayer(projectPath);
                        if (chkDomainLayer.Checked)
                            await GenerateDomainLayer(projectPath);
                        if (chkPresentationLayer.Checked)
                            await GeneratePresentationLayer(projectPath);

                        // توليد الكود للجداول المحددة
                        if (lstTables.SelectedItems.Count > 0)
                        {
                            foreach (var item in lstTables.SelectedItems)
                            {
                                var table = _availableTables.First(t => t.Name == item.ToString());
                                await GenerateCodeForTableAsync(table);
                            }
                        }

                        // توليد ملف الحل
                        if (chkGenerateSolution.Checked)
                            await GenerateSolutionFile(projectPath);

                        UpdateStatusLabel("تم توليد المشروع بنجاح");
                        ShowSuccess("تم توليد المشروع", "تم توليد المشروع بنجاح. هل تريد فتح المجلد؟");
                    }
                    // تحديث إعدادات السياق
                   
                }
                catch (Exception ex)
                {
                    ShowError("خطأ في توليد المشروع", ex);
                }
                finally
                {
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Visible = false;
                }
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (lstTables.SelectedIndex != -1)
            {
                var selectedTable = _availableTables[lstTables.SelectedIndex];
                ShowPreview(selectedTable);
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog();
            dialog.Description = "اختر مجلد حفظ الكود المولد";
            dialog.SelectedPath = txtOutputPath.Text;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text = dialog.SelectedPath;
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            FrmSettings.Instance.ShowDialog(this);
        }

        #endregion

        #region Helper Methods

        private void LoadTableDetails(TableInfo table)
        {
            try
            {
                if (table == null)
                {
                    gridColumns.Rows.Clear();
                    return;
                }

                UpdateStatusLabel($"جاري تحميل تفاصيل الجدول {table.Name}...");
                progressBar.Visible = true;

                // تحميل الأعمدة من قاعدة البيانات
                var columns = _codeGenerationService.GetTableColumns(
                    _context.DatabaseType,
                    _context.ConnectionString,
                    table.Name
                );

                // تحديث معلومات الجدول
                table.Columns = columns;

                // عرض التفاصيل
                DisplayTableDetails(table, columns);

                UpdateStatusLabel($"تم تحميل تفاصيل الجدول {table.Name}");
            }
            catch (Exception ex)
            {
                ShowError("خطأ في تحميل تفاصيل الجدول", ex);
                UpdateStatusLabel("فشل تحميل تفاصيل الجدول");
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private void DisplayTableDetails(TableInfo table, IEnumerable<ColumnInfo> columns)
        {
            gridColumns.Rows.Clear();

            foreach (var column in columns)
            {
                if (column != null)
                {
                    string referencedTable = "";
                    if (column.IsForeignKey && table.ForeignKeys != null)
                    {
                        var fk = table.ForeignKeys.FirstOrDefault(f => f.LocalColumn == column.Name);
                        if (fk != null)
                        {
                            referencedTable = fk.ReferencedTable;
                        }
                    }

                    gridColumns.Rows.Add(
                        column.Name,
                        column.DataType,
                        column.IsNullable,
                        column.IsPrimaryKey,
                        column.IsForeignKey,
                        referencedTable
                    );
                }
            }
        }

        private async Task GenerateCodeForTableAsync(TableInfo table)
        {
            try
            {
                UpdateStatusLabel($"جاري توليد الكود للجدول {table.Name}...");
                _context.TableInfo = table;
                var options = new GenerationOptions
                {
                    // Core Generation Options
                    GenerateEntities = chkEntities.Checked,
                    GenerateDTOs = chkDTOs.Checked,
                    GenerateRepositories = chkRepositories.Checked,
                    GenerateServices = chkServices.Checked,
                    GenerateControllers = chkControllers.Checked,
                    GenerateUnitTests = chkUnitTests.Checked,
                    GenerateValidation = chkValidation.Checked,
                    GenerateSwagger = chkSwagger.Checked,
                    GenerateDependencyInjection = chkDependencyInjection.Checked,

                    // CRUD Operations
                    GenerateCreate = chkGenerateCreate.Checked,
                    GenerateRead = chkGenerateRead.Checked,
                    GenerateUpdate = chkGenerateUpdate.Checked,
                    GenerateDelete = chkGenerateDelete.Checked,

                    // Additional Features
                    EnableAsyncOperations = true,
                    EnableDependencyInjection = chkDependencyInjection.Checked,
                    EnableLogging = true,
                    GenerateApiDocs = chkSwagger.Checked,
                    GenerateModels = chkEntities.Checked,
                    GenerateValidators = chkValidation.Checked,

                    // Project Structure
                    GenerateStartupClass = true,
                    GenerateProgram = true,
                    GenerateGitignore = true,
                    GenerateReadme = true,
                    GenerateSolutionFile = true,

                    // Architecture Components
                    GenerateInfrastructureLayer = true,
                    GenerateApplicationLayer = true,
                    GenerateDomainLayer = true,
                    GeneratePresentationLayer = true
                };
                _context.Options = options;
                await _codeGenerationService.GenerateCodeAsync(_context);
                UpdateStatusLabel($"تم توليد الكود للجدول {table.Name}");
            }
            catch (Exception ex)
            {
                ShowError($"خطأ في توليد الكود للجدول {table.Name}", ex);
            }
        }

        private void UpdateGenerationContext(TableInfo table)
        {
            _context.TableInfo = table;
            _context.ArchitecturePattern = cmbArchitecture.SelectedItem?.ToString().Trim();
            _context.TargetLanguage = (ProgrammingLanguage)cmbLanguage.SelectedIndex;
            _context.Options = new GenerationOptions
            {
                // Core Generation Options
                GenerateEntities = chkEntities.Checked,
                GenerateDTOs = chkDTOs.Checked,
                GenerateRepositories = chkRepositories.Checked,
                GenerateServices = chkServices.Checked,
                GenerateControllers = chkControllers.Checked,
                GenerateUnitTests = chkUnitTests.Checked,
                GenerateValidation = chkValidation.Checked,
                GenerateSwagger = chkSwagger.Checked,
                GenerateDependencyInjection = chkDependencyInjection.Checked,

                // CRUD Operations
                GenerateCreate = chkGenerateCreate.Checked,
                GenerateRead = chkGenerateRead.Checked,
                GenerateUpdate = chkGenerateUpdate.Checked,
                GenerateDelete = chkGenerateDelete.Checked,

                // Additional Features
                EnableAsyncOperations = true,
                EnableDependencyInjection = chkDependencyInjection.Checked,
                EnableLogging = true,
                GenerateApiDocs = chkSwagger.Checked,
                GenerateModels = chkEntities.Checked,
                GenerateValidators = chkValidation.Checked,

                // Project Structure
                GenerateStartupClass = true,
                GenerateProgram = true,
                GenerateGitignore = true,
                GenerateReadme = true,
                GenerateSolutionFile = true,

                // Architecture Components
                GenerateInfrastructureLayer = true,
                GenerateApplicationLayer = true,
                GenerateDomainLayer = true,
                GeneratePresentationLayer = true
            };
            _context.Namespace = txtNamespace.Text;
            _context.OutputPath = txtOutputPath.Text;
            _context.ArchitecturePattern = cmbArchitecture.SelectedItem?.ToString().Trim().Replace(" ", "");
        }

        private void UpdateArchitectureOptions()
        {
            switch (_context.ArchitecturePattern?.ToLower())
            {
                case "clean architecture":
                    EnableAllOptions();
                    break;

                case "layered architecture":
                    EnableAllOptions();
                    chkDTOs.Checked = false;
                    chkDTOs.Enabled = false;
                    break;

                case "microservices":
                    EnableAllOptions();
                    break;

                case "ddd (domain-driven design)":
                    EnableAllOptions();
                    break;

                case "cqrs":
                    EnableAllOptions();
                    break;

                default:
                    DisableModernOptions();
                    break;
            }
        }

        private void UpdateLanguageOptions()
        {
            switch (_context.TargetLanguage)
            {
                case ProgrammingLanguage.CSharp:
                    EnableAllOptions();
                    break;

                case ProgrammingLanguage.AspNetWebForms:
                    DisableModernOptions();
                    break;

                case ProgrammingLanguage.AspNetMvc:
                    EnableWebOptions();
                    break;

                case ProgrammingLanguage.AspNetCore:
                    EnableAllOptions();
                    break;

                case ProgrammingLanguage.TypeScript:
                    EnableTypeScriptOptions();
                    break;
            }
        }

        private void EnableAllOptions()
        {
            chkEntities.Enabled = true;
            chkDTOs.Enabled = true;
            chkRepositories.Enabled = true;
            chkServices.Enabled = true;
            chkControllers.Enabled = true;
            chkUnitTests.Enabled = true;
            chkValidation.Enabled = true;
            chkSwagger.Enabled = true;
            chkDependencyInjection.Enabled = true;
        }

        private void DisableModernOptions()
        {
            chkDTOs.Enabled = false;
            chkUnitTests.Enabled = false;
            chkSwagger.Enabled = false;
            chkDependencyInjection.Enabled = false;
        }

        private void EnableWebOptions()
        {
            EnableAllOptions();
            chkDTOs.Enabled = false;
            chkSwagger.Enabled = false;
        }

        private void EnableTypeScriptOptions()
        {
            chkEntities.Enabled = true;
            chkDTOs.Enabled = true;
            chkServices.Enabled = true;
            chkControllers.Enabled = false;
            chkUnitTests.Enabled = true;
            chkValidation.Enabled = true;
            chkSwagger.Enabled = false;
            chkDependencyInjection.Enabled = false;
        }

        private void UpdateSettingsInfo()
        {
            try
            {
                var settings = Properties.Settings.Default;

                lblDIInfo.Text = settings.EnableDI ? "مفعل" : "معطل";
                lblDIInfo.ForeColor = settings.EnableDI ? Color.Green : Color.Red;

                lblValidationInfo.Text = settings.EnableValidation ? "مفعل" : "معطل";
                lblValidationInfo.ForeColor = settings.EnableValidation ? Color.Green : Color.Red;

                lblTestingInfo.Text = settings.EnableTesting ? "مفعل" : "معطل";
                lblTestingInfo.ForeColor = settings.EnableTesting ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                ShowError("خطأ في تحديث معلومات الإعدادات", ex);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtNamespace.Text))
            {
                ShowError("خطأ في المدخلات", "الرجاء إدخال اسم النطاق (Namespace)");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                ShowError("خطأ في المدخلات", "الرجاء تحديد مسار حفظ الملفات");
                return false;
            }

            if (lstTables.SelectedItems.Count == 0)
            {
                ShowError("خطأ في المدخلات", "الرجاء اختيار جدول واحد على الأقل");
                return false;
            }

            if (!chkEntities.Checked && !chkDTOs.Checked && !chkRepositories.Checked &&
                !chkServices.Checked && !chkControllers.Checked)
            {
                ShowError("خطأ في المدخلات", "الرجاء اختيار نوع كود واحد على الأقل للتوليد");
                return false;
            }

            return true;
        }

        private void UpdateStatusLabel(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }

        private static void ShowError(string title, Exception ex)
        {
            MessageBox.Show(
                $"حدث خطأ: {ex.Message}",
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign
            );
        }

        private static void ShowError(string title, string message)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign
            );
        }

        private void ShowSuccess(string title, string message)
        {
            var result = MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign
            );

            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("explorer.exe", txtOutputPath.Text);
            }
        }

        private void ShowPreview(TableInfo table)
        {
            try
            {
                UpdateStatusLabel("جاري تحضير المعاينة...");

                // تحديث سياق التوليد
                UpdateGenerationContext(table);


                var preview = _codeGenerationService.GeneratePreview(table, _context);

                if (preview.Success)
                {
                    var frmPreview = new FrmPreview(preview);
                    frmPreview.ShowDialog();
                    UpdateStatusLabel("تم عرض المعاينة");
                }
                else
                {
                    ShowError("خطأ في المعاينة", preview.Error);
                    UpdateStatusLabel("فشل عرض المعاينة");
                }
            }
            catch (Exception ex)
            {
                ShowError("خطأ في عرض المعاينة", ex);
                UpdateStatusLabel("فشل عرض المعاينة");
            }
        }

        private void OnSettingsUpdated()
        {
            // تحديث أي شيء يعتمد على الإعدادات
            LoadSettings();
        }

        private static void LoadSettings()
        {
            // تحميل الإعدادات وتحديث واجهة المستخدم
            //_settings = Properties.Settings.Default;
            // ... تحديث العناصر حسب الإعدادات
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // إزالة مستمع الحدث عند إغلاق النموذج
            FrmSettings.SettingsUpdated -= OnSettingsUpdated;
        }

        private async Task GenerateStartupFile(string projectPath)
        {
            UpdateStatusLabel("جاري توليد Startup.cs...");
            var startupPath = Path.Combine(projectPath, "Startup.cs");
            await _codeGenerationService.GenerateStartupFile(_context, startupPath);
        }

        private async Task GenerateProgramFile(string projectPath)
        {
            UpdateStatusLabel("جاري توليد Program.cs...");
            var programPath = Path.Combine(projectPath, "Program.cs");
            await _codeGenerationService.GenerateProgramFile(_context, programPath);
        }

        private async Task GenerateGitignore(string projectPath)
        {
            UpdateStatusLabel("جاري توليد .gitignore...");
            var gitignorePath = Path.Combine(projectPath, ".gitignore");
            await _codeGenerationService.GenerateGitignore(gitignorePath);
        }

        private async Task GenerateReadme(string projectPath)
        {
            UpdateStatusLabel("جاري توليد README.md...");
            var readmePath = Path.Combine(projectPath, "README.md");
            await _codeGenerationService.GenerateReadme(_context, readmePath);
        }

        private async Task GenerateInfrastructureLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Infrastructure...");
            var infrastructurePath = Path.Combine(projectPath, "Infrastructure");
            Directory.CreateDirectory(infrastructurePath);
            await _codeGenerationService.GenerateInfrastructureLayer(_context, infrastructurePath);
        }

        private async Task GenerateApplicationLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Application...");
            var applicationPath = Path.Combine(projectPath, "Application");
            Directory.CreateDirectory(applicationPath);
            await _codeGenerationService.GenerateApplicationLayer(_context, applicationPath);
        }

        private async Task GenerateDomainLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Domain...");
            var domainPath = Path.Combine(projectPath, "Domain");
            Directory.CreateDirectory(domainPath);
            await _codeGenerationService.GenerateDomainLayer(_context, domainPath);
        }

        private async Task GeneratePresentationLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Presentation...");
            var presentationPath = Path.Combine(projectPath, "Presentation");
            
            // إنشاء المجلدات المطلوبة
            Directory.CreateDirectory(presentationPath);
            Directory.CreateDirectory(Path.Combine(presentationPath, "Controllers"));
            Directory.CreateDirectory(Path.Combine(presentationPath, "Views", _context.EntityName));
            Directory.CreateDirectory(Path.Combine(presentationPath, "ViewModels"));
            
            await _codeGenerationService.GeneratePresentationLayer(_context, presentationPath);
        }

        private async Task GenerateSolutionFile(string projectPath)
        {
            UpdateStatusLabel("جاري توليد ملف الحل...");
            await _codeGenerationService.GenerateSolutionFile(_context, projectPath);
        }

        #endregion



    }
}