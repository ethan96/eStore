using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;
using esUtilities;
using System.Configuration;
using System.Text.RegularExpressions;

namespace eStore.BusinessModules
{
    public class EMailNoticeTemplate
    {
        #region Properties
        /// <summary>
        /// If test mode is enable, will append tag [TESTING] to subject, and send mail to eStore IT.
        /// </summary>
        private string testing = ConfigurationManager.AppSettings.Get("TestingMode");
        private bool testMode()
        {
            if (String.IsNullOrEmpty(testing) || testing.ToLower() == "true")
                return true;
            else
                return false;
        }

        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        private string eStoreAdminGroup = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        public string TestingOrderDeptEmail
        {
            get { return testingOrderDeptEmail; }
            set { testingOrderDeptEmail = value; }
        }

        //是否需要密送？
        private bool isneedBcc = true;
        public bool IsneedBcc
        {
            get { return isneedBcc; }
            set { isneedBcc = value; }
        }

        private Store _store;
        public Store Store
        {
            get { return _store; }
        }

        public bool isShowStorePrice = false;

        #endregion

        #region Methods
        /// <summary>
        /// Consturctor
        /// </summary>
        /// <param name="store"></param>
        public EMailNoticeTemplate(Store store)
        {
            _store = store;
        }

        private void LoadOrderMainContent(Order order, EMailOrder emailOrder, Language language = null, MiniSite minisite = null)
        {
            string orderMail = emailOrder.MailContent;//邮件模板
            //string username = order.userX.FirstName.ToUpper() + " " + order.userX.LastName.ToUpper();
            //string firstname = order.userX.FirstName.ToUpper();
            //string lastname = order.userX.LastName.ToUpper();
            string USERNAME = emailOrder.UserName;
            string customerEmail = emailOrder.CustomerEmail;

            string customerERPID = (string.IsNullOrEmpty(order.userX.FederalID)) ? "" : order.userX.FederalID;
            string cartContactGroup = order.cartX.businessGroup.ToString();
            emailOrder.StoreEmail.ContactMail = _store.profile.OrderDeptEmail;
            
            string bankInformation = null;

            setEmailOrderInfor(order.cartX, minisite, ref emailOrder);
            
            if (order.isPStoreOrder())
            {
                string pemailGroup = _store.profile.getStringSetting("ePAPSEmailGroup");
                if (!string.IsNullOrEmpty(pemailGroup))
                {
                    emailOrder.EmailGroup = pemailGroup;
                }
            }
            bankInformation = Store.getBankInformation(order);

            if (string.IsNullOrEmpty(emailOrder.StoreEmail.OurTel))
                SetContactBaseInfor(_store, order.cartX.businessGroup.ToString(), emailOrder.StoreEmail);
            //Channel Partner
            string supportInfoContent = null;
            string channlePartner = null;
            if (order.storeX.channelPartnerReferralEnabled)
            {
                supportInfoContent = "Thank you for selecting <b style='color: Blue;'>{0}</b> to service your account. We will contact you shortly to confirm your order and inform you about the shipping date.  " +
                                    "Should you have any questions on this order, please contact our Call Center at toll free number {1}.";
                eStore.POCOS.PocoX.ChannelPartner cp = order.Channel;
                if (order.isReferredToChannelPartner() && cp != null) // not Advantech channel partner
                {
                    channlePartner = cp.Company;
                    emailOrder.StoreEmail.OurAddress = cp.Address + ", " + cp.City + ", " + cp.Zip + ", " + cp.Country;
                    emailOrder.StoreEmail.OurTel = cp.Phone;
                    supportInfoContent = string.Format(supportInfoContent, channlePartner, cp.Phone);
                    supportInfoContent += "<br /><br />Note: All Advantech terms and conditions are superseded and replace by your selected channel partner terms and conditions.";
                }
                else
                {
                    channlePartner = "Advantech Europe BV";
                    supportInfoContent = string.Format(supportInfoContent, channlePartner, emailOrder.StoreEmail.OurTel);
                }
            }

            string estoreUrl = Store.getCurrStoreUrl(_store.profile,minisite);

            string comment = (string.IsNullOrEmpty(order.CustomerComment)) ? "" : order.CustomerComment.Trim();
            string orderNo = order.OrderNo;
            //string orderDate = order.OrderDate.ToString();
            string promotionCode = order.PromoteCode;
            string purchaseNo = order.PurchaseNO;
            string resellerId = order.ResellerID;
            string courierAccount = order.CourierAccount;

            //string soldToAttention = order.cartX.SoldToContact.Attention;
            string soldToAttention = Store.getCultureFullName(order.cartX.SoldToContact);
            string soldToCompany = order.cartX.SoldToContact.AttCompanyName;
            string soldToCounty = (string.IsNullOrEmpty(order.cartX.SoldToContact.County)) ? "" : order.cartX.SoldToContact.County;
            string soldToState = (string.IsNullOrEmpty(order.cartX.SoldToContact.State)) ? "" : order.cartX.SoldToContact.State;

            //soldToAddress修改后的显示格式
            string soldToAddress = formatContactAddress(order.cartX.SoldToContact);

            string soldToTel = (string.IsNullOrEmpty(order.cartX.SoldToContact.TelExt)) ? order.cartX.SoldToContact.TelNo : order.cartX.SoldToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language, minisite) + ": " + order.cartX.SoldToContact.TelExt;
            string soldToMobile = order.cartX.SoldToContact.Mobile;

            //string billToAttention = order.cartX.BillToContact.Attention;
            string billToAttention = Store.getCultureFullName(order.cartX.BillToContact);
            string billToCompany = order.cartX.BillToContact.AttCompanyName;
            string billToCounty = (string.IsNullOrEmpty(order.cartX.BillToContact.County)) ? "" : order.cartX.BillToContact.County;
            string billToState = (string.IsNullOrEmpty(order.cartX.BillToContact.State)) ? "" : order.cartX.BillToContact.State;

            //billToAddress修改后的显示格式
            string billToAddress = formatContactAddress(order.cartX.BillToContact);

            string billToTel = (string.IsNullOrEmpty(order.cartX.BillToContact.TelExt)) ? order.cartX.BillToContact.TelNo : order.cartX.BillToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + order.cartX.BillToContact.TelExt;
            string billToMobile = order.cartX.BillToContact.Mobile;

            //string shipToAttention = order.cartX.ShipToContact.Attention;
            string shipToAttention = Store.getCultureFullName(order.cartX.ShipToContact);
            string shipToCompany = order.cartX.ShipToContact.AttCompanyName;
            string shipToCounty = (string.IsNullOrEmpty(order.cartX.ShipToContact.County)) ? "" : order.cartX.ShipToContact.County;
            string shipToState = (string.IsNullOrEmpty(order.cartX.ShipToContact.State)) ? "" : order.cartX.ShipToContact.State;

            //shipToAddress修改后的显示格式
            string shipToAddress = formatContactAddress(order.cartX.ShipToContact);

            string shipToTel = (string.IsNullOrEmpty(order.cartX.ShipToContact.TelExt)) ? order.cartX.ShipToContact.TelNo : order.cartX.ShipToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + order.cartX.ShipToContact.TelExt;
            string shipToMobile = order.cartX.ShipToContact.Mobile;

            string shippingMethod = order.ShippingMethod;
            string subTotal = FormatPriceWithCurrencyDecimal(order.cartX.TotalAmount, order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate);
            string freight = FormartFreight(order.Freight, order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate);
            string tax = FormartTax(order.Tax, order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate);
            string taxrate = formatTaxRate(order.TaxRate);
            string discount = "-" + FormatPriceWithCurrencyDecimal(order.TotalDiscount.GetValueOrDefault(), order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate);
            string total = FormatPriceWithCurrencyDecimal(order.totalAmountX, order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate);
            if (isShowStorePrice)
                total += string.Format("<br />({0})", FormatPriceWithCurrencyDecimal(order.totalAmountX, order.currencySign, null));

            string orderdetailItemsCustomerDisplay = showCartItems(order.cartX, Store, false);
            string orderdetailItemsInternalDisplay = showCartItems(order.cartX, Store, true);
            string result = null;
            string customerResult = null;
            string internalResult = null;

            string clickhere = Store.getCurrStoreUrl(order.cartX.storeX, minisite); //clickhere is a url link to check order            
            //If TestingMode is true, then use QA URL as StoreURL
            clickhere = (System.Configuration.ConfigurationManager.AppSettings.Get("TestingMode") == "true") ? System.Configuration.ConfigurationManager.AppSettings.Get("QATestingServerURL") : clickhere;

            // "Dev", "QA", "Staging", "Production"
            //switch (System.Configuration.ConfigurationManager.AppSettings.Get("StoreStatus"))
            //{ 
            //    case "Dev":
            //        clickhere = System.Configuration.ConfigurationManager.AppSettings.Get("DevServerURL");
            //        break;
            //    case "QA":
            //        clickhere = System.Configuration.ConfigurationManager.AppSettings.Get("QATestingServerURL");
            //        break;
            //    case "Staging":
            //        clickhere = System.Configuration.ConfigurationManager.AppSettings.Get("StagingServerURL");
            //        break;
            //    case "Production":
            //        clickhere = System.Configuration.ConfigurationManager.AppSettings.Get("ProductionServerURL");
            //        break;
            //    default:
            //        clickhere = System.Configuration.ConfigurationManager.AppSettings.Get("ProductionServerURL");
            //        break;
            //}

            clickhere = clickhere + "/Account/orderdetail.aspx?orderid=" + order.OrderNo + "&storeid=" + order.StoreID;

            String paymentTypeContent = emailOrder.PaymentTypeContent;
            //Remove it after order type is implatmet, Mike will implement it.
            //order.PaymentType = "PayByCreditCard";
            
            // Replace tag
            if (string.IsNullOrEmpty(orderMail))
            {
                throw new Exception("FailToGetTemplate");
            }
            else
            {
                result = orderMail.Replace("[/USERNAME]", USERNAME);
                //result = result.Replace("[/LASTNAME]", lastname);
                result = result.Replace("[/ERPID]", customerERPID);
                result = result.Replace("[/STOREURL]", estoreUrl);
                result = result.Replace("[/OurTel]", emailOrder.StoreEmail.OurTel);
                result = result.Replace("[/ourAddress]", emailOrder.StoreEmail.OurAddress);
                result = result.Replace("[/Comment]", comment);
                result = result.Replace("[/ORDERNO]", orderNo);
                if (minisite != null && (minisite.MiniSiteType == MiniSite.SiteType.IotMart || minisite.MiniSiteType == MiniSite.SiteType.UShop))
                    result = result.Replace("/AUS/logo.gif", string.Format("/{0}/{1}/logo.gif", minisite.StoreID, minisite.SiteName));

                //Channel Partner content


                if (order.isConfirmdOrder && (order.statusX == Order.OStatus.ConfirmdButNeedTaxIDReview || order.statusX == Order.OStatus.ConfirmdButNeedFreightReview))
                {
                    result = result.Replace("[/SupportInfo]", "A representative will contact you shortly to confirm your selection.<br />Thank you for shopping at the Advantech eStore.");
                    result = result.Replace("[/OrderingInformation]", "THIS IS NOT AN ORDER CONFIRMATION");
                    result = result.Replace("[/Content]", "THIS IS NOT AN ORDER CONFIRMATION");

                }
                else
                {
                    result = result.Replace("[/OrderingInformation]", "Ordering Information");
                    if (string.IsNullOrEmpty(supportInfoContent))
                        result = result.Replace("[/SupportInfo]", string.Empty);
                    else
                        result = result.Replace("[/SupportInfo]", supportInfoContent);
                }

                if (string.IsNullOrEmpty(channlePartner))
                    result = result.Replace("[/CHANNLEPARTNER]", string.Empty);
                else
                    result = result.Replace("[/CHANNLEPARTNER]", channlePartner + ",");

                if (string.IsNullOrEmpty(order.PromoteCode))
                    result = result.Replace("[/PROMOTIONCODE]", string.Empty);
                else
                    result = result.Replace("[/PROMOTIONCODE]", string.Format("<p><label>{1}: </label>{0}</p>", promotionCode, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Promote_code, true, language,minisite)));

                if (string.IsNullOrEmpty(order.PurchaseNO))
                    result = result.Replace("[/PURCHASENO]", string.Empty);
                else
                    result = result.Replace("[/PURCHASENO]", string.Format("<p><label>{1}: </label>{0}</p>", purchaseNo, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_P_O_Number, true, language,minisite)));

                if (string.IsNullOrEmpty(order.customerPONoX))
                    result = result.Replace("[/customerPONo]", string.Empty);
                else
                    result = result.Replace("[/customerPONo]", string.Format("<p><label>Customer PO: </label>{0}</p>", order.customerPONoX));

                if (string.IsNullOrEmpty(order.RegistrationNumber))
                    result = result.Replace("[/Company_Registration_Number]", string.Empty);
                else
                    result = result.Replace("[/Company_Registration_Number]", string.Format("<p><label>{1}: {0}</label></p>", order.RegistrationNumber, "Company Registration Number"));

