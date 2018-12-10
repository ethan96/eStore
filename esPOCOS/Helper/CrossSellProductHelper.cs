using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class CrossSellProductHelper : Helper
    {
        private static CachePool cache = CachePool.getInstance();

        public CrossSellProduct getCrossSellProduct(int crossSellProductid)
        {
            CrossSellProduct _crossSellProduct = null;
                _crossSellProduct = (from p in context.CrossSellProducts
                       where p.CrossSellProductID==crossSellProductid
                       select p).FirstOrDefault();

            if (_crossSellProduct != null)
                _crossSellProduct.helper = this;

            return _crossSellProduct;
        }

        #region Create Update Delete
        public int save(CrossSellProduct _crossSellProduct)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_crossSellProduct == null || _crossSellProduct.validate() == false) return 1;
            //Try to retrieve object from DB
            CrossSellProduct _exist_productcategory = getCrossSellProduct(_crossSellProduct.CrossSellProductID);
            try
            {
                if (_exist_productcategory == null)  //object not exist 
                {
                    //Insert
                    context.CrossSellProducts.AddObject(_crossSellProduct);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.CrossSellProducts.ApplyCurrentValues(_crossSellProduct);
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

        public int delete(CrossSellProduct _crossSellProduct)
        {
            CrossSellProduct _exist_productcategory = getCrossSellProduct(_crossSellProduct.CrossSellProductID);
            if (_exist_productcategory == null || _exist_productcategory.validate() == false) return 1;
            try
            {
                context.CrossSellProducts.DeleteObject(_exist_productcategory);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion
    }
}
