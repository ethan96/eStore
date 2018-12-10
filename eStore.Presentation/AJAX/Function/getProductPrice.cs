using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using eStore.Presentation.Product;

namespace eStore.Presentation.AJAX.Function
{
    /// <summary>
    /// this function is for get product information by ajax in  order by partno
    /// </summary>
    public class getProductPrice : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            string formatedprice = "call...";
            if (string.IsNullOrWhiteSpace(context.Request.QueryString["id"])==false)
            {
                string id = context.Request.QueryString["id"].ToString();
                //skip if matched OrderByPartnoExcludeRules
                if (Presentation.eStoreContext.Current.Store.isValidOrderbyPN(id) == false)
                    return "";
                //order by no是否检查产品是os,如果是价格为call price
                bool isCheckOS = false;
                if (!string.IsNullOrEmpty(context.Request.QueryString["isCheckOS"]))
                    bool.TryParse(context.Request.QueryString["isCheckOS"].ToString(), out isCheckOS);
                POCOS.Part part = eStoreContext.Current.Store.getPart(id, true);

                if (part == null)
                    part = Presentation.eStoreContext.Current.Store.getVendorPartForOrderbyPartNo(id);
                if (part != null && part.isSAPParts && !part.isTOrPParts())
                {
                    decimal _price = 0;
                    //如果要 检查是否是os
                    if (isCheckOS)
                        isCheckOS = part.isOS();
                    if (part.isOrderable(true) && !isCheckOS)
                        _price = part.getListingPrice().value;

                    eStore.Presentation.Product.PriceStyle style = Product.PriceStyle.productprice;
                    Enum.TryParse<PriceStyle>(context.Request.QueryString["pricestyle"], out style);

                    if (style == PriceStyle.productprice)
                    {
                        formatedprice = Presentation.Product.ProductPrice.FormartPrice(_price);
                    }
                    else if (style == PriceStyle.productpriceLarge)
                    {
                        if (part is POCOS.Product)
                            formatedprice = Presentation.Product.ProductPrice.getPrice((POCOS.Product)part, PriceStyle.productpriceLarge);
                        else
                            formatedprice = Presentation.Product.ProductPrice.FormartPrice(_price);
                    }
                    else
                    {
                        formatedprice = Presentation.Product.ProductPrice.FormartPrice(_price);
                    }
                
                    //string discription = part.phasedOut == true ? part.productDescX.Trim() + "<p class=\"regularprice\"><span>Phased Out</span></p>" : part.productDescX.Trim();
                    var job = new JObject
                    {
                        new JProperty("discription",part.productDescX.Trim()),
                        new JProperty("price",formatedprice),
                        new JProperty("phasedOut",part.notAvailable)
                    };
                    return JsonConvert.SerializeObject(job);
                }
            }
            return "";
        }
    }
}
