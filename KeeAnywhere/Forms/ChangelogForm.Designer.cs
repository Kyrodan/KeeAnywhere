namespace KeeAnywhere.Forms
{
    partial class ChangelogForm
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
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnOpenSettings = new System.Windows.Forms.Button();
            this.m_browser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(577, 60);
            this.m_bannerImage.TabIndex = 5;
            this.m_bannerImage.TabStop = false;
            // 
            // m_btnClose
            // 
            this.m_btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Location = new System.Drawing.Point(490, 522);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 9;
            this.m_btnClose.Text = "&Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            // 
            // m_btnOpenSettings
            // 
            this.m_btnOpenSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOpenSettings.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_btnOpenSettings.Location = new System.Drawing.Point(333, 522);
            this.m_btnOpenSettings.Name = "m_btnOpenSettings";
            this.m_btnOpenSettings.Size = new System.Drawing.Size(151, 23);
            this.m_btnOpenSettings.TabIndex = 10;
            this.m_btnOpenSettings.Text = "Open &Setting now";
            this.m_btnOpenSettings.UseVisualStyleBackColor = true;
            // 
            // m_browser
            // 
            this.m_browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_browser.Location = new System.Drawing.Point(13, 66);
            this.m_browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_browser.Name = "m_browser";
            this.m_browser.Size = new System.Drawing.Size(552, 450);
            this.m_browser.TabIndex = 14;
            // 
            // ChangelogForm
            // 
            this.AcceptButton = this.m_btnOpenSettings;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.ClientSize = new System.Drawing.Size(577, 557);
            this.Controls.Add(this.m_browser);
            this.Controls.Add(this.m_btnOpenSettings);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangelogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeeAnywhere Upgraded";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnOpenSettings;
        private System.Windows.Forms.WebBrowser m_browser;
    }
}