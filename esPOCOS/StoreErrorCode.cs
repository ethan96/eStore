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
    public partial class StoreErrorCode
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
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
                    if (Store != null && Store.StoreID != value)
                    {
                        Store = null;
                    }
                    _storeID = value;
                }
            }
        }
        private string _storeID;
    
        public virtual string ErrorCode
        {
            get;
            set;
        }
    
        public virtual string DefaultMessage
        {
            get;
            set;
        }
    
        public virtual string UserActionMessage
        {
            get;
            set;
        }
    
        public virtual string Status
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
		 if (ErrorCode == null) {
						 error_message.Add(new ErrorMessage("ErrorCode", "ErrorCode can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(StoreErrorCode); // Get type pointer
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
    
        public virtual Store Store
        {
            get { return _store; }
            set
            {
                if (!ReferenceEquals(_store, value))
                {
                    var previousValue = _store;
                    _store = value;
                    FixupStore(previousValue);
                }
            }
        }
        private Store _store;
    
        public virtual ICollection<StoreErrorCodeGlobalResource> StoreErrorCodeGlobalResources
        {
            get
            {
                if (_storeErrorCodeGlobalResources == null)
                {
                    var newCollection = new FixupCollection<StoreErrorCodeGlobalResource>();
                    newCollection.CollectionChanged += FixupStoreErrorCodeGlobalResources;
                    _storeErrorCodeGlobalResources = newCollection;
                }
                return _storeErrorCodeGlobalResources;
            }
            set
            {
                if (!ReferenceEquals(_storeErrorCodeGlobalResources, value))
                {
                    var previousValue = _storeErrorCodeGlobalResources as FixupCollection<StoreErrorCodeGlobalResource>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreErrorCodeGlobalResources;
                    }
                    _storeErrorCodeGlobalResources = value;
                    var newValue = value as FixupCollection<StoreErrorCodeGlobalResource>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreErrorCodeGlobalResources;
                    }
                }
            }
        }
        private ICollection<StoreErrorCodeGlobalResource> _storeErrorCodeGlobalResources;

        #endregion

        #region Association Fixup
    
        private void FixupStore(Store previousValue)
        {
            if (previousValue != null && previousValue.StoreErrorCodes.Contains(this))
            {
                previousValue.StoreErrorCodes.Remove(this);
            }
    
            if (Store != null)
            {
                if (!Store.StoreErrorCodes.Contains(this))
                {
                    Store.StoreErrorCodes.Add(this);
                }
                if (StoreID != Store.StoreID)
                {
                    StoreID = Store.StoreID;
                }
            }
        }
    
        private void FixupStoreErrorCodeGlobalResources(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreErrorCodeGlobalResource item in e.NewItems)
                {
                    item.StoreErrorCode = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreErrorCodeGlobalResource item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreErrorCode, this))
                    {
                        item.StoreErrorCode = null;
                    }
                }
            }
        }

        #endregion

    }
}