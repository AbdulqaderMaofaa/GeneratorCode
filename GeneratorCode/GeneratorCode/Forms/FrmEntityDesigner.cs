using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GeneratorCode.Core.DomainModel;
using GeneratorCode.Core.Interfaces;
using GeneratorCode.Core.Logging;
using GeneratorCode.Core.Models;
using GeneratorCode.Core.Services;
using GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    public partial class FrmEntityDesigner : Form
    {
        private readonly IDomainModelService _modelService;
        private readonly ILogger _logger;
        private DomainEntity _selectedEntity;
        private bool _isDirty;

        public FrmEntityDesigner()
        {
            _logger = LoggerFactory.Default;
            _modelService = new DomainModelService();

            InitializeComponent();
            ApplyTheme();
            SetupFormStyling();
            SetupGridColumns();
            SetupEvents();
            SetupDefaults();
            SetupTooltips();
            UpdateUI();
        }

        private void SetupTooltips()
        {
            var tip = AppTheme.CreateTooltipProvider();
            tip.SetToolTip(btnAddEntity, "إضافة كيان جديد");
            tip.SetToolTip(btnRemoveEntity, "حذف الكيان المحدد");
            tip.SetToolTip(btnRenameEntity, "إعادة تسمية الكيان المحدد");
            tip.SetToolTip(btnAddProperty, "إضافة خاصية جديدة للكيان");
            tip.SetToolTip(btnRemoveProperty, "حذف الخاصية المحددة");
            tip.SetToolTip(btnGenerate, "توليد الكود (Ctrl+G)");
            tip.SetToolTip(btnSaveModel, "حفظ النموذج (Ctrl+S)");
            tip.SetToolTip(btnLoadModel, "تحميل نموذج محفوظ");
            tip.SetToolTip(btnExportJson, "نسخ النموذج كـ JSON");
            tip.SetToolTip(btnImportJson, "استيراد نموذج من JSON");
            tip.SetToolTip(btnBack, "العودة (Escape)");
        }

        private void SetupFormStyling()
        {
            AppTheme.StyleForm(this);
            AppTheme.StyleTabControl(tabControl);
            lstEntities.RightToLeft = RightToLeft.Yes;
            gridProperties.RightToLeft = RightToLeft.Yes;
            gridRelations.RightToLeft = RightToLeft.Yes;
        }

        private void ApplyTheme()
        {
            AppTheme.StyleDataGridView(gridProperties);
            AppTheme.StyleDataGridView(gridRelations);
            AppTheme.StyleGroupBox(grpEntities, AppTheme.PrimaryDark);
            AppTheme.StyleGroupBox(grpActions, AppTheme.PrimaryDark);
            AppTheme.StyleButton(btnAddEntity, AppTheme.Success);
            AppTheme.StyleButton(btnRemoveEntity, AppTheme.Danger);
            AppTheme.StyleButton(btnRenameEntity, AppTheme.Warning);
            AppTheme.StyleButton(btnAddProperty, AppTheme.Success);
            AppTheme.StyleButton(btnRemoveProperty, AppTheme.Danger);
            AppTheme.StyleButton(btnAddRelation, AppTheme.Primary);
            AppTheme.StyleButton(btnRemoveRelation, AppTheme.Danger);
            AppTheme.StyleButton(btnGenerate, AppTheme.Success, large: true);
            AppTheme.StyleButton(btnSaveModel, AppTheme.Primary);
            AppTheme.StyleButton(btnLoadModel, AppTheme.Primary);
            AppTheme.StyleButton(btnExportJson, AppTheme.Purple);
            AppTheme.StyleButton(btnImportJson, AppTheme.Purple);
            AppTheme.StyleButton(btnBack, AppTheme.Gray);
            AppTheme.StyleRichTextBoxConsole(txtPreview);
        }

        private void SetupDefaults()
        {
            cmbDatabaseType.Items.AddRange(new object[] { "SQL Server", "MySQL", "PostgreSQL", "Oracle", "SQLite" });
            cmbDatabaseType.SelectedIndex = 0;

            cmbTargetFramework.Items.AddRange(new object[] { ".NET 6", ".NET 7", ".NET 8", ".NET 9" });
            cmbTargetFramework.SelectedIndex = 2;

            txtProjectName.Text = "MyProject";
            txtNamespace.Text = "MyProject";
        }

        private void SetupGridColumns()
        {
            gridProperties.Columns.Clear();

            gridProperties.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPropName", HeaderText = "الاسم",
                FillWeight = 22, MinimumWidth = 100
            });
            gridProperties.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colPropType", HeaderText = "النوع",
                FillWeight = 16, MinimumWidth = 90,
                DataSource = Enum.GetValues(typeof(DomainPropertyType)),
                FlatStyle = FlatStyle.Flat
            });
            gridProperties.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colRequired", HeaderText = "مطلوب",
                FillWeight = 8, MinimumWidth = 60
            });
            gridProperties.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colPK", HeaderText = "مفتاح",
                FillWeight = 8, MinimumWidth = 55
            });
            gridProperties.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colIdentity", HeaderText = "تلقائي",
                FillWeight = 8, MinimumWidth = 55
            });
            gridProperties.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colMaxLength", HeaderText = "الحد الأقصى",
                FillWeight = 12, MinimumWidth = 80
            });
            gridProperties.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDefault", HeaderText = "القيمة الافتراضية",
                FillWeight = 14, MinimumWidth = 90
            });
            gridProperties.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDescription", HeaderText = "الوصف",
                FillWeight = 16, MinimumWidth = 90
            });

            gridRelations.Columns.Clear();

            gridRelations.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colTargetEntity", HeaderText = "الكيان الهدف",
                FillWeight = 25, MinimumWidth = 120,
                FlatStyle = FlatStyle.Flat
            });
            gridRelations.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colRelType", HeaderText = "نوع العلاقة",
                FillWeight = 20, MinimumWidth = 110,
                DataSource = Enum.GetValues(typeof(RelationType)),
                FlatStyle = FlatStyle.Flat
            });
            gridRelations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFKName", HeaderText = "اسم المفتاح الأجنبي",
                FillWeight = 22, MinimumWidth = 120
            });
            gridRelations.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colDeleteBehavior", HeaderText = "سلوك الحذف",
                FillWeight = 18, MinimumWidth = 110,
                DataSource = Enum.GetValues(typeof(DeleteBehavior)),
                FlatStyle = FlatStyle.Flat
            });
            gridRelations.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNavProp", HeaderText = "خاصية التنقل",
                FillWeight = 20, MinimumWidth = 110
            });
        }

        private void SetupEvents()
        {
            KeyDown += FrmEntityDesigner_KeyDown;
            lstEntities.SelectedIndexChanged += LstEntities_SelectedIndexChanged;
            btnAddEntity.Click += BtnAddEntity_Click;
            btnRemoveEntity.Click += BtnRemoveEntity_Click;
            btnRenameEntity.Click += BtnRenameEntity_Click;

            btnAddProperty.Click += BtnAddProperty_Click;
            btnRemoveProperty.Click += BtnRemoveProperty_Click;
            btnAddRelation.Click += BtnAddRelation_Click;
            btnRemoveRelation.Click += BtnRemoveRelation_Click;

            gridProperties.CellValueChanged += GridProperties_CellValueChanged;
            gridProperties.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;
            gridRelations.CellValueChanged += GridRelations_CellValueChanged;
            gridRelations.CurrentCellDirtyStateChanged += Grid_CurrentCellDirtyStateChanged;

            btnSaveModel.Click += BtnSaveModel_Click;
            btnLoadModel.Click += BtnLoadModel_Click;
            btnExportJson.Click += BtnExportJson_Click;
            btnImportJson.Click += BtnImportJson_Click;
            btnGenerate.Click += BtnGenerate_Click;
            btnBack.Click += BtnBack_Click;

            tabControl.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl.SelectedIndex == 2)
                    RefreshPreview();
            };

            cmbDatabaseType.SelectedIndexChanged += (s, e) => RefreshPreview();
        }

        private void FrmEntityDesigner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (_isDirty)
                {
                    var result = MessageBox.Show("يوجد تغييرات غير محفوظة. هل تريد الخروج؟",
                        "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes) return;
                }
                Close();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                e.Handled = true;
                btnSaveModel.PerformClick();
            }
        }

        private void BtnAddEntity_Click(object sender, EventArgs e)
        {
            var name = PromptInput("اسم الكيان الجديد:", "إضافة كيان");
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                var entity = new DomainEntity
                {
                    Name = name,
                    TableName = name,
                    Properties = new List<DomainProperty>
                    {
                        new DomainProperty { Name = "Id", ColumnName = "Id", Type = DomainPropertyType.Int, IsPrimaryKey = true, IsIdentity = true, IsRequired = true, Order = 0 }
                    }
                };
                _modelService.AddEntity(entity);
                RefreshEntityList();
                lstEntities.SelectedItem = name;
                _isDirty = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void BtnRemoveEntity_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null) return;
            var result = MessageBox.Show($"هل تريد حذف الكيان '{_selectedEntity.Name}'؟", "تأكيد الحذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;

            try
            {
                _modelService.RemoveEntity(_selectedEntity.Name);
                _selectedEntity = null;
                RefreshEntityList();
                _isDirty = true;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnRenameEntity_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null) return;
            var newName = PromptInput("الاسم الجديد:", "إعادة تسمية", _selectedEntity.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName == _selectedEntity.Name) return;

            try
            {
                var updated = _selectedEntity;
                var oldName = updated.Name;
                updated.Name = newName;
                updated.TableName = newName;
                _modelService.UpdateEntity(oldName, updated);
                RefreshEntityList();
                lstEntities.SelectedItem = newName;
                _isDirty = true;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnAddProperty_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null) { ShowError("اختر كياناً أولاً"); return; }
            var row = gridProperties.Rows[gridProperties.Rows.Add()];
            row.Cells["colPropName"].Value = "NewProperty";
            row.Cells["colPropType"].Value = DomainPropertyType.String;
            row.Cells["colRequired"].Value = false;
            row.Cells["colPK"].Value = false;
            row.Cells["colIdentity"].Value = false;
            SavePropertiesFromGrid();
        }

        private void BtnRemoveProperty_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null || gridProperties.CurrentRow == null) return;
            if (gridProperties.CurrentRow.IsNewRow) return;
            gridProperties.Rows.RemoveAt(gridProperties.CurrentRow.Index);
            SavePropertiesFromGrid();
        }

        private void BtnAddRelation_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null) { ShowError("اختر كياناً أولاً"); return; }
            var row = gridRelations.Rows[gridRelations.Rows.Add()];
            row.Cells["colRelType"].Value = RelationType.OneToMany;
            row.Cells["colDeleteBehavior"].Value = DeleteBehavior.Cascade;
            SaveRelationsFromGrid();
        }

        private void BtnRemoveRelation_Click(object sender, EventArgs e)
        {
            if (_selectedEntity == null || gridRelations.CurrentRow == null) return;
            if (gridRelations.CurrentRow.IsNewRow) return;
            gridRelations.Rows.RemoveAt(gridRelations.CurrentRow.Index);
            SaveRelationsFromGrid();
        }

        private void Grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid?.IsCurrentCellDirty == true)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void GridProperties_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _selectedEntity == null) return;
            SavePropertiesFromGrid();
        }

        private void GridRelations_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || _selectedEntity == null) return;
            SaveRelationsFromGrid();
        }

        private void LstEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEntities.SelectedItem == null) return;
            var entityName = lstEntities.SelectedItem.ToString();
            _selectedEntity = _modelService.CurrentModel.Entities
                .FirstOrDefault(en => en.Name == entityName);

            if (_selectedEntity != null)
            {
                LoadEntityToGrid(_selectedEntity);
                UpdateRelationTargetDropdown();
            }
            UpdateUI();
        }

        private void LoadEntityToGrid(DomainEntity entity)
        {
            gridProperties.Rows.Clear();
            foreach (var prop in entity.Properties.OrderBy(p => p.Order))
            {
                var idx = gridProperties.Rows.Add();
                var row = gridProperties.Rows[idx];
                row.Cells["colPropName"].Value = prop.Name;
                row.Cells["colPropType"].Value = prop.Type;
                row.Cells["colRequired"].Value = prop.IsRequired;
                row.Cells["colPK"].Value = prop.IsPrimaryKey;
                row.Cells["colIdentity"].Value = prop.IsIdentity;
                row.Cells["colMaxLength"].Value = prop.MaxLength?.ToString() ?? "";
                row.Cells["colDefault"].Value = prop.DefaultValue ?? "";
                row.Cells["colDescription"].Value = prop.Description ?? "";
            }

            gridRelations.Rows.Clear();
            if (entity.Relations != null)
            {
                foreach (var rel in entity.Relations)
                {
                    var idx = gridRelations.Rows.Add();
                    var row = gridRelations.Rows[idx];
                    row.Cells["colTargetEntity"].Value = rel.TargetEntity;
                    row.Cells["colRelType"].Value = rel.RelationType;
                    row.Cells["colFKName"].Value = rel.ForeignKeyName ?? "";
                    row.Cells["colDeleteBehavior"].Value = rel.DeleteBehavior;
                    row.Cells["colNavProp"].Value = rel.NavigationPropertyName ?? "";
                }
            }

            if (tabControl.SelectedIndex == 2)
                RefreshPreview();
        }

        private void SavePropertiesFromGrid()
        {
            if (_selectedEntity == null) return;
            _selectedEntity.Properties.Clear();

            for (int i = 0; i < gridProperties.Rows.Count; i++)
            {
                var row = gridProperties.Rows[i];
                if (row.IsNewRow) continue;
                var name = row.Cells["colPropName"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(name)) continue;

                var prop = new DomainProperty
                {
                    Name = name,
                    ColumnName = name,
                    Type = row.Cells["colPropType"].Value is DomainPropertyType t ? t : DomainPropertyType.String,
                    IsRequired = row.Cells["colRequired"].Value is true,
                    IsPrimaryKey = row.Cells["colPK"].Value is true,
                    IsIdentity = row.Cells["colIdentity"].Value is true,
                    DefaultValue = row.Cells["colDefault"].Value?.ToString(),
                    Description = row.Cells["colDescription"].Value?.ToString(),
                    Order = i
                };

                if (int.TryParse(row.Cells["colMaxLength"].Value?.ToString(), out int maxLen))
                    prop.MaxLength = maxLen;

                _selectedEntity.Properties.Add(prop);
            }
            _isDirty = true;
        }

        private void SaveRelationsFromGrid()
        {
            if (_selectedEntity == null) return;
            _selectedEntity.Relations.Clear();

            for (int i = 0; i < gridRelations.Rows.Count; i++)
            {
                var row = gridRelations.Rows[i];
                if (row.IsNewRow) continue;
                var target = row.Cells["colTargetEntity"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(target)) continue;

                _selectedEntity.Relations.Add(new DomainRelation
                {
                    SourceEntity = _selectedEntity.Name,
                    TargetEntity = target,
                    RelationType = row.Cells["colRelType"].Value is RelationType rt ? rt : RelationType.OneToMany,
                    ForeignKeyName = row.Cells["colFKName"].Value?.ToString() ?? "",
                    DeleteBehavior = row.Cells["colDeleteBehavior"].Value is DeleteBehavior db ? db : DeleteBehavior.Cascade,
                    NavigationPropertyName = row.Cells["colNavProp"].Value?.ToString() ?? ""
                });
            }
            _isDirty = true;
        }

        private void UpdateRelationTargetDropdown()
        {
            var col = gridRelations.Columns["colTargetEntity"] as DataGridViewComboBoxColumn;
            if (col == null) return;
            var names = _modelService.CurrentModel.Entities
                .Where(en => en.Name != _selectedEntity?.Name)
                .Select(en => en.Name).ToList();
            col.DataSource = names;
        }

        private void RefreshEntityList()
        {
            lstEntities.Items.Clear();
            foreach (var entity in _modelService.CurrentModel.Entities)
            {
                lstEntities.Items.Add(entity.Name);
            }
            UpdateRelationTargetDropdown();
            UpdateUI();
        }

        private void RefreshPreview()
        {
            if (_selectedEntity == null) { txtPreview.Text = ""; return; }

            var dbType = GetSelectedDatabaseType();
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine();
            sb.AppendLine($"namespace {txtNamespace.Text}.Domain.Entities");
            sb.AppendLine("{");
            if (!string.IsNullOrWhiteSpace(_selectedEntity.Description))
                sb.AppendLine($"    /// <summary>{_selectedEntity.Description}</summary>");
            sb.AppendLine($"    [Table(\"{_selectedEntity.TableName}\")]");
            sb.AppendLine($"    public class {_selectedEntity.Name}");
            sb.AppendLine("    {");

            foreach (var prop in _selectedEntity.Properties.OrderBy(p => p.Order))
            {
                var attrs = new List<string>();
                if (prop.IsPrimaryKey) attrs.Add("[Key]");
                if (prop.IsRequired && prop.Type == DomainPropertyType.String) attrs.Add("[Required]");
                if (prop.MaxLength.HasValue) attrs.Add($"[MaxLength({prop.MaxLength})]");

                foreach (var attr in attrs)
                    sb.AppendLine($"        {attr}");

                var csharpType = MapToCSharpType(prop.Type, !prop.IsRequired && IsValueType(prop.Type));
                sb.AppendLine($"        public {csharpType} {prop.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            if (_selectedEntity.Relations != null)
            {
                foreach (var rel in _selectedEntity.Relations)
                {
                    var navName = string.IsNullOrWhiteSpace(rel.NavigationPropertyName)
                        ? rel.TargetEntity + (rel.RelationType == RelationType.OneToMany ? "s" : "")
                        : rel.NavigationPropertyName;

                    if (rel.RelationType == RelationType.OneToMany)
                        sb.AppendLine($"        public virtual ICollection<{rel.TargetEntity}> {navName} {{ get; set; }}");
                    else if (rel.RelationType == RelationType.ManyToMany)
                        sb.AppendLine($"        public virtual ICollection<{rel.TargetEntity}> {navName} {{ get; set; }}");
                    else
                        sb.AppendLine($"        public virtual {rel.TargetEntity} {navName} {{ get; set; }}");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            txtPreview.Text = sb.ToString();
        }

        private async void BtnSaveModel_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = $"{_modelService.CurrentModel.ProjectName ?? "domain-model"}.json"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                ApplyModelSettings();
                await _modelService.SaveModelAsync(dlg.FileName);
                _isDirty = false;
                ShowStatus("تم حفظ النموذج بنجاح", AppTheme.Success);
            }
            catch (Exception ex) { ShowError(ex.Message); }
            finally { Cursor = Cursors.Default; }
        }

        private async void BtnLoadModel_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                await _modelService.LoadModelAsync(dlg.FileName);
                ApplyModelToUI();
                RefreshEntityList();
                _isDirty = false;
                ShowStatus("تم تحميل النموذج بنجاح", AppTheme.Success);
            }
            catch (Exception ex) { ShowError(ex.Message); }
            finally { Cursor = Cursors.Default; }
        }

        private void BtnExportJson_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyModelSettings();
                var json = _modelService.ExportToJson();
                Clipboard.SetText(json);
                ShowStatus("تم نسخ JSON إلى الحافظة", AppTheme.Primary);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnImportJson_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "JSON Files (*.json)|*.json" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var json = System.IO.File.ReadAllText(dlg.FileName);
                _modelService.ImportFromJson(json);
                ApplyModelToUI();
                RefreshEntityList();
                ShowStatus("تم استيراد النموذج بنجاح", AppTheme.Success);
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            ApplyModelSettings();
            var validation = _modelService.ValidateModel();
            if (!validation.IsValid)
            {
                ShowError("أخطاء في النموذج:\n" + string.Join("\n", validation.Errors));
                return;
            }

            using var dlg = new FolderBrowserDialog { Description = "اختر مجلد الحفظ" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            _modelService.CurrentModel.OutputPath = dlg.SelectedPath;

            try
            {
                Cursor = Cursors.WaitCursor;
                var migrationForm = new FrmMigrationManager(_modelService, dlg.SelectedPath);
                migrationForm.ShowDialog(this);
            }
            finally { Cursor = Cursors.Default; }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            if (_isDirty)
            {
                var result = MessageBox.Show("يوجد تغييرات غير محفوظة. هل تريد الخروج؟",
                    "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;
            }
            Close();
        }

        private void ApplyModelSettings()
        {
            _modelService.CurrentModel.ProjectName = txtProjectName.Text;
            _modelService.CurrentModel.DefaultNamespace = txtNamespace.Text;
            _modelService.CurrentModel.TargetDatabaseType = GetSelectedDatabaseType();
            _modelService.CurrentModel.TargetFramework = GetSelectedFramework();
        }

        private void ApplyModelToUI()
        {
            txtProjectName.Text = _modelService.CurrentModel.ProjectName;
            txtNamespace.Text = _modelService.CurrentModel.DefaultNamespace;

            var dbIdx = (int)_modelService.CurrentModel.TargetDatabaseType;
            if (dbIdx >= 0 && dbIdx < cmbDatabaseType.Items.Count)
                cmbDatabaseType.SelectedIndex = dbIdx;

            var fw = _modelService.CurrentModel.TargetFramework;
            for (int i = 0; i < cmbTargetFramework.Items.Count; i++)
            {
                if (cmbTargetFramework.Items[i].ToString().Contains(fw.Replace("net", "").Replace(".0", "")))
                {
                    cmbTargetFramework.SelectedIndex = i;
                    break;
                }
            }
        }

        private void UpdateUI()
        {
            var hasEntity = _selectedEntity != null;
            var isEmpty = lstEntities.Items.Count == 0;
            lblEmptyState.Visible = isEmpty;
            btnRemoveEntity.Enabled = hasEntity;
            btnRenameEntity.Enabled = hasEntity;
            btnAddProperty.Enabled = hasEntity;
            btnRemoveProperty.Enabled = hasEntity;
            btnAddRelation.Enabled = hasEntity;
            btnRemoveRelation.Enabled = hasEntity;
            tabControl.Enabled = hasEntity;
        }

        private DatabaseType GetSelectedDatabaseType()
        {
            return cmbDatabaseType.SelectedIndex switch
            {
                0 => DatabaseType.SqlServer,
                1 => DatabaseType.MySql,
                2 => DatabaseType.PostgreSql,
                3 => DatabaseType.Oracle,
                4 => DatabaseType.SQLite,
                _ => DatabaseType.SqlServer
            };
        }

        private string GetSelectedFramework()
        {
            return cmbTargetFramework.SelectedIndex switch
            {
                0 => "net6.0",
                1 => "net7.0",
                2 => "net8.0",
                3 => "net9.0",
                _ => "net8.0"
            };
        }

        private static string MapToCSharpType(DomainPropertyType type, bool nullable)
        {
            var baseType = type switch
            {
                DomainPropertyType.Int => "int",
                DomainPropertyType.Long => "long",
                DomainPropertyType.Short => "short",
                DomainPropertyType.Byte => "byte",
                DomainPropertyType.Bool => "bool",
                DomainPropertyType.String => "string",
                DomainPropertyType.Decimal => "decimal",
                DomainPropertyType.Double => "double",
                DomainPropertyType.Float => "float",
                DomainPropertyType.DateTime => "DateTime",
                DomainPropertyType.DateOnly => "DateOnly",
                DomainPropertyType.TimeOnly => "TimeOnly",
                DomainPropertyType.Guid => "Guid",
                DomainPropertyType.ByteArray => "byte[]",
                _ => "string"
            };
            return nullable ? baseType + "?" : baseType;
        }

        private static bool IsValueType(DomainPropertyType type)
        {
            return type != DomainPropertyType.String && type != DomainPropertyType.ByteArray;
        }

        private void ShowStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string PromptInput(string prompt, string title, string defaultValue = "")
        {
            using var form = new Form
            {
                Text = title,
                Width = 400,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = AppTheme.FormBackground,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true,
                Font = new Font("Segoe UI", 10F)
            };

            var lbl = new Label { Text = prompt, Left = 20, Top = 15, Width = 340, Font = new Font("Segoe UI", 10F) };
            var txt = new TextBox { Text = defaultValue, Left = 20, Top = 45, Width = 340, Font = new Font("Segoe UI", 10F) };
            var btnOk = new Button
            {
                Text = "موافق",
                Left = 200,
                Top = 85,
                Width = 80,
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.Primary,
                ForeColor = Color.White
            };
            var btnCancel = new Button
            {
                Text = "إلغاء",
                Left = 280,
                Top = 85,
                Width = 80,
                DialogResult = DialogResult.Cancel,
                FlatStyle = FlatStyle.Flat,
                BackColor = AppTheme.GridLine,
                ForeColor = Color.Black
            };

            form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            return form.ShowDialog(this) == DialogResult.OK ? txt.Text.Trim() : null;
        }
    }
}
