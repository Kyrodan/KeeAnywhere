using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.AmazonS3
{
    public static class AmazonS3Helper
    {
        public static AmazonS3Client GetApi(AccountConfiguration account)
        {
            var credentials = new BasicAWSCredentials(account.Id, account.Secret);

            var region = RegionEndpoint.USWest1;
            if (account.AdditionalSettings != null && account.AdditionalSettings.ContainsKey("AWSRegion"))
            {
                var regionName = account.AdditionalSettings["AWSRegion"];
                region = RegionEndpoint.GetBySystemName(regionName);
            }

            var api = new AmazonS3Client(credentials, region);

            return api;
        }
    }
}
