using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{
    /// <summary>
    /// This class needs to be serializable
    /// </summary>
    [Serializable]
    public partial class VSAPCompany
    {
        private String _countryCode = null;

        public String countryCodeX
        {
            get
            {
                if (String.IsNullOrEmpty(_countryCode))
                {
                    try
                    {
                        CountryHelper helper = new CountryHelper();
                        _countryCode = helper.getCountrybyCountrynameORCode(Country).Shorts;
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Warn("Exception at country code lookup", Country, "", "", ex);
                        _countryCode = Country;
                    }
                }

                return _countryCode;
            }

            set { _countryCode = value; }
        }

        private String _state = null;
        public String stateX
        {
            get
            {
                if (_state == null)
                    _state = LookupUtil.findState(this.ZipCode);

                return _state;
            }
        }
    }
}
