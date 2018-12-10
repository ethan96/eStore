using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;
using eStore.Utilities;


namespace eStore.BusinessModules
{
    public partial class PackingMethod
    {
        #region Attributes
        public enum Rule {FairPacking,SinglePacking,WeightPacking};
        public Rule PackingRule
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Assign quantity for one packaging box
        /// </summary>
        private int setQtyForPack = 1;
        public int SetQtyForPack
        {
            get
            {
                return setQtyForPack;
            }
            set
            {
                setQtyForPack = value;
            }
        }
        #endregion


        #region Methods
        /// <summary>
        /// The product in cart can get a particular box, this function will help to find suitable box for this cart item
        /// If no dimensional weight base information, please input zero.
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public List<PackagingBox> getDefaultBox(CartItem cartItem, ProductBox box, decimal dimensionalWeightBase, string defaultCurrency)
        {
            List<PackagingBox> boxes = null;

            switch (this.PackingRule)
            {
                case PackingMethod.Rule.FairPacking:
                    boxes = fairPacking(cartItem, box, dimensionalWeightBase, defaultCurrency);
                    break;

                case PackingMethod.Rule.SinglePacking:
                    boxes = singlePacking(cartItem, box, dimensionalWeightBase, defaultCurrency);
                    break;

                case PackingMethod.Rule.WeightPacking:
                    //weightPacking(_box);
                    break;
            }
           
            return boxes;
        }

        /// <summary>
        /// This method is to measure the weight of single cart item and make necessary weight adjustment.
        /// The return value will be in Imperial standard, LBS
        /// </summary>
        /// <param name="cartItem">cartItem's measure unit is cm/kg</param>
        /// <returns></returns>
        private Decimal measureItemWeight(CartItem cartItem)
        {
            return convertAndRoundWeight_KG2LB(cartItem.weight);
        }

        private Decimal measureProductWeight(Part part)
        {
            return convertAndRoundWeight_KG2LB(part.ShipWeightKG.GetValueOrDefault());
        }

        /// <summary>
        /// the input weight shall be in KG and the return will be in LB
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        private Decimal convertAndRoundWeight_KG2LB(Decimal weight)
        {
            Decimal itemWeight = MeasureUnit.convertKG2LB(weight);

            // Adjust if product weight is too light
            if (itemWeight > 0.5m && itemWeight <= 1)
                itemWeight = 1;
            else if (itemWeight <= 0.5m)
                itemWeight = 0.5m;

            return itemWeight;
        }
        #endregion


        #region Packing Rules
        /// <summary>
        /// Fair Packing method can pack particular cart items into serveral pacakges, and make it fair packing. This method will avoid a situation that
        /// some boxes are filled fully and some box is filled with only 1 item.
        /// This method is suitable for parts with prefix 170, 96TO, MIO, ADAM, SQF
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="box"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        private List<PackagingBox> fairPacking(CartItem cartItem, ProductBox box, decimal dimensionalWeightBase, String defaultCurrency)
        {
            List<PackagingBox> boxList = new List<PackagingBox>();

            try
            {
                // the cart item's cost value, not used at the moment
                //Decimal itemValue = (cartItem.Part == null) ? 0 : cartItem.Part.Cost.GetValueOrDefault();

                string packageBoxId = box.StoreID + "-" + box.BoxID.ToString();

                //find out how many boxes are needed
                int quantity = cartItem.Qty;
                int boxCount = (int)Math.Ceiling((Decimal)quantity / box.Capacity); // count needed boxes
                int meanValue = (int)Math.Ceiling((Decimal)quantity / boxCount);  //item per box

                int packSize = 0;
                Decimal totalWeight = 0m;   

                // ItemWeight will be adjusted if the cartItems are 170, 96TO, MIO, ADAM, and measure unit will be changed from kg to lb.
                Decimal itemWeight = measureItemWeight(cartItem);

                do
                {
                    //calculate # of items to pack into a single box
                    packSize = (quantity > meanValue) ? meanValue : quantity;
                    //calculate how much weight this box will contain.
                    totalWeight = packSize * itemWeight;

                    //new measure unit, the measure unit in ProductBox is in Imperial standard
                    MeasureUnit measureUnit = new MeasureUnit(box.LengthINCH,
                                                                box.WidthINCH,
                                                                box.HighINCH, 
                                                                totalWeight, MeasureUnit.UnitType.IMPERIAL);

                    PackingBoxDetail packageDetail = new PackingBoxDetail();
                    packageDetail.SProductID = cartItem.SProductID;
                    packageDetail.Qty = packSize;
                    PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
                    packagingBox.PackingBoxDetails.Add(packageDetail);
                    packagingBox.QtyOfItemInBox = packSize;
                    packagingBox.InsuredValue = cartItem.UnitPrice * packSize;   //product insured value
                    packagingBox.BoxId = packageBoxId;
                    packagingBox.RemainCapacity = box.Capacity - packSize;
                    packagingBox.InsuredCurrency = defaultCurrency;
                    boxList.Add(packagingBox);

                    quantity = quantity - packSize;
                } while (quantity > 0);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed at creating combination packages", "", "", "", ex);
            }

            return boxList;
        }

