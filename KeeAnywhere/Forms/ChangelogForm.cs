using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.StorageProviders;
using KeePass.UI;

namespace KeeAnywhere.Forms
{
    public partial class ChangelogForm : Form
    {
        private bool _isUpgraded;

        public ChangelogForm()
        {
            InitializeComponent();
        }

        public void InitEx(bool isUpgraded)
        {
            _isUpgraded = isUpgraded;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            if (_isUpgraded)
            {
                this.Text = "KeeAnywhere upgraded";
                BannerFactory.CreateBannerEx(this, m_bannerImage,
                    PluginResources.KeeAnywhere_48x48, "KeeAnywhere has been upgraded",
                    "Please check changelog and adjust your settings if needed.");
            }
            else
            {
                this.Text = "KeeAnywhere Changelog";
                BannerFactory.CreateBannerEx(this, m_bannerImage,
                    PluginResources.KeeAnywhere_48x48, "KeeAnywhere Changelog",
                    "See detailed changes for each version.");
            }

            var wc = new WebClient {Proxy = ProxyTools.GetProxy()};

            var version = this.GetVersionTag();
            var url = string.Format("https://raw.githubusercontent.com/Kyrodan/KeeAnywhere/{0}/CHANGELOG.md", version);

            try
            {
                var markdown = wc.DownloadString(url);
                var html = CommonMark.CommonMarkConverter.Convert(markdown);
                m_browser.DocumentText = html;
            }
            catch
            {
                //Ignore
            }

            m_btnOpenSettings.Visible = _isUpgraded;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private string GetVersionTag()
        {
            var tag = "master";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                if (versionAttribute != null)
                    tag = "v" + versionAttribute.InformationalVersion;

                if (tag.EndsWith("unstable"))
                    tag = "develop";

            }
            catch
            {
                // ignored
            }

            return tag;
        }
    }
}
