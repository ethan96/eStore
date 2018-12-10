using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
 public    class SuggestingProduct:IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            string type =( context.Request["type"]??string.Empty).ToLower();
            POCOS.Cart cart;
            if (type == "cart_cart_aspx")
                cart = Presentation.eStoreContext.Current.UserShoppingCart;
            else if (type == "quotation_quote_aspx" &&Presentation.eStoreContext.Current.Quotation!=null)
            { cart = Presentation.eStoreContext.Current.Quotation.cartX; }
            else
                return string.Empty;
            if (cart == null || cart.CartItems.Count == 0)
                return string.Empty;
            List<POCOS.Product> suggestingProducts = Presentation.eStoreContext.Current.Store.getCustomersAlsoBought(cart);

            var rlt = (from p in suggestingProducts.Distinct().Take(4)
                              select new JObject {
                                new JProperty("ProductID",p.name),
                                new JProperty("Desc",((POCOS.Product)p).productDescX??p.name),
                                new JProperty("Hyperlink",UrlRewriting.MappingUrl.getMappingUrl(p).Replace("~","")),
                                new JProperty("image",p.thumbnailImageX??string.Empty),
                                new JProperty("price",Presentation.Product.ProductPrice.FormartPrice(p.getListingPrice().value))
                    }
                    );
            return JsonConvert.SerializeObject(rlt);
        
        }
    }
}
