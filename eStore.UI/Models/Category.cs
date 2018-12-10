using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace eStore.UI.Models
{
    public class Category
    {
        public Category() { }
        public Category(POCOS.ProductCategory category, bool isSimp = false, string filterType = "")
        {
            Id = category.CategoryPath;
            Name = eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(category);
            if (!isSimp)
            {
                Description = eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(category);
                HtmlContext = category.HtmlContent;
                Image = string.IsNullOrEmpty(category.ImageURL) ?
                    "https://buy.advantech.com/images/photounavailable.gif" :
                    (category.ImageURL.ToLower().StartsWith("http") ?
                    category.ImageURL : esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.eStoreContext.Current.Store.profile.Settings["ProductCategoryPicurePath"].ToString() + category.ImageURL));


                List<eStore.POCOS.Advertisement> ads = eStore.Presentation.eStoreContext.Current.Store.sliderBanner(category, false, true);
                if (ads != null && ads.Any(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite))
                {
                    eStore.POCOS.Advertisement ad = ads.First(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite);
                    if (!string.IsNullOrEmpty(ad.Imagefile))
                    {
                        BannerImage = string.Format(" url(\"{0}\")", esUtilities.CommonHelper.ResolveUrl("~/resource" + ad.Imagefile));
                    }
                    else
                    { BannerImage = string.Format(" url(\"{0}\")", esUtilities.CommonHelper.ResolveUrl("~/App_Themes/V4/images/eStore_BG_category_banner.jpg")); }
                }
                else
                {
                    BannerImage = string.Format(" url(\"{0}\")", esUtilities.CommonHelper.ResolveUrl("~/App_Themes/V4/images/eStore_BG_category_banner.jpg"));
                }
                List<eStore.POCOS.Advertisement> sliderBannerads = eStore.Presentation.eStoreContext.Current.Store.sliderBanner(category, true, false);

                if (sliderBannerads != null && sliderBannerads.Any(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite))
                    SliderBanners = sliderBannerads.Where(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite)
                                       .Select(x => $@"<div class=""cycle-slideitem"" style =""background-image:url({x.imagefileX})"" 
data-cycle-href=""{x.Hyperlink}"" alt=""{x.AlternateText}"" {(string.IsNullOrEmpty(x.HtmlContent) ? "data-cycle-btnstyle = 'blue'  data-cycle-btntext='More'" : x.HtmlContent)}  
data-cycle-title=""{ System.Net.WebUtility.HtmlEncode(x.Title)}"" data-cycle-desc=""{System.Net.WebUtility.HtmlEncode(x.AlternateText)}"" ></div>").ToArray()
                                       ;
                _storeid = category.Storeid;
                Modules.YouAreHere yah = new Modules.YouAreHere();
                yah.productCategory = category;
                StringBuilder sbYouarehere = new StringBuilder();
                sbYouarehere.AppendFormat("<a href=\"{0}\">{1}</a>"
                 , esUtilities.CommonHelper.ResolveUrl("~/")
                 , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Home));
                sbYouarehere.Append(yah.getYourAreHereString(null, false));

                Breadcrumbs = sbYouarehere.ToString();
                var simpProductList = category.simpleProductList;
                if (!string.IsNullOrEmpty(filterType))
                    Presentation.eStoreContext.Current.Store.margerSimpleProduct(ref simpProductList, filterType);
                ProductCount = simpProductList.Count();
                isTabCategory = category.isMatrixCategory() && !category.childCategoriesX.Any();
                Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category));

                POCOS.Address storeAddress  = Presentation.eStoreContext.Current. Store.getAddressByCountry(Presentation.eStoreContext.Current.CurrentCountry, category.businessGroup);
                if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("ShowTel", true)
                    && storeAddress != null 
                    && !string.IsNullOrEmpty(storeAddress.Tel))
                    this.BUPhoneNumber = storeAddress.Tel;
                DisplayType = category.DisplayTypeX.ToString();
                ShowReadMore = eStore.Presentation.eStoreContext.Current.getBooleanSetting("eStore_ReadMore_for_Category");
            }
        }

        public void setBaseInforAndMatrix(POCOS.ProductCategory category, bool isSetSpec = false)
        {
            if (category != null)
            {
                if (category.isMatrixCategory() || category.DisplayTypeX == POCOS.ProductCategory.RenderStyle.SelectBySpec)
                {
                    foreach (var pc in category.childCategoriesX)
                    { 
                        Category mca = new Category(pc,true);
                        mca.setBaseInforAndMatrix(pc);
                        Children.Add(mca);
                    }
                    if (isSetSpec && !(category.childCategoriesX != null && category.childCategoriesX.Any()))
                    {
                        Models.CategoryHelper helper = new CategoryHelper();
                        Specs = helper.margSpec(category);
                    }
                }
            }
        }

        public void setParentCategory(POCOS.ProductCategory category)
        {
            // category tab ls
            if (category.isMatrixCategory())
            {
                POCOS.ProductCategory parent = null;
                if (category.childCategoriesX.Any() || category.parentCategoryX == null)
                {
                    this.Parent = new Category(category, true);
                    parent = category;
                }
                else
                {
                    this.Parent = new Category(category.parentCategoryX, true);
                    parent = category.parentCategoryX;
                }
                this.Parent.isTabCategory = true;

                if (parent.childCategoriesX.Any())
                {
                    foreach (var cate in parent.childCategoriesX)
                    {
                        var mcate = new Category(cate, true);
                        mcate.isTabCategory = cate.isMatrixCategory();
                        mcate.Url = cate.StoreUrl;
                        this.Parent.Children.Add(mcate);
                    }
                }
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlContext { get; set; }
        public bool ShowReadMore { get; set; }
        public string Image { get; set; }
        public string BannerImage { get; set; }
        public string[] SliderBanners { get; set; }
        public string Price { get; set; }
        public string Url { get; set; }
        public string Breadcrumbs { get; set; }
        public List<Product> Products { get; set; }
        public int ProductCount { get; set; }
        private string _storeid { get; set; }
        private List<Category> _childre = new List<Category>();
	    public List<Category> Children
	    {
		    get { return _childre;}
		    set { _childre = value;}
	    }
        public Category Parent { get; set; }
        public List<CategoryAttrCate> Specs { get; set; }
        public bool isTabCategory { get; set; }
        public string DisplayType { get; set; }
        public string BUPhoneNumber { get; set; } = string.Empty;
    }
}