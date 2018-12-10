using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.IO;

namespace eStore.BusinessModules
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BaseService : System.Web.Services.WebService
    {
        private HttpContext _context = null;

        public BaseService()
        {
            _context = this.Context;
        }

        /// <summary>
        /// entity object to json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual string setObj2Json(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        protected virtual void responseJson(string json) 
        {
            _context.Response.ContentType = "application/json";
            this.Context.Response.Write(json);
            this.Context.Response.End();
        }

        protected virtual void responseStr(string str)
        {
            this.Context.Response.Write(str);
            this.Context.Response.End();
        }

        /// <summary>
        /// json to entity object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        protected virtual T setJson2Obj<T>(string json)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
    }
}
