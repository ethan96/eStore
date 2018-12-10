using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.KitTheme
{
    public partial class Products : BaseTheme
    {
        public POCOS.ProductCategory CurrentCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            BindCategories();
        }

        protected void BindCategories()
        {
            if (CurrentCategory != null)
            {
                if (CurrentCategory.childCategoriesX != null && CurrentCategory.childCategoriesX.Any())
                {
                    rpCategories.DataSource = CurrentCategory.childCategoriesX.Select(c => new
                    {
                        Name = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(c),
                        Description = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(c), //esUtilities.StringUtility.subString(eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(c), 200),
                        Href = ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(c)),
                        Image = esUtilities.CommonHelper.ResolveUrl(string.IsNullOrEmpty(c.ImageURL) ? "~/App_Themes/Default/photounavailable.gif" : "~/resource/ProductCategory/" + c.ImageURL),
                        Price = string.Format("From<span>{0}</span><span class=\"price\" >{1}</span>", eStore.Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                                    , eStore.Presentation.Product.ProductPrice.FormartPriceWithoutCurrency(c.getLowestPrice())),

                    }).ToList();
                    rpCategories.DataBind();
                }
            }
        }
    }
}