using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.BusinessModules;
using eStore.POCOS.DAL;

namespace eStore.UI
{
    public partial class CmsDetail : Presentation.eStoreBaseControls.eStoreBasePage
    {
        private CMS _cms;
        public CMS Cms
        {
            get
            {
                if (_cms == null && !string.IsNullOrEmpty(Request.QueryString["CMSID"]))
                    _cms = Presentation.eStoreContext.Current.Store.getCMSByID(Request.QueryString["CMSID"]);
                return _cms;
            }
        }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindDefaultInfo();
                if (Cms != null && string.IsNullOrEmpty(Cms.HYPER_LINK.ToLower().Replace("http://", "")))
                    hlDetails.Visible = false;
                else
                {
                    hlDetails.NavigateUrl = Cms.HYPER_LINK;
                    hlDetails.Target = "_blank";
                    hlDetails.Visible = true;
                }
            }
            ltAppCate.Text = string.Format("<div class='iot-storiesArticleCategory'>{0}{1}</div>"
                    , string.IsNullOrEmpty(Request.QueryString["app"]) ? "" : "<span>" + Request.QueryString["app"] + "</span>"
                    , string.IsNullOrEmpty(Request.QueryString["cate"]) ? "" : "<span>" + Request.QueryString["cate"] + "</span>");
        }

        protected void bindDefaultInfo()
        {
            if (Cms != null)
            {
                if (CMSManager.showPicOrNot(Cms.RECORD_IMG, false))
                {
                    Image1.ImageUrl = Cms.RECORD_IMG;
                    Image1.Visible = true;
                    Image1.AlternateText = Cms.TITLE;
                }
                ltTitle.Text = Cms.TITLE;
                ltContext.Text = string.IsNullOrEmpty(Cms.contentX) ? Cms.ABSTRACT : Cms.contentX;
                ltDate.Text = Cms.LASTUPDATED.ToString();
                Presentation.eStoreContext.Current.keywords.Add("Keywords", Cms.RECORD_ID);
                this.isExistsPageMeta = this.setPageMeta(Cms.TITLE,string.IsNullOrEmpty( Cms.ABSTRACT)?Cms.TITLE:Cms.ABSTRACT, Cms.ABSTRACT);
            }
        }

    }
}