using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eStore.Presentation.ProductFeeds;

namespace eStore.UI.Services
{
    /// <summary>
    /// Summary description for ProductFeeds
    /// </summary>
    public class ProductFeeds : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
         
           FeedSubscriptions subscription=new eStore.Presentation.ProductFeeds.Impl.Octopart();
           System.IO.MemoryStream stream = new System.IO.MemoryStream(subscription.Subscribe().ToArray());
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=eStoreProductFeeds_" + DateTime.Now.ToString("M_dd_yyyy_H_M_s") + ".csv");
            HttpContext.Current.Response.ContentType = "application/text";
            stream.Position = 0;
            stream.CopyTo(HttpContext.Current.Response.OutputStream);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}