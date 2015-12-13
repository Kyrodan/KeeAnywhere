using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeeAnywhere.OAuth2
{
    public partial class OAuth2Form : Form
    {
        private IOAuth2Provider m_provider;

        public OAuth2Form()
        {
            InitializeComponent();
        }

        public void InitEx(IOAuth2Provider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            m_provider = provider;

            provider.Initialize();
        }

        private void OnLoad(object sender, EventArgs e)
        {

            m_browser.Navigate(m_provider.AuthorizationUrl);
        }

        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!e.Url.ToString().StartsWith(m_provider.RedirectionUrl.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // we need to ignore all navigation that isn't to the redirect uri.
                return;
            }

            try
            {
                var isOk = m_provider.Claim(e.Url);
                if (isOk)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (ArgumentException)
            {
                //                e.Cancel = true;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
