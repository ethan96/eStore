using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Cart
{
    public partial class emptycart :  Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var cart = Presentation.eStoreContext.Current.UserShoppingCart;
                var cartChangeMessage = eStore.Presentation.CartUtility.CheckAndMegerCartItem(cart);
                if (!string.IsNullOrEmpty(cartChangeMessage))
                {
                    lblCartItemMessage.Visible = true;
                    lblCartItemMessage.Text = cartChangeMessage;
                    cart.cartItemChangedMessage.Clear();
                }
            }
            else
                lblCartItemMessage.Visible = false;
        }
    }
}