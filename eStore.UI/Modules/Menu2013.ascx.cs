using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class Menu2013 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }
        void BindData()
        {
            List<POCOS.ProductCategory> datasource = eStoreContext.Current.Store.getTopLeveluStoreCategories(eStoreContext.Current.MiniSite).OrderBy(c => c.Sequence).ThenBy(p => p.LocalCategoryName).Take(2).ToList();

            this.rpCategoryGroup.DataSource = datasource;
            this.rpCategoryGroup.DataBind();
 
            this.rpCategoryItem.DataSource = datasource;
            this.rpCategoryItem.DataBind();
          
        }
    }
}