                if (string.IsNullOrEmpty(order.ResellerID))
                    result = result.Replace("[/RESELLERID]", string.Empty);
                else
                    result = result.Replace("[/RESELLERID]", string.Format("<p><label>{1}: </label>{0}</p>", resellerId, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Reseller_ID, true, language,minisite)));

                if (string.IsNullOrEmpty(order.CourierAccount))
                    result = result.Replace("[/Courier_account]", string.Empty);
                else
                    result = result.Replace("[/Courier_account]", string.Format("<p><label>{1}: </label>{0}</p>", courierAccount, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Courier_Account, true, language,minisite)));

                List<string> vatNo = new List<string>();

                if (string.IsNullOrEmpty(order.cartX.SoldToContact.VATNumbe))
                    result = result.Replace("[/SOLDVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/SOLDVATADDRESS]", string.Format("VAT:{0}", order.cartX.SoldToContact.VATNumbe));
                    vatNo.Add(order.cartX.SoldToContact.VATNumbe);
                }

                if (string.IsNullOrEmpty(order.cartX.BillToContact.VATNumbe))
                    result = result.Replace("[/BILLVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/BILLVATADDRESS]", string.Format("VAT:{0}", order.cartX.BillToContact.VATNumbe));
                    vatNo.Add(order.cartX.BillToContact.VATNumbe);
                }

                if (string.IsNullOrEmpty(order.cartX.ShipToContact.VATNumbe))
                    result = result.Replace("[/SHIPVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/SHIPVATADDRESS]", string.Format("VAT:{0}", order.cartX.ShipToContact.VATNumbe));
                    vatNo.Add(order.cartX.ShipToContact.VATNumbe);
                }

                if (!vatNo.Any())
                    result = result.Replace("[/VATADDRESS]", string.Empty);
                else
                    result = result.Replace("[/VATADDRESS]", string.Format("<br />VAT Number: {0}", string.Join(" / ", vatNo.Distinct())));

                if (string.IsNullOrEmpty(emailOrder.EmailGroup))
                {
                    result = result.Replace("[/Mailgroup]", string.Empty);
                    result = result.Replace("[/OurEmail]", string.Empty);
                }
                else
                {
                    result = result.Replace("[/Mailgroup]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailOrder.EmailGroup));
                    result = result.Replace("[/OurEmail]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailOrder.EmailGroup));
                }
                if (string.IsNullOrEmpty(bankInformation))
                    result = result.Replace("[/BANKINFORMATION]", string.Empty);
                else
                    result = result.Replace("[/BANKINFORMATION]", bankInformation);

                if (_store.storeID == "AEU")
                {
                    result = result.Replace("[/LegalForm]", string.Format("<tr><td>Legal Form:{0}</td><td>Legal Form:{1}</td><td>Legal Form:{2}</td></tr>",
                        order.cartX.SoldToContact.LegalForm, order.cartX.BillToContact.LegalForm, order.cartX.ShipToContact.LegalForm));
                }
                else
                    result = result.Replace("[/LegalForm]", string.Empty);

                result = result.Replace("[/PAYMENTTYPE]", paymentTypeContent);

                result = result.Replace("[/contactMail]", emailOrder.StoreEmail.ContactMail);
                result = result.Replace("[/CLICKHERE]", clickhere);

                result = result.Replace("[/SOLDTOATTENTION]", soldToAttention);
                result = result.Replace("[/BILLTOATTENTION]", billToAttention);
                result = result.Replace("[/SHIPTOATTENTION]", shipToAttention);
                result = result.Replace("[/SOLDTOCOMPANY]", soldToCompany);
                result = result.Replace("[/BILLTOCOMPANY]", billToCompany);
                result = result.Replace("[/SHIPTOCOMPANY]", shipToCompany);
                result = result.Replace("[/SOLDTOADDRESS]", soldToAddress);
                result = result.Replace("[/BILLTOADDRESS]", billToAddress);
                result = result.Replace("[/SHIPTOADDRESS]", shipToAddress);
                result = result.Replace("[/SOLDTOTEL]", soldToTel);
                result = result.Replace("[/BILLTOTEL]", billToTel);
                result = result.Replace("[/SHIPTOTEL]", shipToTel);
                result = result.Replace("[/SOLDTOMOBILE]", soldToMobile);
                result = result.Replace("[/BILLTOMOBILE]", billToMobile);
                result = result.Replace("[/SHIPTOMOBILE]", shipToMobile);
                result = result.Replace("[/SUBTOTAL]", subTotal);

                result = result.Replace("[/StoreName]", order.storeX.StoreName);
                result = result.Replace("[/OurFax]", emailOrder.StoreEmail.OurFax);
                result = result.Replace("[/termurl]", _store.profile.getStringSetting("eStore_termurl"));
                result = result.Replace("[/returnurl]", _store.profile.getStringSetting("eStore_returnurl"));
                result = result.Replace("[/privacyurl]", _store.profile.getStringSetting("eStore_privacyurl"));
                result = result.Replace("[/StoreLogo]", _store.profile.getStringSetting("eStore_StoreLogo"));

                if (Store.offerShippingService == false)
                {
                    result = result.Replace("[/SHIPPINGMETHOD]", string.Empty);
                    try
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Freight[\w\W]*?</tr>", "");
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    result = result.Replace("[/SHIPPINGMETHOD]", string.Format("<p><label>{1}: </label>{0}</p>", shippingMethod, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Shipping_Method, true, language)));
                    result = result.Replace("[/FREIGHT]", freight);
                }

                if (Store.hasTaxCalculator == false)
                {
                    try
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Tax[\w\W]*?</tr>", "");
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    result = result.Replace("[/TAXRATE]", taxrate);
                    result = result.Replace("[/TAX]", tax);
                }

                if (order.TotalDiscount != null && order.TotalDiscount > 0)
                    result = result.Replace("[/DISCOUNT]", discount);
                else
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""CartDiscount[\w\W]*?</tr>", "");

                if (order.Surcharge != null && order.Surcharge > 0)
                    result = result.Replace("[/SURCHARGE]", FormatPriceWithCurrencyDecimal(order.Surcharge.GetValueOrDefault(), order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate));
                else
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Surcharge[\w\W]*?</tr>", "");

                result = result.Replace("[/TOTAL]", total);
                result = result.Replace("[/subject_tax]", Store.specialTaxMessage(order.cartX.ShipToContact));
                if (order.DutyAndTax != null && order.DutyAndTax > 0)
                    result = result.Replace("[/DutyAndTax]", FormatPriceWithCurrencyDecimal(order.DutyAndTax.GetValueOrDefault(), order.cartX.localCurrencyX.CurrencySign, order.cartX.LocalCurExchangeRate));
                else
                {
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""DutyAndTax[\w\W]*?</tr>", "");
                }

                string internalorderDate = Store.getLocalTime(order.OrderDate.Value, order.userX);
                //Internal sales can see the available date and qty
                internalResult = result.Replace("[/CARTITEMS]", orderdetailItemsInternalDisplay);
                internalResult = internalResult.Replace("[/colspan]", "3");
                internalResult = internalResult.Replace("[/DetailModeTH]", string.Format("<th style='color: #F63'>{0}</th><th style='color: #F63'>{1}</th>",
                    Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_date, true, language), Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_Qty, true, language, minisite)));
                internalResult = internalResult.Replace("[/ORDERDATE]", internalorderDate);

                string customerorderDate = Store.getLocalTime(order.OrderDate.Value, order.userX, true);
                customerResult = result.Replace("[/CARTITEMS]", orderdetailItemsCustomerDisplay);
                customerResult = customerResult.Replace("[/colspan]", "1");
                customerResult = customerResult.Replace("[/DetailModeTH]", "");
                customerResult = customerResult.Replace("[/ORDERDATE]", customerorderDate);

                emailOrder.InternalResult = internalResult;
                emailOrder.CustomerResult = customerResult;
            }

            emailOrder.MailFromName = String.Format("{1} {0}", _store.storeID.ToUpper(), Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore, true, language,minisite));
            emailOrder.MailSubject = Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Ordering_Information, true, language, minisite);
            //return emailOrder;
        }

        /// <summary>
        ///  This function will compose the order confirm notice mail then send out.
        /// </summary>
        /// <param name="order"></param>
        /// <returns>Order notice EMail as string</returns>
        public EMailReponse getOrderMailContent(Order order, Language language = null, MiniSite minisite = null)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();

            try
            {
                if (order == null)
                    throw new Exception("OrderIsNull");

                EMailOrder emailOrder = new EMailOrder();
                emailOrder.UserName = Store.getCultureGreetingName(order.userX.FirstName, order.userX.LastName);
                emailOrder.CustomerEmail = order.UserID;

                //Payment Type 和 模板内容
                string paymentTypeName = Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type,true,language,minisite);
                switch (order.paymentTypeX)
                {
                    case Payment.Payment_Type.PayByWireTransfer:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByBankConfirmation, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>" + Store.Tanslation("eStorePayment_" + order.PaymentType) + "</p>";
                        break;
                    case Payment.Payment_Type.PayByCreditCard:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByCreditCardConfirmation, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Credit Card Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByNetTerm:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByNetTermConfirmation, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Net Term Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByDirectly:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByDirectly, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Direct payment in our office</p>";
                        break;
                    case Payment.Payment_Type.Daoupay:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByCreditCardConfirmation, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>"+ Store.Tanslation("eStorePayment_"+ order.PaymentType) +"</p>";
                        break;
                    default:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByBankConfirmation, language, minisite);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Wire Transfer</p>";
                        break;
                }

                LoadOrderMainContent(order, emailOrder, language, minisite);
                // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                if (testMode() == true)
                {
                    emailOrder.MailFromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language,minisite));
                    emailOrder.MailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language,minisite)) + emailOrder.MailSubject;
                    orderDeptEmailResponse = sendMail(testingOrderDeptEmail, emailOrder.CustomerEmail, emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.InternalResult, _store.storeID.ToUpper());

                    customerEmailResponse = sendMail(emailOrder.CustomerEmail, emailOrder.EmailGroup.Split(';')[0], emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.CustomerResult, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    orderDeptEmailResponse = sendMail(emailOrder.EmailGroup, emailOrder.CustomerEmail, emailOrder.MailFromName, "(" + Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal, true, language,minisite) + ")" + emailOrder.MailSubject, emailOrder.InternalResult, _store.storeID.ToUpper(), "", emailOrder.EmailGroup);

                    List<string> attacheds = null;
                    var settings = Store.getSettings();
                    if (settings.Keys.Contains("eStore_acceptation_file") && !string.IsNullOrEmpty(settings["eStore_acceptation_file"]))
                        attacheds = new List<string> { settings["eStore_acceptation_file"] };

                    customerEmailResponse = sendMail(emailOrder.CustomerEmail, emailOrder.EmailGroup.Split(';')[0], emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.CustomerResult, _store.storeID.ToUpper(), "", emailOrder.EmailGroup, attacheds);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else if (ex.Message == "OrderIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed to send order email", "", "", "", ex);
                return customerEmailResponse;
            }
        }

        //EDI Order
        public EMailReponse getEDIOrderMailContent(Order order)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();

            try
            {
                if (order == null)
                    throw new Exception("OrderIsNull");

                EMailOrder emailOrder = new EMailOrder();
                emailOrder.UserName = Store.getCultureGreetingName(order.userX.FirstName, order.userX.LastName);
                emailOrder.CustomerEmail = order.UserID;

                //Payment Type 和 模板内容
                string paymentTypeName = Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type);
                emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.EdiOrder);
                LoadOrderMainContent(order, emailOrder, null, null);
                emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Net Term Payment</p>";

                emailOrder.MailFromName = "EDI Order";
                emailOrder.MailSubject = "Advantech EDI Ordering Information";
                if (testMode())
                {
                    emailOrder.MailFromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester));
                    emailOrder.MailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING)) + emailOrder.MailSubject;
                    orderDeptEmailResponse = sendMail(eStoreAdminGroup, "eStoreEDI@advantech.com", emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.InternalResult, _store.storeID.ToUpper());
                }
                else
                {
                    string ltronSalesGroup = _store.profile.getStringSetting("LTronSalesGroup");
                    if (String.IsNullOrEmpty(ltronSalesGroup))
                        ltronSalesGroup = eStoreAdminGroup;
                    orderDeptEmailResponse = sendMail(ltronSalesGroup, "eStoreEDI@advantech.com", emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.InternalResult, _store.storeID.ToUpper(), eStoreAdminGroup);
                }

                customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                return customerEmailResponse;
            }
            catch (Exception ex)
            {
                if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else if (ex.Message == "OrderIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed to send order email", "", "", "", ex);
                return customerEmailResponse;
            }
        }

        public EMailReponse getOrderSyncMailContent(Order order, string userName, string customerEmail)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();

            try
            {
                if (order == null)
                    throw new Exception("OrderIsNull");

                EMailOrder emailOrder = new EMailOrder();
                emailOrder.UserName = userName;
                emailOrder.CustomerEmail = customerEmail;

                //Payment Type 和 模板内容
                string paymentTypeName = Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type);
                switch (order.paymentTypeX)
                {
                    case Payment.Payment_Type.PayByWireTransfer:
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Wire Transfer</p>";
                        break;
                    case Payment.Payment_Type.PayByCreditCard:
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Credit Card Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByNetTerm:
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Net Term Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByDirectly:
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Direct payment in our office</p>";
                        break;
                    default:
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Wire Transfer</p>";
                        break;
                }

                emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.OrderSyncConfirmation);

                LoadOrderMainContent(order, emailOrder);

                emailOrder.MailSubject = "Online customer order has been entered to SAP";

                // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                if (testMode() == true)
                {
                    emailOrder.MailFromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester));
                    emailOrder.MailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING)) + emailOrder.MailSubject;
                    orderDeptEmailResponse = sendMail(testingOrderDeptEmail, emailOrder.CustomerEmail.Split(';')[0], emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.InternalResult, _store.storeID.ToUpper());

                    customerEmailResponse = sendMail(emailOrder.CustomerEmail, emailOrder.EmailGroup.Split(';')[0], emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.CustomerResult, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    //正式的  发给用户,抄送给我们
                    customerEmailResponse = sendMail(emailOrder.CustomerEmail, emailOrder.EmailGroup.Split(';')[0], emailOrder.MailFromName, emailOrder.MailSubject, emailOrder.CustomerResult, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else if (ex.Message == "OrderIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed to send order email", "", "", "", ex);
                return customerEmailResponse;
            }
        }

        public string getOrderMailHtml(Order order,Language language = null, MiniSite minisite = null)
        {
            string htmlContent = string.Empty;
            try
            {
                if (order == null)
                    htmlContent = "OrderIsNull";

                EMailOrder emailOrder = new EMailOrder();
                emailOrder.UserName = Store.getCultureGreetingName(order.userX.FirstName, order.userX.LastName);
                emailOrder.CustomerEmail = order.UserID;

                //Payment Type 和 模板内容
                string paymentTypeName = Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Payment_Type, true, language,minisite);
                switch (order.paymentTypeX)
                {
                    case Payment.Payment_Type.PayByWireTransfer:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByBankConfirmation, language);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Wire Transfer</p>";
                        break;
                    case Payment.Payment_Type.PayByCreditCard:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByCreditCardConfirmation, language);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Credit Card Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByNetTerm:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByNetTermConfirmation, language);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Net Term Payment</p>";
                        break;
                    case Payment.Payment_Type.PayByDirectly:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByDirectly, language);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Direct payment in our office</p>";
                        break;
                    default:
                        emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.PayByBankConfirmation, language);
                        emailOrder.PaymentTypeContent = "<p><label>" + paymentTypeName + " : </label>Wire Transfer</p>";
                        break;
                }

                LoadOrderMainContent(order, emailOrder, language, minisite);

                htmlContent = emailOrder.CustomerResult;
            }
            catch (Exception)
            {
                htmlContent = "It's have exception. please contact estore.it!";
            }
            return htmlContent;
        }
        
        private void LoadQuotationMainContent(Quotation quote, EMailOrder emailQuotation, Language language = null, MiniSite minisite = null)
        {
            string quoteMail = emailQuotation.MailContent;
            emailQuotation.StoreEmail.ContactMail = _store.profile.OrderDeptEmail;
            
            string customerERPID = (string.IsNullOrEmpty(quote.userX.FederalID)) ? "" : quote.userX.FederalID;

            setEmailOrderInfor(quote.cartX, minisite, ref emailQuotation);

            if (quote.isPStoreOrder())
            {
                string pemailGroup = _store.profile.getStringSetting("ePAPSEmailGroup");
                if (!string.IsNullOrEmpty(pemailGroup))
                {
                    emailQuotation.EmailGroup = pemailGroup;
                }
            }
            if (string.IsNullOrEmpty(emailQuotation.StoreEmail.OurTel))
                SetContactBaseInfor(_store, quote.cartX.businessGroup.ToString(), emailQuotation.StoreEmail);
            string estoreUrl = Store.getCurrStoreUrl(_store.profile,minisite);
            string quoteNo = quote.QuotationNumber;
            string comments = (string.IsNullOrEmpty(quote.Comments)) ? "" : quote.Comments.Trim();
            string promotionCode = (string.IsNullOrEmpty(quote.PromoteCode)) ? "" : String.Format("{1}: {0} <br />", quote.PromoteCode, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Promotion_Code, true, language,minisite));
            string quotedetailInternalDisplay = null;
            string quotedetailCustomerDisplay = null;


            string result = null;
            string resultInternalDisplay = null;
            string resultCustomerDisplay = null;


            //string soldToAttention = quote.cartX.SoldToContact.Attention;
            string soldToAttention = Store.getCultureFullName(quote.cartX.SoldToContact);
            string soldToCompany = quote.cartX.SoldToContact.AttCompanyName;
            string soldToCounty = (string.IsNullOrEmpty(quote.cartX.SoldToContact.County)) ? "" : quote.cartX.SoldToContact.County;
            string soldToState = (string.IsNullOrEmpty(quote.cartX.SoldToContact.State)) ? "" : quote.cartX.SoldToContact.State;

            //soldToAddress修改后的显示格式
            string soldToAddress = formatContactAddress(quote.cartX.SoldToContact);

            string soldToTel = (string.IsNullOrEmpty(quote.cartX.SoldToContact.TelExt)) ? quote.cartX.SoldToContact.TelNo : quote.cartX.SoldToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.SoldToContact.TelExt;
            string soldToMobile = quote.cartX.SoldToContact.Mobile;

            //string billToAttention = quote.cartX.BillToContact.Attention;
            string billToAttention = Store.getCultureFullName(quote.cartX.BillToContact);
            string billToCompany = quote.cartX.BillToContact.AttCompanyName;
            //string billToAddress = (string.IsNullOrEmpty(quote.cartX.BillToContact.Address2)) ? quote.cartX.BillToContact.Address1 : quote.cartX.BillToContact.Address1 + "<br /> " + quote.cartX.BillToContact.Address2;
            string billToCounty = (string.IsNullOrEmpty(quote.cartX.BillToContact.County)) ? "" : quote.cartX.BillToContact.County;
            string billToState = (string.IsNullOrEmpty(quote.cartX.BillToContact.State)) ? "" : quote.cartX.BillToContact.State;

            //billToAddress修改后的显示格式
            string billToAddress = formatContactAddress(quote.cartX.BillToContact);

            string billToTel = (string.IsNullOrEmpty(quote.cartX.BillToContact.TelExt)) ? quote.cartX.BillToContact.TelNo : quote.cartX.BillToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.BillToContact.TelExt;
            string billToMobile = quote.cartX.BillToContact.Mobile;

            //string shipToAttention = quote.cartX.ShipToContact.Attention;
            string shipToAttention = Store.getCultureFullName(quote.cartX.ShipToContact);
            string shipToCompany = quote.cartX.ShipToContact.AttCompanyName;
            //string shipToAddress = (string.IsNullOrEmpty(quote.cartX.ShipToContact.Address2)) ? quote.cartX.ShipToContact.Address1 : quote.cartX.ShipToContact.Address1 + "<br /> " + quote.cartX.ShipToContact.Address2;
            string shipToCounty = (string.IsNullOrEmpty(quote.cartX.ShipToContact.County)) ? "" : quote.cartX.ShipToContact.County;
            string shipToState = (string.IsNullOrEmpty(quote.cartX.ShipToContact.State)) ? "" : quote.cartX.ShipToContact.State;

            //billToAddress修改后的显示格式
            string shipToAddress = formatContactAddress(quote.cartX.ShipToContact);

            string shipToTel = (string.IsNullOrEmpty(quote.cartX.ShipToContact.TelExt)) ? quote.cartX.ShipToContact.TelNo : quote.cartX.ShipToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.ShipToContact.TelExt;
            string shipToMobile = quote.cartX.ShipToContact.Mobile;
            string checkquote = Store.getCurrStoreUrl(quote.cartX.storeX, minisite);
            //If TestingMode is true, then use QA URL as StoreURL
            checkquote = (System.Configuration.ConfigurationManager.AppSettings.Get("TestingMode") == "true") ? System.Configuration.ConfigurationManager.AppSettings.Get("QATestingServerURL") : checkquote;

            checkquote = checkquote + "/Quotation/confirm.aspx?quotation=" + quote.QuotationNumber + "&storeid=" + quote.StoreID;

            string comment = string.IsNullOrEmpty(quote.Comments) ? string.Empty : quote.Comments.Trim();



            if (string.IsNullOrEmpty(quoteMail))
            {
                throw new Exception("FailToGetTemplate");
            }
            else
            {
                string cartTotalAmount = FormatPriceWithCurrencyDecimal(quote.cartX.TotalAmount, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string freight = FormartFreight(quote.Freight, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string taxrate = formatTaxRate(quote.TaxRate);
                string tax = FormartTax(quote.Tax, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string discount = "-" + FormatPriceWithCurrencyDecimal(quote.TotalDiscount.GetValueOrDefault(), quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string quoteTotalAmount = FormatPriceWithCurrencyDecimal(quote.totalAmountX, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                if (isShowStorePrice)
                    quoteTotalAmount += string.Format("<br />({0})", FormatPriceWithCurrencyDecimal(quote.totalAmountX, quote.currencySign, null));

                string shippingMethod = quote.ShippingMethod;

                //Internal quotation email display
                quotedetailInternalDisplay = showCartItems(quote.cartX, Store, true);
                //Customer quotation email display
                quotedetailCustomerDisplay = showCartItems(quote.cartX, Store, false);

                result = quoteMail.Replace("[/USERNAME]", emailQuotation.UserName);
                //result = result.Replace("[/LASTNAME]", lastname);
                result = result.Replace("[/ERPID]", customerERPID);
                result = result.Replace("[/OurTel]", emailQuotation.StoreEmail.OurTel);
                result = result.Replace("[/STOREURL]", estoreUrl);
                result = result.Replace("[/ourAddress]", emailQuotation.StoreEmail.OurAddress);
                result = result.Replace("[/QUOTENUMBER]", quoteNo);
                if (minisite != null && (minisite.MiniSiteType == MiniSite.SiteType.IotMart || minisite.MiniSiteType == MiniSite.SiteType.UShop))
                    result = result.Replace("/AUS/logo.gif", string.Format("/{0}/{1}/logo.gif", minisite.StoreID, minisite.SiteName));

                if (string.IsNullOrEmpty(promotionCode))
                    result = result.Replace("[/PROMOTIONCODE]", string.Empty);
                else
                    result = result.Replace("[/PROMOTIONCODE]", string.Format("<p><label>{1}: </label>{0}</p>", promotionCode, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Promotion_Code, true, language,minisite)));

                if (string.IsNullOrEmpty(quote.ShipmentTerm))
                    result = result.Replace("[/Courier_account]", string.Empty);
                else
                    result = result.Replace("[/Courier_account]", string.Format("<p><label>{1}: </label>{0}</p>", quote.ShipmentTerm, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Courier_Account, true, language, minisite)));

                if (string.IsNullOrEmpty(quote.RegistrationNumber))
                    result = result.Replace("[/Company_Registration_Number]", string.Empty);
                else
                    result = result.Replace("[/Company_Registration_Number]", string.Format("<p><label>{1}: {0}</label></p>", quote.RegistrationNumber, "Company Registration Number"));

                if (string.IsNullOrEmpty(quote.ResellerID))
                    result = result.Replace("[/RESELLERID]", string.Empty);
                else
                    result = result.Replace("[/RESELLERID]", string.Format("<p><label>{1}: </label>{0}</p>", quote.ResellerID, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Reseller_ID, true, language, minisite)));

                List<string> vatNo = new List<string>();

                if (string.IsNullOrEmpty(quote.cartX.SoldToContact.VATNumbe))
                    result = result.Replace("[/SOLDVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/SOLDVATADDRESS]", string.Format("VAT:{0}", quote.cartX.SoldToContact.VATNumbe));
                    vatNo.Add(quote.cartX.SoldToContact.VATNumbe);
                }

                if (string.IsNullOrEmpty(quote.cartX.BillToContact.VATNumbe))
                    result = result.Replace("[/BILLVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/BILLVATADDRESS]", string.Format("VAT:{0}", quote.cartX.BillToContact.VATNumbe));
                    vatNo.Add(quote.cartX.BillToContact.VATNumbe);
                }

                if (string.IsNullOrEmpty(quote.cartX.ShipToContact.VATNumbe))
                    result = result.Replace("[/SHIPVATADDRESS]", string.Empty);
                else
                {
                    result = result.Replace("[/SHIPVATADDRESS]", string.Format("VAT:{0}", quote.cartX.ShipToContact.VATNumbe));
                    vatNo.Add(quote.cartX.ShipToContact.VATNumbe);
                }

                if (!vatNo.Any())
                    result = result.Replace("[/VATADDRESS]", string.Empty);
                else
                    result = result.Replace("[/VATADDRESS]", string.Format("<br />VAT Number: {0}", string.Join(" / ", vatNo.Distinct())));

                if (string.IsNullOrEmpty(emailQuotation.EmailGroup))
                {
                    result = result.Replace("[/Mailgroup]", string.Empty);
                    result = result.Replace("[/OurEmail]", string.Empty);
                }
                else
                {
                    result = result.Replace("[/Mailgroup]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailQuotation.EmailGroup));
                    result = result.Replace("[/OurEmail]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailQuotation.EmailGroup));
                }                    

                result = result.Replace("[/CHECKQUOTE]", checkquote);
                result = result.Replace("[/COMMENTS]", comments);
                result = result.Replace("[/SOLDTOATTENTION]", soldToAttention);
                result = result.Replace("[/BILLTOATTENTION]", billToAttention);
                result = result.Replace("[/SHIPTOATTENTION]", shipToAttention);
                result = result.Replace("[/SOLDTOCOMPANY]", soldToCompany);
                result = result.Replace("[/BILLTOCOMPANY]", billToCompany);
                result = result.Replace("[/SHIPTOCOMPANY]", shipToCompany);
                result = result.Replace("[/SOLDTOADDRESS]", soldToAddress);
                result = result.Replace("[/BILLTOADDRESS]", billToAddress);
                result = result.Replace("[/SHIPTOADDRESS]", shipToAddress);
                result = result.Replace("[/SOLDTOTEL]", soldToTel);
                result = result.Replace("[/BILLTOTEL]", billToTel);
                result = result.Replace("[/SHIPTOTEL]", shipToTel);
                result = result.Replace("[/SOLDTOMOBILE]", soldToMobile);
                result = result.Replace("[/BILLTOMOBILE]", billToMobile);
                result = result.Replace("[/SHIPTOMOBILE]", shipToMobile);
                result = result.Replace("[/SUBTOTAL]", cartTotalAmount);

                if (Store.offerShippingService == false)
                {
                    result = result.Replace("[/SHIPPINGMETHOD]", string.Empty);
                    try
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Freight[\w\W]*?</tr>", "");
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    result = result.Replace("[/SHIPPINGMETHOD]", string.Format("<p><label>{1}: </label>{0}</p>", shippingMethod, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Shipping_Method, true, language, minisite)));
                    result = result.Replace("[/FREIGHT]", freight);
                }

                if (Store.hasTaxCalculator == false)
                {
                    try
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Tax[\w\W]*?</tr>", "");
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    result = result.Replace("[/TAXRATE]", taxrate);
                    result = result.Replace("[/TAX]", tax);
                }
                result = result.Replace("[/Comment]", comment);
                if (_store.storeID == "AEU")
                {
                    result = result.Replace("[/LegalForm]", string.Format("<tr><td>Legal Form:{0}</td><td>Legal Form:{1}</td><td>Legal Form:{2}</td></tr>",
                        quote.cartX.SoldToContact.LegalForm, quote.cartX.BillToContact.LegalForm, quote.cartX.ShipToContact.LegalForm));
                }
                else
                    result = result.Replace("[/LegalForm]", string.Empty);

                if (quote.TotalDiscount != null && quote.TotalDiscount > 0)
                    result = result.Replace("[/DISCOUNT]", discount);
                else
                {
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""CartDiscount[\w\W]*?</tr>", "");
                }

                if (quote.DutyAndTax != null && quote.DutyAndTax > 0)
                    result = result.Replace("[/DutyAndTax]", FormatPriceWithCurrencyDecimal(quote.DutyAndTax.GetValueOrDefault(), quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate));
                else
                {
                    result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""DutyAndTax[\w\W]*?</tr>", "");
                }

                if (quote.StoreID.ToUpper() == "AEU" && (quote.needVATReview() || quote.needFreightReview()))
                    result = result.Replace("[/QUOTEINFORMATION]", "<span style=\"font-size:24px; color:#003D7C\"><b>THIS IS NOT A QUOTE CONFIRMATION</b><br /></span>");
                else
                    result = result.Replace("[/QUOTEINFORMATION]", string.Empty);
                result = result.Replace("[/SupportInfo]", "<p>Thank you for allowing Advantech online store to provide you with a Quotation Request. </p>");
                result = result.Replace("[/TOTAL]", quoteTotalAmount);
                result = result.Replace("[/subject_tax]", Store.specialTaxMessage(quote.cartX.ShipToContact));

                result = result.Replace("[/StoreName]", quote.storeX.StoreName);
                result = result.Replace("[/OurFax]", emailQuotation.StoreEmail.OurFax);
                result = result.Replace("[/termurl]", _store.profile.getStringSetting("eStore_termurl"));
                result = result.Replace("[/returnurl]", _store.profile.getStringSetting("eStore_returnurl"));
                result = result.Replace("[/privacyurl]", _store.profile.getStringSetting("eStore_privacyurl"));
                result = result.Replace("[/StoreLogo]", _store.profile.getStringSetting("eStore_StoreLogo"));

                string InternalquoteDate = Store.getLocalTime(quote.QuoteDate.Value, quote.userX);
                string InternalvalidDate = Store.getLocalTime(quote.QuoteExpiredDate.Value, quote.userX);

                resultInternalDisplay = result.Replace("[/CARTITEMS]", quotedetailInternalDisplay);
                resultInternalDisplay = resultInternalDisplay.Replace("[/colspan]", "3");
                resultInternalDisplay = resultInternalDisplay.Replace("[/DetailModeTH]", string.Format("<th style='color: #F63'>{0}</th><th style='color: #F63'>{1}</th>",
                            Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_date, true, language), Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_Qty, true, language, minisite)));
                resultInternalDisplay = resultInternalDisplay.Replace("[/QUOTEDATE]", InternalquoteDate);
                resultInternalDisplay = resultInternalDisplay.Replace("[/ValidDate]", InternalvalidDate);


                string CustomerlquoteDate = Store.getLocalTime(quote.QuoteDate.Value, quote.userX, true);
                string CustomervalidDate = Store.getLocalTime(quote.QuoteExpiredDate.Value, quote.userX, true);

                resultCustomerDisplay = result.Replace("[/CARTITEMS]", quotedetailCustomerDisplay);
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/colspan]", "1");
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/DetailModeTH]", "");
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/QUOTEDATE]", CustomerlquoteDate);
                resultCustomerDisplay = resultCustomerDisplay.Replace("[/ValidDate]", CustomervalidDate);

                emailQuotation.InternalResult = resultInternalDisplay;
                emailQuotation.CustomerResult = resultCustomerDisplay;
            }

            emailQuotation.MailFromName = String.Format("{1} {0}", _store.storeID.ToUpper(), Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore, true, language, minisite));
            emailQuotation.MailSubject = Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Quotation_Information, true, language, minisite);
        }

        /// <summary>
        /// This function will compose the quotation confirm notice mail then send out.
        /// </summary>
        /// <param name="quote"></param>
        /// <returns></returns>
        public EMailReponse getQuoteMailContent(Quotation quote, string mailto = null, string mailfrom = null, Language language = null, MiniSite minisite = null)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();
            try
            {
                if (quote == null)
                    throw new Exception("QuotationIsNull");

                EMailOrder emailQuotation = new EMailOrder();

                //string firstname = quote.userX.FirstName.ToUpper();
                //string lastname = quote.userX.LastName.ToUpper();
                emailQuotation.UserName = Store.getCultureGreetingName(quote.userX.FirstName, quote.userX.LastName);
                emailQuotation.CustomerEmail = quote.userX.UserID;
                //add by cherry, for OM use.
                if (!string.IsNullOrEmpty(mailto))
                    emailQuotation.CustomerEmail = mailto;

                //add by cherry, for OM use.
                emailQuotation.EmailGroup = _store.profile.OrderDeptEmail;
                if (!string.IsNullOrEmpty(mailfrom))
                    emailQuotation.EmailGroup = mailfrom;

                emailQuotation.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.QuotationConfirmation, language,minisite);

                LoadQuotationMainContent(quote, emailQuotation, language, minisite);

                // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                if (testMode() == true)
                {
                    emailQuotation.MailFromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language, minisite));
                    emailQuotation.MailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language, minisite)) + emailQuotation.MailSubject;
                    orderDeptEmailResponse = sendMail(testingOrderDeptEmail, emailQuotation.CustomerEmail, emailQuotation.MailFromName, emailQuotation.MailSubject, emailQuotation.InternalResult, _store.storeID.ToUpper());
                    customerEmailResponse = sendMail(emailQuotation.CustomerEmail, emailQuotation.EmailGroup.Split(';')[0], emailQuotation.MailFromName, emailQuotation.MailSubject, emailQuotation.CustomerResult, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    orderDeptEmailResponse = sendMail(emailQuotation.EmailGroup, emailQuotation.CustomerEmail, emailQuotation.MailFromName, "(" + Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal, true, language, minisite) + ")" + emailQuotation.MailSubject, emailQuotation.InternalResult, _store.storeID.ToUpper(), "", emailQuotation.EmailGroup);

                    List<string> attacheds = null;
                    var settings = Store.getSettings();
                    if (settings.Keys.Contains("eStore_acceptation_file") && !string.IsNullOrEmpty(settings["eStore_acceptation_file"]))
                        attacheds = new List<string> { settings["eStore_acceptation_file"] };

                    customerEmailResponse = sendMail(emailQuotation.CustomerEmail, emailQuotation.EmailGroup.Split(';')[0], emailQuotation.MailFromName, emailQuotation.MailSubject, emailQuotation.CustomerResult, _store.storeID.ToUpper(), "", emailQuotation.EmailGroup, attacheds);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "QuotationIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed to send quotation email.", "", "", "", ex);
                return customerEmailResponse;
            }
        }

        public string getQuoteMailHtml(Quotation quote, Language language = null, MiniSite minisite = null)
        {
            string htmlContent = string.Empty;
            try
            {
                EMailOrder emailQuotation = new EMailOrder();

                emailQuotation.UserName = Store.getCultureGreetingName(quote.userX.FirstName, quote.userX.LastName);
                emailQuotation.CustomerEmail = quote.userX.UserID;
                emailQuotation.EmailGroup = _store.profile.OrderDeptEmail;

                emailQuotation.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.QuotationConfirmation, language);

                LoadQuotationMainContent(quote, emailQuotation, language, minisite);

                htmlContent = emailQuotation.CustomerResult;;
            }
            catch (Exception)
            {
                htmlContent = "It's have exception. please contact estore.it!";
            }
            return htmlContent;
        }

        /// <summary>
        /// This function will compose the transferred confirm notice mail then send out.
        /// System will send a internal mail to sales(who confirmed the quotation) and a external mail to customer(who is user has a transferred quotation)
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="receiver">Store representative sales who take charge of this quotation</param>
        /// <returns></returns>
        public EMailReponse getTransferredQuoteMailContent(Quotation quote, User receiver, Language language = null, MiniSite minisite = null)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();
            
            try
            {
                if (quote == null)
                    throw new Exception("QuotationIsNull.");

                if (receiver == null)
                    throw new Exception("ReceiverUserIsNull");

                string confirmBy = quote.ConfirmedBy;
                string quoteMail = null;

                //Receiver information
                //string receiverName = receiver.FirstName.ToUpper() + " " + receiver.LastName.ToUpper();
                //string receiverFirstname = receiver.FirstName.ToUpper();
                //string receiverLastname = receiver.LastName.ToUpper();
                string RECEIVERNAME = Store.getCultureGreetingName(receiver.FirstName, receiver.LastName);

                string receiverEmail = receiver.UserID;
                string receiverCompany = receiver.CompanyName;
                string receiverAddress = (string.IsNullOrEmpty(receiver.mainContact.Address2)) ? receiver.mainContact.Address1 : receiver.mainContact.Address1 + "<br /> " + receiver.mainContact.Address2;
                string receiverTel = (string.IsNullOrEmpty(receiver.mainContact.TelExt)) ? receiver.mainContact.TelNo : receiver.mainContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ":" + receiver.mainContact.TelExt;
                string receiverMobile = receiver.mainContact.Mobile;

                UserHelper userHelper = new UserHelper();
                User quoteCreater = userHelper.getUserbyID(quote.ConfirmedBy);
                string createrEmail = quoteCreater.UserID;
                //string createrFirstname = quoteCreater.FirstName.ToUpper();
                // string createrLastname = quoteCreater.LastName.ToUpper();
                string REPRESENTATIVENAME = Store.getCultureFullName(quoteCreater.FirstName, quoteCreater.LastName);
                string customerERPID = (string.IsNullOrEmpty(receiver.FederalID)) ? "" : receiver.FederalID;


                string ourAddress = string.Empty;
                POCOS.Address _storeAddress = Store.getAddressByCountry(quote.cartX.BillToContact.Country, quote.cartX.businessGroup);
                string contactMail = _store.profile.OrderDeptEmail;
                string emailGroup = contactMail;
                string comments = (string.IsNullOrEmpty(quote.Comments)) ? "" : quote.Comments.Trim();
                string ourTel = string.Empty;
                string ourFax = string.Empty;

                if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup")) // now only ushop minisite use site Parameter email group
                    emailGroup = minisite.Settings["EmailGroup"];
                else
                {
                    if (_storeAddress != null)
                    {
                        ourTel = string.Format("{0} {1}", _storeAddress.Tel, _storeAddress.ServiceTime);
                        ourAddress = _storeAddress.fullAddress;
                        if (string.IsNullOrEmpty(_storeAddress.EmailGroup))
                            emailGroup = contactMail;
                        else if (!contactMail.Split(';').Contains(_storeAddress.EmailGroup))
                        {
                            if (!(_storeAddress.Exclusive.HasValue && _storeAddress.Exclusive.Value))
                                emailGroup = contactMail + ";" + _storeAddress.EmailGroup;
                            else
                                emailGroup = _storeAddress.EmailGroup;
                        }


                        //if (!string.IsNullOrEmpty(_storeAddress.EmailGroup) && !_store.profile.OrderDeptEmail.Split(';').Contains(emailGroup))
                        //    emailGroup = ";" + _storeAddress.EmailGroup;
                    }
                    if (string.IsNullOrEmpty(ourTel))
                    {
                        ContactBaseInfor bi = new ContactBaseInfor();
                        SetContactBaseInfor(_store, quote.cartX.businessGroup.ToString(), bi);
                        ourTel = bi.OurTel;
                        ourFax = bi.OurFax;
                    }
                }

                string estoreUrl = Store.getCurrStoreUrl(_store.profile,minisite);
                string quoteNo = quote.QuotationNumber;
                string promotionCode = quote.PromoteCode;
                string quotedetailInternalDisplay = null;
                string quotedetailCustomerDisplay = null;
                string representativeEmail = receiver.UserID;
                string representativeContactPhone = quoteCreater.mainContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quoteCreater.TelExt;
                string representativeUserName = Store.getCultureFullName(receiver.FirstName, receiver.LastName);
                string result = null;
                string resultInternalDisplay = null;
                string resultCustomerDisplay = null;

                string soldToAttention = Store.getCultureFullName(quote.cartX.SoldToContact);
                string soldToCompany = quote.cartX.SoldToContact.AttCompanyName;
                string soldToCounty = (string.IsNullOrEmpty(quote.cartX.SoldToContact.County)) ? "" : ", " + quote.cartX.SoldToContact.County;
                string soldToState = (string.IsNullOrEmpty(quote.cartX.SoldToContact.State)) ? "" : ", " + quote.cartX.SoldToContact.State;
                string soldToAddress = formatContactAddress(quote.cartX.SoldToContact);
                string soldToTel = (string.IsNullOrEmpty(quote.cartX.SoldToContact.TelExt)) ? quote.cartX.SoldToContact.TelNo : quote.cartX.SoldToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.SoldToContact.TelExt;
                string soldToMobile = quote.cartX.SoldToContact.Mobile;

                string billToAttention = Store.getCultureFullName(quote.cartX.BillToContact);
                string billToCompany = quote.cartX.BillToContact.AttCompanyName;
                string billToAddress = formatContactAddress(quote.cartX.BillToContact);
                string billToCounty = (string.IsNullOrEmpty(quote.cartX.BillToContact.County)) ? "" : ", " + quote.cartX.BillToContact.County;
                string billToState = (string.IsNullOrEmpty(quote.cartX.BillToContact.State)) ? "" : ", " + quote.cartX.BillToContact.State;
                string billToTel = (string.IsNullOrEmpty(quote.cartX.BillToContact.TelExt)) ? quote.cartX.BillToContact.TelNo : quote.cartX.BillToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.BillToContact.TelExt;
                string billToMobile = quote.cartX.BillToContact.Mobile;

                string shipToAttention = Store.getCultureFullName(quote.cartX.ShipToContact);
                string shipToCompany = quote.cartX.ShipToContact.AttCompanyName;
                string shipToAddress = formatContactAddress(quote.cartX.ShipToContact);
                string shipToCounty = (string.IsNullOrEmpty(quote.cartX.ShipToContact.County)) ? "" : ", " + quote.cartX.ShipToContact.County;
                string shipToState = (string.IsNullOrEmpty(quote.cartX.ShipToContact.State)) ? "" : ", " + quote.cartX.ShipToContact.State;
                string shipToTel = (string.IsNullOrEmpty(quote.cartX.ShipToContact.TelExt)) ? quote.cartX.ShipToContact.TelNo : quote.cartX.ShipToContact.TelNo + ", " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext, true, language,minisite) + ": " + quote.cartX.ShipToContact.TelExt;
                string shipToMobile = quote.cartX.ShipToContact.Mobile;
                string checkquote = Store.getCurrStoreUrl(quote.cartX.storeX, minisite);
                //If TestingMode is true, then use QA URL as StoreURL
                checkquote = (System.Configuration.ConfigurationManager.AppSettings.Get("TestingMode") == "true") ? System.Configuration.ConfigurationManager.AppSettings.Get("QATestingServerURL") : checkquote;

                checkquote = checkquote + "/Quotation/confirm.aspx?quotation=" + quote.QuotationNumber + "&storeid=" + quote.StoreID;

                string comment = string.IsNullOrEmpty(quote.Comments) ? string.Empty : quote.Comments.Trim();

                quoteMail = _store.getTemplate(Store.TEMPLATE_TYPE.TransferredQuotationNotice, language, minisite);

                if (string.IsNullOrEmpty(quoteMail))
                {
                    throw new Exception("FailToGetTemplate");
                }
                else
                {
                    string cartTotalAmount = FormatPriceWithCurrencyDecimal(quote.cartX.TotalAmount, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                    string freight = FormartFreight(quote.Freight, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                    string taxrate = formatTaxRate(quote.TaxRate);
                    string tax = FormartTax(quote.Tax, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                    string quoteTotalAmount = FormatPriceWithCurrencyDecimal(quote.totalAmountX, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                    if (isShowStorePrice)
                        quoteTotalAmount += string.Format("<br />({0}{1})", FormatPriceWithCurrencyDecimal(quote.totalAmountX, quote.currencySign, null));
                    string shippingMethod = quote.ShippingMethod;
                    string discount = "-" + FormatPriceWithCurrencyDecimal(quote.TotalDiscount.GetValueOrDefault(), quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                    //Internal quotation email display
                    quotedetailInternalDisplay += showCartItems(quote.cartX, Store, true);
                    //Customer quotation email display
                    quotedetailCustomerDisplay += showCartItems(quote.cartX, Store, false);

                    result = quoteMail.Replace("[/RECEIVERNAME]", RECEIVERNAME);
                    //result = result.Replace("[/RECEIVER_LASTNAME]", receiverLastname);
                    result = result.Replace("[/ERPID]", customerERPID);
                    result = result.Replace("[/OurTel]", ourTel);
                    result = result.Replace("[/STOREURL]", estoreUrl);
                    result = result.Replace("[/ourAddress]", ourAddress);
                    result = result.Replace("[/CHECKQUOTE]", checkquote);
                    result = result.Replace("[/QUOTENUMBER]", quoteNo);
                    //result = result.Replace("[/RepresentativeUserName]", createrName);
                    result = result.Replace("[/REPRESENTATIVENAME]", REPRESENTATIVENAME);
                    //result = result.Replace("[/REPRESENTATIVE_LASTNAME]", createrLastname); 
                    result = result.Replace("[/RepresentativeEmail]", createrEmail);
                    result = result.Replace("[/RepresentativeContactPhone]", representativeContactPhone);

                    result = result.Replace("[/StoreName]", quote.storeX.StoreName);
                    result = result.Replace("[/OurFax]", ourFax);
                    result = result.Replace("[/termurl]", _store.profile.getStringSetting("eStore_termurl"));
                    result = result.Replace("[/returnurl]", _store.profile.getStringSetting("eStore_returnurl"));
                    result = result.Replace("[/privacyurl]", _store.profile.getStringSetting("eStore_privacyurl"));
                    result = result.Replace("[/StoreLogo]", _store.profile.getStringSetting("eStore_StoreLogo"));

                    if (string.IsNullOrEmpty(promotionCode))
                        result = result.Replace("[/PROMOTIONCODE]", string.Empty);
                    else
                        result = result.Replace("[/PROMOTIONCODE]", string.Format("<p><label>{1}: </label>{0}</p>", promotionCode, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Promotion_Code, true, language,minisite)));

                    if (string.IsNullOrEmpty(quote.ShipmentTerm))
                        result = result.Replace("[/Courier_account]", string.Empty);
                    else
                        result = result.Replace("[/Courier_account]", string.Format("<p><label>{1}: </label>{0}</p>", quote.ShipmentTerm, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Courier_Account, true, language, minisite)));

                    if (string.IsNullOrEmpty(quote.RegistrationNumber))
                        result = result.Replace("[/Company_Registration_Number]", string.Empty);
                    else
                        result = result.Replace("[/Company_Registration_Number]", string.Format("<p><label>{1}: {0}</label></p>", quote.RegistrationNumber, "Company Registration Number"));

                    if (string.IsNullOrEmpty(quote.ResellerID))
                        result = result.Replace("[/RESELLERID]", string.Empty);
                    else
                        result = result.Replace("[/RESELLERID]", string.Format("<p><label>{1}: </label>{0}</p>", quote.ResellerID, Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Reseller_ID, true, language, minisite)));

                    if (string.IsNullOrEmpty(emailGroup))
                    {
                        result = result.Replace("[/Mailgroup]", string.Empty);
                        result = result.Replace("[/OurEmail]", string.Empty);
                    }
                    else
                    {
                        result = result.Replace("[/Mailgroup]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailGroup));
                        result = result.Replace("[/OurEmail]", string.Format("<a href=\"mailto:{0}\">{0}</a>", emailGroup));
                    }                        

                    List<string> vatNo = new List<string>();

                    if (string.IsNullOrEmpty(quote.cartX.SoldToContact.VATNumbe))
                        result = result.Replace("[/SOLDVATADDRESS]", string.Empty);
                    else
                    {
                        result = result.Replace("[/SOLDVATADDRESS]", string.Format("VAT:{0}", quote.cartX.SoldToContact.VATNumbe));
                        vatNo.Add(quote.cartX.SoldToContact.VATNumbe);
                    }

                    if (string.IsNullOrEmpty(quote.cartX.BillToContact.VATNumbe))
                        result = result.Replace("[/BILLVATADDRESS]", string.Empty);
                    else
                    {
                        result = result.Replace("[/BILLVATADDRESS]", string.Format("VAT:{0}", quote.cartX.BillToContact.VATNumbe));
                        vatNo.Add(quote.cartX.BillToContact.VATNumbe);
                    }

                    if (string.IsNullOrEmpty(quote.cartX.ShipToContact.VATNumbe))
                        result = result.Replace("[/SHIPVATADDRESS]", string.Empty);
                    else
                    {
                        result = result.Replace("[/SHIPVATADDRESS]", string.Format("VAT:{0}", quote.cartX.ShipToContact.VATNumbe));
                        vatNo.Add(quote.cartX.ShipToContact.VATNumbe);
                    }

                    if (!vatNo.Any())
                        result = result.Replace("[/VATADDRESS]", string.Empty);
                    else
                        result = result.Replace("[/VATADDRESS]", string.Format("<br />VAT Number: {0}", string.Join(" / ", vatNo.Distinct())));

                    result = result.Replace("[/COMMENTS]", comments);
                    result = result.Replace("[/SOLDTOATTENTION]", soldToAttention);
                    result = result.Replace("[/BILLTOATTENTION]", billToAttention);
                    result = result.Replace("[/SHIPTOATTENTION]", shipToAttention);
                    result = result.Replace("[/SOLDTOCOMPANY]", soldToCompany);
                    result = result.Replace("[/BILLTOCOMPANY]", billToCompany);
                    result = result.Replace("[/SHIPTOCOMPANY]", shipToCompany);
                    result = result.Replace("[/SOLDTOADDRESS]", soldToAddress);
                    result = result.Replace("[/BILLTOADDRESS]", billToAddress);
                    result = result.Replace("[/SHIPTOADDRESS]", shipToAddress);
                    result = result.Replace("[/SOLDTOTEL]", soldToTel);
                    result = result.Replace("[/BILLTOTEL]", billToTel);
                    result = result.Replace("[/SHIPTOTEL]", shipToTel);
                    result = result.Replace("[/SOLDTOMOBILE]", soldToMobile);
                    result = result.Replace("[/BILLTOMOBILE]", billToMobile);
                    result = result.Replace("[/SHIPTOMOBILE]", shipToMobile);
                    result = result.Replace("[/SUBTOTAL]", cartTotalAmount);

                    if (Store.offerShippingService == false)
                    {
                        result = result.Replace("[/SHIPPINGMETHOD]", string.Empty);
                        try
                        {
                            result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Freight[\w\W]*?</tr>", "");
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        result = result.Replace("[/SHIPPINGMETHOD]", string.Format("<p><label>{1}: </label>{0}</p>", shippingMethod, Store.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Shipping_Method, true, language, minisite)));
                        result = result.Replace("[/FREIGHT]", freight);
                    }

                    if (Store.hasTaxCalculator == false)
                    {
                        try
                        {
                            result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""Tax[\w\W]*?</tr>", "");
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        result = result.Replace("[/TAXRATE]", taxrate);
                        result = result.Replace("[/TAX]", tax);
                    }
                    result = result.Replace("[/TOTAL]", quoteTotalAmount);
                    result = result.Replace("[/subject_tax]", Store.specialTaxMessage(quote.cartX.ShipToContact));
                    result = result.Replace("[/Comment]", comment);
                    if (_store.storeID == "AEU")
                    {
                        result = result.Replace("[/LegalForm]", string.Format("<tr><td>Legal Form:{0}</td><td>Legal Form:{1}</td><td>Legal Form:{2}</td></tr>",
                            quote.cartX.SoldToContact.LegalForm, quote.cartX.BillToContact.LegalForm, quote.cartX.ShipToContact.LegalForm));
                    }
                    else
                        result = result.Replace("[/LegalForm]", string.Empty);

                    if (quote.TotalDiscount != null && quote.TotalDiscount > 0)
                        result = result.Replace("[/DISCOUNT]", discount);
                    else
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""CartDiscount[\w\W]*?</tr>", "");
                    }
                    if (quote.DutyAndTax != null && quote.DutyAndTax > 0)
                        result = result.Replace("[/DutyAndTax]", FormatPriceWithCurrencyDecimal(quote.DutyAndTax.GetValueOrDefault(), quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate));
                    else
                    {
                        result = System.Text.RegularExpressions.Regex.Replace(result, @"<tr id=""DutyAndTax[\w\W]*?</tr>", "");
                    }

                    string InternalquoteDate = Store.getLocalTime(quote.QuoteDate.Value, quote.userX);
                    string InternalvalidDate = Store.getLocalTime(quote.QuoteExpiredDate.Value, quote.userX);

                    resultInternalDisplay = result.Replace("[/CARTITEMS]", quotedetailInternalDisplay);
                    resultInternalDisplay = resultInternalDisplay.Replace("[/colspan]", "3");
                    resultInternalDisplay = resultInternalDisplay.Replace("[/DetailModeTH]", string.Format("<th style='color: #F63'>{0}</th><th style='color: #F63'>{1}</th>", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_date,true,language,minisite)
                        , Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Available_Qty,true,language,minisite)));
                    resultInternalDisplay = resultInternalDisplay.Replace("[/QUOTEDATE]", InternalquoteDate);
                    resultInternalDisplay = resultInternalDisplay.Replace("[/ValidDate]", InternalvalidDate);

                    string CustomerquoteDate = Store.getLocalTime(quote.QuoteDate.Value, quote.userX,true);
                    string CustomervalidDate = Store.getLocalTime(quote.QuoteExpiredDate.Value, quote.userX,true);
                    
                    resultCustomerDisplay = result.Replace("[/CARTITEMS]", quotedetailCustomerDisplay);
                    resultCustomerDisplay = resultCustomerDisplay.Replace("[/colspan]", "1");
                    resultCustomerDisplay = resultCustomerDisplay.Replace("[/DetailModeTH]", "");
                    resultCustomerDisplay = resultCustomerDisplay.Replace("[/QUOTEDATE]", CustomerquoteDate);
                    resultCustomerDisplay = resultCustomerDisplay.Replace("[/ValidDate]", CustomervalidDate);
                }

                string mailFromName = String.Format("{1} {0}", _store.storeID.ToUpper(), Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore, true, language, minisite));
                string mailSubject = Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Quotation_Information, true, language, minisite);

                // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                if (testMode() == true)
                {
                    mailFromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language, minisite));
                    mailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language, minisite)) + mailSubject;
                    orderDeptEmailResponse = sendMail(createrEmail, receiverEmail, mailFromName, "(" + Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal, true, language, minisite) + ")" + mailSubject, resultInternalDisplay, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse = sendMail(receiverEmail, createrEmail, mailFromName, mailSubject, resultCustomerDisplay, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    orderDeptEmailResponse = sendMail(createrEmail, receiverEmail, mailFromName, "(" + Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal, true, language, minisite) + ")" + mailSubject, resultInternalDisplay, _store.storeID.ToUpper(), emailGroup, emailGroup);
                    customerEmailResponse = sendMail(receiverEmail, createrEmail, mailFromName, mailSubject, resultCustomerDisplay, _store.storeID.ToUpper(), "", emailGroup + ";" + createrEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "QuotationIsNull" || ex.Message == "ReceiverUserIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed at sending transferred quotation mail.", "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// This function will compose the  quotation for PDF format
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="shipment"></param>
        /// <returns></returns>
        public string getQuoteInPdfContent(Quotation quote, eStore.POCOS.Shipment shipment)
        {
            try
            {
                if (quote == null)
                    throw new Exception("Quote is null.");

                if (shipment == null)
                    throw new Exception("Shipment is null");
                string quotePDF = null;
                //string username = quote.userX.FirstName.ToUpper() + " " + quote.userX.LastName.ToUpper();
                //string firstname = quote.userX.FirstName.ToUpper();
                //string lastname = quote.userX.LastName.ToUpper();
                string quoteNo = quote.QuotationNumber;
                string quoteDate = quote.QuoteDate.ToString();
                //string quoteExpDate = quote.QuoteExpiredDate.ToString();
                string quoteExpDate = Store.getLocalTime(quote.QuoteExpiredDate.Value, quote.userX);
                string salesPerson = null;      // Need this information
                string salesPersonEmail = null;
                string salesPersonPhone = null;
                string version = quote.Version;
                string result = null;

                //Sold to information
                string soldToCompanyName = quote.cartX.SoldToContact.AttCompanyName;
                string soldToAttention = Store.getCultureFullName(quote.userX.FirstName, quote.userX.LastName);
                string soldToAddress = quote.cartX.SoldToContact.Address1 + "<br />" + quote.cartX.SoldToContact.Address2;
                string soldToState = string.IsNullOrEmpty(quote.cartX.SoldToContact.State) ? "" : quote.cartX.SoldToContact.State;
                string soldToCounty = string.IsNullOrEmpty(quote.cartX.SoldToContact.County) ? "" : quote.cartX.SoldToContact.County;
                string soldToCountry = quote.cartX.SoldToContact.Country;
                soldToAddress += ",<br />" + quote.cartX.SoldToContact.City + soldToCounty + soldToState + "&nbsp;" + quote.cartX.SoldToContact.ZipCode + "&nbsp;" + soldToCountry;
                string soldToPhone = (string.IsNullOrEmpty(quote.cartX.SoldToContact.TelExt)) ? quote.cartX.SoldToContact.TelNo : quote.cartX.SoldToContact.TelNo + "  " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext) + ": " + quote.cartX.SoldToContact.TelExt;
                string soldToFax = quote.cartX.SoldToContact.FaxNo;

                //Ship to information
                string shipToCompanyName = quote.cartX.ShipToContact.AttCompanyName;
                string shipToAttention = Store.getCultureFullName(quote.userX.FirstName, quote.userX.LastName);
                string shipToAddress = quote.cartX.ShipToContact.Address1 + "<br />" + quote.cartX.ShipToContact.Address2;
                string shipToState = string.IsNullOrEmpty(quote.cartX.ShipToContact.State) ? "" : quote.cartX.ShipToContact.State;
                string shipToCounty = string.IsNullOrEmpty(quote.cartX.ShipToContact.County) ? "" : quote.cartX.ShipToContact.County;
                string shipToCountry = quote.cartX.ShipToContact.Country;
                shipToAddress += ", <br />" + quote.cartX.ShipToContact.City + shipToCounty + shipToState + "&nbsp;" + quote.cartX.ShipToContact.ZipCode + "&nbsp;" + shipToCountry;
                string shipToPhone = (string.IsNullOrEmpty(quote.cartX.ShipToContact.TelExt)) ? quote.cartX.ShipToContact.TelNo : quote.cartX.ShipToContact.TelNo + " " + Store.Tanslation(POCOS.Store.TranslationKey.eStore_Ext) + ": " + quote.cartX.ShipToContact.TelExt;
                string shipToFax = quote.cartX.ShipToContact.FaxNo;

                string erpId = quote.userX.FederalID ?? "";    //need this information
                string shipmethod = shipment.ShippingMethod;
                string quoteDetail = null;
                quoteDetail = showCartItems(quote.cartX, Store, false);

                string taxrate = formatTaxRate(quote.TaxRate);
                string tax = FormatPriceWithDecimal(quote.Tax, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string subtotal = FormatPriceWithCurrencyDecimal(quote.cartX.TotalAmount, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                string total = FormatPriceWithCurrencyDecimal(quote.cartX.TotalAmount, quote.cartX.localCurrencyX.CurrencySign, quote.cartX.LocalCurExchangeRate);
                if (isShowStorePrice)
                    total += string.Format("<br />({0}{1})", FormatPriceWithCurrencyDecimal(quote.totalAmountX, quote.currencySign, null));

                result = Store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.QuotationInPDF);
                result = quotePDF.Replace("[/QuoteNO]", quoteNo);
                result = result.Replace("[/QuoteDate]", quoteDate);
                result = result.Replace("[/QUOTEEXPDATE]", quoteExpDate);
                result = result.Replace("[/SalesPerson]", salesPerson);
                result = result.Replace("[/SalesPersonEmail]", salesPersonEmail);
                result = result.Replace("[/SalesPersonPhone]", salesPersonPhone);
                result = result.Replace("[/Version]", version);
                result = result.Replace("[/SOLDTOCOMPANYNAME]", soldToCompanyName);
                result = result.Replace("[/SOLDTOATTENTION]", soldToAttention);
                result = result.Replace("[/SOLDTOADDRESS]", soldToAddress);
                result = result.Replace("[/SOLDTOSTATE]", soldToState);
                result = result.Replace("[/SOLDTOPHONE]", soldToPhone);
                result = result.Replace("[/SOLDTOFAX]", soldToFax);
                result = result.Replace("[/SHIPTOCOMPANYNAME]", shipToCompanyName);
                result = result.Replace("[/SHIPTOATTENTION]", shipToAttention);
                result = result.Replace("[/SHIPTOADDRESS]", shipToAddress);
                result = result.Replace("[/SHIPTOSTATE]", shipToState);
                result = result.Replace("[/SHIPTOPHONE]", shipToPhone);
                result = result.Replace("[/SHIPTOFAX]", shipToFax);
                result = result.Replace("[/ERPID]", erpId);
                result = result.Replace("[/SHIPMETHOD]", shipmethod);
                result = result.Replace("[/QUOTECONTENT]", quoteDetail);
                result = result.Replace("[/TAX]", tax);
                result = result.Replace("[/TAXRATE]", taxrate);
                result = result.Replace("[/SUBTOTAL]", subtotal);
                result = result.Replace("[/TOTAL]", total);
                result = result.Replace("[/subject_tax]", Store.specialTaxMessage(quote.cartX.ShipToContact));

                return result;
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed to generate quotation PDF page", "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// This function is used to display cart items. 
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="detailMode">detail mode is for internal sales mail display.</param>
        /// <returns>HTML table rows</returns>
        public string showCartItems(Cart cart, Store store, bool detailMode)
        {
            string cartContent = null;
            //var _cartitem = cartItems.OrderBy(cartitem => cartitem.type);

            // Show all cart items
            int i = 1;      //Standart product line number starts from 1
            //foreach (CartItem c in _cartitem)
            foreach (CartItem c in cart.cartItemsX)
            {
                string promotionMessage = c.PromotionMessage == null || string.IsNullOrEmpty(c.PromotionMessage) ? string.Empty :
                                            "<br /><span style='color:red'>" + c.PromotionMessage+"</span>";
                string discountTotal = c.DiscountAmount == null ? string.Empty :
                        string.Format("<br/><span style='color:red'>-{0}</span>", FormatPriceWithCurrencyDecimal((decimal)c.DiscountAmount, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate));

                string specialProductICO = "";
                bool isShowDeliveryTag = false;
                if (store.getSettings().ContainsKey("eStore_show_EmailFastdelivery"))
                    Boolean.TryParse(store.getSettings()["eStore_show_EmailFastdelivery"], out isShowDeliveryTag);
                if (isShowDeliveryTag && c.partX is POCOS.Product)
                {
                    if (store.isWeek2DeliveryProducts(c.partX))
                        specialProductICO = " <span style='font-weight:normal;color:red'>(2-week shipment)</span>";
                    else if (store.isFastDeliveryProducts(c.partX as POCOS.Product))
                        specialProductICO = " <span style='font-weight:normal;color:red'>(48hr Shipment)</span>";
                }

                if (c.type == Product.PRODUCTTYPE.STANDARD)
                {
                    //ATP info.
                    string availableDateTime = "-";
                    string availableQty = "-";
                    if (c.atp != null)
                    {
                        availableDateTime = Store.getLocalTime(c.atp.availableDate);
                        availableQty = c.atp.availableQty.ToString();
                    }
                    
                    string warningStyle = (c.atp.availableQty <= 0) ? " style=\"font-weight: bold; color: #FF0000; \"" : "";

                    if (detailMode == true)
                        cartContent += "<tr style=\"font-weight: bold; color=\"blue;\"\"><td " + warningStyle + "><b>" + i++ + "</b></td><td " + warningStyle + "><b style=\"white-space:nowrap;\">" + c.productNameX + (c.productNameX.Equals(c.SProductID) ? "" : " [" + c.SProductID + "]") + "</b>" + specialProductICO + "</td><td><b>" + c.Description + promotionMessage + "</b>" + string.Format("{0}", string.IsNullOrEmpty(c.CustomerMessage) ? "" : "<br /><span style='color:red'>" + c.CustomerMessage + "</span>") + "</td><td style='color: #F63' align=\"right\">" + availableDateTime + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">" + availableQty + "</span></td><td ><b>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>" + c.Qty + "</b></td><td align=\"right\" ><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";
                    else
                        cartContent += "<tr style=\"font-weight: bold; color=\"blue;\"\"><td><b>" + i++ + "</b></td><td width=\"20%\" style=\"white-space:nowrap;\" ><b>" + c.productNameX + "</b>" + specialProductICO + "</td><td ><b>" + c.Description + promotionMessage + "</b>" + string.Format("{0}", string.IsNullOrEmpty(c.CustomerMessage) ? "" : "<br /><span style='color:red'>" + c.CustomerMessage + "</span>") + "</td><td>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</td><td align=\"right\">" + c.Qty + "</td><td align=\"right\" ><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";
                    if (c.warrantyItemX != null)
                    {
                        CartItem wi = c.warrantyItemX;

                        warningStyle = "";
                        if (detailMode == true)
                            cartContent += "<tr style=\"font-weight: bold;\"><td " + warningStyle + "></td><td " + warningStyle + "><b style=\"white-space:nowrap;\">" + wi.productNameX + "</b></td><td><b>" + wi.Description + "</b></td><td style='color: #F63' align=\"right\">" + availableDateTime + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">" + availableQty + "</span></td><td ><b>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>" + wi.Qty + "</b></td><td align=\"right\" ><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td></tr>";
                        else
                            cartContent += "<tr style=\"font-weight: bold;\"><td></td><td width=\"20%\" style=\"white-space:nowrap;\" ><b>" + wi.SProductID + "</b></td><td ><b>" + wi.Description + "</b></td><td>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</td><td align=\"right\">" + wi.Qty + "</td><td align=\"right\" ><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td></tr>";
                    
                    }
                
                }
                else if (c.type == Product.PRODUCTTYPE.CTOS)
                {
                    // Show for customer
                    Boolean isSBC_CTO = c.btosX.isSBCBTOS();
                    if (detailMode == false)
                    {
                        cartContent += "<tr style=\"font-weight: bold;\"><td><b>" + i++ + "</b></td><td style=\"white-space:nowrap;\" ><b>" + c.productNameX + "</b>"+ specialProductICO + "</td><td><b>" + c.Description + promotionMessage + "</b></td><td><b>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>" + c.Qty + "</b></td><td align=\"right\"><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";
                        //foreach (BTOSConfig b in c.BTOSystem.BTOSConfigsWithoutNoneItems)
                        foreach (BTOSConfig b in c.btosX.BTOSConfigsWithoutNoneItems)
                        {
                            if (isSBC_CTO)
                            {
                                int sbcqty;
                                decimal sbcunitprice;
                                if (b.BTOSConfigDetails != null && b.BTOSConfigDetails.Count > 0)
                                {
                                    sbcqty = b.BTOSConfigDetails.FirstOrDefault().Qty;
                                    sbcunitprice = b.BTOSConfigDetails.FirstOrDefault().AdjustedPrice.GetValueOrDefault();
                                }
                                else
                                {
                                    sbcqty = 1;
                                    sbcunitprice = b.AdjustedPrice.GetValueOrDefault();
                                }
                                string categoryComponentDesc = string.Empty;
                                foreach (BTOSConfigDetail bcf in b.BTOSConfigDetails)
                                {
                                    if (string.IsNullOrEmpty(categoryComponentDesc))
                                        categoryComponentDesc = b.CategoryComponentDesc;
                                    categoryComponentDesc += "<br>"+bcf.SProductID;
                                }
                                cartContent += "<tr><td>&nbsp;</td><td class='paddingleft3' align=\"left\">" + categoryComponentDesc + "</td><td align=\"left\">"
                                    + b.OptionComponentDesc + "</td><td>" + FormatPriceWithCurrencyDecimal(sbcunitprice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate)
                                    + "</td><td align=\"right\">" + b.Qty * c.Qty * sbcqty + "</td><td align=\"right\" >"
                                    + FormatPriceWithCurrencyDecimal((b.AdjustedPrice.GetValueOrDefault() * b.Qty * c.Qty), cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td></tr>";
                            }
                            else
                                cartContent += "<tr><td>&nbsp;</td><td class='paddingleft3' align=\"left\">" + b.CategoryComponentDesc + "</td><td align=\"left\"  colspan=\"4\">" + b.OptionComponentDesc + "</td></tr>";
                        }
                    }

                    // Show for sales
                    if (detailMode == true)
                    {
                        string warningCTOSStyle = (c.atp.availableQty <= 0) ? " style=\"white-space:nowrap;font-weight: bold; color: #FF0000; \"" : "";
                        cartContent += "<tr style=\"font-weight: bold;\"><td " + warningCTOSStyle + "><b>" + i++ + "</b></td><td " + warningCTOSStyle + "><b>" + c.productNameX + "</b>"+ specialProductICO + "</td><td><b>" + c.Description + promotionMessage + "</b></td><td style='color: #F63' align=\"right\"><b>" + c.atp.availableDate + "</b></td><td style='color: #F63' align=\"right\"><b><span " + warningCTOSStyle + ">" + c.atp.availableQty + "</span></b></td><td><b>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>" + c.Qty + "</b></td><td align=\"right\"><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";
                        //foreach (BTOSConfig b in c.BTOSystem.BTOSConfigsWithoutNoneItems)
                        foreach (BTOSConfig b in c.btosX.BTOSConfigsWithoutNoneItems)
                        {
                            if (b.BTOSConfigDetails != null && b.BTOSConfigDetails.Count > 0)
                            {
                                ATP _atp = null;

                                string availableDateTime = "-";
                                string availableQty = "";
                                DateTime atpdate = DateTime.Now;

                                string warningStyle = "style=\" white-space:nowrap;\"";
                                foreach (BTOSConfigDetail bcd in b.BTOSConfigDetails)
                                {

                                    _atp = bcd.BTOSConfig.atp;
                                    if (_atp != null)
                                    {
                                        if (_atp.availableDate > atpdate)
                                            atpdate = _atp.availableDate;
                                        availableQty += (string.IsNullOrEmpty(availableQty) ? "" : "/") + _atp.availableQty.ToString();

                                        if (_atp.availableQty <= 0)
                                            warningStyle = " style=\"font-weight: bold; color: #FF0000; white-space:nowrap;\"";
                                    }
                                }
                                availableDateTime = Store.getLocalTime(atpdate);

                                //ATP info.


                                cartContent += "<tr><td>&nbsp;</td><td class='paddingleft3' align=\"left\" " + warningStyle + ">"
                                     + string.Join("<br />",
                                    (from detail in b.BTOSConfigDetails
                                     orderby detail.SProductID
                                     select detail.SProductID
                                     ).ToArray())
                                    + "</td><td align=\"left\">" + b.OptionComponentDesc + "</td><td style='color: #F63' align=\"right\">"
                                    + availableDateTime + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">" + availableQty + "</span></td><td>"
                                    + 
                                    string.Join("/",
                                    (from detail in b.BTOSConfigDetails
                                     orderby detail.SProductID
                                     select FormatPriceWithDecimal(detail.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate)).ToArray()
                                    ) + "</td><td align=\"right\">"
                                    + string.Join("/",
                                    (from detail in b.BTOSConfigDetails
                                     orderby detail.SProductID
                                     select detail.Qty * c.Qty * b.Qty).ToArray())
                                    + "</td><td align=\"right\">" + FormatPriceWithCurrencyDecimal((b.AdjustedPrice.GetValueOrDefault() * b.Qty * c.Qty), cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</td></tr>";
                            }
                        }
                    }
                }
                else if (c.type == Product.PRODUCTTYPE.BUNDLE)
                {
                    Bundle bundle = c.bundleX;
                    if (detailMode == true)
                    {
                        string availableDateTime = "-";
                        string availableQty = "-";
                        if (c.atp != null)
                        {
                            availableDateTime = Store.getLocalTime(c.atp.availableDate);
                            availableQty = c.atp.availableQty.ToString();
                        }

                        string warningStyle = (c.atp.availableQty <= 0) ? " style=\"font-weight: bold; color: #FF0000; \"" : "";
                        cartContent += "<tr style=\"font-weight: bold; color=\"blue;\"\"><td " + warningStyle + "><b>" + i++ + "</b></td><td " + warningStyle + "><b style=\"white-space:nowrap;\">" + c.productNameX + "</b>"+ specialProductICO + "</td><td><b>" + c.Description + promotionMessage + "</b></td><td style='color: #F63' align=\"right\">" + availableDateTime + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">" + availableQty + "</span></td><td ><b>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>" + c.Qty + "</b></td><td align=\"right\" ><b>" + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";

                        foreach (BundleItem item in bundle.BundleItems)
                        {
                            cartContent += "<tr style=\"font-weight: bold; color=\"blue;\"\"><td></td><td "
                                + warningStyle + "><b style=\"white-space:nowrap;\">"
                                + item.ItemSProductID + "</b></td><td><b>" + item.ItemDescription + "</b></td><td style='color: #F63' align=\"right\">"
                                + item.part.atp.availableDate.ToShortDateString() + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">"
                                + item.part.atp.availableQty + "</span></td><td ><b>" + FormatPriceWithDecimal(item.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td><td align=\"right\"><b>"
                                + item.quantity * c.Qty + "</b></td><td align=\"right\" ><b>" + FormatPriceWithDecimal((item.AdjustedPrice * item.quantity * c.Qty), cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</b></td></tr>";


                            if (item.btosX != null)
                            {
                                availableQty = "";
                                foreach (BTOSConfig b in item.btosX.BTOSConfigsWithoutNoneItems)
                                {
                                    if (b.BTOSConfigDetails != null && b.BTOSConfigDetails.Count > 0)
                                    {
                                        ATP _atp = null;

                                        
                                        DateTime atpdate = DateTime.Now;

                                          warningStyle = "style=\" white-space:nowrap;\"";
                                        foreach (BTOSConfigDetail bcd in b.BTOSConfigDetails)
                                        {

                                            _atp = bcd.BTOSConfig.atp;
                                            if (_atp != null)
                                            {
                                                if (_atp.availableDate > atpdate)
                                                    atpdate = _atp.availableDate;
                                                availableQty += (string.IsNullOrEmpty(availableQty) ? "" : "/") + _atp.availableQty.ToString();

                                                if (_atp.availableQty <= 0)
                                                    warningStyle = " style=\"font-weight: bold; color: #FF0000; white-space:nowrap;\"";
                                            }
                                        }
                                        availableDateTime = Store.getLocalTime(atpdate);

                                        //ATP info.


                                        cartContent += "<tr><td>&nbsp;</td><td align=\"left\" " + warningStyle + ">"
                                             + string.Join("<br />",
                                            (from detail in b.BTOSConfigDetails
                                             orderby detail.SProductID
                                             select detail.SProductID
                                             ).ToArray())
                                            + "</td><td align=\"left\">" + b.OptionComponentDesc + "</td><td style='color: #F63' align=\"right\">"
                                            + availableDateTime + "</td><td style='color: #F63' align=\"right\"><span " + warningStyle + ">" + availableQty + "</span></td><td>"
                                            +
                                            string.Join("/",
                                            (from detail in b.BTOSConfigDetails
                                             orderby detail.SProductID
                                             select FormatPriceWithDecimal(detail.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate)).ToArray()
                                            ) + "</td><td align=\"right\">"
                                            + string.Join("/",
                                            (from detail in b.BTOSConfigDetails
                                             orderby detail.SProductID
                                             select detail.Qty * c.Qty * b.Qty * item.Qty).ToArray())
                                            + "</td><td align=\"right\">" + FormatPriceWithDecimal((b.AdjustedPrice.GetValueOrDefault() * b.Qty * c.Qty * item.Qty), cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + "</td></tr>";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        cartContent += "<tr style=\"font-weight: bold; color=\"blue;\"\"><td><b>" + i++ + "</b></td><td width=\"20%\" style=\"white-space:nowrap;\" ><b>"
                            + c.SProductID + "</b>"+ specialProductICO + "</td><td ><b>" + c.Description + promotionMessage + "</b></td><td>" + FormatPriceWithCurrencyDecimal(c.UnitPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate)
                            + "</td><td align=\"right\">" + c.Qty + "</td><td align=\"right\" ><b>"
                            + FormatPriceWithCurrencyDecimal(c.AdjustedPrice, cart.localCurrencyX.CurrencySign, cart.LocalCurExchangeRate) + discountTotal + "</b></td></tr>";

                        int biIndex = 0;
                        foreach (BundleItem item in bundle.BundleItems)
                        {
                            biIndex++;
                            cartContent += "<tr><td align=\"center\">" + biIndex + "</td><td align=\"left\">"
                                + item .ItemSProductID+ "</td><td align=\"left\"  colspan=\"4\">" 
                                + item.ItemDescription + "</td></tr>";

                            if (item.btosX != null)
                            {
                                bool isSBC_CTO = item.btosX.isSBCBTOS();
                                foreach (BTOSConfig b in item.btosX.BTOSConfigsWithoutNoneItems)
                                {
                                    string categoryComponentDesc = string.Empty;
                                    foreach (BTOSConfigDetail bcf in b.BTOSConfigDetails)
                                    {
                                        if (string.IsNullOrEmpty(categoryComponentDesc))
                                            categoryComponentDesc = b.CategoryComponentDesc;
                                        categoryComponentDesc += "<br>" + bcf.SProductID;
                                    }
                                    cartContent += "<tr><td>&nbsp;</td><td class='paddingleft3' align=\"left\">" + categoryComponentDesc + "</td><td align=\"left\"  colspan=\"4\">" + b.OptionComponentDesc + "</td></tr>";
                                }
                            }
                        }
                    }
                }
            }

            return cartContent;
        }

        /// <summary>
        /// This function will compose the call me now email then send to order dept of store.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public EMailReponse getCallMeNowContent(Dictionary<string, string> user, Store store, string countryName = null, Language language = null,MiniSite minisite = null)
        {
            if (user == null)
                throw new Exception("UserIsNull");
            if (store == null)
                throw new Exception("StoreIsNull");

            EMailReponse response = new EMailReponse();
            try
            {
                string result = "";
                string Country = user["Country"];
                string Name = user["Name"];
                string Phone = (string.IsNullOrEmpty(user["Ext"])) ? user["Phone"] : user["Phone"] + " Ext: " + user["Ext"];
                //string extention = user["Ext"];
                string senderEMail = (string.IsNullOrEmpty(user["EMail"])) ? store.profile.OrderDeptEmail : user["EMail"];

                string StoreURL = store.getCurrStoreUrl(store.profile, minisite);
                string orderDeptEmail = store.profile.OrderDeptEmail;


                if (!String.IsNullOrWhiteSpace(countryName))
                {
                    POCOS.Address _storeAddress = store.getAddressByCountry(countryName, POCOS.Store.BusinessGroup.eP);
                    if (_storeAddress != null)
                    {
                        if (!string.IsNullOrEmpty(_storeAddress.EmailGroup))
                            orderDeptEmail += ";" + _storeAddress.EmailGroup;
                    }
                }

                result = store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.CallMeNow, language);

                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("FailToGetTemplate");
                }
                else
                {
                    result = result.Replace("[/Country]", Country);
                    result = result.Replace("[/ContactEmail]", (string.IsNullOrEmpty(user["ContactEMail"])) ? "" : user["ContactEMail"]);
                    result = result.Replace("[/Name]", Name);
                    result = result.Replace("[/Phone]", Phone);
                    result = result.Replace("[/StoreURL]", StoreURL);
                }

                string mailFromName = Name;
                string mailSubject = store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Advantech_eStore_Call_me_now_notice, true, language, minisite);

                // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                if (testMode() == true)
                {
                    mailFromName += string.Format(" {0} ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language, minisite));
                    mailSubject = string.Format("[{0}] ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language, minisite)) + mailSubject;
                    EMailReponse orderDeptEmailResponse = sendMail(testingOrderDeptEmail, senderEMail, mailFromName, mailSubject, result, _store.storeID.ToUpper());
                    orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return orderDeptEmailResponse;
                }
                else
                {
                    EMailReponse orderDeptEmailResponse = sendMail(orderDeptEmail, senderEMail, mailFromName, mailSubject, result, _store.storeID.ToUpper());
                    orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return orderDeptEmailResponse;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "UserIsNull" || ex.Message == "StoreIsNull")
                {
                    response.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                }
                else if (ex.Message == "FailToGetTemplate")
                {
                    response.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                }
                else
                {
                    response.ErrCode = EMailReponse.ErrorCode.UnknowError;
                }
                return response;

            }

        }

        public void getRateGiftContent(GiftLog giftlog, User user)
        {
            EMailReponse orderDeptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();

            try
            {
                if (giftlog == null)
                    throw new Exception("GiftLogIsNull");

                
                EMailOrder emailOrder = new EMailOrder();
                emailOrder.UserName = Store.getCultureGreetingName(user.FirstName, user.LastName);
                emailOrder.CustomerEmail = user.UserID;

                //Payment Type 和 模板内容
                emailOrder.MailContent = _store.getTemplate(Store.TEMPLATE_TYPE.RateGift);
                string mailtemp = emailOrder.MailContent;
                mailtemp = mailtemp.Replace("{$.UserName}", emailOrder.UserName);
                mailtemp = mailtemp.Replace("{$.Title}", giftlog.GiftActivityX.GiftName);
                mailtemp = mailtemp.Replace("{$.NewGid}", giftlog.LogId.ToString());
                mailtemp = mailtemp.Replace("{$.Desc}", giftlog.GiftActivityX.GiftDescription);

                emailOrder.InternalResult = mailtemp;
                emailOrder.CustomerResult = mailtemp;
                emailOrder.EmailGroup = _store.profile.OrderDeptEmail;

                emailOrder.MailFromName = "IotMart Store";
                emailOrder.MailSubject = giftlog.GiftActivityX.Advertisement.AlternateText;

                eStore.BusinessModules.Task.SendMailEvent emailEvent = new Task.SendMailEvent(emailOrder, this);
                if(emailEvent != null)
                    eStore.BusinessModules.Task.EventManager.getInstance(_store.storeID.ToUpper()).QueueCommonTask(emailEvent);

            }
            catch (Exception ex)
            {
                if (ex.Message == "FailToGetTemplate")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                else if (ex.Message == "OrderIsNull")
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                else
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;

                eStoreLoger.Error("Failed to send order email", "", "", "", ex);
                customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
            }
        }


        
        /// <summary>
        /// This method will return store tel and service time base on EA/EP
        /// </summary>
        /// <param name="store"></param>
        /// <param name="storeDivision">EA/EP</param>
        /// <returns></returns>
        public void SetContactBaseInfor(Store store, string storeDivision, ContactBaseInfor bsinfor)
        {
            string telAndServiceTime = null;
            string eAtel = null;
            string ePtel = null;
            string eFax = null;
            string division = storeDivision.ToUpper();

            try
            {
                foreach (StoreAddress addr in store.profile.StoreAddresses)
                {
                    if (addr.Division == "EA")
                        eAtel = addr.Address.Tel + " " + addr.Address.ServiceTime;
                    else if (addr.Division == "EP")
                        ePtel = addr.Address.Tel + " " + addr.Address.ServiceTime;
                    eFax = addr.Address.Fax;
                }

                switch (division)
                {
                    case "EA":
                        telAndServiceTime = eAtel;
                        break;

                    case "EP":
                        telAndServiceTime = ePtel;
                        break;

                    default:
                        telAndServiceTime = ePtel;
                        break;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Failed to identify eA or eP telphone number.", "", "", "", ex);
            }

            bsinfor.OurTel = telAndServiceTime;
            bsinfor.OurFax = eFax;
        }


        private string FormatPriceWithCurrencyDecimal(decimal? price, string currency, decimal? localCurExchangeRate, string format = "C2")
        {
            if (price == null || price == 0)
                return "N/A";
            decimal localPrice = price.GetValueOrDefault();
            string localCurrency = Store.profile.defaultCurrency.CurrencySign;
            if (!string.IsNullOrEmpty(currency) && localCurExchangeRate != null)
            {
                localPrice = price.GetValueOrDefault() * localCurExchangeRate.GetValueOrDefault();
                localCurrency = currency;
            }
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(Store.profile.CultureCode);
            culture.NumberFormat.CurrencySymbol = localCurrency;
            return localPrice.ToString(format, culture);
        }

        private string FormatPriceWithDecimal(decimal? price, string currency, decimal? localCurExchangeRate)
        {
            if (price == null || price == 0)
                return "N/A";
            return FormatPriceWithCurrencyDecimal(price.GetValueOrDefault(), currency,localCurExchangeRate);
        }

        private string FormartFreight(decimal? price,string currency, decimal? localCurExchangeRate)
        {
            string rlt = "C2";
            if (Utilities.Converter.getCartPriceRoundingUnit(Store.storeID) < 1)
                rlt = "C2";
            else
                rlt = "C0";
            if (price == null || price == 0)
                return Store.getZeroFreightDispayString();
            else
                return FormatPriceWithCurrencyDecimal(price.GetValueOrDefault(), currency, localCurExchangeRate, rlt);
        }

        private string FormartTax(decimal? price, string currency, decimal? localCurExchangeRate)
        {
            string rlt = "C2";
            if (Utilities.Converter.getCartPriceRoundingUnit(Store.storeID) < 1)
                rlt = "C2";
            else
                rlt = "C0";
            if (price == null || price == 0)
                return Store.getZeroTaxDispayString();
            else
                return FormatPriceWithCurrencyDecimal(price.GetValueOrDefault(),currency,localCurExchangeRate, rlt);
        }

        private string formatTaxRate(decimal? value)
        {
            return formatTaxRate(value.GetValueOrDefault());
        }

        private string formatTaxRate(decimal value)
        {
            if (value == 0)
                return "N/A";
            return string.Format("{0}%", string.Format("{0:n2}", value));
        }

        /// <summary>
        /// format mail contact address string
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        private string formatContactAddress(CartContact contact)
        {
            return _store.profile.formatContactAddress(contact);
        }

        /// <summary>
        /// This method is used to send Email and it return mail response. If programmer needs to save the Email, mail.SaveEmail should be set as true.
        /// </summary>
        /// <param name="mailToAddress"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailFromName"></param>
        /// <param name="subject"></param>
        /// <param name="mailBody"></param>
        /// <returns></returns>
        public EMailReponse sendMail(string mailToAddress, string mailFrom, string mailFromName, string subject, string mailBody, string storeId, String mailCC = "", String mailBcc = "", List<string> Attached = null, List<System.Net.Mail.LinkedResource> Resources = null)
        {
            try
            {
                if (!testMode())
                {
                    if (IsneedBcc)
                    {
                        if (string.IsNullOrEmpty(mailBcc))
                        {
                            mailBcc = eStoreAdminGroup + ";" + _store.profile.OrderDeptEmail;
                        }
                        else
                        {
                            //如果包含就不添加
                            if (!mailBcc.Contains(eStoreAdminGroup))
                                mailBcc = eStoreAdminGroup + ";" + mailBcc;
                        }
                    }
                    else
                        mailBcc = "";
                }
                EMail mail = new EMail(mailToAddress, mailFrom, mailFromName, subject, mailBody, storeId, mailCC, mailBcc);
                if (Resources != null)
                    mail.CustomizedImage = Resources;
                mail.Attached = Attached;
                mail.SaveEmail = true;
                EMailReponse response = mail.sendMailNow();
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// set email order infor form address
        /// </summary>
        /// <param name="_cart"></param>
        /// <param name="_minisite"></param>
        /// <param name="emailOrder"></param>
        protected void setEmailOrderInfor(POCOS.Cart _cart, POCOS.MiniSite _minisite, ref EMailOrder emailOrder)
        {
            if (_minisite != null && _minisite.Settings.Keys.Contains("EmailGroup")) // now only ushop minisite use site Parameter email group
            {
                emailOrder.StoreEmail.ContactMail = _minisite.Settings["EmailGroup"];
                emailOrder.EmailGroup = emailOrder.StoreEmail.ContactMail;
            }
            else
            {
                emailOrder.EmailGroup = emailOrder.StoreEmail.ContactMail;
                POCOS.Address _storeAddress = Store.getAddressByCountry(_cart.BillToContact.Country, _cart.businessGroup);
                if (_storeAddress != null)
                {
                    emailOrder.StoreEmail.OurTel = string.Format("{0} {1}", _storeAddress.Tel, _storeAddress.ServiceTime);
                    emailOrder.StoreEmail.OurAddress = _storeAddress.fullAddress;
                    if (!emailOrder.StoreEmail.ContactMail.Split(';').Contains(_storeAddress.EmailGroup))
                    {
                        if (!(_storeAddress.Exclusive.HasValue && _storeAddress.Exclusive.Value))
                            emailOrder.EmailGroup += (";" + _storeAddress.EmailGroup);
                        else
                            emailOrder.EmailGroup = _storeAddress.EmailGroup;
                    }
                }
            }
        }

        public EMailReponse sendWidgetRequestEmail(List<VWidgetModel> ls, string fromName, string subject, Store store, Language language = null, MiniSite minisite = null, string mailto = "")
        {

            if (!ls.Any() || ls.FirstOrDefault(c=>c.Key.Equals("TempId",StringComparison.OrdinalIgnoreCase)) == null) 
                return new EMailReponse { ErrCode = EMailReponse.ErrorCode.EmptyMailContent };
            else
            {
                if (!ls.Any())
                    throw new Exception("EmailContextIsNull");
                if (store == null)
                    throw new Exception("StoreIsNull");

                EMailReponse response = new EMailReponse();
                try
                {
                    string result = "";
                    StringBuilder sbls = new StringBuilder();
                    var emodel = ls.FirstOrDefault(c => c.Key.Equals("Email", StringComparison.OrdinalIgnoreCase));
                    string senderEMail = (emodel == null ? "" : emodel.Value);
                    string StoreURL = store.getCurrStoreUrl(store.profile, minisite);
                    string DeptEmail = mailto;
                    if (string.IsNullOrEmpty(DeptEmail))
                    {
                        DeptEmail = store.profile.OrderDeptEmail;
                        if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup"))
                            DeptEmail = minisite.Settings["EmailGroup"];
                    }
                    if (!esUtilities.StringUtility.CheckEmail(senderEMail))
                        senderEMail = DeptEmail.Split(';').FirstOrDefault();

                    result = store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.WidgetRequest, language, minisite);

                    if (string.IsNullOrEmpty(result))
                    {
                        throw new Exception("FailToGetTemplate");
                    }
                    else
                    {
                        foreach (var t in ls)
                        {
                            if (t.Key.Equals("TempId", StringComparison.OrdinalIgnoreCase))
                                continue;
                            else if (t.Key.Equals("title", StringComparison.OrdinalIgnoreCase))
                            {
                                var titlemodel = ls.FirstOrDefault(c => c.Key.Equals("title", StringComparison.OrdinalIgnoreCase));
                                result = result.Replace("[/Title]", (titlemodel == null ? "None" : titlemodel.Value));
                            }
                            else
                                sbls.AppendFormat("<li><span class=\"bolder\"><label>{0}</label></span><br />{1}</li>", t.Title, t.Value);
                        }                            
                        result = result.Replace("[/Context]", sbls.ToString());
                    }

                    string mailFromName = fromName;
                    string mailSubject = subject;

                    // Test mode will add prefix "eStore 3.0 Testing" in subject, and the order department email  will be replace with eStore team member's email.
                    if (testMode() == true)
                    {
                        mailFromName += string.Format(" {0} ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language, minisite));
                        mailSubject = string.Format("[{0}] ", store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language, minisite)) + mailSubject;
                        EMailReponse orderDeptEmailResponse = sendMail(testingOrderDeptEmail, senderEMail, mailFromName, mailSubject, result, _store.storeID.ToUpper());
                        //orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                        return orderDeptEmailResponse;
                    }
                    else
                    {
                        EMailReponse orderDeptEmailResponse = sendMail(DeptEmail, senderEMail, mailFromName, mailSubject, result, _store.storeID.ToUpper());
                        //orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                        return orderDeptEmailResponse;
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message == "UserIsNull" || ex.Message == "StoreIsNull")
                    {
                        response.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                    }
                    else if (ex.Message == "FailToGetTemplate")
                    {
                        response.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                    }
                    else
                    {
                        response.ErrCode = EMailReponse.ErrorCode.UnknowError;
                    }
                    return response;

                }
            }

        }

        /// <summary>
        /// send forgot password mail
        /// </summary>
        /// <param name="user">estore user</param>
        /// <param name="subject"></param>
        /// <param name="store"></param>
        /// <param name="language"></param>
        /// <param name="minisite"></param>
        /// <returns></returns>
        public EMailReponse SendForgotPassWordEmail(POCOS.User user, string subject, Store store, Language language = null, MiniSite minisite = null)
        {
            EMailReponse response = new EMailReponse();
            try
            {
                string senderEMail = store.profile.OrderDeptEmail;
                if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup"))
                    senderEMail = minisite.Settings["EmailGroup"];
                senderEMail = senderEMail.Split(';').LastOrDefault();
                string fromName = senderEMail.Split('@').FirstOrDefault();

                string result = store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.ForgotPassWord, language, minisite);

                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("FailToGetTemplate");
                }
                else
                {
                    result = result.Replace("[/UserName]",store.getCultureFullName(user.FirstName, user.LastName));
                    result = result.Replace("[/ForgotUrl]", string.Format("{0}/Account/ForgetPassword-Update.aspx?email={1}&tempid={2}"
                        , esUtilities.CommonHelper.GetStoreLocation()
                        , user.UserID
                        , user.FollowUpComments));
                }
                

                EMailReponse orderDeptEmailResponse = sendMail(user.UserID, senderEMail, fromName, subject, result, _store.storeID.ToUpper());
                //orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                return orderDeptEmailResponse;
            }
            catch (Exception ex)
            {
                if (ex.Message == "UserIsNull" || ex.Message == "StoreIsNull")
                {
                    response.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                }
                else if (ex.Message == "FailToGetTemplate")
                {
                    response.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                }
                else
                {
                    response.ErrCode = EMailReponse.ErrorCode.UnknowError;
                }
                return response;
            }
        }

        public EMailReponse SendSubscribeUsEmail(string userName, string userEmail, string subject,string additionalDetail, Store store, Language language = null, MiniSite minisite = null)
        {
            EMailReponse deptEmailResponse = new EMailReponse();
            EMailReponse customerEmailResponse = new EMailReponse();

            try
            {
                var emailGrouop = store.profile.OrderDeptEmail;
                //string senderEMail = store.profile.OrderDeptEmail;
                if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup"))
                    emailGrouop = minisite.Settings["EmailGroup"];
                //senderEMail = senderEMail.Split(';').LastOrDefault();

                string fromName = emailGrouop.Split('@').FirstOrDefault();

                string mailBody = store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.SubscribeUs, language, minisite);



                if (string.IsNullOrEmpty(mailBody))
                {
                    throw new Exception("FailToGetTemplate");
                }
                //else
                //{
                //    result = result.Replace("[/UserName]", store.getCultureFullName(user.FirstName, user.LastName));
                //    result = result.Replace("[/StoreLogo]", _store.profile.getStringSetting("eStore_StoreLogo"));
                //    result = result.Replace("[/STOREURL]", Store.getCurrStoreUrl(_store.profile, minisite));
                //    result = result.Replace("[/USERID]", user.UserID);
                //}
                string pattern = @"\bSubscribeDetail\b";
                string mailBodyWithBAA = "";
                if (additionalDetail != null)
                    mailBodyWithBAA = Regex.Replace(mailBody, pattern, additionalDetail);

                mailBody = Regex.Replace(mailBody, pattern, "");

                var mailSubject = subject;
                if (testMode() == true)
                {
                    mailSubject = string.Format("[{0}] ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_eStore_3_0_TESTING, true, language, minisite)) + subject;
                    fromName += string.Format(" {0} ", Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Tester, true, language, minisite));

                    deptEmailResponse = sendMail(testingOrderDeptEmail, userEmail, fromName, mailSubject, mailBodyWithBAA, _store.storeID.ToUpper());

                    customerEmailResponse = sendMail(userEmail, emailGrouop.Split(';')[0], fromName, mailSubject, mailBody, _store.storeID.ToUpper(), "", testingOrderDeptEmail);
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }
                else
                {
                    deptEmailResponse = sendMail(emailGrouop, userEmail, fromName, "(" + Store.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_internal, true, language, minisite) + ")" + mailSubject, mailBodyWithBAA, _store.storeID.ToUpper(), "", emailGrouop);

                    //customerEmailResponse = sendMail(userEmail, emailGrouop.Split(';')[0], fromName, mailSubject, mailBody, _store.storeID.ToUpper(), "", emailGrouop);
                    customerEmailResponse = sendMail(userEmail, emailGrouop.Split(';')[0], fromName, mailSubject, mailBody, _store.storeID.ToUpper(), "", "");
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                    return customerEmailResponse;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "UserIsNull" || ex.Message == "StoreIsNull")
                {
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                }
                else if (ex.Message == "FailToGetTemplate")
                {
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                }
                else
                {
                    customerEmailResponse.ErrCode = EMailReponse.ErrorCode.UnknowError;
                }
                return customerEmailResponse;
            }
        }

        public EMailReponse SendRegisterEmail(POCOS.User user, string subject, Store store, Language language = null, MiniSite minisite = null)
        {
            EMailReponse response = new EMailReponse();
            try
            {
                string senderEMail = store.profile.OrderDeptEmail;
                if (minisite != null && minisite.Settings.Keys.Contains("EmailGroup"))
                    senderEMail = minisite.Settings["EmailGroup"];
                senderEMail = senderEMail.Split(';').LastOrDefault();
                string fromName = senderEMail.Split('@').FirstOrDefault();

                string result = store.getTemplate(BusinessModules.Store.TEMPLATE_TYPE.Register, language, minisite);

                if (string.IsNullOrEmpty(result))
                {
                    throw new Exception("FailToGetTemplate");
                }
                else
                {
                    result = result.Replace("[/UserName]", store.getCultureFullName(user.FirstName, user.LastName));
                    result = result.Replace("[/StoreLogo]", _store.profile.getStringSetting("eStore_StoreLogo"));
                    result = result.Replace("[/STOREURL]", Store.getCurrStoreUrl(_store.profile, minisite));
                    result = result.Replace("[/USERID]", user.UserID);
                }


                EMailReponse orderDeptEmailResponse = sendMail(user.UserID, senderEMail, fromName, subject, result, _store.storeID.ToUpper());
                //orderDeptEmailResponse.ErrCode = EMailReponse.ErrorCode.NoError;
                return orderDeptEmailResponse;
            }
            catch (Exception ex)
            {
                if (ex.Message == "UserIsNull" || ex.Message == "StoreIsNull")
                {
                    response.ErrCode = EMailReponse.ErrorCode.CalledFunctionException;
                }
                else if (ex.Message == "FailToGetTemplate")
                {
                    response.ErrCode = EMailReponse.ErrorCode.FailToGetEMailTemplate;
                }
                else
                {
                    response.ErrCode = EMailReponse.ErrorCode.UnknowError;
                }
                return response;
            }
        }

        #endregion

    }

    public class EMailOrder
    {
        //用户名
        public string UserName { get; set; }

        //收件人
        public string CustomerEmail { get; set; }

        public string EmailGroup { get; set; }

        private ContactBaseInfor storeEmail = null;
        public ContactBaseInfor StoreEmail
        {
            get
            {
                if (storeEmail == null)
                    storeEmail = new ContactBaseInfor();
                return storeEmail;
            }
        }

        public string MailFromName { get; set; }

        public string MailSubject { get; set; }

        //支付类型
        public string PaymentTypeContent { get; set; }

        //读取的模板
        public string MailContent { get; set; }

        //邮件内容
        public string InternalResult { get; set; }
        
        //邮件内容
        public string CustomerResult { get; set; }
    }

    public class VWidgetModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
    }

    public class ContactBaseInfor
    {
        private string ourTel;
        public string OurTel
        {
            get { return ourTel; }
            set { ourTel = value; }
        }

        private string ourFax;
        public string OurFax
        {
            get { return ourFax; }
            set { ourFax = value; }
        }


        private string ourAddress;

        public string OurAddress
        {
            get { return ourAddress; }
            set { ourAddress = value; }
        }

        private string contactMail;

        public string ContactMail
        {
            get { return contactMail; }
            set { contactMail = value; }
        }

        
    }
}
