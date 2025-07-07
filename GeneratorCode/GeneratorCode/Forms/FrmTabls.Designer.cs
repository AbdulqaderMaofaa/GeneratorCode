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
            lstTables = new System.Windows.Forms.ListBox();
            gridColumns = new System.Windows.Forms.DataGridView();
            cmbArchitecture = new System.Windows.Forms.ComboBox();
            cmbLanguage = new System.Windows.Forms.ComboBox();
            txtNamespace = new System.Windows.Forms.TextBox();
            txtOutputPath = new System.Windows.Forms.TextBox();
            btnGenerate = new System.Windows.Forms.Button();
            btnPreview = new System.Windows.Forms.Button();
            btnBrowse = new System.Windows.Forms.Button();
            btnSettings = new System.Windows.Forms.Button();
            chkEntities = new System.Windows.Forms.CheckBox();
            chkDTOs = new System.Windows.Forms.CheckBox();
            chkRepositories = new System.Windows.Forms.CheckBox();
            chkServices = new System.Windows.Forms.CheckBox();
            chkControllers = new System.Windows.Forms.CheckBox();
            chkUnitTests = new System.Windows.Forms.CheckBox();
            chkValidation = new System.Windows.Forms.CheckBox();
            chkSwagger = new System.Windows.Forms.CheckBox();
            chkDependencyInjection = new System.Windows.Forms.CheckBox();
            chkGenerateCreate = new System.Windows.Forms.CheckBox();
            chkGenerateRead = new System.Windows.Forms.CheckBox();
            chkGenerateUpdate = new System.Windows.Forms.CheckBox();
            chkGenerateDelete = new System.Windows.Forms.CheckBox();
            lblDatabaseType = new System.Windows.Forms.Label();
            lblConnectionInfo = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblDIInfo = new System.Windows.Forms.Label();
            lblValidationInfo = new System.Windows.Forms.Label();
            lblTestingInfo = new System.Windows.Forms.Label();
            grpProjectStructure = new System.Windows.Forms.GroupBox();
            chkGenerateStartup = new System.Windows.Forms.CheckBox();
            chkGenerateProgram = new System.Windows.Forms.CheckBox();
            chkGenerateGitignore = new System.Windows.Forms.CheckBox();
            chkGenerateReadme = new System.Windows.Forms.CheckBox();
            chkGenerateSolution = new System.Windows.Forms.CheckBox();
            grpArchitecture = new System.Windows.Forms.GroupBox();
            chkInfrastructureLayer = new System.Windows.Forms.CheckBox();
            chkApplicationLayer = new System.Windows.Forms.CheckBox();
            chkDomainLayer = new System.Windows.Forms.CheckBox();
            chkPresentationLayer = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)gridColumns).BeginInit();
            grpProjectStructure.SuspendLayout();
            grpArchitecture.SuspendLayout();
            SuspendLayout();
            // 
            // lstTables
            // 
            lstTables.FormattingEnabled = true;
            lstTables.ItemHeight = 20;
            lstTables.Location = new System.Drawing.Point(16, 63);
            lstTables.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            lstTables.Name = "lstTables";
            lstTables.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            lstTables.Size = new System.Drawing.Size(265, 644);
            lstTables.TabIndex = 0;
            // 
            // gridColumns
            // 
            gridColumns.AllowUserToAddRows = false;
            gridColumns.AllowUserToDeleteRows = false;
            gridColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridColumns.Location = new System.Drawing.Point(291, 63);
            gridColumns.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            gridColumns.Name = "gridColumns";
            gridColumns.ReadOnly = true;
            gridColumns.RowHeadersWidth = 51;
            gridColumns.Size = new System.Drawing.Size(760, 646);
            gridColumns.TabIndex = 1;
            // 
            // cmbArchitecture
            // 
            cmbArchitecture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbArchitecture.FormattingEnabled = true;
            cmbArchitecture.Location = new System.Drawing.Point(16, 718);
            cmbArchitecture.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cmbArchitecture.Name = "cmbArchitecture";
            cmbArchitecture.Size = new System.Drawing.Size(265, 28);
            cmbArchitecture.TabIndex = 2;
            // 
            // cmbLanguage
            // 
            cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbLanguage.FormattingEnabled = true;
            cmbLanguage.Location = new System.Drawing.Point(291, 718);
            cmbLanguage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cmbLanguage.Name = "cmbLanguage";
            cmbLanguage.Size = new System.Drawing.Size(265, 28);
            cmbLanguage.TabIndex = 3;
            // 
            // txtNamespace
            // 
            txtNamespace.Location = new System.Drawing.Point(16, 760);
            txtNamespace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtNamespace.Name = "txtNamespace";
            txtNamespace.Size = new System.Drawing.Size(265, 27);
            txtNamespace.TabIndex = 4;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Location = new System.Drawing.Point(291, 760);
            txtOutputPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(651, 27);
            txtOutputPath.TabIndex = 5;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new System.Drawing.Point(951, 718);
            btnGenerate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new System.Drawing.Size(100, 35);
            btnGenerate.TabIndex = 6;
            btnGenerate.Text = "توليد";
            btnGenerate.UseVisualStyleBackColor = true;
            // 
            // btnPreview
            // 
            btnPreview.Location = new System.Drawing.Point(843, 718);
            btnPreview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new System.Drawing.Size(100, 35);
            btnPreview.TabIndex = 7;
            btnPreview.Text = "معاينة";
            btnPreview.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(951, 757);
            btnBrowse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(100, 35);
            btnBrowse.TabIndex = 8;
            btnBrowse.Text = "استعراض";
            btnBrowse.UseVisualStyleBackColor = true;
            // 
            // btnSettings
            // 
            btnSettings.Location = new System.Drawing.Point(735, 718);
            btnSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(100, 35);
            btnSettings.TabIndex = 9;
            btnSettings.Text = "إعدادات";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Visible = false;
            // 
            // chkEntities
            // 
            chkEntities.AutoSize = true;
            chkEntities.Location = new System.Drawing.Point(16, 800);
            chkEntities.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkEntities.Name = "chkEntities";
            chkEntities.Size = new System.Drawing.Size(79, 24);
            chkEntities.TabIndex = 10;
            chkEntities.Text = "Entities";
            chkEntities.UseVisualStyleBackColor = true;
            // 
            // chkDTOs
            // 
            chkDTOs.AutoSize = true;
            chkDTOs.Location = new System.Drawing.Point(107, 800);
            chkDTOs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkDTOs.Name = "chkDTOs";
            chkDTOs.Size = new System.Drawing.Size(65, 24);
            chkDTOs.TabIndex = 11;
            chkDTOs.Text = "DTOs";
            chkDTOs.UseVisualStyleBackColor = true;
            // 
            // chkRepositories
            // 
            chkRepositories.AutoSize = true;
            chkRepositories.Location = new System.Drawing.Point(185, 800);
            chkRepositories.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkRepositories.Name = "chkRepositories";
            chkRepositories.Size = new System.Drawing.Size(113, 24);
            chkRepositories.TabIndex = 12;
            chkRepositories.Text = "Repositories";
            chkRepositories.UseVisualStyleBackColor = true;
            // 
            // chkServices
            // 
            chkServices.AutoSize = true;
            chkServices.Location = new System.Drawing.Point(307, 800);
            chkServices.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkServices.Name = "chkServices";
            chkServices.Size = new System.Drawing.Size(84, 24);
            chkServices.TabIndex = 13;
            chkServices.Text = "Services";
            chkServices.UseVisualStyleBackColor = true;
            // 
            // chkControllers
            // 
            chkControllers.AutoSize = true;
            chkControllers.Location = new System.Drawing.Point(404, 800);
            chkControllers.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkControllers.Name = "chkControllers";
            chkControllers.Size = new System.Drawing.Size(103, 24);
            chkControllers.TabIndex = 14;
            chkControllers.Text = "Controllers";
            chkControllers.UseVisualStyleBackColor = true;
            // 
            // chkUnitTests
            // 
            chkUnitTests.AutoSize = true;
            chkUnitTests.Location = new System.Drawing.Point(513, 800);
            chkUnitTests.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkUnitTests.Name = "chkUnitTests";
            chkUnitTests.Size = new System.Drawing.Size(94, 24);
            chkUnitTests.TabIndex = 15;
            chkUnitTests.Text = "Unit Tests";
            chkUnitTests.UseVisualStyleBackColor = true;
            // 
            // chkValidation
            // 
            chkValidation.AutoSize = true;
            chkValidation.Location = new System.Drawing.Point(624, 800);
            chkValidation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkValidation.Name = "chkValidation";
            chkValidation.Size = new System.Drawing.Size(98, 24);
            chkValidation.TabIndex = 16;
            chkValidation.Text = "Validation";
            chkValidation.UseVisualStyleBackColor = true;
            // 
            // chkSwagger
            // 
            chkSwagger.AutoSize = true;
            chkSwagger.Location = new System.Drawing.Point(731, 800);
            chkSwagger.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkSwagger.Name = "chkSwagger";
            chkSwagger.Size = new System.Drawing.Size(89, 24);
            chkSwagger.TabIndex = 17;
            chkSwagger.Text = "Swagger";
            chkSwagger.UseVisualStyleBackColor = true;
            // 
            // chkDependencyInjection
            // 
            chkDependencyInjection.AutoSize = true;
            chkDependencyInjection.Location = new System.Drawing.Point(832, 800);
            chkDependencyInjection.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkDependencyInjection.Name = "chkDependencyInjection";
            chkDependencyInjection.Size = new System.Drawing.Size(175, 24);
            chkDependencyInjection.TabIndex = 18;
            chkDependencyInjection.Text = "Dependency Injection";
            chkDependencyInjection.UseVisualStyleBackColor = true;
            // 
            // chkGenerateCreate
            // 
            chkGenerateCreate.AutoSize = true;
            chkGenerateCreate.Location = new System.Drawing.Point(16, 835);
            chkGenerateCreate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkGenerateCreate.Name = "chkGenerateCreate";
            chkGenerateCreate.Size = new System.Drawing.Size(74, 24);
            chkGenerateCreate.TabIndex = 19;
            chkGenerateCreate.Text = "Create";
            chkGenerateCreate.UseVisualStyleBackColor = true;
            // 
            // chkGenerateRead
            // 
            chkGenerateRead.AutoSize = true;
            chkGenerateRead.Location = new System.Drawing.Point(100, 835);
            chkGenerateRead.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkGenerateRead.Name = "chkGenerateRead";
            chkGenerateRead.Size = new System.Drawing.Size(65, 24);
            chkGenerateRead.TabIndex = 20;
            chkGenerateRead.Text = "Read";
            chkGenerateRead.UseVisualStyleBackColor = true;
            // 
            // chkGenerateUpdate
            // 
            chkGenerateUpdate.AutoSize = true;
            chkGenerateUpdate.Location = new System.Drawing.Point(177, 835);
            chkGenerateUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkGenerateUpdate.Name = "chkGenerateUpdate";
            chkGenerateUpdate.Size = new System.Drawing.Size(80, 24);
            chkGenerateUpdate.TabIndex = 21;
            chkGenerateUpdate.Text = "Update";
            chkGenerateUpdate.UseVisualStyleBackColor = true;
            // 
            // chkGenerateDelete
            // 
            chkGenerateDelete.AutoSize = true;
            chkGenerateDelete.Location = new System.Drawing.Point(267, 835);
            chkGenerateDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkGenerateDelete.Name = "chkGenerateDelete";
            chkGenerateDelete.Size = new System.Drawing.Size(75, 24);
            chkGenerateDelete.TabIndex = 22;
            chkGenerateDelete.Text = "Delete";
            chkGenerateDelete.UseVisualStyleBackColor = true;
            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Location = new System.Drawing.Point(16, 14);
            lblDatabaseType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Size = new System.Drawing.Size(50, 20);
            lblDatabaseType.TabIndex = 23;
            lblDatabaseType.Text = "label1";
            // 
            // lblConnectionInfo
            // 
            lblConnectionInfo.AutoSize = true;
            lblConnectionInfo.Location = new System.Drawing.Point(287, 14);
            lblConnectionInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblConnectionInfo.Name = "lblConnectionInfo";
            lblConnectionInfo.Size = new System.Drawing.Size(50, 20);
            lblConnectionInfo.TabIndex = 24;
            lblConnectionInfo.Text = "label2";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new System.Drawing.Point(947, 839);
            lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(50, 20);
            lblStatus.TabIndex = 25;
            lblStatus.Text = "label3";
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(237, 1008);
            progressBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(760, 20);
            progressBar.TabIndex = 26;
            // 
            // lblDIInfo
            // 
            lblDIInfo.AutoSize = true;
            lblDIInfo.Location = new System.Drawing.Point(828, 837);
            lblDIInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDIInfo.Name = "lblDIInfo";
            lblDIInfo.Size = new System.Drawing.Size(50, 20);
            lblDIInfo.TabIndex = 27;
            lblDIInfo.Text = "label4";
            // 
            // lblValidationInfo
            // 
            lblValidationInfo.AutoSize = true;
            lblValidationInfo.Location = new System.Drawing.Point(620, 837);
            lblValidationInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblValidationInfo.Name = "lblValidationInfo";
            lblValidationInfo.Size = new System.Drawing.Size(50, 20);
            lblValidationInfo.TabIndex = 28;
            lblValidationInfo.Text = "label5";
            // 
            // lblTestingInfo
            // 
            lblTestingInfo.AutoSize = true;
            lblTestingInfo.Location = new System.Drawing.Point(509, 837);
            lblTestingInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTestingInfo.Name = "lblTestingInfo";
            lblTestingInfo.Size = new System.Drawing.Size(50, 20);
            lblTestingInfo.TabIndex = 29;
            lblTestingInfo.Text = "label6";
            // 
            // grpProjectStructure
            // 
            grpProjectStructure.Controls.Add(chkGenerateStartup);
            grpProjectStructure.Controls.Add(chkGenerateProgram);
            grpProjectStructure.Controls.Add(chkGenerateGitignore);
            grpProjectStructure.Controls.Add(chkGenerateReadme);
            grpProjectStructure.Controls.Add(chkGenerateSolution);
            grpProjectStructure.Location = new System.Drawing.Point(73, 903);
            grpProjectStructure.Name = "grpProjectStructure";
            grpProjectStructure.Size = new System.Drawing.Size(400, 100);
            grpProjectStructure.TabIndex = 0;
            grpProjectStructure.TabStop = false;
            grpProjectStructure.Text = "هيكل المشروع";
            // 
            // chkGenerateStartup
            // 
            chkGenerateStartup.Checked = true;
            chkGenerateStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            chkGenerateStartup.Location = new System.Drawing.Point(10, 20);
            chkGenerateStartup.Name = "chkGenerateStartup";
            chkGenerateStartup.Size = new System.Drawing.Size(104, 24);
            chkGenerateStartup.TabIndex = 0;
            chkGenerateStartup.Text = "Startup.cs";
            // 
            // chkGenerateProgram
            // 
            chkGenerateProgram.Checked = true;
            chkGenerateProgram.CheckState = System.Windows.Forms.CheckState.Checked;
            chkGenerateProgram.Location = new System.Drawing.Point(10, 40);
            chkGenerateProgram.Name = "chkGenerateProgram";
            chkGenerateProgram.Size = new System.Drawing.Size(104, 24);
            chkGenerateProgram.TabIndex = 1;
            chkGenerateProgram.Text = "Program.cs";
            // 
            // chkGenerateGitignore
            // 
            chkGenerateGitignore.Checked = true;
            chkGenerateGitignore.CheckState = System.Windows.Forms.CheckState.Checked;
            chkGenerateGitignore.Location = new System.Drawing.Point(10, 60);
            chkGenerateGitignore.Name = "chkGenerateGitignore";
            chkGenerateGitignore.Size = new System.Drawing.Size(104, 24);
            chkGenerateGitignore.TabIndex = 2;
            chkGenerateGitignore.Text = ".gitignore";
            // 
            // chkGenerateReadme
            // 
            chkGenerateReadme.Checked = true;
            chkGenerateReadme.CheckState = System.Windows.Forms.CheckState.Checked;
            chkGenerateReadme.Location = new System.Drawing.Point(150, 20);
            chkGenerateReadme.Name = "chkGenerateReadme";
            chkGenerateReadme.Size = new System.Drawing.Size(104, 24);
            chkGenerateReadme.TabIndex = 3;
            chkGenerateReadme.Text = "README.md";
            // 
            // chkGenerateSolution
            // 
            chkGenerateSolution.Checked = true;
            chkGenerateSolution.CheckState = System.Windows.Forms.CheckState.Checked;
            chkGenerateSolution.Location = new System.Drawing.Point(150, 40);
            chkGenerateSolution.Name = "chkGenerateSolution";
            chkGenerateSolution.Size = new System.Drawing.Size(104, 24);
            chkGenerateSolution.TabIndex = 4;
            chkGenerateSolution.Text = "Solution File";
            // 
            // grpArchitecture
            // 
            grpArchitecture.Controls.Add(chkInfrastructureLayer);
            grpArchitecture.Controls.Add(chkApplicationLayer);
            grpArchitecture.Controls.Add(chkDomainLayer);
            grpArchitecture.Controls.Add(chkPresentationLayer);
            grpArchitecture.Location = new System.Drawing.Point(607, 903);
            grpArchitecture.Name = "grpArchitecture";
            grpArchitecture.Size = new System.Drawing.Size(400, 100);
            grpArchitecture.TabIndex = 1;
            grpArchitecture.TabStop = false;
            grpArchitecture.Text = "الطبقات المعمارية";
            // 
            // chkInfrastructureLayer
            // 
            chkInfrastructureLayer.Checked = true;
            chkInfrastructureLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            chkInfrastructureLayer.Location = new System.Drawing.Point(10, 20);
            chkInfrastructureLayer.Name = "chkInfrastructureLayer";
            chkInfrastructureLayer.Size = new System.Drawing.Size(104, 24);
            chkInfrastructureLayer.TabIndex = 0;
            chkInfrastructureLayer.Text = "Infrastructure";
            // 
            // chkApplicationLayer
            // 
            chkApplicationLayer.Checked = true;
            chkApplicationLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            chkApplicationLayer.Location = new System.Drawing.Point(10, 40);
            chkApplicationLayer.Name = "chkApplicationLayer";
            chkApplicationLayer.Size = new System.Drawing.Size(104, 24);
            chkApplicationLayer.TabIndex = 1;
            chkApplicationLayer.Text = "Application";
            // 
            // chkDomainLayer
            // 
            chkDomainLayer.Checked = true;
            chkDomainLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            chkDomainLayer.Location = new System.Drawing.Point(150, 20);
            chkDomainLayer.Name = "chkDomainLayer";
            chkDomainLayer.Size = new System.Drawing.Size(104, 24);
            chkDomainLayer.TabIndex = 2;
            chkDomainLayer.Text = "Domain";
            // 
            // chkPresentationLayer
            // 
            chkPresentationLayer.Checked = true;
            chkPresentationLayer.CheckState = System.Windows.Forms.CheckState.Checked;
            chkPresentationLayer.Location = new System.Drawing.Point(150, 40);
            chkPresentationLayer.Name = "chkPresentationLayer";
            chkPresentationLayer.Size = new System.Drawing.Size(104, 24);
            chkPresentationLayer.TabIndex = 3;
            chkPresentationLayer.Text = "Presentation";
            // 
            // FrmTabls
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1067, 1055);
            Controls.Add(grpProjectStructure);
            Controls.Add(grpArchitecture);
            Controls.Add(lblTestingInfo);
            Controls.Add(lblValidationInfo);
            Controls.Add(progressBar);
            Controls.Add(lblDIInfo);
            Controls.Add(lblStatus);
            Controls.Add(lblConnectionInfo);
            Controls.Add(lblDatabaseType);
            Controls.Add(chkGenerateDelete);
            Controls.Add(chkGenerateUpdate);
            Controls.Add(chkGenerateRead);
            Controls.Add(chkGenerateCreate);
            Controls.Add(chkDependencyInjection);
            Controls.Add(chkSwagger);
            Controls.Add(chkValidation);
            Controls.Add(chkUnitTests);
            Controls.Add(chkControllers);
            Controls.Add(chkServices);
            Controls.Add(chkRepositories);
            Controls.Add(chkDTOs);
            Controls.Add(chkEntities);
            Controls.Add(btnSettings);
            Controls.Add(btnBrowse);
            Controls.Add(btnPreview);
            Controls.Add(btnGenerate);
            Controls.Add(txtOutputPath);
            Controls.Add(txtNamespace);
            Controls.Add(cmbLanguage);
            Controls.Add(cmbArchitecture);
            Controls.Add(gridColumns);
            Controls.Add(lstTables);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "FrmTabls";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "توليد الكود";
            ((System.ComponentModel.ISupportInitialize)gridColumns).EndInit();
            grpProjectStructure.ResumeLayout(false);
            grpArchitecture.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstTables;
        private System.Windows.Forms.DataGridView gridColumns;
        private System.Windows.Forms.ComboBox cmbArchitecture;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.CheckBox chkEntities;
        private System.Windows.Forms.CheckBox chkDTOs;
        private System.Windows.Forms.CheckBox chkRepositories;
        private System.Windows.Forms.CheckBox chkServices;
        private System.Windows.Forms.CheckBox chkControllers;
        private System.Windows.Forms.CheckBox chkUnitTests;
        private System.Windows.Forms.CheckBox chkValidation;
        private System.Windows.Forms.CheckBox chkSwagger;
        private System.Windows.Forms.CheckBox chkDependencyInjection;
        private System.Windows.Forms.CheckBox chkGenerateCreate;
        private System.Windows.Forms.CheckBox chkGenerateRead;
        private System.Windows.Forms.CheckBox chkGenerateUpdate;
        private System.Windows.Forms.CheckBox chkGenerateDelete;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.Label lblConnectionInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblDIInfo;
        private System.Windows.Forms.Label lblValidationInfo;
        private System.Windows.Forms.Label lblTestingInfo;
        private System.Windows.Forms.GroupBox grpProjectStructure;
        private System.Windows.Forms.CheckBox chkGenerateStartup;
        private System.Windows.Forms.CheckBox chkGenerateProgram;
        private System.Windows.Forms.CheckBox chkGenerateGitignore;
        private System.Windows.Forms.CheckBox chkGenerateReadme;
        private System.Windows.Forms.CheckBox chkGenerateSolution;
        private System.Windows.Forms.GroupBox grpArchitecture;
        private System.Windows.Forms.CheckBox chkInfrastructureLayer;
        private System.Windows.Forms.CheckBox chkApplicationLayer;
        private System.Windows.Forms.CheckBox chkDomainLayer;
        private System.Windows.Forms.CheckBox chkPresentationLayer;
    }
} 