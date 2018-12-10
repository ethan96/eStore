using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class Events
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

        public string HOURS
        { get; set; }

        public string MINUTE
        { get; set; }

        public string SECOND
        { get; set; }

        public string CLICKTIME
        { get; set; }
    }
}
