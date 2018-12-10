using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    class getPstoreAdBanner : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            Dictionary<string, string> keywrods = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                keywrods.Add(key, System.Web.HttpUtility.UrlDecode(context.Request.QueryString[key]));
            }
            keywrods.Remove("func");
            POCOS.MiniSite miniSite = Presentation.eStoreContext.Current.Store.profile.MiniSites.FirstOrDefault(x => x.SiteName == "CertifiedPeripherals");
            BusinessModules.PStore pstore =   BusinessModules.PStore.getInstance(eStoreContext.Current.Store.profile);
            IList<POCOS.Advertisement> ads = pstore.getAdBanner(miniSite, keywrods);
            var rlt = (from ad in ads
                       where ad.Imagefile != null
                       select new JObject {
                    new JProperty("id", ad.id),
                    new JProperty("HtmlContent", string.IsNullOrEmpty(ad.htmlContentX)?string.Empty:ad.htmlContentX),
                    new JProperty("type", ad.AdType),
                    new JProperty("Hyperlink",esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink)),
                    //new JProperty("Title", ad.Title),
                    new JProperty("Title", ad.AlternateText),
                    //new JProperty("image",ad.Imagefile.StartsWith("http",true,null)?ad.Imagefile:esUtilities.CommonHelper.GetStoreFtpServerPath(ad.Imagefile)),
                    new JProperty("image",ad.Imagefile.StartsWith("http",true,null)?ad.Imagefile:String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation() , ad.Imagefile )),
                    new JProperty("Target", string.IsNullOrEmpty(ad.Target)?"_self": ad.Target),
                    new JProperty("LocationPath", string.IsNullOrEmpty(ad.LocationPath)?"": ad.LocationPath)
                    });

            return JsonConvert.SerializeObject(rlt);
        }
    }
}