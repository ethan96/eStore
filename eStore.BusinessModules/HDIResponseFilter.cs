using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    public class HDIResponseFilter : System.IO.Stream
    {
        private System.IO.Stream baseStream;

        public HDIResponseFilter(System.IO.Stream responsestream)
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
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string originalText = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
            originalText = originalText.Replace("\"/App_themes/", "\"/App_Themes/");
            originalText = originalText.Replace("\"../App_themes/", "\"/App_Themes/");
            originalText = originalText.Replace("\"../App_Themes/", "\"/App_Themes/");
            originalText = originalText.Replace("\"App_themes/", "\"/App_Themes/");
            originalText = originalText.Replace("\"~/App_themes/", "\"/App_Themes/");

            originalText = originalText.Replace("'/App_themes/", "'/App_Themes/");
            originalText = originalText.Replace("'../App_themes/", "'/App_Themes/");
            originalText = originalText.Replace("'../App_Themes/", "'/App_Themes/");
            originalText = originalText.Replace("'~/App_themes/", "'/App_Themes/");
            originalText = originalText.Replace("'App_themes/", "'/App_Themes/");

            originalText = originalText.Replace("'/App_Themes/", "'http://advantech.vo.llnwd.net/o35/eStore/App_Themes/");
            originalText = originalText.Replace("\"/App_Themes/", "\"http://advantech.vo.llnwd.net/o35/eStore/App_Themes/");
            originalText = originalText.Replace("\"App_Themes/", "\"http://advantech.vo.llnwd.net/o35/eStore/App_Themes/");
            originalText = originalText.Replace("'App_Themes/", "'http://advantech.vo.llnwd.net/o35/eStore/App_Themes/");

            originalText = originalText.Replace("\"~/Scripts/", "\"/Scripts/");
            originalText = originalText.Replace("\"../Scripts/", "\"/Scripts/");

            originalText = originalText.Replace("'~/Scripts/", "'/Scripts/");
            originalText = originalText.Replace("'../Scripts/", "'/Scripts/");

            originalText = originalText.Replace("\"/Scripts/", "\"http://advantech.vo.llnwd.net/o35/eStore/Scripts/");
            originalText = originalText.Replace("'/Scripts/", "'http://advantech.vo.llnwd.net/o35/eStore/Scripts/");
            originalText = originalText.Replace("'Scripts/", "'http://advantech.vo.llnwd.net/o35/eStore/Scripts/");
            originalText = originalText.Replace("\"Scripts/", "\"http://advantech.vo.llnwd.net/o35/eStore/Scripts/");

            originalText = originalText.Replace("\"~/images/", "\"/reimagessource/");
            originalText = originalText.Replace("\"../images/", "\"/images/");
            originalText = originalText.Replace("\"~/Images/", "\"/images/");
            originalText = originalText.Replace("\"../Images/", "\"/images/");
            originalText = originalText.Replace("\"/Images/", "\"/images/");

            originalText = originalText.Replace("'/images/", "'/images/");
            originalText = originalText.Replace("'../images/", "'/images/");
            originalText = originalText.Replace("'/Images/", "'/images/");
            originalText = originalText.Replace("'../Images/", "'/images/");
            originalText = originalText.Replace("'/Images/", "'/images/");
            originalText = originalText.Replace("Images/", "images/");

            originalText = originalText.Replace("'/images/", "'http://advantech.vo.llnwd.net/o35/eStore/images/");
            originalText = originalText.Replace("\"/images/", "\"http://advantech.vo.llnwd.net/o35/eStore/images/");
            originalText = originalText.Replace("'images/", "'http://advantech.vo.llnwd.net/o35/eStore/images/");
            originalText = originalText.Replace("\"images/", "\"http://advantech.vo.llnwd.net/o35/eStore/images/");

            originalText = originalText.Replace("\\resource/", "/resource/");
            originalText = originalText.Replace("\\resource\\", "/resource/");
            originalText = originalText.Replace("\"~/resource/", "\"/resource/");
            originalText = originalText.Replace("\"../resource/", "\"/resource/");
            originalText = originalText.Replace("\"~/Resource/", "\"/resource/");
            originalText = originalText.Replace("\"../Resource/", "\"/resource/");
            originalText = originalText.Replace("\"/Resource/", "\"/resource/");

            originalText = originalText.Replace("'~/resource/", "'/resource/");
            originalText = originalText.Replace("'../resource/", "'/resource/");
            originalText = originalText.Replace("'/Resource/", "'/resource/");
            originalText = originalText.Replace("'../Resource/", "'/resource/");
            originalText = originalText.Replace("'/Resource/", "'/resource/");

            originalText = originalText.Replace("'/resource/", "'http://advantech.vo.llnwd.net/o35/eStore/");
            originalText = originalText.Replace("\"/resource/", "\"http://advantech.vo.llnwd.net/o35/eStore/");
            originalText = originalText.Replace("'resource/", "'http://advantech.vo.llnwd.net/o35/eStore/");
            originalText = originalText.Replace("\"resource/", "\"http://advantech.vo.llnwd.net/o35/eStore/");

            originalText = originalText.Replace("https://buy.advantech.com", "http://buy.advantech.com");

            originalText = System.Text.RegularExpressions.Regex.Replace(originalText, @"\s+(?=<)|\s+$|(?<=>)\s+", "");
            buffer = System.Text.Encoding.UTF8.GetBytes(originalText);

            this.baseStream.Write(buffer, 0, buffer.Length);
        }
    }
}
