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
    public partial class StoreProduct
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
    
        public virtual int StoreId
        {
            get { return _storeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_storeId != value)
                    {
                        if (Store != null && Store.Id != value)
                        {
                            Store = null;
                        }
                        _storeId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _storeId;
    
        public virtual bool IsMB
        {
            get;
            set;
        }
    
        public virtual int ProductId
        {
            get;
            set;
        }
    
        public virtual int OrdinalNo
        {
            get;
            set;
        }
    
        public virtual bool IsSoldInBundleOnly
        {
            get;
            set;
        }
    
        public virtual System.DateTime StartFrom
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
    
        public virtual Nullable<int> ReplacementStoreProductId
        {
            get { return _replacementStoreProductId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_replacementStoreProductId != value)
                    {
                        if (StoreProduct1 != null && StoreProduct1.Id != value)
                        {
                            StoreProduct1 = null;
                        }
                        _replacementStoreProductId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _replacementStoreProductId;
    
        public virtual decimal Price
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> FreeShipping
        {
            get;
            set;
        }
    
        public virtual string ShipFrom
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> CallForPrice
        {
            get;
            set;
        }
    
        public virtual Nullable<int> LimitQty
        {
            get;
            set;
        }
    
        public virtual Nullable<decimal> PTDCost
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> SpecialShipment
        {
            get;
            set;
        }
    
        public virtual string MetaKeyword
        {
            get;
            set;
        }
    
        public virtual string MetaTitle
        {
            get;
            set;
        }
    
        public virtual string MetaDescription
        {
            get;
            set;
        }
    
        public virtual Nullable<bool> SpecialOrder
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
            Type type = typeof(StoreProduct); // Get type pointer
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
    
        public virtual ICollection<StoreDeal> StoreDeals
        {
            get
            {
                if (_storeDeals == null)
                {
                    var newCollection = new FixupCollection<StoreDeal>();
                    newCollection.CollectionChanged += FixupStoreDeals;
                    _storeDeals = newCollection;
                }
                return _storeDeals;
            }
            set
            {
                if (!ReferenceEquals(_storeDeals, value))
                {
                    var previousValue = _storeDeals as FixupCollection<StoreDeal>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreDeals;
                    }
                    _storeDeals = value;
                    var newValue = value as FixupCollection<StoreDeal>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreDeals;
                    }
                }
            }
        }
        private ICollection<StoreDeal> _storeDeals;
    
        public virtual ICollection<StoreProductAssociate> StoreProductAssociates
        {
            get
            {
                if (_storeProductAssociates == null)
                {
                    var newCollection = new FixupCollection<StoreProductAssociate>();
                    newCollection.CollectionChanged += FixupStoreProductAssociates;
                    _storeProductAssociates = newCollection;
                }
                return _storeProductAssociates;
            }
            set
            {
                if (!ReferenceEquals(_storeProductAssociates, value))
                {
                    var previousValue = _storeProductAssociates as FixupCollection<StoreProductAssociate>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreProductAssociates;
                    }
                    _storeProductAssociates = value;
                    var newValue = value as FixupCollection<StoreProductAssociate>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreProductAssociates;
                    }
                }
            }
        }
        private ICollection<StoreProductAssociate> _storeProductAssociates;
    
        public virtual ICollection<StoreProductAssociate> StoreProductAssociates1
        {
            get
            {
                if (_storeProductAssociates1 == null)
                {
                    var newCollection = new FixupCollection<StoreProductAssociate>();
                    newCollection.CollectionChanged += FixupStoreProductAssociates1;
                    _storeProductAssociates1 = newCollection;
                }
                return _storeProductAssociates1;
            }
            set
            {
                if (!ReferenceEquals(_storeProductAssociates1, value))
                {
                    var previousValue = _storeProductAssociates1 as FixupCollection<StoreProductAssociate>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreProductAssociates1;
                    }
                    _storeProductAssociates1 = value;
                    var newValue = value as FixupCollection<StoreProductAssociate>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreProductAssociates1;
                    }
                }
            }
        }
        private ICollection<StoreProductAssociate> _storeProductAssociates1;
    
        public virtual ICollection<StoreProductBundleList> StoreProductBundleLists
        {
            get
            {
                if (_storeProductBundleLists == null)
                {
                    var newCollection = new FixupCollection<StoreProductBundleList>();
                    newCollection.CollectionChanged += FixupStoreProductBundleLists;
                    _storeProductBundleLists = newCollection;
                }
                return _storeProductBundleLists;
            }
            set
            {
                if (!ReferenceEquals(_storeProductBundleLists, value))
                {
                    var previousValue = _storeProductBundleLists as FixupCollection<StoreProductBundleList>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreProductBundleLists;
                    }
                    _storeProductBundleLists = value;
                    var newValue = value as FixupCollection<StoreProductBundleList>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreProductBundleLists;
                    }
                }
            }
        }
        private ICollection<StoreProductBundleList> _storeProductBundleLists;
    
        public virtual ICollection<StoreProduct> StoreProducts1
        {
            get
            {
                if (_storeProducts1 == null)
                {
                    var newCollection = new FixupCollection<StoreProduct>();
                    newCollection.CollectionChanged += FixupStoreProducts1;
                    _storeProducts1 = newCollection;
                }
                return _storeProducts1;
            }
            set
            {
                if (!ReferenceEquals(_storeProducts1, value))
                {
                    var previousValue = _storeProducts1 as FixupCollection<StoreProduct>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStoreProducts1;
                    }
                    _storeProducts1 = value;
                    var newValue = value as FixupCollection<StoreProduct>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStoreProducts1;
                    }
                }
            }
        }
        private ICollection<StoreProduct> _storeProducts1;
    
        public virtual StoreProduct StoreProduct1
        {
            get { return _storeProduct1; }
            set
            {
                if (!ReferenceEquals(_storeProduct1, value))
                {
                    var previousValue = _storeProduct1;
                    _storeProduct1 = value;
                    FixupStoreProduct1(previousValue);
                }
            }
        }
        private StoreProduct _storeProduct1;
    
        public virtual PStore Store
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
        private PStore _store;
    
        public virtual ICollection<PStoreProduct> PStoreProducts
        {
            get
            {
                if (_pStoreProducts == null)
                {
                    var newCollection = new FixupCollection<PStoreProduct>();
                    newCollection.CollectionChanged += FixupPStoreProducts;
                    _pStoreProducts = newCollection;
                }
                return _pStoreProducts;
            }
            set
            {
                if (!ReferenceEquals(_pStoreProducts, value))
                {
                    var previousValue = _pStoreProducts as FixupCollection<PStoreProduct>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupPStoreProducts;
                    }
                    _pStoreProducts = value;
                    var newValue = value as FixupCollection<PStoreProduct>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupPStoreProducts;
                    }
                }
            }
        }
        private ICollection<PStoreProduct> _pStoreProducts;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupStoreProduct1(StoreProduct previousValue)
        {
            if (previousValue != null && previousValue.StoreProducts1.Contains(this))
            {
                previousValue.StoreProducts1.Remove(this);
            }
    
            if (StoreProduct1 != null)
            {
                if (!StoreProduct1.StoreProducts1.Contains(this))
                {
                    StoreProduct1.StoreProducts1.Add(this);
                }
                if (ReplacementStoreProductId != StoreProduct1.Id)
                {
                    ReplacementStoreProductId = StoreProduct1.Id;
                }
            }
            else if (!_settingFK)
            {
                ReplacementStoreProductId = null;
            }
        }
    
        private void FixupStore(PStore previousValue)
        {
            if (previousValue != null && previousValue.StoreProducts.Contains(this))
            {
                previousValue.StoreProducts.Remove(this);
            }
    
            if (Store != null)
            {
                if (!Store.StoreProducts.Contains(this))
                {
                    Store.StoreProducts.Add(this);
                }
                if (StoreId != Store.Id)
                {
                    StoreId = Store.Id;
                }
            }
        }
    
        private void FixupStoreDeals(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreDeal item in e.NewItems)
                {
                    item.StoreProductId = Id;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreDeal item in e.OldItems)
                {
                }
            }
        }
    
        private void FixupStoreProductAssociates(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreProductAssociate item in e.NewItems)
                {
                    item.StoreProduct = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreProductAssociate item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreProduct, this))
                    {
                        item.StoreProduct = null;
                    }
                }
            }
        }
    
        private void FixupStoreProductAssociates1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreProductAssociate item in e.NewItems)
                {
                    item.StoreProduct1 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreProductAssociate item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreProduct1, this))
                    {
                        item.StoreProduct1 = null;
                    }
                }
            }
        }
    
        private void FixupStoreProductBundleLists(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreProductBundleList item in e.NewItems)
                {
                    item.StoreProduct = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreProductBundleList item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreProduct, this))
                    {
                        item.StoreProduct = null;
                    }
                }
            }
        }
    
        private void FixupStoreProducts1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StoreProduct item in e.NewItems)
                {
                    item.StoreProduct1 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StoreProduct item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreProduct1, this))
                    {
                        item.StoreProduct1 = null;
                    }
                }
            }
        }
    
        private void FixupPStoreProducts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PStoreProduct item in e.NewItems)
                {
                    item.StoreProduct = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PStoreProduct item in e.OldItems)
                {
                    if (ReferenceEquals(item.StoreProduct, this))
                    {
                        item.StoreProduct = null;
                    }
                }
            }
        }

        #endregion

    }
}
