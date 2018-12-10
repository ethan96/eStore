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
    public partial class RuleSet
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int RuleSetID
        {
            get;
            set;
        }
    
        public virtual string CreatedBy
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
    	   
    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(RuleSet); // Get type pointer
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
    
        public virtual ICollection<RuleSetDetail> RuleSetDetails
        {
            get
            {
                if (_ruleSetDetails == null)
                {
                    var newCollection = new FixupCollection<RuleSetDetail>();
                    newCollection.CollectionChanged += FixupRuleSetDetails;
                    _ruleSetDetails = newCollection;
                }
                return _ruleSetDetails;
            }
            set
            {
                if (!ReferenceEquals(_ruleSetDetails, value))
                {
                    var previousValue = _ruleSetDetails as FixupCollection<RuleSetDetail>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupRuleSetDetails;
                    }
                    _ruleSetDetails = value;
                    var newValue = value as FixupCollection<RuleSetDetail>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupRuleSetDetails;
                    }
                }
            }
        }
        private ICollection<RuleSetDetail> _ruleSetDetails;
    
        public virtual ICollection<Menu> Menus
        {
            get
            {
                if (_menus == null)
                {
                    var newCollection = new FixupCollection<Menu>();
                    newCollection.CollectionChanged += FixupMenus;
                    _menus = newCollection;
                }
                return _menus;
            }
            set
            {
                if (!ReferenceEquals(_menus, value))
                {
                    var previousValue = _menus as FixupCollection<Menu>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupMenus;
                    }
                    _menus = value;
                    var newValue = value as FixupCollection<Menu>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMenus;
                    }
                }
            }
        }
        private ICollection<Menu> _menus;
    
        public virtual ICollection<ProductCategory> ProductCategories
        {
            get
            {
                if (_productCategories == null)
                {
                    var newCollection = new FixupCollection<ProductCategory>();
                    newCollection.CollectionChanged += FixupProductCategories;
                    _productCategories = newCollection;
                }
                return _productCategories;
            }
            set
            {
                if (!ReferenceEquals(_productCategories, value))
                {
                    var previousValue = _productCategories as FixupCollection<ProductCategory>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupProductCategories;
                    }
                    _productCategories = value;
                    var newValue = value as FixupCollection<ProductCategory>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupProductCategories;
                    }
                }
            }
        }
        private ICollection<ProductCategory> _productCategories;

        #endregion

        #region Association Fixup
    
        private void FixupRuleSetDetails(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (RuleSetDetail item in e.NewItems)
                {
                    item.RuleSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (RuleSetDetail item in e.OldItems)
                {
                    if (ReferenceEquals(item.RuleSet, this))
                    {
                        item.RuleSet = null;
                    }
                }
            }
        }
    
        private void FixupMenus(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Menu item in e.NewItems)
                {
                    item.RuleSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Menu item in e.OldItems)
                {
                    if (ReferenceEquals(item.RuleSet, this))
                    {
                        item.RuleSet = null;
                    }
                }
            }
        }
    
        private void FixupProductCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ProductCategory item in e.NewItems)
                {
                    item.RuleSet = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProductCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.RuleSet, this))
                    {
                        item.RuleSet = null;
                    }
                }
            }
        }

        #endregion

    }
}