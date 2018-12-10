using System.Collections.Generic;

using System.Collections;
using System.Net;
using System.Data;
using System;
using eStore.POCOS;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;
using esUtilities;
using System.Runtime.Serialization;
using System.IO;

namespace eStore.POCOS.Sync
{
    public class PISSync
    {

        eStore3Entities6 context = new eStore3Entities6();
        PISEntities pcontext = null;
        List<sp_GetLiteratureTable_Result> _literatures;
        List<sp_GetLiteratureTableByLANG_Result> _literatureslang;
        private const string catalogid1="1-2MLAX2";
        private const string catalogid2="1-2JKBQD";
        private const string catalogid_IOT = "b04d9404-a69e-48ff-bdf2-a60d9d2b0af9";
        private const string catalogid_Ushop = "1aa27c93-28f9-4a9b-936d-9351c93e6ee1";
        private const string catelogid_BB = "736d1add-de2e-47ef-a7c4-00625ad8d53f";
        AdvantechPIS.PISProductWS pisws;
        AdvantechPAPS.PAPSService oPAPSWebservice ;
        private string oldfeature;
        private string olddesc;        
        PISEntities pis;
        List<string> _bbPartNo = null;
        private string pisADOConnectionString;

        public PISSync()
        {
            pcontext = new PISEntities();
            pisws = new AdvantechPIS.PISProductWS();
            oPAPSWebservice = new AdvantechPAPS.PAPSService();
            pis = new PISEntities();
        }

        //destructor
        ~PISSync()
        {
            if (pcontext != null)
                pcontext.Dispose();
            if (pisws != null)
                pisws.Dispose();
            if (oPAPSWebservice != null)
                oPAPSWebservice.Dispose();
            if (pis != null)
                pis.Dispose();
        }

