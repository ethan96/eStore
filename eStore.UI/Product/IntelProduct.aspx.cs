using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.Text;

namespace eStore.UI.Product
{
    public partial class IntelProduct : Presentation.eStoreBaseControls.eStoreBasePage
    {
        private List<POCOS.ProductCategory> _intelProductList;
        public List<POCOS.ProductCategory> IntelProductList
        {
            get {
                if (_intelProductList == null)                
                {
                    POCOS.MiniSite intelMiniSite = Presentation.eStoreContext.Current.Store.profile.getMiniSiteByApplicationPath("/Intel");
                    if (intelMiniSite != null)
                        _intelProductList = Presentation.eStoreContext.Current.Store.getTopLeveluStoreCategories(intelMiniSite).OrderBy(p=>p.Sequence).ToList();
                }
                
                return _intelProductList; 
            }
        }

        private bool _isLoad = true;
        public bool IsLoad
        {
            get { return _isLoad; }
            set { _isLoad = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Intel");
            eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner("Intel");
            if (!IsPostBack)
            {
                IsLoad = false;//默认加载时, 不加载Matrix
                if (IntelProductList != null)
                {
                    bindCategory();
                    btSearchClick(sender, e);
                }
                else
                {
                    ddlIntel1.Visible = false;
                    ddlIntel2.Visible = false;
                    ddlIntel3.Visible = false;
                    ddlIntel4.Visible = false;
                    ddlIntel4.Visible = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.Form["__EVENTARGUMENT"]))
                {
                    IsLoad = false;
                    btSearchClick(sender, e);
                }
            }
        }

        protected void btSearchClick(object sender, EventArgs e)
        {
            int categoryId = 0;
            if (ddlIntel5.Visible && !string.IsNullOrEmpty(ddlIntel5.SelectedValue))
                int.TryParse(ddlIntel5.SelectedValue, out categoryId);
            else if (ddlIntel4.Visible && !string.IsNullOrEmpty(ddlIntel4.SelectedValue))
                int.TryParse(ddlIntel4.SelectedValue, out categoryId);
            else if (ddlIntel3.Visible && !string.IsNullOrEmpty(ddlIntel3.SelectedValue))
                int.TryParse(ddlIntel3.SelectedValue, out categoryId);
            else if (ddlIntel2.Visible && !string.IsNullOrEmpty(ddlIntel2.SelectedValue))
                int.TryParse(ddlIntel2.SelectedValue, out categoryId);
            else if (ddlIntel1.Visible && !string.IsNullOrEmpty(ddlIntel1.SelectedValue))
                int.TryParse(ddlIntel1.SelectedValue, out categoryId);
            if (categoryId > 0)
                bindData(categoryId);
        }

        private void bindData(int categoryId)
        {
            if (rblIntel.SelectedItem.Text.ToLower().Contains("system"))
            {
                ProductMatrix1.Visible = false;
                ProductList1.Visible = false;
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(categoryId);
                List<eStore.POCOS.CTOSSpecMask> mask = new List<eStore.POCOS.CTOSSpecMask>();

                POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
                psr = Presentation.eStoreContext.Current.Store.getMatchProducts(productCategory.CategoryPath, string.Empty);
                if (psr._specrules != null)
                {
                    foreach (POCOS.VProductMatrix s in psr._specrules)
                    {
                        if (mask.Any(x => x.AttrID == s.AttrID.ToString()) == false)
                        {
                            mask.Add(new eStore.POCOS.CTOSSpecMask
                            {
                                AttrID = s.AttrID.ToString(),
                                Attrid2 = s.AttrID,
                                CategoryPath = productCategory.CategoryPath,
                                Name = s.AttrName,Sequence = s.seq,
                                CatID = s.CatID
                            });
                        }
                    }
                }
                ProductCompare1.CTOSSpecMask = mask;
                ProductCompare1.compareProducts = psr._products;
                ProductCompare1.IsLoad = false;

                if (!string.IsNullOrEmpty(Request.Form["__EVENTARGUMENT"]))
                {
                    int pageCount = 0;
                    int.TryParse(Request.Form["__EVENTARGUMENT"], out pageCount);
                    ProductCompare1.GenerateCompareTable(pageCount);
                }
                else
                    ProductCompare1.GenerateCompareTable();
            }
            else
            {
                POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(categoryId);
                if (productCategory != null)
                {
                    psr = Presentation.eStoreContext.Current.Store.getMatchProducts(productCategory.CategoryPath, "");

                    if (psr._specrules != null && psr._specrules.Count > 0)
                    {
                        ProductMatrix1.Visible = true;
                        ProductList1.Visible = false;
                        ProductMatrix1.Products = psr._products;
                        ProductMatrix1.VProductMatrixList = psr._specrules;
                        if (IsLoad)
                            ProductMatrix1.BindMatrixTable();
                    }
                    else
                    {
                        ProductMatrix1.Visible = false;
                        ProductList1.Visible = true;
                        ProductList1.productList = psr._products;
                        if (IsLoad)
                            ProductList1.BindProductTable();
                    }
                }
            }
        }

