using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KeePassLib;

namespace KeeAnywhere.StorageProviders
{
    public static class ProxyTools
    {
        public static HttpClient CreateHttpClient()
        {

            return new HttpClient(CreateHttpClientHandler()) {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public static HttpClientHandler CreateHttpClientHandler()
        {
            var handler = new HttpClientHandler();
            handler.ApplyProxy();

            return handler;
        }

        public static void ApplyProxy(this HttpClientHandler handler)
        {
            var proxy = GetProxy();
            if (proxy == null)
            {
                handler.UseProxy = false;
                return;
            }

            handler.UseProxy = true;
            handler.Proxy = proxy;
        }

        public static IWebProxy GetProxy()
        {
            var proxy = GetKeePassWebProxy();
            if (proxy == null) return null;

            proxy.Credentials = GetKeePassProxyCredentials();
            return proxy;
        }

        public static IWebProxy GetKeePassWebProxy()
        {
            IWebProxy proxy;

            var proxyAddress = KeePass.Program.Config.Integration.ProxyAddress;
            var proxyPort = KeePass.Program.Config.Integration.ProxyPort;

            switch (KeePass.Program.Config.Integration.ProxyType)
            {
                case ProxyServerType.None:
                    proxy = null; ; // Use null proxy
                    break;

                case ProxyServerType.Manual:
                    if (string.IsNullOrEmpty(proxyAddress))
                    {
                        // First try default (from config), then system
                        proxy = System.Net.WebRequest.DefaultWebProxy;

                        //if (proxy == null)
                        //    proxy = SystemWebProxy;
                    }
                    else if (!string.IsNullOrEmpty(proxyPort))
                        proxy = new WebProxy(proxyAddress, int.Parse(proxyPort));
                    else
                        proxy = new WebProxy(proxyAddress);

                    break;

                case ProxyServerType.System:
                    proxy = System.Net.WebRequest.DefaultWebProxy;
                    break;

                default:
                    throw new NotImplementedException();

            }

            return proxy;
        }

        public static ICredentials GetKeePassProxyCredentials()
        {
            ICredentials credentials = null;

            var userName = KeePass.Program.Config.Integration.ProxyUserName;
            var password = KeePass.Program.Config.Integration.ProxyPassword;
            var proxyAuthType = KeePass.Program.Config.Integration.ProxyAuthType;

            if (proxyAuthType == ProxyAuthType.Auto)
            {
                if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password))
                    proxyAuthType = ProxyAuthType.Manual;
                else
                    proxyAuthType = ProxyAuthType.Default;
            }

            switch (proxyAuthType)
            {
                case ProxyAuthType.None:
                    credentials = null;
                    break;

                case ProxyAuthType.Default:
                    credentials = CredentialCache.DefaultCredentials;
                    break;

                case ProxyAuthType.Manual:
                    if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password))
                        credentials = new NetworkCredential(
                            userName, password);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return credentials;
        }
    }
}
