using Helpers = GeneratorCode.Helpers;

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
            // Main Groups
            grpDatabaseType = new System.Windows.Forms.GroupBox();
            grpConnectionDetails = new System.Windows.Forms.GroupBox();
            grpAuthentication = new System.Windows.Forms.GroupBox();
            grpActions = new System.Windows.Forms.GroupBox();
            
            // Database Type Group Controls
            lblDatabaseType = new System.Windows.Forms.Label();
            cmbDatabaseType = new System.Windows.Forms.ComboBox();
            picDatabaseIcon = new System.Windows.Forms.PictureBox();
            
            // Connection Details Group Controls
            lblServer = new System.Windows.Forms.Label();
            cmbServer = new System.Windows.Forms.ComboBox();
            lblPort = new System.Windows.Forms.Label();
            txtPort = new System.Windows.Forms.TextBox();
            lblDatabase = new System.Windows.Forms.Label();
            cmbDatabase = new System.Windows.Forms.ComboBox();
            
            // Authentication Group Controls
            lblUsername = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            lblPassword = new System.Windows.Forms.Label();
            txtPassword = new System.Windows.Forms.TextBox();
            chkSaveCredentials = new System.Windows.Forms.CheckBox();
            
            // Actions Group Controls
            btnTestConnection = new System.Windows.Forms.Button();
            btnConnect = new System.Windows.Forms.Button();
            btnCodeFirst = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnViewLogs = new System.Windows.Forms.Button();
            
            // Status Controls
            lblStatus = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            panelStatus = new System.Windows.Forms.Panel();
            picStatus = new System.Windows.Forms.PictureBox();
            
            // Suspend layouts
            grpDatabaseType.SuspendLayout();
            grpConnectionDetails.SuspendLayout();
            grpAuthentication.SuspendLayout();
            grpActions.SuspendLayout();
            panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picDatabaseIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picStatus).BeginInit();
            SuspendLayout();

            // 
            // grpDatabaseType
            // 
            grpDatabaseType.Controls.Add(picDatabaseIcon);
            grpDatabaseType.Controls.Add(lblDatabaseType);
            grpDatabaseType.Controls.Add(cmbDatabaseType);
            grpDatabaseType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            grpDatabaseType.ForeColor = System.Drawing.Color.DarkBlue;
            grpDatabaseType.Location = new System.Drawing.Point(20, 20);
            grpDatabaseType.Name = "grpDatabaseType";
            grpDatabaseType.Padding = new System.Windows.Forms.Padding(10);
            grpDatabaseType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpDatabaseType.Size = new System.Drawing.Size(560, 90);
            grpDatabaseType.TabIndex = 0;
            grpDatabaseType.TabStop = false;
            grpDatabaseType.Text = "🗄️ نوع قاعدة البيانات";

            // 
            // picDatabaseIcon
            // 
            picDatabaseIcon.BackColor = System.Drawing.Color.Transparent;
            picDatabaseIcon.Location = new System.Drawing.Point(15, 30);
            picDatabaseIcon.Name = "picDatabaseIcon";
            picDatabaseIcon.Size = new System.Drawing.Size(32, 32);
            picDatabaseIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picDatabaseIcon.TabIndex = 2;
            picDatabaseIcon.TabStop = false;

            // 
            // lblDatabaseType
            // 
            lblDatabaseType.AutoSize = true;
            lblDatabaseType.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblDatabaseType.ForeColor = System.Drawing.Color.Black;
            lblDatabaseType.Location = new System.Drawing.Point(460, 35);
            lblDatabaseType.Name = "lblDatabaseType";
            lblDatabaseType.Size = new System.Drawing.Size(80, 20);
            lblDatabaseType.TabIndex = 0;
            lblDatabaseType.Text = "اختر النوع:";

            // 
            // cmbDatabaseType
            // 
            cmbDatabaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbDatabaseType.Font = new System.Drawing.Font("Segoe UI", 9F);
            cmbDatabaseType.FormattingEnabled = true;
            cmbDatabaseType.Location = new System.Drawing.Point(60, 32);
            cmbDatabaseType.Name = "cmbDatabaseType";
            cmbDatabaseType.Size = new System.Drawing.Size(390, 28);
            cmbDatabaseType.TabIndex = 1;

            // 
            // grpConnectionDetails
            // 
            grpConnectionDetails.Controls.Add(lblServer);
            grpConnectionDetails.Controls.Add(cmbServer);
            grpConnectionDetails.Controls.Add(lblPort);
            grpConnectionDetails.Controls.Add(txtPort);
            grpConnectionDetails.Controls.Add(lblDatabase);
            grpConnectionDetails.Controls.Add(cmbDatabase);
            grpConnectionDetails.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            grpConnectionDetails.ForeColor = System.Drawing.Color.DarkGreen;
            grpConnectionDetails.Location = new System.Drawing.Point(20, 120);
            grpConnectionDetails.Name = "grpConnectionDetails";
            grpConnectionDetails.Padding = new System.Windows.Forms.Padding(10);
            grpConnectionDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpConnectionDetails.Size = new System.Drawing.Size(560, 180);
            grpConnectionDetails.TabIndex = 1;
            grpConnectionDetails.TabStop = false;
            grpConnectionDetails.Text = "🌐 تفاصيل الاتصال";

            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblServer.ForeColor = System.Drawing.Color.Black;
            lblServer.Location = new System.Drawing.Point(470, 35);
            lblServer.Name = "lblServer";
            lblServer.Size = new System.Drawing.Size(70, 20);
            lblServer.TabIndex = 0;
            lblServer.Text = "السيرفر:";

            // 
            // cmbServer
            // 
            cmbServer.Font = new System.Drawing.Font("Segoe UI", 9F);
            cmbServer.FormattingEnabled = true;
            cmbServer.Location = new System.Drawing.Point(15, 32);
            cmbServer.Name = "cmbServer";
            cmbServer.Size = new System.Drawing.Size(445, 28);
            cmbServer.TabIndex = 1;

            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblPort.ForeColor = System.Drawing.Color.Black;
            lblPort.Location = new System.Drawing.Point(470, 75);
            lblPort.Name = "lblPort";
            lblPort.Size = new System.Drawing.Size(50, 20);
            lblPort.TabIndex = 2;
            lblPort.Text = "المنفذ:";

            // 
            // txtPort
            // 
            txtPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            txtPort.Location = new System.Drawing.Point(15, 72);
            txtPort.Name = "txtPort";
            txtPort.Size = new System.Drawing.Size(445, 27);
            txtPort.TabIndex = 3;
            txtPort.Text = "1433";

            // 
            // lblDatabase
            // 
            lblDatabase.AutoSize = true;
            lblDatabase.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblDatabase.ForeColor = System.Drawing.Color.Black;
            lblDatabase.Location = new System.Drawing.Point(440, 115);
            lblDatabase.Name = "lblDatabase";
            lblDatabase.Size = new System.Drawing.Size(100, 20);
            lblDatabase.TabIndex = 4;
            lblDatabase.Text = "قاعدة البيانات:";

            // 
            // cmbDatabase
            // 
            cmbDatabase.Font = new System.Drawing.Font("Segoe UI", 9F);
            cmbDatabase.FormattingEnabled = true;
            cmbDatabase.Location = new System.Drawing.Point(15, 112);
            cmbDatabase.Name = "cmbDatabase";
            cmbDatabase.Size = new System.Drawing.Size(415, 28);
            cmbDatabase.TabIndex = 5;

            // 
            // grpAuthentication
            // 
            grpAuthentication.Controls.Add(lblUsername);
            grpAuthentication.Controls.Add(txtUsername);
            grpAuthentication.Controls.Add(lblPassword);
            grpAuthentication.Controls.Add(txtPassword);
            grpAuthentication.Controls.Add(chkSaveCredentials);
            grpAuthentication.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            grpAuthentication.ForeColor = System.Drawing.Color.DarkOrange;
            grpAuthentication.Location = new System.Drawing.Point(20, 310);
            grpAuthentication.Name = "grpAuthentication";
            grpAuthentication.Padding = new System.Windows.Forms.Padding(10);
            grpAuthentication.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpAuthentication.Size = new System.Drawing.Size(560, 150);
            grpAuthentication.TabIndex = 2;
            grpAuthentication.TabStop = false;
            grpAuthentication.Text = "🔐 المصادقة";

            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblUsername.ForeColor = System.Drawing.Color.Black;
            lblUsername.Location = new System.Drawing.Point(440, 35);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(100, 20);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "اسم المستخدم:";

            // 
            // txtUsername
            // 
            txtUsername.Font = new System.Drawing.Font("Segoe UI", 9F);
            txtUsername.Location = new System.Drawing.Point(15, 32);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(415, 27);
            txtUsername.TabIndex = 1;

            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblPassword.ForeColor = System.Drawing.Color.Black;
            lblPassword.Location = new System.Drawing.Point(460, 75);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(80, 20);
            lblPassword.TabIndex = 2;
            lblPassword.Text = "كلمة المرور:";

            // 
            // txtPassword
            // 
            txtPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            txtPassword.Location = new System.Drawing.Point(15, 72);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '•';
            txtPassword.Size = new System.Drawing.Size(415, 27);
            txtPassword.TabIndex = 3;

            // 
            // chkSaveCredentials
            // 
            chkSaveCredentials.AutoSize = true;
            chkSaveCredentials.Font = new System.Drawing.Font("Segoe UI", 9F);
            chkSaveCredentials.ForeColor = System.Drawing.Color.Black;
            chkSaveCredentials.Location = new System.Drawing.Point(400, 115);
            chkSaveCredentials.Name = "chkSaveCredentials";
            chkSaveCredentials.Size = new System.Drawing.Size(140, 24);
            chkSaveCredentials.TabIndex = 4;
            chkSaveCredentials.Text = "حفظ بيانات الدخول";
            chkSaveCredentials.UseVisualStyleBackColor = true;

            // 
            // grpActions
            // 
            grpActions.Controls.Add(btnTestConnection);
            grpActions.Controls.Add(btnConnect);
            grpActions.Controls.Add(btnCancel);
            grpActions.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            grpActions.ForeColor = System.Drawing.Color.DarkRed;
            grpActions.Location = new System.Drawing.Point(20, 470);
            grpActions.Name = "grpActions";
            grpActions.Padding = new System.Windows.Forms.Padding(10);
            grpActions.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            grpActions.Size = new System.Drawing.Size(560, 80);
            grpActions.TabIndex = 3;
            grpActions.TabStop = false;
            grpActions.Text = "⚡ العمليات";

            // 
            // btnTestConnection
            // 
            btnTestConnection.BackColor = Helpers.AppTheme.Primary;
            btnTestConnection.FlatAppearance.BorderSize = 0;
            btnTestConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnTestConnection.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnTestConnection.ForeColor = System.Drawing.Color.White;
            btnTestConnection.Location = new System.Drawing.Point(380, 30);
            btnTestConnection.Name = "btnTestConnection";
            btnTestConnection.Size = new System.Drawing.Size(160, 40);
            btnTestConnection.TabIndex = 0;
            btnTestConnection.Text = "🔍 اختبار الاتصال";
            btnTestConnection.UseVisualStyleBackColor = false;

            // 
            // btnConnect
            // 
            btnConnect.BackColor = Helpers.AppTheme.Success;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnConnect.ForeColor = System.Drawing.Color.White;
            btnConnect.Location = new System.Drawing.Point(200, 30);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new System.Drawing.Size(160, 40);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "✅ اتصال";
            btnConnect.UseVisualStyleBackColor = false;

            // 
            // btnCancel
            // 
            btnCancel.BackColor = Helpers.AppTheme.Danger;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.White;
            btnCancel.Location = new System.Drawing.Point(20, 30);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(160, 40);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "❌ إلغاء";
            btnCancel.UseVisualStyleBackColor = false;

            // 
            // btnViewLogs - positioned after btnCodeFirst, before panelStatus
            // 
            btnViewLogs.BackColor = Helpers.AppTheme.Purple;
            btnViewLogs.FlatAppearance.BorderSize = 0;
            btnViewLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnViewLogs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnViewLogs.ForeColor = System.Drawing.Color.White;
            btnViewLogs.Location = new System.Drawing.Point(20, 560);
            btnViewLogs.Name = "btnViewLogs";
            btnViewLogs.Size = new System.Drawing.Size(160, 40);
            btnViewLogs.TabIndex = 3;
            btnViewLogs.Text = "📋 عرض السجلات";
            btnViewLogs.UseVisualStyleBackColor = false;

            // 
            // btnCodeFirst
            // 
            btnCodeFirst.BackColor = Helpers.AppTheme.PurpleDark;
            btnCodeFirst.FlatAppearance.BorderSize = 0;
            btnCodeFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnCodeFirst.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            btnCodeFirst.ForeColor = System.Drawing.Color.White;
            btnCodeFirst.Location = new System.Drawing.Point(190, 560);
            btnCodeFirst.Name = "btnCodeFirst";
            btnCodeFirst.Size = new System.Drawing.Size(390, 40);
            btnCodeFirst.TabIndex = 4;
            btnCodeFirst.Text = "Code First - تصميم الكيانات وتوليد المشروع";
            btnCodeFirst.UseVisualStyleBackColor = false;
            btnCodeFirst.Cursor = System.Windows.Forms.Cursors.Hand;

            // 
            // panelStatus
            // 
            panelStatus.BackColor = Helpers.AppTheme.PanelBackground;
            panelStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelStatus.Controls.Add(picStatus);
            panelStatus.Controls.Add(lblStatus);
            panelStatus.Controls.Add(progressBar);
            panelStatus.Location = new System.Drawing.Point(20, 610);
            panelStatus.Name = "panelStatus";
            panelStatus.Size = new System.Drawing.Size(560, 60);
            panelStatus.TabIndex = 4;

            // 
            // picStatus
            // 
            picStatus.BackColor = System.Drawing.Color.Transparent;
            picStatus.Location = new System.Drawing.Point(520, 10);
            picStatus.Name = "picStatus";
            picStatus.Size = new System.Drawing.Size(24, 24);
            picStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            picStatus.TabIndex = 2;
            picStatus.TabStop = false;

            // 
            // lblStatus
            // 
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblStatus.Location = new System.Drawing.Point(10, 12);
            lblStatus.Name = "lblStatus";
            lblStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            lblStatus.Size = new System.Drawing.Size(500, 20);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "جاهز للاتصال...";
            lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(10, 40);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(540, 8);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            progressBar.TabIndex = 1;
            progressBar.Visible = false;

            // 
            // FrmConnection
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = Helpers.AppTheme.SurfaceLight;
            ClientSize = new System.Drawing.Size(600, 690);
            Controls.Add(panelStatus);
            Controls.Add(btnViewLogs);
            Controls.Add(btnCodeFirst);
            Controls.Add(grpActions);
            Controls.Add(grpAuthentication);
            Controls.Add(grpConnectionDetails);
            Controls.Add(grpDatabaseType);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmConnection";
            RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "🔗 الاتصال بقاعدة البيانات";
            
            // Resume layouts
            grpDatabaseType.ResumeLayout(false);
            grpDatabaseType.PerformLayout();
            grpConnectionDetails.ResumeLayout(false);
            grpConnectionDetails.PerformLayout();
            grpAuthentication.ResumeLayout(false);
            grpAuthentication.PerformLayout();
            grpActions.ResumeLayout(false);
            panelStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picDatabaseIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)picStatus).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // Database Type Group
        private System.Windows.Forms.GroupBox grpDatabaseType;
        private System.Windows.Forms.Label lblDatabaseType;
        private System.Windows.Forms.ComboBox cmbDatabaseType;
        private System.Windows.Forms.PictureBox picDatabaseIcon;
        
        // Connection Details Group
        private System.Windows.Forms.GroupBox grpConnectionDetails;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.ComboBox cmbServer;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ComboBox cmbDatabase;
        
        // Authentication Group
        private System.Windows.Forms.GroupBox grpAuthentication;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkSaveCredentials;
        
        // Actions Group
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCodeFirst;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnViewLogs;
        
        // Status Panel
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox picStatus;
    }
} 