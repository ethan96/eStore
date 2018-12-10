using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class BoxHelper : Helper
    {
        #region Business Read
        public   Box getBoxByPartnoPre(string partnoPrefix, Store _store)
        {
            if (string.IsNullOrEmpty(partnoPrefix)) return null;

            try
            {

                var _ProductBox = (from s in context.ProductBoxRules
                                   join b in context.Boxes on s.BoxID equals b.BoxID
                                   where (s.PartNoPrefix == partnoPrefix && b.StoreID == _store.StoreID)
                                   select b).FirstOrDefault();


                return _ProductBox;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// Return all boxes belongs to this storeid
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>

        public List<Box> getBoxes(string storeid)
        {
            try
            {
                var _productboxrule = (from x in context.Boxes
                                       where x.StoreID.Equals(storeid)
                                       select x);

                if (_productboxrule != null)
                {
                    foreach (Box box in _productboxrule)
                        box.helper = this;

                    return _productboxrule.OrderBy(p => p.WidthINCH).ThenBy(p => p.LengthINCH).ThenBy(p => p.HighINCH).ToList();
                }
                else
                    return new List<Box>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }

        }

        #endregion

        #region Creat Update Delete
        public   int save(Box _box)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_box == null || _box.validate() == false) return 1;
            //Try to retrieve object from DB
            ProductBoxRule _exist_rule = null;
            try
            {
                if (_exist_rule == null)  //object not exist 
                {
                    context.Boxes.AddObject(_box);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(ProductBoxRule _boxrule)
        {

            if (_boxrule == null || _boxrule.validate() == false) return 1;
            try
            {
                context.DeleteObject(_boxrule);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        }

        public int delete(Box  _box)
        {

            if (_box == null || _box.validate() == false) return 1;
            try
            {
                context.DeleteObject(_box);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(BoxHelper).ToString();
        }
        #endregion
    }
}