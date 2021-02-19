using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.Runtime;
using KeePass.UI;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    public partial class AmazonS3AccountForm : Form
    {
        public AmazonS3AccountForm()
        {
            InitializeComponent();

            m_cmbRegion.DataSource = new List<RegionEndpoint>(RegionEndpoint.EnumerableAllRegions);
            m_cmbRegion.DisplayMember = "DisplayName";
            m_cmbRegion.SelectedItem = RegionEndpoint.USWest1;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.KeeAnywhere_48x48, "Authorize to Amazon S3",
                "Please enter your Amazon S3 credentials here.");

            m_rbTypeAmazon.Checked = true;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private async void OnTest(object sender, EventArgs e)
        {
            await TestConnection();
        }

        private async Task TestConnection()
        {
            if (this.TestResult != null) return;

            m_lblTestResult.Visible = true;

            if (string.IsNullOrEmpty(this.AccessKey) ||string.IsNullOrEmpty(this.SecretKey))
            {
                m_lblTestResult.Text = "Please enter Access Key and Secret Key";
                return;
            }

            if (m_chkUseSessionToken.Checked == true && string.IsNullOrEmpty(this.SessionToken))
            {
                m_lblTestResult.Text = "Please enter Session Token";
                return;
            }

            if (!IsAmazon && string.IsNullOrEmpty(EndpointUrl))
            {
                m_lblTestResult.Text = "Please enter Endpoint URL";
                return;
            }

            this.Enabled = false;
            m_lblTestResult.Text = "Testing Connection...";

            this.TestResult = null;

            AWSCredentials credentials;

            if (m_chkUseSessionToken.Checked == false)
                credentials = new BasicAWSCredentials(this.AccessKey, this.SecretKey);
            else
                credentials = new SessionAWSCredentials(this.AccessKey, this.SecretKey, this.SessionToken);
            
            try
            {
                var config = this.IsAmazon ? AmazonS3Helper.GetConfig(this.AWSRegion) : AmazonS3Helper.GetConfig(this.EndpointUrl);

                using (var api = AmazonS3Helper.GetApi(credentials, config))
                {
                    var buckets = await api.ListBucketsAsync();
                    m_lblTestResult.Text = "Connection succeeded.";
                    this.TestResult = true;
                }
            }
            catch (Exception ex)
            {
                m_lblTestResult.Text = "Connection failed: " + ex.Message;
                this.TestResult = false;
            }
            finally
            {
                this.Enabled = true;
            }
        }

        public void ClearTestResult()
        {
            if (this.TestResult == null) return;

            this.TestResult = null;
            m_lblTestResult.Text = null;
            m_lblTestResult.Visible = false;
        }

        public string AccessKey { get { return m_txtAccessKey.Text.Trim(); } }
        public string SecretKey { get { return m_txtSecretKey.Text.Trim(); } }
        public string SessionToken { get { return m_txtSessionToken.Text.Trim(); } }
        public bool UseSessionToken { get { return m_chkUseSessionToken.Checked; } }
        public RegionEndpoint AWSRegion { get { return (RegionEndpoint) m_cmbRegion.SelectedItem; } }
        public string EndpointUrl { get { return m_cmbEndpointUrl.Text.Trim(); } }
        public bool IsAmazon { get { return m_rbTypeAmazon.Checked; } } 

        protected bool? TestResult { get; set; }

        private void OnTextChanged(object sender, EventArgs e)
        {
            ClearTestResult();
        }


        private async void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                return;

            if (this.TestResult == null || !this.TestResult.Value)
                e.Cancel = true;
        }

        private async void OnOk(object sender, EventArgs e)
        {
            await TestConnection();

            if (this.TestResult != null && this.TestResult.Value)
                this.DialogResult = DialogResult.OK;
        }
        
        private void OnUseSessionTokenChanged(object sender, EventArgs e)
        {
            m_txtSessionToken.Enabled = m_chkUseSessionToken.Checked;
            ClearTestResult();
        }

        private void OnTypeChanged(object sender, EventArgs e)
        {
            m_cmbRegion.Enabled = m_rbTypeAmazon.Checked;
            m_cmbEndpointUrl.Enabled = m_rbTypeOther.Checked;
            ClearTestResult();
        }
    }
}
