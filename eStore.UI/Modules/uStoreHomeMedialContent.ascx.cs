using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class uStoreHomeMedialContent : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
      
            //int itemsCount = 5;
            List<POCOS.ProductCategory> spc = eStoreContext.Current.Store.getTopLeveluStoreCategories(eStoreContext.Current.MiniSite);
            this.rpCategories.DataSource = spc.OrderBy(c => c.Sequence).ThenBy(p => p.LocalCategoryName);
            this.rpCategories.DataBind();
            
        }
    }
}