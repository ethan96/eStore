using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class StoreParameterHelper : Helper
    {
        public StoreParameter getStoreParameterByKeyAndStore(string storeid, string para, bool createDefault = false)
        {
            var p = context.StoreParameters.FirstOrDefault(c => c.StoreID == storeid && c.SiteParameter == para);
            if(createDefault)
                p = p ?? (p = new StoreParameter { StoreID = storeid, ParaDesc = para, SiteParameter = para, ParaValue = para });
            return p;
        }

        public List<StoreParameter> getStoreParameterByKey(string key)
        {
            return context.StoreParameters.Where(c => c.SiteParameter == key).ToList();
        }
    }
}
