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
    public partial class RewardRecord
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int RecordID
        {
            get;
            set;
        }
    
        public virtual string StoreID
        {
            get;
            set;
        }
    
        public virtual string UserID
        {
            get;
            set;
        }
    
        public virtual Nullable<int> RewardID
        {
            get;
            set;
        }
    
        public virtual Nullable<int> ActivityID
        {
            get;
            set;
        }
    
        public virtual Nullable<int> TransactionType
        {
            get;
            set;
        }
    
        public virtual Nullable<int> RecordType
        {
            get;
            set;
        }
    
        public virtual string OrderNo
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Qty
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Point
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> TotalPoint
        {
            get;
            set;
        }
    
        public virtual string SalesEmail
        {
            get;
            set;
        }
    
        public virtual string Receiver1
        {
            get;
            set;
        }
    
        public virtual string EmailAddress1
        {
            get;
            set;
        }
    
        public virtual string CompanyName1
        {
            get;
            set;
        }
    
        public virtual string Zip1
        {
            get;
            set;
        }
    
        public virtual string Address1
        {
            get;
            set;
        }
    
        public virtual string Mobile1
        {
            get;
            set;
        }
    
        public virtual string Tel1
        {
            get;
            set;
        }
    
        public virtual string Receiver2
        {
            get;
            set;
        }
    
        public virtual string EmailAddress2
        {
            get;
            set;
        }
    
        public virtual string CompanyName2
        {
            get;
            set;
        }
    
        public virtual string Zip2
        {
            get;
            set;
        }
    
        public virtual string Address2
        {
            get;
            set;
        }
    
        public virtual string Mobile2
        {
            get;
            set;
        }
    
        public virtual string Tel2
        {
            get;
            set;
        }
    
        public virtual string CreatedBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreatedDate
        {
            get;
            set;
        }
    
        public virtual string UpdatedBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> UpdatedDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ShippingTime
        {
            get;
            set;
        }
    
        public virtual Nullable<int> SendMailStatus_Internal
        {
            get;
            set;
        }
    
        public virtual Nullable<int> SendMailStatus_Corporate
        {
            get;
            set;
        }
    
        public virtual Nullable<int> SendMailStatus_Sales
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   
    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(RewardRecord); // Get type pointer
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

    }
}