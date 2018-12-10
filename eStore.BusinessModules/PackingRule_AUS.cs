using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class PackingRule_AUS : IPackingRule
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

        private List<POCOS.PackagingBox> MegerPacking(POCOS.Store store, Dictionary<Part, int> missingDimensionRuleItems, decimal dimensionalWeightBase, Dictionary<string, decimal> productPrices)
        {
            List<PackagingBox> packagingBoxes = new List<PackagingBox>();

            Dictionary<ProductBox, Dictionary<Part, int>> boxes = new Dictionary<ProductBox, Dictionary<Part, int>>();
            Dictionary<Part, int> noDimensionItems = new Dictionary<Part, int>();

            foreach (var item in missingDimensionRuleItems)
            {
                ProductBox productbox = getProductBox(store, item.Key);

                //if can not get box by dimension, then get by weight
                int timesfortesting = 1;
                if (productbox == null)
                {
                    noDimensionItems.Add(item.Key, item.Value * timesfortesting);
                }
                else if (boxes.Keys.Any(x => x.CompareTo(productbox) == 0))
                {
                    boxes[boxes.Keys.First(x => x.CompareTo(productbox) == 0)].Add(item.Key, item.Value * timesfortesting);
                }
                else
                {
                    Dictionary<Part, int> temp = new Dictionary<Part, int>();
                    temp.Add(item.Key, item.Value * timesfortesting);
                    boxes.Add(productbox, temp);
                }
            }

            for (int i = 0; i < boxes.Count; i++)
            {
                ProductBox box = boxes.ElementAt(i).Key;
                //find similar box, if more then 1 item then merge
                Dictionary<ProductBox, int> similarboxes = boxes
                    .Where(b => Math.Abs(b.Key.CompareTo(box)) <= 1 && b.Value.Sum(v => v.Value) > 0)
                    .ToDictionary(x => x.Key, y => y.Value.Sum(v => v.Value));

                if (similarboxes.Any() && similarboxes.Count > 1)
                {
                    ProductBox mergedBox = mergeBox(store, similarboxes);
                    if (mergedBox != null)
                    {
                        Dictionary<Part, int> temp = new Dictionary<Part, int>();
                        foreach (ProductBox sb in similarboxes.Keys)
                        {
                            foreach (var old in boxes[sb])
                            {
                                temp.Add(old.Key, old.Value);
                            }
                            boxes.Remove(sb);
                        }
                        decimal totalWeight = temp.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                        decimal totalPrice = temp.Sum(x => getCartPrice(productPrices,x.Key) * x.Value);
                        packagingBoxes.AddRange(createPackagingBox(store, mergedBox, temp, totalWeight, 1, totalPrice, dimensionalWeightBase));
                    }
                }
            }

            foreach (var item in boxes)
            {
                int cnt = item.Value.Sum(x => x.Value);
                ProductBox fittingBox = findFittingMergeBox(store, item.Key, cnt);
                if (fittingBox == null)
                {
                    foreach (var resolveditem in resolveOverflowedPacking(store, item.Key, item.Value))
                    {
                        cnt = resolveditem.Sum(x => x.Value);
                        fittingBox = findFittingMergeBox(store, item.Key, cnt);
                        decimal totalWeight = resolveditem.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                        decimal totalPrice = resolveditem.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
                        packagingBoxes.AddRange(createPackagingBox(store, fittingBox, resolveditem, totalWeight, 1, totalPrice, dimensionalWeightBase));
                    }
                }
                else
                {
                    decimal totalWeight = item.Value.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                    decimal totalPrice = item.Value.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
                    packagingBoxes.AddRange(createPackagingBox(store, fittingBox, item.Value, totalWeight, 1, totalPrice, dimensionalWeightBase));
                }
            }

            if (noDimensionItems.Any())
            {
                decimal biggestboxweight = MeasureUnit.convertLB2KG(store.getMaximumBox().volumnWeight) / 1.5m - 1;
                decimal totalWeight = noDimensionItems.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                decimal totalPrice = noDimensionItems.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
                if (totalWeight > biggestboxweight)
                {
                    foreach (var resolveditem in resolveOverflowedPacking(store, biggestboxweight, noDimensionItems))
                    {
                        totalWeight = resolveditem.Sum(x => x.Key.ShipWeightKG.GetValueOrDefault() * x.Value);
                        totalPrice = resolveditem.Sum(x => getCartPrice(productPrices, x.Key) * x.Value);
                        packagingBoxes.AddRange(createPackagingBox(store, null, resolveditem, totalWeight, 1, totalPrice, dimensionalWeightBase));
                    }
                }
                else
                {
                    packagingBoxes.AddRange(createPackagingBox(store, null, noDimensionItems, totalWeight, 1, totalPrice, dimensionalWeightBase));
                }
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

        private ProductBox mergeBox(POCOS. Store store, Dictionary< ProductBox,int> boxes)
        {
            if (boxes == null || boxes.Any() == false)
                return null;
            else
            {
                ProductBox newbox = new ProductBox();
                newbox.WidthINCH = boxes.Keys.Max(x => x.WidthINCH) + 1;
                newbox.LengthINCH = boxes.Keys.Max(x => x.LengthINCH) + 1;
                newbox.HighINCH = boxes.Sum(x => x.Key.HighINCH * x.Value) + 1;
                newbox.StoreID = store.StoreID;
                ProductBox storebox = getClosestStoreBox(store, newbox);
                return storebox;
            }
        }


        private List<Dictionary<Part, int>> resolveOverflowedPacking(POCOS.Store store, ProductBox box, Dictionary<Part, int> waitingPackItems)
        {
            List<Dictionary<Part, int>> reslut = new List<Dictionary<Part, int>>();

            int end = waitingPackItems.Sum(x => x.Value);
            int start = 1;
            int inputitemcnt = 0;
            ProductBox fittingBox = null;
            do
            {
                inputitemcnt = (start + end) / 2;
                fittingBox = findFittingMergeBox(store, box, inputitemcnt);
                if (fittingBox == null)
                {
                    end = inputitemcnt;
                }
                else
                {
                    start = inputitemcnt;
                }

            } while (end > start + 1);


            do
            {
                Dictionary<Part, int> oneboxitems = new Dictionary<Part, int>();
                int totalItemsInbox = 0;
                int totalcnt = waitingPackItems.Count;
                for (int i = 0; i < totalcnt; i++)
                {
                    var item = waitingPackItems.ElementAt(i);
                    if (item.Value == 0)
                    {
                        continue;
                    }
                    else if (totalItemsInbox < inputitemcnt && item.Value > 0)
                    {

                        if (inputitemcnt - totalItemsInbox >= item.Value)
                        {
                            totalItemsInbox += item.Value;
                            oneboxitems.Add(item.Key, item.Value);
                            waitingPackItems[item.Key] = 0;
                        }
                        else
                        {

                            oneboxitems.Add(item.Key, (inputitemcnt - totalItemsInbox));
                            waitingPackItems[item.Key] = item.Value - (inputitemcnt - totalItemsInbox);
                            totalItemsInbox = inputitemcnt;
                        }

                    }
                    if (totalItemsInbox == inputitemcnt)
                    {
                        break;
                    }

                }
                reslut.Add(oneboxitems);

            } while (waitingPackItems.Sum(x => x.Value) > 0);

            return reslut;
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

        ProductBox findFittingMergeBox(POCOS.Store store, ProductBox box, int qty)
        {

            ProductBox fittingBox = (from mb in getPackingDimension(box, qty)
                                     select getClosestStoreBox(store, mb))
                                     .Where(x => x != null)
                                     .OrderBy(x => x.volumn)
                                     .FirstOrDefault();

            return fittingBox;
        }

        List<ProductBox> getPackingDimension(ProductBox box, int qty)
        {
            List<ProductBox> mergedmboxes = new List<ProductBox>();
            box.sortDimension();
            for (int i = 1; Math.Ceiling((decimal)qty / i) >= i; i++)
            {
                decimal temp = Math.Ceiling((decimal)qty / i);

                mergedmboxes.AddRange(generatePossiblyBoxes(box, 1, i, (int)temp));

                for (int j = 2; Math.Ceiling(temp / j) >= i && i >= j; j++)
                {
                    mergedmboxes.AddRange(generatePossiblyBoxes(box, j, i, (int)Math.Ceiling(temp / j)));
                }
            }
            return mergedmboxes;
        }

        List<ProductBox> generatePossiblyBoxes(ProductBox box, int p1, int p2, int p3)
        {
            List<ProductBox> possiblyBoxes = new List<ProductBox>();
            decimal margin = 1m;

            ProductBox box1 = new ProductBox();
            box1.WidthINCH = box.WidthINCH * p1 + margin;
            box1.LengthINCH = box.LengthINCH * p2 + margin;
            box1.HighINCH = box.HighINCH * p3 + margin;
            box1.StoreID = box.StoreID;
            possiblyBoxes.Add(box1);

            ProductBox box2 = new ProductBox();
            box2.WidthINCH = box.WidthINCH * p2 + margin;
            box2.LengthINCH = box.LengthINCH * p3 + margin;
            box2.HighINCH = box.HighINCH * p1 + margin;
            box2.StoreID = box.StoreID;
            possiblyBoxes.Add(box2);

            ProductBox box3 = new ProductBox();
            box3.WidthINCH = box.WidthINCH *p3 + margin;
            box3.LengthINCH = box.LengthINCH * p1 + margin;
            box3.HighINCH = box.HighINCH * p2 + margin;
            box3.StoreID = box.StoreID;
            possiblyBoxes.Add(box3);

            return possiblyBoxes;
        }

        #region packing ctos

        //get ctos boxes
        public List<PackagingBox> getCtosBox(Part ctos, BTOSystem btos, int qty, decimal dimensionalWeightBase, eStore.POCOS.Store store)
        {
            ProductBox box;
            List<ProductBox> candidateBoxes = new List<ProductBox>();
            List<PackagingBox> boxList = new List<PackagingBox>();

            //create a virtual system box
            ProductBox systemBox = getProductBox(store, ctos);
            if (systemBox != null)
                candidateBoxes.Add(systemBox);
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
            {
                box = getProductBox(store, part);
                //no dimension item, then get by weight
                if (box == null)
                {
                    box = store.getDefaultBox();
                }
                else
                {
                    box.WidthINCH++;
                    box.LengthINCH++;
                    box.HighINCH++;
                    box = getClosestStoreBox(store, box);

                    if (box == null)
                        box = store.getMaximumBox();
                }
            }
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

        private ProductBox getProductBox(POCOS.Store store, POCOS.Part part)
        {
            ProductBox box = null;
            if (part != null
                && part.DimensionHeightCM.GetValueOrDefault() > 0
                && part.DimensionLengthCM.GetValueOrDefault() > 0
                && part.DimensionWidthCM.GetValueOrDefault() > 0)
            {
                box = new ProductBox();
                box.LengthINCH = MeasureUnit.convertCM2IN(part.DimensionLengthCM.GetValueOrDefault());
                box.WidthINCH = MeasureUnit.convertCM2IN(part.DimensionWidthCM.GetValueOrDefault());
                box.HighINCH = MeasureUnit.convertCM2IN(part.DimensionHeightCM.GetValueOrDefault());
                box.BoxID = 0;  //virtual box
                box.StoreID = part.StoreID;
                //box = getClosestStoreBox(store, tempbox);
                string[] individualpackaging = { "LCD" };
                if (individualpackaging.Contains(part.VendorProductLine))
                    box.AdditionalBox = true;
            }
            return box;
        }

        private ProductBox getClosestStoreBox(POCOS.Store store, ProductBox box)
        {
            try
            {
                List<ProductBox> _StoreAvailableBoxs = store.getStoreAvailableBox();
                var _ProductBox = (from s in _StoreAvailableBoxs
                                   where box.CompareTo(s) < 1
                                   orderby s.volumn
                                   select s).FirstOrDefault();
                return _ProductBox;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
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
