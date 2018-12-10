using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace eStore.Presentation.AJAX.Function
{
    public class getCMSByPartNumber : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            List<JObject> ls = new List<JObject>();
            if (!string.IsNullOrEmpty(context.Request["keyword"]))
            {
                string keyword = System.Web.HttpUtility.UrlDecode(context.Request["keyword"].ToString());
                System.Data.DataSet ds = eStoreContext.Current.Store.getCmsByPartNo(keyword);
                if (ds != null && ds.Tables.Count > 0)
                {
                    var drs = ds.Tables[0].Select("HYPER_LINK <> ''");
                    foreach (DataRow dr in drs)
                        ls.Add(new JObject { new JProperty("key", dr["TITLE"].ToString()), new JProperty("value", dr["HYPER_LINK"].ToString()) });
                } 
            }
            string cc = JsonConvert.SerializeObject(ls);
            return cc;
        }
    }
}
