using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace eStore.Presentation.AJAX.Function
{
    public class getCBOMPrice : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            POCOS.Product_Ctos ctos = (POCOS.Product_Ctos)Presentation.eStoreContext.Current.Store.getProduct(context.Request["productID"]);

            string requestpara;
            requestpara = context.Request["para"];
            string[] para = requestpara.Split(';');

            System.Collections.Specialized.NameValueCollection paraCollection = new System.Collections.Specialized.NameValueCollection();
            foreach (string parakeyvalue in para)
            {
                string[] keyvalue = parakeyvalue.Split(':');
                paraCollection.Add(keyvalue[0], keyvalue[1]);
            }
            //centralize logical
            Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            BTOSystem newbtos =  ctosMgr.updateBTOS(ctos, paraCollection);
            Price _price = ctos.updateBTOSPrice(newbtos);

            POCOS.Product.PRICINGMODE pricemode;
            POCOS.Price listprice = new Price(), markupprice = new Price();
            pricemode = ctos.getListingPrice(listprice, markupprice, newbtos);
            string exchangedprice;
            if (Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID == Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencyID)
                exchangedprice = "0";
            else
            {
                exchangedprice = Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(Presentation.eStoreContext.Current.Store.getCurrencyExchangeValue(listprice.value, Presentation.eStoreContext.Current.CurrentCurrency)
                    , Presentation.eStoreContext.Current.CurrentCurrency);
            }
            Dictionary<int, decimal> warrantyitemsprice = new Dictionary<int, decimal>();
            
            CTOSBOM _warryPart = null;
            decimal defautprice = 0m;
            foreach (CTOSBOM component in ctos.components)
            {
                if (component.isWarrantyType())
                {
                    warrantyitemsprice = ctos.getWarrantyItemsPrice(component, newbtos);
                    BTOSConfig selectItem = ctos.getSelectedComponentOptions(newbtos, component).FirstOrDefault();
                    defautprice = selectItem == null || selectItem.matchedOption == null ? 0m : warrantyitemsprice[selectItem.matchedOption.ID];
                    if (selectItem != null)
                        _warryPart = selectItem.matchedOption;
                    break;
                }
            }

            var addonStr = context.Request["addons"];
            decimal addonPrices = 0;
            if (!string.IsNullOrEmpty(addonStr))
            {
                string[] addons = addonStr.Split(';');
                System.Collections.Specialized.NameValueCollection addonCollection = new System.Collections.Specialized.NameValueCollection();
                foreach (string parakeyvalue in addons)
                {
                    string[] keyvalue = parakeyvalue.Split(':');
                    addonCollection.Add(keyvalue[0], keyvalue[1]);
                }
                foreach (string addonoption in addonCollection.Keys)
                {
                    try
                    {
                        int qty = 0;
                        string qtyStr = addonCollection[addonoption];
                        if (string.IsNullOrEmpty(qtyStr) || qtyStr == "0")
                            continue;
                        if (!int.TryParse(qtyStr, out qty) || qty == 0)
                            continue;

                        string sproducid = addonoption.Substring(addonoption.LastIndexOf('_') + 1);
                        POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(sproducid);

                        var _wPart = _warryPart.CTOSComponent.firstPart;
                        if (_warryPart != null && _wPart != null)
                            addonPrices += product.getPriceWhitWarantyPrice(_warryPart.CTOSComponent.firstPart) * qty;
                        else
                            addonPrices += product.getNetPrice(false).value;

                    }
                    catch (Exception)
                    {

                    }
                }

                
            }
            JObject o = new JObject(
                            new JProperty("PRICINGMODE", pricemode.ToString()),
                            //new JProperty("listprice", Presentation.Product.ProductPrice.FormartPriceWithDecimal(listprice.value)),
                            new JProperty("listprice", (listprice.value == 0) ? "Call for price" : Presentation.Product.ProductPrice.FormartPriceWithDecimal(listprice.value + addonPrices)),
                            new JProperty("hlistprice", Presentation.Product.ProductPrice.getSitePrice(listprice.value).value),
                            new JProperty("markupprice", Presentation.Product.ProductPrice.FormartPriceWithDecimal(markupprice.value)),
                            new JProperty("warrantyprice",
                                (from wp in warrantyitemsprice
                                 select new JObject(
                                     new JProperty("id", wp.Key),
                                     new JProperty("price", Presentation.Product.ProductPrice.getSitePrice(wp.Value - defautprice).value),
                                     new JProperty("priceDisplay", Presentation.Product.ProductPrice.getSitePrice(wp.Value - defautprice).valueWithoutCurrency)
                                     )
                                )
                                ),
                            new JProperty("exchangedprice", exchangedprice)
                            );
            return JsonConvert.SerializeObject(o);

        }
    }
}
