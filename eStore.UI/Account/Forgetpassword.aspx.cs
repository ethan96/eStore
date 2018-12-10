using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Account
{
    public partial class Forgetpassword : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btSendMail.Text = eStore.Presentation.eStoreLocalization.Tanslation("eStore_Reset_my_password");
            }
        }

        protected void btSendMail_Click(object sender, EventArgs e)
        {
            if (!UVerification1.VerificationUser())
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Verification_Code_Error"));
                return;
            }
            var userid = esUtilities.StringUtility.replaceSpecialString(tbEmail.Text.Trim(), true);
            if (esUtilities.StringUtility.CheckEmail(userid))
            {
                POCOS.User user = eStore.Presentation.eStoreContext.Current.Store.getUser(userid);
                if (user == null || (user != null && user.newUser))
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Can_not_find_user"), userid));
                    return;
                }
                user.FollowUpComments = Guid.NewGuid().ToString();
                user.LastUpdated = DateTime.Now;
                user.save();

                esUtilities.EMailReponse response = null;
                eStore.BusinessModules.EMailNoticeTemplate mailTemplate = new BusinessModules.EMailNoticeTemplate(Presentation.eStoreContext.Current.Store);
                response = mailTemplate.SendForgotPassWordEmail(user
                    , eStore.Presentation.eStoreLocalization.Tanslation("eStore_ForgotPassWord_Subjec")
                    , eStore.Presentation.eStoreContext.Current.Store
                    , eStore.Presentation.eStoreContext.Current.CurrentLanguage
                    , eStore.Presentation.eStoreContext.Current.MiniSite);
                if (response != null && response.ErrCode == esUtilities.EMailReponse.ErrorCode.NoError)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_An_email_is_sent_to_your_inbox"));
                    //Response.Redirect("/");
                }
                else
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(string.Format(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Can_not_send_email_to"), userid));
                    return;
                }
            }
            else
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation("eStore_Please_enter_a_valid_email_address"));
            }
        }
    }
}