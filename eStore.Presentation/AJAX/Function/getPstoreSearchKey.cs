using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using eStore.POCOS;

namespace eStore.Presentation.AJAX.Function
{
    class getPstoreSearchKey : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {

            int maxRows = 10;
            string keyword = context.Request["keyword"].ToString();
            int.TryParse(context.Request["maxRows"].ToString(), out maxRows);

            if (maxRows > 20)
                maxRows = 20;   //limited amount of hint search, this is to prevent hacking crash
            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(eStoreContext.Current.Store.profile);
          List<PStoreProduct> products=pstore.getMatchProducts(keyword, false, maxRows);
          var rlt = (from p in products
                     orderby (p.name.ToUpper().IndexOf(keyword.ToUpper()) < 0) ? 99 : p.name.ToUpper().IndexOf(keyword.ToUpper()), p.name
                       select new JObject {
                        new JProperty("value", p.SProductID),
                         new JProperty("label", p.name),
                          new JProperty("Description", p.productDescX)
                       }).Take(maxRows);
            return JsonConvert.SerializeObject(rlt);
        }
    }
}
