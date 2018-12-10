using System;
using System.Collections.Generic;

namespace eStore.Presentation.VModles.Member
{
    public class VRegiesterUser
    {
        private string _FirstName;

        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; }
        }


        private string _LastName;

        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; }
        }

        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }

        private string _CompanyName;

        public string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = value; }
        }

        private string _Country;

        public string Country
        {
            get { return _Country; }
            set { _Country = value; }
        }

        private string _CountryCode;

        public string CountryCode
        {
            get { return _CountryCode; }
            set { _CountryCode = value; }
        }


        private string _JobFunction;

        public string JobFunction
        {
            get { return _JobFunction; }
            set { _JobFunction = value; }
        }

        private string _JobTitle;

        public string JobTitle
        {
            get { return _JobTitle; }
            set { _JobTitle = value; }
        }

        private string _Phonenumber;

        public string Phonenumber
        {
            get { return _Phonenumber; }
            set { _Phonenumber = value; }
        }

        private string _Mobilenumber;

        public string Mobilenumber
        {
            get { return _Mobilenumber; }
            set { _Mobilenumber = value; }
        }

        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        private string _City;

        public string City
        {
            get { return _City; }
            set { _City = value; }
        }

        private string _State;

        public string State
        {
            get { return _State; }
            set { _State = value; }
        }

        private string _Zip;

        public string Zip
        {
            get { return _Zip; }
            set { _Zip = value; }
        }

        private string _CompanyID;

        public string CompanyID
        {
            get { return _CompanyID; }
            set { _CompanyID = value; }
        }



        public VRegiesterUser()
        { }

        public VRegiesterUser(POCOS.User user)
        {
            if (user != null)
            {
                _FirstName = user.FirstName;
                _LastName = user.LastName;
                _Email = user.UserID;
                _CompanyName = user.CompanyName;
                _JobFunction = user.JobFunction;
                _JobTitle = user.JobTitle;
                _Phonenumber = user.TelNo;
                _Address = user.mainContact.Address1;
                _Mobilenumber = user.mainContact.Mobile;
                _Country = user.mainContact.Country;
                _State = user.mainContact.State;
                _City = user.mainContact.City;
                _Zip = user.mainContact.ZipCode;
                _CompanyID = user.CompanyID;
                _CountryCode = user.mainContact.CountryCode;
            }
        }

        public POCOS.User GetUser(POCOS.User _user = null)
        {
            if (_user == null)
                _user = new POCOS.User
                {
                    CompanyID = $"{this.Email}-{(new Random()).Next(1000, 9999)}" //随机产生公司id
                };

            _user.FirstName = esUtilities.StringUtility.replaceSpecialString(this._FirstName);
            _user.LastName = esUtilities.StringUtility.replaceSpecialString(this._LastName);
            _user.UserID = esUtilities.StringUtility.replaceSpecialString(this._Email,true);
            _user.LoginPassword = string.IsNullOrEmpty(_Password) ? _user.LoginPassword : esUtilities.StringUtility.StringEncry(this._Password);
            _user.CompanyName = esUtilities.StringUtility.replaceSpecialString(this._CompanyName);
            //Country = this._Country,
            _user.JobFunction = esUtilities.StringUtility.replaceSpecialString(this._JobFunction);
            _user.JobTitle = esUtilities.StringUtility.replaceSpecialString(this._JobTitle);
            _user.TelNo = esUtilities.StringUtility.replaceSpecialString(this._Phonenumber);
            _user.CreatedDate = DateTime.Now;
            //mobile
            //address
            //city
            //state
            //zip

            var mainContact = _user.mainContact;
            if (mainContact == null)
            {
                mainContact = new POCOS.Contact(_user)
                {
                    AddressID = _user.CompanyID
                };
            }

            //mainContact.AddressID = user.CompanyID;
            mainContact.AttCompanyName = _user.CompanyName;
            mainContact.FirstName = _user.FirstName;
            mainContact.LastName = _user.LastName;
            mainContact.Address1 = esUtilities.StringUtility.replaceSpecialString(this._Address);
            mainContact.TelNo = esUtilities.StringUtility.replaceSpecialString(this._Phonenumber);
            mainContact.Mobile = esUtilities.StringUtility.replaceSpecialString(this._Mobilenumber);
            mainContact.State = this._State;
            mainContact.City = esUtilities.StringUtility.replaceSpecialString(this._City);
            mainContact.Country = this._Country;
            mainContact.AddressID = _user.CompanyID;
            mainContact.ZipCode = esUtilities.StringUtility.replaceSpecialString(this._Zip);
            mainContact.CountryCode = _CountryCode;
            if (_user.mainContact == null)
            {
                _user.Contacts.Add(mainContact);
            }
            return _user;
        }
    }
}
