using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class ProductCategoryMetadataHelper : Helper { 
        #region Business Read
        public ProductCategoryMetadata get(int id)
        {
            return context.ProductCategoryMetadatas.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.ProductCategoryMetadatas.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(ProductCategoryMetadata _productcategorymetadata)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_productcategorymetadata == null || _productcategorymetadata.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_productcategorymetadata.Id ==0 || !isExists(_productcategorymetadata.Id))  //object not exist 
                {
                    //Insert
                    context.ProductCategoryMetadatas.AddObject( _productcategorymetadata);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductCategoryMetadatas.ApplyCurrentValues( _productcategorymetadata);
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
        public int delete(ProductCategoryMetadata _productcategorymetadata)
        {
       
            if (_productcategorymetadata == null || _productcategorymetadata.validate() == false) return 1;
            try
            {
                context.DeleteObject(_productcategorymetadata);
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