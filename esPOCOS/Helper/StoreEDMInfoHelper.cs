namespace eStore.POCOS.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class StoreEDMInfoHelper : Helper
    {
        public StoreEDMInfo getStoreEDMInfoByStoreID(string storeid)
        {
            var _StoreEDMInfo = (from storeEDMInfo in context.StoreEDMInfoes
                                 where storeEDMInfo.StoreId == storeid
                                 select storeEDMInfo).FirstOrDefault();

            if (_StoreEDMInfo != null)
            {
                _StoreEDMInfo.helper = this;
            }

            return _StoreEDMInfo;
        }
    }
}
