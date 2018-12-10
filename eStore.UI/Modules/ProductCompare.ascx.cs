using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.Product;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Modules
{
    public partial class ProductCompare : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool btnInPriceCell = false;
        public int proCount = 0;

        private Presentation.eStoreControls.PagingModeType _pagingModeType = Presentation.eStoreControls.PagingModeType.QueryString;
        public Presentation.eStoreControls.PagingModeType PagingModeType
        {
            get { return _pagingModeType; }
            set { _pagingModeType = value; }
        }
        //默认是 false.
        public Boolean IsLoad { get; set; }
        public Boolean showRemoveButton { get; set; }
        public Boolean showPrintButton { get; set; }
        public Boolean showHeaderButtons { get; set; }
        private  bool _useHtmlControlForAdd2Cart=false;
        /// <summary>
        /// this property need set true and add script as below for the page when using proc.html
        ///                 eStore.UI.eStoreScripts.addProducttoCart(product.ProductID, 1
        ///                    , function (result) {
        ///                         if (result) { location = location; }
        ///                  });
        /// </summary>
        public bool  useHtmlControlForAdd2Cart
        {
            get { return _useHtmlControlForAdd2Cart; }
            set { _useHtmlControlForAdd2Cart = value; }
    }
        public IList<POCOS.Product> compareProducts { get; set; }
        public List<POCOS.CTOSSpecMask> CTOSSpecMask { get; set; }

        protected override void OnInit(EventArgs e)
        {
            this.CollectionPager1.PagingMode = PagingModeType;
            this.EnsureChildControls();
            base.OnInit(e);
   }

        protected override void CreateChildControls()
        {
            proCount = eStore.Presentation.eStoreContext.Current.getIntegerSetting("proCompareCount");
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.GenerateCompareTable();
                base.ChildControlsCreated = true;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.CollectionPager1.Visible)
                toppager.Text = string.Format("<div class=\"rightside\">{0}</div>", CollectionPager1.RenderedHtml);
            else
                toppager.Text = string.Empty;
            base.Render(writer);
        }
        private void btnRemoveFromList_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                ProductCompareManagement.RemoveProductFromCompareList(e.CommandArgument.ToString());
                Page.Response.Redirect("~/Compare.aspx");
            }
        }
        private void btnAddtoCart_Command(object sender, CommandEventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            if (e.CommandName == "AddtoCart")
            {
                List<POCOS.Product> selectProductList =  new List<POCOS.Product>();
                POCOS.Product prod = Presentation.eStoreContext.Current.Store.getProduct(e.CommandArgument.ToString());
                selectProductList.Add(prod);
                QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
                qcc.AddToCart();
                //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
                //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
                //else
                this.Response.Redirect("~/Cart/Cart.aspx");
            }
        }

        private void btnAddtoQuote_Command(object sender, CommandEventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            if (e.CommandName == "AddtoQuote")
            {
                List<POCOS.Product> selectProductList = new List<POCOS.Product>();
                POCOS.Product prod = Presentation.eStoreContext.Current.Store.getProduct(e.CommandArgument.ToString());
                selectProductList.Add(prod);
                QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
                qcc.AddToQuote();
                this.Response.Redirect("~/Quotation/Quote.aspx");
            }
        }
        private HtmlTableCell AddCell(HtmlTableRow row, string text)
        {
            var cell = new HtmlTableCell();
            cell.InnerHtml = text;
            row.Cells.Add(cell);
            return cell;
        }
        private HtmlTableCell AddHeaderCell(HtmlTableRow row, string text)
        {
            var cell = new HtmlTableCell("th");
            cell.InnerHtml = text;
            row.Cells.Add(cell);
            return cell;
        }
        public void GenerateCompareTable(int pageCount = 0)
        {
            if (!IsLoad)//false就加载
            {
                if (eStore.Presentation.eStoreContext.Current.MiniSite != null && 
                        (eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.IotMart || eStore.Presentation.eStoreContext.Current.MiniSite.MiniSiteType == POCOS.MiniSite.SiteType.UShop))
                    btnInPriceCell = true;

                this.tblCompareProducts.Rows.Clear();
                this.tblCompareProducts.Width = "100%";
                if (compareProducts != null)
                { }
                else if (!string.IsNullOrEmpty(Request["category"]))
                {
                    compareProducts = ProductCompareManagement.GetCompareProductsByCategoryID();

                    showRemoveButton = false;
                }
                else if (!string.IsNullOrEmpty(Request["parts"]))
                {
                    compareProducts = ProductCompareManagement.GetCompareProductsFromQueryString();

                    showRemoveButton = false;
                }
                else if (!string.IsNullOrEmpty(Request["partno"]))
                {
                    compareProducts = ProductCompareManagement.GetCompareSystemsByComponent();

                    showRemoveButton = false;
                }
                else
                {
                    compareProducts = ProductCompareManagement.GetCompareProducts();
                    showRemoveButton = showRemoveButton && true;
                }

                if (proCount > 0)
                    compareProducts = compareProducts.Take(proCount).ToList();

                bindCollectionPagerFonts();
                if (compareProducts.Count > this.CollectionPager1.PageSize)
                {
                    CollectionPager1.DataSource = compareProducts;

                    int pi = 1;
                    if (!string.IsNullOrEmpty(Request["Page"]))
                    {
                        int.TryParse(Request["Page"], out pi);
                    }
                    else if (pageCount > 0)//如果有post page, 赋值
                    {
                        //this.CollectionPager1.ChangePage(pageCount);
                        //pi = this.CollectionPager1.CurrentPage;
                        pi = pageCount;
                    }
                    if (pi > compareProducts.Count / this.CollectionPager1.PageSize + 1 || pi < 1)
                        pi = 1;
                    int ifrom = (pi - 1) * CollectionPager1.PageSize;
                    var _compareProducts = compareProducts.Skip(ifrom).Take(CollectionPager1.PageSize);
                    compareProducts = _compareProducts.ToList<POCOS.Product>();
                }

                if (compareProducts.Count > 0)
                {
                    var headerRow = new HtmlTableRow();
                    this.AddHeaderCell(headerRow, "&nbsp;");
                    var productNameRow = new HtmlTableRow();
                    this.AddHeaderCell(productNameRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Name));

                    var priceRow = new HtmlTableRow();
                    this.AddHeaderCell(priceRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price));
                    var datasheetRow = new HtmlTableRow();
                    this.AddHeaderCell(datasheetRow, "Datasheet");
                    var ImageRow = new HtmlTableRow();
                    this.AddHeaderCell(ImageRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Image));

                    //var DescRow = new HtmlTableRow();
                    //this.AddHeaderCell(DescRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Description));

                    var ShortFeatureRow = new HtmlTableRow();
                    this.AddHeaderCell(ShortFeatureRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Feature));
                    bool showShortFeature = true;
                    foreach (var product in compareProducts)
                    {

                        var headerCell = new HtmlTableCell();
                        var headerCellDiv = new HtmlGenericControl("div");
                        var priceCell = new HtmlTableCell();
                        Literal ltprice = new Literal();
                        ltprice.Text = Presentation.Product.ProductPrice.getSimplePrice(product);
                        priceCell.Controls.Add(ltprice);

                        var btnproductCustomize = new Button();
                        var btnAddtoCart = new Button();
                        var btnAddtoQuote = new Button();


                        if (useHtmlControlForAdd2Cart)
                        {
                            System.Text.StringBuilder sbHeaderCellInnerHtml = new System.Text.StringBuilder();
                            if (product is POCOS.Product_Ctos)
                            {

                                sbHeaderCellInnerHtml.AppendFormat("<a class=\"storeBlueButton\" style=\"color:white;padding-bottom: 4px;\" href=\"{0}\">{1}</a>"
                                    , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product))
                                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Customize_it));

                            }
                            else if (product.getListingPrice().value > 0)
                            {

                                sbHeaderCellInnerHtml.AppendFormat("<img src=\"{2}\" onclick=\"add2cart('{0}',1)\" style=\"CURSOR: pointer;\" title=\"{1}\" class=\"needlogin\">"
                                  , product.SProductID
                                  , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart)
                                  , ResolveUrl("~/images/icon_addcart.png")
                                  );
                                sbHeaderCellInnerHtml.AppendFormat(" <img src=\"{2}\"  onclick=\"add2quote('{0}',1)\"  style=\"CURSOR: pointer;\" title=\"{1}\" class=\"needlogin\">"
                                , product.SProductID
                                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation)
                                 , ResolveUrl("~/images/icon_addquote.png")
                                );



                            }
                            if (sbHeaderCellInnerHtml.Length > 0)
                                headerCellDiv.InnerHtml = sbHeaderCellInnerHtml.ToString();
                        }
                        else
                        {
                            if (showRemoveButton)
                            {
                                var btnRemoveFromList = new Button();
                                btnRemoveFromList.CssClass = "btnEeleteStyle";
                                btnRemoveFromList.ToolTip = GetLocaleResourceString(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Remove));
                                btnRemoveFromList.Text = GetLocaleResourceString(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Remove));
                                btnRemoveFromList.CommandName = "Remove";
                                btnRemoveFromList.Command += new CommandEventHandler(this.btnRemoveFromList_Command);
                                btnRemoveFromList.CommandArgument = product.SProductID.ToString();
                                btnRemoveFromList.CausesValidation = false;

                                btnRemoveFromList.ID = "btnRemoveFromList" + product.SProductID.ToString();

                                headerCellDiv.Controls.Add(btnRemoveFromList);
                            }
                            if (product is POCOS.Product_Ctos &&!btnInPriceCell)
                            {
                                btnproductCustomize.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Customize_it);
                                btnproductCustomize.PostBackUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
                                btnproductCustomize.CssClass = "storeBlueButton";
                                if (!btnInPriceCell)
                                    headerCellDiv.Controls.Add(btnproductCustomize);
                                else
                                    priceCell.Controls.Add(btnproductCustomize);
                            }
                            else if (product.getListingPrice().value > 0)
                            {
                                btnAddtoCart.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
                                btnAddtoCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
                                btnAddtoCart.CommandName = "AddtoCart";
                                btnAddtoCart.CssClass = "needlogin btnStyle";
                                btnAddtoCart.Command += new CommandEventHandler(this.btnAddtoCart_Command);
                                btnAddtoCart.CommandArgument = product.SProductID.ToString();
                                btnAddtoCart.CausesValidation = false;

                                btnAddtoCart.ID = "btnAddtoCart" + product.SProductID.ToString();
                                if (!btnInPriceCell)
                                    headerCellDiv.Controls.Add(btnAddtoCart);
                                else
                                    priceCell.Controls.Add(btnAddtoCart);


                                btnAddtoQuote.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
                                btnAddtoQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
                                btnAddtoQuote.CommandName = "AddtoQuote";
                                btnAddtoQuote.CssClass = "needlogin btnStyle";
                                btnAddtoQuote.Command += new CommandEventHandler(this.btnAddtoQuote_Command);
                                btnAddtoQuote.CommandArgument = product.SProductID.ToString();
                                btnAddtoQuote.CausesValidation = false;

                                btnAddtoQuote.ID = "btnAddtoQuote" + product.SProductID.ToString();
                                if (!btnInPriceCell)
                                    headerCellDiv.Controls.Add(btnAddtoQuote);
                                else
                                    priceCell.Controls.Add(btnAddtoQuote);

                            }
                        }

                        headerCell.Align = "center";
                        headerCell.Controls.Add(headerCellDiv);
                        headerRow.Cells.Add(headerCell);

                        var productImage = new HtmlImage();
                        productImage.Border = 0;
                        productImage.Width = 125;
                        var productImageLink = new HyperLink();


                        productImageLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
                        //productImage.Align = "center";
                        productImage.Alt = "Product image";
                        string productPicture = product.thumbnailImageX;
                        if (!string.IsNullOrEmpty(productPicture))
                            productImage.Src = productPicture;
                        else
                            productImage.Src = "";
                        productImageLink.Controls.Add(productImage);
                        var ImageCell = new HtmlTableCell();
                        ImageCell.Controls.Add(productImageLink);
                        ImageCell.Align = "center";
                        ImageRow.Cells.Add(ImageCell);


                        //product datasheet
                        HyperLink productdatasheetLink = null;
                        if (product is POCOS.Product_Ctos)
                        {
                            POCOS.Product_Ctos ctos = (POCOS.Product_Ctos)product;
                            if (!string.IsNullOrEmpty(ctos.dataSheetX))
                            {
                                productdatasheetLink = new HyperLink();
                                productdatasheetLink.Text = "Datasheet";
                                productdatasheetLink.NavigateUrl = ctos.dataSheetX;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(product.dataSheetX))
                            {
                                productdatasheetLink = new HyperLink();
                                productdatasheetLink.Text = "Datasheet";
                                productdatasheetLink.NavigateUrl = product.dataSheetX;
                            }
                        }
                        if (productdatasheetLink != null && !string.IsNullOrEmpty(productdatasheetLink.NavigateUrl))
                        {
                            var datasheetRowCell = new HtmlTableCell();
                            productdatasheetLink.Target = "_blank";
                            datasheetRowCell.Controls.Add(productdatasheetLink);
                            datasheetRowCell.Align = "center";
                            datasheetRow.Cells.Add(datasheetRowCell);
                        }
                        else
                        {
                            var datasheetRowCell = new HtmlTableCell();
                            datasheetRowCell.InnerText = "-";
                            datasheetRowCell.Align = "center";
                            datasheetRow.Cells.Add(datasheetRowCell);
                        }
                        var productNameCell = new HtmlTableCell();
                        productNameCell.Align = "center";
                        var productLink = new HyperLink();
                        productLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
                        productLink.Text = Server.HtmlEncode(product.name + " ");
                        productLink.Attributes.Add("class", "link");
                        productNameCell.Controls.Add(productLink);

                        HtmlImage productstatus = new HtmlImage();
                        productstatus.Src = string.Format("{1}images/{0}.gif", product.status.ToString(), esUtilities.CommonHelper.GetStoreLocation());
                        productNameCell.Controls.Add(productstatus);
                        productNameRow.Cells.Add(productNameCell);
                        
                        priceCell.Align = "center";
                        

                        priceRow.Cells.Add(priceCell);

                        var DescCell = new HtmlTableCell();
                        DescCell.Align = "left";
                        DescCell.InnerHtml = product.productDescX;
                        // DescRow.Cells.Add(DescCell);


                        var ShortFeatureCell = new HtmlTableCell();
                        ShortFeatureCell.Align = "left";
                        ShortFeatureCell.VAlign = "top";

                        //this restriction shall apply only to US store
                        if ((product.name.ToLower().StartsWith("sys-") || product.name.ToLower().StartsWith("aimc-")) && product.StoreID == "AUS")
                        {
                            ShortFeatureCell.InnerText = string.Empty;
                            showShortFeature = false;
                        }
                        else
                        {
                            ShortFeatureCell.InnerHtml = string.Format("<ul>{0}</ul>", product is POCOS.Product_Ctos ? ((POCOS.Product_Ctos)product).ShortFeatures : product.productFeatures);

                        }
                        ShortFeatureRow.Cells.Add(ShortFeatureCell);


                    }
                    productNameRow.Attributes.Add("class", "product-name");
                    if (showHeaderButtons)
                        this.tblCompareProducts.Rows.Add(headerRow);

                    this.tblCompareProducts.Rows.Add(productNameRow);
                    this.tblCompareProducts.Rows.Add(ImageRow);

                    this.tblCompareProducts.Rows.Add(priceRow);
                    this.tblCompareProducts.Rows.Add(datasheetRow);
                    // this.tblCompareProducts.Rows.Add(DescRow);
                    if (showShortFeature && Presentation.eStoreContext.Current.getBooleanSetting("showShortFeature", true))
                        this.tblCompareProducts.Rows.Add(ShortFeatureRow);


                    var specificationAttributes = from spec in compareProducts[0].specs
                                                  select new
                                                  {
                                                      CatID = spec.CatID,
                                                      ID = spec.AttrID,
                                                      Name = spec.LocalAttributeName
                                                  };

                    for (int i = 1; i < compareProducts.Count; i++)
                    {
                        specificationAttributes = specificationAttributes.Union(
                             from spec in compareProducts[i].specs
                             orderby spec.seq, spec.LocalAttributeName
                             select new
                             {
                                 CatID = spec.CatID,
                                 ID = spec.AttrID,
                                 Name = spec.LocalAttributeName
                             });
                    }

                    if (Request["category"] != null && !string.IsNullOrEmpty(Request["category"]))
                    {
                        string _category = Request["category"].ToString();
                        CTOSSpecMask = Presentation.eStoreContext.Current.Store.getCTOSSpecMask(_category);
                    }
                    if (this.CTOSSpecMask != null && this.CTOSSpecMask.Count > 0)
                    {

                        specificationAttributes = (from s in specificationAttributes
                                                   from m in CTOSSpecMask
                                                   where s.ID == m.Attrid2 && s.CatID == m.CatID
                                                   orderby m.Sequence, m.Name
                                                   select s);
                    }




                    foreach (var specificationAttribute in specificationAttributes)
                    {

                        var productRow = new HtmlTableRow();
                        this.AddHeaderCell(productRow, Server.HtmlEncode(specificationAttribute.Name)).Align = "left";

                        foreach (var product2 in compareProducts)
                        {
                            var productCell = new HtmlTableCell();

                            var productSpecificationAttributes2 = product2.specs.FirstOrDefault(s => s.AttrID == specificationAttribute.ID);
                            productCell.InnerHtml = (productSpecificationAttributes2 != null) ? productSpecificationAttributes2.LocalValueName : "&nbsp;";

                            productCell.Align = "center";
                            productCell.VAlign = "top";
                            productRow.Cells.Add(productCell);
                        }
                        this.tblCompareProducts.Rows.Add(productRow);
                    }

                    string width = Math.Round((decimal)(90M / compareProducts.Count), 0).ToString() + "%";
                    for (int i = 0; i < this.tblCompareProducts.Rows.Count; i++)
                    {
                        var row = this.tblCompareProducts.Rows[i];
                        for (int j = 1; j < row.Cells.Count; j++)
                        {
                            if (j == (row.Cells.Count - 1))
                            {
                                row.Cells[j].Style.Add("width", width);
                                //row.Cells[j].Style.Add("text-align", "center");
                            }
                            else
                            {
                                row.Cells[j].Style.Add("width", width);
                                //row.Cells[j].Style.Add("text-align", "center");
                            }
                        }
                    }



                }
                else
                {

                    tblCompareProducts.Visible = false;
                }
            }
        }


        protected void bindCollectionPagerFonts()
        {
            CollectionPager1.ResultsFormat = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Displaying_results);
            CollectionPager1.NextText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Next);
            CollectionPager1.BackText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Previous);
            CollectionPager1.FirstText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_First);
            CollectionPager1.LastText = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Last);
        }
    }
}