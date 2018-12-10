using System;
using System.Web;
using eStore.Presentation.AJAX;
using System.Web.SessionState;

namespace eStore.UI.proc
{
    public class json : IHttpHandler, IReadOnlySessionState
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //write your handler implementation here.

            int funcvalue;
            Int32.TryParse(context.Request["func"],out funcvalue);

            Presentation.AJAX.AJAXFunctionType funcType = (Presentation.AJAX.AJAXFunctionType)funcvalue;
         
            IAJAX ajax = Presentation.AJAX.AJAXManagement.getAJAXFunction(funcType);
            string rlt = ajax.ProcessRequest(context);
           
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
            context.Response.Write(rlt);
        }

        #endregion
    }
}
