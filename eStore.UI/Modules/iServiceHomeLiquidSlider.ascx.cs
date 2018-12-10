using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class iServiceHomeLiquidSlider : System.Web.UI.UserControl
    {

        protected string headerpromote
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            List<POCOS.Advertisement> Advertisements = Presentation.eStoreContext.Current.Store.getiServiceHomeBanners(Presentation.eStoreContext.Current.MiniSite);

            this.eStoreLiquidSlider1.Advertisements = Advertisements.Where(x => x.segmentType == eStore.POCOS.Advertisement.AdvertisementType.HomeBanner).ToList();


            System.Text.StringBuilder sbheaderpromote = new System.Text.StringBuilder();

            List<POCOS.Advertisement> headerpromoteads = Advertisements.Where(x => x.segmentType == eStore.POCOS.Advertisement.AdvertisementType.TodayHighLight).Take(4).ToList();

            if (headerpromoteads.Count() == 4)
            {
                sbheaderpromote.AppendFormat("<div class=\"headerpromote clearfix\">");
                sbheaderpromote.AppendFormat("    <div class=\"blk_pmo\">");
                sbheaderpromote.AppendFormat("        <div class=\"pmo_firstrow clearfix\">");
                sbheaderpromote.Append(getPromoteItem(headerpromoteads[0]));
                sbheaderpromote.Append(getPromoteItem(headerpromoteads[1]));
                sbheaderpromote.AppendFormat("            <div class=\"clear\">");
                sbheaderpromote.AppendFormat("            </div>");
                sbheaderpromote.AppendFormat("        </div>");
                sbheaderpromote.AppendFormat("        <!-- end pmo_firstrow -->");
                sbheaderpromote.AppendFormat("        <div class=\"pmo_secondrow clearfix\">");

                sbheaderpromote.Append(getPromoteItem(headerpromoteads[2]));
                sbheaderpromote.Append(getPromoteItem(headerpromoteads[3]));
                sbheaderpromote.AppendFormat("        </div>");
                sbheaderpromote.AppendFormat("        <!-- end pmo_secondrow -->");
                sbheaderpromote.AppendFormat("    </div>");
                sbheaderpromote.AppendFormat("    <!-- end blk_pmo -->");
                sbheaderpromote.AppendFormat("</div>");
            }

            headerpromote = sbheaderpromote.ToString();
        }


        string getPromoteItem(POCOS.Advertisement ad)
        {
            System.Text.StringBuilder sbheaderpromote = new System.Text.StringBuilder();
            POCOS.Part part = Presentation.eStoreContext.Current.Store.getProduct(ad.HtmlContent);
            decimal price = part == null ? 0m : part.getListingPrice().value;
            string url = part == null ? ad.Hyperlink : Presentation.UrlRewriting.MappingUrl.getMappingUrl(part);

            sbheaderpromote.AppendFormat("<div class=\"pmo_repeat\">");
            sbheaderpromote.AppendFormat("<div class=\"pmo_price\">");
            sbheaderpromote.AppendFormat("<div class=\"txt_black\">");
            sbheaderpromote.AppendFormat("Price</div>");
            sbheaderpromote.AppendFormat("{0}</div>", Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(price));
            sbheaderpromote.AppendFormat("<div class=\"pmo_info\">");
            sbheaderpromote.AppendFormat("<div class=\"pmo_category\">");
            sbheaderpromote.AppendFormat("{0}</div>", ad.Title);
            sbheaderpromote.AppendFormat("<div class=\"pmo_no\">");
            sbheaderpromote.AppendFormat("{0}</div>", ad.AlternateText);
            sbheaderpromote.AppendFormat("</div>");
            sbheaderpromote.AppendFormat("<a href=\"{0}\">", esUtilities.CommonHelper.ConvertToAppVirtualPath(url));
            sbheaderpromote.AppendFormat("<img src=\"{0}\" width=\"220\" height=\"150\" alt=\"promote01\"></a></div>", (ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.Imagefile)));

            return sbheaderpromote.ToString();
        }

    }
}