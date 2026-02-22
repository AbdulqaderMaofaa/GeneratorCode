using Helpers = GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    partial class FrmEntityDesigner
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelTop = new System.Windows.Forms.Panel();
            lblDatabaseType = new System.Windows.Forms.Label();
            cmbDatabaseType = new System.Windows.Forms.ComboBox();
            lblFramework = new System.Windows.Forms.Label();
            cmbTargetFramework = new System.Windows.Forms.ComboBox();
            lblProjectName = new System.Windows.Forms.Label();
            txtProjectName = new System.Windows.Forms.TextBox();
            lblNamespaceLabel = new System.Windows.Forms.Label();
            txtNamespace = new System.Windows.Forms.TextBox();

            grpEntities = new System.Windows.Forms.GroupBox();
            lstEntities = new System.Windows.Forms.ListBox();
            btnAddEntity = new System.Windows.Forms.Button();
            btnRemoveEntity = new System.Windows.Forms.Button();
            btnRenameEntity = new System.Windows.Forms.Button();

            tabControl = new System.Windows.Forms.TabControl();
            tabProperties = new System.Windows.Forms.TabPage();
            tabRelations = new System.Windows.Forms.TabPage();
            tabPreview = new System.Windows.Forms.TabPage();

            gridProperties = new System.Windows.Forms.DataGridView();
            btnAddProperty = new System.Windows.Forms.Button();
            btnRemoveProperty = new System.Windows.Forms.Button();

            gridRelations = new System.Windows.Forms.DataGridView();
            btnAddRelation = new System.Windows.Forms.Button();
            btnRemoveRelation = new System.Windows.Forms.Button();

            txtPreview = new System.Windows.Forms.RichTextBox();

            grpActions = new System.Windows.Forms.GroupBox();
            btnGenerate = new System.Windows.Forms.Button();
            btnSaveModel = new System.Windows.Forms.Button();
            btnLoadModel = new System.Windows.Forms.Button();
            btnExportJson = new System.Windows.Forms.Button();
            btnImportJson = new System.Windows.Forms.Button();
            btnBack = new System.Windows.Forms.Button();
            lblStatus = new System.Windows.Forms.Label();

            panelTop.SuspendLayout();
            grpEntities.SuspendLayout();
            tabControl.SuspendLayout();
            tabProperties.SuspendLayout();
            tabRelations.SuspendLayout();
            tabPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridRelations).BeginInit();
            grpActions.SuspendLayout();
            SuspendLayout();

            // ================================================================
            // panelTop
            // ================================================================
            panelTop.BackColor = Helpers.AppTheme.PrimaryDark;
            panelTop.Controls.Add(lblDatabaseType);
            panelTop.Controls.Add(cmbDatabaseType);
            panelTop.Controls.Add(lblFramework);
            panelTop.Controls.Add(cmbTargetFramework);
            panelTop.Controls.Add(lblProjectName);
            panelTop.Controls.Add(txtProjectName);
            panelTop.Controls.Add(lblNamespaceLabel);
            panelTop.Controls.Add(txtNamespace);
            panelTop.Location = new System.Drawing.Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new System.Drawing.Size(1200, 95);
            panelTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // Row 1
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Font = Helpers.AppTheme.DefaultFontBold;
            lblDatabaseType.ForeColor = Helpers.AppTheme.TextOnDark;
            lblDatabaseType.Location = new System.Drawing.Point(1060, 15);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Text = "قاعدة البيانات:";

            cmbDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbDatabaseType.Font = Helpers.AppTheme.DefaultFont;
            cmbDatabaseType.Location = new System.Drawing.Point(880, 12);
            cmbDatabaseType.Name = "cmbDatabaseType";
            cmbDatabaseType.Size = new System.Drawing.Size(170, 25);

            lblFramework.AutoSize = true;
            lblFramework.Font = Helpers.AppTheme.DefaultFontBold;
            lblFramework.ForeColor = Helpers.AppTheme.TextOnDark;
            lblFramework.Location = new System.Drawing.Point(780, 15);
            lblFramework.Name = "lblFramework";
            lblFramework.Text = "إصدار .NET:";

            cmbTargetFramework.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbTargetFramework.Font = Helpers.AppTheme.DefaultFont;
            cmbTargetFramework.Location = new System.Drawing.Point(630, 12);
            cmbTargetFramework.Name = "cmbTargetFramework";
            cmbTargetFramework.Size = new System.Drawing.Size(140, 25);

            // Row 2
            lblProjectName.AutoSize = true;
            lblProjectName.Font = Helpers.AppTheme.DefaultFontBold;
            lblProjectName.ForeColor = Helpers.AppTheme.TextOnDark;
            lblProjectName.Location = new System.Drawing.Point(1060, 55);
            lblProjectName.Name = "lblProjectName";
            lblProjectName.Text = "اسم المشروع:";

            txtProjectName.Font = Helpers.AppTheme.DefaultFont;
            txtProjectName.Location = new System.Drawing.Point(840, 52);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.Size = new System.Drawing.Size(210, 25);

            lblNamespaceLabel.AutoSize = true;
            lblNamespaceLabel.Font = Helpers.AppTheme.DefaultFontBold;
            lblNamespaceLabel.ForeColor = Helpers.AppTheme.TextOnDark;
            lblNamespaceLabel.Location = new System.Drawing.Point(720, 55);
            lblNamespaceLabel.Name = "lblNamespaceLabel";
            lblNamespaceLabel.Text = "Namespace:";

            txtNamespace.Font = Helpers.AppTheme.DefaultFont;
            txtNamespace.Location = new System.Drawing.Point(480, 52);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(230, 25);

            // ================================================================
            // grpEntities
            // ================================================================
            grpEntities.Controls.Add(lblEmptyState);
            grpEntities.Controls.Add(lstEntities);
            grpEntities.Controls.Add(btnAddEntity);
            grpEntities.Controls.Add(btnRemoveEntity);
            grpEntities.Controls.Add(btnRenameEntity);
            grpEntities.Font = Helpers.AppTheme.DefaultFontBold;
            grpEntities.Location = new System.Drawing.Point(900, 100);
            grpEntities.Name = "grpEntities";
            grpEntities.Padding = new System.Windows.Forms.Padding(10);
            grpEntities.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpEntities.Size = new System.Drawing.Size(288, 570);
            grpEntities.TabIndex = 1;
            grpEntities.TabStop = false;
            grpEntities.Text = "الكيانات";

            lstEntities.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lstEntities.Font = Helpers.AppTheme.DefaultFont;
            lstEntities.ItemHeight = 30;
            lstEntities.Location = new System.Drawing.Point(13, 35);
            lstEntities.Name = "lstEntities";
            lstEntities.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lstEntities.Size = new System.Drawing.Size(262, 480);
            lstEntities.TabIndex = 0;

            // Entity buttons
            btnAddEntity.BackColor = Helpers.AppTheme.Success;
            btnAddEntity.Font = Helpers.AppTheme.DefaultFontBold;
            btnAddEntity.Location = new System.Drawing.Point(188, 522);
            btnAddEntity.Name = "btnAddEntity";
            btnAddEntity.Size = new System.Drawing.Size(85, 38);
            btnAddEntity.TabIndex = 1;
            btnAddEntity.Text = "إضافة";

            btnRemoveEntity.BackColor = Helpers.AppTheme.Danger;
            btnRemoveEntity.Font = Helpers.AppTheme.DefaultFontBold;
            btnRemoveEntity.Location = new System.Drawing.Point(98, 522);
            btnRemoveEntity.Name = "btnRemoveEntity";
            btnRemoveEntity.Size = new System.Drawing.Size(85, 38);
            btnRemoveEntity.TabIndex = 2;
            btnRemoveEntity.Text = "حذف";

            btnRenameEntity.BackColor = Helpers.AppTheme.Warning;
            btnRenameEntity.Font = Helpers.AppTheme.DefaultFontBold;
            btnRenameEntity.Location = new System.Drawing.Point(8, 522);
            btnRenameEntity.Name = "btnRenameEntity";
            btnRenameEntity.Size = new System.Drawing.Size(85, 38);
            btnRenameEntity.TabIndex = 3;
            btnRenameEntity.Text = "تسمية";

            // ================================================================
            // tabControl
            // ================================================================
            tabControl.Controls.Add(tabProperties);
            tabControl.Controls.Add(tabRelations);
            tabControl.Controls.Add(tabPreview);
            tabControl.Font = Helpers.AppTheme.DefaultFont;
            tabControl.Location = new System.Drawing.Point(12, 100);
            tabControl.Name = "tabControl";
            tabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabControl.RightToLeftLayout = true;
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(882, 570);
            tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            tabControl.ItemSize = new System.Drawing.Size(200, 35);
            tabControl.TabIndex = 2;

            // ================================================================
            // tabProperties
            // ================================================================
            tabProperties.BackColor = Helpers.AppTheme.SurfaceWhite;
            tabProperties.Controls.Add(gridProperties);
            tabProperties.Controls.Add(btnAddProperty);
            tabProperties.Controls.Add(btnRemoveProperty);
            tabProperties.Location = new System.Drawing.Point(4, 39);
            tabProperties.Name = "tabProperties";
            tabProperties.Padding = new System.Windows.Forms.Padding(10);
            tabProperties.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabProperties.Size = new System.Drawing.Size(874, 527);
            tabProperties.TabIndex = 0;
            tabProperties.Text = "الخصائص";

            // gridProperties
            gridProperties.AllowUserToAddRows = false;
            gridProperties.AllowUserToDeleteRows = false;
            gridProperties.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            gridProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            gridProperties.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            gridProperties.ColumnHeadersDefaultCellStyle.BackColor = Helpers.AppTheme.Primary;
            gridProperties.ColumnHeadersDefaultCellStyle.Font = Helpers.AppTheme.DefaultFontBold;
            gridProperties.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            gridProperties.ColumnHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(5);
            gridProperties.ColumnHeadersHeight = 42;
            gridProperties.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridProperties.DefaultCellStyle.Font = Helpers.AppTheme.DefaultFontSmall;
            gridProperties.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(4);
            gridProperties.DefaultCellStyle.SelectionBackColor = Helpers.AppTheme.Primary;
            gridProperties.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            gridProperties.AlternatingRowsDefaultCellStyle.BackColor = Helpers.AppTheme.AlternateRow;
            gridProperties.EnableHeadersVisualStyles = false;
            gridProperties.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            gridProperties.GridColor = Helpers.AppTheme.GridLine;
            gridProperties.Location = new System.Drawing.Point(13, 13);
            gridProperties.Name = "gridProperties";
            gridProperties.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            gridProperties.RowHeadersVisible = false;
            gridProperties.RowTemplate.Height = 34;
            gridProperties.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridProperties.Size = new System.Drawing.Size(848, 460);
            gridProperties.TabIndex = 0;

            // Property buttons
            btnAddProperty.BackColor = Helpers.AppTheme.Success;
            btnAddProperty.Font = Helpers.AppTheme.DefaultFontBold;
            btnAddProperty.Location = new System.Drawing.Point(711, 480);
            btnAddProperty.Name = "btnAddProperty";
            btnAddProperty.Size = new System.Drawing.Size(150, 38);
            btnAddProperty.TabIndex = 1;
            btnAddProperty.Text = "➕ إضافة خاصية";

            btnRemoveProperty.BackColor = Helpers.AppTheme.Danger;
            btnRemoveProperty.Font = Helpers.AppTheme.DefaultFontBold;
            btnRemoveProperty.Location = new System.Drawing.Point(551, 480);
            btnRemoveProperty.Name = "btnRemoveProperty";
            btnRemoveProperty.Size = new System.Drawing.Size(150, 38);
            btnRemoveProperty.TabIndex = 2;
            btnRemoveProperty.Text = "➖ حذف خاصية";

            // ================================================================
            // tabRelations
            // ================================================================
            tabRelations.BackColor = Helpers.AppTheme.SurfaceWhite;
            tabRelations.Controls.Add(gridRelations);
            tabRelations.Controls.Add(btnAddRelation);
            tabRelations.Controls.Add(btnRemoveRelation);
            tabRelations.Location = new System.Drawing.Point(4, 39);
            tabRelations.Name = "tabRelations";
            tabRelations.Padding = new System.Windows.Forms.Padding(10);
            tabRelations.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabRelations.Size = new System.Drawing.Size(874, 527);
            tabRelations.TabIndex = 1;
            tabRelations.Text = "العلاقات";

            // gridRelations
            gridRelations.AllowUserToAddRows = false;
            gridRelations.AllowUserToDeleteRows = false;
            gridRelations.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            gridRelations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            gridRelations.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            gridRelations.ColumnHeadersDefaultCellStyle.BackColor = Helpers.AppTheme.Primary;
            gridRelations.ColumnHeadersDefaultCellStyle.Font = Helpers.AppTheme.DefaultFontBold;
            gridRelations.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            gridRelations.ColumnHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(5);
            gridRelations.ColumnHeadersHeight = 42;
            gridRelations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridRelations.DefaultCellStyle.Font = Helpers.AppTheme.DefaultFontSmall;
            gridRelations.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(4);
            gridRelations.DefaultCellStyle.SelectionBackColor = Helpers.AppTheme.Primary;
            gridRelations.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            gridRelations.AlternatingRowsDefaultCellStyle.BackColor = Helpers.AppTheme.AlternateRow;
            gridRelations.EnableHeadersVisualStyles = false;
            gridRelations.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            gridRelations.GridColor = Helpers.AppTheme.GridLine;
            gridRelations.Location = new System.Drawing.Point(13, 13);
            gridRelations.Name = "gridRelations";
            gridRelations.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            gridRelations.RowHeadersVisible = false;
            gridRelations.RowTemplate.Height = 34;
            gridRelations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridRelations.Size = new System.Drawing.Size(848, 460);
            gridRelations.TabIndex = 0;

            // Relation buttons
            btnAddRelation.BackColor = Helpers.AppTheme.Primary;
            btnAddRelation.Font = Helpers.AppTheme.DefaultFontBold;
            btnAddRelation.Location = new System.Drawing.Point(711, 480);
            btnAddRelation.Name = "btnAddRelation";
            btnAddRelation.Size = new System.Drawing.Size(150, 38);
            btnAddRelation.TabIndex = 1;
            btnAddRelation.Text = "➕ إضافة علاقة";

            btnRemoveRelation.BackColor = Helpers.AppTheme.Danger;
            btnRemoveRelation.Font = Helpers.AppTheme.DefaultFontBold;
            btnRemoveRelation.Location = new System.Drawing.Point(551, 480);
            btnRemoveRelation.Name = "btnRemoveRelation";
            btnRemoveRelation.Size = new System.Drawing.Size(150, 38);
            btnRemoveRelation.TabIndex = 2;
            btnRemoveRelation.Text = "➖ حذف علاقة";

            // ================================================================
            // tabPreview
            // ================================================================
            tabPreview.BackColor = Helpers.AppTheme.ConsoleBg;
            tabPreview.Controls.Add(txtPreview);
            tabPreview.Location = new System.Drawing.Point(4, 39);
            tabPreview.Name = "tabPreview";
            tabPreview.Size = new System.Drawing.Size(874, 527);
            tabPreview.TabIndex = 2;
            tabPreview.Text = "المعاينة";

            txtPreview.BackColor = Helpers.AppTheme.ConsoleBg;
            txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            txtPreview.Font = Helpers.AppTheme.ConsoleFont;
            txtPreview.ForeColor = Helpers.AppTheme.ConsoleHighlight;
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.TabIndex = 0;
            txtPreview.Text = "";
            txtPreview.WordWrap = false;

            // ================================================================
            // grpActions
            // ================================================================
            grpActions.Controls.Add(btnGenerate);
            grpActions.Controls.Add(btnSaveModel);
            grpActions.Controls.Add(btnLoadModel);
            grpActions.Controls.Add(btnExportJson);
            grpActions.Controls.Add(btnImportJson);
            grpActions.Controls.Add(btnBack);
            grpActions.Controls.Add(lblStatus);
            grpActions.Font = Helpers.AppTheme.DefaultFontBold;
            grpActions.Location = new System.Drawing.Point(12, 676);
            grpActions.Name = "grpActions";
            grpActions.Padding = new System.Windows.Forms.Padding(10);
            grpActions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpActions.Size = new System.Drawing.Size(1176, 90);
            grpActions.TabIndex = 3;
            grpActions.TabStop = false;
            grpActions.Text = "العمليات";

            // btnGenerate
            btnGenerate.BackColor = Helpers.AppTheme.Success;
            btnGenerate.Font = Helpers.AppTheme.HeaderFont;
            btnGenerate.Location = new System.Drawing.Point(1006, 30);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new System.Drawing.Size(160, 50);
            btnGenerate.TabIndex = 0;
            btnGenerate.Text = "⚡ توليد الكود";

            // btnSaveModel
            btnSaveModel.BackColor = Helpers.AppTheme.Primary;
            btnSaveModel.Font = Helpers.AppTheme.ButtonFontLarge;
            btnSaveModel.Location = new System.Drawing.Point(856, 30);
            btnSaveModel.Name = "btnSaveModel";
            btnSaveModel.Size = new System.Drawing.Size(140, 50);
            btnSaveModel.TabIndex = 1;
            btnSaveModel.Text = "💾 حفظ";

            // btnLoadModel
            btnLoadModel.BackColor = Helpers.AppTheme.Primary;
            btnLoadModel.Font = Helpers.AppTheme.ButtonFontLarge;
            btnLoadModel.Location = new System.Drawing.Point(706, 30);
            btnLoadModel.Name = "btnLoadModel";
            btnLoadModel.Size = new System.Drawing.Size(140, 50);
            btnLoadModel.TabIndex = 2;
            btnLoadModel.Text = "📂 تحميل";

            // btnExportJson
            btnExportJson.BackColor = Helpers.AppTheme.Purple;
            btnExportJson.Font = Helpers.AppTheme.ButtonFontLarge;
            btnExportJson.Location = new System.Drawing.Point(556, 30);
            btnExportJson.Name = "btnExportJson";
            btnExportJson.Size = new System.Drawing.Size(140, 50);
            btnExportJson.TabIndex = 3;
            btnExportJson.Text = "📋 تصدير";

            // btnImportJson
            btnImportJson.BackColor = Helpers.AppTheme.Purple;
            btnImportJson.Font = Helpers.AppTheme.ButtonFontLarge;
            btnImportJson.Location = new System.Drawing.Point(406, 30);
            btnImportJson.Name = "btnImportJson";
            btnImportJson.Size = new System.Drawing.Size(140, 50);
            btnImportJson.TabIndex = 4;
            btnImportJson.Text = "📥 استيراد";

            // btnBack
            btnBack.BackColor = Helpers.AppTheme.Gray;
            btnBack.Font = Helpers.AppTheme.ButtonFontLarge;
            btnBack.Location = new System.Drawing.Point(10, 30);
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(120, 50);
            btnBack.TabIndex = 5;
            btnBack.Text = "رجوع";

            // lblStatus
            lblStatus.AutoSize = true;
            lblStatus.Font = Helpers.AppTheme.DefaultFont;
            lblStatus.ForeColor = Helpers.AppTheme.Success;
            lblStatus.Location = new System.Drawing.Point(150, 45);
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "";

            // ================================================================
            // FrmEntityDesigner
            // ================================================================
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = Helpers.AppTheme.FormBackground;
            ClientSize = new System.Drawing.Size(1200, 780);
            Controls.Add(panelTop);
            Controls.Add(grpEntities);
            Controls.Add(tabControl);
            Controls.Add(grpActions);
            Font = Helpers.AppTheme.DefaultFont;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmEntityDesigner";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "مصمم الكيانات - Code First";

            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            grpEntities.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            tabProperties.ResumeLayout(false);
            tabRelations.ResumeLayout(false);
            tabPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridRelations).EndInit();
            grpActions.ResumeLayout(false);
            grpActions.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cmbDatabaseType;
        private System.Windows.Forms.Label lblFramework;
        private System.Windows.Forms.ComboBox cmbTargetFramework;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label lblNamespaceLabel;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.GroupBox grpEntities;
        private System.Windows.Forms.Label lblEmptyState;
        private System.Windows.Forms.ListBox lstEntities;
        private System.Windows.Forms.Button btnAddEntity;
        private System.Windows.Forms.Button btnRemoveEntity;
        private System.Windows.Forms.Button btnRenameEntity;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.TabPage tabRelations;
        private System.Windows.Forms.TabPage tabPreview;
        private System.Windows.Forms.DataGridView gridProperties;
        private System.Windows.Forms.Button btnAddProperty;
        private System.Windows.Forms.Button btnRemoveProperty;
        private System.Windows.Forms.DataGridView gridRelations;
        private System.Windows.Forms.Button btnAddRelation;
        private System.Windows.Forms.Button btnRemoveRelation;
        private System.Windows.Forms.RichTextBox txtPreview;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnSaveModel;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.Button btnExportJson;
        private System.Windows.Forms.Button btnImportJson;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblStatus;
    }
}
