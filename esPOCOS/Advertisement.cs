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
    public partial class Advertisement
    {
        #region Primitive Properties
     
    	public List<ErrorMessage> error_message{
    		get;
    		set;
    	}
    
    
        public virtual int id
        {
            get;
            set;
        }
    
        public virtual string StoreID
        {
            get;
            set;
        }
    
        public virtual string AdType
        {
            get;
            set;
        }
    
        public virtual string Title
        {
            get;
            set;
        }
    
        public virtual string ImageDimension
        {
            get;
            set;
        }
    
        public virtual string ImageMimetype
        {
            get;
            set;
        }
    
        public virtual Nullable<byte> ElapsedTime
        {
            get;
            set;
        }
    
        public virtual string Hyperlink
        {
            get;
            set;
        }
    
        public virtual string Target
        {
            get;
            set;
        }
    
        public virtual string Map
        {
            get;
            set;
        }
    
        public virtual string AlternateText
        {
            get;
            set;
        }
    
        public virtual Nullable<int> Seq
        {
            get;
            set;
        }
    
        public virtual System.DateTime StartDate
        {
            get;
            set;
        }
    
        public virtual System.DateTime EndDate
        {
            get;
            set;
        }
    
        public virtual string UpdateBy
        {
            get;
            set;
        }
    
        public virtual System.DateTime UpdateDate
        {
            get;
            set;
        }
    
        public virtual bool Publish
        {
            get;
            set;
        }
    
        public virtual bool Preview
        {
            get;
            set;
        }
    
        public virtual int ClickTimes
        {
            get;
            set;
        }
    
        public virtual int DisplayTimes
        {
            get;
            set;
        }
    
        public virtual string Imagefile
        {
            get;
            set;
        }
    
        public virtual string Keywords
        {
            get;
            set;
        }
    
        public virtual Nullable<int> Weight
        {
            get;
            set;
        }
    
        public virtual string HtmlContent
        {
            get;
            set;
        }
    
        public virtual string LocationPath
        {
            get;
            set;
        }
    
        public virtual Nullable<int> MiniSiteID
        {
            get { return _miniSiteID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_miniSiteID != value)
                    {
                        if (MiniSite != null && MiniSite.ID != value)
                        {
                            MiniSite = null;
                        }
                        _miniSiteID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<int> _miniSiteID;
    
        public virtual string AdvContext
        {
            get;
            set;
        }

        #endregion

        #region Validation
    	
    	public bool validate() {
    		error_message = new List<ErrorMessage>();
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
            Type type = typeof(Advertisement); // Get type pointer
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
    
        public virtual ICollection<MenuAdvertisement> MenuAdvertisements
        {
            get
            {
                if (_menuAdvertisements == null)
                {
                    var newCollection = new FixupCollection<MenuAdvertisement>();
                    newCollection.CollectionChanged += FixupMenuAdvertisements;
                    _menuAdvertisements = newCollection;
                }
                return _menuAdvertisements;
            }
            set
            {
                if (!ReferenceEquals(_menuAdvertisements, value))
                {
                    var previousValue = _menuAdvertisements as FixupCollection<MenuAdvertisement>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupMenuAdvertisements;
                    }
                    _menuAdvertisements = value;
                    var newValue = value as FixupCollection<MenuAdvertisement>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMenuAdvertisements;
                    }
                }
            }
        }
        private ICollection<MenuAdvertisement> _menuAdvertisements;
    
        public virtual MiniSite MiniSite
        {
            get { return _miniSite; }
            set
            {
                if (!ReferenceEquals(_miniSite, value))
                {
                    var previousValue = _miniSite;
                    _miniSite = value;
                    FixupMiniSite(previousValue);
                }
            }
        }
        private MiniSite _miniSite;
    
        public virtual ICollection<GiftActivity> GiftActivities
        {
            get
            {
                if (_giftActivities == null)
                {
                    var newCollection = new FixupCollection<GiftActivity>();
                    newCollection.CollectionChanged += FixupGiftActivities;
                    _giftActivities = newCollection;
                }
                return _giftActivities;
            }
            set
            {
                if (!ReferenceEquals(_giftActivities, value))
                {
                    var previousValue = _giftActivities as FixupCollection<GiftActivity>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupGiftActivities;
                    }
                    _giftActivities = value;
                    var newValue = value as FixupCollection<GiftActivity>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupGiftActivities;
                    }
                }
            }
        }
        private ICollection<GiftActivity> _giftActivities;

        #endregion

        #region Association Fixup
    
        private bool _settingFK = false;
    
        private void FixupMiniSite(MiniSite previousValue)
        {
            if (MiniSite != null)
            {
                if (MiniSiteID != MiniSite.ID)
                {
                    MiniSiteID = MiniSite.ID;
                }
            }
            else if (!_settingFK)
            {
                MiniSiteID = null;
            }
        }
    
        private void FixupMenuAdvertisements(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MenuAdvertisement item in e.NewItems)
                {
                    item.Advertisement = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (MenuAdvertisement item in e.OldItems)
                {
                    if (ReferenceEquals(item.Advertisement, this))
                    {
                        item.Advertisement = null;
                    }
                }
            }
        }
    
        private void FixupGiftActivities(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (GiftActivity item in e.NewItems)
                {
                    item.Advertisement = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (GiftActivity item in e.OldItems)
                {
                    if (ReferenceEquals(item.Advertisement, this))
                    {
                        item.Advertisement = null;
                    }
                }
            }
        }

        #endregion

    }
}
