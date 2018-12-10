using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    public class ScriptTranslation : IAJAX
    {

        public string ProcessRequest(System.Web.HttpContext context)
        {
            List<POCOS.Translation> dict = Presentation.eStoreContext.Current.Store.getSeriesTranslation("ScriptMessage_"); 
            var rlt = (from t in dict
                       select new JObject {
                        new JProperty("key",t.Key.Remove(0,14)),
                         new JProperty("value",t.Value) 
                       });
            return JsonConvert.SerializeObject(rlt);
            throw new NotImplementedException();
        }
    }
}
