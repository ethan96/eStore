using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace eStore.Presentation.UrlRewriting
{
    public class RewriteModule : RewriteBaseModule
    {

        public override void Rewrite(string requestedPath, HttpApplication app)
        {
            // Implement functionality here that mimics the 'URL Mapping' features of ASP.NET 2.0
            RewriteConfigHandler config = (RewriteConfigHandler)ConfigurationManager.GetSection("UrlRewriteRule");
            string pathOld = null;
            string pathNew = "";

            if (config.Enabled())
            {
                pathOld = app.Request.RawUrl;

                //Get the request page without the querystring parameters
                string requestedPage = app.Request.RawUrl.ToLower();
                if (requestedPage.IndexOf("?") > -1)
                {
                    requestedPage = requestedPage.Substring(0, requestedPage.IndexOf("?"));
                }

                //Format the requested page (url) to have a ~ instead of the virtual path of the app
                string appVirtualPath = app.Request.ApplicationPath;
                if (requestedPage.Length >= appVirtualPath.Length)
                {
                    if (requestedPage.Substring(0, appVirtualPath.Length).ToLower() == appVirtualPath.ToLower())
                    {
                        requestedPage = requestedPage.Substring(appVirtualPath.Length);
                        if (requestedPage.Length == 0)
                        { requestedPage = "~/"; }
                        else if (requestedPage.Substring(0, 1) == "/")
                        {
                            requestedPage = "~" + requestedPage;
                        }
                        else
                        {
                            requestedPage = "~/" + requestedPage;
                        }
                    }
                }

                //Get the new path to rewrite the url to if it meets one of the defined virtual urls.
                pathNew = config.MappedUrl(requestedPage);
                pathNew = pathNew.Replace("+", "%2B");

                //If the requested url matches one of the virtual one the lets go and rewrite it.
                if (pathNew.Length > 0)
                {
                    if (pathNew.IndexOf("?") > -1)
                    {
                        //The matched page has a querystring defined                        

                        if (pathOld.IndexOf("?") > -1)
                        {
                            pathNew += "&" +pathOld.Substring(pathOld.IndexOf("?")+1);
                        }
                    }
                    else
                    {
                        //The matched page doesn't have a querystring defined
                        if (pathOld.IndexOf("?") > -1)
                        {
                            pathNew += pathOld.Substring(pathOld.IndexOf("?"));
                        }
                    }
                    //Rewrite to the new url
                    HttpContext.Current.RewritePath(VirtualPathUtility.ToAbsolute(pathNew));
                }

            }
            //config.Enabled
        }
    }
}