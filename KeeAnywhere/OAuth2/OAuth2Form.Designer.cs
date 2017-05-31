namespace KeeAnywhere.OAuth2
{
    partial class OAuth2Form
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
            this.m_browser = new System.Windows.Forms.WebBrowser();
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_pnlWait = new System.Windows.Forms.Panel();
            this.m_lblWait = new System.Windows.Forms.Label();
            this.m_pgbWait = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.m_pnlWait.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_browser
            // 
            this.m_browser.AllowWebBrowserDrop = false;
            this.m_browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_browser.Location = new System.Drawing.Point(0, 66);
            this.m_browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_browser.Name = "m_browser";
            this.m_browser.ScriptErrorsSuppressed = true;
            this.m_browser.Size = new System.Drawing.Size(834, 546);
            this.m_browser.TabIndex = 0;
            this.m_browser.WebBrowserShortcutsEnabled = false;
            this.m_browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.OnDocumentCompleted);
            this.m_browser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.OnNavigated);
            this.m_browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.OnNavigating);
            this.m_browser.NewWindow += new System.ComponentModel.CancelEventHandler(this.OnNewWindow);
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(834, 60);
            this.m_bannerImage.TabIndex = 17;
            this.m_bannerImage.TabStop = false;
            // 
            // m_pnlWait
            // 
            this.m_pnlWait.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pnlWait.Controls.Add(this.m_lblWait);
            this.m_pnlWait.Controls.Add(this.m_pgbWait);
            this.m_pnlWait.Location = new System.Drawing.Point(0, 66);
            this.m_pnlWait.Name = "m_pnlWait";
            this.m_pnlWait.Size = new System.Drawing.Size(834, 546);
            this.m_pnlWait.TabIndex = 18;
            this.m_pnlWait.UseWaitCursor = true;
            this.m_pnlWait.Visible = false;
            // 
            // m_lblWait
            // 
            this.m_lblWait.AutoSize = true;
            this.m_lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblWait.Location = new System.Drawing.Point(109, 181);
            this.m_lblWait.Name = "m_lblWait";
            this.m_lblWait.Size = new System.Drawing.Size(161, 13);
            this.m_lblWait.TabIndex = 1;
            this.m_lblWait.Text = "Processing ... Please Wait!";
            this.m_lblWait.UseWaitCursor = true;
            // 
            // m_pgbWait
            // 
            this.m_pgbWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_pgbWait.Location = new System.Drawing.Point(109, 151);
            this.m_pgbWait.Name = "m_pgbWait";
            this.m_pgbWait.Size = new System.Drawing.Size(615, 23);
            this.m_pgbWait.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.m_pgbWait.TabIndex = 0;
            this.m_pgbWait.UseWaitCursor = true;
            this.m_pgbWait.Value = 100;
            // 
            // OAuth2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 612);
            this.Controls.Add(this.m_pnlWait);
            this.Controls.Add(this.m_bannerImage);
            this.Controls.Add(this.m_browser);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OAuth2Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OAuth2Form";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnLoad);
            this.Resize += new System.EventHandler(this.OnResize);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.m_pnlWait.ResumeLayout(false);
            this.m_pnlWait.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser m_browser;
        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Panel m_pnlWait;
        private System.Windows.Forms.Label m_lblWait;
        private System.Windows.Forms.ProgressBar m_pgbWait;
    }
}