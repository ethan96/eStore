using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

 namespace eStore.POCOS{ 

public partial class Contact { 
 
#region Extension Methods 

    public enum CONTACTTYPE { SOLDTO, BILLTO, SHIPTO, GENERIC, UNKNOWN };

    public enum VATValidResult { VALID = 1, INVALID = -1, UNKNOW = 0 };

    private eStore.POCOS.Contact.VATValidResult _VATValidStatus;
    public eStore.POCOS.Contact.VATValidResult VATValidStatus
    {
        get
        {
            //_VATValidStatus默认等于0. 这里的0是UNKNOW
            return _VATValidStatus;
        }
        set
        {
            _VATValidStatus = value;
        }
    }

    private CONTACTTYPE _type = CONTACTTYPE.UNKNOWN;
    
    /// <summary>
    /// Default constructor for entity framework use
    /// </summary>
    public Contact()
    {
    }

    /// <summary>
    /// eStore application shall use this constructor for any new contact creation.
    /// </summary>
    /// <param name="user">the creator</param>
    public Contact(User user)
    {
        User = user;
        CreatedBy = user.UserID;
        CreatedDate = DateTime.Now;
        LastUpdateTime = CreatedDate;
        UpdatedBy = CreatedBy;
    }

    public virtual void setAttention(User modifier, String firstName, String lastName, String companyName = "")
    {
        //Attention = name;
        FirstName = firstName;
        LastName = lastName;
        AttCompanyName = companyName;
        UpdatedBy = modifier.UserID;
        LastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// This function will validate required fields.  If any one of them is either null or empty, this function will return false and
    /// nothing will be changed.
    /// </summary>
    /// <param name="modifier"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="city"></param>
    /// <param name="state"></param>
    /// <param name="zipCode"></param>
    /// <param name="country"></param>
    /// <returns></returns>
    public virtual Boolean setAddress(User modifier, String address1, String address2, String city, String state, String zipCode, String country)
    {
        if (address1 == null || address1.Length == 0 || city == null || city.Length == 0 || state == null || state.Length == 0 ||
            zipCode == null || zipCode.Length == 0 || country == null || country.Length == 0)
            return false;   //invalid

        Address1 = address1;
        Address2 = address2;
        City = city;
        State = state;
        ZipCode = zipCode;
        countryX = country;
        UpdatedBy = modifier.UserID;
        LastUpdateTime = DateTime.Now;

        return true;
    }


    public virtual CONTACTTYPE type
    {
        get
        {
            if (type == CONTACTTYPE.UNKNOWN)
            {
                switch (ContactType)
                {
                    case "SoldTo":
                        _type = CONTACTTYPE.SOLDTO;
                        break;
                    case "ShipTo":
                        _type = CONTACTTYPE.SHIPTO;
                        break;
                    case "BillTo":
                        _type = CONTACTTYPE.BILLTO;
                        break;
                    default:
                        _type = CONTACTTYPE.GENERIC;
                        break;
                }
            }

            return _type;
        }

        set
        {
            switch (value)
            {
                case CONTACTTYPE.SHIPTO:
                    ContactType = "ShipTo";
                    break;
                case CONTACTTYPE.BILLTO:
                    ContactType = "BillTo";
                    break;
                case CONTACTTYPE.SOLDTO:
                    ContactType = "SoldTo";
                    break;
                case CONTACTTYPE.GENERIC:
                default:
                    ContactType = "Generic";
                    break;
            }
            _type = value;
        }
    }

    public String countryX
    {
        get { return Country; }
        set
        {
            Country = value;
            try
            {
                CountryHelper helper = new CountryHelper();
                Country country = helper.getCountrybyCountrynameORCode(Country);
                if (country != null)
                    CountryCode = country.Shorts;
            }
            catch (Exception)
            {
                CountryCode = Country;
            }
        }
    }

    public String countryCodeX
    {
        get
        {
            if (String.IsNullOrEmpty(CountryCode))
            {
                try
                {
                    CountryHelper helper = new CountryHelper();
                    CountryCode = helper.getCountrybyCountrynameORCode(Country).Shorts;
                    //save();
                }
                catch (Exception ex)
                {
                    eStoreLoger.Warn("Excpeiton at country code lookup", Country, "", "", ex);
                    CountryCode = Country;
                }
            }

            return CountryCode;
        }

        set
        {
            CountryCode = value;
        }
    }
#endregion 
	} 
 }