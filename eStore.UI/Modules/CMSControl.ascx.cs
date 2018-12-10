using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class CMSControl : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        #region Property

        public string BAA { get; set; }
        public string CmsType { get; set; }

        private List<POCOS.CMS> _cmsList;
        public List<POCOS.CMS> CmsList
        {
            get
            {
                if (_cmsList == null || _cmsList.Count <= 0)
                {
                    _cmsList = new List<POCOS.CMS>();
                    _cmsList.Add(new POCOS.CMS());
                }
                return _cmsList;
            }
            set { _cmsList = value; }
        }

        private bool isShowPic = true;
        /// <summary>
        /// 是否需要显示图片 默认为True
        /// </summary>
        public bool IsShowPic
        {
            get { return isShowPic; }
            set { isShowPic = value; }
        }
        private bool _needLogin = true;
        public bool needLogin
        {
            get { return _needLogin; }
            set { _needLogin = value; }
        }

        #endregion

        #region .Net Events

        protected override void OnPreRender(EventArgs e)
        {
            repeaterCMS.DataSource = CmsList;
            repeaterCMS.DataBind();
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ltcmstype.Text = CmsType;
            if (string.IsNullOrEmpty(BAA))
                pbaa.Visible = false;
            else
            {
                pbaa.Visible = true;
                ltbaa.Text = BAA + (CmsList.Count > 0 ? "(" + CmsList.Count + ")" : "");
            }
        }

        public string getUrl(object CMSID, object cmsType,object title)
        {
            if (needLogin && eStore.Presentation.eStoreContext.Current.User == null)
                return "<a href='#' class='needlogin'>" + title.ToString() + "</a>";
            else
                return string.Format("<span class='hread'><a href='/CMS/CmsDetail.aspx?CMSID={0}&CMSType={1}' target='_blank'>{2}</a></span>", CMSID.ToString(), cmsType.ToString(), "Read More");
        }

        #endregion


        #region Repeter Events

        protected void repeaterCMS_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.CMS item = e.Item.DataItem as POCOS.CMS;
                Literal ltCmsDetailContext = e.Item.FindControl("ltCmsDetailContext") as Literal;
                ltCmsDetailContext.Text = esUtilities.StringUtility.substringToHTML(string.IsNullOrEmpty(item.contentX) ? item.ABSTRACT : item.contentX, 300, "...");
            }
        }

        public string getCmsView(object obj)
        {
            if (obj == null)
                return "";
            string link = obj.ToString();
            if (string.IsNullOrEmpty(link.ToLower().Replace("http://", "")))
                return "";
            else
                return string.Format("<a href='{0}' target='_blank' class='eStore_btn'>View</a>", link);
        }

        #endregion

        #region Button Events

        #endregion

        #region BindData

        #endregion

        #region Business

        #endregion
    }
}