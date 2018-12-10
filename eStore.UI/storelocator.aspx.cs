using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI
{
    public partial class storelocator : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String userIP =  Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userIP))
                userIP = string.IsNullOrEmpty( Request.UserHostAddress) ? "" : Request.UserHostAddress;
            else if (userIP.Contains(","))
            {
                userIP = userIP.Split(',').First().Trim();
            }
            String referralName = Request["f"];
            String referrer = String.Empty;
            String redirectURL = String.Empty;

           
                referrer = String.IsNullOrEmpty(referralName) ? "www.advantech.com" : referralName;

                string  url =  (getStoreURL(referrer, userIP));

                if  (string.IsNullOrEmpty(url)==false)
                    Response.Redirect(url);
                else
                    Response.Redirect("http://buy.advantech.com");
           
        }

        public String getStoreURL(String referrer, String userIP)
        {
            String redirectURL = "http://buy.advantech.com";

            switch (referrer.ToUpper())
            {
                case "WWW.ADVANTECH.COM":
                case "ADVANTECH.COM":
                    redirectURL = redirectByStore(userIP);
                    break;

                case "WWW.ADVANTECH.COM.TW":
                case "WWW.ADVANTECH.TW":
                case "ADVANTECH.TW":
                    //redirectURL = "http://buy.advantech.com.tw";
                    redirectURL = redirectByStore(userIP);
                    break;

                case "WWW.ADVANTECH.EU":
                case "ADVANTECH.EU":
                case "WWW.ADVANTECH.EU.COM":
                case "ADVANTECH.EU.COM":
                case "WWW.ADVANTECH.FR":
                case "ADVANTECH.FR":
                case "WWW.ADVANTECH.DE":
                case "ADVANTECH.DE":
                case "WWW.ADVANTECH.IT":
                case "ADVANTECH.IT":
                case "WWW.ADVANTECH-UK.COM":
                case "ADVANTECH-UK.COM":
                case "WWW.ADVANTECH.BE":
                case "ADVANTECH.BE":
                case "WWW.ADVANTECH.PL":
                case "ADVANTECH.PL":
                case "WWW.ADVANTECH.NL":
                case "ADVANTECH.NL":
                case "WWW.ADVANTECH.PT":
                case "ADVANTECH.PT":
                    redirectURL = "http://buy.advantech.eu";
                    break;

                case "WWW.ADVANTECH.COM.CN":
                case "ADVANTECH.CN":
                case "WWW.ADVANTECH.CN":
                case "ADVANTECH.COM.CN":
                    redirectURL = "http://buy.advantech.com.cn";
                    break;

                case "WWW.ADVANTECHKOREA.CO.KR":
                case "WWW.ADVANTECH.CO.KR":
                case "ADVANTECHKOREA.CO.KR":
                case "WWW.ADVANTECHKOREA.COM":
                case "ADVANTECHKOREA.COM":
                case "WWW.ADVANTECH.KR":
                case "ADVANTECH.KR":
                    redirectURL = "http://buy.advantech.co.kr";
                    break;

                case "WWW.ADVANTECH.CO.JP":
                case "ADVANTECH.CO.JP":
                    redirectURL = "http://buy.advantech.co.jp";
                    break;

                case "WWW.ADVANTECH.RU":
                case "ADVANTECH.RU":
                    redirectURL = "http://buy.advantech.com.tw";
                    break;

                //case "WWW.ADVANTECH.IN":
                //case "ADVANTECH.IN":
                //case "WWW.ADVANTECH.COM.MY":
                //case "ADVANTECH.COM.MY":
                //case "WWW.ADVANTECHSG.COM.SG":
                //case "ADVANTECHSG.COM.SG":
                //    redirectURL = "http://buy.advantech.com.my";
                //    break;

                case "WWW.ADVANTECH.NET.AU":
                case "ADVANTECH.NET.AU":
                    redirectURL = "http://buy.advantech.net.au";
                    break;

                //case "WWW.ADVANTECH.COM.BR":
                //case "ADVANTECH.COM.BR":
                default:
                    //redirectURL = "http://buy.advantech.com";
                    redirectURL = redirectByStore(userIP);
                    break;
            }

            return redirectURL;
        }

        private String redirectByStore(String userIP)
        {
            StoreSolution solution = StoreSolution.getInstance();
            string _countryCode = solution.getCountryCodeByIp(userIP);
            Store store = solution.locateStore( _countryCode);
            if (store != null)
                return String.Format("http://{0}", store.profile.StoreURL.Trim());
            else
                return "http://buy.advantech.com";
        }
    }
}