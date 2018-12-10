using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace eStore.BusinessModules
{

    public class CMSManager
    {

        public class DataModle
        {
            private string _cmsDisplayName;

            /// <summary>
            /// 显示的 Name
            /// </summary>
            public string CmsDisplayName
            {
                get 
                { 
                    return _cmsDisplayName.Replace("_"," "); 
                }
                set { _cmsDisplayName = value; }
            }

            private string _baa;
            public string Baa
            {
                get { return _baa; }
                set { _baa = value; }
            }

            private List<POCOS.CMS> _dataSorce;
            public List<POCOS.CMS> DataSorce
            {
                get { return _dataSorce; }
                set { _dataSorce = value; }
            }

            private int _qty;

            /// <summary>
            /// 位置
            /// </summary>
            public int Qty
            {
                get { return _qty; }
                set { _qty = value; }
            }

            private bool _isShowPic = true;
            /// <summary>
            /// 是否需要显示图片 默认为True
            /// </summary>
            public bool IsShowPic
            {
                get { return _isShowPic; }
                set { _isShowPic = value; }
            }
        }

        public static CMSManager _cmsManager = null;
        public static CMSManager Instance
        {
            get
            {
                if (_cmsManager == null)
                    _cmsManager = new CMSManager();
                return _cmsManager;
            }
        }

        /// <summary>
        /// 是否显示图片
        /// </summary>
        /// <param name="picStr"></param>
        /// <returns></returns>
        public static bool showPicOrNot(object picStr, bool isShowPic = true)
        {
            bool result = true;
            if (!isShowPic)
                result = false;
            else
            {
                if (picStr == null || string.IsNullOrEmpty(picStr.ToString()))
                    result = false;
                else
                {
                    System.Collections.ArrayList imgTypes = new System.Collections.ArrayList()
                    {
                        "jpg","gif","bmp","png" 
                    };
                    var pic = picStr.ToString().Split('.');
                    if (imgTypes.Contains(pic[pic.Length - 1].ToLower()))
                        result = true;
                    else
                        result = false;
                }

            }
            return result;
        }

        public DataModle getCms(CMSType cmsTpye, POCOS.Store store)
        {
            CMSHelper cmsHelper = new CMSHelper(cmsTpye, "Customer Zone", store);
            return new DataModle
            {
                CmsDisplayName = cmsTpye.ToString(),
                DataSorce = cmsHelper.getCMSList(),
            };
        }
        public DataModle getCmsByBAA(CMSType cmsTpye, POCOS.Store store,string baa)
        {
            CMSHelper cmsHelper = new CMSHelper(cmsTpye, "Customer Zone", store);
            return new DataModle
            {
                CmsDisplayName = cmsTpye.ToString(),
                Baa = baa,
                DataSorce = cmsHelper.getCMSList(baa),
            };
        }

        /// <summary>
        /// get Cms List buy Category
        /// </summary>
        /// <param name="cmsTpye"></param>
        /// <param name="store"></param>
        /// <param name="categoryls"></param>
        /// <returns></returns>
        public DataModle getCmsCategoryList(CMSType cmsTpye, POCOS.Store store, List<string> categoryls)
        {
            CMSHelper cmsHelper = new CMSHelper(cmsTpye, "Customer Zone", store);
            return new DataModle
            {
                CmsDisplayName = cmsTpye.ToString(),
                DataSorce = cmsHelper.getCmsCategoryList(categoryls, cmsTpye.ToString()),
            };
        }

        public POCOS.CMS getCMSByID(POCOS.Store store, string id)
        {
            CMSHelper cmsHelper = new CMSHelper( store);
            return cmsHelper.getCMSByID(id);
        }


        /// <summary>
        /// for storea right Advertisement layout
        /// </summary>
        /// <param name="resourceAdv"></param>
        /// <returns></returns>
        public static string reSetResourceCMSContext(POCOS.Advertisement resourceAdv, BusinessModules.Store store, bool isUseCache = true)
        {
            string contexthtml = "";
            if (resourceAdv.complexAdvertisementContent != null)
            {
                if (resourceAdv.complexAdvertisementContent is POCOS.Advertisement.ResourcesContent)
                {
                    if (isUseCache)
                    {
                        var cacheAdv = resourceAdv.getAdvertisementFromCache();
                        if (cacheAdv != null)
                            return cacheAdv.complexAdvertisementContent.htmlContext;
                    }

                    System.Text.StringBuilder sbContent = new System.Text.StringBuilder();
                    POCOS.Advertisement.ResourcesContent resourceContent = resourceAdv.complexAdvertisementContent as POCOS.Advertisement.ResourcesContent;
                    List<string> validtype = resourceContent.getValidCMSResourceTypes();
                    if (validtype.Count > 0)
                    {
                        sbContent.AppendFormat("<div class=\"eStore_system_listFloatResources\"><div class=\"eStore_system_title\">Resources</div>");
                        for (int i = 0; i < resourceContent.CMSResourceTypes.Count; i++)
                        {
                            string ctype = resourceContent.CMSResourceTypes[i];
                            if (validtype.Contains(ctype))
                            {
                                CMSManager.DataModle datamode = null;
                                if (resourceContent.PisCategorys != null && resourceContent.PisCategorys.Count > 0)
                                    datamode = store.getCms(ctype, resourceContent.PisCategorys);
                                else
                                    datamode = store.getCms(ctype, resourceContent.BusinessApArea);
                                if (datamode.DataSorce.Count != 0)
                                {
                                    sbContent.AppendFormat("<div class=\"eStore_system_block\"><div class=\"eStore_openBox_title eStore_openBox\">{0}</div><div class=\"eStore_openBox_select\"><ul class=\"eStore_listPoint eStore_listPointCMS\">", ctype.Replace("Case Study", "Case Studies"));
                                    int maxcount = 3;
                                    List<string> cmsids = new List<string>();
                                    foreach (var modle in datamode.DataSorce)
                                    {
                                        if (!cmsids.Contains(modle.RECORD_ID))//remove duplicate items
                                        {
                                            if (maxcount <= 0)
                                            {
                                                sbContent.AppendFormat("</ul><a href=\"/Cms/CMSpager.aspx?advid={0}&item={1}\" target=\"_blank\"><p class=\"more\">More>></p></a>", resourceContent.Advid, i);
                                                break;
                                            }
                                            sbContent.AppendFormat("<li><a href='/CMS/CmsDetail.aspx?CMSID={0}&CMSType={1}' target='_blank'>{2}</a></li>", modle.RECORD_ID, modle.cmsTypeX, esUtilities.StringUtility.subString(modle.TITLE, 60));
                                            cmsids.Add(modle.RECORD_ID);
                                            maxcount--;
                                        }
                                    }
                                    if (maxcount > 0)
                                        sbContent.AppendLine("</ul>");
                                    sbContent.AppendLine("</div></div>");
                                }
                            }
                        }
                        sbContent.AppendLine("</div>");
                        sbContent.AppendLine("<script>showCmsResourceAdv();</script>");
                    }
                    contexthtml = sbContent.ToString();
                    resourceAdv.complexAdvertisementContent.htmlContext = contexthtml;
                    resourceAdv.cacheToMemory();
                }
                else
                    contexthtml = resourceAdv.htmlContentX;
            }
            else
                contexthtml = resourceAdv.HtmlContent;

            return contexthtml;
        }

        /// <summary>
        /// get cms api from www advantech
        /// </summary>
        /// <param name="rbu"></param>
        /// <param name="attribute"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static List<AdvantechCmsModel> GetCmsFromApi(string rbu, string attribute = "", string type = "", string tags = "")
        {
            try
            {
                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri("http://www.advantech.com/");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string pars = string.Format("/api/cms/getlistby?rbu={0}{1}{2}{3}"
                        , rbu
                        , string.IsNullOrWhiteSpace(type) ? "" : "&type="+type
                        , string.IsNullOrWhiteSpace(attribute) ? "" : "&attribute=" + attribute
                        , string.IsNullOrWhiteSpace(tags) ? "" : "&tag=" + System.Web.HttpUtility.UrlEncode(tags, Encoding.UTF8));

                    var task = httpClient.GetAsync(pars);
                    task.Result.EnsureSuccessStatusCode();
                    if (task.Result.IsSuccessStatusCode)
                    {
                        HttpResponseMessage response = task.Result;
                        List<AdvantechCmsModel> data = response.Content.ReadAsAsync<List<AdvantechCmsModel>>().Result;
                        return data.OrderByDescending(c=>c.CreatedDateX).ToList();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return new List<AdvantechCmsModel>();
            }
            return new List<AdvantechCmsModel>();
        }
    }
}