        private void bindCategory()
        {
            if (rblIntel.Items.Count == 0)
            {
                rblIntel.Items.Clear();
                foreach (POCOS.ProductCategory pcItem in IntelProductList)
                {
                    var pname = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(pcItem);
                    rblIntel.Items.Add(new ListItem(string.IsNullOrEmpty(pname) ? pcItem.CategoryName : pname, pcItem.CategoryID.ToString()));
                }
                if (rblIntel.Items.Count > 0)
                    rblIntel.SelectedIndex = 0;
                if (!string.IsNullOrEmpty(rblIntel.SelectedValue))
                {
                    POCOS.ProductCategory currentPc = IntelProductList.FirstOrDefault(p => p.CategoryID == int.Parse(rblIntel.SelectedValue));
                    genrateCategory(ddlIntel1, currentPc);
                }
            }
        }

        private void genrateCategory(DropDownList ddlIntel, POCOS.ProductCategory parentIntelCategory)
        {
            if (parentIntelCategory != null)
            {
                ddlIntel5.Visible = false;
                if (ddlIntel.ID == "ddlIntel1")
                {
                    ddlIntel2.Visible = false;
                    ddlIntel3.Visible = false;
                    ddlIntel4.Visible = false;
                }
                else
                {
                    if (ddlIntel.ID == "ddlIntel2")
                    {
                        ddlIntel3.Visible = false;
                        ddlIntel4.Visible = false;
                    }
                    else if (ddlIntel.ID == "ddlIntel3")
                        ddlIntel4.Visible = false;
                }

                ddlIntel.Items.Clear();
                ddlIntel.Visible = true;

                List<POCOS.ProductCategory> intelList = parentIntelCategory.childCategoriesX.ToList();
                foreach (POCOS.ProductCategory pcItem in intelList)
                {
                    var lcname = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(pcItem);
                    ddlIntel.Items.Add(new ListItem(string.IsNullOrEmpty(lcname) ? pcItem.CategoryName : lcname, pcItem.CategoryID.ToString()));
                }

                if (ddlIntel.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(ddlIntel.SelectedValue))
                    {
                        POCOS.ProductCategory currentPc = intelList.FirstOrDefault(p => p.CategoryID == int.Parse(ddlIntel.SelectedValue));
                        if (ddlIntel.ID == "ddlIntel1")
                            genrateCategory(ddlIntel2, currentPc);
                        else if (ddlIntel.ID == "ddlIntel2")
                            genrateCategory(ddlIntel3, currentPc);
                        else if (ddlIntel.ID == "ddlIntel3")
                            genrateCategory(ddlIntel4, currentPc);
                        else if (ddlIntel.ID == "ddlIntel4")
                            genrateCategory(ddlIntel5, currentPc);
                    }
                }
                else
                    ddlIntel.Visible = false;
            }
            else
                ddlIntel.Visible = false;
        }

        protected void ddlIntel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(int.Parse(ddlIntel1.SelectedValue));
            genrateCategory(ddlIntel2, productCategory);
        }

        protected void ddlIntel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(int.Parse(ddlIntel2.SelectedValue));
            genrateCategory(ddlIntel3, productCategory);
        }

        protected void ddlIntel3_SelectedIndexChanged(object sender, EventArgs e)
        {
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(int.Parse(ddlIntel3.SelectedValue));
            genrateCategory(ddlIntel4, productCategory);
        }

        protected void ddlIntel4_SelectedIndexChanged(object sender, EventArgs e)
        {
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(int.Parse(ddlIntel4.SelectedValue));
            genrateCategory(ddlIntel5, productCategory);
        }

        protected void rblIntel_SelectedIndexChanged(object sender, EventArgs e)
        {
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(int.Parse(rblIntel.SelectedValue));
            genrateCategory(ddlIntel1, productCategory);
        }
    }
}