using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class EUUPSPriceHelper:Helper
    {
        /// <summary>
        /// 通过zoneCode获取EUUPSPrice
        /// </summary>
        /// <param name="zoneCode">zoneCode</param>
        /// <returns></returns>
        public EUUPSPrice getEUUPSPriceByZoneCode(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            EUUPSPrice _euUPSPrice = null;
            try
            {
                if (_euUPSPrice == null)
                {
                    _euUPSPrice = (from p in context.EUUPSPrices
                                   where p.ZoneCode == zoneCode
                                   select p).FirstOrDefault();
                }
                else
                {
                    _euUPSPrice.helper = this;
                }
                return _euUPSPrice;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
       
        //获取EUUPSPrice列表
        public List<EUUPSPrice> getEUUPSPriceList()
        {
            List<EUUPSPrice> _euUPSPrice = null;
            try
            {
                if (_euUPSPrice == null)
                {
                    _euUPSPrice = (from p in context.EUUPSPrices
                                   select p).ToList();
                }
                else
                {
                    foreach (EUUPSPrice p in _euUPSPrice.ToList())
                        p.helper = this;
                }
                return _euUPSPrice;
            }
            catch(Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        //通过id获取相应的price
        public EUUPSPrice getEUUPSPriceByID(int id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
                return null;
            EUUPSPrice _euUPSPrice = null;
            _euUPSPrice = (from c in context.EUUPSPrices
                          where c.ID == id
                          select c).FirstOrDefault();
            if (_euUPSPrice != null)
                _euUPSPrice.helper = this;
            return _euUPSPrice;
        }

        //通过关键字,判断是否已有选项存在
        public bool isRepeat(string fieldKey,string para)
        {
            bool repeat = false;
            if (string.IsNullOrEmpty(fieldKey))
                return repeat = false;
            if (fieldKey == "ZoneCode")
            {
                var zoneCode = (from c in context.EUUPSZones
                                where c.ZoneCode==para
                                select c.ZoneCode).Distinct().ToList();
                if (zoneCode.Count>0)
                    return repeat = true;
            }
            else if (fieldKey == "Method")
            {
                var method = (from c in context.EUUPSZones
                              where c.Method==para
                              select c.Method).Distinct().ToList();
                if (method.Count>0)
                    return repeat = true;
            }
            else
                return repeat = false;
            return repeat;
        }

        //显示ZoneCode和Method下的价格表
        public List<EUUPSPrice> getEUUPSPriceListByField(string zoneCode,string method)
        {
            if (string.IsNullOrEmpty(zoneCode) || string.IsNullOrEmpty(method))
                return null;
            if (zoneCode == "ALL" && method == "ALL")//查找所有数据
            {
                var _euUPSPrice = (from c in context.EUUPSPrices.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.StartKG).ThenBy(p => p.EndKG).ThenBy(p => p.Price)
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode == "ALL" && method != "ALL")//查找method的数据
            {
                var _euUPSPrice = (from c in context.EUUPSPrices.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.StartKG).ThenBy(p => p.EndKG).ThenBy(p => p.Price)
                                   where c.Method == method
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method == "ALL")//查找zoneCode的数据
            {
                var _euUPSPrice = (from c in context.EUUPSPrices.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.StartKG).ThenBy(p => p.EndKG).ThenBy(p => p.Price)
                                   where c.ZoneCode == zoneCode
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method != "ALL")
            {
                var _euUPSPrice = (from c in context.EUUPSPrices.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.StartKG).ThenBy(p => p.EndKG).ThenBy(p => p.Price)
                                   where c.Method == method && c.ZoneCode == zoneCode//查找zoneCode和method的数据
                                   select c).ToList();
                return _euUPSPrice;
            }
            else
                return new List<EUUPSPrice>();
        }

        //保存有关EUUPSPrice的更改
        public int save(EUUPSPrice _euUPSPrice)
        {
            if (_euUPSPrice == null && _euUPSPrice.validate() == false)
                return 1;
            EUUPSPrice _euUPSPrice_exist = getEUUPSPriceByID(_euUPSPrice.ID);
            try
            {
                if (_euUPSPrice_exist == null)
                {
                    context.EUUPSPrices.AddObject(_euUPSPrice);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context.EUUPSPrices.ApplyCurrentValues(_euUPSPrice);
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

        //删除指定的EUUPSPrice
        public int delete(EUUPSPrice _euUPSPrice)
        {
            if (_euUPSPrice == null || _euUPSPrice.validate() == false) return 1;
            try
            {
                context.DeleteObject(_euUPSPrice);
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
