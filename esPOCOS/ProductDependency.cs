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
    public partial class ProductDependency
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int DependencyID
        {
            get;
            set;
        }
    
        public virtual string SProductID
        {
            get { return _sProductID; }
            set
            {
                if (_sProductID != value)
                {
                    if (ParentPart != null && ParentPart.SProductID != value)
                    {
                        ParentPart = null;
                    }
                    _sProductID = value;
                }
            }
        }
        private string _sProductID;
    
        public virtual string DependentProductID
        {
            get { return _dependentProductID; }
            set
            {
                if (_dependentProductID != value)
                {
                    if (PartDependency != null && PartDependency.SProductID != value)
                    {
                        PartDependency = null;
                    }
                    _dependentProductID = value;
                }
            }
        }
        private string _dependentProductID;
    
        public virtual string StoreID
        {
            get { return _storeID; }
            set
            {
                if (_storeID != value)
                {
                    if (ParentPart != null && ParentPart.StoreID != value)
                    {
                        ParentPart = null;
                    }
                    if (PartDependency != null && PartDependency.StoreID != value)
                    {
                        PartDependency = null;
                    }
                    _storeID = value;
                }
            }
        }
        private string _storeID;
    
        public virtual System.DateTime CreatedDate
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (SProductID == null) {
						 error_message.Add(new ErrorMessage("SProductID", "SProductID can not be Null "));
				}
		 if (DependentProductID == null) {
						 error_message.Add(new ErrorMessage("DependentProductID", "DependentProductID can not be Null "));
				}
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
            Type type = typeof(ProductDependency); // Get type pointer
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
    
        public virtual Part ParentPart
        {
            get { return _parentPart; }
            set
            {
                if (!ReferenceEquals(_parentPart, value))
                {
                    var previousValue = _parentPart;
                    _parentPart = value;
                    FixupParentPart(previousValue);
                }
            }
        }
        private Part _parentPart;
    
        public virtual Part PartDependency
        {
            get { return _partDependency; }
            set
            {
                if (!ReferenceEquals(_partDependency, value))
                {
                    var previousValue = _partDependency;
                    _partDependency = value;
                    FixupPartDependency(previousValue);
                }
            }
        }
        private Part _partDependency;

        #endregion

        #region Association Fixup
    
        private void FixupParentPart(Part previousValue)
        {
            if (previousValue != null && previousValue.DependencytoProducts.Contains(this))
            {
                previousValue.DependencytoProducts.Remove(this);
            }
    
            if (ParentPart != null)
            {
                if (!ParentPart.DependencytoProducts.Contains(this))
                {
                    ParentPart.DependencytoProducts.Add(this);
                }
                if (StoreID != ParentPart.StoreID)
                {
                    StoreID = ParentPart.StoreID;
                }
                if (SProductID != ParentPart.SProductID)
                {
                    SProductID = ParentPart.SProductID;
                }
            }
        }
    
        private void FixupPartDependency(Part previousValue)
        {
            if (previousValue != null && previousValue.ProductDependencies.Contains(this))
            {
                previousValue.ProductDependencies.Remove(this);
            }
    
            if (PartDependency != null)
            {
                if (!PartDependency.ProductDependencies.Contains(this))
                {
                    PartDependency.ProductDependencies.Add(this);
                }
                if (StoreID != PartDependency.StoreID)
                {
                    StoreID = PartDependency.StoreID;
                }
                if (DependentProductID != PartDependency.SProductID)
                {
                    DependentProductID = PartDependency.SProductID;
                }
            }
        }

        #endregion

    }
}