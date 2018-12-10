using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eStore.Presentation.Product
{
    public class ProductCompareManagement
    {
        /// <summary>
        /// Clears a "compare products" list
        /// </summary>
        public static void ClearCompareProducts()
        {
            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("eStoreCompareProducts");
            if (compareCookie != null)
            {
                compareCookie.Values.Clear();
                compareCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Set(compareCookie);
            }
        }

        /// <summary>
        /// Gets a "compare products" list
        /// </summary>
        /// <returns>"Compare products" list</returns>
        public static IList<POCOS.Product> GetCompareProducts()
        {
            IList<POCOS.Product> products = new List<POCOS.Product>();
            List<string> productIds =  GetCompareProductsIds();
            foreach (string productId in productIds)
            {
                var product = Presentation.eStoreContext.Current.Store.getProduct(productId);
                if (product != null)
                    products.Add(product);
            }     
            return products;
        }
        public static IList<POCOS.Product> GetCompareProductsFromQueryString()
        {
            IList<POCOS.Product> products = new List<POCOS.Product>();
            List<string> productIds = GetCompareProductsIdsFromQueryString();
            foreach (string productId in productIds)
            {
                var product = Presentation.eStoreContext.Current.Store.getProduct(productId);
                if (product != null)
                    products.Add(product);
            }
            return products;
        }


        public static IList<POCOS.Product> GetCompareSystemsByComponent()
        {
            IList<POCOS.Product> products = new List<POCOS.Product>();
            string partno = HttpContext.Current.Request["partno"].ToString();
           
            List<POCOS.Product_Ctos> ctoss = Presentation.eStoreContext.Current.Store.searchCTOSbyPN(partno);
            if (ctoss != null)
            {
                foreach (POCOS.Product_Ctos c in ctoss)
                {
                    products.Add((POCOS.Product)c);
                }
            }
            return products;
        }
        public static IList<POCOS.Product> GetCompareProductsByCategoryID()
        {
            List<POCOS.Product> products = new List<POCOS.Product>();

            string _category = HttpContext.Current.Request["category"].ToString();
            POCOS.ProductCategory productCategory = Presentation.eStoreContext.Current.Store.getProducts(_category, out products);

            return products;
        }
        /// <summary>
        /// Gets a "compare products" identifier list
        /// </summary>
        /// <returns>"compare products" identifier list</returns>
        public static List<string> GetCompareProductsIds()
        {
            List<string> productIds = new List<string>();
            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("eStoreCompareProducts");
            if ((compareCookie == null) || (compareCookie.Values == null))
                return productIds;
            string[] values = compareCookie.Values.GetValues("CompareProductIds");
            if (values != null)
                productIds = values.ToList<string>();
            return productIds;
        }

        public static List<string> GetCompareProductsIdsFromQueryString()
        {
            List<string> productIds = new List<string>();
            string parts = HttpContext.Current.Request["parts"];
            if ((parts == null) || ( string.IsNullOrEmpty(parts)))
                return productIds;
            string[] values = parts.Split(',');
            if (values != null)
                productIds = values.ToList<string>();
            return productIds;
        }
        /// <summary>
        /// Removes a product from a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        public static void RemoveProductFromCompareList(string productId)
        {
            var oldProductIds = GetCompareProductsIds();
            var newProductIds = new List<string>();
            newProductIds.AddRange(oldProductIds);
            newProductIds.Remove(productId);

            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("eStoreCompareProducts");
            if ((compareCookie == null) || (compareCookie.Values == null))
                return;
            compareCookie.Values.Clear();
            foreach (string newProductId in newProductIds)
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(compareCookie);
        }

        /// <summary>
        /// Adds a product to a "compare products" list
        /// </summary>
        /// <param name="productId">Product identifer</param>
        public static void AddProductToCompareList(string productId)
        {


            var oldProductIds = GetCompareProductsIds();
            var newProductIds = new List<string>();
            newProductIds.Add(productId);
            foreach (string oldProductId in oldProductIds)
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);

            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("eStoreCompareProducts");
            if (compareCookie == null)
                compareCookie = new HttpCookie("eStoreCompareProducts");
            compareCookie.Values.Clear();
            int maxProducts = 4;
            int i = 1;
            foreach (string newProductId in newProductIds)
            {
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
                if (i == maxProducts)
                    break;
                i++;
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(compareCookie);
        }

        public static void setProductIDs(List<string> productIds)
        {

 

            HttpCookie compareCookie = HttpContext.Current.Request.Cookies.Get("eStoreCompareProducts");
            if (compareCookie == null)
                compareCookie = new HttpCookie("eStoreCompareProducts");
            compareCookie.Values.Clear();
           
            foreach (string newProductId in productIds)
            {
                compareCookie.Values.Add("CompareProductIds", newProductId.ToString());
               
            }
            compareCookie.Expires = DateTime.Now.AddDays(10.0);
            HttpContext.Current.Response.Cookies.Set(compareCookie);
        }

        public string generateCompareUI(List<POCOS.Product> parts)
        {
            if (parts == null || !parts.Any())
                return string.Empty;

            StringBuilder sbCompare = new StringBuilder();
            StringBuilder sbCompareTop = new StringBuilder();
            StringBuilder sbCompareBottom = new StringBuilder();
            sbCompareTop.AppendFormat("<div class=\"eStore_compare_contentTop\"><div class=\"eStore_compare_left\"><div class=\"eStore_productBlock_name\">{0}</div><div class=\"eStore_productBlock_action\">{1}</div><div class=\"eStore_productBlock_link\">{2}</div></div><!--left--><div class=\"eStore_compare_right\"><div class=\"carouselBannerSingle\" id=\"comparingProducts\"><ul>"
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Name)
                , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Price)
                 , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet));
            foreach (var product in parts)
            {
                var hasPDF = !string.IsNullOrEmpty(product.dataSheetX);
                string priceHtml = string.Empty;
                if (!product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire))
                    priceHtml = Presentation.Product.ProductPrice.getPrice(product);
                sbCompareTop.AppendFormat("<li><div class=\"eStore_productBlock\"><div class=\"eStore_productBlock_pic\"><a href=\"#\" class=\"close\"><img src=\"{12}images/orderlistTable_close.png\" alt=\"remove this product\" /></a><a href=\"{7}\" ><img src=\"{0}\" alt=\"{13}\" title=\"{1}\" /></a>{9}</div><div class=\"eStore_productBlock_name\"><a href=\"{7}\" >{1}</a></div><div class=\"eStore_productBlock_action\"><div class=\"priceOrange\">{2}</div><a href=\"{7}\"  class=\"eStore_btn\">{10}</a></div><div class=\"eStore_productBlock_link\"><a href=\"{5}\" class=\"eStore_linkDataSheet{11}\">{6}</a></div></div></li>"
                    , product.thumbnailImageX
                    , product.name
                    , priceHtml
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation)
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart)
                    , product.dataSheetX
                    , hasPDF ? eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet) : "N/A"
                    , esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product))
                    , product.SProductID
                    ,
                    (product is POCOS.Product ?
                    string.Format("<span class=\"icon\"><img src=\"{1}images/{0}.gif\" alt=\"{0}\" /></span>", ((POCOS.Product)product).status.ToString(), esUtilities.CommonHelper.ResolveUrl("~/"))
                    : "")
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More)
                    , hasPDF ? "" : " not-active"
                    , esUtilities.CommonHelper.ResolveUrl("~/")
                    , System.Net.WebUtility.HtmlEncode(product.productDescX));
            }
            sbCompareTop.Append("</ul><div class=\"clearfix\"></div><div class=\"carousel-control\"><a id=\"prev\" class=\"prev\" href=\"#\"></a><a id=\"next\" class=\"next\" href=\"#\"></a></div></div></div></div>");

            var specificationAttributes = from spec in parts[0].specs
                                          select new
                                          {
                                              CatID = spec.CatID,
                                              Category = spec.LocalCatName,
                                              ID = spec.AttrID,
                                              Name = spec.LocalAttributeName
                                          };

            for (int i = 1; i < parts.Count; i++)
            {
                specificationAttributes = specificationAttributes.Union(
                     from spec in parts[i].specs
                     select new
                     {
                         CatID = spec.CatID,
                         Category = spec.LocalCatName,
                         ID = spec.AttrID,
                         Name = spec.LocalAttributeName
                     });
            }

            var specCategories = (from s in specificationAttributes
                                  group s by new { s.CatID, s.Category } into g
                                  select new
                                  {
                                      g.Key.CatID,
                                      g.Key.Category,
                                      Attributes = (from a in g
                                                    group a by new { a.ID } into ag
                                                    select new
                                                    {
                                                        ag.Key.ID,
                                                        Name = ag.Select(x => x.Name).FirstOrDefault()
                                                    })
                                  });

            sbCompareBottom.Append("<div class=\"eStore_compare_contentBottom\">");
            foreach (var sc in specCategories)
            {
                sbCompareBottom.Append("<div class=\"eStore_compare_contentCategory\">");
                sbCompareBottom.AppendFormat("<div class=\"eStore_compare_contentCategory_title eStore_openBox\">{0}</div>", sc.Category);
                sbCompareBottom.Append("<div class=\"eStore_compare_table\">");

                StringBuilder sbCompareBottomRight = new StringBuilder();
                sbCompareBottom.Append("<ul class=\"eStore_compare_left\">");
                foreach (var sa in sc.Attributes)
                {
                    sbCompareBottom.AppendFormat("<li>{0}</li>", sc.Attributes.Count() > 1 ? sa.Name : string.Empty);
                    sbCompareBottomRight.Append("<ul>");
                    foreach (var p in parts)
                    {
                        var spec = p.specs.FirstOrDefault(x => x.CatID == sc.CatID && x.AttrID == sa.ID);
                        sbCompareBottomRight.AppendFormat("<li>{0}</li>", spec == null ? "-" : spec.LocalValueName);
                    }
                    sbCompareBottomRight.Append("</ul>");
                }
                sbCompareBottom.Append("</ul>");

                sbCompareBottom.Append("<div class=\"eStore_compare_right\">");
                sbCompareBottom.Append(sbCompareBottomRight.ToString());
                sbCompareBottom.Append("</div>");

                sbCompareBottom.Append("</div>");
                sbCompareBottom.Append("</div>");
            }
            sbCompareBottom.Append("</div>");
            sbCompareTop.Append("<div class=\"clearfix\"></div>");
            sbCompareTop.Append(sbCompareBottom.ToString());
            return sbCompareTop.ToString();
        }
        public string generateCompareMobileUI(List<POCOS.Product> parts)
        {
            if (parts == null || !parts.Any())
                return string.Empty;

            StringBuilder sbCompare = new StringBuilder();

            sbCompare.Append("<div class=\"eStore_compareProduct_list eStore_compareProduct\">");
            sbCompare.AppendFormat("<h3 class=\"eStore_slideToggle\">Display <span>{0}</span> Results</h3>", parts.Count());
            sbCompare.Append("<ul>");

            foreach (var product in parts)
            {
                var hasPDF = !string.IsNullOrEmpty(product.dataSheetX);
                string priceHtml = string.Empty;
                if (!product.isIncludeSatus(POCOS.Product.PRODUCTMARKETINGSTATUS.Inquire))
                    priceHtml = Presentation.Product.ProductPrice.getPrice(product);

                sbCompare.Append("<li>");
                sbCompare.Append("<div class=\"eStore_compareProduct_listLeft\">");
                sbCompare.AppendFormat("<div class=\"title\"><a href=\"{1}\" >{0}</a></div>", product.name, esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product)));
                sbCompare.AppendFormat("<div class=\"priceOrange\">{0}</div>", priceHtml);
                sbCompare.AppendFormat("<a href=\"{0}\" class=\"eStore_btn\">{1}</a>", esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(product))
                    , eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_More));
                sbCompare.AppendFormat("<a href=\"{0}\" class=\"eStore_linkDataSheet{1}\">{2}</a>", product.dataSheetX,
                    (hasPDF ? "" : " not-active")
              , (hasPDF ? eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Datasheet) : "N/A")
                  );
                sbCompare.Append("</div>");
                sbCompare.Append("<div class=\"eStore_compareProduct_listRight\">");
                sbCompare.Append($"<a href=\"#\" class=\"close\"><img src=\"{esUtilities.CommonHelper.ResolveUrl("~/")}images/orderlistTable_close.png\" /></a>");
                sbCompare.Append("</div>");
                sbCompare.Append("<div class=\"clearfix\"></div>");
                sbCompare.Append("</li>");
            }
            sbCompare.Append("</ul>");
            sbCompare.Append("</div>");

            var specificationAttributes = from spec in parts[0].specs
                                          select new
                                          {
                                              CatID = spec.CatID,
                                              Category = spec.LocalCatName,
                                              ID = spec.AttrID,
                                              Name = spec.LocalAttributeName
                                          };

            for (int i = 1; i < parts.Count; i++)
            {
                specificationAttributes = specificationAttributes.Union(
                     from spec in parts[i].specs
                     orderby spec.seq, spec.LocalAttributeName
                     select new
                     {
                         CatID = spec.CatID,
                         Category = spec.LocalCatName,
                         ID = spec.AttrID,
                         Name = spec.LocalAttributeName
                     });
            }

            var specCategories = (from s in specificationAttributes
                                  group s by new { s.CatID, s.Category } into g
                                  select new
                                  {
                                      g.Key.CatID,
                                      g.Key.Category,
                                      Attributes = (from a in g
                                                    group a by new { a.ID } into ag
                                                    select new
                                                    {
                                                        ag.Key.ID,
                                                        Name = ag.Select(x => x.Name).FirstOrDefault()
                                                    })
                                  });


            foreach (var sc in specCategories)
            {
                sbCompare.Append("<div class=\"eStore_compareProduct_table eStore_compareProduct\">");
                if (sc.Attributes.Count() > 1)
                    sbCompare.AppendFormat("<h3>{0}</h3>", sc.Category);
                sbCompare.Append("<ul>");
                foreach (var sa in sc.Attributes)
                {

                    sbCompare.Append("<li>");
                    sbCompare.AppendFormat("<h4 class=\"eStore_slideToggle\">{0}</h4>", sa.Name);
                    sbCompare.Append("<div class=\"eStore_compareProduct_tableList\">");
                    sbCompare.Append("<table>");

                    foreach (var p in parts)
                    {
                        var spec = p.specs.FirstOrDefault(x => x.CatID == sc.CatID && x.AttrID == sa.ID);
                        sbCompare.Append("<tr>");
                        sbCompare.AppendFormat("<td class=\"title\">{0}</td>", p.name);
                        sbCompare.AppendFormat("<td>{0}</td>", spec == null ? "-" : spec.LocalValueName);
                    }
                    sbCompare.Append("</tr>");
                    sbCompare.Append("</table>");
                    sbCompare.Append("</li>");
                }

                sbCompare.Append("</ul>");
                sbCompare.Append("</div>");
            }

            return sbCompare.ToString();
        }

    }
}
