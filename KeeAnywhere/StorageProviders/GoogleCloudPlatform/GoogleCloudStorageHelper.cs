using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using KeeAnywhere.Configuration;

namespace KeeAnywhere.StorageProviders.GoogleCloudPlatform
{
    public static class GoogleCloudStorageHelper
    {
        public static GoogleCredential GetCredentialsFromFile(string path)
        {
            GoogleCredential credentials;
            try
            {
                credentials = GoogleCredential.FromFile(path);
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException("Invalid credentials key file provided", ex);
            }

            return credentials;
        }

        public static GoogleCredential GetCredentialsFromJson(string json)
        {
            GoogleCredential credentials;
            try
            {
                credentials = GoogleCredential.FromJson(json);
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException("Invalid credentials JSON provided", ex);
            }

            return credentials;
        }

        public static GoogleCredential CreateCredentials(Stream stream)
        {
            GoogleCredential credentials;
            try
            {
                credentials = GoogleCredential.FromStream(stream);
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException("Invalid credentials JSON provided", ex);
            }

            return credentials;
        }
    }
}
