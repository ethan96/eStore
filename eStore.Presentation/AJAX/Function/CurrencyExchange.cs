using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    public class CurrencyExchange : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            decimal listPrice = 0;
            string targetCurrency = context.Request["currency"];
            string exchangedprice;
            if (decimal.TryParse(context.Request["listprice"], out listPrice))
            {
                if (string.IsNullOrEmpty(targetCurrency))

                    exchangedprice = "0";
                else
                    if (Presentation.eStoreContext.Current.Store.SupportCurrencies.ContainsKey(targetCurrency))
                    {
                        decimal rlt=Presentation.eStoreContext.Current.Store.getCurrencyExchangeValue(listPrice, Presentation.eStoreContext.Current.Store.SupportCurrencies[targetCurrency]);
                        if (rlt > 0)
                        {
                            exchangedprice = Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(rlt
                          , Presentation.eStoreContext.Current.Store.SupportCurrencies[targetCurrency]);
                            Presentation.eStoreContext.Current.CurrentCurrency = Presentation.eStoreContext.Current.Store.SupportCurrencies[targetCurrency];
                        }
                        else
                            exchangedprice = "0";
                    }

                    else
                        exchangedprice = "0";
                {
                    decimal rlt = Presentation.eStoreContext.Current.Store.getCurrencyExchangeValue(listPrice, Presentation.eStoreContext.Current.CurrentCurrency);
                    if(rlt>0)
                    exchangedprice = Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(rlt
                        , Presentation.eStoreContext.Current.CurrentCurrency);
                    else
                        exchangedprice = "0";
                }
            }
            else
                exchangedprice = "0";
            JObject o = new JObject(new JProperty("exchangedprice", exchangedprice));
            return JsonConvert.SerializeObject(o);
        }
    }
}
