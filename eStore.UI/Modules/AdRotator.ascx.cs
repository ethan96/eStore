using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class AdRotator : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
                BindData();
                this.BindScript("url", "easing", "jquery.easing.1.3.js");
                this.BindScript("url", "jquery.coda-slider-2.0", "jquery.coda-slider-2.0.js");
      
        }
        private void BindData() 
        {
            this.rpAdRotator.DataSource = Presentation.eStoreContext.Current.Store.getHomeBanners(Presentation.eStoreContext.Current.MiniSite);
            this.rpAdRotator.DataBind();
        }
    }
}