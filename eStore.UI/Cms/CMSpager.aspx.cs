using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using eStore.POCOS.DAL;

namespace eStore.UI
{
    public partial class CMSpager : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        int advid = 0; int item = 0;
        private string cmsType;
        string baa = "";
        POCOS.Advertisement.ResourcesContent resource;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.AddStyleSheet("/Styles/CMS.css");
            this.BindScript("url", "pic", "pic.js");
            if (!string.IsNullOrEmpty(Request.QueryString["CMSType"]))
                cmsType = Request.QueryString["CMSType"];
            if (!string.IsNullOrEmpty(Request.QueryString["baa"]))
                baa = HttpUtility.UrlDecode(Request.QueryString["baa"]);
            if (!string.IsNullOrWhiteSpace(Request.QueryString["advid"]) && !string.IsNullOrWhiteSpace(Request.QueryString["item"]))
            { 
                 
                 if (  int.TryParse(Request.QueryString["advid"], out advid) && int.TryParse(Request.QueryString["item"], out item) )
                 {
                     POCOS.Advertisement  adv = eStore.Presentation.eStoreContext.Current.Store.getAdByID(advid);
                     if (adv != null && adv.complexAdvertisementContent is POCOS.Advertisement.ResourcesContent) {
                         resource = (POCOS.Advertisement.ResourcesContent)adv.complexAdvertisementContent;
                         baa = resource.BusinessApArea;
                         cmsType = resource.CMSResourceTypes[item];
                         //needlogin  = resource.CMSResourceTypesIsShow[item];
                     }
                 }
            }
            CMSType _cmsType;
            if (string.IsNullOrWhiteSpace(cmsType) || !Enum.TryParse(cmsType.Trim().Replace(" ", "_").Replace("/", "Slash"), out _cmsType))
                return;

            List<CMSManager.DataModle> ls = new List<CMSManager.DataModle>();
            if (resource != null)
            {
                if (resource.PisCategorys != null && resource.PisCategorys.Count > 0)
                    ls.Add(eStore.Presentation.eStoreContext.Current.Store.getCms(_cmsType, resource.PisCategorys));
                else
                    ls.Add(eStore.Presentation.eStoreContext.Current.Store.getCms(_cmsType, resource.BusinessApArea)); //如果有其他选项卡继续添加
            }
            else
                ls.Add(eStore.Presentation.eStoreContext.Current.Store.getCms(_cmsType, baa));

            bindData(ls);
            string baaInfo = string.IsNullOrEmpty(baa) ? "" : " - " + baa;
            string pageTitle = $"{cmsType}{baaInfo} - {Presentation.eStoreContext.Current.Store.profile.StoreName}";
            this.isExistsPageMeta = this.setPageMeta(pageTitle, pageTitle,$"{cmsType},{baa}");
        }


        protected void bindData(List<CMSManager.DataModle> ls)
        {
            CMSTab1.DataSorce = ls;
            CMSTab1.needLogin = (resource != null && resource.CMSResourceTypesIsShow.Any() && resource.CMSResourceTypesIsShow.Count() > item) ? resource.CMSResourceTypesIsShow[item] : false;
            CMSTab1.DataBind();
        }

    }
}