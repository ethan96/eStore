using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Text;
using eStore.Presentation.Product;
using esUtilities;
using eStore.BusinessModules.Templates;
using System.Net;


namespace eStore.UI.Modules
{
    public partial class CTOS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool EnableSystemIntegrityCheck { get; set; }
        public Product_Ctos _ctos { get; set; }

        public string productResourceJsonStr
        {
            get
            {
                if (_ctos != null)
                    return eStore.Presentation.Product.ProductResource.getResourceCheckingList(_ctos);
                else
                    return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {


            if (_ctos != null)
            {
                if (_ctos.notAvailable || _ctos.status == POCOS.Product.PRODUCTSTATUS.PHASED_OUT) //PHASED_OUT产品是会被搜索到的，所以没有加入到notAvailable的验证中。
                {
                    rpBTOSConfigDetails.Visible = true;
                    rpBTOSConfigDetails.DataSource = _ctos.ReplaceProductsXX;
                    rpBTOSConfigDetails.DataBind();
                    ltPhaseOut.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Phase_out);
                    ltPhaseOut.Visible = true;
                }

                int ItemNO = esUtilities.CommonHelper.QueryStringInt("ItemNO");
                string source = esUtilities.CommonHelper.QueryString("source");
                BTOSystem BTOS = getBTOSFromCart();

                renderCTOS(_ctos, BTOS);

                OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new OpenGraphProtocolAdapter(_ctos);
                OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);

                //20160809 Alex:add  CTOS page structured date 
                StructuredDataMarkup structuredDataMarkup = new StructuredDataMarkup();
                structuredDataMarkup.GenerateProductStruturedData(_ctos, this.Page);
                structuredDataMarkup.GenerateBreadcrumbStruturedData(_ctos, this.Page);

                this.setPageMeta(_ctos.pageTitle, _ctos.metaData, _ctos.keywords);
                renderProductResource(_ctos);
                bindSystemIntegrityCheck(_ctos);
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true);

                return;
            }
            bindFonts();


            BindTranslationFont();
            this.BindScript("url", "jquery.scrollfollow", "jquery.scrollfollow.js");
            this.BindScript("url", "lightbox", "jquery.lightbox-0.5.min.js");
            //this.BindScript("url", "HashTable", "HashTable.js");
            this.BindScript("Script", "lightBox", "$(function() {$(\".productLargeImages a\").lightBox({maxHeight: 600,maxWidth: 800}); });");
            this.AddStyleSheet(ResolveUrl("~/Styles/jquery.lightbox-0.5.css"));

