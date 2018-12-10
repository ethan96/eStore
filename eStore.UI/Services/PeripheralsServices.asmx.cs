using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;

namespace eStore.UI.Services
{
    /// <summary>
    /// Summary description for PeripheralsServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class PeripheralsServices : System.Web.Services.WebService
    {

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool PrepareAdd2Store(string storeid, string partno)
        {
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);
            POCOS.Part part = store.addVendorPartToPart(partno);
            return part != null && part.isOrderable() && !(part is POCOS.Product);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool SyncPrice(string storeid, string partno)
        {
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);
            POCOS.Part part = store.getPart(partno);
            if (part!=null && (part is POCOS.PStoreProduct))
            {
                POCOS.Sync.PISSync syncJob = new POCOS.Sync.PISSync();
                List<POCOS.Part> parts = new List<POCOS.Part>();
                parts.Add(part);
                string error = syncJob.syncPrice(store.profile, parts, true);
                if (string.IsNullOrEmpty(error))
                    return true;
                else

                    return false;
            }
            else
                return false;
        }
    }
}
