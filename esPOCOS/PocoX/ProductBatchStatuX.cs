using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ProductBatchStatu
    {
        public List<Product.PRODUCTMARKETINGSTATUS> getMarketingStatus()
        {
            var product = (new POCOS.Product { MarketingStatus = MarketingStatus.GetValueOrDefault() });
            return product.marketingstatus;
        }

        public void UpdateProductSatatus(int? newStatus = null)
        {
            var helper = new PartHelper();
            var productls = helper.prefetchPartList(this.StoreId, ProductBatchStatusMappings.Select(c => c.SProductId).ToList());
            //bool isReflash = (IsPublish == false || !(DateTime.Now >= StartDate && DateTime.Now <= EndDate));

            foreach (var item in ProductBatchStatusMappings.ToList())
            {
                var part = productls.FirstOrDefault(c=>c.SProductID == item.SProductId);
                if (part != null && part is POCOS.Product)
                { 
                    Product pro = part as Product;
                    pro.UpdateProductBatchStatus();
                    pro.save();
                }
            }
        }
    }
}
