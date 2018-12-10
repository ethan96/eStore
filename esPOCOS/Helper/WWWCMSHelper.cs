using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Data;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public static class CmsUtility
    {
        /// <summary>
        /// change storeid to rub for cms
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>

        public static DateTime parseDate(object obj)
        {
            DateTime dt;
            DateTime.TryParse(obj.ToString(), out dt);
            return dt;
        }
    }

    public partial class WWWCMSHelper
    {
        private static WWWCMSHelper _cmsManagement = null;

        public string[] strCMSType { get; set; }

        public string[] strRBU { get; set; }

        public string[] list_CL { get; set; }

        public string[] list_BU { get; set; }

        public string[] list_PD { get; set; }

        public string[] list_BAA { get; set; }

        public string[] list_eJF { get; set; }

        public string[] list_eL { get; set; }

        public string[] list_Activity { get; set; }

        public string[] strContactLocation { get; set; }        
        
        private POCOS.WWWCMS.AdvantechWebServiceLocal _cms { get; set; }
        
        public static WWWCMSHelper getInstance()
        {
            if (_cmsManagement == null)
            {
                _cmsManagement = new WWWCMSHelper();
                try
                {
                    POCOS.WWWCMS.AdvantechWebServiceLocal cms = new POCOS.WWWCMS.AdvantechWebServiceLocal();
                    _cmsManagement._cms = cms;
                    _cmsManagement.strCMSType = cms.getCMS_CategoryName_ListBy("News Type");
                    _cmsManagement.strRBU = cms.getCMS_CategoryName_ListBy("Target Area");
                    _cmsManagement.list_CL = cms.getCMS_CategoryName_ListBy("Corporate Level");
                    _cmsManagement.list_BU = cms.getCMS_CategoryName_ListBy("BU/Sector");
                    _cmsManagement.list_PD = cms.getCMS_CategoryName_ListBy("PD");
                    _cmsManagement.list_BAA = cms.getCMS_CategoryName_ListBy("Business Application Areas");
                    _cmsManagement.list_eJF = cms.getCMS_CategoryName_ListBy("eLearning(Job Function)");
                    _cmsManagement.list_eL = cms.getCMS_CategoryName_ListBy("eLearning(Level)");
                    _cmsManagement.list_Activity = cms.getCMS_CategoryName_ListBy("Activity");
                    _cmsManagement.strContactLocation = cms.getCMS_CategoryName_ListBy("Content Location");
                }
                catch (Exception ex)
                {
                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                }
            }
            return _cmsManagement;
        }

        /// <summary>
        /// get Events by RBU
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Events> getEventsByRBU(string storeid)
        {
            List<Events> ls = new List<Events>();
            DataSet ds = new DataSet();
            try
            {
                WWWCMS.AdvantechWebServiceLocal aws = new WWWCMS.AdvantechWebServiceLocal();
                ds = aws.getCorpEventsByRBU(storeid);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Events ev = new Events
                        {
                            ABSTRACT = dr["ABSTRACT"].ToString(),
                            AP_TYPE = dr["AP_TYPE"].ToString(),
                            BOOTH = dr["BOOTH"].ToString(),
                            CATEGORY_NAME = dr["CATEGORY_NAME"].ToString(),
                            CITY = dr["CITY"].ToString(),
                            CLICKTIME = dr["CLICKTIME"].ToString(),
                            CONTACT_EMAIL = dr["CONTACT_EMAIL"].ToString(),
                            CONTACT_NAME = dr["CONTACT_NAME"].ToString(),
                            CONTACT_PHONE = dr["CONTACT_PHONE"].ToString(),
                            COUNTRY = dr["COUNTRY"].ToString(),
                            EVENT_END = CmsUtility.parseDate(dr["EVENT_END"]),
                            EVENT_START = CmsUtility.parseDate(dr["EVENT_START"]),
                            HOURS = dr["HOURS"].ToString(),
                            HYPER_LINK = dr["HYPER_LINK"].ToString(),
                            LASTUPDATED = CmsUtility.parseDate(dr["LASTUPDATED"]),
                            MINUTE = dr["MINUTE"].ToString(),
                            RECORD_ID = dr["RECORD_ID"].ToString(),
                            RECORD_IMG = dr["RECORD_IMG"].ToString(),
                            RELEASE_DATE = CmsUtility.parseDate(dr["RELEASE_DATE"]),
                            SECOND = dr["SECOND"].ToString(),
                            TITLE = dr["TITLE"].ToString()
                        };
                        ls.Add(ev);
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }
            return ls.OrderByDescending(c =>c.EVENT_START).ToList();
        }

        public DataSet getCMSByPartNumber(string[] keyWords, string storeId, string[] cmsType = null)
        {
            DataSet ds = new DataSet();
            try
            {
                if (keyWords != null && keyWords.Length > 0)
                {
                    string[] cms_type = cmsType;
                    if(cms_type == null)
                        cms_type = new string[]  // 所有类型
                            { 
                                "Case Study",
                                "Featured Article", 
                                "Flash Demo", 
                                "Introduction", 
                                "Introduction Video", 
                                "Marketing Collateral", 
                                "News", 
                                "Product Spotlight",
                                "Solutions", 
                                "Training",
                                "Video", 
                                "Webcast",
                                "White Papers", 
                            };
                    POCOS.WWWCMS.AdvantechWebServiceLocal cms = new POCOS.WWWCMS.AdvantechWebServiceLocal();
                    ds = cms.getCMSByPartNumber(cms_type, keyWords, (new StoreParameterHelper()).getStoreParameterByKeyAndStore(storeId, "StoreRBU", true).ParaValue, "", 20);
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error(ex.Message, "", "", "", ex);
            }
            
            return ds;
        }

        public DataSet getCMSByPartNumber(string keyWords, string storeId, string[] cmsType = null)
        {
            if (!string.IsNullOrEmpty(keyWords))
                return getCMSByPartNumber(keyWords.Split(','), storeId, cmsType);
            else
                return new DataSet();
        }
        
        public DataSet getCMSByBAA(string stringBAAList, string stringCMSType, string stringRBU, int intReturnRow)
        {
            return _cms.getCMSByBAA(stringBAAList, stringCMSType, stringRBU, intReturnRow);
        }

        public DataSet getCMSBy(string strCMSType, string strRBU, string[] list_CL, string[] list_BU, string[] list_PD, string[] list_BAA, string[] list_eJF, string[] list_eL, string[] list_Activity, string strContactLocation, int intReturnRow)
        {
            return _cms.getCMSBy(strCMSType, strRBU, list_CL, list_BU,  list_PD, list_BAA, list_eJF, list_eL, list_Activity, strContactLocation, intReturnRow);
        }

        public DataSet getCMSMaster(string cmsID)
        {
            return _cms.getCMSMaster(cmsID);
        }

        public string getCMSDetailContent(string cmsID)
        {
            return _cms.getCMSDetailContent(cmsID);
        }
    }


    public class WWWCmsNew
    {

        public DataSet getCMSNewByModel(string[] modelNos, string storeId, string cmsType, int rowCount = 20)
        {
            POCOS.WWWCMSNew.AdminWebService cmsHelper = new WWWCMSNew.AdminWebService();
            DataSet ds = cmsHelper.GetCmsBy(cmsType, (new StoreParameterHelper()).getStoreParameterByKeyAndStore(storeId, "StoreRBU", true).ParaValue, null, modelNos, null, null, rowCount,true);
            return ds;
        }

    }


}
