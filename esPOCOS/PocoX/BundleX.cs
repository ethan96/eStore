using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;


namespace eStore.POCOS
{
    public partial class Bundle
    {
        /// <summary>
        /// Default constructor.  It's usually for entity framework to load entity from DB
        /// </summary>
        public Bundle()
        { }

        /// <summary>
        /// This shall be common constructor for creating new bundle instance
        /// </summary>
        /// <param name="storeID"></param>
        public Bundle(Product_Bundle creator)
        {
            this.StoreID = creator.StoreID;
            this.SourceTemplateID = creator.sourceTemplateID;
            if (this.SourceTemplateID.HasValue)   //dynamic product bundle instance
                this.SourceType = "DYNAMIC";
        }

        public void addItem(Part part, int qty, int sequence=1)
        {
            if (this.BundleItems == null)
                BundleItems = new List<BundleItem>();

            BundleItem item = new BundleItem(part, qty, sequence);
            this.BundleItems.Add(item);
            calculateWarranty();
        }

        public void removeItem(int itemid)
        {
            BundleItem item = this.BundleItems.FirstOrDefault(x => x.BundleItemID == itemid);
            removeItem(item);
        }
        public void removeItem(BundleItem item)
        {
            this.BundleItems.Remove(item);
        }


        /// <summary>
        /// adjustedPrice is a read-only property.  It returns discounted value
        /// </summary>
        public Decimal adjustedPrice
        {
            get  {  return this.BundleItems.Sum(x => x.adjustedTotal);  }
        }

        /// <summary>
        /// originalPrice is a read-only property.  It holds the original value (before discount) of current bundle
        /// </summary>
        public Decimal originalPrice
        {
            get { return this.BundleItems.Sum(x => x.total); }
        }

        public Bundle clone()
        {
            Bundle newBundle = new Bundle();
            newBundle.StoreID = this.StoreID;
            newBundle.SourceTemplateID = this.SourceTemplateID;
            newBundle.SourceType = this.SourceType;
            foreach (BundleItem item in BundleItems)
            {
                BundleItem newitem = new BundleItem();
                newitem.ItemSProductID = item.ItemSProductID;
                newitem.part = item.part;
                newitem.ItemDescription = item.ItemDescription;
                newitem.Qty = item.Qty;
                newitem.Sequence = item.Sequence;
                newitem.Price = item.Price;
                newitem.AdjustedPrice = item.AdjustedPrice;
                newitem.storeID = item.storeID;
                if (item.btosX != null)
                {
                    //do deep clone here
                    //item.BTOSystem = this.BTOSystem;  //shalow copy will be phased out
                    //if (String.IsNullOrEmpty(this.BTOSystem.storeID))
                    //    this.BTOSystem.storeID = this.StoreID;

                    //item.BTOSystem = this.BTOSystem.clone();
                    newitem.BTOSystem = item.btosX.clone();
                    newitem.BTOConfigID = newitem.BTOSystem.BTOConfigID;
                }
                 newBundle.BundleItems.Add(newitem);

            } 
         
            return newBundle;
        }

        public Dictionary<Part, int> parts
        {
            get
            {
                Dictionary<Part, int> partItems = new Dictionary<Part, int>();

                foreach (BundleItem item in BundleItems)
                {

                    if (partItems.ContainsKey(item.part))
                        partItems[item.part] = partItems[item.part] + item.quantity;
                    else
                        partItems.Add(item.part, item.quantity);

                }

                return partItems;
            }
        }

