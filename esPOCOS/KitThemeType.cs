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
    public partial class KitThemeType
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
    
        public virtual Nullable<int> ThemeId
        {
            get { return _themeId; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_themeId != value)
                    {
                        if (KitTheme != null && KitTheme.Id != value)
                        {
                            KitTheme = null;
                        }
                        _themeId = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _themeId;
    
        public virtual string Title
        {
            get;
            set;
        }
    
        public virtual string HyperLink
        {
            get;
            set;
        }
    
        public virtual string Target
        {
            get;
            set;
        }
    
        public virtual string KeyWords
        {
            get;
            set;
        }
    
        public virtual string MetaDescription
        {
            get;
            set;
        }
    
        public virtual Nullable<int> Seq
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
            Type type = typeof(KitThemeType); // Get type pointer
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
    
        public virtual KitTheme KitTheme
        {
            get { return _kitTheme; }
            set
            {
                if (!ReferenceEquals(_kitTheme, value))
                {
                    var previousValue = _kitTheme;
                    _kitTheme = value;
                    FixupKitTheme(previousValue);
                }
            }
        }
        private KitTheme _kitTheme;
    
        public virtual ICollection<KitThemeMapping> KitThemeMappings
        {
            get
            {
                if (_kitThemeMappings == null)
                {
                    var newCollection = new FixupCollection<KitThemeMapping>();
                    newCollection.CollectionChanged += FixupKitThemeMappings;
                    _kitThemeMappings = newCollection;
                }
                return _kitThemeMappings;
            }
            set
            {
                if (!ReferenceEquals(_kitThemeMappings, value))
                {
                    var previousValue = _kitThemeMappings as FixupCollection<KitThemeMapping>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupKitThemeMappings;
                    }
                    _kitThemeMappings = value;
                    var newValue = value as FixupCollection<KitThemeMapping>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupKitThemeMappings;
                    }
                }
            }
        }
        private ICollection<KitThemeMapping> _kitThemeMappings;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupKitTheme(KitTheme previousValue)
        {
            if (previousValue != null && previousValue.KitThemeTypes.Contains(this))
            {
                previousValue.KitThemeTypes.Remove(this);
            }
    
            if (KitTheme != null)
            {
                if (!KitTheme.KitThemeTypes.Contains(this))
                {
                    KitTheme.KitThemeTypes.Add(this);
                }
                if (ThemeId != KitTheme.Id)
                {
                    ThemeId = KitTheme.Id;
                }
            }
            else if (!_settingFK)
            {
                ThemeId = null;
            }
        }
    
        private void FixupKitThemeMappings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (KitThemeMapping item in e.NewItems)
                {
                    item.KitThemeType = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KitThemeMapping item in e.OldItems)
                {
                    if (ReferenceEquals(item.KitThemeType, this))
                    {
                        item.KitThemeType = null;
                    }
                }
            }
        }

        #endregion

    }
}
