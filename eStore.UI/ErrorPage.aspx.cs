using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class ErrorPage : Presentation.eStoreBaseControls.eStoreBasePage
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["message"]))
                this.lmessage.Text = Request["message"];
            else
                lmessage.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Please_be_patient);
            try
            {
                var _page = Context.Handler as Presentation.eStoreBaseControls.eStoreBasePage;
                if (_page != null && _page.UserActivitLog != null)
                {
                    this.UserActivitLog.ProductID = _page.UserActivitLog.ProductID;
                    this.UserActivitLog.CategoryType = _page.UserActivitLog.CategoryType;
                    this.UserActivitLog.CategoryName = _page.UserActivitLog.CategoryName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}