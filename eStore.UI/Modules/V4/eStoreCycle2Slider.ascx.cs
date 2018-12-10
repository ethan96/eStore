using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules.V4
{
    public partial class eStoreCycle2Slider : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<POCOS.Advertisement> Advertisements { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected virtual string generateAdvertisementsHTML()
        {
            if (Advertisements == null || Advertisements.Count == 0)
                return string.Empty;

            StringBuilder sbhtml = new StringBuilder();

            sbhtml.Append("<div class=\"eStore_index_banner\">");
            sbhtml.AppendFormat("<div class=\"carouselBanner\" id=\"{0}\">", this.ClientID);
            sbhtml.Append("<ul>");
            for (int i = 0; i < Advertisements.Count(); i++)
            {
                POCOS.Advertisement ad = Advertisements[i];
                sbhtml.Append("<li>");


                sbhtml.AppendFormat("<a href=\"{0}\" title=\"{1}\" target=\"{2}\">", ad.Hyperlink, ad.Title, ad.Target);
                sbhtml.AppendFormat("<div style=\"background-image: url({0}\");>"
     , ad.imagefileX);
                sbhtml.AppendFormat("</div><img src=\"{0}\"  alt=\"{1}\"></a> </li>", ad.smallImagefileX, ad.Title);

            }
            sbhtml.Append("</ul>");
            sbhtml.Append("<div class=\"clearfix\">");
            sbhtml.Append("</div>");
            sbhtml.AppendFormat("<div class=\"carousel-controlCenter\" id=\"{0}_bannercontrolCenter\">", this.ClientID);
            sbhtml.Append("<a id=\"prev\" class=\"prev\" href=\"#\"></a><a id=\"next\" class=\"next\" href=\"#\"></a><span");
            sbhtml.Append("id=\"pager\" class=\"pager\"></span>");
            sbhtml.Append("</div>");
            sbhtml.Append("</div>");
            sbhtml.Append("</div>");

            return sbhtml.ToString();

        }
    }
}