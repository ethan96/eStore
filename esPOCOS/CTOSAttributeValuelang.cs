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
    public partial class CTOSAttributeValuelang
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int AttrValue_id
        {
            get { return _attrValue_id; }
            set
            {
                if (_attrValue_id != value)
                {
                    if (CTOSAttributeValue != null && CTOSAttributeValue.AttrValue_id != value)
                    {
                        CTOSAttributeValue = null;
                    }
                    _attrValue_id = value;
                }
            }
        }
        private int _attrValue_id;
    
        public virtual string LangID
        {
            get;
            set;
        }
    
        public virtual string LangValue
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (LangID == null) {
						 error_message.Add(new ErrorMessage("LangID", "LangID can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(CTOSAttributeValuelang); // Get type pointer
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
    
        public virtual CTOSAttributeValue CTOSAttributeValue
        {
            get { return _cTOSAttributeValue; }
            set
            {
                if (!ReferenceEquals(_cTOSAttributeValue, value))
                {
                    var previousValue = _cTOSAttributeValue;
                    _cTOSAttributeValue = value;
                    FixupCTOSAttributeValue(previousValue);
                }
            }
        }
        private CTOSAttributeValue _cTOSAttributeValue;

        #endregion

        #region Association Fixup
    
        private void FixupCTOSAttributeValue(CTOSAttributeValue previousValue)
        {
            if (previousValue != null && previousValue.CTOSAttributeValuelangs.Contains(this))
            {
                previousValue.CTOSAttributeValuelangs.Remove(this);
            }
    
            if (CTOSAttributeValue != null)
            {
                if (!CTOSAttributeValue.CTOSAttributeValuelangs.Contains(this))
                {
                    CTOSAttributeValue.CTOSAttributeValuelangs.Add(this);
                }
                if (AttrValue_id != CTOSAttributeValue.AttrValue_id)
                {
                    AttrValue_id = CTOSAttributeValue.AttrValue_id;
                }
            }
        }

        #endregion

    }
}
