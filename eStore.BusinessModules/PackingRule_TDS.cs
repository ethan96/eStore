using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class PackingRule_TDS : IPackingRule
    {
        public List<POCOS.PackagingBox> packing(POCOS.Store store, POCOS.Cart cart, decimal dimensionalWeightBase)
        {
            List<PackagingBox> packagingBoxes = new List<PackagingBox>();
            try   //packing result shall be in whole. In other sense, either all or none
            {
                foreach (CartItem c in cart.cartItemsX)
                {

                    if (c.type == Product.PRODUCTTYPE.STANDARD)
                    {
                        packagingBoxes.AddRange(createPackagingBox(c.partX.ShipWeightKG.GetValueOrDefault(), c.Qty, dimensionalWeightBase));
                    }
                    else if (c.type == Product.PRODUCTTYPE.BUNDLE)
                    {
                        foreach (BundleItem item in c.bundleX.BundleItems)
                        {
                            if (item.btosX != null)
                                packagingBoxes.AddRange(getCtosBox(item.btosX, c.Qty, dimensionalWeightBase));
                            else
                                packagingBoxes.AddRange(createPackagingBox(c.partX.ShipWeightKG.GetValueOrDefault(), c.Qty * item.quantity, dimensionalWeightBase));
                        }
                    }
                    else if (c.type == Product.PRODUCTTYPE.CTOS)
                    {
                        packagingBoxes.AddRange(getCtosBox(c.btosX, c.Qty, dimensionalWeightBase));
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

        /// <summary>
        /// DHL Freight not need box only use weight
        /// </summary>
        /// <param name="store"></param>
        /// <param name="box"></param>
        /// <param name="details"></param>
        /// <param name="weight"></param>
        /// <param name="qty"></param>
        /// <param name="InsuredValue"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <returns></returns>
        private List<PackagingBox> createPackagingBox(decimal weight, int qty, decimal dimensionalWeightBase)
        {
            if (weight == 0m)
                weight = 0.5m;
            List<PackagingBox> packagingBoxList = new List<PackagingBox>();
            for (int i = 0; i < qty; i++)
            {
                //new measure unit, the measure unit in ProductBox is in Imperial standard
                MeasureUnit measureUnit = new MeasureUnit(0, 0, 0, weight, MeasureUnit.UnitType.METRICS);
             
                PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
                packagingBox.Weight = weight;
                packagingBoxList.Add(packagingBox);
            }
            return packagingBoxList;
        }

        public List<PackagingBox> getCtosBox(BTOSystem btos, int qty, decimal dimensionalWeightBase)
        {
            List<PackagingBox> boxList = new List<PackagingBox>();

            //create a virtual system box
            decimal btosweight = 0m;
            Dictionary<Part, int> sysdetail = new Dictionary<Part, int>();
            foreach (var part in btos.parts)
            {
                sysdetail.Add(part.Key, part.Value);
                var box = createPackagingBox(part.Key.ShipWeightKG.GetValueOrDefault(), part.Value, dimensionalWeightBase);
                btosweight += part.Key.ShipWeightKG.GetValueOrDefault() * part.Value;
                boxList.AddRange(box);
            }
            return boxList;

        }
    }

}
