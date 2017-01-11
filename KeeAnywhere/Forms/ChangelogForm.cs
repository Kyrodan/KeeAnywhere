using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            var outStream = new MemoryStream();
            var assembly = Assembly.GetExecutingAssembly();
            using (var inReader = new StreamReader(assembly.GetManifestResourceStream("KeeAnywhere.CHANGELOG.md")))
            {

                var outWriter = new StreamWriter(outStream);

                CommonMark.CommonMarkConverter.Convert(inReader, outWriter);
                outWriter.Flush();
                outStream.Position = 0;

                m_browser.DocumentStream = outStream;
            }

            m_btnOpenSettings.Visible = _isUpgraded;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

    }
}
