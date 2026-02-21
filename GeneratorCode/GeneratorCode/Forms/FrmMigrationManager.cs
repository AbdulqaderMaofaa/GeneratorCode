using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Logging;
using GeneratorCode.Core.Services;
using GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    public partial class FrmMigrationManager : Form
    {
        private readonly IDomainModelService _modelService;
        private readonly IMigrationService _migrationService;
        private readonly ILogger _logger;
        private readonly string _projectPath;
        private CancellationTokenSource _cts;

        public FrmMigrationManager(IDomainModelService modelService, string projectPath)
        {
            _logger = LoggerFactory.Default;
            _modelService = modelService;
            _projectPath = projectPath;
            _migrationService = new EfCoreMigrationService(_logger);

            InitializeComponent();
            ApplyTheme();
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
                if (e.KeyCode == Keys.F5) BtnRefresh_Click(s, e);
            };
            SetupEvents();
            LoadProjectInfo();
        }

        private void ApplyTheme()
        {
            AppTheme.StyleForm(this);
            AppTheme.StyleDataGridView(gridMigrations);
            AppTheme.StyleGroupBox(grpMigrations, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpOutput, AppTheme.PrimaryDark);
            AppTheme.StyleButton(btnGenerateCode, AppTheme.Primary, large: true);
            AppTheme.StyleButton(btnAddMigration, AppTheme.Success);
            AppTheme.StyleButton(btnUpdateDatabase, AppTheme.Primary);
            AppTheme.StyleButton(btnRollback, AppTheme.Warning);
            AppTheme.StyleButton(btnGenerateScript, AppTheme.Purple);
            AppTheme.StyleButton(btnRemoveLast, AppTheme.Danger);
            AppTheme.StyleButton(btnRefresh, AppTheme.GrayDark);
            AppTheme.StyleButton(btnClose, AppTheme.Gray);
            AppTheme.StyleButton(btnCancel, AppTheme.Danger);
        }

        private void SetupEvents()
        {
            btnAddMigration.Click += BtnAddMigration_Click;
            btnUpdateDatabase.Click += BtnUpdateDatabase_Click;
            btnRollback.Click += BtnRollback_Click;
            btnGenerateScript.Click += BtnGenerateScript_Click;
            btnRemoveLast.Click += BtnRemoveLast_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnGenerateCode.Click += BtnGenerateCode_Click;
            btnClose.Click += (s, e) => Close();
            btnCancel.Click += BtnCancel_Click;
        }

        private void LoadProjectInfo()
        {
            var model = _modelService.CurrentModel;
            lblProjectName.Text = $"المشروع: {model.ProjectName}";
            lblDatabaseInfo.Text = $"قاعدة البيانات: {model.TargetDatabaseType}";
            lblOutputPath.Text = $"المسار: {_projectPath}";
            txtMigrationName.Text = "InitialCreate";
        }

        private async void BtnGenerateCode_Click(object sender, EventArgs e)
        {
            try
            {
                SetBusy(true, "جاري توليد الكود...");
                AppendOutput("=== بدء توليد كود EF Core ===", AppTheme.LogInfo);

                var generator = new Core.CodeFirst.EfCoreCodeFirstGenerator(_logger);
                var result = await generator.GenerateFullProjectAsync(
                    _modelService.CurrentModel,
                    new Core.Models.CodeGenerationContext
                    {
                        OutputPath = _projectPath,
                        Namespace = _modelService.CurrentModel.DefaultNamespace,
                        TargetFramework = _modelService.CurrentModel.TargetFramework,
                        DatabaseType = _modelService.CurrentModel.TargetDatabaseType
                    });

                if (result.Success)
                {
                    AppendOutput($"تم توليد {result.GeneratedFiles?.Count ?? 0} ملف بنجاح", AppTheme.Success);
                    foreach (var file in result.GeneratedFiles ?? new System.Collections.Generic.List<Core.Models.GeneratedFile>())
                        AppendOutput($"  > {file.RelativePath}", Color.LightGray);
                }
                else
                {
                    AppendOutput($"فشل التوليد: {result.Message}", AppTheme.Danger);
                }
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void BtnAddMigration_Click(object sender, EventArgs e)
        {
            var migName = txtMigrationName.Text.Trim();
            if (string.IsNullOrWhiteSpace(migName))
            {
                MessageBox.Show("يرجى إدخال اسم الـ Migration", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _cts = new CancellationTokenSource();
                SetBusy(true, "جاري إنشاء Migration...");
                AppendOutput($"=== إنشاء Migration: {migName} ===", AppTheme.LogInfo);

                var progress = new Progress<string>(msg => AppendOutput(msg, Color.LightGray));
                var result = await _migrationService.AddMigrationAsync(migName, _projectPath, progress, _cts.Token);

                HandleResult(result, "تم إنشاء Migration بنجاح");
            }
            catch (OperationCanceledException)
            {
                AppendOutput("تم إلغاء العملية", Color.Yellow);
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private async void BtnUpdateDatabase_Click(object sender, EventArgs e)
        {
            var connStr = _modelService.CurrentModel.ConnectionString;
            if (string.IsNullOrWhiteSpace(connStr))
            {
                MessageBox.Show("لم يتم تحديد سلسلة الاتصال. يرجى التحقق من إعدادات المشروع.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _cts = new CancellationTokenSource();
                SetBusy(true, "جاري تحديث قاعدة البيانات...");
                AppendOutput("=== تحديث قاعدة البيانات ===", AppTheme.LogInfo);

                var progress = new Progress<string>(msg => AppendOutput(msg, Color.LightGray));
                var result = await _migrationService.UpdateDatabaseAsync(_projectPath, connStr, progress, _cts.Token);

                HandleResult(result, "تم تحديث قاعدة البيانات بنجاح");
            }
            catch (OperationCanceledException)
            {
                AppendOutput("تم إلغاء العملية", Color.Yellow);
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private async void BtnRollback_Click(object sender, EventArgs e)
        {
            if (gridMigrations.CurrentRow == null) { MessageBox.Show("اختر migration للتراجع إليه"); return; }
            var migName = gridMigrations.CurrentRow.Cells[0].Value?.ToString();
            if (string.IsNullOrWhiteSpace(migName)) return;

            try
            {
                _cts = new CancellationTokenSource();
                SetBusy(true, $"جاري التراجع إلى {migName}...");
                AppendOutput($"=== التراجع إلى: {migName} ===", AppTheme.LogInfo);

                var progress = new Progress<string>(msg => AppendOutput(msg, Color.LightGray));
                var result = await _migrationService.RollbackAsync(migName, _projectPath, progress, _cts.Token);

                HandleResult(result, $"تم التراجع إلى {migName} بنجاح");
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private async void BtnGenerateScript_Click(object sender, EventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();
                SetBusy(true, "جاري توليد SQL Script...");
                AppendOutput("=== توليد SQL Script ===", AppTheme.LogInfo);

                var progress = new Progress<string>(msg => AppendOutput(msg, Color.LightGray));
                var result = await _migrationService.GenerateScriptAsync(null, null, _projectPath, progress, _cts.Token);

                if (result.Success && !string.IsNullOrWhiteSpace(result.ScriptContent))
                {
                    using var dlg = new SaveFileDialog { Filter = "SQL Files (*.sql)|*.sql", FileName = "migration.sql" };
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(dlg.FileName, result.ScriptContent);
                        AppendOutput($"تم حفظ السكريبت: {dlg.FileName}", AppTheme.Success);
                    }
                }
                else
                {
                    HandleResult(result, "تم توليد السكريبت");
                }
            }
            catch (Exception ex)
            {
                    AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private async void BtnRemoveLast_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("هل تريد حذف آخر Migration؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _cts = new CancellationTokenSource();
                SetBusy(true, "جاري حذف آخر Migration...");
                AppendOutput("=== حذف آخر Migration ===", AppTheme.LogInfo);

                var progress = new Progress<string>(msg => AppendOutput(msg, Color.LightGray));
                var result = await _migrationService.RemoveLastMigrationAsync(_projectPath, progress, _cts.Token);

                HandleResult(result, "تم حذف آخر Migration بنجاح");
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private async void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetBusy(true, "جاري تحميل قائمة Migrations...");
                var migrations = await _migrationService.ListMigrationsAsync(_projectPath);
                gridMigrations.Rows.Clear();
                foreach (var mig in migrations)
                {
                    gridMigrations.Rows.Add(mig.Name, mig.Id, mig.AppliedDate?.ToString("yyyy-MM-dd HH:mm") ?? "-",
                        mig.IsApplied ? "مطبق" : "غير مطبق");
                }
            }
            catch (Exception ex)
            {
                AppendOutput($"خطأ في تحميل القائمة: {ex.Message}", AppTheme.Danger);
            }
            finally { SetBusy(false); }
        }

        private void HandleResult(Core.Models.MigrationResult result, string successMsg)
        {
            if (result.Success)
            {
                AppendOutput(successMsg, AppTheme.Success);
                if (!string.IsNullOrWhiteSpace(result.Output))
                    AppendOutput(result.Output, Color.LightGray);
                _ = RefreshMigrationsAsync();
            }
            else
            {
                AppendOutput("فشلت العملية", AppTheme.Danger);
                if (!string.IsNullOrWhiteSpace(result.ErrorOutput))
                    AppendOutput(result.ErrorOutput, AppTheme.Danger);
                if (!string.IsNullOrWhiteSpace(result.Output))
                    AppendOutput(result.Output, Color.LightGray);
            }
        }

        private void AppendOutput(string text, Color color)
        {
            if (InvokeRequired) { Invoke(new Action(() => AppendOutput(text, color))); return; }
            txtOutput.SelectionStart = txtOutput.TextLength;
            txtOutput.SelectionColor = color;
            txtOutput.AppendText(text + Environment.NewLine);
            txtOutput.ScrollToCaret();
        }

        private void SetBusy(bool busy, string message = "")
        {
            if (InvokeRequired) { Invoke(new Action(() => SetBusy(busy, message))); return; }
            progressBar.Visible = busy;
            progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            lblStatusBar.Text = busy ? message : "جاهز";
            btnCancel.Visible = busy;
            btnCancel.Enabled = busy;
            btnAddMigration.Enabled = !busy;
            btnUpdateDatabase.Enabled = !busy;
            btnRollback.Enabled = !busy;
            btnGenerateScript.Enabled = !busy;
            btnRemoveLast.Enabled = !busy;
            btnGenerateCode.Enabled = !busy;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
            btnCancel.Enabled = false;
        }

        private async Task RefreshMigrationsAsync()
        {
            try
            {
                var migrations = await _migrationService.ListMigrationsAsync(_projectPath);
                gridMigrations.Rows.Clear();
                foreach (var mig in migrations)
                    gridMigrations.Rows.Add(mig.Name, mig.Id, mig.AppliedDate?.ToString("yyyy-MM-dd HH:mm") ?? "-", mig.IsApplied ? "مطبق" : "غير مطبق");
            }
            catch { }
        }
    }
}
