using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Modules
{
    public partial class CategoryWithSubCategory : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.ProductCategory productCategory { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( productCategory != null && productCategory.childCategoriesX != null)
            {
                this.subCategory.DataSource = productCategory.childCategoriesX;
                this.subCategory.DataBind();
            }
        }
    }
}