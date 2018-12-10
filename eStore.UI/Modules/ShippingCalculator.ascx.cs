using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.BusinessModules;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    public partial class ShippingCalculator : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Cart cart { get; set; }
        public String shippingMethod { get; set; }
        private string _CourierAccount;
        public string CourierAccount
        {
            get
            { return _CourierAccount; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    txtCourierAccount.Text = _CourierAccount = value;
                }
            }
        }
        public Boolean isAvailable { get; set; }

        public string ShippingCarrierValue {
            get { return rblShippingCarrier.SelectedValue; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindFonts();
                if (!Presentation.eStoreContext.Current.Store.offerShippingService)
                {
                    this.rblShippingCarrier.Items.Remove(this.rblShippingCarrier.Items.FindByValue("Recommend"));
                    pRecommendedCarrier.Visible = false;
                }
                else
                {
                    if (string.IsNullOrEmpty(CourierAccount))
                        this.rblShippingCarrier.Items.FindByValue("Recommend").Selected = true;
                    if (eStoreContext.Current.Store.onlyLocalFixRate)
                    {
                        pRecommendedCarrier.Visible = true;
                        pShippingMethodOptions.Visible = false;
                        btnCalculateShipping.Visible = false;
                    }
                    else
                    {
                        this.pRecommendedCarrier.Visible = true;
                        if (Presentation.eStoreContext.Current.User.role == POCOS.User.Role.Employee)
                        {
                            this.btnShowPackingDetails.Visible = true;
                            this.PackageDetails1.Visible = true;
                        }
                    }

                }

                if (Presentation.eStoreContext.Current.Store.offerDropShipmentService
                    && Presentation.eStoreContext.Current.User.role == POCOS.User.Role.Employee
                    && Presentation.eStoreContext.Current.getBooleanSetting("BBFlag", false) == false)
                { }
                else
                {
                    this.rblShippingCarrier.Items.Remove(this.rblShippingCarrier.Items.FindByValue("Dropoff"));
                }

                string enableCustomerShipMethod = "";
                Presentation.eStoreContext.Current.Store.profile.Settings.TryGetValue("EnableCustomerShipMethod", out enableCustomerShipMethod);
                if (!string.IsNullOrEmpty(enableCustomerShipMethod) && enableCustomerShipMethod == "enabled")
                {

                }
                else
                {
                    this.rblShippingCarrier.Items.Remove(this.rblShippingCarrier.Items.FindByValue("Customer"));
                }

                if (this.rblShippingCarrier.Items.Count == 1)
                {
                    this.rblShippingCarrier.Items[0].Selected = true;
                    this.rblShippingCarrier.Visible = false;
                }

            }
            if (Presentation.eStoreContext.Current.Store.offerDropShipmentService
                    && Presentation.eStoreContext.Current.User.role == POCOS.User.Role.Employee
                    && Presentation.eStoreContext.Current.getBooleanSetting("BBFlag", false) == false)
            {
                this.rblShippingCarrier.Items.FindByValue("Dropoff").Attributes.Add("class", "adminonly");
            }
        }
        public Boolean isCustomerCourier
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.txtCourierAccount.Text);
            }
            set
            {
                rblShippingCarrier.ClearSelection();
                rblShippingCarrier.Items.FindByValue("Customer").Selected = true;
                pRecommendedCarrier.Visible = false;
                pCustomerCarrier.Visible = true;
            }
        }
        public ShippingMethod CalculateShippingRate(Boolean performErrorChecking = false)
        {
            ShippingMethod sm = new ShippingMethod();

            switch (this.rblShippingCarrier.SelectedValue)
            {
                case "Dropoff":
                case "Recommend":
                    if (cart.ShipToContact == null)
                    {
                        isAvailable = false;
                        sm = null;
                    }
                    else
                    {
                        if (rblShippingRate.Visible == true && rblShippingRate.Items.Count > 0 && rblShippingRate.SelectedIndex >= 0)
                            shippingMethod = rblShippingRate.SelectedValue;
                        List<ShippingMethod> sms;
                        sms = null;
                        try
                        {

                            sms = eStoreContext.Current.Store.getAvailableShippingMethods(cart, this.rblShippingCarrier.SelectedValue == "Dropoff");
                        }
                        catch (Exception ex)
                        {
                            sms = null;
                            isAvailable = false;
                            Utilities.eStoreLoger.Error("Can not get shipping", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, ex);
                        }
                        if (sms == null || sms.Count == 0)
                        {
                            sm = null;
                            isAvailable = false;
                            lShippingRate.Visible = true;
                            rblShippingRate.Visible = false;
                            // btnCalculateShipping.Visible = false;
                        }
                        else
                        {
                            isAvailable = false;
                            lShippingRate.Visible = false;
                            rblShippingRate.Visible = true;
                            //btnCalculateShipping.Visible = true;
                            this.rblShippingRate.Items.Clear();
                            /*
                            var orderdsms = from osm in sms
                                            orderby osm.ShippingCarrier descending, osm.ShippingCostWithPublishedRate
                                            select osm;

                            foreach (ShippingMethod _sm in orderdsms.ToList())
                             */
                            foreach (ShippingMethod _sm in sms)
                            {
                                if (_sm.Error == null)
                                {
                                    ListItem item = new ListItem();
                                    item.Value = _sm.ShippingMethodDescription;
                                    if (_sm.ShippingMethodDescription == this.shippingMethod)
                                    {
                                        item.Selected = true;
                                        sm = _sm;
                                        isAvailable = true;
                                    }
                                    else
                                        item.Selected = false;
                                    item.Text = _sm.ShippingMethodDescription + " [" + Presentation.Product.ProductPrice.FormartPriceWithDecimal((decimal)_sm.ShippingCostWithPublishedRate) + "]"
                                        + (_sm.Discount > 0f ? " Shipping discount applied at next step" : string.Empty);
                                    _sm.StoreAddress = Presentation.eStoreContext.Current.CurrentAddress;
                                    _sm.CheckFreight(cart);
                                    if (!string.IsNullOrEmpty(_sm.AlertMessage))
                                    {
                                        item.Attributes.Add("data-trigger", "tooltip");
                                        item.Attributes.Add("data-content", _sm.AlertMessage);
                                    }
                                    this.rblShippingRate.Items.Add(item);
                                }
                                else
                                {
                                    Presentation.eStoreContext.Current.AddStoreErrorCode(_sm.Error.Code.ToString());
                                }
                            }
                            if (cart.ShipToContact != null && cart.ShipToContact.countryCodeX == "CA" && Presentation.eStoreContext.Current.Store.storeID != "ABB")
                            {
                                ListItem gi = new ListItem();
                                gi.Value = "UPS Ground";
                                gi.Text = "UPS Ground (using my own carrier account)";
                                this.rblShippingRate.Items.Add(gi);
                                ListItem fi = new ListItem();
                                fi.Value = "FedEx International Ground";
                                fi.Text = "FedEx International Ground (using my own carrier account)";
                                this.rblShippingRate.Items.Add(fi);
                            }
                            if (rblShippingRate.Items.Count == 0)
                            {
                                sm = null;
                                isAvailable = false;
                                lShippingRate.Visible = true;
                                rblShippingRate.Visible = false;
                            }
                            else if (rblShippingRate.SelectedIndex < 0)
                            {
                                rblShippingRate.SelectedIndex = 0;
                                sm = sms.FirstOrDefault(x => x.ShippingMethodDescription == rblShippingRate.SelectedValue);
                                isAvailable = true;
                            }
                        }
                    }
                    if (eStoreContext.Current.Store.onlyLocalFixRate && this.rblShippingCarrier.SelectedValue == "Recommend")
                    {
                        pShippingMethodOptions.Visible = false;
                        btnCalculateShipping.Visible = false;

                    }
                    else
                    {
                        pShippingMethodOptions.Visible = true;
                        btnCalculateShipping.Visible = true;
                        if (Presentation.eStoreContext.Current.User.role == POCOS.User.Role.Employee)
                        {
                            this.btnShowPackingDetails.Visible = true;
                            this.PackageDetails1.Visible = true;
                        }
                    }
                    pCustomerCarrier.Visible = false;
                    break;
                case "Customer":
                    pShippingMethodOptions.Visible = false;
                    pCustomerCarrier.Visible = true;
                    if (string.IsNullOrEmpty(this.txtCourierAccount.Text))
                    {
                        sm = null;
                        isAvailable = false;
                        if (performErrorChecking)
                        {
                            Presentation.eStoreContext.Current.AddStoreErrorCode("Courier Account is Empty");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CourierAccount))
                        {
                            if (!IsPostBack)
                            {
                                bindOwnCarrierShipping();
                                ddlShipping.ClearSelection();
                                ListItem item = ddlShipping.Items.FindByText(this.shippingMethod);
                                if (item != null)
                                    item.Selected = true;
                            }
                        }

                        sm.ServiceCode = this.txtCourierAccount.Text;
                        sm.ShippingCarrier = ddlShipping.SelectedItem != null ? ddlShipping.SelectedItem.Value : "";
                        sm.ShippingMethodDescription = ddlShipping.SelectedItem != null ? ddlShipping.SelectedItem.Text : "no support carrier";
                        sm.ShippingCostWithPublishedRate = 0;
                        isAvailable = true;
                    }
                    break;
                default:
                    pShippingMethodOptions.Visible = false;
                    pCustomerCarrier.Visible = false;
                    sm = null;
                    isAvailable = false;
                    break;
            }
            return sm;
        }
        //CalculateShipping
        protected void btnCalculateShipping_Click(object sender, EventArgs e)
        {
            ContactSelector ContactSelector1 = (ContactSelector)this.Parent.FindControl("ContactSelector1");
            if (ContactSelector1.setCartContact())
            {
                ShippingMethod sm = CalculateShippingRate(true);
            }
            bindOwnCarrierShipping();
            if (btnCalculateShipping.Visible)
                Page.ClientScript.RegisterStartupScript(
             this.GetType(), "anchor", string.Format("location.hash = '#btnCalculateShipping';", rblShippingCarrier.ClientID), true);
        }

        protected void rblShippingCarrier_SelectedIndexChanged(object sender, EventArgs e)
        {
            ContactSelector ContactSelector1 = (ContactSelector)this.Parent.FindControl("ContactSelector1");
            if (ContactSelector1.setCartContact())
            {
                ShippingMethod sm = CalculateShippingRate();
            }
            bindOwnCarrierShipping();
            string parameter = Request["__EVENTARGUMENT"];
            if (parameter == "UPS Ground" || parameter == "FedEx International Ground")
            {
                ListItem selected = this.ddlShipping.Items.FindByText(parameter);
                if (selected != null)
                    selected.Selected = true;
            }
            if (btnCalculateShipping.Visible)
                Page.ClientScript.RegisterStartupScript(
              this.GetType(), "anchor", string.Format("location.hash = '#btnCalculateShipping';", rblShippingCarrier.ClientID), true);

        }

        protected void bindFonts()
        {
            btnCalculateShipping.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Calculate_Shipping);
            lShippingRate.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_contact_Advantech_for_shipment_arrangement);
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("ableDeliveryMessage"))
                ltDeliveryMessage.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Delivery_message);
        }

        public void bindOwnCarrierShipping()
        {
            if (this.rblShippingCarrier.SelectedValue == "Customer")
            {
                ddlShipping.Items.Clear();
                //Presentation.eStoreContext.Current.User.mainContact.countryCodeX
                List<RateServiceName> OwnCarrierRateServiceList;
                if (cart.ShipToContact != null)
                {
                    OwnCarrierRateServiceList = eStore.Presentation.eStoreContext.Current.Store.getSupportedShipping(cart.ShipToContact.countryCodeX);
                    if (cart.ShipToContact.countryCodeX == "CA")
                    {
                        RateServiceName ug = new RateServiceName();
                        ug.CarierName = "UPS Ground";
                        ug.DefaultServiceName = "UPS Ground";
                        ug.MessageCode = "03";
                        OwnCarrierRateServiceList.Add(ug);

                        RateServiceName fg = new RateServiceName();
                        fg.CarierName = "FedEx International Ground";
                        fg.DefaultServiceName = "FedEx International Ground";
                        fg.MessageCode = "FEDEX_GROUND";
                        OwnCarrierRateServiceList.Add(fg);
                    }
                }
                else
                    OwnCarrierRateServiceList = new List<RateServiceName>();
                RateServiceName dhl = OwnCarrierRateServiceList.FirstOrDefault(c => c.DefaultServiceName == "DHL Worldwide Package Express");
                if (dhl != null)
                    OwnCarrierRateServiceList.Remove(dhl);
                if (OwnCarrierRateServiceList == null || OwnCarrierRateServiceList.Count == 0)
                {
                    ddlShipping.Enabled = false;
                    txtCourierAccount.Enabled = false;
                    Presentation.eStoreContext.Current.AddStoreErrorCode("No support carrier");
                }
                else
                {
                    foreach (RateServiceName ownRS in OwnCarrierRateServiceList)
                        ddlShipping.Items.Add(new ListItem(ownRS.DefaultServiceName, ownRS.MessageCode));
                    ddlShipping.Enabled = true;
                    txtCourierAccount.Enabled = true;
                }
                if (Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("BBFlag", false) == true)
                {
                    var bbcust = Presentation.eStoreContext.Current.Store.GetBBcustomerdataFromMyAdvantech(eStoreContext.Current.User.actingUser.UserID);
                    if (bbcust != null && !string.IsNullOrEmpty(bbcust.IncotermText))
                        txtCourierAccount.Text = bbcust.IncotermText;
                }
            }
        }

        protected void btnShowPackingDetails_Click(object sender, EventArgs e)
        {
            this.PackageDetails1.Cart = this.cart;
            this.PackageDetails1.bindPackageDetails();
        }
    }
}