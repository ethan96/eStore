using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ProductCategoryHelper : Helper
    {

        #region Business Read

        private static CachePool cache = CachePool.getInstance();

        /// <summary>
        /// This function should not be call , replaced y getCTOSRootProductCategory or getStandardRootProductCategory
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getProductCategories(string storeid)
        {

            var _pcs = (from pc in context.ProductCategories.Include("ChildCategories")
                        where pc.Storeid == storeid && pc.ParentCategoryID == null && pc.Publish==true
                        select pc);

            return _pcs.ToList();

        }

        public List<ProductCategory> getAllProductCategories(string storeid)
        {

            var _pcs = (from pc in context.ProductCategories
                        where pc.Storeid == storeid && pc.Publish == true
                        select pc);

            return _pcs.ToList();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getAllStandardProductCategory(string storeid)
        {
            var _pcs = (from pc in context.ProductCategories.Include("ChildCategories")
                        where pc.Storeid == storeid && pc.CategoryType.Equals("StandardCategory",StringComparison.OrdinalIgnoreCase)
                            && (pc.Publish.HasValue && pc.Publish.Value)
                        select pc);

            return _pcs.ToList();
        }


        public List<ProductCategory> getStandardProdcutCategoryIncludeReplication(string storeid, string sourceStoreid)
        {

            var _pclist = from map in context.ReplicationCategoryProductsMappings.Include("ProductCategory1").Include("ProductCategory")
                           where map.StoreIDTo == storeid && map.StoreIDFrom == sourceStoreid
                               && map.ProductCategory1 != null && map.ProductCategory1.Publish.Value
                           select map.ProductCategory1;
            return _pclist.OrderByDescending(c=>c.CreatedDate).ToList();
        }

    

        public ProductCategory getProductCategory(int categoryid, string storeid, Boolean includeNonpublished = false)
        {
            ProductCategory _pc = null;

            if (includeNonpublished)
            {
                _pc = (from pc in context.ProductCategories.Include("ChildCategories")
                           where pc.Storeid == storeid && pc.CategoryID == categoryid
                           select pc).FirstOrDefault();
            }
            else
            {
                _pc = (from pc in getStoreProductCategories(storeid)
                           where pc.Storeid == storeid && pc.CategoryID == categoryid && pc.Publish == true
                           select pc).FirstOrDefault();
            }

            if (_pc != null)
                _pc.helper = this;

            return _pc;
        }

        /// <summary>
        /// Get Product Category by category path
        /// </summary>
        /// <param name="categorypath"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>

        public ProductCategory getProductCategory(string categorypath, string storeid, Boolean includeNonPublished = false)
        {
            ProductCategory _pc = null;
            if (includeNonPublished)
            {
                _pc = (from pc in context.ProductCategories.Include("ChildCategories").Include("ProductCategroyMappings")
                           where pc.Storeid == storeid && pc.CategoryPath.Equals(categorypath,StringComparison.OrdinalIgnoreCase)
                           select pc).FirstOrDefault();
            }
            else
            {
                _pc = (from pc in getStoreProductCategories(storeid)
                           where pc.Storeid == storeid && pc.CategoryPath.Equals(categorypath,StringComparison.OrdinalIgnoreCase) && pc.Publish == true
                           select pc).FirstOrDefault();
            }

            if (_pc != null)
                _pc.helper = this;

            return _pc;

        }

        public ProductCategory getProductCategoryForProductCategorySEO(string categorypathseo, string storeid, Boolean includeNonPublished = false)
        {
            ProductCategory _pc = null;
            if (includeNonPublished)
            {
                _pc = (from pc in context.ProductCategories.Include("ChildCategories").Include("ProductCategroyMappings")
                       where pc.Storeid == storeid && pc.CategoryPathSEO.Equals(categorypathseo, StringComparison.OrdinalIgnoreCase)
                       select pc).FirstOrDefault();
            }
            else
            {
                _pc = (from pc in getStoreProductCategories(storeid)
                       where pc.Storeid == storeid && pc.CategoryPathSEO.Equals(categorypathseo, StringComparison.OrdinalIgnoreCase) && pc.Publish == true
                       select pc).FirstOrDefault();
            }

            if (_pc != null)
                _pc.helper = this;

            return _pc;

        }

        /// <summary>
        /// search ProductCategory of StartWith CategoryPath for campaign
        /// </summary>
        /// <param name="categorypath"></param>
        /// <param name="storeid"></param>
        /// <param name="includeNonPublished"></param>
        /// <returns></returns>
        public List<ProductCategory> searchProductCategoryofStartWithCategoryPath(List<String> categorypath, string storeid, Boolean includeNonPublished = false)
        {
            List<ProductCategory> _pc = null;
            if (includeNonPublished)
            {
                _pc = (from pc in context.ProductCategories.Include("ChildCategories").Include("ProductCategroyMappings")
                       from path in categorypath
                       where pc.Storeid == storeid && pc.CategoryPath.ToUpper().StartsWith(path.ToUpper())
                       select pc).ToList();
            }
            else
            {
                _pc = (from pc in context.ProductCategories.Include("ChildCategories").Include("ProductCategroyMappings")
                       from path in categorypath
                       where pc.Storeid == storeid && pc.CategoryPath.ToUpper().StartsWith(path.ToUpper()) && pc.Publish == true
                       select pc).ToList();
            }

            if (_pc != null)
            {
                foreach (ProductCategory pc in _pc)
                    pc.helper = this;
            }
                

            return _pc;

        }

        /// <summary>
        /// For OM return rule set details.
        /// </summary>
        /// <param name="rulesetid"></param>
        /// <returns></returns>

        public List<VRule> getRules(int rulesetid , string storeid, out RuleSet ruleset)
        {
            var _r = (from r in context.VRules
                     where r.RuleSetId == rulesetid 
                     select  r).Distinct();  

            var _rset = (from r in context.RuleSets.Include("RuleSetDetails")
                        where r.RuleSetID == rulesetid
                        select r).FirstOrDefault();

            ruleset = _rset;
            return _r.ToList();
        
        }

        /// <summary>
        /// Cached whole store productcategories
        /// Return published CTOS root product category
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getCTOSRootProductCategory(string storeid, Boolean refreshCache = false)
        {
            List<ProductCategory> categories = getStoreProductCategories(storeid).Where(x => x.ParentCategoryID == null && x.CategoryType.ToLower() == "ctoscategory").ToList();

            if (!refreshCache && categories != null)
            {
                return categories;
            }
            else
            {
                var _pc = (from pc in context.ProductCategories.Include("ChildCategories")
                           where pc.Storeid == storeid && pc.Publish == true && pc.ParentCategoryID == null && pc.CategoryType.ToLower() == "ctoscategory"
                           select pc);

                foreach (ProductCategory pc in _pc)
                {
                    pc.helper = this;
                }
                return _pc.ToList();
            }        
        }

        public List<POCOS.Application> getRootApplication(string storeid, Boolean refreshCache = false)
        {
            List<Application> categories =(  List<Application> ) getStoreProductCategories(storeid).OfType<Application>().Where(x=>x.ParentCategoryID ==null ).ToList();

            if (!refreshCache && categories != null)
            {
                return categories;
            }
            else
            {
                var _pc = (from pc in context.ProductCategories.OfType<Application>().Include("ChildCategories")
                           where pc.Storeid == storeid 
                           && pc.Publish == true && pc.ParentCategoryID == null 
                           && pc.CategoryType.ToLower() == "application"
                           select pc);

                foreach (Application pc in _pc)
                {
                    pc.helper = this;
                }
                return _pc.ToList();
            }   
        }
        public List<ProductCategory> getuStoreRootCategory(string storeid, Boolean refreshCache = false)
        {
            List<ProductCategory> categories = getStoreProductCategories(storeid).Where(x => x.ParentCategoryID == null && x.CategoryType.ToLower() == "ustorecategory").ToList();

            if (!refreshCache && categories != null)
            {
                return categories;
            }
            else
            {
                var _pc = (from pc in context.ProductCategories.Include("ChildCategories")
                           where pc.Storeid == storeid && pc.Publish == true && pc.ParentCategoryID == null && pc.CategoryType.ToLower() == "ustorecategory"
                           orderby pc.Sequence
                           select pc);

                foreach (ProductCategory pc in _pc)
                {
                    pc.helper = this;
                }

         
                return _pc.ToList();
            }
        }
        public List<ProductCategory> getStoreProductCategories(string storeid, bool up = true)
        {
            string key = storeid + ".StoreProductCategories";
            if (up == false)
                key = storeid + ".StoreProductCategoriesAll";
            Object categories = cache.getObject(key);

            if (categories != null)
            {
                return (List<ProductCategory>)categories;
            }
            else
            {
                var _pc = (from pc in context.ProductCategories.Include("CategoriesGlobalResources")
                           where pc.Storeid == storeid && (up == false ? true : pc.Publish == true )
                            && (string.IsNullOrEmpty(pc.CategoryStatus) || !"delete".Equals(pc.CategoryStatus,StringComparison.OrdinalIgnoreCase))
                           select pc);

                foreach (ProductCategory pc in _pc)
                {
                    pc.helper = this;
                }

                //Add store product categories to cache.
                cache.cacheObject(key, _pc.ToList());
                return _pc.ToList();
            }
        }

        public List<ProductCategory> getchildCategories(ProductCategory category)
        {
            if (category == null)
                return new List<ProductCategory>();
            return getStoreProductCategories(category.Storeid).Where(x => x.ParentCategoryID == category.CategoryID
                && x.MiniSiteID ==category.MiniSiteID
                ).OrderBy(x=>x.Sequence).ThenBy(x=>x.localCategoryNameX).ToList();
        }

        public ProductCategory getParentCategory(ProductCategory category, bool up = true)
        {
            if (category == null)
                return null;
            return getStoreProductCategories(category.Storeid, up).FirstOrDefault(x => x.CategoryID == category.ParentCategoryID
                && x.MiniSiteID == category.MiniSiteID
                );
        }


        /// <summary>
        ///  Return published Standard root  product category
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getStandardRootProductCategory(string storeid, Boolean refreshCache = false)
        {
              
             List<ProductCategory> categories = getStoreProductCategories(storeid).Where(x =>x.ParentCategoryID ==null && x.CategoryType.ToLower() == "standardcategory").ToList();

             if (!refreshCache && categories != null)
             {
                 return categories;
             }
             else
             {
                 var _pc = (from pc in context.ProductCategories.Include("ChildCategories")
                            where pc.Storeid == storeid && pc.Publish == true && pc.ParentCategoryID == null && pc.CategoryType.ToLower() == "standardcategory"
                            select pc);

                 foreach (ProductCategory pc in _pc)
                 {
                     pc.helper = this;
                 }

                
                 return _pc.ToList();
             }
        }

        public List<ProductCategory> getTopLevelCategories(string storeid, Boolean refreshCache = false)
        {

            List<ProductCategory> categories = getStoreProductCategories(storeid).Where(x => x.ParentCategoryID == null && x.MiniSiteID == null).ToList();
            return categories;
        }

        /// <summary>
        /// Return Standard category for OM
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getOMStandardRootProductCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "standardcategory", refreshCache);
        }

        /// <summary>
        /// Return CTOS productcategory for OM
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getOMCTOSRootProductCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "ctoscategory", refreshCache);
        }

        /// <summary>
        /// get product category by paths 
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getProductCategories(List<string> paths, string storeid)
        {
            var ls = (from pc in context.ProductCategories.Include("ProductCategroyMappings")
                      where pc.Storeid == storeid && paths.Contains(pc.CategoryPath)
                      select pc).ToList();
            foreach (var li in ls)
                li.helper = this;
            return ls;
        }

        /// <summary>
        /// Return Application category for OM
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getOMRootApplicationCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "application", refreshCache);
        }
        public List<ProductCategory> getOMRootuStoreCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "ustorecategory", refreshCache);
        }

        public List<ProductCategory> getOMRootPromotionCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "PromotionCategory", refreshCache);
        }

        public List<ProductCategory> getOMRootDeliveryCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "DeliveryCategory", refreshCache);
        }

        private List<ProductCategory> getOMProductRootCategory(String storeID, String categoryType, Boolean refreshCache)
        {
            string key = storeID + "_OM" + categoryType + "RootCategoryList";
            List<ProductCategory> productCategories = null;

            if (!refreshCache)
            {
                Object categories = cache.getObject(key);

                if (categories != null)
                {
                    var count = (from pc in context.ProductCategories
                                 where pc.Storeid == storeID && pc.ParentCategoryID == null && pc.CategoryType.ToLower() == categoryType
                                 && (pc.CategoryStatus == null || (pc.CategoryStatus != null && pc.CategoryStatus.ToLower() != "delete"))
                                 select pc.CategoryName).Count();

                    productCategories = (List<ProductCategory>)categories;
                    if (productCategories.Count() != count)
                    {
                        cache.releaseCacheItem(key);
                        productCategories = null;
                    }
                }
            }

            if (productCategories == null)
            {
                var _pc = (from pc in context.ProductCategories.Include("ChildCategories")
                           where pc.Storeid == storeID && pc.ParentCategoryID == null && pc.CategoryType.ToLower() == categoryType
                           && (pc.CategoryStatus == null || (pc.CategoryStatus != null && pc.CategoryStatus.ToLower() != "delete"))
                           select pc);
                productCategories = _pc.ToList();
                foreach (ProductCategory pc in productCategories)
                    pc.helper = this;
                
                //Add store product categories to cache.
                cache.cacheObject(key, _pc.ToList());
            }

            return productCategories;
        }

        public List<ProductCategory> getOMRootApplicationProductCategory(string storeid, Boolean refreshCache = false)
        {
            return getOMProductRootCategory(storeid, "ApplicationCategory", refreshCache);
        }
        /// <summary>
        /// get CTOS products under provided category, using old mapping table. 
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public List<Product> getCategoryProductList(ProductCategory pc)
        {
            try
            {
                List<ProductCategroyMapping> mappedprod = new List<ProductCategroyMapping>();
                mappedprod = (from pcm in context.ProductCategroyMappings
                              where pcm.StoreID == pc.Storeid && pcm.CategoryID == pc.CategoryID
                              select pcm).ToList();

                var products = getOrderableProducts(mappedprod, pc.Storeid);
                if (products.Any())
                {
                    foreach (POCOS.Product p in products)
                    {
                        var maritem = mappedprod.FirstOrDefault(c => c.SProductID == p.SProductID);
                        p.CategorySeq = maritem.Seq;
                    }
                }
                return products;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<Product>();
            }
        }

        public List<SimpleProduct> getSimpleProducts(ProductCategory pc)
        {
            try
            {
                POCOS.Store store = (new StoreHelper()).getStorebyStoreid(pc.Storeid);
                var partValidProductStatus = store.getStringSetting("ProductLogisticsStatus").Split(',');
                var productValidProductStatus = store.getStringSetting("ValidProductStatus").Split(',');

                var mappedprod = (from pcm in context.ProductCategroyMappings
                                  from p in context.SimpleProducts
                                    where pcm.StoreID == pc.Storeid && pcm.CategoryID == pc.CategoryID && p.StoreID == pcm.StoreID
                                        && p.SProductID == pcm.SProductID &&
                                        (p.StockStatus==null  || p.StockStatus=="" ||
                                        partValidProductStatus.Contains(p.StockStatus) )&& productValidProductStatus.Contains(p.Status)
                                        && p.PublishStatus
                              select
                              new { product=p,
                              seq=pcm.Seq}).ToList();
                mappedprod.ForEach(x => x.product.Sequnce = x.seq);
                return mappedprod.Select(x=>x.product).ToList();
            }
            catch (Exception ex)
            {
                return new List<SimpleProduct>();
            }
        }


        /// <summary>
        /// This method takes product and product category mapping and return the products in the mapping.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private List<Product> getOrderableProducts(List<ProductCategroyMapping> map, String storeId)
        {
            List<Product> products = null;
            try
            {
                //string pids = string.Join(",", (map.Select(x => x.SProductID).ToArray()));
                PartHelper parthelper = new PartHelper();
                List<Part> prefetched = parthelper.prefetchPartList(storeId, map.Select(x => x.SProductID).ToList());

                List<Product> rlt = new List<Product>();
                foreach (Part p in prefetched)
                {
                    if (p is Product && p.isOrderable())
                    {
                        rlt.Add(p as Product);
                    }
                }
                products = (from p in rlt
                            from m in map
                            where p.SProductID == m.SProductID && p.StoreID==m.StoreID 
                            orderby m.Seq, p.SProductID
                            select p
                                 ).ToList();
            }
            catch (Exception)
            {
                products = new List<Product>();
            }

            return products;
        }

        /// <summary>
        /// Get component Products under provided category, using rules set
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>

        

          public List<Product> getComponentProductsbyCategory(ProductCategory pc)
        {


            return pc.productList;

            //List<Product> products = new List<Product>();

            //var _cnt = (from c in context.RuleSetDetails
            //            where c.RuleSetId==pc.RuleSetId
            //            select c).Count();

            //var _baseproduct = (from p in context.Parts.OfType<Product>()
            //                    from m in context.VProductMatrices
            //                    from r in context.RuleSetDetails
            //                    where p.SProductID == m.ProductNo && p.StoreID == pc.Storeid
            //                    && r.RuleSetId == pc.RuleSetId
            //                    && p.PublishStatus == true
            //                    && r.AttributeCatId == m.CatID
            //                    && r.AttributeId == m.AttrID
            //                    && r.AttributeValueId == m.AttrValueID
            //                    && r.AttributeId == 999998
            //                    select p).ToList();
            //                //select  p).Distinct().ToList();

            //var _products = (from p in context.Parts.OfType<Product>()
            //                 from m in context.VProductMatrices
            //                 from r in context.RuleSetDetails
            //                 where p.SProductID == m.ProductNo && p.StoreID == pc.Storeid
            //                 && r.RuleSetId == pc.RuleSetId
            //                 && r.AttributeCatId == m.CatID
            //                 && r.AttributeId == m.AttrID
            //                 && r.AttributeValueId == m.AttrValueID
            //                 && r.AttributeId != 999998
            //                 group m by new { m.ProductNo, m.AttrID, m.AttrValueID, m.CatID } into mgroup
            //                 where mgroup.Count() > 0
            //                 select   mgroup  ).ToList();

            //PartHelper phelper = new PartHelper();

            //foreach (var a in _products) {
            //    Product p = (Product) phelper.getPart(a.Key.ToString(),pc.Store);
            //    products.Add(p);
            //}

            ////foreach (var a in _baseproduct)
            ////{
            ////    Product p = (Product)phelper.getPart(a.Key.ToString(), pc.Store);
            ////    products.Add(p);
            ////}

            //products.AddRange(_baseproduct);
            //return products;


        
        }

        /// <summary>
        /// For You are here
        /// </summary>
        /// <param name="part"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>

          public List<ProductCategory> getProductCategoryByPartno(Part part, string storeid)
          {
              //Store store = new StoreHelper().getStorebyStoreid(storeid);
              //string validproductstatus = store.Settings["ValidProductStatus"];
              //string[] activestatus = validproductstatus.Replace("'", "").Split(',');

              var _ctosPC = (from p in context.ProductCategroyMappings

                             where p.SProductID == part.SProductID
                             && p.StoreID == part.StoreID

                             select p.CategoryID).Distinct().ToList();
              List<ProductCategory> result = new List<ProductCategory>();
              foreach (var pcid in _ctosPC)
              {
                  ProductCategory pc = this.getProductCategory(pcid, storeid);
                  if (pc != null)
                      result.Add(pc);
              }
              return result;
          }

        /// <summary>
        /// get specific Product category
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="categorypath"></param>
        /// <returns></returns>

        public ProductCategory getProductCategories(string storeid, string categorypath, out List<Product> products)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeid);
            string validproductstatus = store.Settings["ValidProductStatus"];
            string[] validarray = new string[10];
            if (string.IsNullOrEmpty(validproductstatus) == false)
            {
                validarray = validproductstatus.Split(',');
            }
            
            var _pcs = (from pc in context.ProductCategories.Include("ChildCategories")
                        where pc.Storeid == storeid && pc.CategoryPath == categorypath && pc.Publish==true
                        select pc).FirstOrDefault();

            var _products = from p in context.ProductCategories
                            from x in context.ProductCategroyMappings
                            from pro in context.Parts.OfType<Product>()
                            where p.CategoryPath == categorypath && p.Storeid == storeid && x.StoreID == storeid
                            && pro.SProductID == x.SProductID && pro.StoreID == x.StoreID && x.CategoryID == p.CategoryID
                            && pro.PublishStatus == true && validarray.Contains(pro.Status) && p.Publish == true
                            orderby x.Seq, p.LocalCategoryName
                            select pro;

            if (_products == null)
                products = new List<Product>();
            else
                products = _products.ToList();

            PartHelper partHelper = new PartHelper();
            partHelper.setContext(this.context);  //assign product category context to parthelper to assure they use the same context
            foreach (Product p in products) {
                p.parthelper = partHelper;
                /*
                if (p._helper == null)
                    p._helper = new PartHelper();

                p._helper.context  = this.context ;
                 * */
            }


            if (_pcs != null)
                _pcs.helper = this;

            return _pcs;

        }


        /// <summary>
        /// For OM returns widgets used by given product category
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public List<Widget> getWidgetsByProductCategory(ProductCategory pc)
        {
            if (pc == null) return null;

            try
            {

                var _widgets = (from wp in context.Widgets
                                where wp.CategoryIDs.Contains(pc.CategoryPath) && wp.WidgetPage.StoreID == pc.Storeid
                                select wp);

                if (_widgets != null)
                    return _widgets.ToList();
                else
                    return new List<Widget>();

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// For OM , return menus links to the given productcategory
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>
        public List<Menu> getMenuByProductCategory(ProductCategory pc)
        {
            if (pc == null) return null;

            try
            {
                var _menus = (from mn in context.Menus 
                                where mn.StoreID == pc.Storeid && mn.CategoryPath.ToUpper() == pc.CategoryPath.ToUpper()
                                select mn);

                if (_menus != null)
                    return _menus.ToList();
                else
                    return new List<Menu>();

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<string> getAllCategoryType(string storeId)
        {
            var categoryTypeList = (from p in context.ProductCategories
                                where p.Storeid == storeId && p.Publish == true 
                                select p.CategoryType).Distinct().ToList();

            return categoryTypeList;
        }

        public List<LanguageResource> getCategoryResourceTemplate(string storeId, int languageId, string categoryType)
        {
            var categoryList = (from p in context.ProductCategories
                                   from l in context.Languages
                                   let cc = context.CategoriesGlobalResources.Where(x => x.StoreId == storeId && x.LanguageId == languageId && x.Categoryid == p.CategoryID).FirstOrDefault()
                                where p.Storeid == storeId && p.CategoryType == categoryType && p.Publish == true && l.Id == languageId
                                   select new LanguageResource
                                   {
                                       DocId = p.CategoryID,
                                       StoreId = p.Storeid,
                                       DisplayKeyName = p.CategoryName,
                                       DisplayKeyValue = p.Description,
                                       DisplayExtendedDesc = p.ExtendedDescription,
                                       LanguageName = l.Name + "(" + l.Location + ")",
                                       LocalName = (cc == null ? "" : cc.LocalName),
                                       LocalDesc = (cc == null ? "" : cc.LocalDescription),
                                       LocalExtendedDesc = (cc == null ? "" : cc.LocalExtDescription)
                                   }).Distinct().ToList();

            List<LanguageResource> tranList = new List<LanguageResource>();
            foreach (LanguageResource item in categoryList)
            {
                LanguageResource categoryItem = tranList.FirstOrDefault(p => p.DocId == item.DocId);
                if (categoryItem == null)
                    tranList.Add(item);
            }
            return tranList;
        }

        /// <summary>
        /// 根据category path获取目标store相似的category
        /// </summary>
        /// <param name="fromStore"></param>
        /// <param name="toStore"></param>
        /// <param name="categorypath"></param>
        /// <returns></returns>
        public List<ProductCategory> GetSearchDiffSiteCategory(string fromStore, string toStore, string categorypath)
        {
            List<ProductCategory> categories = new List<ProductCategory>();
            var ls = context.spSearchDiffSiteCategory(fromStore, toStore, categorypath).OrderByDescending(c=>c.level).ToList();
            if(ls.Any())
                categories = searchProductCategoryofStartWithCategoryPath(ls.Select(c => c.CategoryPath).ToList(), toStore).ToList();
            return (from ca in categories
                    from item in ls
                    where ca.CategoryPath == item.CategoryPath
                    orderby item.level descending
                    select ca).ToList();
        }

        #endregion

        #region Create Update Delete
        public int save(ProductCategory _productcategory)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_productcategory == null || _productcategory.validate() == false) return 1;
            //Try to retrieve object from DB
            ProductCategory _exist_productcategory = getProductCategory(_productcategory.CategoryID,_productcategory.Storeid, true )  ;
            try
            {
                if (_exist_productcategory == null)  //object not exist 
                {
                    //Insert
                    context.ProductCategories.AddObject(_productcategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.ProductCategories.ApplyCurrentValues(_productcategory);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(ProductCategory _productcategory)
        {

            if (_productcategory == null || _productcategory.validate() == false) return 1;
            try
            {
                context.DeleteObject(_productcategory);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region OM functions

        /// <summary>
        /// get Product Category List By keyword
        /// </summary>
        /// <param name="KeyWords"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<ProductCategory> getProductCategoryByKeywords(string KeyWords, string storeid,string categoryType)
        {
            if (!string.IsNullOrEmpty(KeyWords) && !string.IsNullOrEmpty(storeid))
            {
                var CategoryList = from c in context.ProductCategories.Include("ChildCategories").Include("ProductCategroyMappings")
                                   where c.Storeid == storeid && (!string.IsNullOrEmpty(c.CategoryPath)) &&
                                   (c.CategoryName.ToUpper().Contains(KeyWords.ToUpper()) || c.CategoryPath.ToUpper().Contains(KeyWords.ToUpper())) && c.CategoryType.ToLower() == categoryType.ToLower()
                                   orderby c.CategoryName
                                   select c;

                List<ProductCategory> rlt = CategoryList.ToList();
                foreach (ProductCategory pc in rlt)
                    pc.helper = this;

                return rlt;
            }
            else
                return new List<ProductCategory>();
        }

        public bool checkProductCategory_CategoryPathSEO(string storeid,ProductCategory myproductCategory)
        {
            bool isDuplicate = false;

            var CategoryList = from c in context.ProductCategories
                               where c.Storeid == storeid 
                               && (c.CategoryPath == myproductCategory.CategoryPathSEO || c.CategoryPathSEO == myproductCategory.CategoryPathSEO)
                               && c.CategoryID == myproductCategory.CategoryID
                               select c;
            // 同筆資料
            List<ProductCategory> rlt = CategoryList.ToList();

            if (rlt.Count > 0)
            {
                return isDuplicate;
            }

            CategoryList = null;

            CategoryList = from c in context.ProductCategories
                               where c.Storeid == storeid
                               && (c.CategoryPath == myproductCategory.CategoryPathSEO || c.CategoryPathSEO == myproductCategory.CategoryPathSEO)
                               select c;

            rlt = CategoryList.ToList();

            if (rlt.Count > 0)
            {
                isDuplicate = true;
            }

            return isDuplicate;
        }

        #endregion

        #region Others
        /// <summary>
        /// get all products in endnodes categories
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="categorypaths">CATEGORYPaths join category path by |</param>
        /// <returns></returns>
        public List<string> getAllProductsInHierarchicalCategories(string storeid, string categorypaths)
        {
            return context.getAllProductsInHierarchicalCategories(storeid, categorypaths).ToList();
        }

        private static string myclassname()
        {
            return typeof(ProductCategoryHelper).ToString();
        }
        #endregion
        
        #region Intel

        public bool syncCategoryAndProduct(ProductCategory parentCategory, List<ProductCategory> subcategories, List<ProductCategroyMapping> mappings)
        {
            return true;
        }

        //public bool syncIntelCategoryAndProduct(string storeid,string parentcategorypath)
        //{
        //    ProductCategory parentCategory=this.getProductCategory(parentcategorypath,storeid,true);


        //        List<ProductCategory> subcategories=this.context.getIntelProductCategroy(storeid).ToList();

        //        List<ProductCategroyMapping> mappings = this.context.getIntelProductCategroyMapping(storeid).ToList();

        //        return true;

        //}

        public bool CheckSpecSetId(List<SpecMask> SpecMaskList, List<VProductMatrix> pmatries)
        {
            var setids = SpecMaskList.Select(c => new { AttrCatId = c.AttrCatId, Attrid = c.Attrid });
            var ls = (from sm in setids
                      from pms in pmatries
                      where sm.AttrCatId == pms.CatID && sm.Attrid == pms.AttrID
                      group pms by pms.SpecSetID into g 
                      select g).ToList();
            return ls.Count() == 1;
        }

        #endregion
    }
}