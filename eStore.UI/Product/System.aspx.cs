using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;
using eStore.Presentation;
using eStore.POCOS;
using eStore.BusinessModules.Templates;
using System.Text;
using System.Configuration;
using eStore.Presentation.Product;
using System.Net;

namespace eStore.UI.Product
{
    public partial class SystemPage : BaseProduct
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        public bool EnableSystemIntegrityCheck { get; set; }
        public Product_Ctos _ctos { get; set; }
        private int _productWidgetId = 0;
        public int productWidgetId
        {
            get { return _productWidgetId; }
        }
        public bool hasWidgetConfig
        {
            get { return productWidgetId != 0; }
        }
        
        protected string productId=string.Empty;
        protected  string navigator = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
           _ctos = this.getProduct();
           if (_ctos != null)
            {
                bindFonts();

                BindTranslationFont();

                if (_ctos.notAvailable || _ctos.status == POCOS.Product.PRODUCTSTATUS.PHASED_OUT) //PHASED_OUT产品是会被搜索到的，所以没有加入到notAvailable的验证中。
               {
                   rpBTOSConfigDetails.Visible = true;
                   rpBTOSConfigDetails.DataSource = _ctos.ReplaceProductsXX;
                   rpBTOSConfigDetails.DataBind();
                   if (_ctos.status != POCOS.Product.PRODUCTSTATUS.TOBEREVIEW) // tobereivew 不列入 phase out 行列。
                   {
                       ltPhaseOut.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Phase_out);
                       ltPhaseOut.Visible = true;
                   }
               }
               hfProductID.Value = _ctos.SProductID;
               productId = _ctos.SProductID;
               int ItemNO = esUtilities.CommonHelper.QueryStringInt("ItemNO");
               string source = esUtilities.CommonHelper.QueryString("source");
               BTOSystem BTOS = getBTOSFromCart();

                renderCTOS(_ctos, BTOS);

