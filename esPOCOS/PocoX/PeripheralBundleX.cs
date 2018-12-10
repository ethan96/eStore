using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace eStore.POCOS
{
    public partial class PeripheralBundle
    {

     public   POCOS.Product_Bundle generateProductBundle()
        {
            Product_Bundle product_bundle = null;
            POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
            POCOS.Part part = new Part();
            part = helper.getPart(this.BundleProductID, this.StoreID);
            if (part is Product_Bundle)
            {
                 product_bundle = new Product_Bundle();
                product_bundle.SProductID = this.BundleProductID;
                product_bundle.StoreID = this.StoreID;
                product_bundle.DisplayPartno = part.name;
                product_bundle.ProductDesc = this.Description;
                product_bundle.ProductType = part.ProductType;
                foreach (PeripheralBundleItem item in this.PeripheralBundleItems)
                {
                    if (item.Part != null)
                    {
                        ProductBundleItem bi = new ProductBundleItem();
                        bi.Qty = 1;
                        bi.ItemSProductID = item.SProductID;
                        bi.Sequence = 0;
                        product_bundle.ProductBundleItems.Add(bi);
                    }
                }
            }

            return product_bundle;
        }
    }
}
