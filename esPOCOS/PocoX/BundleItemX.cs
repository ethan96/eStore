using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    /// <summary>
    /// This class is to provide a way to create bundle item
    /// </summary>
    public partial class BundleItem
    {
        public String storeID = null;

        public BundleItem()
        {
        }

        public BundleItem(Part partItem, int qty, int sequence = 1, decimal adjustedPrice = 0m, string peripheralProducts = "", string peripheralCategoryName = "")
        {           
            part = partItem;
            quantity = qty;
            this.Sequence = sequence;
            this.ItemDescription = part.productDescX;
            if (adjustedPrice > 0)
                this.Price = adjustedPrice;
            else
                this.Price = part.getNetPrice(false).value;
            this.AdjustedPrice = this.Price;
            this.peripheralProducts = peripheralProducts;
            this.peripheralCategoryName = peripheralCategoryName;
        }
        /// <summary>
        ///   BTOSystem doesn't have storeID associated with.  To make sure StoreID is associated, this propert is a safe way
        ///   for retrieve BTOSystem with gurantee that storeID will be associated.
        /// </summary>
        public BTOSystem btosX
        {
            get
            {
                if (this.BTOSystem != null && String.IsNullOrEmpty(this.BTOSystem.storeID))
                {
                    this.BTOSystem.storeID = this.storeID;
                    this.BTOSystem.initPartReferences();
                }
                return this.BTOSystem;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private POCOS.Part _part;
        public Part part
        {
            get
            {
                if (_part == null)
                {
                    //fetch product on the run
                    if (!String.IsNullOrWhiteSpace(this.storeID))
                        _part = (new PartHelper()).getPart(this.ItemSProductID, this.storeID);
                }
                return _part;
            }

            set
            {
                _part = value;
                this.ItemSProductID = value.SProductID;
                this.storeID = value.StoreID;
            }
        }

        public int quantity
        {
            get  { return this.Qty.GetValueOrDefault(); }
            set { this.Qty = value;  }
        }

        public Decimal adjustedPrice
        {
            get { return AdjustedPrice.GetValueOrDefault(); }
            set { AdjustedPrice = value; }
        }

        /// <summary>
        /// This property holds the discounted value of this bundle item
        /// </summary>
        public decimal adjustedTotal
        {
            get {   return quantity * adjustedPrice;      }
        }

        /// <summary>
        /// This property holds the orignal value (before discount) of this bundle item
        /// </summary>
        public Decimal total
        {
            get { return quantity * Price.GetValueOrDefault();}
        }

        /// <summary>
        /// This method determine if itself is a warranty config
        /// <summary>
        public Boolean isWarrantyBundle()
        {
            return ItemDescription.ToUpper().Contains("EXTENDED WARRANTY");   
        }

        public string peripheralProducts { get; set; }

        public string peripheralCategoryName { get; set; }
    }
}
