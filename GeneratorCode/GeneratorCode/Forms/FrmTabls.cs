using GeneratorCode.Core.Factories;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Services;
using GeneratorCode.Core.Logging;
using GeneratorCode.GeneratorCode.Helpers;
using GeneratorCode.Helpers;
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
        private readonly ILogger _logger;
        private List<TableInfo> _availableTables;
        private FrmProgress _progressForm;

        #endregion

        #region Constructors

        public FrmTabls(string connectionString, string databaseType)
        {
            _logger = LoggerFactory.Default;
            _logger.LogDebug("Initializing FrmTabls", "FrmTabls");

            InitializeComponent();

            KeyDown += FrmTabls_KeyDown;

            SetupFormStyling();
            ApplyTheme();

            // تهيئة أعمدة الجدول
            SetupDataGridColumns();

            // إنشاء سياق افتراضي
            _context = new CodeGenerationContext
            {
                ConnectionString = connectionString,
                DatabaseType = DatabaseTypeExtensions.ParseDatabaseType(databaseType),
                Namespace = "GeneratedCode",
                OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            // إنشاء خدمة توليد الكود
            var templateEngine = new Core.TemplateEngine.SimpleTemplateEngine(_logger);
            var patternFactory = new ArchitecturePatternFactory(templateEngine);
            var databaseFactory = new DatabaseProviderFactory();
            var diProviderFactory = new DIProviderFactory();

            _codeGenerationService = new CodeGenerationService(
                patternFactory, databaseFactory, diProviderFactory, templateEngine, _logger);

            InitializeFormWithContext();
            SetupTooltips();

            // إضافة مستمع لحدث تحديث الإعدادات
            FrmSettings.SettingsUpdated += OnSettingsUpdated;

            _logger.LogInfo("FrmTabls initialized successfully", "FrmTabls",
                new Dictionary<string, object> { { "DatabaseType", databaseType } });
        }

        #endregion

        #region Initialization Methods

        private void SetupFormStyling()
        {
            AppTheme.StyleForm(this);
            AppTheme.StyleTabControl(tabControl);
        }

        private void ApplyTheme()
        {
            AppTheme.StyleButton(btnBrowse, AppTheme.Primary);
            AppTheme.StyleButton(btnSettings, AppTheme.Purple);
            AppTheme.StyleButton(btnGenerate, AppTheme.Success, large: true);
            AppTheme.StyleButton(btnPreview, AppTheme.Primary);
            AppTheme.StyleButton(btnViewLogs, AppTheme.Purple);

            AppTheme.StyleGroupBox(grpTablesInfo, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpColumnsInfo, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpArchitecture, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpCodeGeneration, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpCrudOperations, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpProjectStructure, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpProjectSettings, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpDatabaseInfo, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpActions, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpProgress, AppTheme.PrimaryDark);
        }

        private void SetupDataGridColumns()
        {
            AppTheme.StyleDataGridView(gridColumns);

            gridColumns.Columns.Add("ColumnName", "اسم العمود");
            gridColumns.Columns.Add("DataType", "نوع البيانات");
            gridColumns.Columns.Add("IsNullable", "يقبل Null");
            gridColumns.Columns.Add("IsPrimaryKey", "مفتاح رئيسي");
            gridColumns.Columns.Add("IsForeignKey", "مفتاح خارجي");
            gridColumns.Columns.Add("ReferencedTable", "الجدول المرجعي");

            // RTL-specific overrides
            gridColumns.RightToLeft = RightToLeft.Yes;
            gridColumns.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridColumns.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetupTooltips()
        {
            var tooltip = AppTheme.CreateTooltipProvider();
            tooltip.SetToolTip(chkGenerateAllTables, "إذا تم تحديد هذا الخيار، سيتم توليد الكود لجميع الجداول في قاعدة البيانات");
            tooltip.SetToolTip(cmbArchitecture, "اختر النمط المعماري الذي تريد استخدامه في مشروعك");
            tooltip.SetToolTip(cmbLanguage, "اختر لغة البرمجة أو إطار العمل المطلوب");
            tooltip.SetToolTip(txtNamespace, "مساحة الاسم التي سيتم استخدامها في الكود المولد");
            tooltip.SetToolTip(txtOutputPath, "المسار الذي سيتم حفظ الملفات المولدة فيه");
            tooltip.SetToolTip(btnGenerate, "انقر لبدء عملية توليد الكود");
            tooltip.SetToolTip(btnPreview, "انقر لمعاينة الكود قبل التوليد");
            tooltip.SetToolTip(lstTables, "قائمة الجداول المتاحة في قاعدة البيانات - يمكن تحديد عدة جداول بالضغط على Ctrl");
            tooltip.SetToolTip(gridColumns, "تفاصيل الأعمدة للجدول المحدد");

            // التلميحات ستتبع إعدادات RTL للنموذج تلقائياً
        }

        private void InitializeFormWithContext()
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

        private void LoadArchitecturePatterns()
        {
            cmbArchitecture.Items.Clear();
            cmbArchitecture.Items.AddRange(new string[] {
                "Clean Architecture",
                "Simple Architecture",
                "Layered Architecture",
                "CQRS",
                "DDD (Domain-Driven Design)",
                "Microservices"
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
            lblConnectionInfo.ForeColor = AppTheme.StatusLedGreen;
        }

        private void LoadTables()
        {
            UpdateStatusLabel("جاري تحميل الجداول...");
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Visible = true;
            Cursor = Cursors.WaitCursor;

            try
            {
                _availableTables = _codeGenerationService.GetTables(
                    _context.DatabaseType, _context.ConnectionString);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                _logger.LogError("Error loading tables", ex, "FrmTabls.LoadTables");
                _availableTables = new List<TableInfo>();
                UpdateStatusLabel("فشل في تحميل الجداول");
                progressBar.Visible = false;
                MessageBox.Show(
                    $"تعذّر تحميل الجداول من قاعدة البيانات:\n{ex.Message}",
                    "خطأ في الاتصال",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_availableTables == null || !_availableTables.Any())
            {
                Cursor = Cursors.Default;
                _availableTables = new List<TableInfo>();
                UpdateStatusLabel("لا توجد جداول في قاعدة البيانات");
                progressBar.Visible = false;
                MessageBox.Show(
                    "لم يتم العثور على أي جداول في قاعدة البيانات.\nتأكد من أن قاعدة البيانات تحتوي على جداول.",
                    "لا توجد جداول",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
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
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Visible = false;
            Cursor = Cursors.Default;

            lblTablesSelection.Text = $"الجداول المتاحة ({_availableTables.Count} جدول متاح):";
            if (chkGenerateAllTables.Checked)
            {
                lblTablesSelection.Text = $"سيتم توليد الكود لجميع الجداول ({_availableTables.Count} جداول):";
            }

            lstTables.RightToLeft = RightToLeft.Yes;
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

            // خيارات اختيار الجداول
            chkGenerateAllTables.Checked = false; // الافتراضي هو التحديد اليدوي

            // خيارات بنية المشروع
            chkGenerateStartup.Checked = true;
            chkGenerateProgram.Checked = true;
            chkGenerateGitignore.Checked = true;
            chkGenerateReadme.Checked = true;
            chkGenerateSolution.Checked = true;

            // خيارات طبقات النمط المعماري
            chkInfrastructureLayer.Checked = true;
            chkApplicationLayer.Checked = true;
            chkDomainLayer.Checked = true;
            chkPresentationLayer.Checked = true;
        }

        #endregion

        #region Event Handlers

        private void LstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTables.SelectedIndex != -1 && _availableTables != null &&
                lstTables.SelectedIndex < _availableTables.Count)
            {
                var selectedTable = _availableTables[lstTables.SelectedIndex];
                if (selectedTable != null)
                {
                    LoadTableDetails(selectedTable);
                }
            }

            // تحديث النص التوضيحي للجداول المحددة
            if (!chkGenerateAllTables.Checked)
            {
                int selectedCount = lstTables.SelectedItems.Count;
                if (selectedCount > 0)
                {
                    lblTablesSelection.Text = $"الجداول المتاحة (محدد {selectedCount} من {lstTables.Items.Count}):";
                }
                else
                {
                    lblTablesSelection.Text = "الجداول المتاحة (يمكن تحديد عدة جداول):";
                }
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

        private void ChkGenerateAllTables_CheckedChanged(object sender, EventArgs e)
        {
            // عند تفعيل خيار "توليد جميع الجداول"
            if (chkGenerateAllTables.Checked)
            {
                // تحديد جميع الجداول
                for (int i = 0; i < lstTables.Items.Count; i++)
                {
                    lstTables.SetSelected(i, true);
                }
                lstTables.Enabled = false; // منع التحديد اليدوي
                lblTablesSelection.Text = $"سيتم توليد الكود لجميع الجداول ({lstTables.Items.Count} جداول):";
            }
            else
            {
                // إلغاء تحديد جميع الجداول
                lstTables.ClearSelected();
                lstTables.Enabled = true; // تمكين التحديد اليدوي
                lblTablesSelection.Text = "الجداول المتاحة (يمكن تحديد عدة جداول):";
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // تحديث الحالة عند تغيير التبويب
            switch (tabControl.SelectedIndex)
            {
                case 0: // تبويب الجداول
                    UpdateStatusLabel("اختر الجداول المطلوبة لتوليد الكود");
                    break;
                case 1: // تبويب النمط المعماري
                    UpdateStatusLabel("اختر النمط المعماري ولغة البرمجة");
                    break;
                case 2: // تبويب خيارات التوليد
                    UpdateStatusLabel("حدد خيارات التوليد المطلوبة");
                    break;
                case 3: // تبويب الإعدادات
                    UpdateStatusLabel("تحقق من إعدادات المشروع");
                    break;
                case 4: // تبويب المخرجات
                    UpdateStatusLabel("جاهز لتوليد الكود");
                    ValidateReadyForGeneration();
                    break;
            }
        }

        private void ValidateReadyForGeneration()
        {
            // التحقق من جاهزية التوليد
            bool isValid = true;
            string validationMessage = "";

            // التحقق من اختيار الجداول
            if (!chkGenerateAllTables.Checked && lstTables.SelectedItems.Count == 0)
            {
                isValid = false;
                validationMessage = "يجب اختيار جدول واحد على الأقل أو تحديد خيار توليد جميع الجداول";
            }

            // التحقق من مسار الإخراج
            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                isValid = false;
                validationMessage = "يجب تحديد مسار الإخراج";
            }

            // التحقق من مساحة الاسم
            if (string.IsNullOrWhiteSpace(txtNamespace.Text))
            {
                isValid = false;
                validationMessage = "يجب تحديد مساحة الاسم";
            }

            // تحديث واجهة المستخدم
            if (isValid)
            {
                btnGenerate.Enabled = true;
                btnPreview.Enabled = true;
                UpdateStatusLabel("✅ جاهز للتوليد - جميع الإعدادات صحيحة");
                lblStatus.ForeColor = AppTheme.StatusLedGreen;
            }
            else
            {
                btnGenerate.Enabled = false;
                btnPreview.Enabled = false;
                UpdateStatusLabel($"❌ {validationMessage}");
                lblStatus.ForeColor = AppTheme.StatusLedRed;
            }
        }

        private async void BtnGenerate_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                try
                {
                    // إنشاء وإعداد نافذة التقدم
                    _progressForm = new FrmProgress();

                    // حساب إجمالي الملفات المتوقعة
                    int totalFiles = CalculateExpectedFilesCount();
                    _progressForm.SetTotalFiles(totalFiles);

                    // عرض نافذة التقدم
                    _progressForm.Show();
                    _progressForm.ReportInfo($"بدء توليد المشروع: {_context.Namespace}");

                    // تحديث السياق الأساسي
                    if (_availableTables != null && _availableTables.Any())
                    {
                        UpdateGenerationContext(_availableTables.First());
                    }

                    // إنشاء مجلد المشروع
                    var projectPath = Path.Combine(_context.OutputPath, _context.Namespace);
                    Directory.CreateDirectory(projectPath);
                    _progressForm.SetProjectPath(projectPath);
                    _progressForm.ReportInfo($"تم إنشاء مجلد المشروع: {projectPath}");

                    // توليد هيكل المشروع
                    _progressForm.ShowStep("توليد هيكل المشروع");

                    if (chkGenerateStartup.Checked)
                        await GenerateStartupFile(projectPath);

                    if (chkGenerateProgram.Checked)
                        await GenerateProgramFile(projectPath);

                    if (chkGenerateGitignore.Checked)
                        await GenerateGitignore(projectPath);

                    if (chkGenerateReadme.Checked)
                        await GenerateReadme(projectPath);

                    // توليد الطبقات المعمارية
                    _progressForm.ShowStep("توليد الطبقات المعمارية");

                    if (chkInfrastructureLayer.Checked)
                        await GenerateInfrastructureLayer(projectPath);
                    if (chkApplicationLayer.Checked)
                        await GenerateApplicationLayer(projectPath);
                    if (chkDomainLayer.Checked)
                        await GenerateDomainLayer(projectPath);
                    if (chkPresentationLayer.Checked)
                        await GeneratePresentationLayer(projectPath);

                    // توليد الكود للجداول المحددة أو جميع الجداول
                    _progressForm.ShowStep("توليد الكود للجداول");

                    if (chkGenerateAllTables.Checked)
                    {
                        // توليد لجميع الجداول
                        if (_availableTables != null && _availableTables.Any())
                        {
                            foreach (var table in _availableTables)
                            {
                                _progressForm.ShowTable(table.Name);
                                await GenerateCodeForTableAsync(table);
                            }
                        }
                    }
                    else
                    {
                        // توليد للجداول المحددة فقط
                        if (lstTables.SelectedItems.Count > 0 && _availableTables != null)
                        {
                            foreach (var item in lstTables.SelectedItems)
                            {
                                if (item != null)
                                {
                                    var table = _availableTables.FirstOrDefault(t => t != null && t.Name == item.ToString());
                                    if (table != null)
                                    {
                                        _progressForm.ShowTable(table.Name);
                                        await GenerateCodeForTableAsync(table);
                                    }
                                }
                            }
                        }
                    }

                    // توليد ملف الحل
                    _progressForm.ShowStep("توليد ملف الحل");
                    if (chkGenerateSolution.Checked)
                        await GenerateSolutionFile(projectPath);

                    // إنهاء التقدم
                    _progressForm.CompleteProgress();

                    // حساب عدد الجداول المُولدة
                    int generatedTablesCount = chkGenerateAllTables.Checked
                        ? _availableTables.Count
                        : lstTables.SelectedItems.Count;

                    _progressForm.ReportInfo($"تم توليد {generatedTablesCount} جدول بنجاح!");
                    _progressForm.ReportInfo($"مسار المشروع: {projectPath}");

                    // إخفاء مؤشر التقدم في النافذة الرئيسية
                    UpdateStatusLabel("تم توليد المشروع بنجاح");
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Visible = false;
                }
                catch (Exception ex)
                {
                    _progressForm?.ReportError($"حدث خطأ أثناء التوليد: {ex.Message}");
                    ShowError("خطأ في التوليد", ex);

                    // إخفاء مؤشر التقدم في النافذة الرئيسية
                    UpdateStatusLabel("فشل في توليد المشروع");
                    progressBar.Style = ProgressBarStyle.Blocks;
                    progressBar.Visible = false;
                }
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (lstTables.SelectedIndex != -1 && _availableTables != null &&
                lstTables.SelectedIndex < _availableTables.Count)
            {
                var selectedTable = _availableTables[lstTables.SelectedIndex];
                if (selectedTable != null)
                {
                    ShowPreview(selectedTable);
                }
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnBrowse.Enabled)
                    return;

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => BtnBrowse_Click(sender, e)));
                    return;
                }

                using var dialog = new FolderBrowserDialog
                {
                    Description = "اختر مسار لحفظ الكود المولد",
                    SelectedPath = !string.IsNullOrEmpty(txtOutputPath.Text) && Directory.Exists(txtOutputPath.Text)
                        ? txtOutputPath.Text
                        : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    txtOutputPath.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error opening folder browser dialog", ex, "FrmTabls.BtnBrowse_Click");
                MessageBox.Show(
                    $"حدث خطأ أثناء فتح نافذة اختيار المجلد:\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            FrmSettings.Instance.ShowDialog(this);
        }

        private void BtnViewLogs_Click(object sender, EventArgs e)
        {
            LogViewerHelper.ShowLogViewer(this);
        }

        #endregion

        #region Helper Methods

        private void LoadTableDetails(TableInfo table)
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
            progressBar.Visible = false;
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
            if (table == null)
            {
                _logger.LogWarning("Table info is null in GenerateCodeForTableAsync", null, "FrmTabls.GenerateCodeForTableAsync");
                UpdateStatusLabel("خطأ: معلومات الجدول غير متوفرة");
                return;
            }

            _logger.LogInfo($"Starting code generation for table: {table.Name}", "FrmTabls.GenerateCodeForTableAsync",
                new Dictionary<string, object> { { "TableName", table.Name }, { "ArchitecturePattern", _context.ArchitecturePattern ?? "N/A" } });

            UpdateStatusLabel($"جاري توليد الكود للجدول {table.Name}...");

            try
            {
                // تحديث السياق لكل جدول على حدة
                UpdateGenerationContext(table);

                // تقرير الملفات الفردية
                if (chkEntities.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}.cs", "Entity");
                if (chkDTOs.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}DTO.cs", "DTO");
                if (chkRepositories.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}Repository.cs", "Repository");
                if (chkServices.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}Service.cs", "Service");
                if (chkControllers.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}Controller.cs", "Controller");
                if (chkUnitTests.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}Tests.cs", "Unit Test");
                if (chkValidation.Checked)
                    _progressForm?.ReportFileGenerated($"{table.Name}Validator.cs", "Validator");

                var result = await _codeGenerationService.GenerateCodeAsync(_context);

                if (result.Success)
                {
                    _logger.LogInfo($"Code generation completed successfully for table: {table.Name}. Generated {result.GeneratedFiles.Count} files",
                        "FrmTabls.GenerateCodeForTableAsync");
                    UpdateStatusLabel($"تم توليد الكود للجدول {table.Name}");
                }
                else
                {
                    _logger.LogError($"Code generation failed for table: {table.Name}", null, "FrmTabls.GenerateCodeForTableAsync",
                        new Dictionary<string, object> { { "Errors", string.Join("; ", result.Errors) } });
                    UpdateStatusLabel($"فشل توليد الكود للجدول {table.Name}");
                    ShowError("خطأ في التوليد", result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during code generation for table: {table.Name}", ex, "FrmTabls.GenerateCodeForTableAsync");
                UpdateStatusLabel($"خطأ في توليد الكود للجدول {table.Name}");
                ShowError("خطأ في التوليد", ex);
            }
        }

        private void UpdateGenerationContext(TableInfo table)
        {
            if (table == null)
                return;

            _context.TableInfo = table;
            _context.TableName = table.Name ?? string.Empty;
            _context.EntityName = table.Name ?? string.Empty; // Set EntityName from table name
            _context.ClassName = table.Name ?? string.Empty; // Set ClassName from table name
            _context.ArchitecturePattern = cmbArchitecture.SelectedItem?.ToString()?.Trim()?.Replace(" ", "") ?? string.Empty;
            _context.TargetLanguage = (ProgrammingLanguage)cmbLanguage.SelectedIndex;
            _context.Options = CreateGenerationOptions();
            _context.Namespace = txtNamespace.Text;
            _context.OutputPath = txtOutputPath.Text;

            if (cmbTargetFramework != null && cmbTargetFramework.SelectedIndex >= 0)
            {
                _context.TargetFramework = cmbTargetFramework.SelectedIndex switch
                {
                    0 => "net6.0",
                    1 => "net7.0",
                    2 => "net8.0",
                    3 => "net9.0",
                    _ => "net8.0"
                };
            }
        }

        /// <summary>
        /// إنشاء خيارات التوليد من واجهة المستخدم
        /// </summary>
        private GenerationOptions CreateGenerationOptions()
        {
            return new GenerationOptions
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
        }

        private void UpdateArchitectureOptions()
        {
            switch (_context.ArchitecturePattern?.ToLower())
            {
                case "clean architecture":
                case "cleanarchitecture":
                    EnableAllOptions();
                    break;

                case "simple architecture":
                case "simplearchitecture":
                    EnableAllOptions();
                    chkDTOs.Checked = false;
                    chkDTOs.Enabled = false;
                    chkSwagger.Checked = false;
                    chkSwagger.Enabled = false;
                    break;

                case "layered architecture":
                case "layeredarchitecture":
                    EnableAllOptions();
                    chkDTOs.Checked = false;
                    chkDTOs.Enabled = false;
                    break;

                case "microservices":
                case "microservicesarchitecture":
                    EnableAllOptions();
                    break;

                case "ddd (domain-driven design)":
                case "ddd":
                    EnableAllOptions();
                    break;

                case "cqrs":
                    EnableAllOptions();
                    break;

                default:
                    EnableAllOptions();
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



        private bool ValidateInputs()
        {
            // التحقق من Namespace
            if (string.IsNullOrWhiteSpace(txtNamespace.Text))
            {
                ShowError("خطأ في المدخلات", "الرجاء إدخال اسم النطاق (Namespace)");
                txtNamespace.Focus();
                return false;
            }

            // التحقق من صحة Namespace (يجب أن يكون معرف C# صالح)
            if (!IsValidNamespace(txtNamespace.Text))
            {
                ShowError("خطأ في المدخلات", "اسم النطاق (Namespace) غير صالح. يجب أن يبدأ بحرف أو underscore ويحتوي فقط على أحرف وأرقام و underscores");
                txtNamespace.Focus();
                return false;
            }

            // التحقق من Output Path
            if (string.IsNullOrWhiteSpace(txtOutputPath.Text))
            {
                ShowError("خطأ في المدخلات", "الرجاء تحديد مسار حفظ الملفات");
                txtOutputPath.Focus();
                return false;
            }

            // التحقق من أن المسار صالح
            try
            {
                var fullPath = Path.GetFullPath(txtOutputPath.Text);
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    ShowError("خطأ في المدخلات", $"المسار المحدد غير موجود: {directory}");
                    txtOutputPath.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowError("خطأ في المدخلات", $"مسار غير صالح: {ex.Message}");
                txtOutputPath.Focus();
                return false;
            }

            // التحقق من الجداول المتاحة
            if (_availableTables == null || !_availableTables.Any())
            {
                ShowError("خطأ في المدخلات", "لا توجد جداول متاحة للتوليد");
                return false;
            }

            // التحقق من الجداول المحددة إذا لم يتم تفعيل خيار "توليد جميع الجداول"
            if (!chkGenerateAllTables.Checked && lstTables.SelectedItems.Count == 0)
            {
                ShowError("خطأ في المدخلات", "الرجاء اختيار جدول واحد على الأقل أو تفعيل خيار 'توليد جميع الجداول'");
                return false;
            }

            // التحقق من اختيار نوع كود واحد على الأقل
            if (!chkEntities.Checked && !chkDTOs.Checked && !chkRepositories.Checked &&
                !chkServices.Checked && !chkControllers.Checked)
            {
                ShowError("خطأ في المدخلات", "الرجاء اختيار نوع كود واحد على الأقل للتوليد");
                return false;
            }

            return true;
        }

        /// <summary>
        /// التحقق من صحة Namespace (يجب أن يكون معرف C# صالح)
        /// </summary>
        private bool IsValidNamespace(string namespaceName)
        {
            if (string.IsNullOrWhiteSpace(namespaceName))
                return false;

            // يجب أن يبدأ بحرف أو underscore
            if (!char.IsLetter(namespaceName[0]) && namespaceName[0] != '_')
                return false;

            // يجب أن يحتوي فقط على أحرف وأرقام و underscores ونقاط
            foreach (var c in namespaceName)
            {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                    return false;
            }

            // يجب ألا يحتوي على مسافات
            if (namespaceName.Contains(' '))
                return false;

            return true;
        }

        private void UpdateStatusLabel(string message)
        {
            lblStatus.Text = message;
            Application.DoEvents();
        }

        private void ShowError(string title, Exception ex)
        {
            _logger.LogError($"Error in FrmTabls: {title}", ex, "FrmTabls.ShowError");

            var errorMessage = new System.Text.StringBuilder();
            errorMessage.AppendLine($"حدث خطأ: {ex.Message}");

            if (ex.InnerException != null)
            {
                errorMessage.AppendLine();
                errorMessage.AppendLine($"تفاصيل إضافية: {ex.InnerException.Message}");
            }

            // إضافة نوع الخطأ للمساعدة في التشخيص
            errorMessage.AppendLine();
            errorMessage.AppendLine($"نوع الخطأ: {ex.GetType().Name}");

            // إضافة StackTrace في وضع التطوير (اختياري)
#if DEBUG
            errorMessage.AppendLine();
            errorMessage.AppendLine("Stack Trace:");
            errorMessage.AppendLine(ex.StackTrace);
#endif

            MessageBox.Show(
                errorMessage.ToString(),
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
            if (table == null)
            {
                ShowError("خطأ في المعاينة", "معلومات الجدول غير متوفرة");
                return;
            }

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
                ShowError("خطأ في المعاينة", preview.Error ?? "حدث خطأ غير معروف");
                UpdateStatusLabel("فشل عرض المعاينة");
            }
        }

        private void OnSettingsUpdated()
        {
            // تحديث أي شيء يعتمد على الإعدادات
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = Properties.Settings.Default;

            if (!string.IsNullOrEmpty(settings.DefaultNamespace))
                txtNamespace.Text = settings.DefaultNamespace;

            if (!string.IsNullOrEmpty(settings.DefaultOutputPath))
                txtOutputPath.Text = settings.DefaultOutputPath;

            chkDependencyInjection.Checked = settings.EnableDI;
            chkValidation.Checked = settings.EnableValidation;
            chkUnitTests.Checked = settings.EnableTesting;
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
            _progressForm?.ReportFileGenerated("Startup.cs", "تم توليد");
        }

        private async Task GenerateProgramFile(string projectPath)
        {
            UpdateStatusLabel("جاري توليد Program.cs...");
            var programPath = Path.Combine(projectPath, "Program.cs");
            await _codeGenerationService.GenerateProgramFile(_context, programPath);
            _progressForm?.ReportFileGenerated("Program.cs", "تم توليد");
        }

        private async Task GenerateGitignore(string projectPath)
        {
            UpdateStatusLabel("جاري توليد .gitignore...");
            var gitignorePath = Path.Combine(projectPath, ".gitignore");
            await _codeGenerationService.GenerateGitignore(gitignorePath);
            _progressForm?.ReportFileGenerated(".gitignore", "تم توليد");
        }

        private async Task GenerateReadme(string projectPath)
        {
            UpdateStatusLabel("جاري توليد README.md...");
            var readmePath = Path.Combine(projectPath, "README.md");
            await _codeGenerationService.GenerateReadme(_context, readmePath);
            _progressForm?.ReportFileGenerated("README.md", "تم توليد");
        }

        private async Task GenerateInfrastructureLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Infrastructure...");
            await _codeGenerationService.GenerateInfrastructureLayer(_context, projectPath);
            _progressForm?.ReportFileGenerated("Infrastructure Layer", "تم توليد طبقة");
        }

        private async Task GenerateApplicationLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Application...");
            await _codeGenerationService.GenerateApplicationLayer(_context, projectPath);
            _progressForm?.ReportFileGenerated("Application Layer", "تم توليد طبقة");
        }

        private async Task GenerateDomainLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Domain...");
            await _codeGenerationService.GenerateDomainLayer(_context, projectPath);
            _progressForm?.ReportFileGenerated("Domain Layer", "تم توليد طبقة");
        }

        private async Task GeneratePresentationLayer(string projectPath)
        {
            UpdateStatusLabel("جاري توليد طبقة Presentation...");
            await _codeGenerationService.GeneratePresentationLayer(_context, projectPath);
            _progressForm?.ReportFileGenerated("Presentation Layer", "تم توليد طبقة");
        }

        private async Task GenerateSolutionFile(string projectPath)
        {
            UpdateStatusLabel("جاري توليد ملف الحل...");
            await _codeGenerationService.GenerateSolutionFile(_context, projectPath);
            _progressForm?.ReportFileGenerated("Solution File", "تم توليد");
        }

        private int CalculateExpectedFilesCount()
        {
            int count = 0;

            // حساب عدد الجداول المُولدة
            int tablesCount = chkGenerateAllTables.Checked
                ? _availableTables.Count
                : lstTables.SelectedItems.Count;

            // ملفات هيكل المشروع
            if (chkGenerateStartup.Checked) count++;
            if (chkGenerateProgram.Checked) count++;
            if (chkGenerateGitignore.Checked) count++;
            if (chkGenerateReadme.Checked) count++;
            if (chkGenerateSolution.Checked) count++;

            // ملفات الطبقات المعمارية (تقدير 2-3 ملفات لكل طبقة)
            if (chkInfrastructureLayer.Checked) count += 2;
            if (chkApplicationLayer.Checked) count += 2;
            if (chkDomainLayer.Checked) count += 2;
            if (chkPresentationLayer.Checked) count += 2;

            // ملفات الكود لكل جدول
            if (tablesCount > 0)
            {
                int filesPerTable = 0;
                if (chkEntities.Checked) filesPerTable++;
                if (chkDTOs.Checked) filesPerTable++;
                if (chkRepositories.Checked) filesPerTable++;
                if (chkServices.Checked) filesPerTable++;
                if (chkControllers.Checked) filesPerTable++;
                if (chkUnitTests.Checked) filesPerTable++;
                if (chkValidation.Checked) filesPerTable++;

                count += tablesCount * filesPerTable;
            }

            return Math.Max(count, 1); // على الأقل ملف واحد
        }

        private void FrmTabls_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
        }

        #endregion
    }
}