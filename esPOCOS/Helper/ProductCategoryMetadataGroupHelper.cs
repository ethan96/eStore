using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class ProductCategoryMetadataGroupHelper : Helper { 
        #region Business Read
        public ProductCategoryMetadataGroup get(int id)
        {
            return context.ProductCategoryMetadataGroups.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.ProductCategoryMetadataGroups.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(ProductCategoryMetadataGroup _productcategorymetadatagroup)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_productcategorymetadatagroup == null || _productcategorymetadatagroup.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_productcategorymetadatagroup.Id ==0 || !isExists(_productcategorymetadatagroup.Id))  //object not exist 
                {
                    //Insert
                    context.ProductCategoryMetadataGroups.AddObject( _productcategorymetadatagroup);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductCategoryMetadataGroups.ApplyCurrentValues( _productcategorymetadatagroup);
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
        public int delete(ProductCategoryMetadataGroup _productcategorymetadatagroup)
        {
       
            if (_productcategorymetadatagroup == null || _productcategorymetadatagroup.validate() == false) return 1;
            try
            {
                context.DeleteObject(_productcategorymetadatagroup);
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