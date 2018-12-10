using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Category
{
    public partial class CategoryWithSubList : VCategoryBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (productCategory != null && productCategory.childCategoriesX != null && productCategory.childCategoriesX.Any())
            {
                rpCateList.DataSource = productCategory.childCategoriesX;
                rpCateList.DataBind();
            }
        }
    }
}