using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    /// <summary>
    /// The class is a singleton which provides packing manager instance and provides packing function and packing list
    /// PackingManager is responsible for packing products in the box, and provide packing list.
    /// </summary>
    public class PackingManager
    {
        public enum PackingType { PackingRule_TDS, PackingRule_USA, PackingRule_AEU, PackingRule_ABB };

        public PackingType packingType = PackingType.PackingRule_USA;


        //public PackingType packingType{get;set;}


        //this attribute identifies which store current PackingManager serves for
        private eStore.POCOS.Store _store;


        #region Methods
        /// <summary>
        /// Default constructor
        /// </summary>
        public PackingManager(eStore.POCOS.Store storeProfile)
        {
            _store = storeProfile;
            switch (_store.StoreID)
            {
                //case "TDS":
                //    packingType = PackingType.PackingRule_TDS;
                //    break;
                case "ABB":
                    packingType = PackingType.PackingRule_ABB;
                    break;
                default:
                    packingType = PackingType.PackingRule_USA;
                    break;
            }
        }
  
        /// <summary>
        /// Get packing list by cart. If no dimensional weight base information, please input zero.
        /// </summary>
        /// <param name="cart">cart</param>
        /// <param name="dimensionalWeightBase">dimensional weight base</param>
        /// <returns>list of Packlist</returns>
        public eStore.POCOS.PackingList getPackingList(Cart cart, decimal dimensionalWeightBase)
        {
            eStore.POCOS.PackingList packingList = null;
            lock (_store)   //there might be competion issue at context. Hereby we sync access Store to prevent simultenous access
            {
                try
                {
                    packingList = new eStore.POCOS.PackingList(cart);
                    //packingList.PackagingBoxes = packProductsIntoBox(cart, dimensionalWeightBase);
                    IPackingRule packmgt;
                    switch (packingType)
                    { 
                        case PackingType.PackingRule_TDS:
                            packmgt = new PackingRule_TDS();
                            break;
                        case PackingType.PackingRule_ABB:
                            packmgt = new PackingRule_ABB();
                            break;
                        case PackingType.PackingRule_USA:
                        default:
                            packmgt = new PackingRule_AUS();
                            break;
                    }
                    packingList.PackagingBoxes = packmgt.packing(_store, cart, dimensionalWeightBase);
                    if (packingList == null || packingList.PackagingBoxes == null || packingList.PackagingBoxes.Count == 0)
                        throw new Exception("Failed at creating packing list");
                }
                catch (Exception ex)
                {
                    //leave no trace
                    eStoreLoger.Error("Failed at creating packing list", cart.UserID, "", cart.StoreID, ex);
                }
            }
            return packingList;
        }

        /// <summary>
        /// Pack cart items into boxes
        /// </summary>
        /// <param name="_cart"></param>
        /// <returns>List of PackagingBox</returns>
        private ICollection<PackagingBox> packProductsIntoBox(Cart cart, decimal dimensionalWeightBase = 0)
        {
            String defaultCurrency = _store.defaultCurrency.CurrencyID;

            List<PackagingBox> packagingBoxes = new List<PackagingBox>();
            List<CartItem> _mergeCartItemlist = new List<CartItem>();   //Use multiple product type to merge packaging
            ProductBox box = null;
            PackingMethod packingMethod = new PackingMethod();

            try   //packing result shall be in whole. In other sense, either all or none
            {
                foreach (CartItem c in cart.cartItemsX)
                {
                    //Assigned box for the cart item                    
                    if (c.type == Product.PRODUCTTYPE.STANDARD)
                    {
                        box = new ProductBoxHelper().getProductBoxByPartno(c.SProductID, _store);

                        if (box == null)
                        {
                            //Pack in single box if prefix is 170, 96TO, MIO, ADAM 
                            string _productPrefix = "^170|^96TO|^MIO|^ADAM|^SQF|^PCL-101|^PCL-102";
                            Regex r = new Regex(_productPrefix);
                            Match m = r.Match(c.SProductID);
                            if (m.Success)  //product which can be packed in the same box within box capacity
                            {
                                //Step 1. paste tag on cart item
                                c.PackTag = CartItem.PackingTag.SingleProductTypeMergePack;

                                //Assigned a box for cart item
                                box = getDefaultBox();
                                box.Capacity = 10;
                            }
                            else
                            {
                                c.PackTag = CartItem.PackingTag.WeightPack;
                                //if can't find from box rule, then try to find by volumn.
                                box = getDefaultBox(c.partX);
                                box.Capacity = 1;
                            }
                        }
                        else
                        {
                            //use assigned box
                            c.PackTag = CartItem.PackingTag.IndividualPack;
                            box.Capacity = 1;
                        }
                    }
                    else if (c.type == Product.PRODUCTTYPE.CTOS)
                    {
                        c.PackTag = CartItem.PackingTag.CtosPack;
                    }
                    else if (c.type == Product.PRODUCTTYPE.BUNDLE)
                    {
                        c.PackTag = CartItem.PackingTag.BundlePack;
                    }

                    switch (c.PackTag)
                    {
                        case CartItem.PackingTag.SingleProductTypeMergePack:
                            //step 1. assign rule
                            packingMethod.PackingRule = PackingMethod.Rule.FairPacking;
                            //step 2. get boxes
                            packagingBoxes.AddRange(packingMethod.getDefaultBox(c, box, dimensionalWeightBase, defaultCurrency));
                            break;

                        case CartItem.PackingTag.MultipleProductTypeMergePack:
                            // Reserve to use
                            break;

                        case CartItem.PackingTag.IndividualPack:
                            packingMethod.PackingRule = PackingMethod.Rule.SinglePacking;
                            packagingBoxes.AddRange(packingMethod.getDefaultBox(c, box, dimensionalWeightBase, defaultCurrency));
                            break;

                        case CartItem.PackingTag.WeightPack:
                            packingMethod.PackingRule = PackingMethod.Rule.SinglePacking;
                            packagingBoxes.AddRange(packingMethod.getDefaultBox(c, box, dimensionalWeightBase, defaultCurrency));
                            break;

                        case CartItem.PackingTag.CtosPack:
                            packingMethod.PackingRule = PackingMethod.Rule.SinglePacking;
                            packagingBoxes.AddRange(packingMethod.getCtosBox(c, dimensionalWeightBase, _store, defaultCurrency));
                            break;
                        case CartItem.PackingTag.BundlePack:
                            packingMethod.PackingRule = PackingMethod.Rule.SinglePacking;
                            packagingBoxes.AddRange(packingMethod.getBundleBox(c, dimensionalWeightBase, _store, defaultCurrency));
                            break;
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

        private ProductBox getDefaultBox()
        {
            ProductBox box = null;

            box = new ProductBoxHelper().getDefaultBox(_store);
            if (box == null)    //no default box or smallest box found
            {
                //If store can't get the default box, then use AUS default box;
                box = new ProductBoxHelper().getDefaultBox("AUS");
            }

            if (box == null)
            {
                eStoreLoger.Fatal("Failed to get the default smallest box by store", "", "", _store.StoreID);
                throw new Exception("Failed to get the default smallest box by store");
            }

            return box;
        }
        private ProductBox getDefaultBox(POCOS.Part part)
        {
            ProductBox box = null;

            decimal volumn = 0m;
            if (part != null
          && part.DimensionHeightCM.GetValueOrDefault() > 0
          && part.DimensionLengthCM.GetValueOrDefault() > 0
          && part.DimensionWidthCM.GetValueOrDefault() > 0)
            {
                volumn = MeasureUnit.convertCM2IN(part.DimensionHeightCM.GetValueOrDefault())
                  * MeasureUnit.convertCM2IN(part.DimensionLengthCM.GetValueOrDefault())
                  * MeasureUnit.convertCM2IN(part.DimensionWidthCM.GetValueOrDefault());
                box = new ProductBoxHelper().findPackingBoxByVolumn(volumn);
            }

            if (box == null)
            {
                box = getDefaultBox();
                if (volumn > 0 && volumn > box.volumn)
                {
                    box = new ProductBox();
                    box.LengthINCH = MeasureUnit.convertCM2IN(part.DimensionLengthCM.GetValueOrDefault()) + 1;
                    box.WidthINCH = MeasureUnit.convertCM2IN(part.DimensionWidthCM.GetValueOrDefault()) + 1;
                    box.HighINCH = MeasureUnit.convertCM2IN(part.DimensionHeightCM.GetValueOrDefault()) + 1;
                    box.BoxID = 0;  //virtual box
                    box.StoreID = part.StoreID;
                }
            }

            return box;
        }
        #endregion
    }
}