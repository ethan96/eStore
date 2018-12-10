using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Collections;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Data.Objects;
using eStore.Proxy;
using eStore.POCOS.Sync;

namespace eStore.POCOS.DAL
{

    public partial class PartHelper : Helper
    {
        #region Business Read

        public PartHelper() : base()
        {
        }

        public PartHelper(eStore3Entities6 carryonContext)
        {
            if (carryonContext != null)
                context = carryonContext;
            else
                context = new eStore3Entities6();
        }



        public Part getPart(string sproductid, Store store, bool includeInvalidStatus = false )
        {
            return getPart(sproductid, store.StoreID, includeInvalidStatus);
        }

        public List<Part> getParts(string storeid)
        {
            var ls = (from c in context.Parts
                      where c.StoreID == storeid
                      select c).ToList();
            return ls;
        }

        public List<Product> getProducts(string storeId)
        {
            var ls = (from c in context.Parts.OfType<POCOS.Product>()
                      where c.StoreID == storeId
                      select c).ToList();
            return ls;
        }

        /// <summary>
        /// For WWW webservice used, get CTOS by displayname
        /// </summary>
        /// <param name="systemname"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public Part getPartbyDisplayName(string systemname, string storeid) {
            try
            {
                lock (context)
                {
                    var _system = (from c in context.Parts.OfType<Product>()
                                   where c.DisplayPartno.Equals(systemname) && c.StoreID.Equals(storeid)
                                   select c).FirstOrDefault();
                    return _system;
                }
            }
            catch (Exception ex) {
                eStoreLoger.Fatal(ex.Message, "", "", storeid, ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_systemname"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public Part getPartbyIdOrDisplayName(string _systemname, string storeid)
        {
            try
            {
                var systemname = _systemname.ToUpper();
                var _system = (from c in context.Parts.OfType<Product>()
                               where (c.DisplayPartno.ToUpper().Equals(systemname) || c.SProductID.ToUpper().Equals(systemname)) && c.StoreID.Equals(storeid)
                               select c).FirstOrDefault();
                if (_system != null)
                    _system.parthelper = this;
                return _system;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", storeid, ex);
                return null;
            }
        }

        /// <summary>
        /// This method returns list of active eStore products
        /// </summary>
        /// <param name="store"></param>
        /// <param name="partPrefix"></param>
        /// <returns></returns>
        public List<Product> getActiveStandardProducts(Store store, String partPrefix = null)
        {
            string[] validarray = getValidProductStatus(store);
            List<Product> products = null;

            lock (context)
            {
                if (String.IsNullOrWhiteSpace(partPrefix))
                {
                    products = (from pro in context.Parts.OfType<Product>()
                                where pro.StoreID == store.StoreID
                                      && pro.PublishStatus == true
                                      && validarray.Contains(pro.Status)
                                      && !(pro is Product_Ctos)
                                      && !(pro is Product_Bundle)
                                      && !pro.SProductID.EndsWith("BTO") && !pro.SProductID.EndsWith("-ES")
                                select pro).ToList<Product>();
                }
                else
                {
                    products = (from pro in context.Parts.OfType<Product>()
                                where pro.StoreID == store.StoreID
                                      && pro.PublishStatus == true
                                      && validarray.Contains(pro.Status)
                                      && !(pro is Product_Ctos)
                                      && !(pro is Product_Bundle)
                                      && pro.SProductID.StartsWith(partPrefix)
                                      && !pro.SProductID.EndsWith("BTO") && !pro.SProductID.EndsWith("-ES")
                                select pro).ToList<Product>();
                }
            }

            //keep context with each acquired part
            foreach (Product product in products)
                product.parthelper = this;

            return products;
        }

        /// <summary>
        /// This method returns list of active eStore products
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="partPrefix"></param>
        /// <returns></returns>
        public List<Product> getActiveStandardProducts(String storeID, String partPrefix = null)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeID);
            return getActiveStandardProducts(store, partPrefix);
        }

        /// <summary>
        /// Return product spec in matrix. 
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public List<VProductMatrix> getMatrix(Product part)
        {
            lock (context)
            {
                var p = from px in context.VProductMatrices
                        where px.ProductNo == part.SProductID
                        select px;

                return p.ToList();
            }
        }


        /// <summary>
        /// Return published and active CTOS product list. OM please set includeNonPublish = true
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Product_Ctos> getCTOSProducts(string storeid, bool includeNonPublish = false, Boolean includeSolutionOnly = false)
        {
            string[] validarray = getValidProductStatus(storeid, includeSolutionOnly);

            return getCTOSProductInCertainStates(storeid, validarray, includeNonPublish);

        }

        /// <summary>
        /// Return published and active CTOS product list by LimitResource Checking status. OM please set includeNonPublish = true
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Product_Ctos> getCTOSProductsByLimitResourceCheckingStatus(string storeid, bool limitResourceIsChecked, bool includeNonPublish = false, Boolean includeSolutionOnly = false)
        {
            string[] validarray = getValidProductStatus(storeid, includeSolutionOnly);

            return getCTOSProductInCertainStates(storeid, validarray, includeNonPublish).Where(p => p.LimitResourceIsChecked == limitResourceIsChecked).ToList();

        }

        /// <summary>
        /// This method is to retrieve solution only systems
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        public List<Product_Ctos> getSolutionOnlyCTOS(String storeID)
        {
            String[] solutionStates = { "SOLUTION_ONLY" };

            return getCTOSProductInCertainStates(storeID, solutionStates);
        }

        /// <summary>
        /// This method is to retrieve CTOS products that are in the specified states of input parameter
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="productStates"></param>
        /// <param name="includeNonPublish"></param>
        /// <returns></returns>
        private List<Product_Ctos> getCTOSProductInCertainStates(String storeID, String[] productStates, Boolean includeNonPublish= false)
        {
            List<Product_Ctos> systems;

            lock (context)
            {
                if (includeNonPublish == false)
                {
                    systems = (from pro in context.Parts.OfType<Product_Ctos>()
                               where pro.StoreID == storeID && productStates.Contains(pro.Status.ToUpper()) && pro.PublishStatus == true
                               select pro).ToList();
                }
                else
                {
                    systems = (from pro in context.Parts.OfType<Product_Ctos>()
                               where pro.StoreID == storeID
                               select pro).ToList();
                }
            }

            //need to keep helper context for navigation properties
            foreach (Product_Ctos item in systems)
                item.parthelper = this;     //item._helper = this;

            return systems;
        }

        /// <summary>
        /// Return published and active CTOS product list. OM please set includeNonPublish = true
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Product_Ctos> getCTOSProductsByProductLine(string storeid, string productline, bool includeNonPublish = false)
        {
            string[] validarray = getValidProductStatus(storeid);
            List<Product_Ctos> _parts;

            lock (context)
            {
                if (includeNonPublish == false)
                {
                    _parts = (from pro in context.Parts.OfType<Product_Ctos>()
                              where pro.StoreID == storeid && validarray.Contains(pro.Status.ToUpper()) && pro.ProductType == "CTOS" && pro.PublishStatus == true
                              && pro.CTOSProductLine == productline
                              select pro).ToList();
                }

                else
                {
                    _parts = (from pro in context.Parts.OfType<Product_Ctos>()
                              where pro.StoreID == storeid && pro.ProductType == "CTOS"
                               && pro.CTOSProductLine == productline
                              select pro).ToList();
                }
            }

            //need to keep helper context for navigation properties
            foreach (Product_Ctos item in _parts)
                item.parthelper = this;         // item._helper = this;

            return _parts.ToList();

        }

