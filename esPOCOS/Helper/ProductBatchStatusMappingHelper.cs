using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ProductBatchStatusMappingHelper : Helper
    {

        internal int save(ProductBatchStatusMapping productBatchStatusMapping)
        {
            //if parameter is null or validation is false, then return  -1 
            if (productBatchStatusMapping == null || productBatchStatusMapping.validate() == false) return 1;
            //Try to retrieve object from DB             

            ProductBatchStatusMapping _exist_camp = geProductBatchStatusMappingByID(productBatchStatusMapping);
            try
            {
                if (productBatchStatusMapping.helper != null && productBatchStatusMapping.helper.context != null)
                    context = productBatchStatusMapping.helper.context;

                POCOS.Part part = null;
                if (_exist_camp == null)  //object not exist 
                {
                    context.ProductBatchStatusMappings.AddObject(productBatchStatusMapping); //state=added.                            
                    context.SaveChanges();


                    part = (new PartHelper()).getPart(productBatchStatusMapping.SProductId, productBatchStatusMapping.StoreId, true);
                    // will update product Marketing Status to old status 
                    if (part != null && part is POCOS.Product)
                    {
                        Product pro = part as Product;
                        pro.UpdateProductBatchStatus();
                        pro.save();
                    }

                    return 0;
                }
                else
                {
                    if (productBatchStatusMapping.IsPublish != _exist_camp.IsPublish)
                    {
                        var helper = new PartHelper();
                        part = helper.getPart(productBatchStatusMapping.SProductId, productBatchStatusMapping.StoreId, true);
                    }
                    //Update
                    context.ProductBatchStatusMappings.ApplyCurrentValues(productBatchStatusMapping);
                    context.SaveChanges();

                    // will update product Marketing Status to old status 
                    if (part != null && part is POCOS.Product)
                    {
                        Product pro = part as Product;
                        pro.UpdateProductBatchStatus();
                        pro.save();
                    }

                    return 0;
                }

            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);

                return -5000;
            }
        }

        internal int delete(ProductBatchStatusMapping productBatchStatusMapping)
        {
            if (productBatchStatusMapping == null || productBatchStatusMapping.validate() == false) return 1;
            productBatchStatusMapping = geProductBatchStatusMappingByID(productBatchStatusMapping);
            try
            {
                var helper = new PartHelper();
                var part = helper.getPart(productBatchStatusMapping.SProductId, productBatchStatusMapping.StoreId, true);

                context.ProductBatchStatusMappings.DeleteObject(productBatchStatusMapping);
                context.SaveChanges();

                // will update product Marketing Status to old status                
                if (part != null && part is POCOS.Product)
                {
                    Product pro = part as Product;
                    pro.UpdateProductBatchStatus();
                    pro.save();
                }
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public ProductBatchStatusMapping geProductBatchStatusMappingByID(ProductBatchStatusMapping productBatchStatusMapping)
        {
            return context.ProductBatchStatusMappings.Include("ProductBatchStatu").FirstOrDefault(c => c.Id == productBatchStatusMapping.Id
                || (c.StoreId == productBatchStatusMapping.StoreId && c.SProductId == productBatchStatusMapping.SProductId
                    && c.BatchId == productBatchStatusMapping.BatchId));
        }

        public List<ProductBatchStatusMapping> GetBatchStatusMappingByProduct(Product product, bool isIncludPhout = false)
        {
            List<ProductBatchStatusMapping> ls;
            if (!isIncludPhout)
            {
                ls = (from c in context.ProductBatchStatusMappings.Include("ProductBatchStatu")
                      where c.StoreId == product.StoreID && c.SProductId.Equals(product.SProductID, StringComparison.OrdinalIgnoreCase) && (c.IsPublish == null || c.IsPublish == true)
                          && ((!c.BatchId.HasValue && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now) ||
                                (c.BatchId.HasValue && c.ProductBatchStatu.IsPublish == true && c.ProductBatchStatu.StartDate <= DateTime.Now && c.ProductBatchStatu.EndDate >= DateTime.Now))
                          select c).ToList();
            }
            else
            {
                ls = (from c in context.ProductBatchStatusMappings.Include("ProductBatchStatu")
                          where c.StoreId == product.StoreID && c.SProductId.Equals(product.SProductID, StringComparison.OrdinalIgnoreCase)
                          select c).ToList();
            }
            return ls;
        }

        public List<ProductBatchStatusMapping> GetBatchStatusMappingWithAfterBegin(Product product)
        {
            List<ProductBatchStatusMapping> ls;
            ls = (from c in context.ProductBatchStatusMappings.Include("ProductBatchStatu")
                  where c.StoreId == product.StoreID && c.SProductId.Equals(product.SProductID, StringComparison.OrdinalIgnoreCase) && (c.IsPublish == null || c.IsPublish == true)
                  && ((!c.BatchId.HasValue && (DateTime.Now < c.StartDate || DateTime.Now < c.EndDate)) ||
                        (c.BatchId.HasValue && c.ProductBatchStatu.IsPublish == true && (DateTime.Now < c.ProductBatchStatu.StartDate || DateTime.Now < c.ProductBatchStatu.EndDate)))
                  select c).ToList();
            return ls;
        }

        public List<ProductBatchStatusMapping> getJustExpiredByStoreid(string storeid)
        {
            List<ProductBatchStatusMapping> ls = (from c in context.ProductBatchStatusMappings
                                                  where c.StoreId == storeid && (c.IsPublish == null || c.IsPublish == true) &&
                                                  c.BatchId == null && c.EndDate < DateTime.Now
                                                  select c).ToList();
            return ls;
        }

        public List<ProductBatchStatusMapping> getWillStartStatus(string storeid)
        {
            List<ProductBatchStatusMapping> ls = (from c in context.ProductBatchStatusMappings
                                                  where c.StoreId == storeid && (c.IsPublish == null || c.IsPublish == false) &&
                                                  c.BatchId == null && c.StartDate < DateTime.Now && c.EndDate >= DateTime.Now
                                                  select c).ToList();
            return ls;
        }
    }
}
