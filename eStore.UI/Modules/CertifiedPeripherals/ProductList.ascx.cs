using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules.CertifiedPeripherals
{
    public partial class ProductList :  Presentation.eStoreBaseControls.eStoreBaseUserControl 
    {
        private bool _showBorder = false;
        public bool ShowBorder
        {
            get { return _showBorder; }
            set { _showBorder = value; }
        }

        private bool _showcomparecheckbox = false;
        public bool ShowCompareCheckbox
        {
            get { return _showcomparecheckbox; }
            set { _showcomparecheckbox = value; }
        }

        private bool _showPrice = true;
        public bool ShowPrice
        {
            get { return _showPrice; }
            set { _showPrice = value; }
        }

         private bool _showActions = false;
        public bool ShowActions
        {
            get { return _showActions; }
            set { _showActions = value; }
        }
        public List<POCOS.PStoreProduct> Products;
        protected void Page_Load(object sender, EventArgs e)
        {
         
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (Products != null)
            {
                this.rpProducts.DataSource = Products;
                this.rpProducts.DataBind();
            }
            base.OnPreRender(e);

        }

        protected void lAdd2Cart_Click(object sender, EventArgs e)
        {

   
                try
                {
                    LinkButton s = (LinkButton)sender;
                    string paras = s.CommandArgument;

                    POCOS.Part part= Presentation.eStoreContext.Current.Store.getPart(paras);
                    if (part != null)
                    { 
                        POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
                        CartItem cartitem = cart.addItem(part, 1);
                        cart.save();
                        this.Response.Redirect("~/Cart/Cart.aspx");
                    }
                }
                catch (Exception)
                {


                }

 
        }
    }
}