        #region Certificates
        public List<Certificates> GetCertificateByModelID(Part _part)
        {
            List<Certificates> certificate = new List<Certificates>();
            try
            {
                //string c1 = catalogid1;
                //string c2 = catalogid2;
                //if (isBBPartNo(productName) == true)
                //{
                //    c1 = catelogid_BB;
                //    c2 = string.Empty;
                //}
                //查出categoryID
                //var categoryid = (from p in pis.spGetModelByPN_estore_NEW(c1, c2, productName)
                //                 select p.CATEGORY_ID).FirstOrDefault();
                //ICC Try IOT & U-shop catalog
                //if(categoryid == null)
                //    categoryid = (from p in pis.spGetModelByPN_estore_NEW(catalogid_IOT, catalogid_Ushop, productName)
                //                  select p.CATEGORY_ID).FirstOrDefault();

                //ICC Try B+B catalog
                //if (categoryid == null)
                //    categoryid = (from p in pis.spGetModelByPN_estore_NEW(catelogid_BB, catalogid2, productName)
                //                  select p.CATEGORY_ID).FirstOrDefault();

                //Use interface
                VPisInfo ps = new VPisInfo(_part, pis);
                var categoryid = ps.getCategoryID(_part.SProductID);

                if (!string.IsNullOrEmpty(categoryid))
                {
                    //查出certificate list
                    var certificateList = (from p in pis.sp_GetCertificateByModelID(categoryid)
                                           select p).ToList();
                    if (certificateList != null)
                    {
                        foreach (var c in certificateList)
                        {
                            Certificates ct = new Certificates();
                            ct.CertificateImagePath = c.certpicpath.Replace("http://downloadt.advantech.com", "https://downloadssl.advantech.com"); ;
                            ct.CertificateName = c.certname;
                            certificate.Add(ct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return certificate;
        }
        #endregion

        public void SyncPeriphalMaster(Store store)
        {

            var _pp = from p in context.PeripheralProducts
                      //where p.partNo =="SQF-S25S8-64G-ETE"
                      select p;

            foreach (PeripheralProduct pp in _pp.ToList())
            {
                Part compatiblePart = constructRelatedPart(pp.SProductID, store, true);
                if (compatiblePart != null)
                    save(compatiblePart);


            }



        }

        /// <summary>
        /// Sync all Parts with PIS related products, save new part to Parts table.
        /// </summary>
        /// <param name="store"></param>
        public void syncStoreProductAccessories(Store store, string partprefix=null)
        {
            //List<Product> products = getStandardProducts(store, partprefix);
            List<Product> products = (new PartHelper()).getActiveStandardProducts(store, partprefix);

            foreach (Product p in products)
            {
                if ( !(p is Product_Ctos) && !(p is Product_Bundle))
                    syncProductAccessories(p, store);
            }
        }

        /// <summary>
        /// Sync all Parts with PAPS compatible products
        /// </summary>
        /// <param name="store"></param>
        //public void syncAllCompatibleProducts(Store store)
        //{
        //    string producttype = Product.PRODUCTTYPE.STANDARD.ToString();
        //    var _parts = from p in context.Parts
        //                 where p.StoreID == store.StoreID && p.ProductType.ToUpper() == producttype.ToUpper() && p.SProductID=="AIMB-762G2-00A1E"
        //                 select p;

        //    foreach (Part p in _parts.ToList())
        //    {
        //        SynCompatibleListPAPS(p, store);

        //    }

        //}

        /// <summary>
        /// This method is sync all active products in specified store
        /// </summary>
        /// <param name="store"></param>
        /// <param name="partprefix"></param>
        public String syncStoreActiveProducts(Store store, string partprefix = null)
        {
            StringBuilder errors = new StringBuilder();
            List<Product> products = (new PartHelper()).getActiveStandardProducts(store, partprefix);

            String error = null; 
            foreach (Product p in products)
            {
                if (!(p is Product_Ctos))
                {
                    error = syncStandardProduct(store, p, false);
                    if (String.IsNullOrWhiteSpace(error))
                        errors.Append(error);
                }
            }

            return errors.ToString();
        }

        public String syncStandardProduct(Store store, Product product, Boolean includePriceSync = false)
        {
            StringBuilder errors = new StringBuilder();
         
            //find out product model in PIS
            PISModel _model = getModel(product);

            //check if product (part) has been assigned to new model
            if (_model != null && !_model.modelName.Equals(product.ModelNo))
                product.ModelNo = _model.modelName;

            //sync product related parts
            String error = syncProductAccessories(product, store);
            if (!String.IsNullOrWhiteSpace(error))
                errors.Append(error);

            //Sync product from PIS or sync P-Trade part information from PAPS
            if (product.isPTradePart()) //P-Trade part 
                error = syncPAPSDetail(product);
            else  //Advantech product
                error = SyncPISDetail(product, store);
            if (!String.IsNullOrWhiteSpace(error))
                errors.Append(error);

            //sync price
            if (includePriceSync)
            {
                List<Part> syncParts = new List<Part>();
                syncParts.Add(product);

                error = syncPrice(store, syncParts);
                if (String.IsNullOrWhiteSpace(error))
                    errors.Append(error);
            }

            if (product.isUpdated)
                product.save();

            return errors.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="partprefix"></param>
        /// <param name="writeLog"></param>
        /// <returns></returns>
        public String syncAllPIS(Store store, string partprefix=null, Action<String> writeLog=null)
        {
            StringBuilder errorLog = new StringBuilder();
            List<Part> parts = (new PartHelper()).getActiveParts(store, partprefix);

            foreach (Part p in parts)
            {
                if (!(p is Product_Ctos))
                {
                    errorLog.Append(SyncPIS(p, store,false,true));
                    //if (p.isPTradePart())
                    //{
                    //    try
                    //    {
                    //        errorLog.Append(getPAPSDetail(p));
                    //        save(p, errorLog,true);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        errorLog.AppendLine(String.Format("Fails at getPAPSDetail of Part {0}", p.SProductID));
                    //        Console.WriteLine(p.SProductID + " PAPS error");
                    //    }
                    //}

                }

                //If write log delagation method is passed in, write the error through it.
                if (writeLog != null && errorLog.Length > 0)
                {
                    writeLog(errorLog.ToString());
                    errorLog.Clear();
                }
            }

            /* no longer needed
            //set HD part to "N" from "O" for AUS store
            if (store.StoreID == "AUS" || store.StoreID == "ASC")
            {
                var HDParts = (from p in context.Parts
                               where p.StoreID == store.StoreID
                                  && p.StockStatus == "O"
                                  && p.VendorProductLine.ToUpper() == "HDD"
                                  && p.LastUpdated > new DateTime(2011, 10, 20)
                               select p).ToList();
                foreach (Part p in HDParts)
                {
                    p.StockStatus = "N";
                }
                context.SaveChanges();
            }
             * */

            return errorLog.ToString();
        }

        public void SyncCTOSProducts(string storeid, string tostoreid)
        {
            //Loop all Parts in the given store
            //Call Storeprocedure in PIS to get product information
            AdvStoreEntities acontext = new AdvStoreEntities();

            var _allCTOSProducts = (from ctos in acontext.vCTOS
                                    where ctos.storeid.ToUpper() == storeid
                                    select ctos);

            foreach (var ct in _allCTOSProducts.ToList())
            {
                //set data from SAP
                //set data from PIS
                //Sync related product
                Product_Ctos ctos = new Product_Ctos();
                ctos.StoreID = tostoreid;
                ctos.SProductID = ct.cbomid.ToString();
                ctos.DisplayPartno = ct.Con_Item_Number;
                ctos.VendorProductName = ct.Con_Item_Number;

                ctos.ProductFeatures = ct.Con_item_features;
                ctos.ShortFeatures = ct.short_features;
                ctos.LongFeatures = ct.Con_item_features;

                ctos.ProductDesc = ct.Con_Item_Desc;
                ctos.VendorProductDesc = ct.Con_Item_Desc;
                ctos.VendorProductLine = ct.Con_Item_Line;
                ctos.VendorSuggestedPrice = ct.base_list_price;
                ctos.VProductID = ct.cbomid.ToString();
                ctos.VendorID = "Advantech" + ct.storeid;

                ctos.BTONo = ct.Con_Item_virtual_part;
                ctos.DimensionHeightCM = ct.Height_M;
                ctos.DimensionLengthCM = ct.Depth_M;
                ctos.DimensionWidthCM = ct.Width_M;

                ctos.StorePrice = ct.base_list_price.HasValue ? ct.base_list_price.Value : 0;
                ctos.SystemEstimateSum = ct.price_sum;
                ctos.CreatedBy = "Sync";

                ctos.PromotePrice = ct.pro_price;
                ctos.PromoteStart = ct.pro_startdate;
                ctos.PromoteEnd = ct.pro_enddate;
                ctos.promotionMarkUpPrice = ct.Promotemarkup.HasValue ? ct.Promotemarkup.Value : 0;
                ctos.ShowPrice = true;
                ctos.Status = getProductstatus(ct.state).ToString();
                ctos.PublishStatus = ctos.Status == "DELETED" ? false : true;
                ctos.LastUpdated = DateTime.Now;
                ctos.PriceSource = Product.PRICESOURCE.LOCAL.ToString();
                ctos.ProductType = Product.PRODUCTTYPE.CTOS.ToString();
                save(ctos);
            }
        }

        public Product constructProduct(Product _part, bool isrelatedpart, bool sync = false)
        {
            
            Product prod = _part;
            prod.StoreID = _part.StoreID;
            prod.SProductID = _part.SProductID;
            prod.DisplayPartno = string.IsNullOrEmpty(prod.DisplayPartno) ? _part.SProductID : prod.DisplayPartno;
            prod.ShowPrice = true;
            prod.PublishStatus = true;
            prod.StorePrice = _part.VendorSuggestedPrice.Value;
            prod.PriceSource = Part.PRICESOURCE.VENDOR.ToString();
            prod.Status = Product.PRODUCTSTATUS.GENERAL.ToString();
            prod.ProductDesc = string.IsNullOrEmpty( prod.ProductDesc)?_part.VendorProductDesc: prod.ProductDesc;
            prod.ProductFeatures = string.IsNullOrEmpty(prod.ProductFeatures) ? _part.VendorProductDesc : prod.ProductFeatures;
            prod.ExtendedDesc = _part.ExtendedDesc;
            prod.ImageURL = _part.TumbnailImageID;
            prod.PromotePrice = 0;
            prod.ExpiredDate = null;
            prod.Keywords = null;
            prod.VendorProductDesc = string.IsNullOrEmpty(prod.VendorProductDesc) ? prod.SProductID : prod.VendorProductDesc;
            return prod;
        }
        
        private Product.PRODUCTSTATUS getProductstatus(string status)
        {

            //SQ,new,none,Quickly,hot,clear,Inactive,Featured,Deleted,promote

            switch (status.Trim().ToUpper())
            {
                //case "CL":
                //case "CLEAR":
                //    return Product.PRODUCTSTATUS.CLEARANCE;

                //case "NEW":
                //case "NW":
                //    return Product.PRODUCTSTATUS.NEW;

                case "SQ":
                case "NONE":
                case "A":
                case "FEATURED":
                case "GENERAL":
                case "ACTIVE":
                case "":
                    return Product.PRODUCTSTATUS.GENERAL;

                //case "QUICKLY":
                //    return Product.PRODUCTSTATUS.EXPRESS;

                //case "HOT":
                //case "HT":
                //    return Product.PRODUCTSTATUS.HOT;

                case "INACTIVE":
                    return Product.PRODUCTSTATUS.INACTIVE;
                case "INACTIVE_AUTO":
                    return Product.PRODUCTSTATUS.INACTIVE_AUTO;

                case "DELETED":
                    return Product.PRODUCTSTATUS.DELETED;
                case "TOBEREVIEW":
                    return Product.PRODUCTSTATUS.TOBEREVIEW;

                //case "PROMOTE":
                //case "SALE":
                //case "PM":
                //    return Product.PRODUCTSTATUS.PROMOTION;
                default:
                    return Product.PRODUCTSTATUS.GENERAL;

            }


        }
        
        public Product constructPart(string partno, Store _store, bool isrelatedpart)
        {
            Part newPart = new Part();

            var _part = (from p in context.Parts
                         where p.SProductID == partno && p.StoreID == _store.StoreID
                         select p).FirstOrDefault();

            if (_part != null)
            {
                //cleanPart(_part);
                newPart = _part;
            }
            else
            {
                newPart.CreatedDate = DateTime.Now;
                newPart.CreatedBy = "Sync Job";
            }

            if (!(newPart is Product))
            {
                newPart.StoreID = _store.StoreID;
                if (string.IsNullOrEmpty(newPart.SProductID) == true)
                    newPart.SProductID = partno;

                newPart.VProductID = newPart.SProductID;
                newPart.VendorID = "Advantech" + _store.StoreID;
                newPart.LastUpdatedBy = "Sync Job";
                newPart.LastUpdated = DateTime.Now;
                newPart.priceSource = Part.PRICESOURCE.VENDOR;

                newPart.VendorSuggestedPrice = 0;
            }

            //getProductPriceFromSap(newPart.SProductID, _store);
            //getprice from SAP
            setLogistic(_store, newPart);
            List<Part> ls = new List<Part>();
            ls.Add(newPart);
            syncPrice(_store, ls);
            

            //if (newPart.isPTradePart())
            //    getPAPSDetail(newPart);

            if (string.IsNullOrEmpty(newPart.StockStatus) == false)
                SyncPIS(newPart, _store);


            Product newProduct = new Product(newPart);

            newProduct.Status = Product.PRODUCTSTATUS.GENERAL.ToString();
            newProduct.DisplayPartno = newPart.SProductID;
            newProduct.ProductDesc = newPart.VendorProductDesc ?? "";
            newProduct.ProductFeatures = newPart.VendorFeatures ?? "";
            newProduct.PriceSource = "VENDOR";

            return newProduct;

        }
        
        public Part constructRelatedPart(string partno, Store _store, bool isrelatedpart)
        {
            Part newPart = (from p in context.Parts
                           where p.SProductID == partno && p.StoreID == _store.StoreID
                           select p).FirstOrDefault();

            if (newPart == null)
            {
                SAPProduct existsSapProduct = getSAPProduct(partno, _store);
                if (existsSapProduct != null)
                {
                    newPart = new Part();

                    newPart.StoreID = _store.StoreID;

                    if (string.IsNullOrEmpty(newPart.SProductID) == true)
                        newPart.SProductID = partno;

                    newPart.VProductID = partno;
                    newPart.VendorID = "Advantech" + _store.StoreID;
                    newPart.CreatedBy = "Sync Job";
                    newPart.LastUpdatedBy = "Sync Job";
                    newPart.CreatedDate = DateTime.Now;
                    newPart.LastUpdated = DateTime.Now;
                    newPart.priceSource = Part.PRICESOURCE.VENDOR;
                    newPart.ProductType = "RelatedParts";

                    //List<string> partnos = new List<string>();
                    //partnos.Add(partno);
                    //Dictionary<string, decimal> pricelist = getPriceFromSAP(partnos, _store);
                    //newPart.VendorSuggestedPrice = pricelist[partno];

                    //getprice from SAP
                    setLogistic(_store, newPart);

                    SyncPIS(newPart, _store);

                    //if (newPart.isPTradePart())
                    //    getPAPSDetail(newPart);
                }
            }

            return newPart;
        }


        private void cleanPart(Part p)
        {
            eStore3Entities6 actingContext = getActingContext(p);

            foreach (ProductResource ps in p.ProductResources.ToList())
            {
                //IsLocalResource 没有值或者=false则为pis数据，同步时删除，否则为本地数据，同步不删除，OM中做处理。
                if (ps.IsLocalResource.HasValue == false || ps.IsLocalResource == false)
                    actingContext.ProductResources.DeleteObject(ps);
            }

            foreach (RelatedProduct rp in p.RelatedProducts.ToList())
            {
                actingContext.RelatedProducts.DeleteObject(rp);
            }            
        }

        public PISModel getModel(Part part)
        {
            if (part.isTenDigitPart())  //no model in nature for 96xxx PTrade part -- 2017.12.5 ptrade part use partno for model on pis.
            {
                if (part.SProductID.ToUpper().StartsWith("PSD-")) //Let PSD product in.
                { }
                else
                    return null;
            }

            String partNo = part.SProductID;
            CachePool cachePool = CachePool.getInstance();
            String cacheKey = part.StoreID + ".Model." + partNo;
            PISModel model = (PISModel)cachePool.getObject(cacheKey);
            if (model == null)
            {
                List<spGetModelByPN_estore_Result> newModels = null;
                spGetModelByPN_estore_Result matchPISModel = null;
                 try
                 {
                    if ((part is Product) && ((Product)part).status == Product.PRODUCTSTATUS.COMING_SOON)
                    {
                        try
                        {
                            StringBuilder sbcmd = new StringBuilder();
                            sbcmd.AppendLine("SELECT Model.DISPLAY_NAME 	, PRODUCT.PART_NO , Model.MODEL_ID  as CATEGORY_ID ");
                            sbcmd.AppendLine("FROM         Model INNER JOIN ");
                            sbcmd.AppendLine("model_product ON Model.MODEL_NAME = model_product.model_name INNER JOIN ");
                            sbcmd.AppendLine("PRODUCT ON model_product.part_no = PRODUCT.PART_NO ");
                            sbcmd.AppendFormat("where PRODUCT.PART_NO='{0}'", part.SProductID);
                            newModels = (from m in pcontext.ExecuteStoreQuery<spGetModelByPN_estore_Result>(sbcmd.ToString(), null)
                                            orderby m.DISPLAY_NAME.Length descending
                                            select m).ToList();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        /*
                        newModel = (from m in pcontext.spGetModelByPN_estore_NEW(catalogid1, catalogid2, partNo)
                                    orderby m.DISPLAY_NAME ascending
                                    select m).FirstOrDefault();
                            * */
                        //string c1 = catalogid1;
                        //string c2 = catalogid2;
                        //if (isBBPartNo(partNo) == true)
                        //{
                        //    c1 = catelogid_BB;
                        //    c2 = string.Empty;
                        //}
                        //newModels = (from m in pcontext.spGetModelByPN_estore_NEW(c1, c2, partNo)
                        //            orderby m.DISPLAY_NAME.Length descending
                        //            select m).ToList();
                        //ICC Try IOT & U-shop catalog
                        //if (newModels.Count == 0)
                        //    newModels = (from m in pcontext.spGetModelByPN_estore_NEW(catalogid_IOT, catalogid_Ushop, partNo)
                        //                 orderby m.DISPLAY_NAME.Length descending
                        //                 select m).ToList();

                        //ICC Try B+B catalog
                        //if (newModels.Count == 0)
                        //    newModels = (from m in pcontext.spGetModelByPN_estore_NEW(catelogid_BB, catalogid2, partNo)
                        //                 orderby m.DISPLAY_NAME.Length descending
                        //                 select m).ToList();

                        //Use interface
                        VPisInfo vp = new VPisInfo(part, pcontext);
                        newModels = vp.getModelByPartNo(part.SProductID);
                    }

                    //find the most match model using naming convention here
                    foreach (spGetModelByPN_estore_Result newModel in newModels)
                    {
                        if (partNo.StartsWith(newModel.DISPLAY_NAME, StringComparison.CurrentCultureIgnoreCase))
                        {
                            matchPISModel = newModel;
                            break;
                        }
                    }

                    //if no match model, then use the first model returned from PIS
                    if (matchPISModel == null)
                        matchPISModel = newModels.FirstOrDefault();
                 }
                catch (Exception ex)
                {
                }

                model = new PISModel(matchPISModel, pcontext, pisws);
                if (model != null)
                    cachePool.cacheObject(cacheKey, model, CachePool.CacheOption.Hour12);
            }

            if (model != null && !model.isBlankModel())
                return model;
            else
                return null;
        }

        /// <summary>
        /// This method is to sync product related accessory from PIS platform
        /// </summary>
        /// <param name="_part"></param>
        /// <param name="_store"></param>
        /// <returns></returns>
        public String syncProductAccessories(Part _part, Store _store)
        {
            StringBuilder error = new StringBuilder();
            PISModel _model = getModel(_part);

            if (_model != null)
            {
                List<sp_GetRelatedProducts_Result> _relatedp = pcontext.sp_GetRelatedProducts(_model.modelName).ToList();

                if (_relatedp != null && _relatedp.Count>0 )
                {
                    try
                    {
                        List<RelatedProduct> syncedItems = new List<RelatedProduct>();

                        foreach (sp_GetRelatedProducts_Result item in _relatedp)
                        {
                            RelatedProduct match = _part.RelatedProducts.FirstOrDefault(r => r.RelatedSProductID == item.ACCPN);
                            if (match == null)  //new item
                            {
                                Part relatedPart = constructRelatedPart(item.ACCPN, _store, true);
                                if (relatedPart != null && String.IsNullOrWhiteSpace(relatedPart.StockStatus) == false)  //Part is available in store region
                                {
                                    match = new RelatedProduct();
                                    //bind association
                                    match.StoreID = _part.StoreID;
                                    match.SProductID = _part.SProductID;
                                    //fill up rest of item informaiton
                                    match.RelatedSProductID = item.ACCPN;
                                    match.Relationship = item.relation_type;
                                    match.CreatedDate = DateTime.Now;

                                    //using related parts description instead of models and save part if it's new
                                    relatedPart.VendorProductDesc = item.PRODUCT_DESC;
                                    if (relatedPart.parthelper == null)     //new item
                                    {
                                        if (save(relatedPart))    //save successful
                                            _part.RelatedProducts.Add(match);
                                        else
                                            error.AppendLine("Failed at save related part " + relatedPart.SProductID);
                                    }
                                    else
                                        _part.RelatedProducts.Add(match);
                                }
                            }
                            else
                            {
                                if (match.Relationship.Equals(item.relation_type) == false)
                                    match.Relationship = item.relation_type;
                            }
                            syncedItems.Add(match);
                        }

                        //remove old, non-used items
                        eStore3Entities6 actingContext = context;
                        if (_part.parthelper != null && _part.parthelper.getContext() != null)
                            actingContext = _part.parthelper.getContext();
                        foreach (RelatedProduct item in _part.RelatedProducts.ToList())
                        {
                            if (!item.IseStoreSetting.HasValue)
                            {
                                if (syncedItems.Contains(item) == false)
                                {
                                    _part.RelatedProducts.Remove(item);
                                    actingContext.RelatedProducts.DeleteObject(item);
                                }
                                else
                                {
                                    if (item.RelatedID == 0)
                                        actingContext.RelatedProducts.AddObject(item);
                                    else
                                        actingContext.RelatedProducts.ApplyCurrentValues(item);
                                }
                            }
                        }

                        actingContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        error.AppendLine(_part.SProductID + " Exception : " + ex.StackTrace);
                    }
                }
            }
            else
            {
                error.AppendLine(_part.SProductID + " does not have model associated.");
            }

            return error.ToString();
        }

        public String updateCTOSImageURL(Store store, String partPrefix=null, Action<String> writeLog = null)
        {
            StringBuilder error = new StringBuilder();
            StringBuilder errorCTOS = new StringBuilder();

            List<Product_Ctos> systems = null;
            if (String.IsNullOrWhiteSpace(partPrefix))
                systems = (new PartHelper()).getCTOSProducts(store.StoreID, false, true /*include solution_only system*/);
            else
                systems = (new PartHelper()).getMatchedCTOSProducts(store.StoreID, partPrefix, true /*include solution_only system*/);

            foreach (Product_Ctos ctos in systems)
            {
                errorCTOS.Clear();
                bool isSave = false;
                //Update CTOS thumbnail Image
                if (ctos.specSources != null && ctos.specSources.Count > 0)
                {
                    String newImageURL = ctos.specSources[0].thumbnailImageX;

                    //Hardcoded logic per Russell request.  This part need to be removed once this CTOS product is phased out.
                    //if (ctos.SProductID == "21330" || ctos.SProductID == "21336")
                    //    newImageURL = "http://downloadt.advantech.com/download/downloadlit.aspx?LIT_ID=ba9b0918-796f-4aa0-9cba-48e0e7697b5a";
                    if (ctos.ProductResources.Any(x => x.ResourceType == "eStoreLocalMainImage"))
                    {
                        newImageURL= ctos.ProductResources.First(x => x.ResourceType == "eStoreLocalMainImage").ResourceURL;
                    }
                    
                    if (this.hasNewValidURL(ctos.ImageURL, newImageURL))
                    {
                        ctos.ImageURL = newImageURL;
                        ctos.LastUpdated = DateTime.Now;
                        isSave = true;
                    }

                    //use CTOS local datasheet if there is any.  Otherwise take CTOS data sheet from its first main spec source part
                    String newDataSheet = string.Empty;
                    //如果ctos自身有datesheet resource,  使用自身resource
                    if (ctos.ProductResources.Any(x => x.ResourceType == "Datasheet"))
                    {
                        POCOS.ProductResource prDataSheet = ctos.ProductResources.OrderByDescending(p => p.ResourceID).FirstOrDefault(p => p.ResourceType == "Datasheet");
                        newDataSheet = prDataSheet.ResourceURL;
                    }
                    else
                        newDataSheet = ctos.specSources[0].dataSheetX;

                    if (!string.IsNullOrEmpty(newDataSheet))
                    {
                        if (ctos.DataSheet != newDataSheet)
                        {
                            //sync datasheet
                            ctos.DataSheet = newDataSheet;
                            isSave = true;
                        }
                    }
                    else
                        error.AppendLine("No CTOS data sheet found, " + ctos.DisplayPartno + ", @main component, " + ctos.specSources[0].SProductID + ".");

                    if (isSave)
                    {
                        if (ctos.save() != 0) //failed at update
                            error.AppendLine("Failed at updating CTOS, " + ctos.DisplayPartno);
                    }
                }
                else
                    error.AppendLine("No main component is found at CTOS, " + ctos.DisplayPartno + ".");

                //if writeLog method is provided, output error immediately instead of returning it
                if (writeLog != null && error.Length > 0)
                {
                    writeLog(error.ToString());
                    error.Clear();
                }
            }
            
            return error.ToString();
        }

        /// <summary>
        /// get part MIN_ORDER_QTY from pis
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int? getPartMIN_ORDER_QTY(Part p, Store _store)
        {
            string plant = _store.Settings["SAPDefaultPlant"];
            var moqProduct = (from c in pcontext.SAP_PRODUCT_STATUS
                              where c.DLV_PLANT == plant && c.PART_NO.ToUpper() == p.SProductID.ToUpper()
                              select c.MIN_ORDER_QTY).FirstOrDefault();
            int? moq = null;
            if (moqProduct != null && moqProduct.HasValue)
                moq = (int)moqProduct.Value;
            return moq;
        }

        /// <summary>
        /// Round up CTOS price, update to storeprice only the price greater than 0 
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public String RoundUpCTOSPrice(Store store, Action<String> writeLog = null) {
            StringBuilder errors = new StringBuilder();
            /*
            var _ctos = from c in context.Parts.OfType<Product_Ctos>()
                        where c.StoreID == store.StoreID
                        select c;
            */
            List<Product_Ctos> _ctos = (new PartHelper()).getCTOSProducts(store.StoreID, false, true /*include solution_only system*/);

            try
            {
                foreach (Product_Ctos c in _ctos.ToList())
                {
                    try
                    {
                        //recalculate CTOS default price
                        Price storeprice = c.recalculateCTOSDefaultPrice(store.ctosRoundingUnit);
                        //only overwrite local store price when the price is designated to be automatically updated.
                        if (c.priceSourceX == Part.PRICESOURCE.VENDOR)
                        {
                            c.StorePrice = storeprice.value;
                            c.LastUpdated = DateTime.Now;
                        }

                        if (c.save() != 0)  //fail at update
                            errors.AppendLine("Failed at update CTOS rounded price, " + c.DisplayPartno);
                        else
                        {
                            if (c.changeLogs.Any())
                            {
                                foreach (var log in c.changeLogs)
                                    log.save();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(String.Format("Problem at rounding {0} CTOS price : {1}", c.DisplayPartno, ex.StackTrace));
                    }

                    if (writeLog != null && errors.Length > 0)
                    {
                        writeLog(errors.ToString());
                        errors.Clear();
                    }
                }
            }
            catch (Exception ex) {
                eStore.Utilities.eStoreLoger.Fatal(ex.Message, "RoundUpCTOSPrice", "", "", ex);
            }

            return errors.ToString();
        }

        //同步coming soon需要的图片和信息
        public void SyncComingSoonFromPIS(Part _part, Store _store)
        {
            PISModel model = getModel(_part);
           
            //Find the model info in the PIS
            if (model != null)
            {
                string imageurl = "";
                string logistic = _store.Settings["ProductLogisticsOrg"];
                String lang = getLang(_store.StoreID);
                sp_GetModelInfo_estore_Result _minfo = model.getModelInfo(lang);
                _part.VendorFeatures = model.getProductFeature(lang);

                if (_minfo != null)
                {
                    if (String.IsNullOrWhiteSpace(_part.VendorProductName))
                        _part.VendorProductName = _minfo.DISPLAY_NAME;
                    _part.ModelNo = _minfo.DISPLAY_NAME;
                    if (_part is Product)
                    {
                        if (!(_part is Product_Ctos))
                        {
                            _part.VendorProductDesc = string.IsNullOrEmpty(_minfo.PRODUCT_DESC) ? _part.VendorProductDesc : _minfo.PRODUCT_DESC;
                            _part.VendorExtendedDesc = _minfo.EXTENDED_DESC;
                        }
                    }
                }

                if (string.IsNullOrEmpty(_minfo.tumbnail_image_id) == false)
                {
                    if (_minfo.tumbnail_image_id.Contains("download") || _minfo.tumbnail_image_id.Contains("downloadt"))
                        imageurl = _minfo.tumbnail_image_id.Replace("http://download.advantech.com", "http://downloadt.advantech.com").Replace("http://downloadt.advantech.com", "https://downloadssl.advantech.com");
                    else
                        imageurl = String.Format("https://downloadssl.advantech.com/download/downloadlit.aspx?LIT_ID={0}", _minfo.tumbnail_image_id);
                }

                if (hasNewValidURL(_part.TumbnailImageID, imageurl))
                    _part.TumbnailImageID = imageurl;

                if (_part is Product && !String.IsNullOrWhiteSpace(_part.TumbnailImageID))
                    ((Product)_part).ImageURL = _part.TumbnailImageID;
            }
        }

        /// <summary>
        /// This method sync part information with PIS
        /// </summary>
        /// <param name="_part"></param>
        /// <param name="_store"></param>
        /// <param name="isrelatedPart"></param>
        /// <param name="ignoreOrg">If this parameter is true, it will return a part as long as it's defned in SAP regardless if 
        /// this part is available in store region or not.  This parameter is mainly for AOnline solution use.</param>
        /// <returns></returns>
        public String SyncPIS(Part _part, Store _store, Boolean ignoreOrg = false, bool? _existpart = null)
        {
            StringBuilder errors = new StringBuilder();
            try
            {
                olddesc = _part.VendorProductDesc;
                oldfeature = _part.VendorFeatures;

                //sync part basic information from SAP
                setLogistic(_store, _part, ignoreOrg);

                PISModel model = getModel(_part);
                //Find the model info in the PIS
                if (model != null)
                {
                    string imageurl = "";
                    string datasheetID = "";
                    string datasheeturl = "";
                    string largeimg = "";
                    string downloadurl = "";
                    string Manualurl = "";
                    string Driverurl = "";
                    string utitilitesUrl = "";
                    string threedOnlineViewApiUrl = "";
                    string logistic = _store.Settings["ProductLogisticsOrg"];
                    String lang = getLang(_store.StoreID);
                    sp_GetModelInfo_estore_Result _minfo = model.getModelInfo(lang);
                    string pisfeatures = model.getProductFeature(lang);
                    string pisdescription = "";
                    string pisextendedDesc = "";

                    if (_minfo != null)
                    {
                        if (String.IsNullOrWhiteSpace(_part.VendorProductName)) //only assign new value when its empty
                            _part.VendorProductName = _minfo.DISPLAY_NAME;
                        datasheetID = model.getDataSheetID(lang);
                        pisdescription = _minfo.PRODUCT_DESC == null ? _part.VendorProductDesc : _minfo.PRODUCT_DESC;
                        pisextendedDesc = _minfo.EXTENDED_DESC;
                    }

                    _part.VendorFeatures = pisfeatures;
                    //if it is standard products, then we can use PIS' description.
                    if (_part is Product)
                    {
                        if (!(_part is Product_Ctos))
                        {
                            _part.VendorProductDesc = string.IsNullOrEmpty(pisdescription) ? _part.SProductID : pisdescription;
                            _part.VendorExtendedDesc = pisextendedDesc;

                            SAPProductWarrantyHelper helper = new SAPProductWarrantyHelper();
                            SAP_Product_Warranty warrantyModel = new SAP_Product_Warranty();
                            warrantyModel = helper.getSAPProductWarranty(_part.SProductID);
                            if (warrantyModel != null)
                            {
                                ((Product)_part).WarrantyMonth = warrantyModel.WarrantyMonth;
                            }
                        }
                    }

                    //compose product resources
                    if (!string.IsNullOrEmpty(_minfo.tumbnail_image_id)&&!_minfo.tumbnail_image_id.EndsWith("LIT_ID="))
                    {
                        List<ProductResource> resources = new List<ProductResource>();

                        //_part.TumbnailImageID = _minfo.tumbnail_image_id;
                        if (_minfo.tumbnail_image_id.Contains("download") || _minfo.tumbnail_image_id.Contains("downloadt"))
                            imageurl = _minfo.tumbnail_image_id.Replace("http://download.advantech.com", "http://downloadt.advantech.com").Replace("http://downloadt.advantech.com", "https://downloadssl.advantech.com");
                        else
                        {
                            //imageurl = pisws.GetPhysicalDownloadURL(_minfo.tumbnail_image_id).Replace("http://download.advantech.com", "http://downloadt.advantech.com");
                            imageurl = String.Format("https://downloadssl.advantech.com/download/downloadlit.aspx?LIT_ID={0}", _minfo.tumbnail_image_id);
                        }

                        //datasheeturl = "http://support.advantech.com.tw/support/DownloadDatasheet.aspx?Literature_ID=" + datasheetID;

                        PISSetting pISSetting = PISSetting.GetCurrent(_store.StoreID);

                        if (!string.IsNullOrEmpty(datasheetID))
                            datasheeturl = $"{pISSetting.WSite}/products/todatasheet/" + _minfo.product_id;
                        
                        largeimg = "http://www.advantech.com/products/LargeImgShow-pis.asp?product_ID=" + _minfo.product_id + "&model=" + model.modelName;
                       
                        downloadurl = $"{pISSetting.SupportSite}/support/SearchResult.aspx?keyword=" + model.modelName;
                        Manualurl = $"{pISSetting.SupportSite}/support/SearchResult.aspx?keyword=" + model.modelName + "&searchtabs=Manual";
                        Driverurl = $"{pISSetting.SupportSite}/support/SearchResult.aspx?keyword=" + model.modelName + "&searchtabs=Driver";
                        utitilitesUrl = $"{pISSetting.SupportSite}/support/SearchResult.aspx?keyword=" + model.modelName + "&searchtabs=Utility";

                        if (_store.getBooleanSetting("BBFlag", false) == true)
                        {
                            //datasheeturl = "http://support.advantech-bb.com/download?product_model_name=" + _minfo.product_id + "#Datasheet";
                            //datasheeturl = "http://support.advantech-bb.com/download?product_model_name=" + model.modelName + "#Datasheet";
                            //if (!string.IsNullOrEmpty(datasheeturl))
                            //    datasheeturl = datasheeturl + "?CID=736d1add-de2e-47ef-a7c4-00625ad8d53f";
                            string bbDatasheet = GetBBeSotreProductDatasheet(_part);
                            if (!string.IsNullOrEmpty(bbDatasheet))
                                datasheeturl = bbDatasheet;
                            downloadurl = "http://support.advantech-bb.com/download?product_model_name=" + model.modelName;
                            Manualurl = "http://support.advantech-bb.com/download?product_model_name=" + model.modelName + "#Manual";
                            Driverurl = "http://support.advantech-bb.com/download?product_model_name=" + model.modelName + "#Driver";
                            utitilitesUrl = "http://support.advantech-bb.com/download?product_model_name=" + model.modelName + "#Utility";
                        }

                        //把datasheet赋值一个去product
                        if (_part is Product && !(_part is Product_Ctos))
                        {
                            //获取最后一个 local datasheet,如果存在 使用local datasheet. 如果不存在 就使用pis中的datasheet
                            POCOS.ProductResource currentDatasheetResource = _part.ProductResources.ToList().Where(p => p.ResourceType == "Datasheet" && 
                                                                            p.IsLocalResource.HasValue && p.IsLocalResource.Value).OrderByDescending(p => p.ResourceID).FirstOrDefault();
                            if (currentDatasheetResource != null)
                                ((Product)_part).DataSheet = currentDatasheetResource.ResourceURL;
                            else
                                ((Product)_part).DataSheet = datasheeturl;

                        }
                        if (!string.IsNullOrEmpty(datasheeturl))
                            resources.Add(new ProductResource(_part, "Datasheet", "Datasheet", datasheeturl));
                        resources.Add(new ProductResource(_part, "LargeImage", "LargeImage", largeimg));
                        resources.Add(new ProductResource(_part, "Download", "Download", downloadurl));
                        resources.Add(new ProductResource(_part, "Manual", "Manual", Manualurl));
                        resources.Add(new ProductResource(_part, "Driver", "Driver", Driverurl));
                        resources.Add(new ProductResource(_part, "Utilities", "Utilities", utitilitesUrl));

                        resources.AddRange(model.get3DModelProductResource(lang));
                        var Certificatesls = GetCertificateByModelID(_part);
                        if (Certificatesls != null)
                        {
                            foreach (var item in Certificatesls)
                            {
                                resources.Add(new ProductResource(_part, item.CertificateName, "Certificates", item.CertificateImagePath));
                            }
                        }


                        //若Resource中有3D檔且為.STEP,則Call 官網3dmodel online view API取得ViewerURL存入ProductResource
                        foreach (var rs in resources) 
                        {
                            if (rs.ResourceName == "STEP" && _store.getBooleanSetting("BBFlag", false) == false) //Exclude ABB store
                            {
                                if (_store.StoreID == "ACN")
                                {

                                    threedOnlineViewApiUrl = "http://www.advantech.com.cn/api/products/threedmodel/" + model.categoryID + "?jsonpcallback";
                                }
                                else
                                {
                                    threedOnlineViewApiUrl = "http://www.advantech.com/api/products/threedmodel/" + model.categoryID + "?jsonpcallback";
                                }
                                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(threedOnlineViewApiUrl);
                                request.Method = WebRequestMethods.Http.Get;
                                request.ContentType = "application/json";
                                using (var response = (HttpWebResponse)request.GetResponse())
                                {
                                    if (response.StatusCode == HttpStatusCode.OK)
                                    {
                                        using (var stream = response.GetResponseStream())
                                        using (var reader = new StreamReader(stream))
                                        {
                                            var temp = reader.ReadToEnd();
                                            ThreedmodelViewer viewer = JsonHelper.JsonDeserialize<ThreedmodelViewer>(temp);
                                            if (viewer.error == "")
                                            {
                                                //Add new 3DModel Online View 
                                                resources.Add(new ProductResource(_part, "3DModelOnlineView", "3DModelOnlineView", viewer.viewerURL));
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }

                        //apply product resource update
                        errors.Append(updateProductResources(_part, resources));
                    }

                    if (hasNewValidURL(_part.TumbnailImageID, imageurl))
                    {
                        //only save image while it is valid
                        _part.TumbnailImageID = imageurl;
                    }

                    if (_part is Product && !String.IsNullOrWhiteSpace(_part.TumbnailImageID)
                         && !esUtilities.StringUtility.IsLocalUrl(((Product)_part).ImageURL) && ((Product)_part).ImageURL != _part.TumbnailImageID)
                    {
                        ((Product)_part).ImageURL = _part.TumbnailImageID;
                    }

                    _part.ProductinfoProvider = "AdvantechPIS";
                    _part.ModelNo = _minfo.DISPLAY_NAME;
                    _part.LastUpdated = DateTime.Now;



                    if (string.IsNullOrEmpty(oldfeature))
                        oldfeature = "";

                    if (string.IsNullOrEmpty(olddesc))
                        olddesc = "";

                    if (!olddesc.Equals(_part.VendorProductDesc) || !oldfeature.Equals(_part.VendorFeatures))
                        _part.hasProductInfoChanged = true;
                }
                else if (_part.isEPAPS())
                {
                    errors.Append(PAPSSync.getInstance().Sync(ref _part));
                    var papsResouces = PAPSSync.getInstance().getPAPSProductResource(_part);
                    if (papsResouces != null && papsResouces.Any())
                    {
                        errors.Append(updateProductResources(_part, papsResouces));
                    }

                }
                else
                {
                    if (!_part.isTenDigitPart() && !_part.isPTradePart())
                        errors.AppendLine(_part.SProductID + " has no model associated to.");
                }

                //更新MIN ORDER QTY
                //_part.MininumnOrderQty = getPartMIN_ORDER_QTY(_part, _store); ;
                int? moq = getPartMIN_ORDER_QTY(_part, _store);
                if (moq != null && moq.HasValue)
                    _part.MininumnOrderQty = moq;
                else
                    _part.MininumnOrderQty = 0;

                //save part and product resource update
                if (ignoreOrg == false) //when ignoreOrg is true, the part may not carrier by current store. Therefore no point to save it
                    save(_part, errors, _existpart);
            }
            catch (Exception e)
            {
                Console.WriteLine(_part.SProductID + ": " + e.InnerException);
                errors.AppendLine(_part.SProductID + " exception : " + e.StackTrace);
            }
            return errors.ToString();
        }

        /// <summary>
        /// This method is to sync part details from PIS
        /// </summary>
        /// <param name="_part"></param>
        /// <param name="_store"></param>
        /// <returns>error message if there is any</returns>
        public String SyncPISDetail(Part syncPart, Store _store)
        {
            StringBuilder errors = new StringBuilder();
            /*
            try
            {
                olddesc = syncPart.VendorProductDesc;
                oldfeature = syncPart.VendorFeatures;

                //update basic SAP product info
                setLogistic(_store, syncPart);

                string imageurl = "";
                string datasheetID = "";
                string datasheeturl = "";
                string largeimg = "";
                string downloadurl = "";
                string Manualurl = "";
                string Driverurl = "";
                string utitilitesUrl = "";

                string logistic = _store.Settings["ProductLogisticsOrg"];

                //Find the model info in the PIS
                PISModel _model = getModel(syncPart.SProductID);

                if (_model != null)
                {
                    if (syncPart.ModelNo.Equals(_model.modelName)== false)
                    {
                        syncPart.ModelNo = _model.modelName;
                        syncPart.isUpdated = true;
                    }

                    //retrieve model information from PIS
                    sp_GetModelInfo_estore_Result _minfo = pcontext.sp_GetModelInfo_estore(_model.categoryID, getLang(_store.StoreID)).FirstOrDefault();
                    
                    List<sp_GetProductFeature_Result> pf = pcontext.sp_GetProductFeature(_model.modelName, getLang(_store.StoreID)).OrderBy(x => x.FEATURE_SEQ).ToList();

                    string pisfeatures = "";
                    string pisdescription = "";
                    string pisextendedDesc = "";

                    foreach (sp_GetProductFeature_Result pfs in pf.ToList())
                        pisfeatures = pisfeatures + "<li>" + pfs.FEATURE_DESC + "</li>";

                    if (syncPart.VendorFeatures.Equals(pisfeatures) == false)
                    {
                        syncPart.VendorFeatures = pisfeatures;
                        syncPart.isUpdated = true;
                    }

                    if (_minfo != null)
                    {
                        if (syncPart.VendorProductName.Equals(_minfo.DISPLAY_NAME) == false)
                        {
                            syncPart.VendorProductName = _minfo.DISPLAY_NAME;
                            syncPart.isUpdated = true;
                        }

                        datasheetID = getDatasheetID(_minfo.product_id, syncPart, _store);
                        pisdescription = _minfo.PRODUCT_DESC == null ? syncPart.VendorProductDesc : _minfo.PRODUCT_DESC;
                        pisextendedDesc = _minfo.EXTENDED_DESC;
                    }

                    //if it is standard products, then we can use PIS' description.
                    if (syncPart is Product)
                    {
                        if (!(syncPart is Product_Ctos))
                        {
                            if (syncPart.VendorProductDesc.Equals(pisdescription) == false)
                            {
                                syncPart.VendorProductDesc = pisdescription;
                                syncPart.isUpdated = true;
                            }

                            if (syncPart.VendorExtendedDesc.Equals(pisextendedDesc) == false)
                            {
                                syncPart.VendorExtendedDesc = pisextendedDesc;
                                syncPart.isUpdated = true;
                            }
                        }
                    }

                    //valid thumbnail image URL
                    if (string.IsNullOrEmpty(_minfo.tumbnail_image_id) == false)
                    {
                        if (_minfo.tumbnail_image_id.Contains("download") || _minfo.tumbnail_image_id.Contains("downloadt"))
                            imageurl = _minfo.tumbnail_image_id.Replace("http://download.advantech.com", "http://downloadt.advantech.com");
                        else
                            imageurl = pisws.GetPhysicalDownloadURL(_minfo.tumbnail_image_id).Replace("http://download.advantech.com", "http://downloadt.advantech.com");

                        if (hasNewValidURL(syncPart.TumbnailImageID, imageurl))
                        {
                            //only save image while it is valid
                            syncPart.TumbnailImageID = imageurl;
                            syncPart.isUpdated = true;
                        }
                    }

                    //compose product resource links
                    datasheeturl = "http://support.advantech.com.tw/support/DownloadDatasheet.aspx?Literature_ID=" + datasheetID;
                    largeimg = "http://www.advantech.com/products/LargeImgShow-pis.asp?product_ID=" + _minfo.product_id + "&model=" + _model.modelName;
                    downloadurl = "http://support.advantech.com.tw/Support/DownloadSearchByProduct2.aspx?keyword=" + _model.modelName;
                    Manualurl = "http://support.advantech.com.tw/Support/DownloadSearchByProduct2.aspx?keyword=" + _model.modelName + "&ctl00_ContentPlaceHolder1_EbizTabStripNoForm2_Tab=Manual";
                    Driverurl = "http://support.advantech.com.tw/Support/DownloadSearchByProduct2.aspx?keyword=" + _model.modelName + "&ctl00_ContentPlaceHolder1_EbizTabStripNoForm2_Tab=Driver";
                    utitilitesUrl = "http://support.advantech.com.tw/Support/DownloadSearchByProduct2.aspx?keyword=" + _model.modelName + "&ctl00_ContentPlaceHolder1_EbizTabStripNoForm2_Tab=Utility";

                    List<ProductResource> newResources = new List<ProductResource>();
                    newResources.Add(createProductResource("Datasheet", datasheeturl, "Datasheet"));
                    newResources.Add(createProductResource("LargeImage", largeimg, "LargeImage"));
                    newResources.Add(createProductResource("Download", downloadurl, "Download"));
                    newResources.Add(createProductResource("Manual", Manualurl, "Manual"));
                    newResources.Add(createProductResource("Driver", Driverurl, "Driver"));
                    newResources.Add(createProductResource("Utilities", utitilitesUrl, "Utilities"));
                    newResources.AddRange(get3DModelProductResource(syncPart, _minfo));

                    String error = updateProductResources(syncPart, newResources);
                    if (!String.IsNullOrWhiteSpace(error))
                        errors.AppendLine(error);

                    if (syncPart.ProductinfoProvider.Equals("AdvantechPIS") == false)
                    {
                        syncPart.ProductinfoProvider = "AdvantechPIS";
                        syncPart.isUpdated = true;
                    }

                    if (syncPart.ModelNo.Equals(_minfo.DISPLAY_NAME) == false)
                    {
                        syncPart.ModelNo = _minfo.DISPLAY_NAME;
                        syncPart.isUpdated = true;
                    }
                }

                if (string.IsNullOrEmpty(oldfeature))
                    oldfeature = "";

                if (string.IsNullOrEmpty(olddesc))
                    olddesc = "";

                if (!olddesc.Equals(syncPart.VendorProductDesc) || !oldfeature.Equals(syncPart.VendorFeatures))
                {
                    syncPart.hasProductInfoChanged = true;
                    syncPart.isUpdated = true;
                }

                if (syncPart.isUpdated)
                    syncPart.LastUpdated = DateTime.Now;
            }
            catch (Exception e)
            {
                errors.AppendLine(syncPart.SProductID + " : " + e.StackTrace);
            }
         * */

            return errors.ToString();
        }

        /// <summary>
        /// This method is to check whether the URL is changed or not.  If it's changed, it further validates whether the new URL
        /// is valid or not.
        /// </summary>
        /// <param name="origURL"></param>
        /// <param name="newURL"></param>
        /// <returns></returns>
        private Boolean hasNewValidURL(String origURL, String newURL)
        {
            Boolean hasNewURL = false;
            if ( !String.IsNullOrWhiteSpace(newURL) &&
                ( String.IsNullOrWhiteSpace(origURL) || origURL.Equals(newURL)==false))
            {
                hasNewURL = true;
            }

            //if (!string.IsNullOrEmpty(imageurl) && validateURL(imageurl))
            if (hasNewURL)
            {
                //local resource以/resource/开始.   要加判断,不然下面的 web request会报错.  
                if (newURL.ToLower().StartsWith("/resource/") || validateURL(newURL))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// get Language by Store
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private string getLang(string storeid)
        {

            return PISSetting.GetCurrent(storeid).Lang;
        }

        /// <summary>
        /// This method will find and return the matched SAPProduct
        /// </summary>
        /// <param name="_partno"></param>
        /// <param name="_store"></param>
        /// <returns></returns>
        public SAPProduct getSAPProduct(string _partno, Store _store, Boolean ignoreOrg = false)
        {
            SAPProduct sapProduct = null;
            if (ignoreOrg)
            {
                sapProduct = (from parts in context.SAPProducts
                              where parts.PART_NO == _partno
                              select parts).FirstOrDefault();
            }
            else
            {
                string logisticorg = getSetting(_store, "ProductLogisticsOrg"); // 是否考虑使用 PriceSAPOrg ？
                sapProduct = (from parts in context.SAPProducts
                                         where parts.PART_NO == _partno && parts.ORG_ID == logisticorg
                                         select parts).FirstOrDefault();
            }

            #region 现在把 同步cost\Currency 放到同步Price中.
            //if (sapProduct != null && _store != null && !String.IsNullOrEmpty(sapProduct.Currency) && !sapProduct.Currency.Equals(_store.defaultCurrency.CurrencyID))
            //{
            //    bool isExchangeVale = true;
            //    string errorMsg = "";
            //    eStore.POCOS.MySAPDAL.MYSAPDAL mysapdal = new MySAPDAL.MYSAPDAL();
            //    var sapDetail = mysapdal.GetProductCost(sapProduct.PART_NO, _store.StoreID, ref errorMsg);
            //    if (!string.IsNullOrEmpty(errorMsg))
            //        eStore.Utilities.eStoreLoger.Fatal(errorMsg);
            //    else
            //    {
            //        if (sapDetail != null && sapDetail.Any())
            //        {
            //            var sapD = sapDetail.FirstOrDefault();
            //            if (sapD != null)
            //            {
            //                sapProduct.Cost = sapD.Cost;
            //                sapProduct.Currency = sapD.CostCurrency;
            //                if (_store.defaultCurrency.CurrencyID.Equals(sapProduct.Currency, StringComparison.OrdinalIgnoreCase))
            //                    isExchangeVale = false;
            //            }
            //        }
            //    }
            //    if (isExchangeVale)
            //    {
            //        //The logistics site has different currency from its online store.  Cost needs to be converted to local currency
            //        Decimal cost = _store.getCurrencyExchangeValue(sapProduct.Cost.GetValueOrDefault(), sapProduct.Currency, _store.defaultCurrency.CurrencyID);
            //        if (cost > 0)
            //        {
            //            sapProduct.Cost = cost;
            //            sapProduct.Currency = _store.defaultCurrency.CurrencyID;
            //        }
            //    }
            //}
            #endregion

            return sapProduct;
        }

        /// <summary>
        /// set Part information from SAP tables
        /// </summary>
        /// <param name="_store"></param>
        /// <param name="_part"></param>
        private void setLogistic(Store _store, Part _part, Boolean ignoreOrg = false)
        {
            SAPProduct _plogis = getSAPProduct(_part.SProductID, _store, ignoreOrg);
            if (_plogis != null)
            {
                //part的成本价 不根据SAPProduct走了. 在同步价格的时候, 直接获取web service.  更新Part.Cost
                //如果要根据SAPProduct走,记得上面的getSAPProduct里面要把Cost 计算加上.
                //_part.Cost = _plogis.Cost;
                //_part.Currency = _plogis.Currency;

                if (_part.StockStatus != _plogis.STATUS)
                {
                    _part.StockStatus = _plogis.STATUS;
                    _part.hasProductStockStatusChanged = true;
                }

                _part.VendorProductDesc = string.IsNullOrEmpty(_plogis.PRODUCT_DESC) ? _plogis.PART_NO : _plogis.PRODUCT_DESC;
                _part.VendorProductGroup = _plogis.PRODUCT_GROUP;
                _part.VendorProductLine = _plogis.PRODUCT_LINE;
                _part.ModelNo = _plogis.MODEL_NO;
                _part.VendorProductName = _plogis.PART_NO;
                _part.VendorExtendedDesc = _plogis.EXTENTED_DESC;
                _part.ShipWeightKG = _plogis.SHIP_WEIGHT;

                
                if (String.IsNullOrWhiteSpace(_part.PriceType))
                    _part.PriceType = "Price";

                if (String.IsNullOrWhiteSpace(_part.ProductType))
                    _part.ProductType = "Parts";
                if (_part.StockStatus == "A" && _part is Product)
                {
                    Product castedProduct = (Product)_part;
                    if (castedProduct.status == Product.PRODUCTSTATUS.COMING_SOON)
                    {
                        _part.ProductType = "STANDARD";
                        castedProduct.status = eStore.POCOS.Product.PRODUCTSTATUS.GENERAL;
                    }
                }

                //if (_part.StockStatus == "O" && _part is Product)
                //{
                //    Product castedProduct = (Product)_part;
                //    if (castedProduct.notAvailable && getValidProductsStatus(_store).Contains(castedProduct.Status)
                //        && castedProduct.status != Product.PRODUCTSTATUS.PHASED_OUT
                //        )
                //    {
                //        castedProduct.status = eStore.POCOS.Product.PRODUCTSTATUS.PHASED_OUT;
                //    }
                //}


                _part.RoHSStatus = _plogis.RoHS_Status;

                if (String.IsNullOrWhiteSpace(_part.ProductinfoProvider))
                {
                    _part.ProductinfoProvider = "AdvantechPIS";
                    _part.isUpdated = true;
                }

                _part.ABCInd = _plogis.ABC_Ind;
                _part.ProductDivision = _plogis.Product_Division;

                if (_part.LocalPrice.HasValue == false)
                     _part.LocalPrice = 0;

                _part.Certificate = _plogis.CERTIFICATE;
                _part.ProductSite = _plogis.PRODUCT_SITE;

                if (_part.CurrentStockQty.HasValue == false)
                    _part.CurrentStockQty = 99999;

               _part.NetWeightKG = _plogis.NET_WEIGHT;

                if (_part.Dimension != _plogis.Dimension)
                {
                    _part.Dimension = _plogis.Dimension;
                    _part.setDimension();
                }

                context.SAPProducts.Detach(_plogis);
            }
            else 
            {
                _part.isSAPParts = false;
            }
        }
        
        private string getSetting(Store _store, string parameter)
        {
            return _store.Settings[parameter];

        }


        private bool ifPartExist(string partno, string storeid)
        {
            var _part = (from parts in context.Parts
                         where parts.SProductID == partno && (parts.StoreID == storeid)
                         select parts).FirstOrDefault();

            if (_part != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// This will be phased out and replaced by syncPAPSDetail method
        /// </summary>
        /// <param name="_part"></param>
        /// <returns></returns>
        private String getPAPSDetail(Part _part)
        {
            StringBuilder errors = new StringBuilder();
            DataSet dsAttribute = new DataSet();
            AdvantechPAPS.PAPSProduct pproduct = new AdvantechPAPS.PAPSProduct();

            oPAPSWebservice.getPAPSProductDetail(_part.SProductID, ref pproduct, ref dsAttribute);

            if (pproduct != null)
            {
                if (hasNewValidURL(_part.TumbnailImageID, pproduct.GetProductImgName))
                {
                    _part.TumbnailImageID = pproduct.GetProductImgName;
                }

                if (_part.TumbnailImageID == null)
                    _part.TumbnailImageID = "";

                //using PAPS description
                if(!string.IsNullOrEmpty(pproduct.GetProductDescription) && !pproduct.GetProductDescription.ToUpper().Contains("NO DESCRIPTION"))
                     _part.VendorProductDesc = pproduct.GetProductDescription;

                DataSet attributes = pproduct.GetAttrDataset;

                _part.VendorFeatures = "";
                foreach (DataRow d in attributes.Tables[0].Rows)
                {
                    if (!d[0].ToString().Contains("Part Number") && !d[0].ToString().Contains("Manufacturer") && !d[0].ToString().Contains("Description"))
                        _part.VendorFeatures = _part.VendorFeatures + "<li>" + d[0] + ":" + d[1] + "</li>";
                }

                ProductResource pr_datasheet = new ProductResource();
                pr_datasheet.ResourceType = "Datasheet";
                pr_datasheet.ResourceName = "Datasheet";
                pr_datasheet.ResourceURL = "http://ags.advantech.com/PTDFiles/" + pproduct.GetProductDatasheet.Replace("../PTDFiles/", "");

                ProductResource pr_largeimg = new ProductResource();
                pr_largeimg.ResourceType = "LargeImage";
                pr_largeimg.ResourceName = "LargeImage";
                pr_largeimg.ResourceURL = "http://ags.advantech.com/PTDFiles/" + pproduct.GetProductLargePic.Replace("PTDFiles/", "");

                List<ProductResource> resources = new List<ProductResource>();
                resources.Add(pr_datasheet);
                resources.Add(pr_largeimg);
                errors.Append(updateProductResources(_part, resources));

                _part.ProductinfoProvider = "AdvantechPAPS";
            }

            return errors.ToString();
        }

        /// <summary>
        /// This method is to sync P-Trade part information with PAPS
        /// </summary>
        /// <param name="_part"></param>
        /// <returns>error message if there is any</returns>
        private String syncPAPSDetail(Part pTradePart)
        {
            StringBuilder errors = new StringBuilder();
            try
            {
                AdvantechPAPS.PAPSService oPAPSWebservice = new AdvantechPAPS.PAPSService();
                DataSet dsAttribute = new DataSet();
                AdvantechPAPS.PAPSProduct pproduct = new AdvantechPAPS.PAPSProduct();

                oPAPSWebservice.getPAPSProductDetail(pTradePart.SProductID, ref pproduct, ref dsAttribute);

                if (pproduct != null)
                {
                    if (hasNewValidURL(pTradePart.TumbnailImageID, pproduct.GetProductImgName))
                    {
                        pTradePart.TumbnailImageID = pproduct.GetProductImgName;
                        pTradePart.isUpdated = true;
                    }

                    if (pTradePart.TumbnailImageID == null)
                        pTradePart.TumbnailImageID = "";

                    //using PAPS description
                    if (!string.IsNullOrEmpty(pproduct.GetProductDescription) && !pproduct.GetProductDescription.ToUpper().Contains("NO DESCRIPTION") && !pTradePart.VendorProductDesc.Equals(pproduct.GetProductDescription))
                    {
                        pTradePart.VendorProductDesc = pproduct.GetProductDescription;
                        pTradePart.isUpdated = true;
                    }

                    //sync part features
                    DataSet attributes = pproduct.GetAttrDataset;
                    try
                    {
                        if (attributes != null && attributes.Tables != null)
                        {
                            String vendorFeatures = "";
                            foreach (DataRow d in attributes.Tables[0].Rows)
                            {
                                if (!d[0].ToString().Contains("Part Number") && !d[0].ToString().Contains("Manufacturer") && !d[0].ToString().Contains("Description"))
                                    vendorFeatures = vendorFeatures + "<li>" + d[0] + ":" + d[1] + "</li>";
                            }

                            if (!pTradePart.VendorFeatures.Equals(vendorFeatures))
                            {
                                pTradePart.VendorFeatures = vendorFeatures;
                                pTradePart.isUpdated = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.StackTrace);
                    }

                    //update product resources
                    List<ProductResource> newResources = new List<ProductResource>();
                    ProductResource pr_datasheet = new ProductResource();
                    pr_datasheet.ResourceType = "Datasheet";
                    pr_datasheet.ResourceName = "Datasheet";
                    pr_datasheet.ResourceURL = "http://ags.advantech.com/PTDFiles/" + pproduct.GetProductDatasheet.Replace("../PTDFiles/", "");

                    ProductResource pr_largeimg = new ProductResource();
                    pr_largeimg.ResourceType = "LargeImage";
                    pr_largeimg.ResourceName = "LargeImage";
                    pr_largeimg.ResourceURL = "http://ags.advantech.com/PTDFiles/" + pproduct.GetProductLargePic.Replace("PTDFiles/", "");

                    newResources.Add(pr_datasheet);
                    newResources.Add(pr_largeimg);

                    String error = updateProductResources(pTradePart, newResources);
                    if (String.IsNullOrWhiteSpace(error))
                        errors.AppendLine(error);
                }

                if (!pTradePart.ProductinfoProvider.Equals("AdvantechPAPS"))
                {
                    pTradePart.ProductinfoProvider = "AdvantechPAPS";
                    pTradePart.isUpdated = true;
                }
            }
            catch (Exception e)
            {
                errors.AppendLine(e.StackTrace);
            }

            return errors.ToString();
        }

        /// <summary>
        /// This method will update product's resources based on the input resource information.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="resource"></param>
        /// <returns>error message if there is any</returns>
        private String updateProductResources(Part part, List<ProductResource> resources)
        {
            StringBuilder errors = new StringBuilder();

            //create a new list if there is no existing resource list
            eStore3Entities6 actingContext = getActingContext(part);
            if (resources != null)
            {
                try
                {
                    //remove the resources that are no longer related to this P-Trade part in PAPS
                    //local recource不需要判断.
                    foreach (ProductResource rp in part.ProductResources.ToList().Where(p=> !p.IsLocalResource.HasValue || !p.IsLocalResource.Value))
                    {
                        //check if any existing related part has been removed from related parts in PIS
                        ProductResource match = resources.FirstOrDefault(p => p.ResourceType.Equals(rp.ResourceType) && p.ResourceName.Equals(rp.ResourceName) && p.ResourceURL.Equals(rp.ResourceURL));
                        if (match == null)
                        {
                            part.ProductResources.Remove(rp);
                            actingContext.ProductResources.DeleteObject(rp);
                            actingContext.SaveChanges();
                        }
                        else
                            resources.Remove(match);
                    }

                    //sync remaining resources with current P-Trade part resource settings
                    foreach (ProductResource rp in resources)
                    {
                        try
                        {
                            //add new resource to P-trade part if it doesn't exist
                            rp.StoreID = part.StoreID;
                            rp.SProductID = part.SProductID;
                            part.ProductResources.Add(rp);
                            actingContext.ProductResources.AddObject(rp);
                        }
                        catch (Exception e)
                        {
                            errors.AppendLine(e.StackTrace);
                        }
                    }

                    actingContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    errors.AppendLine(part.SProductID + " Exception : " + ex.StackTrace);
                }
            }

            return errors.ToString();
        }

        /// <summary>
        /// This method is to return the most proper context for saving part changes
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private eStore3Entities6 getActingContext(Part part)
        {
            if (part.parthelper != null && part.parthelper.getContext() != null)
                return part.parthelper.getContext();
            else
                return context;
        }


        public bool save(Part _part, StringBuilder errorLog = null,bool?_existpart=null)
        {
            
            if(_existpart==null)
              _existpart = (from _p in context.Parts.OfType<Part>()
                              where _p.StoreID == _part.StoreID && _p.SProductID == _part.SProductID
                              select _p).Any();

            try
            {
                if (!_existpart.GetValueOrDefault())
                {
                    context.AddObject("Parts", _part);
                    context.SaveChanges();
                }
                else
                {
                    eStore3Entities6 actingContext = getActingContext(_part);
                    actingContext.Parts.ApplyCurrentValues(_part);
                    actingContext.SaveChanges();
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error:" + _part.SProductID + ex.InnerException);
                eStore.Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                if (errorLog != null)
                    errorLog.AppendLine(ex.StackTrace);
                return false;
            }

        }

        private bool save(Product _product)
        {
            bool  _existproduct = (from _p in context.Parts.OfType<Product>()
                                 where _p.StoreID == _product.StoreID && _p.SProductID == _product.SProductID
                                 select _p).Any();

            try
            {
                if (_existproduct)
                {
                    var _existpart = (from _p in context.Parts
                                      where _p.StoreID == _product.StoreID && _p.SProductID == _product.SProductID
                                      select _p).FirstOrDefault();

                    //  context.ObjectStateManager.ChangeObjectState(_existpart, EntityState.Modified);
                    if (_existpart != null)
                    {
                        context.DeleteObject(_existpart);
                        context.SaveChanges();
                    }

                    context.Parts.AddObject(_product);
                    context.SaveChanges();
                }
                else
                {
                    eStore3Entities6 actingContext = getActingContext(_product);
                    actingContext.Parts.ApplyCurrentValues(_product);
                    actingContext.SaveChanges();
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(_product.SProductID + ":" + ex.InnerException);
                eStore.Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return false;
                // throw ex;
            }

        }


        private bool validateURL(string url)
        {
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(url);

                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();
                if (HttpStatusCode.OK == rspFP.StatusCode && rspFP.ContentLength > 0)
                {
                    // HTTP = 200 - Internet connection available, server online
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // Other status - Server or connection not available
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException ex)
            {
                // Exception - connection not available
                eStore.Utilities.eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return false;
                
            }

        }


        /// <summary>
        /// Sync whole store price with sending 100 partno each time, or only sync those partno which start with partprefix.
        /// </summary>
        /// <param name="_store"></param>
        /// <param name="partprefix"></param>

        public String SyncPrice(Store _store, string partprefix=null)
        {
            DateTime now = DateTime.Now.Date;
            StringBuilder errors = new StringBuilder();
            List<Part> _parts = new List<Part>();
            string[] sapvalidarray = getValidSAPStatus(_store);
            if (!string.IsNullOrWhiteSpace(partprefix))
                _parts = (from p in context.Parts
                          where p.StoreID == _store.StoreID
                          && p.PriceType.ToUpper() != "RATE"
                          && (string.IsNullOrEmpty(p.StockStatus) || sapvalidarray.Contains(p.StockStatus.ToUpper()))
                          && p.SProductID.StartsWith(partprefix)
                          select p).ToList<Part>();
            else
                _parts = (from p in context.Parts
                          where p.StoreID == _store.StoreID
                          && p.PriceType.ToUpper() != "RATE"
                           && (string.IsNullOrEmpty(p.StockStatus) || sapvalidarray.Contains(p.StockStatus.ToUpper()))
                          select p).ToList<Part>();

            if (_parts != null && _parts.Count() > 0)
            {
                //StringBuilder partnos = new StringBuilder("IPC-630MB-30ZE;");
                int totalcnt = _parts.Count();
                int _run_cnt = 40;
                int needtosend = (totalcnt / _run_cnt) + 1;

                int cnt = 0;
                int senttimes = 0;
                List<Part> sendparts = new List<Part>();
                foreach (Part p in _parts.ToList())
                {
                    if (!(p is Product_Ctos) && !(p is Product_Bundle) && !p.isInactive)
                    {
                        sendparts.Add(p);
                        //partnos.Append(p.SProductID.Trim().ToUpper() + ";");
                        cnt = cnt + 1;

                        if (cnt == _run_cnt || needtosend <= senttimes || totalcnt == cnt)
                        {
                            String errorMsg = null;

                            errorMsg = syncPrice(_store, sendparts);

                            if (!String.IsNullOrWhiteSpace(errorMsg))
                                errors.Append(errorMsg);

                            cnt = 0;
                            sendparts.Clear();
                            senttimes = senttimes + 1;
                            //partnos = new StringBuilder("IPC-630MB-30ZE;");
                        }
                    }
                }

            }

            return errors.ToString();
        }

        /// <summary>
        /// This method is the sync part's price from SAP.  Depending on store, some store may sync price from MyAdvantech
        /// interface, some sync price from other util.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="syncParts"></param>
        /// <param name="saveUpdate"></param>
        /// <returns></returns>
        public String syncPrice(Store store, List<Part> syncParts, Boolean saveUpdate = true)
        {
            String errorMsg = null;
            try
            {
                //if (store.StoreID == "AUS" )
                //    errorMsg = SynSAPPriceMyAdvantech(store, syncParts, saveUpdate);
                //else
                if (store.StoreID == "ABR")
                    errorMsg = SynSAPPriceMySAPDAL(store, syncParts, saveUpdate, true);//for abr price only, include tax in list price
                else if (store.StoreID == "ATH")
                    errorMsg = SynSAPPriceMySAPDAL(store, syncParts, saveUpdate); // sync ath store part price
                else if (store.StoreID == "ACN")
                {

                    errorMsg = SynSAPPriceMySAPDAL(store, syncParts.Where(x => x.isEAProduct()).ToList(), saveUpdate, false, "C100077");
                    errorMsg += SynSAPPriceMySAPDAL(store, syncParts.Where(x => x.isEAProduct() == false).ToList(), saveUpdate, false, "");
                }
                else if (store.StoreID == "ABB")
                {
                    errorMsg = syncBBPrice(store, syncParts, false);//正式上線之後移除true
                }
                else
                    errorMsg = syncSAPPrice(store, syncParts, saveUpdate);
            }
            catch (Exception e)
            {
                errorMsg = e.StackTrace;
            }

            return errorMsg;
        }

        /// <summary>
        /// This method is to sync product price from another store.  Some store like ATH, ASC and AMX don't have their pricing scheme.  Indeed, their store price
        /// will be original from other stores.  ATH product price will be from SAP.  ASC and AMX product price shall be from AUS.  Though price may origin from
        /// other store, but store many have different currency.  Therefore product price shall be converted to local currency if currency is different.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="sourceStoreID"></param>
        /// <param name="syncParts"></param>
        /// <param name="saveUpdate"></param>
        /// <returns></returns>
        private String copyPriceFromAnotherStore(Store store, string sourceStoreID ,List<Part> syncParts, Boolean saveUpdate = true)
        {
            StringBuilder errors = new StringBuilder();

            Store sourcStore = (new StoreHelper()).getStorebyStoreid(sourceStoreID);
            PartHelper helper = new PartHelper();
            List<Part> sourceparts = helper.prefetchPartList(sourceStoreID, string.Join(",", syncParts.Select(x => x.SProductID).ToArray()));

            //pricesourcestore.defaultCurrency

            List<MySAPDALPriceAdapter.MySAPDALResult> pricelist = new List<MySAPDALPriceAdapter.MySAPDALResult>();
            if (sourceparts.Any())
            {
                foreach(Part part in sourceparts)
                {
                    string partn = part.SProductID.Trim().ToUpper();
                    decimal price = sourcStore.getCurrencyExchangeValue(part.VendorSuggestedPrice.GetValueOrDefault(), store.defaultCurrency);

                    try
                    {
                        if (!pricelist.Any(c => c.ProductID.Equals(partn, StringComparison.OrdinalIgnoreCase)))
                        {
                            pricelist.Add(new MySAPDALPriceAdapter.MySAPDALResult(partn
                                , price
                                , 0
                                , 0
                                , 0
                                , false));
                        }
                    }
                    catch (Exception)
                    {
                        errors.AppendLine(String.Format("Price sync fails at part {0}", partn));
                    }
                }
            }

            String updateError = updateprice(store, syncParts, pricelist, saveUpdate);
            if (!String.IsNullOrWhiteSpace(updateError))
                errors.AppendLine();
            return string.Empty;
        }

        /// <summary>
        /// Action function, connect to SAP to get price
        /// </summary>
        /// <param name="strPartNOList"></param>
        /// <param name="_store"></param>
        /// <param name="sendparts"></param>
        //private void SynSAPPrice(string strPartNOList, Store _store, List<Part> sendparts)
        private String syncSAPPrice(Store store, List<Part> syncParts, Boolean saveUpdate = true)
        {
            //string[] arrPartNO;
            StringBuilder errors = new StringBuilder();
            string xmlin;
            string xmlout;
            int iRet = 0;

            AdvantechSAPPrice.eBizAEU_WS wsprice = new AdvantechSAPPrice.eBizAEU_WS();

            //arrPartNO = strPartNOList.Split(';');
            xmlin = "<NewDataSet>";

            //add an additional entry to make sure the leading part no in the query is an existing part in SAP
            xmlin = xmlin + "<Table1>" + "<Matnr>IPC-630MB-30ZE</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
            //for (int i = 0; i <= arrPartNO.Length - 1; i++)
            foreach (Part part in syncParts)
            {
                //xmlin = xmlin + "<Table1>" + "<Matnr>" + arrPartNO[i] + "</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
                xmlin = xmlin + "<Table1>" + "<Matnr>" + part.SProductID + "</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
            }

            xmlin = xmlin + "</NewDataSet>";
            try
            {
                //if(_store.StoreID =="AKR")
                //    iRet = wsprice.eBizAEU_GetKRMultiPrice ("168", getSetting(_store, "PriceSAPOrg"), getSetting(_store, "PriceSAPLvlL1"), xmlin, out xmlout);
                //else
                iRet = wsprice.eBizAEU_GetMultiPrice("168", getSetting(store, "PriceSAPOrg"), getSetting(store, "PriceSAPLvlL1"), xmlin, out xmlout);

                DataSet oDsWrap = new DataSet();
                System.IO.StringReader sr = new System.IO.StringReader(xmlout);
                oDsWrap.ReadXml(sr);

                List<MySAPDALPriceAdapter.MySAPDALResult> pricelist = new List<MySAPDALPriceAdapter.MySAPDALResult>();
                if (oDsWrap.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow d in oDsWrap.Tables[0].Rows)
                    {
                        string partn = d["Matnr"].ToString().ToUpper();
                        decimal price = decimal.Parse(d["Netwr"].ToString());

                        try
                        {
                            if (!pricelist.Any(c => c.ProductID.Equals(partn, StringComparison.OrdinalIgnoreCase)))
                            {
                                pricelist.Add(new MySAPDALPriceAdapter.MySAPDALResult(partn
                                    , price
                                    , 0
                                    , 0
                                    , 0
                                    , false));
                            }
                        }
                        catch (Exception)
                        {
                            errors.AppendLine(String.Format("Price sync fails at part {0}", partn));
                        }
                    }
                }

                String updateError = updateprice(store,syncParts, pricelist, saveUpdate);
                if (!String.IsNullOrWhiteSpace(updateError))
                    errors.AppendLine();
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error(ex.Message, "", "", store.StoreID, ex);
                errors.AppendLine(String.Format("syncSAPPrice fails.  Exception : {0}", ex.StackTrace));
            }

            return errors.ToString();
        }

        private string syncBBPrice(Store store, List<Part> syncParts, bool isTesting = false)
        {
            MySAPDAL.MYSAPDAL dal = new MySAPDAL.MYSAPDAL();
            if (isTesting == true)
                dal.Url = "http://aclecampaign2:4002/services/MYSAPDAL.asmx";
            MySAPDAL.SAPDALDS.ProductInDataTable dtIn = new MySAPDAL.SAPDALDS.ProductInDataTable();
            MySAPDAL.SAPDALDS.ProductOutDataTable dtOut = new MySAPDAL.SAPDALDS.ProductOutDataTable();
            string errMsg = string.Empty;
            //List<string> ausPartNo = new List<string>();
            foreach (Part p in syncParts)
            {
                //Use US10 to sync all price
                //var isAUSStandardProduct = p.CheckMaterialGroup(store.Settings["BBMaterialGroup"]);
                //if (isAUSStandardProduct == true)
                //{
                //    var isBBproduct = p.CheckProductLine(store.Settings["BBProductLine"]);
                //    if (isBBproduct == false)
                //    {
                //        ausPartNo.Add(p.SProductID);
                //        p.isBBproduct(store.Settings["BBMaterialGroup"], store.Settings["BBProductLine"]);
                //        continue;
                //    }
                //}
                dtIn.AddProductInRow(p.SProductID, 1);
            }
            bool result = dal.GetPrice(store.Settings["PriceSAPLvlL1"], store.Settings["PriceSAPLvlL1"], store.Settings["PriceSAPOrg"], dtIn, ref dtOut, ref errMsg);

            StringBuilder sb = new StringBuilder();
            if (result == true)
            {
                foreach (MySAPDAL.SAPDALDS.ProductOutRow dr in dtOut.Rows)
                {
                    var p = (new PartHelper()).getPart(dr.PART_NO, store);
                    //var p = syncParts.Where(x => x.SProductID.Trim().ToUpper().Equals(dr.PART_NO.Trim().ToUpper(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (p != null)
                    {
                        bool updateddata = false;
                        decimal newprice = 0;
                        decimal.TryParse(dr.UNIT_PRICE, out newprice);
                        if (newprice > 0 && newprice != p.VendorSuggestedPrice)
                        {
                            p.VendorSuggestedPrice = newprice;
                            p.hasProductPriceChanged = true;
                            updateddata = true;
                        }

                        decimal recyclefee = 0;
                        decimal.TryParse(dr.RECYCLE_FEE, out recyclefee);
                        if (recyclefee > 0 && p.RecycleFee != recyclefee)
                        {
                            p.RecycleFee = recyclefee;
                            updateddata = true;
                        }

                        if (p is Product && ((Product)p).priceSourceX != Part.PRICESOURCE.LOCAL)
                        {
                            var storeprice = eStore.Utilities.Converter.round(p.VendorSuggestedPrice.GetValueOrDefault(), store.productRoundingUnit, true);
                            if (storeprice != ((Product)p).StorePrice)
                            {
                                ((Product)p).StorePrice = storeprice;
                                updateddata = true;
                            }
                        }
                        //store.fixSpecialProductPrice(p, ref PriceChanged);

                        //Update cost and currency
                        SAPProduct sapproduct = getSAPProduct(p.SProductID, store);
                        if (sapproduct != null)
                        {
                            decimal cost = sapproduct.Cost.HasValue ? sapproduct.Cost.Value : 0;
                            string currency = sapproduct.Currency;

                            if (cost > 0 && p.Cost != cost)
                            {
                                p.Cost = cost;
                                updateddata = true;
                            }

                            if (!string.IsNullOrEmpty(currency) && p.Currency != currency)
                            {
                                p.Currency = currency;
                                updateddata = true;
                            }
                        }

                        if (updateddata == true)
                        {
                            p.LastUpdated = DateTime.Now;
                            eStore3Entities6 actingContext = getActingContext(p);
                            actingContext.Parts.ApplyCurrentValues(p);
                            actingContext.SaveChanges();
                        }
                    }
                    else
                    {
                        sb.AppendFormat("Part No: {0} does not exist.<br />", dr.PART_NO.Trim().ToUpper());
                    }
                }
            }
            else
                sb.Append("Sync B+B price error, " + errMsg);

            //Sync AUS part
            //if (ausPartNo.Count > 0)
            //{
            //    Store aus = (new StoreHelper()).getStorebyStoreid("AUS");
            //    if (aus != null)
            //    {
            //        foreach (var pn in ausPartNo)
            //        {
            //            List<Part> ps = (new PartHelper()).getActiveParts(store, pn);
            //            string ausmsg = syncSAPPrice(aus, ps, true);
            //            if (!string.IsNullOrEmpty(ausmsg))
            //                sb.AppendFormat("<br />Sync Advantech product in B+B store error: Part No: {0}, {1}", pn, ausmsg);
            //        }
            //    }
            //}

            return sb.ToString();
        }

        public List<MySAPDALPriceAdapter.MySAPDALResult> getSAPPriceByCustomerID(string org, string customer, List<string> parts)
        {
            //string[] arrPartNO;
            StringBuilder errors = new StringBuilder();
            string xmlin;
            string xmlout;
            int iRet = 0;

            AdvantechSAPPrice.eBizAEU_WS wsprice = new AdvantechSAPPrice.eBizAEU_WS();

            //arrPartNO = strPartNOList.Split(';');
            xmlin = "<NewDataSet>";

            //add an additional entry to make sure the leading part no in the query is an existing part in SAP
            xmlin = xmlin + "<Table1>" + "<Matnr>IPC-630MB-30ZE</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
            //for (int i = 0; i <= arrPartNO.Length - 1; i++)
            foreach (string  part in parts)
            {
                //xmlin = xmlin + "<Table1>" + "<Matnr>" + arrPartNO[i] + "</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
                xmlin = xmlin + "<Table1>" + "<Matnr>" + part + "</Matnr>" + " <Mglme>1</Mglme>" + "</Table1>";
            }

            xmlin = xmlin + "</NewDataSet>";
            try
            {
                //if(_store.StoreID =="AKR")
                //    iRet = wsprice.eBizAEU_GetKRMultiPrice ("168", getSetting(_store, "PriceSAPOrg"), getSetting(_store, "PriceSAPLvlL1"), xmlin, out xmlout);
                //else
                iRet = wsprice.eBizAEU_GetMultiPrice("168", org , customer, xmlin, out xmlout);
                DataSet oDsWrap = new DataSet();
                System.IO.StringReader sr = new System.IO.StringReader(xmlout);
                oDsWrap.ReadXml(sr);

                List<MySAPDALPriceAdapter.MySAPDALResult> pricelist = new List<MySAPDALPriceAdapter.MySAPDALResult>();
                if (oDsWrap.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow d in oDsWrap.Tables[0].Rows)
                    {
                        string partn = d["Matnr"].ToString();
                        decimal price = decimal.Parse(d["Netwr"].ToString());

                        try
                        {
                            if (!pricelist.Any(c => c.ProductID.Equals(partn, StringComparison.OrdinalIgnoreCase)))
                            {
                                pricelist.Add(new MySAPDALPriceAdapter.MySAPDALResult() 
                                {
                                    ProductID = partn,
                                    ListPrice = price,
                                    UnitPrice = 0
                                });
                            }

                        }
                        catch (Exception)
                        {
                            errors.AppendLine(String.Format("Price sync fails at part {0}", partn));
                        }
                    }
                }
                return pricelist;

            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error(ex.Message, "", "", customer, ex);
                errors.AppendLine(String.Format("syncSAPPrice fails.  Exception : {0}", ex.StackTrace));
            }

            return new List<MySAPDALPriceAdapter.MySAPDALResult>();
        }


        /// <summary>
        /// Get price from MyAdvantech webservice, which can separate price and recyclefees
        /// Save to Parts after retrieve price.
        /// </summary>
        /// <param name="strPartNOList"></param>
        /// <param name="_store"></param>
        /// <param name="sendparts"></param>

        //private void SynSAPPriceMyAdvantech(string strPartNOList, Store _store, List<Part> sendparts)
        private String SynSAPPriceMyAdvantech(Store _store, List<Part> syncParts, Boolean saveUpdate = true)
        {
            List<MySAPDALPriceAdapter.MySAPDALResult> pricelist = new List<MySAPDALPriceAdapter.MySAPDALResult>();

            AdvantechSAPPriceMy.eBizAEU_WS ws = new AdvantechSAPPriceMy.eBizAEU_WS();
            string errormsg = "";
            DataTable dtin = new DataTable("Productin");
            DataTable dtout = new DataTable("Productout");

            dtin.Columns.Add("PartNo");
            dtin.Columns.Add("Qty");

            //string[] arrPartNO = strPartNOList.Split(';');

            //for (int i = 0; i <= arrPartNO.Length - 1; i++) { 
            dtin.Rows.Add("IPC-630MB-30ZE", 1);     //sample part
            foreach (Part part in syncParts)
            {
                if (!string.IsNullOrEmpty(part.SProductID))
                    dtin.Rows.Add(part.SProductID, 1);
            }

            dtout = ws.GetMultiPrice(getSetting(_store, "PriceSAPOrg"), getSetting(_store, "PriceSAPLvlL1"), dtin, ref errormsg);

            foreach (DataRow dr in dtout.Rows)
            {
                try
                {
                    String partNo = (String)dr["PartNo"];
                    Console.WriteLine("{0}:{1}:{2}", partNo, dr["NetPrice"], dr["RecycleFee"]);
                    if (!pricelist.Any(c => c.ProductID.Equals(partNo, StringComparison.OrdinalIgnoreCase)))
                    {
                        pricelist.Add(new MySAPDALPriceAdapter.MySAPDALResult(partNo
                                , Decimal.Parse(dr["NetPrice"].ToString())
                                , 0
                                , Decimal.Parse(dr["RecycleFee"].ToString())
                                , 0
                                , false));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    errormsg += " Exception : " + e.StackTrace;
                }
            }

            String updateError = updateprice(_store,syncParts, pricelist, saveUpdate);
            if (!String.IsNullOrWhiteSpace(errormsg) && !String.IsNullOrWhiteSpace(updateError))
                errormsg += updateError;

            return errormsg;
        }

        private String SynSAPPriceMySAPDAL(Store _store, List<Part> syncParts, Boolean saveUpdate = true, bool includeTax = false, string PriceSAPLvl="")
        {
            MySAPDALPriceAdapter adpater = new MySAPDALPriceAdapter();
            adpater.PriceSAPLvl = PriceSAPLvl;
            adpater.includeTax = includeTax;
            List<MySAPDALPriceAdapter.MySAPDALResult> reslut = adpater.getSAPPrice(_store, syncParts);
            String updateError = updateprice(_store, syncParts, reslut, saveUpdate);
           
            return adpater.ErrorMessage + updateError;
        }

        /// <summary>
        /// This method is to update part's listing price based on input new prices.  If saveUpdate is true, new update will
        /// be synced and be saved to DB too.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="pricelist"></param>
        /// <param name="saveUpdate"></param>
        /// <returns></returns>
        private String updateprice(Store _store, List<Part> parts, List<MySAPDALPriceAdapter.MySAPDALResult> pricelist, Boolean saveUpdate = true)
        {
            StringBuilder errors = new StringBuilder();
            decimal adjustrate = _store.getDecimalSetting("SAPPriceAdjustRate",1m);
            bool PriceChanged = false;
            bool CostChanged = false;

            bool IsCostFromSAP = true;//Cost是否走SAPProduct 还是走webservice
            DataTable costTable = null;
            //生产和出货不是同一个org，cost就走webservice
            if (getSetting(_store, "PriceSAPOrg") != getSetting(_store, "ProductLogisticsOrg"))
            {
                IsCostFromSAP = false;
                try
                {
                    eStore.POCOS.MySAPDAL.MYSAPDAL mysapdal = new MySAPDAL.MYSAPDAL();
                    string[] costPartList = new string[parts.Count];
                    for (int i = 0; i < parts.Count; i++)
                    {
                        costPartList[i] = parts[i].SProductID;
                    }
                    //根据SproductId 和OrgId 获取Cost,Currency
                    costTable = mysapdal.GetSAPPNCost(costPartList, getSetting(_store, "PriceSAPOrg"));
                }
                catch (Exception ex)
                {
                    errors.AppendLine("GetSAPPNCost exception : " + ex.StackTrace);
                }
            }
            

            foreach (Part p in parts.ToList())
            {
                PriceChanged = false; CostChanged = false;
                decimal updateCost = 0; string updateCurrency = string.Empty;
                try
                {
                    if (string.IsNullOrEmpty(p.PriceType) || p.PriceType.ToUpper() != "RATE")
                    {
                        //for unknown reason, some of SProductId has additional space at its end
                        String productID = p.SProductID.Trim().ToUpper();
                        var pro = pricelist.FirstOrDefault(c=>c.ProductID.Equals(p.SProductID.Trim(), StringComparison.OrdinalIgnoreCase));
                        if (pro != null)
                        {
                            decimal newprice = pro.includeTax ? pro.ListPrice += pro.Tax : pro.ListPrice;
                            if (newprice > 0)
                            {
                                //check if price is recently updated
                                //if (newprice != p.VendorSuggestedPrice)
                                {
                                    if (p.VendorSuggestedPrice != newprice)
                                    {
                                        PriceChanged = true;
                                        p.VendorSuggestedPrice = newprice;
                                        p.hasProductPriceChanged = true;

                                        if (adjustrate != 1m && adjustrate > 0)
                                        {
                                            p.VendorSuggestedPrice = newprice * adjustrate;
                                        }

                                        //reverse special handling of ASC price, the price used to be set as "local"
                                        if (p.StoreID.Equals("ASC"))
                                        {
                                            p.PriceSourceProvider = "SAP";
                                            p.LocalPrice = p.VendorSuggestedPrice.GetValueOrDefault();

                                            if (p is Product)
                                            {
                                                ((Product)p).StorePrice = p.LocalPrice.GetValueOrDefault();
                                                ((Product)p).PriceSource = "VENDOR";
                                            }
                                        }
                                        
                                    }

                                    if (pro != null && p.RecycleFee != pro.RecyleFee)
                                    {
                                        p.RecycleFee = pro.RecyleFee;
                                        PriceChanged = true;
                                    }

                                    if (p is Product && ((Product)p).priceSourceX != Part.PRICESOURCE.LOCAL)
                                    {
                                        var storeprice = eStore.Utilities.Converter.round(p.VendorSuggestedPrice.GetValueOrDefault(), _store.productRoundingUnit,true);
                                        if (storeprice != ((Product)p).StorePrice)
                                        {
                                            ((Product)p).StorePrice = storeprice;
                                            PriceChanged = true;
                                        }
                                    }
                                    _store.fixSpecialProductPrice(p, ref PriceChanged);
                                }
                            }
                            else
                            {
                                Console.WriteLine(p.SProductID + " has 0 price, " + newprice);
                                errors.AppendLine(p.SProductID + " has 0 price, " + newprice);
                            }
                        }
                        else
                        {
                            Console.WriteLine(p.SProductID + " has no price returned from SAP");
                            errors.AppendLine(p.SProductID + " has no price returned from SAP");
                        }
                    }
                    else
                    {
                        Console.WriteLine(p.SProductID + " has price update problem.");
                        errors.AppendLine(p.SProductID + " has price update problem.");
                    }

                    if (IsCostFromSAP)
                    {
                        POCOS.SAPProduct sapProduct = getSAPProduct(p.SProductID, _store);
                        if (sapProduct != null)
                        {
                            updateCost = sapProduct.Cost.HasValue ? sapProduct.Cost.Value : 0;
                            updateCurrency = sapProduct.Currency;
                        }
                    }
                    else
                    {
                        if (costTable != null && costTable.Rows.Count > 0)
                        {
                            DataRow partRow = costTable.Select("PART_NO = '" + p.SProductID + "'").FirstOrDefault();
                            if (partRow != null && partRow["COST"] != null && partRow["CURRENCY"] != null)
                            {
                                updateCost = decimal.Parse(partRow["COST"].ToString());
                                updateCurrency = partRow["CURRENCY"].ToString();
                            }
                        }
                    }

                    if (!p.Cost.HasValue)
                    {
                        p.Cost = 0;
                        CostChanged = true;
                    }

                    if (updateCost > 0 && p.Cost != updateCost)
                    {
                        p.Cost = updateCost;
                        CostChanged = true;
                    }

                    if (!string.IsNullOrEmpty(updateCurrency) && p.Currency != updateCurrency)
                    {
                        p.Currency = updateCurrency;
                        CostChanged = true;
                    }

                    if (p.Currency != _store.defaultCurrency.CurrencyID && p.Cost.GetValueOrDefault() > 0)
                    {
                        //The logistics site has different currency from its online store.  Cost needs to be converted to local currency
                        Decimal cost = _store.getCurrencyExchangeValue(p.Cost.GetValueOrDefault(), p.Currency, _store.defaultCurrency.CurrencyID);
                        if (cost > 0)
                        {
                            p.Cost = cost;
                            p.Currency = _store.defaultCurrency.CurrencyID;
                            CostChanged = true;
                        }
                    }

                    if (PriceChanged || CostChanged)
                    {
                        p.LastUpdated = DateTime.Now;
                        if (saveUpdate == true)
                        {
                            eStore3Entities6 actingContext = getActingContext(p);
                            actingContext.Parts.ApplyCurrentValues(p);
                            actingContext.SaveChanges();
                            //save(p, errors);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(p.SProductID + " syncPrice exception : " + e.StackTrace);
                    errors.AppendLine(p.SProductID + " syncPrice exception : " + e.StackTrace);
                    //throw e;
                }

            }

            return errors.ToString();
        }

        /*
        private bool isPtrade(string partno)
        {
            if (partno.StartsWith("96"))
                return true;
            else
                return false;
        }

        * */

        private void printDatasetColumn(DataSet ds)
        {

            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                Console.WriteLine(ds.Tables[0].Columns[i].ColumnName);

            }
        }

        private static string myclassname()
        {
            return typeof(PISSync).ToString();
        }

        /// <summary>
        /// Return valid SAP status
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private string[] getValidSAPStatus(Store store)
        {
            string validproductstatus = store.getStringSetting("ProductLogisticsStatus");
            string[] validarray = null;
            if (string.IsNullOrEmpty(validproductstatus) == false)
            {
                validproductstatus = validproductstatus.ToUpper();
                validarray = validproductstatus.Split(',');
            }

            return validarray;
        }

        /// <summary>
        /// return valid Product status
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        private string[] getValidProductsStatus(Store store)
        {
            string validproductstatus = store.getStringSetting("ValidProductStatus");

            string[] validarray = new string[10];
            if (string.IsNullOrEmpty(validproductstatus) == false)
            {
                validarray = validproductstatus.Split(',');
            }

            return validarray;
        }


        /// <summary>
        /// This method is to construct product for AOnline Solution only
        /// </summary>
        /// <param name="partno"></param>
        /// <param name="_store"></param>
        /// <param name="isrelatedpart"></param>
        /// <returns></returns>
        public Product constructSolutionProduct(string partno, Store _store)
        {
            Part newPart = new Part();
            newPart.StoreID = _store.StoreID;
            newPart.SProductID = partno;

            return constructSolutionProduct(newPart, _store);
        }

        /// <summary>
        /// This method shall be invoked for constructing new product only
        /// </summary>
        /// <param name="part"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        public Product constructSolutionProduct(Part part, Store store)
        {
            Product newProduct = new Product(part);
            //newProduct.parthelper = null;
            newProduct.CreatedDate = DateTime.Now;
            newProduct.CreatedBy = "PISSync";
            newProduct.LastUpdatedBy = "Sync Job";
            newProduct.LastUpdated = DateTime.Now;
            newProduct.status = Product.PRODUCTSTATUS.SOLUTION_ONLY;
            newProduct.DisplayPartno = part.SProductID;
            newProduct.VendorSuggestedPrice = 0;

            SyncPIS(newProduct, store, true);
            //if (newProduct.isPTradePart())
            //    getPAPSDetail(newProduct);

            return newProduct;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelNo"></param>
        /// <returns></returns>
        public List<PAP> getPapsByModelNo(string modelNo)
        {
            if (string.IsNullOrEmpty(modelNo))
                return new List<PAP>();

            var ls = (from p in pis.PAPS
                      where p.Board_Model_Name == modelNo && p.Cable_Part_No != "No support"
                      select p).ToList();
            return ls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partNo"></param>
        /// <returns></returns>
        public List<V_Spec_V2> getVSpec()
        {            
            List<string> types = new List<string>(){"Size","Brightness","Touch Screen"};

            var ls = (from v in pis.V_Spec_V2
                      where types.Contains(v.AttrName)
                      select v).ToList();
            return ls;
        }

        //检查modelName 是否存在pis
        public bool validateModel(string modelName)
        {
            string sql = "Select m.Model_name as DISPLAY_NAME  from  model m left join Model_Publish mpu on mpu.model_name =m.model_name and mpu.Site_ID ='ACL' " +
                           " where m.MODEL_NAME =@ModelName AND (mpu.Publish_Status <>'Pre-release' OR mpu.Publish_Status  IS NULL) and mpu.Active_FLG ='Y'";
            string result = pcontext.ExecuteStoreQuery<string>(sql, (new System.Data.SqlClient.SqlParameter("@ModelName", modelName))).FirstOrDefault(); ;

            return !string.IsNullOrEmpty(result);
            //现在是根据model 获取feature. 没上面那个准
            //sp_GetProductFeature_Result result = pcontext.sp_GetProductFeature(modelName, lang).FirstOrDefault();
            //return result != null;
        }

        /// <summary>
        /// Check part No. is B+B part No.
        /// </summary>
        /// <param name="pn"></param>
        /// <returns></returns>
        //private bool isBBPartNo(string pn)
        //{
        //    if (_bbPartNo == null)
        //    {
        //        _bbPartNo = (from x in pcontext.sp_GetBBAllPartNo()
        //                     select x.part_no.ToUpper().Trim()).ToList();
        //    }
        //    return _bbPartNo.Contains(pn.ToUpper());
        //}

        private string GetBBeSotreProductDatasheet(Part part)
        {
            try
            {
                if (string.IsNullOrEmpty(pisADOConnectionString))
                {
                    PISEntities aclpis = new PISEntities();
                    System.Data.EntityClient.EntityConnection ec = aclpis.Connection as System.Data.EntityClient.EntityConnection;
                    pisADOConnectionString = ec.StoreConnection.ConnectionString;
                }
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(pisADOConnectionString))
                {
                    con.Open();

                    if (con.State == System.Data.ConnectionState.Closed)
                        con.Open();

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(string.Format("SELECT TOP 1 Datasheets FROM [PIS].[dbo].[v_datasheet] where productNo='{0}' and itemtype='Part' and LANG='ENU' order by Created desc", part.SProductID), con);
                    cmd.CommandType = CommandType.Text;
                    object ds = cmd.ExecuteScalar();
                    if (ds != null)
                        return ds.ToString();
                }
            }
            catch
            { }
            return string.Empty;
        }
    }

    /// <summary>
    /// PISModel provides a central place of retrieving PIS model information
    /// </summary>
    public class PISModel
    {
        private const string catalogid1 = "1-2MLAX2";
        private const string catalogid2 = "1-2JKBQD";
        private const String defaultLang = "ENU";
        public String categoryID;
        public String modelName;
        public String partNo;
        private PISEntities pisContext = null;
        AdvantechPIS.PISProductWS pisws;
        //index with language key
        private Dictionary<String, String> datasheets = new Dictionary<String, String>();
        //index with language key
        private Dictionary<String, String> productFeatures = new Dictionary<String, String>();
        //index with language key
        private Dictionary<String, sp_GetModelInfo_estore_Result> modelInfos = new Dictionary<string,sp_GetModelInfo_estore_Result>();
        private List<sp_GetLiteratureTable_Result> _literatures = null;

        public PISModel(spGetModelByPN_estore_Result model, PISEntities pContext, AdvantechPIS.PISProductWS pisws)
        {
            if (model != null)
            {
                this.categoryID = model.CATEGORY_ID;
                this.modelName = model.DISPLAY_NAME;
                this.partNo = model.PART_NO;
            }
            else
            {
                this.categoryID = "";
                this.modelName = "";
                this.partNo = "";
            }
            this.pisContext = pContext;
            this.pisws = pisws;
        }

        /// <summary>
        /// This method indicates if current PISModel has model value or not
        /// </summary>
        /// <returns></returns>
        public bool isBlankModel()
        {
            return String.IsNullOrEmpty(modelName);
        }

        /// <summary>
        /// This method returns product literature in a particular language.  If there is nothing found, it return English
        /// literature instead.
        /// </summary>
        private List<sp_GetLiteratureTable_Result> getProductLiteratures(String lang)
        {
            if (_literatures == null)
            {
                sp_GetModelInfo_estore_Result modelInfo = getModelInfo(lang);

                _literatures = (from x in pisContext.sp_GetLiteratureTable(modelInfo.product_id)
                                orderby x.LAST_UPDATED descending
                                select x).ToList();
            }
            return _literatures;
        }


        public sp_GetModelInfo_estore_Result getModelInfo(String lang)
        {
            sp_GetModelInfo_estore_Result modelInfo = retrieveLocalizedModelInfo(lang);
            //if exist return existing feature
            if (modelInfo == null)
                modelInfo = retrieveLocalizedModelInfo(defaultLang);

            return modelInfo;
        }

        /// <summary>
        /// This method is to retrieve product feature in a particular language. 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private sp_GetModelInfo_estore_Result retrieveLocalizedModelInfo(String lang)
        {
            sp_GetModelInfo_estore_Result modelInfo = null;
            if (modelInfos.ContainsKey(lang))
                modelInfo = modelInfos[lang];
            else
            {
                //try to retrieve product feature from PIS
                modelInfo = pisContext.sp_GetModelInfo_estore(this.categoryID, lang).FirstOrDefault();

                if (modelInfo == null)
                    modelInfo = new sp_GetModelInfo_estore_Result();

                //cache before return
                modelInfos.Add(lang, modelInfo);
            }

            if (String.IsNullOrWhiteSpace(modelInfo.product_id))
                return null;
            else
                return modelInfo;
        }


        /// <summary>
        /// The method will return product feature in particular langugae.  If the localized feature is not available
        /// , it will return English feature instead.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public String getProductFeature(String lang)
        {
            String feature = retrieveLocalizedProductFeature(lang);
            //if exist return existing feature
            if (!String.IsNullOrWhiteSpace(feature))
                return feature;

            //try to retrieve default English version
            if (lang.Equals(defaultLang) == false)
                feature = retrieveLocalizedProductFeature(defaultLang);

            return feature;
        }

        /// <summary>
        /// This method is to retrieve product feature in a particular language. 
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private String retrieveLocalizedProductFeature(String lang)
        {
            String feature = null;
            if (productFeatures.ContainsKey(lang))
                feature = productFeatures[lang];
            else
            {
                //try to retrieve product feature from PIS
                feature = retrievePISProductFeature(lang);
                if (feature == null)
                    feature = "";

                //cache before return
                productFeatures.Add(lang, feature);
            }

            return feature;
        }

        /// <summary>
        /// This method is to retrieve product feature from PIS
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private String retrievePISProductFeature(String lang)
        {
            List<sp_GetProductFeature_Result> features = pisContext.sp_GetProductFeature(this.modelName, lang).OrderBy(x => x.FEATURE_SEQ).ToList();
            if (features == null)
                return null;

            StringBuilder productFeature = new StringBuilder();
            foreach (sp_GetProductFeature_Result pfs in features)
                productFeature.AppendFormat("<li>{0}</li>", pfs.FEATURE_DESC);

            return productFeature.ToString();
        }

        /// <summary>
        /// This method will compose 3D product resources if there is any
        /// </summary>
        /// <param name="_part"></param>
        /// <param name="_minfo"></param>
        /// <returns></returns>
        public List<ProductResource> get3DModelProductResource(String lang)
        {
            List<ProductResource> resources = new List<ProductResource>();
            sp_GetModelInfo_estore_Result modelInfo = getModelInfo(lang);
            List<sp_GetLiteratureTable_Result> literatures = getProductLiteratures(lang);
            CachePool cache = CachePool.getInstance();

            foreach (sp_GetLiteratureTable_Result li in literatures)
            {
                if (li.LIT_TYPE == "Product - Photo(3D)")
                {
                    ProductResource prs = new ProductResource();
                    prs.ResourceURL = "http://www.advantech.com/products/3dmodel_process.asp?Literature_ID="
                                        + li.LITERATURE_ID + "&model=" + modelInfo.DISPLAY_NAME + "&modelid=" + modelInfo.product_id + "&BU=" + "&filetype=" + li.FILE_EXT;
                    prs.ResourceType = "3DModel";
                    prs.ResourceName = (li.FILE_EXT == null) ? "" : li.FILE_EXT;
                    resources.Add(prs);

                }
                else if (li.LIT_TYPE == "Product - Photo(B)")
                {
                    ProductResource prs = new ProductResource();
                    try
                    {
                        //Object imageURLObj = cache.getObject(li.LITERATURE_ID);
                        //if (imageURLObj != null)
                        //    prs.ResourceURL = (String)imageURLObj;
                        //else
                        //{
                        //    String imageurl = pisws.GetPhysicalDownloadURL(li.LITERATURE_ID);
                        //    string filename = li.FILE_NAME + "." + li.FILE_EXT;
                        //    prs.ResourceURL = "http://downloadt.advantech.com/" + imageurl;
                        //}
                        prs.ResourceURL = "https://downloadssl.advantech.com/download/downloadlit.aspx?LIT_ID=" + li.LITERATURE_ID;
                        prs.ResourceType = "LargeImages";
                        prs.ResourceName = (li.FILE_EXT == null) ? "" : li.FILE_EXT;
                        resources.Add(prs);
                        cache.cacheObject(li.LITERATURE_ID, prs.ResourceURL);
                    }
                    catch (Exception)
                    {
                        //ignore and do nothing
                    }
                }
            }
            
            return resources;
        }


        /// <summary>
        /// This method is to retrieve product datasheet in specified language.  If no localized datasheet
        /// available, it returns English datasheet.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public String getDataSheetID(String lang)
        {
            String datasheetID = getLocalizedDataSheetID(lang);
            if (String.IsNullOrWhiteSpace(datasheetID))
                datasheetID = getLocalizedDataSheetID(defaultLang);

            return datasheetID;
        }

        /// <summary>
        /// This method is to retrieve datasheet in a particular language.
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private String getLocalizedDataSheetID(String lang)
        {
            if (datasheets.ContainsKey(lang))
                return (String)datasheets[lang];
            else
            {
                sp_GetModelInfo_estore_Result modelInfo = getModelInfo(lang);
                sp_GetLiteratureTableByLANG_Result literature = (from x in pisContext.sp_GetLiteratureTableByLANG(modelInfo.product_id, catalogid1, catalogid2, lang)
                                                                where x.LIT_TYPE.ToUpper() == "PRODUCT - DATASHEET"
                                                                orderby x.LAST_UPDATED descending
                                                                select x).FirstOrDefault();
                String datasheetID = "";
                if (literature != null)
                    datasheetID = literature.LITERATURE_ID;
                datasheets.Add(lang, datasheetID);

                return datasheetID;
            }
        }
    }

    [DataContract]
    public class ThreedmodelViewer
    {
        [DataMember]
        public string error { get; set; }
        [DataMember]
        public string urn { get; set; }
        [DataMember]
        public string threeDModelFile { get; set; }
        [DataMember]
        public string viewerURL { get; set; }
    }
}
