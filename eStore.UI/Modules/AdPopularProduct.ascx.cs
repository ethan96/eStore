using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class AdPopularProduct : System.Web.UI.UserControl
    {
        public string PopularType { get; set; }

        public string SproductId { get; set; }

        public string KeyWord
        {
            get {
                string _keyWrod = string.Empty;
                if (string.IsNullOrEmpty(SproductId) && PopularType.ToUpper() == "PRODUCT" && Request["ProductID"] != null)
                    SproductId = Request["ProductID"];

                _keyWrod = "&type=" + PopularType.ToLower() + "&keyword=" + SproductId;

                //获取 源product或者category
                string sourceProduct = SproductId;
                if (PopularType.ToUpper() == "CATEGORY" && Request["category"] != null)
                    sourceProduct = Request["category"];

                _keyWrod += "&sourceKey=" + sourceProduct.ToUpper();
                return _keyWrod;
            }
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Request["display"] != null && Request["display"].ToLower() == "productlist")
                //    MostProductContainer.Attributes.Add("class", "RecentPopularProduct RecentPopularProduct_Gray");
            }
        }
        //是否显示 Popular Model
        public bool isShowPopularModel()
        {
            bool isShow = false;
            
            if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct"))
            {
                //暂时不需要判断奇偶数。
                //if (getPopularFromCookie())
                //    isShow = true;
                //else
                //{
                //    string uniqueNo = Session.SessionID.Substring(Session.SessionID.Length - 1);
                //    int numberId = 0;
                //    if (!int.TryParse(uniqueNo, out numberId))
                //    {                        
                //        char cUniqueNo = char.Parse(uniqueNo.ToUpper());
                //        numberId = Convert.ToInt32(cUniqueNo) - 64;//获取 A-Z 对应的1-26
                //    }
                //    numberId = numberId % 2;
                //    if (numberId == 0)
                //        isShow = true;
                //}
                isShow = true;
            }

            return isShow;
        }
        //查看cookie中是否有点击Model.  如果以前有 显示并点击过Popular Model, 那么继续显示
        private bool getPopularFromCookie()
        {
            bool isClick = false;
            HttpCookie modelCookie = Request.Cookies[Request.Url.Host + "_PopularModel"];
            if (modelCookie != null && modelCookie.Values.Count >= 1)
            {
                string popualrModelNo = modelCookie.Values["popualrModel"];
                if (!string.IsNullOrEmpty(popualrModelNo))
                    isClick = true;
            }
            return isClick;
        }

        //点击 推送视窗, 写cookie
        public void setPopularCookie(string sproductId)
        {
            HttpCookie modelCookie = new HttpCookie(Request.Url.Host + "_PopularModel");
            modelCookie.Values["popualrModel"] = sproductId;
            modelCookie.Expires = DateTime.Now.AddMonths(6);
            Response.Cookies.Add(modelCookie);
        }
    }
}