               OpenGraphProtocolAdapter OpenGraphProtocolAdapter = new OpenGraphProtocolAdapter(_ctos);
               OpenGraphProtocolAdapter.addOpenGraphProtocolMetedata(this.Page);
               setCanonicalPage(Presentation.UrlRewriting.MappingUrl.getMappingUrl(_ctos));
               //20160809 Alex:add System page structured date 
               eStore.Presentation.StructuredDataMarkup structuredDataMarkup = new eStore.Presentation.StructuredDataMarkup();
               structuredDataMarkup.GenerateProductStruturedData(_ctos, this.Page);
               structuredDataMarkup.GenerateBreadcrumbStruturedData(_ctos, this.Page);
                structuredDataMarkup.GenerateLPSections(_ctos, this.Page);
                this.isExistsPageMeta = this.setPageMeta(_ctos.pageTitle, _ctos.metaData, _ctos.keywords);
                this.BlockSearchIndexing = _ctos.notAvailable;
               renderProductResource(_ctos);
               bindSystemIntegrityCheck(_ctos);
           }
           else
           {
               Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true, 404);

               return;
           }
         

           if (this.Page is eStore.Presentation.eStoreBaseControls.eStoreBasePage)
           {
               ((eStore.Presentation.eStoreBaseControls.eStoreBasePage)this.Page).OnLoggedinHandler += new Presentation.eStoreBaseControls.eStoreBasePage.OnLoggedin(CTOS_OnLoggedinHandler);

           }

           if (_ctos != null && eStoreContext.Current.User != null && 
               !IsPostBack)
            {
                Presentation.eStoreContext.Current.Store.PublishStoreEvent(
                  Request.Url.ToString(),
                    eStoreContext.Current.User
                    , _ctos
                    , BusinessModules.Task.EventType.BuildSystem); 
            }

           if (!IsPostBack)
           {
               List<Certificates> certificate = _ctos.Certificates;
               if (certificate != null && certificate.Count > 0)
               {
                   plCertification.Visible = true;
                   rptCertification.DataSource = certificate;
                   rptCertification.DataBind();
               }
               if (eStoreContext.Current.getBooleanSetting("hasProductWidget") && _ctos.widgetPagesX != null && _ctos.widgetPagesX.Any())
               {
                   var widget = _ctos.widgetPagesX.OrderByDescending(c => c.Id).FirstOrDefault();
                   if (widget != null)
                       _productWidgetId = widget.WidgetPageID.GetValueOrDefault();
               }

                //2017/02/23 test for ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureProductDetail", GoogleGAHelper.MeasureProductDetail(_ctos).ToString(), true);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureAddToCart", GoogleGAHelper.MeasureAddToCart(_ctos).ToString(), true);
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

            bindSetpText(ref btnAdd2QuoteTop, ref btnAdd2QuoteFloat
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation)
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation)
                , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
                , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
                , "AddtoQuotation"
                );

            bindSetpText(ref btnAdd2CartTop, ref btnAdd2CartFloat
           , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart)
           , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart)
           , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
           , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
           , "AddtoCart"
           );

            bindSetpText(ref btnUpdateTop, ref btnUpdateFloat
           , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Update)
           , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Update)
           , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
           , eStore.Presentation.eStoreContext.Current.Store.Tanslation("Next")
           , "Update"
           );
 

            //hEmail.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_emailThisPage);

            //hPrint.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PrintableVersion);
            //hRequestQuantityDiscount.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount));
            hRequestQuantityDiscountTop.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount)); //hRequestQuantityDiscount.Text;

            //lAddtoCompareList.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            //lAddtoCompareList.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            lPriceExtendedDescripton.Text = eStore.Presentation.eStoreContext.Current.Store.Tanslation("ProductPriceExtendedDescription");
        }

        private void bindSetpText(ref LinkButton top, ref LinkButton floatbtn, string text, string firststeptext, string middlesteptext, string laststeptext, string actiongroup)
        {
            top.Text = string.Format("<span>{0}</span>", text);
            top.Attributes.Add("firststep", firststeptext);
            top.Attributes.Add("middlestep", middlesteptext);
            top.Attributes.Add("laststep", laststeptext);
            top.Attributes.Add("actiongroup", actiongroup);
            
            floatbtn.Text = top.Text;
            floatbtn.Attributes.Add("firststep", firststeptext);
            floatbtn.Attributes.Add("middlestep", middlesteptext);
            floatbtn.Attributes.Add("laststep", laststeptext);
            floatbtn.Attributes.Add("actiongroup", actiongroup);
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
                messageInfo.Add("OMSystemLink", "http://buy.advantech.com/admin/Cbom/System_New.aspx?CID=" + ctos.SProductID + "&StoreId=" + ctos.StoreID);

                messageInfo.Add("EmailTo", Presentation.eStoreContext.Current.getStringSetting("CbomEditor"));
                messageInfo.Add("EmailCC", ConfigurationManager.AppSettings["eStoreItEmailGroup"]);

                string error_messages = string.Empty;
                if (ctos.ErrorMessages != null && ctos.ErrorMessages.Count > 0)
                    foreach (var c in ctos.ErrorMessages)
                        error_messages += string.Format("{0}<br />", c);
                messageInfo.Add("Error_messages", error_messages);
                EmailBasicTemplate emailContext = new EmailBasicTemplate();
                emailContext.sendEmailInBasicTemplate(Presentation.eStoreContext.Current.Store, messageInfo, false, "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">Dear eStore owner,<br><br><span style=\"color:Red;\"><i><a href=\"[/ProductLink]\">[/ProductNmae]</a></i></span>  need your maintenance because selections in one or more categories are invalid. Please verify the CBOM configuration, and make proper changes. Once complete, select the appropriate status to re-publish this system again.    <span style=\"color:Red;\">&nbsp;&nbsp;<a href=\"[/OMSystemLink]\">Click here to modify CTOS settings!</a></span><br /><br /><b>Error Message:</b><br />[/Error_messages]<br><br><b> [/UserID]</b> from <b>[/UserIP]</b> is viewing this product<br><span style=\"font-size:12px\">[/UserAGENT]</span><br><br>Thanks,<br><br>eStore IT team");

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
            StringBuilder CTOSBuilder = new  StringBuilder();
            bool showParts = false;
            if (Presentation.eStoreContext.Current.User != null)
            {
                showParts = Presentation.eStoreContext.Current.User.actingUser.hasRight(eStore.POCOS.User.ACToken.ATP);
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
            Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            this.CTOSModules.Text = ctosMgr.composeBOMUI(CTOS, BTOS, ref navigator, showParts);

            this.lProductName.Text = CTOS.name;
            this.lShortDescription.Text = CTOS.productDescX;
            this.lProductFeature.Text = CTOS.productFeatures;
  
            this.YouAreHereMutli1.ProductName = CTOS.name;
            this.YouAreHereMutli1.productCategories = new List<POCOS.ProductCategory> { CTOS.GetDefalutCategory(Presentation.eStoreContext.Current.MiniSite) };
            lbfloatPrice.Text = this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(CTOS, BTOS, Presentation.Product.PriceStyle.productpriceLarge);
            if (CTOS.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION))
            {
                LtPromotionMessage.Text = string.Format("<p style=\"padding:6px 0px;\">{0}</p>", CTOS.PromoteMessage.Trim());
                LtPromotionMessage.Visible = true;
            }
           

            if (CTOS.salesMarketings.Any())
            {
                foreach (var item in CTOS.salesMarketings)
                {
                    BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(item);
                    ltproductStatus.Text += string.Format("<img src='{0}' title='{1}' alt='{1}' />", delivery.Ico, delivery.Message);
                }
            }

            this.ChangeCurrency1.defaultListPrice = CTOS.getListingPrice().value;
            //hEmail.PostBackUrl = Request.RawUrl + "#";
            
                //hEmail.Attributes.Add("onclick", "return showProductSharetoFriendsDialog();");
                //hRequestQuantityDiscount.NavigateUrl = Request.RawUrl + "#";
                hRequestQuantityDiscountTop.NavigateUrl ="~/Product/QuantityDiscountRequest.aspx";
                //hRequestQuantityDiscount.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
                hRequestQuantityDiscountTop.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
 
            QuantityDiscount._product = CTOS;
            //hPrint.PostBackUrl = "/Product/print.aspx?ProductID=" + CTOS.SProductID.ToString();

            var isBelowCost = CTOS.checkIsBelowCost(BTOS);
            this.lbl_configItemsTitle.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;

            this.btnAdd2CartTop.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnAdd2QuoteTop.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnUpdateTop.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.lbl_configItemsTitle.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            hlPreview.Visible = CTOS.ShowPrice && !CTOS.notAvailable && !isBelowCost;
            ltResources.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;

            this.btnAdd2QuoteFloat.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnAdd2CartFloat.Visible = CTOS.ShowPrice && !bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.btnUpdateFloat.Visible = CTOS.ShowPrice && bShowUpdateButton && !CTOS.notAvailable && !isBelowCost;
            this.hRequestQuantityDiscountTop.Visible = CTOS.ShowPrice && !CTOS.notAvailable && !isBelowCost;
            //this.hRequestQuantityDiscount.Visible = CTOS.ShowPrice && !CTOS.notAvailable && !isBelowCost;
            List<string> exclude = new List<string>() { "SYS-4U510-4A51EE", "SYS-4U510-4A52EE", "SYS-2U2320-4A51EE", "SYS-2U2320-4A52EE", "SYS-4U4000-4A51EE", "SYS-4U4000-4A52EE", "SYS-4U4000-4A53EE" };
            if ((Presentation.eStoreContext.Current.Store.storeID == "AEU" && exclude.Contains(CTOS.name)))
            {
                hRequestQuantityDiscountTop.Visible = false;
            }

            if (CTOS.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire))
            {
                this.btnAdd2CartTop.Visible = false;
                this.btnAdd2QuoteTop.Visible = false;
                this.btnUpdateTop.Visible = false;
                hlPreview.Visible = false; ;
                panel_system_listFloat.Visible = false;
                navigator = string.Empty;
                system_content.Visible = false;

                this.QuantityDiscount.ShoppingControl = Models.ShoppingControl.Inquire;
                lProductprice.Visible = false;
                this.hRequestQuantityDiscountTop.Text = "<span>Inquire</span>";
                this.hRequestQuantityDiscountTop.CssClass = "eStore_btn";
                if (!string.IsNullOrEmpty(CTOS.dataSheetX))
                {
                    hDatasheet.Visible = true;
                    hDatasheet.NavigateUrl = CTOS.dataSheetX;
                }

            }

            if (Presentation.eStoreContext.Current.getBooleanSetting("QuoteOnly", false))
            {
                btnAdd2CartFloat.Visible = false;
                btnAdd2CartTop.Visible = false;
                btnAdd2QuoteFloat.CssClass = "ctosneedlogin eStore_btn SystemIntegrityCheck";
                btnAdd2QuoteTop.CssClass = "ctosneedlogin eStore_btn SystemIntegrityCheck";
            }

            string defaultOptions=  Request["options"];
            if (!string.IsNullOrEmpty(defaultOptions))
            {
                RegisterArrayDeclaration("defaultOptions", $"'{string.Join("','", defaultOptions.Split(','))}'");
            }
            string softwareSubscriptionMessage = getSoftwareSubscriptionMessage(CTOS);
            if (hasSoftwareSubscriptionOption)
            {
               
                //btnAdd2CartTop.Visible = false;
                //btnAdd2CartFloat.Visible = false;
                //btnAdd2QuoteTop.CssClass = "ctosneedlogin eStore_btn SystemIntegrityCheck";
                //btnAdd2QuoteFloat.CssClass = "ctosneedlogin eStore_btn SystemIntegrityCheck";

                if (!string.IsNullOrEmpty(softwareSubscriptionMessage))
                {
                    lSoftwareSubscriptionMessage.Text = softwareSubscriptionMessage;
                    lSoftwareSubscriptionMessage.Visible = true;
                }

            }

            if (CTOS.status != POCOS.Product.PRODUCTSTATUS.PHASED_OUT && !CTOS.notAvailable && CTOS.publishStatus == POCOS.Product.PUBLISHSTATUS.PUBLISHED && !CTOS.isValid())
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
            if (Presentation.eStoreContext.Current.getBooleanSetting("TutorialEnabled"))
            {
                this.BindScript("url", "eStoreTutorial.js", "v4/eStoreTutorial.js");
                this.AddStyleSheet(ResolveUrl("~/Styles/eStoreTutorial.css"));
                string eStoreTutorial = "$(function () {     InitTutorial(\"System\", \"" + eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID + "\", \"system-tutorial-image.txt\", \".eStore_product_productPic\", \"system-tutorial-preview.txt\", \".systemBomPreview\"); });";
                this.BindScript("Script", "eStoreTutorial_Home", eStoreTutorial, true);
            }
            //for float div

            if (Presentation.eStoreContext.Current.getBooleanSetting("ShowProductWarrantyImage", true))
            {
                if (CTOS.WarrantyMonth != 0 && CTOS.WarrantyMonth != null)
                {
                    int? Month = CTOS.WarrantyMonth;
                    float? ConvertToWarrantyYear;
                    if (Month % 12 == 0)
                    {
                        ConvertToWarrantyYear = Month / 12;
                imgproductwarranty.Visible = true;
                        imgproductwarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", ConvertToWarrantyYear);
                        imgproductwarranty.ToolTip = imgproductwarranty.AlternateText = string.Format("{0} years extended warranty", ConvertToWarrantyYear);
                    }
                    else
                    {
                        if (Month == 3)
                        {
                            string str = Month.ToString() + "month";
                            imgproductwarranty.Visible = true;
                            imgproductwarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", str);
                            imgproductwarranty.ToolTip = imgproductwarranty.AlternateText = string.Format("{0} month extended warranty", Month);
                        }
                        else
                        {
                            imgproductwarranty.Visible = true;
                imgproductwarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", CTOS.defaultWarrantyYear);
                imgproductwarranty.ToolTip = imgproductwarranty.AlternateText = string.Format("{0} years extended warranty", CTOS.defaultWarrantyYear);
                        }
                    }
                }
                else
                {
                    imgproductwarranty.Visible = true;
                    imgproductwarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", CTOS.defaultWarrantyYear);
                    imgproductwarranty.ToolTip = imgproductwarranty.AlternateText = string.Format("{0} years extended warranty", CTOS.defaultWarrantyYear);
                }
            }
            else
            { imgproductwarranty.Visible = false; }

            //目前仅AJP Ctos使用 72小时到货
            //48 hours fast delivery is not available to System.  CTOS shall be 5 days.  Will put this back with right icon and message
            if (CTOS.deliveryMarketings.Any())
            {
                foreach (var item in CTOS.deliveryMarketings)
                {
                    BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(item);
                    ltFastDelivery.Text += string.Format("<img class='floatLeft' src='{1}' title='{2}' /><br /><b>Estimated Ship Date:{0}</b>"
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


        }

        private void bindSystemIntegrityCheck(Product_Ctos CTOS)
        {
            try
            {
                bool hasStorages = (from com in (List<CTOSBOM>)CTOS.components
                                    where com.CTOSComponent != null && com.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.Storage
                                    select 1).Any();
                bool hasOS = (from com in (List<CTOSBOM>)CTOS.components
                              where com.CTOSComponent != null && com.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.OS
                              select 1).Any();

                this.EnableSystemIntegrityCheck = (hasOS &&hasStorages);
            }
            catch (Exception)
            {

                this.EnableSystemIntegrityCheck = false;
            }

        }

        private bool hasSoftwareSubscriptionOption = false;
        private string getSoftwareSubscriptionMessage(Product_Ctos CTOS)
        {
            var component= (from com in (List<CTOSBOM>)CTOS.components
                         where com.CTOSComponent != null && com.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.SoftwareSubscription
                         select com
               ).FirstOrDefault();

            if (component != null)
            {
              string message = (from CTOSBOM o in component.options
                              select o.Message

                            ).FirstOrDefault();
                hasSoftwareSubscriptionOption = true;
                return message;
            }
            return string.Empty;
        }

        private BTOSystem upateBTOS(Product_Ctos CTOS)
        {
            //centralize logical
            Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            return ctosMgr.updateBTOS(CTOS, Request.Form);
        }
        protected void popLoginDialog(object sender)
        {
            if (string.IsNullOrEmpty(Request["needlogin"]))
            {
                string purpose = "";
                if (sender is  Button)
                { purpose = (sender as Button).Text; }
                else if (sender is LinkButton)
                { purpose = (sender as LinkButton).Text; }
                else if (sender is HyperLink)
                { purpose = (sender as HyperLink).Text; }

                if (Request.RawUrl.IndexOf("?") > 0)
                    Response.Redirect(Request.RawUrl + "&needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
                else
                    Response.Redirect(Request.RawUrl + "?needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
            }
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
                Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                CartItem cartitem = ctosMgr.Add2Cart(cart, _ctos, Request.Params);

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
                if (_ctos.hasMainComponent(newbtos) == false)
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
                Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                CartItem cartitem = ctosMgr.Add2Cart(quotation.cartX, _ctos, Request.Params);

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
                                    Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                                    ctosMgr.updateCart(Presentation.eStoreContext.Current.UserShoppingCart, cartitem, _ctos, Request.Params);

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
                         
                                    Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                                    ctosMgr.updateCart(Presentation.eStoreContext.Current.Quotation.cartX, cartitem, _ctos, Request.Params);

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
                              Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                              BTOS = ctosMgr.reconfigFromCart(Presentation.eStoreContext.Current.UserShoppingCart, cartitem, _ctos);
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
                                Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
                                BTOS = ctosMgr.reconfigFromCart(Presentation.eStoreContext.Current.Quotation.cartX, cartitem, _ctos);
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

        protected void lAddtoCompareList_Click1(object sender, EventArgs e)
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
            //Resources type
            StringBuilder sbframe = new StringBuilder();
            //LightBox -- small pictures
            StringBuilder sbLightBox = new StringBuilder();
            //Description -- product Resources
            StringBuilder sbLiterature = new StringBuilder();

            string defaultImage = "";
            List<POCOS.Part> specSources = new List<Part>();
            if (ctos.specSources != null)
            {
                specSources = ctos.specSources.TakeWhile(p => !p.SProductID.StartsWith("option", true, null)).ToList();
            }
            if(ctos.productResourcesX.Any())
                specSources.Insert(0, ctos); // 默认显示ctos 本身的 resource

            if (specSources.Any())
            {
          
                bool has3DModelDownloadFile;
                bool no3DModelOnlineView;
                List<POCOS.ProductResource> ctosresources = ctos.productResourcesX.ToList();
             
                sbLiterature.Append("<div class=\"eStore_title\">" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Resouces) + "[ResourceType]</div>");

                int index = 0;
                foreach (POCOS.Part _part in specSources)
                {
                    //Hardcoded logic per Russell request.  This part need to be removed once this CTOS product is phased out.
                    //String thumbnailImage = ((ctos.SProductID == "21330" || ctos.SProductID == "21336") && _part.SProductID == "HPC-7480-66A1E") ? "http://downloadt.advantech.com/download/downloadlit.aspx?LIT_ID=ba9b0918-796f-4aa0-9cba-48e0e7697b5a" : _part.TumbnailImageID;
                    String thumbnailImage = ctos.geteStoreLocalMainImage(index != 0);
                    if (string.IsNullOrEmpty(thumbnailImage))
                        thumbnailImage = _part.TumbnailImageID;
                    //ctos TumbnailImageID为null,ImageUrl有值
                    if (string.IsNullOrEmpty(thumbnailImage) && _part is POCOS.Product)
                        thumbnailImage = (_part as POCOS.Product).thumbnailImageX;

                    var item = ctos.getRootComponentBySprocutid(_part.SProductID);

                    if (index == 0)
                    {
                        if (string.IsNullOrEmpty(thumbnailImage))
                        {
                            POCOS.ProductResource defaultResource = _part.productResourcesX.FirstOrDefault(p => p.ResourceType == "LargeImages" && !string.IsNullOrEmpty(p.ResourceURL));
                            thumbnailImage = defaultResource != null ? defaultResource.ResourceURL : "";
                        }
                        //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", _part.TumbnailImageID, _part.name);
                        imgbigimage.Text = string.Format("<img src=\"{0}\" alt=\"{1}\" class=\"eStore_product_picBigImg\" />", thumbnailImage, _part.name);
                        defaultImage = thumbnailImage;
                        imgbigimage2.Text = string.Format("<img src=\"{0}\" alt=\"{1}\" />", thumbnailImage, _part.name);
                        sbLightBox.AppendFormat("<li class=\"on\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", thumbnailImage, _part.name);
                        //sbLiterature.AppendFormat("<div id=\"tab_{0}\" >", index);
                        sbframe.AppendFormat("<li class=\"on {2}\" labfor=\"{1}\">{0}</li>", item == null ? _part.name : item.ComponentName, _part.SProductID, _part.productResourcesX.Any() ? "" : "hiddenitem");
                    }
                    else
                    {
                        sbLightBox.AppendFormat(" <li><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", thumbnailImage, _part.name, index);
                        sbframe.AppendFormat("<li class=\"{2}\" labfor=\"{1}\">{0}</li>", item == null ? _part.name : item.ComponentName, _part.SProductID, _part.productResourcesX.Any() ? "" : "hiddenitem");
                    }


                    StringBuilder sbResourceTemp = new StringBuilder();
                    has3DModelDownloadFile = false;
                    no3DModelOnlineView = true;
                    foreach (POCOS.ProductResource pr in _part.productResourcesX)
                    {
                        //sbResourceTemp = new StringBuilder();
                        switch (pr.ResourceType)
                        {
                            case "LargeImages":
                                sbLightBox.AppendFormat("<li><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", pr.ResourceURL, _part.name, _part.name);
                                break;
                            case "Utilities":
                                if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                    sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('utilities', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Utilities), ctos.SProductID);
                                break;
                            case "Driver":
                                if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                    sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('driver', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Driver), ctos.SProductID);
                                break;
                            case "Datasheet":
                                string para = "?utm_source=eStore";
                                if (pr.ResourceURL.IndexOf("?") > -1)
                                    para = "&utm_source=eStore";
                                sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('datasheet', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL + para,
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet), ctos.SProductID);
                                break;
                            case "Download":
                                sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('files', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Download), ctos.SProductID);
                                break;
                            case "Manual":
                                sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\" onclick=\"return createUnicaActivity('manual', '{2}', '{0}');\">{1}</a></span>", pr.ResourceURL,
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Manual), ctos.SProductID);
                                break;
                            case "3DModelOnlineView":
                                sbResourceTemp.AppendFormat("<span><a class=\"fancybox fancybox.iframe\" href=\"{0}\">{1}</a></span>", pr.ResourceURL,
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
                        sbResourceTemp.AppendFormat("<span><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></span>",
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                           _part.SProductID);

                    if (ctos.specs != null && ctos.specs.TakeWhile(s => s.CatID != 999999).Count() > 0)
                        sbResourceTemp.AppendFormat("<span class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></span>",
                          "Extended Specifications",
                         ctos.SProductID);

                    string policyUrl = eStore.Presentation.eStoreContext.Current.Store.policylink();
                    if (!string.IsNullOrEmpty(policyUrl))
                    {
                        sbResourceTemp.AppendFormat("<span>{0}</span>", policyUrl);
                    }

                    if (sbResourceTemp.Length > 0)
                    {
                        sbResourceTemp.Insert(0, string.Format("<div id=\"RTab-{0}\" class=\"hiddenitem tab-resource\">", _part.SProductID));
                        sbResourceTemp.Append("</div>");
                    }
                    sbLiterature.Append(sbResourceTemp);

                    index++;
                }
            }
            else
            {
                imgbigimage.Text = string.Format("<img src=\"{0}\" alt=\"{1}\" class=\"eStore_product_picBigImg\" />", ctos.thumbnailImageX, ctos.name);
                defaultImage = ctos.thumbnailImageX;
                imgbigimage2.Text = string.Format("<img src=\"{0}\" alt=\"{1}\" />", ctos.thumbnailImageX, ctos.name);
                sbLightBox.AppendFormat("<li class=\"on\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", ctos.thumbnailImageX, ctos.name);
            }

            POCOS.ProductResource prsource = ctos.productResourcesX.FirstOrDefault(x => x.ResourceType == "video" && x.IsLocalResource == true);
            if (prsource != null)
            {
                string videoInfor = string.Format("<li><a href=\"{2}\" class=\"youtube\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" msrc=\"$.bgimage\" data-BG=\"$.bgimage\" /></a></li><script type=\"text/javascript\">showVideo();</script>"
                                    , (!string.IsNullOrEmpty(prsource.advertisementX.Imagefile) ? esUtilities.CommonHelper.GetStoreLocation() + "resource" + prsource.advertisementX.Imagefile : "../images/video_icon.png")
                                    , prsource.advertisementX.AlternateText
                                    , prsource.advertisementX.Hyperlink);
                videoInfor += prsource.advertisementX.htmlContentX;
                sbLightBox.Insert(0, videoInfor.Replace("$.bgimage", defaultImage));
            }

            if (sbLightBox.Length > 0)
                productLightBox.Text = sbLightBox.ToString();
            if (sbframe.Length > 0)
            {
                sbframe.Insert(0, "<div class=\"eStore_titleTab carouselBannerSingle\" id=\"eStore_ResourcesTab\"><ul>");
                sbframe.Append("</ul><div class=\"carousel-control\"><a id=\"prev\" class=\"prev\" href=\"#\"></a><a id=\"next\" class=\"next\" href=\"#\"></a></div></div>");
            }
            ltResources.Text = sbLiterature.ToString().Replace("[ResourceType]",sbframe.ToString());
        }
        
        protected void bindFonts()
        {
            txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
        }
        private POCOS.Product_Ctos getProduct()
        {
            string ProductName = esUtilities.CommonHelper.QueryString("ProductName");
            var product = Presentation.eStoreContext.Current.Store.getProduct(ProductName, true);
            base.CurrProduct = product;
            base.ProductNo = ProductName;
            POCOS.Product_Ctos ctos;
            ////get by ProductID first, if can not find it, try to use ProductName
            //if (product == null)
            //{
            //    string ProductId = esUtilities.CommonHelper.QueryString("ProductID");
            //    product = Presentation.eStoreContext.Current.Store.getProduct(ProductId, true);
            //}
            if (product != null)
            {
                if (product is Product_Ctos)
                {
                    ctos = (POCOS.Product_Ctos)product;
                    this.UserActivitLog.ProductID = ctos.SProductID;
                    this.UserActivitLog.CategoryType = ctos.productType.ToString();
                }
                else
                {
                    //如果是Product, 直接转到product页面
                    ctos = null;
                    Response.Redirect(ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product)));
                }
            }
            else
            {
                ctos = null;
                this.UserActivitLog.ProductID = ProductName;
                this.UserActivitLog.CategoryType = "ErrorPart";
                base.ToSearch();
            }
            return ctos;
        }

        //加载Certification
        protected void rptCertification_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Image image = e.Item.FindControl("imgCertification") as Image;
                Certificates c = e.Item.DataItem as Certificates;
                image.ImageUrl = c.CertificateImagePath;
                image.AlternateText = c.CertificateName;
                image.ToolTip = c.CertificateName;
                image.Attributes.Add("style", "margin-right:5px; max-width:150px; max-height:50px;");
            }
        }
    }
}