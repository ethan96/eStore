using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    public class LowestPrice : IAJAX
    {

        public string ProcessRequest(System.Web.HttpContext context)
        {
            decimal miniprice = 0;
            if (context.Request.QueryString["id"] != null)
            {
                string id = context.Request.QueryString["id"].ToString();
                POCOS.ProductCategory productCategory = eStoreContext.Current.Store.getProductCategory(id);

                if (productCategory != null)
                { miniprice = productCategory.getLowestPrice(); }
            }
            JObject job;
            if (miniprice == 0)
            {
                job = new JObject
                {
                    new JProperty("LowestPrice", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Call_for_Price))
                };
            }
            else
            {
                job = new JObject
                {
                    new JProperty("LowestPrice",eStore.Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(miniprice))
                };
            }
            return JsonConvert.SerializeObject(job);
        }
    }
}
