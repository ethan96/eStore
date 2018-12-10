using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.Utilities;
using esUtilities;

namespace eStore.UI.Modules
{
    public partial class QuantityDiscountRequest : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Product _product { get; set; }
        private bool _needLogin = true;
        public bool NeedLogin
        {
            get
            {
                return _needLogin;
            }
            set
            {
                _needLogin = value;
            }
        }
        public Models.ShoppingControl ShoppingControl { get; set; } = Models.ShoppingControl.Normal;


        private bool _hidequantity = true;
        public bool HideQuantity
        {
            get
            {
                return _hidequantity;
            }
            set
            {
                _hidequantity = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindFonts();
            }
            if (!NeedLogin && HideQuantity)
            {
                txt_quantity.Text = "1";
                this.trbudget.Visible = false;
                this.trquantity.Visible = false;
                this.trleadtime.Visible = false;
                DateTime dTime = DateTime.Today.AddDays(15);
                txt_leadtime.Text = dTime.Month + "/" + dTime.Day + "/" + dTime.Year;
            }
            if (_product != null)
            {
                txt_product.Text = _product.name;
                txt_desc.Text = esUtilities.CommonHelper.RemoveHtmlTags(_product.productDescX);
                hSProductID.Value = _product.SProductID;
            }
            switch (ShoppingControl)
            {
                case Models.ShoppingControl.CLA:
                    lbl_formdesc.Text = "Please fill in the below request form and our team will contact you shortly:";
                    this.trbudget.Visible = false;
                    this.trZipCode.Visible = false;
                    this.trAddress.Visible = false;
                    this.lbl_whattimeArea.Visible = false;
                    this.trComments.Visible = false;
                    this.divCLAAccount.Visible = true;
                    break;
                case Models.ShoppingControl.Inquire:
                    this.trbudget.Visible = false;
                    this.trquantity.Visible = false;
                    this.trleadtime.Visible = false;
                    this.lbl_whattimeArea.Visible = false;
                    this.trAddress.Visible = false;
                    this.trZipCode.Visible = false;
                    break;
                default:
                    break;
            }
        }

        protected override void CreateChildControls()
        {
            BindCountry(string.Empty);
            if (!IsPostBack)
            {
                bindQuantityDiscount();
            }
            base.CreateChildControls();
        }

        protected void bindQuantityDiscount()
        {

            if (Presentation.eStoreContext.Current.User != null)
            {

                DateTime dTime = DateTime.Today.AddDays(15);
                txt_leadtime.Text = dTime.Month + "/" + dTime.Day + "/" + dTime.Year;

                txt_firstname.Text = Presentation.eStoreContext.Current.User.FirstName;
                txt_lastname.Text = Presentation.eStoreContext.Current.User.LastName;
                txt_companyname.Text = Presentation.eStoreContext.Current.User.CompanyName;
                string adressStr = "{0},{1},{2}";
                try
                {
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.getStringSetting("quantityAdress")))
                        adressStr = Presentation.eStoreContext.Current.getStringSetting("quantityAdress");
                    txt_address.Text = string.Format(adressStr, Presentation.eStoreContext.Current.User.mainContact.Address1, Presentation.eStoreContext.Current.User.mainContact.City, Presentation.eStoreContext.Current.User.mainContact.State);
                }
                catch (Exception)
                {
                    txt_address.Text = string.Format("{0},{1},{2}", Presentation.eStoreContext.Current.User.mainContact.Address1, Presentation.eStoreContext.Current.User.mainContact.City, Presentation.eStoreContext.Current.User.mainContact.State);
                }
                txt_email.Text = Presentation.eStoreContext.Current.User.UserID;
                txtzipcode.Text = Presentation.eStoreContext.Current.User.mainContact.ZipCode;
                txt_telephone.Text = Presentation.eStoreContext.Current.User.mainContact.TelNo + " Ext. " + Presentation.eStoreContext.Current.User.mainContact.TelExt
                                    + " , " + Presentation.eStoreContext.Current.User.mainContact.Mobile;


