using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.POCOS
{
    public partial class CartContact
    {
        public enum AddressValidationStatus { Unknown, Invalid, POBOX, Valid, CustomerConfirmed, CCRConfirmed }
        private eStore.POCOS.Contact.VATValidResult _VATValidStatus;
        public eStore.POCOS.Contact.VATValidResult VATValidStatus { get {
            //_VATValidStatus默认等于0. 这里的0是UNKNOW
            return _VATValidStatus;
        }
            set
            {
                _VATValidStatus = value;
            }
        }

        public AddressValidationStatus ValidationStatusX
        {
            get
            {
                AddressValidationStatus status = AddressValidationStatus.Unknown;
                if (!string.IsNullOrEmpty(this.ValidationStatus))
                    Enum.TryParse<AddressValidationStatus>(this.ValidationStatus, out status);
                return status;
            }
        }
        /// <summary>
        /// Country code needs to be 2 characters short country code
        /// </summary>
        public String countryCodeX
        {
            get
            {
                if (String.IsNullOrEmpty(CountryCode))
                {
                    if (countryX != null)
                        CountryCode = countryX.Shorts;
                    else
                        CountryCode = Country;
                }

                return CountryCode;
            }

            set { CountryCode = value; }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CartContact))
                return false;
            CartContact compareContact= (CartContact)obj;
            if (compareContact.AttCompanyName == this.AttCompanyName && compareContact.VATNumbe == this.VATNumbe && compareContact.Address1 == this.Address1
                && compareContact.ZipCode == this.ZipCode && compareContact.City == this.City && compareContact.countryCodeX == this.countryCodeX 
                && compareContact.TelNo == this.TelNo && compareContact.FaxNo == this.FaxNo && compareContact.FirstName == this.FirstName 
                && compareContact.LastName == this.LastName && compareContact.Address2 == this.Address2 && compareContact.UserID == this.UserID 
                && compareContact.AddressID == this.AddressID)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //复制contact
        public void copyContact(CartContact copyObj)
        {
            if (copyObj != null)
            {
                this.AddressID = copyObj.AddressID;
                this.AttCompanyName = copyObj.AttCompanyName;
                this.VATNumbe = copyObj.VATNumbe;
                this.Address1 = copyObj.Address1;
                this.ZipCode = copyObj.ZipCode;
                this.City = copyObj.City;
                this.countryCodeX = copyObj.countryCodeX;
                this.TelNo = copyObj.TelNo;
                this.FaxNo = copyObj.FaxNo;
                this.FirstName = copyObj.FirstName;
                this.LastName = copyObj.LastName;
                this.Address2 = copyObj.Address2;
                this.UserID = copyObj.UserID;
            }
        }

        private Country _countryX;
        public Country countryX
        {
            get {
                if (_countryX == null)
                {
                    try
                    {
                        CountryHelper helper = new CountryHelper();
                         _countryX = helper.getCountrybyCountrynameORCode(Country);
                        if (_countryX != null &&
                            (CountryCode != _countryX.Shorts || Country != _countryX.CountryName))
                        {
                            CountryCode = _countryX.Shorts;
                            Country = _countryX.CountryName;
                            save();
                        }
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Warn("Excpetion at country code loop up", Country, "", "", ex);
                        _countryX = null;
                    }
                }
                return _countryX;
            }
        }

        public CountryState stateX
        {
            get {
                if (countryX == null)
                    return null;
                else {
                    if (string.IsNullOrEmpty(State))
                        return null;
                    else
                        return countryX.CountryStates.FirstOrDefault(x => x.StateShorts == this.State);
                }
            }
        }

        public string stateNameX
        {
            get {
                if (stateX == null)
                    return string.Empty;
                else
                    return stateX.StateName;
            }
        }

        public void copy(CartContact source)
        {
            this.Address1 = source.Address1;
            this.Address2 = source.Address2;
            this.AddressID = source.AddressID;
            this.AttCompanyName = source.AttCompanyName;
            //this.Attention = source.Attention;
            this.FirstName = source.FirstName;
            this.LastName = source.LastName;
            this.City = source.City;
            this.ContactType = source.ContactType;            
            this.County = source.County;
            this.CreatedBy = source.CreatedBy;
            this.CreatedDate = source.CreatedDate;
            this.FaxNo = source.FaxNo;
            //this.helper = source.helper;
            this.LastUpdateTime = source.LastUpdateTime;
            this.Mobile = source.Mobile;
            this.State = source.State;
            this.TelExt = source.TelExt;
            this.TelNo = source.TelNo;
            this.UpdatedBy = source.UpdatedBy;
            this.UserID = source.UserID;
            this.ZipCode = source.ZipCode;

            //Need to call countryCodeX first, then the countryname will be fill up.
            this.countryCodeX = source.countryCodeX;
            this.Country = source.Country;

            this.VATNumbe = source.VATNumbe;
            this.LegalForm = source.LegalForm;
            this.VATValidStatus = source.VATValidStatus;
            this._countryX = source.countryX;
        }

        public void copy(Contact source)
        {
            this.Address1 = source.Address1;
            this.Address2 = source.Address2;
            this.AddressID = source.AddressID;
            this.AttCompanyName = source.AttCompanyName;
            this.FirstName = source.FirstName;
            this.LastName = source.LastName;
            this.City = source.City;
            this.ContactType = source.ContactType;
            this.Country = source.Country;
            this.County = source.County;
            this.CreatedBy = source.CreatedBy;
            this.CreatedDate = DateTime.Now;
            this.FaxNo = source.FaxNo;
            this.LastUpdateTime = DateTime.Now;
            this.Mobile = source.Mobile;
            this.State = source.State;
            this.TelExt = source.TelExt;
            this.TelNo = source.TelNo;
            this.UpdatedBy = source.UpdatedBy;
            this.UserID = source.UserID;
            this.ZipCode = source.ZipCode;
            this.countryCodeX = source.countryCodeX;

            this.VATNumbe = source.VATNumbe;
            this.LegalForm = source.LegalForm;
            this.VATValidStatus = source.VATValidStatus;
        }

        public void copy(VSAPCompany source)
        {
            this.Address1 = source.Address;
            this.AddressID = source.CompanyID;
            this.Address2 = "";
            //this.Attention = "";
            this.FirstName = "";
            this.LastName = "";
            //this.AttCompanyName = source.CompanyName;
            this.AttCompanyName = source.companyNameX;
            this.City = source.City;
            this.Country = source.Country;
            this.CreatedBy = "SAP Adapter";
            this.CreatedDate = DateTime.Now;
            this.LastUpdateTime = DateTime.Now;
            this.UpdatedBy = "SAP Adapter";
            this.State = "";
            this.FaxNo = source.FaxNo;
            this.TelNo = source.TelNo;
            this.ZipCode = source.ZipCode;
            this.countryCodeX = source.countryCodeX;
            this.State = source.stateX;
            //this.VATNumbe = source.VATNumbe;
            //this.LegalForm = source.LegalForm;
        }


        public bool isVerifyState()
        {
            return string.IsNullOrEmpty(this.stateNameX) && this.countryX.CountryStates != null && this.countryX.CountryStates.Any();
        }

        /// <summary>
        /// This is use to customize the Zipcode to SAP format
        /// </summary>
        /// <returns></returns>

        public string getSAPZipCode()
        {
            if (CountryCode.Equals("CA"))
            {
                if ( this.ZipCode.Contains(" ") == false)
                {
                    return this.ZipCode.Substring(0, 3) + " " + this.ZipCode.Substring(3, 3);
                }
            }
            else if (CountryCode.Equals("US"))
            {
                if (this.ZipCode.Length > 5)
                    return this.ZipCode.Substring(0, 5);
                if (this.ZipCode.Length < 5)
                    return this.ZipCode.PadLeft(5,'0');
            }

            return this.ZipCode;

        }

        /// <summary>
        /// Use for SAP sync, return the juristic code format
        /// </summary>
        /// <returns></returns>
        public string getJuristicCode() {

            if (CountryCode.Equals("US"))
                return State +  getSAPZipCode();
            else if (CountryCode.Equals("BR"))
                return State + " " + getSAPZipCode();
            else
                return "";
         }

        /// <summary>
        /// This property applies business logic at validating zip code value.
        /// It returns true if the zip code is valid.  Otherwise it returns false.
        /// </summary>
        public Boolean hasValidZipCode
        {
            get
            {
                //判断运送国家如果是Canada.  zipcode 的格式  N9N 9N9
                bool isValid = true;
                Regex re = new Regex(@"^\w{1}\d{1}\w{1}\s{1}\d{1}\w{1}\d{1}$");
                if (Country.ToLower() == "canada" && !re.IsMatch(ZipCode))
                        isValid = false;

                return isValid;
            }
        }
    }


}
