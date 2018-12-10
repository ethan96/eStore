using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// This class is to provide a way to create bundle
    /// </summary>
    public partial class BundleItem
    {
        public BundleItem()
        {
        }
        public BundleItem(Part partItem, int qty)
        {
            //this.ItemSProductID = part.SProductID;
            //this.Qty = qty;
            part = partItem;
            quantity = qty;
        }

        public BundleItem(Part partItem, int qty, int sequence)
        {
            //this.ItemSProductID = part.SProductID;
            //this.Qty = qty;

            part = partItem;
            quantity = qty;
            this.Sequence = sequence;
            this.ItemDescription = part.productDescX;
            this.Price = part.getListingPrice().value;
            this.AdjustedPrice = this.Price;

        }
        private POCOS.Part _part;
        public Part part
        {
            get
            {

                return _part;
            }
            set
            {
                _part = value;
                this.ItemSProductID = value.SProductID;
            }
        }

        public int quantity
        {
            get
            {
                return this.Qty;
            }
            set
            {
                this.Qty = value;
            }
        }
        public decimal subtotal
        {
            get {

                return quantity * AdjustedPrice.Value;
           }
        }
    }
}
