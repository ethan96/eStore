using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using eStore.POCOS.PocoX;

namespace eStore.POCOS
{

    /// <summary>
    /// Cart is a simple unit to retain shopping item list for Shopping cart, Quotation and Orders
    /// </summary>

    public partial class Cart : PromotionCodeEnabled
    {

        #region Extension Busines Logic Methods
        private bool _modified;
        private int _nextId = 0;
        //the following attributes are used to provide safely properties
        private Store _storeX = null;
        private User _userX = null;
        private Boolean validated = false;

        /// <summary>
        /// Constructor only for entity framework : eStore application shall not use this constructor
        /// </summary>
        public Cart() { }

        /// <summary>
        /// Default constructor for creating new Cart.  Cart ID will be in the format of store id + "Cart" + creating date and time
        /// </summary>
        /// <param name="cart"></param>
        /// 
        public Cart(Store actingStore, User actingUser)
            : this()
        {
            //store and user shall be stored here. In current implementation if store and user references are assigned, their records
            //will be updated along with CartMaster save activity.  To prevent accidental overwrite we use additional references here.
            storeX = actingStore;  //storeID will be automatically populated in this assignment
            userX = actingUser; //userId will be automatically populated in this assignment

            CreatedBy = userX.UserID;
            Status = "open";
            Currency = storeX.defaultCurrency.CurrencyID;
            CartID = storeX.StoreID + ".Cart." + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            CreatedDate = DateTime.Now;
            LastUpdateDate = CreatedDate;

            Status = "open";

            CartItems = new List<CartItem>();
        }

        /// <summary>
        /// In destructor, updated Cart will be stored before the instance is released.
        /// </summary>
        ~Cart()
        {
            /*
            if (_modified && !forTestOnly)
                save();
             * */
        }

        /// <summary>
        /// This is safely property for retrieving store reference.  No public access to navigation property Store shall be done.
        /// </summary>
        public Store storeX
        {
            get
            {
                if (_storeX == null)
                    _storeX = Store;

                return _storeX;
            }

            set
            {
                _storeX = value;
                StoreID = (_storeX == null) ? "" : _storeX.StoreID;
            }
        }

        public MiniSite minisiteX
        {
            get 
            {
                if (this.MiniSite != null)
                    return this.MiniSite;
                else if (this.MiniSiteID.HasValue)
                    return storeX.MiniSites.FirstOrDefault(c => c.ID == this.MiniSiteID.GetValueOrDefault());
                else
                    return this.MiniSite;
            }
        }

        /// <summary>
        /// cartItemsX will return only non-warranty cart item.  
        /// To get entire cart items including warranty items in cart, please use CartItems
        /// </summary>
        public List<CartItem> cartItemsX
        {
            get
            {
                return (from item in this.CartItems
                        where item.type != Product.PRODUCTTYPE.WARRANTY
                        orderby item.ItemNo 
                        select item).ToList();

            }
        }
        /// <summary>
        /// This is safely property for retrieving user reference.  No public access to navigation property User shall be done.
        /// </summary>
        public User userX
        {
            get
            {
                if (_userX == null)
                    _userX = User;  //navigation property

                return _userX;
            }

            set
            {
                _userX = value;
                UserID = (_userX == null) ? "" : _userX.UserID;
            }
        }

        private string _currencySign = null;
        /// <summary>
        /// currency Sign
        /// </summary>
        public string currencySign
        {
            get
            {

                if (string.IsNullOrEmpty(_currencySign))
                    _currencySign = CurrencyHelper.getCurrencybyID(Currency).CurrencySign;
                return _currencySign;
            }
            set { _currencySign = value; }
        }

        private Currency _localCurrencyX;
        public Currency localCurrencyX  
        {
            get 
            {
                if (_localCurrencyX != null && _localCurrencyX.CurrencyID != LocalCurrency)
                    _localCurrencyX = null;
                if (_localCurrencyX == null)
                {
                    if (!string.IsNullOrEmpty(LocalCurrency))
                        _localCurrencyX = CurrencyHelper.getCurrencybyID(LocalCurrency);
                    else
                        _localCurrencyX = new Currency();                    
                }
                return _localCurrencyX; 
            }
        }


        //this property is to indicate whether cart content has been modified since last time saved
        public Boolean isModified() { return _modified; }
        public void ResetModified() { _modified = false; }