            if (this.Page is eStore.Presentation.eStoreBaseControls.eStoreBasePage)
            {
                ((eStore.Presentation.eStoreBaseControls.eStoreBasePage)this.Page).OnLoggedinHandler += new Presentation.eStoreBaseControls.eStoreBasePage.OnLoggedin(CTOS_OnLoggedinHandler);

            }
        }

        bool CTOS_OnLoggedinHandler(string Sender)
        {
            bool result = false;
            string[] hierarchyID = Sender.Split('_');

            try
            {
                Control control = this.FindControl(hierarchyID.Last());
                if (control != null && control is LinkButton)
                {
                    if (control.ID.StartsWith("btnAdd2Cart"))
                    {
                        result = true;
                        this.btnAdd2Cart_Click(control, null);

                    }
                    else if (control.ID.StartsWith("btnAdd2Quote"))
                    {
                        result = true;
                        this.btnAdd2Quote_Click(control, null);
                    }

                }
            }
            catch (Exception)
            {


            }
            return result;
        }

        /// <summary>
        /// Bind Translation Fonts
        /// </summary>
        private void BindTranslationFont()
        {
            lbl_configItemsTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Build_Your_System);
            lbl_configItemsmessage.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Login_first_to_build_your_system);
            btnAdd2Quote.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation));
            btnAdd2QuoteTop.Text = btnAdd2Quote.Text;
            btnAdd2QuoteFloat.Text = btnAdd2Quote.Text;
            btnAdd2Cart.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart));
            btnAdd2CartTop.Text = btnAdd2Cart.Text;
            btnAdd2CartFloat.Text = btnAdd2Cart.Text;

            btnUpdate.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Update));
            btnUpdateTop.Text = btnUpdate.Text;
            btnUpdateFloat.Text = btnUpdate.Text;

            hEmail.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_emailThisPage);

            hPrint.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PrintableVersion);
            hRequestQuantityDiscount.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount));
            hRequestQuantityDiscountTop.Text = hRequestQuantityDiscount.Text;


            lAddtoCompareList.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            lPriceExtendedDescripton.Text = eStore.Presentation.eStoreContext.Current.Store.Tanslation("ProductPriceExtendedDescription");
        }

        private POCOS.Product_Ctos getProduct()
        {
            string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            var product = Presentation.eStoreContext.Current.Store.getProduct(ProductId, true);
            POCOS.Product_Ctos ctos;
            if (product != null)
            {
                ctos = (POCOS.Product_Ctos)product;
                if (ctos.notAvailable)
                {
                    if (Presentation.eStoreContext.Current.User != null && Presentation.eStoreContext.Current.User.actingRole == User.Role.Employee)
                    {
                        rpBTOSConfigDetails.Visible = true;
                        rpBTOSConfigDetails.DataSource = ctos.ReplaceProductsX;
                        rpBTOSConfigDetails.DataBind();
                        ltPhaseOut.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Phase_out);
                        ltPhaseOut.Visible = true;
                    }
                    else
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true);
                }

            }
            else
                ctos = null;
            return ctos;
        }

        private void sendInvalidSystemDetails(POCOS.Product_Ctos ctos)
        {
            if (eStoreContext.Current.isRequestFromSearchEngine() == false)
            {
                Dictionary<String, String> messageInfo = new Dictionary<string, string>();
                messageInfo.Add("EmailSubject", ctos.name + " is invalid");
                messageInfo.Add("UserID", Presentation.eStoreContext.Current.User == null ? "Customer" : Presentation.eStoreContext.Current.User.UserID);
                messageInfo.Add("ProductNmae", ctos.name);
                messageInfo.Add("ProductLink", Request.Url.OriginalString);
                messageInfo.Add("UserIP", Presentation.eStoreContext.Current.getUserIP());
                messageInfo.Add("UserAGENT", Request.UserAgent);

                messageInfo.Add("EmailTo", Presentation.eStoreContext.Current.getStringSetting("CbomEditor"));
                messageInfo.Add("EmailCC", System.Configuration.ConfigurationManager.AppSettings["eStoreItEmailGroup"]);

                string error_messages = string.Empty;
                if (ctos.ErrorMessages != null && ctos.ErrorMessages.Count > 0)
                    foreach (var c in ctos.ErrorMessages)
                        error_messages += string.Format("{0}<br />", c);
                messageInfo.Add("Error_messages", error_messages);
                EmailBasicTemplate emailContext = new EmailBasicTemplate();
                emailContext.sendEmailInBasicTemplate(Presentation.eStoreContext.Current.Store, messageInfo, false, "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">Dear eStore owner,<br><br><span style=\"color:Red;\"><i><a href=\"[/ProductLink]\">[/ProductNmae]</a></i></span>  need your maintenance because selections in one or more categories are invalid. Please verify the CBOM configuration, and make proper changes. Once complete, select the appropriate status to re-publish this system again.<br /><br /><b>Error Message:</b><br />[/Error_messages]<br><br><b> [/UserID]</b> from <b>[/UserIP]</b> is viewing this product<br><span style=\"font-size:12px\">[/UserAGENT]</span><br><br>Thanks,<br><br>eStore IT team");

            }
        }
        private void renderCTOS(Product_Ctos CTOS, BTOSystem BTOS)
        {
            bool bShowUpdateButton;
            if (BTOS == null)
            {
                bShowUpdateButton = false;
                BTOS = CTOS.getDefaultBTOS();
            }
            else
            {
                bShowUpdateButton = true;
            }
            Price _price = CTOS.updateBTOSPrice(BTOS);
            StringBuilder CTOSBuilder = new System.Text.StringBuilder();
            bool showParts = false;
            if (Presentation.eStoreContext.Current.User != null)
            {
                showParts = Presentation.eStoreContext.Current.User.actingUser.hasRight(User.ACToken.ATP);
                this.lbl_configItemsmessage.Visible = false;
            }
            else
            {
                this.lbl_configItemsmessage.Visible = true;
            }

            //registe client variable for adding anchorforspecialcategories
            int[] promotingComponents = (from con in (List<POCOS.CTOSBOM>)CTOS.components
                                         where con.Defaults == true
                                         select con.ComponentID).ToArray();

            foreach (int id in promotingComponents)
            {
                Page.ClientScript.RegisterArrayDeclaration("specialcategories", string.Format("'{0}'", id.ToString()));
            }
            //centralize logical
            string navigator = string.Empty;
              Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            this.CTOSModules.Text =ctosMgr.composeBOMUI(CTOS, BTOS, ref navigator, showParts);

            this.lProductName.Text = CTOS.name;
            this.lShortDescription.Text = CTOS.productDescX;
            this.lProductFeature.Text = CTOS.productFeatures;
            this.YouAreHereMutli1.ProductName = CTOS.name;
            this.YouAreHereMutli1.productCategories = CTOS.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite).ToList();
            this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(CTOS, BTOS, Presentation.Product.PriceStyle.productpriceLarge);
            if (CTOS.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION))
            {
                LtPromotionMessage.Text = string.Format("<p style=\"padding:6px 0px;\">{0}</p>", CTOS.PromoteMessage.Trim());
                LtPromotionMessage.Visible = true;
            }
            this.imgproductStatus.ImageUrl = string.Format("/images/{0}.gif", CTOS.status.ToString());
            this.ChangeCurrency1.defaultListPrice = CTOS.getListingPrice().value;
            hEmail.PostBackUrl = Request.RawUrl + "#";
            if (Presentation.eStoreContext.Current.User != null)
            {
                hEmail.Attributes.Add("onclick", "return showProductSharetoFriendsDialog();");
                hRequestQuantityDiscount.NavigateUrl = Request.RawUrl + "#";
                hRequestQuantityDiscountTop.NavigateUrl = Request.RawUrl + "#";
                hRequestQuantityDiscount.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
                hRequestQuantityDiscountTop.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");

            }
            QuantityDiscount._product = CTOS;
            hPrint.PostBackUrl = "/Product/print.aspx?ProductID=" + CTOS.SProductID.ToString();

            var isBelowCost = CTOS.checkIsBelowCost(BTOS);
            this.btnAdd2Cart.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnAdd2Quote.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnUpdate.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.lbl_configItemsTitle.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;

            this.btnAdd2CartTop.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnAdd2QuoteTop.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnUpdateTop.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.lbl_configItemsTitle.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;

            this.btnAdd2QuoteFloat.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnAdd2CartFloat.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnUpdateFloat.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.hRequestQuantityDiscountTop.Visible = CTOS.ShowPrice && !CTOS.notAvailable && !isBelowCost;
            this.hRequestQuantityDiscount.Visible = CTOS.ShowPrice && !CTOS.notAvailable && !isBelowCost;

            if (!CTOS.notAvailable && CTOS.publishStatus == POCOS.Product.PUBLISHSTATUS.PUBLISHED && !CTOS.isValid())
            {
                try
                {
                    sendInvalidSystemDetails(CTOS);
                }
                catch (Exception)
                {


                }
            }

            Presentation.eStoreContext.Current.keywords.Add("ProductID", CTOS.SProductID);
            Presentation.eStoreContext.Current.keywords.Add("KeyWords", CTOS.keywords +
                (string.IsNullOrEmpty(CTOS.DisplayPartno) ? "" : "," + CTOS.DisplayPartno));
            Presentation.eStoreContext.Current.BusinessGroup = CTOS.businessGroup;

            //for float div
            StringBuilder BTOSBuilder = new StringBuilder();
            //add marin
            BTOSBuilder.Append("<div class=\"floatbtospanelDiv\">");

            BTOSBuilder.AppendFormat(this.lProductprice.Text);

            BTOSBuilder.Append("<div class=\"configItemsTitle\">");
            BTOSBuilder.Append("<div class=\"coloptionimg showfloatsystemdetail\">");
            BTOSBuilder.Append("Show Details");
            BTOSBuilder.Append("</div>");
            BTOSBuilder.Append("</div>");
            BTOSBuilder.Append("<div id=\"floatsystemdetail\">");
            BTOSBuilder.Append("<ul>");
            var floatpaneldata = (from con in BTOS.BTOSConfigsWithoutNoneItems
                                  group con by new { con.CategoryComponentID, con.CategoryComponentDesc }
                                      into g
                                      where g.Count() > 0
                                      select new
                                      {
                                          CategoryComponentID = g.Key.CategoryComponentID,
                                          CategoryComponentDesc = g.Key.CategoryComponentDesc,
                                          OptionComponentDesc = (g.Count() > 0 ? string.Join(" | ", g.Select(x => x.OptionComponentDesc).ToArray()) : "None")
                                      });

            foreach (var bc in floatpaneldata)
            {
                BTOSBuilder.AppendFormat("<li  id=\"btos-{0}\"><span class=\"btosCategory\">{1}</span> : <span class=\"btosSelectItem\">{2}</span></li>", bc.CategoryComponentID, bc.CategoryComponentDesc, bc.OptionComponentDesc);
            }

            BTOSBuilder.Append("</ul>");
            BTOSBuilder.AppendFormat(this.lProductprice.Text);
            BTOSBuilder.Append("</div>");

            //add marin
            BTOSBuilder.Append("</div>");
            lFloatBTOS.Text = BTOSBuilder.ToString();

            if (Presentation.eStoreContext.Current.getBooleanSetting("ShowProductWarrantyImage", true))
            {
                imgproductwarranty.Visible = true;
                imgproductwarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", CTOS.defaultWarrantyYear);
                imgproductwarranty.ToolTip = string.Format("{0} years extended warranty", CTOS.defaultWarrantyYear);
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("eStore_linkWarrantyPolicy"))
                {
                    imgproductwarranty.NavigateUrl = "/go/WarrantyPolicy";
                    imgproductwarranty.Target = "_blank";
                }
            }
            else
            { imgproductwarranty.Visible = false; }

            //目前仅AJP Ctos使用 72小时到货
            //48 hours fast delivery is not available to System.  CTOS shall be 5 days.  Will put this back with right icon and message
            if (eStore.Presentation.eStoreContext.Current.Store.storeID == "AJP" && Presentation.eStoreContext.Current.Store.isFastDeliveryProducts(CTOS))
            {
                //imgFastDelivery .Visible = true;
                //imgFastDelivery.ImageUrl = string.Format("~/images/{0}/48_hours_v3_40.jpg", Presentation.eStoreContext.Current.Store.storeID);
                //imgFastDelivery.AlternateText = string.Format("Fast Delivery");
                divFastDelivery.InnerHtml = string.Format("<img class='floatLeft' src='{1}' title='{2}' /><div class='floatLeft'>Estimated Ship Date:<br />{0}</div>"
                    , DateTime.Now.AddHours(88).ToString("MM/dd/yy")
                    , "/images/" + Presentation.eStoreContext.Current.Store.storeID + "/72_hours_v3_40.jpg"
                    , "Estimated Ship Dates are estimates only. Delays in your delivery date may apply due to delays with selected options, or by the shipping method chosen during checkout.");
                divFastDelivery.Visible = true;
            }
            else
            {
                divFastDelivery.Visible = false;
            }

            
        }

        private void bindSystemIntegrityCheck(Product_Ctos CTOS)
        {
            try
            {
                string[] StorageIDs = System.Configuration.ConfigurationManager.AppSettings["StorageID"].Split(';');
                string OSID = System.Configuration.ConfigurationManager.AppSettings["OSID"];
                var items = from com in (List<CTOSBOM>)CTOS.components
                            where StorageIDs.Contains(com.ComponentID.ToString())
                            select com;
                var hasOS = from com in (List<CTOSBOM>)CTOS.components
                            where com.ComponentID.ToString() == OSID
                            select com;

                this.EnableSystemIntegrityCheck = (hasOS != null && items != null && hasOS.Count() > 0 && items.Count() > 0);
            }
            catch (Exception)
            {

                this.EnableSystemIntegrityCheck = false;
            }

        }
        private BTOSystem upateBTOS(Product_Ctos CTOS)
        {
            //centralize logical
            Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            return ctosMgr.updateBTOS(CTOS, Request.Form);
        }

        protected void btnAdd2Cart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            Product_Ctos _ctos = this.getProduct();
            if (_ctos != null)
            {
                POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
                BTOSystem newbtos = this.upateBTOS(_ctos);
                if (_ctos.hasMainComponent(newbtos) == false)
                {
                    eStoreContext.Current.AddStoreErrorCode("CTOS integrity problem, please contact our rep to complete your order!");
                    return;
                }
                var price = _ctos.updateBTOSPrice(newbtos);
                if (price.value < newbtos.getCost())
                {
                    eStoreContext.Current.AddStoreErrorCode("Price Error");
                    return;
                }
                CartItem cartitem = cart.addItem(_ctos, 1, newbtos);

                
                if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
                    cartitem.CustomerMessage = this.txtComment.Text;
                cart.save();
              
                    Presentation.eStoreContext.Current.Store.PublishStoreEvent(
                        Request.Url.ToString(),
                        eStoreContext.Current.User, 
                        _ctos, BusinessModules.Task.EventType.Add2Cart);
                 
                this.Response.Redirect("~/Cart/Cart.aspx");
            }

        }


        protected void btnAdd2Quote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            Product_Ctos _ctos = this.getProduct();
            if (_ctos != null)
            {
                POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
                //if (quotation.statusX != POCOS.Quotation.QStatus.Open && quotation.statusX != POCOS.Quotation.QStatus.NeedTaxIDReview)
                if (quotation.isModifiable() == false)
                {
                    quotation = eStoreContext.Current.User.actingUser.openingQuote;
                    Presentation.eStoreContext.Current.Quotation = quotation;
                }
                BTOSystem newbtos = this.upateBTOS(_ctos);
                if (_ctos.hasMainComponent(newbtos)==false)
                {
                    eStoreContext.Current.AddStoreErrorCode("CTOS integrity problem, please contact our rep to complete your order!");
                    return;
                }
                var price = _ctos.updateBTOSPrice(newbtos);
                if (price.value < newbtos.getCost())
                {
                    eStoreContext.Current.AddStoreErrorCode("CTOS price error, please contact our rep to complete your order.");
                    return;
                }
                CartItem cartitem = quotation.cartX.addItem(_ctos, 1, newbtos);
                if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != txtComment.ToolTip)
                    cartitem.CustomerMessage = this.txtComment.Text;
                quotation.save();

                this.Response.Redirect("~/Quotation/Quote.aspx");
            }
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            Product_Ctos _ctos = this.getProduct();
            if (_ctos != null)
            {
                POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
                //if (quotation.statusX != POCOS.Quotation.QStatus.Open && quotation.statusX != POCOS.Quotation.QStatus.NeedTaxIDReview)
                if (quotation.isModifiable() == false)
                {
                    quotation = eStoreContext.Current.User.actingUser.openingQuote;
                    Presentation.eStoreContext.Current.Quotation = quotation;
                }
                BTOSystem newbtos = this.upateBTOS(_ctos);
                _ctos.updateBTOSPrice(newbtos);


                int ItemNO = esUtilities.CommonHelper.QueryStringInt("ItemNO");
                string source = esUtilities.CommonHelper.QueryString("source");

                BTOSystem BTOS = new BTOSystem();
                BTOS = null;
                if (ItemNO != 0 && !string.IsNullOrEmpty(source))
                {
                    switch (source)
                    {
                        case "Cart":
                            {
                                POCOS.CartItem cartitem = Presentation.eStoreContext.Current.UserShoppingCart.getItem(ItemNO);
                                if (cartitem != null)
                                {
                                    if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != txtComment.ToolTip)
                                        cartitem.CustomerMessage = this.txtComment.Text;
                                    cartitem.BTOSystem = newbtos;
                                    Presentation.eStoreContext.Current.UserShoppingCart.updateItem(cartitem);

                                    Presentation.eStoreContext.Current.UserShoppingCart.save();
                                    this.Response.Redirect("~/Cart/Cart.aspx");
                                }
                                break;
                            }
                        case "Quotation":
                            {
                                POCOS.CartItem cartitem = Presentation.eStoreContext.Current.Quotation.getItem(ItemNO);
                                if (cartitem != null)
                                {
                                    if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != txtComment.ToolTip)
                                        cartitem.CustomerMessage = this.txtComment.Text;
                                    cartitem.BTOSystem = newbtos;
                                    Presentation.eStoreContext.Current.Quotation.updateItem(cartitem);

                                    Presentation.eStoreContext.Current.Quotation.save();
                                    this.Response.Redirect("~/Quotation/Quote.aspx");
                                }
                                break;
                            }
                        case "Order":
                            {

                                break;
                            }
                        default:
                            break;
                    }
                }


            }
        }

        private POCOS.BTOSystem getBTOSFromCart()
        {
            if (Presentation.eStoreContext.Current.User == null)// not login
                return null;

            int ItemNO = esUtilities.CommonHelper.QueryStringInt("ItemNO");
            string source = esUtilities.CommonHelper.QueryString("source");

            BTOSystem BTOS = new BTOSystem();
            BTOS = null;
            if (ItemNO != 0 && !string.IsNullOrEmpty(source))
            {
                switch (source)
                {
                    case "Cart":
                        {
                            POCOS.CartItem cartitem = Presentation.eStoreContext.Current.UserShoppingCart.getItem(ItemNO);
                            if (cartitem != null)
                            {
                                //BTOS = cartitem.BTOSystem;
                                BTOS = cartitem.btosX;
                                if (string.IsNullOrEmpty(cartitem.CustomerMessage) && !IsPostBack)
                                {
                                    this.txtComment.Text = cartitem.CustomerMessage;
                                }
                            }
                            break;
                        }
                    case "Quotation":
                        {
                            POCOS.CartItem cartitem = Presentation.eStoreContext.Current.Quotation.getItem(ItemNO);
                            if (cartitem != null)
                            {
                                //BTOS = cartitem.BTOSystem;
                                BTOS = cartitem.btosX;
                                if (string.IsNullOrEmpty(cartitem.CustomerMessage) && !IsPostBack)
                                {
                                    this.txtComment.Text = cartitem.CustomerMessage;
                                }
                            }
                            break;
                        }
                    case "Order":
                        {
                            //POCOS.CartItem cartitem = Presentation.eStoreContext.Current.Order.get(ItemNO);
                            //if (cartitem != null)
                            //{
                            //    BTOS = cartitem.BTOSystem;
                            //    Presentation.eStoreContext.Current.Order.removeItem(cartitem);
                            //}
                            break;
                        }
                    default:
                        break;
                }
            }
            else
                BTOS = null;

            return BTOS;
        }

        protected void lAddtoCompareList_Click1(object sender, ImageClickEventArgs e)
        {
            var product = this.getProduct();
            if (product != null)
            {
                ProductCompareManagement.AddProductToCompareList(product.SProductID);
                Response.Redirect("~/Compare.aspx");
            }
            else
                Response.Redirect("~/");
        }

        private void renderProductResource(POCOS.Product_Ctos ctos)
        {

            //All Img Picture
            StringBuilder sbLargePicture = new StringBuilder();
            //Small Picture
            StringBuilder sbframe = new StringBuilder();

            //LightBox
            StringBuilder sbLightBox = new StringBuilder();
            //Description
            StringBuilder sbLiterature = new StringBuilder();
            int largeimagecount = 0;
            if (ctos.specSources != null)
            {
                List<POCOS.Part> specSources = ctos.specSources.TakeWhile(p => !p.SProductID.StartsWith("option", true, null)).ToList(); ;
                POCOS.Part part=null;
                bool has3DModelDownloadFile;
                bool no3DModelOnlineView;
                List<POCOS.ProductResource> ctosresources = ctos.productResourcesX.ToList();
                if (specSources.Any() == false && ctosresources.Any(x => x.ResourceType != "eStoreLocalMainImage") == true)
                {
                    specSources.Add(ctos);
                }
                else
                {
                    //如果不作为主要resource，则追加LargeImages
                    foreach (POCOS.ProductResource pr in ctosresources.Where(x => x.ResourceType == "LargeImages"))
                    {
                        sbLightBox.AppendFormat("<li><a href=\"{0}\" title=\"{1}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\"  height=\"32px\"/></a></li>", ctos.replaceSpecialResource(pr, true), ctos.name, ctos.name);
                        largeimagecount++;
                    }
                }
              
                switch (specSources.Count())
                {

                    case 0:
                        sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", ctos.thumbnailImageX, ctos.name);
                        break;
                    case 1:
                        part = specSources.First();
                        String thumbnailImage1 = ctos.geteStoreLocalMainImage(false);
                        if (string.IsNullOrEmpty(thumbnailImage1))
                        {
                            thumbnailImage1 = part.thumbnailImageX;
                        }
                        sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", thumbnailImage1, part.SProductID);

                        sbLiterature.Append("<div><div class=\"resourceheader\">" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Resouces) + "</div><ul class=\"resourcelist\">");
                        has3DModelDownloadFile = false;
                        no3DModelOnlineView = true;
                        foreach (POCOS.ProductResource pr in part.productResourcesX)
                        {
                            switch (pr.ResourceType)
                            {
                                case "LargeImages":
                                    sbLightBox.AppendFormat("<li><a href=\"{0}\" title=\"{1}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\"  height=\"32px\"/></a></li>", ctos.replaceSpecialResource(pr, true), part.name, part.name);
                                    largeimagecount++;
                                    break;
                                case "Utilities":
                                    if (!(part.specs != null && part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                        sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, false),
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Utilities));
                                    break;
                                case "Driver":
                                    if (!(part.specs != null && part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                        sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, false),
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Driver));
                                    break;
                                case "Datasheet":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, false),
                                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet));
                                    break;
                                case "Download":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, false),
                                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Download));
                                    break;
                                case "Manual":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, false),
                                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Manual));
                                    break;
                                case "3DModelOnlineView":
                                   sbLiterature.AppendFormat("<span><a class=\"fancybox fancybox.iframe\" href=\"{0}\">{1}</a></span>", pr.ResourceURL, 
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_3DModelOnlineView));
                                   no3DModelOnlineView = false;
                                   break;
                                case "3DModel":
                                    has3DModelDownloadFile = true;
                                    break;
                                case "LargeImage":
                                default:
                                    break;

                            }
                        }
                        if (has3DModelDownloadFile && no3DModelOnlineView)
                            sbLiterature.AppendFormat("<li><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></li>",
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                               part.SProductID);
                        if (ctos.specs != null && ctos.specs.TakeWhile(s => s.CatID != 999999).Count() > 0)
                            sbLiterature.AppendFormat("<li class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></li>",
                              "Extended Specifications",
                             ctos.SProductID);
                        sbLiterature.Append("</ul></div>");

                        if (largeimagecount > 0)
                        {
                            sbLightBox.Insert(0, "<ul class=\"productLargeImages\"><li><img src=\"/images/btn_loop.jpg\"  width=\"32px\"  height=\"32px\"/></li>");
                            sbLightBox.Append("</ul>");
                            this.productframes.InnerHtml = sbLightBox.ToString();

                        }
                        // ProductLiteratureString = string.Format("<div class=\"divLargePicture\">{0} </div>{1}{2}", sbLargePicture.ToString(), sbLightBox.ToString(), sbLiterature.ToString());
                        this.productimages.InnerHtml = sbLargePicture.ToString();
                        this.productresources.InnerHtml = sbLiterature.ToString();

                        break;
                    default:

                        int index = 0;
                        foreach (POCOS.Part _part in specSources)
                        {
                            //Hardcoded logic per Russell request.  This part need to be removed once this CTOS product is phased out.
                            //String thumbnailImage = ((ctos.SProductID == "21330" || ctos.SProductID == "21336") && _part.SProductID == "HPC-7480-66A1E") ? "http://downloadt.advantech.com/download/downloadlit.aspx?LIT_ID=ba9b0918-796f-4aa0-9cba-48e0e7697b5a" : _part.TumbnailImageID;
                            String thumbnailImage = ctos.geteStoreLocalMainImage(index != 0);
                            if (string.IsNullOrEmpty(thumbnailImage))
                            {
                                thumbnailImage = _part.thumbnailImageX;
                            }

                            if (index == 0)
                            {
                                //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", _part.TumbnailImageID, _part.name);
                                sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", thumbnailImage, _part.name);
                                sbLiterature.AppendFormat("<div id=\"tab_{0}\" >", index);
                            }
                            else
                            {
                                //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\" class=\"ui-tabs-hide\"  width=\"150px\" />",thumbnailImage, _part.name);
                                sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\" class=\"ui-tabs-hide\"  width=\"150px\" />", _part.thumbnailImageX, _part.name);
                                sbLiterature.AppendFormat("<div id=\"tab_{0}\" class=\"ui-tabs-hide\">", index);

                            }

                            //sbframe.AppendFormat(" <li><a href=\"#tab_{2}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", _part.TumbnailImageID, _part.name, index);
                            sbframe.AppendFormat(" <li><a href=\"#tab_{2}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", thumbnailImage, _part.name, index);
                            sbLiterature.Append("<ul class=\"resourcelist\">");
                            has3DModelDownloadFile = false;
                            no3DModelOnlineView = true;
                            foreach (POCOS.ProductResource pr in _part.productResourcesX)
                            {

                                switch (pr.ResourceType)
                                {
                                    case "LargeImages":
                                        sbLightBox.AppendFormat("<li><a href=\"{0}\" title=\"{1}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", ctos.replaceSpecialResource(pr, true), _part.name, _part.name);
                                        largeimagecount++;
                                        break;
                                    case "Utilities":
                                        if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                            sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, index != 0),
                                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Utilities));
                                        break;
                                    case "Driver":
                                        if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                            sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, index != 0),
                                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Driver));
                                        break;
                                    case "Datasheet":
                                        sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, index != 0),
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet));
                                        break;
                                    case "Download":
                                        sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, index != 0),
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Download));
                                        break;
                                    case "Manual":
                                        sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", ctos.replaceSpecialResource(pr, index != 0),
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Manual));
                                        break;
                                    case "3DModelOnlineView":
                                        sbLiterature.AppendFormat("<span><a class=\"fancybox fancybox.iframe\" href=\"{0}\">{1}</a></span>", pr.ResourceURL, 
                                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_3DModelOnlineView));
                                        no3DModelOnlineView = false;
                                        break;
                                    case "3DModel":
                                        has3DModelDownloadFile = true;
                                        break;
                                    case "LargeImage":
                                    default:
                                        break;

                                }
                            }
                            if (has3DModelDownloadFile && no3DModelOnlineView)
                                sbLiterature.AppendFormat("<li><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></li>",
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                                   _part.SProductID);

                            if (ctos.specs != null && ctos.specs.TakeWhile(s => s.CatID != 999999).Count() > 0)
                                sbLiterature.AppendFormat("<li class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></li>",
                                  "Extended Specifications",
                                 ctos.SProductID);
                            sbLiterature.Append("</ul></div>");
                            index++;
                        }
                        this.productimages.InnerHtml = sbLargePicture.ToString();
                        this.productimages.Attributes.Add("class", "ui-tabs");
                        this.productframes.InnerHtml = string.Format("<ul class=\"frameUl\">{0}</ul>", sbframe.ToString());
                        if (largeimagecount > 0)
                        {
                            sbLightBox.Insert(0, "<ul class=\"productLargeImages\"><li><img src=\"/images/btn_loop.jpg\"  width=\"32px\"  height=\"32px\"/></li>");
                            sbLightBox.Append("</ul>");
                            this.productframes.InnerHtml = sbLightBox.ToString();

                        }
                        this.productresources.InnerHtml = sbLiterature.ToString();
                        this.productresources.Attributes.Add("class", "ui-tabs");
                        break;
                }


            }


        }

        private string replaceCTOSSpecialResource(Product_Ctos ctos, POCOS.ProductResource resource, bool keepOriginalResource)
        {
            if (resource == null)
                return string.Empty;
            if (!keepOriginalResource && ctos != null && ctos.productResourcesX.Any())
            {
                if (ctos.productResourcesX.Any(x => x.ResourceType == resource.ResourceType))
                {
                    return ctos.productResourcesX.First(x => x.ResourceType == resource.ResourceType).ResourceURL;
                }
                else
                    return resource.ResourceURL;
            }
            else
                return resource.ResourceURL;

        }

        protected void bindFonts()
        {
            txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
        }

    }
}