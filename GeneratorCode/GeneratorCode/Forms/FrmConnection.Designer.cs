namespace GeneratorCode.Forms
{
    partial class FrmConnection
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
            lblDatabaseType = new System.Windows.Forms.Label();
            cmbDatabaseType = new System.Windows.Forms.ComboBox();
            lblServer = new System.Windows.Forms.Label();
            cmbServer = new System.Windows.Forms.ComboBox();
            lblUsername = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            lblPassword = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            lblDatabase = new System.Windows.Forms.Label();
            cmbDatabase = new System.Windows.Forms.ComboBox();
            btnTestConnection = new System.Windows.Forms.Button();
            btnConnect = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblPort = new System.Windows.Forms.Label();
            txtPort = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Location = new System.Drawing.Point(400, 20);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblDatabaseType.Size = new System.Drawing.Size(122, 20);
            lblDatabaseType.TabIndex = 0;
            lblDatabaseType.Text = "نوع قاعدة البيانات";
            // 
            // cmbDatabaseType
            // 
            cmbDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbDatabaseType.FormattingEnabled = true;
            cmbDatabaseType.Location = new System.Drawing.Point(20, 20);
            cmbDatabaseType.Name = "cmbDatabaseType";
            cmbDatabaseType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            cmbDatabaseType.Size = new System.Drawing.Size(360, 28);
            cmbDatabaseType.TabIndex = 1;
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Location = new System.Drawing.Point(400, 70);
            lblServer.Name = "lblServer";
            lblServer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblServer.Size = new System.Drawing.Size(55, 20);
            lblServer.TabIndex = 2;
            lblServer.Text = "السيرفر";
            // 
            // cmbServer
            // 
            cmbServer.FormattingEnabled = true;
            cmbServer.Location = new System.Drawing.Point(20, 70);
            cmbServer.Name = "cmbServer";
            cmbServer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            cmbServer.Size = new System.Drawing.Size(360, 28);
            cmbServer.TabIndex = 3;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new System.Drawing.Point(400, 120);
            lblUsername.Name = "lblUsername";
            lblUsername.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblUsername.Size = new System.Drawing.Size(101, 20);
            lblUsername.TabIndex = 4;
            lblUsername.Text = "اسم المستخدم";
            // 
            // txtUsername
            // 
            txtUsername.Location = new System.Drawing.Point(20, 120);
            txtUsername.Name = "txtUsername";
            txtUsername.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            txtUsername.Size = new System.Drawing.Size(360, 27);
            txtUsername.TabIndex = 5;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(400, 170);
            lblPassword.Name = "lblPassword";
            lblPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblPassword.Size = new System.Drawing.Size(80, 20);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "كلمة المرور";
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(20, 170);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            txtPassword.Size = new System.Drawing.Size(360, 27);
            txtPassword.TabIndex = 7;
            // 
            // lblDatabase
            // 
            lblDatabase.AutoSize = true;
            lblDatabase.Location = new System.Drawing.Point(400, 220);
            lblDatabase.Name = "lblDatabase";
            lblDatabase.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblDatabase.Size = new System.Drawing.Size(96, 20);
            lblDatabase.TabIndex = 8;
            lblDatabase.Text = "قاعدة البيانات";
            // 
            // cmbDatabase
            // 
            cmbDatabase.FormattingEnabled = true;
            cmbDatabase.Location = new System.Drawing.Point(20, 220);
            cmbDatabase.Name = "cmbDatabase";
            cmbDatabase.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            cmbDatabase.Size = new System.Drawing.Size(360, 28);
            cmbDatabase.TabIndex = 9;
            // 
            // btnTestConnection
            // 
            btnTestConnection.Location = new System.Drawing.Point(280, 270);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new System.Drawing.Size(150, 35);
            btnTestConnection.TabIndex = 10;
            btnTestConnection.Text = "اختبار الاتصال";
            btnTestConnection.UseVisualStyleBackColor = true;
            // 
            // btnConnect
            // 
            btnConnect.Location = new System.Drawing.Point(125, 270);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(150, 35);
            btnConnect.TabIndex = 11;
            btnConnect.Text = "اتصال";
            btnConnect.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(20, 270);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(100, 35);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "إلغاء";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(20, 320);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(500, 10);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            progressBar.TabIndex = 0;
            progressBar.Visible = false;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new System.Drawing.Point(400, 95);
            lblPort.Name = "lblPort";
            lblPort.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblPort.Size = new System.Drawing.Size(50, 20);
            lblPort.TabIndex = 13;
            lblPort.Text = "البورت";
            lblPort.Visible = false;
            // 
            // txtPort
            // 
            txtPort.Location = new System.Drawing.Point(20, 95);
            txtPort.Name = "txtPort";
            txtPort.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            txtPort.Size = new System.Drawing.Size(360, 27);
            txtPort.TabIndex = 14;
            txtPort.Text = "5432";
            txtPort.Visible = false;
            // 
            // FrmConnection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(540, 350);
            Controls.Add(txtPort);
            Controls.Add(lblPort);
            Controls.Add(progressBar);
            Controls.Add(btnCancel);
            Controls.Add(btnConnect);
            Controls.Add(btnTestConnection);
            Controls.Add(cmbDatabase);
            Controls.Add(lblDatabase);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(lblUsername);
            Controls.Add(cmbServer);
            Controls.Add(lblServer);
            Controls.Add(cmbDatabaseType);
            Controls.Add(lblDatabaseType);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmConnection";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "اتصال بقاعدة البيانات";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cmbDatabaseType;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ComboBox cmbDatabase;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
    }
} 