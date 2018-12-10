using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class CategoryV4 : System.Web.UI.UserControl
    {
        

        private List<int> availablepagesize  = new List<int>(); //分页条数

        //private int pagesize = 9;
        private int page = 1; //当前页数
        protected string SortType = "";
        protected int productCount = 0;
        protected int size = 9;

        public POCOS.ProductCategory CurrentCategory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string pagesizeStr = eStore.Presentation.eStoreContext.Current.getStringSetting("eStore_PageSizes", "9");
            availablepagesize = pagesizeStr.Split(',').Select(c => int.Parse(c)).ToList();

            pMatrix.Visible = eStore.Presentation.eStoreContext.Current.getBooleanSetting("showMatrix");
            hdCategoryid.Value = CurrentCategory.CategoryPath;
            if (Request.QueryString["page"] != null)
                int.TryParse(Request.QueryString["page"], out page);

            if (Request.QueryString["sort"] != null)
            {
                SortType = Request.QueryString["sort"];
            }

            if (CurrentCategory != null)
            {
                //var prolstemp = CurrentCategory.PerGetproductListAsycn();
                if(CurrentCategory.isMatrixCategory() && !CurrentCategory.childCategoriesX.Any())
                    hdCategoryDisType.Value = "Mtrix";
                else
                    hdCategoryDisType.Value = CurrentCategory.DisplayTypeX.ToString();
                POCOS.ProductCategory productcategory;



                if (CurrentCategory.parentCategoryX != null)
                {
                    int assignedrcid = -1;
                    if (int.TryParse(Request["rcid"], out assignedrcid))
                    {
                        productcategory = CurrentCategory.getRootCategory(assignedrcid);
                    }
                    else
                        productcategory = CurrentCategory.getRootCategory();

                }

                else
                    productcategory = CurrentCategory;

                //特殊类型category Delivery or promotion
                if (productcategory.isSpecialCategory)
                    Presentation.eStoreContext.Current.Store.fixSpecialProductCategory(productcategory);

                BindSubCategory(productcategory, CurrentCategory);

                //System.Threading.Tasks.Task.WaitAny(prolstemp); // includ product ls   
                BindUi(CurrentCategory);
                BindProducts(CurrentCategory);
            }
        }

        private void BindUi(POCOS.ProductCategory category)
        {
            hlHighest.Attributes.Add("data-Sort", "PriceHighest");
            hlLowest.Attributes.Add("data-Sort", "PriceLowest");
            ltLaest.Attributes.Add("data-Sort", "LatestedAdd");

            if (string.IsNullOrEmpty(SortType))
                SortType = category.SortType;

            size = availablepagesize.Min();

            foreach (var item in availablepagesize)
            {
                phSize.Controls.Add(new HyperLink()
                {
                    CssClass = item == size ? "on" : "",
                    Text = item.ToString()
                });
            }
        }



        /// <summary>
        /// bind sub category
        /// </summary>
        /// <param name="productcategory"></param>
        protected void BindSubCategory(POCOS.ProductCategory productcategory, POCOS.ProductCategory curr)
        {
            //sub categories
            var pcateids = curr.GetParentIds();
            if (productcategory.childCategoriesX != null && productcategory.childCategoriesX.Any())
            {
                System.Text.StringBuilder sbSubCategories = new System.Text.StringBuilder();
                sbSubCategories.Append("<ol class=\"eStore_category_link_linkBlock row20 eStore_category_moblieBlock\">");
                foreach (POCOS.ProductCategory spc in productcategory.childCategoriesX.Where(c => c.simpleProductList.Count > 0).OrderBy(c => c.Sequence))
                {
                    if (spc.childCategoriesX != null && spc.childCategoriesX.Any() && spc.simpleProductList.Count > 0)
                    {
                        sbSubCategories.AppendFormat("<li><div class=\"dcate{5}\"><a ref=\"{1}\" {3} class=\"category_linkBlock haveList\" href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a></div>"
                            , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(spc)
                            , spc.CategoryPath
                            , spc.simpleProductList.Count
                            , (!string.IsNullOrEmpty(spc.CreatedBy) && spc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + productcategory.CategoryPath + "\"" : ""
                            , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(spc))
                            , curr.CategoryID == spc.CategoryID ? " on" : ""
                            );
                        sbSubCategories.AppendFormat("<ul class=\"eStore_category_link_linkListBlock{0}\">", pcateids.Contains(spc.CategoryID) || spc.CategoryID.Equals(curr.CategoryID) ? " show" : "");
                        foreach (POCOS.ProductCategory sspc in spc.childCategoriesX.Where(c => c.simpleProductList.Count > 0).OrderBy(c => c.Sequence))
                        {
                            //the 4th level
                            if (sspc.childCategoriesX != null && sspc.childCategoriesX.Any() && sspc.simpleProductList.Count > 0)
                            {
                                sbSubCategories.AppendFormat("<li><div class=\"dcate{5}\"><a ref=\"{1}\" {3} class=\"category_linkBlock haveList\" href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a></div>"
                                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sspc)
                                    , sspc.CategoryPath
                                    , sspc.simpleProductList.Count
                                    , (!string.IsNullOrEmpty(sspc.CreatedBy) && sspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + spc.CategoryPath + "\"" : ""
                                    , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(sspc))
                                    , curr.CategoryID == sspc.CategoryID ? " on" : ""
                                    );
                                sbSubCategories.AppendFormat("<ul class=\"eStore_category_link_linkListBlock{0}\">", pcateids.Contains(sspc.CategoryID) || sspc.CategoryID.Equals(curr.CategoryID) ? " show" : "");
                                foreach (POCOS.ProductCategory ssspc in sspc.childCategoriesX.Where(c => c.simpleProductList.Count > 0))
                                {
                                    sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} {7}{8} ppath=\"{4}\" class=\"category_linkBlock{6}\" href=\"{5}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a></li>"
                                        , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(ssspc)
                                        , ssspc.CategoryPath
                                        , ssspc.simpleProductList.Count
                                        , (!string.IsNullOrEmpty(ssspc.CreatedBy) && ssspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + sspc.CategoryPath + "\"" : ""
                                        , sspc.CategoryPath
                                        , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(ssspc))
                                        , curr.CategoryID == ssspc.CategoryID ? " on" : ""
                                        , ssspc.isMatrixCategory() ? "isTab='true'" : ""
                                        , ssspc.DisplayTypeX == POCOS.ProductCategory.RenderStyle.CustomURL ? " skipjs='true'" : ""
                                        );
                                }
                                sbSubCategories.Append("</ul>");
                            }
                            else
                            {
                                sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} {7}{8}  ppath=\"{4}\" class=\"category_linkBlock{6}\"  href=\"{5}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                                    , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(sspc)
                                    , sspc.CategoryPath
                                    , sspc.simpleProductList.Count
                                    , (!string.IsNullOrEmpty(sspc.CreatedBy) && sspc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + spc.CategoryPath + "\"" : ""
                                    , spc.CategoryPath
                                    , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(sspc))
                                    , curr.CategoryID == sspc.CategoryID ? " on" : ""
                                    , sspc.isMatrixCategory() ? "isTab='true'" : ""
                                    , sspc.DisplayTypeX== POCOS.ProductCategory.RenderStyle.CustomURL ? " skipjs='true'" : ""
                                    );
                            }

                        }
                        sbSubCategories.Append("</ul>");
                    }
                    else
                    {
                        sbSubCategories.AppendFormat("<li><a ref=\"{1}\" {3} {6}{7} class=\"category_linkBlock{5}\"  href=\"{4}\">{0} (<span class=\"search_linkBlock_Number\">{2}</span>)</a>"
                            , eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(spc)
                            , spc.CategoryPath
                            , spc.simpleProductList.Count
                            , (!string.IsNullOrEmpty(spc.CreatedBy) && spc.CreatedBy.StartsWith("Temp_")) ? "parentPath=\"" + productcategory.CategoryPath + "\"" : ""
                            , ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(spc))
                            , curr.CategoryID == spc.CategoryID ? " on" : ""
                            , spc.isMatrixCategory() ? "isTab='true'" : ""
                            , spc.DisplayTypeX == POCOS.ProductCategory.RenderStyle.CustomURL ? " skipjs='true'" : ""
                            );
                    }
                    sbSubCategories.Append("</li>");
                }
                sbSubCategories.Append("</ol>");

                lSubCategories.Text = sbSubCategories.ToString();
            }
        }


        protected void BindProducts(POCOS.ProductCategory category)
        {
            if (category.simpleProductList != null && category.simpleProductList.Any())
            {
                var products = new List<POCOS.SimpleProduct>();
                Dictionary<string, object> urlQuery = new Dictionary<string, object>();

                products = category.simpleProductList;

                switch (SortType)
                {
                    case "PriceHighest":
                        hlHighest.CssClass = "on";
                        products = products.OrderBy(x => x.mappedToProduct().getListingPrice().value == 0 ? 1 : 0).OrderByDescending(x => x.mappedToProduct().getListingPrice().value).ToList();
                        break;
                    case "PriceLowest":
                        hlLowest.CssClass = "on";
                        products = products.OrderBy(x => x.mappedToProduct().getListingPrice().value == 0 ? 1 : 0).ThenBy(x => x.mappedToProduct().getListingPrice().value).ToList();
                        break;
                    case "LatestedAdd":
                        ltLaest.CssClass = "on";
                        products = products.OrderByDescending(x => x.CreatedDate).ToList();
                        break;
                    default:
                        Presentation.eStoreContext.Current.Store.fixProductQtyByCategory(category, ref products); //order by
                        break;
                }

                productCount = products.Count;
                int pageCount = (int)Math.Ceiling((double)products.Count / size);
                if (products.Count < (page - 1) * size)
                    page = 1;
                if (pageCount > 1)
                    urlQuery.Add("page", "$page$");

                ltBpage.Text = ltPage.Text = page.ToString();
                ltBpageCount.Text = ltPageCount.Text = pageCount.ToString();

                var curUrl = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(category)); //url
                string queryStr = esUtilities.StringUtility.SubUrl(HttpContext.Current.Request.Url.Query, urlQuery, "category");


                if (pageCount > 1)
                {
                    if (page < pageCount)
                        hBnext.NavigateUrl = hnext.NavigateUrl = curUrl + queryStr.Replace("$page$", (page + 1).ToString());
                    if (page > 1)
                        hBprev.NavigateUrl = hprev.NavigateUrl = curUrl + queryStr.Replace("$page$", (page - 1).ToString());
                }
                if (products.Count > (page - 1) * size)
                    products = products.Skip((page - 1) * size).Take(size).ToList();
                else
                    products = products.Take(size).ToList();

                var prols = new List<POCOS.Product>();
                foreach (var p in products)
                    prols.Add(p.mappedToProduct());

                rpProducts.DataSource = prols.Select(product => new {
                    Id = product.SProductID,
                    Name = product.name,
                    Description = product.productDescX,
                    Fetures = product.productFeatures,
                    Image = esUtilities.CommonHelper.ResolveUrl(esUtilities.CommonHelper.fixLocalImgPath(product.thumbnailImageX)),
                    Status = product.marketingstatus.Select(c => c.ToString()).ToList(),
                    Price = product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire)?"":Presentation.Product.ProductPrice.getPrice(product),
                    Url = esUtilities.CommonHelper.ResolveUrl(eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(product)),
                    CreatedDate = product.CreatedDate
                });
                rpProducts.DataBind();
            }
        }

        /// <summary>
        /// show sale images
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string BindImageIco(object obj)
        {
            List<string> ls = obj as List<string>;
            string str = "";
            if (ls != null)
            {
                foreach (var item in ls)
                    str += $"<img src='/images/{item}.gif' alt='{item}' />";
            }
            return str;
        }
    }
}