using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class AdRotatorSelect : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public int BannerWidth { get; set; }
        private int[] avaiableWidth = { 780, 470, 781 };
        public List<POCOS.Advertisement> BannerList { get; set; }
        public bool showSmallImage { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            BindData();
            this.BindScript("url", "BannerJS", "HomeBanner/js/scripts.js");
            this.AddStyleSheet(ResolveUrl("~/Scripts/HomeBanner/style.css"));
            base.OnPreRender(e);
        }
        
        private void BindData()
        {
            if (BannerList != null && BannerList.Count > 0)
            {
                if (avaiableWidth.Contains( BannerWidth))
                {
                    plBanner.CssClass = "SliderBanner"+BannerWidth.ToString();
                }
                plBanner.Visible = true;
                StringBuilder BannerContent = new StringBuilder();
                StringBuilder BannerJS = new StringBuilder();
                BannerJS.Append("<script>if (!window.slider)var slider = {};");
                int i = 1;
                foreach (var item in BannerList)
                {
                    string _target = string.IsNullOrEmpty(item.Target) ? "" : string.Format(" target='{0}'",item.Target);
                    BannerContent.Append("<a href='" + item.Hyperlink + "'" + _target + "><img id='slide-img-" + i + "' src='" + esUtilities.CommonHelper.GetStoreLocation(false) + "resource" + item.Imagefile + "' class='slide' title='" + item.AlternateText + "' /></a>");
                    if (i==1)
                        BannerJS.Append("slider.data = [{ 'id': 'slide-img-1', 'client': 'nature beauty', 'desc': '" + item.AlternateText + "'}");
                    else
                        BannerJS.Append(",{'id': 'slide-img-" + i + "', 'client': 'nature beauty', 'desc': '" + item.AlternateText + "'}");
                    if (i == BannerList.Count)
                        BannerJS.Append("]");
                    i++;
                }
                BannerJS.Append("</script>");
                ltBannerContent.Text = BannerContent.ToString();
                ltBannerJS.Text = BannerJS.ToString();
            }
            else
                plBanner.Visible = false;
        }
    }
}