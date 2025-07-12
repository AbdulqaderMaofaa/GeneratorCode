namespace GeneratorCode.Forms
{
    partial class FrmTabls
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl = new System.Windows.Forms.TabControl();
            tabTables = new System.Windows.Forms.TabPage();
            grpTablesInfo = new System.Windows.Forms.GroupBox();
            lblTablesSelection = new System.Windows.Forms.Label();
            chkGenerateAllTables = new System.Windows.Forms.CheckBox();
            lstTables = new System.Windows.Forms.ListBox();
            grpColumnsInfo = new System.Windows.Forms.GroupBox();
            gridColumns = new System.Windows.Forms.DataGridView();
            tabArchitecture = new System.Windows.Forms.TabPage();
            grpArchitecture = new System.Windows.Forms.GroupBox();
            chkApplicationLayer = new System.Windows.Forms.CheckBox();
            chkDomainLayer = new System.Windows.Forms.CheckBox();
            chkPresentationLayer = new System.Windows.Forms.CheckBox();
            lblArchitecturePattern = new System.Windows.Forms.Label();
            cmbArchitecture = new System.Windows.Forms.ComboBox();
            lblProgrammingLanguage = new System.Windows.Forms.Label();
            cmbLanguage = new System.Windows.Forms.ComboBox();
            tabGeneration = new System.Windows.Forms.TabPage();
            grpCodeGeneration = new System.Windows.Forms.GroupBox();
            chkEntities = new System.Windows.Forms.CheckBox();
            chkDTOs = new System.Windows.Forms.CheckBox();
            chkRepositories = new System.Windows.Forms.CheckBox();
            chkServices = new System.Windows.Forms.CheckBox();
            chkControllers = new System.Windows.Forms.CheckBox();
            chkUnitTests = new System.Windows.Forms.CheckBox();
            chkValidation = new System.Windows.Forms.CheckBox();
            chkSwagger = new System.Windows.Forms.CheckBox();
            chkDependencyInjection = new System.Windows.Forms.CheckBox();
            grpCrudOperations = new System.Windows.Forms.GroupBox();
            chkGenerateCreate = new System.Windows.Forms.CheckBox();
            chkGenerateRead = new System.Windows.Forms.CheckBox();
            chkGenerateUpdate = new System.Windows.Forms.CheckBox();
            chkGenerateDelete = new System.Windows.Forms.CheckBox();
            grpProjectStructure = new System.Windows.Forms.GroupBox();
            chkGenerateStartup = new System.Windows.Forms.CheckBox();
            chkGenerateProgram = new System.Windows.Forms.CheckBox();
            chkGenerateGitignore = new System.Windows.Forms.CheckBox();
            chkGenerateReadme = new System.Windows.Forms.CheckBox();
            chkGenerateSolution = new System.Windows.Forms.CheckBox();
            tabSettings = new System.Windows.Forms.TabPage();
            grpProjectSettings = new System.Windows.Forms.GroupBox();
            lblNamespace = new System.Windows.Forms.Label();
            txtNamespace = new System.Windows.Forms.TextBox();
            lblOutputPath = new System.Windows.Forms.Label();
            txtOutputPath = new System.Windows.Forms.TextBox();
            btnBrowse = new System.Windows.Forms.Button();
            btnSettings = new System.Windows.Forms.Button();
            grpDatabaseInfo = new System.Windows.Forms.GroupBox();
            lblDatabaseType = new System.Windows.Forms.Label();
            lblConnectionInfo = new System.Windows.Forms.Label();
            tabOutput = new System.Windows.Forms.TabPage();
            grpActions = new System.Windows.Forms.GroupBox();
            btnPreview = new System.Windows.Forms.Button();
            btnGenerate = new System.Windows.Forms.Button();
            grpProgress = new System.Windows.Forms.GroupBox();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblStatus = new System.Windows.Forms.Label();
            chkInfrastructureLayer = new System.Windows.Forms.CheckBox();
            tabControl.SuspendLayout();
            tabTables.SuspendLayout();
            grpTablesInfo.SuspendLayout();
            grpColumnsInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridColumns).BeginInit();
            tabArchitecture.SuspendLayout();
            grpArchitecture.SuspendLayout();
            tabGeneration.SuspendLayout();
            grpCodeGeneration.SuspendLayout();
            grpCrudOperations.SuspendLayout();
            grpProjectStructure.SuspendLayout();
            tabSettings.SuspendLayout();
            grpProjectSettings.SuspendLayout();
            grpDatabaseInfo.SuspendLayout();
            tabOutput.SuspendLayout();
            grpActions.SuspendLayout();
            grpProgress.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabTables);
            tabControl.Controls.Add(tabArchitecture);
            tabControl.Controls.Add(tabGeneration);
            tabControl.Controls.Add(tabSettings);
            tabControl.Controls.Add(tabOutput);
            tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tabControl.Location = new System.Drawing.Point(0, 0);
            tabControl.Multiline = false;
            tabControl.Name = "tabControl";
            tabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabControl.RightToLeftLayout = true;
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(1200, 800);
            tabControl.TabIndex = 0;
            // 
            // tabTables
            // 
            tabTables.Controls.Add(grpTablesInfo);
            tabTables.Controls.Add(grpColumnsInfo);
            tabTables.Location = new System.Drawing.Point(4, 32);
            tabTables.Name = "tabTables";
            tabTables.Padding = new System.Windows.Forms.Padding(10);
            tabTables.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabTables.Size = new System.Drawing.Size(1192, 764);
            tabTables.TabIndex = 0;
            tabTables.Text = "الجداول والأعمدة";
            // 
            // grpTablesInfo
            // 
            grpTablesInfo.Controls.Add(lblTablesSelection);
            grpTablesInfo.Controls.Add(chkGenerateAllTables);
            grpTablesInfo.Controls.Add(lstTables);
            grpTablesInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpTablesInfo.Location = new System.Drawing.Point(829, 13);
            grpTablesInfo.Name = "grpTablesInfo";
            grpTablesInfo.Padding = new System.Windows.Forms.Padding(10);
            grpTablesInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpTablesInfo.Size = new System.Drawing.Size(350, 738);
            grpTablesInfo.TabIndex = 0;
            grpTablesInfo.TabStop = false;
            grpTablesInfo.Text = "🗂️ الجداول المتاحة";
            // 
            // lblTablesSelection
            // 
            lblTablesSelection.AutoSize = true;
            lblTablesSelection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblTablesSelection.ForeColor = System.Drawing.Color.Black;
            lblTablesSelection.Location = new System.Drawing.Point(151, 33);
            lblTablesSelection.Name = "lblTablesSelection";
            lblTablesSelection.Size = new System.Drawing.Size(186, 20);
            lblTablesSelection.TabIndex = 0;
            lblTablesSelection.Text = "اختر الجداول لتوليد الكود لها:";
            // 
            // chkGenerateAllTables
            // 
            chkGenerateAllTables.AutoSize = true;
            chkGenerateAllTables.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateAllTables.ForeColor = System.Drawing.Color.Black;
            chkGenerateAllTables.Location = new System.Drawing.Point(187, 65);
            chkGenerateAllTables.Name = "chkGenerateAllTables";
            chkGenerateAllTables.Size = new System.Drawing.Size(150, 24);
            chkGenerateAllTables.TabIndex = 1;
            chkGenerateAllTables.Text = "توليد جميع الجداول";
            chkGenerateAllTables.UseVisualStyleBackColor = true;
            // 
            // lstTables
            // 
            lstTables.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lstTables.FormattingEnabled = true;
            lstTables.ItemHeight = 20;
            lstTables.Location = new System.Drawing.Point(13, 95);
            lstTables.Name = "lstTables";
            lstTables.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lstTables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lstTables.Size = new System.Drawing.Size(324, 624);
            lstTables.TabIndex = 2;
            // 
            // grpColumnsInfo
            // 
            grpColumnsInfo.Controls.Add(gridColumns);
            grpColumnsInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpColumnsInfo.ForeColor = System.Drawing.Color.DarkGreen;
            grpColumnsInfo.Location = new System.Drawing.Point(13, 13);
            grpColumnsInfo.Name = "grpColumnsInfo";
            grpColumnsInfo.Padding = new System.Windows.Forms.Padding(10);
            grpColumnsInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpColumnsInfo.Size = new System.Drawing.Size(800, 738);
            grpColumnsInfo.TabIndex = 1;
            grpColumnsInfo.TabStop = false;
            grpColumnsInfo.Text = "📋 تفاصيل الأعمدة";
            // 
            // gridColumns
            // 
            gridColumns.AllowUserToAddRows = false;
            gridColumns.AllowUserToDeleteRows = false;
            gridColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            gridColumns.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            gridColumns.Location = new System.Drawing.Point(10, 33);
            gridColumns.Name = "gridColumns";
            gridColumns.ReadOnly = true;
            gridColumns.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            gridColumns.RowHeadersWidth = 51;
            gridColumns.Size = new System.Drawing.Size(780, 695);
            gridColumns.TabIndex = 0;
            // 
            // tabArchitecture
            // 
            tabArchitecture.Controls.Add(grpArchitecture);
            tabArchitecture.Controls.Add(lblArchitecturePattern);
            tabArchitecture.Controls.Add(cmbArchitecture);
            tabArchitecture.Controls.Add(lblProgrammingLanguage);
            tabArchitecture.Controls.Add(cmbLanguage);
            tabArchitecture.Location = new System.Drawing.Point(4, 32);
            tabArchitecture.Name = "tabArchitecture";
            tabArchitecture.Padding = new System.Windows.Forms.Padding(10);
            tabArchitecture.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabArchitecture.Size = new System.Drawing.Size(1192, 764);
            tabArchitecture.TabIndex = 1;
            tabArchitecture.Text = "النمط المعماري";
            // 
            // grpArchitecture
            // 
            grpArchitecture.Controls.Add(chkInfrastructureLayer);
            grpArchitecture.Controls.Add(chkApplicationLayer);
            grpArchitecture.Controls.Add(chkDomainLayer);
            grpArchitecture.Controls.Add(chkPresentationLayer);
            grpArchitecture.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpArchitecture.ForeColor = System.Drawing.Color.DarkOrange;
            grpArchitecture.Location = new System.Drawing.Point(13, 180);
            grpArchitecture.Name = "grpArchitecture";
            grpArchitecture.Padding = new System.Windows.Forms.Padding(10);
            grpArchitecture.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpArchitecture.Size = new System.Drawing.Size(1166, 150);
            grpArchitecture.TabIndex = 4;
            grpArchitecture.TabStop = false;
            grpArchitecture.Text = "🏛️ طبقات النمط المعماري";
            // 
            // chkApplicationLayer
            // 
            chkApplicationLayer.AutoSize = true;
            chkApplicationLayer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkApplicationLayer.ForeColor = System.Drawing.Color.Black;
            chkApplicationLayer.Location = new System.Drawing.Point(912, 36);
            chkApplicationLayer.Name = "chkApplicationLayer";
            chkApplicationLayer.Size = new System.Drawing.Size(241, 24);
            chkApplicationLayer.TabIndex = 1;
            chkApplicationLayer.Text = "طبقة التطبيق  Application Layer";
            chkApplicationLayer.UseVisualStyleBackColor = true;
            // 
            // chkDomainLayer
            // 
            chkDomainLayer.AutoSize = true;
            chkDomainLayer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkDomainLayer.ForeColor = System.Drawing.Color.Black;
            chkDomainLayer.Location = new System.Drawing.Point(939, 62);
            chkDomainLayer.Name = "chkDomainLayer";
            chkDomainLayer.Size = new System.Drawing.Size(214, 24);
            chkDomainLayer.TabIndex = 2;
            chkDomainLayer.Text = " طبقة المجال  Domain Layer";
            chkDomainLayer.UseVisualStyleBackColor = true;
            // 
            // chkPresentationLayer
            // 
            chkPresentationLayer.AutoSize = true;
            chkPresentationLayer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkPresentationLayer.ForeColor = System.Drawing.Color.Black;
            chkPresentationLayer.Location = new System.Drawing.Point(972, 114);
            chkPresentationLayer.Name = "chkPresentationLayer";
            chkPresentationLayer.Size = new System.Drawing.Size(181, 24);
            chkPresentationLayer.TabIndex = 3;
            chkPresentationLayer.Text = "طبقة العرض  API Layer";
            chkPresentationLayer.UseVisualStyleBackColor = true;
            // 
            // lblArchitecturePattern
            // 
            lblArchitecturePattern.AutoSize = true;
            lblArchitecturePattern.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblArchitecturePattern.ForeColor = System.Drawing.Color.DarkBlue;
            lblArchitecturePattern.Location = new System.Drawing.Point(1051, 13);
            lblArchitecturePattern.Name = "lblArchitecturePattern";
            lblArchitecturePattern.Size = new System.Drawing.Size(123, 23);
            lblArchitecturePattern.TabIndex = 0;
            lblArchitecturePattern.Text = "النمط المعماري:";
            // 
            // cmbArchitecture
            // 
            cmbArchitecture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbArchitecture.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbArchitecture.FormattingEnabled = true;
            cmbArchitecture.Location = new System.Drawing.Point(779, 45);
            cmbArchitecture.Name = "cmbArchitecture";
            cmbArchitecture.Size = new System.Drawing.Size(400, 31);
            cmbArchitecture.TabIndex = 1;
            // 
            // lblProgrammingLanguage
            // 
            lblProgrammingLanguage.AutoSize = true;
            lblProgrammingLanguage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblProgrammingLanguage.ForeColor = System.Drawing.Color.DarkBlue;
            lblProgrammingLanguage.Location = new System.Drawing.Point(1045, 95);
            lblProgrammingLanguage.Name = "lblProgrammingLanguage";
            lblProgrammingLanguage.Size = new System.Drawing.Size(96, 23);
            lblProgrammingLanguage.TabIndex = 2;
            lblProgrammingLanguage.Text = "لغة البرمجة:";
            // 
            // cmbLanguage
            // 
            cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLanguage.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbLanguage.FormattingEnabled = true;
            cmbLanguage.Location = new System.Drawing.Point(779, 127);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new System.Drawing.Size(400, 31);
            cmbLanguage.TabIndex = 3;
            // 
            // tabGeneration
            // 
            tabGeneration.Controls.Add(grpCodeGeneration);
            tabGeneration.Controls.Add(grpCrudOperations);
            tabGeneration.Controls.Add(grpProjectStructure);
            tabGeneration.Location = new System.Drawing.Point(4, 32);
            tabGeneration.Name = "tabGeneration";
            tabGeneration.Padding = new System.Windows.Forms.Padding(10);
            tabGeneration.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabGeneration.Size = new System.Drawing.Size(1192, 764);
            tabGeneration.TabIndex = 2;
            tabGeneration.Text = "خيارات التوليد";
            // 
            // grpCodeGeneration
            // 
            grpCodeGeneration.Controls.Add(chkEntities);
            grpCodeGeneration.Controls.Add(chkDTOs);
            grpCodeGeneration.Controls.Add(chkRepositories);
            grpCodeGeneration.Controls.Add(chkServices);
            grpCodeGeneration.Controls.Add(chkControllers);
            grpCodeGeneration.Controls.Add(chkUnitTests);
            grpCodeGeneration.Controls.Add(chkValidation);
            grpCodeGeneration.Controls.Add(chkSwagger);
            grpCodeGeneration.Controls.Add(chkDependencyInjection);
            grpCodeGeneration.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpCodeGeneration.ForeColor = System.Drawing.Color.DarkBlue;
            grpCodeGeneration.Location = new System.Drawing.Point(570, 13);
            grpCodeGeneration.Name = "grpCodeGeneration";
            grpCodeGeneration.Padding = new System.Windows.Forms.Padding(10);
            grpCodeGeneration.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpCodeGeneration.Size = new System.Drawing.Size(609, 202);
            grpCodeGeneration.TabIndex = 0;
            grpCodeGeneration.TabStop = false;
            grpCodeGeneration.Text = "🔧 توليد الكود";
            // 
            // chkEntities
            // 
            chkEntities.AutoSize = true;
            chkEntities.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkEntities.ForeColor = System.Drawing.Color.Black;
            chkEntities.Location = new System.Drawing.Point(503, 39);
            chkEntities.Name = "chkEntities";
            chkEntities.Size = new System.Drawing.Size(80, 24);
            chkEntities.TabIndex = 0;
            chkEntities.Text = "الكائنات";
            chkEntities.UseVisualStyleBackColor = true;
            // 
            // chkDTOs
            // 
            chkDTOs.AutoSize = true;
            chkDTOs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkDTOs.ForeColor = System.Drawing.Color.Black;
            chkDTOs.Location = new System.Drawing.Point(476, 69);
            chkDTOs.Name = "chkDTOs";
            chkDTOs.Size = new System.Drawing.Size(107, 24);
            chkDTOs.TabIndex = 1;
            chkDTOs.Text = "كائنات النقل";
            chkDTOs.UseVisualStyleBackColor = true;
            // 
            // chkRepositories
            // 
            chkRepositories.AutoSize = true;
            chkRepositories.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkRepositories.ForeColor = System.Drawing.Color.Black;
            chkRepositories.Location = new System.Drawing.Point(473, 99);
            chkRepositories.Name = "chkRepositories";
            chkRepositories.Size = new System.Drawing.Size(110, 24);
            chkRepositories.TabIndex = 2;
            chkRepositories.Text = "المستودعات";
            chkRepositories.UseVisualStyleBackColor = true;
            // 
            // chkServices
            // 
            chkServices.AutoSize = true;
            chkServices.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkServices.ForeColor = System.Drawing.Color.Black;
            chkServices.Location = new System.Drawing.Point(499, 129);
            chkServices.Name = "chkServices";
            chkServices.Size = new System.Drawing.Size(84, 24);
            chkServices.TabIndex = 3;
            chkServices.Text = "الخدمات";
            chkServices.UseVisualStyleBackColor = true;
            // 
            // chkControllers
            // 
            chkControllers.AutoSize = true;
            chkControllers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkControllers.ForeColor = System.Drawing.Color.Black;
            chkControllers.Location = new System.Drawing.Point(494, 159);
            chkControllers.Name = "chkControllers";
            chkControllers.Size = new System.Drawing.Size(89, 24);
            chkControllers.TabIndex = 4;
            chkControllers.Text = "التحكمات";
            chkControllers.UseVisualStyleBackColor = true;
            // 
            // chkUnitTests
            // 
            chkUnitTests.AutoSize = true;
            chkUnitTests.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkUnitTests.ForeColor = System.Drawing.Color.Black;
            chkUnitTests.Location = new System.Drawing.Point(242, 39);
            chkUnitTests.Name = "chkUnitTests";
            chkUnitTests.Size = new System.Drawing.Size(127, 24);
            chkUnitTests.TabIndex = 5;
            chkUnitTests.Text = "اختبارات الوحدة";
            chkUnitTests.UseVisualStyleBackColor = true;
            // 
            // chkValidation
            // 
            chkValidation.AutoSize = true;
            chkValidation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkValidation.ForeColor = System.Drawing.Color.Black;
            chkValidation.Location = new System.Drawing.Point(296, 69);
            chkValidation.Name = "chkValidation";
            chkValidation.Size = new System.Drawing.Size(73, 24);
            chkValidation.TabIndex = 6;
            chkValidation.Text = "التحقق";
            chkValidation.UseVisualStyleBackColor = true;
            // 
            // chkSwagger
            // 
            chkSwagger.AutoSize = true;
            chkSwagger.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkSwagger.ForeColor = System.Drawing.Color.Black;
            chkSwagger.Location = new System.Drawing.Point(206, 99);
            chkSwagger.Name = "chkSwagger";
            chkSwagger.Size = new System.Drawing.Size(163, 24);
            chkSwagger.TabIndex = 7;
            chkSwagger.Text = "توثيق API (Swagger)";
            chkSwagger.UseVisualStyleBackColor = true;
            // 
            // chkDependencyInjection
            // 
            chkDependencyInjection.AutoSize = true;
            chkDependencyInjection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkDependencyInjection.ForeColor = System.Drawing.Color.Black;
            chkDependencyInjection.Location = new System.Drawing.Point(256, 129);
            chkDependencyInjection.Name = "chkDependencyInjection";
            chkDependencyInjection.Size = new System.Drawing.Size(113, 24);
            chkDependencyInjection.TabIndex = 8;
            chkDependencyInjection.Text = "حقن التبعيات";
            chkDependencyInjection.UseVisualStyleBackColor = true;
            // 
            // grpCrudOperations
            // 
            grpCrudOperations.Controls.Add(chkGenerateCreate);
            grpCrudOperations.Controls.Add(chkGenerateRead);
            grpCrudOperations.Controls.Add(chkGenerateUpdate);
            grpCrudOperations.Controls.Add(chkGenerateDelete);
            grpCrudOperations.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpCrudOperations.ForeColor = System.Drawing.Color.DarkGreen;
            grpCrudOperations.Location = new System.Drawing.Point(609, 13);
            grpCrudOperations.Name = "grpCrudOperations";
            grpCrudOperations.Padding = new System.Windows.Forms.Padding(10);
            grpCrudOperations.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpCrudOperations.Size = new System.Drawing.Size(570, 180);
            grpCrudOperations.TabIndex = 1;
            grpCrudOperations.TabStop = false;
            grpCrudOperations.Text = "📝 عمليات CRUD";
            // 
            // chkGenerateCreate
            // 
            chkGenerateCreate.AutoSize = true;
            chkGenerateCreate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateCreate.ForeColor = System.Drawing.Color.Black;
            chkGenerateCreate.Location = new System.Drawing.Point(13, 35);
            chkGenerateCreate.Name = "chkGenerateCreate";
            chkGenerateCreate.Size = new System.Drawing.Size(120, 24);
            chkGenerateCreate.TabIndex = 0;
            chkGenerateCreate.Text = "إنشاء (Create)";
            chkGenerateCreate.UseVisualStyleBackColor = true;
            // 
            // chkGenerateRead
            // 
            chkGenerateRead.AutoSize = true;
            chkGenerateRead.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateRead.ForeColor = System.Drawing.Color.Black;
            chkGenerateRead.Location = new System.Drawing.Point(13, 65);
            chkGenerateRead.Name = "chkGenerateRead";
            chkGenerateRead.Size = new System.Drawing.Size(110, 24);
            chkGenerateRead.TabIndex = 1;
            chkGenerateRead.Text = "قراءة (Read)";
            chkGenerateRead.UseVisualStyleBackColor = true;
            // 
            // chkGenerateUpdate
            // 
            chkGenerateUpdate.AutoSize = true;
            chkGenerateUpdate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateUpdate.ForeColor = System.Drawing.Color.Black;
            chkGenerateUpdate.Location = new System.Drawing.Point(13, 95);
            chkGenerateUpdate.Name = "chkGenerateUpdate";
            chkGenerateUpdate.Size = new System.Drawing.Size(135, 24);
            chkGenerateUpdate.TabIndex = 2;
            chkGenerateUpdate.Text = "تحديث (Update)";
            chkGenerateUpdate.UseVisualStyleBackColor = true;
            // 
            // chkGenerateDelete
            // 
            chkGenerateDelete.AutoSize = true;
            chkGenerateDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateDelete.ForeColor = System.Drawing.Color.Black;
            chkGenerateDelete.Location = new System.Drawing.Point(13, 125);
            chkGenerateDelete.Name = "chkGenerateDelete";
            chkGenerateDelete.Size = new System.Drawing.Size(121, 24);
            chkGenerateDelete.TabIndex = 3;
            chkGenerateDelete.Text = "حذف (Delete)";
            chkGenerateDelete.UseVisualStyleBackColor = true;
            // 
            // grpProjectStructure
            // 
            grpProjectStructure.Controls.Add(chkGenerateStartup);
            grpProjectStructure.Controls.Add(chkGenerateProgram);
            grpProjectStructure.Controls.Add(chkGenerateGitignore);
            grpProjectStructure.Controls.Add(chkGenerateReadme);
            grpProjectStructure.Controls.Add(chkGenerateSolution);
            grpProjectStructure.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpProjectStructure.ForeColor = System.Drawing.Color.DarkOrange;
            grpProjectStructure.Location = new System.Drawing.Point(570, 240);
            grpProjectStructure.Name = "grpProjectStructure";
            grpProjectStructure.Padding = new System.Windows.Forms.Padding(10);
            grpProjectStructure.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpProjectStructure.Size = new System.Drawing.Size(609, 180);
            grpProjectStructure.TabIndex = 2;
            grpProjectStructure.TabStop = false;
            grpProjectStructure.Text = "📁 بنية المشروع";
            // 
            // chkGenerateStartup
            // 
            chkGenerateStartup.AutoSize = true;
            chkGenerateStartup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateStartup.ForeColor = System.Drawing.Color.Black;
            chkGenerateStartup.Location = new System.Drawing.Point(471, 40);
            chkGenerateStartup.Name = "chkGenerateStartup";
            chkGenerateStartup.Size = new System.Drawing.Size(112, 24);
            chkGenerateStartup.TabIndex = 0;
            chkGenerateStartup.Text = "ملف Startup";
            chkGenerateStartup.UseVisualStyleBackColor = true;
            // 
            // chkGenerateProgram
            // 
            chkGenerateProgram.AutoSize = true;
            chkGenerateProgram.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateProgram.ForeColor = System.Drawing.Color.Black;
            chkGenerateProgram.Location = new System.Drawing.Point(462, 70);
            chkGenerateProgram.Name = "chkGenerateProgram";
            chkGenerateProgram.Size = new System.Drawing.Size(121, 24);
            chkGenerateProgram.TabIndex = 1;
            chkGenerateProgram.Text = "ملف Program";
            chkGenerateProgram.UseVisualStyleBackColor = true;
            // 
            // chkGenerateGitignore
            // 
            chkGenerateGitignore.AutoSize = true;
            chkGenerateGitignore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateGitignore.ForeColor = System.Drawing.Color.Black;
            chkGenerateGitignore.Location = new System.Drawing.Point(455, 100);
            chkGenerateGitignore.Name = "chkGenerateGitignore";
            chkGenerateGitignore.Size = new System.Drawing.Size(128, 24);
            chkGenerateGitignore.TabIndex = 2;
            chkGenerateGitignore.Text = "ملف .gitignore";
            chkGenerateGitignore.UseVisualStyleBackColor = true;
            // 
            // chkGenerateReadme
            // 
            chkGenerateReadme.AutoSize = true;
            chkGenerateReadme.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateReadme.ForeColor = System.Drawing.Color.Black;
            chkGenerateReadme.Location = new System.Drawing.Point(460, 130);
            chkGenerateReadme.Name = "chkGenerateReadme";
            chkGenerateReadme.Size = new System.Drawing.Size(123, 24);
            chkGenerateReadme.TabIndex = 3;
            chkGenerateReadme.Text = "ملف README";
            chkGenerateReadme.UseVisualStyleBackColor = true;
            // 
            // chkGenerateSolution
            // 
            chkGenerateSolution.AutoSize = true;
            chkGenerateSolution.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkGenerateSolution.ForeColor = System.Drawing.Color.Black;
            chkGenerateSolution.Location = new System.Drawing.Point(250, 35);
            chkGenerateSolution.Name = "chkGenerateSolution";
            chkGenerateSolution.Size = new System.Drawing.Size(119, 24);
            chkGenerateSolution.TabIndex = 4;
            chkGenerateSolution.Text = "ملف Solution";
            chkGenerateSolution.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(grpProjectSettings);
            tabSettings.Controls.Add(grpDatabaseInfo);
            tabSettings.Location = new System.Drawing.Point(4, 32);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new System.Windows.Forms.Padding(10);
            tabSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabSettings.Size = new System.Drawing.Size(1192, 764);
            tabSettings.TabIndex = 3;
            tabSettings.Text = "الإعدادات";
            // 
            // grpProjectSettings
            // 
            grpProjectSettings.Controls.Add(lblNamespace);
            grpProjectSettings.Controls.Add(txtNamespace);
            grpProjectSettings.Controls.Add(lblOutputPath);
            grpProjectSettings.Controls.Add(txtOutputPath);
            grpProjectSettings.Controls.Add(btnBrowse);
            grpProjectSettings.Controls.Add(btnSettings);
            grpProjectSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpProjectSettings.ForeColor = System.Drawing.Color.DarkBlue;
            grpProjectSettings.Location = new System.Drawing.Point(13, 13);
            grpProjectSettings.Name = "grpProjectSettings";
            grpProjectSettings.Padding = new System.Windows.Forms.Padding(10);
            grpProjectSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpProjectSettings.Size = new System.Drawing.Size(1166, 200);
            grpProjectSettings.TabIndex = 0;
            grpProjectSettings.TabStop = false;
            grpProjectSettings.Text = "🔧 إعدادات المشروع";
            // 
            // lblNamespace
            // 
            lblNamespace.AutoSize = true;
            lblNamespace.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblNamespace.ForeColor = System.Drawing.Color.Black;
            lblNamespace.Location = new System.Drawing.Point(948, 33);
            lblNamespace.Name = "lblNamespace";
            lblNamespace.Size = new System.Drawing.Size(205, 23);
            lblNamespace.TabIndex = 0;
            lblNamespace.Text = "مساحة الاسم:   namespace";
            // 
            // txtNamespace
            // 
            txtNamespace.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtNamespace.Location = new System.Drawing.Point(297, 59);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(856, 30);
            txtNamespace.TabIndex = 1;
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblOutputPath.ForeColor = System.Drawing.Color.Black;
            lblOutputPath.Location = new System.Drawing.Point(1055, 109);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new System.Drawing.Size(97, 23);
            lblOutputPath.TabIndex = 2;
            lblOutputPath.Text = "مسار الحفظ:";
            // 
            // txtOutputPath
            // 
            txtOutputPath.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            txtOutputPath.Location = new System.Drawing.Point(297, 135);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(856, 30);
            txtOutputPath.TabIndex = 3;
            // 
            // btnBrowse
            // 
            btnBrowse.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnBrowse.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnBrowse.ForeColor = System.Drawing.Color.White;
            btnBrowse.Location = new System.Drawing.Point(168, 135);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(123, 30);
            btnBrowse.TabIndex = 4;
            btnBrowse.Text = "📁 استعراض";
            btnBrowse.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            btnSettings.BackColor = System.Drawing.Color.FromArgb(155, 89, 182);
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSettings.ForeColor = System.Drawing.Color.White;
            btnSettings.Location = new System.Drawing.Point(24, 135);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(138, 30);
            btnSettings.TabIndex = 5;
            btnSettings.Text = "⚙️ إعدادات";
            btnSettings.UseVisualStyleBackColor = false;
            // 
            // grpDatabaseInfo
            // 
            grpDatabaseInfo.Controls.Add(lblDatabaseType);
            grpDatabaseInfo.Controls.Add(lblConnectionInfo);
            grpDatabaseInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpDatabaseInfo.ForeColor = System.Drawing.Color.DarkGreen;
            grpDatabaseInfo.Location = new System.Drawing.Point(13, 233);
            grpDatabaseInfo.Name = "grpDatabaseInfo";
            grpDatabaseInfo.Padding = new System.Windows.Forms.Padding(10);
            grpDatabaseInfo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpDatabaseInfo.Size = new System.Drawing.Size(1166, 120);
            grpDatabaseInfo.TabIndex = 1;
            grpDatabaseInfo.TabStop = false;
            grpDatabaseInfo.Text = "🗄️ معلومات قاعدة البيانات";
            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblDatabaseType.ForeColor = System.Drawing.Color.Black;
            lblDatabaseType.Location = new System.Drawing.Point(1011, 33);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Size = new System.Drawing.Size(142, 23);
            lblDatabaseType.TabIndex = 0;
            lblDatabaseType.Text = "نوع قاعدة البيانات:";
            // 
            // lblConnectionInfo
            // 
            lblConnectionInfo.AutoSize = true;
            lblConnectionInfo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblConnectionInfo.ForeColor = System.Drawing.Color.Green;
            lblConnectionInfo.Location = new System.Drawing.Point(1011, 63);
            lblConnectionInfo.Name = "lblConnectionInfo";
            lblConnectionInfo.Size = new System.Drawing.Size(100, 23);
            lblConnectionInfo.TabIndex = 1;
            lblConnectionInfo.Text = "حالة الاتصال:";
            // 
            // tabOutput
            // 
            tabOutput.Controls.Add(grpActions);
            tabOutput.Controls.Add(grpProgress);
            tabOutput.Location = new System.Drawing.Point(4, 32);
            tabOutput.Name = "tabOutput";
            tabOutput.Padding = new System.Windows.Forms.Padding(10);
            tabOutput.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            tabOutput.Size = new System.Drawing.Size(1192, 764);
            tabOutput.TabIndex = 4;
            tabOutput.Text = "التنفيذ والمخرجات";
            // 
            // grpActions
            // 
            grpActions.Controls.Add(btnPreview);
            grpActions.Controls.Add(btnGenerate);
            grpActions.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpActions.ForeColor = System.Drawing.Color.DarkBlue;
            grpActions.Location = new System.Drawing.Point(13, 13);
            grpActions.Name = "grpActions";
            grpActions.Padding = new System.Windows.Forms.Padding(10);
            grpActions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpActions.Size = new System.Drawing.Size(1166, 120);
            grpActions.TabIndex = 0;
            grpActions.TabStop = false;
            grpActions.Text = "🎯 الإجراءات";
            // 
            // btnPreview
            // 
            btnPreview.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnPreview.FlatAppearance.BorderSize = 0;
            btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnPreview.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnPreview.ForeColor = System.Drawing.Color.White;
            btnPreview.Location = new System.Drawing.Point(723, 36);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new System.Drawing.Size(200, 50);
            btnPreview.TabIndex = 0;
            btnPreview.Text = "👁️ معاينة الكود";
            btnPreview.UseVisualStyleBackColor = false;
            // 
            // btnGenerate
            // 
            btnGenerate.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGenerate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnGenerate.ForeColor = System.Drawing.Color.White;
            btnGenerate.Location = new System.Drawing.Point(943, 36);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new System.Drawing.Size(200, 50);
            btnGenerate.TabIndex = 1;
            btnGenerate.Text = "🚀 توليد الكود";
            btnGenerate.UseVisualStyleBackColor = false;
            // 
            // grpProgress
            // 
            grpProgress.Controls.Add(progressBar);
            grpProgress.Controls.Add(lblStatus);
            grpProgress.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            grpProgress.ForeColor = System.Drawing.Color.DarkGreen;
            grpProgress.Location = new System.Drawing.Point(13, 153);
            grpProgress.Name = "grpProgress";
            grpProgress.Padding = new System.Windows.Forms.Padding(10);
            grpProgress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpProgress.Size = new System.Drawing.Size(1166, 120);
            grpProgress.TabIndex = 1;
            grpProgress.TabStop = false;
            grpProgress.Text = "📊 التقدم والحالة";
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(13, 35);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(1140, 30);
            progressBar.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblStatus.ForeColor = System.Drawing.Color.Black;
            lblStatus.Location = new System.Drawing.Point(13, 75);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(42, 23);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "جاهز";
            // 
            // chkInfrastructureLayer
            // 
            chkInfrastructureLayer.AutoSize = true;
            chkInfrastructureLayer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            chkInfrastructureLayer.ForeColor = System.Drawing.Color.Black;
            chkInfrastructureLayer.Location = new System.Drawing.Point(873, 88);
            chkInfrastructureLayer.Name = "chkInfrastructureLayer";
            chkInfrastructureLayer.Size = new System.Drawing.Size(280, 24);
            chkInfrastructureLayer.TabIndex = 1;
            chkInfrastructureLayer.Text = "طبقة البنية التحتية  Infrastructure Layer";
            chkInfrastructureLayer.UseVisualStyleBackColor = true;
            // 
            // FrmTabls
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            ClientSize = new System.Drawing.Size(1200, 800);
            Controls.Add(tabControl);
            Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmTabls";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "🔧 مولد الكود - اختيار الجداول وتوليد الكود";
            tabControl.ResumeLayout(false);
            tabTables.ResumeLayout(false);
            grpTablesInfo.ResumeLayout(false);
            grpTablesInfo.PerformLayout();
            grpColumnsInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridColumns).EndInit();
            tabArchitecture.ResumeLayout(false);
            tabArchitecture.PerformLayout();
            grpArchitecture.ResumeLayout(false);
            grpArchitecture.PerformLayout();
            tabGeneration.ResumeLayout(false);
            grpCodeGeneration.ResumeLayout(false);
            grpCodeGeneration.PerformLayout();
            grpCrudOperations.ResumeLayout(false);
            grpCrudOperations.PerformLayout();
            grpProjectStructure.ResumeLayout(false);
            grpProjectStructure.PerformLayout();
            tabSettings.ResumeLayout(false);
            grpProjectSettings.ResumeLayout(false);
            grpProjectSettings.PerformLayout();
            grpDatabaseInfo.ResumeLayout(false);
            grpDatabaseInfo.PerformLayout();
            tabOutput.ResumeLayout(false);
            grpActions.ResumeLayout(false);
            grpProgress.ResumeLayout(false);
            grpProgress.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // Tab Control
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabTables;
        private System.Windows.Forms.TabPage tabArchitecture;
        private System.Windows.Forms.TabPage tabGeneration;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabOutput;
        
        // Tab 1: Tables & Columns
        private System.Windows.Forms.GroupBox grpTablesInfo;
        private System.Windows.Forms.GroupBox grpColumnsInfo;
        private System.Windows.Forms.ListBox lstTables;
        private System.Windows.Forms.DataGridView gridColumns;
        private System.Windows.Forms.CheckBox chkGenerateAllTables;
        private System.Windows.Forms.Label lblTablesSelection;
        
        // Tab 2: Architecture Pattern
        private System.Windows.Forms.ComboBox cmbArchitecture;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.GroupBox grpArchitecture;
        private System.Windows.Forms.CheckBox chkApplicationLayer;
        private System.Windows.Forms.CheckBox chkDomainLayer;
        private System.Windows.Forms.CheckBox chkPresentationLayer;
        private System.Windows.Forms.Label lblArchitecturePattern;
        private System.Windows.Forms.Label lblProgrammingLanguage;
        
        // Tab 3: Generation Options
        private System.Windows.Forms.GroupBox grpCodeGeneration;
        private System.Windows.Forms.CheckBox chkEntities;
        private System.Windows.Forms.CheckBox chkDTOs;
        private System.Windows.Forms.CheckBox chkRepositories;
        private System.Windows.Forms.CheckBox chkServices;
        private System.Windows.Forms.CheckBox chkControllers;
        private System.Windows.Forms.CheckBox chkUnitTests;
        private System.Windows.Forms.CheckBox chkValidation;
        private System.Windows.Forms.CheckBox chkSwagger;
        private System.Windows.Forms.CheckBox chkDependencyInjection;
        
        private System.Windows.Forms.GroupBox grpCrudOperations;
        private System.Windows.Forms.CheckBox chkGenerateCreate;
        private System.Windows.Forms.CheckBox chkGenerateRead;
        private System.Windows.Forms.CheckBox chkGenerateUpdate;
        private System.Windows.Forms.CheckBox chkGenerateDelete;
        
        private System.Windows.Forms.GroupBox grpProjectStructure;
        private System.Windows.Forms.CheckBox chkGenerateStartup;
        private System.Windows.Forms.CheckBox chkGenerateProgram;
        private System.Windows.Forms.CheckBox chkGenerateGitignore;
        private System.Windows.Forms.CheckBox chkGenerateReadme;
        private System.Windows.Forms.CheckBox chkGenerateSolution;
        
        // Tab 4: Settings
        private System.Windows.Forms.GroupBox grpProjectSettings;
        private System.Windows.Forms.GroupBox grpDatabaseInfo;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.Label lblConnectionInfo;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.Label lblOutputPath;
        
        // Tab 5: Output & Actions
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.GroupBox grpProgress;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkInfrastructureLayer;
    }
} 