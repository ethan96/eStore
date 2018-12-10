using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ProductSubCategory : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategory != null)
            {
                this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryDescription(productCategory);

                this.dlSubCategory.DataSource = productCategory.childCategoriesX;
                this.dlSubCategory.DataBind();
            }
        }
    }
}