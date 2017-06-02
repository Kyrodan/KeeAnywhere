using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.OAuth2
{
    public interface IOAuth2Provider
    {
        Task Initialize();
        bool CanClaim(Uri uri, string documentTitle);
        Task<bool> Claim(Uri uri, string documentTitle);

        Uri PreAuthorizationUrl { get; }
        Uri AuthorizationUrl { get; }
        Uri RedirectionUrl { get; }

        string FriendlyProviderName { get; }
    }
}
