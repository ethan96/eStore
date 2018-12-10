using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
namespace eStore.UI.Modules
{
    public partial class eStoreLiquidSlider : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public enum NavigationType {
            Tabs, Numbers, Thumbnail
        }
        public NavigationType navigationType
        {
            get;
            set;
        }

        public int MinHeight { get; set; }
        public bool showDescription { get; set; }

        private bool _dynamicArrows=true;
        public bool dynamicArrows { 
            get { return _dynamicArrows; }
            set { _dynamicArrows = value; }
        }
        private bool _hoverArrows = true;
        public bool hoverArrows
        {
            get { return _hoverArrows; }
            set { _hoverArrows = value; }
        }

        private bool _dynamicTabs = true;
        public bool dynamicTabs
        {
            get { return _dynamicTabs; }
            set { _dynamicTabs = value; }
        } 
        public List<POCOS.Advertisement> Advertisements { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindScript("url", "liquid-slider", "jquery.liquid-slider.js");
            this.AddStyleSheet(ResolveUrl("~/Styles/liquid-slider.css"));
        }
        protected string generateAdvertisementsHTML()
        {
            if (Advertisements == null || Advertisements.Count == 0)
                return string.Empty;

            StringBuilder sbhtml = new StringBuilder();
            sbhtml.AppendFormat("<div class=\"liquid-slider\" id=\"{0}\">", this.ClientID);
            for (int i = 0; i < Advertisements.Count(); i++)
            {
                POCOS.Advertisement ad = Advertisements[i];
                sbhtml.Append("<div>");
                sbhtml.AppendFormat("<h2 class=\"title\">{0}</h2>", ad.Title);

                if (string.IsNullOrEmpty(ad.htmlContentX) || navigationType == eStoreLiquidSlider.NavigationType.Thumbnail)
                {
                    string _target = string.IsNullOrEmpty(ad.Target) ? "" : string.Format(" target='{0}'", ad.Target);
                    //sbhtml.AppendFormat("<a href='" + ad.Hyperlink + "'" + _target + "><img id='slide-img-" + i + "' src='" + ResolveUrl("~/resource" + ad.Imagefile) + "' class='slide' title='" + ad.AlternateText + "' /></a>");
                    //remove imge link if you more button
                    if (showDescription)
                    {
                        sbhtml.AppendFormat("<img id='slide-img-" + i
                            + "' src='" + (ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.Imagefile))
                            + ((navigationType == eStoreLiquidSlider.NavigationType.Thumbnail) ? ("' thumbnail='" + (ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.htmlContentX))) : string.Empty)
                            + "' class='slide' title='" + esUtilities.StringUtility.replaceSpecialString(ad.AlternateText) + "' />");

                        sbhtml.AppendFormat("<div  class=\""+
                            (navigationType == eStoreLiquidSlider.NavigationType.Thumbnail ? "contenttop" : "content")
                            +"\"><h2 class=\"contenttitle\">{0}</h2>{1}<div class=\"more\"><a href=\"{2}\">Learn More</a></div></div>", ad.Title, ad.AlternateText, esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink));
                    }
                    else
                    {
                        sbhtml.AppendFormat("<a href='" + esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink) + "'" + _target + "><img id='slide-img-" + i 
                            + "' src='" + (ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.Imagefile))
                            + ((navigationType == eStoreLiquidSlider.NavigationType.Thumbnail) ? ("' thumbnail='" + (ad.Imagefile.StartsWith("http", true, null) ? ad.Imagefile : String.Format("{0}resource{1}", esUtilities.CommonHelper.GetStoreLocation(), ad.htmlContentX))) : string.Empty)
                            + "' class='slide' title='" + esUtilities.StringUtility.replaceSpecialString(ad.AlternateText) + "' /></a>");
                   
                    }
                }
                else
                {
                    sbhtml.AppendFormat(ad.htmlContentX);
                }

                sbhtml.Append("</div>");
            }

            sbhtml.Append("</div>");

            bool autoSlide = true;
            //add scripts
            sbhtml.Append("<script type=\"text/javascript\" language=\"javascript\">$('#");
            sbhtml.Append(this.ClientID);
            sbhtml.Append("').liquidSlider({ dynamicTabsPosition: 'bottom', swipe: false, includeTitle: false");
            
            if (navigationType == eStoreLiquidSlider.NavigationType.Tabs)
            {
                sbhtml.Append(",dynamicTabsNumber: false");
            }
            else if (navigationType == eStoreLiquidSlider.NavigationType.Thumbnail)
            {
                sbhtml.Append(",dynamicTabsThumbnail: true");
               
            }
            if (MinHeight > 0)
            {
                sbhtml.Append(",autoHeight: false");
                sbhtml.Append(",minHeight: "+MinHeight);
            }
            if (Advertisements.Count() == 1)
            {
                dynamicArrows = false;
                //dynamicTabs = false;
                autoSlide = false;
             
            }
            sbhtml.AppendFormat(",autoSlide: {0}", autoSlide.ToString().ToLower());

            sbhtml.AppendFormat(",dynamicTabs: {0}", dynamicTabs.ToString().ToLower());
            sbhtml.AppendFormat(",dynamicArrows: {0}", dynamicArrows.ToString().ToLower());
            sbhtml.AppendFormat(",hoverArrows: {0}", hoverArrows.ToString().ToLower());

            sbhtml.Append("});");

            if (dynamicTabs && navigationType==NavigationType.Tabs)
            {
                sbhtml.Append("adjustTabWdith();");
            }
            sbhtml.Append("</script>");
            return sbhtml.ToString();
        }
    }
}