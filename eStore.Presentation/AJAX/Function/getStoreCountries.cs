using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace eStore.Presentation.AJAX.Function
{
    class getStoreCountries : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {

            List<POCOS.Country> countries = Presentation.eStoreContext.Current.Store.profile.Countries.ToList();
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingRole == POCOS.User.Role.Employee)
            {
                eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                countries = eSolution.countries.OrderBy(c => c.CountryName).ToList();
            }
            else
            {
                countries = eStoreContext.Current.Store.profile.Countries.ToList();
                //获得是否有ship 的其他国家.
                string shipToCountry = eStore.Presentation.eStoreContext.Current.getStringSetting("Ship_To_Country");
                if (!string.IsNullOrEmpty(shipToCountry))
                {
                    eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                    string[] countryStr = shipToCountry.Split('|');
                    foreach (string itemCountry in countryStr)
                    {
                        if (!string.IsNullOrEmpty(itemCountry))
                        {
                            POCOS.Country _shipCountry = eSolution.countries.FirstOrDefault(p => p.CountryName == itemCountry || p.Shorts == itemCountry);
                            countries.Add(_shipCountry);  
                        }
                    }
                }
            }

            var _country = (from country in countries
                           where country.Shorts == context.Request["id"]
                           select country).FirstOrDefault();
            if (_country != null)
            {
                var rlt = (from state in _country.CountryStates
                           orderby state.StateName
                           select new JObject {
                           new JProperty("state", state.StateName),
                           new JProperty("short", state.StateShorts)
                       });
                return JsonConvert.SerializeObject(rlt);
            }
            else return "";

        }
    }
}
