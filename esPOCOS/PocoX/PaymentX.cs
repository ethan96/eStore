using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Payment 
    { 
 
#region Extension Methods 
        // Sale                          :   immediate charge
        // Authorization            :   Pre-authorize -- to complete the transaction, another step of DelayCapture need to be performed.
        // DelayCapture           :   This action is to capture and realize pre-authorized transaction
        // Credit                        :   This action is to credit back realized charge
        // Void                           :   This is to void pre-authorized transaction
        // AddressValidation    :   This request is to validate user billing address with credit card billing address
        //     *** following transactions type are complex transactions
        // Reauthorization        :   This is to change pre-authorized amount to new amount
        // ReauthorizationAddition :   This is to charge additonal amount on top of original payment
        public enum TransactionType { Sale, Authorization, DelayCapture, Credit, Void, AddresValidation, /*complex transactions*/ Reauthorization, /* for record only*/ ReauthorizationAddition};

        /// <summary>
        /// Approved              -- successful transaction
        /// AuthenticationFailed  -- this will only happen when merchant information becomes invalid (expired or be blocked)
        /// NotSupported          -- This happens when currency is not supported, transaction type is not supported and
        ///                             other not supported condition (for details, refer to result code and transaction description
        /// Timeout               -- Client timeout when transaction exceed the timeout time.
        /// Declined              -- Payment information (for example credit card number or expiration date) is not correct.
        /// Referral              -- Transaction can not be approved electronically, but can be approved with verbal authorization.
        ///                          In this case, application shall suggest user to contact Advantech to complete the transaction
        /// DuplicateTransaction  -- 
        /// InsufficientFund      -- Insufficient funds available in account
        /// ExceedTransactionLimit-- Exceed per transaction limit
        /// GeneralError          -- for details refer to transaction description and transaction code
        /// CommunicationError    -- Any failure due to communication shall be classified in the status
        /// NeedAttention         -- When payment is in this status, eStore shall suggest user to contact Advantech eStore to 
        ///                             resolve the issue.
        /// FraudAlert            -- When payment is in this status, eStore shall suggest user to contact Advantech eStore to 
        ///                             resolve the issue.
        ///Voided                    voided payment                             
        /// RemoteServerError	  -- Bank's transaction server has unknow error.
        /// </summary>
        public enum PaymentStatus
        {
            Approved, AuthenticationFailed, NotSupported, Timeout, Declined, Referral, DuplicateTransaction,
            InsufficientFund, ExceedTransactionLimit, GeneralError, CommunicationError, RemoteServerError,NeedAttention, FraudAlert, New
            , Voided, InvalidAccountNumber, InvalidExpirationDate, ExceedsPerTransactionLimit, InvalidAmount
            //IC 2014/07/08 Add PaymentStatus 23/24/51
        };

        public enum Payment_Type { PayByCreditCard, PayByWireTransfer, PayByNetTerm, NotClassified, PayByDirectly, Daoupay };

        private static Dictionary<String, PaymentStatus> _stringDictionary = null;
        private static Dictionary<PaymentStatus, String> _enumDictionary = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Payment()
        {
            //generate unique payment ID in format of YYYYMMDD_HHmmss_fff
            this.PaymentID = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

            //create dictionary if need
            if (_enumDictionary == null)
                initDictionary();

            statusX = PaymentStatus.New;
            this.CreatedDate = DateTime.Now;
        }

        public Payment(String cardno, String holdername, String cardType, 
                        String expirationDate, String securityCode, Decimal amount) : this()
        {
            this.cardNo = cardno;
            this.CardHolderName = holdername;
            this.CardType = cardType;
            this.CardExpiredDate = expirationDate;
            this.SecurityCode = securityCode;
            this.Amount = amount;
        }

        /// <summary>
        /// This is a runtime property for storing non-encyrpted cardNo. Before payment is stored in DB, it'll encypt this
        /// number and generated a encypted number.
        /// </summary>
        public String cardNo
        {
            get;
            set;
        }
        
        /// <summary>
        /// This property is used to check payment solution environment is ready or not.
        /// AKR payment solution(INIpaySolution) needs to check API installation environment is fine or not.
        /// If env is ready, it should be set as true;
        /// </summary>
        public bool envStatusIsReady
        {
            get;
            set;
        }

        public PaymentStatus statusX
        {
            get 
            {
                if (stringDictionary.ContainsKey(this.Status))
                    return (PaymentStatus)stringDictionary[this.Status];
                else
                    return PaymentStatus.NotSupported;
            }

            set { this.Status = enumDictionary[value]; }
        }

        /// <summary>
        /// This property is to retain the transaction response values returned by service provider
        /// </summary>
        public IDictionary<String, String> responseValues
        {
            get;
            set;
        }

        public String cardExpiredMonth
        {
            get
            {
                if (CardExpiredDate.Length != 4)    //invalid expiration format
                    return "01";
                else
                    return CardExpiredDate.Substring(0, 2);
            }
        }

        public String cardExpiredYear
        {
            get
            {
                if (CardExpiredDate.Length != 4)    //invalid expiration format
                    return "00";
                else
                    return CardExpiredDate.Substring(2,2);
            }
        }

        public Payment_Type paymentTypeX
        {
            get
            {
                if (String.IsNullOrWhiteSpace(PaymentType))
                    return Payment_Type.PayByCreditCard;
                else
                {
                    if (PaymentType.Equals("WireTransfer"))
                        return Payment_Type.PayByWireTransfer;
                    else if (PaymentType.Equals("NetTerm"))
                        return Payment_Type.PayByNetTerm;
                    else if (PaymentType.Equals("PayDirectly"))
                        return Payment_Type.PayByDirectly;
                    else if (PaymentType.StartsWith("Daoupay",true,null))
                        return Payment_Type.Daoupay;
                    else
                        return Payment_Type.PayByCreditCard;
                }
            }
        }


        private Dictionary<PaymentStatus, String> enumDictionary
        {
            get
            {
                if (_enumDictionary == null)
                    initDictionary();

                return _enumDictionary;
            }
        }

        private Dictionary<String, PaymentStatus> stringDictionary
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
            _enumDictionary = new Dictionary<PaymentStatus,string>();
            _stringDictionary = new Dictionary<string,PaymentStatus>();

            foreach (int value in Enum.GetValues(typeof(PaymentStatus)))
            {
                PaymentStatus status = (PaymentStatus)value;
                String name = Enum.GetName(typeof(PaymentStatus), status);
                _enumDictionary.Add(status, name);
                _stringDictionary.Add(name, status);
            }
        }

        /// <summary>
        /// Each store's payment menthod has individual error code.
        /// </summary>
        public string errorCode
        {
            get;
            set;
        }
        public string PurchaseNO
        {
            get;
            set;
        }
        public string FederalID
        {
            get;
            set;
        }

        public void copyAccountInfo(Payment origPaymentInfo)
        {
            this.Amount = origPaymentInfo.Amount;
            this.CardExpiredDate = origPaymentInfo.CardExpiredDate;
            this.CardHolderName = origPaymentInfo.CardHolderName;
            this.CardType = origPaymentInfo.CardType;
            this.CartID = origPaymentInfo.CartID;
            this.LastFourDigit = origPaymentInfo.LastFourDigit;
            this.OrderNo = origPaymentInfo.OrderNo;
            this.PaymentType = origPaymentInfo.PaymentType;
            this.SecurityCode = origPaymentInfo.SecurityCode;
            this.StoreID = origPaymentInfo.StoreID;
            this.StatusCode = origPaymentInfo.StatusCode;
        }

        /// <summary>
        /// This is a runtime property showing whether the payment is an accepted transaction
        /// zero transaction is invalid
        /// </summary>
        public Boolean isPaymentAccepted()
        {
            if ((this.statusX == Payment.PaymentStatus.Approved || this.statusX == Payment.PaymentStatus.FraudAlert) && this.Amount>0)
                return true;
            else
                return false;
        }
        
        /// <summary>
        /// eStore valid payment 
        /// for ui and om
        /// </summary>
        /// <returns></returns>
        public bool isValidPaymentAccepted()
        {
            if (this.statusX == Payment.PaymentStatus.Approved 
                    || this.statusX == Payment.PaymentStatus.FraudAlert
                    || this.statusX == PaymentStatus.NeedAttention)
                return true;
            else
                return false;
        }

        public bool isSimulation {
            get {
                if (!string.IsNullOrWhiteSpace(this.Comment2) && this.Comment2 == "Simulation")
                    return true;
                else
                    return false;
            }
        }

#endregion 
	} 
 }