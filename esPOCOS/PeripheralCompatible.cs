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
    public partial class PeripheralCompatible
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int PeripheralID
        {
            get { return _peripheralID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_peripheralID != value)
                    {
                        if (PeripheralProduct != null && PeripheralProduct.PeripheralID != value)
                        {
                            PeripheralProduct = null;
                        }
                        _peripheralID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private int _peripheralID;
    
        public virtual string PartNo
        {
            get { return _partNo; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_partNo != value)
                    {
                        if (Part != null && Part.SProductID != value)
                        {
                            var previousValue = _part;
                            _part = null;
                            FixupPart(previousValue, skipKeys: true);
                        }
                        _partNo = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _partNo;
    
        public virtual Nullable<System.DateTime> CreateDate
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

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (PartNo == null) {
						 error_message.Add(new ErrorMessage("PartNo", "PartNo can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(PeripheralCompatible); // Get type pointer
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
    
        public virtual PeripheralProduct PeripheralProduct
        {
            get { return _peripheralProduct; }
            set
            {
                if (!ReferenceEquals(_peripheralProduct, value))
                {
                    var previousValue = _peripheralProduct;
                    _peripheralProduct = value;
                    FixupPeripheralProduct(previousValue);
                }
            }
        }
        private PeripheralProduct _peripheralProduct;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupPart(Part previousValue, bool skipKeys = false)
        {
            if (previousValue != null && previousValue.PeripheralCompatibles.Contains(this))
            {
                previousValue.PeripheralCompatibles.Remove(this);
            }
    
            if (Part != null)
            {
                if (!Part.PeripheralCompatibles.Contains(this))
                {
                    Part.PeripheralCompatibles.Add(this);
                }
                if (StoreID != Part.StoreID)
                {
                    StoreID = Part.StoreID;
                }
                if (PartNo != Part.SProductID)
                {
                    PartNo = Part.SProductID;
                }
            }
            else if (!_settingFK && !skipKeys)
            {
                StoreID = null;
            }
        }
    
        private void FixupPeripheralProduct(PeripheralProduct previousValue)
        {
            if (previousValue != null && previousValue.PeripheralCompatibles.Contains(this))
            {
                previousValue.PeripheralCompatibles.Remove(this);
            }
    
            if (PeripheralProduct != null)
            {
                if (!PeripheralProduct.PeripheralCompatibles.Contains(this))
                {
                    PeripheralProduct.PeripheralCompatibles.Add(this);
                }
                if (PeripheralID != PeripheralProduct.PeripheralID)
                {
                    PeripheralID = PeripheralProduct.PeripheralID;
                }
            }
        }

        #endregion

    }
}