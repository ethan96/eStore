using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace eStore.UI
{
    public partial class SiteMap : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            string path = string.Format(@"C:\\{1}\\Sitemap\\Sitemap_{0}.xml", Presentation.eStoreContext.Current.Store.profile.StoreID, Presentation.eStoreContext.Current.getStringSetting("eStoreResources3C", "eStoreResources3C"));
            xmldoc.Load(path);
            Response.ContentType = "application/xml";
            Response.Charset = "utf-8";
            Response.Write(xmldoc.InnerXml);
            Response.End();
        }
    }
}