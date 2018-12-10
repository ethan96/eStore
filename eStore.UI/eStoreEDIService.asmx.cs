using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace eStore.UI
{
    /// <summary>
    /// Summary description for eStoreEDIService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class eStoreEDIService : System.Web.Services.WebService
    {

        [WebMethod]
        public string getProduct(string productid)
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            return em.exportProduct(productid);
        }
        [WebMethod]
        public string getProductInfo(string productid,string ip)
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            BusinessModules.Store store = em.getStore("AUS", ip);
            try
            {
                POCOS.Product product = store.getProduct(productid);
                if (product != null)
                {
                    BusinessModules.LTron.ExportManager.AdvantechProductInfoEDI edi = new BusinessModules.LTron.ExportManager.AdvantechProductInfoEDI();
                    BusinessModules.LTron.ProductInfo pi = new BusinessModules.LTron.ProductInfo();
                    pi.ProductNo = product.SProductID;
                    pi.Currency = store.profile.defaultCurrency.CurrencyID;
                    pi.Price = product.getListingPrice().value;
                    pi.URL = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product).Replace("~", "http://" + store.profile.StoreURL);
                    edi.ProductInfo = pi;

                    return em.Serializer<BusinessModules.LTron.ExportManager.AdvantechProductInfoEDI>(edi);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }

        //this mothode will take long time, removed
        //[WebMethod]
        //public string getAllProducts()
        //{
        //    BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
        //    return em.exportProducts();
        //}

        // Alex 20180326 :Get static XML file directly to improve loading time
        [WebMethod]
        public string getAllProducts()
        {
            String ediXMLPath = System.Configuration.ConfigurationManager.AppSettings["EDIAllProductXMLPath"];
            string ediXML = string.Empty;
            if (!string.IsNullOrEmpty(ediXMLPath))
            {
                try
                {
                    eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
                    eStore.BusinessModules.Store _store = soltion.getStore("AUS");
                    if (_store != null)
                    {
                        ediXMLPath = ediXMLPath.Insert(ediXMLPath.Length - 4, _store.storeID);
                        System.IO.StreamReader sr = new System.IO.StreamReader(ediXMLPath);
                        ediXML = sr.ReadToEnd();
                        sr.Dispose();
                        sr.Close();
                    }
                }
                catch (Exception)
                {

                }
            }

            //防止windowservice 没跑.
            if (string.IsNullOrEmpty(ediXML))
            {
                BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
                ediXML = em.exportProducts();
            }

            return ediXML;
        }

        //this mothode will take long time, removed
        [WebMethod]
        public string getProducts(List<string> productids)
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            return em.exportProducts(productids);
        }

        [WebMethod]
        public string getProductCategories()
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            return em.exportCategories();
        }

        [WebMethod]
        public string getSystemCategories()
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            return em.exportCategories(true);
        }

        [WebMethod]
        public string getConfigurationSystem(string productid)
        {
            BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
            return em.exportSystem(productid);
        }
        [WebMethod]
        public string getConfigurationSystems()
        {
            String ediXMLPath = System.Configuration.ConfigurationManager.AppSettings["EDIXMLPath"];
            string ediXML = string.Empty;
            if (!string.IsNullOrEmpty(ediXMLPath))
            {
                try
                {
                    eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
                    eStore.BusinessModules.Store _store = soltion.getStore("AUS");
                    if (_store != null)
                    {
                        ediXMLPath = ediXMLPath.Insert(ediXMLPath.Length - 4, _store.storeID);
                        System.IO.StreamReader sr = new System.IO.StreamReader(ediXMLPath);
                        ediXML = sr.ReadToEnd();
                        sr.Dispose();
                        sr.Close();
                    }
                }
                catch (Exception)
                {
                    
                }
            }

            //防止windowservice 没跑.
            if (string.IsNullOrEmpty(ediXML))
            {
                BusinessModules.LTron.ExportManager em = new BusinessModules.LTron.ExportManager();
                ediXML = em.exportConfigurationSystems();
            }

            return ediXML;
        }
    }
}