        /// <summary>
        /// This method is to validate shopping cart items to remove phased out items from the shopping cart.  Usually this method
        /// will only be called by User class before it return shopping cart back to its caller.
        /// </summary>
        public void initValidation()
        {
            if (CartItems != null && validated == false)
            {
                try
                {
                    foreach (CartItem item in this.cartItemsX.ToList())
                    {
                        if (item.partX == null || item.partX.notAvailable)
                        {
                            cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                            {
                                SProductId = item.SProductID,
                                Name = item.ProductName
                            });
                            this.CartItems.Remove(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Warn("Exception at removing phased out cart item", "", "", "", ex);
                }
            }

            validated = true;
        }

        /// <summary>
        /// This method is to add shopping item into Cart.  It's a simple method, so no advanced parameters
        /// will be taken.  For setting special delivery date, the caller need to explicitly set them up in
        /// return CartItem.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="orderQuantity"></param>
        /// <param name="unitPrice"> the parameter is optional and only need to be provided when the unit price is overwrited by sales.</param>
        /// <param name="customerNote">optional</param>
        /// <param name="autoOrder">optional</param>
        /// <param name="autoOrderQty">optional</param>
        /// <returns>
        ///     Success : New created cart item
        ///     Failure : null
        /// </returns>
        public CartItem addItem(Part product, int orderQuantity, BTOSystem btos = null, Decimal unitPrice = 0,
                                String customerNote = null, Boolean autoOrder = false, int autoOrderQty = 0, Boolean mergeWithExisting = true, Part warranty = null, int dependencyItemNo = 0)
        {
            //valid CTOS item
            if (product is Product_Ctos && btos == null)
            {
                //use default configuration
                btos = ((Product_Ctos)product).getDefaultBTOS();
            }

            //如果不符合最小下单将跳出
            if (!fitMOQ(product, orderQuantity, btos))
                return null;


            //try to merge with existing cart item if the new ordering product is the same
            if (!(product is Product_Ctos) && !(product is Product_Bundle) && mergeWithExisting == true)    //if new ording item is not CTOS system
            {
                //check existing item
                CartItem match = null;

                List<CartItem> matchList = (from item in CartItems
                                            where item.SProductID.Equals(product.SProductID)
                                            select item).ToList();

                if (matchList != null && matchList.Count > 0)
                {
                    if (warranty == null)
                        match = matchList.FirstOrDefault(c => c.warrantyItemX == null);
                    else //有Warranty
                        match = matchList.FirstOrDefault(c => c.warrantyItemX != null && c.warrantyItemX.partX.SProductID.Equals(warranty.SProductID));
                }
                if (match != null)  //find match
                {
                    match.addOrderQuantity(orderQuantity, product);
                    updateTotal();
                    return match;  //更新数据后跳出
                }
            }

            //Prepare instance of new CartItem
            CartItem cartItem = new CartItem(nextAvailableId, product, this, orderQuantity, btos, unitPrice, customerNote, autoOrder, autoOrderQty);
            if (dependencyItemNo > 0)
            {
                cartItem.RelatedType = 1000;
                cartItem.RelatedItem = dependencyItemNo;
            }
            //add cart item in transaction basis
            //add CartItem to Cart item list
            CartItems.Add(cartItem);
            if (warranty != null)
                cartItem.addWarranty(warranty);
            _modified = true;
            updateTotal();
            return cartItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        /// <param name="btos"></param>
        /// <returns></returns>
        public bool fitMOQ(Part part, int qty, BTOSystem btos = null)
        {
            bool isFitMOQ = true;
            //不满足最小下单数将跳出
            if (part is Product)
            {
                Product product = (Product)part;
                switch (product.productType)
                {
                    case Product.PRODUCTTYPE.CTOS:
                        if (btos == null)
                            btos = ((Product_Ctos)product).getDefaultBTOS();
                        if (product.SProductID.StartsWith("SBC-BTO") && btos != null)
                        {
                            foreach (var c in btos.parts)
                            {
                                if (!compareOrderMOQ(c.Key, qty))
                                    isFitMOQ = false;
                            }
                        }
                        break;
                    case Product.PRODUCTTYPE.BUNDLE:
                        Product_Bundle bundle = product as Product_Bundle;
                        foreach (var b in bundle.ProductBundleItems)
                        {
                            if (!compareOrderMOQ(b.part, qty))
                                isFitMOQ = false;
                        }
                        break;
                    case Product.PRODUCTTYPE.STANDARD:
                    default:
                        isFitMOQ = compareOrderMOQ(product, qty);
                        break;
                }
            }
            else   //regular part
            {
                isFitMOQ = compareOrderMOQ(part, qty);
            }
            return isFitMOQ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public bool compareOrderMOQ(Part part, int qty)
        {
            if (part != null && part.MininumnOrderQty != null && part.MininumnOrderQty.HasValue && part.MininumnOrderQty.Value > qty)
            {
                if (this.error_message == null)
                    this.error_message = new List<ErrorMessage>();
                this.error_message.Add(new ErrorMessage("MOQ_restriction", part.SProductID));
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method will add new cart item regardlessly if same part exists in other item or not.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="orderQuantity"></param>
        /// <returns></returns>
        public CartItem addSeparatedItem(Part product, int orderQuantity)
        {
            return addItem(product, orderQuantity, null, 0, null, false, 0, false);
        }

        /// <summary>
        /// This function will return matched cart item
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns>
        ///     Found : matched item
        ///     not found : null
        /// </returns>
        public CartItem getItem(int itemNo)
        {
            CartItem match = null;

            foreach (CartItem item in CartItems)
            {
                //find matched item by item no
                if (item.ItemNo == itemNo)
                {
                    match = item;
                    break;
                }
            }
            return match;
        }

        public CartItem getItem(string BTOconfigID)
        {
            CartItem match = null;

            foreach (CartItem item in CartItems)
            {
                //find matched item by item no
                if (item.BTOConfigID == BTOconfigID)
                {
                    match = item;
                    break;
                }
            }
            return match;
        }

        /// <summary>
        /// This method is to remove item  from cart
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public Boolean removeItem(int itemNo)
        {
            Boolean result = false;

            CartItem item = getItem(itemNo);
            if (item != null)
                result = removeItem(item);

            return result;
        }

        public Boolean removeItem(CartItem item)
        {
            if (item == null)
                return true;
            if (item.warrantyItemX != null)
                CartItems.Remove(item.warrantyItemX);
            Boolean result = CartItems.Remove(item);
            if (result)
                _modified = true;

            //if transaction completes successfully, update total amount
            if (result)
                updateTotal();

            return result;
        }

        /// <summary>
        /// This method is to remove cart item that is converted to order or quotation
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void removeConvertedItem(CartItem rmItem)
        {
            if (rmItem == null)
                return;

            //find matched cart item
            var item = (from ci in CartItems
                        where ci.SProductID.Equals(rmItem.SProductID) && ci.Qty == rmItem.Qty
                        select ci).FirstOrDefault();

            if (item != null)
            {
                if (item.warrantyItemX != null)
                {
                    CartItems.Remove(item.warrantyItemX);
                }
                CartItems.Remove(item);
            }
        }

        /// <summary>
        /// This method is to empty cart
        /// </summary>
        /// <returns></returns>
        public Boolean removeAllItem()
        {
            CartItems.Clear();

            //update DB in real time
            //if (forTestOnly == false)
                //save();

            _modified = true;

            //if transaction completes successfully, update total amount
            updateTotal();

            return true;
        }

        /// <summary>
        /// Update cart item -- this method will update DB record in real time.  Before calling this item, please make sure
        /// the item has been added to cart earlier.  Otherwise, the method will return failure.
        /// </summary>
        /// <param name="item">Updating item -- this item shall have been added to cart earlier</param>
        /// <returns>
        ///     true -- successful update
        ///     false -- failure updating DB or the item is not found in cart
        /// </returns>
        public Boolean updateItem(CartItem item)
        {
            if (item == null)
                return false;

            Boolean updated = true;
            if (CartItems.Contains(item))
            {
                item.reflectBTOSChange();

                //if (forTestOnly == false)
                    //;//save();
            }
            else
                updated = false;

            updateTotal();

            return updated;
        }

        public void copyTo(Cart cart, int minisiteId = 0)
        {
            cart.removeAllItem();

            //copy the rest of cart properties
            cart.CreatedBy = CreatedBy;
            cart.CreatedDate = DateTime.Now;
            cart.Currency = Currency;
            cart.LastUpdateBy = LastUpdateBy;
            cart.LastUpdateDate = cart.CreatedDate;
            cart.ProtectionStatus = ProtectionStatus;
            cart.Status = Status;
            cart.TotalAmount = TotalAmount;
            cart.userX = userX;
            cart.storeX = storeX;
            //cart.helper = helper;
            cart.setBillTo(this.BillToContact);
            cart.setShipTo(this.ShipToContact);
            cart.setSoldTo(this.SoldToContact);
            cart.LocalCurExchangeRate = this.LocalCurExchangeRate;
            cart.LocalCurrency = this.LocalCurrency;
            cart.ProtectionStatus = this.ProtectionStatus;
            cart.SalesDivision = this.SalesDivision;
            if (minisiteId > 0)
                cart.MiniSiteID = minisiteId;
            else if(MiniSiteID.HasValue && MiniSiteID.Value > 0)
                cart.MiniSiteID = MiniSiteID;

            //packing list shall also be cloned too.  Due to time constrain, this feature will be deferred to 1st release ********

            //copy cart items
            appendTo(cart);
        }

        
        public void appendTo(Cart cart)
        {
            //copy cart items
            foreach (CartItem item in cartItemsX.ToList())
            {
                CartItem newItem = new CartItem();

                newItem.cartX = cart;
                item.copyTo(newItem);
                newItem.OldItemNo = newItem.ItemNo;
                newItem.ItemNo = cart.nextAvailableId;
                if (item.warrantyItemX != null)
                {
                    CartItem ewItem = new CartItem();
                    ewItem.cartX = cart;
                    item.warrantyItemX.copyTo(ewItem);
                    ewItem.ItemNo = cart.nextAvailableId;
                    newItem.WarrantyItem = ewItem.ItemNo;
                    cart.CartItems.Add(newItem);
                    cart.CartItems.Add(ewItem);
                }
                else
                    cart.CartItems.Add(newItem);
            }

            cart.updateTotal();
        }

        /// <summary>
        /// This method composes a new Cart instance with cart items specified in the input array
        /// </summary>
        /// <param name="actingUser"></param>
        /// <param name="itemNoList"></param>
        /// <returns></returns>
        public Order checkOut(User actingUser, int[] itemNoList)
        {
            Cart newCart = new Cart(storeX, actingUser);
            CartItem item = null;
            CartItem newItem = null;

            foreach (int itemNo in itemNoList)
            {
                item = getItem(itemNo);
                if (item != null)
                {
                    newItem = new CartItem();
                    newItem.cartX = newCart;
                    item.copyTo(newItem);
                    newItem.ItemNo = newCart.nextAvailableId;
                    newCart.CartItems.Add(newItem);
                }
            }

            //copy the rest of cart properties
            newCart.CreatedBy = CreatedBy;
            newCart.LastUpdateBy = LastUpdateBy;
            newCart.CreatedDate = DateTime.Now;
            newCart.LastUpdateDate = newCart.CreatedDate;
            newCart.Currency = Currency;
            newCart.ProtectionStatus = ProtectionStatus;
            newCart.Status = Status;
            newCart.userX = userX;
            newCart.storeX = storeX;

            newCart.updateTotal();

            //create a new order
            Order newOrder = new Order(storeX, actingUser, newCart);

            return newOrder;
        }

        /// <summary>
        /// This method is to check out entire cart and use it to create a new Order
        /// </summary>
        /// <param name="actingUser"></param>
        /// <returns></returns>
        public Order checkOut(User actingUser, int minisiteId = 0)
        {
            Cart newCart = new Cart(storeX, actingUser);
            copyTo(newCart, minisiteId);
            newCart.userX = actingUser;

            return new Order(storeX, actingUser, newCart);
        }

        /// <summary>
        /// This method is to remove checked out cart items specified in input Cart instance from current shopping cart.
        /// Usually this method is called after user successfully check out complete or partial cart items and complete 
        /// the payment process.
        /// </summary>
        /// <param name="cart"></param>
        public void releaseToOrder(User actingUser, Order releasingOrder)
        {
            foreach (CartItem item in releasingOrder.cartX.CartItems)
            {
                //removeItem(item.ItemNo);
                removeConvertedItem(item);
            }

            _modified = true;
            updateTotal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Boolean setBillTo(Object contact)
        {
            Boolean success = false;
            try
            {
                CartContact cartContact = new CartContact();
                //copy contact information
                if (contact is Contact) //contact type
                    cartContact.copy((Contact)contact);
                else if (contact is VSAPCompany)
                {
                    cartContact.copy((VSAPCompany)contact);
                    cartContact.UserID = this.UserID;   //VSAPCompany doesn't has userID property
                }
                else if (contact is CartContact)
                    //cartContact = (CartContact)contact;
                    cartContact.copy((CartContact)contact);
                else
                    cartContact = null;

                if (cartContact != null)
                {
                    if (BillToContact == null)
                    {
                        new CartContactHelper().save(cartContact); //after save, the ID generated
                        BilltoID = cartContact.ContactID;
                        BillToContact = cartContact;
                    }
                    else
                        BillToContact.copy(cartContact);
                }
                else
                    success = false;
            }
            catch (Exception)
            {
                //log error here
                success = false;
            }

            return success;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Boolean setShipTo(Object contact)
        {
            Boolean success = false;
            try
            {
                CartContact cartContact = new CartContact();
                //copy contact information
                if (contact is Contact) //contact type
                    cartContact.copy((Contact)contact);
                else if (contact is VSAPCompany)
                {
                    cartContact.copy((VSAPCompany)contact);
                    cartContact.UserID = this.UserID;   //VSAPCompany doesn't has userID property
                }
                else if (contact is CartContact)
                    //cartContact = (CartContact)contact;
                    cartContact.copy((CartContact)contact);
                else
                    cartContact = null;

                if (cartContact != null)
                {
                    if (ShipToContact == null)
                    {
                        new CartContactHelper().save(cartContact); //after save, the ID generated
                        ShiptoID = cartContact.ContactID;
                        ShipToContact = cartContact;
                    }
                    else
                        ShipToContact.copy(cartContact);
                }
                else
                    success = false;
            }
            catch (Exception)
            {
                //log error here
                success = false;
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Boolean setSoldTo(Object contact)
        {
            Boolean success = false;
            try
            {
                CartContact cartContact = new CartContact();
                //copy contact information
                if (contact is Contact) //contact type
                    cartContact.copy((Contact)contact);
                else if (contact is VSAPCompany)
                {
                    cartContact.copy((VSAPCompany)contact);
                    cartContact.UserID = this.UserID;   //VSAPCompany doesn't has userID property
                }
                else if (contact is CartContact)
                    //cartContact = (CartContact)contact;
                    cartContact.copy((CartContact)contact);
                else
                    cartContact = null;

                if (cartContact != null)
                {
                    if (SoldToContact == null)
                    {
                        new CartContactHelper().save(cartContact); //after save, the ID generated
                        SoldtoID = cartContact.ContactID;
                        SoldToContact = cartContact;
                    }
                    else
                        SoldToContact.copy(cartContact);
                }
                else
                    success = false;
            }
            catch (Exception)
            {
                //log error here
                success = false;
            }

            return success;
        }


        /// <summary>
        /// This property indicates which contact group shall be used for current cart
        /// </summary>
        public Store.BusinessGroup businessGroup
        {
            get
            {
                Dictionary<Store.BusinessGroup, int> group = new Dictionary<Store.BusinessGroup, int>();

                Store.BusinessGroup itemgroup = POCOS.Store.BusinessGroup.eP;
                foreach (CartItem item in CartItems)
                {
                    if (item.partX is Product_Ctos && item.StoreID != "AEU") // AEU CTOS will use standard produt Business Group
                    {
                        //check to see if the cart item is straight from DB.  When it's straight from DB, the storeID in BTOSystem
                        //will be empty
                        //if (item.BTOSystem != null && String.IsNullOrEmpty(item.BTOSystem.storeID))
                        //    item.BTOSystem.storeID = item.StoreID;
                        //IDictionary<Part, int> parts = item.BTOSystem.parts;
                        IDictionary<Part, int> parts = item.btosX.parts;
                        foreach (Part part in parts.Keys)
                        {
                            //only count Advantech main stream product
                            if (part != null && part.isMainStream())
                            {
                                if (part is POCOS.Product)
                                    itemgroup = ((POCOS.Product)part).businessGroup;
                                else
                                {
                                    if (part.isEAProduct())
                                        itemgroup = POCOS.Store.BusinessGroup.eA;
                                    else
                                        itemgroup = POCOS.Store.BusinessGroup.eP;
                                }
                            }
                            if (group.ContainsKey(itemgroup))
                            {
                                group[itemgroup] += parts[part];
                            }
                            else
                                group.Add(itemgroup, parts[part]);

                        }
                    }
                    else  //standard product or part
                    {
                        //only count Advantech main stream product
                        if (item.partX != null && item.partX.isMainStream())
                        {
                            if (item.partX is POCOS.Product)
                                itemgroup = ((POCOS.Product)item.partX).businessGroup;
                            else
                            {
                                if (item.partX.isEAProduct())
                                    itemgroup = POCOS.Store.BusinessGroup.eA;
                                else
                                    itemgroup = POCOS.Store.BusinessGroup.eP;
                            }

                            if (group.ContainsKey(itemgroup))
                            {
                                group[itemgroup] += item.Qty;
                            }
                            else
                                group.Add(itemgroup, item.Qty);
                        }
                    }
                }

                if (group.Any())
                    return group.OrderByDescending(x => x.Value).First().Key;
                else
                    return Store.BusinessGroup.eP;
            }
        }

        /// <summary>
        /// This method is introduced for performance purpose. When it's invoked, it'll prefetch ATP price of all products in current
        /// purchasing cart.
        /// </summary>
        public void prefetchCartItemATP()
        {
            //acquire ATP value of relatedProducts

            if (CartItems != null)
            {
                //Though its same product, but the part instance may be different due to data acquisition time and mechanism.
                //Therefore to make sure no duplicate product nodes, two dictionaries would need to be applied here.
                //The content of these two dictionaries shall be consistent
                Dictionary<Part, int> fetchingList = new Dictionary<Part, int>();
                Dictionary<String, Part> checkList = new Dictionary<String, Part>();

                //gathering product list in current cart
                foreach (CartItem item in CartItems)
                {
                    try
                    {
                        if (item.partX is Product_Ctos)
                        {
                            //check to see if the cart item is straight from DB.  When it's straight from DB, the storeID in BTOSystem
                            //will be empty
                            //if (item.BTOSystem != null && String.IsNullOrEmpty(item.BTOSystem.storeID))
                            //    item.BTOSystem.storeID = item.StoreID;
                            //IDictionary<Part, int> parts = item.BTOSystem.parts;
                            IDictionary<Part, int> parts = item.btosX.parts;
                            foreach (Part part in parts.Keys)
                            {
                                //if (! fetchingList.ContainsKey(part))
                                if (!checkList.ContainsKey(part.SProductID))
                                {
                                    fetchingList.Add(part, parts[part] * item.Qty);
                                    checkList.Add(part.SProductID, part);
                                }
                                else
                                {
                                    Part exist = checkList[part.SProductID];
                                    fetchingList[exist] = fetchingList[exist] + (parts[part] * item.Qty);
                                }
                            }
                        }
                        else  //standard product or part
                        {
                            //only count Advantech main stream product
                            if (item.partX != null && item.partX.isMainStream())
                            {
                                Part part = item.partX;

                                //if (!fetchingList.ContainsKey(part))
                                if (!checkList.ContainsKey(part.SProductID))
                                {
                                    fetchingList.Add(part, item.Qty);
                                    checkList.Add(part.SProductID, part);
                                }
                                else
                                {
                                    Part exist = checkList[part.SProductID];
                                    fetchingList[exist] = fetchingList[exist] + item.Qty;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Error("Exception thrown at prefetchATP", "", "", "", ex);
                    }
                }

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                PartHelper helper = new PartHelper();
                helper.setATPs(storeX, fetchingList);
            }
        }


        private void copyContactInfo(Contact contact, CartContact cartContact)
        {
            cartContact.Address1 = contact.Address1;
            cartContact.Address2 = contact.Address2;
            cartContact.AddressID = contact.AddressID;
            cartContact.AttCompanyName = contact.AttCompanyName;
            //cartContact.Attention = contact.Attention;
            cartContact.FirstName = contact.FirstName;
            cartContact.LastName = contact.LastName;
            cartContact.City = contact.City;
            cartContact.ContactType = contact.ContactType;
            cartContact.Country = contact.Country;
            cartContact.County = contact.County;
            cartContact.CreatedBy = contact.CreatedBy;
            cartContact.CreatedDate = DateTime.Now;
            cartContact.FaxNo = contact.FaxNo;
            cartContact.LastUpdateTime = DateTime.Now;
            cartContact.Mobile = contact.Mobile;
            cartContact.State = contact.State;
            cartContact.TelExt = contact.TelExt;
            cartContact.TelNo = contact.TelNo;
            cartContact.UpdatedBy = contact.UpdatedBy;
            cartContact.UserID = contact.UserID;
            cartContact.ZipCode = contact.ZipCode;
            cartContact.countryCodeX = contact.countryCodeX;
        }

        /// <summary>
        /// This method is to copy availabe information from SAPCompany to regular user contact entry
        /// </summary>
        /// <param name="sapContact"></param>
        /// <param name="contact"></param>
        private void copyContactInfo(VSAPCompany sapContact, Contact contact)
        {
            contact.Address1 = sapContact.Address;
            contact.AddressID = sapContact.CompanyID;
            //contact.AttCompanyName = sapContact.CompanyName;
            contact.AttCompanyName = sapContact.companyNameX;
            contact.City = sapContact.City;
            contact.countryX = sapContact.Country;
            contact.FaxNo = sapContact.FaxNo;
            contact.TelNo = sapContact.TelNo;
            contact.ZipCode = sapContact.ZipCode;
            contact.State = sapContact.stateX;
        }

        /// <summary>
        /// This method should be invoked whenever item is added, updated and removed.
        /// Promotion calculation will not be handled here.  The promotion calculation will be trigger by Presentation layer logic.
        /// Promotion recalculation will be invoked in
        /// 1. Cart Detail page -- PageLoad and Update button click (cart.aspx.cs)
        /// 2. Quotation Cart Detail Page -- PageLoad and Update button click (quotes.aspx.cs)
        /// 3. Order Cart Detail Page -- PageLoad and Promotion Code button click (confirm.aspx.cs)
        /// </summary>
        public void updateTotal()
        {
            TotalAmount = 0m;

            //iterate through all items in this cart
            foreach (CartItem item in CartItems)
            {
                TotalAmount += item.AdjustedPrice;
                //CartTotalDiscount += item.discountAmount;   //**** this property is not defined in entity and DB yet.
            }

            LastUpdateDate = DateTime.Now;
        }

        /// <summary>
        /// In Cart level, only Promotions without coupon code will be applied.
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        public decimal applyPromotionCode(String promotionCode)
        {
            DiscountDetail result = this.calculatePromotionTotalDiscount(this.userX, this, 0, 0, string.Empty);
            if (result.codeStatus == CampaignManager.PromotionCodeStatus.Valid)
            {
                return result.discountAmount;
            }
            else
                return 0;
        }

        public Cart getFreeGroupShipmentEligibleCartItems()
        {
            return this.getFreeGroupShipmentEligibleCartItems(this.userX, this);
        }

        public void setFreeGroupShipPromotionStrategy()
        {
            Cart FreeGroupShipmencart = this.getFreeGroupShipmentEligibleCartItems(this.userX, this);
            if (FreeGroupShipmencart != null)
            {
                foreach (CartItem item in FreeGroupShipmencart.cartItemsX)
                {
                    CartItem cartitem = this.getItem(item.ItemNo);
                    if (cartitem != null)
                    {
                        cartitem.PromotionStrategy = item.PromotionStrategy;
                        cartitem.PromotionMessage = item.PromotionMessage;
                    }
                }
            }
        }

        public void clearFreeGroupShipmentPromotionStrategy()
        {

            Cart FreeGroupShipmencart = this.getFreeGroupShipmentEligibleCartItems(this.userX, this);
            if (FreeGroupShipmencart != null)
            {
                foreach (CartItem item in FreeGroupShipmencart.cartItemsX)
                {
                    CartItem cartitem = this.getItem(item.ItemNo);
                    if (cartitem != null)
                    {
                        cartitem.PromotionStrategy = null;
                        cartitem.PromotionMessage = null;
                        cartitem.DiscountAmount = null;
                    }
                }
            }
        }

        internal int nextAvailableId
        {
            get
            {
                if (_nextId == 0)    //never initialized
                {
                    //sorting existing items to get the highest item no
                    var sortedItems = from item in CartItems
                                      orderby item.ItemNo descending
                                      select item;
                    if (sortedItems != null && sortedItems.Count() > 0)
                        _nextId = ((CartItem)sortedItems.First()).ItemNo;
                }

                return ++_nextId;
            }
        }

        /// <summary>
        /// this attribute is to compute weight
        /// </summary>
        public Decimal weight
        {
            get
            {
                Decimal _weight = 0;
                if (cartItemsX != null && cartItemsX.Count > 0)
                {
                    foreach (var c in cartItemsX)
                        _weight += c.weight;
                }
                return _weight;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Part> _parts = null;
        public List<Part> Parts
        {
            get
            {
                if (_parts == null)
                    prefetchCartPartList();
                return _parts;
            }
            set { _parts = value; }
        }

        /// <summary>
        /// 将Cart中使用到的Part加入到缓存中。
        /// </summary>
        public void prefetchCartPartList()
        {
            List<string> list = new List<string>();
            if (this != null && this.CartItems.Any())
            {
                foreach (CartItem item in this.CartItems)
                {
                    if (item.type == Product.PRODUCTTYPE.CTOS)
                    {
                        if (item.BTOSystem.BTOSConfigsWithoutNoneItems != null && item.BTOSystem.BTOSConfigsWithoutNoneItems.Any())
                        {
                            foreach (var detail in item.BTOSystem.BTOSConfigsWithoutNoneItems)
                                list.AddRange(detail.BTOSConfigDetails.Select(c => c.SProductID));
                        }
                    }
                    else
                        list.Add(item.SProductID);
                }
            }
            PartHelper ph = new PartHelper();
            _parts = ph.prefetchPartList(this.StoreID, list);
        }

        /// <summary>
        /// 验证产品购物车是否低于成本价格
        /// true 将视为无效的订单
        /// </summary>
        /// <returns></returns>
        public bool isBelowCost()
        {
            return getCostPrice() > TotalAmount;
        }

        public bool isPStoreOrder()
        {
            if (this.cartItemsX.Any())
            {
                return this.cartItemsX.Count(x =>   x.partX.isPStoreProduct()) == this.cartItemsX.Count();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取Cart中产品Cost的总和
        /// </summary>
        /// <returns></returns>
        public decimal getCostPrice()
        {
            decimal TotalCost = 0m;
            //updateTotal();
            foreach (CartItem item in CartItems)
            {
                switch (item.type)
                {
                    case Product.PRODUCTTYPE.STANDARD:
                        TotalCost += item.partX.CostX * item.Qty;
                        break;
                    case Product.PRODUCTTYPE.BUNDLE:
                        TotalCost += ((Product_Bundle)item.partX).CostX * item.Qty;
                        break;
                    case Product.PRODUCTTYPE.CTOS:
                        TotalCost += item.BTOSystem.getCost() * item.Qty;
                        break;
                    default:
                        break;
                }
            }
            return TotalCost;
        }

        public List<ICartItemChangedMessage> cartItemChangedMessage = new List<ICartItemChangedMessage>();
        public void refresh(bool forceRefresh=false)
        {
            if (forceRefresh ||(LastUpdateDate.HasValue && LastUpdateDate.Value.AddHours(24) < DateTime.Now))
            {
                bool isUpdate = false;
                Price priceItem = new Price();
 
                foreach (CartItem item in cartItemsX)
                {
                    if (item.btosX != null)
                    {
                        if (item.partX != null && item.partX is POCOS.Product_Ctos)
                        {
                            POCOS.Product_Ctos itemCtos = (POCOS.Product_Ctos)item.partX;
                            if (itemCtos.isOrderable())
                            {
                                bool? refreshresult = item.btosX.refresh();

                                if (refreshresult.HasValue)
                                {
                                    if (refreshresult.GetValueOrDefault())
                                    {
                                        Decimal ctosRoundingUnit = this.Store != null ? this.storeX.ctosRoundingUnit : 1m;
                                        decimal oldUnitPrice = item.UnitPrice;//旧的cartItem价钱
                                        itemCtos.recalculateCTOSDefaultPrice(ctosRoundingUnit);
                                        itemCtos.updateBTOSPrice(item.btosX);
                                        decimal newUnitPrice = item.btosX.Price;//新的cartItem价钱

                                        if (oldUnitPrice != newUnitPrice)
                                        {
                                            cartItemChangedMessage.Add(new CartItemChangedMessage_PriceChanged() {
                                             SProductId=item.SProductID,
                                             Name=item.ProductName,
                                             NewPrice =newUnitPrice,
                                              OldPrice=oldUnitPrice
                                            });
                                            isUpdate = true;
                                        }
                                    }
                                    else
                                    {
                                        cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                                        {
                                            SProductId = item.SProductID,
                                            Name = item.ProductName
                                        });
                                        CartItems.Remove(item);
                                        isUpdate = true;
                                    }
                                }
                            }
                            else// not orderable
                            {
                                cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                                {
                                    SProductId = item.SProductID,
                                    Name = item.ProductName
                                });
                                CartItems.Remove(item);
                                isUpdate = true;
                            }

                        }
                        else// cannot get parts
                        {
                            cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                            {
                                SProductId = item.SProductID,
                                Name = item.ProductName
                            });
                            CartItems.Remove(item);
                            isUpdate = true;
                        }

                    }
                    else if (item.bundleX != null)
                    {
                        bool isunpublish = false; 
                        if (item.partX != null && item.partX is POCOS.Product_Bundle)
                        {
                            POCOS.Product_Bundle itemBundle = (POCOS.Product_Bundle)item.partX;
                            if (itemBundle.isOrderable() )
                            {
                                bool? refreshresult = item.bundleX.refresh();
                                if (refreshresult.HasValue)
                                {
                                    if (refreshresult.GetValueOrDefault())
                                    {
                                        cartItemChangedMessage.Add(new CartItemChangedMessage_PriceChanged()
                                        {
                                            SProductId = item.SProductID,
                                            Name = item.ProductName,
                                            NewPrice = priceItem.value,
                                            OldPrice = item.UnitPrice
                                        });
                                        item.bundleX.updatePrice(item.bundleX. adjustedPrice);
                                        item.updateUnitPrice(priceItem.value);
                                        isUpdate = true;
                                    }
                                    else
                                    {
                                        isunpublish = true;
                                    }
                                }
                            }
                            else
                                isunpublish = true;
                        }
                        else
                            isunpublish = true;

                        if (isunpublish)
                        {
                            cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                            {
                                SProductId = item.SProductID,
                                Name = item.ProductName
                            });
                            CartItems.Remove(item);
                            isUpdate = true;
                        }
                    }
                    else
                    {
                        if (item.partX != null && item.partX.isOrderable())
                        {
                            if (item.partX is POCOS.Product)
                                priceItem = ((POCOS.Product)item.partX).getListingPrice();
                            else if (item.partX is POCOS.PStoreProduct)
                                priceItem = ((POCOS.PStoreProduct)item.partX).getListingPrice();
                            else
                                priceItem = item.partX.getListingPrice();

                            if (priceItem.value > 0 && priceItem.value != item.UnitPrice)
                            {
                                cartItemChangedMessage.Add(new CartItemChangedMessage_PriceChanged()
                                {
                                    SProductId = item.SProductID,
                                    Name = item.ProductName,
                                    NewPrice = priceItem.value,
                                    OldPrice = item.UnitPrice
                                });
                                item.updateUnitPrice(priceItem.value);
                                isUpdate = true;
                            }
                            else if(priceItem.value <= 0)
                            {
                                cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                                {
                                    SProductId = item.SProductID,
                                    Name = item.ProductName
                                });
                                CartItems.Remove(item);
                                isUpdate = true;
                            }
                        }
                        else
                        {
                            cartItemChangedMessage.Add(new CartItemChangedMessage_Unavailable()
                            {
                                SProductId = item.SProductID,
                                Name = item.ProductName
                            });
                            CartItems.Remove(item);
                            isUpdate = true;
                        }
                    }
                }

                if (isUpdate)
                {
                    reconcile();

                    LastUpdateDate = DateTime.Now;
                    save();
                }
            }
        }


        public void fixCartCurrency(POCOS.Currency targetCurrency)
        {
            if (this.Currency != targetCurrency.CurrencyID)
            {
                var sourceCurrency = new POCOS.Currency(this.Currency);
                this.LocalCurExchangeRate = sourceCurrency.ToUSDRate.GetValueOrDefault() / targetCurrency.ToUSDRate.GetValueOrDefault();
                this.LocalCurrency = targetCurrency.CurrencyID;
            }
            else
            {
                if (this.LocalCurExchangeRate.HasValue)
                    this.LocalCurExchangeRate = null;
                this.LocalCurrency = null;
            }
        }


        #endregion

        #region Unit Test
        public Boolean forTestOnly
        {
            get;
            set;
        }

        public void print()
        {
            Console.WriteLine("\n\n---- Shopping list in cart : {0} -------", CartID, UserID);
            Console.WriteLine("   ItemNo      PriceAfterDisc     Qty    OrigPrice");
            foreach (CartItem item in CartItems)
                Console.WriteLine("   {0}     {1}     {2}     {3}", item.ItemNo, item.AdjustedPrice, item.Qty, item.UnitPrice);
            Console.WriteLine("");
            Console.WriteLine("\n   Total charge : {0}\n", TotalAmount);
            Console.WriteLine("---- End of shopping list -----------\n\n");
        }

        #endregion //Unit Test

        #region OM
        //The following methods are for FollowUp used in OM
        protected override String getInstanceID() { return this.CartID; }
        protected override String getInstanceStoreID() { return this.StoreID; }
        protected override String getInstanceOwner() { return this.UserID; }
        protected override TrackingLog.TrackType getTrackType() { return TrackingLog.TrackType.CART; }

        /// <summary>
        /// This method is an expensive method. When it's invoked, it will trigger a process to complete reconcile and recalculate
        /// entire Cart price.  This method shall only be used in OM.  Promotion recalculation will not be include in this process.
        /// If there is promotion involved, a seperated function call need to be made.
        /// </summary>
        public void reconcile()
        {
            //reconcile cart item changes
            foreach (CartItem item in CartItems)
                item.reconcile();

            this.updateTotal();
        }

        #endregion OM
    }

    public interface ICartItemChangedMessage{
       
    }
    public class CartItemChangedMessage_Unavailable : ICartItemChangedMessage
    {
        public string SProductId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    
    }
    public class CartItemChangedMessage_PriceChanged : ICartItemChangedMessage
    {
        public string SProductId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
    }
}