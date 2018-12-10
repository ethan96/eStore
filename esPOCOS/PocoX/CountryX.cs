using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class Country { 
 
	 #region Extension Methods 
        public CountryAddress getCountryAddress(int addressID)
        {
            return this.CountryAddresses.FirstOrDefault(p => p.AddressID == addressID);
        }

        /// <summary>
        /// There are only two business groups available, EA and EP
        /// </summary>
        /// <param name="businessGroup"></param>
        /// <returns></returns>
        public CountryAddress getCountryAddress(String businessGroup)
        {
            return CountryAddresses.FirstOrDefault(sAddr => sAddr.Division == businessGroup.ToUpper());
        }

        private Dictionary<string, string> _settingsX;
        /// <summary>
        /// This method will merge store settings with country settings
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSettings(Store store)
        {
            if (_settingsX == null)
            {
                //init settingsX with original country settings
                _settingsX = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                lock (_settingsX)
                {
                    try
                    {
                        foreach (CountryParameter sp in this.CountryParameters)
                        {
                            if (!_settingsX.ContainsKey(sp.Parameter))
                                _settingsX.Add(sp.Parameter, sp.ParaValue);
                        }


                        //Merge country settings with store settings
                        //Insert store settings if it does not exist in country settings.
                        if (store != null)
                        {
                            foreach (var item in store.Settings)
                            {
                                if (!_settingsX.ContainsKey(item.Key))
                                    _settingsX.Add(item.Key, item.Value);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //ignore exception and move on
                        String stackTrace = ex.StackTrace;
                    }
                }
              
            }
            return _settingsX;
        }

        /// <summary>
        /// this function is to find aeu country ups shipping method by zip code 
        /// if zip code is not null will choose zip code from and end
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        public List<EUUPSZone> getEUUPSByZipCode(string zipcode = "")
        {
            List<EUUPSZone> ls = this.EUUPSZones.ToList();
            List<EUUPSZone> _nullZipCode = new List<EUUPSZone>();
            List<EUUPSZone> _zipCodeZone = new List<EUUPSZone>();
            foreach (var s in ls)
            {
                if (string.IsNullOrEmpty(s.PostcodeFrom) && string.IsNullOrEmpty(s.PostcodeEnd))
                    _nullZipCode.Add(s);
                else
                    _zipCodeZone.Add(s);
            }
            if (string.IsNullOrEmpty(zipcode))
                return _nullZipCode;

            
            int _zipcode = 0,_postcodeFrom = 0,_postcodeEnd = 0;
            if (int.TryParse(zipcode, out _zipcode))// zip code is interge
            {
                var cc = _zipCodeZone.Where(c => _zipcode >= (int.TryParse(c.PostcodeFrom, out _postcodeFrom) ? _postcodeFrom : -1)
                                            && _zipcode <= (int.TryParse(c.PostcodeEnd, out _postcodeEnd) ? _postcodeEnd : -1)).ToList();
                if (!cc.Any())
                    cc = _nullZipCode;
                return cc;
            }
            else
            {
                var cc = _zipCodeZone.Where(c => zipcode.StartsWith(c.PostcodeFrom)
                                              && c.PostcodeFrom==c.PostcodeEnd).ToList();
                if (!cc.Any())
                    cc = _nullZipCode;
                return cc;
            }
          
        }
        private Store _storeX = null;
        public Store storeX
        {
            get
            {
                if (_storeX == null)
                    _storeX = new StoreHelper().getStorebyStoreid(this.StoreID);

                return _storeX;
            }

            set { _storeX = value; }
        }
        

     #endregion 
	} 
 }