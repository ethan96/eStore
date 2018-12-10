using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class ProductCategoryNew : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request["category"] == null || string.IsNullOrEmpty(Request["category"]))
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true, 404);
                else
                {
                    string _category = Request["category"].ToString();
                    hcategory.Value = _category;
                    POCOS.ProductCategory productcategory = Presentation.eStoreContext.Current.Store.getProductCategory(_category);
                    if (productcategory != null)
                    {
                        if (productcategory.parentCategoryX != null)
                            Response.Redirect(Presentation.UrlRewriting.MappingUrl.getMappingUrl(productcategory));

                        List<POCOS.Advertisement> ads = Presentation.eStoreContext.Current.Store.sliderBanner(productcategory, true);
                        if (ads != null && ads.Any(x => x.MiniSite == eStore.Presentation.eStoreContext.Current.MiniSite))
                        {
                            eStoreCycle2Slider1.Advertisements = ads;
                        }

                        if (productcategory.isSpecialCategory)
                            Presentation.eStoreContext.Current.Store.fixSpecialProductCategory(productcategory);

                        var papsMarketingStatus = new eStore.POCOS.Product.PRODUCTMARKETINGSTATUS[] { eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.NEW, eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.HOT, eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.FEATURE };
                    

                        if (productcategory.childCategoriesX != null && productcategory.childCategoriesX.Any())
                        {
                            System.Text.StringBuilder sbSubCategories = new System.Text.StringBuilder();
                            sbSubCategories.Append("<ol class=\"eStore_category_link_linkBlock row20 eStore_category_moblieBlock\">");
                            foreach (POCOS.ProductCategory spc in productcategory.childCategoriesX.OrderBy(c => c.Sequence))
                            {
                                if (spc.childCategoriesX != null && spc.childCategoriesX.Any())
                                {
                                    sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} class=\"category_linkBlock haveList\" href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                                        , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(spc)
                                        , spc.CategoryPath
                                        , spc.simpleProductList.Count
                                        , (!string.IsNullOrEmpty(spc.CreatedBy) && spc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + productcategory.CategoryPath + "\"" : ""
                                        , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(spc))
                                        );
                                    sbSubCategories.Append("<ul class=\"eStore_category_link_linkListBlock\">");
                                    foreach (POCOS.ProductCategory sspc in spc.childCategoriesX.OrderBy(c => c.Sequence))
                                    {
                                        //sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} class=\"category_linkBlock\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a></li>"
                                        //    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sspc), sspc.CategoryPath, sspc.simpleProductList.Count
                                        //    , (!string.IsNullOrEmpty(sspc.CreatedBy) && sspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + spc.CategoryPath + "\"" : "");
                                        //the 4th level
                                        if (sspc.childCategoriesX != null && sspc.childCategoriesX.Any())
                                        {
                                            sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} class=\"category_linkBlock haveList\" href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                                                , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sspc)
                                                , sspc.CategoryPath
                                                , sspc.simpleProductList.Count
                                                , (!string.IsNullOrEmpty(sspc.CreatedBy) && sspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + spc.CategoryPath + "\"" : ""
                                                , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(sspc))
                                                );
                                            sbSubCategories.Append("<ul class=\"eStore_category_link_linkListBlock\">");
                                            foreach (POCOS.ProductCategory ssspc in sspc.childCategoriesX)
                                            {
                                                sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} ppath=\"{4}\" class=\"category_linkBlock\" href=\"{5}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a></li>"
                                                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(ssspc)
                                                    , ssspc.CategoryPath
                                                    , ssspc.simpleProductList.Count
                                                    , (!string.IsNullOrEmpty(ssspc.CreatedBy) && ssspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + sspc.CategoryPath + "\"" : ""
                                                    , sspc.CategoryPath
                                                    , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(ssspc))
                                                    );
                                            }
                                            sbSubCategories.Append("</ul>");
                                        }
                                        else
                                        {
                                            sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} ppath=\"{4}\" class=\"category_linkBlock\"  href=\"{5}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                                                , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sspc)
                                                , sspc.CategoryPath
                                                , sspc.simpleProductList.Count
                                                , (!string.IsNullOrEmpty(sspc.CreatedBy) && sspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + spc.CategoryPath + "\"" : ""
                                                , spc.CategoryPath
                                                , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(sspc))
                                                );
                                        }

                                    }
                                    sbSubCategories.Append("</ul>");
                                }
                                else
                                {
                                    sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} class=\"category_linkBlock\"  href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                                        , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(spc)
                                        , spc.CategoryPath
                                        , spc.simpleProductList.Count
                                        , (!string.IsNullOrEmpty(spc.CreatedBy) && spc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + productcategory.CategoryPath + "\"" : ""
                                        , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(spc))
                                        );
                                }
                                sbSubCategories.Append("</li>");
                            }
                            sbSubCategories.Append("</ol>");
                            Presentation.eStoreContext.Current.BusinessGroup = productcategory.businessGroup;
                            lSubCategories.Text = sbSubCategories.ToString();
                            this.isExistsPageMeta = setPageMeta(productcategory.pageTitle, productcategory.metaData, productcategory.keywords);

                            Presentation.eStoreContext.Current.keywords.Add("CategoryID", _category);
                        }
                        else
                            Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Product_category_is_not_found), null, true, 404);

                    }

                }
            }
        }

     
    }
}