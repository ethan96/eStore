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
    public partial class PackingList
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
                    if (Cart != null && Cart.StoreID != value)
                    {
                        Cart = null;
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
                    if (Cart != null && Cart.CartID != value)
                    {
                        Cart = null;
                    }
                    _cartID = value;
                }
            }
        }
        private string _cartID;
    
        public virtual int PackingListID
        {
            get;
            set;
        }
    
        public virtual System.DateTime CreatedDate
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
            Type type = typeof(PackingList); // Get type pointer
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
    
        public virtual ICollection<PackagingBox> PackagingBoxes
        {
            get
            {
                if (_packagingBoxes == null)
                {
                    var newCollection = new FixupCollection<PackagingBox>();
                    newCollection.CollectionChanged += FixupPackagingBoxes;
                    _packagingBoxes = newCollection;
                }
                return _packagingBoxes;
            }
            set
            {
                if (!ReferenceEquals(_packagingBoxes, value))
                {
                    var previousValue = _packagingBoxes as FixupCollection<PackagingBox>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPackagingBoxes;
                    }
                    _packagingBoxes = value;
                    var newValue = value as FixupCollection<PackagingBox>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPackagingBoxes;
                    }
                }
            }
        }
        private ICollection<PackagingBox> _packagingBoxes;
    
        public virtual Cart Cart
        {
            get { return _cart; }
            set
            {
                if (!ReferenceEquals(_cart, value))
                {
                    var previousValue = _cart;
                    _cart = value;
                    FixupCart(previousValue);
                }
            }
        }
        private Cart _cart;

        #endregion

        #region Association Fixup
    
        private void FixupCart(Cart previousValue)
        {
            if (previousValue != null && previousValue.PackingLists.Contains(this))
            {
                previousValue.PackingLists.Remove(this);
            }
    
            if (Cart != null)
            {
                if (!Cart.PackingLists.Contains(this))
                {
                    Cart.PackingLists.Add(this);
                }
                if (CartID != Cart.CartID)
                {
                    CartID = Cart.CartID;
                }
                if (StoreID != Cart.StoreID)
                {
                    StoreID = Cart.StoreID;
                }
            }
        }
    
        private void FixupPackagingBoxes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PackagingBox item in e.NewItems)
                {
                    item.PackingList = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PackagingBox item in e.OldItems)
                {
                    if (ReferenceEquals(item.PackingList, this))
                    {
                        item.PackingList = null;
                    }
                }
            }
        }

        #endregion

    }
}
