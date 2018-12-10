using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    public class setCartContent : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
        
            POCOS.Cart cart = eStoreContext.Current.UserShoppingCart;
            var cartitems = from cs in cart.CartItems
                            select cs.Description ;

            return JsonConvert.SerializeObject(cartitems);
        
        }
    }
}
