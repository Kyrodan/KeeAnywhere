using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KeePassLib.Native;

namespace KeeAnywhere.WebRequest
{
    class RequestStream : Stream
    {
        private readonly KeeAnywhereWebRequest request;

        public RequestStream(KeeAnywhereWebRequest request)
        {
            this.request = request;
        }

        public override void Close()
        {
            if (NativeLib.IsUnix()) // mono does not automatically call GetResponse
            {
                this.request.GetResponse();
            }
            base.Close();
        }

        List<byte> bytes = new List<byte>();
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.bytes.AddRange(buffer.Skip(offset).Take(count));
        }

        internal Stream GetReadableStream()
        {
            return new MemoryStream(this.bytes.ToArray());
        }
    }
}