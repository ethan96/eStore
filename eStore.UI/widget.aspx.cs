using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using System.Configuration;
using eStore.Presentation.Widget;

namespace eStore.UI
{
    public partial class widget : Presentation.eStoreBaseControls.eStoreBasePage
    {
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
                return this.Widget1.isMobileFriendly; 
            }
        }

        protected bool isFullSize
        {
            get
            {
                return this.Widget1.isFullSize;
            }
        }

        protected override void OnPreRenderComplete(EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.Widget1.CANONICAL))
                setCanonicalPage(this.Widget1.CANONICAL);
            base.OnPreRenderComplete(e);
        }
    }

}