        /// <summary>
        /// This method is to adjust bundle item price to distribute price difference between 
        /// new bundle promotion price (after rounding and discount) and 
        /// the total price of net prices of all bundle items to eStore main products in bundle item list. 
        /// </summary>
        public void updatePrice(Decimal newPrice,bool isFromCartItem = false)
        {
            calculateWarranty();
            Decimal adjustableProductTotal = 0.0m;
            Decimal adjustableAllProductTotal = 0.0m;//CartItem.bundlex 调用的时候使用
            List<BundleItem> adjustableItems = new List<BundleItem>();

            foreach (BundleItem item in BundleItems)
            {
                if (item.part.isMainStream())
                {
                    adjustableProductTotal += item.Price.GetValueOrDefault() * item.Qty.GetValueOrDefault();
                    adjustableItems.Add(item);
                }

                if (isFromCartItem)//CartItem.bundlex 调用的时候使用
                    adjustableAllProductTotal += item.part.getNetPrice(false).value;
            }

            if (adjustableProductTotal <= 0) //no main item found
            {
                String errorMessage = String.Format("No price adjustment item found for price adjustment Bundle {0}", this.BundleID);
                eStoreLoger.Error(errorMessage, "", "", this.StoreID);
            }
            else
            {
                //calculate adjustRate
                Decimal priceDiff = 0.0m;
                if (isFromCartItem)
                    priceDiff = adjustableAllProductTotal - newPrice;
                else
                    priceDiff = this.adjustedPrice - newPrice;
                Decimal newAdjustableProductTotal = adjustableProductTotal - priceDiff;
                Decimal adjustRate = newAdjustableProductTotal / adjustableProductTotal;

                //adjust main item unit selling price
                Decimal adjustedMainItemTotal = 0;
                foreach (BundleItem item in adjustableItems)
                {
                    //item.AdjustedPrice = Math.Round(item.NetPrice.GetValueOrDefault() * adjustRate, 2);
                    item.AdjustedPrice = Converter.round(item.Price.GetValueOrDefault() * adjustRate, StoreID);
                    adjustedMainItemTotal += item.AdjustedPrice.GetValueOrDefault() * item.Qty.GetValueOrDefault();
                }

                //final adjustment after rounding, add the delta to the first adjustable item
                if (adjustableItems.Count() > 0 && (adjustedMainItemTotal != newAdjustableProductTotal))
                {
                    BundleItem adjustingitem = adjustableItems.First();
                    adjustingitem.AdjustedPrice = adjustingitem.AdjustedPrice.GetValueOrDefault() + (newAdjustableProductTotal - adjustedMainItemTotal) / adjustingitem.Qty.GetValueOrDefault();
                }
            }
        }

        public Boolean inited = false;
        /// <summary>
        /// init method shall be invoked before using any property in this bundle.  Its main purpose is to make sure
        /// all bundle items in it have storeID associated with.
        /// </summary>
        /// <param name="storeID"></param>
        public void init()
        {
            //this initialization is to assure each BundleItem has StoreID associated with
            foreach (BundleItem item in BundleItems)
                item.storeID = this.StoreID;

            inited = true;
        }

        private Decimal calculateWarranty()
        {
            Decimal warrantyPrice = 0m;
            //calculate warrantable item total cost
            Decimal warrantableCost = getWarrantableTotal();
            //find warranty items
            warrantyPrice = updateWarrantyItemPrice(warrantableCost);
            return warrantyPrice;
        }

        /// <summary>
        /// this method returns the total cost of bundle warrantable items
        /// </summary>
        /// <returns></returns>
        public Decimal getWarrantableTotal()
        {
            Decimal warranty = 0m;

            foreach (BundleItem item in this.BundleItems)
            {
                if (item.part != null && item.part.isWarrantable())
                {
                    if (item.part is Product_Ctos)
                        warranty += item.btosX.getWarrantableTotal() * item.Qty.GetValueOrDefault();
                    else
                        warranty += item.AdjustedPrice.GetValueOrDefault() * item.Qty.GetValueOrDefault();
                }
            }

            return warranty;
        }

        /// <summary>
        /// This method return matched warranty BTOSConfigDetail item associated with specifying part
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private Decimal updateWarrantyItemPrice(Decimal warrantablePrice)
        {
            Decimal warrantyPrice = 0m;
            foreach (BundleItem warrantyItem in BundleItems.Where(x => x.part != null && x.part.isWarrantyPart()))
            {
                warrantyItem.AdjustedPrice = Converter.round(warrantyItem.part.getNetPrice().value / 100 * warrantablePrice, StoreID);
                warrantyPrice += warrantyItem.adjustedTotal;
            }

            return warrantyPrice;
        }

        public void reconcile()
        {
            //更新bundle中的btos的价格,并把总价赋值给bundle
            if (this.BundleItems != null && this.BundleItems.Count > 0)
            {
                foreach (BundleItem item in this.BundleItems)
                {
                    if (item.btosX != null)
                    {
                        item.btosX.reconcile();
                        item.AdjustedPrice = item.btosX.Price;
                    }
                }
            }
            calculateWarranty();
        }

        public bool? refresh()
        {
            if (!inited)
                init();
            bool? result = null;
            PartHelper parthelper = new PartHelper();
            foreach (BundleItem item in BundleItems)
            {
                if (item.part != null)
                {
                    if (item.btosX != null)
                    {
                        result = item.btosX.refresh();
                        if (result.GetValueOrDefault())
                        {
                            reconcile();
                        }
                    }
                    else if (!item.part.isOrderableBase())
                    {
                        result = false;
                        break;
                    }
                    else if (item.Price != item.part.getNetPrice().value)
                    {
                        item.Price = item.part.getNetPrice().value;
                        item.AdjustedPrice = item.Price;
                        result = true;
                    }
                }
                else
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

    }

}
