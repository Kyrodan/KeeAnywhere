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
        Task<bool> Claim(Uri uri, string documentTitle);

        Uri AuthorizationUrl { get; }
        Uri RedirectionUrl { get; }

        string FriendlyProviderName { get; }
    }
}
