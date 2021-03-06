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
    public partial class PRODUCT_LOGISTICS_NEW
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string PRODUCT_ID
        {
            get;
            set;
        }
    
        public virtual string PART_NO
        {
            get;
            set;
        }
    
        public virtual string PRODUCT_DESC
        {
            get;
            set;
        }
    
        public virtual string STATUS
        {
            get;
            set;
        }
    
        public virtual string ORG_ID
        {
            get;
            set;
        }
    
        public virtual string VERSION
        {
            get;
            set;
        }
    
        public virtual string UOM
        {
            get;
            set;
        }
    
        public virtual string MODEL_NO
        {
            get;
            set;
        }
    
        public virtual string CERTIFICATE
        {
            get;
            set;
        }
    
        public virtual string PRODUCT_LINE
        {
            get;
            set;
        }
    
        public virtual string PRODUCT_GROUP
        {
            get;
            set;
        }
    
        public virtual string TUMBNAIL_IMAGE_ID
        {
            get;
            set;
        }
    
        public virtual string IMAGE_ID
        {
            get;
            set;
        }
    
        public virtual string PRODUCT_TYPE
        {
            get;
            set;
        }
    
        public virtual string ONLINE_PUBLISH
        {
            get;
            set;
        }
    
        public virtual string EXTENTED_DESC
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> NEW_PRODUCT_DATE
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> SHIP_WEIGHT
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> NET_WEIGHT
        {
            get;
            set;
        }
    
        public virtual string PRODUCT_SITE
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> LAST_UPDATED
        {
            get;
            set;
        }
    
        public virtual string LAST_UPDATED_BY
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CREATED_DATE
        {
            get;
            set;
        }
    
        public virtual string CREATED_BY
        {
            get;
            set;
        }
    
        public virtual string Controller_Code
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Cost
        {
            get;
            set;
        }
    
        public virtual string Currency
        {
            get;
            set;
        }
    
        public virtual string RoHS_Status
        {
            get;
            set;
        }
    
        public virtual string ABC_Ind
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> Region_Dim_Weight
        {
            get;
            set;
        }
    
        public virtual string Product_Division
        {
            get;
            set;
        }
    
        public virtual string Dimension
        {
            get;
            set;
        }

        #endregion
        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (PART_NO == null) {
						 error_message.Add(new ErrorMessage("PART_NO", "PART_NO can not be Null "));
				}
		 if (ORG_ID == null) {
						 error_message.Add(new ErrorMessage("ORG_ID", "ORG_ID can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(PRODUCT_LOGISTICS_NEW); // Get type pointer
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
