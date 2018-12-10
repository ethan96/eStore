using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
namespace eStore.Presentation
{
    public class OpenGraphProtocol
    {
        public enum OpenGraphProtocolType { Product, Category, WebSite }
        public string title { get; set; }
        public OpenGraphProtocolType type { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string locale { get; set; }
        public string site_name { get; set; }
        public string audio { get; set; }
        public string video { get; set; }
    }
    public partial class OpenGraphProtocolAdapter
    {
        private OpenGraphProtocol openGraphProtocol;
        public OpenGraphProtocolAdapter(POCOS.Product product)
        {
            openGraphProtocol = new OpenGraphProtocol();
            openGraphProtocol.title = product.name;
            openGraphProtocol.type = OpenGraphProtocol.OpenGraphProtocolType.Product;
            openGraphProtocol.url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product).Replace("~/", esUtilities.CommonHelper.GetStoreLocation());
            openGraphProtocol.image = product.thumbnailImageX;
            openGraphProtocol.description = product.productDescX;
            if (Presentation.eStoreContext.Current.Store != null && Presentation.eStoreContext.Current.Store.profile != null)
                openGraphProtocol.site_name = Presentation.eStoreContext.Current.Store.profile.Title;
        }
        public OpenGraphProtocolAdapter(string PageName)
        {
            switch (PageName)
            {
                case "HomePage":
                    if (Presentation.eStoreContext.Current.Store != null && Presentation.eStoreContext.Current.Store.profile != null)
                    {
                        openGraphProtocol = new OpenGraphProtocol();
                        openGraphProtocol.title = Presentation.eStoreContext.Current.Store.profile.Title;
                        openGraphProtocol.type = OpenGraphProtocol.OpenGraphProtocolType.WebSite;
                        openGraphProtocol.url = esUtilities.CommonHelper.GetStoreLocation();

                        String imgUrl = esUtilities.CommonHelper.GetStoreLocation() + "App_Themes/Default/{0}/logo.jpg";
                        if (Presentation.eStoreContext.Current.MiniSite != null)
                            imgUrl = String.Format(imgUrl, Presentation.eStoreContext.Current.MiniSite.SiteName);
                        else
                            imgUrl = imgUrl.Replace("/{0}", "");

                        openGraphProtocol.image = imgUrl;
                        openGraphProtocol.description = Presentation.eStoreContext.Current.Store.profile.MetaDesc;
                        openGraphProtocol.site_name = Presentation.eStoreContext.Current.Store.profile.Title;
                    }
                    break;
                default:
                    break;

            }
        }
        public bool addOpenGraphProtocolMetedata(System.Web.UI.Page page)
        {
            if (page == null|| this.openGraphProtocol==null)
            { return false; }
            else
            {
                HtmlMeta ogType = new HtmlMeta();
                ogType.Attributes.Add("property", "og:type");
                ogType.Attributes.Add("content", openGraphProtocol.type.ToString());
                page.Header.Controls.Add(ogType);

                HtmlMeta ogTitle = new HtmlMeta();
                ogTitle.Attributes.Add("property", "og:title");
                ogTitle.Attributes.Add("content", openGraphProtocol.title);
                page.Header.Controls.Add(ogTitle);

                HtmlMeta ogUrl = new HtmlMeta();
                if (!string.IsNullOrEmpty(openGraphProtocol.url))
                {
                    ogUrl.Attributes.Add("property", "og:url");
                    ogUrl.Attributes.Add("content", openGraphProtocol.url);
                    page.Header.Controls.Add(ogUrl);
                }

                if (!string.IsNullOrEmpty(openGraphProtocol.image))
                {
                    HtmlMeta ogImage = new HtmlMeta();
                    ogImage.Attributes.Add("property", "og:image");
                    ogImage.Attributes.Add("content", openGraphProtocol.image);
                    page.Header.Controls.Add(ogImage);
                }

                if (!string.IsNullOrEmpty(openGraphProtocol.description))
                {
                    HtmlMeta ogDesc = new HtmlMeta();
                    ogDesc.Attributes.Add("property", "og:description");
                    ogDesc.Attributes.Add("content", openGraphProtocol.description);
                    page.Header.Controls.Add(ogDesc);
                }

                if (!string.IsNullOrEmpty(openGraphProtocol.site_name))
                {
                    HtmlMeta ogSiteName = new HtmlMeta();
                    ogSiteName.Attributes.Add("property", "og:site_name");
                    ogSiteName.Attributes.Add("content", openGraphProtocol.site_name);
                    page.Header.Controls.Add(ogSiteName);
                }


                return true;
            }
        }


   
    }
}