        public Part getPart(string sproductid, string storeid, bool includephaseout = false)
        {
            //In performance consideration, part instance usually will be cached for certain time before release.
            //get part shall start checkup from cached pool

            Part part = CachePool.getInstance().getPart(storeid, sproductid);
            if (part != null)
                return part;
            else
            {
                //Edward, include CTOS displaypartno matching.
                try
                {
                    if (includephaseout == false)
                    {
                        string producttype = Product.PRODUCTTYPE.CTOS.ToString();
                        string[] sapvalidarray = getValidSAPStatus(storeid);
                        lock (context)
                        {
                            part = (from f in context.Parts
                                    where (f.SProductID == sproductid || f.VendorProductName.Equals(sproductid))
                                        && f.StoreID == storeid && (sapvalidarray.Contains(f.StockStatus) || (f is Product_Ctos))
                                    select f).FirstOrDefault();
                        }
                    }
                    else
                    {
                        lock (context)
                        {
                            part = (from f in context.Parts
                                    where (f.SProductID == sproductid || f.VendorProductName.Equals(sproductid))
                                    && f.StoreID == storeid
                                    orderby f.SProductID == sproductid descending
                                    select f).FirstOrDefault();
                        }
                    }

                    if (part != null)
                    {
                        //parthelper needs to be kept with part for navigation properties retrieval
                        part.parthelper = this;

                        //cache this part for performance consideration. Caching time will be determined by CachePool implementation
                        CachePool.getInstance().cachePart(part);
                    }

                    return part;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// This method is to retrieve product group ID the specified product belongs to.
        /// </summary>
        /// <author>Jay</author>
        /// <param name="storeId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<String> getProductGroupIDs(Product product)
        {
            try
            {
                if (product is Product_Ctos)
                {
                    lock (context)
                    {
                        var result = from f in context.ProductCategroyMappings
                                     where f.SProductID == product.SProductID
                                     select f.CategoryPath;

                        return result.ToList<String>();
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<Part> prefetchPartList(String storeID, String partList)
        {
            if (!string.IsNullOrEmpty(partList))
            {
                return prefetchPartList(storeID, partList.Replace(";", ",").Split(',').ToList());
            }
            else
                return new List<Part>();
        }
        /// <summary>
        /// This method will prefetch parts used in CTOS from DB and cache them DB for performance boost
        /// </summary>
        /// <param name="ctos"></param>
        public List<Part> prefetchPartList(String storeID, List<string> partList)
        {
            List<Part> rlt = new List<Part>();
            try
            {
                if (partList != null && partList.Any())
                {
                    CachePool cache = CachePool.getInstance();
                    //String[] parts = partList.Replace(";",",").Split(',');
                    List<string> unchechedparts = new List<string>();
                    foreach (string p in partList)
                    {
                        Part cachedpart = cache.getPart(storeID, p);
                        if (cachedpart == null)
                        {
                            unchechedparts.Add(p);
                        }
                        else
                        {
                            rlt.Add(cachedpart);
                        }
                    }
                    if (unchechedparts.Any())
                    {
                        lock (context)
                        {
                            var query = (from f in context.Parts
                                         where f.StoreID == storeID && unchechedparts.Contains(f.SProductID)
                                         select f).ToList();

                            foreach (Part part in query)
                            {
                                //parthelper needs to be kept with part for navigation properties retrieval
                                part.parthelper = this;
                                rlt.Add(part);
                                cache.cachePart(part);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //error message here
                eStoreLoger.Warn("Prefect fails", "", "", "", ex);
                return new List<Part>();
            }
            return rlt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="partList"></param>
        /// <returns></returns>
        public List<Product> prefetchProductList(string storeID, List<string> partList)
        {
            List<Product> rlt = new List<Product>();
            try
            {
                if (partList != null && partList.Any())
                {
                    CachePool cache = CachePool.getInstance();
                    //String[] parts = partList.Replace(";",",").Split(',');
                    List<string> unchechedparts = new List<string>();
                    foreach (string p in partList)
                    {
                        Part cachedpart = cache.getPart(storeID, p);
                        if (cachedpart == null)
                            unchechedparts.Add(p);
                        else
                        {
                            if(cachedpart is POCOS.Product)
                                rlt.Add((cachedpart as POCOS.Product));
                        }
                    }
                    if (unchechedparts.Any())
                    {
                        lock (context)
                        {
                            var query = (from f in context.Parts.OfType<POCOS.Product>()
                                         where f.StoreID == storeID && (unchechedparts.Contains(f.SProductID) || unchechedparts.Contains(f.DisplayPartno))
                                         select f).ToList();

                            foreach (Product part in query)
                            {
                                //parthelper needs to be kept with part for navigation properties retrieval
                                part.parthelper = this;
                                rlt.Add(part);
                                cache.cachePart(part);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //error message here
                eStoreLoger.Warn("Prefect Product fails", "", "", "", ex);
                return new List<Product>();
            }
            return rlt;
        }

        public List<SAPProduct> prefetchSAPProductList(Store store, List<string> partNoList)
        {
            List<SAPProduct> rlt = new List<SAPProduct>();

            try
            {
                if (partNoList != null && partNoList.Any())
                {
                    string orgid = store.Settings["ProductLogisticsOrg"];
                    lock (context)
                    {
                        rlt = (from p in context.SAPProducts
                                         where p.ORG_ID == orgid && partNoList.Contains(p.PART_NO)
                                         select p).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                //error message here
                eStoreLoger.Warn("Prefect sapproduct fails", "", "", "", ex);
                return new List<SAPProduct>();
            }
            return rlt;
        }

        public void setATPs(Store store, Dictionary<Part, int> partnos)
        {

            SAPProxy sp = new SAPProxy();
            Hashtable parts = new Hashtable();

            foreach (Part a in partnos.Keys)
            {
                if (!parts.ContainsKey(a.SProductID))
                {
                    if (a.needRetrieveATP())
                        parts.Add(a.SProductID, partnos[a]);
                }
            }

            if (store.Settings == null)
            {
                store = new StoreHelper().getStorebyStoreid(store.StoreID);
            }
            if (!store.getBooleanSetting("getATPFromSAP"))
            {
                foreach (Part a in partnos.Keys)
                { 
                  a.atp = new ATP(DateTime.MaxValue, 0);
                }
                return;
            }
            try
            {
                if (parts.Count > 0)
                {
                    List<ProductAvailability> lpv = sp.getMultiATP(store.Settings["SAPDefaultPlant"], DateTime.Now, parts);

                    foreach (Part a in partnos.Keys)
                    {
                        var atpTemp = sp.GetAvailability(a.SProductID, lpv);
                        a.atp = new ATP(atpTemp.RequestDate, atpTemp.QtyATP) { Message = atpTemp.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                //getMultiATP non catch exception or others
                eStoreLoger.Warn("Exception at prefetch part ATP", "", "", "", ex);
            }

        }

        /// <summary>
        /// Return SAP partno hint, not include BTO, ES (engineer samples) and AGS parts
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Dictionary<string, string> getOrderbyPNHint(string keyword, Store store , bool isom)
        {
            string orgid = store.Settings["ProductLogisticsOrg"];
            string[] sapvalidarray = getValidSAPStatus(store.StoreID);

            List<string> sapvalidlist = new List<string>(sapvalidarray);

            if (!isom) {
                sapvalidlist.Add("O"); //include phaseout parts if it is not OM.
                sapvalidarray = sapvalidlist.ToArray();
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();

            var _parts = from p in context.SAPProducts
                         where p.ORG_ID == orgid
                         && p.PART_NO.ToLower().Contains(keyword.ToLower()) && sapvalidarray.Contains(p.STATUS.ToUpper())
                             //&&p.PRODUCT_LINE.ToUpper()!="HDD"
                         && ((isom == false && p.ABC_Ind != "T" && p.ABC_Ind != "P") || isom)//exclude temp or project parts at frontend
                         select new { PART_NO = p.PART_NO, PRODUCT_DESC = p.PRODUCT_DESC };

            if (_parts != null)
            {
                List<string> filterlist_arg = (from p in _parts select p.PART_NO).ToList();

                List<string> filterlist = filterInvalidPart(filterlist_arg, store);

                foreach (string p in filterlist)
                {
                    string desc = (from par in _parts where par.PART_NO.Equals(p) select par.PRODUCT_DESC).FirstOrDefault();
                    dic.Add(p, desc);
                }

            }
            return dic;
        }

        
        
        /// <summary>
        /// This method filter out the No Want To Show products from order by partno list
        /// </summary>
        /// <param name="_parts"></param>
        /// <param name="_store"></param>
        /// <returns></returns>
        private List<string> filterInvalidPart(List<string> _parts, Store _store)
        {
                      
            List<string> starwith = (from or in _store.OrderByPartnoExcludeRules
                           where or.MatchCriteria.Equals("STARTWITH")
                           select  or.ExcludePartno).ToList<string>();

            List<string> endwith = (from or in _store.OrderByPartnoExcludeRules
                           where or.MatchCriteria.Equals("ENDWITH")
                           select or.ExcludePartno ).ToList<string>();

            foreach (string pno in _parts.ToList())
            {
                foreach (string _startwith in starwith)
                {
                    if (pno.StartsWith(_startwith))
                        _parts.Remove(pno);
                }

                foreach (string _endwith in endwith)
                {
                    if (pno.EndsWith(_endwith))
                        _parts.Remove(pno);
                }

            }

            return _parts;
        
        }
         

        /// <summary>
        /// Return SAP partno hint, not include BTO, ES (engineer samples) and AGS parts
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public SAPProduct getSAPProduct(string partno, Store store)
        {
            string orgid = store.Settings["ProductLogisticsOrg"];

            lock (context)
            {
                var _parts = (from p in context.SAPProducts
                              where p.ORG_ID == orgid && p.PART_NO.ToUpper().Equals(partno.ToUpper())
                              select p).FirstOrDefault();


                return _parts;
            }
        }
        public List<PartGradePrice> getPartGradePrices(Part part)
        {
            return context.PartGradePrices.Where(x => x.StoreID == part.StoreID && x.SProductID == part.SProductID).ToList();
        }
        /// <summary>
        /// Get productspec according to product type
        /// </summary>
        /// <param name="p"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<VProductMatrix> getProductSpec(Part p, string storeid, bool omuse=false)
        {
            if (p is Product_Ctos)
            {
                if (omuse == false)
                    return getCTOSProductSpec(p.SProductID, storeid);
                else
                    return getOMCTOSProductSpec(p.SProductID,storeid);
            }
            else if (p is Product_Bundle)
            {
                Product_Bundle pb = (p as Product_Bundle);
                List<VProductMatrix> mergedSpecs = new List<VProductMatrix>();
                foreach (POCOS.Part specpart in pb.specSources)
                {
                    if (specpart.isMainStream())
                        mergedSpecs.AddRange(specpart.specs);
                }
                return mergedSpecs;
            }
            else
                return getSingleProductSpec(p.SProductID, storeid);
        }

        public List<VProductMatrix> getProductEPAPSSpec(Part p, string storeid, bool omuse = false)
        {
            
            var fields =
                (from mata in context.ProductCategoryMetadatas
                 from ptd in context.PTDProducts
                 where ptd.ProductCategoryId == mata.ProductCategoryId
                 && ptd.PartNo == p.SProductID
                 select mata).ToList();

            var values = (from matavaule in context.ProductCategoryMetadataValues
                          from ptd in context.PTDProducts
                          where ptd.PartNo == p.SProductID
                          && ptd.Id == matavaule.ProductId
                          select matavaule).ToList();
            string fieldName = string.Empty;
            string fieldValue = string.Empty;
            List<VProductMatrix> _spec = new List<VProductMatrix>();
            LanguagePackHelper langePackageHelper = new LanguagePackHelper();

            foreach (var v in values)
            {
                var field = fields.Where(x => x.Id == v.ProductCategoryMetadataId).FirstOrDefault();
                if (field != null)
                {
                    fieldName = field.FieldName;
                    fieldValue = v.FieldValue;

                    _spec.Add(new VProductMatrix
                    {
                        ProductNo = p.SProductID,
                        LocalCatName = fieldName,
                        LocalAttributeName = fieldName,
                        LocalValueName = fieldValue,
                        CatID = field.Id,
                        AttrID = field.Id,
                        AttrValueID = v.Id,
                        AttrCatName = field.FieldName,
                        AttrName = field.FieldName,
                        AttrValueName = v.FieldValue,
                        seq = field.OrdinalNo,
                        isProductSepc = field.ShowOnStore.GetValueOrDefault()
                    });
                }
            }
            if (omuse)
            {
                return _spec;
            }
            else
                return _spec.Where(x => x.isProductSepc).ToList();
        }

        /// <summary>
        /// Get Single product Spec from PIS system.
        /// </summary>
        /// <param name="sproductid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private List<VProductMatrix> getSingleProductSpec(string sproductid, string storeid)
        {
            List<VProductMatrix> retspecs = new List<VProductMatrix>();
            lock (context)
            {
                var _specs = (from sp in context.VProductMatrices
                              where sp.ProductNo.ToUpper().Equals(sproductid.ToUpper())
                              select new
                              {
                                  sp.LocalCatName,
                                  sp.AttrCatName,
                                  sp.AttrName,
                                  sp.AttrValueName,
                                  sp.LocalAttributeName,
                                  sp.LocalValueName,
                                  sp.seq,
                                  sp.AttrID,
                                  sp.AttrValueID,
                                  sp.CatID,
                                  sp.ProductNo,
                                  sp.SpecSetID
                              }).Distinct().ToList();

                foreach (var s in _specs)
                {
                    VProductMatrix v = new VProductMatrix();
                    v.AttrCatName = s.AttrCatName;
                    v.AttrName = s.AttrName;
                    v.AttrValueName = s.AttrValueName;
                    v.LocalCatName = s.LocalCatName;
                    v.LocalAttributeName = s.LocalAttributeName;
                    v.LocalValueName = s.LocalValueName;
                    v.AttrID = s.AttrID;
                    v.AttrValueID = s.AttrValueID;
                    v.CatID = s.CatID;
                    v.ProductNo = s.ProductNo;
                    v.SpecSetID = s.SpecSetID;
                    retspecs.Add(v);
                }
            }

            return retspecs;
        }

        /// <summary>
        /// Edward, Get CTOS product Spec, migrate from 2.0
        /// </summary>
        /// <param name="sproductid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private  List<VProductMatrix> getCTOSProductSpec(string sproductid, string storeid)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeid);
            List<VProductMatrix> _ctosspec = new List<VProductMatrix>();
            lock (context)
            {

                var _specs = from sp in context.CTOSSpecs
                             where sp.SproductID == sproductid && sp.StoreID == storeid && sp.LangID.ToUpper().Equals(store.StoreLangID.ToUpper())
                             orderby sp.Sequence
                             select sp;
                //ctos SpecMask 的cat = attr,attr = attrValue

                foreach (CTOSSpec cts in _specs.ToList())
                {
                    VProductMatrix vpm = new VProductMatrix();
                    vpm.ProductNo = cts.SproductID;
                    vpm.LocalCatName = cts.AttrName;
                    vpm.LocalAttributeName = cts.AttrName;
                    vpm.LocalValueName = cts.AttrValue_name;

                    vpm.CatID = cts.AttrID;
                    vpm.AttrID = cts.AttrID;
                    vpm.AttrValueID = cts.AttrValueID;

                    vpm.AttrCatName = cts.AttrName;
                    vpm.AttrName = cts.AttrName;
                    vpm.AttrValueName = cts.AttrValue_name;
                    vpm.seq = cts.Sequence;
                    _ctosspec.Add(vpm);
                }
            }

            return _ctosspec;
        }


        public List<Product> getProductsBySproductIDList(string[] sproductIDs, string storeid)
        {
            List<Product> products = null;
            lock (context)
            {
                var _products = (from ps in context.Parts.OfType<Product>()
                                 from pn in sproductIDs
                                 where ps.SProductID == pn && ps.StoreID == storeid && ps.PublishStatus == true
                                 select ps);

                products = _products.ToList();
            }
            correlateHelperWithParts(products);
            return products;
        }

        public List<Product> getProductsByState(Product.PRODUCTSTATUS productstatus, Store store,MiniSite site,int take = 10)
        {
            string _productstatus = productstatus.ToString();
            List<Product> products = null;
            lock (context)
            {
                
                if (site == null)
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                 where ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).Distinct().Take(take).ToList();
                }
                else
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                 from pcm in context.ProductCategroyMappings
                                 from pc in context.ProductCategories
                                 where
                                 pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                 && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                 && pc.MiniSiteID == site.ID &&
                                 ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).Distinct().Take(take).ToList();
                }
            }
            correlateHelperWithParts(products);
            return products;

        }

        public List<Product> getProductsByState(Product.PRODUCTMARKETINGSTATUS productstatus, Store store, MiniSite site, int take = 10)
        {
            List<Product> products = null;
            lock (context)
            {
                int markSt = (int)Math.Pow(2, (int)productstatus);
                if (site == null)
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                where (ps.MarketingStatus & markSt) == markSt
                                && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).Distinct().Take(take).ToList();
                }
                else
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == site.ID && (ps.MarketingStatus & markSt) == markSt
                                && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).Distinct().Take(take).ToList();
                }
            }
            correlateHelperWithParts(products);
            return products;

        }

        public List<string> getProductsIDsByState(Product.PRODUCTSTATUS productstatus, Store store, MiniSite site, int take = 10)
        {
            string _productstatus = productstatus.ToString();
            List<string> productids = null;
            lock (context)
            {
                if (site == null)
                {
                    productids = (from ps in context.Parts.OfType<Product>()
                                where ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps.SProductID).Distinct().Take(take).ToList();
                }
                else
                {
                    productids = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == site.ID &&
                                ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps.SProductID).Distinct().Take(take).ToList();
                }
            }
            return productids;
        }

        public List<Product> getProductsByStateOrderByLastUpdate(Product.PRODUCTSTATUS productstatus, Store store, MiniSite site, int take = 10) 
        {
            string _productstatus = productstatus.ToString();
            List<Product> products = null;
            lock (context)
            {

                if (site == null)
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                where ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).ToList().Distinct().Take(take).ToList();
                }
                else
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == site.ID && (pc.Publish.HasValue && pc.Publish.Value) && 
                                ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                orderby ps.ProductLastUpdated descending
                                select ps).ToList().Distinct().Take(take).ToList();
                }
            }
            correlateHelperWithParts(products);
            return products;

        }
        
        public List<Product> getCategoryOptionByState(Product.PRODUCTSTATUS productstatus, Store store, MiniSite site, ProductCategory cate)
        {
            string _productstatus = productstatus.ToString();
            List<Product> products = null;
            lock (context)
            {
                if (site == null)
                {
                    return new List<Product>();
                }
                else
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == site.ID && (pc.Publish.HasValue && pc.Publish.Value) &&
                                cate.Storeid == pc.Storeid && cate.CategoryID == pc.CategoryID && cate.CategoryPath == pc.CategoryPath &&
                                ps.Status.ToUpper() == _productstatus.ToUpper() && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).ToList().Distinct().ToList();
                }
            }

            if (cate.childCategoriesX.Any())
	        {
                foreach(var c in cate.childCategoriesX)
                    products.AddRange(getCategoryOptionByState(productstatus, store, site, c));
	        }
            correlateHelperWithParts(products);
            return products;
        }

        public List<Product> getCategoryOptionByState(Product.PRODUCTMARKETINGSTATUS productstatus, Store store, MiniSite site, ProductCategory cate)
        {
            List<Product> products = null;
            lock (context)
            {
                int markSt = (int)Math.Pow(2, (int)productstatus);
                if (site == null)
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == null && (pc.Publish.HasValue && pc.Publish.Value) &&
                                cate.Storeid == pc.Storeid && cate.CategoryID == pc.CategoryID && cate.CategoryPath == pc.CategoryPath &&
                                (ps.MarketingStatus & markSt) == markSt && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).ToList().Distinct().ToList();
                }
                else
                {
                    products = (from ps in context.Parts.OfType<Product>()
                                from pcm in context.ProductCategroyMappings
                                from pc in context.ProductCategories
                                where
                                pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                                && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                                && pc.MiniSiteID == site.ID && (pc.Publish.HasValue && pc.Publish.Value) &&
                                cate.Storeid == pc.Storeid && cate.CategoryID == pc.CategoryID && cate.CategoryPath == pc.CategoryPath &&
                                (ps.MarketingStatus & markSt) == markSt && ps.StoreID == store.StoreID && ps.PublishStatus == true
                                select ps).ToList().Distinct().ToList();
                }
            }

            if (cate.childCategoriesX.Any())
            {
                foreach (var c in cate.childCategoriesX)
                    products.AddRange(getCategoryOptionByState(productstatus, store, site, c));
            }
            correlateHelperWithParts(products);
            return products;
        }

        public List<string> getProducts(MiniSite site)
        {
            string key = string.Format("{0}_{1}_Products", site.StoreID, site.SiteName);
            CachePool cache = CachePool.getInstance();
            List<string> products = (List<string>)cache.getObject(key);
            if (products == null)
            {
                string[] validstatus = getValidProductStatus(site.StoreID);
                products = (from ps in context.Parts.OfType<Product>()
                            from pcm in context.ProductCategroyMappings
                            from pc in context.ProductCategories
                            where
                            pc.Storeid == pcm.StoreID && pc.CategoryID == pcm.CategoryID
                            && pcm.StoreID == ps.StoreID && pcm.SProductID == ps.SProductID
                            && pc.MiniSiteID == site.ID &&
                        validstatus.Contains(ps.Status.ToUpper()) && ps.StoreID == site.StoreID && ps.PublishStatus == true
                            select ps.SProductID).Distinct().ToList();
                if (products == null)
                    products = new List<string>();

                cache.cacheObject(key, products, CachePool.CacheOption.Hour2);
            }
            return products;
        }


/// <summary>
/// Search Parts Orderby PN
/// </summary>
/// <param name="keyword"></param>
/// <param name="store"></param>
/// <param name="includeInvalidStatus">OM use true</param>
/// <returns></returns>
        public List<Part> searchPartsOrderbyPN(string keyword, Store store, bool includeInvalidStatus = false)
        {
            PISEntities pcontext = new PISEntities();
            string orgid = store.Settings["ProductLogisticsOrg"];
            string priceSAPOrg = store.Settings["PriceSAPOrg"];
            string priceSAPLvlL1 = store.Settings["PriceSAPLvlL1"];
            string[] validstatus = getValidSAPStatus(store.StoreID);
            List<Part> _sapParts = new List<Part>();
            SAPProxy sapproxy = new SAPProxy();
            List<string> partnos = new List<string>();
            if (!includeInvalidStatus)
            {
                lock (context)
                {
                    partnos = (from p in context.SAPProducts
                               where p.ORG_ID == orgid
                                  && p.PART_NO.ToUpper().Contains(keyword.ToUpper())
                                   && validstatus.Contains(p.STATUS.Trim().ToUpper())
                               select p.PART_NO).ToList();
                }
                partnos = filterInvalidPart(partnos, store);

            }
            else
            {
                lock (context)
                {
                    //PRODUCT_TYPE = ZCTO,  这样的产品是虚拟料号
                    partnos = (from p in context.SAPProducts
                               where p.ORG_ID == orgid && p.PRODUCT_TYPE != "ZCTO" 
                                  && p.PART_NO.ToUpper().Contains(keyword.ToUpper())
                               select p.PART_NO).ToList();
                }
                partnos = filterInvalidPart(partnos, store);

            }

            //List<string> pricelist = new List<string>();
            foreach (string pn in partnos.ToList())
            {
                Part sapPart = getPart(pn, store, includeInvalidStatus);
                if (sapPart == null)
                {

                    sapPart = new Part();
                    sapPart.SProductID = pn;
                    sapPart.VProductID = pn;
                    sapPart.PriceSourceProvider = "SAP";
                    sapPart.StoreID = store.StoreID;
                    sapPart.VendorID = "Advantech" + store.StoreID;
                    sapPart.LastUpdated = DateTime.Now;
                    sapPart.LastUpdatedBy = "OrderByPartno";
                    sapPart.CreatedDate = DateTime.Now;
                    sapPart.CreatedBy = "OrderByPartno";

                    //Get Price from SAP
                    //  sapPart.VendorSuggestedPrice = sapproxy.getProductPriceFromSap(pn, priceSAPOrg, priceSAPLvlL1);
                    //Get Part data from SAP tables
                }

                setLogistic(store, sapPart);
                //pricelist.Add(pn);
                _sapParts.Add(sapPart);
            }

            //Batch get price from SAP

            PISSync pissync = new PISSync();

            //Dictionary<string, decimal> plist = pissync.getPriceFromSAP(pricelist, store);
            String error = pissync.syncPrice(store, _sapParts, false);

            foreach (Part p in _sapParts.ToList())
            {
                pissync.SyncPIS(p, store);

                /* replaced by syncPrice method
                if (plist.ContainsKey(p.SProductID) == true) {

                    if (p.VendorSuggestedPrice != plist[p.SProductID])
                        p.hasProductPriceChanged  = true;

                    p.VendorSuggestedPrice = plist[p.SProductID];
                }
                 * */
            }

            return _sapParts;

        }

        /// <summary>
        /// Add part to database , prepare for add to cart/quote
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public Product AddtoStore(string partno, Store store, bool issave = true,bool isSaveAllSatus = false)
        {
            //trim space from partno
            partno = partno.Trim();
            //validate the existance of adding part.  If it's already an exist product, return null.  If it's part-only or not exist, then move on
            //to register this part as eStore product.
            Part existingPart = getPart(partno, store, true);
            if (existingPart != null && existingPart is Product)        //the adding part is already registered as eStore product
                return ((Product)existingPart);

            //start registering new part as eStore product

            //construct new product from SAP part
            PISSync pissync = new PISSync();
            string[] sapvalidstatus = getValidSAPStatus(store.StoreID);
            Product sapPart = pissync.constructPart(partno, store, false);
            //validate part status.  If part is not sellable, then return immediately
            //some time will save all stockstatus products ,set isSaveAllSatus true
            if (!isSaveAllSatus)
            {
                if (!sapvalidstatus.Contains(sapPart.StockStatus))
                    return null;
            }

            //fill up product information            
            Product p = pissync.constructProduct(sapPart, false);

            if (string.IsNullOrEmpty(p.VendorProductDesc) == false)
            {
                //List<string> partsno = new List<string>();
                //partsno.Add(partno);
                /*
                Dictionary<string, decimal> pricelist = pissync.getPriceFromSAP(partsno, store);
                if (pricelist.ContainsKey(partno))
                    p.StorePrice = pricelist[partno];
                else
                    p.StorePrice = 0;
                p.VendorSuggestedPrice = p.StorePrice;
                 * */
                List<Part> partList = new List<Part>();
                partList.Add(p);
                pissync.syncPrice(store, partList, false);  //update price without save the update

                p.PriceSource = Product.PRICESOURCE.VENDOR.ToString();
                p.parthelper = this;

                if (issave == true)
                {
                    int result = save(p);
                    if (result < 0)
                        return null;
                }

                return p;
            }
            else //Vendor product description can not be empty
            {
                return null;
            }

        }

        /// <summary>
        ///  OM use, only add Part to database, not Product.
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="store"></param>
        /// <param name="issave"></param>
        /// <returns></returns>
        public Part AddParttoStore(string partno, Store store, bool issave = true)
        {
            //trim space from partno
            partno = partno.Trim();

            //check if part already exists in DB, if yes, then return it without further process
            Part part = getPart(partno, store, true);
            if (part != null)
                return part;

            PISSync pissync = new PISSync();
            string[] sapvalidstatus = getValidSAPStatus(store.StoreID);
            //Part p = pissync.constructPart(partno, store, false);

            Part p2 = new Part();
            p2.SProductID = partno;
            p2.StoreID = store.StoreID;
            setLogistic(store, p2);

            //if part's status is not in publishable mode, then stop and return null
            if (!sapvalidstatus.Contains(p2.StockStatus))
                return null;

            p2.VProductID = partno;
            p2.VendorID = "Advantech" + store.StoreID;
            p2.CreatedBy = "Sync Job";
            p2.LastUpdatedBy = "Sync Job";
            p2.CreatedDate = DateTime.Now;
            p2.LastUpdated = DateTime.Now;
            p2.priceSource = Part.PRICESOURCE.VENDOR;         
 
           // Product p = pissync.constructProduct(sapPart, false);

            if (string.IsNullOrEmpty(p2.VendorProductDesc) == false)
            {
                p2.parthelper = this;

                /*
                List<string> partsno = new List<string>();
                partsno.Add(partno);
                Dictionary<string, decimal> pricelist = pissync.getPriceFromSAP(partsno, store); 
                p2.VendorSuggestedPrice = pricelist[partno];              
                 * */
                List<Part> partList = new List<Part>();
                partList.Add(p2);
                pissync.syncPrice(store, partList, false);  //update price without saving
                pissync.SyncPIS(p2, store); // snyc pis informations
                if (issave == true)
                {
                    if (save(p2) < 0)
                        return null;
                }

                return p2;
            }
            else //partno not exist in SAP
            {
                return null;
            }
        }
        //get comingsoon from pis
        public Product getComingSoonProductFromPIS(string partno, Store store, bool issave = true)
        {
            partno = partno.Trim();
            Part existingPart = getPart(partno, store, true);
            if (existingPart is Product)        //the adding part is already registered as eStore product
                return ((Product)existingPart);

            PISHelper pisHelper = new PISHelper();
            Sync.PRODUCT comingSoonPart = pisHelper.getProductListInPISByKeyWord(partno).FirstOrDefault();
            if (comingSoonPart != null)
            {
                Product newProduct = new Product();
                newProduct.StoreID = store.StoreID;
                newProduct.SProductID = partno;
                newProduct.VProductID = partno;
                newProduct.DisplayPartno = partno;
                newProduct.VendorProductName = partno;
                newProduct.VendorID = "Advantech" + store.StoreID;
                newProduct.CreatedBy = "Sync Job";
                newProduct.CreatedDate = DateTime.Now;
                newProduct.LastUpdatedBy = "Sync Job";
                newProduct.LastUpdated = DateTime.Now;
                newProduct.PriceSourceProvider = Part.PRICESOURCE.VENDOR.ToString();//Part
                newProduct.PriceSource = Part.PRICESOURCE.VENDOR.ToString();//Product PriceSouce
                newProduct.ProductType = Product.PRODUCTSTATUS.COMING_SOON.ToString();//Part ProductType
                newProduct.Status = Product.PRODUCTSTATUS.COMING_SOON.ToString();
                newProduct.VendorSuggestedPrice = 0;
                newProduct.LocalPrice = 0;
                newProduct.StorePrice = 0;
                newProduct.ProductFeatures = "";
                newProduct.PriceType = "Price";
                newProduct.StockStatus = "S";
                newProduct.TumbnailImageID = "https://buy.advantech.com/images/photounavailable.gif";
                newProduct.ImageURL = "https://buy.advantech.com/images/photounavailable.gif";
                newProduct.ModelNo = "";
                newProduct.VendorProductDesc = comingSoonPart.PRODUCT_DESC;
                newProduct.ProductDesc = comingSoonPart.PRODUCT_DESC;
                newProduct.VendorExtendedDesc = comingSoonPart.EXTENTED_DESC;
                newProduct.ExtendedDesc = comingSoonPart.EXTENTED_DESC;
                newProduct.ShipWeightKG = comingSoonPart.ship_weight.HasValue ? (decimal)comingSoonPart.ship_weight : 0;
                newProduct.NetWeightKG = comingSoonPart.net_weight.HasValue ? (decimal)comingSoonPart.net_weight : 0;
                newProduct.DimensionHeightCM = comingSoonPart.DimensionH.HasValue ? (decimal)comingSoonPart.DimensionH : 0;
                newProduct.DimensionLengthCM = comingSoonPart.DimensionL.HasValue ? (decimal)comingSoonPart.DimensionL : 0;
                newProduct.DimensionWidthCM = comingSoonPart.DimensionW.HasValue ? (decimal)comingSoonPart.DimensionW : 0;
                new eStore.POCOS.Sync.PISSync().SyncComingSoonFromPIS(newProduct, store);
                newProduct.parthelper = this;
                if (issave)
                {
                    int result = save(newProduct);
                    if (result < 0)
                        return null;
                }

                return newProduct;
            }
            return null;
        }

        public List<Part> getComingSoonPartFromPIS(List<POCOS.Sync.PRODUCT> pisProductList, Store store)
        {
            List<POCOS.Part> partList = new List<Part>();
            if (pisProductList != null)
            {
                foreach (var item in pisProductList)
                {
                    POCOS.Part convertPart;
                    POCOS.SAPProduct sapProdcut = getSAPProduct(item.PART_NO,store);
                    convertPart = getPart(item.PART_NO, store.StoreID, true);
                    if (convertPart == null)
                    {
                        convertPart = new Part();
                        convertPart.StoreID = store.StoreID;
                        convertPart.ModelNo = "";
                        convertPart.SProductID = item.PART_NO;
                        convertPart.VProductID = item.PART_NO;
                        convertPart.TumbnailImageID = "https://buy.advantech.com/images/photounavailable.gif";
                        convertPart.VendorProductDesc = item.PRODUCT_DESC;
                        convertPart.VendorExtendedDesc = item.EXTENTED_DESC;
                        convertPart.VendorSuggestedPrice = 0;
                        convertPart.ShipWeightKG = 0;
                        if (item.ship_weight != null && item.ship_weight.HasValue)
                            convertPart.ShipWeightKG = (decimal)item.ship_weight.Value;                        
                        convertPart.StockStatus = "S";
                        convertPart.error_message = null;
                        convertPart.isSAPParts = sapProdcut != null;
                        new eStore.POCOS.Sync.PISSync().SyncComingSoonFromPIS(convertPart, store);
                    }
                    partList.Add(convertPart);
                }
            }
            return partList;
        }
        /// <summary>
        /// Get SAP Product data from PIS
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>
        public SAP_PRODUCT getSAPProductFromPIS(string partno)
        {
            return (new PISHelper()).getSAPProductInPISByPartNo(partno);
        }
        /// <summary>
        /// Only return data 
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="store"></param>
        /// <returns></returns>

        public Part getPartOrderbyPartno(string partno, Store store)
        {
            //trim space from partno
            partno = partno.Trim();

            Part estoreproduct = getPart(partno, store, true);
            if (estoreproduct != null)
                return estoreproduct;

            estoreproduct = AddParttoStore(partno, store, false);
            return estoreproduct;
        }



        public List<Product_Ctos> searchCTOSbyPN(string partno, Store store)
        {
            string[] validstatus = getValidProductStatus(store.StoreID);
            string[] partnolist = partno.Split(',');

            List<Product_Ctos> productList = null;
            lock (context)
            {
                var _ctosproducts = from p in context.Parts.OfType<Product_Ctos>()
                                     from cb in context.CTOSBOMs
                                     //from cc in context.CTOSComponents
                                     from cd in context.CTOSComponentDetails
                                     where p.SProductID == cb.SProductID && p.StoreID == cb.StoreID && p.StoreID == store.StoreID
                                     && cb.ComponentID == cd.ComponentID && cb.StoreID == cd.StoreID &&
                                     (cd.SProductID == partno || partnolist.Contains(p.DisplayPartno.ToUpper())) && p.PublishStatus == true && validstatus.Contains(p.Status)
                                     select p;

                productList = _ctosproducts.Distinct().ToList();
            }
            foreach (Product_Ctos ctos in productList)
                ctos.parthelper = this;     //ctos._helper = this;

            return productList;

        }

        /// <summary>
        /// To get Product list by CTOS Component to see where to use, for OM use
        /// </summary>
        /// <param name="componentid"></param>
        /// <returns></returns>
        public List<Product> getCTOSProductsbyComponentid(int componentid,string storeid)
        {
            List<Product> products = null;
            lock (context)
            {
                products = (from p in context.Parts.OfType<Product>()
                            from ctos in context.CTOSBOMs
                            where ctos.ComponentID == componentid && p.SProductID == ctos.SProductID && p.StoreID == ctos.StoreID
                            && p.StoreID == storeid
                            select p).ToList();
            }
            return products;
        }

        /// <summary>
        /// This method serves only AOnline solution.  
        /// </summary>
        /// <param name="partNo"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public Product getSolutionProduct(String partNo, Store store)
        {
            Part part = (from p in context.Parts
                            where p.SProductID == partNo && p.StoreID == store.StoreID
                            select p).FirstOrDefault();
            if (part == null)
            {
                PISSync pisSync = new PISSync();
                return pisSync.constructSolutionProduct(partNo, store);
            }
            else
            {
                if (part is Product)
                    return (Product)part;
                else
                {
                    PISSync pisSync = new PISSync();
                    return pisSync.constructSolutionProduct(part, store);                    
                }
            }
        }

        /// <summary>
        /// get information from SAP tables and assign to Parts
        /// </summary>
        /// <param name="_store"></param>
        /// <param name="_part"></param>
        private void setLogistic(Store _store, Part _part)
        {
            string logisticorg = getSetting(_store, "ProductLogisticsOrg");
            var _plogis = (from parts in context.SAPProducts
                           where parts.PART_NO == _part.SProductID && parts.ORG_ID == logisticorg
                           select parts).FirstOrDefault();

            if (_plogis != null)
            {
                _part.Cost = _plogis.Cost;

                if (_part.StockStatus != _plogis.STATUS)
                    _part.hasProductStockStatusChanged = true;

                _part.StockStatus = _plogis.STATUS;
                _part.VendorProductDesc = _plogis.PRODUCT_DESC;
                _part.VendorProductGroup = _plogis.PRODUCT_GROUP;
                _part.VendorProductLine = _plogis.PRODUCT_LINE;
                _part.ModelNo = _plogis.MODEL_NO;
                //_part.VendorProductName = _plogis.PRODUCT_DESC;
                _part.VendorProductName = _plogis.PART_NO;
                _part.ShipWeightKG = _plogis.SHIP_WEIGHT;
                _part.PriceType = "Price";
                if (String.IsNullOrWhiteSpace(_part.ProductType))
                    _part.ProductType = "Parts";
                _part.RoHSStatus = _plogis.RoHS_Status;
                _part.Currency = _plogis.Currency;
                _part.ProductinfoProvider = "AdvantechSAP";
                _part.ABCInd = _plogis.ABC_Ind;
                _part.ProductDivision = _plogis.Product_Division;
                if (_part.LocalPrice.HasValue == false)
                    _part.LocalPrice = 0;
                _part.Certificate = _plogis.CERTIFICATE;
                _part.ProductSite = _plogis.PRODUCT_SITE;
                _part.CurrentStockQty = 99999;
                _part.NetWeightKG = _plogis.NET_WEIGHT;
                _part.Dimension = _plogis.Dimension;
                _part.setDimension();

                context.SAPProducts.Detach(_plogis);
            }

        }

        public bool iseStoreproduct(string partno, string storeid)
        {
            if (string.IsNullOrEmpty(partno))
            {
                return false;
            }
            string modelname = getModelname(partno);

            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);
            return  (from p in context.Parts.OfType<Product>()
                             where (p.SProductID.ToUpper().Equals(partno.ToUpper()) || p.DisplayPartno.StartsWith(modelname))
                             && p.StoreID == storeid && validarray.Contains(p.Status) && sapvalidarray.Contains(p.StockStatus)
                             select p).Any();

        }

        public bool iseStoreCTOSPart(string partno, string storeid)
        {
            if (string.IsNullOrEmpty(partno))
            {
                return false;
            }
            string modelname = getModelname(partno);

            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);
            return (from p in context.Parts.OfType<Product_Ctos>()
                    from part in context.Parts
                    from cb in context.CTOSBOMs
                    from cbdetail in context.CTOSComponentDetails
                    where p.SProductID == cb.SProductID && p.StoreID == storeid && cb.StoreID == storeid
                         && validarray.Contains(p.Status)
                         && cb.ComponentID == cbdetail.ComponentID && cb.StoreID == cbdetail.StoreID &&
                         (cbdetail.SProductID.ToUpper().Equals(partno) || p.DisplayPartno.StartsWith(modelname))
                         && cbdetail.SProductID == part.SProductID && p.StoreID == part.StoreID
                         && sapvalidarray.Contains(part.StockStatus)
                    select p).Any();

        }


        /// <summary>
        /// For WWW website use, return the products and ctoss list if the product exists.
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="store"></param>
        /// <returns></returns>

        public bool iseStoreproduct(string partno, string storeid, out List<String> products, out List<String> ctoss)
        {
            if (string.IsNullOrEmpty(partno))
            {
                products = null;
                ctoss = null;
                return false;
            }


            string modelname = getModelname(partno);

            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);

            List<String> _part = (from p in context.Parts.OfType<Product>()
                                   where (p.SProductID.ToUpper().Equals(partno.ToUpper()) || p.SProductID.StartsWith(modelname))
                             && p.StoreID == storeid && validarray.Contains(p.Status) && sapvalidarray.Contains(p.StockStatus) && p.PublishStatus == true
                             select p.SProductID).ToList() ;

            List<String> _ctos = (from p in context.Parts.OfType<Product_Ctos>()
                                 // from part in context.Parts
                                  from cb in context.CTOSBOMs
                                  from cbdetail in context.CTOSComponentDetails
                                  where p.SProductID == cb.SProductID && p.StoreID == storeid && cb.StoreID == storeid
                                       && validarray.Contains(p.Status)
                                       && cb.ComponentID == cbdetail.ComponentID && cb.StoreID == cbdetail.StoreID
                                       && (cbdetail.SProductID.ToUpper().Equals(partno.ToUpper()) || cbdetail.SProductID.ToUpper().StartsWith(modelname) || p.DisplayPartno.StartsWith(modelname))
                                       && p.PublishStatus == true
                                       && (from pp in context.Parts 
                                           where sapvalidarray.Contains(pp.StockStatus) && pp.StoreID == storeid 
                                           select pp.SProductID).Contains(cbdetail.SProductID)
                                      
                                  select p.SProductID).Distinct().ToList() ;

            products = _part;
            ctoss = _ctos;

            if (_part != null || _ctos != null)
                return true;
            else
                return false;

        }

        public List<string> geteStoreOrderableProductsByModel(string modelno, string storeid)
        { 
            return context.geteStoreOrderableProductsByModel(storeid, modelno).Select(x => x.SProductID).ToList();
        }
        public string link2eStore(string modelno, string storeid)
        {
            return string.Join("",  context.spLink2eStore(storeid, modelno, null).ToList());
        }

        public List<spLink2eStoreRawResult> link2eStoreRaw(string modelno, string pn, string storeid)
        {
            return context.spLink2eStoreRaw(storeid, modelno, pn).ToList();
        }
        public Dictionary<string, List<string>> geteStoreSystemsByOptionModel(string modelno, string storeid)
        {
            Dictionary<string, List<string>> _ctos = (from c in context.geteStoreSystemsByOptionModel(storeid, modelno)
                                                      group c.ProductID by c.PARTNO into g
                                                      select g).ToDictionary(x => x.Key, y => y.ToList());
            return _ctos;
        }
        
        public Dictionary<string, List<string>> geteStoreSystemsByOptionParts(string[] parts, string storeid)
        {
            string partsstring = string.Join(",", parts);
            Dictionary<string, List<string>> _ctos = (from c in context.geteStoreSystemsByOptionParts(storeid, partsstring)
                                                      group c.ProductID by c.PARTNO into g
                                                      select g).ToDictionary(x => x.Key, y => y.ToList());
            return _ctos;

        }
        /// <summary>
        ///  return string before 2nd hypen '-', like SYS-4W5120-4U01, will return SYS-4W5120
        /// </summary>
        /// <param name="partno"></param>
        /// <returns></returns>

        public string getModelname(string partno) {

            string modelname = partno;
            if (string.IsNullOrEmpty(partno) == false && partno.IndexOf('-') > 0)
            {
                string[] p = partno.Split('-');
                if(p.Length>=2)
                modelname = p[0] + "-" + p[1];
            }

            return modelname;
        }


        private string getSetting(Store _store, string parameter)
        {

            return _store.Settings[parameter];

        }

        private void correlateHelperWithParts(IList<Part> parts)
        {
            foreach (Part part in parts.ToList())
                correlateHelperWithPart(part);
        }

        private void correlateHelperWithParts(IList<Product> products)
        {
            foreach (Product product in products.ToList())
                correlateHelperWithPart(product);
        }

        private void correlateHelperWithPart(Part part)
        {
            //parthelper needs to be kept with part for navigation properties retrieval
            part.parthelper = this;

            //cache this part for performance consideration. Caching time will be determined by CachePool implementation
            CachePool.getInstance().cachePart(part);
        }

        /// <summary>
        /// Sync all partno from PIS and refresh price
        /// It accept a CTOS or Standard product or Part.
        /// if all sync success, it will return "", else return error message
        /// </summary>
        /// <param name="ctos"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        /// 
        public string syncPartInfo(Part part, Store store) {
            try
            {
            PISSync pissync = new PISSync();

            if (part is Product_Ctos)
            {
                Product_Ctos ctos = (Product_Ctos)part;
                foreach (CTOSBOM c in ctos.CTOSBOMs)
                {
                    string partlist = c.getPartList();
                    if (!string.IsNullOrEmpty(partlist))
                    {
                        string[] partnolist = partlist.Split(',');

                        foreach (string pno in partnolist)
                        {
                            pissync.syncAllPIS(store, pno);
                            pissync.SyncPrice(store, pno);
                        }
                    }

                }

            }

            else
            {

                pissync.syncAllPIS(store, part.SProductID);
                pissync.SyncPrice(store, part.SProductID);

            }
             
                return "";
            }
            catch (Exception ex)
            {
                eStoreLoger.Info("Sync " + part.SProductID + "Failed", "", "", store.StoreID, ex);
                return ex.Message;
            }

        }

        /// <summary>
        /// get all idk attributes by product
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="productid"></param>
        /// <returns></returns>
        public List<IDKAttribute> getIDKAttributes(string storeid,string productid)
        {
            Dictionary<string, int> displayingAttributes = new Dictionary<string, int>();
            displayingAttributes.Add("Size",1);
            //displayingAttributes.Add("Resolution",2);
            displayingAttributes.Add("Original Brightness",3);
            displayingAttributes.Add("Touch Screen",2);

            lock (context)
            {
                return (from addon in context.PeripheralAddOns
                        from item in context.PeripheralAddOnBundleItems
                        from attribute in context.IDKAttributes
                        where item.StoreID == addon.StoreID
                        && item.AddOnItemID == addon.AddOnItemID
                        && item.SProductID == attribute.SProductID
                        && displayingAttributes.Keys.Contains(attribute.AttributeName)
                        && item.StoreID == storeid
                        && addon.SProductID == productid
                        select attribute).ToList()
                        .OrderBy(x => displayingAttributes[x.AttributeName])
                        .ToList();
            }
        }

        /// <summary>
        /// filter addons by selected attibutes
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="productid"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<string> getMatchedAddonsByAttributes(string storeid, string productid, Dictionary<string,string> filter)
        {
            List<string> rlt = new List<string>();
            if (filter != null || filter.Any())
            {
               
                //filter matched values, then checking name and value are matched
                string[] filtervalues = filter.Select(x => x.Value).ToArray();
                lock (context)
                {
                    var temp = (from attribute in context.IDKAttributes
                                from addon in context.PeripheralAddOns
                                from item in context.PeripheralAddOnBundleItems
                                where item.StoreID == addon.StoreID
                                && item.AddOnItemID == addon.AddOnItemID
                                && item.StoreID == storeid
                                && item.SProductID == attribute.SProductID
                                && addon.SProductID == productid
                                && (addon.Publish.HasValue && addon.Publish.Value)
                                && filtervalues.Contains(attribute.AttributeValue)
                                select new
                                {
                                    AddOnItemID = addon.AddOnItemID,
                                    AttributeName = attribute.AttributeName,
                                    AttributeValue = attribute.AttributeValue
                                }
                             );
                    if (temp.Any())
                    {
                        //checking name and value are matched
                        //after filtered by attribute, product's attributes count same as filter count will be returned
                        rlt = (from t in temp.ToList()
                               from f in filter
                               where t.AttributeName == f.Key
                               && t.AttributeValue == f.Value
                               group t by t.AddOnItemID into g
                               where g.Count() == filter.Count()
                               select g.Key.ToString()).ToList();
                    }
                }

            }
            return rlt;
        
        }

        public object  getIDKAddons(string storeid, string productid)
        {
            Dictionary<string, int> displayingAttributes = new Dictionary<string, int>();
            displayingAttributes.Add("Interface", 0);
            displayingAttributes.Add("Size", 1);
            displayingAttributes.Add("Touch Screen", 2);
            displayingAttributes.Add("Original Brightness", 3);


            var tmp = (from addon in context.PeripheralAddOns.Include("PeripheralAddOnBundleItems.Part")
                       let attribute = (from attribute in context.IDKAttributes
                                        from item in context.PeripheralAddOnBundleItems
                                        where item.StoreID == addon.StoreID
                                         && item.AddOnItemID == addon.AddOnItemID
                                         && item.StoreID == storeid
                                         && item.SProductID == attribute.SProductID
                                          && displayingAttributes.Keys.Contains(attribute.AttributeName)
                                        select new
                                        {
                                            attribute.AttributeName,
                                            attribute.AttributeValue
                                        }
                                          ).Union(
                                          from attribute in context.IDKAttributes
                                          where addon.AddOnProductID == attribute.SProductID
                                    && displayingAttributes.Keys.Contains(attribute.AttributeName)
                                          select new
                                          {
                                              attribute.AttributeName,
                                              attribute.AttributeValue
                                          }
                                          )
                                     
                       where addon.SProductID == productid && addon.StoreID == storeid
                       select new
                       {
                           addon,
                           attribute
                       });
            
            if (tmp.Any())
            {
                var t2 = tmp.ToList();
                List<string> itemids = new List<string>();
            
                foreach (var t in t2)
                {
                    itemids.AddRange(t.addon.PeripheralAddOnBundleItems.Select(x => x.SProductID).ToList());
                   
                }
                System.Text.RegularExpressions.Regex rgObj = new System.Text.RegularExpressions.Regex(@"[-+±]?(?:\b[0-9]+(?:\.[0-9]*)?|\.[0-9]+\b)(?:[eE][-+]?[0-9]+\b)?");
             
                prefetchPartList(storeid, itemids.Distinct().ToList());

                var rlt = (from t in t2
                           where t.addon.addOnProduct != null && t.addon.isOrderEable()
                           && t.addon.addOnProduct.getListingPrice().value > 0
                           select new
                           {
                               Name = t.addon.addOnProduct.name,
                               Id = t.addon.AddOnItemID,
                               Desc = t.addon.addOnProduct.productDescX,
                               Price = t.addon.addOnProduct.getListingPrice(),
                               Warrantyprice = t.addon.addOnProduct is Product_Bundle ? ((Product_Bundle)t.addon.addOnProduct).bundle.getWarrantableTotal() :
                             (((Product)t.addon.addOnProduct).isWarrantable() ? t.addon.addOnProduct.getListingPrice().value : 0),
                               Detail = t.addon.addOnProduct is Product_Bundle ? (from d in (t.addon.addOnProduct as Product_Bundle).bundle.BundleItems
                                                                                  select new
                                                                                  {
                                                                                      Name = d.ItemSProductID,
                                                                                      Desc = d.ItemSProductID.StartsWith("96CB") ? d.ItemDescription.Replace("MONITOR + LCD KIT, ", "") : d.ItemDescription
                                                                                  }) : null
                               ,
                              t.attribute 
                           }).ToList();

                Dictionary<string, List<string>> attrs = new Dictionary<string, List<string>>();
                foreach (var t in rlt)
                {
                    foreach (var a in t.attribute)
                    {
                        if (attrs.ContainsKey(a.AttributeName))
                        {
                            if (attrs[a.AttributeName].Contains(a.AttributeValue) == false)
                                attrs[a.AttributeName].Add(a.AttributeValue);
                        }
                        else
                        {
                            List<string> al = new List<string>();
                            al.Add(a.AttributeValue);
                            attrs.Add(a.AttributeName, al);
                        }
                    }
                }

                Dictionary<string, List<string>> attrsTemp = new Dictionary<string, List<string>>();
                foreach (var li in attrs)
                {
                    var cc = li.Value.OrderBy(c => rgObj.IsMatch(c) ?
                                                        Convert.ToDouble(rgObj.Match(System.Text.RegularExpressions.Regex.Replace(c, @"^[-+±\.]+", "")).Value) : -1);
                    attrsTemp.Add(li.Key, cc.ToList());
                }
                attrs = attrsTemp;

                var rlt2 = new { Attibutes=attrs.OrderBy(x=>displayingAttributes[x.Key]),Addons=rlt };
                return rlt2;
            }
            else
                return null;
        }

        public object getIDKCompatibilityEmbeddedBoard(string storeid, string prodcutid)
        {
            var tmp = (from product in context.Parts.OfType<Product>()
                       let Processor = (from attr in context.VProductMatrices
                                        where attr.ProductNo == product.SProductID
                                        && (attr.LocalAttributeName.Equals("Processor", StringComparison.OrdinalIgnoreCase)
                                        || attr.LocalAttributeName.Equals("CPU Type", StringComparison.OrdinalIgnoreCase))
                                        select attr.LocalValueName).FirstOrDefault()
                       from addon in context.PeripheralAddOns.Include("PeripheralAddOnBundleItems").Include("PeripheralAddOnBundleItems.Part").Include("Product")
                       from addonitem in context.PeripheralAddOnBundleItems
                       where product.StoreID == addon.StoreID
                       && product.SProductID == addon.SProductID
                       && addon.AddOnItemID == addonitem.AddOnItemID
                       && product.StoreID == storeid
                       && addonitem.SProductID == prodcutid
                       && addonitem.Sequence == 0
                       select new
                       {
                           product.SProductID,
                           category="",
                           addon,
                           Processor

                       });

            if (tmp.Any())
            {
                var tmp2 = tmp.ToList();
               

                Dictionary<string, List<string>> attrs = new Dictionary<string, List<string>>();

                var Processors = tmp2.Select(x => x.Processor).Where(x=>string.IsNullOrEmpty(x)==false).Distinct().OrderBy(x=>x).ToList();
                var Categories = (new ProductCategoryHelper()).getProductCategory("AUS_30003", storeid).childCategoriesX;
                attrs.Add("Processor", Processors);
                attrs.Add("Category", Categories.Select(x=>x.CategoryName).ToList());

                List<string> itemids = new List<string>();
                foreach (var t in tmp2)
                {
             
 
                        itemids.Add(t.SProductID);
                        itemids.AddRange(t.addon.PeripheralAddOnBundleItems.Select(x => x.SProductID).ToList());
 
                }
                prefetchPartList(storeid, itemids.Distinct().ToList());

                var embeddedBoards = (from p in tmp2
                                      where p.addon.addOnReversionProduct != null
                                      && p.addon.addOnReversionProduct.isOrderable()
                                      && p.addon.addOnReversionProduct.getListingPrice().value > 0
                                      && Categories.Any(x => x.productList.Any(pl => pl.SProductID == p.SProductID))
                                      && p.Processor != null
                                      select new
                                      {
                                          Name = p.addon.addOnReversionProduct.name,
                                          Id = p.addon.AddOnItemID,
                                          Desc = p.addon.addOnReversionProduct.productDescX,
                                          Price = p.addon.addOnReversionProduct.getListingPrice(),
                                          Warrantyprice = p.addon.addOnReversionProduct.isWarrantable() ? p.addon.addOnReversionProduct.getListingPrice().value : 0,
                                          Detail = (from d in (p.addon.addOnReversionProduct as Product_Bundle).bundle.BundleItems
                                                    select new
                                                    {
                                                        Name = d.ItemSProductID,
                                                        Desc = d.ItemSProductID.StartsWith("96CB") ? d.ItemDescription.Replace("MONITOR + LCD KIT, ", "") : d.ItemDescription
                                                    })
                               ,
                                          attribute = string.Format("Processor={0}|Category={1}",
                                          p.Processor,
                                        Categories.First(x => x.productList.Any(pl => pl.SProductID == p.SProductID)).CategoryName).Split('|').Select(x => new
                                          {
                                              AttributeName = x.Split('=')[0],
                                              AttributeValue = x.Split('=')[1]
                                          })
                                      }
                                        ).Where(x=>x.Detail.Count()==3).ToList();

                var rlt2 = new { Attibutes = attrs.OrderBy(x=>x.Key).Select(x=>new{Key=x.Key,Value=x.Value }), Addons = embeddedBoards };
                return rlt2;

            }
            else

                return null;



        }

        /// <summary>
        /// return valid Product status
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        protected string[] getValidProductStatus(string storeid, Boolean includeSolutionOnly = false)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeid);
            return getValidProductStatus(store, includeSolutionOnly);
        }

