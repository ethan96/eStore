using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class PopularModelHelper : Helper
    {
        public PopularModel getPopulareProductByModel(string modelName, string storeId)
        {
            var modelItem = (from pm in context.PopularModels
                             where pm.ModelName == modelName && pm.StoreId == storeId && pm.Publish == true 
                             select pm).FirstOrDefault();

            return modelItem;
        }

        public List<PopularModel> getPopulareProductByModel(List<string> modelName, string storeId)
        {
            var modelItem = (from pm in context.PopularModels
                             where modelName.Contains(pm.ModelName) && pm.StoreId == storeId && pm.Publish == true 
                             select pm).ToList();

            return modelItem;
        }
    }
}
