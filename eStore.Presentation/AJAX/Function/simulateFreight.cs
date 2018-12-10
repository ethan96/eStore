using eStore.POCOS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eStore.Presentation.AJAX.Function
{
    public class simulateFreight : IAJAX
    {
        public string ProcessRequest(HttpContext context)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(context.Request["type"]))
            {
                string tp = context.Request["type"].ToString();

                if (tp == "getpart")
                    return this.GetParts(context);
            }

            return JsonConvert.SerializeObject(string.Empty);
        }

        public string GetParts(HttpContext context)
        {
            string keyword = context.Request["q"] ?? "";

            Dictionary<string, string> parts = eStoreContext.Current.Store.getOrderbyPNHint(keyword);
            if (!parts.ContainsKey("SBC-BTO"))
                parts.Add("SBC-BTO", "SBC-BTO");

            var rlt = (from p in parts
                       orderby p.Value
                       select new Newtonsoft.Json.Linq.JObject {
                        new Newtonsoft.Json.Linq.JProperty("id",  p.Key ),
                         new Newtonsoft.Json.Linq.JProperty("name",  p.Key )
                       }).Take(10);

            return JsonConvert.SerializeObject(rlt);
        }
    }
}
