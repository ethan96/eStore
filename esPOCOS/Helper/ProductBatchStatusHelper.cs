// -----------------------------------------------------------------------
// <copyright file="ProductBatchStatusHelper.cs" company="Adv">
// Product Batch Status Base Helper
// </copyright>
// -----------------------------------------------------------------------

using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Product Batch Status Helper
    /// </summary>
    public class ProductBatchStatusHelper : Helper
    {

        internal int save(ProductBatchStatu productBatchStatus)
        {
            if (productBatchStatus == null || productBatchStatus.validate() == false)
                return 1;
            try
            {
                var exitAddress = getProductBatchStatuById(productBatchStatus.Id);
                if (exitAddress == null)
                {
                    context.ProductBatchStatus.AddObject(productBatchStatus);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context = productBatchStatus.helper.context;
                    context.ProductBatchStatus.ApplyCurrentValues(productBatchStatus);
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

        internal int delete(ProductBatchStatu productBatchStatus)
        {
            if (productBatchStatus == null || productBatchStatus.validate() == false) return 1;
            POCOS.ProductBatchStatu cm = getProductBatchStatuById(productBatchStatus.Id);
            var proids = cm.ProductBatchStatusMappings.Select(c => c.Id).ToList();
            try
            {
                foreach (var id in proids)
                    (new ProductBatchStatusMapping { Id = id }).delete();
                context = new eStore3Entities6();
                cm = context.ProductBatchStatus.FirstOrDefault(c => c.Id == cm.Id);
                context.ProductBatchStatus.DeleteObject(cm);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public ProductBatchStatu getProductBatchStatuById(int pid)
        {
            return context.ProductBatchStatus.Include("ProductBatchStatusMappings").FirstOrDefault(c => c.Id == pid);
        }

        public IEnumerable<ProductBatchStatu> getAllByStoreid(string storeid, DateTime? startdate = null, DateTime? enddate = null, bool? isPublish = null)
        {
            if (startdate != null && enddate != null)
            {
                var ls = (from c in context.ProductBatchStatus
                    where c.StoreId.Equals(storeid, StringComparison.OrdinalIgnoreCase) &&
                          c.StartDate <= startdate && c.EndDate >= enddate &&
                          (isPublish == null || c.IsPublish == isPublish)
                    select c);
                return ls;
            }
            else
                return from c in context.ProductBatchStatus
                            where c.StoreId.Equals(storeid, StringComparison.OrdinalIgnoreCase) && 
                            (isPublish == null || c.IsPublish == isPublish)
                            select c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public IEnumerable<ProductBatchStatu> getJustExpiredByStoreid(string storeid)
        {
            return from c in context.ProductBatchStatus
                   where c.StoreId.Equals(storeid, StringComparison.OrdinalIgnoreCase) && c.IsPublish == true &&
                   c.EndDate <= DateTime.Now
                   select c;
        }

    }
}