        /// <summary>
        /// Single packing can pack each 1 cart item into 1 pariticular box.  If the cartItem is a BTO system, there will be an additional 
        /// parameter, adjustedWeight, provided.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="box"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="defaultCurrency"></param>
        /// <param name="adjustedWeight">Optional parameter -- userd only by BTOS item</param>
        /// <returns></returns>
        private List<PackagingBox>singlePacking(CartItem cartItem, ProductBox box, decimal dimensionalWeightBase, String defaultCurrency, Decimal adjustedWeight = 0)
        {
            Decimal singleItemWeight = (adjustedWeight > 0) ? adjustedWeight : measureItemWeight(cartItem);
            List<PackagingBox> boxList = new List<PackagingBox>();
            string packageBoxId = box.StoreID + "-" + box.BoxID.ToString();

            int packSize = 1;
            for (int i = 0; i < cartItem.Qty; i++)
            {
                //new measure unit, the measure unit in ProductBox is in Imperial standard
                MeasureUnit measureUnit = new MeasureUnit(box.LengthINCH,
                                                            box.WidthINCH,
                                                            box.HighINCH,
                                                            singleItemWeight, MeasureUnit.UnitType.IMPERIAL);

                PackingBoxDetail packageDetail = new PackingBoxDetail();
                packageDetail.SProductID = cartItem.SProductID;
                packageDetail.Qty = packSize;
                PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
                packagingBox.PackingBoxDetails.Add(packageDetail);
                packagingBox.QtyOfItemInBox = packSize;
                packagingBox.InsuredValue = cartItem.UnitPrice * packSize;
                packagingBox.BoxId = packageBoxId;
                packagingBox.InsuredCurrency = defaultCurrency;
                boxList.Add(packagingBox);
            }

            return boxList;
        }

        /// <summary>
        /// Single packing can pack each 1 cart item into 1 pariticular box.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="quantity"></param>
        /// <param name="box"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        private List<PackagingBox> singlePacking(Part part, int quantity, ProductBox box, decimal dimensionalWeightBase, String defaultCurrency)
        {
            Decimal itemWeight = measureProductWeight(part);
            List<PackagingBox> boxList = new List<PackagingBox>();
            string packageBoxId = box.StoreID + "-" + box.BoxID.ToString();

            for (int i = 0; i < quantity; i++)
            {
                //new measure unit, the measure unit in ProductBox is in Imperial standard
                MeasureUnit measureUnit = new MeasureUnit(box.LengthINCH,
                                                            box.WidthINCH,
                                                            box.HighINCH,
                                                            itemWeight, MeasureUnit.UnitType.IMPERIAL);

                PackingBoxDetail packageDetail = new PackingBoxDetail();
                packageDetail.SProductID = part.SProductID;
                packageDetail.Qty = 1;
                PackagingBox packagingBox = new PackagingBox(measureUnit, dimensionalWeightBase);
                packagingBox.PackingBoxDetails.Add(packageDetail);
                packagingBox.QtyOfItemInBox = 1;
                packagingBox.InsuredValue = part.Cost.GetValueOrDefault();
                packagingBox.BoxId = packageBoxId;
                packagingBox.InsuredCurrency = defaultCurrency;
                boxList.Add(packagingBox);
            }

            return boxList;
        }

