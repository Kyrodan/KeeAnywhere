using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.UI;
using Microsoft.Azure.Storage.Auth;

namespace KeeAnywhere.StorageProviders.Azure
{
    public partial class AzureAccountForm : Form
    {
        public AzureAccountForm(StorageType storageType)
        {
            InitializeComponent();

            switch (storageType)
            {
                case StorageType.AzureBlob:
                    m_lblItemName.Text = AzureResources.Label_Item_Name_Blob;
                    this.Text = AzureResources.Form_Title_Blob;
                    break;
                case StorageType.AzureFile:
                    m_lblItemName.Text = AzureResources.Label_Item_Name_File;
                    this.Text = AzureResources.Form_Title_File;
                    break;
                default:
                    throw new ArgumentException("The given storage type was unknown.", nameof(storageType));
            }

            this.StorageType = storageType;
            this.Update();
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            this.Icon = PluginResources.Icon_OneDrive_16x16;

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                PluginResources.KeeAnywhere_48x48, AzureResources.Keepass_Banner_Title,
                AzureResources.Keepass_Banner_Line);
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

            if (string.IsNullOrEmpty(this.AccountName) ||string.IsNullOrEmpty(this.AccessToken))
            {
                m_lblTestResult.Text = AzureResources.Form_Test_Credentials_Required;
                return;
            }

            this.Enabled = false;
            m_lblTestResult.Text = AzureResources.Form_Test_Testing_Connection;

            this.TestResult = null;

            try
            {
                var error = false;
                var credentials = new StorageCredentials(this.AccountName, this.AccessToken);
                switch (this.StorageType)
                {
                    case StorageType.AzureBlob:
                        error = await AzureBlobStorageProvider.TestContainer(credentials, this.ItemName);
                        break;
                    case StorageType.AzureFile:
                        error = await AzureFileStorageProvider.TestShare(credentials, this.ItemName);
                        break;
                    default:
                        error = true;
                        break;
                }

                if (error)
                {
                    throw new Exception("Could not connect to container.");
                }

                m_lblTestResult.Text = AzureResources.Form_Test_Succeeded;
                this.TestResult = true;
            }
            catch (Exception ex)
            {
                m_lblTestResult.Text = AzureResources.Form_Test_Failed;
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

        public string AccountName { get { return m_txtAccountName.Text.Trim(); } }
        public string AccessToken { get { return m_txtAccessToken.Text.Trim(); } }
        public string ItemName { get { return m_txtItemName.Text.Trim(); } }
        public readonly StorageType StorageType;

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
    }
}
