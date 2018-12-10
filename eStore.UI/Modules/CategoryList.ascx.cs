using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CategoryList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
      public  ICollection<POCOS.ProductCategory> ChildProductCategories { set {
            if (value != null)
            {
                this.rpCategoryList.DataSource = value;
                this.rpCategoryList.DataBind();
            }
        } }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}