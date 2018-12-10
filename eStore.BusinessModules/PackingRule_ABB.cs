using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class PackingRule_ABB : IPackingRule
    {

        /// <summary>
        /// Packing bb related cart item, and it only concern weight
        /// </summary>
        /// <param name="store"></param>
        /// <param name="cart"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <returns></returns>
        public List<POCOS.PackagingBox> packing(POCOS.Store store, POCOS.Cart cart, decimal dimensionalWeightBase)
        {
            List<PackagingBox> packagingBoxes = new List<PackagingBox>();
            Dictionary<Part, int> missingDimensionRuleItems = new Dictionary<Part, int>();
            Dictionary<string, decimal> productPrices = new Dictionary<string, decimal>();

            try
            {
                //bool useKG = BS.CountryIsEMEA(c.cartHeader.B_Country);

                //String weightUnits = useKG ? "KGS" : "LBS";


                Decimal maxWeight = 70; // fedex:100, ups:75, usps:70,(lb) select  the miniumum one
                Decimal minWeight = 2.0M; // bb partno minimum weight (lb)

                //if (useKG)
                //{
                //    maxWeight = Common.PoundsToKilograms(maxWeight);
                //}

                //if (minWeight == Decimal.Zero)
                //{
                //    minWeight = 0.5M;
                //}


                foreach (CartItem c in cart.cartItemsX)
                {
                        Decimal thisDetailWeight = Decimal.Zero;

                        if (c.weight == System.Decimal.Zero)
                        {
                            thisDetailWeight += minWeight * c.Qty;
                        }
                        else
                        {
                            thisDetailWeight += MeasureUnit.convertKG2LB(c.weight) * c.Qty;
                        }
                        //if (useKG)
                        //{
                        //    thisDetailWeight = Common.PoundsToKilograms(thisDetailWeight);
                        //}

                    bool foundPackage = false;

                        foreach (var packag in packagingBoxes)
                        {
                            if (packag.Weight + thisDetailWeight <= maxWeight)
                            {
                                packag.Weight += thisDetailWeight;
                                foundPackage = true;
                                break;
                            }
                        }

                        if (!foundPackage)
                        {
                            if (thisDetailWeight > maxWeight)
                            {
                                int neededPackages = (int)Math.Ceiling(thisDetailWeight / maxWeight);

                                Decimal pckgWeight = thisDetailWeight / neededPackages;

                                for (int i = 0; i < neededPackages; i++)
                                {

                                
                                PackagingBox packagingBox = CreatePackingBox(store, pckgWeight, dimensionalWeightBase);
                                packagingBoxes.Add(packagingBox);
                                }
                            }
                            else
                            {

                                PackagingBox packagingBox = CreatePackingBox(store, thisDetailWeight, dimensionalWeightBase);
                                packagingBoxes.Add(packagingBox);
                            }
                        }
                   
                }

            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed at creating packing list", "", "", "", ex);
                packagingBoxes = null;
            }

            return packagingBoxes;
        }

        private PackagingBox CreatePackingBox(POCOS.Store store, decimal weight,  decimal dimensionalWeightBase)
        {
            //weight = Math.Ceiling(MeasureUnit.convertKG2LB(weight));
            
            //new measure unit, the measure unit in ProductBox is in Imperial standard
            //BB箱子的內容,大小不是重點，只在乎箱子的重量與數目
            MeasureUnit measureUnit = new MeasureUnit(0, 0, 0, weight, MeasureUnit.UnitType.IMPERIAL);
            PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
            packagingBox.PackageID += 1;
            string packageBoxId = store.StoreID + "-" + packagingBox.PackageID.ToString();

            
            PackingBoxDetail packageDetail = new PackingBoxDetail();
            packageDetail.SProductID = "";
            packageDetail.Qty = 0;
            packagingBox.PackingBoxDetails.Add(packageDetail);
            packagingBox.QtyOfItemInBox = 0;
            packagingBox.InsuredValue = 0;   //product insured value
            packagingBox.BoxId = packageBoxId;
            packagingBox.RemainCapacity = 0;
            packagingBox.InsuredCurrency = store.defaultCurrency.CurrencyID;

            return packagingBox;
        }

    }
}
