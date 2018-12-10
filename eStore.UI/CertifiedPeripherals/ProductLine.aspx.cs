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
    public partial class ProductLine : Presentation.eStoreBaseControls.eStoreBasePage
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
            //string bootstrapstyle = CommonHelper.GetStoreLocation() + "Styles/bootstrap.min.css";
            //AddStyleSheet(bootstrapstyle);
            string style = CommonHelper.GetStoreLocation() + "Styles/CertifiedPeripherals.css";
            AddStyleSheet(style);
            BindScript("url", "json3.min", "json3.min.js");
            BindScript("url", "underscore", "underscore.js");
            BindScript("url", "jquery.easing.1.3", "jquery.easing.1.3.js");
            BindScript("url", "jquery.carouFredSel-6.2.1-packed", "jquery.carouFredSel-6.2.1-packed.js");
            BindScript("url", "jquery.colorbox", "jquery.colorbox.js");
            BindScript("url", "angular", "angular.js");

            BindScript("url", "ui-bootstrap-tpls-0.11.0", "bootstrap/ui-bootstrap-tpls-0.11.0.js");
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
                    helper.perfetchFilter(category);
                    List<PStoreProduct> products = category.productList;
                    //this.PaginationProductList1.Products = products;
                    AJAXProductList1.Products = products;

                    List<PStoreProduct> promotionproducts = products.Where(x => x.pStorePromotionType != eStore.POCOS.StoreDeal.PromotionType.Other).ToList();
                
                    if (promotionproducts.Any())
                    {
                       this.CarouFredProductList1.Products = promotionproducts;
                        this.PromotionProductsPanel.Visible = true;
                    }
                    else
                    {
                        this.PromotionProductsPanel.Visible = false;
                    }
                    PStoreProductCategory adcategory = category.parentX == null ? category : category.parentX;
                    imgCategoryBanner.ImageUrl = string.Format("http://wfcache.advantech.com/www/certified-peripherals/documents/banner/{0}.jpg", adcategory.Name);
                    BusinessModules.PStore  pstore= new BusinessModules.PStore(Presentation.eStoreContext.Current.Store.profile);
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
                        rpsubcategories.DataSource = category.childCategoriesX;
                        rpsubcategories.DataBind();
                        Literal subcategorytitle = (Literal)rpsubcategories.Controls[0].Controls[0].FindControl("subcategorytitle");
                        subcategorytitle.Text = category.DisplayName;
                    }
                    else
                    {
                    }
                }

            }
        }
    }
}