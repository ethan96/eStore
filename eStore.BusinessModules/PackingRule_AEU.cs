using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class PackingRule_AEU : IPackingRule
    {


        public List<POCOS.PackagingBox> packing(POCOS.Store store, POCOS.Cart cart, decimal dimensionalWeightBase)
        {
            List<PackagingBox> packagingBoxes = new List<PackagingBox>();
            Dictionary<Part, int> missingDimensionRuleItems = new Dictionary<Part, int>();
            Dictionary<string, decimal> productPrices = new Dictionary<string, decimal>();

            try   //packing result shall be in whole. In other sense, either all or none
            {

                foreach (CartItem c in cart.cartItemsX)
                {
                    if (c.type == Product.PRODUCTTYPE.STANDARD)
                    {
                        ProductBox box = getProductBoxByBoxRule(store, c.partX.name);
                        if (box != null)
                        {
                            Dictionary<Part, int> detail = new Dictionary<Part, int>();
                            detail.Add(c.partX, 1);
                            //循环qty，创建box.这里的价钱应该是单价. 不是总价
                            packagingBoxes.AddRange(createPackagingBox(store, box, detail, c.partX.ShipWeightKG.GetValueOrDefault(), c.Qty, c.UnitPrice, dimensionalWeightBase));
                        }
                        else
                        {
                            //判断产品的价格有没有 存在,如果有就更新 否则添加
                            if (productPrices.ContainsKey(c.partX.SProductID) && productPrices[c.partX.SProductID] < c.UnitPrice)
                                productPrices[c.partX.SProductID] = c.UnitPrice;
                            else
                                productPrices.Add(c.partX.SProductID, c.UnitPrice);

                            if (missingDimensionRuleItems.ContainsKey(c.partX))
                                missingDimensionRuleItems[c.partX]+=c.Qty;
                            else
                                missingDimensionRuleItems.Add(c.partX, c.Qty);
                        }
                    }
                    else if (c.type == Product.PRODUCTTYPE.BUNDLE)
                    {
                        foreach (BundleItem item in c.bundleX.BundleItems)
                        {
                            if (item.btosX != null)
                            {
                                packagingBoxes.AddRange(getCtosBox(c.partX, item.btosX, c.Qty, dimensionalWeightBase, store));
                            }
                            else
                            {
                                ProductBox box = getProductBoxByBoxRule(store, item.part.name);
                                if (box != null)
                                {
                                    Dictionary<Part, int> detail = new Dictionary<Part, int>();
                                    detail.Add(item.part, 1);
                                    packagingBoxes.AddRange(createPackagingBox(store, box, detail, c.partX.ShipWeightKG.GetValueOrDefault(), c.Qty * item.quantity, item.adjustedPrice, dimensionalWeightBase));
                                }
                                else
                                {
                                    //判断产品的价格有没有 存在,如果有就更新 否则添加
                                    if (productPrices.ContainsKey(item.part.SProductID))
                                    {
                                        if (productPrices[item.part.SProductID] < item.adjustedPrice)
                                            productPrices[item.part.SProductID] = item.adjustedPrice;
                                    }
                                    else
                                        productPrices.Add(item.part.SProductID, item.adjustedPrice);

                                    if (missingDimensionRuleItems.ContainsKey(item.part))
                                        missingDimensionRuleItems[item.part] += c.Qty * item.quantity;
                                    else
                                        missingDimensionRuleItems.Add(item.part, c.Qty * item.quantity);
                                }
                            }
                        }
                    }
                    else if (c.type == Product.PRODUCTTYPE.CTOS)
                    {
                        packagingBoxes.AddRange(getCtosBox(c.partX as Product_Ctos, c.btosX, c.Qty, dimensionalWeightBase, store));
                    }
                }

                if (missingDimensionRuleItems.Any())
                    packagingBoxes.AddRange(MegerPacking(store, missingDimensionRuleItems, dimensionalWeightBase, productPrices));
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed at creating packing list", "", "", "", ex);
                packagingBoxes = null;
            }

            return packagingBoxes;
        }

        private List<POCOS.PackagingBox> MegerPacking(POCOS.Store store, Dictionary<Part, int> packingItems, decimal dimensionalWeightBase, Dictionary<string, decimal> productPrices)
        {
            List<PackagingBox> packagingBoxes = new List<PackagingBox>();
            Dictionary<ProductBox, Dictionary<Part, int>> boxes = new Dictionary<ProductBox, Dictionary<Part, int>>();

            decimal biggestboxweight = MeasureUnit.convertLB2KG(store.getMaximumBox().volumnWeight);
            decimal totalWeight = packingItems.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
            decimal totalPrice = packingItems.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
            if (totalWeight > biggestboxweight)
            {
                foreach (var resolveditem in resolveOverflowedPacking(store, biggestboxweight, packingItems))
                {
                    totalWeight = resolveditem.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                    totalPrice = resolveditem.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
                    packagingBoxes.AddRange(createPackagingBox(store, null, resolveditem, totalWeight, 1, totalPrice, dimensionalWeightBase));
                }
            }
            else
            {
                packagingBoxes.AddRange(createPackagingBox(store, null, packingItems, totalWeight, 1, totalPrice, dimensionalWeightBase));
            }

            return packagingBoxes;
        }

        //返回产品 在cartitem中的价钱
        private decimal getCartPrice(Dictionary<string, decimal> productPrices, Part part)
        {
            decimal price = 0;
            if (part != null )
            {
                if (productPrices.ContainsKey(part.SProductID))
                    price = productPrices[part.SProductID];
                else
                    price = part.getListingPrice().value;
            }
            
            return price;
        }

        private List<Dictionary<Part, int>> resolveOverflowedPacking(POCOS.Store store, decimal maximumWeight, Dictionary<Part, int> waitingPackItems)
        {
            List<Dictionary<Part, int>> reslut = new List<Dictionary<Part, int>>();
            do
            {
                Dictionary<Part, int> oneboxitems = new Dictionary<Part, int>();
                decimal totalweightinbox = 0m;
                int totalcnt = waitingPackItems.Count;
                for (int j = 0; j < totalcnt; j++)
                {
                    var item = waitingPackItems.ElementAt(j);
                    if (item.Value == 0)
                    {
                        continue;
                    }
                    else if (item.Key.ShipWeightKG.GetValueOrDefault() > maximumWeight)
                    {
                        totalweightinbox = item.Key.ShipWeightKG.GetValueOrDefault();
                        oneboxitems.Add(item.Key, item.Value);
                        waitingPackItems[item.Key]--;
                    }
                    else
                    {

                        int capability = (int)Math.Floor((maximumWeight - totalweightinbox) / item.Key.ShipWeightKG.GetValueOrDefault());

                        //all item
                        if (capability >= item.Value)
                        {
                            totalweightinbox += item.Key.ShipWeightKG.GetValueOrDefault() * item.Value;
                            oneboxitems.Add(item.Key, item.Value);
                            waitingPackItems[item.Key] = 0;
                        }
                        else if (capability > 0)
                        {
                            totalweightinbox += item.Key.ShipWeightKG.GetValueOrDefault() * capability;
                            oneboxitems.Add(item.Key, capability);
                            waitingPackItems[item.Key] = item.Value - capability;
                        }
                    }
                    if (totalweightinbox >= maximumWeight)
                        break;
                }
                reslut.Add(oneboxitems);
            } while (waitingPackItems.Sum(x => x.Value) > 0);

            return reslut;
        }

        #region packing ctos

        //get ctos boxes
        public List<PackagingBox> getCtosBox(Part ctos, BTOSystem btos, int qty, decimal dimensionalWeightBase, eStore.POCOS.Store store)
        {
            ProductBox box;
            List<ProductBox> candidateBoxes = new List<ProductBox>();
            List<PackagingBox> boxList = new List<PackagingBox>();

            decimal btosweight = 0m;
            Dictionary<Part, int> sysdetail = new Dictionary<Part, int>();

            decimal additionalTotalPrice = 0;
            //计算出 一个料号 在btos中的价格
            Dictionary<string, decimal> configProductPrice = btos.prices;
            foreach (var part in btos.parts)
            {
                box = getCtosPartBox(store, part.Key);
                sysdetail.Add(part.Key, part.Value);
                btosweight += part.Key.ShipWeightKG.GetValueOrDefault() * part.Value;

                if (box != null)
                {
                    if (box.AdditionalBox)
                    {
                        Dictionary<Part, int> detail = new Dictionary<Part, int>();
                        detail.Add(part.Key, part.Value);

                        sysdetail.Remove(part.Key);
                        //minus additional part shipweightKG
                        btosweight -= part.Key.ShipWeightKG.GetValueOrDefault() * part.Value;

                        decimal additionalPrice = 0;
                        if (configProductPrice.ContainsKey(part.Key.SProductID))
                        {
                            additionalPrice = configProductPrice[part.Key.SProductID];
                            additionalTotalPrice += additionalPrice;
                        }

                        boxList.AddRange(createPackagingBox(store, box, detail, part.Key.ShipWeightKG.GetValueOrDefault(), qty, additionalPrice, dimensionalWeightBase));
                    }
                    else
                        candidateBoxes.Add(box);
                }
            }

            if (sysdetail.Count > 0)
            {
                //find the largest box for CTOS packing
                ProductBox systemBox = new ProductBox();
                systemBox = (from productBox in candidateBoxes
                             orderby productBox.volumn descending
                             select productBox).FirstOrDefault();

                boxList.AddRange(createPackagingBox(store, systemBox, sysdetail, btosweight, qty, btos.Price - additionalTotalPrice, dimensionalWeightBase));
            }

            return boxList;

        }

        private ProductBox getCtosPartBox(POCOS.Store store, Part part)
        {
            //get by rule
            ProductBox box = getProductBoxByBoxRule(store, part.name);
            if (box == null)
                box = store.getMaximumBox();
            return box;
        }

        #endregion

        private List<PackagingBox> createPackagingBox(POCOS.Store store, ProductBox box, Dictionary<Part, int> details, decimal weight, int qty, decimal InsuredValue, decimal dimensionalWeightBase)
        {
            weight = Math.Ceiling(MeasureUnit.convertKG2LB(weight));
            if (weight == 0)
                weight = 1m;
            if (box == null)
                box = getClosestStoreBoxByDimensionWeight(store, weight);
            List<PackagingBox> packagingBoxList = new List<PackagingBox>();
            for (int i = 0; i < qty; i++)
            {
                //new measure unit, the measure unit in ProductBox is in Imperial standard
                MeasureUnit measureUnit = new MeasureUnit(box.LengthINCH,
                                                            box.WidthINCH,
                                                            box.HighINCH,
                                                            weight, MeasureUnit.UnitType.IMPERIAL);
                PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
                string packageBoxId = box.StoreID + "-" + box.BoxID.ToString();
                foreach (var detial in details)
                {
                    PackingBoxDetail packageDetail = new PackingBoxDetail();
                    packageDetail.SProductID = detial.Key.SProductID;
                    packageDetail.Qty = detial.Value;
                    packagingBox.PackingBoxDetails.Add(packageDetail);
                }
                packagingBox.QtyOfItemInBox = details.Sum(x => x.Value) * qty;
                packagingBox.InsuredValue = InsuredValue;   //product insured value
                packagingBox.BoxId = packageBoxId;
                packagingBox.RemainCapacity = box.Capacity - qty;
                packagingBox.InsuredCurrency = store.defaultCurrency.CurrencyID;
                packagingBoxList.Add(packagingBox);
            }
            return packagingBoxList;
        }

        private ProductBox getClosestStoreBoxByDimensionWeight(POCOS.Store store, decimal weightLBS)
        {
            try
            {
                List<ProductBox> _StoreAvailableBoxs = store.getStoreAvailableBox();
                var _ProductBox = (from s in _StoreAvailableBoxs
                                   where weightLBS * 1.2m < s.volumnWeight
                                   orderby s.volumn
                                   select s).FirstOrDefault();
                if (_ProductBox == null)
                    _ProductBox = store.getMaximumBox();
                return _ProductBox;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        private ProductBox getProductBoxByBoxRule(POCOS.Store store, string partno)
        {
            if (string.IsNullOrEmpty(partno)) return null;

            try
            {
                List<ProductBox> _StoreAvailableBoxs = store.getStoreAvailableBox();
                var _ProductBox = (from s in _StoreAvailableBoxs
                                   where (partno.StartsWith(s.PartNoPrefix)  
                                   &&(string.IsNullOrEmpty(s.ExceptionPrefix)||!partno.StartsWith(s.ExceptionPrefix) )
                                   && (string.IsNullOrEmpty(s.ExceptionSuffix) || !partno.EndsWith(s.ExceptionSuffix)))
                                   orderby s.PartNoPrefix.Length descending
                                   select s).FirstOrDefault();

                return _ProductBox;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
    }
}
