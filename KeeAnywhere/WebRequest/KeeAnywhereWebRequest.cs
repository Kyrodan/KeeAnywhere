using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using KeeAnywhere.StorageProviders;
using KeePassLib.Serialization;

namespace KeeAnywhere.WebRequest
{
    public sealed class KeeAnywhereWebRequest : System.Net.WebRequest
    {
        private readonly IStorageProviderFileOperations _provider;
        private readonly string _itemPath;

        private RequestStream _requestStream;
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

        public KeeAnywhereWebRequest(IStorageProviderFileOperations provider, string itemPath)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (itemPath == null) throw new ArgumentNullException("itemPath");

            _provider = provider;
            _itemPath = itemPath;
        }


        public override WebResponse GetResponse()
        {
            if (_response != null) return _response;

            if (!_provider.IsCompletePathValid(_itemPath)) throw new IOException(string.Format("Path is invalid: {0}", _itemPath));

            if (this.Method == IOConnection.WrmDeleteFile)
            {
                //var isOk = Task.Run(async () => await _provider.Delete(_itemPath));

                //if (!isOk.Result)
                //throw new InvalidOperationException(string.Format("KeeAnywhere: Delete of item {0} failed.", _itemPath));

                //_response = new KeeAnywhereWebResponse();
                throw new InvalidOperationException(string.Format("KeeAnywhere: Delete item {0} not supported.", _itemPath));
            }
            else if (this.Method == IOConnection.WrmMoveFile)
            {
                ////TODO: Is check for same account needed?
                //var destUrl = new StorageUri(Headers[IOConnection.WrhMoveFileTo]);
                //var itemDestPath = destUrl.GetPath();

                //Task.Run(async () => await _provider.Move(_itemPath, itemDestPath));

                //return new KeeAnywhereWebResponse();
                throw new InvalidOperationException(string.Format("KeeAnywhere: Move item {0} not supported.", _itemPath));
            }
            else if (this.Method == WebRequestMethods.Http.Post)
            {
                var task = Task.Run(async () =>
                {
                    using (var stream = this._requestStream.GetReadableStream())
                    {
                        await _provider.Save(stream, _itemPath);
                    }
                });

                task.Wait();
                if (task.IsFaulted)
                {
                    throw new InvalidOperationException(string.Format("KeeAnywhere: Upload to folder {0} failed",
                        _itemPath), task.Exception);
                }

                _response = new KeeAnywhereWebResponse();
            }
            else // Get File
            {
                var task = Task.Run(async () => await _provider.Load(_itemPath));

                // Issue #44: Sometimes can't load kdbx file (Dropbox, hubiC)
                //var wrappeedStream = new WrapperStream(stream.Result); 
                task.Wait();
                var memoryStream = task.Result as MemoryStream;

                if (memoryStream == null)
                {
                    using (task.Result)
                    {
                        memoryStream = new MemoryStream();
                        task.Result.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                    }
                }

                _response = new KeeAnywhereWebResponse(memoryStream);
            }

            return _response;
        }

        public override Stream GetRequestStream()
        {
            if (_requestStream == null)
                _requestStream = new RequestStream(this);

            return _requestStream;
        }
    }
}