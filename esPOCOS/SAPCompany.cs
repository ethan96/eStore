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
    public partial class SAPCompany
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string CompanyID
        {
            get;
            set;
        }
    
        public virtual string OrgID
        {
            get;
            set;
        }
    
        public virtual string ParentCompanyID
        {
            get;
            set;
        }
    
        public virtual string CompanyName
        {
            get;
            set;
        }
    
        public virtual string Address
        {
            get;
            set;
        }
    
        public virtual string FaxNo
        {
            get;
            set;
        }
    
        public virtual string TelNo
        {
            get;
            set;
        }
    
        public virtual string CompanyType
        {
            get;
            set;
        }
    
        public virtual string PriceClass
        {
            get;
            set;
        }
    
        public virtual string PtradePriceClass
        {
            get;
            set;
        }
    
        public virtual string Currency
        {
            get;
            set;
        }
    
        public virtual string Country
        {
            get;
            set;
        }
    
        public virtual string Region
        {
            get;
            set;
        }
    
        public virtual string ZipCode
        {
            get;
            set;
        }
    
        public virtual string City
        {
            get;
            set;
        }
    
        public virtual string Attention
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> CreditLimit
        {
            get;
            set;
        }
    
        public virtual string CreditTerm
        {
            get;
            set;
        }
    
        public virtual string ShipVia
        {
            get;
            set;
        }
    
        public virtual string Url
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> LastUpdated
        {
            get;
            set;
        }
    
        public virtual string UpdatedBy
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreatedDate
        {
            get;
            set;
        }
    
        public virtual string CreatedBy
        {
            get;
            set;
        }
    
        public virtual string CompanyPriceType
        {
            get;
            set;
        }
    
        public virtual string SalesUserID
        {
            get;
            set;
        }
    
        public virtual string ShipCondition
        {
            get;
            set;
        }
    
        public virtual string Attribute4
        {
            get;
            set;
        }
    
        public virtual string SalesOffice
        {
            get;
            set;
        }
    
        public virtual string SalesGroup
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (CompanyID == null) {
						 error_message.Add(new ErrorMessage("CompanyID", "CompanyID can not be Null "));
				}
		 if (OrgID == null) {
						 error_message.Add(new ErrorMessage("OrgID", "OrgID can not be Null "));
				}
		 if (ParentCompanyID == null) {
						 error_message.Add(new ErrorMessage("ParentCompanyID", "ParentCompanyID can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(SAPCompany); // Get type pointer
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
