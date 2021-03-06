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
    public partial class Box
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int BoxID
        {
            get;
            set;
        }
    
        public virtual string StoreID
        {
            get;
            set;
        }
    
        public virtual decimal WidthINCH
        {
            get;
            set;
        }
    
        public virtual decimal LengthINCH
        {
            get;
            set;
        }
    
        public virtual decimal HighINCH
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> Default
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

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(Box); // Get type pointer
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
    
        public virtual ICollection<ProductBoxRule> ProductBoxRules
        {
            get
            {
                if (_productBoxRules == null)
                {
                    var newCollection = new FixupCollection<ProductBoxRule>();
                    newCollection.CollectionChanged += FixupProductBoxRules;
                    _productBoxRules = newCollection;
                }
                return _productBoxRules;
            }
            set
            {
                if (!ReferenceEquals(_productBoxRules, value))
                {
                    var previousValue = _productBoxRules as FixupCollection<ProductBoxRule>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProductBoxRules;
                    }
                    _productBoxRules = value;
                    var newValue = value as FixupCollection<ProductBoxRule>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProductBoxRules;
                    }
                }
            }
        }
        private ICollection<ProductBoxRule> _productBoxRules;

        #endregion

        #region Association Fixup
    
        private void FixupProductBoxRules(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProductBoxRule item in e.NewItems)
                {
                    item.Box = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProductBoxRule item in e.OldItems)
                {
                    if (ReferenceEquals(item.Box, this))
                    {
                        item.Box = null;
                    }
                }
            }
        }

        #endregion

    }
}
