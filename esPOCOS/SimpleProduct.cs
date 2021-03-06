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
    public partial class SimpleProduct
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string StoreID
        {
            get;
            set;
        }
    
        public virtual string SProductID
        {
            get;
            set;
        }
    
        public virtual string VendorProductName
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> VendorSuggestedPrice
        {
            get;
            set;
        }
    
        public virtual string PriceType
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> LocalPrice
        {
            get;
            set;
        }
    
        public virtual string PriceSourceProvider
        {
            get;
            set;
        }
    
        public virtual string VendorProductDesc
        {
            get;
            set;
        }
    
        public virtual string StockStatus
        {
            get;
            set;
        }
    
        public virtual string ModelNo
        {
            get;
            set;
        }
    
        public virtual string VendorProductLine
        {
            get;
            set;
        }
    
        public virtual string VendorProductGroup
        {
            get;
            set;
        }
    
        public virtual string TumbnailImageID
        {
            get;
            set;
        }
    
        public virtual string ImageID
        {
            get;
            set;
        }
    
        public virtual string ProductType
        {
            get;
            set;
        }
    
        public virtual string VendorExtendedDesc
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Cost
        {
            get;
            set;
        }
    
        public virtual string RoHSStatus
        {
            get;
            set;
        }
    
        public virtual string ABCInd
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> LocalCost
        {
            get;
            set;
        }
    
        public virtual string DisplayPartno
        {
            get;
            set;
        }
    
        public virtual bool ShowPrice
        {
            get;
            set;
        }
    
        public virtual bool PublishStatus
        {
            get;
            set;
        }
    
        public virtual decimal StorePrice
        {
            get;
            set;
        }
    
        public virtual string PriceSource
        {
            get;
            set;
        }
    
        public virtual string Status
        {
            get;
            set;
        }
    
        public virtual string ProductDesc
        {
            get;
            set;
        }
    
        public virtual string ProductFeatures
        {
            get;
            set;
        }
    
        public virtual string ExtendedDesc
        {
            get;
            set;
        }
    
        public virtual string ImageURL
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> PromotePrice
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> PromoteStart
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> PromoteEnd
        {
            get;
            set;
        }
    
        public virtual string PromoteMessage
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> PromoteMarkup
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EffectiveDate
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ExpiredDate
        {
            get;
            set;
        }
    
        public virtual Nullable<int> WarrantyYear
        {
            get;
            set;
        }
    
        public virtual string Id
        {
            get;
            set;
        }
    
        public virtual int MarketingStatus
        {
            get;
            set;
        }
    
        public virtual string StoreUrl
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
    	   		 if (StoreID == null) {
						 error_message.Add(new ErrorMessage("StoreID", "StoreID can not be Null "));
				}
		 if (SProductID == null) {
						 error_message.Add(new ErrorMessage("SProductID", "SProductID can not be Null "));
				}
		 if (DisplayPartno == null) {
						 error_message.Add(new ErrorMessage("DisplayPartno", "DisplayPartno can not be Null "));
				}
		 if (Status == null) {
						 error_message.Add(new ErrorMessage("Status", "Status can not be Null "));
				}
		 if (ProductDesc == null) {
						 error_message.Add(new ErrorMessage("ProductDesc", "ProductDesc can not be Null "));
				}
		 if (ProductFeatures == null) {
						 error_message.Add(new ErrorMessage("ProductFeatures", "ProductFeatures can not be Null "));
				}
		 if (Id == null) {
						 error_message.Add(new ErrorMessage("Id", "Id can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(SimpleProduct); // Get type pointer
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
