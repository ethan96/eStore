using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    /// <summary>
    /// 仅用于内部用户
    /// </summary>
    public class InternalHDIResponseFilter : System.IO.Stream
    {
        private System.IO.Stream baseStream;

        public InternalHDIResponseFilter(System.IO.Stream responsestream)
        {
            if (responsestream == null)
                throw new ArgumentNullException("Responsestream is nothing");
            this.baseStream = responsestream;
        }

        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string originalText = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
            originalText = originalText.Replace("http://advantech.vo.llnwd.net/o35/eStore/", "/");
            buffer = System.Text.Encoding.UTF8.GetBytes(originalText);
            this.baseStream.Write(buffer, 0, buffer.Length);
        }
    }
}
