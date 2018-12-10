using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CategoryHeader : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategory != null)
            {
                this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
                this.imgCategory.ImageUrl = eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() +  productCategory.ImageURL;
            }
        }
    }
}