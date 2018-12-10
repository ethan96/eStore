using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.POCOS.DAL;
using eStore.POCOS;
using System.Web.DynamicData;
using eStore.Presentation.Product;
using System.Text;
using eStore.BusinessModules.Task;
using System.Net;

namespace eStore.UI.Modules
{
    public partial class Product : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public string storeCurrencySign
        {
            get
            {
                return eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign;
            }
        }
        public POCOS.Product CurrentProduct
        {
            get;
            set;
        }

        bool? _ShowATP = null;
        public Boolean ShowATP
        {
            get
            {
                if (_ShowATP == null)
                {
                    if (Presentation.eStoreContext.Current.User == null)
                        _ShowATP = false;
                    else
                        _ShowATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                }
                return _ShowATP ?? false;
            }
        }

        public bool EnableSystemIntegrityCheck { get; set; }

        private bool isAUSMSCLAPart
        {
            get
            {
                if (Presentation.eStoreContext.Current.Store.storeID == "AUS" && CurrentProduct.SProductID.StartsWith("968T"))
                    return true;
                return false;
            }
        }





        private int _productWidgetId = 0;
        public int productWidgetId
        {
            get { return _productWidgetId; }
        }



        protected override void CreateChildControls()
        {
            if (rblWarranty.Items.Count <= 0)
            {
                var warrantyList = Presentation.eStoreContext.Current.Store.profile.ExtendedWaranties.ToList();
                if (warrantyList != null && warrantyList.Count > 0)
                {
                    foreach (var wa in warrantyList)
                    {
                        if (wa.partX == null)
                        {
                            //do not add the item if parts is not exists
                            if (string.IsNullOrEmpty(wa.PartNo))//None item
                            {
                                ListItem noneitem = new ListItem(string.Format(" {1}", wa.PartNo, wa.Description, wa.Rate), wa.PartNo);
                                noneitem.Attributes.Add("rate", "0");
                                rblWarranty.Items.Add(noneitem);
                            }
                        }
                        else
                        {  //change display format
                            ListItem warrantyItem = new ListItem(
                                    string.Format(" {0} {1} [{2}]"
                                     , wa.partX.VendorProductDesc
                                    , ShowATP ? string.Format("<span class=\"colorRed\" >[ {0} ]</span>", wa.partX.SProductID) : string.Empty
                                    ,
                                    string.Format(" <span class=\"priceSing\">{2}</span>{0}<span class=\"addtionprice\" ><img src=\"{1}\" /></span> "
                                 , Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign
                                 , ResolveUrl("~/images/priceprocessing.gif")
                                 , "+")
             )
                                , wa.partX.SProductID);
                            warrantyItem.Attributes.Add("rate", wa.partX.getNetPrice(false).value.ToString());
                            rblWarranty.Items.Add(warrantyItem);
                        }
                    }
                    rblWarranty.SelectedIndex = 0;
                    panelWarranty.Visible = true;
                }
            }
            base.CreateChildControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RenderProduct(CurrentProduct);
            BindFont();
            if (!CurrentProduct.isWarrantable())
                rblWarranty.Items.Clear();
            this.bindSystemIntegrityCheck(CurrentProduct);
            this.setPageMeta(CurrentProduct.pageTitle, CurrentProduct.metaData, CurrentProduct.keywords);

            OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new OpenGraphProtocolAdapter(CurrentProduct);
            OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);

            //20160809 Alex:add Product page structured date 
            StructuredDataMarkup structuredDataMarkup = new StructuredDataMarkup();
            structuredDataMarkup.GenerateProductStruturedData(CurrentProduct, this.Page);
            structuredDataMarkup.GenerateBreadcrumbStruturedData(CurrentProduct, this.Page);
            structuredDataMarkup.GenerateLPSections(CurrentProduct, this.Page);

            this.BindScript("url", "lightbox", "jquery.lightbox-0.5.min.js");
            this.BindScript("Script", "lightBox", "$(function() {$(\".productLargeImages a\").lightBox(); });");
            this.AddStyleSheet(ResolveUrl("~/Styles/jquery.lightbox-0.5.css"));
            this.BindScript("url", "productaddons", "productaddons.js");

            if (this.Page is eStore.Presentation.eStoreBaseControls.eStoreBasePage)
            {
                ((eStore.Presentation.eStoreBaseControls.eStoreBasePage)this.Page).OnLoggedinHandler += new Presentation.eStoreBaseControls.eStoreBasePage.OnLoggedin(CTOS_OnLoggedinHandler);

            }

            ProductDependencies1.btnAddDependencyToCartClicked += new EventHandler(ProductDependencies1_btnAddDependencyToCartClicked);
            ProductDependencies1.btnAddDependencyToQuoteClicked += new EventHandler(ProductDependencies1_btnAddDependencyToQuoteClicked);

