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
    public partial class LanguagePack
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
    
        public virtual string ObjectName
        {
            get;
            set;
        }
    
        public virtual int ObjectId
        {
            get;
            set;
        }
    
        public virtual string FieldName
        {
            get;
            set;
        }
    
        public virtual string Value
        {
            get;
            set;
        }
    
        public virtual int LanguageId
        {
            get { return _languageId; }
            set
            {
                if (_languageId != value)
                {
                    if (Language != null && Language.Id != value)
                    {
                        Language = null;
                    }
                    _languageId = value;
                }
            }
        }
        private int _languageId;

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
    	   		 if (ObjectName == null) {
						 error_message.Add(new ErrorMessage("ObjectName", "ObjectName can not be Null "));
				}
		 if (FieldName == null) {
						 error_message.Add(new ErrorMessage("FieldName", "FieldName can not be Null "));
				}
		 if (Value == null) {
						 error_message.Add(new ErrorMessage("Value", "Value can not be Null "));
				}

    	   
    	   if(error_message.Count>0) {
    	   	return false;
    	   }else {
    	    return true;
    	   }
    	
    	}
    	
    	/* public void Write()
        {
            Type type = typeof(LanguagePack); // Get type pointer
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
    
        public virtual Language Language
        {
            get { return _language; }
            set
            {
                if (!ReferenceEquals(_language, value))
                {
                    var previousValue = _language;
                    _language = value;
                    FixupLanguage(previousValue);
                }
            }
        }
        private Language _language;

        #endregion

        #region Association Fixup
    
        private void FixupLanguage(Language previousValue)
        {
            if (Language != null)
            {
                if (LanguageId != Language.Id)
                {
                    LanguageId = Language.Id;
                }
            }
        }

        #endregion

    }
}
