using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class PONo : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool mustInput = false;

        public string PONoText
        {
            get
            {
                return txtPoNo.Text.Trim().Replace("Purchase Order number", "");
            }
            set
            {
                txtPoNo.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ltMustInput.Text = mustInput ? "*" : "";
            
        }
    }
}