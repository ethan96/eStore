using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class ProductCategoryMetadataValueHelper : Helper { 
        #region Business Read
        public ProductCategoryMetadataValue get(int id)
        {
            return context.ProductCategoryMetadataValues.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.ProductCategoryMetadataValues.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(ProductCategoryMetadataValue _productcategorymetadatavalue)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_productcategorymetadatavalue == null || _productcategorymetadatavalue.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_productcategorymetadatavalue.Id ==0 || !isExists(_productcategorymetadatavalue.Id))  //object not exist 
                {
                    //Insert
                    context.ProductCategoryMetadataValues.AddObject( _productcategorymetadatavalue);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductCategoryMetadataValues.ApplyCurrentValues( _productcategorymetadatavalue);
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
        public int delete(ProductCategoryMetadataValue _productcategorymetadatavalue)
        {
       
            if (_productcategorymetadatavalue == null || _productcategorymetadatavalue.validate() == false) return 1;
            try
            {
                context.DeleteObject(_productcategorymetadatavalue);
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