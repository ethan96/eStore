using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
namespace eStore.POCOS
{
    public partial class PeripheralAddOn
    {
        private Product _addOnProduct = null;
        private PartHelper _helper = new PartHelper();
        /// <summary>
        /// This method will return add-on product specified in this PeripheralAddIOn item.  The return product can be
        /// either standard product or bundle product depending on add-on item settings.
        /// </summary>
        /// <returns>add-on product</returns>
        public Product addOnProduct
        {
            get
            {
                if (_addOnProduct == null)
                {
                    _addOnProduct = getAssociatedProduct(this.AddOnProductID);
                    //construct dynamic bundle product if there are custom items specified in peripheral bundle items.
                    if (_addOnProduct != null)
                    {
                        if (_addOnProduct is Product_Bundle)
                        {
                            if (this.PeripheralAddOnBundleItems != null && this.PeripheralAddOnBundleItems.Count() > 0)
                            {
                                Product_Bundle productBundle = ((Product_Bundle)_addOnProduct).clone(this.AddOnItemID);
                                productBundle.ProductDesc = this.Description;
                                var idkpart = this.PeripheralAddOnBundleItems.FirstOrDefault(x => x.Sequence.GetValueOrDefault() == 0);
                                if (idkpart != null)
                                {
                                    productBundle.DisplayPartno = idkpart.SProductID;
                                    productBundle.ProductDesc = idkpart.Part.productDescX;
                                }
                                //compose dynamic product bundle items
                                foreach (PeripheralAddOnBundleItem item in PeripheralAddOnBundleItems)
                                    productBundle.addProductBundleItem(item.Part, item.Qty.GetValueOrDefault(), item.Sequence.GetValueOrDefault());

                                productBundle.initialize();

                                //update product bundle value
                                Bundle bundle = productBundle.bundle;
                                productBundle.priceSource = Part.PRICESOURCE.LOCAL;
                                productBundle.VendorSuggestedPrice = bundle.originalPrice;
                                productBundle.StorePrice = productBundle.VendorSuggestedPrice.GetValueOrDefault();

                                _addOnProduct = productBundle;
                            }
                        }
                        else if (_addOnProduct is Product)
                        { 
                        }
                    
                    }
                }

                return _addOnProduct;
            }
        }

        public Product addOnReversionProduct
        {
            get
            {
                if (_addOnProduct == null)
                {
                    _addOnProduct = getAssociatedProduct(this.AddOnProductID);
                    //construct dynamic bundle product if there are custom items specified in peripheral bundle items.
                    if (_addOnProduct != null && _addOnProduct is Product_Bundle &&
                            this.PeripheralAddOnBundleItems != null 
                            && this.PeripheralAddOnBundleItems.Count() > 0
                            && this.PeripheralAddOnBundleItems.Any(x => x.Sequence.GetValueOrDefault() != 0)
                        ) //regular product item
                    {
                        //use addOnProduct as template to create a new Product_Bundle instance
                        Product_Bundle productBundle = ((Product_Bundle)_addOnProduct).clone(this.AddOnItemID);
                        productBundle.ProductDesc = this.Product.productDescX;
                        productBundle.DisplayPartno = this.SProductID;
                        productBundle.addProductBundleItem(this.Product, 1, 0);
                      
                        //compose dynamic product bundle items
                        foreach (PeripheralAddOnBundleItem item in PeripheralAddOnBundleItems.Where(x => x.Sequence.GetValueOrDefault() != 0))
                            productBundle.addProductBundleItem(item.Part, item.Qty.GetValueOrDefault(), item.Sequence.GetValueOrDefault());

                        productBundle.initialize();

                        //update product bundle value
                        Bundle bundle = productBundle.bundle;
                        productBundle.priceSource = Part.PRICESOURCE.LOCAL;
                        productBundle.VendorSuggestedPrice = bundle.originalPrice;
                        productBundle.StorePrice = productBundle.VendorSuggestedPrice.GetValueOrDefault();

                        _addOnProduct = productBundle;
                    }
                }

                return _addOnProduct;
            }
        }
        /// <summary>
        /// This method is to find matched product
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        private Product getAssociatedProduct(String productID)
        {
            Part matched = _helper.getPart(productID, this.StoreID);
            if (matched is Product)     //add-on item only associates to eStore product
                return (Product)matched;   //forcing cast
            else
                return null;
        }

        /// <summary>
        /// Addon 下面的产品作为整体出售如果有一个料号不能出售 整个Addon 将不能出售
        /// </summary>
        /// <returns></returns>
        public bool isOrderEable()
        {
            bool isOrder = true;
            foreach (PeripheralAddOnBundleItem item in PeripheralAddOnBundleItems)
            {
                if (!item.Part.isOrderable())
                {
                    isOrder = false;
                    break;
                }
            }
            return isOrder;
        }

    }
}
