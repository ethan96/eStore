using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;


namespace eStore.POCOS.DAL
{

    public partial class StoreHelper : Helper
    {

        #region Business Read
        /// <summary>
        /// Get Store object by storeid
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public Store getStorebyStoreid(string storeid)
        {
            if (string.IsNullOrEmpty(storeid)) return null;
            else
                return getStore(true, storeid);
        }

        /// <summary>
        /// get store object by hostname ,like "buy.advantech.com"
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public Store getStorebyHostname(string hostname)
        {
            if (string.IsNullOrEmpty(hostname)) return null;
            else
                return getStore(false, hostname);
        }
        //根据dmf获取store
        public Store getStoreByDMF(string dmfId)
        {
            try
            {
                Store _store = (from s in context.Stores
                                where s.DMFID == dmfId
                                select s).FirstOrDefault();

                return _store;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// Return true if the ERPID/CompanyID exist in eStore contacts, Cartcontacts or SAP
        /// </summary>
        /// <param name="_companyid"></param>
        /// <returns></returns>
        public bool isCompanyIDExist(string _companyid)
        {

            if (string.IsNullOrEmpty(_companyid)) 
                return false;

            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {


                    var _us = (from us in context.Contacts
                               where us.AddressID == _companyid
                               select us).FirstOrDefault();

                    var _sap = (from us in context.SAPCompanies
                                where us.CompanyID == _companyid
                                select us).FirstOrDefault();

                    var _cartcontact = (from us in context.CartContacts
                                        where us.AddressID == _companyid
                                        select us).FirstOrDefault();

                    if (_us == null && _sap == null && _cartcontact == null)
                    {
                        return false;

                    }
                    else
                    {
                        return true;
                    }


                }


            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return true;
            }


        }

        /// <summary>
        /// Private GetStore function by using expression. Cosolidate retrieve logics to here.
        /// </summary>
        /// <param name="byID"></param>
        /// <param name="IDHost"></param>
        /// <returns></returns>

        private Store getStore(bool byID, string IDHost)
        {
            //check if store is already loaded 
            Store store = getCachedStore(IDHost.ToUpper());
            if (store != null)
                return store;

            try
            {
                var _store = (from s in context.Stores.Include("StoreCurrencies").Include("StoreCariers").Include("ShipFromAddress")
                                  .Include("StoreFreightRates")
                                  .Include("Countries.CountryCurrencies").Include("MiniSites.SiteParameters")
                           .Where(MyExpressions.whereclause(byID, IDHost))
                              select s).FirstOrDefault();

                foreach (StoreCarier sc in _store.StoreCariers)
                {
                    context.LoadProperty(sc.ShippingCarier, "RateServiceNames");
                }

                //if (_store.Settings == null)
                //    _store.Settings = new Dictionary<string, string>();

                //foreach (StoreParameter sp in _store.StoreParameters)
                //{

                //    _store.Settings.Add(sp.SiteParameter, sp.ParaValue);
                //}

                if (_store.ErrorCodes == null)
                    _store.ErrorCodes = new Dictionary<string, string>();
                foreach (StoreErrorCode sec in _store.StoreErrorCodes)
                {
                    _store.ErrorCodes.Add(sec.ErrorCode, sec.UserActionMessage);
                }

                //compos translation to Dictionary
                if (_store.Localization == null)
                    _store.Localization = new Dictionary<string, string>();

                foreach (Translation tra in _store.translationX)
                {
                    _store.Localization.Add(tra.Key, tra.Value);
                    if (tra.TranslationGlobalResources != null && tra.TranslationGlobalResources.Any())
                    {
                        foreach (var tgs in tra.TranslationGlobalResources)
                        {
                            if (!string.IsNullOrEmpty(tgs.LocalName))
                            {
                                _store.Localization.Add($"{tra.Key}_{tgs.LanguageId.ToString()}", tra.Value);
                            }
                        }
                    }
                }

                _store.LocationIps = (new LocationIpHelper()).GetActiveIps();

                _store.helper = this;

                //keep loaded stores
                cacheStore(_store);

                return _store;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

        public Store getStoreOnly(string storeid)
        {
            var _store = (from s in context.Stores
                                where s.StoreID == storeid
                          select s).FirstOrDefault();
            return _store;
        }

        public List<VSAPCompany> getSAPCompanies(Store store)
        {

            string orgid = store.Settings["ProductLogisticsOrg"];
            var _companies = (from com in context.VSAPCompanies
                              where com.OrgID == orgid && !com.CompanyName.ToLower().Contains("advantech")
                              select com);

            return _companies.ToList();
        }

        public List<POCOS.UserGrade> getUserGrade(Store store)
        {
            return context.UserGrades.Where(x => x.StoreID == store.StoreID).ToList();
        }

        /// <summary>
        /// Return SAP companies by keyword.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="keyword"></param>
        /// <param name="maxCount">This parameter is an optional parameter.  It indicates what the max count of records shall be retrieved.  The default value is 10000 (all).</param>
        /// <returns></returns>

        public List<VSAPCompany> getSAPCompaniesbyKeyword(Store store, string keyword, int maxCount = 10000)
        {

            string orgid = store.Settings["ProductLogisticsOrg"];
            var _companies = (from com in context.VSAPCompanies
                              where com.OrgID == orgid && !com.CompanyName.ToLower().Contains("advantech")
                              && (com.CompanyName.ToUpper().Contains(keyword.ToUpper()) || com.CompanyID.ToUpper().Contains(keyword.ToUpper()))
                              select com).Take(maxCount);

            return _companies.ToList();
        }

        /// <summary>
        /// Return SAP company by company id
        /// </summary>
        /// <param name="store"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public VSAPCompany getSAPCompany(Store store, string companyID)
        {
            VSAPCompany sapCompany = CachePool.getInstance().getSAPCompany(store.StoreID, companyID);

            if (sapCompany == null)
            {
                string orgid = store.Settings["ProductLogisticsOrg"];
                var _company = (from com in context.VSAPCompanies
                                where com.OrgID == orgid && !com.CompanyName.ToLower().Contains("advantech")
                                && com.CompanyID.ToUpper().Equals(companyID.ToUpper())
                                select com).FirstOrDefault();
                if (_company != null)
                {
                    sapCompany = (VSAPCompany)_company;
                    CachePool.getInstance().cacheSAPCompany(store.StoreID, sapCompany);
                }
            }

            return sapCompany;
        }

        public List<Store> getStores()
        {
            try
            {
                /*
                var _store = (from s in context.Stores
                              select s);
                return _store.ToList();
                */

                //though this is less efficient method, but it has integrity promise and performance
                //insurance
                List<Store> stores = new List<Store>();
                Store store = null;
                IList<String> _storeIds = getStoreIds();
                foreach (String storeId in _storeIds)
                {
                    store = getStorebyStoreid(storeId);
                    if (store != null)
                        stores.Add(store);
                }

                return stores;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<string> getStoreIds()
        {
#if uStore

            List<String> _storeIds = (from s in context.Stores.OfType<POCOS.uStore>()
                                      select s.StoreID).ToList();
#else

            List<String> _storeIds = (from s in context.Stores
                                       select s.StoreID).ToList();
#endif
            return _storeIds;
        }

        /// <summary>
        /// For OM use, return Order and Quotation total amount and count. 
        /// </summary>
        /// <returns></returns>

        public List<VDashboard> getDashboard()
        {

            var _dash = from d in context.VDashboards
                        select d;

            if (_dash != null)
                return _dash.ToList();
            else
                return new List<VDashboard>();

        }

        /// <summary>
        /// this method is to retain loaded store in cache
        /// </summary>
        /// <param name="store"></param>
        private void cacheStore(Store store)
        {
            CachePool.getInstance().cacheStore(store);
        }

        /// <summary>
        /// StoreID can be either storeID or storeURL
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        private Store getCachedStore(String storeID)
        {
            return CachePool.getInstance().getStore(storeID);
        }


        public List<RateServiceName> getSupportedShipping(string country, string storeid)
        {

            CountryHelper cp = new CountryHelper();
            Country _country = cp.getCountrybyCountrynameORCode(country);
            Store currentStore = getCachedStore(storeid);
            if (currentStore != null)
            {
                if (_country == null)
                    _country = currentStore.Country;
                var rs = from c in context.RateServiceNames
                         from s in context.StoreCariers
                         where s.CarierName == c.CarierName && s.StoreID.ToUpper().Equals(storeid.ToUpper())
                         && (c.SupportedRegion == null || c.SupportedRegion == "" || c.SupportedRegion.Contains(country)
                         || c.SupportedRegion.ToUpper().Contains(_country.Region.ToUpper()))
                         select c;

                if (rs != null)
                {
                    //if shipfrom country <> shipto country, method display FedEx International Ground
                    string shipFromCountry = currentStore.ShipFromAddress == null ? currentStore.Country.Shorts : currentStore.ShipFromAddress.countryCodeX;
                    if (shipFromCountry != country)
                    {
                        RateServiceName fedexGround = rs.FirstOrDefault(f => f.DefaultServiceName == "FedEx Ground");
                        if (fedexGround != null)
                            fedexGround.DefaultServiceName = "FedEx International Ground";
                    }
                    return rs.ToList();
                }
                else
                    return new List<RateServiceName>();
            }
            else
                return new List<RateServiceName>();
        }


        #endregion

        #region Creat Update Delete
        public int save(Store _store)
        {

            //if parameter is null or validation is false, then return  -1 

            if (_store == null || _store.validate() == false) return 1;
            //Try to retrieve object from DB

            Store _exist_store = getStorebyStoreid(_store.StoreID);
            try
            {
                //save new country addresses first
                foreach (Country country in _store.Countries)
                {
                    foreach (CountryAddress cAddress in country.CountryAddresses)
                    {
                        if (cAddress.AddressID == 0)    //new entry
                        {
                            context.Addresses.AddObject(cAddress.Address);
                            context.CountryAddresses.AddObject(cAddress);
                        }
                    }
                }

                //apply update
                if (_exist_store == null)  //object not exist 
                {
                    //Insert                
                    context.Stores.AddObject(_store);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_store.helper != null && _store.helper.context != null)
                        context = _store.helper.context;
                    //Update                  
                    //context.Stores.Attach(_exist_store);
                    context.Stores.ApplyCurrentValues(_store);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(Store _store)
        {

            if (_store == null || _store.validate() == false) return 1;
            try
            {

                context.DeleteObject(_store);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region OrderSync

        public string OrderSync(Order order)
        {
            //Sync order to SAP
            return "Dummy Success";
        }


        #endregion
        #region Update ShipFromAddressID


        public int updateShipFromAddress(Store _store, Address ShipFromAddress)
        {

            if (_store == null || ShipFromAddress == null || _store.validate() == false) return 1;
            try
            {
                //using new context to make sure 
                eStore3Entities6 newcontext = new eStore3Entities6();

                Store _exist_store = newcontext.Stores.FirstOrDefault(x => x.StoreID == _store.StoreID);
                if (_exist_store == null)
                    return 1;

                //set relative but doesn't save to database;
                _store.ShipFromAddressID = ShipFromAddress.AddressID;
                if (_store.helper != null && _store.helper.context != null)
                {
                    ShipFromAddress = _store.helper.context.Addresses.FirstOrDefault(x => x.AddressID == ShipFromAddress.AddressID);
                    if (ShipFromAddress != null)
                        _store.ShipFromAddress = ShipFromAddress;
                }

                //get new entity and save to database;
                _exist_store.ShipFromAddressID = ShipFromAddress.AddressID;
                newcontext.Stores.ApplyCurrentValues(_exist_store);
                newcontext.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion
        #region Others

        private static string myclassname()
        {
            return typeof(StoreHelper).ToString();
        }
        #endregion
    }

    public static class MyExpressions
    {

        public static Expression<Func<Store, bool>> whereclause(bool id, string host_storeid)
        {
            if (id == true)
                return (s) => s.StoreID == host_storeid;
            else
                return (s) => s.StoreURL == host_storeid || (s.MiniSites.Any(m => m.StoreURL.Equals(host_storeid, StringComparison.OrdinalIgnoreCase)));
        }

        //public static Expression<Func<uStore, bool>> whereclauseforustore(bool id, string host_storeid)
        //{
        //    if (id == true)
        //        return (s) => s.StoreID == host_storeid;
        //    else
        //        return (s) => s.uStoreURL == host_storeid;
        //}

    }
}