using GeneratorCode.Helpers;

namespace GeneratorCode.Forms
{
    partial class FrmPreview
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
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.lblLineCount = new System.Windows.Forms.Label();
            this.txtPreview = new System.Windows.Forms.RichTextBox();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = AppTheme.PrimaryDark;
            this.panelToolbar.Controls.Add(this.btnCopy);
            this.panelToolbar.Controls.Add(this.btnSaveAs);
            this.panelToolbar.Controls.Add(this.lblLineCount);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(0, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Padding = new System.Windows.Forms.Padding(5);
            this.panelToolbar.Size = new System.Drawing.Size(900, 40);
            this.panelToolbar.TabIndex = 0;
            // 
            // btnCopy
            // 
            AppTheme.StyleButton(this.btnCopy, AppTheme.Primary);
            this.btnCopy.Location = new System.Drawing.Point(10, 5);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(100, 30);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "نسخ الكل";
            // 
            // btnSaveAs
            // 
            AppTheme.StyleButton(this.btnSaveAs, AppTheme.Success);
            this.btnSaveAs.Location = new System.Drawing.Point(120, 5);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(100, 30);
            this.btnSaveAs.TabIndex = 1;
            this.btnSaveAs.Text = "حفظ كملف";
            // 
            // lblLineCount
            // 
            this.lblLineCount.AutoSize = true;
            this.lblLineCount.Font = AppTheme.DefaultFontSmall;
            this.lblLineCount.ForeColor = System.Drawing.Color.White;
            this.lblLineCount.Location = new System.Drawing.Point(240, 10);
            this.lblLineCount.Name = "lblLineCount";
            this.lblLineCount.Size = new System.Drawing.Size(0, 15);
            this.lblLineCount.TabIndex = 2;
            // 
            // txtPreview
            // 
            this.txtPreview.BackColor = AppTheme.ConsoleBg;
            this.txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreview.Font = AppTheme.ConsoleFont;
            this.txtPreview.ForeColor = AppTheme.ConsoleHighlight;
            this.txtPreview.Location = new System.Drawing.Point(0, 40);
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ReadOnly = true;
            this.txtPreview.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtPreview.Size = new System.Drawing.Size(900, 510);
            this.txtPreview.TabIndex = 1;
            this.txtPreview.Text = "";
            this.txtPreview.WordWrap = false;
            // 
            // FrmPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = AppTheme.FormBackground;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.txtPreview);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmPreview";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "معاينة الكود";
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Label lblLineCount;
        private System.Windows.Forms.RichTextBox txtPreview;
    }
}
