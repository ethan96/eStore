using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS.DAL;
using eStore.POCOS;
using esUtilities;
using eStore.Presentation.SearchConfiguration;

namespace eStore.UI.CertifiedPeripherals
{
    public partial class ProductLineV2 : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Presentation.eStoreContext.Current.SearchConfiguration = new PStoreSearchConfiguration();
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

        protected string productsdata = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request["path"];
            if (!string.IsNullOrEmpty(path))
            {
                POCOS.ProductCategory category = Presentation.eStoreContext.Current.Store.getProductCategory(path);
                if (category != null)
                    Response.Redirect(Presentation.UrlRewriting.MappingUrl.getMappingUrl(category));
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("product category is not available ", null, true, 404);
                    return;
                }
            }
            else
            {
                Response.Redirect("~/");
            }

            BusinessModules.PStore pstore = BusinessModules.PStore.getInstance(Presentation.eStoreContext.Current.Store.profile);
            if (!pstore.isActive())
            {
                Response.Redirect("~/");
            }
            //string bootstrapstyle = CommonHelper.GetStoreLocation() + "Styles/bootstrap.min.css";
            //AddStyleSheet(bootstrapstyle);
            string style = CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);

            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");
            BindScript("url", "underscore", "underscore.js");
            BindScript("url", "CertifiedPeripherals", "CertifiedPeripherals.js");

            int id = 0;
            if (int.TryParse(Request["category"], out id))
            {
                PStoreProductCategoryHelper helper = new PStoreProductCategoryHelper();
                PStoreProductCategory category = helper.get(Presentation.eStoreContext.Current.Store.profile, id);
                if (category != null)
                {
                    YouAreHere1.productCategory = category;
                    Presentation.eStoreContext.Current.keywords.Add("CategoryID", id.ToString());
                    //helper.perfetchFilter(category);
                    List<PStoreProduct> products = category.productList;
                    this.PaginationProductList1.Products = products.OrderBy(x=>x.OrdinalNo).ThenByDescending(x=>x.Id).ToList();
                    this.isExistsPageMeta = this.setPageMeta(category.MetaTitle, category.MetaDescription, category.MetaKeyword);
                    PStoreProductCategory adcategory = category.parentX == null ? category : category.parentX;
                    imgCategoryBanner.ImageUrl = string.Format("https://wfcache.advantech.com/www/certified-peripherals/documents/banner/{0}.jpg", adcategory.Name);
                   POCOS.Advertisement ad = pstore.getTodaysDeals(adcategory);
                    if (ad != null)
                    {
                        headerbanner.Visible = true;
                        if (!string.IsNullOrEmpty(ad.htmlContentX))
                        {
                            bannerImg.InnerHtml = ad.htmlContentX;
                        }
                        else
                        {
                            bannerImg.Controls.Clear();
                            Image img = new Image();
                            img.ImageUrl = ad.imageFileHost; ;
                            bannerImg.Controls.Add(img);

                        }
                        bannertitle.Text = ad.Title;
                        bannercontent.Text = ad.AlternateText;
                        bannerlink.NavigateUrl = esUtilities.CommonHelper.ConvertToAppVirtualPath(ad.Hyperlink);
                        if (string.IsNullOrEmpty(ad.Target) == false)
                        {
                            bannerlink.Target = ad.Target;
                        }
                    }
                    else
                    {
                        headerbanner.Visible = false;
                    }
                    imgCategoryBanner.ToolTip = category.DisplayName;
                    lname.Text = category.DisplayName;
                    subcategorytitleforspec.Text = category.DisplayName;
                    if (category.childCategoriesX != null & category.childCategoriesX.Any())
                    {
                        this.PaginationProductList1.ShowCompareCheckbox = false;
                        rpsubcategories.DataSource = category.childCategoriesX;
                        rpsubcategories.DataBind();
                        Literal subcategorytitle = (Literal)rpsubcategories.Controls[0].Controls[0].FindControl("subcategorytitle");
                        subcategorytitle.Text = category.DisplayName;

                        List<PStoreProduct> promotionproducts = products.Where(x => x.pStorePromotionType != eStore.POCOS.StoreDeal.PromotionType.Other)
                            .OrderBy(x => (x.storeDeals == null || x.storeDeals.Any() == false) ? int.MaxValue : x.storeDeals.Max(d => d.OrdinalNo))
                            .ToList();

                        if (promotionproducts.Any())
                        {
                            this.CarouFredProductList1.Products = promotionproducts;
                            this.PromotionProductsPanel.Visible = true;
                        }
                        else
                        {
                            this.PromotionProductsPanel.Visible = false;
                        }

                    }
                    else
                    {
                        if (category.parentX != null)
                        {
                            rpsubcategories.DataSource = category.parentX.childCategoriesX;
                            rpsubcategories.DataBind();
                            Literal subcategorytitle = (Literal)rpsubcategories.Controls[0].Controls[0].FindControl("subcategorytitle");
                            subcategorytitle.Text = category.parentX.DisplayName;
                        }
                        if (category.filter != null && category.filter.Any())
                        {
                            rpFilter.DataSource = category.filter;
                            rpFilter.DataBind();
                            specfilter.Visible = true;
                        }
                        else
                        {
                            specfilter.Visible = false;
                        }
                    }

                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("product category is not available ", null, true, 404);
                }

            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("product category is not available ", null, true, 404);
            }
        }
    }
}