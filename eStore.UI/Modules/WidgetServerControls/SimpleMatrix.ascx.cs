using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using eStore.Presentation.Product;

namespace eStore.UI.Modules.WidgetServerControls
{
    public partial class SimpleMatrix  : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
    

        private List<POCOS.Product> Products { get; set; }
        private List<POCOS.VProductMatrix> VProductMatrixList { get; set; }
        public string CategoryID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CategoryID))
                return;
            if (CategoryID.Contains(','))
            {
                Products = new List<POCOS.Product>();
                VProductMatrixList = new List<POCOS.VProductMatrix>();
                foreach(string pid in CategoryID.Split(','))
                {
                    POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(pid);
                    if (product != null)
                    {
                        Products.Add(product);
                        VProductMatrixList.AddRange(product.specs);
                    }
                }
            }
            else
            {
                POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
                psr = Presentation.eStoreContext.Current.Store.getMatchProducts(CategoryID, string.Empty);
                this.Products = psr._products;
                this.VProductMatrixList = psr._specrules;
            }
            this.GenerateMatrixTable();
        }
        void GenerateMatrixTable()
        {


            this.Products = Products;
            this.tblMatrixProducts.Rows.Clear();

            if (Products != null & VProductMatrixList != null & Products.Count() > 0 && VProductMatrixList.Count > 0)
            {

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
                               }).Take(4);
                var headerRow = new HtmlTableRow();
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select), 1, 1);
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_PartNumber), 1, 1);

              
                
                foreach (var cat in Cat)
                {
                    //this.AddHeaderCell(headerRow, cat.display, 1, cat.attribute.Count());
                    foreach (var attr in cat.attribute)
                    {
                        HtmlTableCell cell = this.AddHeaderCell(headerRow, attr.display);
                        cell.ID = attr.id.ToString();
                        
                    }
                }
                this.AddHeaderCell(headerRow, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price), 1, 1);
                this.tblMatrixProducts.Rows.Add(headerRow);
               

                foreach (POCOS.Product _product in Products.OrderByDescending(p => p.name))
                {
                    var productRow = new HtmlTableRow();

                    HtmlTableCell productSelectCell = new HtmlTableCell();
                    //HtmlInputCheckBox inputCheckBox = new HtmlInputCheckBox();
                    //inputCheckBox.ClientIDMode = System.Web.UI.ClientIDMode.Static;
                    //inputCheckBox.Value = _product.name;
                    //inputCheckBox.Name = "cbproduct";
                    //productSelectCell.Controls.Add(inputCheckBox);
                    productSelectCell.InnerHtml = "<input type='checkbox' name='cbproduct' value='" + _product.name + "' />";
                    productRow.Cells.Add(productSelectCell);


                    var productNameCell = new HtmlTableCell();
                    var productLink = new HyperLink();
                    productLink.Text =  _product.productDescX;
                    productLink.CssClass = "jTipProductDetail";
                    productLink.Attributes.Add("id", _product.SProductID);
                    productLink.Attributes.Add("name", _product.name);

                    productLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(_product);
                    productNameCell.Controls.Add(productLink);
                    HtmlImage productstatus = new HtmlImage();
                    productstatus.Src = string.Format("/images/{0}.gif", _product.status.ToString());
                    productNameCell.Controls.Add(productstatus);
                    productNameCell.Attributes.Add("class", "nowrap");
                    productRow.Cells.Add(productNameCell);

                    for (int i=2;i<headerRow.Cells.Count-1;i++)
                    {
                        POCOS.VProductMatrix attrValue = _product.specs.FirstOrDefault(vpm => vpm.AttrID.ToString() == headerRow.Cells[i].ID);

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

      
    }
}