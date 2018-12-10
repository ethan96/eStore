using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ContactDetails : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private string _validationGroup = "ContactDetails";
        public string ValidationGroup
        {
            get { return _validationGroup; }
            set { _validationGroup = value; }
        }
        

        private  Presentation.eStoreContact _Contact;
        public Presentation.eStoreContact Contact
        {
            get
            {
                if (_Contact == null)
                {
                    _Contact = new Presentation.eStoreContact();
                    _Contact.CreatedDate = DateTime.Now;
                }
                //_Contact.Attention = this.txtAttention.Text;
                _Contact.FirstName = this.CultureFullName1.FirstName;
                _Contact.LastName = this.CultureFullName1.LastName;
                _Contact.AttCompanyName = this.txtAttCompanyName.Text;
                _Contact.ZipCode = this.txtZipCode.Text;
                _Contact.Address1 = this.txtAddress1.Text;
                _Contact.Address2 = "";
                _Contact.FaxNo = this.txtFaxNo.Text;
                _Contact.TelNo = this.txtTelNo.Text;
                _Contact.TelExt = this.txtTelExt.Text;
                _Contact.Mobile = this.txtMobile.Text;
                _Contact.City = this.txtCity.Text;
                _Contact.Country = this.CountrySelector1.Country;
                _Contact.CountryCode = this.CountrySelector1.CountryCode;
                _Contact.State = this.CountrySelector1.State;

                _Contact.LegalForm = this.txtlegalForm.Text;

                if (Presentation.eStoreContext.Current.getBooleanSetting("EnableVATSetting"))
                {
                    _Contact.VATNumbe = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATNumbe; //新版没有Contact中没有vat的修改，使用cart页面中最新的。cart页面中有设置
                    _Contact.VATValidStatus = Presentation.eStoreContext.Current.User.actingUser.mainContact.VATValidStatus;
                }
                

                switch (hcontactType.Value)
                {

                    case "CartContact":
                        _Contact.contactType = Presentation.ContactType.CartContact;
                        break;
                    case "VSAPCompany":
                        _Contact.contactType = Presentation.ContactType.VSAPCompany;
                        break;
                    case "Customer":
                        _Contact.contactType = Presentation.ContactType.Customer;
                        break;
                  default:
                        _Contact.contactType = Presentation.ContactType.Contact;
                        break;
                }

                _Contact.AddressID = string.IsNullOrEmpty(this.hAddressID.Value) ? string.Empty : this.hAddressID.Value;

                //setContact(new Presentation.eStoreContact(), true);

                return _Contact;
            }

            set
            {
                if (value != null)
                {
                    //_Contact.Attention = this.txtAttention.Text;
                    this.CultureFullName1.FirstName = value.FirstName;
                    this.CultureFullName1.LastName = value.LastName;
                    this.txtAttCompanyName.Text = value.AttCompanyName;
                    this.txtZipCode.Text = value.ZipCode;
                    this.txtAddress1.Text = value.Address1;
                    //_Contact.Address2 = this.txtAddress2.Text;
                    this.txtFaxNo.Text = value.FaxNo;
                    this.txtTelNo.Text = value.TelNo;
                    this.txtTelExt.Text = value.TelExt;
                    this.txtMobile.Text = value.Mobile;
                    this.txtCity.Text = value.City;
                    this.CountrySelector1.Country = value.Country;
                    this.CountrySelector1.CountryCode = value.CountryCode;
                    this.CountrySelector1.State = value.State;

                    this.txtlegalForm.Text = value.LegalForm;
                    this.txtVAT.Text = value.VATNumbe;

                    //POCOS.Contact.VATValidResult status = POCOS.Contact.VATValidResult.UNKNOW;
                    //Enum.TryParse(this.hDetailVATValidStatus.Value, out status);
                    //_Contact.VATValidStatus = status;
                    //this.hDetailVATValidStatus.Value = value.VATValidStatus.ToString();

                    //bool isselect;
                    //if (bool.TryParse(this.hIsBillto.Value, out isselect))
                    //    _Contact.isBillTo = isselect;
                    //else
                    //    _Contact.isBillTo = false;

                    //if (bool.TryParse(this.hIsShippto.Value, out isselect))
                    //    _Contact.isShipTo = isselect;
                    //else
                    //    _Contact.isShipTo = false;

                    //if (bool.TryParse(this.hIsSoldto.Value, out isselect))
                    //    _Contact.isSoldTo = isselect;
                    //else
                    //    _Contact.isSoldTo = false;


                    switch (value.contactType)
                    {
                        case Presentation.ContactType.CartContact:
                            hcontactType.Value = Presentation.ContactType.CartContact.ToString();
                            break;
                        case Presentation.ContactType.VSAPCompany:
                            hcontactType.Value = Presentation.ContactType.VSAPCompany.ToString();
                            break;
                        case Presentation.ContactType.Customer:
                            hcontactType.Value = Presentation.ContactType.Customer.ToString();
                            break;
                        default:
                            hcontactType.Value = Presentation.ContactType.Contact.ToString();
                            break;
                    }

                    this.hAddressID.Value = string.IsNullOrEmpty(value.AddressID) ? string.Empty : value.AddressID;

                }
                else
                {
                    //_Contact.Attention = this.txtAttention.Text;
                    this.CultureFullName1.FirstName = string.Empty;
                    this.CultureFullName1.LastName = string.Empty;
                    this.txtAttCompanyName.Text = string.Empty;
                    this.txtZipCode.Text = string.Empty;
                    this.txtAddress1.Text = string.Empty;
                    //_Contact.Address2 = this.txtAddress2.Text;
                    this.txtFaxNo.Text = string.Empty;
                    this.txtTelNo.Text = string.Empty;
                    this.txtTelExt.Text = string.Empty;
                    this.txtMobile.Text = string.Empty;
                    this.txtCity.Text = string.Empty;
                    this.CountrySelector1.Country = string.Empty;
                    this.CountrySelector1.CountryCode = string.Empty;
                    this.CountrySelector1.State = string.Empty;

                    this.txtlegalForm.Text = string.Empty;
                    this.txtVAT.Text = string.Empty;

                    //this.hDetailVATValidStatus.Value = string.Empty;
                    this.hcontactType.Value = string.Empty;
                    this.hAddressID.Value = string.Empty;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bindFonts();
        }

        public void setContact(Presentation.eStoreContact contact,bool bindData=false)
        {
            _Contact = contact;
            if (contact != null && !bindData)
            { 
                if(contact.isBillTo)
                    ltTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Bill_to_address);
                else if(contact.isShipTo)
                    ltTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Ship_to_address);
                else if(contact.isSoldTo)
                    ltTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Sold_to_address);
            }
            if (bindData)
            {
                //this.txtAttention.Text = string.IsNullOrEmpty(_Contact.Attention) ? string.Empty : _Contact.Attention;
                this.CultureFullName1.FirstName = string.IsNullOrEmpty(_Contact.FirstName) ? string.Empty : _Contact.FirstName;
                this.CultureFullName1.LastName = string.IsNullOrEmpty(_Contact.LastName) ? string.Empty : _Contact.LastName;
                this.txtAttCompanyName.Text = string.IsNullOrEmpty(_Contact.AttCompanyName) ? string.Empty : _Contact.AttCompanyName;
                this.txtZipCode.Text = string.IsNullOrEmpty(_Contact.ZipCode) ? string.Empty : _Contact.ZipCode;
                this.txtAddress1.Text = string.IsNullOrEmpty(_Contact.Address1) ? string.Empty : _Contact.Address1;
                //this.txtAddress2.Text = string.IsNullOrEmpty(_Contact.Address2) ? string.Empty : _Contact.Address2;
                this.txtFaxNo.Text = string.IsNullOrEmpty(_Contact.FaxNo) ? string.Empty : _Contact.FaxNo;
                this.txtTelNo.Text = string.IsNullOrEmpty(_Contact.TelNo) ? string.Empty : _Contact.TelNo;
                this.txtTelExt.Text = string.IsNullOrEmpty(_Contact.TelExt) ? string.Empty : _Contact.TelExt;
                this.txtMobile.Text = string.IsNullOrEmpty(_Contact.Mobile) ? string.Empty : _Contact.Mobile;
                this.txtCity.Text = string.IsNullOrEmpty(_Contact.City) ? string.Empty : _Contact.City;
                 this.hcontactType.Value=  _Contact.contactType.ToString();
                this.hAddressID.Value= string.IsNullOrEmpty(_Contact.AddressID) ? string.Empty : _Contact.AddressID;

                this.CountrySelector1.CountryCode = _Contact.CountryCode;
                this.CountrySelector1.State = _Contact.State;

                this.txtlegalForm.Text = _Contact.LegalForm;
                this.txtVAT.Text = _Contact.VATNumbe;
                //this.hDetailVATValidStatus.Value = ((int)_Contact.VATValidStatus).ToString();
                
            }
        }

        protected void bindFonts()
        {
       
            lAttCompanyName.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Company_Name);
            lAddress1.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address_1);
            txtAddress1.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address_1);
            //lAddress2.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address_2);
            lCity.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_City);
            txtCity.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_City);
            lZipCode.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_ZipCode);
            txtZipCode.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_ZipCode);
            lTelNo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_TelNo);
            txtTelNo.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_TelNo);
            lFaxNo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_FaxNo);
            lMobile.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Mobile);
            ltTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address_Information);
        }

        public void SaveOrUpdate()
        {
            Presentation.eStoreContact contact = this.Contact;
            if (contact.contactType == Presentation.ContactType.Contact || contact.contactType == Presentation.ContactType.CartContact)
            {
                if (String.IsNullOrEmpty(contact.AddressID))    //new contact
                {
                    POCOS.Contact cNew = Presentation.eStoreContext.Current.User.actingUser.addContact(contact.AttCompanyName, contact.FirstName, contact.LastName, contact.Country, contact.State, contact.City, contact.ZipCode, contact.Address1, contact.TelNo, contact.Address2, contact.TelExt);
                    cNew.FaxNo = contact.FaxNo;
                    cNew.Mobile = contact.Mobile;
                    cNew.Address2 = contact.Address2;

                    cNew.VATNumbe = contact.VATNumbe;
                    cNew.LegalForm = contact.LegalForm;
                    cNew.VATValidStatus = contact.VATValidStatus;

                    Presentation.eStoreContext.Current.User.save();
                    contact.AddressID = cNew.AddressID;
                }
                else   //updating existing contact
                {
                    POCOS.Contact cNew = Presentation.eStoreContext.Current.User.actingUser.updateContact(contact.AddressID, contact.AttCompanyName, contact.FirstName, contact.LastName, contact.Country, contact.State, contact.City, contact.ZipCode, contact.Address1, contact.TelNo, contact.Address2, contact.TelExt);
                    cNew.FaxNo = contact.FaxNo;
                    cNew.Mobile = contact.Mobile;
                    cNew.Address2 = contact.Address2;

                    cNew.VATNumbe = contact.VATNumbe;
                    cNew.LegalForm = contact.LegalForm;
                    cNew.VATValidStatus = contact.VATValidStatus;

                    Presentation.eStoreContext.Current.User.save();
                    if (contact.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID)
                    {
                        Presentation.eStoreContext.Current.Store.syncStoreUserToSSOUserProfile(Presentation.eStoreContext.Current.User.actingUser, BusinessModules.Store.SSO_Update_Type.Contact);
                        //Presentation.eStoreContext.Current.User.updateSSOContact(User.SSO_Update_Type.Contact);
                    }
                }
            }


        }
    }
}