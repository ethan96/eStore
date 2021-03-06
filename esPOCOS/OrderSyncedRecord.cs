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
    public partial class OrderSyncedRecord
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string OrderNo
        {
            get { return _orderNo; }
            set
            {
                if (_orderNo != value)
                {
                    if (Order != null && Order.OrderNo != value)
                    {
                        Order = null;
                    }
                    _orderNo = value;
                }
            }
        }
        private string _orderNo;
    
        public virtual string StoreID
        {
            get { return _storeID; }
            set
            {
                if (_storeID != value)
                {
                    if (Order != null && Order.StoreID != value)
                    {
                        Order = null;
                    }
                    _storeID = value;
                }
            }
        }
        private string _storeID;
    
        public virtual string CartID
        {
            get { return _cartID; }
            set
            {
                if (_cartID != value)
                {
                    if (Order != null && Order.CartID != value)
                    {
                        Order = null;
                    }
                    _cartID = value;
                }
            }
        }
        private string _cartID;
    
        public virtual string SalesOffice
        {
            get;
            set;
        }
    
        public virtual string CashManagmentGroup
        {
            get;
            set;
        }
    
        public virtual string SalesDistrict
        {
            get;
            set;
        }
    
        public virtual string SalesGroup
        {
            get;
            set;
        }
    
        public virtual string Division
        {
            get;
            set;
        }
    
        public virtual string BankCode
        {
            get;
            set;
        }
    
        public virtual string DeliveryPriority
        {
            get;
            set;
        }
    
        public virtual string SalesPerson
        {
            get;
            set;
        }
    
        public virtual string SalesAssistant
        {
            get;
            set;
        }
    
        public virtual string EMPCODE3
        {
            get;
            set;
        }
    
        public virtual string ShippingCondition
        {
            get;
            set;
        }
    
        public virtual string Account
        {
            get;
            set;
        }
    
        public virtual string Distribution_Channel
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> EarlyShipping
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> PartialShipment
        {
            get;
            set;
        }
    
        public virtual string ReturnMessage
        {
            get;
            set;
        }
    
        public virtual string Status
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreatedDate
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (OrderNo == null) {
						 error_message.Add(new ErrorMessage("OrderNo", "OrderNo can not be Null "));
				}
		 if (StoreID == null) {
						 error_message.Add(new ErrorMessage("StoreID", "StoreID can not be Null "));
				}
		 if (CartID == null) {
						 error_message.Add(new ErrorMessage("CartID", "CartID can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(OrderSyncedRecord); // Get type pointer
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
    
        public virtual Order Order
        {
            get { return _order; }
            set
            {
                if (!ReferenceEquals(_order, value))
                {
                    var previousValue = _order;
                    _order = value;
                    FixupOrder(previousValue);
                }
            }
        }
        private Order _order;

        #endregion

        #region Association Fixup
    
        private void FixupOrder(Order previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.OrderSyncedRecord, this))
            {
                previousValue.OrderSyncedRecord = null;
            }
    
            if (Order != null)
            {
                Order.OrderSyncedRecord = this;
                if (StoreID != Order.StoreID)
                {
                    StoreID = Order.StoreID;
                }
                if (CartID != Order.CartID)
                {
                    CartID = Order.CartID;
                }
                if (OrderNo != Order.OrderNo)
                {
                    OrderNo = Order.OrderNo;
                }
            }
        }

        #endregion

    }
}
