using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment.files
{
    public partial class AEU_CompleteOffer : BaseComplete
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string ClientValidataionFunction()
        {
            return "return checkPaymentInfo();";
        }
    }
}