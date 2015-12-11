using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePass.UI;

namespace KeeAnywhere.OAuth2
{
    public interface IOAuth2Provider
    {
        void Initialize();
        bool Claim(Uri uri);

        Uri AuthorizationUrl { get; }
        Uri RedirectionUrl { get; }
    }

    public static class OAuth2Flow
    {
        public static bool TryAuthenticate(IOAuth2Provider provider)
        {
            var dlg = new OAuth2Form();
            dlg.InitEx(provider);
            var result = UIUtil.ShowDialogAndDestroy(dlg);

            return result == DialogResult.OK;
        }
    }
}
