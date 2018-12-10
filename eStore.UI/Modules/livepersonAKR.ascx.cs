using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class livepersonAKR : livepersonBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserLargerImage == true)
            {
                imgLarger.Visible = true;
                imgSmaller.Visible = false;

            }
            else
            {
                imgLarger.Visible = false;
                imgSmaller.Visible = true;
            }
        }
    }
}