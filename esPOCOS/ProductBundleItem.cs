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
    public partial class ProductBundleItem
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int ProductBundleItemID
        {
            get;
            set;
        }
    
        public virtual string StoreID
        {
            get { return _storeID; }
            set
            {
                if (_storeID != value)
                {
                    if (Product_Bundle != null && Product_Bundle.StoreID != value)
                    {
                        Product_Bundle = null;
                    }
                    _storeID = value;
                }
            }
        }
        private string _storeID;
    
        public virtual string SProductID
        {
            get { return _sProductID; }
            set
            {
                if (_sProductID != value)
                {
                    if (Product_Bundle != null && Product_Bundle.SProductID != value)
                    {
                        Product_Bundle = null;
                    }
                    _sProductID = value;
                }
            }
        }
        private string _sProductID;
    
        public virtual string ItemSProductID
        {
            get;
            set;
        }
    
        public virtual int Qty
        {
            get;
            set;
        }
    
        public virtual Nullable<int> Sequence
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> Assembly
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
		 if (ItemSProductID == null) {
						 error_message.Add(new ErrorMessage("ItemSProductID", "ItemSProductID can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(ProductBundleItem); // Get type pointer
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
    
        public virtual Product_Bundle Product_Bundle
        {
            get { return _product_Bundle; }
            set
            {
                if (!ReferenceEquals(_product_Bundle, value))
                {
                    var previousValue = _product_Bundle;
                    _product_Bundle = value;
                    FixupProduct_Bundle(previousValue);
                }
            }
        }
        private Product_Bundle _product_Bundle;

        #endregion

        #region Association Fixup
    
        private void FixupProduct_Bundle(Product_Bundle previousValue)
        {
            if (previousValue != null && previousValue.ProductBundleItems.Contains(this))
            {
                previousValue.ProductBundleItems.Remove(this);
            }
    
            if (Product_Bundle != null)
            {
                if (!Product_Bundle.ProductBundleItems.Contains(this))
                {
                    Product_Bundle.ProductBundleItems.Add(this);
                }
                if (StoreID != Product_Bundle.StoreID)
                {
                    StoreID = Product_Bundle.StoreID;
                }
                if (SProductID != Product_Bundle.SProductID)
                {
                    SProductID = Product_Bundle.SProductID;
                }
            }
        }

        #endregion

    }
}