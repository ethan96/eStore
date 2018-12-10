using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Cms
{
    public partial class IotSuccessStory : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected Dictionary<string, List<string>> categoryModels = new Dictionary<string, List<string>>();

        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "jquery.easing", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "iot.carousel", "iot.carousel.js");
            BindScript("url", "custom-form-elements", "custom-form-elements.js");
            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.vgrid", "jquery.vgrid.js");
            if (!IsPostBack)
            {
                bindIotCategories();
                bindCmsInfors();
            }
        }

        protected void bindIotCategories()
        {
            var rootCategorys = eStoreContext.Current.Store.getTopLeveluStoreCategories(eStoreContext.Current.MiniSite);
            ddlIotCategories.Items.Add(new ListItem("-- " + eStore.Presentation.eStoreLocalization.Tanslation("eStore_IotMart_Story_Select_Categories") + " --", "all"));

            foreach (var cate in rootCategorys)
            {
                ddlIotCategories.Items.Add(new ListItem(cate.localCategoryNameX, cate.localCategoryNameX));
                var models = cate.productList.Select(c => c.ModelNo).Distinct().ToList();
                categoryModels.Add(cate.localCategoryNameX, models);
                //if (cate.childCategoriesX.Any())
                //{
                //    foreach (var sub in cate.childCategoriesX)
                //    {
                //        ddlIotCategories.Items.Add(new ListItem("　" + sub.localCategoryNameX, sub.CategoryPath));
                //        modeljson = string.Join(",", sub.productList.Select(c => c.ModelNo).Distinct().ToList());
                //        ls.Add("{'CategoryPath':'" + sub.CategoryPath + "','Models':'" + modeljson + "'}");
                //    }
                //}
            }
        }

        protected void bindCmsInfors()
        {
            var dv = getCmsData();
            List<string> al = new List<string>();
            ddlCmsAppType.Items.Add(new ListItem("-- " + eStore.Presentation.eStoreLocalization.Tanslation("eStore_IotMart_Story_Select_Business_Application_Area") + " --", "all"));
            al.Add("all");
            foreach (var dr in dv)
            {
                foreach(var c in dr.CmsApps)
                {
                    if (!string.IsNullOrEmpty(c) && !al.Contains(c,(new myContains())))
                    {
                        al.Add(c);
                        ddlCmsAppType.Items.Add(new ListItem(c, c));
                    }
                }
            }
            rpCmsList.DataSource = dv;
            rpCmsList.DataBind();
        }



        protected List<CmsMap> getCmsData()
        {
            List<CmsMap> list = new List<CmsMap>();
            var models = eStore.Presentation.eStoreContext.Current.Store.getStoreAllProductModel(eStore.Presentation.eStoreContext.Current.MiniSite);
            if (models.Any())
            {
                var cmss = eStore.Presentation.eStoreContext.Current.Store.getNewCmsByModels(models.ToArray(), "Case Study",50);

                if (eStore.Presentation.eStoreContext.Current.MiniSite.StoreID == "ATW" && cmss != null && cmss.Tables[0].Rows.Count < 50)
                {
                    var cc = eStore.Presentation.eStoreContext.Current.Store.getNewCmsByModels(models.ToArray(), "Case Study", 50 - cmss.Tables[0].Rows.Count, "AUS");
                    object[] obj = new object[cmss.Tables[0].Columns.Count];
                    foreach (System.Data.DataRow dr in cc.Tables[0].Rows)
                    {
                        dr.ItemArray.CopyTo(obj, 0);
                        cmss.Tables[0].Rows.Add(obj);
                    }
                }
                foreach (System.Data.DataRow dr in cmss.Tables[0].Rows)
                {
                    var ms = dr["Relation_Product_Name"].ToString().Split(',');
                    var cmsmap = new CmsMap();
                    cmsmap.Title = dr["TITLE"].ToString();
                    cmsmap.RECORD_ID = dr["RECORD_ID"].ToString();
                    cmsmap.Context = esUtilities.StringUtility.striphtml(dr["ABSTRACT"].ToString());
                    cmsmap.ImageUrl = dr["RECORD_IMG"].ToString();
                    cmsmap.CmsApps = dr["AP_TYPE"].ToString().Split(',').ToList();
                    foreach (var m in models)
                    {
                        if (ms.Contains(m,(new myContains())))
                            cmsmap.Models.Add(m);
                    }
                    list.Add(cmsmap);
                }
            }
            return list;
        }

        protected string showImag(object img)
        {
            if (img != null && !string.IsNullOrEmpty(img.ToString().Trim()))
            {
                return string.Format("<img src=\"{0}\" style=\"width:120px\" lazysrc=\"{1}\" class=\"lazyImg\" />"
                        , ResolveUrl("~/images/Loading.gif"), img.ToString());
            }

            return "";
        }

        protected string megerCate(object obj)
        { 
            if(obj != null)
            {
                List<string> cates = new List<string>();
                CmsMap cms = obj as CmsMap;
                foreach(var m in cms.Models)
                {
                    foreach(var c in  categoryModels)
                    {
                        if(!cates.Contains(c.Key,(new myContains())) && c.Value.Contains(m,(new myContains())))
                            cates.Add(c.Key);
                    }
                }
                return string.Join(",",cates);
            }
            return "";
        }
    }

    public class myContains : IEqualityComparer<string>
    {

        public bool Equals(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
                return false;
            return x.Trim().Equals(y.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return 0;
        }
    }


    public class CmsMap
    {
        private string _RECORD_ID;

        public string RECORD_ID
        {
            get { return _RECORD_ID; }
            set { _RECORD_ID = value; }
        }
        

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _context;

        public string Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }


        private List<string> _cmsApps = new List<string>();

        public List<string> CmsApps
        {
            get { return _cmsApps; }
            set { _cmsApps = value; }
        }

        public string cmsAppsStr
        {
            get
            {
                return string.Join(",", CmsApps);
            }
        }

        private List<string> _models = new List<string>();

        public List<string> Models
        {
            get { return _models; }
            set { _models = value; }
        }
        

    }
}