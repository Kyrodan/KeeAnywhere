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
            this.SuspendLayout();
            // 
            // m_browser
            // 
            this.m_browser.AllowWebBrowserDrop = false;
            this.m_browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_browser.IsWebBrowserContextMenuEnabled = false;
            this.m_browser.Location = new System.Drawing.Point(0, 0);
            this.m_browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.m_browser.Name = "m_browser";
            this.m_browser.Size = new System.Drawing.Size(511, 497);
            this.m_browser.TabIndex = 0;
            this.m_browser.WebBrowserShortcutsEnabled = false;
            this.m_browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.OnDocumentCompleted);
            // 
            // OAuth2Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 497);
            this.Controls.Add(this.m_browser);
            this.Name = "OAuth2Form";
            this.Text = "OAuth2Form";
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser m_browser;
    }
}