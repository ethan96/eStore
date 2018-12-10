using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eStore.Presentation.AJAX.Function
{
    public class CreateUnicaActivity : IAJAX
    {
        public string ProcessRequest(HttpContext context)
        {
            try
            {
                if (eStoreContext.Current.User != null &&
                    !string.IsNullOrEmpty(context.Request["activitytype"]) &&
                    !string.IsNullOrEmpty(context.Request["productID"]) &&
                    !string.IsNullOrEmpty(context.Request["url"]))
                {
                    POCOS.Product p = eStoreContext.Current.Store.getProduct(context.Request["productID"].ToString());
                    if (p != null)
                    {
                        BusinessModules.Task.EventType tp = BusinessModules.Task.EventType.NewUserRequest;
                        switch (context.Request["activitytype"].ToString().ToLower())
                        {
                            case "datasheet":
                                tp = BusinessModules.Task.EventType.DownloadDatasheet;
                                break;
                            case "files":
                                tp = BusinessModules.Task.EventType.DownloadFiles;
                                break;
                            case "driver":
                                tp = BusinessModules.Task.EventType.DownloadDriver;
                                break;
                            case "manual":
                                tp = BusinessModules.Task.EventType.DownloadManual;
                                break;
                            case "utilities":
                                tp = BusinessModules.Task.EventType.DownloadUtilities;
                                break;
                            default:
                                break;
                        }
                        if (tp != BusinessModules.Task.EventType.NewUserRequest)
                            eStoreContext.Current.Store.PublishStoreEvent(context.Request["url"].ToString(), eStoreContext.Current.User, p, tp);
                    }
                }
                else if (eStoreContext.Current.User != null && !string.IsNullOrEmpty(context.Request["activitytype"]) && !string.IsNullOrEmpty(context.Request["url"]))
                {
                    BusinessModules.Task.EventType tp = BusinessModules.Task.EventType.NewUserRequest;
                    switch (context.Request["activitytype"].ToString().ToLower())
                    {
                        case "callmenow":
                            tp = BusinessModules.Task.EventType.CallInbound;
                            break;
                        case "livechat":
                            tp = BusinessModules.Task.EventType.EmailInbound;
                            break;
                        default:
                            break;
                    }
                    if (tp != BusinessModules.Task.EventType.NewUserRequest)
                        eStoreContext.Current.Store.PublishStoreEvent(context.Request["url"].ToString(), eStoreContext.Current.User, null, tp);
                }
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
