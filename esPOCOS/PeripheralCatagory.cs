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
    public partial class PeripheralCatagory
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int catagoryID
        {
            get;
            set;
        }
    
        public virtual string name
        {
            get;
            set;
        }
    
        public virtual int seq
        {
            get;
            set;
        }
    
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
                        if (Store != null && Store.StoreID != value)
                        {
                            Store = null;
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
    
        public virtual string CategoryClassification
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
            Type type = typeof(PeripheralCatagory); // Get type pointer
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
    
        public virtual ICollection<PeripheralsubCatagory> PeripheralsubCatagories
        {
            get
            {
                if (_peripheralsubCatagories == null)
                {
                    var newCollection = new FixupCollection<PeripheralsubCatagory>();
                    newCollection.CollectionChanged += FixupPeripheralsubCatagories;
                    _peripheralsubCatagories = newCollection;
                }
                return _peripheralsubCatagories;
            }
            set
            {
                if (!ReferenceEquals(_peripheralsubCatagories, value))
                {
                    var previousValue = _peripheralsubCatagories as FixupCollection<PeripheralsubCatagory>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPeripheralsubCatagories;
                    }
                    _peripheralsubCatagories = value;
                    var newValue = value as FixupCollection<PeripheralsubCatagory>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPeripheralsubCatagories;
                    }
                }
            }
        }
        private ICollection<PeripheralsubCatagory> _peripheralsubCatagories;
    
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

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupStore(Store previousValue)
        {
            if (Store != null)
            {
                if (StoreID != Store.StoreID)
                {
                    StoreID = Store.StoreID;
                }
            }
            else if (!_settingFK)
            {
                StoreID = null;
            }
        }
    
        private void FixupPeripheralsubCatagories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PeripheralsubCatagory item in e.NewItems)
                {
                    item.PeripheralCatagory = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PeripheralsubCatagory item in e.OldItems)
                {
                    if (ReferenceEquals(item.PeripheralCatagory, this))
                    {
                        item.PeripheralCatagory = null;
                    }
                }
            }
        }

        #endregion

    }
}