        protected String[] getValidProductStatus(Store store, Boolean includeSolutionOnly = false)
        {
            string validproductstatus = store.getStringSetting("ValidProductStatus");
            if (includeSolutionOnly)
                validproductstatus = String.Format("{0},{1}", validproductstatus, Product.PRODUCTSTATUS.SOLUTION_ONLY.ToString());
            string[] validarray = new string[10];
            if (string.IsNullOrEmpty(validproductstatus) == false)
            {
                validarray = validproductstatus.Split(',');
            }

            return validarray;
        }


        /// <summary>
        /// Return valid SAP status
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        protected string[] getValidSAPStatus(string storeid)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeid);
            return getValidSAPStatus(store);
        }

        protected String[] getValidSAPStatus(Store store)
        {
            return store.getOrderablePartStates();
        }

        //获取没有被category使用的产品
        public List<Product> getProductNotInCategory(Store store)
        {
            string[] validarray = getValidProductStatus(store);
            string[] sapvalidarray = getValidSAPStatus(store);
            List<Product> productList = new List<Product>();
            productList = (from p in context.Parts.OfType<Product>()
                           where p.StoreID == store.StoreID && validarray.Contains(p.Status)
                           && sapvalidarray.Contains(p.StockStatus) && p.PublishStatus == true
                           && !(from pcm in context.ProductCategroyMappings
                                where pcm.StoreID == store.StoreID
                                select pcm.SProductID
                            ).Contains(p.SProductID)
                           select p).ToList();
            return productList;
        }

        /// <summary>
        /// get hot sale product by storeid 
        /// </summary>
        /// <param name="store">storeid </param>
        /// <param name="interval">count</param>
        /// <returns></returns>
        public List<Product> getHotDeals(string storeid, int interval , int count = 3)
        {
            List<Product> ls = new List<Product>();
            try
            {
                var cc = context.sp_GetHotDeals(storeid, interval).Take(count).ToList();
                if (cc.Any())
                    ls = getProductsBySproductIDList(cc.Select(c => c.SProductID).ToArray(), storeid);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error(ex.Message);
            }
            return ls;
        }

        public List<Product> getProductsBySolutionId(int solutionId, string storeid = null)
        {
            List<Product> products = (from p in context.Parts.OfType<Product>()
                                     from s in context.SolutionsAssosociateItems
                                     where s.SolutionId == solutionId && p.SProductID == s.SProductID
                                     select p).ToList();
            return products;
        }


        public Product getProductById(string productId)
        {
            Product product = context.Parts.OfType<Product>().FirstOrDefault(p => p.SProductID == productId);
            return product;
        }

        public List<spSearchCtosWithDefaultPartNo_Result> SearchCtosWithDefaultPartNo(string sourcestoreID, string sourceSystemNo, string targetstoreid)
        {
            return context.spSearchCtosWithDefaultPartNo(sourcestoreID, sourceSystemNo, targetstoreid).ToList();
        }
        #endregion

        #region Creat Update Delete
        public int save(Part _part)
        {
            //this block is for unit test purpose only
            if (_part.dummy)
            {
                //dummy product shall not be stored to DB, the helper class shall only cache it for ongoing testing purpose
                CachePool cache = CachePool.getInstance();
                if (cache.getPart(_part.StoreID, _part.SProductID) == null)
                    cache.cachePart(_part);
                return 0;
            }

            //if parameter is null or validation is false, then return  -1 
            if (_part == null || _part.validate() == false) return 1;

            //check if part has already existed
            Part _exist_part = getPart(_part.SProductID, _part.StoreID, true);
            
            try
            {
                if (_exist_part == null)  //part doesn't not exist
                {
                    context.Parts.AddObject(_part);
                    context.SaveChanges();
                    _part.parthelper = this;
                    return 0;
                }
                else
                {
                    if (!(_exist_part is Product) && (_part is Product))  // If the existing part is eStore part-only and is not eStore product, use sp to add product only
                    {
                        Product p = (Product)_part;

                        var ret = context.InsertProductOnly(p.StoreID, p.SProductID, p.DisplayPartno, p.ShowPrice, p.PublishStatus, p.StorePrice, p.PriceSource,
                            p.Status, p.ProductDesc, p.ProductFeatures, p.ExtendedDesc, p.ImageURL, p.PromotePrice, p.PromoteStart, p.PromoteEnd,
                            p.PromoteMessage, p.ClearanceThreshold, p.PromoteMarkup, p.EffectiveDate, p.ExpiredDate, p.Keywords, p.ProductGroup,p.pageTitle,p.PageDescription);
                        context.SaveChanges();

                        //drop cached part since it's now become eStore product
                        CachePool.getInstance().releaseStoreCacheProduct(_part.StoreID, _part.SProductID);
                    }
                    else
                    {
                        if (_part.parthelper !=null && _part.parthelper.context != null)
                            context = _part.parthelper.context;

                        context.Parts.ApplyCurrentValues(_part);
                        context.SaveChanges();
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(Part _part)
        {
            if (_part == null || _part.validate() == false) return 1;
            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    context.DeleteObject(_part);
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

        /// <summary>
        /// Delete replaceProduct records
        /// </summary>
        /// <param name="_part"></param>
        /// <returns></returns>
        public int delete(ReplaceProduct _replaceproduct)
        {
            if (_replaceproduct == null || _replaceproduct.validate() == false) return 1;
            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    ReplaceProduct rep = (from rp in context.ReplaceProducts
                                          where rp.ID == _replaceproduct.ID
                                          select rp).FirstOrDefault();

                    context.DeleteObject(rep);
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

        /// <summary>
        /// Delete ProductBundleItem
        /// </summary>
        /// <param name="_productBundle"></param>
        /// <returns></returns>
        public int delete(ProductBundleItem _productBundle)
        {
            if (_productBundle == null || _productBundle.validate() == false) return 1;
            try
            {
                using (eStore3Entities6 context = new eStore3Entities6())
                {
                    ProductBundleItem bundle = (from b in context.ProductBundleItems
                                          where b.ProductBundleItemID == _productBundle.ProductBundleItemID
                                          select b).FirstOrDefault();

                    context.DeleteObject(bundle);
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
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(PartHelper).ToString();
        }
        #endregion

#region OM Only

        /// <summary>
        /// OM only method!  This method will return keyword matched products in specified store regardless what status they are. 
        /// The search will conduct on basis of PartNo and DisplayName match
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        public List<Product> getMatchedProducts(Store store, String keyword)
        {
            keyword = keyword.ToUpper();
            List<Product> products = (from p in context.Parts.OfType<Product>()
                                                        where p.StoreID == store.StoreID && !p.SProductID.EndsWith("BTO")  && 
                                                                    (p.SProductID.ToUpper().Contains(keyword) || p.DisplayPartno.ToUpper().Contains(keyword) || p.ModelNo.ToUpper().Contains(keyword))
                                                        select p).ToList();
                            
            return products;
        }

        /// <summary>
        /// OM used.  Using to search all products in given store
        /// 现在能查product 和ctos,添加参数 看是否只查product
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public Dictionary<string, string> getProductsHint(string storeid,bool onlyProduct = true)
        {

            Dictionary<string, string> products = new Dictionary<string, string>();

            string[] validarray = getValidProductStatus(storeid);
            string[] sapvalidarray = getValidSAPStatus(storeid);
            string toBeReviewStatu = Product.PRODUCTSTATUS.TOBEREVIEW.ToString();

            var _products = from p in context.Parts.OfType<Product>()
                            where p.StoreID == storeid && (onlyProduct ? !(p is Product_Ctos) : true)
                            && p.Status != toBeReviewStatu
                            //&& validarray.Contains(p.Status) && sapvalidarray.Contains(p.StockStatus)
                            select new { p.SProductID, p.DisplayPartno };

            if (_products != null)
            {
                foreach (var p in _products.ToList())
                    products.Add(p.SProductID, p.DisplayPartno);
            }

            return products;
        }


        /// <summary>
        /// OM used.  Using to search all CTOS products in given store
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public Dictionary<string, string> getCTOSProductsHint(string storeid, Boolean includeSolutionOnly = false)
        {
            Dictionary<string, string> products = new Dictionary<string, string>();
            string[] validarray = getValidProductStatus(storeid, includeSolutionOnly);
            string toBeReviewStatu = Product.PRODUCTSTATUS.TOBEREVIEW.ToString();

            var _products = from p in context.Parts.OfType<Product_Ctos>()
                            where p.StoreID == storeid && validarray.Contains(p.Status) && p.Status != toBeReviewStatu 
                            select new { p.SProductID, p.DisplayPartno };

            if (_products != null)
            {
                foreach (var p in _products.ToList())
                    products.Add(p.SProductID, p.DisplayPartno);
            }

            return products;
        }

        //根据 sproductId 查找 ctos list
        public List<Product_Ctos> getMatchedCTOSProducts(string storeid, string keyword, Boolean includeSolutionOnly=false)
        {
            List<Product_Ctos> _products = new List<Product_Ctos>();
            string[] validarray = getValidProductStatus(storeid, includeSolutionOnly);

            _products = (from p in context.Parts.OfType<Product_Ctos>()
                            where p.StoreID == storeid && p.PublishStatus == true && validarray.Contains(p.Status) 
                            && (p.SProductID.Contains(keyword) || p.DisplayPartno.Contains(keyword))
                            select p).ToList();

            foreach (Product_Ctos ctos in _products)
                ctos.parthelper = this;
            return _products;
        }

        //获取part和product
        public List<Part> getMatchStorePartAndProducts(Store store, string keyword)
        {
            //string[] sapvalidarray = getValidSAPStatus(store.StoreID);
            //&& sapvalidarray.Contains(p.StockStatus) 
            List<Part> _products = (from p in context.Parts
                                    where p.StoreID == store.StoreID && (p.SProductID.Contains(keyword) || p.VendorProductName.Contains(keyword))
                                    && !(p is Product_Ctos)
                                    select p).ToList();

            if (_products == null)
                _products = new List<Part>();

            return _products;
        }

        /// <summary>
        /// OM Return Hint for Partno in SAP but not exist in Products.
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSAPPartsNonEstoreHint(Store store)
        {

            string orgid = store.Settings["ProductLogisticsOrg"];
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //PRODUCT_TYPE = ZCTO,  这样的产品是虚拟料号
            var _parts = from p in context.SAPProducts
                         where p.ORG_ID == orgid && !p.PART_NO.EndsWith("BTO") && !p.PART_NO.EndsWith("-ES") && !p.PART_NO.StartsWith("AGS") && !p.PART_NO.StartsWith("96SW")
                         && p.PRODUCT_TYPE != "ZCTO"
                         && !(from prods in context.Parts.OfType<Product>() where prods.StoreID == store.StoreID select prods.SProductID).Contains(p.PART_NO)
                         select new { PART_NO = p.PART_NO, PRODUCT_DESC = p.PRODUCT_DESC };

            foreach (var x in _parts.ToList())
            {

                dic.Add(x.PART_NO, x.PRODUCT_DESC);
            }

            return dic;
        }
        //获取part和product
        public Dictionary<string, string> getPartAndProductHint(string storeid, string keyword)
        {

            Dictionary<string, string> products = new Dictionary<string, string>();

            string[] sapvalidarray = getValidSAPStatus(storeid);
            //.OfType<Product>()
            var _products = from p in context.Parts
                            where p.StoreID == storeid && (p.SProductID.Contains(keyword) || p.VendorProductName.Contains(keyword))
                            && sapvalidarray.Contains(p.StockStatus) && !(p is Product_Ctos)
                            select new { p.SProductID, p.VendorProductName };

            if (_products != null)
            {
                foreach (var p in _products.ToList())
                {
                    if (!products.ContainsKey(p.SProductID))
                        products.Add(p.SProductID, p.VendorProductName);
                }
            }

            return products;
        }

        /// <summary>
        /// 获取所有的sapproduct partno
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Dictionary<string, string> getSAPProductHint(Store store, string keyword)
        {
            Dictionary<string, string> products = new Dictionary<string, string>();
            string orgid = store.Settings["ProductLogisticsOrg"];
            var parts = (from p in context.SAPProducts
                         where p.ORG_ID == orgid && p.PART_NO.StartsWith(keyword)
                         select new { p.PART_NO,p.MODEL_NO }).ToList();
            if (parts != null && parts.Any())
            {
                foreach (var p in parts)
                {
                    if (!products.ContainsKey(p.PART_NO))
                        products.Add(p.PART_NO, p.MODEL_NO);
                }
            }
            return products;
        }


        /// <summary>
        /// For OM, return CTOS that using this component
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public List<Product_Ctos> getCTOSByComponentID(int id)
        {
            var _ctos = (from p in context.Parts.OfType<Product_Ctos>()
                         from c in context.CTOSBOMs
                         where p.SProductID == c.SProductID && c.ComponentID == id
                         select p);

            if (_ctos != null)
            {
                foreach (Product_Ctos cc in _ctos)
                    cc.parthelper = this;                    //cc._helper = this;
                return _ctos.ToList();
            }
            else
            {
                return new List<Product_Ctos>();
            }

        }

        /// <summary>
        /// This for OM use, not apply CTOS specmask, return all.
        /// </summary>
        /// <param name="sproductid"></param>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private List<VProductMatrix> getOMCTOSProductSpec(string sproductid, string storeid)
        {
            var _specs = from sp in context.OMCTOSSpecs
                         where sp.SproductID == sproductid && sp.StoreID == storeid
                         orderby sp.Sequence
                         select sp;

            List<VProductMatrix> _ctosspec = new List<VProductMatrix>();

            foreach (OMCTOSSpec cts in _specs.ToList())
            {
                VProductMatrix vpm = new VProductMatrix();

                vpm.ProductNo = cts.SproductID;
                vpm.LocalAttributeName = cts.AttrName;
                vpm.LocalValueName = cts.AttrValue_name;
                vpm.AttrValueID = cts.AttrValueID;
                vpm.AttrID = cts.AttrID;
                vpm.ID = cts.ID;
                vpm.seq = cts.Sequence;
                _ctosspec.Add(vpm);

            }

            return _ctosspec;
        }

        /// <summary>
        /// Edward, Export CBOMS
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="includeall"></param>
        /// <returns></returns>

        public List<ExportCBOM_Result> exportCBOM(string storeid, bool includeall=false) {
            try
            {
                List<ExportCBOM_Result> _cboms = context.spExportCBOM(storeid, includeall).ToList();
                return _cboms;

            }catch(Exception ex){
                eStoreLoger.Fatal(ex.Message, "exportCBOM", "", storeid, ex);
                return new List<ExportCBOM_Result>();
            }
        }

        /// <summary>
        /// Edward Replace component, the sproducts parameter is not supported now.
        /// </summary>
        /// <param name="oldcomponentid"></param>
        /// <param name="newcomponentid"></param>
        /// <param name="storeid"></param>
        /// <param name="sproductids"></param>
        /// <param name="isdelete"> if no new component, this should be true</param>

        public void replaceComponent(int oldcomponentid, int newcomponentid,string storeid, string sproductids , bool isdelete) {

            try
            {
                object result = context.spReplacePartno(oldcomponentid, newcomponentid, storeid, sproductids, isdelete);
                if (context != null)
                    context.Dispose();
            }
            catch (Exception ex) {
                eStoreLoger.Fatal(ex.Message, "replaceComponent", "", storeid, ex);
            } 
        }

        /// <summary>
        /// This method returns only active parts per input store
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<Part> getActiveParts(Store store, String partPrefix = null)
        {
            string[] sapvalidarray = getValidSAPStatus(store);
            string storeOrgId = store.Settings["ProductLogisticsOrg"];

            List<Part> activeParts = null;
            if (String.IsNullOrWhiteSpace(partPrefix))
            {
                activeParts = (from part in context.Parts
                               from sapPart in context.SAPProducts
                               where part.StoreID == store.StoreID
                                  && sapPart.ORG_ID == storeOrgId
                                  && part.SProductID == sapPart.PART_NO
                                   && (sapvalidarray.Contains(part.StockStatus) || sapvalidarray.Contains(sapPart.STATUS))
                                     && !(part is Product_Ctos) && !(part is Product_Bundle)
                               select part).ToList();
            }
            else
            {
                activeParts = (from part in context.Parts
                               from sapPart in context.SAPProducts
                               where part.StoreID == store.StoreID
                                  && sapPart.ORG_ID == storeOrgId
                                  && part.SProductID.StartsWith(partPrefix) 
                                  && part.SProductID == sapPart.PART_NO
                                   && (sapvalidarray.Contains(part.StockStatus) || sapvalidarray.Contains(sapPart.STATUS))
                                     && !(part is Product_Ctos) && !(part is Product_Bundle)
                               select part).ToList();
            }

            //keep context with each acquired part
            foreach (Part part in activeParts)
                part.parthelper = this;

            return activeParts;
        }


        public List<Part> getEDIExportingParts(Store store)
        {
            string[] sapvalidarray = getValidSAPStatus(store);
            string storeOrgId = store.Settings["ProductLogisticsOrg"];

            List<Part> products = new List<Part>();

            var categoriesandspecs = (from part in context.Parts
                                      from sapPart in context.SAPProducts
                                      let specs = context.VProductMatrices.Where(x => x.ProductNo.Equals(part.SProductID, StringComparison.OrdinalIgnoreCase))
                                      let categories = (from p in context.ProductCategroyMappings
                                                        from pc in context.ProductCategories
                                                        where p.SProductID == part.SProductID
                                                        && p.StoreID == part.StoreID
                                                        && p.CategoryID == pc.CategoryID
                                                        && p.StoreID == pc.Storeid
                                                        && pc.Publish == true
                                                        select pc).Distinct()
                                      where part.StoreID == store.StoreID
                                         && sapPart.ORG_ID == storeOrgId
                                         && part.SProductID == sapPart.PART_NO
                                         && (sapvalidarray.Contains(part.StockStatus) || sapvalidarray.Contains(sapPart.STATUS))
                                         && !(part is Product_Ctos) && !(part is Product_Bundle)
                                         && part is Product

                                      select new
                                      {
                                          part.SProductID,
                                          categories,
                                          specs,
                                      }
                           ).ToList();

            var parts = (from part in context.Parts.OfType<POCOS.Product>().Include("ProductResources").Include("RelatedProducts")
                         where part.StoreID == store.StoreID
                             && (sapvalidarray.Contains(part.StockStatus))
                               && !(part is Product_Ctos) && !(part is Product_Bundle)
                         select part).ToList();

            Dictionary<POCOS.Part, int> checkatplist = new Dictionary<Part, int>();
            var activeParts = from p in parts
                              from pr in categoriesandspecs
                              where p.SProductID == pr.SProductID
                              select new
                              {
                                  part = p,
                                  categories = pr.categories,
                                  specs = pr.specs,
                              };

            foreach (var data in activeParts)
            {
                if (data.part is POCOS.Product)
                {
                    POCOS.Product product = data.part as POCOS.Product;
                    product.specs = data.specs.ToList();
                    product.productCategories = data.categories.ToList();
                    products.Add(product);
                    checkatplist.Add(product, 1);
                }
            }
            int skip = 0;
            do
            {
                setATPs(store, checkatplist.Skip(skip).Take(200).ToDictionary(x=>x.Key,y=>y.Value));
                skip += 200;
            } while (skip < checkatplist.Count);

            return products;
        }

        public List<Part> getEDIExportingParts(Store store, List<string> partids)
        {
            string[] sapvalidarray = getValidSAPStatus(store);
            string storeOrgId = store.Settings["ProductLogisticsOrg"];

            List<Part> products = new List<Part>();

            var categoriesandspecs = (from part in context.Parts
                                      from sapPart in context.SAPProducts
                                      let specs = context.VProductMatrices.Where(x => x.ProductNo.Equals(part.SProductID, StringComparison.OrdinalIgnoreCase))
                                      let categories = (from p in context.ProductCategroyMappings
                                                        from pc in context.ProductCategories
                                                        where p.SProductID == part.SProductID
                                                        && p.StoreID == part.StoreID
                                                        && p.CategoryID == pc.CategoryID
                                                        && p.StoreID == pc.Storeid
                                                        && pc.Publish == true
                                                        select pc).Distinct()
                                      where part.StoreID == store.StoreID
                                         && sapPart.ORG_ID == storeOrgId
                                         && part.SProductID == sapPart.PART_NO
                                         && (sapvalidarray.Contains(part.StockStatus) || sapvalidarray.Contains(sapPart.STATUS))
                                       && partids.Contains(part.SProductID)

                                      select new
                                      {
                                          part.SProductID,
                                          categories,
                                          specs,
                                      }
                           ).ToList();

            var parts = (from part in context.Parts.Include("ProductResources").Include("RelatedProducts")
                         where part.StoreID == store.StoreID
                             && (sapvalidarray.Contains(part.StockStatus))
                           && partids.Contains(part.SProductID)
                         select part).ToList();

            Dictionary<POCOS.Part, int> checkatplist = new Dictionary<Part, int>();
            var activeParts = from p in parts
                              from pr in categoriesandspecs
                              where p.SProductID == pr.SProductID
                              select new
                              {
                                  part = p,
                                  categories = pr.categories,
                                  specs = pr.specs,
                              };

            foreach (var data in activeParts)
            {
                POCOS.Part product = data.part as POCOS.Part;
                product.specs = data.specs.ToList();
                if (data.part is POCOS.Product)
                {
                    (product as POCOS.Product).productCategories = data.categories.ToList();
                }
                products.Add(product);
                if (product.StockStatus.ToUpper() == "O")
                {
                    checkatplist.Add(product, 1);
                }
            }
            setATPs(store, checkatplist);

            return products;
        }
        /// <summary>
        /// 删除Product 产品
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="sporductId"></param>
        /// <returns></returns>
        public Boolean convertProductToPart(string storeId, string sproductId)
        {
            try
            {
                List<sp_ConverProductToPart_Result> result = (from p in context.sp_ConverProductToPart(storeId, sproductId) select p).ToList();

                if (result == null || result.Count == 0)
                    return true;
                return result.FirstOrDefault().RecordCount > 0;
            }
            catch (Exception)
            {
                return true;
            }           
        }

        //根据Model 返回 对应的随机产品
        public Product getProductByModelNo(List<string> modelList, Store store)
        {
            string[] validarray = getValidProductStatus(store);
            validarray = validarray.Where(p => p != "PHASED_OUT" && p != "COMING_SOON").ToArray();
            string[] sapvalidarray = getValidSAPStatus(store.StoreID);

            List<Product> products = (from p in context.Parts.OfType<Product>() 
                            where p.StoreID == store.StoreID 
                              && p.PublishStatus == true 
                              && p.ShowPrice == true 
                              && validarray.Contains(p.Status) 
                              && sapvalidarray.Contains(p.StockStatus) 
                              && modelList.Contains(p.ModelNo) 
                              && !(p is Product_Ctos) && !(p is Product_Bundle) 
                        select p).ToList<Product>();

            products = products.Where(p => p.getNetPrice().value > 0 && p.isOrderable()).ToList();
            if (products != null && products.Count > 0)
            {
                Random random = new Random();
                int productIndex = random.Next(products.Count);
                Product popularProduct = products[productIndex];
                return popularProduct;
            }
            return null;
        }

        public List<Product> getProductListByModelNo(List<string> modelList, Store store)
        {
            string[] validarray = getValidProductStatus(store);
            validarray = validarray.Where(p => p != "PHASED_OUT" && p != "COMING_SOON").ToArray();
            string[] sapvalidarray = getValidSAPStatus(store.StoreID);

            List<Product> products = (from p in context.Parts.OfType<Product>()
                                      where p.StoreID == store.StoreID
                                        && p.PublishStatus == true
                                        && p.ShowPrice == true
                                        && validarray.Contains(p.Status)
                                        && sapvalidarray.Contains(p.StockStatus)
                                        && modelList.Contains(p.ModelNo)
                                        && !(p is Product_Ctos) && !(p is Product_Bundle)
                                      select p).ToList<Product>();

            products = products.Where(p => p.getNetPrice().value > 0 && p.isOrderable()).ToList();
            return products;
        }

        public List<string> getStoreProductAllModel(Store store, MiniSite minisite = null)
        {
            string[] validarray = getValidProductStatus(store);
            validarray = validarray.Where(p => p != "PHASED_OUT" && p != "COMING_SOON").ToArray();
            string[] sapvalidarray = getValidSAPStatus(store.StoreID);
            if (minisite != null)
            {
                List<string> models = (from p in context.Parts.OfType<Product>()
                                          from pc in context.ProductCategories
                                          from pcm in context.ProductCategroyMappings
                                          where p.StoreID == store.StoreID
                                            && p.PublishStatus == true
                                            && p.ShowPrice == true
                                            && validarray.Contains(p.Status)
                                            && sapvalidarray.Contains(p.StockStatus)
                                            && !(p is Product_Ctos) && !(p is Product_Bundle)
                                            && pc.Storeid == store.StoreID
                                            && pc.MiniSiteID == minisite.ID
                                            && pc.Publish == true
                                            && pcm.CategoryID == pc.CategoryID
                                            && pcm.StoreID == pc.Storeid
                                            && pcm.SProductID == p.SProductID
                                          select p.ModelNo).Distinct().ToList();
                return models;                
            }
            // if need will add minisite != null
            
            return new List<string>();
        }
#endregion
    }
}