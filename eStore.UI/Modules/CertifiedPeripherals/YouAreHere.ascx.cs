using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class YouAreHere : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.PStoreProductCategory productCategory { get; set; }
        public string ProductName { get; set; }
        public StringBuilder sbYouarehere = new StringBuilder();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected override void OnPreRender(EventArgs e)
        {

            if (!string.IsNullOrEmpty(ProductName))
            {
                if (productCategory != null)
                {
                    CreatYouareHeres(productCategory, true);
                }
            }
            else
            {
                if (productCategory != null )
                {
                    CreatYouareHeres(productCategory, true);
                }
            }
            sbYouarehere.Insert(0, string.Format(" <ul class=\"youarehere\"><li class=\"home\"><a href=\"{0}\">{1}</a></li><li><a href=\"{2}\">{3}</a></li>"
            , ResolveUrl("~/")
            , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home)
            , ResolveUrl("~/CertifiedPeripherals/Default.aspx")
            , eStore.Presentation.eStoreLocalization.Tanslation("Certified Peripherals")
            ));

            sbYouarehere.Append("</ul>");
            lYouarehere.Text = sbYouarehere.ToString();

            base.OnPreRender(e);
        }


        void CreatYouareHeres(POCOS.PStoreProductCategory category, bool isLink)
        {
            if (isLink)

                sbYouarehere.Insert(0, string.Format(" <li><a href=\"{0}\">{1}</a></li>"
                     , ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category))
                     , esUtilities.CommonHelper.RemoveHtmlTags(category.DisplayName, true)));
            else
                sbYouarehere.Insert(0, string.Format(" <li>{0}</li>"
                            , esUtilities.CommonHelper.RemoveHtmlTags(category.DisplayName, true)));
            if (category.parentX != null)
            {
                CreatYouareHeres(category.parentX, true);
            }
        }
    }

}