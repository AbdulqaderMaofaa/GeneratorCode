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
            rtbDetails = new System.Windows.Forms.RichTextBox();
            btnClose = new System.Windows.Forms.Button();
            SuspendLayout();

            // 
            // rtbDetails
            // 
            rtbDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            rtbDetails.Font = new System.Drawing.Font("Consolas", 9F);
            rtbDetails.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            rtbDetails.ForeColor = System.Drawing.Color.White;
            rtbDetails.Location = new System.Drawing.Point(0, 0);
            rtbDetails.Name = "rtbDetails";
            rtbDetails.ReadOnly = true;
            rtbDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            rtbDetails.Size = new System.Drawing.Size(900, 660);
            rtbDetails.TabIndex = 0;
            rtbDetails.Text = "";

            // 
            // btnClose
            // 
            btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnClose.Location = new System.Drawing.Point(0, 660);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(900, 40);
            btnClose.TabIndex = 1;
            btnClose.Text = "إغلاق";
            btnClose.UseVisualStyleBackColor = true;

            // 
            // FrmLogDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(rtbDetails);
            this.Controls.Add(btnClose);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "FrmLogDetails";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "تفاصيل السجل";

            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbDetails;
        private System.Windows.Forms.Button btnClose;
    }
}

