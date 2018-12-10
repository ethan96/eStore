using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.BusinessModules.Task
{
    public class SyncBBorderToMyAdvantech: TaskBase
    {
        private POCOS.Store _store = null;
        private POCOS.Order _order;

        public SyncBBorderToMyAdvantech(POCOS.Store store, POCOS.Order order)
        {
            this._store = store;
            this._order = order;
        }

        public override object execute(object obj)
        {
            if (this._store == null || this._order == null)
            {
                eStore.Utilities.eStoreLoger.Error("Send BB order to MyAdvantech failed. No order or store data");
                return null;
            }

            try
            {
                MyAdvantechBB.BBorderAPI bbAPI = new MyAdvantechBB.BBorderAPI();

                string url = _store.getStringSetting("BBOrderWebServiceURL");
                if (string.IsNullOrEmpty(url))
                    url = "http://my.advantech.com/Services/BBorderAPI.asmx";

                if (testMode() == true)
                    url = "http://my.advantech.com:4002/Services/BBorderAPI.asmx";

                bbAPI.Timeout = 30000;
                bbAPI.Url = url;
                bbAPI.CreateSAPOrderFromBBeStore(_order.OrderNo);
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error(string.Format("Send BB order to MyAdvantech failed. Order No: {0}", _order.OrderNo), "", "", _store.StoreID, ex);
            }
            return null;
        }
    }
}
