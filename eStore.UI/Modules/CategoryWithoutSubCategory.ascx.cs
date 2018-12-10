using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CategoryWithoutSubCategory : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string getFormatedAJAXMinPrice()
        {
            string format = Presentation.eStoreContext.Current.getStringSetting("MinPriceFormat");
            if (string.IsNullOrEmpty(format))
                format = "{0} {1}";
            return string.Format(format
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_From)
                , string.Format("<span id=\"{0}\"><img alt=\"loading...\" src=\"{1}images/priceprocessing.gif\" /></span>", productCategory.CategoryPath,esUtilities.CommonHelper.GetStoreLocation()));
        }

    }
}