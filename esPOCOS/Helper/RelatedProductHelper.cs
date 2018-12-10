using System;
using System.Collections.Generic;
using System.Linq;
using eStore.Utilities;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class RelatedProductHelper : Helper
    {
        internal int save(RelatedProduct relatedProduct)
        {
            //if parameter is null or validation is false, then return  -1 
            if (relatedProduct == null || relatedProduct.validate() == false) return 1;
            //Try to retrieve object from DB
            RelatedProduct _exist_relatedProduct = getRelatedProductById(relatedProduct.RelatedID, relatedProduct.StoreID);//
            try
            {
                if (_exist_relatedProduct == null)  //object not exist 
                {
                    //Insert
                    context.RelatedProducts.AddObject(relatedProduct);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.RelatedProducts.ApplyCurrentValues(relatedProduct);
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

        public RelatedProduct getRelatedProductById(int id, string storeid)
        {
            RelatedProduct relatedProduct = context.RelatedProducts.FirstOrDefault(r => r.RelatedID == id && r.StoreID == storeid);
            return relatedProduct;
        }

        public int delete(RelatedProduct relatedProduct)
        {
            if (relatedProduct == null || relatedProduct.validate() == false) return 1;
            try
            {
                var exrelatedProduct = getRelatedProductById(relatedProduct.RelatedID, relatedProduct.StoreID);
                if (exrelatedProduct != null)
                {
                    context.DeleteObject(exrelatedProduct);
                    context.SaveChanges();
                    return 0;
                }
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
