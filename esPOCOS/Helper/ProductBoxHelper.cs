using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ProductBoxHelper : Helper
    {
        #region Business Read
        public ProductBox getProductBoxByPartno(string partno, Store _store)
        {
            if (string.IsNullOrEmpty(partno)) return null;

            try
            {
                var _ProductBox = (from s in context.ProductBoxes
                                   where (partno.StartsWith(s.PartNoPrefix) && s.StoreID == _store.StoreID)
                                   orderby  s.PartNoPrefix.Length descending
                                   select s).FirstOrDefault();

                return _ProductBox;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// This method is to find the most compact box that matches the input box volumn.
        /// The measure unit of the input volumn is in INCH
        /// </summary>
        /// <param name="volumn"></param>
        /// <returns></returns>
        public ProductBox findPackingBoxByVolumn(decimal volumn)
        {
            try
            {
                var _ProductBox = (from s in context.ProductBoxes
                                   where (s.HighINCH*s.WidthINCH*s.LengthINCH>volumn)
                                   orderby s.HighINCH * s.WidthINCH * s.LengthINCH
                                   select s).FirstOrDefault();

                return _ProductBox;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        public List<ProductBox> getStoreAvailableBox(string storeid)
        {
            try
            {
                var _StoreAvailableBoxs = (from s in context.ProductBoxes
                                           where s.StoreID == storeid
                                           && !s.PartNoPrefix.StartsWith("EKI-")
                                           select s).ToList();

                return _StoreAvailableBoxs;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return  new List<ProductBox>();
            }
        }
        public ProductBox getClosestStoreBox(ProductBox box, string  store)
        {
            try
            {
                List<ProductBox> _StoreAvailableBoxs = this.getStoreAvailableBox(store);
                var _ProductBox = (from s in _StoreAvailableBoxs
                                   where box.CompareTo(s)<1
                                   orderby s.volumn
                                   select s).FirstOrDefault();

                if (_ProductBox == null)
                    return box;
                else

                    return _ProductBox;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return box;
            }
        
        }

        public ProductBox findPackingBoxByVolumn(POCOS.Part part)
        {
            try
            {
                if (part != null
                   && part.DimensionHeightCM.GetValueOrDefault() > 0
                   && part.DimensionLengthCM.GetValueOrDefault() > 0
                   && part.DimensionWidthCM.GetValueOrDefault() > 0)
                {
                    List<decimal> dimention = new List<decimal>();
                    dimention.Add(MeasureUnit.convertCM2IN(part.DimensionHeightCM.GetValueOrDefault()));
                    dimention.Add(MeasureUnit.convertCM2IN(part.DimensionLengthCM.GetValueOrDefault()));
                    dimention.Add(MeasureUnit.convertCM2IN(part.DimensionWidthCM.GetValueOrDefault()));
                    dimention.Sort();
                    decimal volumn = dimention[0] * dimention[1] * dimention[2];
                    var _ProductBox = (from s in context.ProductBoxes
                                       where (s.HighINCH * s.WidthINCH * s.LengthINCH > volumn)
                                       && s.HighINCH >= dimention[0]
                                        && s.LengthINCH >= dimention[1]
                                         && s.WidthINCH >= dimention[2]
                                       orderby s.HighINCH * s.WidthINCH * s.LengthINCH
                                       select s).FirstOrDefault();
                    return _ProductBox;
                }
                else if (part.ShipWeightKG > 0)
                {
                    return null; //return findPackingBoxByVolumn(part.ShipWeightKG.GetValueOrDefault() * 5000);
                }
                else
                {
                    return null;
                }

              
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        public ProductBox getDefaultBox(Store _store)
        {
            if (_store == null) return null;

            return getDefaultBox(_store.StoreID);
        }

        public ProductBox getDefaultBox(String storeID)
        {
            if (String.IsNullOrEmpty(storeID)) return null;

            try
            {
                var _ProductBox = (from box in context.ProductBoxes
                                   where box.StoreID.Equals(storeID)
                                   && box.Default == true
                                   select box).FirstOrDefault();

                return _ProductBox;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        #endregion



        #region Others

        private static string myclassname()
        {
            return typeof(ProductBoxHelper).ToString();
        }
        #endregion
    }
}