using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CategoryWithSubCategoryAndProducts : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        private int _MaxProducts = 2;
        public int MaxProducts
        {
            get { return _MaxProducts; }
            set
            {
                _MaxProducts = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( productCategory != null && productCategory.childCategoriesX != null)
            {
                hCategory.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);

                hCategory.NavigateUrl = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory);
                this.rpsubCategoryList.DataSource = productCategory.childCategoriesX.OrderBy(c => c.Sequence).ThenBy(c => c.LocalCategoryName);
                this.rpsubCategoryList.DataBind();
            }
        }

        protected void rpsubCategoryList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory category = (POCOS.ProductCategory)e.Item.DataItem;
                
                DataList dlSubCategoryProducts = (DataList)e.Item.FindControl("dlSubCategoryProducts");
                Panel pSeefullselection = (Panel)e.Item.FindControl("pSeefullselection");
                List<POCOS.Product> products = new List<POCOS.Product>();
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProducts(category.CategoryPath, out products);
                if (products != null && products.Count > 0)
                {
                    if (products.Count > 0)
                    {

                        pSeefullselection.Visible = true;
                    }
                    else
                    { pSeefullselection.Visible = false; }

                    dlSubCategoryProducts.DataSource = products.Take(MaxProducts);
                    dlSubCategoryProducts.RepeatColumns = MaxProducts;
                    dlSubCategoryProducts.DataBind();
              
                }
            }
        }
    }
}