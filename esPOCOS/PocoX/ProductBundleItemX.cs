using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{
    public partial class ProductBundleItem
    {
        /// <summary>
        /// Default constructor.  This constructor is usually for entity frame
        /// </summary>
        public ProductBundleItem()
        {
        }

        private Part _part;
        public Part part
        {
            get {
                if (_part == null)
                {
                    PartHelper helper = new PartHelper();
                    _part = helper.getPart(this.ItemSProductID,this.StoreID,true);
                }
                return _part;
            }
            set { _part = value; }
        }
        
        public ProductBundleItem(Product_Bundle container, Part part, int qty, int seq = 1)
        {
            //when navigation property is assigned, it's associated key values shall also be automatically populated.
            //Therefore there shall be no need for initializing storeID and productID
            this.part = part;
            this.ItemSProductID = part.SProductID;
            this.Product_Bundle = container;
            if (qty <= 0)
                qty = 1;    //quality can not be less than 1
            this.Qty = qty;
            this.Sequence = seq;
        }
    }
}
