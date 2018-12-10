using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Advertisement
    {
        public Advertisement() { }
        public Advertisement(POCOS.Advertisement advertisement)
        {
            this.id = advertisement.id;
            this.id = advertisement.id;
            this.id = advertisement.id;
            this.AdType = advertisement.AdType;
            this.Title = advertisement.Title;
            this.Hyperlink = advertisement.Hyperlink;
            this.Target = advertisement.Target;
            this.AlternateText = advertisement.AlternateText;
            this.Seq = advertisement.Seq;
            this.Imagefile = advertisement.Imagefile;
            this.HtmlContent = advertisement.HtmlContent;
            this.Map = advertisement.Map;
        }
        public virtual int id
        {
            get;
            set;
        }
        public virtual string AdType
        {
            get;
            set;
        }

        public virtual string Title
        {
            get;
            set;
        }
        public virtual string Hyperlink
        {
            get;
            set;
        }

        public virtual string Target
        {
            get;
            set;
        }
        public virtual string AlternateText
        {
            get;
            set;
        }

        public virtual Nullable<int> Seq
        {
            get;
            set;
        }
        public virtual string Imagefile
        {
            get;
            set;
        }
   public virtual string SmallImagefile
        {
            get {
                if (!string.IsNullOrEmpty(Imagefile) && Imagefile.Contains("."))
                {
                    return Imagefile.Insert(Imagefile.LastIndexOf('.') , "_S");
                }
                else
                    return string.Empty;
            }
        }
        public virtual string HtmlContent
        {
            get;
            set;
        }
        public virtual string Map
        {
            get;
            set;
        }
    }
}