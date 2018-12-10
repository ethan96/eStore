using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    class getAdBanner : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            Dictionary<string, string> keywrods = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                keywrods.Add(key,System.Web.HttpUtility.UrlDecode(context.Request.QueryString[key]));
            }
            keywrods.Remove("func");
            IList<POCOS.Advertisement> ads = eStoreContext.Current.Store.getAdBanner(eStoreContext.Current.MiniSite,keywrods);
            var rlt = (from ad in ads
                       where (!string.IsNullOrEmpty(ad.Imagefile) || !string.IsNullOrEmpty(ad.HtmlContent))
                       select new JObject {
                    new JProperty("id", ad.id),
                    new JProperty("HtmlContent", string.IsNullOrEmpty(ad.AdvContext) ? eStore.BusinessModules.CMSManager.reSetResourceCMSContext(ad,eStoreContext.Current.Store) : ad.AdvContext),
                    new JProperty("type", ad.AdType),
                    new JProperty("Hyperlink",esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink)),
                    //new JProperty("Title", ad.Title),
                    new JProperty("Title", ad.AlternateText),
                    //new JProperty("image",ad.Imagefile.StartsWith("http",true,null)?ad.Imagefile:esUtilities.CommonHelper.GetStoreFtpServerPath(ad.Imagefile)),
                    new JProperty("image",string.IsNullOrEmpty(ad.Imagefile)?"":(ad.Imagefile.StartsWith("http",true,null)?ad.Imagefile:String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation() , ad.Imagefile ))),
                    new JProperty("Target", string.IsNullOrEmpty(ad.Target)?"_self": ad.Target),
                    new JProperty("LocationPath", string.IsNullOrEmpty(ad.LocationPath)?"": ad.LocationPath)
                    });

            return JsonConvert.SerializeObject(rlt);
        }
    }
}