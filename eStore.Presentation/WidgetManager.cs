using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation
{
    public class WidgetManager
    {
        public User GetUser(string UserID)
        {
            return eStoreContext.Current.Store.getUser(UserID);
        }

        public POCOS.Product getProduct(string ProductID)
        {
            return eStoreContext.Current.Store.getProduct(ProductID);
        }
        public POCOS.Product_Ctos getCTOS(string CBOMID)
        {
            return (Product_Ctos)eStoreContext.Current.Store.getProduct(CBOMID);
        }

        public POCOS.Order getOrder()
        { return eStoreContext.Current.Order; }
    }
}
