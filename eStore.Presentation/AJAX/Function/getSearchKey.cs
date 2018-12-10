using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eStore.Presentation.AJAX.Function
{


    class getSearchKey : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {

            int maxRows = 10;
            string keyword = context.Request["keyword"] ?? "";
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 2)
                return "";
            int.TryParse(context.Request["maxRows"].ToString(), out maxRows);

            if (maxRows > 20)
                maxRows = 20;   //limited amount of hint search, this is to prevent hacking crash
            //POCOS.ProductSpecRules psr = eStoreContext.Current.Store.getMatchProducts(keyword,false, maxRows);
            List<POCOS.spProductSearch_Result> result = eStoreContext.Current.Store.getMatchProducts(keyword, maxRows);
            var rlt = (from p in result
                        
                       select new JObject {
                        new JProperty("value", p.Name),
                         new JProperty("label", p.Name),
                          new JProperty("Description", p.Name)
                       }).Take(maxRows);
            return JsonConvert.SerializeObject(rlt);
        }
    }
}
