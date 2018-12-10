using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class ContactList : System.Web.UI.UserControl
    {
        private List<Presentation.eStoreContact> _eStoreContacts;
        public List<Presentation.eStoreContact> eStoreContacts
        {
            set
            {
                if (value == null)
                    return;

                this._eStoreContacts = value;

                Presentation.eStoreContact originalContact = value.Where(c => c.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID || c.contactType == Presentation.ContactType.Customer).FirstOrDefault();
                if (originalContact != null)
                {
                    this._eStoreContacts.Remove(originalContact);
                    this._eStoreContacts.Insert(0, originalContact);
                }

                this.rpContact.DataSource = this._eStoreContacts;
                this.rpContact.DataBind();

                this.ddl_CompanyName.Items.Clear();
                foreach (var contact in this._eStoreContacts.GroupBy(c => c.AttCompanyName).Select(c => c.Key).ToList())
                {
                    if (string.IsNullOrEmpty(contact))
                        continue;

                    ListItem li = new ListItem(contact, contact);
                    if (!this.ddl_CompanyName.Items.Contains(li))
                    {
                        this.ddl_CompanyName.Items.Add(li);
                    }
                }
                ddl_CompanyName.Items.Insert(0, new ListItem("All"));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void rpContact_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Presentation.eStoreControls.Repeater rp = (Presentation.eStoreControls.Repeater)e.Item.FindControl("rpRelatedOrders");
                LinkButton lb = e.Item.FindControl("lb_Delete") as LinkButton;
                lb.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Delete);

                Presentation.eStoreContact ec = e.Item.DataItem as Presentation.eStoreContact;
                if (ec.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID || ec.contactType == Presentation.ContactType.Customer)
                {
                    lb.Visible = false;
                    Literal lt = e.Item.FindControl("lt_Default") as Literal;
                    lt.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Default_Value);
                    lt.Visible = true;
                }
            }
        }

        protected void rpContact_OnItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                Presentation.eStoreContact ec = this.geteStoreContacts().FirstOrDefault(x => x.AddressID == e.CommandArgument.ToString());
                this.geteStoreContacts().Remove(ec);
                if (ec.contactType == Presentation.ContactType.Contact)
                {
                    Presentation.eStoreContext.Current.User.actingUser.deleteContact((POCOS.Contact)getContact(ec));
                    Presentation.eStoreContext.Current.User.actingUser.save();
                }
                this.eStoreContacts = this.geteStoreContacts();
            }
        }

        public List<Presentation.eStoreContact> geteStoreContacts()
        {
            _eStoreContacts = new List<Presentation.eStoreContact>();
            foreach (POCOS.Contact contact in Presentation.eStoreContext.Current.User.actingUser.Contacts)
            {
                Presentation.eStoreContact UserContact = _eStoreContacts.Find(x => x.AddressID == contact.AddressID);
                if (UserContact == null)
                {
                    if (contact.AddressID == Presentation.eStoreContext.Current.User.actingUser.CompanyID)
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
            return _eStoreContacts.OrderBy(c => c.AddressID).ToList();
        }

        private object getContact(Presentation.eStoreContact sc)
        {
            object contact = null;
            if (sc == null)
                return null;
            switch (sc.contactType)
            {
                case Presentation.ContactType.Contact:
                    contact = Presentation.eStoreContext.Current.User.actingUser.Contacts.FirstOrDefault(c => c.AddressID == sc.AddressID);
                    break;
                case Presentation.ContactType.CartContact:
                    if (sc != null)
                        contact = sc.toCartContact();
                    break;
                case Presentation.ContactType.VSAPCompany:
                case Presentation.ContactType.Customer:
                    POCOS.Contact newcontact = new POCOS.Contact();
                    newcontact.FirstName = string.IsNullOrEmpty(sc.FirstName) ? string.Empty : sc.FirstName;
                    newcontact.LastName = string.IsNullOrEmpty(sc.LastName) ? string.Empty : sc.LastName;
                    newcontact.AttCompanyName = string.IsNullOrEmpty(sc.AttCompanyName) ? string.Empty : sc.AttCompanyName;
                    newcontact.FaxNo = string.IsNullOrEmpty(sc.FaxNo) ? string.Empty : sc.FaxNo;
                    newcontact.TelNo = string.IsNullOrEmpty(sc.TelNo) ? string.Empty : sc.TelNo;
                    newcontact.TelExt = string.IsNullOrEmpty(sc.TelExt) ? string.Empty : sc.TelExt;
                    newcontact.Mobile = string.IsNullOrEmpty(sc.Mobile) ? string.Empty : sc.Mobile;
                    newcontact.Address1 = string.IsNullOrEmpty(sc.Address1) ? string.Empty : sc.Address1;
                    newcontact.Address2 = string.IsNullOrEmpty(sc.Address2) ? string.Empty : sc.Address2;
                    newcontact.City = string.IsNullOrEmpty(sc.City) ? string.Empty : sc.City;
                    newcontact.State = string.IsNullOrEmpty(sc.State) ? string.Empty : sc.State;
                    newcontact.County = string.IsNullOrEmpty(sc.County) ? string.Empty : sc.County;
                    newcontact.countryX = string.IsNullOrEmpty(sc.Country) ? string.Empty : sc.Country;
                    newcontact.ZipCode = string.IsNullOrEmpty(sc.ZipCode) ? string.Empty : sc.ZipCode;
                    newcontact.AddressID = sc.AddressID;
                    newcontact.UserID = Presentation.eStoreContext.Current.User.actingUser.UserID;
                    contact = newcontact;
                    break;
                default:
                    break;

            }
            return contact;
        }
    }
}