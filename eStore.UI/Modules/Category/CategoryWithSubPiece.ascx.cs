using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Category
{
    public partial class CategoryWithSubPiece : VCategoryBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rpCateList.DataSource = productCategory.childCategoriesX;
            rpCateList.DataBind();
        }
    }
}