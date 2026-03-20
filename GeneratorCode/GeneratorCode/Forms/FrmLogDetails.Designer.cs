namespace GeneratorCode.Forms
{
    partial class FrmLogDetails
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
            panelToolbar = new System.Windows.Forms.Panel();
            btnCopyAll = new System.Windows.Forms.Button();
            btnSearch = new System.Windows.Forms.Button();
            txtSearchBox = new System.Windows.Forms.TextBox();
            rtbDetails = new System.Windows.Forms.RichTextBox();
            btnClose = new System.Windows.Forms.Button();
            panelToolbar.SuspendLayout();
            SuspendLayout();

            // 
            // panelToolbar
            // 
            panelToolbar.BackColor = Helpers.AppTheme.PrimaryDark;
            panelToolbar.Controls.Add(btnCopyAll);
            panelToolbar.Controls.Add(txtSearchBox);
            panelToolbar.Controls.Add(btnSearch);
            panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            panelToolbar.Location = new System.Drawing.Point(0, 0);
            panelToolbar.Name = "panelToolbar";
            panelToolbar.Padding = new System.Windows.Forms.Padding(5);
            panelToolbar.Size = new System.Drawing.Size(900, 40);
            panelToolbar.TabIndex = 0;

            // 
            // btnCopyAll
            // 
            Helpers.AppTheme.StyleButton(btnCopyAll, Helpers.AppTheme.Primary);
            btnCopyAll.Location = new System.Drawing.Point(10, 5);
            btnCopyAll.Name = "btnCopyAll";
            btnCopyAll.Size = new System.Drawing.Size(100, 30);
            btnCopyAll.TabIndex = 0;
            btnCopyAll.Text = "نسخ الكل";

            // 
            // txtSearchBox
            // 
            txtSearchBox.Location = new System.Drawing.Point(120, 8);
            txtSearchBox.Name = "txtSearchBox";
            txtSearchBox.Size = new System.Drawing.Size(200, 23);
            txtSearchBox.TabIndex = 1;
            txtSearchBox.PlaceholderText = "بحث في النص...";
            txtSearchBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // 
            // btnSearch
            // 
            Helpers.AppTheme.StyleButton(btnSearch, Helpers.AppTheme.Success);
            btnSearch.Location = new System.Drawing.Point(330, 5);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new System.Drawing.Size(80, 30);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "بحث";

            // 
            // rtbDetails
            // 
            Helpers.AppTheme.StyleRichTextBoxConsole(rtbDetails);
            rtbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            rtbDetails.Location = new System.Drawing.Point(0, 40);
            rtbDetails.Name = "rtbDetails";
            rtbDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            rtbDetails.Size = new System.Drawing.Size(900, 620);
            rtbDetails.TabIndex = 1;
            rtbDetails.Text = "";

            // 
            // btnClose
            // 
            Helpers.AppTheme.StyleButton(btnClose, Helpers.AppTheme.Danger);
            btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnClose.Location = new System.Drawing.Point(0, 660);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(900, 40);
            btnClose.TabIndex = 2;
            btnClose.Text = "إغلاق";

            // 
            // FrmLogDetails
            // 
            this.BackColor = Helpers.AppTheme.FormBackground;
            this.Font = Helpers.AppTheme.DefaultFont;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(rtbDetails);
            this.Controls.Add(panelToolbar);
            this.Controls.Add(btnClose);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmLogDetails";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "تفاصيل السجل";

            panelToolbar.ResumeLayout(false);
            panelToolbar.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtSearchBox;
        private System.Windows.Forms.RichTextBox rtbDetails;
        private System.Windows.Forms.Button btnClose;
    }
}

