using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS.DAL
{
    public partial class PStoreProductHelper : POCOS.DAL.Helper
    {

        public List<StoreDeal> getStoreDeal(string storeId, int StoreProductId)
        {

            List<StoreDeal> storeDeals = getStoreDeals(storeId).Where(x => x.StoreProductId == StoreProductId).ToList();
            return storeDeals;
        }


        public List<StoreDeal> getStoreDeals(string storeId)
        {
            string key = string.Format("{0}_StoreDeals", storeId);
            var cachedata = CachePool.getInstance().getObject(key);

            if (cachedata == null)
            {
                DateTime today = DateTime.Now.Date;
                List<StoreDeal> storeDeals = (from pp in context.Parts.OfType<PStoreProduct>()
                                              from a in context.StoreDeals
                                              where pp.PSProductId == a.StoreProductId
                                              && a.ExpireDate >= today
                                              && a.StartFrom <= today
                                              && pp.StoreID == storeId
                                              select a).ToList();
                CachePool.getInstance().cacheObject(key, storeDeals, CachePool.CacheOption.Hour2);
                return storeDeals;
            }
            else
            {
                return (List<StoreDeal>)cachedata;
            }

        }
        public Dictionary<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>> getPromotionPStoreProducts(string storeId, List<eStore.POCOS.StoreDeal.PromotionType> types)
        {
            var _types = types.Select(x => x.ToString());
            DateTime today = DateTime.Now.Date;
            var storeDeals = (from pp in context.Parts.OfType<PStoreProduct>()
                              from a in context.StoreDeals
                              from t in _types
                              where pp.PSProductId == a.StoreProductId
                              && a.ExpireDate >= today
                              && a.StartFrom <= today
                              && pp.StoreID == storeId
                              && t == a.Name
                              select new
                              {
                                  pStorePromotionType = (t == "NewArrive" ? "BestSeller" : t),
                                  a.OrdinalNo,
                                  pp.SProductID
                              }).ToList();

            if (storeDeals.Any())
            {
                List<Part> parts = (new PartHelper()).prefetchPartList(storeId, storeDeals.Select(x => x.SProductID).ToList());
                return (from p in parts.OfType<POCOS.PStoreProduct>()
                        from deal in storeDeals
                        where p.SProductID == deal.SProductID && p.isOrderable() && p.getListingPrice().value > 0m
                        group
                        new
                        {
                            deal.OrdinalNo,
                            product = p
                        }
                      by (eStore.POCOS.StoreDeal.PromotionType)Enum.Parse(typeof(eStore.POCOS.StoreDeal.PromotionType), deal.pStorePromotionType) into g
                        select new { g.Key, Values = g.Distinct().OrderBy(x => x.OrdinalNo).Select(x => x.product).ToList() }).ToDictionary(x => x.Key, x => x.Values);
            }
            else
                return new Dictionary<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>>();
        }

        private List<PStoreProduct> getOrderablePStoreProducts(List<Part> parts)
        {
            return (from p in parts.OfType<POCOS.PStoreProduct>()
                    where p.isOrderable() && p.getListingPrice().value > 0m
                    select p).ToList();
        }
        public List<PStoreProduct> getAllPStoreProducts(string storeid)
        {
            List<PStoreProduct> parts = context.Parts.OfType<PStoreProduct>().ToList();
            return (from p in parts
                    where p.isOrderable() && p.getListingPrice().value > 0m
                    select p).ToList();
        }

        public List<PStoreProduct> getPromotionPStoreProducts(string storeId, eStore.POCOS.StoreDeal.PromotionType type)
        {
            List<eStore.POCOS.StoreDeal.PromotionType> types = new List<eStore.POCOS.StoreDeal.PromotionType>();
            types.Add(type);
            Dictionary<eStore.POCOS.StoreDeal.PromotionType, List<PStoreProduct>> rlt = getPromotionPStoreProducts(storeId, types);
            if (rlt.ContainsKey(type))
                return rlt[type];
            else
                return new List<PStoreProduct>();
        }
 

        public List<PStoreProduct> GetStoreProductYouMayAlsoBuyByStoreProduct(string storeId, string sproductid)
        {
            DateTime today = DateTime.Now.Date;
            var associates = (
                from p in context.Parts.OfType<PStoreProduct>()
                from palsoby in context.Parts.OfType<PStoreProduct>()
                from a in context.StoreProductYouMayAlsoBuys
                where a.StoreProductId == p.PSProductId &&
                 palsoby.PSProductId == a.AssociateStoreProductId
                && p.StoreID == storeId
                && p.SProductID == sproductid
                && a.ExpireDate >= today
                && a.StartDate <= today
                select palsoby.SProductID).ToList();
            if (associates.Any())
            {
                return getOrderablePStoreProducts((new PartHelper()).prefetchPartList(storeId, associates));
            }
            else
                return new List<PStoreProduct>();
        }


        public List<PStoreProduct> GetStoreProductAssociateByStoreProductId(string storeId, string sproductid)
        {

            var associates = (from p in context.Parts.OfType<PStoreProduct>()
                              from palsoby in context.Parts.OfType<PStoreProduct>()
                              from a in context.StoreProductAssociates
                              where a.StoreProductId == p.PSProductId && palsoby.PSProductId == a.AssociateStoreProductId
                              && p.StoreID == storeId
                              && p.SProductID == sproductid
                              select palsoby.SProductID).ToList();
            if (associates.Any())
            {
                return getOrderablePStoreProducts((new PartHelper()).prefetchPartList(storeId, associates));
            }
            else
                return new List<PStoreProduct>();
        }

        public List<PStoreProduct> GetStoreProductRecentlyView(string storeId, List<string> ids)
        {

            POCOS.DAL.PartHelper parthelper = new DAL.PartHelper();
            return getOrderablePStoreProducts(parthelper.prefetchPartList(storeId, ids));
        }

        public List<PStoreProduct> GetStoreProductBundleWithMe(string storeId, string sproductid)
        {


            return new List<PStoreProduct>();
        }
        public string GetFeature(PStoreProduct product, POCOS.Language language)
        {
            return (from m in context.ProductCategoryMetadatas
                    from v in context.ProductCategoryMetadataValues
                    where m.Id == v.ProductCategoryMetadataId
                    && m.FieldName == "Features"
                    && v.ProductId == product.PProductId
                    select v.FieldValue).FirstOrDefault();
        }
 

        public List<VProductMatrix> getMetadata(PStoreProduct product, POCOS.Language language)
        {
            var fields = context.ProductCategoryMetadatas.Where(x => x.ProductCategoryId == product.ProductCategoryId).ToList();
            var values = context.ProductCategoryMetadataValues.Where(x => x.ProductId == product.PProductId).ToList();
            string fieldName = string.Empty;
            string fieldValue = string.Empty;
            List<VProductMatrix> _spec = new List<VProductMatrix>();
            LanguagePackHelper langePackageHelper = new LanguagePackHelper();

            foreach (var v in values)
            {
                var field = fields.Where(x => x.Id == v.ProductCategoryMetadataId).FirstOrDefault();
                if (field != null)
                {
                    if (language != null)
                    {
                        fieldName = langePackageHelper.Translate("ProductCategoryMetadatas", field.Id, language.Id, "", field.FieldName);
                        if (string.IsNullOrEmpty(fieldName))
                            fieldName = field.FieldName;
                        fieldValue = langePackageHelper.Translate("ProductCategoryMetadataValues", v.Id, language.Id, "", v.FieldValue);
                        if (string.IsNullOrEmpty(fieldValue))
                            fieldValue = v.FieldValue;
                    }
                    else
                    {
                        fieldName = field.FieldName;
                        fieldValue = v.FieldValue;
                    }

                    _spec.Add(new VProductMatrix
                    {
                        ProductNo = product.SProductID,
                        LocalCatName = fieldName,
                        LocalAttributeName = fieldName,
                        LocalValueName = fieldValue,
                        CatID = field.Id,
                        AttrID = field.Id,
                        AttrValueID = v.Id,
                        AttrCatName = field.FieldName,
                        AttrName = field.FieldName,
                        AttrValueName = v.FieldValue,
                        seq=field.OrdinalNo,
                        isProductSepc = field.ShowOnStore.GetValueOrDefault()
                    });
                }
            }
            return _spec;
        }


        public List<StoreProductBundleList> getBundleList(PStoreProduct product)
        {
            List<StoreProductBundleList> list = new List<StoreProductBundleList>();
            try
            {
                DateTime today = DateTime.Now.Date;
                var bundleitems = (from b in context.StoreProductBundles
                                   from bl in context.StoreProductBundleLists
                                   from rbl in context.StoreProductBundleLists.Include("StoreProductBundle")
                                   where b.Id == bl.StoreProductBundleId
                                       && bl.StoreProductId == product.PSProductId
                                       && rbl.StoreProductBundleId == b.Id
                                       //&& rbl.GroupName != bl.GroupName
                                       && ((b.BundleType == "BundleWith" && bl.IsPrimary) || (b.BundleType == "BundleOnly" && bl.IsPrimary == false))
                                       && b.StoreId == product.PStoreId
                                       && b.ExpireDate >=today
                                       && b.StartDate <= today
                                        && bl.ExpireDate >= today
                                        && bl.StartDate <= today
                                        && rbl.ExpireDate >= today
                                        && rbl.StartDate <= today
                                   select rbl);


                if (product.IsSoldInBundleOnly)
                {
                    var bundles = (from bi in bundleitems
                                   from sp in context.StoreProducts
                                   from mp in context.MBCPUMemorySpecs
                                   where bi.IsPrimary && bi.StoreProductId == sp.Id
                                   && sp.ProductId == mp.Id
                                   select new
                                   {
                                       Item = bi,
                                       PartNo = mp.PartNo
                                   }).Union(
                                    from bi in bundleitems
                                    from sp in context.StoreProducts
                                    from pp in context.PTDProducts
                                    where !bi.IsPrimary && bi.StoreProductId == sp.Id
                                    && sp.ProductId == pp.Id
                                    select new
                                    {
                                        Item = bi,
                                        PartNo = pp.PartNo
                                    }
                                    ).ToList();

                    bundles.ForEach(x => x.Item.storeIdX = product.StoreID);
                    bundles.ForEach(x => x.Item.sproductIdX = x.PartNo);
                    List<string> partnos = bundles.Where(x => !string.IsNullOrEmpty(x.PartNo)).Select(x => x.PartNo).Distinct().ToList();

                    List<Part> parts = (new PartHelper()).prefetchPartList(product.StoreID, partnos);

                    foreach (var b in bundles)
                    {
                        Part part = parts.FirstOrDefault(x => x.SProductID == b.PartNo);
                        if (part != null)
                        {
                            b.Item.partX = part;
                            list.Add(b.Item);
                        }
                    }
                }
                else
                {
                    var bundles = (from bi in bundleitems
                                   from sp in context.StoreProducts
                                   from pp in context.PTDProducts
                                   where bi.StoreProductId == sp.Id
                                   && sp.ProductId == pp.Id
                                   select new
                                   {
                                       Item = bi,
                                       PartNo = pp.PartNo
                                   }).ToList();

                    bundles.ForEach(x => x.Item.storeIdX = product.StoreID);
                    bundles.ForEach(x => x.Item.sproductIdX = x.PartNo);
                    List<string> partnos = bundles.Where(x => !string.IsNullOrEmpty(x.PartNo)).Select(x => x.PartNo).Distinct().ToList();

                    List<Part> parts = (new PartHelper()).prefetchPartList(product.StoreID, partnos);

                    foreach (var b in bundles)
                    {
                        Part part = parts.FirstOrDefault(x => x.SProductID == b.PartNo);
                        if (part != null)
                        {
                            b.Item.partX = part;
                            list.Add(b.Item);
                        }
                    }

                }


            }
            catch (Exception)
            {


            }
            return list;

        }

        public List<StoreProductBundleList> getSolutionList(int pstoreid,string storeid)
        {
            List<StoreProductBundleList> list = new List<StoreProductBundleList>();
            try
            {
                DateTime today = DateTime.Now.Date;
                var bundleitems = (from b in context.StoreProductBundles
                                   from bl in context.StoreProductBundleLists.Include("StoreProductBundle")
                                   where b.Id == bl.StoreProductBundleId
                              && b.StoreId == pstoreid
                              && b.BundleType == "Solution"
                              && b.ExpireDate >= today
                              && b.StartDate <=today
                              && bl.ExpireDate >= today
                              && bl.StartDate <= today
                                   select bl);
 
                    var bundles = (from bi in bundleitems
                                   from sp in context.StoreProducts
                                   from pp in context.PTDProducts
                                   where bi.StoreProductId == sp.Id
                                   && sp.ProductId == pp.Id
                                   select new
                                   {
                                       Item = bi,
                                       PartNo = pp.PartNo
                                   }).ToList();

                    bundles.ForEach(x => x.Item.storeIdX = storeid);
                    bundles.ForEach(x => x.Item.sproductIdX = x.PartNo);
                    List<string> partnos = bundles.Where(x => !string.IsNullOrEmpty(x.PartNo)).Select(x => x.PartNo).Distinct().ToList();

                    List<Part> parts = (new PartHelper()).prefetchPartList(storeid, partnos);

                    foreach (var b in bundles)
                    {
                        Part part = parts.FirstOrDefault(x => x.SProductID == b.PartNo);
                        b.Item.partX = part;
                        list.Add(b.Item);
                    }
            }
            catch (Exception)
            {


            }
            return list;

        }

        public List<PStoreProduct> searchOnlybyKeywords(string keywords, string storeid, Int32 maxCount = 9999, bool includePhaseout = false)
        {
            List<PStoreProduct> _products = new List<PStoreProduct>();
            keywords = keywords.ToLower();
 
                var productids = (from prod in context.Parts.OfType<PStoreProduct>()
                             from mv in context.ProductCategoryMetadataValues 
                             where  mv.ProductId==prod.PProductId
                             && prod.StoreID == storeid && mv.FieldValue!=null && mv.FieldValue.ToLower().Contains(keywords.ToLower())
                             select prod.SProductID ).Distinct().Take(maxCount).ToList();
                List<Part> parts = (new PartHelper()).prefetchPartList(storeid, productids);
             
                    foreach (Part p in parts)
                    {
                        if (p is PStoreProduct  )
                        {
                            PStoreProduct pp = (PStoreProduct)p;
                            if (includePhaseout)
                            {
                                _products.Add(pp);
                            }
                            else
                            {
                                if (pp.isOrderable())
                                {
                                    _products.Add(pp);
                                }
                            }
                        }
                    }
    
                return _products;
        }

    }
}
