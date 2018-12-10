using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class AddressBook : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private POCOS.User currentuser;
        public POCOS.User CurrentUser
        {
            get
            {
                if (this.currentuser == null)
                {
                    this.currentuser = Presentation.eStoreContext.Current.User.actingUser;
                }
                return this.currentuser;
            }
            set
            {
                this.currentuser = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current == null)
                {
                    Response.Redirect(Request.ApplicationPath);
                }
                Presentation.VModles.Member.VLoginDialog LogInfor = new Presentation.VModles.Member.VLoginDialog
                {
                    logInfor = Presentation.eStoreIoc.Resolve<Presentation.VModles.Member.LogInfor>()
                };
                lb_EditProfile.PostBackUrl = string.Format(LogInfor.logInfor.EditLink
                    , Presentation.eStoreContext.Current.storeMembershippass
                    , CurrentUser.UserID
                    , Presentation.eStoreContext.Current.Store.profile.StoreLangID
                    , CurrentUser.authKey
                    , "Go eStore"
                    , CurrentUrlEncodePath.Replace(HttpUtility.UrlEncode("?sso=y"), string.Empty)
                    , Presentation.eStoreContext.Current.BusinessGroup
                    );

                if (!string.IsNullOrEmpty(Request["sso"]) && Request["sso"].ToString() == "y")
                {
                    POCOS.User ssoUSer = Presentation.eStoreContext.Current.Store.ssoLogin(Presentation.eStoreContext.Current.getUserIP(), CurrentUser.authKey, CurrentUser.UserID);
                    if (ssoUSer != null)
                    {
                        if (Presentation.eStoreContext.Current.User.UserID == ssoUSer.UserID)
                        {
                            Presentation.eStoreContext.Current.User = ssoUSer;
                        }
                        else
                        {
                            if (Presentation.eStoreContext.Current.User.hasRight(POCOS.User.ACToken.SwitchRole))
                            {
                                Presentation.eStoreContext.Current.User.switchRole(POCOS.User.Role.Employee, ssoUSer.UserID);
                            }
                            else
                            {
                                Presentation.eStoreContext.Current.User = ssoUSer;
                            }
                        }
                        CurrentUser = Presentation.eStoreContext.Current.User.actingUser;
                 
                    }
                }

                this.ContactList1.eStoreContacts = this.ContactList1.geteStoreContacts();
                this.SetLinkButtonStatus(false);
                bindFonts();
            }
        }

        private void bindFonts()
        {
            this.lb_EditProfile.Text = Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.Cart_Edit);
            this.lb_EditAddressBook.Text = Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.Cart_Edit);
            this.lb_AddAddressBook.Text = Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.Create);
            this.lb_AddorUpdate.Text = Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.Cart_Save);
            this.lb_Cancel.Text = Presentation.eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.Cart_Cancel);
        }

        private void SetLinkButtonStatus(bool status)
        {
            this.ContactList1.Visible = !status;
            this.ContatcDetails1.Visible = status;

            this.lb_EditAddressBook.Enabled = !status;
            this.lb_EditAddressBook.Visible = !status;
            this.lb_AddAddressBook.Enabled = !status;
            this.lb_AddAddressBook.Visible = !status;

            this.lb_AddorUpdate.Enabled = status;
            this.lb_AddorUpdate.Visible = status;
            this.lb_Cancel.Enabled = status;
            this.lb_Cancel.Visible = status;
        }

        protected void lb_EditAddressBook_Click(object sender, EventArgs e)
        {
            if (this.Request.Form["AddressBook"] != null)
            {
                Presentation.eStoreContact ec = this.eStoreContacts.FirstOrDefault(x => x.AddressID == Request.Form["AddressBook"].ToString());
                if (ec != null)
                {
                    this.ContatcDetails1.Contact = ec;
                    this.SetLinkButtonStatus(true);
                }
            }
            
        }

        protected void lb_AddAddressBook_Click(object sender, EventArgs e)
        {
            this.ContatcDetails1.Contact = null;
            this.SetLinkButtonStatus(true);
        }

        protected void lb_AddorUpdate_Click(object sender, EventArgs e)
        {
            this.ContatcDetails1.SaveOrUpdate();
            this.SetLinkButtonStatus(false);
            this.ContactList1.eStoreContacts = this.ContactList1.geteStoreContacts();
        }

        protected void lb_Cancel_Click(object sender, EventArgs e)
        {
            this.ContatcDetails1.Contact = null;
            this.SetLinkButtonStatus(false);
        }

        private List<Presentation.eStoreContact> _eStoreContacts;
        public List<Presentation.eStoreContact> eStoreContacts
        {
            get
            {
                _eStoreContacts = new List<Presentation.eStoreContact>();
                foreach (POCOS.Contact contact in CurrentUser.Contacts)
                {
                    Presentation.eStoreContact UserContact = _eStoreContacts.Find(x => x.AddressID == contact.AddressID);
                    if (UserContact == null)
                    {
                        if (contact.AddressID == CurrentUser.CompanyID)
                        {
                            POCOS.Contact tmpContact = new POCOS.Contact();
                            tmpContact = contact;
                            tmpContact.CreatedDate = DateTime.Now;
                            UserContact = new Presentation.eStoreContact(tmpContact);
                        }
                        else
                        { UserContact = new Presentation.eStoreContact(contact); }

                        _eStoreContacts.Add(UserContact);
                    }
                }
                return _eStoreContacts;
            }
        }
    }
}