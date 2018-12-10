using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class DMF { 
 
	 #region Extension Methods 
    List<String> _countryCoverage = null;
    List<String> _countryAbbreviationCoverage = null;
    public List<String> getCountryCoverage(Boolean includingCountryAbbreviation = false, string rbu = "")
    {
        if (_countryCoverage == null)
        {
            try
            {
                List<Country> countries = (from country in Store.Countries
                                                              where country != null
                                                                  && !string.IsNullOrEmpty(country.CountryName)
                                                                  && country.DMF == this.DMFID 
                                                                  && (string.IsNullOrEmpty(rbu) ? true : country.RbuOM.Trim() == rbu)
                                                              select country).ToList();
                _countryCoverage = new List<String>();
                _countryAbbreviationCoverage = new List<String>();
                foreach (Country country in countries)
                {
                    _countryCoverage.Add(country.CountryName.ToUpper());
                    _countryAbbreviationCoverage.Add(country.Shorts.ToUpper());
                }
            }
            catch (Exception)
            {
                //ignore exception
            }
        }

        List<String> result = new List<String>();

        if (_countryCoverage != null)
            result.AddRange(_countryCoverage);
        if (includingCountryAbbreviation && _countryAbbreviationCoverage!= null)
            result.AddRange(_countryAbbreviationCoverage);
            
        return result;
    }
 #endregion 
	} 
 }