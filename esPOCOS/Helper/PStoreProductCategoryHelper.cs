using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class PStoreProductCategoryHelper : Helper
    {
        #region Business Read
        public PStoreProductCategory get(POCOS.PStore pstore, int id)
        {
            return getCachedCategoires(pstore).FirstOrDefault(x => x.Id == id);
        }
        public PStoreProductCategory get(POCOS.Store store, int id)
        {
            return getCachedCategoires(store).FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.PStoreProductCategories.Any(x => x.Id == id);
        }
        private List<PStoreProductCategory> _cachedCategoires;
        private object locker = new object();
        private List<PStoreProductCategory> getCachedCategoires(POCOS.PStore pstore)
        {

            lock (locker)
            {
                if (_cachedCategoires == null)
                {
                    string key = string.Format("{0}_CachedPStoreCategoires", pstore.Name);
                    _cachedCategoires = (List<PStoreProductCategory>)CachePool.getInstance().getObject(key);
                    if (_cachedCategoires == null)
                    {
                        _cachedCategoires = getAllCategories(pstore);
                        _cachedCategoires.ForEach(x => x.pstoreX = pstore);
                        CachePool.getInstance().cacheObject(key, _cachedCategoires);
                    }
                }
            }
            return _cachedCategoires;
        }
        private List<PStoreProductCategory> getCachedCategoires(POCOS.Store store)
        {

            lock (locker)
            {
                if (_cachedCategoires == null)
                {
                    string key = string.Format("{0}_CachedPStoreCategoires", store.StoreID);
                    _cachedCategoires = (List<PStoreProductCategory>)CachePool.getInstance().getObject(key);
                    if (_cachedCategoires == null)
                    {
                        PStoreHelper pstorehelper = new PStoreHelper();
                        PStore pstore = pstorehelper.getPStoreByStore(store);
             
                        _cachedCategoires = getAllCategories(pstore);
                        _cachedCategoires.ForEach(x => x.pstoreX = pstore);
                        CachePool.getInstance().cacheObject(key, _cachedCategoires);
                    }
                }
            }
            return _cachedCategoires;
        }

        public List<POCOS.PStoreProductCategory.SpecFilter> getSpecFilter(PStoreProductCategory category)
        {
            var fields = (from pcm in context.ProductCategoryMetadatas
                          from pcmv in context.ProductCategoryMetadataValues
                          from pc in context.PStoreProductCategories
                          from ezca in context.EZCatalogAttributes
                          from p in context.Parts.OfType<POCOS.PStoreProduct>()
                          where pcm.PublishStatus && pcm.Status
                          && pcm.ProductCategoryId == pc.Id
                          && pcmv.ProductCategoryMetadataId == pcm.Id
                          && pc.Name == ezca.Category_Id
                          && pcm.FieldName == ezca.Attribute_Name
                      && p.ProductCategoryId == pcm.ProductCategoryId
                      && p.PProductId == pcmv.ProductId
                          && ezca.Attribute_Display == "Enable"
                          && ezca.Attribute_Type == "Primary"
                          && pc.Id == category.Id
                          select new
                          {
                              ProductNo = p.SProductID,
                              AttrName = pcm.FieldName,
                              AttrValueName = pcmv.FieldValue,
                              pcm.OrdinalNo
                          }).ToList();

            var filter = (from f in fields
                          where category.productList.Any(x => x.SProductID == f.ProductNo)
                          group f by new { f.AttrName, f.OrdinalNo } into fg
                          orderby fg.Key.OrdinalNo
                          select new POCOS.PStoreProductCategory.SpecFilter
                          {
                              AttributeName = fg.Key.AttrName,
                              Values = (from v in fg.ToList()
                                        group v by v.AttrValueName into vg
                                      
                                        select new POCOS.PStoreProductCategory.SpecFilterValue
                                        {
                                            Name = vg.Key,
                                            Products = string.Join(",", vg.Select(x => x.ProductNo).ToArray()),
                                            Count = vg.Count()
                                        }).OrderBy(x=>x.Sequnce).ThenBy(x=>x.Name).ToList()
                          }).ToList();
            return filter;
        }
        public void perfetchFilter(PStoreProductCategory category)
        {

            if (category != null && category.productList.Any())
            {

                var missingfilterproductids = (from p in category.productList
                                               where p.isInitSpecFilter == false
                                               select p.PProductId);
                if (missingfilterproductids.Any() == false)
                    return;
                var fields = (from pcm in context.ProductCategoryMetadatas
                              from pcmv in context.ProductCategoryMetadataValues
                              from pc in context.PStoreProductCategories
                              from ezca in context.EZCatalogAttributes
                              from p in context.Parts.OfType<POCOS.PStoreProduct>()
                              where pcm.PublishStatus && pcm.Status
                              && pcm.ProductCategoryId == pc.Id
                              && pcmv.ProductCategoryMetadataId == pcm.Id
                              && missingfilterproductids.Contains(pcmv.ProductId)
                              && pc.Name == ezca.Category_Id
                              && pcm.FieldName == ezca.Attribute_Name
                          && p.ProductCategoryId == pcm.ProductCategoryId
                          && p.PProductId == pcmv.ProductId
                              && ezca.Attribute_Display == "Enable"
                              && ezca.Attribute_Type == "Primary"
                              orderby pcm.OrdinalNo
                              select new
                              {
                                  pcmv.ProductId,
                                  pcmv.FieldValue,
                                  pcm.FieldName
                              }).ToList();

                foreach (var p in category.productList)
                {
                    if (p.isInitSpecFilter == false)
                    {
                        p.simpleSpec = (from f in fields
                                        where f.ProductId == p.PProductId
                                        select new PStoreProduct.SimpleSpec
                                        {
                                            AttrCatName = f.FieldName,
                                            AttrName = f.FieldName,
                                            AttrValueName = f.FieldValue
                                        }).ToList();
                    }
                }
            }

        }


        public List<PStoreProductCategory> getAllCategories(POCOS.PStore pstore)
        {
            if (pstore == null)
                return new List<PStoreProductCategory>();

            var categories = (from pc in context.PStoreProductCategories
                              from spc in context.StoreProductCategories
                              where pc.Id == spc.ProductCategoryId
                              && spc.IsPublish == true
                              && spc.StoreId == pstore.Id
                              select
                              new
                              {

                                  Category = pc,
                                  Sequence = spc.Sequence,
                                  spc.MetaDescription,
                                  spc.MetaKeyword,
                                  spc.MetaTitle
                              }
                            ).Distinct().ToList();
            foreach (var c in categories)
            {
                c.Category.MetaDescription = c.MetaDescription;
                c.Category.MetaKeyword = c.MetaKeyword;
                c.Category.MetaTitle = c.MetaTitle;
                c.Category.Sequence = c.Sequence;
            }
   
            return categories.Select(x => x.Category).ToList();
        }
        public List<PStoreProductCategory> getTopLevelCategory(POCOS.PStore pstore)
        {
            if (pstore == null)
                return new List<PStoreProductCategory>();
            return getCachedCategoires(pstore)
                .Where(x => x.ParentProductCategoryId == null && x.Status == true).OrderBy(x => x.Sequence).ToList();
        }

        public List<PStoreProductCategory> getSubCategories(POCOS.PStore pstore, PStoreProductCategory category)
        {
            if (category == null || pstore == null)
                return new List<PStoreProductCategory>();
            return getCachedCategoires(pstore)
                .Where(x => x.ParentProductCategoryId == category.Id && x.Status == true).OrderBy(x => x.Sequence).ToList();
        }
        public PStoreProductCategory getParent(POCOS.PStore pstore, PStoreProductCategory category)
        {
            if (category == null || pstore==null)
                return null;
            return getCachedCategoires(pstore)
                .Where(x => x.Id == category.ParentProductCategoryId && x.Status == true).FirstOrDefault();
        }
        public List<PStoreProduct> getProducts(Store store, PStoreProductCategory category)
        {
            if (category == null)
                return new List<PStoreProduct>();

            string[] sapvalidarray = store.getOrderablePartStates();

            List<string> pns = (from c in context.PStoreProductCategories
                                from p in context.Parts.OfType<PStoreProduct>()
                                where
                                c.Id == category.Id
                                && p.ProductCategoryId == c.Id
                                 && p.StoreID == store.StoreID
                                 && sapvalidarray.Contains(p.StockStatus)
                                select p.SProductID).ToList();
            return (new PartHelper()).prefetchPartList(store.StoreID, pns).OfType<PStoreProduct>().Where(x => x.isOrderable()).ToList();
        }
        #endregion
        #region Creat Update Delete
        public int save(PStoreProductCategory _pstoreproductcategory)
        {

            //if parameter is null or validation is false, then return  -1 
            if (_pstoreproductcategory == null || _pstoreproductcategory.validate() == false) return 1;
            //Try to retrieve object from DB

            try
            {
                if (_pstoreproductcategory.Id == 0 || !isExists(_pstoreproductcategory.Id))  //object not exist 
                {
                    //Insert
                    context.PStoreProductCategories.AddObject(_pstoreproductcategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PStoreProductCategories.ApplyCurrentValues(_pstoreproductcategory);
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
        public int delete(PStoreProductCategory _pstoreproductcategory)
        {

            if (_pstoreproductcategory == null || _pstoreproductcategory.validate() == false) return 1;
            try
            {
                context.DeleteObject(_pstoreproductcategory);
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
    }
}