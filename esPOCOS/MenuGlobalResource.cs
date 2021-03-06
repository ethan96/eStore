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
    public partial class MenuGlobalResource
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
                        if (Menu != null && Menu.StoreID != value)
                        {
                            var previousValue = _menu;
                            _menu = null;
                            FixupMenu(previousValue, skipKeys: true);
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
    
        public virtual Nullable<int> MenuId
        {
            get { return _menuId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_menuId != value)
                    {
                        if (Menu != null && Menu.MenuID != value)
                        {
                            var previousValue = _menu;
                            _menu = null;
                            FixupMenu(previousValue, skipKeys: true);
                        }
                        _menuId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _menuId;
    
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
            Type type = typeof(MenuGlobalResource); // Get type pointer
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
    
        public virtual Menu Menu
        {
            get { return _menu; }
            set
            {
                if (!ReferenceEquals(_menu, value))
                {
                    var previousValue = _menu;
                    _menu = value;
                    FixupMenu(previousValue);
                }
            }
        }
        private Menu _menu;

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
    
        private void FixupMenu(Menu previousValue, bool skipKeys = false)
        {
            if (previousValue != null && previousValue.MenuGlobalResources.Contains(this))
            {
                previousValue.MenuGlobalResources.Remove(this);
            }
    
            if (Menu != null)
            {
                if (!Menu.MenuGlobalResources.Contains(this))
                {
                    Menu.MenuGlobalResources.Add(this);
                }
                if (MenuId != Menu.MenuID)
                {
                    MenuId = Menu.MenuID;
                }
                if (StoreId != Menu.StoreID)
                {
                    StoreId = Menu.StoreID;
                }
            }
            else if (!_settingFK && !skipKeys)
            {
                MenuId = null;
                StoreId = null;
            }
        }

        #endregion

    }
}
