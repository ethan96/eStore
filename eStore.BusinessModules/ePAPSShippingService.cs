using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    public class ePAPSShippingService
    {
        private List<ProductShippingDimension> _uspsProductBoxes;

        private List<ProductShippingDimension> _uspsFreeShipProductBoxes;

        private List<ProductShippingDimension> _upsProductBoxes;

        private List<ProductShippingDimension> _upsFreeShipProductBoxes;

        private List<ProductShippingDimension> _freeDropShipProductBoxes;

        private List<ProductShippingDimension> _dropShipProductBoxes;

        public PAPSExecuteResult getStandardShippingMethods(POCOS.Store store, Cart cart)
        {
            // check if ship to country is in USA
            if (!cart.ShipToContact.CountryCode.ToUpper().Contains("US"))
            {
                return new PAPSExecuteResult
                {
                    Status = "Error",
                    Message = "Ship to is not in USA"
                };
            }
            // prepare ProductShippingDimension
            var productShippingDimensions = new List<ProductShippingDimension>();
            foreach (var item in cart.cartItemsX)
            {
                var itemNo = item.ItemNo;
                string partNo = string.Empty;
                if (item.bundleX!=null)
                {
                    int? bundleID = null;
                    if (item.bundleX.SourceType == "ePAPSBundleWith" && item.bundleX.SourceTemplateID.HasValue)
                    {
                        bundleID = item.bundleX.SourceTemplateID.Value;
                    }
                    foreach (var bItem in item.bundleX.BundleItems)
                    {                        
                        partNo = bItem.ItemSProductID;
                        var qty = (int)bItem.quantity;
                        var productShippingDimension = new ProductShippingDimension
                        {
                            ID = 0,
                            ItemNo = itemNo,
                            PartNo = partNo,
                            Quantity = qty,
                            BundleID = bundleID
                        };
                        productShippingDimensions.Add(productShippingDimension);
                    }
                }
                else
                {
                    partNo = item.partX.SProductID;
                    var qty = item.Qty;
                    var productShippingDimension = new ProductShippingDimension
                    {
                        ID = 0,
                        ItemNo = itemNo,
                        PartNo = partNo,
                        Quantity = qty
                    };
                    productShippingDimensions.Add(productShippingDimension);
                }
                //var partNo = item.partX.SProductID;
                //var qty = item.Qty;
                //var productShippingDimension = new ProductShippingDimension
                //{
                //    ID = 0,
                //    ItemNo = itemNo,
                //    PartNo = partNo,
                //    Quantity = qty
                //};
                //productShippingDimensions.Add(productShippingDimension);
            }
            // retrieve ProductShippingDimension detail info
            productShippingDimensions = getProductShippingDimensions(store.StoreID, productShippingDimensions);
            var uspsShippingInstructions = new List<ShippingInstruction>();
            var toZipCode = cart.ShipToContact.ZipCode;
            var fromZipCode = store.ShipFromAddress.ZipCode;
            try
            {
                var response = getFreightEstimationX(store, productShippingDimensions, fromZipCode, toZipCode);            
                return response;
            }
            catch (Exception ex) // catch exception thrown by getUSPSFreightEstimation when StorCarier USPS not defined.
            {
                var response = new PAPSExecuteResult
                {
                    Status = "Error",
                    Message = ex.Message
                };
                return response;
            }
        }

        public List<ProductShippingDimension> getProductShippingDimensions(string storeId, List<ProductShippingDimension> productShippingDimensions)
        {
            var boxes = new List<ProductShippingDimension>();
            var partHelper = new PartHelper();
            var pStoreProductHelper = new PStoreProductHelper();
            var partList = new List<string>();
            BusinessModules.Store store=BusinessModules.StoreSolution.getInstance().getStore(storeId);
            string ePAPS_Ignore_PartNo_In_Shipping_Calculation = store.profile.getStringSetting("ePAPS_Ignore_PartNo_In_Shipping_Calculation");
            char[] delimiters = new char[] {','};
            string[] values = ePAPS_Ignore_PartNo_In_Shipping_Calculation.Split(delimiters);
            
            foreach (var p in productShippingDimensions)
            {
                bool excluded = false;
                if (values != null && values.Length > 0)
                {                    
                    foreach (var v in values)
                    {
                        if (!string.IsNullOrEmpty(v) && p.PartNo.ToUpper().Contains(v.ToUpper()))
                        {
                            excluded = true;
                            break;
                        }
                    }
                }
                if (!excluded)
                    partList.Add(p.PartNo);
            }
            var data = (from p in partHelper.prefetchPartList(storeId, partList)
                       let pstoreproduct=p is POCOS.PStoreProduct ?(POCOS.PStoreProduct)p:null
                        where pstoreproduct !=null 
                        select new
                        {
                            Id = pstoreproduct.PProductId,
                            SProductID = p.SProductID,
                            Width = p.DimensionLengthCM,
                            Depth = p.DimensionWidthCM,
                            Height = p.DimensionHeightCM,
                            ShipWeightKG = p.ShipWeightKG,
                            ShipFrom = pstoreproduct.ShipFrom,
                            FreeShipping = pstoreproduct.FreeShipping,
                        }).ToList();

            if (data.Count != partList.Count) throw new Exception("Some part no not exist in Part table.");

            var result = (from d in data
                          join c in productShippingDimensions on d.SProductID equals c.PartNo into grp
                          from g in grp.DefaultIfEmpty()
                          select new 
                          {
                              Id = d.Id,
                              SProductID = d.SProductID,
                              ItemNo = g == null ? 1 : g.ItemNo, //c.ItemNo,
                              Width = d.Width,
                              Depth = d.Depth,
                              Height = d.Height,
                              ShipWeightKG = d.ShipWeightKG,
                              ShipFrom = d.ShipFrom,
                              FreeShipping = d.FreeShipping,
                              Quantity = g == null ? 1 : g.Quantity //c.Quantity,                              
                          }).Distinct().ToList();
                        
            foreach (var d in result)
            {
                if (d.Width != null)
                {
                    bool? isBundleFreeShipping = null;
                    var dim = productShippingDimensions.FirstOrDefault(x => x.PartNo == d.SProductID && x.ItemNo == d.ItemNo);
                    int? bundleID = null;
                    // check if bundle item free shipping
                    // get bundle id
                    if (dim.BundleID.HasValue)
                    {
                        bundleID = dim.BundleID;
                        POCOS.StoreProductBundle papsbundle = (new POCOS.DAL.StoreProductBundleHelper()).get(dim.BundleID.Value);
                        if (papsbundle != null)
                        {
                            isBundleFreeShipping = papsbundle.FreeShipping.GetValueOrDefault(false);
                        }
                    }
                    boxes.Add(new ProductShippingDimension
                    {
                        ID = (int)d.Id,
                        PartNo = d.SProductID,
                        ItemNo = d.ItemNo,
                        Quantity = d.Quantity,
                        Width = (decimal)Math.Ceiling(d.Width.GetValueOrDefault() / 2.54m),
                        Depth = (decimal)Math.Ceiling(d.Depth.GetValueOrDefault() / 2.54m),
                        Height = (decimal)Math.Ceiling(d.Height.GetValueOrDefault() / 2.54m),
                        ShipWeightKG = d.ShipWeightKG,
                        ShipFrom = d.ShipFrom,
                        FreeShipping = isBundleFreeShipping != null ? isBundleFreeShipping : d.FreeShipping, // changed to support bundle free shipping
                        BundleID = bundleID
                    });
                }
            }
            return boxes;
        }
        
        private List<ShippingMethod> getUSPSFreightEstimation(POCOS.Store store, List<ProductShippingDimension> boxes, string fromZipCode, string toZipCode, out List<ShippingInstruction> uspsShippingInstructions)
        {
            var shippingMethods = new List<ShippingMethod>();
            var shippingCarrier = getShippingCarrier(store, "USPS");
            if (shippingCarrier == null) throw new Exception("StorCarier USPS not defined.");
            var usps = new ShippingCarrierUSPS(store, shippingCarrier);
            shippingMethods = usps.getFreightEstimationX(store.StoreID, boxes, fromZipCode, toZipCode, out uspsShippingInstructions);
            return shippingMethods;
        }

        public POCOS.Cart prepareCart(POCOS.Store store, List<ProductShippingDimension> productShippingDimensions)
        {
            List<string> pns = productShippingDimensions.Select(x => x.PartNo).ToList();
            POCOS.DAL.PartHelper helper = new POCOS.DAL.PartHelper();
            POCOS.User actingUser = new POCOS.User();
            actingUser.UserID = "paps@advantech.com";
            List<POCOS.Part> parts = helper.prefetchPartList(store.StoreID, pns);
            POCOS.Cart cart = new POCOS.Cart(store, actingUser);

            foreach (var psd in productShippingDimensions)
            {
                var part = parts.FirstOrDefault(x => x.SProductID == psd.PartNo);
                if (part == null)
                {
                    part = helper.AddParttoStore(psd.PartNo, store, false);
                }
                if (part != null)
                {
                    cart.addItem(part, productShippingDimensions.FirstOrDefault(x => x.PartNo == part.SProductID).Quantity);
                }
                else
                {
                    throw new Exception(string.Format("PartNo: {0} not exists in SAP", psd.PartNo));
                }
            }
            return cart;
        }
        
        private List<ShippingMethod> getUPSFreightEstimation(POCOS.Store store, List<ProductShippingDimension> boxes, string fromZipCode, string toZipCode, out List<ShippingInstruction> upsShippingInstructions)
        {
            var shippingMethods = new List<ShippingMethod>();
            upsShippingInstructions = new List<ShippingInstruction>();

            // prepare POCOS.ShippingCarrier
            // create cart
            POCOS.Cart cart = prepareCart(store, boxes);
            cart.ShipToContact = new CartContact();
            cart.ShipToContact.ZipCode = toZipCode;
            cart.ShipToContact.countryCodeX = "US";
            var shippingCarrier = getShippingCarrier(store, "UPS_US");
            if (shippingCarrier == null) throw new Exception("StorCarier UPS_US not defined.");

            var ups = new ShippingCarrierUPS(store, shippingCarrier);
            shippingMethods = ups.getFreightEstimation(cart, cart.storeX.ShipFromAddress);

            // get UPS Ground Shipping Method only
            var groundShip = shippingMethods.Where(x => x.ShippingMethodDescription == "UPS Ground").FirstOrDefault();
            var returnShippingMethods = new List<ShippingMethod>();
            returnShippingMethods.Add(groundShip);
            // prepare dropShipInstructions based on cart.PackingLists
            var packingList = groundShip.PackingList;
            foreach (var p in packingList.PackagingBoxes)
            {
                foreach (var b in p.PackingBoxDetails)
                {
                    var box = boxes.Where(x => x.PartNo == b.SProductID).FirstOrDefault();
                    var qty = b.Qty;
                    var itemNo = box.ItemNo;
                    for(var i=0;i<qty;i++)
                    {
                        upsShippingInstructions.Add(new ShippingInstruction
                        {
                            ContainerNo = p.BoxId,
                            ItemNo = itemNo,
                            Rate = groundShip.ShippingCostWithPublishedRate,
                            ServiceCode = groundShip.ServiceCode,
                            ShippingCarrier = groundShip.ShippingCarrier,
                            ShipVia = "",
                            ShippingMethodId = 0,
                            ContainerFreight = groundShip.ShippingCostWithPublishedRate,
                            Level = 0,
                            PartNo = b.SProductID
                        });
                    }
                }
            }

            return returnShippingMethods;
        }        

        /// <summary>
        /// This is for vendro drop ship but not free. (ShipFrom is not blank, null, Advantech and FreeShipping is true)
        /// We use UPS Ground ship from Milpitas to customer's zipcode to estimate freight charge we need to collect.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="boxes"></param>
        /// <param name="fromZipCode"></param>
        /// <param name="toZipCode"></param>
        /// <param name="dropShipInstructions"></param>
        /// <returns></returns>
        private List<ShippingMethod> getDropShipEstimation(POCOS.Store store, List<ProductShippingDimension> boxes, string fromZipCode, string toZipCode, out List<ShippingInstruction> dropShipInstructions)
        {
            var shippingMethods = new List<ShippingMethod>();
            dropShipInstructions = new List<ShippingInstruction>();
            // replace below lines with UPS            
            shippingMethods = getUPSFreightEstimation(store, boxes, fromZipCode, toZipCode, out dropShipInstructions);
            return shippingMethods;
        }        

        public ShippingCarier getShippingCarrier(POCOS.Store store, string carrierName)
        {
            var shippingCarrier = store.StoreCariers.Where(x => x.CarierName == carrierName).Select(x => x.ShippingCarier).FirstOrDefault();
            return shippingCarrier;
        }

        private List<ProductShippingDimension> getProductShippingDimension(POCOS.Store store, List<ProductShippingDimension> productShippingDimensions)
        {
            var boxes = new List<ProductShippingDimension>();
            //var shippingCarrier = getShippingCarrier(store, "USPS");
            //var shippingCarrierUSPS = new ShippingCarrierUSPS(store, shippingCarrier);
            var ePAPSShippingService = new ePAPSShippingService();
            boxes = ePAPSShippingService.getProductShippingDimensions(store.StoreID, productShippingDimensions);
            return boxes;
        }

        /// <summary>
        /// Prepare 5 different groups for different shipping methods
        /// group 1 (_uspsProductBoxes) for products can be placed into USPS package box and is not free shipping
        /// group 2 (_uspsFreeShipProductBoxes) for products can be placed into USPS package box and is free shipping
        /// group 3 (_upsProductBoxes) for products can not be placed into USPS package box and is not free shipping
        /// group 4 (_upsFreeShipProductBoxes) for products can be placed into UPS package box and is free shipping
        /// group 5 (_freeDropShipProductBoxes) for drop ship products which is free shipping
        /// group 6 (_dropShipProductBoxes) for drop ship products which is not free shipping
        /// 
        /// Spec Changes: (2/1/2016 requested by Danny)
        /// 1. Split ship-via via vendor drop ship and ship from Advantech first
        /// 2. For ship from Advantech items
        /// 2.1 if individual item's size can be fit into USPS then ship them all in USPS
        /// 2.2 else ship them all in UPS
        /// </summary>
        /// <param name="store"></param>
        /// <param name="boxes"></param>
        private void prepareDifferentShippingMethodProductBoxes(POCOS.Store store, List<ProductShippingDimension> boxes)
        {
            _uspsProductBoxes = new List<ProductShippingDimension>();
            _uspsFreeShipProductBoxes = new List<ProductShippingDimension>();
            _upsProductBoxes = new List<ProductShippingDimension>();
            _upsFreeShipProductBoxes = new List<ProductShippingDimension>();
            _freeDropShipProductBoxes = new List<ProductShippingDimension>();
            _dropShipProductBoxes = new List<ProductShippingDimension>();
            var shippingCarrier = getShippingCarrier(store, "USPS");
            if (shippingCarrier == null) throw new Exception("StorCarier USPS not defined.");
            var shippingCarrierUSPS = new ShippingCarrierUSPS(store, shippingCarrier);
            var availableUSPSContainer = shippingCarrierUSPS.getAvailableUSPSContainers();
            // 2/1/2016 change request
            bool shipViaUSPS = true;
            foreach (var b in boxes)
            {
                if (b.ShipFrom == null || b.ShipFrom.Trim() == "" || b.ShipFrom.Trim() == "Advantech")
                {
                    ProductShippingDimension box = new ProductShippingDimension
                    {
                        ID = b.ID,
                        ItemNo = b.ItemNo,
                        Depth = b.Depth,
                        Height = b.Height,
                        Width = b.Width,
                        FreeShipping = b.FreeShipping,
                        PartNo = b.PartNo,
                        ShipFrom = b.ShipFrom,
                        ShipWeightKG = b.ShipWeightKG,
                        Quantity = 1
                    };
                    shipViaUSPS = (from a in availableUSPSContainer
                                   where a.Width >= b.Width && a.Depth >= b.Depth && a.Height >= b.Height
                                   select a).Count() >= 1;
                }
                if (!shipViaUSPS) break;
            }
            foreach (var b in boxes)
            {
                for (var i = 0; i < b.Quantity; i++)
                {
                    ProductShippingDimension box = new ProductShippingDimension
                    {
                        ID = b.ID,
                        ItemNo = b.ItemNo,
                        Depth = b.Depth,
                        Height = b.Height,
                        Width = b.Width,
                        FreeShipping = b.FreeShipping,
                        PartNo = b.PartNo,
                        ShipFrom = b.ShipFrom,
                        ShipWeightKG = b.ShipWeightKG,
                        Quantity = 1
                    };
                    if (b.ShipFrom == null || b.ShipFrom.Trim() == "" || b.ShipFrom.Trim() == "Advantech")
                    {
                        // added per 2/1/2016 change request
                        if (shipViaUSPS)
                        {
                            if (b.FreeShipping == true)
                                _uspsFreeShipProductBoxes.Add(box);
                            else
                                _uspsProductBoxes.Add(box);
                        }
                        else
                        {
                            if (b.FreeShipping == true)
                                _upsFreeShipProductBoxes.Add(box);
                            else
                                _upsProductBoxes.Add(box);
                        }
                        /* commented per 2/1/2016 change request
                        var containerAvailable = (from a in availableUSPSContainer
                                                  where a.Width >= b.Width && a.Depth >= b.Depth && a.Height >= b.Height
                                                  select a).Count() >= 1;
                        if (containerAvailable)
                        {
                            if (b.FreeShipping == true)
                                _uspsFreeShipProductBoxes.Add(box);
                            else
                                _uspsProductBoxes.Add(box);
                        }
                        else
                        {
                            if (b.FreeShipping == true)
                                _upsFreeShipProductBoxes.Add(box);
                            else
                                _upsProductBoxes.Add(box);
                        }
                        */ 
                    }
                    else
                    {
                        if (b.FreeShipping == true)
                            _freeDropShipProductBoxes.Add(box);
                        else
                            _dropShipProductBoxes.Add(box);
                    }
                }
            }
        }        

        public PAPSExecuteResult getFreightEstimationX(POCOS.Store store, List<ProductShippingDimension> productShippingDimensions, string fromZipCode, string toZipCode)
        {
            List<ShippingInstruction> shippingInstructions = new List<ShippingInstruction>();
            var shippingMethods = new List<ShippingMethod>();
            var shippingDimensions = new List<ProductShippingDimension>();
            foreach (var p in productShippingDimensions)
            {
                shippingDimensions.Add(new ProductShippingDimension
                {
                    ID = p.ID,
                    ItemNo = p.ItemNo,
                    PartNo = p.PartNo,
                    Width = p.Width,
                    Depth = p.Depth,
                    Height = p.Height,
                    FreeShipping = p.FreeShipping,
                    Quantity = p.Quantity,
                    ShipFrom = p.ShipFrom,
                    ShipWeightKG = p.ShipWeightKG,
                    BundleID = p.BundleID
                });
            }
            var productBoxes = new List<ProductShippingDimension>();
            try
            {
                productBoxes = getProductShippingDimension(store, shippingDimensions);
            }
            catch (Exception ex)
            {
                return new PAPSExecuteResult
                {
                    Status = "Error",
                    Message = ex.Message
                };
            }

            prepareDifferentShippingMethodProductBoxes(store, productBoxes);
            var shippingMethodId = 1;

            var uspsShippingMethods = new List<ShippingMethod>();
            var uspsFreeShippingMethods = new List<ShippingMethod>();
            var upsShippingMethods = new List<ShippingMethod>();
            var upsFreeShippingMethods = new List<ShippingMethod>();
            var dropshipFreeShippingMethods = new List<ShippingMethod>();
            var dropshipShippingMethods = new List<ShippingMethod>();

            // prepare USPS shipping instructions            
            var uspsShippingInstructions = new List<ShippingInstruction>();
            if (_uspsProductBoxes.Count > 0)
            {
                try
                {
                    uspsShippingMethods = getUSPSFreightEstimation(store, _uspsProductBoxes, fromZipCode, toZipCode, out uspsShippingInstructions);

                    foreach (var i in uspsShippingInstructions)
                    {
                        i.ShippingMethodId = shippingMethodId;
                        i.ShippingCarrier = uspsShippingMethods[0].ShippingCarrier;
                        i.ServiceCode = uspsShippingMethods[0].ServiceCode.Replace("&lt;sup&gt;&#8482;&lt;/sup&gt;", "");
                        i.Rate = uspsShippingMethods[0].ShippingCostWithPublishedRate;
                    }
                    shippingMethodId++;
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }            

            // prepare USPS free shipping instructions
            var uspsFreeShippingInstructions = new List<ShippingInstruction>();
            if (_uspsFreeShipProductBoxes.Count > 0)
            {
                try
                {
                    uspsFreeShippingMethods = getUSPSFreightEstimation(store, _uspsFreeShipProductBoxes, fromZipCode, toZipCode, out uspsFreeShippingInstructions);
                    foreach (var i in uspsFreeShippingInstructions)
                    {
                        i.ShippingMethodId = shippingMethodId;
                        i.ShippingCarrier = uspsFreeShippingMethods[0].ShippingCarrier;
                        i.ServiceCode = uspsFreeShippingMethods[0].ServiceCode.Replace("&lt;sup&gt;&#8482;&lt;/sup&gt;", "");
                        i.Rate = uspsFreeShippingMethods[0].ShippingCostWithPublishedRate;
                    }
                    shippingMethodId++;
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }            

            // prepare UPS shipping instructions
            var upsShippingInstructions = new List<ShippingInstruction>();
            if (_upsProductBoxes.Count > 0)
            {
                try
                {
                    upsShippingMethods = getUPSFreightEstimation(store, _upsProductBoxes, fromZipCode, toZipCode, out upsShippingInstructions);
                    foreach (var i in upsShippingInstructions)
                    {
                        i.ShippingMethodId = shippingMethodId;
                        i.ShippingCarrier = upsShippingMethods[0].ShippingCarrier;
                        i.ServiceCode = upsShippingMethods[0].ServiceCode;
                        i.ShippingMethodDescription = upsShippingMethods[0].ShippingMethodDescription;
                        i.Rate = upsShippingMethods[0].ShippingCostWithPublishedRate;
                    }
                    shippingMethodId++;
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }

            // prepare UPS free shipping instructions
            var upsFreeShippingInstructions = new List<ShippingInstruction>();
            if (_upsFreeShipProductBoxes.Count > 0)
            {
                try
                {
                    upsFreeShippingMethods = getUPSFreightEstimation(store, _upsFreeShipProductBoxes, fromZipCode, toZipCode, out upsFreeShippingInstructions);
                    foreach (var i in upsFreeShippingInstructions)
                    {
                        i.ShippingMethodId = shippingMethodId;
                        i.ShippingCarrier = upsFreeShippingMethods[0].ShippingCarrier;
                        i.ServiceCode = upsFreeShippingMethods[0].ServiceCode;
                        i.ShippingMethodDescription = upsFreeShippingMethods[0].ShippingMethodDescription;
                        i.Rate = upsFreeShippingMethods[0].ShippingCostWithPublishedRate;
                    }
                    shippingMethodId++;
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }
            
            // prepare Free Drop Ship shipping instructions
            var dropshipFreeShippingInstructions = new List<ShippingInstruction>();
            if (_freeDropShipProductBoxes.Count > 0)
            {
                try
                {
                    dropshipFreeShippingMethods = new List<ShippingMethod>();
                    var vendors = _freeDropShipProductBoxes.Select(x => x.ShipFrom).Distinct();
                    foreach (var v in vendors)
                    {
                        foreach (var p in _freeDropShipProductBoxes.Where(x => x.ShipFrom == v))
                        {
                            // prepare free dropship shipping instructions for this ShipFrom vendor                    
                            var instruction = new ShippingInstruction
                            {
                                ShippingMethodId = shippingMethodId,
                                ShippingCarrier = v,
                                ServiceCode = "Vendor Drop Ship",
                                Rate = (float)0,
                                ContainerNo = v,
                                ShipVia = "Vendor Drop Ship",
                                ItemNo = p.ItemNo,
                                PartNo = p.PartNo,
                                Level = 1,
                            };
                            dropshipFreeShippingInstructions.Add(instruction);
                        }
                    }
                    shippingMethodId++;
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }            

            // charged customer as UPS ground from 95035 to customer's zip code
            var dropshipShippingInstructions = new List<ShippingInstruction>();
            if (_dropShipProductBoxes.Count > 0)
            {
                try
                {
                    dropshipShippingMethods = getDropShipEstimation(store, _dropShipProductBoxes, fromZipCode, toZipCode, out dropshipShippingInstructions);
                    foreach (var i in dropshipShippingInstructions)
                    {
                        i.ShippingMethodId = shippingMethodId;
                        i.ShippingCarrier = dropshipShippingMethods[0].ShippingCarrier;
                        i.ServiceCode = dropshipShippingMethods[0].ServiceCode;
                        i.Rate = dropshipShippingMethods[0].ShippingCostWithPublishedRate;
                    }
                }
                catch (Exception ex)
                {
                    return new PAPSExecuteResult
                    {
                        Status = "Error",
                        Message = ex.Message
                    };
                }
            }            

            // prepare Drop Ship shipping methods
            // there will be only one shipping method : Standard Shipping (5-10 business days)
            float rate = 0;
            if (uspsShippingMethods != null)
                foreach (var m in uspsShippingMethods)
                {
                    rate += m.ShippingCostWithPublishedRate;
                }
            if (upsShippingMethods != null)
                foreach (var m in upsShippingMethods)
                {
                    rate += m.ShippingCostWithPublishedRate;
                }
            if (dropshipShippingMethods != null)
                foreach (var m in dropshipShippingMethods)
                {
                    rate += m.ShippingCostWithPublishedRate;
                }
            var shippingMethod = new ShippingMethod
            {
                ShippingCarrier = "PAPS",
                ServiceCode = "Standard Shipping (5-10 Business Days)",
                ShippingCostWithPublishedRate = rate
            };
            shippingMethods.Add(shippingMethod);

            shippingInstructions = new List<ShippingInstruction>();
            if (uspsShippingInstructions != null)
                shippingInstructions.AddRange(uspsShippingInstructions);
            if (uspsFreeShippingInstructions != null)
                shippingInstructions.AddRange(uspsFreeShippingInstructions);
            if (upsShippingInstructions != null)
                shippingInstructions.AddRange(upsShippingInstructions);
            if (dropshipFreeShippingInstructions != null)
                shippingInstructions.AddRange(dropshipFreeShippingInstructions);
            if (dropshipShippingInstructions != null)
                shippingInstructions.AddRange(dropshipShippingInstructions);

            return new PAPSExecuteResult
            {
                Status = "OK",
                Message = "OK",
                ShippingMethods = shippingMethods,
                ShippingInstructions = shippingInstructions
            };
        }
        
    }

    public class PAPSExecuteResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ShippingMethod> ShippingMethods { get; set; }
        public List<ShippingInstruction> ShippingInstructions { get; set; }
    }
}
