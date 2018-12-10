using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class SpecialProductPriceHelper : Helper
    {
        /// <summary>
        /// 根据id获取SpecialProductPrice信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SpecialProductPrice getSpecialProductPriceById(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return null;
            try
            {
                var result = (from p in context.SpecialProductPrices
                              where p.Id == id
                              select p).FirstOrDefault();
                if (result != null)
                    result.Helper = this;
                return result;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取所有SpecialProductPrice
        /// </summary>
        /// <returns></returns>
        public List<SpecialProductPrice> getSpecialProductPriceList(string storeid)
        {
            List<SpecialProductPrice> _specialProductPrice = new List<SpecialProductPrice>();
            try
            {
                _specialProductPrice = (from p in context.SpecialProductPrices
                                        where p.StoreId == storeid
                                        select p).ToList();
                foreach (SpecialProductPrice price in _specialProductPrice)
                    price.Helper = this;
                return _specialProductPrice;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public int save(SpecialProductPrice specialProductPrice)
        {
            //if parameter is null or validation is false, then return  -1 
            if (specialProductPrice == null || specialProductPrice.validate() == false) return 1;
            //Try to retrieve object from DB
            SpecialProductPrice _exist_camp = getSpecialProductPriceById(specialProductPrice.Id);
            try
            {
                if (specialProductPrice.Helper != null && specialProductPrice.Helper.context != null)
                    context = specialProductPrice.Helper.context;
                if (_exist_camp == null)  //object not exist 
                {
                    context.SpecialProductPrices.AddObject(specialProductPrice); //state=added.                            
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.SpecialProductPrices.ApplyCurrentValues(specialProductPrice);
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

        public int delete(SpecialProductPrice specialProductPrice)
        {
            if (specialProductPrice == null || specialProductPrice.validate() == false) return 1;
            //specialProductPrice will not be permenently removed.  Instead it'll be marked as deleted
            try
            {
                context = specialProductPrice.Helper.context;
                context.DeleteObject(specialProductPrice);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
