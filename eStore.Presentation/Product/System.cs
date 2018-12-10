using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Presentation.Product
{
    public class System
    {
          static POCOS.Product_Ctos getSystem(string Productid)
        {
            POCOS.Product_Ctos ctos;
            string key = string.Format("{0}_CTOS_{1}", eStoreContext.Current.Store.storeID , Productid);
            object obj2 = eStoreCache.Get(key);
            if ( (obj2 != null))
            {
                return (POCOS.Product_Ctos)obj2;
            }
            ctos = (POCOS.Product_Ctos)eStoreContext.Current.Store.getProduct(Productid);

            eStoreCache.Max(key, ctos);
            return ctos;
        }

    }
}
