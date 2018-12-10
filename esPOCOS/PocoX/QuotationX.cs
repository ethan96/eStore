using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// Quotation will have two stages, preparing and confirmed.   
    /// </summary>
    public partial class Quotation : PromotionCodeEnabled
    {
        private Store _storeX = null;
        private User _userX = null;
        //Revisible period is a indicator of how long this quotation shall remain revisible.
        //The measurable unit is of quotation variable period is in hour.
        //Deafult value is 1 day, 24 hours. This variable is to
        private int _modifiablePeriod = 24;   //24 hours
        private int _validTime = 14; //2 weeks.  the measurement unit is in day.  
        
        /// <summary>
        /// Open :  can be modified
        /// Unfinished :    quotation which doesn't be finalized as confirmed (runtime only)
        /// Confirmed  :    quotation which is confirmed and still not expired
        /// Expired    :    quotation which is confirmed, but exceeds its expiration date (runtime only)
        /// NeedTaxAndFreightReview : quotation is in confirmed state, but either tax or freight might be undeterminable at this time
        /// NeedTaxIDReview : quotation is in confirmed state, but the Tax ID user input is not valid
        /// NeedOverallReview : quotation is in confirmed state, but the quotation has both Tax&Freight issue and invalid Tax ID issue
        /// </summary>
        public enum QStatus { Open, Confirmed, Expired, NeedTaxAndFreightReview, NeedTaxIDReview, NeedGeneralReview, Unfinished, NeedFreightReview, ConfirmedbutNeedTaxIDReview, ConfirmedbutNeedFreightReview };

        public enum QuoteSource { eStore, eQuotation };
        //quotation 成功状态
        public static string[] SuccessfullyStatus = new string[] { "Confirmed", "ConfirmdbutNeedTaxIDReview", "ConfirmedbutNeedFreightReview" };

        //For quotation converted to orders
        public string ordernoX;

        //default constructor shall only be used by Entity Framework for instantiation
        public Quotation()
        {
            //get new quotation ID from Quotation helper **********
            //temporary solution
            QuotationNumber = "QUOTE." + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

            QuoteDate = DateTime.Now;
            QuoteExpiredDate = DateTime.Now.AddDays(_validTime);

            //set initial status as "Open"
            statusX = QStatus.Open;

            Source = QuoteSource.eStore;

            Version = "1";
        }

        /// <summary>
        /// Default constructor for eStore application
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        public Quotation(Store store, User user)
            : this()
        {
            storeX = store;
            userX = user;
            Source = QuoteSource.eStore;

            //overwrite default quotation number with store particualr no
            if (isPStoreOrder())
            {
                QuotationNumber = storeX.getNextQuotationNumber("P");
            }
            else
            {
                QuotationNumber = storeX.nextQuotationNumber;
            }
        }

        /// <summary>
        /// This constructor is to construction quotation from user shopping cart
        /// </summary>
        /// <param name="cart"></param>
        public Quotation(Cart cart) : this(cart.storeX, cart.userX)
        {
            /*
            storeX = cart.storeX;
            userX = cart.userX;

            //overwrite default quotation number with store particualr no
            QuotationNumber = storeX.nextQuotationNumber;
             */

            //perform deep cart copy
            cart.copyTo(this.cartX);

            //in cart clone process, cart helper will be copied too.  Since this is new cart, no cart helper shall be assigned
            this.cartX.helper = null;   
        }

        /// <summary>
        /// Derived Property to be external property of Cart.  Cart property from entity framework shall be private and hidden from
        /// external direct access.
        /// </summary>
        public Cart cartX
        {
            get
            {
                if (Cart == null)
                    //Cart = new Cart(_store, _userX);
                    Cart = new Cart(storeX, userX);

                return Cart;
            }

            set { Cart = value; }
        }

        /// <summary>
        /// eStore application calls this method whenever user confirms and finalizes current quotation.  Whenever this method
        /// is called, current quotation will become final and will no longer allow revising.
        /// </summary>
        /// <returns></returns>
        public int confirm(User user)
        {
            int result = 0;

            if (isModifiable())
            {
                ConfirmedBy = user.UserID;
                ConfirmedDate = DateTime.Now;
                DueDate = ConfirmedDate.GetValueOrDefault().AddDays(2); //this property is reserved and not used
                RequiredDate = ConfirmedDate.GetValueOrDefault().AddDays(2); //this property is reserved and not used
                EarlyShipFlag = false;
                if (this.needVATReview())
                    statusX = QStatus.ConfirmedbutNeedTaxIDReview;
                else if (this.needFreightReview())
                    statusX = QStatus.ConfirmedbutNeedFreightReview;
                else
                    statusX = QStatus.Confirmed;
                //save promotion log, if checkout to order, then clone order type logs
                if (PromoteCode != null)
                {
                    try
                    {

                        foreach (PromotionAppliedLog log in this.promotionAppliedLogs)
                            log.save();

                    }
                    catch (Exception ex)
                    {
                        eStore.Utilities.eStoreLoger.Error("exception at save promotion applied record", "", "", "", ex);
                    }
                }  
            }
            else
                result = -1;

            return result;
        }

        /// <summary>
        /// This method return an indicator whether current quotation is modifiable or not.  By default a quotation will remain
        /// modifiable for 24 hours from the moment it's created.  If user doesn't confirm this quotation before its modifiable time
        /// is expired, the quotation will become "abandomed" quotation and will no longer available and visible to end user.  
        /// But eStore owners can still query this abandomed quotations at eStore OM interface and make follow-up call to user.
        /// </summary>
        /// <returns></returns>
        public Boolean isModifiable()
        {
            if (statusX == QStatus.Open || statusX == QStatus.NeedTaxIDReview || statusX == QStatus.NeedFreightReview)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method is to create a clone instance of itself. The main purpose of this method is to provide quotation
        /// revision feature.
        /// </summary>
        /// <returns></returns>
        public Quotation clone()
        {
            Quotation newQuote = new Quotation(storeX, userX);
            //Quotation newQuote = new Quotation(_store, _userX);
            try
            {
                newQuote.Version = (int.Parse(Version) + 1).ToString();
                newQuote.VATNumber = this.VATNumber;
            }
            catch (Exception)
            {
                //log exception here

                newQuote.QuotationNumber = String.Copy(QuotationNumber);
                newQuote.Version = "2";   //assume the orignal version is not integer type string
            }

            cartX.copyTo(newQuote.cartX);
            newQuote.helper = this.helper;

            return newQuote;
        }

        public Quotation revise()
        {
            Quotation newQuotation = clone();
            newQuotation.cartX.refresh(true);
            return newQuotation;
        }

        public Boolean transfer(User newOwner)
        {
            if (newOwner == null)
                return false;
            else
            {
                this.userX = newOwner;
                save();
                return true;
            }
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <param name="product"></param>
        /// <param name="orderQuantity"></param>
        /// <param name="btos"></param>
        /// <param name="unitPrice"></param>
        /// <param name="customerNote"></param>
        /// <param name="autoOrder"></param>
        /// <param name="autoOrderQty"></param>
        /// <returns></returns>
        public CartItem addItem(Part product, int orderQuantity, BTOSystem btos = null, Decimal unitPrice = 0,
                                String customerNote = null, Boolean autoOrder = false, int autoOrderQty = 0, Boolean mergeWithExisting = true)
        {
            CartItem cartItem = cartX.addItem(product, orderQuantity, btos, unitPrice, customerNote, autoOrder, autoOrderQty);

            //update promotion discount if there is any
            if (!String.IsNullOrEmpty(this.PromoteCode) && cartItem != null)   //has promotion code
                applyPromotionCode(userX, this.PromoteCode);

            return cartItem;
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
        /// This method is to merge the cart items from input shopping cart to current quotation
        /// </summary>
        /// <param name="cart"></param>
        public void mergeCartItems(Cart cart)
        {
            //perform deep cart copy
            cart.appendTo(this.cartX);
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public CartItem getItem(int itemNo)
        {
            return cartX.getItem(itemNo);
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <param name="itemNo"></param>
        /// <returns></returns>
        public Boolean removeItem(int itemNo)
        {
            Boolean status = cartX.removeItem(itemNo);

            //update promotion discount if there is any
            if (!String.IsNullOrEmpty(this.PromoteCode))   //has promotion code
                applyPromotionCode(userX, this.PromoteCode);

            return status;
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean removeItem(CartItem item)
        {
            Boolean status = cartX.removeItem(item);;

            //update promotion discount if there is any
            if (!String.IsNullOrEmpty(this.PromoteCode))   //has promotion code
                applyPromotionCode(userX, this.PromoteCode);

            return status;
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <returns></returns>
        public Boolean removeAllItem()
        {
            this.PromoteCode = null;
            this.TotalDiscount = 0m;

            return cartX.removeAllItem();
        }

        /// <summary>
        /// This is a delegating method to the same method of Cart
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean updateItem(CartItem item)
        {
            Boolean status = cartX.updateItem(item);

            //update promotion discount if there is any
            if (this.PromoteCode!=null)   //has promotion code
                applyPromotionCode(userX, this.PromoteCode);

            return status;
        }

        /// <summary>
        /// After calling this method, the caller should call quotation.delete() to remove this quotation since
        /// it's already released to Order.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Order releaseToOrder(User actingUser, Order order)
        {
            //this is just an empty function since there is no additional process need.

            return order;
        }

        public Order checkOut(Store store, User user)
        {
            Order order = new Order(store, user, this);
            //re-calculate promotion when check out.
            if (this.PromoteCode != null)
                order.applyPromotionCode(this.userX, this.PromoteCode);
            //if(this.statusX == Order.OStatus.NeedTaxIDReview)

            return order;
        }

        public Decimal totalAmountX
        {
            get
            {
                updateTotalAmount();

                return TotalAmount.GetValueOrDefault();
            }
        }
        private decimal? _cartDiscountX;
        public decimal? cartDiscountX
        {
            get
            {

                if (_cartDiscountX.HasValue == false)
                {
                    _cartDiscountX = this.TotalDiscount - (FreightDiscount.GetValueOrDefault() + TaxDiscount.GetValueOrDefault() + this.cartX.cartItemsX.Sum(x => x.DiscountAmount).GetValueOrDefault());
                }
                return _cartDiscountX.GetValueOrDefault();
            }
            set
            {
                _cartDiscountX = value;
            }
        }

        public void updateTotalAmount()
        {
            TotalDiscount = Math.Round(cartDiscountX.GetValueOrDefault()+FreightDiscount.GetValueOrDefault() + TaxDiscount.GetValueOrDefault() + this.cartX.cartItemsX.Sum(x => x.DiscountAmount).GetValueOrDefault(), 2);
            TotalAmount = Math.Round(cartX.TotalAmount + Freight.GetValueOrDefault() + Tax.GetValueOrDefault() + DutyAndTax.GetValueOrDefault() - TotalDiscount.GetValueOrDefault(), 2);
        }

        /// <summary>
        /// check Quotation totalAmount is below Cost ?
        /// </summary>
        /// <returns></returns>
        public bool isBelowCost()
        {
            return cartX.isBelowCost();
        }

        public bool isPStoreOrder()
        {
            return cartX.isPStoreOrder();
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

        public Store storeX
        {
            get
            {
                if (_storeX == null)
                    _storeX = cartX.storeX;  //navigation property

                return _storeX;
            }

            set
            {
                _storeX = value;
                StoreID = (_storeX == null) ? "" : _storeX.StoreID;
            }
        }

        private string _currencySign = null;
        // order currency
        public string currencySign
        {
            get
            {
                if (string.IsNullOrEmpty(_currencySign) && cartX != null)
                    _currencySign = cartX.currencySign;
                return _currencySign;
            }
            set { _currencySign = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public QStatus statusX
        {
            get
            {
                QStatus curStatus = QStatus.Open;
                if (stringDictionary.ContainsKey(this.Status))
                    curStatus = (QStatus)stringDictionary[this.Status];
                else
                    curStatus = QStatus.Open;

                if (curStatus == QStatus.Open || curStatus == QStatus.NeedTaxIDReview || curStatus == QStatus.NeedFreightReview)    //open quotation
                {
                    if (exceed24Hrs())
                        curStatus = QStatus.Unfinished;
                }
                else if (curStatus == QStatus.Confirmed || curStatus == QStatus.NeedGeneralReview ||
                         curStatus == QStatus.NeedTaxAndFreightReview)   //confirmed quotation
                {
                    if (exceed2WorkingWeek())
                        curStatus = QStatus.Expired;
                    else
                        curStatus = QStatus.Confirmed;
                }
                return curStatus;
            }

            set
            {
                if (value != QStatus.Expired && value != QStatus.Unfinished)    //skip runtime-only statuses
                    this.Status = enumDictionary[value];
            }
        }


        /// <summary>
        /// This property mark VAT is able or not
        /// </summary>
        public Boolean needVATReview()
        {
            if (this.statusX == QStatus.NeedTaxAndFreightReview ||
                this.statusX == QStatus.NeedTaxIDReview || this.statusX == QStatus.ConfirmedbutNeedTaxIDReview)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This property mark Freight is able or not
        /// </summary>
        /// <returns></returns>
        public bool needFreightReview()
        {
            if (this.statusX == QStatus.NeedFreightReview || this.statusX == QStatus.ConfirmedbutNeedFreightReview)
                return true;
            else
                return false;
        }


        public CampaignManager.PromotionCodeStatus applyPromotionCode(User user, String promotionCode)
        {
            CampaignManager.PromotionCodeStatus status = CampaignManager.PromotionCodeStatus.Valid;

            //remove promotion code
            //if (String.IsNullOrEmpty(promotionCode))
            //{
            //    this.TotalDiscount = 0m;
            //    this.PromoteCode = "";
            //    return CampaignManager.PromotionCodeStatus.Valid;
            //}

            DiscountDetail result = this.calculatePromotionTotalDiscount(user, cartX, Freight.GetValueOrDefault(), Tax.GetValueOrDefault(), promotionCode);

            status = result.codeStatus;
            if (status == CampaignManager.PromotionCodeStatus.Valid)
            {
                this.cartDiscountX = result.discountAmount;
                this.FreightDiscount = result.freightDiscountAmount;
                this.TaxDiscount = result.taxDiscountAmount;
                this.PromoteCode = promotionCode;
            }
            else if (!string.IsNullOrWhiteSpace(promotionCode))
            {
                this.applyPromotionCode(user,string.Empty);
            }
            else //if(string.IsNullOrWhiteSpace(this.PromoteCode))
            {
                this.cartDiscountX = 0;
                this.FreightDiscount = 0;
                this.TaxDiscount = 0;
                this.PromoteCode = null;
            }
            updateTotalAmount();
            return status;
        }

        /// <summary>
        /// This method exam if current quotation exceed its 24 grace period for modification and confirming
        /// </summary>
        /// <returns></returns>
        private Boolean exceed24Hrs()
        {
            if (QuoteDate.HasValue)
            {
                DateTime quoteDate = QuoteDate.GetValueOrDefault();
                if (DateTime.Now < quoteDate.AddHours(_modifiablePeriod))
                    return false;
                else
                    return true;
            }
            else
                return true;    //default as expired if there is no quotation date assigned since this is abnormal case
        }

        /// <summary>
        /// Check if current quotation exceed 10 weeking days after it's confirmed
        /// </summary>
        /// <returns></returns>
        private Boolean exceed2WorkingWeek()
        {
            if (this.ConfirmedDate.HasValue)
            {
                DateTime confirmDate = ConfirmedDate.GetValueOrDefault();
                if (DateTime.Now < confirmDate.AddDays(14))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        //enum status utility methods
        private static Dictionary<String, QStatus> _stringDictionary = null;
        private static Dictionary<QStatus, String> _enumDictionary = null;
        private Dictionary<QStatus, String> enumDictionary
        {
            get
            {
                if (_enumDictionary == null)
                    initDictionary();

                return _enumDictionary;
            }
        }

        private Dictionary<String, QStatus> stringDictionary
        {
            get
            {
                if (_stringDictionary == null)
                    initDictionary();

                return _stringDictionary;
            }
        }

        private void initDictionary()
        {
            _enumDictionary = new Dictionary<QStatus, string>();
            _stringDictionary = new Dictionary<string, QStatus>();

            foreach (int value in Enum.GetValues(typeof(QStatus)))
            {
                QStatus status = (QStatus)value;
                String name = Enum.GetName(typeof(QStatus), status);
                _enumDictionary.Add(status, name);
                _stringDictionary.Add(name, status);
            }
        }

        public void reconcile()
        {
            cartX.reconcile();

            //update promotion discount if there is any
            if (!String.IsNullOrEmpty(this.PromoteCode))   //has promotion code
                applyPromotionCode(userX, this.PromoteCode);
        }


        public void fixCurrency()
        {
            this.LocalCurrency = cartX.LocalCurrency;
            this.LocalCurExchangeRate = cartX.LocalCurExchangeRate;
        }

        public QuoteSource Source
        {
            get;
            set;
        }

        public string CustomerShippingMethodTemp { get; set; }

#region OMRegion
        //The following methods are for FollowUp used in OM
        protected override String getInstanceID() { return this.QuotationNumber; }
        protected override String getInstanceStoreID() { return this.StoreID; }
        protected override String getInstanceOwner() { return this.UserID; }
        protected override TrackingLog.TrackType getTrackType() { return TrackingLog.TrackType.QUOTATION; }
#endregion OMRegion
    }
}

