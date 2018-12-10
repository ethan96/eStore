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
    public partial class ProductList : Presentation.eStoreBaseControls.eStoreBasePage
    {

        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.bindData();
                base.ChildControlsCreated = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private Control genSubCategoriesTab(POCOS.ProductCategory productCategory, string currentCategory)
        {
            HtmlGenericControl ProductCategoriesDiv = new HtmlGenericControl("div");
            ProductCategoriesDiv.ID = "MatrixTab";
            ProductCategoriesDiv.ClientIDMode = global::System.Web.UI.ClientIDMode.Static;
            StringBuilder sbTabs = new StringBuilder();
            sbTabs.Append("<ul>");
            foreach (POCOS.ProductCategory pc in productCategory.childCategoriesX
                .TakeWhile(c => c.DisplayTypeX == POCOS.ProductCategory.RenderStyle.Tabs)
                .OrderBy(pc => pc.Sequence))
            {

                sbTabs.AppendFormat(" <li{2}><a href=\"{0}\">{1}</a></li>"
                    , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(pc))
                    , pc.LocalCategoryName
                    , pc.CategoryPath.ToLower() == currentCategory.ToLower() ? " class=\"active\"" : "");

            }
            sbTabs.Append("</ul>");
            ProductCategoriesDiv.InnerHtml = sbTabs.ToString();
            return ProductCategoriesDiv;
        }
       
        private void bindData()
        {
         
            phLeftSide.Controls.Clear();
            string category = Request["category"];
            POCOS.ProductSpecRules psr = new POCOS.ProductSpecRules();
            if (string.IsNullOrEmpty(category))//check product type
            {
                if (Request.QueryString["type"] != null)
                {
                    string typestr = Request.QueryString["type"].ToString().ToUpper();
                    try
                    {
                        psr = new POCOS.ProductSpecRules();
                        eStore.POCOS.Product.PRODUCTSTATUS prostatus = (eStore.POCOS.Product.PRODUCTSTATUS)Enum.Parse(typeof(eStore.POCOS.Product.PRODUCTSTATUS), typestr);
                        psr._products = (List<POCOS.Product>)Presentation.eStoreContext.Current.Store.getProducts(prostatus, Presentation.eStoreContext.Current.MiniSite);
                        lCategoryName.Text = typestr;
                        lCategoryDescription.Text = "";

                    }
                    catch (Exception)
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("no available products",null,true);
                    }
                }
            }
            else //get from category
            {
               
                POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProductCategory(category);
                if (productCategory == null)
                {
                    this.UserActivitLog.CategoryName = category;
                    this.UserActivitLog.CategoryType = "ErrorCategory";
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true);
                    return;
                }
                else
                {
                    //set user activit log
                    this.UserActivitLog.CategoryID = productCategory.CategoryID.ToString();
                    this.UserActivitLog.CategoryName = productCategory.CategoryName;
                    this.UserActivitLog.CategoryType = productCategory.CategoryType;

                    this.lCategoryName.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(productCategory);
                    this.lCategoryDescription.Text = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory);
                    this.YouAreHere1.productCategory = productCategory;
                    Presentation.eStoreContext.Current.BusinessGroup = productCategory.businessGroup;

                    this.isExistsPageMeta = setPageMeta(productCategory.pageTitle, productCategory.metaData, productCategory.keywords);

                    //for mini site header
                    if (Presentation.eStoreContext.Current.MiniSite != null && Presentation.eStoreContext.Current.MiniSite.SiteName == "iAutomation" && productCategory != null)
                    {
                        this.pCategoryHeader.Controls.Clear();
                        Modules.CategoryHeader header = LoadControl("~/Modules/CategoryHeader.ascx") as Modules.CategoryHeader;
                        header.productCategory = productCategory;
                        this.pCategoryHeader.Controls.Add(header);
                    }
                    else
                    {
                        eStoreLiquidSlider1.Advertisements = Presentation.eStoreContext.Current.Store.sliderBanner(productCategory);
                    }
                
                }

               if (productCategory.childCategoriesX != null
                    && productCategory.childCategoriesX.TakeWhile(c=>c.DisplayTypeX==POCOS.ProductCategory.RenderStyle.Tabs).Count() > 0)//has tabs
                {
                    category = productCategory.childCategoriesX.TakeWhile(c => c.DisplayTypeX == POCOS.ProductCategory.RenderStyle.Tabs)
                        .OrderBy(pc => pc.Sequence)
                        .ThenBy(pc => pc.CategoryName).First()
                        .CategoryPath;
                    phLeftSide.Controls.Add(genSubCategoriesTab(productCategory, category));
                }
               else if (productCategory.childCategoriesX != null
                   && productCategory.childCategoriesX.Count() == 0
                   && productCategory.parentCategoryX != null
                   && productCategory.DisplayTypeX == POCOS.ProductCategory.RenderStyle.Tabs
                   && productCategory.parentCategoryX.childCategoriesX.Count>1)
               {
                   phLeftSide.Controls.Add(genSubCategoriesTab(productCategory.parentCategoryX, category));
               }

                Presentation.eStoreContext.Current.keywords.Add("CategoryID", category);

                string keyword = this.ProductSpec1.keyword;
                List<POCOS.VProductMatrix> vpms = this.ProductSpec1.getSelectedSpec();

                if (vpms.Count == 0)
                {
                    psr = Presentation.eStoreContext.Current.Store.getMatchProducts(category, keyword);
                }
                else
                {
                    psr = Presentation.eStoreContext.Current.Store.getMatchProducts(category, vpms, keyword);
                }
            }
            //设置Popular Product id
            if (eStore.Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("EnablePopularProduct") && psr._products != null && psr._products.Count > 0)
                AdPopularProduct1.SproductId = string.Join(";", psr._products.Select(p => p.ModelNo).Distinct().ToArray());

            this.ProductSpec1.VProductMatrixList = psr._specrules;

            if (psr._specrules == null || psr._specrules.Count == 0 || (!string.IsNullOrEmpty(Request["display"]) && Request["display"].ToUpper() == "PRODUCTLIST"))// load product list
            {
                Modules.ProductList ucProductList = (Modules.ProductList)LoadControl("~/Modules/ProductList.ascx");
                ucProductList.productList = psr._products;
                if (psr._productcategory != null && psr._productcategory.childCategoriesX.Count == 0)
                    ucProductList.KeepOriginalSequence = true;
                else
                    ucProductList.KeepOriginalSequence = false;

                phLeftSide.Controls.Add(ucProductList);
            }
            else// load product Matrix
            {
                Modules.ProductMatrix ucProductMatrix = (Modules.ProductMatrix)LoadControl("~/Modules/ProductMatrix.ascx");
                ucProductMatrix.Products = psr._products;
                ucProductMatrix.VProductMatrixList = psr._specrules;
                phLeftSide.Controls.Add(ucProductMatrix);
                ProductSpec1.Visible = false;
            }


        }
    }
}