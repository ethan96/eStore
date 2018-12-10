using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI
{
    public partial class index : Presentation.eStoreBaseControls.eStoreBasePage
    {
        /// <summary>
        /// can not over write the master page because used the special holder eStoreConfigurableRightContent,
        /// if this place hodler has controls, it will not apply the setting 
        /// </summary>
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
      
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
  
            OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new OpenGraphProtocolAdapter("HomePage");
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);
        }
        protected override void OnPreRender(EventArgs e)
        {

            base.OnPreRender(e);
            AddStyleSheet(ResolveUrl("~/Styles/iServicesHomepage.css"));
        }
    }
}