        /// <summary>
        /// This function is only for CTOS products to get boxes.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="store"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        public List<PackagingBox> getCtosBox(CartItem cartItem, decimal dimensionalWeightBase, eStore.POCOS.Store store, string defaultCurrency)
        {
            List<ProductBox> candidateBoxes = new List<ProductBox>();

            ProductBox systemBox = getDefaultsystembox(cartItem.height, cartItem.width , cartItem.length, store);
            candidateBoxes.Add(systemBox);
            decimal btosweight = 0m;

            List<PackagingBox> boxList = new List<PackagingBox>();
            candidateBoxes.AddRange(getCtosBoxes(cartItem.btosX.parts, dimensionalWeightBase, store, defaultCurrency, out boxList, out btosweight));

            //find the largest box for CTOS packing
            systemBox = (from productBox in candidateBoxes
                         orderby productBox.volumn descending
                         select productBox).FirstOrDefault();
            boxList.AddRange(singlePacking(cartItem, systemBox, dimensionalWeightBase, defaultCurrency, btosweight));

            return boxList;
        }

        /// <summary>
        /// Items in bundle will be packed in one box.  Since bundle is not CTOS system, there is no assembly need and each item
        /// is individual item, bundle packing algorithm will try to sum up volumn of every item in this bundle and find the most
        /// compact box that can fit all bundle items in it.
        /// </summary>
        /// <param name="cartItem"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="store"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        public List<PackagingBox> getBundleBox(CartItem cartItem, decimal dimensionalWeightBase, eStore.POCOS.Store store, string defaultCurrency)
        {                      
            List<ProductBox> candidateBoxes = new List<ProductBox>();
            decimal btosweight = 0m;
            List<PackagingBox> boxList = new List<PackagingBox>();
            candidateBoxes.AddRange(getCtosBoxes(cartItem.bundleX.parts, dimensionalWeightBase, store, defaultCurrency, out boxList, out btosweight));

            //find the largest box for CTOS packing
            decimal totalvolumns = candidateBoxes.Sum(x => x.volumn);
            ProductBox bundleBox = (new ProductBoxHelper()).findPackingBoxByVolumn(totalvolumns);
            boxList.AddRange(singlePacking(cartItem, bundleBox, dimensionalWeightBase, defaultCurrency, btosweight));

            return boxList;
        }

        /// <summary>
        /// Change parameter to Product_CTOS for general use
        /// </summary>
        /// <param name="ctos"></param>
        /// <param name="dimensionalWeightBase"></param>
        /// <param name="store"></param>
        /// <param name="defaultCurrency"></param>
        /// <returns></returns>
        public decimal getCtosBox(Product_Ctos ctos, decimal dimensionalWeightBase, eStore.POCOS.Store store, string defaultCurrency)
        {
            List<ProductBox> candidateBoxes = new List<ProductBox>();
            List<PackagingBox> boxList = new List<PackagingBox>();

            //create a virtual system box
            ProductBox systemBox = getDefaultsystembox(ctos.DimensionHeightCM.GetValueOrDefault(),ctos.DimensionWidthCM.GetValueOrDefault(),ctos.DimensionLengthCM.GetValueOrDefault(),store);
            candidateBoxes.Add(systemBox);
            decimal btosweight=0m;
            candidateBoxes.AddRange( getCtosBoxes(ctos.getDefaultBTOS().parts, dimensionalWeightBase, store, defaultCurrency, out boxList, out btosweight));

            //find the largest box for CTOS packing
            systemBox = (from productBox in candidateBoxes
                         orderby productBox.volumn descending
                         select productBox).FirstOrDefault();
            boxList.AddRange(singlePacking(ctos, 1, systemBox, dimensionalWeightBase, defaultCurrency));

            decimal dimensionweight = systemBox.volumn / 166;

            if (dimensionweight > btosweight)
                return dimensionweight;
            else
                return btosweight;
        }

