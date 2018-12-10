using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eStore.UI.proc
{
    public class setting : IHttpHandler, IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {

            System.Text.StringBuilder serverSetting = new System.Text.StringBuilder();
            List<POCOS.Translation> dict = Presentation.eStoreContext.Current.Store.getSeriesTranslation("ScriptMessage_");
            var rlt = (from t in dict
                       select new JObject {
                        new JProperty("key",t.Key.Remove(0,14)),
                         new JProperty("value",t.Value) 
                       });

            serverSetting.AppendFormat("var eStoreTranslation={0};\n\n var HomeBannerLineCap={1};"
                , JsonConvert.SerializeObject(rlt)
                , eStore.Presentation.eStoreContext.Current.getIntegerSetting("HomeBannerLineCap", 1));
            bool UseSSL = string.IsNullOrEmpty(context.Request["s"]) ? false : context.Request["s"].Equals("true",StringComparison.OrdinalIgnoreCase);
            serverSetting.Append("function GetStoreLocation() {return \"");
            serverSetting.Append(esUtilities.CommonHelper.GetStoreRelativeLocation());
            serverSetting.Append("\"; }" );
            context.Response.ContentType = "text/javascript";
            context.Response.Charset = "utf-8";
            context.Response.Write(serverSetting.ToString());

        }
    }
}