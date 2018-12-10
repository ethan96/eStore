using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Order : PromotionCodeEnabled
    {
        private Store _storeX = null;
        private User _userX = null;
        private OAlert _alert = OAlert.None;
        public enum OStatus { Open, WaitForPaymentResponse, Confirmed, NeedTaxAndFreightReview, NeedTaxIDReview, NeedGeneralReview, ReviewRejected, ReviewedApproved, Approved, Cancelled, Declined, NotSpecified, Closed_Complete, Closed_Converted, Closed, ConfirmdButNeedTaxIDReview,NeedFreightReview, ConfirmdButNeedFreightReview };
        public enum OAlert { None, NeedTaxAndFreightReview, NeedTaxIDReview, NeedGeneralReview, NeedFreightReview };
        public enum ShippingType { Dropoff, Recommend, Customer, None};
        //order 成功状态
        public static string[] SuccessfullyStatus = new string[] { "Confirmed", "ConfirmdButNeedTaxIDReview", "ConfirmdButNeedFreightReview", "Closed_Converted" };
        //order 失败状态
        public static string[] NoPassStatus = new string[] { "Open", "WaitForPaymentResponse", "Closed_Complete", "Closed", "NeedTaxAndFreightReview", "NeedTaxIDReview", "NeedGeneralReview"};

        private SalesPerson _salesPerson;
        private bool? salesPersonInitFlag;
        public SalesPerson salesPerson
        {
            get 
            {
                if (salesPersonInitFlag.HasValue==false)
                {
                    salesPersonInitFlag = true;
                    SAP_EMPLOYEEHelper saphelper = new SAP_EMPLOYEEHelper();
                    if (!string.IsNullOrEmpty(this.SalesID))
                        _salesPerson = saphelper.getSalesPersonByCode(this.SalesID,this.storeX);
                    else
                    {
                        if (this.Cart == null || this.Cart.SoldToContact == null || string.IsNullOrEmpty(this.Cart.SoldToContact.AddressID))
                            _salesPerson = null;
                        else
                            _salesPerson = saphelper.getSalesPersonByCompany(this.Cart.SoldToContact.AddressID, this.storeX, "VE").FirstOrDefault();
                    }
                }
                return _salesPerson; 
            }
            set { _salesPerson = value; }
        }


        //Programmer should use enum PaymentTypeX instead of PaymentType in string then has union name.
        public Payment.Payment_Type paymentTypeX
        {
            get 
            {
                Payment payment = (from item in this.Payments
                                   where item.PaymentID.Equals(this.PaymentID)
                                   select item).FirstOrDefault();
                if (payment == null)    //no match
                    return Payment.Payment_Type.NotClassified;      //show call for payment detail when it's not classified
                else
                    return payment.paymentTypeX;
            }
        }


        //If order is sync by SAP, this property should be true
        private bool _updateBySAP = false;
        public bool UpdateBySAP
        {
            get { return _updateBySAP; }
            set { _updateBySAP = value; }
        }

        public string CustomerShippingMethodTemp { get; set; }


        /// <summary>
        /// order is EdiOrder or not
        /// </summary>
        /// <returns></returns>
        public bool isEdiOrder()
        {
            if (this.OrderNo.StartsWith("LTR"))
                return true;
            else
                return false;
        }

        //default constructor for entity framework to create basic POCO instance
        public Order()
        {
            //get new order ID from order helper **********
            //temporary solution
            OrderNo = "ORDER." + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            OrderDate = DateTime.Now;
            statusX = OStatus.Open;
        }

        public Order(Store store, User user, Cart cart)
            : this()
        {
            storeX = store;
            userX = user;

            Cart = cart;
            LocalCurrency = cart.LocalCurrency;
            LocalCurExchangeRate = cart.LocalCurExchangeRate;
            CombineOrderFlag = "N";
            ConfirmedBy = user.UserID;
            ConfirmedDate = DateTime.Now;

            DueDate = ConfirmedDate.GetValueOrDefault().AddDays(2);
            EarlyShipFlag = false;
            LastUpdated = ConfirmedDate;
            OrderDate = ConfirmedDate;
            PartialFlag = "N";
            RequiredDate = DueDate;
            ResellerID = user.ResellerID;
            //overwrite default order no with store particular order number
            if (isPStoreOrder())
            {
                OrderNo = storeX.getNextOrderNumber("P");
            }
            else
                OrderNo = storeX.nextOrderNumber;
        }

        public Order(Store store, User user, Quotation quote) : this()
        {
            
            storeX = store;
            userX = user;

            //Cart = quote.cartX;
            Cart = new Cart(store, user);
            quote.cartX.copyTo(Cart);

            CombineOrderFlag = "N";
            ConfirmedBy = user.UserID;
            ConfirmedDate = DateTime.Now;

            Freight = quote.Freight;
            Insurance = quote.Insurance;
            ShipmentTerm = quote.ShipmentTerm;
            Courier = quote.ShipmentTerm;

            ShippingMethod  = quote.ShippingMethod;
            TaxRate = quote.TaxRate;
            Courier = "";
            Tax = quote.Tax;
            TaxAndFees = quote.TaxAndFees;
            OtherTaxAndFees = quote.OtherTaxAndFees;
            VATTax = quote.VATTax;
            DutyAndTax = quote.DutyAndTax;

            TotalDiscount = quote.TotalDiscount;
            PromoteCode = quote.PromoteCode;

            DueDate = ConfirmedDate.GetValueOrDefault().AddDays(2);
            EarlyShipFlag = false;
            LastUpdated = ConfirmedDate;
            OrderDate = ConfirmedDate;
            PartialFlag = "N";
            RequiredDate = DueDate;

            Source = quote.QuotationNumber;
            ResellerID = quote.ResellerID;
            CustomerComment = quote.Comments;
            VATNumbe = quote.VATNumber;
            CourierAccount = quote.ShipmentTerm;

            LocalCurrency = quote.LocalCurrency;
            LocalCurExchangeRate = quote.LocalCurExchangeRate;

            //overwrite default order no with store particular order number
            if (isPStoreOrder())
            {
                OrderNo = storeX.getNextOrderNumber("P");
            }
            else
                OrderNo = storeX.nextOrderNumber;

            // if AEU quotation will check VAT No.
            if (store.getBooleanSetting("EnableVATSetting"))
            {
                if (quote.needVATReview())
                {
                    this.statusX = OStatus.NeedTaxIDReview;
                    this.setAlert(OAlert.NeedTaxIDReview);
                }
            }
            if (store.getBooleanSetting("ProceedWithProblematicFreight"))
            {
                if (quote.needFreightReview())
                {
                    this.statusX = OStatus.NeedFreightReview;
                    this.setAlert(OAlert.NeedFreightReview);
                }
            }
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
                    Cart = new Cart(storeX, userX);

                return Cart;
            }

            set { Cart = value; }
        }

        public string discountType
        {
            get;
            set;
        }

        public Decimal totalAmountX
        {
            get
            {
                //TotalAmount = Math.Round(cartX.TotalAmount + Freight.GetValueOrDefault() + Tax.GetValueOrDefault(), 2);
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
            TotalDiscount = Math.Round(this.cartDiscountX.GetValueOrDefault() + FreightDiscount.GetValueOrDefault() + TaxDiscount.GetValueOrDefault() + this.cartX.cartItemsX.Sum(x => x.DiscountAmount).GetValueOrDefault(), 2);
          
            TotalAmount = Math.Round(cartX.TotalAmount 
                + Freight.GetValueOrDefault() + Tax.GetValueOrDefault() + DutyAndTax.GetValueOrDefault() + Surcharge.GetValueOrDefault() 
                - TotalDiscount.GetValueOrDefault(), 2);
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
        /// check Order totalAmount is below Cost ?
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

        /// <summary>
        /// This method is for UI layer to turn on alert if VAT or other information are not valid. eStore owner
        /// would need to pay attention at reviewing it.
        /// </summary>
        /// <param name="alert"></param>
        public void setAlert(OAlert alert)
        {
            _alert = alert;
        }

        public OAlert GetAlert()
        {
            return _alert;
        }

        /// <summary>
        /// This property allows use to get and set last order status
        /// </summary>
        public OStatus statusX
        {
            get
            {
                if (stringDictionary.ContainsKey(this.OrderStatus))
                    return (OStatus)stringDictionary[this.OrderStatus];
                else
                    return OStatus.NotSpecified;
            }

            set { this.OrderStatus = enumDictionary[value]; }
        }

        /// <summary>
        /// this method is to mark order is confirmd or not
        /// </summary>
        public bool isConfirmdOrder
        {
            get
            {
                switch (this.statusX)
                {
                    case OStatus.ConfirmdButNeedFreightReview:
                    case OStatus.ConfirmdButNeedTaxIDReview:
                    case OStatus.Confirmed:
                        return true;
                    default:
                        return false;
                }
            }
        }
        
        /// <summary>
        /// This is a read-only property.  It returns order's last payment status.  If there isn't last payment available, it return "New"
        /// </summary>
        public Payment.PaymentStatus paymentStatus
        {
            get
            {
                if (this.Payments != null && !String.IsNullOrEmpty(PaymentID))
                {
                    Payment lastPayment = (from payment in Payments
                                           where payment.PaymentID.Equals(this.PaymentID)
                                           select payment).FirstOrDefault();
                    if (lastPayment == null)
                        return Payment.PaymentStatus.New;
                    else
                        return lastPayment.statusX;
                }
                else
                    return Payment.PaymentStatus.New;
            }
        }

        /// <summary>
        /// After user makes payment regardless if it's a successful payment or not. Application shall call this method to keep 
        /// the payment record.  In this method, it will also set order status to proper status according to payment status.
        /// </summary>
        /// <param name="item"></param>
        public void addPayment(Payment item)
        {
            if (Payments == null)
                Payments = new List<Payment>();

            //adjustt order status here
            if (! Payments.Contains(item))
                Payments.Add(item); //only add payment item if this payment is not in order payment list

            //adjust order status here
            switch (item.statusX)
            {
                case Payment.PaymentStatus.Approved:
                case Payment.PaymentStatus.FraudAlert:
                case Payment.PaymentStatus.NeedAttention:
                    statusX = confirmOrderWithAlert();
                    //log promotion code applied record if promotion applied
                    if (PromoteCode!=null)
                    {
                        savePromotionAppliedLogs(PromoteCode);
                    }
                    break;
                default:  //no change at order status
                    break;
            }
        }

        /// <summary>
        /// This method is to retrieve last opening (indirect payment)
        /// </summary>
        /// <returns></returns>
        public Payment getLastOpenPayment()
        {
            Payment payment = (from item in Payments
                               orderby item.CreatedDate descending
                               where item.statusX == Payment.PaymentStatus.New
                               select item).FirstOrDefault();

            return payment;
        }

        /// <summary>
        /// This method is to retrieve last payment instance
        /// </summary>
        /// <returns></returns>
        public Payment getLastPayment()
        {
            Payment payment = (from item in Payments
                               where item.PaymentID.Equals( this.PaymentID)
                               select item).FirstOrDefault();

            return payment;
        }
        public Payment getLastAuthorizationPayment()
        {
            Payment payment = (from item in Payments
                               where item.TranxType == "Authorization" && item.isValidPaymentAccepted()
                               orderby item.CreatedDate descending 
                               select item).FirstOrDefault();

            return payment;
        }
        public CampaignManager.PromotionCodeStatus applyPromotionCode(User user, String promotionCode, bool TaxAndFreightOnly = false)
        {
           

            CampaignManager.PromotionCodeStatus status = CampaignManager.PromotionCodeStatus.Valid;
            if (!string.IsNullOrEmpty(this.PromoteCode) && TaxAndFreightOnly)
            {
                status = CampaignManager.PromotionCodeStatus.Applied;
                return status;
            
            }
            //remove promotion code
            //if (String.IsNullOrEmpty(promotionCode))
            //{
            //    this.TotalDiscount = 0m;
            //    this.PromoteCode = "";
            //    return CampaignManager.PromotionCodeStatus.Valid;
            //}

            DiscountDetail result = this.calculatePromotionTotalDiscount
                (
                    user, 
                    cartX, 
                    Freight.GetValueOrDefault(), 
                    Tax.GetValueOrDefault(), 
                    promotionCode,
                    TaxAndFreightOnly
                );
            CampaignStrategy campaignStrategy = new CampaignStrategy();
            if (result.AppliedStrategies != null)
            {
                    if (result.AppliedStrategies.Count > 0)
                    {
                            campaignStrategy = result.AppliedStrategies[0];
                    }
            }
       
            
            status = result.codeStatus;
            if (status == CampaignManager.PromotionCodeStatus.Valid)
            {
                if (TaxAndFreightOnly)
                {
                    this.cartDiscountX = result.discountAmount;
                    this.FreightDiscount = result.freightDiscountAmount;
                    this.TaxDiscount = result.taxDiscountAmount;
                    this.PromoteCode = promotionCode == null ? string.Empty : promotionCode;
                    this.discountType = campaignStrategy.DiscountType;
                }
                else
                {
                    this.cartDiscountX = result.discountAmount;
                    this.FreightDiscount = result.freightDiscountAmount;
                    this.TaxDiscount = result.taxDiscountAmount;
                    //Empty string means this order has promotion applied, and the promotion strategy doesn't require promotion code
                    this.PromoteCode = promotionCode == null ? string.Empty : promotionCode;
                    this.discountType = campaignStrategy.DiscountType;
                }
            }
            else if (!string.IsNullOrWhiteSpace(promotionCode))
            {
                this.applyPromotionCode(user, string.Empty);
            }
            else// if (string.IsNullOrWhiteSpace(this.PromoteCode))
            {
                this.cartDiscountX = 0;
                this.FreightDiscount = 0;
                this.TaxDiscount = 0;
                //When promotion code is null, it means no promotion is applied.
                this.PromoteCode = null;
                this.discountType = string.Empty;
            }
            updateTotalAmount();
            return status;
        }

        /// <summary>
        /// This method is to further process order status after successful payment if there is suspecious alert
        /// </summary>
        /// <returns></returns>
        private OStatus confirmOrderWithAlert()
        {
            if (_alert == OAlert.NeedGeneralReview)
                return OStatus.NeedGeneralReview;
            else if (_alert == OAlert.NeedTaxAndFreightReview)
                return OStatus.NeedTaxAndFreightReview;
            else if (_alert == OAlert.NeedTaxIDReview)
                return OStatus.ConfirmdButNeedTaxIDReview;
            else if (_alert == OAlert.NeedFreightReview)
                return OStatus.ConfirmdButNeedFreightReview;
            else
                return OStatus.Confirmed;
        }

        private static Dictionary<String, OStatus> _stringDictionary = null;
        private static Dictionary<OStatus, String> _enumDictionary = null;
        private Dictionary<OStatus, String> enumDictionary
        {
            get
            {
                if (_enumDictionary == null)
                    initDictionary();

                return _enumDictionary;
            }
        }

        private Dictionary<String, OStatus> stringDictionary
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
            _enumDictionary = new Dictionary<OStatus, string>();
            _stringDictionary = new Dictionary<string, OStatus>();

            foreach (int value in Enum.GetValues(typeof(OStatus)))
            {
                OStatus status = (OStatus)value;
                String name = Enum.GetName(typeof(OStatus), status);
                _enumDictionary.Add(status, name);
                _stringDictionary.Add(name, status);
            }
        }

        /// <summary>
        /// This method indicates whether the order is referred to Advantech channel partner or not.
        /// </summary>
        public bool isReferredToChannelPartner()
        {
            bool referred = false;
            if (this.storeX.channelPartnerReferralEnabled && this.ChannelID.HasValue)
            {
                if (this.ChannelID != this.storeX.storeChannelAccount.Channelid)
                    referred = true;
            }
            return referred;
        }

        private eStore.POCOS.PocoX.ChannelPartner _channel;
        public eStore.POCOS.PocoX.ChannelPartner Channel
        {
            get 
            {
                if (_channel == null && ChannelID.HasValue)
                        _channel = eStore.POCOS.DAL.ChannelPartnerHelper.getChannelPartner(ChannelID.Value);                     
                return _channel; 
            }
            set 
            { 
                _channel = value;
                if (_channel != null)
                    this.ChannelID = _channel.Channelid;
                else
                    this.ChannelID = null;
            }
        }

        /// <summary>
        /// This is a safe property for retrieving referring Store instance from Order
        /// </summary>
        public Store storeX
        {
            get
            {
                if (_storeX == null && !String.IsNullOrWhiteSpace(this.StoreID))
                    _storeX = (new StoreHelper()).getStorebyStoreid(this.StoreID);

                return _storeX;
            }

            set 
            { 
                _storeX = value; 
                this.StoreID = _storeX.StoreID;
            }
        }

        private ShippingType _shipipingTypeX = ShippingType.None;
        public ShippingType ShipipingTypeX
        {
            get 
            {
                if (_shipipingTypeX == ShippingType.None)
                {
                    if (this.ShippingMethod == "DHL Worldwide Package Express")
                        _shipipingTypeX = ShippingType.Dropoff;
                }
                return _shipipingTypeX; 
            }
            set { _shipipingTypeX = value; }
        }

        /// <summary>
        /// get all Amount > 0 valid payment
        /// </summary>
        public List<Payment> validPayments
        {
            get 
            {
                List<Payment> _validPayments = new List<Payment>();
                if (this.Payments.Any())
                {
                    foreach (var item in this.Payments)
                    {
                        if (item.Amount.HasValue && item.Amount.Value > 0)
                        {
                            if (item.isValidPaymentAccepted() && item.TranxType != "Void")
                                _validPayments.Add(item);
                        }
                    }
                }
                return _validPayments; 
            }
        }

        private OrderSAPSyncStatus _SAPSyncStatusX;
        public OrderSAPSyncStatus SAPSyncStatusX
        {
            get 
            {
                if (string.IsNullOrEmpty(SAPSyncStatus))
                {
                    if (this.StoreID == "AEU")
                    {

                        if (AddressIsSAP())
                            SAPSyncStatus = OrderSAPSyncStatus.GoodToSync.ToString();//全部存在
                        else
                            SAPSyncStatus = OrderSAPSyncStatus.NewSAPCustomer.ToString();//不存在
                    }
                    else
                    {
                        SAPSyncStatus = OrderSAPSyncStatus.GoodToSync.ToString();//do not check other stores, can be sync to sap derictly
                    }

                }
                _SAPSyncStatusX = (OrderSAPSyncStatus)Enum.Parse(typeof(OrderSAPSyncStatus), SAPSyncStatus);
                return _SAPSyncStatusX; 
            }
            set { SAPSyncStatus = value.ToString(); }
        }
        //根据sold bill ship address 判断是否在SAP中.
        public Boolean AddressIsSAP()
        {
            if (!string.IsNullOrEmpty(cartX.SoldToContact.AddressID) &&
                    !string.IsNullOrEmpty(cartX.BillToContact.AddressID) && !string.IsNullOrEmpty(cartX.ShipToContact.AddressID))
            {
                SAPCompanyHelper sHelper = new SAPCompanyHelper();
                if (sHelper.getSAPCompanybyID(cartX.SoldToContact.AddressID) != null && sHelper.getSAPCompanybyID(cartX.BillToContact.AddressID) != null
                                && sHelper.getSAPCompanybyID(cartX.ShipToContact.AddressID) != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public enum OrderSAPSyncStatus { NewSAPCustomer, ApplyingSAPCustomer, GoodToSync, Failed, Success };
#region OM
        //The following methods are for FollowUp used in OM
        protected override String getInstanceID() { return this.OrderNo; }
        protected override String getInstanceStoreID() { return this.StoreID; }
        protected override String getInstanceOwner() { return this.UserID; }
        protected override TrackingLog.TrackType getTrackType() { return TrackingLog.TrackType.ORDER; }

        /// <summary>
        /// This method is an expensive method. When it's invoked, it will trigger a process to complete reconcile and recalculate
        /// entire Order price including cart total, tax and shipping charge.  This method shall only be used in OM
        /// </summary>
        public void reconcile()
        {
            cartX.reconcile();
            this.updateTotalAmount();
        }

        //判断是那个 运送公司
        public string ShippingMethodX
        {
            get 
            {
                if (ShippingMethod.StartsWith("UPS"))
                    return "UPS";
                else if (ShippingMethod.StartsWith("FedEx"))
                    return "FedEx";
                else
                    return "UPS";
            }
        }
        
#endregion OM
        /// <summary>
        /// get order change log
        /// </summary>
        public List<ChangeLog> changeLogs //is can romove to abstract class?
        {
            get
            {
                ChangeLogHelper clh = new ChangeLogHelper();
                return clh.getChangelogs(this.OrderNo);
            }
        }


        private string _customerPONoX;
        public string customerPONoX
        {
            get { return _customerPONoX; }
            set { _customerPONoX = value; }
        }
        
    }
}
