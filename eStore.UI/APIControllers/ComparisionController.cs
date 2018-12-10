using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;

namespace eStore.UI.APIControllers
{
    public class ComparisionController : ApiController
    {
        public string getComparisionContent()
        {
            string[] pns = Request.GetQueryNameValuePairs().Where(x => x.Key.ToUpper() == "PNS").Select(x => x.Value).ToArray();
            List<POCOS.Part> parts = Presentation.eStoreContext.Current.Store.getPartList(string.Join(",", pns));
            Presentation.Product.ProductCompareManagement mgt = new Presentation.Product.ProductCompareManagement();

            return mgt.generateCompareUI(parts.Where(x => x is POCOS.Product ).Select(x => (POCOS.Product) x).ToList());
        }
        public string getMobileComparisionContent()
        {
            string[] pns = Request.GetQueryNameValuePairs().Where(x => x.Key.ToUpper() == "PNS").Select(x => x.Value).ToArray();
        
            List<POCOS.Part> parts = Presentation.eStoreContext.Current.Store.getPartList(string.Join(",", pns));
            Presentation.Product.ProductCompareManagement mgt = new Presentation.Product.ProductCompareManagement();

            return mgt. generateCompareMobileUI(parts.Where(x => x is POCOS.Product).Select(x => (POCOS.Product)x).ToList());
        }
  
    }
}