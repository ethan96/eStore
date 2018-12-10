using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using System.IO;
using System.Web.UI;
using System.IO.Compression;
using System.Web.Optimization;
using System.Web.Http;
using System.Web.Routing;

namespace eStore.UI
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.MapHttpRoute(
                name: "PagedList",
                routeTemplate: "api/{controller}/{action}/{id}/{page}/{pagesize}"
                );
            RouteTable.Routes.MapHttpRoute(
                         name: "DefaultApi",
                         routeTemplate: "api/{controller}/{action}/{id}",
                         defaults: new { id = RouteParameter.Optional }
                     );
            RouteTable.Routes.Add(new Route(
       "sitemap.xml", new PageRouteHandler("~/sitemap.aspx")));

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
            }
        }
        private static string UrlPrefix { get { return "api"; } }
        private static string UrlPrefixRelative { get { return "~/api"; } }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(UrlPrefixRelative);
        }
        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

            //AbortCheckout
            /**** commented out.  This will be handled in a routine scanning job
            if (Session["Order"] != null)
            {
                POCOS.Order order = Session["Order"] as POCOS.Order;
                if (!order.isConfirmdOrder)
                {
                    BusinessModules.Task.EventPublisher eventPublisher = new BusinessModules.Task.EventPublisher(eStore.Presentation.eStoreContext.Current.Order
                        , BusinessModules.Task.EventType.AbortCheckout);

                    BusinessModules.Task.EventManager.getInstance().PublishNewEvent(eventPublisher);
                }
            }
             * */

        }
        protected void Application_AuthenticateRequest(Object sender,
EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id =
                            (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;



                        try
                        {
                            string[] roles = ticket.UserData.Split(';')[2].Split(',');
                            HttpContext.Current.User = new GenericPrincipal(id, roles);
                        }
                        catch
                        {
                            FormsAuthentication.SignOut();
                        }

                    }
                }
            }
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            string acceptEncoding = app.Request.Headers["Accept-Encoding"];
            Stream prevUncompressedStream = app.Response.Filter;

            if (!(app.Context.CurrentHandler is Page ||
                app.Context.CurrentHandler.GetType().Name == "SyncSessionlessHandler") ||
                app.Request["HTTP_X_MICROSOFTAJAX"] != null)
                return;

            if (acceptEncoding == null || acceptEncoding.Length == 0)
                return;

            acceptEncoding = acceptEncoding.ToLower();

            if (acceptEncoding.Contains("deflate") || acceptEncoding == "*")
            {
                // defalte
                app.Response.Filter = new DeflateStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "deflate");
            }
            else if (acceptEncoding.Contains("gzip"))
            {
                // gzip
                app.Response.Filter = new GZipStream(prevUncompressedStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "gzip");
            }
        }
    }
}
