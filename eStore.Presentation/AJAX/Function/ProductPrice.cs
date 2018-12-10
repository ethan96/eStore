using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using eStore.Presentation.Product;
namespace eStore.Presentation.AJAX.Function
{
    /// <summary>
    /// this function for get product price by ajax
    /// </summary>
    class ProductPrice : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            Presentation.Product.PriceStyle style = Product.PriceStyle.productprice;
            if (!string.IsNullOrEmpty(context.Request["isminprice"]) && context.Request["isminprice"] == "true")
            { style = Product.PriceStyle.MinPrice; }
            Enum.TryParse<PriceStyle>(context.Request.QueryString["pricestyle"], out style);
            string formatedprice;
        

            POCOS.Product product =Presentation.eStoreContext.Current.Store.getProduct(context.Request["id"]);

            if (product == null || product.getListingPrice().value == 0)
                formatedprice = Presentation.Product.ProductPrice.getSimplePrice(product, style);
            else
            {
                switch (style)
                { 
                    case  PriceStyle.productpriceLarge:
                        formatedprice = Presentation.Product.ProductPrice.getPrice(product, PriceStyle.productpriceLarge);
                        break;
                    case PriceStyle.MinPrice:
                    case PriceStyle.productprice:
                    default:
                        formatedprice = Presentation.Product.ProductPrice.getSimplePrice(product, style);
                        break;
                }
            }

            JObject o = new JObject(
                         new JProperty("price", formatedprice)
                         );
            return JsonConvert.SerializeObject(o);
        }
    }
}
