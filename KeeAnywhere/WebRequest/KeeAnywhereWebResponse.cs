using System;
using System.IO;
using System.Net;

namespace KeeAnywhere.WebRequest
{
    public sealed class KeeAnywhereWebResponse: WebResponse
    {
        private readonly Stream _stream;

        public KeeAnywhereWebResponse()
        {
        }

        public KeeAnywhereWebResponse(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _stream = stream;
        }

        public override Stream GetResponseStream()
        {
            return _stream;
        }

    }
}
