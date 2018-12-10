using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class CountryHelper : Helper
    {
        #region Business Read
        public bool isECCountry(string countryshort)
        {
            if (String.IsNullOrEmpty(countryshort)) return false;

            try
            {
              var _country = (from cn in context.Countries
                                   where (cn.Shorts == countryshort) && cn.IsEC ==true
                                   select cn).FirstOrDefault();

              if (_country == null)
                  return false;
              else
                  return true;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return false;
            }
        }

        public Country  getCountrybyCountrynameORCode(string countryname) {
            if (countryname.ToUpper() == "USA" || countryname.Equals("United States", StringComparison.OrdinalIgnoreCase))
                countryname = "US";
            var _country = (from c in context.Countries
                           where c.CountryName.ToLower() == countryname.ToLower() || c.Shorts.ToLower() == countryname.Trim().ToLower()
                           select c).FirstOrDefault();
            if (_country != null)
                _country.helper = this;

            return _country;
        }

        /// <summary>
        /// get all store country
        /// </summary>
        /// <returns></returns>
        public List<Country> getAllCountry()
        {
            List<Country> list = (from c in context.Countries.Include("CountryStates")
                                  orderby c.CountryName
                                  select c).ToList();
            return list;
        }

        public List<Country> getIncludeCountries(string storeid)
        {
            var ls = (from c in context.Countries.Include("CountryAddresses").Include("CountryStates")
                      where c.StoreID == storeid
                      orderby c.CountryName
                      select c).ToList();
            if (ls != null)
            {
                foreach (var li in ls)
                    li.helper = this;
            }
            return ls;
        }

        #endregion

        #region Creat Update Delete
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(CartHelper).ToString();
        }
        #endregion

        public int save(Country country)
        {
            if (country == null)
                return 1;
            if (country == null || country.validate() == false) 
                return 1;
            try
            {
                Country _exist_country = getCountrybyCountrynameORCode(country.CountryName);
                if (_exist_country == null)  //object not exist 
                {
                    context.Countries.AddObject(country);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context = country.helper.context;
                    context.Countries.ApplyCurrentValues(country);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}