                this.Visible = true;

            }
            else if (NeedLogin)
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;
            }

         
        }


        //Bind  Country
        private void BindCountry(string _CountryCode)
        {
            if (string.IsNullOrEmpty(_CountryCode))
                if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingUser.mainContact != null)
                {
                    _CountryCode = eStoreContext.Current.User.actingUser.mainContact.countryCodeX;

                }
            if (string.IsNullOrEmpty(_CountryCode))
            {
                _CountryCode = Presentation.eStoreContext.Current.CurrentCountry.CountryName;
            }

            List<POCOS.Country> countries;
            if (eStoreContext.Current.User != null && eStoreContext.Current.User.actingRole == POCOS.User.Role.Employee)
            {
                eStore.BusinessModules.StoreSolution eSolution = eStore.BusinessModules.StoreSolution.getInstance();
                countries = eSolution.countries.OrderBy(c => c.CountryName).ToList();
            }
            else
                countries = eStoreContext.Current.Store.profile.Countries.ToList<POCOS.Country>();

            ddl_country.Items.Add(new ListItem("[" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select_Country) + "]", ""));
            foreach (POCOS.Country country in countries)
            {
                ListItem item = new ListItem();
                if (!String.IsNullOrEmpty(_CountryCode))
                {
                    if (_CountryCode == country.Shorts)
                    {
                        item.Selected = true;

                    }
                }
                item.Text = country.CountryName;
                item.Value = country.Shorts;
                ddl_country.Items.Add(item);
            }
        }

        //Submit
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            if (UVerification1.VerificationUser())
            {
                POCOS.UserRequest requestDiscount = new POCOS.UserRequest(Presentation.eStoreContext.Current.Store.profile, POCOS.UserRequest.ReqType.RequestDiscount);
                POCOS.Product discountProduct = Presentation.eStoreContext.Current.Store.getProduct(hSProductID.Value);
                requestDiscount.ProductName = discountProduct.name;
                requestDiscount.ProductDesc = esUtilities.CommonHelper.RemoveHtmlTags(discountProduct.productDescX);
                int qty = 1;
                int.TryParse(txt_quantity.Text.Trim(), out qty);
                requestDiscount.Quantity = qty;
                // Property is it right ? 
                DateTime dTime = DateTime.Today.AddDays(15);
                if (!string.IsNullOrEmpty(txt_leadtime.Text.Trim()))
                    DateTime.TryParse(txt_leadtime.Text.Trim(), out dTime);
                requestDiscount.InsertDate = dTime;

                decimal budget = 0;
                string budgetStr = txt_budget.Text.Trim();
                if (decimal.TryParse(budgetStr, out budget))
                    budgetStr = Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(budget, Presentation.eStoreContext.Current.CurrentCurrency);
                requestDiscount.Budget = budgetStr;
                requestDiscount.FirstName = txt_firstname.Text.Trim();
                requestDiscount.LastName = txt_lastname.Text.Trim();
                requestDiscount.Country = ddl_country.SelectedValue;
                requestDiscount.Company = txt_companyname.Text.Trim();
                string AddressStr = "{0} {1}";
                try
                {
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.getStringSetting("quantityAdressAndZip")))
                        AddressStr = Presentation.eStoreContext.Current.getStringSetting("quantityAdressAndZip");
                    requestDiscount.Address = string.Format(AddressStr, txt_address.Text.Trim(), txtzipcode.Text.Trim());
                }
                catch (Exception)
                {
                    requestDiscount.Address = string.Format("{0} {1}", txt_address.Text.Trim(), txtzipcode.Text.Trim());
                }
                requestDiscount.Email = txt_email.Text.Trim();
                requestDiscount.Telephone = txt_telephone.Text.Trim();
                if (cb_email.Checked)
                {
                    requestDiscount.ContactType += "Email ";
                }
                if (cb_phone.Checked)
                {
                    if (cb_email.Checked)
                    {
                        string telStr = "Telephone (Best Contact Time: {0}~{1})"; //format stardate and end date
                        telStr = string.Format(telStr, ServerTime1.StartDate, ServerTime1.EndDate);
                        requestDiscount.ContactType += "or <br />" + telStr + "";
                    }
                    else
                    {
                        requestDiscount.ContactType += "Telephone";
                    }
                }
                requestDiscount.SProductId = hSProductID.Value;
                requestDiscount.ExpectedDate = dTime;
                //These values will be initialized in constructor
                //requestDiscount.StoreID = Presentation.eStoreContext.Current.User.store.StoreID;
                //requestDiscount.CreatedDate = DateTime.Now;

                if (ShoppingControl== Models.ShoppingControl.CLA)
                    requestDiscount.Comment = txt_CLAAccount.Text.Trim();
                else
                    requestDiscount.Comment = txt_comments.Text.Trim();

                requestDiscount.save();
                requestDiscount.ProductX = discountProduct;
                eStore.BusinessModules.EMailDiscountRequest request = new BusinessModules.EMailDiscountRequest();
                EMailReponse response = request.getDiscountRequestEmailTemplate(Presentation.eStoreContext.Current.Store, 
                    requestDiscount,
                    NeedLogin ? eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Request_Quantity_Discount) 
                    : eStore.Presentation.eStoreContext.Current.Store.Tanslation("eStore_Advantech_eStore_Contact_for_Price")
                    , Presentation.eStoreContext.Current.User, eStoreContext.Current.CurrentLanguage, eStoreContext.Current.MiniSite);
                //string volumeDiscountRequestEmailContent = eStore.BusinessModules.EMailDiscountRequest.getDiscountRequestEmailTemplate(Presentation.eStoreContext.Current.Store, requestDiscount);
                if (response.SendResult)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings.Get("IsToSiebel") == "true")
                    {
                        Presentation.eStoreContext.Current.Store.AddOnlineRequest2Siebel(Request.Url.ToString(), requestDiscount);
                    }
                    if (NeedLogin)
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_discount_request_is_sent_successful));
                    else
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreContext.Current.Store.Tanslation("Contact_for_Price_is_sent_successful"));

                }
                else
                {
                    if (NeedLogin)
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_discount_request_is_sent_failed));
                    else
                        Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreContext.Current.Store.Tanslation("Contact_for_Price_is_sent_failed"));
                }
            }
        }

        protected void bindFonts()
        {
            //lbl_MainTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Quantity_Discount_Request);
            lbl_formdesc.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Please_fill_out_the);
            lbl_product.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Product);
            lbl_desc.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Product_Description);
            lbl_quantity.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Quantity);
            lbl_leadtime.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Expected_Lead_Time);
            lbl_budget.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Budget);
            lbl_name.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_First_Name);
            ltLastName.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Last_Name);
            lbl_country.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Country);
            lblzipcode.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Zip_Code);
            lbl_companyname.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Company);
            lbl_address.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Address);
            lbl_email.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_eMail);
            lbl_telephone.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Telephone);
            lbl_contactvia.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_You_prefer_we_contact_you_via);
            lbl_byemail.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_eMail);
            lbl_bytel.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Telephone);
            lbl_whattime.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_What_time_is_appropriate);
            lbl_comments.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Comments);
            btn_Submit.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Request_Submit);
            RequiredFieldValidator2.ErrorMessage = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Please_input_Quantity);
            RegularExpressionValidator1.ErrorMessage = RegularExpressionValidator1.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Type_Must_Int);
            RegularExpressionValidator2.ErrorMessage = RegularExpressionValidator2.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Please_Format_date_time);
        }
    }
}