using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    public partial class FooterContent : System.Web.UI.UserControl
    {
        private POCOS.Menu _foodMemu;
        public POCOS.Menu FoodMemu
        {
            get 
            {
                return _foodMemu; 
            }
            set { _foodMemu = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            bindMenuList();

        }

        protected void bindMenuList()
        {
            if (FoodMemu != null)
            {
                ltCategoryName.Text = _foodMemu.MenuName.Trim();
                POCOS.Menu leftMenu = _foodMemu.subMenusX.FirstOrDefault(c => c.MenuName.ToLower() == "left");
                if (leftMenu != null)
                {
                    rtFooterLeft.DataSource = leftMenu.subMenusX.ToList();
                    rtFooterLeft.DataBind();
                }
                POCOS.Menu rightMenu = _foodMemu.subMenusX.FirstOrDefault(c => c.MenuName.ToLower() == "right");
                if (rightMenu != null)
                {
                    rtFooterRight.DataSource = rightMenu.subMenusX.ToList();
                    rtFooterRight.DataBind();
                }
            }
        }
    }
}