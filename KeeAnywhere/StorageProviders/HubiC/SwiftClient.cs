using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeeAnywhere.StorageProviders.HubiC
{
    public class SwiftClient
    {
        private readonly HubiCCredentials _credentials;

        public SwiftClient(HubiCCredentials credentials)
        {
            if (credentials == null) throw new ArgumentNullException("credentials");
            _credentials = credentials;
        }

        private HttpClient GetClient()
        {
            var client = ProxyTools.CreateHttpClient();

            client.DefaultRequestHeaders.Add("X-Auth-Token", _credentials.Token);
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8"));

            return client;
        }

        public async Task<IEnumerable<SwiftContainer>> GetContainers()
        {
            var client = GetClient();
            var response = await client.GetAsync(_credentials.Endpoint);

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var raw = Encoding.UTF8.GetString(bytes);
            var containers = JsonConvert.DeserializeObject<IEnumerable<SwiftContainer>>(raw);

            return containers;
        }

        public async Task<IEnumerable<SwiftObject>> GetObjects(string container, string path)
        {
            if (container == null) throw new ArgumentNullException("container");

            var uri = _credentials.Endpoint + "/" + container + "?delimiter=/";
            if (!string.IsNullOrEmpty(path) && path != "/")
            {
                if (!path.EndsWith("/"))
                    path += "/";

                uri += "&prefix=" + path;
            }

            var client = GetClient();
            var response = await client.GetAsync(uri);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var raw = Encoding.UTF8.GetString(bytes);
            var objects = JsonConvert.DeserializeObject<IEnumerable<SwiftObject>>(raw);

            return objects.Where(_ => string.IsNullOrEmpty(_.VirtualSubDirectory)).ToArray();
        }

        public async Task<Stream> DownloadObject(string container, string path)
        {
            if (container == null) throw new ArgumentNullException("container");
            if (path == null) throw new ArgumentNullException("path");

            var uri = _credentials.Endpoint + "/" + container + "/" + path;
            var client = GetClient();
            var stream = await client.GetStreamAsync(uri);

            return stream;
        }

        public async Task<bool> UploadObject(string container, string path, Stream stream)
        {
            if (container == null) throw new ArgumentNullException("container");
            if (path == null) throw new ArgumentNullException("path");
            if (stream == null) throw new ArgumentNullException("stream");

            var uri = _credentials.Endpoint + "/" + container + "/" + path;
            var client = GetClient();

            var content = new StreamContent(stream);
            var response = await client.PutAsync(uri, content);

            return response.IsSuccessStatusCode;
        }
    }
}
