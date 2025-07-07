namespace GeneratorCode.Forms
{
    partial class FrmSettings
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
            chkEnableDI = new System.Windows.Forms.CheckBox();
            chkEnableValidation = new System.Windows.Forms.CheckBox();
            chkEnableTesting = new System.Windows.Forms.CheckBox();
            txtDefaultNamespace = new System.Windows.Forms.TextBox();
            txtOutputPath = new System.Windows.Forms.TextBox();
            btnBrowse = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            lblNamespace = new System.Windows.Forms.Label();
            lblOutputPath = new System.Windows.Forms.Label();
            grpPostgres = new System.Windows.Forms.GroupBox();
            txtPostgresUsername = new System.Windows.Forms.TextBox();
            lblPostgresPassword = new System.Windows.Forms.Label();
            txtPostgresPassword = new System.Windows.Forms.TextBox();
            lblPostgresPort = new System.Windows.Forms.Label();
            txtPostgresPort = new System.Windows.Forms.TextBox();
            grpSqlServer = new System.Windows.Forms.GroupBox();
            lblSqlServerUsername = new System.Windows.Forms.Label();
            txtSqlServerUsername = new System.Windows.Forms.TextBox();
            lblSqlServerPassword = new System.Windows.Forms.Label();
            txtSqlServerPassword = new System.Windows.Forms.TextBox();
            grpMySql = new System.Windows.Forms.GroupBox();
            lblMySqlUsername = new System.Windows.Forms.Label();
            txtMySqlUsername = new System.Windows.Forms.TextBox();
            lblMySqlPassword = new System.Windows.Forms.Label();
            txtMySqlPassword = new System.Windows.Forms.TextBox();
            lblPostgresUsername = new System.Windows.Forms.Label();
            grpPostgres.SuspendLayout();
            grpSqlServer.SuspendLayout();
            grpMySql.SuspendLayout();
            SuspendLayout();
            // 
            // chkEnableDI
            // 
            chkEnableDI.AutoSize = true;
            chkEnableDI.Location = new System.Drawing.Point(16, 18);
            chkEnableDI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkEnableDI.Name = "chkEnableDI";
            chkEnableDI.Size = new System.Drawing.Size(216, 24);
            chkEnableDI.TabIndex = 0;
            chkEnableDI.Text = "تفعيل Dependency Injection";
            chkEnableDI.UseVisualStyleBackColor = true;
            // 
            // chkEnableValidation
            // 
            chkEnableValidation.AutoSize = true;
            chkEnableValidation.Location = new System.Drawing.Point(16, 54);
            chkEnableValidation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkEnableValidation.Name = "chkEnableValidation";
            chkEnableValidation.Size = new System.Drawing.Size(114, 24);
            chkEnableValidation.TabIndex = 1;
            chkEnableValidation.Text = "تفعيل التحقق";
            chkEnableValidation.UseVisualStyleBackColor = true;
            // 
            // chkEnableTesting
            // 
            chkEnableTesting.AutoSize = true;
            chkEnableTesting.Location = new System.Drawing.Point(16, 89);
            chkEnableTesting.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            chkEnableTesting.Name = "chkEnableTesting";
            chkEnableTesting.Size = new System.Drawing.Size(131, 24);
            chkEnableTesting.TabIndex = 2;
            chkEnableTesting.Text = "تفعيل الاختبارات";
            chkEnableTesting.UseVisualStyleBackColor = true;
            // 
            // txtDefaultNamespace
            // 
            txtDefaultNamespace.Location = new System.Drawing.Point(16, 160);
            txtDefaultNamespace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtDefaultNamespace.Name = "txtDefaultNamespace";
            txtDefaultNamespace.Size = new System.Drawing.Size(479, 27);
            txtDefaultNamespace.TabIndex = 3;
            // 
            // txtOutputPath
            // 
            txtOutputPath.Location = new System.Drawing.Point(16, 220);
            txtOutputPath.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(371, 27);
            txtOutputPath.TabIndex = 4;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(396, 217);
            btnBrowse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(100, 35);
            btnBrowse.TabIndex = 5;
            btnBrowse.Text = "استعراض";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(287, 785);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(100, 35);
            btnSave.TabIndex = 6;
            btnSave.Text = "حفظ";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(395, 785);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 35);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "إلغاء";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblNamespace
            // 
            lblNamespace.AutoSize = true;
            lblNamespace.Location = new System.Drawing.Point(16, 135);
            lblNamespace.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNamespace.Name = "lblNamespace";
            lblNamespace.Size = new System.Drawing.Size(146, 20);
            lblNamespace.TabIndex = 8;
            lblNamespace.Text = "النطاق (Namespace):";
            // 
            // lblOutputPath
            // 
            lblOutputPath.AutoSize = true;
            lblOutputPath.Location = new System.Drawing.Point(16, 195);
            lblOutputPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new System.Drawing.Size(97, 20);
            lblOutputPath.TabIndex = 9;
            lblOutputPath.Text = "مسار الملفات:";
            // 
            // grpPostgres
            // 
            grpPostgres.Controls.Add(lblPostgresUsername);
            grpPostgres.Controls.Add(txtPostgresUsername);
            grpPostgres.Controls.Add(lblPostgresPassword);
            grpPostgres.Controls.Add(txtPostgresPassword);
            grpPostgres.Controls.Add(lblPostgresPort);
            grpPostgres.Controls.Add(txtPostgresPort);
            grpPostgres.Location = new System.Drawing.Point(16, 262);
            grpPostgres.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpPostgres.Name = "grpPostgres";
            grpPostgres.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpPostgres.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpPostgres.Size = new System.Drawing.Size(480, 154);
            grpPostgres.TabIndex = 10;
            grpPostgres.TabStop = false;
            grpPostgres.Text = "PostgreSQL";
            // 
            // txtPostgresUsername
            // 
            txtPostgresUsername.Location = new System.Drawing.Point(5, 30);
            txtPostgresUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtPostgresUsername.Name = "txtPostgresUsername";
            txtPostgresUsername.Size = new System.Drawing.Size(321, 27);
            txtPostgresUsername.TabIndex = 1;
            // 
            // lblPostgresPassword
            // 
            lblPostgresPassword.Location = new System.Drawing.Point(334, 67);
            lblPostgresPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPostgresPassword.Name = "lblPostgresPassword";
            lblPostgresPassword.Size = new System.Drawing.Size(133, 31);
            lblPostgresPassword.TabIndex = 2;
            lblPostgresPassword.Text = "كلمة المرور:";
            // 
            // txtPostgresPassword
            // 
            txtPostgresPassword.Location = new System.Drawing.Point(5, 69);
            txtPostgresPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtPostgresPassword.Name = "txtPostgresPassword";
            txtPostgresPassword.Size = new System.Drawing.Size(321, 27);
            txtPostgresPassword.TabIndex = 3;
            txtPostgresPassword.UseSystemPasswordChar = true;
            // 
            // lblPostgresPort
            // 
            lblPostgresPort.Location = new System.Drawing.Point(334, 107);
            lblPostgresPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPostgresPort.Name = "lblPostgresPort";
            lblPostgresPort.Size = new System.Drawing.Size(133, 31);
            lblPostgresPort.TabIndex = 4;
            lblPostgresPort.Text = "المنفذ:";
            // 
            // txtPostgresPort
            // 
            txtPostgresPort.Location = new System.Drawing.Point(194, 109);
            txtPostgresPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtPostgresPort.Name = "txtPostgresPort";
            txtPostgresPort.Size = new System.Drawing.Size(132, 27);
            txtPostgresPort.TabIndex = 5;
            // 
            // grpSqlServer
            // 
            grpSqlServer.Controls.Add(lblSqlServerUsername);
            grpSqlServer.Controls.Add(txtSqlServerUsername);
            grpSqlServer.Controls.Add(lblSqlServerPassword);
            grpSqlServer.Controls.Add(txtSqlServerPassword);
            grpSqlServer.Location = new System.Drawing.Point(16, 427);
            grpSqlServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpSqlServer.Name = "grpSqlServer";
            grpSqlServer.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpSqlServer.Size = new System.Drawing.Size(480, 154);
            grpSqlServer.TabIndex = 11;
            grpSqlServer.TabStop = false;
            grpSqlServer.Text = "SQL Server";
            // 
            // lblSqlServerUsername
            // 
            lblSqlServerUsername.Location = new System.Drawing.Point(339, 52);
            lblSqlServerUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSqlServerUsername.Name = "lblSqlServerUsername";
            lblSqlServerUsername.Size = new System.Drawing.Size(133, 31);
            lblSqlServerUsername.TabIndex = 0;
            lblSqlServerUsername.Text = "اسم المستخدم:";
            // 
            // txtSqlServerUsername
            // 
            txtSqlServerUsername.Location = new System.Drawing.Point(10, 54);
            txtSqlServerUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtSqlServerUsername.Name = "txtSqlServerUsername";
            txtSqlServerUsername.Size = new System.Drawing.Size(321, 27);
            txtSqlServerUsername.TabIndex = 1;
            // 
            // lblSqlServerPassword
            // 
            lblSqlServerPassword.Location = new System.Drawing.Point(339, 104);
            lblSqlServerPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSqlServerPassword.Name = "lblSqlServerPassword";
            lblSqlServerPassword.Size = new System.Drawing.Size(133, 31);
            lblSqlServerPassword.TabIndex = 2;
            lblSqlServerPassword.Text = "كلمة المرور:";
            // 
            // txtSqlServerPassword
            // 
            txtSqlServerPassword.Location = new System.Drawing.Point(15, 106);
            txtSqlServerPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtSqlServerPassword.Name = "txtSqlServerPassword";
            txtSqlServerPassword.Size = new System.Drawing.Size(321, 27);
            txtSqlServerPassword.TabIndex = 3;
            txtSqlServerPassword.UseSystemPasswordChar = true;
            // 
            // grpMySql
            // 
            grpMySql.Controls.Add(lblMySqlUsername);
            grpMySql.Controls.Add(txtMySqlUsername);
            grpMySql.Controls.Add(lblMySqlPassword);
            grpMySql.Controls.Add(txtMySqlPassword);
            grpMySql.Location = new System.Drawing.Point(19, 604);
            grpMySql.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpMySql.Name = "grpMySql";
            grpMySql.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            grpMySql.Size = new System.Drawing.Size(480, 154);
            grpMySql.TabIndex = 12;
            grpMySql.TabStop = false;
            grpMySql.Text = "MySQL";
            // 
            // lblMySqlUsername
            // 
            lblMySqlUsername.Location = new System.Drawing.Point(337, 43);
            lblMySqlUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMySqlUsername.Name = "lblMySqlUsername";
            lblMySqlUsername.Size = new System.Drawing.Size(133, 31);
            lblMySqlUsername.TabIndex = 0;
            lblMySqlUsername.Text = "اسم المستخدم:";
            // 
            // txtMySqlUsername
            // 
            txtMySqlUsername.Location = new System.Drawing.Point(13, 45);
            txtMySqlUsername.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtMySqlUsername.Name = "txtMySqlUsername";
            txtMySqlUsername.Size = new System.Drawing.Size(321, 27);
            txtMySqlUsername.TabIndex = 1;
            // 
            // lblMySqlPassword
            // 
            lblMySqlPassword.Location = new System.Drawing.Point(337, 85);
            lblMySqlPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblMySqlPassword.Name = "lblMySqlPassword";
            lblMySqlPassword.Size = new System.Drawing.Size(133, 31);
            lblMySqlPassword.TabIndex = 2;
            lblMySqlPassword.Text = "كلمة المرور:";
            // 
            // txtMySqlPassword
            // 
            txtMySqlPassword.Location = new System.Drawing.Point(13, 87);
            txtMySqlPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtMySqlPassword.Name = "txtMySqlPassword";
            txtMySqlPassword.Size = new System.Drawing.Size(321, 27);
            txtMySqlPassword.TabIndex = 3;
            txtMySqlPassword.UseSystemPasswordChar = true;
            // 
            // lblPostgresUsername
            // 
            lblPostgresUsername.Location = new System.Drawing.Point(334, 28);
            lblPostgresUsername.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPostgresUsername.Name = "lblPostgresUsername";
            lblPostgresUsername.Size = new System.Drawing.Size(133, 31);
            lblPostgresUsername.TabIndex = 6;
            lblPostgresUsername.Text = "اسم المستخدم:";
            // 
            // FrmSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(512, 846);
            Controls.Add(grpMySql);
            Controls.Add(grpSqlServer);
            Controls.Add(grpPostgres);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(btnBrowse);
            Controls.Add(txtOutputPath);
            Controls.Add(txtDefaultNamespace);
            Controls.Add(chkEnableTesting);
            Controls.Add(chkEnableValidation);
            Controls.Add(chkEnableDI);
            Controls.Add(lblOutputPath);
            Controls.Add(lblNamespace);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmSettings";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "الإعدادات";
            grpPostgres.ResumeLayout(false);
            grpPostgres.PerformLayout();
            grpSqlServer.ResumeLayout(false);
            grpSqlServer.PerformLayout();
            grpMySql.ResumeLayout(false);
            grpMySql.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnableDI;
        private System.Windows.Forms.CheckBox chkEnableValidation;
        private System.Windows.Forms.CheckBox chkEnableTesting;
        private System.Windows.Forms.TextBox txtDefaultNamespace;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.GroupBox grpPostgres;
        private System.Windows.Forms.TextBox txtPostgresUsername;
        private System.Windows.Forms.Label lblPostgresPassword;
        private System.Windows.Forms.TextBox txtPostgresPassword;
        private System.Windows.Forms.Label lblPostgresPort;
        private System.Windows.Forms.TextBox txtPostgresPort;
        private System.Windows.Forms.GroupBox grpSqlServer;
        private System.Windows.Forms.Label lblSqlServerUsername;
        private System.Windows.Forms.TextBox txtSqlServerUsername;
        private System.Windows.Forms.Label lblSqlServerPassword;
        private System.Windows.Forms.TextBox txtSqlServerPassword;

        private System.Windows.Forms.GroupBox grpMySql;
        private System.Windows.Forms.Label lblMySqlUsername;
        private System.Windows.Forms.TextBox txtMySqlUsername;
        private System.Windows.Forms.Label lblMySqlPassword;
        private System.Windows.Forms.TextBox txtMySqlPassword;
        private System.Windows.Forms.Label lblPostgresUsername;
    }
} 