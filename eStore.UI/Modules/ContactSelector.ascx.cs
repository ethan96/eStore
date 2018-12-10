using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.Presentation;
using eStore.BusinessModules;
using eStore.POCOS.DAL;

namespace eStore.UI.Modules
{
    public partial class ContactSelector : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Cart cart { get; set; }
        public String ShoppingCartType { get; set; }

        private bool? _showSoldTo;
        public bool ShowSoldTo
        {
            get
            {

                if (_showSoldTo == null)
                {
                    _showSoldTo = Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.actingUser.actingRole == User.Role.Employee;
                }
                return _showSoldTo ?? false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
                //this.btnAddSAPCompanyToContactList.PostBackUrl = Request.Url.LocalPath;
                if (Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.AccessSAPContact))
                {
                    //lblSapCompany.Visible = true;
                    //SAPContatSelector1.Visible = true;
                    //btnAddSAPCompanyToContactList.Visible = true;

                    //lblFromCustomer.Visible = true;
                    //txtFromCustomer.Visible = true;
                    //btnFromCustomer.Visible = true;
                }
                else
                {
                    //lblSapCompany.Visible = false;
                    //SAPContatSelector1.Visible = false;
                    //btnAddSAPCompanyToContactList.Visible = false;

                    //lblFromCustomer.Visible = false;
                    //txtFromCustomer.Visible = false;
                    //btnFromCustomer.Visible = false;
                }
            }
            bindFonts();
            //this.ckbSapCompany.Visible = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
            //this.ckbFromCustomer.Visible = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
            //this.ckbNewSoldtoContact.Visible = ContactList1.ShowBillTo;
            //this.contactdetialspanel.Attributes.Add("class", "contactdetials hidden");
            //this.ckbNewBilltoContact.Checked = false;
            //this.ckbNewShipptoContact.Checked = false;
            //this.ckbNewSoldtoContact.Checked = false;
            //this.ckbSapCompany.Checked = false;
            //this.ckbFromCustomer.Checked = false;

