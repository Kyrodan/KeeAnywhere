using System.Windows.Forms;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.StorageProviders.OneDrive
{
    public partial class OneDriveAuthenticationForm : Form
    {
        /// <summary>
        /// OneDrive API instance
        /// </summary>
        public OneDriveConsumerApi m_api { get; private set; }

        public OneDriveAuthenticationForm(OneDriveConsumerApi api)
        {
            InitializeComponent();
            this.Icon = PluginResources.Icon_OneDrive_16x16;


            m_api = api;

            SignOut();
        }

        /// <summary>
        /// Sign the current user out of OneDrive
        /// </summary>
        public void SignOut()
        {
            // First sign the current user out to make sure he/she needs to authenticate again
            var signoutUri = m_api.GetSignOutUri();
            WebBrowser.Navigate(signoutUri);
        }

        private async void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            // Check if the current URL contains the authorization token
            var authorizationCode = m_api.GetAuthorizationTokenFromUrl(e.Url.ToString());

            // Verify if an authorization token was successfully extracted
            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // Get an access token based on the authorization token that we now have
                await m_api.GetAccessToken();
                if (m_api.AccessToken != null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
            }

            // If we're on this page, but we didn't get an authorization token, it means that we just signed out, proceed with signing in again
            if (e.Url.ToString().StartsWith("https://login.live.com/oauth20_desktop.srf"))
            {
                var authenticateUri = m_api.GetAuthenticationUri("wl.offline_access wl.skydrive_update");
                WebBrowser.Navigate(authenticateUri);
            }
        }
    }
}
