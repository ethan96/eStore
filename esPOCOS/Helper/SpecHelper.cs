using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;
using eStore.POCOS;
using eStore.POCOS.Sync;
namespace eStore.POCOS.DAL
{

    public partial class SpecHelper : PartHelper 
    {

        #region Business Read

        private PISEntities pcontext;

        public SpecHelper() : base()
        {
            pcontext = new PISEntities();
        }

        ~SpecHelper()
        {
            if (pcontext != null)
                pcontext.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<POCOS.SpecMask> getSpecMaskByCatePath(string path)
        {
            var ls = (from c in context.SpecMasks
                      where c.Categorypath.Equals(path, StringComparison.OrdinalIgnoreCase)
                      select c).ToList();
            return ls;
        }


        /// <summary>
        /// Return a ProductSpecrules object, include a match product list and spec list.
        /// </summary>
        /// <param name="categorypath"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        /// 
        public ProductSpecRules getProductSpecRulesbak(string categorypath, string storeid, string keyword="")
        {
            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);

            ProductSpecRules productspecs = new ProductSpecRules();
            keyword =  keyword.Replace("%", "");

            try
            {
                var _partx = (from spr in context.VSpecRules
                              from p in context.Parts.OfType<Product>()
                              where spr.ProductNo == p.SProductID && spr.Storeid == storeid && p.StoreID == storeid
                              && spr.CategoryPath == categorypath && p.PublishStatus == true && (p.VendorProductDesc.Contains(keyword) || spr.LocalValueName.Contains(keyword) || p.SProductID.Contains(keyword))
                              && validarray.Contains(p.Status) && sapvalidarray.Contains(p.StockStatus) && p.PublishStatus==true
                              select p).Distinct();

                productspecs._products = _partx.ToList();
                keyword = "%" + keyword + "%";
                var _speccount = context.spGetSpecCount(storeid, categorypath, keyword);

                List<VProductMatrix> vplist = new List<VProductMatrix>();

                foreach (var a in _speccount.ToList())
                {
                    VProductMatrix vpm = new VProductMatrix();
                    vpm.AttrCatName = a.AttrCatName;
                    vpm.CatID = a.CatID.Value;
                    vpm.AttrID = a.AttrID;
                    vpm.AttrName = a.AttrName;
                    vpm.AttrValueID = a.AttrValueID;
                    vpm.AttrValueName = a.AttrValueName;
                    vpm.LocalValueName = a.LocalValueName;
                    vpm.LocalCatName = a.LocalCatName;
                    vpm.LocalAttributeName = a.LocalAttributeName;
                    vpm.productcount = a.productcount.Value;
                    vpm.seq   = a.seq;
                    vplist.Add(vpm);
                }

                if (vplist.Count() > 1)
                {
                    productspecs._specrules = (from spec in vplist
                                               orderby spec.seq 
                                               select spec).ToList<VProductMatrix>();
                }
                else
                    productspecs._specrules = vplist;

                if (string.IsNullOrEmpty(categorypath) == false)
                {
                    var _pc = (from pc in context.ProductCategories
                               where pc.CategoryPath == categorypath && pc.Storeid == storeid
                               select pc).FirstOrDefault();

                    productspecs._productcategory = _pc;
                    productspecs._productcategory.shelper = this;
                }
                return productspecs;

            }
            catch (Exception ex)
            {

                Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

        /// <summary>
        /// get product category products, specs, enable filter by keywords or specs
        /// </summary>
        /// <param name="productCategory"></param>
        /// <param name="keyword"></param>
        /// <param name="selectedspec"></param>
        /// <returns></returns>
        public ProductSpecRules getProductSpecRules( ProductCategory productCategory, string keyword = "", List<VProductMatrix> selectedspec=null)
        {
            ProductSpecRules productspecs = new ProductSpecRules();
            if (productCategory == null)
                return productspecs;
            string categorypath = productCategory.CategoryPath;
            //keyword 这里没有使用, 是否可以删除
            keyword = keyword.Replace("%", "");

            try
            {
                productspecs._productcategory = productCategory;
                productspecs._products = productCategory.productList;
                //all specs in products
                List<VProductMatrix> origspecs = this.getProductsSpec(productspecs._products);
                List<VProductMatrix> specs;
                //filter products by spec
                if (selectedspec != null && origspecs != null)
                {
                    specs = this.getSpecbyRuleSet(selectedspec, origspecs);

                    productspecs._products = (from p in productspecs._products
                                              from s in specs
                                              where p.SProductID == s.ProductNo
                                              select p).Distinct().ToList();
                }
                else
                {
                    specs = origspecs;
                }
                //filter attritbute by specMask and set product count for each attribute value
                if (origspecs.Count() > 1)
                {
                    var mask = (from msk in productCategory.SpecMasks
                                where  msk.Viewable == true
                                select msk
                                    ).ToList();

                    var attrs = (from spec in
                                     (from ospec in origspecs
                                      select new
                                      {
                                          ospec.CatID
                                          ,
                                          ospec.LocalCatName
                                          ,
                                          ospec.LocalAttributeName
                                          ,
                                          ospec.AttrID
                                          ,
                                          ospec.LocalValueName
                                          ,
                                          ospec.AttrValueID
                                      })
                                          .Distinct()
                                 from msk in mask
                                 where spec.AttrID == msk.Attrid && spec.CatID == msk.AttrCatId
                                 orderby msk.Seq
                                 select new VProductMatrix
                                 {
                                     CatID = spec.CatID,
                                     LocalCatName = spec.LocalCatName,
                                     AttrCatName = spec.LocalCatName,
                                     LocalAttributeName = spec.LocalAttributeName,
                                     LocalValueName = spec.LocalValueName,
                                     AttrValueName = spec.LocalValueName,
                                     AttrName = spec.LocalAttributeName,
                                     AttrID = spec.AttrID,
                                     AttrValueID = spec.AttrValueID,
                                     selected = selectedspec != null && (from s in selectedspec
                                                                         where spec.AttrValueID == s.AttrValueID
                                                                         select s).Count() > 0,
                                     seq = msk.Seq,
                                     productcount = (
                                     from s in specs
                                     where spec.AttrID == s.AttrID && spec.AttrValueID == s.AttrValueID
                                     select s.ProductNo
                                         ).Distinct().Count()
                                 }
                                 ).Distinct().ToList();
                    productspecs._specrules = attrs;
                }
                else
                    productspecs._specrules = specs;

                return productspecs;

            }
            catch (Exception ex)
            {

                Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return  new ProductSpecRules();
            }
        }

        private List<VProductMatrix> getSpecbyRuleSet(List<VProductMatrix> ruleset, List<VProductMatrix> source)
        {
            int[] values = (from v in ruleset
                            select v.AttrValueID).ToArray();
            int attrcnt = (from a in ruleset
                           select a.AttrID).Distinct().Count();
            List<VProductMatrix> pns;
            if (source == null)
                pns = (from v in context.VProductMatrices
                       from v2 in
                           (
                               from vpm in context.VProductMatrices
                               where values.Contains(vpm.AttrValueID)
                               group vpm by vpm.ProductNo into g
                               where g.Count() == attrcnt
                               select new { g.Key })
                       where v.ProductNo == v2.Key
                       select v).ToList();
            else
                pns = (from v in source
                       from v2 in
                           (
                               from vpm in source
                               where values.Contains(vpm.AttrValueID)
                               group vpm by vpm.ProductNo into g
                               where g.Count() == attrcnt
                               select new { g.Key })
                       where v.ProductNo == v2.Key
                       select v).ToList();
            return pns;

        }

        private List<VProductMatrix> getProductsSpec(List<Product> products)
        {

            List<VProductMatrix> specs = new List<VProductMatrix>();
            foreach (Product prod in products)
                specs.AddRange(prod.specs);
            return specs;
        }
        /// <summary>
        /// Return All selectable specs for OM
        /// integrate all Specs in PIS and eStore
        /// </summary>
        /// <returns></returns>

        public List<VSpec> getAllSpecs()
        {

            var _sp = from p in context.VSpecs                     
                      select p;

            if (_sp != null)
                return _sp.ToList();
            else
                return new List<VSpec>();
        
        }

        /// <summary>
        /// For OM use, give the specs that can be selected to set spec mask
        /// </summary>
        /// <param name="pc"></param>
        /// <returns></returns>

        public List<VSpec> getSpecsbyRulesetID(string storeid, int rulesetid, string categorypath) {
        
            List<VSpec> vspecs = new List<VSpec>();
            List<spGetSpecByCategory_Result> _vspecs = context.spGetSpecByCategory(storeid,rulesetid,categorypath).ToList();


            //foreach (var m in _vspecs.ToList())
            foreach (var m in _vspecs)
            {
                VSpec vs = new VSpec();
                vs.AttrCatID = m.CatID ;
                vs.AttrCatName = m.AttrCatName ;
                vs.AttrID = m.AttrID ;
                vs.AttrName = m.AttrName ;
                vs.AttrValueID = m.AttrValueID ;
                vs.AttrValueName = m.AttrValueName;
                vspecs.Add(vs);                 
            }

            return vspecs;
        
        }

        public List<VSpec> getCategorySpecMask(POCOS.ProductCategory category)
        {

            List<VSpec> vspecs = new List<VSpec>();

            var _vspecs = (from vpm in context.VProductMatrices
                           from mask in context.SpecMasks
                           //from pc in context.ProductCategories
                           from pm in context.ProductCategroyMappings
                           where vpm.ProductNo == pm.SProductID
                           && pm.CategoryID == category.CategoryID
                           && pm.StoreID == category.Storeid
                           && mask.AttrCatId == vpm.CatID
                           && mask.Attrid == vpm.AttrID
                          select vpm
                          ).Distinct().ToList(); ;
          //foreach (var m in _vspecs.ToList())
          foreach (var m in _vspecs)
          {
              VSpec vs = new VSpec();
              vs.AttrCatID = m.CatID;
              vs.AttrCatName = m.AttrCatName;
              vs.AttrID = m.AttrID;
              vs.AttrName = m.AttrName;
              vs.AttrValueID = m.AttrValueID;
              vs.AttrValueName = m.AttrValueName;
              vspecs.Add(vs);
          }
            return vspecs;

        }




        /// <summary>
        /// For OM , Get all Basetype
        /// </summary>
        /// <returns></returns>
        public List<SpecAttributeCat> getBaseType()
        {

            var _sp = from p in context.SpecAttributeCats 
                      select p;

            return _sp.ToList();

        }

        public ProductSpecRules filterProducts(ProductCategory productCategory, List<VProductMatrix> selectedspec, string keywords = "")
        {
        ProductSpecRules productspecs = new ProductSpecRules();
            if (productCategory == null)
                return productspecs;
            string categorypath = productCategory.CategoryPath;
            string storeid = productCategory.Storeid;
             

            keywords = "%" + keywords + "%";

            ProductSpecRules psr = getProductSpecRules(productCategory, keywords); //return all products and spec in request category
            setSelectedRules(selectedspec, psr._specrules);
            //PartHelper phelper = new PartHelper();           
 
            foreach (Product pp in psr._products.ToArray())
            {
                //List<VProductMatrix> _vp = phelper.getMatrix(pp);
                List<VProductMatrix> _vp = getMatrix(pp);

                foreach (VProductMatrix t1 in selectedspec.OrderBy(x => x.CatID))
                {
                    t1.ismatch = false;
                    foreach (VProductMatrix vp2 in _vp.OrderBy(x => x.CatID))
                    {
                        //check every rule, see if the rule is fullfill
                        if(ismatch(vp2, t1) == true) {
                            t1.ismatch = true;
                            break;
                        }
                    }                  
                }

                int previousattr =0;

                VProductMatrix previousrules = new VProductMatrix();
                bool keep = false;
                foreach (VProductMatrix t1 in selectedspec.OrderBy(x => x.CatID))
                {
                    //If not meet first rule, product still keep
                    //If previous rule is false, even current rule is true, product should be removed
                   

                   if ( selectedspec.Count == 1 && t1.ismatch == true) {
                       keep = true;
                    }
                  
                   else if (selectedspec.Count > 1 && previousattr != 0 && (previousattr == t1.AttrID && previousrules.CatID == t1.CatID))
                   {
                       keep = keep || t1.ismatch;
                   }

                   else if (selectedspec.Count > 1 && previousattr != 0 && (previousattr != t1.AttrID && previousrules.CatID == t1.CatID) )
                   {
                       keep = keep && t1.ismatch;
                   }
                   else if (selectedspec.Count > 1 && previousattr != 0 && (previousattr != t1.AttrID && previousrules.CatID != t1.CatID))
                   {
                       keep = keep && t1.ismatch;
                   }
                   else if (selectedspec.Count > 1) {
                       keep = keep || t1.ismatch;
                   }                  
                
                   
                    previousrules = t1;
                    previousattr = t1.AttrID;
                    
                }

                if (keep == false && selectedspec!=null && selectedspec.Count>0)
                    psr._products.Remove(pp);


            }

            if (string.IsNullOrEmpty(categorypath) == false)
            {
                var _pc = (from pc in context.ProductCategories
                           where pc.CategoryPath == categorypath && pc.Storeid == storeid
                           select pc).FirstOrDefault();

                psr._productcategory = _pc;
                psr._productcategory.shelper = this;
            }
            return psr;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="storeid"></param>
        /// <param name="maxCount">optinal parameter to limit the returning number for performance consideration</param>
        /// <returns></returns>
        public ProductSpecRules searchOnlybyKeywords(string keywords, string storeid, Int32 maxCount = 9999, bool includePhaseout =false)
        {

            ProductSpecRules psr = new ProductSpecRules();

            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);

            List<string> validlist = validarray.ToList();
            validarray = validlist.ToArray();

            List<Product> _products = new List<Product>();
            keywords = keywords.ToLower();

            if (includePhaseout == true)
            {
                _products = (from prod in context.Parts.OfType<Product>().Include("ReplaceProducts")
                                 where (prod.SProductID.ToLower().Contains(keywords) || prod.ProductDesc.ToLower().Contains(keywords) || prod.DisplayPartno.ToLower().Contains(keywords) || prod.ModelNo.ToLower().Contains(keywords))
                                 //&& prod.StoreID == storeid   && !prod.SProductID.EndsWith("BTO")                                
                                && prod.StoreID == storeid && !prod.SProductID.StartsWith("SBC-BTO") && !prod.SProductID.EndsWith("BTO")                               
                             select prod).Take(maxCount).ToList();

            }
            else
            {

                _products = (from prod in context.Parts.OfType<Product>().Include("ReplaceProducts")
                                 where (prod.SProductID.ToLower().Contains(keywords) || prod.ProductDesc.ToLower().Contains(keywords) || prod.DisplayPartno.ToLower().Contains(keywords) || prod.ModelNo.ToLower().Contains(keywords) 
                                 || prod.Keywords.ToLower().Contains(keywords) || prod.ProductFeatures.ToLower().Contains(keywords))
                                 //&& prod.StoreID == storeid && validarray.Contains(prod.Status) && prod.PublishStatus == true && !prod.SProductID.EndsWith("BTO")
                                 && prod.StoreID == storeid && validarray.Contains(prod.Status) && prod.PublishStatus == true && !prod.SProductID.StartsWith("SBC-BTO") && !prod.SProductID.EndsWith("BTO") && !prod.SProductID.StartsWith("SOM") 
                             select prod).Take(maxCount).ToList();

                  foreach (Product p in _products.ToList())
                  {
                      if ((p is Product_Ctos) == false && !sapvalidarray.Contains(p.StockStatus))
                          _products.Remove(p);
                  }
            }

            psr._products = _products;
 

            foreach (Product p in _products)
            {
                p.parthelper = this;
                /*
                if (p._helper == null)
                    p._helper = this;
                 * */
            }
            

            return psr;
        }

        public List<spProductSearch_Result> searchOnlybyKeywords(string keywords, string storeid, Int32 maxCount = 9999)
        {
            try
            {
                List<spProductSearch_Result> result = context.spProductSearchHint(storeid, keywords, maxCount).ToList();
                return result;
            }
            catch (Exception)
            {

                return new List<spProductSearch_Result>();
            }
        }

        public List<spProductSearch_Result> searchOnlybyKeywordsWithCategory(string keywords, string storeid, Int32 maxCount = 9999)
        {
            try
            {
                List<spProductSearch_Result> result = context.spProductSearch(storeid, keywords, maxCount).ToList();
                return result;
            }
            catch (Exception)
            {

                return new List<spProductSearch_Result>();
            }
        }

        public ProductSpecRules searchOnlybyKeywords(string keywords, MiniSite minisite, Int32 maxCount = 9999, bool includePhaseout = false)
        {

            ProductSpecRules psr = new ProductSpecRules();

            string[] validarray = getValidProductStatus(minisite.StoreID);
            string[] sapvalidarray = getValidSAPStatus(minisite.StoreID);

            List<string> validlist = validarray.ToList();
            validarray = validlist.ToArray();

            List<Product> _products = new List<Product>();
            keywords = keywords.ToLower();

            if (includePhaseout == true)
            {
                _products = (from prod in context.Parts.OfType<Product>().Include("ReplaceProducts")
                                    from pc in context.ProductCategories
                                    from pcm in context.ProductCategroyMappings
                             where prod.StoreID == minisite.StoreID && pc.Storeid == minisite.StoreID && pc.MiniSiteID == minisite.ID && pcm.StoreID == pc.Storeid && pcm.CategoryID == pc.CategoryID
                                    && prod.SProductID == pcm.SProductID
                                    && (prod.SProductID.ToLower().Contains(keywords) || prod.ProductDesc.ToLower().Contains(keywords) || prod.DisplayPartno.ToLower().Contains(keywords) || prod.ModelNo.ToLower().Contains(keywords))                 
                                    && !prod.SProductID.StartsWith("SBC-BTO") && !prod.SProductID.EndsWith("BTO")
                             select prod).Take(maxCount).ToList();
            }
            else
            {

                _products = (from prod in context.Parts.OfType<Product>().Include("ReplaceProducts")
                                    from pc in context.ProductCategories
                                    from pcm in context.ProductCategroyMappings
                             where prod.StoreID == minisite.StoreID && pc.Storeid == minisite.StoreID && pc.MiniSiteID == minisite.ID && pcm.StoreID == pc.Storeid && pcm.CategoryID == pc.CategoryID
                                    && prod.SProductID == pcm.SProductID 
                             && ( prod.SProductID.ToLower().Contains(keywords) || prod.ProductDesc.ToLower().Contains(keywords) || prod.DisplayPartno.ToLower().Contains(keywords) || prod.ModelNo.ToLower().Contains(keywords)
                             || prod.Keywords.ToLower().Contains(keywords) || prod.ProductFeatures.ToLower().Contains(keywords))
                             && validarray.Contains(prod.Status) && prod.PublishStatus == true && !prod.SProductID.StartsWith("SBC-BTO") && !prod.SProductID.EndsWith("BTO") && !prod.SProductID.StartsWith("SOM")
                             select prod).Take(maxCount).ToList();

                foreach (Product p in _products.ToList())
                {
                    if ((p is Product_Ctos) == false && !sapvalidarray.Contains(p.StockStatus))
                        _products.Remove(p);
                }
            }

            psr._products = _products;


            foreach (Product p in _products)
            {
                p.parthelper = this;
                /*
                if (p._helper == null)
                    p._helper = this;
                 * */
            }


            return psr;
        }

        /// <summary>
        /// Set selected to spec list
        /// </summary>
        /// <param name="selected"></param>
        /// <param name="distinctspec"></param>

        private void setSelectedRules(List<VProductMatrix> selected, List<VProductMatrix> distinctspec) {

            /*
            foreach (VProductMatrix vpm in selected.ToList())
            { 
                var _x = from x in distinctspec
                         where x.CatID == vpm.CatID && x.AttrID == vpm.AttrID && x.AttrValueID == vpm.AttrValueID 
                         select x;
                foreach (var select in _x.ToList())
                {
                    select.selected = true;
                }

            }
             * */

            foreach (VProductMatrix vpm in selected)
            {
                var _x = from x in distinctspec
                         where x.CatID == vpm.CatID && x.AttrID == vpm.AttrID && x.AttrValueID == vpm.AttrValueID
                         select x;
                foreach (var select in _x)
                {
                    select.selected = true;
                }
            }
        }

        private bool ismatch(VProductMatrix prd, VProductMatrix rule){
            if (prd.CatID == rule.CatID && prd.AttrID == rule.AttrID && prd.AttrValueID == rule.AttrValueID)
                return true;
            else
                return false;
    
    }

        public string getexp(List<VProductMatrix> selectedspec)
        {

            //Select distinct catid
            var _disCatid = from x in selectedspec
                            group x by x.CatID into g
                            select new { catid = g.Key };


            string allwhere = "";

            //foreach (var cat in _disCatid.ToList())
            foreach (var cat in _disCatid)
            {
                string where = "";
                var _x = from m in selectedspec
                         where m.CatID == cat.catid
                         select m;

                foreach (VProductMatrix xn in _x)
                {
                    if (string.IsNullOrEmpty(where) == false)
                        where = where + " OR  attrvalueid= " + xn.AttrValueID;
                    else
                        where = where + "attrvalueid= " + xn.AttrValueID;
                }

                if (string.IsNullOrEmpty(allwhere) == false && string.IsNullOrEmpty(where) == false)
                    allwhere = "(" + allwhere + ") AND (" + where + ")";
                else if (string.IsNullOrEmpty(allwhere) == true && string.IsNullOrEmpty(where) == false)
                {
                    allwhere = where;
                }
            }

            return allwhere;

        }

        private List<VSpecRule> convert(List<VProductMatrix> selectedspec, string storeid, string categorypath)
        {

            List<VSpecRule> lspecrules = new List<VSpecRule>();
            //foreach (VProductMatrix v in selectedspec.ToList())
            foreach (VProductMatrix v in selectedspec)
            {
                VSpecRule vsr = new VSpecRule();
                vsr.CatID = v.CatID;
                vsr.AttrID = v.AttrID;
                vsr.AttrValueID = v.AttrValueID;
                vsr.Storeid = storeid;
                vsr.CategoryPath = categorypath;
                lspecrules.Add(vsr);
            }

            return lspecrules;
        }


        public SpecAttributeCat getSpecCat(SpecAttributeCat speccat) {
            var _speccat = (from x in context.SpecAttributeCats
                           where x.AttrCatID == speccat.AttrCatID && x.AttrCatName == speccat.AttrCatName
                           select x).FirstOrDefault();                     
                return _speccat;

        }


        #endregion

        #region Creat Update Delete

        public int save(SpecAttributeCat  _spec)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_spec == null || _spec.validate() == false) return 1;

            //Try to retrieve object from DB  
            SpecAttributeCat _exist_spec = getSpecCat(_spec);
            try
            {

                if (_exist_spec == null)  //Add 
                {
                    context.SpecAttributeCats.AddObject(_spec); 
                    context.SaveChanges();
                    return 0;
                }
                else //Update 
                {
                    context.SpecAttributeCats.ApplyCurrentValues(_spec);                  
                    context.SaveChanges();
                }

                return 0;
            }

            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);

                return -5000;
            }

        }


        public int delete(SpecAttributeCat _spec)
        {

            if (_spec == null || _spec.validate() == false) return 1;

            try
            {
                context.DeleteObject(_spec);
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

        #region Others

        private static string myclassname()
        {
            return typeof(StoreHelper).ToString();
        }

        public List<ProductSearchOptimizedResult> searchbyKeywords(string keyWords, string storeID, int maxCount, bool includeCategory)
        {
            var ls = context.spProductSearchOptimized(storeID, keyWords, maxCount).ToList();
            return ls;
        }

        public List<string> getftsparser(string keyword, int language)
        {
            try
            {
                var ls = context.spGetFTSParser(esUtilities.StringUtility.GetSafeSQLString(keyword), language);
                return ls.Select(c => c.display_term).ToList();
            }
            catch (Exception)
            {
                return new List<string> { keyword };
            }
        }
        #endregion
    }


}