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
    public partial class TranslationGlobalResource
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
    
        public virtual string StoreId
        {
            get { return _storeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_storeId != value)
                    {
                        if (Translation != null && Translation.StoreID != value)
                        {
                            var previousValue = _translation;
                            _translation = null;
                            FixupTranslation(previousValue, skipKeys: true);
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
        private string _storeId;
    
        public virtual string Key
        {
            get { return _key; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_key != value)
                    {
                        if (Translation != null && Translation.Key != value)
                        {
                            var previousValue = _translation;
                            _translation = null;
                            FixupTranslation(previousValue, skipKeys: true);
                        }
                        _key = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private string _key;
    
        public virtual Nullable<int> LanguageId
        {
            get { return _languageId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_languageId != value)
                    {
                        if (Language != null && Language.Id != value)
                        {
                            Language = null;
                        }
                        _languageId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _languageId;
    
        public virtual string LocalName
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreateDate
        {
            get;
            set;
        }
    
        public virtual string CreateBy
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
            Type type = typeof(TranslationGlobalResource); // Get type pointer
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
    
        public virtual Translation Translation
        {
            get { return _translation; }
            set
            {
                if (!ReferenceEquals(_translation, value))
                {
                    var previousValue = _translation;
                    _translation = value;
                    FixupTranslation(previousValue);
                }
            }
        }
        private Translation _translation;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupLanguage(Language previousValue)
        {
            if (Language != null)
            {
                if (LanguageId != Language.Id)
                {
                    LanguageId = Language.Id;
                }
            }
            else if (!_settingFK)
            {
                LanguageId = null;
            }
        }
    
        private void FixupTranslation(Translation previousValue, bool skipKeys = false)
        {
            if (previousValue != null && previousValue.TranslationGlobalResources.Contains(this))
            {
                previousValue.TranslationGlobalResources.Remove(this);
            }
    
            if (Translation != null)
            {
                if (!Translation.TranslationGlobalResources.Contains(this))
                {
                    Translation.TranslationGlobalResources.Add(this);
                }
                if (StoreId != Translation.StoreID)
                {
                    StoreId = Translation.StoreID;
                }
                if (Key != Translation.Key)
                {
                    Key = Translation.Key;
                }
            }
            else if (!_settingFK && !skipKeys)
            {
                StoreId = null;
                Key = null;
            }
        }

        #endregion

    }
}
