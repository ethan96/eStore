using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.BusinessModules;
using eStore.BusinessModules.SSO.Advantech;

namespace eStore.UI
{
    public partial class equotation : Presentation.eStoreBaseControls.eStoreBasePage
    {
        private bool hasvalidpart = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["quoteID"] != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.Store.profile.getStringSetting("CanGeteQuotationQuote")))
                {
                    //先檢查有沒有登入
                    if (Presentation.eStoreContext.Current.User == null)
                    {
                        popLoginDialog(null);
                    }

                    string quoteID = Request["quoteID"].ToString();
                    POCOS.Equotation.ForeStore ws = new POCOS.Equotation.ForeStore();
                    POCOS.Equotation.Quote quote = ws.GetQuotationMasterByQuoteID(quoteID);
                    ws.Dispose();

                    if (quote != null && User.Identity.Name.Equals(quote.AttentionEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        BindDefaultQuote(quote);
                    }
                    else
                        Response.Redirect("~");
                }
                else
                    Response.Redirect("~");
            }
        }

        protected void gvmyquotation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "FromeQuotation") //From eQuotation
            {
                string quoteID = e.CommandArgument.ToString();
                POCOS.Equotation.ForeStore ws = new POCOS.Equotation.ForeStore();
                POCOS.Equotation.Quote quote = ws.GetQuotationTotalByQuoteID(quoteID);
                ws.Dispose();

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
                            // Standard product
                            if (Presentation.eStoreContext.Current.Store.isValidOrderbyPN(item.partNo) == false)//先檢查可不可賣
                            {
                                Presentation.eStoreContext.Current.AddStoreErrorCode(item.partNo); //有不可賣的 先存到 error code
                                continue;
                            }
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

                                int parent_qty = 0;
                                int.TryParse(item.DisplayQty, out parent_qty); // ATW use Display qty for CTOS
                                decimal parent_price = 0m;
                                decimal.TryParse(item.DisplayUnitPrice, out parent_price);
                                List<BundleItem> bundle = new List<BundleItem>();

                                if (parent_qty > 0)
                                {
                                    foreach (var child in children)
                                    {
                                        //POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(child.partNo); 
                                        POCOS.Part part = getProduct(child.partNo);
                                        if (part != null)
                                        {
                                            BundleItem bi = new BundleItem(part, parent_qty);
                                            //bi.AdjustedPrice = child.newUnitPrice;  //ATW 不用更新子料號
                                            bi.peripheralCategoryName = child.category; //讓 CTOS category name
                                            bundle.Add(bi);
                                        }
                                        else
                                        {
                                            Presentation.eStoreContext.Current.AddStoreErrorCode(child.partNo); //同步後還是找不到
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
                                POCOS.CartContact soldto = new POCOS.CartContact();
                                soldto.UserID = User.Identity.Name;
                                soldto.AddressID = partner.ERPID;
                                soldto.FirstName = partner.FirstName; //From Siebel
                                soldto.LastName = partner.LasteName; //From Siebel
                                soldto.AttCompanyName = partner.Name;
                                soldto.FaxNo = partner.Fax;
                                soldto.TelNo = partner.Tel;
                                soldto.TelExt = string.Empty;
                                soldto.Mobile = partner.Mobile;
                                soldto.Address1 = partner.Address;
                                soldto.Address2 = string.Empty;
                                soldto.City = partner.City;
                                soldto.State = partner.City; //ATW No State value
                                soldto.Country = partner.Country; // ex. eStore : Taiwan,  eQ: TW
                                soldto.County = partner.Country; // ex. eStore : Taiwan,   eQ: TW
                                soldto.CountryCode = partner.Country;
                                soldto.ZipCode = partner.ZipCode;
                                soldto.LastUpdateTime = dt;
                                soldto.UpdatedBy = User.Identity.Name;
                                soldto.CreatedDate = dt;
                                soldto.CreatedBy = User.Identity.Name;
                                soldto.LegalForm = string.Empty;
                                soldto.VATNumbe = string.Empty;
                                soldto.save(); //直接存
                                quotation.cartX.setSoldTo(soldto);
                                break;
                            case "S":
                                POCOS.CartContact shipto = new POCOS.CartContact();
                                shipto.UserID = User.Identity.Name;
                                shipto.AddressID = partner.ERPID;
                                shipto.FirstName = partner.FirstName; //From Siebel
                                shipto.LastName = partner.LasteName; //From Siebel
                                shipto.AttCompanyName = partner.Name;
                                shipto.FaxNo = partner.Fax;
                                shipto.TelNo = partner.Tel;
                                shipto.TelExt = string.Empty;
                                shipto.Mobile = partner.Mobile;
                                shipto.Address1 = partner.Address;
                                shipto.Address2 = string.Empty;
                                shipto.City = partner.City;
                                shipto.State = partner.City; //ATW No State value
                                shipto.Country = partner.Country; // ex. eStore : Taiwan, eQ: TW
                                shipto.County = partner.Country; // ex. eStore : Taiwan,  eQ: TW
                                shipto.CountryCode = partner.Country;
                                shipto.ZipCode = partner.ZipCode;
                                shipto.LastUpdateTime = dt;
                                shipto.UpdatedBy = User.Identity.Name;
                                shipto.CreatedDate = dt;
                                shipto.CreatedBy = User.Identity.Name;
                                shipto.LegalForm = string.Empty;
                                shipto.VATNumbe = string.Empty;
                                shipto.save(); //直接存
                                quotation.cartX.setShipTo(shipto);
                                break;
                            case "B":
                                POCOS.CartContact billto = new POCOS.CartContact();
                                billto.UserID = User.Identity.Name;
                                billto.AddressID = partner.ERPID;
                                billto.FirstName = partner.FirstName; //From Siebel
                                billto.LastName = partner.LasteName; //From Siebel
                                billto.AttCompanyName = partner.Name;
                                billto.FaxNo = partner.Fax;
                                billto.TelNo = partner.Tel;
                                billto.TelExt = string.Empty;
                                billto.Mobile = partner.Mobile;
                                billto.Address1 = partner.Address;
                                billto.Address2 = string.Empty;
                                billto.City = partner.City;
                                billto.State = partner.City; //ATW No State value
                                billto.Country = partner.Country; // ex. eStore : Taiwan, eQ: TW
                                billto.County = partner.Country; // ex. eStore : Taiwan,  eQ: TW
                                billto.CountryCode = partner.Country;
                                billto.ZipCode = partner.ZipCode;
                                billto.LastUpdateTime = dt;
                                billto.UpdatedBy = User.Identity.Name;
                                billto.CreatedDate = dt;
                                billto.CreatedBy = User.Identity.Name;
                                billto.LegalForm = string.Empty;
                                billto.VATNumbe = string.Empty;
                                billto.save(); // 直接存
                                quotation.cartX.setBillTo(billto);
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
                                    Presentation.eStoreContext.Current.AddStoreErrorCode(sm.Error.Code.ToString());
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
                        quotation.QuotationNumber = quote.CustomID;
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

        protected void BindDefaultQuote(POCOS.Equotation.Quote eQuote)
        {
            List<POCOS.Equotation.Quote> eQuotes = new List<POCOS.Equotation.Quote>() { eQuote };
            List<MyQuote> quotes = eQuotes.Select(eq => new MyQuote
            {
                QuotationID = eq.QuoteID,
                QuotationNumber = eq.QuoteNo,
                CustomID = eq.CustomID,
                totalAmountX = Convert.ToDecimal(eq.TotalAmount),
                currencySign = eq.Currency,
                statusX = POCOS.Quotation.QStatus.Confirmed,
                QuoteDate = eq.CreatedDate,
                QuoteExpiredDate = eq.ExpiredDate,
                Source = POCOS.Quotation.QuoteSource.eQuotation
            }).ToList();

            this.gvmyquotation.DataSource = quotes;
            this.gvmyquotation.DataBind();
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
                Presentation.eStoreContext.Current.AddStoreErrorCode(productID);
            }
        }

        private POCOS.Part getProduct(string productID)
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
    }

    public class MyQuote
    {
        public string QuotationID { get; set; }
        public string QuotationNumber { get; set; }
        public string CustomID { get; set; }
        public decimal totalAmountX { get; set; }
        public string currencySign { get; set; }
        public POCOS.Quotation.QStatus statusX { get; set; }
        public DateTime? QuoteDate { get; set; }
        public DateTime? QuoteExpiredDate { get; set; }
        public POCOS.Quotation.QuoteSource Source { get; set; }

    }
}