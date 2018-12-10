using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ProductCategroyMappingHelper : Helper
    {

        #region Business Read
        #endregion

        #region Creat Update Delete 
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(ProductCategroyMappingHelper).ToString();
        }
        #endregion

        internal int save(ProductCategroyMapping productCategroyMapping)
        {
            if (productCategroyMapping == null || productCategroyMapping.validate() == false) return 1;
            ProductCategroyMapping _exist_user = getProductCategroyMappingbyID(productCategroyMapping);
            try
            {
                if (_exist_user == null)  //object not exist 
                {
                    //Insert                  
                    context.ProductCategroyMappings.AddObject(productCategroyMapping);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update            
                    if (productCategroyMapping.helper != null && productCategroyMapping.helper.context != null)
                        context = productCategroyMapping.helper.context;

                    context.ProductCategroyMappings.ApplyCurrentValues(productCategroyMapping);
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

        public ProductCategroyMapping getProductCategroyMappingbyID(ProductCategroyMapping productCategroyMapping)
        {
            if (productCategroyMapping == null)
                return null;
            return context.ProductCategroyMappings.FirstOrDefault(c => c.StoreID == productCategroyMapping.StoreID &&
                            c.CategoryID == productCategroyMapping.CategoryID && c.SProductID == productCategroyMapping.SProductID);
        }

        internal int delete(ProductCategroyMapping productCategroyMapping)
        {
            throw new NotImplementedException();
        }

        public List<ProductCategroyMapping> getMappByTime(string storeid, DateTime startTime, DateTime endTime)
        {
            var ls = (from c in context.ProductCategroyMappings.Include("ProductCategory").Include("Product")
                      where c.StoreID == storeid && c.CreatedDate >= startTime && c.CreatedDate <= endTime
                      select c).ToList();
            return ls;
        }

        public bool UpdateProductMappingDefault(int categoryid, string sproductid, string storeid)
        {
            List<ProductCategroyMapping> lstemp = new List<ProductCategroyMapping>();
            var ls = context.ProductCategroyMappings.Where(c => c.StoreID == storeid && c.SProductID == sproductid);
            foreach (var item in ls.Where(c => c.Default == true))
            {
                item.Default = null;
                item.helper = this;
                lstemp.Add(item);
            }

            foreach (var item in lstemp)
                item.save();

            var defaultitem = ls.FirstOrDefault(c => c.CategoryID == categoryid);
            if (defaultitem != null)
            {
                defaultitem.Default = true;
                defaultitem.helper = this;
                return defaultitem.save() == 0;
            }
            return false;
        }

        public List<ProductCategroyMapping> GetMappingByProduct(Product product)
        {
            return context.ProductCategroyMappings.Include("ProductCategory")
                    .Where(c => c.SProductID == product.SProductID && c.StoreID == product.StoreID).ToList();
        }
    }
}