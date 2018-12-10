using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class ProductDependencyHelper : Helper
    {
        public ProductDependency getProductDependencyByRID(int DependencyID, string sproductid, string storeid)
        {
            var _pro = (from c in context.ProductDependencies
                        where c.SProductID.ToUpper() == sproductid.ToUpper() && c.DependencyID == DependencyID && c.StoreID == storeid
                        select c).FirstOrDefault();
            return _pro;
        }


        internal int save(ProductDependency productDependency)
        {
            //if parameter is null or validation is false, then return  -1 
            if (productDependency == null) return 1;
            //Try to retrieve object from DB
            ProductDependency _exist_productDependency =
                getProductDependencyByRID(productDependency.DependencyID, productDependency.SProductID, productDependency.StoreID);
            try
            {
                if (_exist_productDependency == null)  //object not exist 
                {
                    //Insert
                    context.ProductDependencies.AddObject(productDependency);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductDependencies.ApplyCurrentValues(productDependency);
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

        internal int delete(ProductDependency productDependency)
        {
            if (productDependency == null) return 1;
            try
            {
                ProductDependency _exist_productDependency =
                    getProductDependencyByRID(productDependency.DependencyID, productDependency.SProductID, productDependency.StoreID);
                if (_exist_productDependency != null)
                {
                    context.ProductDependencies.DeleteObject(_exist_productDependency);
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