            psoldto.Visible = ShowSoldTo;

        }
        private void BindData()
        {
            //this.ContactList1.eStoreContacts = geteStoreContacts();
            var contactls = geteStoreContacts().OrderByDescending(c => c.CreatedDate).OrderBy(c => c.AddressID).ToList();
            foreach (var c in contactls)
                c.AttCompanyName = string.IsNullOrEmpty(c.AttCompanyName) ? "N/A" : c.AttCompanyName;
            ddlBillCompany.Items.Clear();
            ddlShipCompany.Items.Clear();
            ddlSoldCompany.Items.Clear();
            var companys = contactls.GroupBy(c => c.AttCompanyName).Select(c => c.Key).ToList();
            foreach (var c in companys)
            {
                if (ddlBillCompany.Items.FindByValue(c) == null)
                {
                    ddlSoldCompany.Items.Add(new ListItem(c, c));
                    ddlShipCompany.Items.Add(new ListItem(c, c));
                    ddlBillCompany.Items.Add(new ListItem(c, c));
                }
            }

            rpContactInforBill.DataSource = contactls;
            rpContactInforBill.DataBind();
            rpContactInforShip.DataSource = contactls;
            rpContactInforShip.DataBind();
            rpContactInforSold.DataSource = contactls;
            rpContactInforSold.DataBind();

            var bill = contactls.FirstOrDefault(c => c.isBillTo);
            var ship = contactls.FirstOrDefault(c => c.isShipTo);
            var sold = contactls.FirstOrDefault(c => c.isSoldTo);
            if (bill != null)
            {
                hIsBillto.Value = bill.AddressID;
                ddlBillCompany.Items.FindByText(bill.AttCompanyName).Selected = true;
            }
            if (ship != null)
            {
                hIsShippto.Value = ship.AddressID;
                ddlShipCompany.Items.FindByText(ship.AttCompanyName).Selected = true;
                if (!IsPostBack)
                    hdShipTo.Value = (ship.AddressID == bill.AddressID).ToString().ToLower();
            }
            if (sold != null)
            {
                hIsSoldto.Value = sold.AddressID;
                ddlSoldCompany.Items.FindByText(sold.AttCompanyName).Selected = true;
                if (!IsPostBack)
                    hfSoldTo.Value = (sold.AddressID == bill.AddressID).ToString().ToLower();
            }
            
        }
        private List<eStoreContact> _eStoreContacts;
        List<eStoreContact> geteStoreContacts()
        {
            if (this.ViewState["eStoreContacts"] == null)
            {
                _eStoreContacts = Presentation.UserContactManager.GeteStoreContacts(cart);
                this.ViewState["eStoreContacts"] = _eStoreContacts;
            }
            else
                _eStoreContacts = (List<eStoreContact>)this.ViewState["eStoreContacts"];
            return _eStoreContacts;
        }
        private void refrashcontacts(List<eStoreContact> ls)
        {
            this.ViewState["eStoreContacts"] = ls;
        }


        protected void btnSaveUserContact_Click(object sender, EventArgs e)
        {
            Button c = sender as Button;
            string addressid = c.CommandArgument.ToString();
            Presentation.eStoreContact contact = ContactDetailsBill.Contact;
            var contactType = this.hdContactType.Value;
            if (!string.IsNullOrEmpty(contactType))
            {
                switch (contactType)
                {
                    case "Bill":
                        contact.isBillTo = true;
                        break;
                    case "Ship":
                        contact.isShipTo = true;
                        break;
                    case "Sold":
                        contact.isSoldTo = true;
                        break;
                    default:
                        break;
                }
            }

            if (contact.contactType == ContactType.Contact || contact.contactType == ContactType.CartContact)
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
            addContactAndSetDefault(contact);
            if (contact.isShipTo || Request["sameAddress"] == "true")
            {
                setCartShipToContact(contact);
                ShippingCalculator ShippingCalculator1 = (ShippingCalculator)this.Parent.FindControl("ShippingCalculator1");
                ShippingCalculator1.CalculateShippingRate();
                ShippingCalculator1.bindOwnCarrierShipping();

                //Check ship to address is correct
                if (eStoreContext.Current.Store.profile.getBooleanSetting("ValidateUSAaddress", false) == true)
                {
                    string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
                    if (!string.IsNullOrEmpty(ShippingCalculator1.shippingMethod))
                        shippingmethods = ShippingCalculator1.shippingMethod.ToUpper();
                    var result = eStoreContext.Current.Store.isValidateUSAShiptoAddress(cart.ShipToContact, shippingmethods, eStoreContext.Current.User);
                    if (result.isValid == false)
                    {
                        if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                            cart.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.POBOX.ToString();
                        else
                            cart.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.Invalid.ToString();
                        eStoreContext.Current.AddStoreErrorCode(eStoreLocalization.Tanslation(result.TranslationKey));
                    }
                    else
                        cart.ShipToContact.ValidationStatus = CartContact.AddressValidationStatus.Valid.ToString();
                }
            }
            else
                setNewContactToCart(contact);
            c.CommandArgument = "";
            BindData();
        }

        private void addContactAndSetDefault(Presentation.eStoreContact contact)
        {
            var ls = this.geteStoreContacts();
            if (contact.isBillTo)
                ls.ForEach(c => c.isBillTo = false);
            if (contact.isShipTo)
                ls.ForEach(c => c.isShipTo = false);
            if (contact.isSoldTo)
                ls.ForEach(c => c.isSoldTo = false);
            Presentation.eStoreContact existsContact = geteStoreContact(contact.AddressID);
            if (existsContact == null)
                ls.Add(contact);
            else
            {
                ls.Remove(existsContact);
                ls.Add(contact);
                refrashcontacts(ls);
            }
        }


        //保存配送方式  sold/bill/ship to
        public bool setCartContact(Presentation.eStoreContact contact = null)
        {
            string BillTo = (contact != null && contact.isBillTo) ? contact.AddressID : this.Request["BillTo"];
            string SoldTo = (contact != null && contact.isSoldTo) ? contact.AddressID : this.Request["SoldTo"];
            string ShipTo = (contact != null && contact.isShipTo) ? contact.AddressID : this.Request["ShipTo"];


            //set bill to
            if (string.IsNullOrEmpty(BillTo))
                Presentation.eStoreContext.Current.AddStoreErrorCode("Select BillTo Address");
            else
                setCartBillToContact(BillTo);

            if (ShowSoldTo)
            {
                string soldSameAddress = this.Request["soldSameAddress"]; // same as bill to
                if (soldSameAddress == "true")
                {
                    if (string.IsNullOrEmpty(BillTo))
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Select ShipTo Address");
                    else
                        setCartSoldToContact(BillTo);
                }
                else
                {
                    //set sold to
                    if (string.IsNullOrEmpty(SoldTo))
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Select SoldTo Address");
                    else
                        setCartSoldToContact(SoldTo);
                }
            }
            else
            {
                setCartSoldToContact(BillTo);
            }

            //set ship to 
            string sameAddress = this.Request["sameAddress"]; // same as bill to
            if (sameAddress == "true")
            {
                if (string.IsNullOrEmpty(BillTo))
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Select ShipTo Address");
                else
                    setCartShipToContact(BillTo);
            }
            else
            {
                if (string.IsNullOrEmpty(ShipTo))
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Select ShipTo Address");
                else
                    setCartShipToContact(ShipTo);
            }

            if (Presentation.eStoreContext.Current.StoreErrorCode.Count > 0)
                return false;
            else
            {
                BindData();
                return true;
            }
        }
        //保存bill to
        public void setCartBillToContact(String addressID)
        {
            //unselect current BillTo
            eStoreContact currentBillTo = geteStoreContacts().FirstOrDefault(ec => ec.isBillTo == true);
            if (currentBillTo != null)
                currentBillTo.isBillTo = false;

            eStoreContact newBillTo = null;
            if (!String.IsNullOrEmpty(addressID))
                newBillTo = geteStoreContact(addressID);

            if (newBillTo != null)
            {
                newBillTo.isBillTo = true;
                cart.setBillTo(getContact(newBillTo));
            }
        }
        //保存 sold to
        public void setCartSoldToContact(String addressID)
        {
            //unselect current SoldTo
            eStoreContact currentSoldTo = geteStoreContacts().FirstOrDefault(ec => ec.isSoldTo == true);
            if (currentSoldTo != null)
                currentSoldTo.isSoldTo = false;

            eStoreContact newSoldTo = null;
            if (!String.IsNullOrEmpty(addressID))
                newSoldTo = geteStoreContact(addressID);

            if (newSoldTo != null)
            {
                newSoldTo.isSoldTo = true;
                cart.setSoldTo(getContact(newSoldTo));
            }
        }
        //保存 ship to
        public void setCartShipToContact(String addressID)
        {
            setCartShipToContact(geteStoreContact(addressID));
        }

        protected eStoreContact getCurrentBillToContact()
        {
            return geteStoreContacts().FirstOrDefault(ec => ec.isBillTo == true);
        }

        protected eStoreContact getCurrentSoldToContact()
        {
            return geteStoreContacts().FirstOrDefault(ec => ec.isSoldTo == true);
        }

        protected eStoreContact getCurrentShipToContact()
        {
            return geteStoreContacts().FirstOrDefault(ec => ec.isShipTo == true);
        }

        protected void setCartShipToContact(eStoreContact newShipTo)
        {
            //unselect current ShipTo
            eStoreContact currentShipTo = geteStoreContacts().FirstOrDefault(ec => ec.isShipTo == true);
            if (currentShipTo != null)
                currentShipTo.isShipTo = false;

            if (newShipTo != null && eStoreContext.Current.Store.isValidatedShiptoAddress(newShipTo.CountryCode, Presentation.eStoreContext.Current.User))
            {
                newShipTo.isShipTo = true;
                cart.setShipTo(getContact(newShipTo));
            }
            else
                Presentation.eStoreContext.Current.AddStoreErrorCode("Please change ShipTo Address");
        }

        protected void btnAddSAPCompanyToContactList_Click(object sender, EventArgs e)
        {
            //POCOS.VSAPCompany VSAPCompany = this.SAPContatSelector1.getSAPCompany();
            //if (VSAPCompany != null)
            //{
            //    this.ContactDetails1.setContact(new eStoreContact(VSAPCompany), true);
            //    this.contactdetialspanel.Attributes.Add("class", "contactdetials");
            //    this.ckbSapCompany.Checked = true;
            //    BindScript("script", "showContactDialog", "$(function() { showContactDialog(); });");
            //}
        }


        public void editContact(eStoreContact sc, string type = "")
        {
            if (sc != null)
            {
                eStoreContact ec = geteStoreContact(sc.AddressID);
                if (ec != null && ec.contactType != sc.contactType)
                    sc.contactType = ec.contactType;

                //this.ckbNewBilltoContact.Checked = sc.isBillTo;
                //this.ckbNewShipptoContact.Checked = sc.isShipTo;
                //this.ckbNewSoldtoContact.Checked = sc.isSoldTo;
                if (string.IsNullOrEmpty(type))
                {
                    sc.isShipTo = (string.IsNullOrEmpty(this.Request["ShipTo"]) == false && this.Request["ShipTo"] == sc.AddressID);
                    sc.isBillTo = (string.IsNullOrEmpty(this.Request["BillTo"]) == false && this.Request["BillTo"] == sc.AddressID);
                    sc.isSoldTo = (string.IsNullOrEmpty(this.Request["SoldTo"]) == false && this.Request["SoldTo"] == sc.AddressID);
                }
                else
                {
                    ContactDetails cd = null;
                    switch (type)
                    { 
                        case "bill":
                            sc.isBillTo = true;
                            cd = this.FindControl("ContactDetailsBill") as ContactDetails;
                            break;
                        case "ship":
                            sc.isShipTo = true;
                            cd = this.FindControl("ContactDetailsShip") as ContactDetails;
                            break;
                        case "sold":
                            sc.isSoldTo = true;
                            cd = this.FindControl("ContactDetailsSold") as ContactDetails;
                            break;
                        default :
                            break;

                    }
                    if(cd != null)
                        cd.setContact(sc, true);
                }
                //this.ContactDetails1.setContact(sc, true);
                //this.contactdetialspanel.Attributes.Add("class", "contactdetials");

                setNewContactToCart();
                BindScript("script", "showContactDialog", "$(function() { showContactDialog(); });");
            }
        }

        /// <summary>
        /// save new cart contact 
        /// 如果 ship to bill to 和 sold to 有变化 将保存到 user cart中
        /// </summary>
        protected void setNewContactToCart(Presentation.eStoreContact contact = null)
        {
            string oldShipToAddressId = string.Empty;
            var shiptocontact = geteStoreContacts().FirstOrDefault(ec => ec.isShipTo == true);
            if(shiptocontact != null)
                oldShipToAddressId = shiptocontact.AddressID;
            if (!string.IsNullOrEmpty(oldShipToAddressId) && setCartContact(contact))
            {
                if (geteStoreContacts().FirstOrDefault(ec => ec.isShipTo == true).AddressID != oldShipToAddressId) // if ship to contact changed will get shipping method again
                {
                    ShippingCalculator ShippingCalculator1 = (ShippingCalculator)this.Parent.FindControl("ShippingCalculator1");
                    ShippingCalculator1.CalculateShippingRate(true);
                    ShippingCalculator1.bindOwnCarrierShipping();
                }
            }
        }

        private eStoreContact geteStoreContact(string AddressID)
        {
            return this.geteStoreContacts().FirstOrDefault(x => x.AddressID == AddressID);
        }
        private object getContact(eStoreContact sc)
        {
            object contact = null;
            if (sc == null)
                return null;
            switch (sc.contactType)
            {
                case ContactType.Contact:
                    contact = Presentation.eStoreContext.Current.User.actingUser.Contacts.FirstOrDefault(c => c.AddressID == sc.AddressID);
                    break;
                case ContactType.CartContact:
                    if (sc != null)
                        contact = sc.toCartContact();
                    break;
                case ContactType.VSAPCompany:
                case ContactType.Customer:
                    POCOS.Contact newcontact = new Contact();
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

        protected void btnFromCustomer_Click(object sender, EventArgs e)
        {
            //POCOS.User customer = Presentation.eStoreContext.Current.Store.getUser(this.txtFromCustomer.Text);
            //if (customer.newUser || customer.mainContact == null)//走sso同步user信息到estore user
            //{
            //    POCOS.User syncUser = Presentation.eStoreContext.Current.Store.syncEstoreUserBySSOUser(customer.UserID);
            //    if (syncUser != null)
            //        customer = Presentation.eStoreContext.Current.Store.getUser(customer.UserID);
            //}

            //if (!customer.newUser && customer.mainContact != null)
            //{
            //    if (!eStoreContext.Current.Store.isValidatedShiptoAddress(customer.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
            //    {
            //        Presentation.eStoreContext.Current.AddStoreErrorCode("You have entered a Shipping Address that this region’s eStore does not support.To select the eStore that offers regional support to this Shipping Address, choose the region from the pull-down box indicated by the country flag above.");
            //    }

            //    geteStoreContacts().RemoveAll(c => c.AddressID == customer.mainContact.AddressID);
            //    foreach (eStoreContact ec in geteStoreContacts())
            //    {
            //        ec.isSoldTo = false;
            //        ec.isShipTo = false;
            //        ec.isBillTo = false;
            //    }

            //    eStoreContact customercontact = new eStoreContact(customer.mainContact);
            //    customercontact.isBillTo = true;
            //    customercontact.isShipTo = true;
            //    customercontact.isSoldTo = true;
            //    customercontact.CreatedDate = DateTime.Now;
            //    customercontact.contactType = ContactType.Customer;
            //    geteStoreContacts().Add(customercontact);

            //    cart.setBillTo(customer.mainContact);
            //    cart.setShipTo(customer.mainContact);
            //    cart.setSoldTo(customer.mainContact);
            //    BindData();

            //    ShippingCalculator ShippingCalculator1 = (ShippingCalculator)this.Parent.FindControl("ShippingCalculator1");
            //    ShippingCalculator1.CalculateShippingRate();
            //}
            //else
            //{
            //    Presentation.eStoreContext.Current.AddStoreErrorCode("not an avilable customer email.");
            //}
        }

        protected void bindFonts()
        {
            //ckbNewBilltoContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Bill_to_address);
            //ckbNewShipptoContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Ship_to_address);
            //ckbNewSoldtoContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_new_Sold_to_address);
            //ckbSapCompany.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_From_SAP_Company);
            //ckbFromCustomer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_From_Customer);
            //lblSapCompany.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Sap_Company_Info);
            //btnAddSAPCompanyToContactList.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Select_to_edit);
            //lblFromCustomer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Customer_email);
            //btnFromCustomer.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Load);
            //btnSaveUserContact.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Save_Contact);
            ltAddress.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Address);
            ltBillTo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Bill_to);
            ltShipTo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Ship_to);
            ltSoldTo.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Sold_to);
        }

        protected void btSave_Click(object sender, EventArgs e)
        { 
            
        }

        protected void rpContactInfor_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            
            var ddl = (Repeater)sender;

            //Control Contant Ship to display use store parameter
            bool showShiptoSetting = eStoreContext.Current.Store.profile.getBooleanSetting("CtrlDefaultEditForShipto", false);

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Presentation.eStoreContact ec = e.Item.DataItem as Presentation.eStoreContact;

                if (ec.AddressID == Presentation.eStoreContext.Current.User.actingUser.mainContact.AddressID || ec.contactType == Presentation.ContactType.Customer)
                {
                    e.Item.FindControl("btnDel").Visible = false;
                    
                    //disable ship to Edit button for Sarah request
                    if (ddl.ID == "rpContactInforShip" && showShiptoSetting)
                    {
                        e.Item.FindControl("btnEdit").Visible = false;
                    }
                }

                Literal ltAddressInfor = e.Item.FindControl("ltAddressInfor") as Literal;
                ltAddressInfor.Text = Presentation.eStoreContext.Current.Store.profile.formatContactAddress(ec.toCartContact());
                Literal ltPhoneInfor = e.Item.FindControl("ltPhoneInfor") as Literal;
                ltPhoneInfor.Text = Presentation.eStoreContext.Current.Store.profile.formatContactPhone(ec.toCartContact());
            }
        }

        protected void rpContactInfor_ItemComm(object source, RepeaterCommandEventArgs e)
        {
            eStoreContact sc = this.geteStoreContacts().FirstOrDefault(x => x.AddressID == e.CommandArgument.ToString());
            eStoreContact ec = geteStoreContact(sc.AddressID);
            if (ec != null && ec.contactType != sc.contactType)
                sc.contactType = ec.contactType;
            string contactType = "";
            Control c = source as Control;
            if (e.CommandName == "editContact")
            {
                switch (c.ID)
                {
                    case "rpContactInforBill":
                        sc.isBillTo = (string.IsNullOrEmpty(this.Request["BillTo"]) == false && this.Request["BillTo"] == sc.AddressID);
                        contactType = "Bill";
                        break;
                    case "rpContactInforShip":
                        sc.isShipTo = (string.IsNullOrEmpty(this.Request["ShipTo"]) == false && this.Request["ShipTo"] == sc.AddressID);
                        contactType = "Ship";
                        break;
                    case "rpContactInforSold":
                        sc.isSoldTo = (string.IsNullOrEmpty(this.Request["SoldTo"]) == false && this.Request["SoldTo"] == sc.AddressID);
                        contactType = "Sold";
                        break;
                }
                ContactDetailsBill.setContact(sc, true);
                btSaveBillto.CommandArgument = sc.AddressID;
                BindScript("script", "showAddress", "$(function() { showAddress('" + contactType + "'); });");

            }
            else if (e.CommandName == "deleteContact")
            {
                if (sc != null)
                {
                    if (sc.isShipTo)
                    {
                        //try to set BillTo as shipTo default, if it's not available, then try to set SoldTo as default ship to
                        eStoreContact newShipTo = getCurrentBillToContact();
                        if (newShipTo == null)
                            newShipTo = getCurrentSoldToContact();

                        //change default ship to address and recalculate shipping charges and available methods
                        setCartShipToContact(newShipTo);
                        ShippingCalculator ShippingCalculator1 = (ShippingCalculator)this.Parent.FindControl("ShippingCalculator1");
                        ShippingCalculator1.CalculateShippingRate();
                    }

                    this.geteStoreContacts().Remove(sc);
                    if (sc.contactType == ContactType.Contact)
                    {
                        Presentation.eStoreContext.Current.User.actingUser.deleteContact((POCOS.Contact)getContact(sc));
                        Presentation.eStoreContext.Current.User.actingUser.save();
                    }
                }
                BindData();
            }
            
        }
    }
}