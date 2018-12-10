using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using eStore.BusinessModules;

namespace eStore.UI.Modules
{
    public partial class CMSTab : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        #region DataModle

        

        #endregion


        private List<CMSManager.DataModle> _dataSorce;
        /// <summary>
        /// Data Souces
        /// </summary>
        public List<CMSManager.DataModle> DataSorce
        {
            get { return _dataSorce; }
            set { _dataSorce = value; }
        }
        private bool _needLogin=true;
        public bool needLogin
        {
            get { return _needLogin; }
            set { _needLogin = value; }
        }

        public override void DataBind()
        {
            CMSManager.DataModle modle = DataSorce.FirstOrDefault();
            if (modle != null && modle.DataSorce != null && modle.DataSorce.Count > 0)
            {
                CMSControl cmsC = (CMSControl)LoadControl("~/Modules/CMSControl.ascx");
                cmsC.needLogin = this.needLogin;
                cmsC.CmsList = modle.DataSorce;
                cmsC.IsShowPic = modle.IsShowPic;
                cmsC.BAA = modle.Baa;
                cmsC.CmsType = modle.CmsDisplayName;
                PlaceHolder1.Controls.Add(cmsC);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}