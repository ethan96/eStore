using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;

namespace eStore.POCOS
{
    public partial class Address
    {
        private string _countryCode;

        /// <summary>
        /// Country code needs to be 2 characters short country code
        /// </summary>
        public String countryCodeX
        {
            get
            {
                if (String.IsNullOrEmpty(_countryCode))
                {
                    try
                    {
                        CountryHelper helper = new CountryHelper();
                        Country _country = helper.getCountrybyCountrynameORCode(Country);
                        _countryCode = _country.Shorts;

                    }
                    catch (Exception)
                    {

                    }
                }
                return _countryCode;
            }

            set { _countryCode = value; }
        }

        /// <summary>
        /// this property get all address information 
        /// </summary>
        private string _ourAddress = string.Empty;
        public string fullAddress
        {
            get 
            {
                if (string.IsNullOrEmpty(_ourAddress))
                    _ourAddress = this.Address1 + ", " + this.City + ", " + this.ZipCode + " " + this.State + ", " + this.Country;
                return _ourAddress; 
            }
        }

        public string addressType { get; set; }

        private string _shortTel;
        public string shortTel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_shortTel))
                {
                    try
                    {
                        Regex regexObj = new Regex(@".*?([\d-+\s]+).*");
                        _shortTel = regexObj.Replace(this.Tel, "$1").Trim();
                    }
                    catch (ArgumentException)
                    {
                        // Syntax error in the regular expression
                        _shortTel = string.Empty;
                    }

                }
                return _shortTel;

            }
        }
    }
}
