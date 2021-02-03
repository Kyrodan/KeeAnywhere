using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using KeeAnywhere.StorageProviders;
using KeePass.UI;

namespace KeeAnywhere.OAuth2
{
    public partial class OidcWaitForm : Form
    {
        private string m_providername;
        private CancellationTokenSource m_tokenSource;

        public OidcWaitForm()
        {
            InitializeComponent();

            this.m_tokenSource = new CancellationTokenSource();
        }

        public async void InitEx(StorageType type)
        {
            if (type == null) throw new ArgumentNullException("type");

            var descriptor = StorageRegistry.Descriptors.FirstOrDefault(d => d.Type == type);
            if (descriptor == null) throw new ArgumentException("type not valid");

            m_providername = descriptor.FriendlyName;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            Icon = PluginResources.Icon_OneDrive_16x16;

            UpdateBanner();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void UpdateBanner()
        {
            if (m_providername == null) return;
            var text = string.Format("Authorize to {0}", m_providername);
            this.Text = text;
            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.KeeAnywhere_48x48, text,
                string.Format("Please follow the instructions to authorize KeeAnywhere to access your {0} account.", m_providername));
        }

        private void OnResize(object sender, EventArgs e)
        {
            UpdateBanner();
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this.m_tokenSource.Token;
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            //this.m_tokenSource.Cancel();
            this.Close();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.Cancel)
            {
                this.m_tokenSource.Cancel();
            }
        }
    }
}