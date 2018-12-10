using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CMS
    {
        public string TITLE
        { get; set; }

        public DateTime RELEASE_DATE
        { get; set; }

        public DateTime LASTUPDATED
        { get; set; }

        public string CATEGORY_NAME
        { get; set; }

        public string RECORD_ID
        { get; set; }

        public string RECORD_IMG
        { get; set; }

        public string HYPER_LINK
        { get; set; }

        public string ABSTRACT
        { get; set; }

        public DateTime EVENT_START
        { get; set; }

        public DateTime EVENT_END
        { get; set; }

        public string COUNTRY
        { get; set; }

        public string CITY
        { get; set; }

        public string BOOTH
        { get; set; }

        public string CONTACT_NAME
        { get; set; }

        public string CONTACT_PHONE
        { get; set; }

        public string CONTACT_EMAIL
        { get; set; }

        public string AP_TYPE
        { get; set; }

        public int HOURS
        { get; set; }

        public int MINUTE
        { get; set; }

        public int SECOND
        { get; set; }

        public int CLICKTIME
        { get; set; }
        public string contentX
        { get; set; }
        public CMSType cmsTypeX
        {
            get
            {
                CMSType ct = CMSType.NA;
                if (Enum.TryParse(this.CATEGORY_NAME.Replace(" ", "_").Replace("/", "Slash"), out ct))
                    return ct;
                else
                    return CMSType.NA;
            }
        }
    }
}
