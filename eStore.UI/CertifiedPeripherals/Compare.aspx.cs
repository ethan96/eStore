using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.SearchConfiguration;
using eStore.Presentation.Product;
using System.Web.UI.HtmlControls;

namespace eStore.UI.CertifiedPeripherals
{
    public partial class Compare : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
            Presentation.eStoreContext.Current.SearchConfiguration = new PStoreSearchConfiguration();
        }
        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();

                base.ChildControlsCreated = true;
            }
        }

        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
            set
            {
                base.OverwriteMasterPageFile = value;
            }
        }
        public List<POCOS.PStoreProduct> compareProducts { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
            string style = esUtilities.CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);
            if (!IsPostBack)
                this.GenerateCompareTable();
        }
        public Boolean IsLoad { get; set; }
        public void GenerateCompareTable(int pageCount = 0)
        {
            if (!IsLoad)//false就加载
            {
                this.tblCompareProducts.Rows.Clear();

                if (compareProducts == null)
                {
                    compareProducts = new List<POCOS.PStoreProduct>();
                    if (!string.IsNullOrEmpty(Request["parts"]))
                    {
                        string partids = Request["parts"];
                        if (!string.IsNullOrEmpty(partids))
                        {
                            string[] values = partids.Split(',');
                            List<POCOS.Part> parts = (new POCOS.DAL.PartHelper()).prefetchPartList(Presentation.eStoreContext.Current.Store.storeID, values.ToList());
                            foreach (POCOS.Part part in parts)
                            {
                                if (part is POCOS.PStoreProduct)
                                    compareProducts.Add((POCOS.PStoreProduct)part);
                            }

                        }
                    }
                }


                if (compareProducts.Count > 0)
                {
                    if (compareProducts.Count > 4)
                        compareProducts = compareProducts.Take(4).ToList();
                    var headerRow = new HtmlTableRow();
                    //this.AddHeaderCell(headerRow, "&nbsp;");

                    var ImageRow = new HtmlTableRow();
                    this.AddHeaderCell(ImageRow, "Picture");

                    var priceRow = new HtmlTableRow();
                    this.AddHeaderCell(priceRow, "Price");

                    var DescRow = new HtmlTableRow();
                    this.AddHeaderCell(DescRow, "Product Description");


                    var productNameRow = new HtmlTableRow();
                    this.AddHeaderCell(productNameRow, "Product");



                    foreach (var product in compareProducts)
                    {

                        var productImage = new HtmlImage();
                        productImage.Border = 0;
                        productImage.Width = 166;
                        var productImageLink = new HyperLink();


                        productImageLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
                        //productImage.Align = "center";
                        productImage.Alt = product.productDescX;

                        string productPicture = product.largeImageX;
                        if (!string.IsNullOrEmpty(productPicture))
                            productImage.Src = productPicture;
                        else
                            productImage.Src = "";
                        productImageLink.Controls.Add(productImage);
                        var ImageCell = new HtmlTableCell();
                        ImageCell.Controls.Add(productImageLink);
                        if (product.isLongevity)
                        {
                            HtmlGenericControl divLongevity = new HtmlGenericControl("div");
                            divLongevity.Attributes.Add("class", "tooltip epaps-Longevity");
                            divLongevity.InnerHtml = "<span class=\"classic\">Products marked with the Longevity icon have a longer life than products without the longevity icon. </span>";
                            ImageCell.Style.Add("position", "relative");
                            ImageCell.Controls.Add(divLongevity);
                        }
                        ImageCell.Align = "center";
                        ImageRow.Cells.Add(ImageCell);

                        var productNameCell = new HtmlTableCell();
                        productNameCell.Align = "center";
                        var productLink = new HyperLink();
                        productLink.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(product);
                        productLink.Text = Server.HtmlEncode(product.name + " ");
                        productLink.Attributes.Add("class", "link");
                        productNameCell.Controls.Add(productLink);
                        productNameRow.Cells.Add(productNameCell);

                        var priceCell = new HtmlTableCell();
                        priceCell.Align = "center";
                        priceCell.Attributes.Add("class", "epaps-productprice");
                        priceCell.InnerHtml = Presentation.Product.ProductPrice.FormartPstorePrice(product.getListingPrice().value);
                        priceRow.Cells.Add(priceCell);

                        var DescCell = new HtmlTableCell();
                        DescCell.Align = "left";
                        DescCell.InnerHtml = product.productDescX;
                        DescRow.Cells.Add(DescCell);


                        this.tblCompareProducts.Rows.Add(ImageRow);

                        this.tblCompareProducts.Rows.Add(priceRow);
                        this.tblCompareProducts.Rows.Add(productNameRow);
                        this.tblCompareProducts.Rows.Add(DescRow);
                    }
                    string[] hideattributes = { "Product", "Product Description" };
                    var specificationAttributes = from spec in compareProducts[0].specs
                                                  where hideattributes.Contains(spec.AttrName)==false
                                                  select new
                                                  {
                                                      spec.seq,
                                                      Name = spec.LocalAttributeName
                                                  };

                    for (int i = 1; i < compareProducts.Count; i++)
                    {
                        specificationAttributes = specificationAttributes.Union(
                             from spec in compareProducts[i].specs
                             where hideattributes.Contains(spec.AttrName) == false
                             orderby spec.seq, spec.LocalAttributeName
                             select new
                             {
                                 spec.seq,
                                 Name = spec.LocalAttributeName
                             });
                    }

                    var sepcname = (from s in specificationAttributes
                                    group s by s.Name into g
                                    orderby g.Max(x => x.seq)
                                    select
                                    new { Name = g.Key });

                    foreach (var specificationAttribute in sepcname)
                    {

                        var productRow = new HtmlTableRow();
                        this.AddHeaderCell(productRow, Server.HtmlEncode(specificationAttribute.Name)).Align = "left";

                        foreach (var product2 in compareProducts)
                        {
                            var productCell = new HtmlTableCell();

                            var productSpecificationAttributes2 = product2.specs.FirstOrDefault(s => s.AttrName == specificationAttribute.Name);
                            productCell.InnerHtml = (productSpecificationAttributes2 != null) ? productSpecificationAttributes2.LocalValueName : "&nbsp;";

                           // productCell.Align = "center";
                            //productCell.VAlign = "top";
                            productRow.Cells.Add(productCell);
                        }
                        this.tblCompareProducts.Rows.Add(productRow);
                    }

                    string width = Math.Round((decimal)(85M / compareProducts.Count), 0).ToString() + "%";
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
    }
}