            if (!IsPostBack)
            {
                List<Certificates> certificate = CurrentProduct.Certificates;
                if (certificate != null && certificate.Count > 0)
                {
                    lblCertification.Visible = plCertification.Visible = true;
                    rptCertification.DataSource = certificate;
                    rptCertification.DataBind();
                }
                if (eStoreContext.Current.getBooleanSetting("hasProductWidget") && CurrentProduct.widgetPagesX != null && CurrentProduct.widgetPagesX.Any())
                {
                    var widget = CurrentProduct.widgetPagesX.OrderByDescending(c=>c.Id).FirstOrDefault();
                    if (widget != null)
                        _productWidgetId = widget.WidgetPageID.GetValueOrDefault();
                }
            }
        }

        private void ProductDependencies1_btnAddDependencyToQuoteClicked(object sender, EventArgs e)
        {
            btnAdd2Quote_Click(sender, e);
        }

        private void ProductDependencies1_btnAddDependencyToCartClicked(object sender, EventArgs e)
        {
            btnAdd2Cart_Click(sender, e);
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

        private void bindSystemIntegrityCheck(POCOS.Product product)
        {
            try
            {
                bool hasStorages = (from pc in product.PeripheralCompatibles
                                    where pc.PeripheralProduct != null
                                    && pc.PeripheralProduct.PeripheralsubCatagory != null
                                    && pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory != null
                                    && pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.integrityCheckTypeX == PeripheralCatagory.IntegrityCheckType.StandardStorage
                                    select 1
                                         ).Any();
                bool hasOS = (from pc in product.PeripheralCompatibles
                              where pc.PeripheralProduct != null
                              && pc.PeripheralProduct.PeripheralsubCatagory != null
                              && pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory != null
                              && pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.integrityCheckTypeX == PeripheralCatagory.IntegrityCheckType.StandardOS
                              select 1
                                           ).Any();


                this.EnableSystemIntegrityCheck = (hasOS && hasStorages);
            }
            catch (Exception)
            {

                this.EnableSystemIntegrityCheck = false;
            }

        }
        /// <summary>
        /// Bind page fonts
        /// </summary>
        private void BindFont()
        {
            btnAdd2Cart.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart));
            btnAdd2CartTop.Text = btnAdd2Cart.Text;
            btnAdd2Quote.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation));
            btnAdd2QuoteTop.Text = btnAdd2Quote.Text;
            //hEmail.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_emailThisPage);
            //hPrint.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PrintableVersion);
            hRequestQuantityDiscountTop.Text = string.Format("<span>{0}", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount));
     
               
            //hRequestQuantityDiscountTop.Text = hRequestQuantityDiscount.Text;
            //lAddtoCompareList.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            //lAddtoCompareList.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            //txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
            lBuilditNow.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_BuildItNow));
            //hlConfigSystem.Text = string.Format("<span class='colorBlue'>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_configured_systems));
            lPriceExtendedDescripton.Text = eStore.Presentation.eStoreContext.Current.Store.Tanslation("ProductPriceExtendedDescription");
            if (CurrentProduct != null)
            {
                ltKits.Text = CurrentProduct.DisplayPartno;
                lreversionaddons.Text = CurrentProduct.DisplayPartno;

                if (isAUSMSCLAPart)
                {
                    this.hRequestQuantityDiscountTop.Text = "<span>I am a CLA Member</span>";
                    this.QuantityDiscount.ShoppingControl = Models.ShoppingControl.CLA;
                }

                if (CurrentProduct.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire))
                {

                    this.QuantityDiscount.ShoppingControl = Models.ShoppingControl.Inquire;
                    lProductprice.Visible = false;
                    this.hRequestQuantityDiscountTop.Text = "<span>Inquire</span>";
                    this.hRequestQuantityDiscountTop.CssClass = "eStore_btn";
                    if (!string.IsNullOrEmpty(CurrentProduct.dataSheetX))
                    {
                        hDatasheet.Visible = true;
                        hDatasheet.NavigateUrl = CurrentProduct.dataSheetX;
                    }
                    btnAdd2Cart.Visible = false;
                    btnAdd2CartTop.Visible = false;

                    btnAdd2Quote.Visible = false;
                    btnAdd2QuoteTop.Visible = false;
                }

                if (Presentation.eStoreContext.Current.getBooleanSetting("QuoteOnly", false))
                {
                    btnAdd2Cart.Visible = false;
                    btnAdd2CartTop.Visible = false;
                    btnAdd2Quote.CssClass = "eStore_btn";
                    btnAdd2QuoteTop.CssClass = "eStore_btn";
                }
            }
        }

        private void RenderProduct(POCOS.Product product)
        {
            if (product == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true,410);
                return;
            }
            if (product.status == POCOS.Product.PRODUCTSTATUS.PHASED_OUT) //
            {
                rpBTOSConfigDetails.Visible = true;
                rpBTOSConfigDetails.DataSource = product.ReplaceProductsXX;
                rpBTOSConfigDetails.DataBind();
                ltPhaseOut.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Phase_out);
                ltPhaseOut.Visible = true;
            }
            this.hfProductID.Value = product.SProductID;
            this.lProductName.Text = product.name;
            this.lShortDescription.Text = product.productDescX;
            this.lProductFeature.Text = product.productFeatures;
            this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(product, Presentation.Product.PriceStyle.productpriceLarge,eStore.Presentation.eStoreContext.Current.CurrentCurrency);
            this.YouAreHereMutli1.ProductName = product.name;
            this.YouAreHereMutli1.productCategories = new List<POCOS.ProductCategory> { product.GetDefalutCategory(Presentation.eStoreContext.Current.MiniSite) };
            //ChangeCurrency1.defaultListPrice = product.getListingPrice().value;
            Presentation.eStoreContext.Current.keywords.Add("ProductID", product.SProductID);
            Presentation.eStoreContext.Current.keywords.Add("KeyWords", product.Keywords);
            Presentation.eStoreContext.Current.BusinessGroup = product.businessGroup;
            if (product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION) && !string.IsNullOrEmpty(product.PromoteMessage))
            {
                LtPromotionMessage.Text = string.Format("{0}", product.PromoteMessage.Trim());
                LtPromotionMessage.Visible = true;
            }
            //OrderInfomation
            lbMobileOrderPartNo.Text = this.lblOrderPartNo.Text = product.name;            
            lbMobileOrderDesc.Text = this.lblOrderDesc.Text = product.productDescX;
            txtOrderQty.Attributes.Add("sproductid", product.SProductID);

            if (product.MininumnOrderQty != null)
                txtOrderQty.Attributes.Add("MOQ", product.MininumnOrderQty.ToString());

            if (product.salesMarketings.Any())
            { 
                foreach(var item in product.salesMarketings)
                {
                    BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(item);
                    ltproductStatus.Text += string.Format("<img src='{0}' title='{1}' alt='{1}' />", delivery.Ico, delivery.Message);
                }
            }



            //hEmail.PostBackUrl = Request.RawUrl + "#";

            string COMContactInfo = Presentation.eStoreContext.Current.getStringSetting("SOMSalesPhoneNumber");
            if (CurrentProduct.SProductID.ToUpper().StartsWith("SOM-") && !string.IsNullOrEmpty(COMContactInfo))
            {
                this.QuantityDiscount.NeedLogin = false;
                //contactforpriceinfo.Visible = true;
                //lSOMContactInfo.Text = COMContactInfo;

                lProductprice.Visible = false;
                //lSOMProductprice.Text = lProductprice.Text;
            }
            else
            {
                //hEmail.Attributes.Add("onclick", "return showProductSharetoFriendsDialog();");
                //hRequestQuantityDiscount.NavigateUrl = Request.RawUrl + "#";
                hRequestQuantityDiscountTop.NavigateUrl = Request.RawUrl + "#";
                //hRequestQuantityDiscount.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
                hRequestQuantityDiscountTop.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
            }
            QuantityDiscount._product = product;

            //hPrint.PostBackUrl = esUtilities.CommonHelper.GetStoreLocation() +"Product/print.aspx?ProductID=" + product.SProductID.ToString();

             
            
            try
            {
                string buildsystemlink = "";
                POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
                var raw = helper.link2eStoreRaw(null, product.SProductID, Presentation.eStoreContext.Current.Store.storeID);
                int totalusedinsystem = raw.Where(x => !string.IsNullOrEmpty(x.systemSProductID)).Select(x => x.systemSProductID).Distinct().Count();
                if (totalusedinsystem==1)
                {
                    buildsystemlink = raw.Where(x => !string.IsNullOrEmpty(x.systemSProductID)).Select(x => x.systemStoreUrl).FirstOrDefault();
                }
                else if (totalusedinsystem > 1)
                {
                    buildsystemlink = ResolveUrl($"~/Configure-System/bypn-{product.SProductID}.htm" );
                }
               
              
                if (string.IsNullOrEmpty(buildsystemlink))
                {
                    this.lBuilditNow.Visible = false;
                }
                else
                {
                    if (Presentation.eStoreContext.Current.getBooleanSetting("Use_Check_configured_system"))
                        lBuilditNow.Visible = true;
                    //this.liBuilditNow.Visible = true;
                    this.lBuilditNow.NavigateUrl = buildsystemlink;
                }
            }
            catch (Exception)
            {
                //hlConfigSystem.Visible = this.liBuilditNow.Visible = false;

            }

            var isBelowCost = product.isBelowCost;
            btnMSLicenseSignUp.Visible = isAUSMSCLAPart;

            this.btnAdd2Cart.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost && !isAUSMSCLAPart;
            this.btnAdd2Quote.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost && !isAUSMSCLAPart;
            this.btnAdd2CartTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost && !isAUSMSCLAPart;
            this.btnAdd2QuoteTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost && !isAUSMSCLAPart;
            //this.hRequestQuantityDiscount.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            if (!(product.ShowPrice && !product.notAvailable && !isBelowCost))//hide build your system if not notAvailable
                lBuilditNow.Visible = false;//this.liBuilditNow.Visible = false;
            this.hRequestQuantityDiscountTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;


            IEnumerable<PeripheralCompatible> PeripheralCompatibles = product.PeripheralCompatibles.ToList();
            this.rpPeripheralCompatibles.DataSource = (from pc in PeripheralCompatibles
                                                       where pc.PeripheralProduct.Publish == true
                                                       group pc.PeripheralProduct by pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory
                                                           into _peripheralProduct
                                                           select
                                                           new PerpherailProductXX
                                                           {
                                                               ID = _peripheralProduct.Key.catagoryID,
                                                               IntegrityCheckType=_peripheralProduct.Key.integrityCheckTypeX==PeripheralCatagory.IntegrityCheckType.None?string.Empty: _peripheralProduct.Key.integrityCheckTypeX.ToString(),
                                                               CategoryName = _peripheralProduct.Key.name,
                                                               peripheralProduct = _peripheralProduct.Where(x => x.partIsOrderable == true).ToList(),
                                                               seq = _peripheralProduct.Key.seq
                                                           }).Where(c=>c.peripheralProduct.Any()).OrderBy(c => c.seq).ToList();

            if (ShowATP)
            {
                //acquire ATP value of relatedProducts
                PartHelper helper = new PartHelper();
                Dictionary<Part, int> updatingList = new Dictionary<Part, int>();
                foreach (PeripheralCompatible pc in PeripheralCompatibles)
                {
                    foreach (var itemPart in pc.PeripheralProduct.partsX)
                    {
                        if (itemPart != null && !updatingList.ContainsKey(itemPart))
                            updatingList.Add(itemPart, 1);
                    }
                }

                foreach (RelatedProduct rp in product.RelatedProductsX)
                {
                    if (rp.RelatedPart != null && !updatingList.ContainsKey(rp.RelatedPart))
                        updatingList.Add(rp.RelatedPart, 1);
                }

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                helper.setATPs(Presentation.eStoreContext.Current.Store.profile, updatingList);
            }

            this.rpPeripheralCompatibles.DataBind();
            this.rpPeripheralCompatibles.Visible = true;


            this.atpdateItem.Visible = ShowATP;
            this.atpqtyheader.Visible = ShowATP;
            this.atpqtyitem.Visible = ShowATP;
            this.atpdateheader.Visible = ShowATP;
            this.atpabcheader.Visible = ShowATP;
            this.atpabcItem.Visible = ShowATP;
            if (ShowATP)
            {
                lbMobileOrderSapABC.Text = this.lblOrderSapABC.Text = product.ABCInd;
                lbMibleOrderAvaliability.Text = this.lblOrderAvaliability.Text = eStore.Presentation.eStoreLocalization.Date(product.atp.availableDate);
                lbMobileOrderSapQty.Text = this.lblOrderSapQty.Text = product.atp.availableQty.ToString();
            }
            string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(product, false);
            if (!string.IsNullOrEmpty(LimitedresourceStr))
            {
                this.txtOrderQty.Attributes.Add("resource", LimitedresourceStr);
            }
            if (!string.IsNullOrEmpty(product.dependencyPartStr))
                this.txtOrderQty.Attributes.Add("dependency", product.dependencyPartStr);
            if (product.isWarrantable())
            {
                txtOrderQty.Attributes.Add("warrantyprice", product.getListingPrice().value.ToString());
            }
            this.renderProductResource(product);
            
            //IEnumerable<RelatedProduct> RelatedProducts = product.getAccessories(1);
            var relatels = (from rp in product.RelatedProductsX
                            where rp.RelatedPart.isOrderable(true) == true && rp.RelatedPart.getListingPrice().value > 0
                            select rp.RelatedPart);
            this.rpRelatedProducts.DataSource = relatels;
            this.rpRelatedProducts.DataBind();

            this.rpRelatedProductsMobile.DataSource = relatels;
            this.rpRelatedProductsMobile.DataBind();

            if (Presentation.eStoreContext.Current.getBooleanSetting("ShowProductWarrantyImage", true))
            {
                if (product.isEPAPS())
                {
                    imgproductwarranty.Visible = false; 
                }
                else
                {
                    if (product.WarrantyMonth != 0 && product.WarrantyMonth != null)
                    {
                        int? month = product.WarrantyMonth;
                        float? cWarrantyYear;
                        if (month % 12 == 0)
                        {
                            cWarrantyYear = month / 12;
                            imgproductwarranty.Visible = true;
                            imgproductwarranty.Title = string.Format("{0} years extended warranty", cWarrantyYear);
                            imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", cWarrantyYear);
                            imgWarranty.AlternateText = string.Format("{0} years extended warranty", cWarrantyYear);
                        }
                        else
                        {
                            if (month == 3)
                            {
                                string str = month.ToString() + "month";
                                imgproductwarranty.Visible = true;
                                imgproductwarranty.Title = string.Format("{0} month extended warranty", month);
                                imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", str);
                                imgWarranty.AlternateText = string.Format("{0} month extended warranty", month);
                            }
                            else if (month == 99)
                            {
                                imgproductwarranty.Visible = true;
                                imgproductwarranty.Title = string.Format("{0} years extended warranty", month);
                                imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", month);
                                imgWarranty.AlternateText = string.Format("{0} years extended warranty", month);
                            }
                            else
                            {
                                imgproductwarranty.Visible = true;
                    imgproductwarranty.Title = string.Format("{0} years extended warranty", product.defaultWarrantyYear);
                    imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", product.defaultWarrantyYear);
                    imgWarranty.AlternateText = string.Format("{0} years extended warranty", product.defaultWarrantyYear);
                            }
                        }
                    }
                    else
                    {
                        imgproductwarranty.Visible = true;
                        imgproductwarranty.Title = string.Format("{0} years extended warranty", product.defaultWarrantyYear);
                        imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", product.defaultWarrantyYear);
                        imgWarranty.AlternateText = string.Format("{0} years extended warranty", product.defaultWarrantyYear);
                    }
                }
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("eStore_linkWarrantyPolicy"))
                {
                    imgproductwarranty.HRef = "/go/WarrantyPolicy";
                    imgproductwarranty.Target = "_blank";
                }
            }
            else
            { 
                imgproductwarranty.Visible = false; 
            }

            if (Presentation.eStoreContext.Current.Store.isFastDeliveryProducts(product))
            {
                if (Presentation.eStoreContext.Current.Store.storeID == "AUS")
                    product.addMarketingstatus(POCOS.Product.PRODUCTMARKETINGSTATUS.TwoDaysFastDelivery);
                foreach (var item in product.deliveryMarketings)
                {
                    BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(item);
                    ltFastDelivery.Text += string.Format("<img class='floatLeft' src='{1}' title='{2}' /><br /><b class='floatLeft'>Estimated Ship Date:{0}</b>"
                    , delivery.EndDeliveryTime.ToString("MM/dd/yy")
                    , delivery.Ico
                    , delivery.Message);
                }
                ltFastDelivery.Visible = true;
            }
            else
            {
                ltFastDelivery.Visible = false;
            }

            this.ProductDependencies1.partLs = product.allDependencyProduct;
        }

        protected void btnAdd2Cart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
            if (isMobile)
                Add2MobileCart(cart);
            else
                Add2Cart(cart);
            //Cart中有错误将不会跳转页面
            //if (cart.error_message != null && cart.error_message.Any())
            //    return;

            cart.save();
            Presentation.eStoreContext.Current.Store.PublishStoreEvent(Request.Url.ToString(), eStoreContext.Current.User
                                   , CurrentProduct, EventType.Add2Cart);

            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            this.Response.Redirect("~/Cart/Cart.aspx");

        }

        protected void btnMSLicenseSignUp_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("http://partners.advantech.com/claform");

        }

        

        protected void btnAdd2Quote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
            //if (quotation.statusX != POCOS.Quotation.QStatus.Open && quotation.statusX != POCOS.Quotation.QStatus.NeedTaxIDReview)
            if (quotation.isModifiable() == false)
            {
                quotation = eStoreContext.Current.User.actingUser.openingQuote;
                Presentation.eStoreContext.Current.Quotation = quotation;
            }
            //CartItem ci = new CartItem();
            //ci.partX
            if(isMobile)
                Add2MobileCart(quotation.cartX);
            else
                Add2Cart(quotation.cartX);

            ////Add to Cart有错误时 跳出
            //if (quotation.cartX.error_message != null && quotation.cartX.error_message.Any())
            //    return;

            quotation.save();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        void Add2Cart(POCOS.Cart cart)
        {
            //POCOS.Cart _cart = new POCOS.Cart();
            //cart.copyTo(_cart);

            List<BundleItem> bundle = new List<BundleItem>();

            CartItem cartitem;
            int dependencyItemNo = 0;
            int qty = 0;
            if (!int.TryParse(this.txtOrderQty.Text, out qty))
            { qty = 1; }
            else
            {
                if (qty < 1)
                    qty = 1;
            }
            foreach (RepeaterItem item in rpPeripheralCompatibles.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    Presentation.eStoreControls.Repeater rpPeripheralProducts = (Presentation.eStoreControls.Repeater)item.FindControl("rpPeripheralProducts");

                    foreach (RepeaterItem ppItem in rpPeripheralProducts.Items)
                    {
                        if (ppItem.ItemType == ListItemType.AlternatingItem || ppItem.ItemType == ListItemType.Item)
                        {
                            TextBox txtQty = (TextBox)ppItem.FindControl("txtQty");
                            HiddenField hSProductID = (HiddenField)ppItem.FindControl("hSProductID");
                            if (!string.IsNullOrWhiteSpace(txtQty.Text))
                            {
                                int result;
                                Int32.TryParse(txtQty.Text, out result);
                                string comSPproductID = hSProductID.Value;
                                if (result > 0)
                                {
                                    PeripheralProduct pp = (from peripheralCompatibles in CurrentProduct.PeripheralCompatibles
                                                            where peripheralCompatibles.PeripheralProduct.SProductID.Equals(comSPproductID)
                                                            && peripheralCompatibles.PeripheralProduct.Publish == true
                                                            select peripheralCompatibles.PeripheralProduct).FirstOrDefault();
                                    if (pp != null)
                                    {
                                        if (pp.partsX != null && pp.partsX.Count > 0)
                                        {
                                            foreach (POCOS.Part itemPart in pp.partsX)
                                            {
                                                string peripheralProducts = "";
                                                if (pp.partsX.Count > 1)
                                                    peripheralProducts = pp.SProductID;
                                                string peripheralCatagoryName = pp.PeripheralsubCatagory != null ?
                                                (pp.PeripheralsubCatagory.PeripheralCatagory != null ? pp.PeripheralsubCatagory.PeripheralCatagory.name : pp.PeripheralsubCatagory.name) : string.Empty;
                                                if (itemPart != null && itemPart.getListingPrice().value > 0)
                                                    bundle.Add(new BundleItem(itemPart, pp.qtyX * result, 1, 0m, peripheralProducts, peripheralCatagoryName));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            //for testing, will get from  warranty selections
            Part warranty = null;
            if (rblWarranty.Items.Count > 0 && !string.IsNullOrEmpty(rblWarranty.SelectedValue))
                warranty = Presentation.eStoreContext.Current.Store.getPart(rblWarranty.SelectedValue);

            if (bundle.Count > 0)
            {
                bundle.Insert(0, new BundleItem(CurrentProduct, 1, 1, 0, "", "Base"));
                POCOS.Product BundleProduct;
                if (bundle.FirstOrDefault(b => b.part.isOS() == true) != null)
                    BundleProduct = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO-OS");
                else
                    BundleProduct = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO");

                Product_Ctos bundleCTOS = null;
                if (BundleProduct is Product_Ctos)
                    bundleCTOS = (Product_Ctos)BundleProduct;
                BTOSystem btos = bundleCTOS.getDefaultBTOS();
                btos.addNoneCTOSBundle(bundle, 1);
                cartitem = cart.addItem(bundleCTOS, qty, btos, 0, null, false, 0, true, warranty);
            }
            else
            {
                cartitem = cart.addItem(CurrentProduct, qty, null, 0, null, false, 0, true, warranty);
                dependencyItemNo = cartitem.ItemNo;
            }
            //if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
            //    cartitem.CustomerMessage = this.txtComment.Text;

            foreach (RepeaterItem item in rpRelatedProducts.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    TextBox txtQty = (TextBox)item.FindControl("txtQty");
                    HiddenField hSProductID = (HiddenField)item.FindControl("hSProductID");
                    if (!string.IsNullOrWhiteSpace(txtQty.Text))
                    {
                        int result;
                        Int32.TryParse(txtQty.Text, out result);
                        if (result > 0)
                        {
                            string relatedSProductID = hSProductID.Value;
                            var part = (from parts in CurrentProduct.RelatedProductsX
                                        where parts.RelatedSProductID.Equals(relatedSProductID)
                                        select parts).FirstOrDefault();
                            if (part != null
                                && ((POCOS.RelatedProduct)part).RelatedPart != null
                                && ((POCOS.RelatedProduct)part).RelatedPart.getListingPrice().value > 0)
                            {
                                cartitem = cart.addItem(((POCOS.RelatedProduct)part).RelatedPart, result, null, 0, null, false, 0, true, warranty);
                            }
                        }

                    }
                }
            }

            //add product addon to cart
            string addonQTYStr = Request.Form["hdProductAddonQtyList"];
            if (!string.IsNullOrEmpty(addonQTYStr))
            {
                var addonQTYList = addonQTYStr.Split('|');
                if (addonQTYList != null && addonQTYList.Any())
                {
                    foreach (var c in addonQTYList)
                    {
                        int result;
                        Int32.TryParse(Request.Form["inputAddonQTY_" + c], out result);
                        if (result > 0)
                        {
                            int peripheralBundleID = 0;
                            int.TryParse(c, out peripheralBundleID);
                            if (CurrentProduct.PeripheralAddOns.Any())
                            {
                                var peripheralBundle = (from bundleitem in CurrentProduct.PeripheralAddOns
                                                        where bundleitem.AddOnItemID.Equals(peripheralBundleID)
                                                        select bundleitem).FirstOrDefault();
                                if (peripheralBundle != null)
                                {
                                    cartitem = cart.addItem(((POCOS.PeripheralAddOn)peripheralBundle).addOnProduct, result * qty, null, 0, null, false, 0, true, warranty);
                                }
                            }
                            else
                            {
                                var peripheralBundle = eStoreContext.Current.Store.getPeripheralAddOnById(peripheralBundleID);
                                if (peripheralBundle != null)
                                {
                                    cartitem = cart.addItem(((POCOS.PeripheralAddOn)peripheralBundle).addOnReversionProduct, result * qty, null, 0, null, false, 0, true, warranty);
                                }
                            }
                        }
                    }
                }
            }

            // add dependency products 
            var DDependencyProductsLs = ProductDependencies1.getSelectDependencyProducts();
            if (DDependencyProductsLs != null && DDependencyProductsLs.Count > 0 && dependencyItemNo > 0)
            {
                foreach (var dic in DDependencyProductsLs)
                    cartitem = cart.addItem(dic.Key, dic.Value, null, 0, null, false, 0, true, warranty, dependencyItemNo);
            }

            if (cart.error_message != null && cart.error_message.Any())
            {
                var MinError = cart.error_message.Where(c => c.columname == "MOQ_restriction").ToList();
                if (MinError != null && MinError.Any())
                {
                    Presentation.eStoreContext.Current.StoreErrorCode.Add(
                            new StoreErrorCode() { UserActionMessage = string.Format(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Minimum_Quantity_Error), string.Join(",", MinError.Select(c => c.message))) }
                        );
                }
            }

            //cart.save();
        }

        void Add2MobileCart(POCOS.Cart cart)
        {
            List<BundleItem> bundle = new List<BundleItem>();

            CartItem cartitem;
            int dependencyItemNo = 0;
            int qty = 0;
            if (!int.TryParse(this.txtMobileOrderQty.Text, out qty))
            { qty = 1; }
            else
            {
                if (qty < 1)
                    qty = 1;
            }
            foreach (RepeaterItem item in rpPeripheralCompatibles.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    Presentation.eStoreControls.Repeater rpPeripheralProducts = (Presentation.eStoreControls.Repeater)item.FindControl("rpPeripheralProductsMobile");

                    foreach (RepeaterItem ppItem in rpPeripheralProducts.Items)
                    {
                        if (ppItem.ItemType == ListItemType.AlternatingItem || ppItem.ItemType == ListItemType.Item)
                        {
                            TextBox txtQty = (TextBox)ppItem.FindControl("txtMobileQty");
                            HiddenField hSProductID = (HiddenField)ppItem.FindControl("hSProductID");
                            if (!string.IsNullOrWhiteSpace(txtQty.Text))
                            {
                                int result;
                                Int32.TryParse(txtQty.Text, out result);
                                string comSPproductID = hSProductID.Value;
                                if (result > 0)
                                {
                                    PeripheralProduct pp = (from peripheralCompatibles in CurrentProduct.PeripheralCompatibles
                                                            where peripheralCompatibles.PeripheralProduct.SProductID.Equals(comSPproductID)
                                                            && peripheralCompatibles.PeripheralProduct.Publish == true
                                                            select peripheralCompatibles.PeripheralProduct).FirstOrDefault();
                                    if (pp != null)
                                    {
                                        if (pp.partsX != null && pp.partsX.Count > 0)
                                        {
                                            foreach (POCOS.Part itemPart in pp.partsX)
                                            {
                                                string peripheralProducts = "";
                                                if (pp.partsX.Count > 1)
                                                    peripheralProducts = pp.SProductID;
                                                string peripheralCatagoryName = pp.PeripheralsubCatagory != null ?
                                                (pp.PeripheralsubCatagory.PeripheralCatagory != null ? pp.PeripheralsubCatagory.PeripheralCatagory.name : pp.PeripheralsubCatagory.name) : string.Empty;
                                                if (itemPart != null && itemPart.getListingPrice().value > 0)
                                                    bundle.Add(new BundleItem(itemPart, pp.qtyX * result, 1, 0m, peripheralProducts, peripheralCatagoryName));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            //for testing, will get from  warranty selections
            Part warranty = null;
            if (rblWarranty.Items.Count > 0 && !string.IsNullOrEmpty(rblWarranty.SelectedValue))
                warranty = Presentation.eStoreContext.Current.Store.getPart(rblWarranty.SelectedValue);

            if (bundle.Count > 0)
            {
                bundle.Insert(0, new BundleItem(CurrentProduct, 1, 1, 0, "", "Base"));
                POCOS.Product BundleProduct;
                if (bundle.FirstOrDefault(b => b.part.isOS() == true) != null)
                    BundleProduct = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO-OS");
                else
                    BundleProduct = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO");

                Product_Ctos bundleCTOS = null;
                if (BundleProduct is Product_Ctos)
                    bundleCTOS = (Product_Ctos)BundleProduct;
                BTOSystem btos = bundleCTOS.getDefaultBTOS();
                btos.addNoneCTOSBundle(bundle, 1);
                cartitem = cart.addItem(bundleCTOS, qty, btos, 0, null, false, 0, true, warranty);
            }
            else
            {
                cartitem = cart.addItem(CurrentProduct, qty, null, 0, null, false, 0, true, warranty);
                dependencyItemNo = cartitem.ItemNo;
            }
            //if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
            //    cartitem.CustomerMessage = this.txtComment.Text;

            foreach (RepeaterItem item in rpRelatedProductsMobile.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    TextBox txtQty = (TextBox)item.FindControl("txtMobileQty");
                    HiddenField hSProductID = (HiddenField)item.FindControl("hSProductID");
                    if (!string.IsNullOrWhiteSpace(txtQty.Text))
                    {
                        int result;
                        Int32.TryParse(txtQty.Text, out result);
                        if (result > 0)
                        {
                            string relatedSProductID = hSProductID.Value;
                            var part = (from parts in CurrentProduct.RelatedProductsX
                                        where parts.RelatedSProductID.Equals(relatedSProductID)
                                        select parts).FirstOrDefault();
                            if (part != null
                                && ((POCOS.RelatedProduct)part).RelatedPart != null
                                && ((POCOS.RelatedProduct)part).RelatedPart.getListingPrice().value > 0)
                            {
                                cartitem = cart.addItem(((POCOS.RelatedProduct)part).RelatedPart, result, null, 0, null, false, 0, true, warranty);
                            }
                        }

                    }
                }
            }

            //add product addon to cart
            string addonQTYStr = Request.Form["hdProductAddonQtyList"];
            if (!string.IsNullOrEmpty(addonQTYStr))
            {
                var addonQTYList = addonQTYStr.Split('|');
                if (addonQTYList != null && addonQTYList.Any())
                {
                    foreach (var c in addonQTYList)
                    {
                        int result;
                        Int32.TryParse(Request.Form["inputAddonQTY_" + c], out result);
                        if (result > 0)
                        {
                            int peripheralBundleID = 0;
                            int.TryParse(c, out peripheralBundleID);
                            if (CurrentProduct.PeripheralAddOns.Any())
                            {
                                var peripheralBundle = (from bundleitem in CurrentProduct.PeripheralAddOns
                                                        where bundleitem.AddOnItemID.Equals(peripheralBundleID)
                                                        select bundleitem).FirstOrDefault();
                                if (peripheralBundle != null)
                                {
                                    cartitem = cart.addItem(((POCOS.PeripheralAddOn)peripheralBundle).addOnProduct, result * qty, null, 0, null, false, 0, true, warranty);
                                }
                            }
                            else
                            {
                                var peripheralBundle = eStoreContext.Current.Store.getPeripheralAddOnById(peripheralBundleID);
                                if (peripheralBundle != null)
                                {
                                    cartitem = cart.addItem(((POCOS.PeripheralAddOn)peripheralBundle).addOnReversionProduct, result * qty, null, 0, null, false, 0, true, warranty);
                                }
                            }
                        }
                    }
                }
            }

            // add dependency products 
            var DDependencyProductsLs = ProductDependencies1.getSelectDependencyProducts();
            if (DDependencyProductsLs != null && DDependencyProductsLs.Count > 0 && dependencyItemNo > 0)
            {
                foreach (var dic in DDependencyProductsLs)
                    cartitem = cart.addItem(dic.Key, dic.Value, null, 0, null, false, 0, true, warranty, dependencyItemNo);
            }

            if (cart.error_message != null && cart.error_message.Any())
            {
                var MinError = cart.error_message.Where(c => c.columname == "MOQ_restriction").ToList();
                if (MinError != null && MinError.Any())
                {
                    Presentation.eStoreContext.Current.StoreErrorCode.Add(
                            new StoreErrorCode() { UserActionMessage = string.Format(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Minimum_Quantity_Error), string.Join(",", MinError.Select(c => c.message))) }
                        );
                }
            }

            //cart.save();
        }

        private void renderProductResource(POCOS.Product part)
        {

            //All Img Picture
            //StringBuilder sbLargePicture = new StringBuilder();
            //Small Picture
            StringBuilder sbframe = new StringBuilder();

            //LightBox
            StringBuilder sbLightBox = new StringBuilder();
            //Description
            StringBuilder sbLiterature = new StringBuilder();
            bool has3DModelDownloadFile;
            bool no3DModelOnlineView;
            imgLargePicturelarg.ImageUrl = imgLargePicture.ImageUrl = part.thumbnailImageX;

            imgLargePicturelarg.AlternateText = part.productDescX;
            imgLargePicturelarg.ToolTip = part.SProductID;

            imgLargePicture.AlternateText = part.productDescX;
            imgLargePicture.ToolTip = part.SProductID;
 
            //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\" width=\"150px\" />", part.TumbnailImageID, part.SProductID);

            has3DModelDownloadFile = false;
            no3DModelOnlineView = true;
            int largeimagecount = 0;
            string videoInfor = "";
            foreach (POCOS.ProductResource pr in part.productResourcesX)
            {
                switch (pr.ResourceType)
                {
                    case "LargeImages":
                        if (sbLightBox.Length == 0)
                            sbLightBox.AppendFormat("<li class=\"on\"><img src=\"{0}\" alt=\"{2}\" title=\"{1}\" data-BG=\"{0}\" /></li>", pr.ResourceURL, part.name,HttpUtility.HtmlEncode( part.productDescX));
                        else
                            sbLightBox.AppendFormat("<li><img src=\"{0}\" alt=\"{2}\" title=\"{1}\" data-BG=\"{0}\" /></li>", pr.ResourceURL, part.name, HttpUtility.HtmlEncode(part.productDescX));
                        largeimagecount++;
                        break;
                    case "Utilities":
                        if (!(part.specs != null && part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                            sbLiterature.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('utilities', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Utilities), part.SProductID);
                        break;
                    case "Driver":
                        if (!(part.specs != null && part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                            sbLiterature.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('driver', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Driver), part.SProductID);
                        break;
                    case "Datasheet":
                        string para = "?utm_source=eStore";
                        if (pr.ResourceURL.IndexOf("?") > -1)
                            para =  "&utm_source=eStore";
                        sbLiterature.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('datasheet', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL + para,
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet), part.SProductID);
                        break;
                    case "Download":
                        sbLiterature.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('files', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Download), part.SProductID);
                        break;
                    case "Manual":
                        sbLiterature.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('manual', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Manual), part.SProductID);
                        break;
                    case "3DModelOnlineView":
                        sbLiterature.AppendFormat("<span><a class=\"fancybox fancybox.iframe\" href=\"{0}\">{1}</a></span>", pr.ResourceURL, 
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_3DModelOnlineView));
                        no3DModelOnlineView = false;
                        break;
                    case "3DModel":
                        has3DModelDownloadFile = true;
                        break;
                    case "video":
                        videoInfor = string.Format("<li><a href=\"{2}\" class=\"youtube\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" msrc=\"$.bgimage\" data-BG=\"$.bgimage\" /></a></li><script type=\"text/javascript\">showVideo();</script>"
                                    ,(!string.IsNullOrEmpty(pr.advertisementX.Imagefile) ? esUtilities.CommonHelper.GetStoreLocation() + "resource"+ pr.advertisementX.Imagefile : "../images/video_icon.png")
                                    ,pr.advertisementX.AlternateText
                                    ,pr.advertisementX.Hyperlink);
                        videoInfor += pr.advertisementX.htmlContentX;
                        break;
                    case "LargeImage":
                    default:
                        break;

                }
            }
            if (has3DModelDownloadFile && no3DModelOnlineView )
                sbLiterature.AppendFormat("<span><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></span>",
                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                   part.SProductID);
            if (part is POCOS.Product && part.specs != null && part.specs.Count > 0)
                sbLiterature.AppendFormat("<span class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></span>",
                  eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Extended_Specifications),
                 part.SProductID);
            string policyUrl = string.Empty; //eStore.Presentation.eStoreContext.Current.Store.policylink();
            if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("showPolicyOnProduct"))
            {
                if (part.isEPAPS())
                {
                    policyUrl= eStore.Presentation.eStoreContext.Current.Store.profile.getStringSetting("pstorePolicyUrl");
                }
                else
                {
                    policyUrl = eStore.Presentation.eStoreContext.Current.Store.profile.getStringSetting("estorePolicyUrl");
                
                }
            }    

            if (!string.IsNullOrEmpty(policyUrl))
            {
                sbLiterature.AppendFormat("<span>{0}</span>", policyUrl);
            }
            if (!string.IsNullOrEmpty(videoInfor))
                sbLightBox.Insert(0, videoInfor.Replace("$.bgimage", part.thumbnailImageX));

            if (largeimagecount > 0)
                this.productframes.InnerHtml = sbLightBox.ToString();

            this.productresources.Text = sbLiterature.ToString();

        }

        protected void rpPeripheralCompatibles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rpPeripheralProducts = e.Item.FindControl("rpPeripheralProducts") as Repeater;
                PerpherailProductXX pc = e.Item.DataItem as PerpherailProductXX;
                rpPeripheralProducts.DataSource = pc.peripheralProduct.Where(c=>c.partIsOrderable).OrderBy(c => c.SProductID).ToList();
                rpPeripheralProducts.DataBind();

                Repeater rpPeripheralProductsMobile = e.Item.FindControl("rpPeripheralProductsMobile") as Repeater;
                rpPeripheralProductsMobile.DataSource = pc.peripheralProduct.Where(c => c.partIsOrderable).OrderBy(c => c.SProductID).ToList();
                rpPeripheralProductsMobile.DataBind();
            }
        }

        protected void rpPeripheralProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                PeripheralProduct _peripheralProduct = e.Item.DataItem as PeripheralProduct;
                TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
                if (_peripheralProduct != null && _peripheralProduct.partsX != null && _peripheralProduct.partsX.Count > 0)
                {
                    if (txtQty != null)
                    {
                        txtQty.Attributes.Add("sproductid", _peripheralProduct.SProductID);
                        string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(_peripheralProduct.partsX);
                        if (!string.IsNullOrEmpty(LimitedresourceStr))
                            txtQty.Attributes.Add("resource", LimitedresourceStr);
                        if (_peripheralProduct.partsX != null && _peripheralProduct.partsX.Count > 0 && _peripheralProduct.isWarrantable)
                            txtQty.Attributes.Add("warrantyprice", _peripheralProduct.getWarrantPrice.ToString());
                        if (!string.IsNullOrEmpty(_peripheralProduct.dependencyPartStr))
                            txtQty.Attributes.Add("dependency", _peripheralProduct.dependencyPartStr);
                        if (_peripheralProduct.hasBuiltInStorage)
                            txtQty.Attributes.Add("hasBuiltInStorage", "true");
                    }
                    Literal ltDescription = e.Item.FindControl("ltDescription") as Literal;
                    if (ltDescription != null)
                    {
                        ltDescription.Text = !string.IsNullOrEmpty(_peripheralProduct.Description) ? _peripheralProduct.Description
                            : string.Join(";", _peripheralProduct.partsX.Select(c => c.productDescX));
                    }
                }


            }
        }

        protected void rpRelatedProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.Part _relatedProduct = e.Item.DataItem as POCOS.Part;
                TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;

                if (_relatedProduct != null)
                {
                    string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(_relatedProduct);
                    if (!string.IsNullOrEmpty(LimitedresourceStr))
                        txtQty.Attributes.Add("resource", LimitedresourceStr);
                    txtQty.Attributes.Add("sproductid", _relatedProduct.SProductID);
                    if (!string.IsNullOrEmpty(_relatedProduct.dependencyPartStr))
                        txtQty.Attributes.Add("dependency", _relatedProduct.dependencyPartStr);

                    if (_relatedProduct.isWarrantable())
                    {
                        txtQty.Attributes.Add("warrantyprice", _relatedProduct.getListingPrice().value.ToString());
                    }
                    if (_relatedProduct.MininumnOrderQty != null)
                        txtQty.Attributes.Add("MOQ", _relatedProduct.MininumnOrderQty.ToString());
                }

            }
        }

        protected string getPeripheralProductPrice(POCOS.Part part)
        {
            if (part.isOS())
                //return eStore.Presentation.eStoreLocalization.Tanslation(Store.TranslationKey.Add_for_price);
                return "Add for price";
            else
                return eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)part.getListingPrice().value);
        }
        protected string getPeripheralProductPrice(List<POCOS.Part> partList)
        {
            if (partList != null && partList.Count > 0)
            {
                decimal totalPrice = 0;
                foreach (var itemPart in partList)
                {
                    if (itemPart.isOS())
                        //return eStore.Presentation.eStoreLocalization.Tanslation(Store.TranslationKey.Add_for_price);
                        return "Add for price";
                    else
                        totalPrice += itemPart.getListingPrice().value;
                }
                return eStore.Presentation.Product.ProductPrice.FormartPrice(totalPrice);
            }
            else
                return "Add for price";
        }
        protected void lAddtoCompareList_Click1(object sender, EventArgs e)
        {
            if (CurrentProduct != null)
            {
                ProductCompareManagement.AddProductToCompareList(CurrentProduct.SProductID);
                Response.Redirect("~/Compare.aspx");
            }
            else
                Response.Redirect("~/");
        }

        protected void rptCertification_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Image image = e.Item.FindControl("imgCertification") as Image;
                Certificates c = e.Item.DataItem as Certificates;
                image.ImageUrl = c.CertificateImagePath;

                image.ToolTip = c.CertificateName;
                image.AlternateText = c.CertificateName;

                image.Attributes.Add("style", "margin-right:5px; max-width:150px; max-height:50px;");
            }
        }

    }

    public class PerpherailProductXX
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public string IntegrityCheckType { get; set; }
        public List<PeripheralProduct> peripheralProduct { get; set; }
        public int seq { get; set; }
    }
}