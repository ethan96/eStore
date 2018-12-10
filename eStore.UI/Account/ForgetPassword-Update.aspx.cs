using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Account
{
    public partial class ForgetPassword_Update : Presentation.eStoreBaseControls.eStoreBasePage
    {
        POCOS.User user = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnSubmit.Text = eStore.Presentation.eStoreLocalization.Tanslation("eStore_Update_Account");
            }
            var userid = esUtilities.StringUtility.replaceSpecialString(Request.QueryString["email"], true);
            var tempid = esUtilities.StringUtility.replaceSpecialString(Request.QueryString["tempid"]);
            if (esUtilities.StringUtility.CheckEmail(userid))
            {
                user = eStore.Presentation.eStoreContext.Current.Store.getUser(userid);
                if (user == null)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Can_not_find_user"), userid));
                    return;
                }
                if (tempid != user.FollowUpComments)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Email_already_past_or_time_out"));
                    //Response.Redirect("/");
                    return;
                }
                if (user.LastUpdated.GetValueOrDefault().AddHours(1) < DateTime.Now)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStoreTime_out"));
                    return;
                }
            }
            else
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Can_not_find_user"), userid));
                return;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            user.LoginPassword = esUtilities.StringUtility.StringEncry(tbpassword.Text.Trim());
            user.LastUpdated = DateTime.Now;
            user.FollowUpComments = "";
            if (user.save() == 0)
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Update_success_please_try_login"));
            }
            else
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Update_message_error"));
            //Response.Redirect("/");
        }
    }

    
}