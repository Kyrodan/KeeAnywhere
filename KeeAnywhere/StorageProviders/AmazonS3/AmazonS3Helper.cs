using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using KeeAnywhere.Configuration;
using KeePassLib;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    public static class AmazonS3Helper
    {
        public static AmazonS3Client GetApi(AccountConfiguration account)
        {
            if (account.AdditionalSettings == null)
            {
                throw new Exception("'account' has no 'AdditionalSetting'.");
            }

            AWSCredentials credentials;
            if (account.AdditionalSettings.ContainsKey("UseSessionToken") && Convert.ToBoolean(account.AdditionalSettings["UseSessionToken"]) == true && account.AdditionalSettings.ContainsKey("SessionToken"))
                credentials = new SessionAWSCredentials(account.Id, account.Secret, account.AdditionalSettings["SessionToken"]);
            else
                credentials = new BasicAWSCredentials(account.Id, account.Secret);

            AmazonS3Config config;

            if (account.AdditionalSettings.ContainsKey("EndpointURL")) {
                config = GetConfig(account.AdditionalSettings["EndpointURL"]);
            } else
            {
                var region = RegionEndpoint.USWest1;

                if (account.AdditionalSettings.ContainsKey("AWSRegion"))
                {
                    var regionName = account.AdditionalSettings["AWSRegion"];
                    region = RegionEndpoint.GetBySystemName(regionName);
                }

                config = GetConfig(region);
            }

            return GetApi(credentials, config);
        }

        public static AmazonS3Config GetConfig(string endpointURL)
        {
            if (string.IsNullOrEmpty(endpointURL)) throw new ArgumentNullException("endpointURL");

            var config = new AmazonS3Config
            {
                //RegionEndpoint = RegionEndpoint.USEast1, // Required???
                ServiceURL = endpointURL,
                ForcePathStyle = true,
                Timeout = Timeout.InfiniteTimeSpan
            };

            return config;
        }

        public static AmazonS3Config GetConfig(RegionEndpoint region)
        {
            if (region == null) throw new ArgumentNullException("region");

            var config = new AmazonS3Config
            {
                RegionEndpoint = region,
                Timeout = Timeout.InfiniteTimeSpan
            };

            return config;
        }

        public static AmazonS3Client GetApi(AWSCredentials credentials, AmazonS3Config config)
        {
            if (credentials == null) throw new ArgumentNullException("credentials");
            if (config == null) throw new ArgumentNullException("config");

            ApplyProxy(config);

            var api = new AmazonS3Client(credentials, config);
            return api;
        }

        private static void ApplyProxy(ClientConfig config)
        {
            var proxyAddress = KeePass.Program.Config.Integration.ProxyAddress;
            var proxyPort = KeePass.Program.Config.Integration.ProxyPort;
            var proxyType = KeePass.Program.Config.Integration.ProxyType;

            if (proxyType == ProxyServerType.Manual)
            {
                if (!string.IsNullOrEmpty(proxyAddress)) config.ProxyHost = proxyAddress;
                if (!string.IsNullOrEmpty(proxyPort)) config.ProxyPort = int.Parse(proxyPort);
                config.ProxyCredentials = ProxyTools.GetKeePassProxyCredentials();
            }
            else if (proxyType == ProxyServerType.System)
            {
                if (config.RegionEndpoint == null) return;

                var systemProxy = System.Net.WebRequest.DefaultWebProxy;
                var endpoint = config.RegionEndpoint.GetEndpointForService("s3");
                var uri = new Uri("https://" + endpoint.Hostname);

                if (systemProxy.IsBypassed(uri)) return;
                var proxyHost = systemProxy.GetProxy(uri);

                config.ProxyHost = proxyHost.Host;
                config.ProxyPort = proxyHost.Port;
                config.ProxyCredentials = systemProxy.Credentials;
            }
        }
    }
}
