using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ProductWidgetMappingHelper : Helper
    {

        /// <summary>
        /// get product widget page mapping by id 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ProductWidgetMapping getProductWidgetMappingById(int p)
        {
            return context.ProductWidgetMappings.FirstOrDefault(c => c.Id == p);
        }

        public ProductWidgetMapping getProductWidgetMappByWidgetIdAndProId(int widgetid, string sproductid)
        {
            return context.ProductWidgetMappings.FirstOrDefault(c => c.WidgetPageID == widgetid && c.SProductID.Equals(sproductid,StringComparison.OrdinalIgnoreCase));
        }
        
        public List<ProductWidgetMapping> getMappingListByWidgetId(int widgetId)
        {
            return context.ProductWidgetMappings.Where(c => c.WidgetPageID == widgetId).ToList();
        }

        public List<ProductWidgetMapping> getMappingListBySproductId(string sproductid, string storeid)
        {
            return context.ProductWidgetMappings.Where(c => c.StoreID == storeid && c.SProductID == sproductid)
                    .OrderByDescending(c=>c.Id).ToList();
        }


        internal int save(ProductWidgetMapping productWidgetMapping)
        {
            if (productWidgetMapping == null || productWidgetMapping.validate() == false)
                return 1;
            try
            {
                var exitMapping = getProductWidgetMappingById(productWidgetMapping.Id);
                if (exitMapping == null)
                {
                    context.ProductWidgetMappings.AddObject(productWidgetMapping);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = productWidgetMapping.helper.context;
                    context.ProductWidgetMappings.ApplyCurrentValues(productWidgetMapping);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }        
        }

       
        internal int delete(ProductWidgetMapping productWidgetMapping)
        {
            if (productWidgetMapping == null)
                return 1;
            if (productWidgetMapping.Id != 0)
                productWidgetMapping = getProductWidgetMappingById(productWidgetMapping.Id);
            else if (productWidgetMapping.WidgetPageID != null && !string.IsNullOrEmpty(productWidgetMapping.SProductID))
                productWidgetMapping = getProductWidgetMappByWidgetIdAndProId(productWidgetMapping.WidgetPageID.GetValueOrDefault()
                        , productWidgetMapping.SProductID);
            else
                return 1;

            if (productWidgetMapping == null || productWidgetMapping.validate() == false) return 1;
            try
            {
                context.ProductWidgetMappings.DeleteObject(productWidgetMapping);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
