using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.Presentation
{
    [Serializable]
    public enum ContactType { Contact, CartContact, VSAPCompany,Customer }

    [Serializable]
    public class eStoreContact
    {
        public ContactType contactType { get; set; }
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
        public eStoreContact() { }
        public eStoreContact(POCOS.Contact contact)
        {
            contactType = ContactType.Contact;
            this.AddressID = contact.AddressID;
            //this.Attention = contact.Attention;
            this.CreatedDate = contact.CreatedDate;
            this.FirstName = string.IsNullOrEmpty(contact.FirstName) ? string.Empty : contact.FirstName;
            this.LastName = string.IsNullOrEmpty(contact.LastName) ? string.Empty : contact.LastName;
            this.AttCompanyName = string.IsNullOrEmpty(contact.AttCompanyName) ? string.Empty : contact.AttCompanyName;
            this.FaxNo = string.IsNullOrEmpty(contact.FaxNo) ? string.Empty : contact.FaxNo;
            this.TelNo = string.IsNullOrEmpty(contact.TelNo) ? string.Empty : contact.TelNo;
            this.TelExt = string.IsNullOrEmpty(contact.TelExt) ? string.Empty : contact.TelExt;
            this.Mobile = string.IsNullOrEmpty(contact.Mobile) ? string.Empty : contact.Mobile;
            this.Address1 = string.IsNullOrEmpty(contact.Address1) ? string.Empty : contact.Address1;
            this.Address2 = string.IsNullOrEmpty(contact.Address2) ? string.Empty : contact.Address2;
            this.City = string.IsNullOrEmpty(contact.City) ? string.Empty : contact.City;
            this.State = string.IsNullOrEmpty(contact.State) ? string.Empty : contact.State;
            this.County = string.IsNullOrEmpty(contact.County) ? string.Empty : contact.County;
            this.Country = string.IsNullOrEmpty(contact.Country) ? string.Empty : contact.Country;
            this.CountryCode = string.IsNullOrEmpty(contact.countryCodeX) ? string.Empty : contact.countryCodeX;
            this.ZipCode = string.IsNullOrEmpty(contact.ZipCode) ? string.Empty : contact.ZipCode;

            this.VATNumbe = string.IsNullOrEmpty(contact.VATNumbe) ? string.Empty : contact.VATNumbe;
            this.LegalForm = string.IsNullOrEmpty(contact.LegalForm) ? string.Empty : contact.LegalForm;
            this.VATValidStatus = contact.VATValidStatus;
        }

        public eStoreContact(POCOS.CartContact cartContact)
        {
            this.CreatedDate = cartContact.CreatedDate;
            this.AddressID = string.IsNullOrEmpty(cartContact.AddressID) ? string.Empty : cartContact.AddressID;
            this.FirstName = string.IsNullOrEmpty(cartContact.FirstName) ? string.Empty : cartContact.FirstName;
            this.LastName = string.IsNullOrEmpty(cartContact.LastName) ? string.Empty : cartContact.LastName;
            this.AttCompanyName = string.IsNullOrEmpty(cartContact.AttCompanyName) ? string.Empty : cartContact.AttCompanyName;
            this.FaxNo = string.IsNullOrEmpty(cartContact.FaxNo) ? string.Empty : cartContact.FaxNo;
            this.TelNo = string.IsNullOrEmpty(cartContact.TelNo) ? string.Empty : cartContact.TelNo;
            this.TelExt = string.IsNullOrEmpty(cartContact.TelExt) ? string.Empty : cartContact.TelExt;
            this.Mobile = string.IsNullOrEmpty(cartContact.Mobile) ? string.Empty : cartContact.Mobile;
            this.Address1 = string.IsNullOrEmpty(cartContact.Address1) ? string.Empty : cartContact.Address1;
            this.Address2 = string.IsNullOrEmpty(cartContact.Address2) ? string.Empty : cartContact.Address2;
            this.City = string.IsNullOrEmpty(cartContact.City) ? string.Empty : cartContact.City;

            this.State = string.IsNullOrEmpty(cartContact.State) ? string.Empty : cartContact.State;
            this.County = string.IsNullOrEmpty(cartContact.County) ? string.Empty : cartContact.County;
            this.Country = string.IsNullOrEmpty(cartContact.Country) ? string.Empty : cartContact.Country;
            this.CountryCode = string.IsNullOrEmpty(cartContact.countryCodeX) ? string.Empty : cartContact.countryCodeX;
            this.ZipCode = string.IsNullOrEmpty(cartContact.ZipCode) ? string.Empty : cartContact.ZipCode;

            this.VATNumbe = string.IsNullOrEmpty(cartContact.VATNumbe) ? string.Empty : cartContact.VATNumbe;
            this.LegalForm = string.IsNullOrEmpty(cartContact.LegalForm) ? string.Empty : cartContact.LegalForm;
            this.VATValidStatus = cartContact.VATValidStatus;
            contactType = ContactType.CartContact;
        }

        /// <summary>
        /// copy eStroeContact to CartContact
        /// </summary>
        /// <returns></returns>
        public POCOS.CartContact toCartContact()
        {
            POCOS.CartContact cartContact = new POCOS.CartContact();

            cartContact.CreatedDate = this.CreatedDate;
            cartContact.AddressID = string.IsNullOrEmpty(this.AddressID) ? string.Empty : this.AddressID;
            cartContact.FirstName = string.IsNullOrEmpty(this.FirstName) ? string.Empty : this.FirstName;
            cartContact.LastName = string.IsNullOrEmpty(this.LastName) ? string.Empty : this.LastName;
            cartContact.AttCompanyName = string.IsNullOrEmpty(this.AttCompanyName) ? string.Empty : this.AttCompanyName;
            cartContact.FaxNo = string.IsNullOrEmpty(this.FaxNo) ? string.Empty : this.FaxNo;
            cartContact.TelNo = string.IsNullOrEmpty(this.TelNo) ? string.Empty : this.TelNo;
            cartContact.TelExt = string.IsNullOrEmpty(this.TelExt) ? string.Empty : this.TelExt;
            cartContact.Mobile = string.IsNullOrEmpty(this.Mobile) ? string.Empty : this.Mobile;
            cartContact.Address1 = string.IsNullOrEmpty(this.Address1) ? string.Empty : this.Address1;
            cartContact.Address2 = string.IsNullOrEmpty(this.Address2) ? string.Empty : this.Address2;
            cartContact.City = string.IsNullOrEmpty(this.City) ? string.Empty : this.City;
            cartContact.State = string.IsNullOrEmpty(this.State) ? string.Empty : this.State;
            cartContact.County = string.IsNullOrEmpty(this.County) ? string.Empty : this.County;
            cartContact.Country = string.IsNullOrEmpty(this.Country) ? string.Empty : this.Country;
            cartContact.countryCodeX = string.IsNullOrEmpty(this.CountryCode) ? string.Empty : this.CountryCode;
            cartContact.ZipCode = string.IsNullOrEmpty(this.ZipCode) ? string.Empty : this.ZipCode;

            cartContact.VATNumbe = string.IsNullOrEmpty(this.VATNumbe) ? string.Empty : this.VATNumbe;
            cartContact.LegalForm = string.IsNullOrEmpty(this.LegalForm) ? string.Empty : this.LegalForm;
            cartContact.VATValidStatus = this.VATValidStatus;
            cartContact.UserID = Presentation.eStoreContext.Current.User.actingUser.UserID;
            return cartContact;
        }

        public eStoreContact(POCOS.VSAPCompany SAPCompany)
        {
            contactType = ContactType.VSAPCompany;
            this.AddressID = string.IsNullOrEmpty(SAPCompany.CompanyID) ? string.Empty : SAPCompany.CompanyID;
            this.FirstName =  string.Empty;
            this.LastName = string.Empty;
            //this.AttCompanyName = string.IsNullOrEmpty(SAPCompany.CompanyName) ? string.Empty : SAPCompany.CompanyName;
            this.AttCompanyName = string.IsNullOrEmpty(SAPCompany.CompanyName) ? string.Empty : SAPCompany.companyNameX;
            this.FaxNo = string.IsNullOrEmpty(SAPCompany.FaxNo) ? string.Empty : SAPCompany.FaxNo;
            this.TelNo = string.IsNullOrEmpty(SAPCompany.TelNo) ? string.Empty : SAPCompany.TelNo;
            this.TelExt =  string.Empty;
            this.Mobile =  string.Empty;
            this.Address1 = string.IsNullOrEmpty(SAPCompany.Address) ? string.Empty : SAPCompany.Address;
            this.Address2 = string.IsNullOrEmpty(string.Empty) ? string.Empty : string.Empty;
            this.State =  SAPCompany.stateX;
            this.County =  string.Empty;
            this.Country = string.IsNullOrEmpty(SAPCompany.Country) ? string.Empty : SAPCompany.Country;
            this.CountryCode = string.IsNullOrEmpty(SAPCompany.countryCodeX) ? string.Empty : SAPCompany.countryCodeX;
            this.ZipCode = string.IsNullOrEmpty(SAPCompany.ZipCode) ? string.Empty : SAPCompany.ZipCode;
            this.City = string.IsNullOrEmpty(SAPCompany.City) ? string.Empty : SAPCompany.City;
            CreatedDate = DateTime.Now;
        }

        public virtual DateTime CreatedDate
        {
            get;
            set;
        }

        public virtual string AddressID
        {
            get;
            set;
        }

        public virtual string Attention
        {
            get
            {
                return Presentation.eStoreContext.Current.Store.getCultureFullName(this.FirstName, this.LastName);
            }

        }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual string AttCompanyName
        {
            get;
            set;
        }

        public virtual string FaxNo
        {
            get;
            set;
        }

        public virtual string TelNo
        {
            get;
            set;
        }

        public virtual string TelExt
        {
            get;
            set;
        }

        public virtual string Mobile
        {
            get;
            set;
        }

        public virtual string Address1
        {
            get;
            set;
        }

        public virtual string Address2
        {
            get;
            set;
        }

        public virtual string City
        {
            get;
            set;
        }

        public virtual string State
        {
            get;
            set;
        }

        public virtual string County
        {
            get;
            set;
        }

        public virtual string Country
        {
            get;
            set;
        }

        public virtual string CountryCode
        {
            get;
            set;
        }
        public virtual string ZipCode
        {
            get;
            set;
        }

        public virtual string VATNumbe
        {
            get;
            set;
        }
        public virtual string LegalForm
        {
            get;
            set;
        }

        public Boolean isShipTo { get; set; }
        public Boolean isSoldTo { get; set; }
        public Boolean isBillTo { get; set; }
    }
}
