using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using eStore.Presentation.UrlRewriting;
using eStore.POCOS;
using System.Xml;
using System.IO;
using eStore.BusinessModules;

namespace eStore.UI
{
    /// <summary>
    /// Summary description for eStoreWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class eStoreWebService : System.Web.Services.WebService
    {
        [WebMethod(EnableSession=true)]
        public bool isSell(string storeid,string PartNo )
        {
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);

            return store.iseStoreproduct(storeid, PartNo);
        }

        [WebMethod(EnableSession=true)]
        public bool isCTOSProduct(string PartNo, string storeid)
        {  
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);
        
            return store.iseStoreCTOSPart(storeid,PartNo);
        }

        [WebMethod(EnableSession = true,Description="if return empty string means not available in eStore")]
        public string orderableProductURL(string PartNumber, string storeid)
        {
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);

            POCOS.Product product = store.getProduct(PartNumber, false);
             if (product != null && product.isOrderable())
             {
                 return MappingUrl.getMappingUrl(product).Replace("~", "http://" + store.profile.StoreURL);
             }
             else
                 return string.Empty;
        }

        [WebMethod(EnableSession=true)]
        public bool isOrderableProduct(string PartNumber, string storeid)
        {
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);

            return store.isPartNumber(PartNumber, storeid);
        }

        [WebMethod(EnableSession=true)]
        public void iseStoreProduct(string PartNo, string storeid, out string standardProductlink, out string buildsystemlink)
        {
            standardProductlink = "";
            buildsystemlink = "";

            List<String> _products;
            List<String> _ctoss;
            BusinessModules.Store store = eStore.BusinessModules.StoreSolution.getInstance().getStore(storeid);
            store.geteStoreProductLink(storeid, PartNo, out _products, out _ctoss);
            if (_products != null)
            {

                if (_products.Count > 1)
                {
                    standardProductlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getPartnolist(_products);
                }
                else if (_products.Count == 1)
                {
                    Part p = store.getPart(_products[0]);
                    if (p != null && p is POCOS.Product)  //there is a exactly the same partno in the system.
                    {
                        standardProductlink = MappingUrl.getMappingUrl(p).Replace("~", "http://" + store.profile.StoreURL);
                    }
                    else
                    {
                        standardProductlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getPartnolist(_products);
                    }
                    
                }
            }

            if (_ctoss != null && _ctoss.Count > 0)
            {
                buildsystemlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getCTOSPartnolist(_ctoss);
            }
        }

        private BusinessModules.Store getStore(string storeid, string ip)
        {
            eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
            string countrycode = soltion.getCountryCodeByIp(ip);
            BusinessModules.Store candidateStore = null;
            if (!countrycode.Equals("XX"))   //non-internal IP
                candidateStore = soltion.locateStore(countrycode, true);
            if (candidateStore == null)
            {
                candidateStore = soltion.getStore(storeid);

            }
            if (candidateStore == null)
            {
                candidateStore = soltion.getStore("AUS");

            }
            return candidateStore;
        }
        //add ip to locate store
        [WebMethod(EnableSession = true)]
        public bool isSellBeta(string storeid, string ip, string PartNo)
        {
            BusinessModules.Store store = getStore(storeid, ip);

            return store.iseStoreproduct(storeid, PartNo);
        }

        [WebMethod(EnableSession = true)]
        public bool isCTOSProductBeta(string storeid, string ip, string PartNo)
        {
            BusinessModules.Store store = getStore(storeid, ip);

            return store.iseStoreCTOSPart(storeid, PartNo);
        }

        [WebMethod(EnableSession = true, Description = "if return empty string means not available in eStore")]
        public string orderableProductURLBeta(string storeid, string ip, string PartNumber)
        {
            BusinessModules.Store store = getStore(storeid, ip);

            POCOS.Product product = store.getProduct(PartNumber, false);
            if (product != null && product.isOrderable())
            {
                return MappingUrl.getMappingUrl(product).Replace("~", "http://" + store.profile.StoreURL);
            }
            else
                return string.Empty;
        }

        [WebMethod(EnableSession = true)]
        public bool isOrderableProductBeta(string storeid, string ip, string PartNumber)
        {
            BusinessModules.Store store = getStore(storeid, ip);

            return store.isPartNumber(PartNumber, storeid);
        }

        [WebMethod(EnableSession = true)]
        public void iseStoreProductBeta(string storeid, string ip, string PartNo, out string standardProductlink, out string buildsystemlink)
        {
            standardProductlink = "";
            buildsystemlink = "";

            List<String> _products;
            List<String> _ctoss;
            BusinessModules.Store store = getStore(storeid, ip);
            store.geteStoreProductLink(storeid, PartNo, out _products, out _ctoss);
            if (_products != null)
            {

                if (_products.Count > 1)
                {
                    standardProductlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getPartnolist(_products);
                }
                else if (_products.Count == 1)
                {
                    Part p = store.getPart(_products[0]);
                    if (p != null && p is POCOS.Product)  //there is a exactly the same partno in the system.
                    {
                        standardProductlink = MappingUrl.getMappingUrl(p).Replace("~", "http://" + store.profile.StoreURL);
                    }
                    else
                    {
                        standardProductlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getPartnolist(_products);
                    }

                }
            }

            if (_ctoss != null && _ctoss.Count > 0)
            {
                buildsystemlink = "http://" + store.profile.StoreURL + "/Compare.aspx?parts=" + getCTOSPartnolist(_ctoss);
            }
        }

        [WebMethod(EnableSession = true)]
        public string link2eStoreByCountry(string country, string modelNO, string[] productids)
        {
            eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
            BusinessModules.Store store = soltion.locateStore(country, true);
            if (store != null)
                return link2eStore(store, modelNO, productids, country,true);
            else
                return string.Empty;
        }

        [WebMethod(EnableSession = true)]
        public string link2eStore(string storeid, string ip, string modelNO, string[] productids)
        {
            BusinessModules.Store store = getStore(storeid, ip);
            if (store != null)
                return link2eStore(store, modelNO, productids);
            else
                return string.Empty;
            
        }

        /// <summary>
        /// 根据ModleNo Search产品 并返回所选 Country
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_sysUrl"></param>
        /// <param name="country"></param>
        /// <param name="store"></param>
        /// <param name="modelNO"></param>
        private void setStoreUrlForLink(ref string _url, ref string _sysUrl, string country, BusinessModules.Store store, string modelNO)
        {
            POCOS.ProductSpecRules psr = store.getMatchProducts(modelNO, false);
            if (psr != null && psr._products != null && psr._products.Any())
            {
                if(psr._products.Count == 1)
                    _url = MappingUrl.getMappingUrl(psr._products.FirstOrDefault()).Replace("~", "http://" + store.profile.StoreURL);
                else
                    _url = "http://" + store.profile.StoreURL + "/Search.aspx?SearchKeyWordsFromWebService=" + modelNO;
            }

            if (!string.IsNullOrEmpty(country))
            {
                var link = string.Format("country={0}&token={1}", country, DateTime.Now.Ticks.ToString());
                if (_url.Contains("?"))
                {
                    _url += string.IsNullOrEmpty(_url) ? "" : "&" + link;
                    _sysUrl += string.IsNullOrEmpty(_sysUrl) ? "" : "&" + link;
                }
                else
                {
                    _url += string.IsNullOrEmpty(_url) ? "" : "?" + link;
                    _sysUrl += string.IsNullOrEmpty(_sysUrl) ? "" : "?" + link;
                }
            }
        }

        private string link2eStore(BusinessModules.Store store, string modelNO, string[] productids,string country = "",bool isUseSearch= false)
        {

            XmlDocument doc = new System.Xml.XmlDocument();
             
            POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
            List<string> products = new List<string>();
            Dictionary<string, List<string>> ctos = new Dictionary<string, List<string>>();
            string xml = helper.link2eStore(modelNO, store.storeID);
            doc.LoadXml(xml);
            XmlNodeList productsnode = doc.GetElementsByTagName("Products");
            if (productsnode.Count > 0)
            {
                var allproducts = productsnode[0].ChildNodes;
                List<XmlNode> removingItems = new List<XmlNode>();
                for  (int i= 0;i< allproducts.Count;i++)
                {
                    if (productids.Any() && !productids.Contains(allproducts[i].SelectSingleNode("./ID").InnerText))
                    {
                        removingItems.Add(allproducts[i]);
                    }
                }

                foreach (var item in removingItems)
                    productsnode[0].RemoveChild(item);

            }
        

            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        [WebMethod(EnableSession = true)]
        public string getProductPageLink(string storeid, string productid)
        {
            eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
            string url = "";
            try
            {
                BusinessModules.Store store = soltion.getStore(storeid);
                POCOS.Part part = store.getPart(productid);

                if (part != null)
                {
                    url = Presentation.UrlRewriting.MappingUrl.getMappingUrl(part);
                    if (url.StartsWith("~/"))
                        url = esUtilities.CommonHelper.GetStoreLocation(false) + url.Remove(0, 2);
                }
            }
            catch (Exception)
            {
                
            }
           
            return url;
        }

        [WebMethod(EnableSession=true)]
        public void releaseStoreCacheProduct(String SProductID)
        {
            Presentation.eStoreContext.Current.Store.releaseStoreCacheProduct(SProductID);
        }

        [WebMethod(EnableSession=true)]
        public void releaseStoreCacheProductCategory(String CategoryPath, String CategoryID=null)
        {
            Presentation.eStoreContext.Current.Store.releaseStoreCacheProductCategory(CategoryPath, CategoryID);
        }
        [WebMethod(EnableSession=true)]
        public void releaseStoreCacheStore(string storeID, String storeURL)
        {
            Presentation.eStoreContext.Current.Store.releaseStoreCacheStore(storeID, storeURL);
        }

        [WebMethod(EnableSession=true)]
        public void releaseAlleStoreCache()
        {
            //clear all cache
            Presentation.eStoreContext.Current.Store.releaseEntireCache();
        }

        [WebMethod(Description = "Return Country code, if return XX, means error or internal IP")]
        public string IP2Nation(string UserHostAddress)
        {
            eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
            return eSolution.getCountryCodeByIp(UserHostAddress);
        }

        [WebMethod(EnableSession=true)]
        public string getUrlByCountryAndReferenceUrl(string countryName, string referenceUrl)
        {
            referenceUrl = (String.IsNullOrWhiteSpace(referenceUrl)) ? "" : referenceUrl.Trim();
            String newURL = referenceUrl;
            if (!string.IsNullOrEmpty(countryName))
            {
                try
                {
                    BusinessModules.Store candidateStore = eStore.BusinessModules.StoreSolution.getInstance().locateStore(countryName, false);
                    if (candidateStore != null)
                    {
                        //need Mike to come out regular express for the replacement
                        if (referenceUrl.ToUpper().StartsWith("HTTP:"))
                        {
                            //check referenceUrl length
                            if (referenceUrl.Length > 8)
                                referenceUrl = referenceUrl.Substring(referenceUrl.IndexOf('/', 8));
                            else
                                referenceUrl = "";
                            
                            newURL = "http://" + candidateStore.profile.StoreURL + referenceUrl;
                        }
                    }
                }
                catch (Exception)
                {
                }                
            }
            return newURL;
        }

        private string getPartnolist(List<String> _products)
        {
            string partnolist = HttpUtility.UrlEncode(string.Join(",", _products));
            return partnolist;
        }

        private string getCTOSPartnolist(List<String> _products)
        {
            string partnolist = HttpUtility.UrlEncode(string.Join(",", _products));
            return partnolist;
        }
        
        [WebMethod(EnableSession = true)]
        public string generateSAPCustomer(string orderNo, string applicationId)
        {
            string errorStr = string.Empty;
            string accountId = "none";
            if (!string.IsNullOrEmpty(orderNo))
            {
                BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomerDAL sapClient = new BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomerDAL();
                System.Data.DataSet myDS = new System.Data.DataSet(); ;
                BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomer.GetAllDataTable resultTable = new BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomer.GetAllDataTable();
                sapClient.GetApplicationByApplicationIDForeStore(applicationId, ref resultTable, ref myDS, ref errorStr);
                if (myDS != null && myDS.Tables.Count > 0 && resultTable != null && resultTable.Rows.Count > 0)
                {
                    eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
                    BusinessModules.Store currentStore = soltion.getStore("AEU");
                    POCOS.Order _order = currentStore.getOrder(orderNo);
                    if (_order != null)
                    {
                        string oldData = string.Empty; string newData = string.Empty; Boolean isSuccess = false;
                        string[] spaceName = null; string[] splitName = null;
                        BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomer.GetAllRow allRow = resultTable.Rows[0] as BusinessModules.MyAdvantechSAPCustomer.CreateSAPCustomer.GetAllRow; ;

                        try
                        {
                            #region SoldTo Old Log
                            oldData = "SoldAddreeId:" + _order.Cart.SoldToContact.AddressID + "||SoldCompanyName:" + _order.Cart.SoldToContact.AttCompanyName + "||SoldVat:" + _order.Cart.SoldToContact.VATNumbe +
                                    "||SoldAddress:" + _order.Cart.SoldToContact.Address1 + "||SoldZip:" + _order.Cart.SoldToContact.ZipCode + "||SoldCity:" + _order.Cart.SoldToContact.City
                                    + "||SoldCountry:" + _order.Cart.SoldToContact.countryCodeX + "||SoldUser:" + _order.Cart.SoldToContact.UserID + "||SoldFirstName:" + _order.Cart.SoldToContact.FirstName;
                            #endregion

                            if (!string.IsNullOrEmpty(allRow.COMPANYID))
                            {
                                accountId = allRow.COMPANYID;
                                _order.Cart.SoldToContact.AddressID = allRow.COMPANYID;
                                _order.Cart.SoldToContact.AttCompanyName = allRow.COMPANYNAME;
                                _order.Cart.SoldToContact.LegalForm = allRow.LEGALFORM;
                                _order.Cart.SoldToContact.VATNumbe = allRow.VATNUMBER;
                                _order.Cart.SoldToContact.Address1 = allRow.ADDRESS;
                                _order.Cart.SoldToContact.ZipCode = allRow.POSTCODE;
                                _order.Cart.SoldToContact.City = allRow.CITY;
                                _order.Cart.SoldToContact.countryCodeX = allRow.COUNTRYCODE_X;//暂时是int类型
                                if (!string.IsNullOrEmpty(allRow.CONTACTPERSONNAME) && (allRow.CONTACTPERSONNAME.Split(' ').Length > 1 || allRow.CONTACTPERSONNAME.Split('.').Length > 1))
                                {
                                    spaceName = allRow.CONTACTPERSONNAME.Split(' ');
                                    splitName = allRow.CONTACTPERSONNAME.Split('.');
                                    if (spaceName.Length > 1)
                                    {
                                        _order.Cart.SoldToContact.FirstName = spaceName[0];
                                        _order.Cart.SoldToContact.LastName = spaceName[1];
                                    }
                                    else
                                    {
                                        _order.Cart.SoldToContact.FirstName = splitName[0];
                                        _order.Cart.SoldToContact.LastName = splitName[1];
                                    }
                                }
                                else
                                    _order.Cart.SoldToContact.FirstName = allRow.CONTACTPERSONNAME;
                                _order.Cart.SoldToContact.UserID = allRow.CONTACTPERSONEMAIL;
                                _order.Cart.SoldToContact.UpdatedBy = allRow.REQUEST_BY;


                                #region SoldTo New Log
                                newData = "SoldAddreeId:" + allRow.COMPANYID + "||SoldCompanyName:" + allRow.COMPANYNAME + "||SoldVat:" + allRow.VATNUMBER +
                                        "||SoldAddress:" + allRow.ADDRESS + "||SoldZip:" + allRow.POSTCODE + "||SoldCity:" + allRow.CITY
                                        + "||SoldCountry:" + allRow.COUNTRYCODE + "||SoldUser:" + allRow.CONTACTPERSONEMAIL + "||SoldFirstName:" + allRow.CONTACTPERSONNAME;
                                #endregion

                                if (_order.Cart.ShipToContact != null)
                                {
                                    if (allRow.HASSHIPTODATA && !string.IsNullOrEmpty(allRow.SHIPTOERPID))
                                    {
                                        #region ShipTo Old Log
                                        oldData += "ShipToAddreeId:" + _order.Cart.ShipToContact.AddressID + "||ShipToCompanyName:" + _order.Cart.ShipToContact.AttCompanyName + "||ShipToVat:" + _order.Cart.ShipToContact.VATNumbe +
                                                "||ShipToAddress:" + _order.Cart.ShipToContact.Address1 + "||ShipToZip:" + _order.Cart.ShipToContact.ZipCode + "||ShipToCity:" + _order.Cart.ShipToContact.City
                                                + "||ShipToCountry:" + _order.Cart.ShipToContact.countryCodeX + "||ShipToUser:" + _order.Cart.ShipToContact.UserID + "||ShipToFirstName:" + _order.Cart.ShipToContact.FirstName;
                                        #endregion

                                        accountId += "," + allRow.SHIPTOERPID;
                                        _order.Cart.ShipToContact.AddressID = allRow.SHIPTOERPID;
                                        _order.Cart.ShipToContact.AttCompanyName = allRow.SHIPTOCOMPANYNAME;
                                        _order.Cart.ShipToContact.VATNumbe = allRow.SHIPTOVATNUMBER;
                                        _order.Cart.ShipToContact.Address1 = allRow.SHIPTOADDRESS;
                                        _order.Cart.ShipToContact.ZipCode = allRow.SHIPTOPOSTCODE;
                                        _order.Cart.ShipToContact.City = allRow.SHIPTOCITY;
                                        _order.Cart.ShipToContact.countryCodeX = allRow.SHIPTOCOUNTRY_X;
                                        if (!string.IsNullOrEmpty(allRow.SHIPTOCONTACTNAME) && (allRow.SHIPTOCONTACTNAME.Split(' ').Length > 1 || allRow.SHIPTOCONTACTNAME.Split('.').Length > 1))
                                        {
                                            spaceName = allRow.SHIPTOCONTACTNAME.Split(' ');
                                            splitName = allRow.SHIPTOCONTACTNAME.Split('.');
                                            if (spaceName.Length > 1)
                                            {
                                                _order.Cart.ShipToContact.FirstName = spaceName[0];
                                                _order.Cart.ShipToContact.LastName = spaceName[1];
                                            }
                                            else
                                            {
                                                _order.Cart.ShipToContact.FirstName = splitName[0];
                                                _order.Cart.ShipToContact.LastName = splitName[1];
                                            }
                                        }
                                        else
                                            _order.Cart.ShipToContact.FirstName = allRow.SHIPTOCONTACTNAME;
                                        _order.Cart.ShipToContact.UserID = allRow.SHIPTOCONTACTEMAIL;

                                        #region ShipTo New Log
                                        newData += "ShipToAddreeId:" + allRow.SHIPTOERPID + "||ShipToCompanyName:" + allRow.SHIPTOCOMPANYNAME + "||ShipToVat:" + allRow.SHIPTOVATNUMBER +
                                                "||ShipToAddress:" + allRow.SHIPTOADDRESS + "||ShipToZip:" + allRow.SHIPTOPOSTCODE + "||ShipToCity:"
                                                + "||ShipToCountry:" + allRow.SHIPTOCOUNTRY + "||ShipToUser:" + allRow.SHIPTOCONTACTEMAIL + "||ShipToFirstName:" + allRow.SHIPTOCONTACTNAME;
                                        #endregion
                                    }
                                    else
                                        _order.Cart.ShipToContact.copyContact(_order.Cart.SoldToContact);
                                    _order.Cart.ShipToContact.UpdatedBy = allRow.REQUEST_BY;
                                }

                                if (_order.Cart.BillToContact != null)
                                {
                                    if (allRow.HASBILLINGDATA && !string.IsNullOrEmpty(allRow.BILLTOERPID))
                                    {
                                        #region BillTo Old Log
                                        oldData += "BillToAddreeId:" + _order.Cart.BillToContact.AddressID + "||BillToCompanyName:" + _order.Cart.BillToContact.AttCompanyName + "||BillToVat:" + _order.Cart.BillToContact.VATNumbe +
                                                "||BillToAddress:" + _order.Cart.BillToContact.Address1 + "||BillToZip:" + _order.Cart.BillToContact.ZipCode + "||BillToCity:" + _order.Cart.BillToContact.City
                                                + "||BillToCountry:" + _order.Cart.BillToContact.countryCodeX + "||BillToUser:" + _order.Cart.BillToContact.UserID;
                                        #endregion

                                        accountId += "," + allRow.BILLTOERPID;
                                        _order.Cart.BillToContact.AddressID = allRow.BILLTOERPID;
                                        _order.Cart.BillToContact.AttCompanyName = allRow.BILLINGCOMPANYNAME;
                                        _order.Cart.BillToContact.VATNumbe = allRow.BILLINGVATNUMBER;
                                        _order.Cart.BillToContact.Address1 = allRow.BILLINGADDRESS;
                                        _order.Cart.BillToContact.ZipCode = allRow.BILLINGPOSTCODE;
                                        _order.Cart.BillToContact.City = allRow.BILLINGCITY;
                                        _order.Cart.BillToContact.countryCodeX = allRow.BILLINGCOUNTRY_X;
                                        if (!string.IsNullOrEmpty(allRow.BILLINGCONTACTNAME) && (allRow.BILLINGCONTACTNAME.Split(' ').Length > 1 || allRow.BILLINGCONTACTNAME.Split('.').Length > 1))
                                        {
                                            spaceName = allRow.BILLINGCONTACTNAME.Split(' ');
                                            splitName = allRow.BILLINGCONTACTNAME.Split('.');
                                            if (spaceName.Length > 1)
                                            {
                                                _order.Cart.BillToContact.FirstName = spaceName[0];
                                                _order.Cart.BillToContact.LastName = spaceName[1];
                                            }
                                            else
                                            {
                                                _order.Cart.BillToContact.FirstName = splitName[0];
                                                _order.Cart.BillToContact.LastName = splitName[1];
                                            }
                                        }
                                        else
                                            _order.Cart.BillToContact.FirstName = allRow.BILLINGCONTACTNAME;
                                        _order.Cart.BillToContact.UserID = allRow.BILLINGCONTACTEMAIL;

                                        #region BillTo New Log
                                        newData += "BillToAddreeId:" + allRow.BILLTOERPID + "||BillToCompanyName:" + allRow.BILLINGCOMPANYNAME + "||BillToVat:" + allRow.BILLINGVATNUMBER +
                                                "||BillToAddress:" + allRow.BILLINGADDRESS + "||BillToZip:" + allRow.BILLINGPOSTCODE + "||BillToCity:" + allRow.BILLINGCITY
                                                + "||BillToCountry:" + allRow.BILLINGCOUNTRY + "||BillToUser:" + allRow.BILLINGCONTACTEMAIL;
                                        #endregion

                                    }
                                    else
                                        _order.Cart.BillToContact.copyContact(_order.Cart.SoldToContact);
                                    _order.Cart.BillToContact.UpdatedBy = allRow.REQUEST_BY;
                                }

                                _order.SAPSyncStatusX = POCOS.Order.OrderSAPSyncStatus.GoodToSync;
                                isSuccess = _order.save() == 0;
                            }
                            else
                                errorStr = "Sold erpid is empty!";
                        }
                        catch (Exception ex)
                        {
                            isSuccess = false;
                            errorStr = ex.Message;
                        }

                        //write log
                        _order.currentFollowUpAssignee = "N/A";
                        _order.currentFollowupStatus = "Update ERP";
                        if (isSuccess)
                        {
                            _order.currentFollowupComment = _order.OrderNo + " <===> Success";
                            if (!currentStore.saveSAPCompanyByContact(_order.Cart.BillToContact, myDS)
                                || !currentStore.saveSAPCompanyByContact(_order.Cart.ShipToContact, myDS)
                                || !currentStore.saveSAPCompanyByContact(_order.Cart.SoldToContact, myDS))
                            {
                                errorStr = "Save SAPCompany have question in eStore！";
                            }
                        }
                        else
                            _order.currentFollowupComment = _order.OrderNo + " <===> Failed";
                        _order.logFollowupActivity(allRow.REQUEST_BY, "CreateSAPCustomer", false);

                        eStore.POCOS.ChangeLog log = new eStore.POCOS.ChangeLog(allRow.REQUEST_BY, POCOS.ChangeLog.ModuleType.Orders.ToString(),
                                    _order.OrderNo, POCOS.ChangeLog.ActiveType.Update_CreateERP.ToString(), oldData, newData, _order.StoreID);
                        log.save();
                    }
                    else
                        errorStr = "OrderNo not exists estore!";
                }
                else
                    errorStr = "ResultTable is empty!";
            }
            else
            {
                errorStr = "OrderNo is empty!";
            }
            if (string.IsNullOrEmpty(errorStr))
            {
                errorStr = "<font size='2' face='Calibri'><span style='font-size:11.0pt'><b style='color:red;font-size:15px;'>Success</b><br>";
                errorStr += "SAP account creation request for order, <b>" + orderNo + "</b>, is granted and has been successfully created.  " +
                        "Click <a href='[/eStoreOrderLink]/Orders/OrderDetails.aspx?OrderNo=" + orderNo + "' style='color:red;'><b>here</b></a> " +
                        "to continue processing eStore order, <b>" + orderNo + "</b>.  The new SAP account ID is <b>" + accountId + "</b>. Thanks</span></font>";
            }
            else
            {
                errorStr = "<font size='2' face='Calibri'><span style='font-size:11.0pt'><b style='color:red;font-size:15px;'>Failed:</b>" + errorStr + "<br>";
                errorStr += "<div style='font-size:14px;'>SAP account creation request for order, <b>" + orderNo + "</b>, is granted and has been successfully created.  " +
                        "But new SAP account information fails at writing back to eStore.  Click <a href='[/eStoreOrderLink]/Orders/OrderDetails.aspx?OrderNo=" + orderNo + "' style='color:red;'><b>here</b></a> " +
                        "to continue processing eStore order, <b>" + orderNo + "</b>.  You may need to manual update new SAP account information before flip the order.  The new SAP account ID is <b>" + accountId + "</b>. Thanks</span></font>";
            }
            return errorStr;
        }


        [WebMethod(EnableSession = true)]
        public List<string> GetEStoreCouponCodes(int CodeQty, int CampaignID,string Storeid, int ExpiredDate,ref string MSG)
        {
            if (CodeQty == null || CodeQty <= 0 || CampaignID == null || CampaignID <= 0 || ExpiredDate == null || ExpiredDate <= 0 || string.IsNullOrEmpty(Storeid))
            {
                MSG = "Input Parameter Error!";
                return new List<string>();
            }
            var campaign = eStore.Presentation.eStoreContext.Current.Store.getCampaignByID(CampaignID, Storeid);
            if (campaign == null)
            {
                MSG = "Can't Get Campaign!";
                return new List<string>();
            }
            if (!campaign.IsCanUse)
            {
                MSG = "Coupons had expired";
                return new List<string>();
            }
            List<string> campaignCodeList = new List<string>();
            for (int i = 0; i < CodeQty; i++)
            {
                var campaignCode = campaign.createNewCampaignCode(ExpiredDate);
                if (campaignCode == null)
                {
                    MSG = "Create New Campaign Code Error !";
                    return new List<string>();
                }
                else
                    campaignCodeList.Add(campaignCode.PromotionCode);
            }
            return campaignCodeList;
        }

        [WebMethod(EnableSession = true, Description = "address validation by UPS/FEDEX")]
        public AddressValidator.ShippingAddressValidationResult ValidateFreightAddress(AddressValidator.ShippingAddress currentAddress, AddressValidator.ValidatationProvider provider,
                                                                        string confirmedBy, string site = "eStore", bool isMakeSure = false)
        {
            AddressValidator.ShippingAddressValidationResult result = new AddressValidator.ShippingAddressValidationResult();
            AddressValidator addressValidator = new AddressValidator();
            if (isMakeSure)
            {
                result.isValid = addressValidator.confirmAsValidAddress(currentAddress, provider, confirmedBy, site);
                //如果失败, 添加message
                if (!result.isValid)
                    result.message = "Save failed.";
            }
            else
                result = addressValidator.validate(currentAddress, provider, confirmedBy, site);
            return result;
        }

        [WebMethod()]
        public void TaskTrigger(string storeid,string taskname)
        {
            eStore.BusinessModules.Task.SyncFilesToBackupServer sfbstask = new eStore.BusinessModules.Task.SyncFilesToBackupServer(taskname);
            eStore.BusinessModules.Task.EventManager.getInstance(storeid.ToUpper()).QueueCommonTask(sfbstask);
        }

        /// <summary>
        /// for erma
        /// </summary>
        /// <param name="ogId"></param>
        /// <param name="pn"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public decimal getPromotionProducts(string ogId, string pn)
        {
            if (string.IsNullOrEmpty(ogId) || string.IsNullOrEmpty(pn))
                return 0;
            POCOS.Part part = eStore.Presentation.eStoreContext.Current.Store.getPart(pn, ogId);
            if (part != null && part is POCOS.Product)
            {
                POCOS.Product pro = part as POCOS.Product;
                return pro.getListingPrice().value;
            }
            else
                return 0;
        }

    }
}
