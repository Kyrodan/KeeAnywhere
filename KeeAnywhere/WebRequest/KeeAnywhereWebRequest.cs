using System;
using System.IO;
using System.Net;
using KeeAnywhere.StorageProviders;
using KeePassLib.Serialization;
using KoenZomers.OneDrive.Api;

namespace KeeAnywhere.WebRequest
{
    public sealed class KeeAnywhereWebRequest : System.Net.WebRequest
    {
        private readonly IStorageProvider _provider;
        private readonly string _itemPath;

        private MemoryStream _requestStream;
        private WebResponse _response;
        private WebHeaderCollection _headers;


        public override ICredentials Credentials { get; set; }
        public override string Method { get; set; }
        public override bool PreAuthenticate { get; set; }
        public override IWebProxy Proxy { get; set; }

        public override WebHeaderCollection Headers
        {
            get
            {
                if (_headers == null)
                    _headers = new WebHeaderCollection();

                return _headers;
            }
            set { _headers = value; }
        }

        public KeeAnywhereWebRequest(IStorageProvider provider, string itemPath)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (itemPath == null) throw new ArgumentNullException("itemPath");

            _provider = provider;
            _itemPath = itemPath;
        }


        public override WebResponse GetResponse()
        {
            if (_response != null) return _response;

            if (this.Method == IOConnection.WrmDeleteFile)
            {
                var isOk = _provider.Delete(_itemPath);

                if (!isOk)
                    throw new InvalidOperationException(string.Format("OneDrive: Delete of item {0} failed", _itemPath));

                _response = new KeeAnywhereWebResponse();
            }
            else if (this.Method == IOConnection.WrmMoveFile)
            {
                //TODO: Is check for same account needed?
                var destUrl = new StorageProviderUri(Headers[IOConnection.WrhMoveFileTo]);
                var itemDestPath = destUrl.GetPath();

                _provider.Move(_itemPath, itemDestPath);

                return new KeeAnywhereWebResponse();
            }
            else if (this.Method == WebRequestMethods.Http.Post)
            {
                var isOk = _provider.Save(_requestStream, _itemPath);
                if (!isOk)
                {
                    throw new InvalidOperationException(string.Format("OneDrive: Upload to folder {0} failed", _itemPath));
                }

                _response = new KeeAnywhereWebResponse();
            }
            else // Get File
            {
                var stream = _provider.Load(_itemPath);

                _response = new KeeAnywhereWebResponse(stream);
            }

            return _response;
        }

        public override Stream GetRequestStream()
        {
            if (_requestStream == null)
                _requestStream = new MemoryStream();

            return _requestStream;
        }
    }
}