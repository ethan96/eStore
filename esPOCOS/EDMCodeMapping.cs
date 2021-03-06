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
    public partial class EDMCodeMapping
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual string EDMName
        {
            get;
            set;
        }
    
        public virtual string EdmCode
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (EDMName == null) {
						 error_message.Add(new ErrorMessage("EDMName", "EDMName can not be Null "));
				}
		 if (EdmCode == null) {
						 error_message.Add(new ErrorMessage("EdmCode", "EdmCode can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(EDMCodeMapping); // Get type pointer
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
    
        public virtual ICollection<CampaignMailLogByEDM> CampaignMailLogByEDMs
        {
            get
            {
                if (_campaignMailLogByEDMs == null)
                {
                    var newCollection = new FixupCollection<CampaignMailLogByEDM>();
                    newCollection.CollectionChanged += FixupCampaignMailLogByEDMs;
                    _campaignMailLogByEDMs = newCollection;
                }
                return _campaignMailLogByEDMs;
            }
            set
            {
                if (!ReferenceEquals(_campaignMailLogByEDMs, value))
                {
                    var previousValue = _campaignMailLogByEDMs as FixupCollection<CampaignMailLogByEDM>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCampaignMailLogByEDMs;
                    }
                    _campaignMailLogByEDMs = value;
                    var newValue = value as FixupCollection<CampaignMailLogByEDM>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCampaignMailLogByEDMs;
                    }
                }
            }
        }
        private ICollection<CampaignMailLogByEDM> _campaignMailLogByEDMs;

        #endregion

        #region Association Fixup
    
        private void FixupCampaignMailLogByEDMs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CampaignMailLogByEDM item in e.NewItems)
                {
                    item.EDMCodeMapping = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CampaignMailLogByEDM item in e.OldItems)
                {
                    if (ReferenceEquals(item.EDMCodeMapping, this))
                    {
                        item.EDMCodeMapping = null;
                    }
                }
            }
        }

        #endregion

    }
}
