using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eStore.Presentation;

namespace eStore.UI.APIControllers
{
    public class SearchController : ApiController
    {
        [HttpGet]
        public string  getproducturl(string productid)
        {

            POCOS.Part prod = eStoreContext.Current.Store.getPart(productid);
            if (prod == null)
            {
                return esUtilities.CommonHelper.ResolveUrl("~/Search.aspx?skey=" + System.Web.HttpUtility.UrlEncode(productid));
            }
            else
            {
                if (prod.isOrderable() && (prod is POCOS.Product || prod is POCOS.PStoreProduct))
                    return esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod));
                else
                {
                    return esUtilities.CommonHelper.ResolveUrl("~/Search.aspx?skey=" + System.Web.HttpUtility.UrlEncode(productid));
                }
            }

        }
        [HttpGet]
        public Models.SearchResult Search(string keywords)
        {
            List<POCOS.ProductSearchOptimizedResult> items = eStoreContext.Current.Store.getMatchProducts2(keywords, 500, true);
            Models.SearchResult result = new Models.SearchResult();
            var products = items.Where(x => x.Group == "Product").ToList();
            var categories = items.Where(x => x.Group == "Category").ToList();
            result.Keywords = keywords;
            result.Count = products.Select(x => x.SProductID).Distinct().Count();
            var trems = eStoreContext.Current.Store.getftsparser(keywords);
            //result.SearchTerms = string.Join(" ", eStoreContext.Current.Store.getftsparser(keywords)); // 暂时去掉分词
            if (products != null && products.Count() > 0)
            {
                var exactmatched = products.FirstOrDefault(x => x.SProductID.Equals(keywords, StringComparison.OrdinalIgnoreCase) || x.DisplayPartno.Equals(keywords, StringComparison.OrdinalIgnoreCase));
                if (exactmatched != null)
                {
                    POCOS.Part prod = eStoreContext.Current.Store.getPart(exactmatched.SProductID);
                    if (prod != null && (prod is POCOS.Product || prod is POCOS.PStoreProduct))
                    {
                        result.exactItem = esUtilities.CommonHelper.ResolveUrl(Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod));
                    }
                }

                if (string.IsNullOrEmpty(result.exactItem))
                {
                    result.groups = (from s in products
                                    group s by new { s.CategoryID } into g
                                    select new Models.SearchGroup(eStoreContext.Current.Store.profile, "eStore", g.Key.CategoryID)
                                    {
                                         Count = g.Select(x => x.SProductID).Distinct().Count()
                                     })
                     .Where(x => x.MiniSite == eStoreContext.Current.MiniSite)
                     .ToList();

                    result.items = pagedItems(eStoreContext.Current.Store.profile, products, 1, 9, null, keywords);
                }

            }

            if (categories != null && categories.Count() > 0)
            {
                result.categories = (
                    from c in categories
                    where c.CategoryID != null
                    let estorecategory = eStoreContext.Current.Store.getProductCategory(c.CategoryID.GetValueOrDefault())
                    where estorecategory != null
                    orderby c.RANK descending
                    select new Models.Category(estorecategory)
                    ).Take(6).ToList();
            }
            foreach (string item in trems)
            {
                result.categories.ForEach((c) => {
                    c.Name = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Name);
                    c.Description = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Description);
                });
            }
            
            return result;
        }
        private List<Models.SearchItem> pagedItems(POCOS.Store store, List<POCOS.ProductSearchOptimizedResult> items, int page, int pagesize, string SortAsc, string keywords)
        {
            if (items == null || items.Any() == false)
                return null;
            List<string> pageditems = new List<string>();
            List<Models.SearchItem> result = new List<Models.SearchItem>();
            var trems = eStoreContext.Current.Store.getftsparser(keywords);
            switch (SortAsc)
            {
                case "Low":
                    pageditems = items.OrderBy(x => x.ListingPrice.GetValueOrDefault(0) == 0 ? 1 : 0)
                        .ThenBy(x => x.ListingPrice).Select(x => x.SProductID).Distinct().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case "High":
                    pageditems = items.OrderByDescending(x => x.ListingPrice).Select(x => x.SProductID).Distinct().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case "New":                    
                    pageditems = items.OrderBy(x => esUtilities.Validator.isBitInclude(eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.NEW, x.MarketingStatus) ? 0 : 1)
                        .Select(x => x.SProductID).Distinct().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case "OnSale":
                    pageditems = items.OrderBy(x => esUtilities.Validator.isBitInclude(eStore.POCOS.Product.PRODUCTMARKETINGSTATUS.PROMOTION, x.MarketingStatus) ? 0 : 1)
                        .Select(x => x.SProductID).Distinct().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    break;
                case "Depth":
                default:
                    pageditems = items.OrderByDescending(x => x.RANK).Select(x => x.SProductID).Distinct().Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    break;
            }
            List<POCOS.Part> parts = (new POCOS.DAL.PartHelper()).prefetchPartList(store.StoreID, pageditems);

            foreach (var rs in pageditems)
            {
                var part = parts.FirstOrDefault(p => p.SProductID.Equals(rs));
                if(part != null)
                    result.Add(new Models.SearchItem(part));
            }

            foreach (var item in trems)
            {
                result.ForEach((c) => {
                    c.Name = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Name);
                    c.Description = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Description);
                });
            }          

            return result;
        }
        [HttpGet]
        public List<Models.SearchItem> PagedItems(string keywords, int page, int pagesize, string selectedGroup, string selectedSource, string SortAsc)
        {
            List<POCOS.ProductSearchOptimizedResult> items = eStoreContext.Current.Store.getMatchProducts2(keywords, 500, true).Where(c=>c.Group.Equals("Product")).ToList();
            if (items != null && items.Any())
            {

                if (!string.IsNullOrEmpty(selectedGroup))
                {
                    var attrs = selectedGroup.Split(',');
                    var fitered = items.Where(x => attrs.Contains(x.CategoryID.GetValueOrDefault(0).ToString())).ToList();
                    return pagedItems(eStoreContext.Current.Store.profile, fitered, page, pagesize, SortAsc, keywords);
                }
                else
                {
                    return pagedItems(eStoreContext.Current.Store.profile, items, page, pagesize, SortAsc, keywords);
                }
            }
            else
            {
                return new List<Models.SearchItem>() ;
            }
        }

        //[HttpGet]
        //public List<Models.SearchItem> PagedItems(string keywords, int page, int pagesize, string selectedAttr, string selectedSource, bool? isSortAsc)
        //{
        //    List<POCOS.ProductSearchOptimizedResult> items = eStoreContext.Current.Store.getMatchProducts2(keywords, 500, true).Where(c => c.Group.Equals("Product")).ToList();
        //    if (items != null && items.Any())
        //    {
        //        if (!string.IsNullOrEmpty(selectedAttr))
        //        {
        //            var attrs = selectedAttr.Split(',');
        //            var fitered = items.Where(x => attrs.Contains(x.CategoryID.GetValueOrDefault(0).ToString())).ToList();
        //            return pagedItems(eStoreContext.Current.Store.profile, fitered, page, pagesize, isSortAsc);
        //        }
        //        else
        //        {
        //            return pagedItems(eStoreContext.Current.Store.profile, items, page, pagesize, isSortAsc);
        //        }

        //    }
        //    else
        //    {
        //        return new List<Models.SearchItem>();
        //    }
        //}

        [HttpGet]
        public Models.MarketingResourcePage SearchMarketings(string keywords)
        {
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("ShowSearchMarketing"))
            {
                var rms = eStoreContext.Current.Store.GetMarketingResourceList(keywords, keywords);
                var tt = new Models.MarketingResourcePage()
                {
                    Count = rms.Count,
                    Groups = rms.GroupBy(m => m.APPNAME).Select(c => new { Key = string.IsNullOrEmpty(c.Key) ? "Other" : c.Key, Count = c.Count() }).ToList(),
                    MarketingResources = rms.OrderByDescending(c => c.Depth).Take(3).Select(c => new Models.MarketingResource(c)).ToList()
                };
                var trems = eStoreContext.Current.Store.getftsparser(keywords);
                foreach (var item in trems)
                {
                    tt.MarketingResources.ForEach((c) =>
                    {
                        c.Title = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Title);
                        c.ShortDesc = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.ShortDesc);
                    });
                }
                
                return tt;
            }
            else
                return new Models.MarketingResourcePage();
        }

        [HttpGet]
        public List<Models.MarketingResource> PagedMarketItems(string keywords, int page, string selectedGroup)
        {
            var rms = eStoreContext.Current.Store.GetMarketingResourceList(keywords, keywords);
            if (!string.IsNullOrEmpty(selectedGroup)) {
                if (selectedGroup == "Other")
                    rms = rms.Where(c => c.APPNAME == null).ToList();
                else
                    rms = rms.Where(c => selectedGroup == c.APPNAME).ToList();
            }
            
            if (rms.Any())
            {
                var tt = rms.OrderByDescending(c => c.Depth).Skip((page - 1) * 3).Take(3).Select(c => new Models.MarketingResource(c)).ToList();
                var trems = eStoreContext.Current.Store.getftsparser(keywords);
                foreach (var item in trems)
                {
                    tt.ForEach((c) => {
                        c.Title = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.Title);
                        c.ShortDesc = esUtilities.StringUtility.ReplaceString(item, "<span class='b'>{0}</span>", c.ShortDesc);
                    });
                }                
                return tt;
            }
                
            else
                return new List<Models.MarketingResource>();
        }
    }
}