using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class livepersonAJP : livepersonBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserLargerImage == true)
            {
                imgBigPic.Visible = true;
                imgSmallPic.Visible = false;
               
            }
            else
            {
                imgBigPic.Visible = false;
                imgSmallPic.Visible = true;
            }
        }
    }
}