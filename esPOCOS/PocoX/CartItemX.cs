using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

 namespace eStore.POCOS{ 

public partial class CartItem
{

    #region Extesion Properties
    /// <summary>
    /// Tag description
    /// 1. AssignedBoxPack - Assigned a box for a cart item
    /// 2. IndividualPack - 1 product in 1 box
    /// 3. SingleProductTypeMergePack - 
    /// 4. MultipleProductTypeMergePack     /** Reserved **/
    /// 5. SpecialSizePack - Special
    /// 6. Ctos pack
    /// </summary>
    public enum PackingTag {IndividualPack, 
                                        SingleProductTypeMergePack,
                                        WeightPack,
                                        MultipleProductTypeMergePack,
                                        CtosPack,
        BundlePack
    };
    private PackingTag packTag;
    public PackingTag PackTag
    {
        get
        {
            return packTag;
        }
        set
        {
            packTag = value;
        }
    }

    private string _currencySign = null;
    public string currencySign
    {
        get 
        { 
            if(string.IsNullOrEmpty(_currencySign))
                _currencySign = cartX.currencySign; 
            return _currencySign; 
        }
        set { _currencySign = value; }
    }

    private eStore.POCOS.CTOSBOM.COMPONENTTYPE? _relatedTypeX;
    public eStore.POCOS.CTOSBOM.COMPONENTTYPE relatedTypeX
    {
        get
        {
            if (!_relatedTypeX.HasValue)
            {
                eStore.POCOS.CTOSBOM.COMPONENTTYPE type = eStore.POCOS.CTOSBOM.COMPONENTTYPE.UNKNOWN;
                if (this.RelatedType.HasValue && this.RelatedType < 1000) //1000 is for product dependency
                    Enum.TryParse<eStore.POCOS.CTOSBOM.COMPONENTTYPE>(this.RelatedType.ToString(), out type);
                _relatedTypeX = type;
            }
            return _relatedTypeX.Value;
        }
        set
        {
            RelatedType = (int)value;
            _relatedTypeX = value;
        }
    }

    #endregion

    #region Extension Methods

        private Part _partX = null;
        private Cart _cartX = null;
        private string _packageTrackingN0 = null;
        public string PackageTrackingNo
        {
            get { return _packageTrackingN0; }
            set { _packageTrackingN0 = value; }
        }
        public string PackageTrackingStatus { get; set; }
        
        /// <summary>
        /// Default constructor : shall be used only by POCO entity framework. eStore application shall not use this constructor
        /// </summary>
        public CartItem() { }

        /// <summary>
        /// This contructor shall be used as the only constructor.
        /// </summary>
        /// <param name="itemNo">an integrate indicating item sequence no</param>
        /// <param name="part"></param>
        /// <param name="orderQuantity"></param>
        /// <param name="btos">this parameter is an optional item and shall be used for adding BTOS related cart item</param>
        /// <param name="adjustedPrice"> the parameter is optional and only need to be provided when the unit price is overwrited by sales or for promotion</param>
        /// <param name="customerNote">optional</param>
        /// <param name="autoOrder">optional</param>
        /// <param name="autoOrderQty">optional</param>
        /// 
        public CartItem(int itemNo, Part partItem, Cart cart, int orderQuantity, BTOSystem btos=null, Decimal adjustedPrice = 0,
                                String customerNote = null, Boolean autoOrder = false, int autoOrderQty = 0)
        {
            //Fill up cart item attributes
            if (autoOrder)
            {
                AutoOrderFlag = autoOrder;
                AutoOrderQty = autoOrderQty;
            }
            else
            {
                AutoOrderFlag = autoOrder;
                AutoOrderQty = 0;
            }

            cartX = cart;
            //CartID = cart.CartID;
            partX = partItem;    //SProductID will be automatically populate in this assignment
            //SProductID = part.SProductID;
            

            CustomerMessage = customerNote;
            DueDate = DateTime.Now.AddDays(2);      //this field is reserved for future use. 
            ItemNo = itemNo;

            if (partX is Product)
            {
                Description = partX.productDescX;

                switch (((Product)partX).productType)
                {
                    case Product.PRODUCTTYPE.CTOS:
                        ItemType = "CTOS";    //Configurable product
                        break;
                    case Product.PRODUCTTYPE.STANDARD:
                    default:
                        ItemType = "FG";    //finished goods
                        break;
                    case Product.PRODUCTTYPE.BUNDLE:
                        ItemType = "BUNDLE";    //Configurable product

                        this.Bundle = (partX as Product_Bundle).bundle.clone(); 
                        break;
                }
            }
            else   //regular part
            {
                Description = partX.productDescX;
                ItemType = "FG";
            }

            if (partX is Product_Ctos)    //if it's CTOS
                UnitPrice = btos.Price;
            else
            {
                //getListingPrice is a override method implemented in both Product and Part.
                //If part is a product, it'll call the implementation in Product. Otherwise
                //it calls the implementation in Part.
                Price listingPrice = partX.getListingPrice();

                UnitPrice = listingPrice.value; //unit price will be product listing price
            }

            if (adjustedPrice >0) //no overwritten price, use Product listing price
                UnitPrice = adjustedPrice;

            Qty = orderQuantity;
            RequiredDate = DueDate;
            SupplierDueDate = DueDate;

            updateTotal();

            if (btos != null)
            {
                this.BTOSystem = btos;
                if (adjustedPrice > 0)   //btos price is not initiated yet
                    btos.updatePrice(adjustedPrice);
                BTOConfigID = btos.BTOConfigID;
            }

        }

        /// <summary>
        /// This is a safely property. The navigation property shall not be used directly
        /// </summary>
        public Part partX
        {
            get
            {
                if (_partX == null)
                {
                    if (Part != null && Part is Product_Bundle && this.bundleX != null)
                    {
                        _partX = ((Product_Bundle)Part).clone(bundleX, 0);
                    }
                    else
                    _partX = Part;   //get navigation property
                }
                return _partX;
            }

            set
            {
                _partX = value;
                SProductID = (_partX == null) ? "" : _partX.SProductID;
                if (_partX != null && (_partX is Product))
                        this.ProductName = ((Product)_partX).name;
                else
                    this.ProductName = SProductID;
            }
        }

        public Cart cartX
        {
            get
            {
                if (_cartX == null)
                {
                    _cartX = Cart;   //get navigation property
                }
                return _cartX;
            }

            set
            {
                _cartX = value;
                
                //Add by Edward, need to set storeid
                if (_cartX != null) {
                    CartID =  _cartX.CartID;
                    StoreID = _cartX.StoreID;
                }
                
                
                
            }
        }

        /// <summary>
        /// This method shall be invoked whenever BTOS is modified after it's added to Cart. Fail to do so will
        /// result with Pricing discrepency problem.
        /// </summary>
        public void reflectBTOSChange()
        {
            if (this.partX is Product_Ctos && BTOSystem != null)
            {
                UnitPrice = BTOSystem.Price;
                updateTotal();
            }
        }

    /// <summary>
    /// this function for reset bunlde if changed default bundle item such as add btos to assebly item 
    /// </summary>
    /// <param name="bundle"></param>
        public void resetBundle(POCOS.Bundle bundle)
        {
            this.Bundle = bundle;
            updateUnitPrice(bundle.adjustedPrice);
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateTotal()
        {
            AdjustedPrice = UnitPrice * Qty;
        }

        /// <summary>
        /// Call this method to update order quantity
        /// </summary>
        /// <param name="qty"></param>
        public void updateQty(int qty)
        {
            Qty = qty;
            updateTotal();
            if (this.warrantyItemX != null)
            {
                this.warrantyItemX.updateQty(qty);
            }
        }

        public void updateUnitPrice(Decimal price)
        {
            UnitPrice = price;
            updateTotal();
            if (this.warrantyItemX != null && warrantyItemX.partX!=null &&warrantyItemX.partX.isWarrantyPart())
            {
                this.warrantyItemX.updateUnitPrice(warrantyItemX.partX.getNetPrice().value * UnitPrice / 100);
            }
        }
        private CartItem _warrantyItemX;
        public CartItem warrantyItemX
        {
            get {
                if (_warrantyItemX == null)
                {
                    if (this.WarrantyItem.HasValue)
                    {
                        _warrantyItemX= this.cartX.getItem(this.WarrantyItem.Value);
                    }
                }
                return _warrantyItemX;
            }
            set
            {
                _warrantyItemX = value;
                if (value == null)
                    WarrantyItem = null;
                else
                    WarrantyItem = value.ItemNo;
            }
        }

        public bool hasWarrantyItem()
        {
            bool reslut = false;
            switch (this.type)
            {
                case POCOS.Product.PRODUCTTYPE.CTOS:
                    {
                        reslut = this.btosX.BTOSConfigs.FirstOrDefault(x => x.isWarrantyConfig()) != null;
                    }
                    break;
                case POCOS.Product.PRODUCTTYPE.BUNDLE:
                    reslut = this.bundleX.BundleItems.FirstOrDefault(x => x.part != null && x.part.isWarrantyPart()) != null;
                    break;

                case POCOS.Product.PRODUCTTYPE.STANDARD:
                default:
                    if (this.warrantyItemX!=null)
                        reslut = true;
                    break;
            }
            return reslut;

        }
        public Decimal getWarrantableTotal()
        { 
            Decimal reslut=0m;
            switch (this.type)
            {
                case POCOS.Product.PRODUCTTYPE.CTOS:
                    {
                        reslut = this.btosX.getWarrantableTotal();
                    }
                    break;
                case POCOS.Product.PRODUCTTYPE.BUNDLE:
                    reslut = this.bundleX.getWarrantableTotal();
                    break;

                case POCOS.Product.PRODUCTTYPE.STANDARD:
                default:

                    if (this.partX.isWarrantable())
                        reslut = this.UnitPrice;
                    break;
            }
            return reslut;
        }

        public Product.PRODUCTTYPE type
        {
            get
            {
                Product.PRODUCTTYPE type;

                switch (ItemType)
                {
                    case "CTOS":
                        type = Product.PRODUCTTYPE.CTOS;
                        break;
                    case "BUNDLE":
                        type = Product.PRODUCTTYPE.BUNDLE;
                        break;
                    case "WARRANTY":
                        type = Product.PRODUCTTYPE.WARRANTY;
                        break;
                    case "FG":
                    default:
                        type = Product.PRODUCTTYPE.STANDARD;
                        break;
                }

                return type;
            }

            set
            {
                switch (value)
                {
                    case Product.PRODUCTTYPE.CTOS:
                        ItemType = "CTOS";
                        break;
                    case Product.PRODUCTTYPE.BUNDLE:
                        ItemType = "BUNDLE";
                        break;
                    case Product.PRODUCTTYPE.WARRANTY:
                        ItemType = "WARRANTY";
                        break;
                    case Product.PRODUCTTYPE.STANDARD:
                    default:
                        ItemType = "FG";
                        break;
                }
            }
        }

        /// <summary>
        /// The dimension unit in CM. Default value will be 5cm
        /// </summary>
        public Decimal height
        {
            get 
            {
                if (this.partX  != null)
                    return partX.DimensionHeightCM.GetValueOrDefault();
                else
                    return 5.0m; //default value when there is no available information
            }
        }

        /// <summary>
        /// The dimension unit in CM. Default value will be 10cm
        /// </summary>
        public Decimal length
        {
            get 
            {
                if (this.partX != null)
                    return partX.DimensionLengthCM.GetValueOrDefault();
                else
                    return 10.0m; //default value when there is no available information
            }
        }

        /// <summary>
        /// The dimension unit in CM. Default value will be 10cm
        /// </summary>
        public Decimal width
        {
            get 
            {
                if (this.partX != null)
                    return partX.DimensionWidthCM.GetValueOrDefault();
                else
                    return 10.0m; //default value when there is no available information
            }
        }

        /// <summary>
        /// The value is just the weight of single item.  If the item quantity is more than 1, the total weight has to be mulitple
        /// quantity with this weight value.
        /// </summary>
        public Decimal weight
        {
            get
            {
                Decimal _weight = 0;
                if (this.BTOSystem != null) // ctos product
                {
                    foreach (var cc in this.BTOSystem.BTOSConfigsWithoutNoneItems)
                    {
                        foreach (var c in cc.parts)
                            _weight += c.Key.ShipWeightKG.GetValueOrDefault()*c.Value;
                    }
                }
                if (this.bundleX != null) // bundel prodct
                {
                    foreach (var c in this.bundleX.parts)
                        _weight += c.Key.ShipWeightKG.GetValueOrDefault()*c.Value;
                }
                if (this.partX != null)
                    return _weight + partX.ShipWeightKG.GetValueOrDefault();
                else
                    return 0.225m; //default value when there is no available information, about 0.5lb
            }
        }

        /// <summary>
        /// This method is only applicable to standard product item.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="part"></param>
        public void addOrderQuantity(int quantity, Part part = null)
        {
            updateQty(Qty + quantity);
            if ((part != null) && !(part is Product_Ctos))
            {
                //adjust unit price and adjusted price
                Price listingPrice = partX.getListingPrice();
                updateUnitPrice(listingPrice.value); //unit price will be product listing price

                //updateTotal();
            }
        }


        /// <summary>
        /// This method is to perform deep copy to copy current value to input cart item
        /// </summary>
        /// <param name="item"></param>
        public void copyTo(CartItem item)
        {
            item.AutoOrderFlag = this.AutoOrderFlag;
            item.AutoOrderQty = this.AutoOrderQty;
            //item.cartX = this.cartX;
            item.StoreID = this.StoreID;
            item.partX = this.partX;    //SProductID will be automatically populate in this assignment
            item.CustomerMessage = this.CustomerMessage;
            item.DueDate = DateTime.Now.AddDays(2);      //this field is reserved for future use. 
            item.ItemNo = this.ItemNo;
            item.OldItemNo = this.OldItemNo;
            item.Description = this.Description;//partX.productDescX;
            item.ItemType = this.ItemType;
            item.UnitPrice = this.UnitPrice;
            item.AdjustedPrice = this.AdjustedPrice;
            item.Qty = this.Qty;
            item.RequiredDate = item.DueDate;
            item.SupplierDueDate = item.DueDate;
            item.WarrantyItem = this.WarrantyItem;
            item.DiscountAmount = this.DiscountAmount;
            item.PromotionMessage = this.PromotionMessage;
            item.AffiliateID = (AffiliateID == null) ? null : String.Copy(AffiliateID);
            //if (this.BTOSystem != null)
            if (this.btosX != null)
            {
                //do deep clone here
                //item.BTOSystem = this.BTOSystem;  //shalow copy will be phased out
                //if (String.IsNullOrEmpty(this.BTOSystem.storeID))
                //    this.BTOSystem.storeID = this.StoreID;

                //item.BTOSystem = this.BTOSystem.clone();
                item.BTOSystem = this.btosX.clone();
                item.BTOConfigID = item.BTOSystem.BTOConfigID;
            }
            if (this.bundleX != null)
            {
                item.Bundle = this.bundleX.clone();
            }

            updateTotal();
        }

        public ATP atp {
            get {
                ATP rlt;
                try
                {
                    if (this.type == Product.PRODUCTTYPE.CTOS)
                    {
                        //this.BTOSystem.initPartReferences();
                        //rlt = this.BTOSystem.BTOSConfigs.OrderByDescending(x => x.atp.availableDate).First().atp;
                        this.btosX.initPartReferences(true);
                        rlt = this.btosX.BTOSConfigs.OrderByDescending(x => x.atp.availableDate).First().atp;
                    }
                    else
                        rlt = this.partX.atp;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Exception at CartItem ATP", CartID, this.SProductID, this.StoreID, ex);
                    rlt = new ATP(DateTime.MaxValue, 0) { Message = "Exception at CartItem ATP" };
                }
                return rlt;
            }
        }

        public String productNameX
        {
            get
            {
                if (String.IsNullOrEmpty(ProductName))
                {
                    if (partX != null && (partX is Product))
                        return ((Product)partX).name;
                    else
                        return SProductID;
                }
                else
                    return ProductName;
            }
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
                    this.BTOSystem.storeID = this.StoreID;

                return this.BTOSystem;
            }
        }

        /// <summary>
        /// bundleX is the safe way of accessing Bundle instance in cart item.   Do not use Bundle in Cart item directly
        /// </summary>
        public Bundle bundleX
        {
            get {
                if (this.Bundle != null && this.Bundle.inited == false)
                {
                    this.Bundle.StoreID = this.StoreID;
                    this.Bundle.init();
                }
                return this.Bundle;
            }
        }

        public CartItem addWarranty(Part warranty)
        {
            CartItem warrantyItem = null;
            if (warranty.isWarrantyPart())
            {

                if (this.partX is Product_Ctos)
                {
                    //isSBCCTOS only
                    Product_Ctos ctos = (Product_Ctos)this.partX;
                    if (ctos.isSBCCTOS())
                    {
                        //remove existent item first
                        //BTOSConfig config = this.btosX.BTOSConfigs.FirstOrDefault(x => x.BTOSConfigDetails.Contains(x.BTOSConfigDetails.FirstOrDefault(d => d.partX!=null&&d.partX.isWarrantyPart())));
                        BTOSConfig config = (from con in this.btosX.BTOSConfigs
                                             where (from detail in con.BTOSConfigDetails
                                                    where detail.partX != null && detail.partX.isWarrantyPart()
                                                    select detail).FirstOrDefault() != null
                                             select con
                                                ).FirstOrDefault();

                        if (config!= null)
                        { 
                            this.btosX.BTOSConfigs.Remove(config); 
                        }
                        this.btosX.addNoneCTOSItem(warranty, 1);
                        this.btosX.reconcile();
                        this.UnitPrice = this.btosX.Price;
                        updateTotal();
                    }
                }
                else if (this.partX is Product_Bundle)
                {
                    if (this.bundleX != null)
                    {
                        BundleItem bi = this.bundleX.BundleItems.FirstOrDefault(x => x.part != null && x.part.isWarrantyPart());
                       
                        if (bi != null)
                        {
                            this.bundleX.removeItem(bi);
                        }
                        this.bundleX.addItem(warranty, 1, 999);
                        this.bundleX.reconcile();
                        this.UnitPrice = this.bundleX.adjustedPrice;
                        updateTotal();
                    }
                
                }
                else
                {
                    //remove existent item first
                    if (this.warrantyItemX != null)
                    {
                        this.cartX.removeItem(this.warrantyItemX);
                    }
                    warrantyItem = new CartItem(this.cartX.nextAvailableId, warranty, this.cartX, this.Qty, null,
                       Converter.CartPriceRound(this.UnitPrice * warranty.getNetPrice().value / 100, this.StoreID),
                        null, false, 0);
                    warrantyItem.type = Product.PRODUCTTYPE.WARRANTY;
                    this.warrantyItemX = warrantyItem;
                    this.cartX.CartItems.Add(warrantyItem);
                }
            }
          
            return warrantyItem;
        }
        public void removeWarranty()
        {
            if (this.partX is Product_Ctos)
            {
                //isSBCCTOS only
                Product_Ctos ctos = (Product_Ctos)this.partX;
                if (ctos.isSBCCTOS())
                {
                    BTOSConfig config = this.btosX.BTOSConfigs.FirstOrDefault(x => x.BTOSConfigDetails.Contains(x.BTOSConfigDetails.FirstOrDefault(d => d.partX != null && d.partX.isWarrantyPart())));
                    if (config != null)
                    {
                        this.btosX.BTOSConfigs.Remove(config);
                    }
                }
            }
            else
            {
                if (this.WarrantyItem.HasValue)
                {
                    this.cartX.removeItem(this.WarrantyItem.Value);
                    this.warrantyItemX = null;
                }
            }
        }

        public string ABCInd
        {
            get
            {
                if (this.partX != null)
                    return partX.ABCInd;
                else
                    return "";
            }
        }

        /// <summary>
        /// 如果CartItem为copy item，OldItemNo 会记录旧的item no.
        /// </summary>
        public int OldItemNo { get; set; }

        #endregion

        #region OM_Only
        /// <summary>
        /// This method is an expensive method. When it's invoked, it will trigger a process to complete reconcile and recalculate
        /// entire Cart price.  This method shall only be triggered by cart or order.
        /// </summary>
        public void reconcile()
        {
            //reconcile BTOS changes
            if (btosX != null)
            {
                btosX.reconcile();
                UnitPrice = btosX.Price;
            }

            if (bundleX != null)
            {
                bundleX.reconcile();
                UnitPrice = bundleX.adjustedPrice;
            }

            this.updateTotal();
        }

#endregion
} 

}