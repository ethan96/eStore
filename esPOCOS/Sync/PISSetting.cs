using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.Sync
{
    public class PISSetting
    {
        public string Lang { get; set; }
        public string WSite { get; set; }
        public string HttpsSite { get; set; }
        public string SupportSite { get; set; }

        public PISSetting(string storeid)
        {
            switch (storeid)
            {
                case "ACN":
                    Lang = "CHS";
                    WSite = "http://www.advantech.com.cn";
                    HttpsSite = "https://www.advantech.com.cn";
                    SupportSite = "http://support.advantech.com.cn";
                    break;
                case "ATW":
                    Lang = "CHT";
                    WSite = "http://www.advantech.com.tw";//www.advantech.tw
                    HttpsSite = "https://www.advantech.com.tw";//www.advantech.tw
                    SupportSite = "http://support.advantech.com.tw";
                    break;
                case "AKR":
                    Lang = "KOR";
                    WSite = "http://www.advantech.co.kr";
                    HttpsSite = "https://www.advantech.co.kr";
                    SupportSite = "http://support.advantech.co.kr";
                    break;
                case "AJP":
                    Lang = "JP";
                    WSite = "http://www.advantech.co.jp";
                    HttpsSite = "https://www.advantech.co.jp";
                    SupportSite = "http://support.advantech.co.jp";
                    break;
                default:
                    Lang = "ENU";
                    WSite = "http://www.advantech.com";
                    HttpsSite = "https://www.advantech.com";
                    SupportSite = "http://support.advantech.com";
                    break;

            }
        }

        public static PISSetting GetCurrent(string storeid)
        {
            return new PISSetting(storeid);
        }
    }
}
