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
    public partial class MenuAdvertisement
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string TreeID
        {
            get;
            set;
        }
    
        public virtual int AdID
        {
            get { return _adID; }
            set
            {
                if (_adID != value)
                {
                    if (Advertisement != null && Advertisement.id != value)
                    {
                        Advertisement = null;
                    }
                    _adID = value;
                }
            }
        }
        private int _adID;
    
        public virtual string storeid
        {
            get { return _storeid; }
            set
            {
                if (_storeid != value)
                {
                    if (Advertisement != null && Advertisement.StoreID != value)
                    {
                        Advertisement = null;
                    }
                    _storeid = value;
                }
            }
        }
        private string _storeid;
    
        public virtual string type
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (TreeID == null) {
						 error_message.Add(new ErrorMessage("TreeID", "TreeID can not be Null "));
				}
		 if (storeid == null) {
						 error_message.Add(new ErrorMessage("storeid", "storeid can not be Null "));
				}
		 if (type == null) {
						 error_message.Add(new ErrorMessage("type", "type can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(MenuAdvertisement); // Get type pointer
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
    
        public virtual Advertisement Advertisement
        {
            get { return _advertisement; }
            set
            {
                if (!ReferenceEquals(_advertisement, value))
                {
                    var previousValue = _advertisement;
                    _advertisement = value;
                    FixupAdvertisement(previousValue);
                }
            }
        }
        private Advertisement _advertisement;

        #endregion

        #region Association Fixup
    
        private void FixupAdvertisement(Advertisement previousValue)
        {
            if (previousValue != null && previousValue.MenuAdvertisements.Contains(this))
            {
                previousValue.MenuAdvertisements.Remove(this);
            }
    
            if (Advertisement != null)
            {
                if (!Advertisement.MenuAdvertisements.Contains(this))
                {
                    Advertisement.MenuAdvertisements.Add(this);
                }
                if (AdID != Advertisement.id)
                {
                    AdID = Advertisement.id;
                }
                if (storeid != Advertisement.StoreID)
                {
                    storeid = Advertisement.StoreID;
                }
            }
        }

        #endregion

    }
}
