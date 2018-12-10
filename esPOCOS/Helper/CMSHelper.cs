using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public enum CMSType
    {
        Case_Study,
        Events,
        Featured_Article,
        Flash_Demo,
        Introduction,
        Introduction_Video,
        Marketing_Collateral,
        News,
        Product_Spotlight,
        Solutions,
        Training,
        Video,
        Webcast,
        White_Papers,
        Industry_Focus,
        eCatalog,
        eDM_Slash_eNewsletter,
        NA
    }
    public class CMSHelper
    {
        private WWWCMSHelper _wwwcmsHelper;
        protected WWWCMSHelper wwwcmsHelper
        {
            get
            {
                if (_wwwcmsHelper == null)
                {
                    _wwwcmsHelper = CachePool.getInstance().getWWWCMS(Store.StoreID);
                    if (_wwwcmsHelper == null)
                    {
                        _wwwcmsHelper = WWWCMSHelper.getInstance();
                        CachePool.getInstance().cacheWWWCMS(_wwwcmsHelper,Store.StoreID);
                    }
                }
                return _wwwcmsHelper;
            }
            set { _wwwcmsHelper = value; }
        }
        POCOS.Store Store { get; set; }
        private CMSType _cmsType;
        public CMSType cmsType
        {
            get { return _cmsType; }
            set
            {
                _cmsType = value;
                _strCMSType = _cmsType.ToString().Replace("_", " ").Replace("Slash", "/");
            }
        }
        private string _strCMSType;

        private string _strRBU;
        public string strRBU
        {
            get 
            {
                if (string.IsNullOrEmpty(_strRBU))
                {
                    if (!string.IsNullOrEmpty(this.Store.getStringSetting("DefaultRBU")))
                        _strRBU = this.Store.Settings["DefaultRBU"];
                    else
                    {
                        List<Country> ls = Store.Countries.ToList();
                        _strRBU = ls.FirstOrDefault(c => c.CountryName == Store.DefaultCountry).RBU;
                    }
                }
                return _strRBU; 
            }
            set { _strRBU = value; }
        }

        private List<string> rbus;
        public List<string> Rbus
        {
            get 
            {
                if (rbus == null)
                {
                    rbus = new List<string>();
                    if (!string.IsNullOrEmpty(strRBU))
                    {
                        var paras = (new StoreParameterHelper()).getStoreParameterByKey("StoreRBU");
                        foreach (var c in strRBU.Split(','))
                        {
                            var para = paras.FirstOrDefault(p => p.StoreID == c);
                            var _rbu = (para == null ? c : para.ParaValue);
                            if (!rbus.Contains(_rbu))
                                rbus.Add(_rbu);
                        }
                    }
                }
                return rbus; 
            }
            set 
            { 
                rbus = value;
                _strRBU = string.Join(",", value);
            }
        }

        private string _strContactLocation;
        public string strContactLocation
        {
            get { return _strContactLocation; }
            set { _strContactLocation = value; }
        }

        private string[] _list_CL;
        public string[] list_CL
        {
            get
            {
                if (_list_CL == null)
                    _list_CL = wwwcmsHelper.list_CL;
                return list_CL;
            }
            set
            { list_CL = value; }
        }

        /// <summary>
        /// 缓存 cms 的条数
        /// </summary>
        private int _qty = 20;
        public int Qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        //public string strList_CL
        //{
        //    set
        //    {
        //        list_CL = wwwcmsHelper.list_CL.Where(c => c.StartsWith(value)).ToArray<string>();
        //    }
        //}



        public CMSHelper(CMSType strCMSType, string strContactLocation, POCOS.Store store)
        {
            this.cmsType = strCMSType;
            this.strContactLocation = strContactLocation;
            this.Store = store;
        }

        public CMSHelper(POCOS.Store store) { this.Store = store; }

        /// <summary>
        /// 根据 rbu 和 cmstype 查看是否所有的baa有cms
        /// </summary>
        /// <param name="cmsType"></param>
        /// <returns></returns>
        public bool hasCMS4AllBaa(string cmsType,string baa)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(baa))
                {
                    var cc = getCMSList();
                    if (cc != null && cc.Any())
                        result = true;
                }
                else
                {
                    foreach (var rbu in Rbus)
                    {
                        var ds = wwwcmsHelper.getCMSByBAA(baa, cmsType, rbu, 1);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("esPOCOS\\Helper\\CMSHelper.cs -->hasCMS4AllBaa", "", "", "", ex);
            }
            
            return result;
        }

        public bool hasCMS4PisCategory(string cmsType, List<string> categoryLs)
        {
            bool result = false;
            if (string.IsNullOrEmpty(cmsType))
                return result;

            var ls = getCmsCategoryList(categoryLs, cmsType);
            if (ls.Any())
                result = true;

            return result;
        }


        public List<CMS> getCMSList()
        {
            List<CMS> _ls = new List<CMS>();
            DataSet ds;
            try
            {
                if (!string.IsNullOrEmpty(_strCMSType) && !string.IsNullOrEmpty(strRBU) && !string.IsNullOrEmpty(strContactLocation))
                {
                    foreach (var rbu in Rbus)
                    {
                        ds = wwwcmsHelper.getCMSBy(_strCMSType
                        , rbu
                        , wwwcmsHelper.list_CL
                        , wwwcmsHelper.list_BU
                        , wwwcmsHelper.list_PD
                        , wwwcmsHelper.list_BAA
                        , wwwcmsHelper.list_eJF
                        , wwwcmsHelper.list_eL
                        , wwwcmsHelper.list_Activity
                        , strContactLocation
                        , Qty);

                        _ls.AddRange(changeCMSDs2List(ds));
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }
            return _ls;
        }

        public List<CMS> getCMSList(string baa)
        {
            List<CMS> _ls = new List<CMS>();
            DataSet ds;
            try
            {
                if (!string.IsNullOrEmpty(_strCMSType) && !string.IsNullOrEmpty(strRBU) && !string.IsNullOrEmpty(strContactLocation) && wwwcmsHelper.list_BAA.Any(x => x == baa))
                {
                    string[] baas= wwwcmsHelper.list_BAA.Where(x=>x==baa).ToArray();
                    foreach (var rbu in Rbus)
                    {
                        ds = wwwcmsHelper.getCMSBy(_strCMSType
                        , rbu
                        , null
                        , null
                        , null
                        , baas
                        , null
                        , null
                        , null
                        , strContactLocation
                        , Qty);

                        _ls.AddRange(changeCMSDs2List(ds));
                    }
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            }
            return _ls;
        }


        public List<CMS> getCmsCategoryList(List<string> categoryls,string strType)
        {
            List<CMS> ls = new List<CMS>();
            WWWCMSNew.AdminWebService helper = new WWWCMSNew.AdminWebService();
            foreach (var rbu in Rbus)
            {
                var ds = helper.GetCmsBySpecial1(strType.Replace("_", " ").Replace("Slash", "/"), rbu, categoryls.ToArray());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (CMS cms in changeCMSDs2List(ds))
                    {
                        if (!ls.Any(x => x.RECORD_ID == cms.RECORD_ID))
                            ls.Add(cms);
                    }
                }
            }
            return ls;
        }


        private List<CMS> changeCMSDs2List(DataSet ds)
        {
            List<CMS> _ls = new List<CMS>();
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    _ls.Add(new CMS
                    {
                        ABSTRACT = dr["ABSTRACT"].ToString(),
                        //AP_TYPE = dr["AP_TYPE"].ToString(),

                        CATEGORY_NAME = _strCMSType,// dr["CATEGORY_NAME"].ToString(),



                        HYPER_LINK = magCmsDrData(dr, "URL", "HYPER_LINK"),
                        LASTUPDATED = parseDate(magCmsDrData(dr, "ModifyDate", "LASTUPDATED")),                       
                        RECORD_ID = magCmsDrData(dr, "CmsID", "RECORD_ID"),
                        RECORD_IMG = magCmsDrData(dr, "ImageURL", "RECORD_IMG"),
                        RELEASE_DATE = parseDate(magCmsDrData(dr, "ReleaseDate", "RELEASE_DATE")),
                       
                        TITLE = dr["TITLE"].ToString()
                    });
                }
            }
            return _ls;
        }

        private string magCmsDrData(DataRow dr, string str1, string str2)
        {
            if (dr.Table.Columns.Contains(str1))
                return dr[str1].ToString();
            else
                return dr[str2].ToString();
        }


        public POCOS.CMS getCMSByID(string id)
        {
            POCOS.CMS cms = null;
            try
            {
                DataSet ds = wwwcmsHelper.getCMSMaster(id);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    cms = new CMS
                                {
                                    ABSTRACT = dr["ABSTRACT"].ToString(),

                                    HYPER_LINK = magCmsDrData(dr, "URL", "HYPER_LINK"),
                                    LASTUPDATED = parseDate(magCmsDrData(dr, "ModifyDate", "LASTUPDATED")),
                                    RECORD_ID = magCmsDrData(dr, "CmsID", "RECORD_ID"),
                                    RECORD_IMG = magCmsDrData(dr, "ImageURL", "RECORD_IMG"),
                                    RELEASE_DATE = parseDate(magCmsDrData(dr, "ReleaseDate", "RELEASE_DATE")),
                                    TITLE = dr["TITLE"].ToString()
                                };

                    //抓不到LASTUPDATED使用RELEASEDATE替代
                    if (cms.LASTUPDATED == DateTime.MinValue)
                    {
                        cms.LASTUPDATED = parseDate(magCmsDrData(dr, "ReleaseDate", "LASTUPDATED"));
                    }

                    if (ds.Tables.Count == 2 && ds.Tables[1].Rows.Count>0)
                    {
                        DataRow dre = ds.Tables[1].Rows[0];
                         
                                    cms.AP_TYPE = dre["AP_TYPE"].ToString();
                                    cms.BOOTH = dre["BOOTH"].ToString();
                                    cms.EVENT_END = parseDate(dre["EVENT_END"]);
                                    cms.CITY = dre["CITY"].ToString();
                                    cms.CLICKTIME = parseInt(dre["CLICKTIME"]);
                                    cms.CONTACT_EMAIL = dre["CONTACT_EMAIL"].ToString();
                                    cms.CONTACT_NAME = dre["CONTACT_NAME"].ToString();
                                    cms.CONTACT_PHONE = dre["CONTACT_PHONE"].ToString();
                                    cms.COUNTRY = dre["COUNTRY"].ToString();
                                  
                                    
                                    cms.SECOND = parseInt(dre["SECOND"]);
                                    cms.HOURS = parseInt(dre["HOURS"]);
                                    cms.MINUTE = parseInt(dre["MINUTE"]);
                                    cms.EVENT_START = parseDate(dre["EVENT_START"]);
                    }
                    cms.contentX = wwwcmsHelper.getCMSDetailContent(id);

                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return cms;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cms"></param>
        /// <returns></returns>
        public string resetContext(CMS _cms)
        {
            string context = wwwcmsHelper.getCMSDetailContent(_cms.RECORD_ID);
            return context;
        }




        protected int parseInt(object obj)
        {
            int c = 0;
            int.TryParse(obj.ToString(), out c);
            return c;
        }
        protected DateTime parseDate(object obj)
        {
            DateTime dt;
            DateTime.TryParse(obj.ToString(), out dt);
            return dt;
        }
    }

}
