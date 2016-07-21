using System;
using System.IO;

namespace KeeAnywhere.WebRequest
{
    // Resolution for Issue #44: Sometimes can't load kdbx file (Dropbox, hubiC)
    // Flush is called in CryptoStream from KeePass. But System.Net.Http.StreamContent.ReadOnlyStream
    // is read-only and therefore no flush is needed.
    internal class WrapperStream : Stream
    {
        private readonly Stream m_s;
        protected Stream BaseStream
        {
            get { return m_s; }
        }

        public override bool CanRead
        {
            get { return m_s.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_s.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return m_s.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return m_s.CanWrite; }
        }

        public override long Length
        {
            get { return m_s.Length; }
        }

        public override long Position
        {
            get { return m_s.Position; }
            set { m_s.Position = value; }
        }

        public override int ReadTimeout
        {
            get { return m_s.ReadTimeout; }
            set { m_s.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return m_s.WriteTimeout; }
            set { m_s.WriteTimeout = value; }
        }

        public WrapperStream(Stream sBase) : base()
        {
            if (sBase == null) throw new ArgumentNullException("sBase");

            m_s = sBase;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset,
            int count, AsyncCallback callback, object state)
        {
            return m_s.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset,
            int count, AsyncCallback callback, object state)
        {
            return BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            m_s.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return m_s.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            m_s.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            if (m_s.CanWrite)
                m_s.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_s.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return m_s.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_s.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            m_s.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_s.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            m_s.WriteByte(value);
        }
    }
}