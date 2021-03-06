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
    public partial class ProductResource
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int ResourceID
        {
            get;
            set;
        }
    
        public virtual string ResourceName
        {
            get;
            set;
        }
    
        public virtual string ResourceURL
        {
            get;
            set;
        }
    
        public virtual string ResourceType
        {
            get;
            set;
        }
    
        public virtual string SProductID
        {
            get { return _sProductID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_sProductID != value)
                    {
                        if (Part != null && Part.SProductID != value)
                        {
                            var previousValue = _part;
                            _part = null;
                            FixupPart(previousValue, skipKeys: true);
                        }
                        _sProductID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _sProductID;
    
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
                        if (Part != null && Part.StoreID != value)
                        {
                            var previousValue = _part;
                            _part = null;
                            FixupPart(previousValue, skipKeys: true);
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
    
        public virtual Nullable<bool> IsLocalResource
        {
            get;
            set;
        }
    
        public virtual string ResourceStatus
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
            Type type = typeof(ProductResource); // Get type pointer
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
    
        public virtual Part Part
        {
            get { return _part; }
            set
            {
                if (!ReferenceEquals(_part, value))
                {
                    var previousValue = _part;
                    _part = value;
                    FixupPart(previousValue);
                }
            }
        }
        private Part _part;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupPart(Part previousValue, bool skipKeys = false)
        {
            if (previousValue != null && previousValue.ProductResources.Contains(this))
            {
                previousValue.ProductResources.Remove(this);
            }
    
            if (Part != null)
            {
                if (!Part.ProductResources.Contains(this))
                {
                    Part.ProductResources.Add(this);
                }
                if (StoreID != Part.StoreID)
                {
                    StoreID = Part.StoreID;
                }
                if (SProductID != Part.SProductID)
                {
                    SProductID = Part.SProductID;
                }
            }
            else if (!_settingFK && !skipKeys)
            {
                StoreID = null;
                SProductID = null;
            }
        }

        #endregion

    }
}
