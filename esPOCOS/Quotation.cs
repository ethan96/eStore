//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
 

namespace eStore.POCOS
{
    public partial class Quotation
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string CartID
        {
            get { return _cartID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_cartID != value)
                    {
                        if (Cart != null && Cart.CartID != value)
                        {
                            Cart = null;
                        }
                        _cartID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _cartID;
    
        public virtual string StoreID
        {
            get { return _storeID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_storeID != value)
                    {
                        if (Cart != null && Cart.StoreID != value)
                        {
                            Cart = null;
                        }
                        _storeID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _storeID;
    
        public virtual string QuotationNumber
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> QuoteDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> QuoteExpiredDate
        {
            get;
            set;
        }
    
        public virtual string Version
        {
            get;
            set;
        }
    
        public virtual string ShipmentTerm
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> EarlyShipFlag
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Freight
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Insurance
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Tax
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> DueDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> RequiredDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ConfirmedDate
        {
            get;
            set;
        }
    
        public virtual string ConfirmedBy
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalAmount
        {
            get;
            set;
        }
    
        public virtual string UserID
        {
            get { return _userID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_userID != value)
                    {
                        if (User != null && User.UserID != value)
                        {
                            User = null;
                        }
                        _userID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _userID;
    
        public virtual string Status
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TaxRate
        {
            get;
            set;
        }
    
        public virtual string ShippingMethod
        {
            get;
            set;
        }
    
        public virtual string PromoteCode
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalDiscount
        {
            get;
            set;
        }
    
        public virtual string LocalCurrency
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> LocalCurExchangeRate
        {
            get;
            set;
        }
    
        public virtual string Comments
        {
            get;
            set;
        }
    
        public virtual string LastUpdateBy
        {
            get;
            set;
        }
    
        public virtual string ResellerID
        {
            get;
            set;
        }
    
        public virtual string VATNumber
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> FreightDiscount
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TaxDiscount
        {
            get;
            set;
        }
    
        public virtual string ResellerCertificate
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> DutyAndTax
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TaxAndFees
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> VATTax
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> OtherTaxAndFees
        {
            get;
            set;
        }
    
        public virtual string RegistrationNumber
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (CartID == null) {
						 error_message.Add(new ErrorMessage("CartID", "CartID can not be Null "));
				}
		 if (StoreID == null) {
						 error_message.Add(new ErrorMessage("StoreID", "StoreID can not be Null "));
				}
		 if (QuotationNumber == null) {
						 error_message.Add(new ErrorMessage("QuotationNumber", "QuotationNumber can not be Null "));
				}
		 if (Version == null) {
						 error_message.Add(new ErrorMessage("Version", "Version can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(Quotation); // Get type pointer
             PropertyInfo[] fields = type.GetProperties(); // Obtain all fields
                foreach (var pinfo in fields) // Loop through fields
                {
                    string name = pinfo.Name; // Get string name
                    object temp = pinfo.GetValue(pinfo.Name,null); // Get value
                if (temp is int) // See if it is an integer.
                {
                    int value = (int)temp;
                    Console.Write(name);
                    Console.Write(" (int) = ");
                    Console.WriteLine(value);
                }
                else if (temp is string) // See if it is a string.
                {
                    string value = temp as string;
                    Console.Write(name);
                    Console.Write(" (string) = ");
                    Console.WriteLine(value);
                }
            }
        }*/
     
    	
    	
        #endregion

        #region Navigation Properties
    
        public virtual Cart Cart
        {
            get { return _cart; }
            set
            {
                if (!ReferenceEquals(_cart, value))
                {
                    var previousValue = _cart;
                    _cart = value;
                    FixupCart(previousValue);
                }
            }
        }
        private Cart _cart;
    
        public virtual User User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                }
            }
        }
        private User _user;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupCart(Cart previousValue)
        {
            if (previousValue != null && previousValue.Quotations.Contains(this))
            {
                previousValue.Quotations.Remove(this);
            }
    
            if (Cart != null)
            {
                if (!Cart.Quotations.Contains(this))
                {
                    Cart.Quotations.Add(this);
                }
                if (CartID != Cart.CartID)
                {
                    CartID = Cart.CartID;
                }
                if (StoreID != Cart.StoreID)
                {
                    StoreID = Cart.StoreID;
                }
            }
        }
    
        private void FixupUser(User previousValue)
        {
            if (User != null)
            {
                if (UserID != User.UserID)
                {
                    UserID = User.UserID;
                }
            }
            else if (!_settingFK)
            {
                UserID = null;
            }
        }

        #endregion

    }
}
