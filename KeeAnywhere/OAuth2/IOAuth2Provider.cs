using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeeAnywhere.OAuth2
{
    public interface IOAuth2Provider
    {
        void Initialize();
        bool Claim(Uri uri);

        Uri AuthorizationUrl { get; }
        Uri RedirectionUrl { get; }
    }
}
