using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.BusinessModules;

namespace eStore.UI.Modules.V4
{
    public partial class MyQuote : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private bool hasvalidpart = false;
        
        public Models.Account Account
        {
            set
            {
                if (value.Quotes != null && value.Quotes.Count > 0)
                {
                    this.rpMyQuote.Visible = true;
                    this.rpMyQuote.DataSource = value.Quotes;
                    this.rpMyQuote.DataBind();
                }
                else
                    this.rpMyQuote.Visible = false;

                if (value.tQuotes != null && value.tQuotes.Count > 0)
                {
                    this.rpMyTrQuote.Visible = true;
                    this.rpMyTrQuote.DataSource = value.tQuotes;
                    this.rpMyTrQuote.DataBind();
                }
                else
                    this.rpMyTrQuote.Visible = false;
            }
        }

        public POCOS.Equotation.Quote eQuote
        {
            set
            {
                this.rpEqQuote.DataSource = this.GetEquotationQuote(value);
                this.rpEqQuote.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ddl_period.Items.Add(new ListItem(Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select), "0"));
                this.ddl_period.Items.Add(new ListItem("1-3 month", "1"));
                this.ddl_period.Items.Add(new ListItem("4-6 month", "2"));
                this.ddl_period.SelectedIndex = 0;
                this.lb_searchQuote.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Search);
                this.revQuotation.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Invalid_Quotation_Number);
            }
        }

        protected void lb_searchQuote_Click(object sender, EventArgs e)
        {
            this.Account = ((new APIControllers.AccountController())
                    .GetAccountQuoteBySearch(tb_searchQuoteNo.Text.Trim(), ddl_period.SelectedValue));
        }

        public void rpMyQuote_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Presentation.eStoreControls.Repeater rp = (Presentation.eStoreControls.Repeater)e.Item.FindControl("rpRelatedOrders");
                Models.Quote quote = e.Item.DataItem as Models.Quote;
                rp.DataSource = quote.Orders;
                rp.DataBind();
            }
        }

        public void rpMyQuote_OnItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Review")
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == e.CommandArgument.ToString()
                                                     select Quotation).FirstOrDefault();

                if (currentdQuotation != null)
                {
                    Presentation.eStoreContext.Current.Quotation = currentdQuotation;
                    switch (Presentation.eStoreContext.Current.Quotation.statusX)
                    {
                        case POCOS.Quotation.QStatus.Open:
                        case POCOS.Quotation.QStatus.NeedTaxIDReview:
                        case POCOS.Quotation.QStatus.NeedFreightReview:
                        case POCOS.Quotation.QStatus.Unfinished:
                            Response.Redirect("~/Quotation/quote.aspx");
                            break;
                        case POCOS.Quotation.QStatus.Confirmed:
                        case POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview:
                        case POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview:
                        case POCOS.Quotation.QStatus.Expired:

                            Response.Redirect("~/Quotation/confirm.aspx");
                            break;
                        default:
                            //add error msg
                            break;
                    }
                }
            }

            if (e.CommandName == "Revise")
            {
                POCOS.Quotation currentdQuotation = (from Quotation in Presentation.eStoreContext.Current.User.actingUser.quotations
                                                     where Quotation.QuotationNumber == e.CommandArgument.ToString()
                                                     select Quotation).FirstOrDefault();

                if (currentdQuotation != null)
                {
                    Presentation.eStoreContext.Current.Quotation = currentdQuotation.revise();
                    if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedTaxIDReview)
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedTaxIDReview;
                    else if (Presentation.eStoreContext.Current.Quotation.statusX == POCOS.Quotation.QStatus.ConfirmedbutNeedFreightReview)
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.NeedFreightReview;
                    else
                        Presentation.eStoreContext.Current.Quotation.statusX = POCOS.Quotation.QStatus.Open;
                    Response.Redirect("~/Quotation/Quote.aspx");
                }
            }

            if (e.CommandName == "Equotation")
            {
                string quoteID = e.CommandArgument.ToString();
                POCOS.DAL.EquotationHelper helper = new POCOS.DAL.EquotationHelper();
                POCOS.Equotation.Quote quote = helper.GetQuotationAllByQuoteID(quoteID);

                if (quote != null && quote.QuoteItems != null && quote.QuotePartners != null)
                {
                    POCOS.Quotation quotation = Presentation.eStoreContext.Current.User.actingUser.createQuote;

                    //Add product
                    foreach (POCOS.Equotation.QuoteItem item in quote.QuoteItems)
                    {
                        int line_no = 0;
                        int.TryParse(item.line_No.Value.ToString(), out line_no);
                        if (line_no < 100)
                        {
                            //Standard product
                            Add2Cart(quotation.cartX, item.partNo, item.qty.Value, item.newUnitPrice.Value);
                        }
                        else
                        {
                            //CTOS product
                            if (line_no % 100 == 0)
                            {
                                //Use parent item to get child item
                                var children = (from q in quote.QuoteItems
                                                where q.qty != null
                                                && q.HigherLevel != null
                                                && q.HigherLevel.Value == line_no
                                                select q).ToList();

                                int parent_qty = item.qty.HasValue ? item.qty.Value : 0;
                                //int.TryParse(item.DisplayQty, out parent_qty); // ATW use Display qty for CTOS
                                decimal parent_price = item.newUnitPrice.HasValue ? item.newUnitPrice.Value : 0m;
                                //decimal.TryParse(item.DisplayUnitPrice, out parent_price); // ATW use DisplayUnitPrice for CTOS
                                List<BundleItem> bundle = new List<BundleItem>();

                                if (parent_qty > 0)
                                {
                                    foreach (var child in children)
                                    {
                                        //POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(child.partNo); 
                                        Part part = getProduct(child.partNo);
                                        if (part != null)
                                        {
                                            BundleItem bi = new BundleItem(part, parent_qty);
                                            //bi.AdjustedPrice = child.newUnitPrice;  //ATW 不用更新子料號
                                            bi.peripheralCategoryName = child.category; //讓 CTOS category name
                                            bundle.Add(bi);
                                        }
                                        else
                                        {
                                            Presentation.eStoreContext.Current.AddStoreErrorCode(child.partNo, null, true); //同步後還是找不到
                                        }
                                    }
                                }

                                if (bundle.Count > 0)
                                {
                                    POCOS.Product BundleProduct = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO");

                                    Product_Ctos bundleCTOS = null;
                                    if (BundleProduct is Product_Ctos)
                                        bundleCTOS = (Product_Ctos)BundleProduct;
                                    BTOSystem btos = bundleCTOS.getDefaultBTOS();

                                    btos.clear();

                                    btos.addNoneCTOSBundle(bundle, 1);

                                    btos.BTONo = item.partNo;

                                    CartItem ci = quotation.cartX.addItem(bundleCTOS, parent_qty, btos, 0, null, false, 0, true);
                                    ci.updateUnitPrice(parent_price); // 強迫更新
                                    ci.updateQty(parent_qty); //強迫更新
                                    quotation.cartX.updateTotal();
                                    hasvalidpart = true;
                                }
                            }
                        }

                    }

                    foreach (POCOS.Equotation.QuotePartner partner in quote.QuotePartners)
                    {
                        DateTime dt = DateTime.Now;
                        switch (partner.Type)
                        {
                            case "SOLDTO":
                                quotation.cartX.setSoldTo(this.getCartContact(partner, dt));
                                break;
                            case "S":
                                quotation.cartX.setShipTo(this.getCartContact(partner, dt));
                                break;
                            case "B":
                                quotation.cartX.setBillTo(this.getCartContact(partner, dt));
                                break;
                            default:
                                break;
                        }
                    }

                    if (hasvalidpart)
                    {
                        if (Presentation.eStoreContext.Current.User.actingUser.mainContact != null)
                        {
                            if (quotation.cartX.ShipToContact == null)
                                if (Presentation.eStoreContext.Current.User.actingUser.mainContact != null
                                    && Presentation.eStoreContext.Current.Store.isValidatedShiptoAddress(Presentation.eStoreContext.Current.User.actingUser.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                                    quotation.cartX.setShipTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                            if (quotation.cartX.SoldToContact == null)
                                quotation.cartX.setSoldTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                            if (quotation.cartX.BillToContact == null
                                && Presentation.eStoreContext.Current.Store.isValidatedBilltoAddress(Presentation.eStoreContext.Current.User.actingUser.mainContact.countryCodeX, Presentation.eStoreContext.Current.User))
                                quotation.cartX.setBillTo(Presentation.eStoreContext.Current.User.actingUser.mainContact);
                        }

                        //Freight
                        if (Presentation.eStoreContext.Current.Store.offerShippingService)
                        {
                            List<ShippingMethod> sms;
                            sms = null;
                            try
                            {
                                sms = Presentation.eStoreContext.Current.Store.getAvailableShippingMethods(quotation.cartX);
                            }
                            catch (Exception ex)
                            {
                                sms = null;
                                Utilities.eStoreLoger.Error("Can not get shipping", Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.getUserIP(), Presentation.eStoreContext.Current.Store.storeID, ex);
                            }
                            if (sms != null && sms.Count > 0)
                            {
                                ShippingMethod sm = sms.FirstOrDefault();
                                if (sm.Error != null)
                                {
                                    Presentation.eStoreContext.Current.AddStoreErrorCode(sm.Error.Code.ToString(), null, true);
                                    return;
                                }
                                else
                                {
                                    quotation.ShippingMethod = sm.ShippingMethodDescription;
                                    quotation.Freight = (decimal)sm.ShippingCostWithPublishedRate;
                                    quotation.FreightDiscount = (decimal)sm.Discount;
                                }
                            }
                        }
                        else
                        {
                            quotation.ShipmentTerm = string.Empty;
                            quotation.ShippingMethod = string.Empty;
                            quotation.Freight = 0;
                            quotation.FreightDiscount = 0;
                        }

                        //Tax
                        quotation.Tax = quote.Tax == null ? 0m : Math.Round(quotation.totalAmountX * quote.Tax.Value, MidpointRounding.AwayFromZero);
                        quotation.updateTotalAmount();
                        quotation.statusX = POCOS.Quotation.QStatus.Confirmed;
                        quotation.QuotationNumber = quote.QuoteNo;
                        //quotation.QuotationNumber = quote.CustomID; //ATW use CustomID as Quotation Number
                        quotation.Source = POCOS.Quotation.QuoteSource.eQuotation;
                        Presentation.eStoreContext.Current.Quotation = quotation;

                        if (Presentation.eStoreContext.Current.StoreErrorCode.Count > 0)
                            Server.Transfer("~/Quotation/confirm.aspx");
                        else
                            this.Response.Redirect("~/Quotation/confirm.aspx");
                    }
                }
                return;
            }

        }

        private void Add2Cart(POCOS.Cart cart, string productID, int qty, decimal price)
        {
            POCOS.Part _product = getProduct(productID);
            if (_product != null && _product.isOrderable(true) && !_product.isOS() && !_product.isTOrPParts() && _product.getListingPrice().value > 0)
            {
                POCOS.CartItem ci = cart.addItem(_product, qty);

                //update list price
                ci.updateUnitPrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(price, eStore.Presentation.eStoreContext.Current.CurrentCurrency));
                if (ci.type == POCOS.Product.PRODUCTTYPE.CTOS && ci.BTOSystem != null)
                    ci.BTOSystem.updatePrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(price, eStore.Presentation.eStoreContext.Current.CurrentCurrency));
                else if (ci.type == POCOS.Product.PRODUCTTYPE.BUNDLE && ci.bundleX != null)
                    ci.bundleX.updatePrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(price, eStore.Presentation.eStoreContext.Current.CurrentCurrency));

                ci.updateQty(qty);
                ci.PromotionMessage = string.Empty;
                ci.PromotionStrategy = null;
                cart.updateItem(ci);
                hasvalidpart = true;
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode(productID, null, true);
            }
        }
        
        private Part getProduct(string productID)
        {
            string ProductId = productID.Trim();

            POCOS.Part part = null;
            if (string.IsNullOrEmpty(ProductId))
                return part;
            try
            {
                part = Presentation.eStoreContext.Current.Store.getPart(ProductId);
                if (part == null)
                    part = Presentation.eStoreContext.Current.Store.addVendorPartToPart(ProductId);
            }
            catch (Exception)
            {
            }
            //POCOS.Product _product = part as POCOS.Product;
            return part;
        }
        
        private CartContact getCartContact(POCOS.Equotation.QuotePartner partner, DateTime dt)
        {
            CartContact cc = new CartContact();
            cc.UserID = Presentation.eStoreContext.Current.User.UserID;
            cc.AddressID = partner.ERPID;
            cc.FirstName = partner.FirstName; //From Siebel
            cc.LastName = partner.LasteName; //From Siebel
            cc.AttCompanyName = partner.Name;
            cc.FaxNo = partner.Fax;
            cc.TelNo = partner.Tel;
            cc.TelExt = string.Empty;
            cc.Mobile = partner.Mobile;
            cc.Address1 = partner.Address;
            cc.Address2 = string.Empty;
            cc.City = partner.City;
            cc.State = partner.State;
            cc.Country = partner.Country; // ex. eStore : Taiwan,  eQ: TW
            cc.County = partner.Country; // ex. eStore : Taiwan,   eQ: TW
            cc.CountryCode = partner.Country;
            cc.ZipCode = partner.ZipCode;
            cc.LastUpdateTime = dt;
            cc.UpdatedBy = Presentation.eStoreContext.Current.User.UserID;
            cc.CreatedDate = dt;
            cc.CreatedBy = Presentation.eStoreContext.Current.User.UserID;
            cc.LegalForm = string.Empty;
            cc.VATNumbe = string.Empty;
            cc.save();
            return cc;
        }
        
        private List<MyEquote> GetEquotationQuote(POCOS.Equotation.Quote quote)
        {
            if (quote != null)
                return (new List<POCOS.Equotation.Quote>() { quote }).Select(eq => new MyEquote
                {
                    QuotationID = eq.QuoteID,
                    QuoteNo = eq.QuoteNo,
                    CustomID = eq.CustomID,
                    SubTotal = Convert.ToDecimal(eq.TotalAmount),
                    currencySign = eq.Currency,
                    Org = eq.Org,
                    StoreID = eq.Org,
                    Status = POCOS.Quotation.QStatus.Confirmed,
                    QuoteDate = eq.CreatedDate,
                    QuoteExpiredDate = eq.ExpiredDate,
                    Source = POCOS.Quotation.QuoteSource.eQuotation,
                    ShipTo = eq.AttentionEmail,
                    QuoteAction = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Quotation_Revise)
                }).ToList();
            else
                return null;
        }

    }

    public class MyEquote
    {
        public string QuotationID { get; set; }
        public string QuoteNo { get; set; }
        public string CustomID { get; set; }
        public decimal SubTotal { get; set; }
        public string ShipTo { get; set; }
        public string currencySign { get; set; }
        public string Org { get; set; }
        public string StoreID { get; set; }
        public POCOS.Quotation.QStatus Status { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteExpiredDate { get; set; }
        public POCOS.Quotation.QuoteSource Source { get; set; }
        public string QuoteAction { get; set; }
    }
}