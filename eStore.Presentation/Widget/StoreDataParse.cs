using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace eStore.Presentation.Widget
{
    public class StoreDataParse
    {
        public static  System.Type getStoreDataType(StoreDataType storeDataType)
        {
            System.Type type;

            type = Assembly.Load("esPOCOS").GetType("eStore.POCOS." + storeDataType.ToString());
            return type;
        }
        public static PropertyInfo[] getPropertyInfo(System.Type type)
        {
            return type.GetProperties();
        }
    }

   
}
