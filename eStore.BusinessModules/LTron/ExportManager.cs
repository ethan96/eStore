using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using eStore.POCOS;

namespace eStore.BusinessModules.LTron
{
    public class ExportManager
    {

        public class AdvantechProductsEDI
        {

            public List<Product> Products { get; set; }
        }
        public class AdvantechProductCategoriesEDI
        {
            public List<ProductCategory> ProductCategories
            { get; set; }

        }
        public class AdvantechProductEDI
        {
            public Product Product { get; set; }
        }
        public class AdvantechSystemEDI
        {
            public ConfigurationSystem ConfigurationSystem { get; set; }
        }
        public class AdvantechSystemsEDI
        {
            public List< ConfigurationSystem> ConfigurationSystems { get; set; }
        }
        public class AdvantechProductInfoEDI
        {
            public ProductInfo ProductInfo { get; set; }
        }


        public ExportManager() { }

        public ExportManager(BusinessModules.Store store)
        {
            this._store = store;
        }

        public void setStorebyIP(string ip)
        {
            _store = getStore("AUS", ip);
        }
        public BusinessModules.Store getStore(string storeid, string ip)
        {
            eStore.BusinessModules.StoreSolution soltion = eStore.BusinessModules.StoreSolution.getInstance();
            string countrycode = soltion.getCountryCodeByIp(ip);
            BusinessModules.Store candidateStore = null;
            if (!countrycode.Equals("XX"))   //non-internal IP
                candidateStore = soltion.locateStore(countrycode, true);
            if (candidateStore == null)
            {
                candidateStore = soltion.getStore(storeid);

            }
            if (candidateStore == null)
            {
                candidateStore = soltion.getStore("AUS");

            }
            return candidateStore;
        }
        private BusinessModules.Store _store;
        private BusinessModules.Store store
        {
            get
            {
                if (_store == null)
                {
                    StoreSolution solution = StoreSolution.getInstance();
                    _store = solution.getStore("AUS");
                }
                return _store;
            }
        }
        public string exportCategories(bool isSystemCategories=false)
        {
            try
            {
                StoreSolution solution = StoreSolution.getInstance();
                BusinessModules.Store store = solution.getStore("AUS");
                List<ProductCategory> ProductCategories = new List<ProductCategory>();

                if (isSystemCategories)
                {
                    foreach (eStore.POCOS.ProductCategory pc in store.getTopLevelCTOSProductCategories(null))
                    {
                        ProductCategories.Add(categoryadpter(pc));
                    }
                }
                else
                {
                    foreach (eStore.POCOS.ProductCategory pc in store.getTopLevelStandardProductCategories(null))
                    {
                        ProductCategories.Add(categoryadpter(pc));
                    }
                }
       

                AdvantechProductCategoriesEDI edi = new AdvantechProductCategoriesEDI();
                edi.ProductCategories = ProductCategories;

                return Serializer<AdvantechProductCategoriesEDI>(edi);
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }

        public string exportProducts(List<string> productids = null)
        {
            try
            {
                List<Product> Products = new List<Product>();
                AdvantechProductsEDI edi = new AdvantechProductsEDI();

                List<eStore.POCOS.Part> parts = new List<POCOS.Part>();
                if (productids == null)
                    parts = (new eStore.POCOS.DAL.PartHelper()).getEDIExportingParts(store.profile);
                else
                    parts = (new eStore.POCOS.DAL.PartHelper()).getEDIExportingParts(store.profile, productids);
                foreach (eStore.POCOS.Part p in parts)
                {
                    if (p is POCOS.Product)

                        Products.Add(productadpter(p as POCOS.Product));
                    else
                        Products.Add(productadpter(p));
                }

                edi.Products = Products;
                return Serializer<AdvantechProductsEDI>(edi);
            }
            catch (Exception)
            {

                return string.Empty;
            }


        }
        public string exportConfigurationSystems()
        {
            try
            {
                List<ConfigurationSystem> ConfigurationSystems = new List<ConfigurationSystem>();
                AdvantechSystemsEDI edi = new AdvantechSystemsEDI();

                List<eStore.POCOS.Product_Ctos> systems = new List<POCOS.Product_Ctos>();

                systems = (new eStore.POCOS.DAL.PartHelper()).getCTOSProducts(store.storeID);
                foreach (eStore.POCOS.Product_Ctos s in systems)
                {

                    ConfigurationSystems.Add(productadpter(s));
                }

                edi.ConfigurationSystems = ConfigurationSystems;
                return Serializer<AdvantechSystemsEDI>(edi);
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }

        public string Serializer<T>(T edi)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(stringWriter, edi);
            string serializedXML = stringWriter.ToString();
            return serializedXML;
        }

        public string exportSystem(string partno)
        {
            POCOS.Part product = store.getPart(partno);
            AdvantechSystemEDI edi = new AdvantechSystemEDI();
            if (product is POCOS.Product_Ctos)
            {
                edi.ConfigurationSystem = productadpter((POCOS.Product_Ctos)product);

                return Serializer<AdvantechSystemEDI>(edi);
            }
            else
                return "";
        }
        public string exportProduct(string partno)
        {
            try
            {

                POCOS.Part product = store.getPart(partno);
                if (product != null)
                {
                    AdvantechProductEDI edi = new AdvantechProductEDI();
                    if (product is POCOS.Product_Ctos)
                    {
                        edi.Product = productadpter((POCOS.Product_Ctos)product);
                    }
                    else if (product is POCOS.Product)
                    { 
                        edi.Product = productadpter((POCOS.Product)product); 
                    }
                    else
                    {
                        edi.Product = productadpter(product);
                    }
                    return Serializer<AdvantechProductEDI>(edi);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {

                return string.Empty;
            }

        }

        private LTron.ProductCategory categoryadpter(eStore.POCOS.ProductCategory ec)
        {
            LTron.ProductCategory lc = new ProductCategory();
            lc.Name = ec.LocalCategoryName;
            lc.ID = ec.CategoryID;
            if (ec.childCategoriesX.Any())
            {
                List<LTron.ProductCategory> subcat = new List<ProductCategory>();
                foreach (eStore.POCOS.ProductCategory sec in ec.childCategoriesX)
                    subcat.Add(categoryadpter(sec));
                lc.SubCategories = subcat;
            }
            return lc;
        }
        private LTron.ConfigurationSystem productadpter(eStore.POCOS.Product_Ctos ep)
        {
            ConfigurationSystem lp = new ConfigurationSystem();

            lp.productcode = ep.SProductID;
            lp.productdisplayname = ep.name;
            //lp.vendor_partno = ep.SProductID;
            lp.productmanufacturer = "Advantech";

            lp.productprice = ep.getListingPrice().value;
            //lp.taxableproduct = "Y";
            //lp.productcondition = "New";
            lp.productdescription = ep.VendorExtendedDesc;
            lp.productdescriptionshort = ep.productDescX;

            lp.extinfo = ep.productFeatures;
            //lp.productdescription_abovepricing = ep.productDescX;
            //lp.stockstatus = ep.atp.availableQty;
            lp.availability = store.isFastDeliveryProducts(ep) ? "Usually Ships in 2 Days" : "";

            lp.productweight = ep.ShipWeightKG.GetValueOrDefault();

            lp.length = ep.DimensionLengthCM.GetValueOrDefault();
            lp.width = ep.DimensionWidthCM.GetValueOrDefault();
            lp.height = ep.DimensionHeightCM.GetValueOrDefault();

            //lp.accessories = ep.RelatedProducts.Select(x => x.RelatedSProductID).ToList();
            lp.replacement = ep.ReplaceProducts.Select(x => x.RepSProductID).ToList();

            //List<POCOS.ProductResource> resources = ep.productResourcesX.ToList();
            //lp.Images = (from r in resources
            //             where r.ResourceType == "LargeImages"
            //             select new Product.Image
            //             {
            //                 URL = r.ResourceURL,
            //                 IsMainImage = false
            //             }).ToList();
            //if (!string.IsNullOrEmpty(ep.thumbnailImageX))
            //{
            //    if (lp.Images.Any(x => x.URL == ep.thumbnailImageX))
            //    {
            //        lp.Images.First(x => x.URL == ep.thumbnailImageX).IsMainImage = true;
            //    }
            //    else
            //    {
            //        Product.Image img = new Product.Image();
            //        img.URL = ep.thumbnailImageX;
            //        img.IsMainImage = true;
            //        lp.Images.Add(img);
            //    }
            //}

            //lp.DataSheet = (from r in resources
            //                where r.ResourceType == "Datasheet"
            //                select r.ResourceURL).FirstOrDefault();
            lp.Filters = (from f in ep.specs
                          orderby f.AttrCatName, f.seq.GetValueOrDefault(), f.AttrName
                          select new Product.Filter
                          {
                              Name = f.AttrName,
                              Value = f.AttrValueName,
                              CategoryName = f.AttrCatName
                          }).ToList();
            lp.ProductCategory = (from epc in ep.productCategories.Where(x => x.MiniSite == null)
                                  select new ProductCategory
                                  {
                                      ID = epc.CategoryID,
                                      Name = epc.LocalCategoryName
                                  }).FirstOrDefault();

            lp.ProductCategories = (from epc in ep.productCategories.Where(x => x.MiniSite == null)
                                    select new ProductCategory
                                    {
                                        ID = epc.CategoryID,
                                        Name = epc.LocalCategoryName
                                    }).ToList();

            if (ep.isValid())
            {
                List<LTron.ConfigurationSystem.Component> components = new List<ConfigurationSystem.Component>();
                foreach (CTOSBOM com in ep.components)
                {
                    if (com.isCategory() && !com.isWarrantyType())
                    {
                        LTron.ConfigurationSystem.Component lcom = new ConfigurationSystem.Component();
                        lcom.Name = com.CTOSComponent.ComponentName;
                        lcom.DisplayType = com.InputType;
                        lcom.IsMainComponent = com.isMainComponent();
                        List<LTron.ConfigurationSystem.Option> options = new List<ConfigurationSystem.Option>();

                        foreach (CTOSBOM option in com.options)
                        {
                            LTron.ConfigurationSystem.Option loption = new ConfigurationSystem.Option();
                            loption.Default = option.Defaults;
                            loption.Name = option.CTOSComponent.ComponentName;
                            List<LTron.ConfigurationSystem.OptionDetail> details = new List<ConfigurationSystem.OptionDetail>();
                            foreach (var cd in option.CTOSComponent.CTOSComponentDetails)
                            {
                                LTron.ConfigurationSystem.OptionDetail lcd = new ConfigurationSystem.OptionDetail { productcode = cd.SProductID, Qty = cd.Qty };
                                details.Add(lcd);
                            }

                            loption.optiondetails = details;
                            options.Add(loption);
                        }
                        lcom.options = options;
                        components.Add(lcom);
                    }
                }
                lp.components = components;
            }
            return lp;
        }
        private LTron.Product productadpter(eStore.POCOS.Product ep)
        {
            Product lp = new Product();

            lp.productcode = ep.SProductID;
            lp.productdisplayname = ep.name;
            //lp.vendor_partno = ep.SProductID;
            lp.productmanufacturer = "Advantech";

            lp.productprice = ep.getListingPrice().value;
            //lp.taxableproduct = "Y";
            //lp.productcondition = "New";
            lp.productdescription = ep.VendorExtendedDesc;
            lp.productdescriptionshort = ep.productDescX;

            lp.extinfo = ep.productFeatures;
            //lp.productdescription_abovepricing = ep.productDescX;
            lp.stockstatus = ep.atp.availableQty;
            lp.availability = store.isFastDeliveryProducts(ep) ? "Usually Ships in 2 Days" : "";

            lp.productweight = ep.ShipWeightKG.GetValueOrDefault();

            lp.length = ep.DimensionLengthCM.GetValueOrDefault();
            lp.width = ep.DimensionWidthCM.GetValueOrDefault();
            lp.height = ep.DimensionHeightCM.GetValueOrDefault();

            lp.accessories = ep.RelatedProducts.Select(x => x.RelatedSProductID).ToList();
            lp.replacement = ep.ReplaceProducts.Select(x => x.RepSProductID).ToList();

            List<POCOS.ProductResource> resources = ep.ProductResources.ToList();
            lp.Images = (from r in resources
                         where r.ResourceType == "LargeImages"
                         select new Product.Image
                         {
                             URL = r.ResourceURL,
                             IsMainImage = false
                         }).ToList();
            if (!string.IsNullOrEmpty(ep.thumbnailImageX))
            {
                if (lp.Images.Any(x => x.URL == ep.thumbnailImageX))
                {
                    lp.Images.First(x => x.URL == ep.thumbnailImageX).IsMainImage = true;
                }
                else
                {
                    Product.Image img = new Product.Image();
                    img.URL = ep.thumbnailImageX;
                    img.IsMainImage = true;
                    lp.Images.Add(img);
                }
            }

            lp.DataSheet = (from r in resources
                            where r.ResourceType == "Datasheet"
                            select r.ResourceURL).FirstOrDefault();
            lp.Filters = (from f in ep.specs
                          orderby f.AttrCatName, f.seq.GetValueOrDefault(), f.AttrName
                          select new Product.Filter
                          {
                              Name = f.AttrName,
                              Value = f.AttrValueName,
                              CategoryName = f.AttrCatName
                          }).ToList();
            lp.ProductCategory = (from epc in ep.productCategories.Where(x=>x.MiniSite==null)
                                  select new ProductCategory
                                  {
                                      ID = epc.CategoryID,
                                      Name = epc.LocalCategoryName
                                  }).FirstOrDefault();

            lp.ProductCategories = (from epc in ep.productCategories.Where(x => x.MiniSite == null)
                                  select new ProductCategory
                                  {
                                      ID = epc.CategoryID,
                                      Name = epc.LocalCategoryName
                                  }).ToList();
            return lp;
        }

        private LTron.Product productadpter(eStore.POCOS.Part ep)
        {
            Product lp = new Product();
            lp.productcode = ep.SProductID;
            lp.productdisplayname = ep.name;
            //lp.vendor_partno = ep.SProductID;
            lp.productmanufacturer = "Advantech";

            lp.productprice = ep.getListingPrice().value;
            //lp.taxableproduct = "Y";
            //lp.productcondition = "New";
            lp.productdescription = ep.VendorExtendedDesc;
            lp.productdescriptionshort = ep.productDescX;

            lp.extinfo = ep.productFeatures;
            //lp.productdescription_abovepricing = ep.productDescX;
            lp.stockstatus = ep.atp.availableQty;
            lp.availability = "";

            lp.productweight = ep.ShipWeightKG.GetValueOrDefault();

            lp.length = ep.DimensionLengthCM.GetValueOrDefault();
            lp.width = ep.DimensionWidthCM.GetValueOrDefault();
            lp.height = ep.DimensionHeightCM.GetValueOrDefault();

            lp.accessories = ep.RelatedProducts.Select(x => x.RelatedSProductID).ToList();

            lp.ProductCategory = new ProductCategory();
            lp.Images = (from r in ep.ProductResources
                         where r.ResourceType == "LargeImages"
                         select new Product.Image
                         {
                             URL = r.ResourceURL,
                             IsMainImage = false
                         }).ToList();
            if (!string.IsNullOrEmpty(ep.thumbnailImageX))
            {
                if (lp.Images.Any(x => x.URL == ep.thumbnailImageX))
                {
                    lp.Images.First(x => x.URL == ep.thumbnailImageX).IsMainImage = true;
                }
                else
                {
                    Product.Image img = new Product.Image();
                    img.URL = ep.thumbnailImageX;
                    img.IsMainImage = true;
                    lp.Images.Add(img);
                }
            }

            lp.DataSheet = ep.dataSheetX;

            lp.Filters = (from f in ep.specs
                          orderby f.AttrCatName, f.seq.GetValueOrDefault(), f.AttrName
                          select new Product.Filter
                          {
                              Name = f.AttrName,
                              Value = f.AttrValueName,
                              CategoryName = f.AttrCatName
                          }).ToList();
            return lp;
        }
    }
}
