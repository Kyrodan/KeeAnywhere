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
using KeePass.UI;

namespace KeeAnywhere.Forms
{
    public partial class DonationForm : Form
    {
        public DonationForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.KeeAnywhere_48x48, "Donate for KeeAnywhere",
                "Your contribution to support KeeAnywhere.");
            
        }

        private void OnShowMeHowToDonate(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Kyrodan/KeeAnywhere/blob/master/DONATE.md");
        }

        public bool IsDontShowMessageAgain { get
            {
                return m_chkDontShowAgain.Checked;
            }
        }
    }
}
