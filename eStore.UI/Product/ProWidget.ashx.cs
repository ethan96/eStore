using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eStore.UI.Modules;
using System.Web.SessionState;
using System.Configuration;
using System.Web.UI.HtmlControls;

namespace eStore.UI
{
    /// <summary>
    /// Summary description for ProWidget
    /// </summary>
    public class ProWidget : IHttpHandler , IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            POCOS.WidgetPage widgetpage = null;
            if (context.Request["widgetid"] != null)
            {
                int widgetid;
                if (int.TryParse(context.Request["widgetid"], out widgetid))
                    widgetpage = Presentation.eStoreContext.Current.Store.getWidgetPage(widgetid);
            }
            if (widgetpage != null)
            {
                List<string> partids = widgetpage.Widgets.Where(x => !string.IsNullOrEmpty(x.SProductIDs)).Select(x => x.SProductIDs).Distinct().ToList();
                if (partids.Any())
                    (new POCOS.DAL.PartHelper()).prefetchPartList(widgetpage.StoreID, partids);
                var widgetPageName = widgetpage.PageName;
                var fileName = string.Format("{0}\\Unzip\\{1}\\{2}\\{3}"
                    , ConfigurationManager.AppSettings.Get("Widget_Path")
                    , Presentation.eStoreContext.Current.Store.storeID
                    , widgetPageName
                    , widgetpage.Path);

                if (!string.IsNullOrEmpty(fileName))
                {
                    //no relative link replacement shall be done at the request through widgetPage parameter
                    String leadingURL = String.Format(@"/resource/Widget/Unzip/{0}/{1}", Presentation.eStoreContext.Current.Store.storeID, widgetPageName);
                    var htmlPage = eStore.Presentation.Widget.WidgetHandler.processHTMLFile(fileName, Presentation.eStoreContext.Current.Store, leadingURL, context.Request.RawUrl);
                    if (htmlPage != null)
                    {
                        string headerStr = "";
                        if (htmlPage.header != null && !string.IsNullOrEmpty(htmlPage.header.links))
                        {
                            HtmlGenericControl node = new HtmlGenericControl("link");
                            node.InnerHtml = htmlPage.header.links;
                            headerStr += node.InnerHtml;
                        }
                        if (htmlPage.header != null && !string.IsNullOrEmpty(htmlPage.header.scripts))
                        {
                            HtmlGenericControl node = new HtmlGenericControl("scripts");
                            node.InnerHtml = htmlPage.header.scripts;
                            headerStr += node.InnerHtml;
                        }
                        context.Response.Write(headerStr + htmlPage.body.value);
                        context.Response.End();
                    }
                }
            }
            context.Response.Write("");
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