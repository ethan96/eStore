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
    public partial class StoreProductBundleList
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int Id
        {
            get;
            set;
        }
    
        public virtual int StoreProductBundleId
        {
            get { return _storeProductBundleId; }
            set
            {
                if (_storeProductBundleId != value)
                {
                    if (StoreProductBundle != null && StoreProductBundle.Id != value)
                    {
                        StoreProductBundle = null;
                    }
                    _storeProductBundleId = value;
                }
            }
        }
        private int _storeProductBundleId;
    
        public virtual int StoreProductId
        {
            get { return _storeProductId; }
            set
            {
                if (_storeProductId != value)
                {
                    if (StoreProduct != null && StoreProduct.Id != value)
                    {
                        StoreProduct = null;
                    }
                    _storeProductId = value;
                }
            }
        }
        private int _storeProductId;
    
        public virtual bool IsPrimary
        {
            get;
            set;
        }
    
        public virtual bool IsDefault
        {
            get;
            set;
        }
    
        public virtual string GroupName
        {
            get;
            set;
        }
    
        public virtual bool IsMutuallyExclusive
        {
            get;
            set;
        }
    
        public virtual int OrdinalNo
        {
            get;
            set;
        }
    
        public virtual decimal BundlePricePerSet
        {
            get;
            set;
        }
    
        public virtual Nullable<int> QuantityPerSet
        {
            get;
            set;
        }
    
        public virtual Nullable<int> MaxSet
        {
            get;
            set;
        }
    
        public virtual System.DateTime StartDate
        {
            get;
            set;
        }
    
        public virtual System.DateTime ExpireDate
        {
            get;
            set;
        }
    
        public virtual string Note
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (GroupName == null) {
						 error_message.Add(new ErrorMessage("GroupName", "GroupName can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(StoreProductBundleList); // Get type pointer
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
    
        public virtual StoreProductBundle StoreProductBundle
        {
            get { return _storeProductBundle; }
            set
            {
                if (!ReferenceEquals(_storeProductBundle, value))
                {
                    var previousValue = _storeProductBundle;
                    _storeProductBundle = value;
                    FixupStoreProductBundle(previousValue);
                }
            }
        }
        private StoreProductBundle _storeProductBundle;
    
        public virtual StoreProduct StoreProduct
        {
            get { return _storeProduct; }
            set
            {
                if (!ReferenceEquals(_storeProduct, value))
                {
                    var previousValue = _storeProduct;
                    _storeProduct = value;
                    FixupStoreProduct(previousValue);
                }
            }
        }
        private StoreProduct _storeProduct;

        #endregion

        #region Association Fixup
    
        private void FixupStoreProductBundle(StoreProductBundle previousValue)
        {
            if (previousValue != null && previousValue.StoreProductBundleLists.Contains(this))
            {
                previousValue.StoreProductBundleLists.Remove(this);
            }
    
            if (StoreProductBundle != null)
            {
                if (!StoreProductBundle.StoreProductBundleLists.Contains(this))
                {
                    StoreProductBundle.StoreProductBundleLists.Add(this);
                }
                if (StoreProductBundleId != StoreProductBundle.Id)
                {
                    StoreProductBundleId = StoreProductBundle.Id;
                }
            }
        }
    
        private void FixupStoreProduct(StoreProduct previousValue)
        {
            if (previousValue != null && previousValue.StoreProductBundleLists.Contains(this))
            {
                previousValue.StoreProductBundleLists.Remove(this);
            }
    
            if (StoreProduct != null)
            {
                if (!StoreProduct.StoreProductBundleLists.Contains(this))
                {
                    StoreProduct.StoreProductBundleLists.Add(this);
                }
                if (StoreProductId != StoreProduct.Id)
                {
                    StoreProductId = StoreProduct.Id;
                }
            }
        }

        #endregion

    }
}