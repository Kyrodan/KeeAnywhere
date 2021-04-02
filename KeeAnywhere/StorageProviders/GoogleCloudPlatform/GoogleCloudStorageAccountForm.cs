using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using KeePass.UI;


namespace KeeAnywhere.StorageProviders.GoogleCloudPlatform
{
    public partial class GoogleCloudStorageAccountForm : Form
    {
        public GoogleCloudStorageAccountForm()
        {
            InitializeComponent();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.KeeAnywhere_48x48, "Authorize to Google Cloud Storage",
                "Please enter your Google Cloud Storage credentials here.");
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

            if (string.IsNullOrEmpty(this.KeyFile))
            {
                m_lblTestResult.Text = "Please select a key file.";
                return;
            }

            if (string.IsNullOrEmpty(this.ProjectId))
            {
                m_lblTestResult.Text = "Please enter project ID.";
                return;
            }

            this.Enabled = false;
            m_lblTestResult.Text = "Testing Connection...";

            this.TestResult = null;

            try
            {
                GoogleCredential credentials = GoogleCredential.FromFile(this.KeyFile);
                if (credentials == null)
                {
                    m_lblTestResult.Text = "The provided key file is invalid.";
                    return;
                }

                using (var client = StorageClient.Create(credentials))
                {
                    var buckets = client.ListBucketsAsync(this.ProjectId);
                    await buckets.ReadPageAsync(1);
                    
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

        public string KeyFile { get { return m_txtKeyFileLocation.Text; } }
        public string ProjectId { get { return m_txtProjectId.Text; } }

        protected bool? TestResult { get; set; }

        private void OnTextChanged(object sender, EventArgs e)
        {
            ClearTestResult();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
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

        private void m_btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = this.m_dlgOpenKeyFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.m_txtKeyFileLocation.Text = this.m_dlgOpenKeyFile.FileName;
            }
        }
    }
}
