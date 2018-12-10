using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using eStore.Presentation.Product;
using eStore.POCOS;
using eStore.Presentation;
using System.Net;

namespace eStore.UI.Modules
{
    public partial class Bundle : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {

        public POCOS.Product_Bundle CurrentBundleProduct
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                RenderProduct(CurrentBundleProduct);
            bindSystemIntegrityCheck(CurrentBundleProduct);
            BindFont();
            this.setPageMeta(CurrentBundleProduct.pageTitle, CurrentBundleProduct.metaData, CurrentBundleProduct.keywords);


            this.BindScript("url", "jquery.scrollfollow", "jquery.scrollfollow.js");
            this.BindScript("url", "lightbox", "jquery.lightbox-0.5.min.js");
            this.BindScript("Script", "lightBox", "$(function() {$(\".productLargeImages a\").lightBox({maxHeight: 600,maxWidth: 800}); });");
            this.AddStyleSheet(ResolveUrl("~/Styles/jquery.lightbox-0.5.css"));

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
                                 , "+"))
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

        private void bindSystemIntegrityCheck(POCOS.Product_Bundle product)
        {
            try
            {
                string StorageID = System.Configuration.ConfigurationManager.AppSettings["StandardStorageID"];
                string OSID = System.Configuration.ConfigurationManager.AppSettings["StandardOSID"];
                var items = from pc in product.peripheralCompatiblesX
                            where StorageID == pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.catagoryID.ToString() || pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.catagoryID.ToString() == OSID
                            select pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory.catagoryID;

                this.EnableSystemIntegrityCheck = (items != null && items.Distinct().Count() >= 2);
            }
            catch (Exception)
            {

                this.EnableSystemIntegrityCheck = false;
            }
        }

        private void BindFont()
        {
            btnAdd2Cart.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart));
            btnAdd2CartTop.Text = btnAdd2Cart.Text;
            btnAdd2Quote.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation));
            btnAdd2QuoteTop.Text = btnAdd2Quote.Text;
            //hEmail.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_emailThisPage);
            //hPrint.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PrintableVersion);
            //hRequestQuantityDiscount.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount));
            hRequestQuantityDiscountTop.Text = string.Format("<span>{0}</span>", eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_RequestQuantityDiscount));
            //lAddtoCompareList.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCompareList);
            //txtComment.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Write_your_special);
        }

        private void RenderProduct(POCOS.Product product)
        {
            if (product == null)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product is not available", null, true, 404);
                return;
            }
            if (product.notAvailable)
            {
                rpBTOSConfigDetails.Visible = true;
                rpBTOSConfigDetails.DataSource = product.ReplaceProductsX;
                rpBTOSConfigDetails.DataBind();
                ltPhaseOut.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Phase_out);
                ltPhaseOut.Visible = true;
            }
            this.lProductName.Text = product.name;
            this.lShortDescription.Text = product.productDescX;
            this.lProductFeature.Text = product.productFeatures;
            this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(product, Presentation.Product.PriceStyle.productpriceLarge);
            this.YouAreHereMutli1.ProductName = product.name;
            this.YouAreHereMutli1.productCategories = new List<POCOS.ProductCategory> { product.GetDefalutCategory(Presentation.eStoreContext.Current.MiniSite) };
            if (product.isWarrantable())
                txtOrderQty.Attributes.Add("warrantyprice", product.getListingPrice().value.ToString());
            ChangeCurrency1.defaultListPrice = product.getListingPrice().value;

            //20160811 Alex/Mike:add Bundle page structured date 
            Presentation.eStoreContext.Current.keywords.Add("ProductID", product.SProductID);
            eStore.Presentation.StructuredDataMarkup structuredDataMarkup = new eStore.Presentation.StructuredDataMarkup();
            structuredDataMarkup.GenerateProductStruturedData(product, this.Page);
            structuredDataMarkup.GenerateBreadcrumbStruturedData(product, this.Page);

            Presentation.eStoreContext.Current.BusinessGroup = product.businessGroup;
            if (product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION) && !string.IsNullOrEmpty(product.PromoteMessage))
            {
                LtPromotionMessage.Text = string.Format("{0}", product.PromoteMessage.Trim());
                LtPromotionMessage.Visible = true;
            }
            string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(product, false);
            if (!string.IsNullOrEmpty(LimitedresourceStr))
            {
                this.txtOrderQty.Attributes.Add("resource", LimitedresourceStr);
            }

            //OrderInfomation
            this.lblOrderPartNo.Text = product.name;
            this.lblOrderDesc.Text = product.productDescX;

            if (product.salesMarketings.Any())
            {
                foreach (var item in product.salesMarketings)
                {
                    BusinessModules.ProductDelivery delivery = eStore.Presentation.ProductDeliveryBss.getDeliveryInfor(item);
                    ltproductStatus.Text += string.Format("<img src='{0}' title='{1}' alt='{1}' />", delivery.Ico, delivery.Message);
                }
            }

            //hEmail.NavigateUrl = Request.RawUrl + "#";
            if (Presentation.eStoreContext.Current.User != null)
            {
                //hEmail.Attributes.Add("onclick", "return showProductSharetoFriendsDialog();");
                //hRequestQuantityDiscount.NavigateUrl = Request.RawUrl + "#";
                hRequestQuantityDiscountTop.NavigateUrl = Request.RawUrl + "#";
                //hRequestQuantityDiscount.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");
                hRequestQuantityDiscountTop.Attributes.Add("onclick", "return showQuantityDiscountRequestDialog();");

            }
            QuantityDiscount._product = product;

            //hPrint.NavigateUrl = "/Product/print.aspx?ProductID=" + product.SProductID.ToString();

            var isBelowCost = product.isBelowCost;

            this.btnAdd2Cart.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            this.btnAdd2Quote.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            this.btnAdd2CartTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            this.btnAdd2QuoteTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            //this.hRequestQuantityDiscount.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;
            this.hRequestQuantityDiscountTop.Visible = product.ShowPrice && !product.notAvailable && !isBelowCost;



            this.atpdateItem.Visible = ShowATP;
            this.atpqtyheader.Visible = ShowATP;
            this.atpqtyitem.Visible = ShowATP;
            this.atpdateheader.Visible = ShowATP;
            if (ShowATP)
            {
                this.lblOrderAvaliability.Text = eStore.Presentation.eStoreLocalization.Date(product.atp.availableDate);
                this.lblOrderSapQty.Text = product.atp.availableQty.ToString();
            }

            this.renderProductResource(product);
            if (this.CurrentBundleProduct.bundle != null && this.CurrentBundleProduct.bundle.BundleItems.Any())
            {

                this.rpBundletems.DataSource = this.CurrentBundleProduct.bundle.BundleItems;
                this.rpBundletems.DataBind();
            }


            if (CurrentBundleProduct.peripheralCompatiblesX != null && CurrentBundleProduct.peripheralCompatiblesX.Count > 0)
            {
                IEnumerable<PeripheralCompatible> PeripheralCompatibles = CurrentBundleProduct.peripheralCompatiblesX;
                this.rpPeripheralCompatibles.DataSource = (from pc in PeripheralCompatibles
                                                           where pc.PeripheralProduct.Publish == true
                                                           group pc.PeripheralProduct by pc.PeripheralProduct.PeripheralsubCatagory.PeripheralCatagory
                                                               into _peripheralProduct
                                                           select
                                                           new PerpherailProductXX
                                                           {
                                                               ID = _peripheralProduct.Key.catagoryID,
                                                               CategoryName = _peripheralProduct.Key.name,
                                                               peripheralProduct = _peripheralProduct.Where(x => x.partIsOrderable == true).ToList(),
                                                               seq = _peripheralProduct.Key.seq
                                                           }).OrderBy(c => c.seq).ToList();
            }
            if (ShowATP)
            {
                //acquire ATP value of relatedProducts
                POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
                Dictionary<Part, int> updatingList = new Dictionary<Part, int>();
                foreach (PeripheralCompatible pc in CurrentBundleProduct.peripheralCompatiblesX)
                {
                    foreach (var itemPart in pc.PeripheralProduct.partsX)
                    {
                        if (itemPart != null && !updatingList.ContainsKey(itemPart))
                            updatingList.Add(itemPart, 1);
                    }
                }

                foreach (RelatedProduct rp in product.RelatedProductsX)
                {
                    if (!updatingList.ContainsKey(rp.RelatedPart))
                        updatingList.Add(rp.RelatedPart, 1);
                }

                //invoke PartHelper to update ATP information of the parts listed in updatingList
                helper.setATPs(Presentation.eStoreContext.Current.Store.profile, updatingList);
            }

            this.rpPeripheralCompatibles.DataBind();
            this.rpPeripheralCompatibles.Visible = true;



            if (Presentation.eStoreContext.Current.getBooleanSetting("ShowProductWarrantyImage", true))
            {
                //product.WarrantyMonth
                //imgproductwarranty.Visible = true;
                //imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", product.defaultWarrantyYear);
                //imgWarranty.AlternateText = string.Format("{0} years extended warranty", product.defaultWarrantyYear);

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
                    //product.WarrantyMonth
                    imgproductwarranty.Visible = true;
                    imgWarranty.ImageUrl = string.Format("~/images/productwarranty{0}.jpg", product.defaultWarrantyYear);
                    imgWarranty.AlternateText = string.Format("{0} years extended warranty", product.defaultWarrantyYear);
                }


                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("eStore_linkWarrantyPolicy"))
                {
                    imgproductwarranty.HRef = "/go/WarrantyPolicy";
                    imgproductwarranty.Target = "_blank";
                }
            }
            else
            { imgproductwarranty.Visible = false; }

            if (product.deliveryMarketings.Any())
            {
                foreach (var item in product.deliveryMarketings)
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

        protected void rpPeripheralCompatibles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rpPeripheralProducts = e.Item.FindControl("rpPeripheralProducts") as Repeater;
                PerpherailProductXX pc = e.Item.DataItem as PerpherailProductXX;
                rpPeripheralProducts.DataSource = pc.peripheralProduct.OrderBy(c => c.SProductID).ToList();
                rpPeripheralProducts.DataBind();
            }
        }

        protected void rpBundletems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                BundleItem bundleItem = e.Item.DataItem as BundleItem;
                TextBox hdBundletemQTY = e.Item.FindControl("hdBundletemQTY") as TextBox;
                if (bundleItem != null && bundleItem.part != null)
                {
                    hdBundletemQTY.Text = bundleItem.Qty.HasValue ? bundleItem.Qty.Value.ToString() : "1";

                    hdBundletemQTY.Attributes.Add("configQty", hdBundletemQTY.Text);
                    hdBundletemQTY.Attributes.Add("sproductid", bundleItem.ItemSProductID);
                    string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(bundleItem.part);
                    if (!string.IsNullOrEmpty(LimitedresourceStr))
                        hdBundletemQTY.Attributes.Add("resource", LimitedresourceStr);
                    //if (bundleItem.part != null && bundleItem.part.isWarrantable())
                    //    hdBundletemQTY.Attributes.Add("warrantyprice", bundleItem.part.getListingPrice().value.ToString());
                    if (!string.IsNullOrEmpty(bundleItem.part.dependencyPartStr))
                        hdBundletemQTY.Attributes.Add("dependency", bundleItem.part.dependencyPartStr);
                    if (bundleItem.part.hasBuiltInStorage())
                        hdBundletemQTY.Attributes.Add("hasBuiltInStorage", "true");
                    if (bundleItem.part.MininumnOrderQty != null)
                        hdBundletemQTY.Attributes.Add("MOQ", bundleItem.part.MininumnOrderQty.ToString());
                }
            }
        }

        protected void rpPeripheralProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                PeripheralProduct _peripheralProduct = e.Item.DataItem as PeripheralProduct;
                //TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
                CheckBox ckbSelected = e.Item.FindControl("ckbSelected") as CheckBox;
                if (_peripheralProduct != null && _peripheralProduct.partsX != null && _peripheralProduct.partsX.Count > 0)
                {
                    ckbSelected.InputAttributes.Add("sproductid", _peripheralProduct.SProductID);
                    string LimitedresourceStr = Presentation.Product.ProductResource.getJsonResourceSetting(_peripheralProduct.partsX);
                    if (!string.IsNullOrEmpty(LimitedresourceStr))
                        ckbSelected.InputAttributes.Add("resource", LimitedresourceStr);
                    if (_peripheralProduct.partsX != null && _peripheralProduct.partsX.Count > 0 && _peripheralProduct.isWarrantable)
                        ckbSelected.InputAttributes.Add("warrantyprice", _peripheralProduct.getWarrantPrice.ToString());
                    if (!string.IsNullOrEmpty(_peripheralProduct.dependencyPartStr))
                        ckbSelected.InputAttributes.Add("dependency", _peripheralProduct.dependencyPartStr);
                    if (_peripheralProduct.hasBuiltInStorage)
                        ckbSelected.InputAttributes.Add("hasBuiltInStorage", "true");
                    ckbSelected.InputAttributes.Add("value", _peripheralProduct.qtyX.ToString());
                }
            }
        }

        protected void btnAdd2Cart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
            Add2Cart(cart);
            cart.save();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            this.Response.Redirect("~/Cart/Cart.aspx");

        }
        protected void btnAdd2Quote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
            if (quotation.statusX != POCOS.Quotation.QStatus.Open)
            {
                quotation = eStoreContext.Current.User.actingUser.openingQuote;
                Presentation.eStoreContext.Current.Quotation = quotation;
            }
            Add2Cart(quotation.cartX);
            quotation.save();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        void Add2Cart(POCOS.Cart cart)
        {
            List<BundleItem> sbcDetails = new List<BundleItem>();

            CartItem cartitem = null;
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
                            //TextBox txtQty = (TextBox)ppItem.FindControl("txtQty");
                            CheckBox ckbSelected = ppItem.FindControl("ckbSelected") as CheckBox;
                            HiddenField hSProductID = (HiddenField)ppItem.FindControl("hSProductID");
                            if (ckbSelected.Checked)
                            {


                                string comSPproductID = hSProductID.Value;

                                PeripheralProduct pp = (from peripheralCompatibles in CurrentBundleProduct.peripheralCompatiblesX
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
                                                sbcDetails.Add(new BundleItem(itemPart, pp.qtyX, 1, 0m, peripheralProducts, peripheralCatagoryName));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            Part warranty = null;
            if (rblWarranty.Items.Count > 0 && !string.IsNullOrEmpty(rblWarranty.SelectedValue))
                warranty = Presentation.eStoreContext.Current.Store.getPart(rblWarranty.SelectedValue);

            if (sbcDetails.Count > 0)
            {
                BundleItem assemblyitem = null;
                if (CurrentBundleProduct.AssemblyProduct != null && CurrentBundleProduct.PeripheralCompatibles.Count == 0)
                {
                    //把warranty 加到cartItem 中.
                    cartitem = cart.addItem(CurrentBundleProduct, qty, null, 0, null, false, 0, true, warranty);
                    assemblyitem = cartitem.bundleX.BundleItems.FirstOrDefault(x => x.ItemSProductID == CurrentBundleProduct.AssemblyProduct.SProductID);
                    if (assemblyitem != null)
                        sbcDetails.Insert(0, new BundleItem(CurrentBundleProduct.AssemblyProduct, 1, 1, assemblyitem.adjustedPrice, "", "Base"));
                }
                else
                {
                    //添加bundle 自身去BundleItem
                    sbcDetails.Insert(0, new BundleItem(CurrentBundleProduct, 1, 1, 0, "", "Base"));
                }

                POCOS.Product SBCBtos;
                if (sbcDetails.FirstOrDefault(b => b.part.isOS() == true) != null)
                    SBCBtos = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO-OS");
                else
                    SBCBtos = Presentation.eStoreContext.Current.Store.getProduct("SBC-BTO");

                Product_Ctos bundleCTOS = null;
                if (SBCBtos is Product_Ctos)
                    bundleCTOS = (Product_Ctos)SBCBtos;
                BTOSystem btos = bundleCTOS.getDefaultBTOS();
                btos.addNoneCTOSBundle(sbcDetails, 1);
                btos.initPartReferences();


                if (assemblyitem != null)
                {
                    assemblyitem.ItemSProductID = bundleCTOS.SProductID;
                    assemblyitem.part = bundleCTOS;
                    assemblyitem.BTOSystem = btos;
                    assemblyitem.AdjustedPrice = btos.Price;
                }
                else
                    //模仿product 添加
                    cartitem = cart.addItem(bundleCTOS, qty, btos, 0, null, false, 0, true, warranty);

                cart.reconcile();
            }
            else
            {
                //把warranty 加到cartItem 中.
                cartitem = cart.addItem(CurrentBundleProduct, qty, null, 0, null, false, 0, true, warranty);
            }

            //if (cartitem != null && !string.IsNullOrEmpty(this.txtComment.Text) && txtComment.Text.Trim() != this.txtComment.ToolTip)
            //    cartitem.CustomerMessage = this.txtComment.Text;

            //cart.save();
        }
        protected string getPeripheralProductPrice(POCOS.Part part)
        {
            if (part.isOS())
                //return eStore.Presentation.eStoreLocalization.Tanslation(Store.TranslationKey.Add_for_price);
                return "Add for price";
            else
                return eStore.Presentation.Product.ProductPrice.FormartPrice((decimal)part.getListingPrice().value);
        }
        protected void lAddtoCompareList_Click(object sender, EventArgs e)
        {

            if (CurrentBundleProduct != null)
            {
                ProductCompareManagement.AddProductToCompareList(CurrentBundleProduct.SProductID);
                Response.Redirect("~/Compare.aspx");
            }
            else
                Response.Redirect("~/");
        }

        private void renderProductResource(POCOS.Product part)
        {
            //Small Picture
            StringBuilder sbframe = new StringBuilder();

            //LightBox
            StringBuilder sbLightBox = new StringBuilder();
            //Description
            StringBuilder sbLiterature = new StringBuilder();

            bool has3DModelDownloadFile;
            bool no3DModelOnlineView;
            //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", part.TumbnailImageID, part.SProductID);            

            var partList = new List<Part>();
            //partList.Add(CurrentBundleProduct); // show resources
            if (CurrentBundleProduct.bundle != null && CurrentBundleProduct.bundle.BundleItems.Any())
                partList.AddRange(CurrentBundleProduct.specSources);

            partList.Insert(0, CurrentBundleProduct);

            int index = 0;
            foreach (POCOS.Part _part in partList)
            {
                // 第一张图片拿 bundle reource的eStoreLocalMainImage
                String thumbnailImage = CurrentBundleProduct.geteStoreLocalMainImage(index != 0);
                if (string.IsNullOrEmpty(thumbnailImage))
                    thumbnailImage = _part.thumbnailImageX;
                string thumbnailImageAlt = _part.name;

                if (index == 0)
                {
                    //bundle 默认上传的图片
                    if (string.IsNullOrEmpty(thumbnailImage) && !string.IsNullOrEmpty(CurrentBundleProduct.thumbnailImageX))
                    {
                        thumbnailImage = CurrentBundleProduct.thumbnailImageX;
                        thumbnailImageAlt = CurrentBundleProduct.DisplayPartno;
                    }
                    //如果图片为空, 那就走resource里面默认拿一个图片
                    if (string.IsNullOrEmpty(thumbnailImage))
                    {
                        POCOS.ProductResource defaultResource = _part.productResourcesX.FirstOrDefault(p => p.ResourceType == "LargeImages" && !string.IsNullOrEmpty(p.ResourceURL));
                        thumbnailImage = defaultResource != null ? defaultResource.ResourceURL : "";
                    }
                    //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", _part.TumbnailImageID, _part.name);

                    imgLargePicture.ImageUrl = imgLargePicturelarg.ImageUrl = thumbnailImage.Replace("~/resource", "/resource");
                    imgLargePicture.ToolTip = imgLargePicturelarg.ToolTip = thumbnailImageAlt;

                    sbLightBox.AppendFormat("<li class=\"on\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", thumbnailImage, _part.SProductID);
                    sbframe.AppendFormat("<li class=\"on\" labfor=\"{0}\">{0}</li>", _part.SProductID);
                }
                else
                {
                    if (string.IsNullOrEmpty(thumbnailImage))
                    {
                        POCOS.ProductResource defaultResource = _part.productResourcesX.FirstOrDefault(p => p.ResourceType == "LargeImages" && !string.IsNullOrEmpty(p.ResourceURL));
                        thumbnailImage = defaultResource != null ? defaultResource.ResourceURL : "";
                    }
                    //sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\" class=\"ui-tabs-hide\"  width=\"150px\" />",thumbnailImage, _part.name);
                    sbLightBox.AppendFormat("<li><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", thumbnailImage, _part.SProductID);
                    sbframe.AppendFormat("<li labfor=\"{0}\">{0}</li>", _part.SProductID);
                }

                //sbframe.AppendFormat(" <li><a href=\"#tab_{2}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", thumbnailImage.Replace("~/resource", "/resource"), thumbnailImageAlt, index);
                if (sbLiterature.Length == 0)
                    sbLiterature.Append("<div class=\"eStore_title\">" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Resouces) + "[ResourceType]</div>");
                has3DModelDownloadFile = false;
                no3DModelOnlineView = true;
                StringBuilder sbResourceTemp = new StringBuilder();
                foreach (POCOS.ProductResource pr in _part.productResourcesX)
                {

                    switch (pr.ResourceType)
                    {
                        case "LargeImages":
                            sbLightBox.AppendFormat("<li><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" data-BG=\"{0}\" /></li>", pr.ResourceURL, part.name, part.name);
                            break;
                        case "Utilities":
                            if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></span>", pr.ResourceURL,
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Utilities));
                            break;
                        case "Driver":
                            if (!(_part.specs != null && _part.specs.Where(s => s.AttrValueName == "ICC").Count() > 0))
                                sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></span>", pr.ResourceURL,
                                    eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Driver));
                            break;
                        case "Datasheet":
                            sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></span>", pr.ResourceURL,
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet));
                            break;
                        case "Download":
                            sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></span>", pr.ResourceURL,
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Download));
                            break;
                        case "Manual":
                            sbResourceTemp.AppendFormat("<span><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></span>", pr.ResourceURL,
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
                    sbResourceTemp.AppendFormat("<span><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></span>",
                        eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                       _part.SProductID);
                if (part is POCOS.Product && part.specs != null && part.specs.Count > 0)
                    sbResourceTemp.AppendFormat("<span class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></span>",
                      eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Extended_Specifications),
                     part.SProductID);

                string policyUrl = string.Empty;
                if (part.isPStoreProduct())
                    policyUrl = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile).policylink();
                else
                    policyUrl = eStore.Presentation.eStoreContext.Current.Store.policylink();
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
            POCOS.ProductResource prsource = CurrentBundleProduct.productResourcesX.FirstOrDefault(x => x.ResourceType == "video" && x.IsLocalResource == true);
            if (prsource != null)
            {
                string videoInfor = string.Format("<li><a href=\"{2}\" class=\"youtube\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" msrc=\"$.bgimage\" data-BG=\"$.bgimage\" /></a></li><script type=\"text/javascript\">showVideo();</script>"
                                    , (!string.IsNullOrEmpty(prsource.advertisementX.Imagefile) ? esUtilities.CommonHelper.GetStoreLocation() + "resource" + prsource.advertisementX.Imagefile : "../images/video_icon.png")
                                    , prsource.advertisementX.AlternateText
                                    , prsource.advertisementX.Hyperlink);
                videoInfor += prsource.advertisementX.htmlContentX;
                sbLightBox.Insert(0, videoInfor.Replace("$.bgimage", imgLargePicture.ImageUrl));
            }

            if (sbLightBox.Length > 0)
                productLightBox.Text = sbLightBox.ToString();

            if (sbframe.Length > 0)
            {
                sbframe.Insert(0, "<div class=\"eStore_titleTab carouselBannerSingle\" id=\"eStore_ResourcesTab\"><ul>");
                sbframe.Append("</ul><div class=\"carousel-control\"><a id=\"prev-eStore_ResourcesTab\" class=\"prev\" href=\"#\"></a><a id=\"next-eStore_ResourcesTab\" class=\"next\" href=\"#\"></a></div></div>");
            }

            productresources.Text = sbLiterature.ToString().Replace("[ResourceType]", sbframe.ToString());
        }
    }
}