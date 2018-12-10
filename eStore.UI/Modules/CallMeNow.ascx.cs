using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;
using esUtilities;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class CallMeNow : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private bool _enableState;
        public bool EnableState
        {
            get { return _enableState; }
            set { _enableState = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.CountrySelector1.EnableState = EnableState;
            bindFonts();
            if (!IsPostBack && Presentation.eStoreContext.Current.User != null)
            {
                txtEmail.Text = Presentation.eStoreContext.Current.User.actingUser.UserID;
                this.txtFirstName.Text = Presentation.eStoreContext.Current.User.actingUser.FirstName;
                this.txtLastName.Text = Presentation.eStoreContext.Current.User.actingUser.LastName;
                this.txtPhone.Text = Presentation.eStoreContext.Current.User.actingUser.TelNo;
                this.txtExt.Text = Presentation.eStoreContext.Current.User.actingUser.TelExt;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (UVerification1.VerificationUser())
            {
                POCOS.UserRequest requestCallMeNow = new POCOS.UserRequest(Presentation.eStoreContext.Current.Store.profile, POCOS.UserRequest.ReqType.CallMeNow);
                requestCallMeNow.Email = txtEmail.Text.Trim();
                requestCallMeNow.Country = getCountry();
                requestCallMeNow.State = this.CountrySelector1.State;
                requestCallMeNow.Telephone = this.txtPhone.Text + "-" + this.txtExt.Text;
                requestCallMeNow.FirstName = this.txtFirstName.Text.Trim();
                requestCallMeNow.LastName = this.txtLastName.Text.Trim();
                requestCallMeNow.Email = this.txtEmail.Text.Trim();
                requestCallMeNow.Comment = Request.RawUrl;
                requestCallMeNow.save();

                Dictionary<String, String> messageInfo = new Dictionary<string, string>();

                EMailNoticeTemplate mailer = new EMailNoticeTemplate(Presentation.eStoreContext.Current.Store); // TODO: Initialize to an appropriate value
                messageInfo.Add("Country", getCountry());
                messageInfo.Add("Name", Presentation.eStoreContext.Current.Store.getCultureGreetingName(this.txtFirstName.Text.Trim(), this.txtLastName.Text.Trim()));
                messageInfo.Add("Phone", this.txtPhone.Text);
                messageInfo.Add("Ext", this.txtExt.Text);
                messageInfo.Add("EMail", !string.IsNullOrEmpty(txtEmail.Text.Trim()) ? txtEmail.Text.Trim() : Presentation.eStoreContext.Current.User == null ? string.Empty : Presentation.eStoreContext.Current.User.actingUser.UserID);
                messageInfo.Add("ContactEMail", !string.IsNullOrEmpty(txtEmail.Text.Trim()) ? txtEmail.Text.Trim() : string.Empty);
                EMailReponse respose = mailer.getCallMeNowContent(messageInfo, Presentation.eStoreContext.Current.Store, getCountry(), eStoreContext.Current.CurrentLanguage);

                Presentation.eStoreContext.Current.AddStoreErrorCode("Request confirmation message");
            }
            else
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "showpopCallMeDialog", "$(function() { popCallMeDialog();});", true);
        }

        private string getCountry()
        {
            return !string.IsNullOrEmpty(this.CountrySelector1.CountryCode) ? this.CountrySelector1.CountryCode : this.CountrySelector1.Country;
        }

        protected void bindFonts()
        {
            txtFirstName.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
            txtPhone.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Phone);
            ltTalk_now.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Talk_with_an_Associate_Now);
            ltInfor_below.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Please_enter_your_information_below);
            ltFirestName.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
            ltLastName.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);
            ltPhone.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Phone);
            ltExt.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Ext);
            ltEmail.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eMail);
            ltWillKeep.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_We_respect_your_privacy_and_will_keep);
            ltCopyRight.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Copyright_Advantech);
        }
    }
}