using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Category
{
    public partial class CategoryWithSubPic : VCategoryBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rpCategoryList.DataSource = productCategory.childCategoriesX;
            rpCategoryList.DataBind();
        }
    }
}