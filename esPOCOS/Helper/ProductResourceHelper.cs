using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ProductResourceHelper : Helper
    {

        #region Business Read
        public ProductResource getProductResourceById(int id)
        {
            if (id == 0)
                return null;
            try
            {
                var _productResource = (from p in context.ProductResources
                                            where p.ResourceID == id
                                            select p).FirstOrDefault();
                if (_productResource != null)
                    _productResource.helper = this;
                return _productResource;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public ProductResource getProductResourceByinfor(ProductResource prSource)
        {
            if (prSource == null)
                return null;
            try
            {
                var _productResource = (from p in context.ProductResources
                                        where p.ResourceName == prSource.ResourceName && p.ResourceType == prSource.ResourceType
                                            && p.ResourceURL == prSource.ResourceURL && p.SProductID == prSource.SProductID && p.StoreID == prSource.StoreID
                                            && p.IsLocalResource == prSource.IsLocalResource
                                        select p).FirstOrDefault();
                if (_productResource != null)
                    _productResource.helper = this;
                return _productResource;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        #endregion

        #region Creat Update Delete
        public int delete(ProductResource prSource)
        {
            if (prSource == null)
                return 1;
            try
            {
                ProductResource _exitobj = null;
                if(prSource.ResourceID == 0)
                    _exitobj = getProductResourceByinfor(prSource);
                else
                    _exitobj = getProductResourceById(prSource.ResourceID);
                if (_exitobj == null)
                    return 1;
                context.ProductResources.DeleteObject(_exitobj);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(ProductResourceHelper).ToString();
        }
        #endregion

        internal int save(ProductResource productResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (productResource == null || productResource.validate() == false) return 1;
            //Try to retrieve object from DB
            ProductResource _exist_pr = getProductResourceById(productResource.ResourceID);
            try
            {
                if (_exist_pr == null)  //object not exist 
                {
                    //Insert
                    context.ProductResources.AddObject(productResource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductResources.ApplyCurrentValues(productResource);
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
    }
}