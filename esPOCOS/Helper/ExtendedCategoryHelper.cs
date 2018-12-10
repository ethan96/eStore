using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class ExtendedCategoryHelper : Helper
    {
        /// <summary>
        /// get ExtendedCategory By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExtendedCategory getExtendedCategoryById(int id)
        {
            return context.ExtendedCategories.FirstOrDefault(c => c.ID == id);
        }

        public int save(ExtendedCategory extendedCategory)
        {
            if (extendedCategory == null || extendedCategory.validate() == false) return 1;
            ExtendedCategory _exist_extendedCategory = getExtendedCategoryById(extendedCategory.ID);
            try
            {
                if (_exist_extendedCategory == null)  //object not exist 
                {
                    context.ExtendedCategories.AddObject(extendedCategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ExtendedCategories.ApplyCurrentValues(extendedCategory);
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

        public int delete(ExtendedCategory extendedCategory)
        {
            if (extendedCategory == null || extendedCategory.validate() == false) return 1;
            try
            {
                var _exist_extendedCategory = getExtendedCategoryById(extendedCategory.ID);
                context.DeleteObject(_exist_extendedCategory);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public List<Product> getProducts(ExtendedCategory extendedCategory)
        {
            try
            {
                List<Product> products = (from p in context.Parts.OfType<Product>()
                                          from m in context.ProductCategroyMappings
                                          where p.SProductID == m.SProductID
                                          && p.StoreID == m.StoreID
                                          && p.StoreID == extendedCategory.StoreID
                                          && m.ExtendedCategoryID == extendedCategory.ID
                                          select p).ToList();
                return products;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Product>();
            }
          

        }
    }
}
