namespace KeeAnywhere.Forms
{
    partial class DonationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DonationForm));
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnDonate = new System.Windows.Forms.Button();
            this.m_chkDontShowAgain = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_lblDivider = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(517, 60);
            this.m_bannerImage.TabIndex = 5;
            this.m_bannerImage.TabStop = false;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnClose.Location = new System.Drawing.Point(430, 218);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 9;
            this.m_btnClose.Text = "&Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            // 
            // m_btnDonate
            // 
            this.m_btnDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnDonate.Location = new System.Drawing.Point(273, 218);
            this.m_btnDonate.Name = "m_btnDonate";
            this.m_btnDonate.Size = new System.Drawing.Size(151, 23);
            this.m_btnDonate.TabIndex = 10;
            this.m_btnDonate.Text = "&Show me how to donate";
            this.m_btnDonate.UseVisualStyleBackColor = true;
            this.m_btnDonate.Click += new System.EventHandler(this.OnShowMeHowToDonate);
            // 
            // m_chkDontShowAgain
            // 
            this.m_chkDontShowAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_chkDontShowAgain.AutoSize = true;
            this.m_chkDontShowAgain.Location = new System.Drawing.Point(12, 222);
            this.m_chkDontShowAgain.Name = "m_chkDontShowAgain";
            this.m_chkDontShowAgain.Size = new System.Drawing.Size(172, 17);
            this.m_chkDontShowAgain.TabIndex = 11;
            this.m_chkDontShowAgain.Text = "&Don\'t show this message again";
            this.m_chkDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(493, 139);
            this.label1.TabIndex = 12;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // m_lblDivider
            // 
            this.m_lblDivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblDivider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.m_lblDivider.Location = new System.Drawing.Point(0, 209);
            this.m_lblDivider.Name = "m_lblDivider";
            this.m_lblDivider.Size = new System.Drawing.Size(516, 2);
            this.m_lblDivider.TabIndex = 13;
            // 
            // DonationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(517, 253);
            this.Controls.Add(this.m_lblDivider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_chkDontShowAgain);
            this.Controls.Add(this.m_btnDonate);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DonationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Donate to KeeAnywhere";
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnDonate;
        private System.Windows.Forms.CheckBox m_chkDontShowAgain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label m_lblDivider;
    }
}