using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class ProductLimitedResourceHelper : Helper
    {
        public ProductLimitedResource getProductLimitedResourceByRID(int resourceID, string sproductid, string storeid)
        {
            var _pro = (from c in context.ProductLimitedResources
                        where c.SProductID.ToUpper() == sproductid.ToUpper() && c.ResourceID == resourceID && c.StoreID == storeid
                        select c).FirstOrDefault();
            return _pro;
        }
        

        internal int save(ProductLimitedResource productLimitedResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (productLimitedResource == null) return 1;
            //Try to retrieve object from DB
            ProductLimitedResource _exist_productlimitedresource = 
                getProductLimitedResourceByRID(productLimitedResource.ResourceID, productLimitedResource.SProductID, productLimitedResource.StoreID);
            try
            {
                if (_exist_productlimitedresource == null)  //object not exist 
                {
                    //Insert
                    context.ProductLimitedResources.AddObject(productLimitedResource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductLimitedResources.ApplyCurrentValues(productLimitedResource);
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

        internal int delete(ProductLimitedResource productLimitedResource)
        {
            if (productLimitedResource == null) return 1;
            try
            {
                ProductLimitedResource _exist_productlimitedresource =
                    getProductLimitedResourceByRID(productLimitedResource.ResourceID, productLimitedResource.SProductID, productLimitedResource.StoreID);
                if (_exist_productlimitedresource != null)
                {
                    context.ProductLimitedResources.DeleteObject(_exist_productlimitedresource);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return -5000;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
