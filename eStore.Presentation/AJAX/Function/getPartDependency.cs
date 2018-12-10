using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    class getPartDependency : IAJAX
    {

        public string ProcessRequest(System.Web.HttpContext context)
        {
            List<Part> dependenPartList = new List<Part>();
            string partNos = string.Empty;
            if (context.Request.QueryString["partNos"] != null)
                partNos = context.Request.QueryString["partNos"];
            if (!string.IsNullOrEmpty(partNos))
            {
                List<Part> parts = eStoreContext.Current.Store.getPartList(partNos);
                if (parts != null && parts.Count > 0)
                {
                    foreach (var p in parts)
                    {
                        if (p.isOrderable(true) && p.getListingPrice().value > 0)
                        {
                            foreach (var pc in p.dependentPartsX)
                            {
                                var _exitPart = dependenPartList.FirstOrDefault(c => c.SProductID == pc.SProductID);
                                if (_exitPart == null && pc.isOrderable(true) && pc.getListingPrice().value > 0)
                                    dependenPartList.Add(pc);
                            }
                        }
                        
                    }
                }
            }
            StringBuilder sbStr = new StringBuilder();
            if (dependenPartList.Count > 0)
            {
                sbStr.Append("<table class='estoretable fontbold' width='100%'>");
                sbStr.Append("<thead><tr><th class='tablecolwidth145'>Part No.</th><th>Description</th><th class='tablecolwidth75'>Unit Price</th><th class='tablecolwidth45'>Qty</th></tr></thead>");
                sbStr.Append("<tbody>");
                foreach (var c in dependenPartList)
                {
                    sbStr.Append("<tr>");
                    sbStr.Append("<td class='tablecolwidth145'><span id='" + c.SProductID + "' name='" + c.SProductID + "'>" + c.SProductID + "</span><input type='hidden' name='DependencyID' id='ID_" + c.SProductID + "' value='" + c.SProductID + "'></td><td class='left'>" + c.productDescX + "</td>");
                    sbStr.Append("<td class='tablecolwidth75 colorRed right'>" + eStore.Presentation.Product.ProductPrice.FormartPrice(c.getListingPrice().value) + "</td>");
                    sbStr.Append("<td class='tablecolwidth45'><input name='DependencyQty' type='text' class='qtytextbox' sproductid='" + c.SProductID + "'></td>");
                    sbStr.Append("</tr>");
                }
                sbStr.Append("</tbody></table>");

            }
            return sbStr.ToString();
        }



        //public string ProcessRequest(System.Web.HttpContext context)
        //{
        //    List<Part> dependenPartList = new List<Part>();
        //    string partNos = string.Empty;
        //    if (context.Request.QueryString["partNos"] != null)
        //        partNos = context.Request.QueryString["partNos"];
        //    if (!string.IsNullOrEmpty(partNos))
        //    {
        //        List<Part> parts = eStoreContext.Current.Store.getPartList(partNos);
        //        if (parts != null && parts.Count > 0)
        //        {
        //            foreach (var p in parts)
        //            {
        //                foreach (var pc in p.dependentPartsX)
        //                {
        //                    var _exitPart = dependenPartList.FirstOrDefault(c => c.SProductID == pc.SProductID);
        //                    if (_exitPart == null)
        //                        dependenPartList.Add(pc);
        //                }
        //            }
        //        }
        //    }
        //    var rlt = from dp in dependenPartList
        //                select new JObject {
        //                new JProperty("productid",  dp.SProductID )
        //               };
        //    return JsonConvert.SerializeObject(rlt);
        //}
    }
}
