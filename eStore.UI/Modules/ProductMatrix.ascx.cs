using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using eStore.POCOS;
using eStore.Presentation.Product;
using System.Text.RegularExpressions;

namespace eStore.UI.Modules
{
    public partial class ProductMatrix : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public IEnumerable<POCOS.Product> Products { get; set; }
        public List<POCOS.VProductMatrix> VProductMatrixList { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            BindMatrixTable();
        }

        public void BindMatrixTable()
        {
            bindFonts();
            this.GenerateMatrixTable();
        }

        void GenerateMatrixTable()
        {
            this.tblMatrixProducts.Rows.Clear();

            if (Products != null && VProductMatrixList != null && Products.Count() > 0 && VProductMatrixList.Count > 0)
            {
                bt_compare.Visible = true;
                bt_AddToCart.Visible = true;
                bt_AddToQuote.Visible = true;

                var Cat = (from cat in VProductMatrixList
                           group cat by
                           new { cat.CatID, cat.LocalCatName }
                               into catgroup
                               select new
                               {
                                   id = catgroup.Key.CatID,
                                   display = catgroup.Key.LocalCatName,
                                   attribute = (from attr in catgroup
                                                select new
                                                {
                                                    id = attr.AttrID,
                                                    display = attr.LocalAttributeName
                                                }).Distinct()
                               }).Take(7);
                var headerRow = new HtmlTableRow();
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select), 2, 1);
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PartNumber), 2, 1);

                var AttributeRow = new HtmlTableRow();
                int filterwordslength = 12;
                Regex rgObj = new Regex(@"^[-+±]?[0-9]{1,3}(?:,?[0-9]{3})*(?:\.[0-9]{1,})?");
                foreach (var cat in Cat)
                {
                    this.AddHeaderCell(headerRow, cat.display, 1, cat.attribute.Count());
                    foreach (var attr in cat.attribute)
                    {
                        HtmlTableCell cell = this.AddCell(AttributeRow, attr.display);
                        cell.ID = attr.id.ToString();
                        var atributeValues = from attrvalue in VProductMatrixList
                                             where attrvalue.CatID == cat.id && attrvalue.AttrID == attr.id && attrvalue.productcount>0
                                             select new
                                             {
                                                 id = attrvalue.AttrValueID,
                                                 display = attrvalue.LocalValueName,
                                                 seq = (rgObj.IsMatch(attrvalue.LocalValueName) &&  esUtilities.StringUtility.IsNumeric(rgObj.Match(Regex.Replace(attrvalue.LocalValueName, @"^[-+±\.]+", "")).Value))?
                                                        Convert.ToDouble(rgObj.Match(Regex.Replace(attrvalue.LocalValueName, @"^[-+±\.]+", "")).Value) : -1
                                             };
                        if (atributeValues.Where(p => p.seq >= 0).Count() > atributeValues.Where(p => p.seq == -1).Count())
                            atributeValues = atributeValues.OrderBy(p => p.seq).ToList();
                        else
                            atributeValues = atributeValues.OrderBy(p => p.display).ToList();



                        bool isSetWith = false;
                        System.Text.StringBuilder sbLi = new System.Text.StringBuilder();
                        foreach (var attrvalue in atributeValues)
                        {
                            string dispayname = Regex.Replace(attrvalue.display, "</?[a-zA-Z][a-zA-Z0-9]*[^<>]*/?>", " ");
                            sbLi.Append(string.Format("<li value='{0}' Title='{1}'>{1}</li>", attrvalue.id, dispayname));
                            if (dispayname.Length > filterwordslength)
                                isSetWith = true;
                        }

                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("<div class='cssDivDropDownList'>");
                        sb.Append("<ul class='estoredropdownlist'>");
                        sb.Append(string.Format("<li class='selectedItem{0}'><span value='0'>--All--</span>",isSetWith==true?" fixedwidth":""));
                        sb.Append("<ul class='option'>");
                        sb.Append("<li value='0'>--All--</li>");
                        sb.Append(sbLi.ToString());
                        sb.Append("</ul>");
                        sb.Append("</li>");
                        sb.Append("</ul>");
                        sb.Append("</div>");

                        LiteralControl ddlDiv = new LiteralControl(sb.ToString());
                        cell.Controls.Add(ddlDiv);
                        
                        LiteralControl br = new LiteralControl("<br/>");
                        cell.Controls.Add(br);
                        cell.VAlign = "bottom";
                        cell.NoWrap = false;
                    }
                }
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price), 2, 1);
                this.tblMatrixProducts.Rows.Add(headerRow);
                AttributeRow.Attributes.Add("class", "attribute");
                this.tblMatrixProducts.Rows.Add(AttributeRow);

                foreach (POCOS.Product _product in Products)
                {
                    var productRow = new HtmlTableRow();

                    HtmlTableCell productSelectCell = new HtmlTableCell();
                    //HtmlInputCheckBox inputCheckBox = new HtmlInputCheckBox();
                    //inputCheckBox.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    //inputCheckBox.Value = _product.name;
                    //inputCheckBox.Name = "cbproduct";
                    //productSelectCell.Controls.Add(inputCheckBox);
                    productSelectCell.InnerHtml = string.Format("<input type='checkbox' name='cbproduct' value='{0}' {1} />",_product.name
                                , _product.MininumnOrderQty == null ? "" : string.Format("MOQ='{0}'", _product.MininumnOrderQty));
                    productRow.Cells.Add(productSelectCell);


                    var productNameCell = new HtmlTableCell();
                    var productLink = new HyperLink();
                    productLink.Text = Server.HtmlEncode(_product.name);
                    productLink.CssClass = "jTipProductDetail";
                    productLink.Attributes.Add("id", _product.SProductID);
                    productLink.Attributes.Add("name", _product.name);

                    productLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(_product);
                    productNameCell.Controls.Add(productLink);
                    HtmlImage productstatus = new HtmlImage();
                    productstatus.Src = string.Format("/images/{0}.gif", _product.status.ToString());
                    productNameCell.Controls.Add(productstatus);
                    productNameCell.Attributes.Add("class", "nowrap");
                    if (Presentation.eStoreContext.Current.Store.isFastDeliveryProducts(_product))
                    {
                        var ImageUrl = string.Format("~/images/{0}/48_hours.jpg", _product.StoreID);
                        var AlternateText = "Fast Delivery within 48 hours";
                        var ToolTip = "Usually ships within 2 business days.";
                        if(Presentation.eStoreContext.Current.Store.storeID == "AJP"){
                            ImageUrl = string.Format("~/images/{0}/72_hours.jpg", _product.StoreID);
                            AlternateText = "Fast Delivery within 72 hours";
                            ToolTip = "Usually ships within 3 business days.";
                        }
                        Image imgfastdelivery = new Image();
                        imgfastdelivery.ImageUrl = ImageUrl;
                        imgfastdelivery.AlternateText = AlternateText;
                        imgfastdelivery.ToolTip = ToolTip;
                        productNameCell.Controls.Add(imgfastdelivery);
                    }
                    productRow.Cells.Add(productNameCell);

                 

                    foreach (HtmlTableCell headerattr in AttributeRow.Cells)
                    {
                        POCOS.VProductMatrix attrValue = _product.specs.FirstOrDefault(vpm => vpm.AttrID.ToString() == headerattr.ID);

                        if (attrValue != null)
                        {
                            HtmlTableCell pc = AddCell(productRow, attrValue.LocalValueName);
                            pc.Attributes.Add("name", attrValue.AttrValueID.ToString());
                        }
                        else
                            AddCell(productRow, "-");
                    }
                    HtmlTableCell pricecell = AddCell(productRow, Presentation.Product.ProductPrice.getSimplePrice(_product));
                    pricecell.Attributes.Add("class", "right");
                    productRow.Attributes.Add("class", "attributevalue");
                    this.tblMatrixProducts.Rows.Add(productRow);
                }

            }
            else
            {
                bt_compare.Visible = false;
                bt_AddToCart.Visible = false;
                bt_AddToQuote.Visible = false;
            }
        }


        private HtmlTableCell AddHeaderCell(HtmlTableRow row, string text, int rowspan = 1, int colspan = 1)
        {
            var cell = new HtmlTableCell("th");
            cell.InnerHtml = text;
            if (rowspan > 1)
                cell.RowSpan = rowspan;
            if (colspan > 1)
                cell.ColSpan = colspan;
            row.Cells.Add(cell);
            return cell;
        }

        private HtmlTableCell AddCell(HtmlTableRow row, string text)
        {
            var cell = new HtmlTableCell();
            cell.InnerHtml = text;
            row.Cells.Add(cell);
            return cell;
        }

        protected void bt_compare_Click(object sender, EventArgs e)
        {
            List<string> selectProductList = getStrSlectProductNo();
            ProductCompareManagement.setProductIDs(selectProductList);
            Response.Redirect("~/Compare.aspx");
        }

        protected void bt_AddToQuote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToQuote();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void bt_AddToCart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToCart();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            this.Response.Redirect("~/Cart/Cart.aspx");
        }

        protected List<string> getStrSlectProductNo()
        {
            List<string> ls = new List<string>();
            string requestproducts = Request["cbproduct"];
            if (!string.IsNullOrEmpty(requestproducts))
            {
                ls = requestproducts.Split(',').ToList<string>();
            }
            return ls;
        }

        protected List<POCOS.Product> getSlectProductNo()
        {
            List<POCOS.Product> proLS = new List<POCOS.Product>();
            var proNOls = getStrSlectProductNo();
            foreach (string proNo in proNOls)
            {
                POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(proNo);
                if (product != null)
                {
                    proLS.Add(product);
                }
            }
            return proLS;
        }

        protected void bindFonts()
        {
            bt_compare.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare);
            bt_AddToQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
            bt_AddToCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
        }
    }
}