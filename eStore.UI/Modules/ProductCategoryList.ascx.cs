using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ProductCategoryList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategory != null)
            {
                this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
                if (productCategory.parentCategoryX == null)
                    this.dlCategory.RepeatColumns = 3;
                else if (productCategory.childCategoriesX.Count > 2)
                    this.dlCategory.RepeatColumns = 3;
                else
                {
                    this.dlCategory.RepeatColumns = 2;
                    this.dlCategory.ItemStyle.CssClass = "SubCategoryTowColumn";
                }
                this.dlCategory.DataSource = productCategory.childCategoriesX;
                this.dlCategory.DataBind();
                eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(productCategory);

                this.YouAreHere1.productCategory = productCategory;
                if (productCategory.parentCategoryX == null)
                    YouAreHere1.Visible = eStoreLiquidSlider1.Advertisements.Any() == false;
            }
        }

        protected void dlCategory_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory category = (POCOS.ProductCategory)e.Item.DataItem;
                PlaceHolder phCategory = (PlaceHolder)e.Item.FindControl("phCategory");
                if (category.childCategoriesX == null || category.childCategoriesX.Count == 0 || category.parentCategoryX.parentCategoryX == null || category.DisplayTypeX==POCOS.ProductCategory.RenderStyle.Tabs)
                {
                    Modules.CategoryWithoutSubCategory categoryItem = (Modules.CategoryWithoutSubCategory)LoadControl("~/Modules/CategoryWithoutSubCategory.ascx");
                    categoryItem.productCategory = category;
                    phCategory.Controls.Add(categoryItem);
                }
                else
                {
                    Modules.CategoryWithSubCategory categoryItem = (Modules.CategoryWithSubCategory)LoadControl("~/Modules/CategoryWithSubCategory.ascx");
                    categoryItem.productCategory = category;
                    phCategory.Controls.Add(categoryItem);
                }
            }
        }
    }
}