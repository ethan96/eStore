using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
   public class getOderByPNSearchKey:IAJAX
    {

        public string ProcessRequest(System.Web.HttpContext context)
        {
            //this function shall only be available for signed in user to prevent hacking navigation
            POCOS.User currentUser = Presentation.eStoreContext.Current.User;
            if (currentUser == null)  //not signed in yet
                return "";

            int maxRows = 10;
            string keyword = context.Request["keyword"] ?? "";
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 3)
                return "";

            int.TryParse(context.Request["maxRows"].ToString(), out maxRows);

            Dictionary<string, string> parts = eStoreContext.Current.Store.getOrderbyPNHint(keyword);
             var rlt = (from p in parts
                       orderby p.Key.IndexOf(keyword.ToUpper()) , p.Key
                       select new JObject {
                        new JProperty("value",  p.Key ),
                         new JProperty("label",  p.Key ), 
                          new JProperty("Description",p.Value)
                       })  .Take(maxRows);
            return JsonConvert.SerializeObject(rlt);
        }
    }
}