        private ProductBox getDefaultsystembox(decimal height, decimal width, decimal length, POCOS.Store store) {
            ProductBox systemBox = new ProductBox();
            systemBox.LengthINCH = MeasureUnit.convertCM2IN(length);
            systemBox.WidthINCH = MeasureUnit.convertCM2IN(width);
            systemBox.HighINCH = MeasureUnit.convertCM2IN(height);
            systemBox.Capacity = 1;
            systemBox.StoreID = store.StoreID;

            return systemBox;
        }

        private List<ProductBox> getCtosBoxes(Dictionary<Part, int> parts, decimal dimensionalWeightBase, eStore.POCOS.Store store, string defaultCurrency, out List<PackagingBox> boxList , out decimal btosWeight )
        {
            ProductBox box;
            List<ProductBox> candidateBoxes = new List<ProductBox>();

             boxList = new List<PackagingBox>();
              btosWeight = 0m;
            //extract CTOS parts which need to be packed separately
            foreach (KeyValuePair<Part, int> p in  parts)
            {
                box = new ProductBoxHelper().getProductBoxByPartno(p.Key.SProductID, store);

                //Step 1. If part needs to seperate packages, get new box to pack. For example: Monitor, Keyboard, DinRail...
                if (box != null)
                {
                    if (box.AdditionalBox == true)
                        boxList.AddRange(singlePacking(p.Key, p.Value, box, dimensionalWeightBase, defaultCurrency));
                    else
                    {
                        //Add this box into comparison list for later decision
                        candidateBoxes.Add(box);
                        btosWeight += measureProductWeight(p.Key);
                        box.Capacity = 1;
                    }
                }
                else if (p.Key.DimensionLengthCM.GetValueOrDefault() > 0
                    && p.Key.DimensionLengthCM.GetValueOrDefault() > 0
                    && p.Key.DimensionLengthCM.GetValueOrDefault() > 0)
                {
                    //create a virtual box which is large enough to fit this item. 1 inch margin space will be reserved
                    box = new ProductBox();
                    box.LengthINCH = MeasureUnit.convertCM2IN(p.Key.DimensionLengthCM.GetValueOrDefault()) + 1;
                    box.WidthINCH = MeasureUnit.convertCM2IN(p.Key.DimensionWidthCM.GetValueOrDefault()) + 1;
                    box.HighINCH = MeasureUnit.convertCM2IN(p.Key.DimensionHeightCM.GetValueOrDefault()) + 1;
                    box.BoxID = 0;  //virtual box
                    box.StoreID = store.StoreID;
                    box.Capacity = 1;
                    btosWeight += measureProductWeight(p.Key);
                    candidateBoxes.Add(box);
                }
                else
                {
                    //create a virtual box with artificial dimension of 6x4x4 for comparison reason
                    box = new ProductBox();
                    box.LengthINCH = 6;
                    box.WidthINCH = 4;
                    box.HighINCH = 4;
                    box.BoxID = 0;  //virtual box
                    box.StoreID = store.StoreID;
                    box.Capacity = 1;
                    btosWeight += measureProductWeight(p.Key);
                    candidateBoxes.Add(box);
                }
            }

            return candidateBoxes;
        
        }


        #endregion
    }
}
