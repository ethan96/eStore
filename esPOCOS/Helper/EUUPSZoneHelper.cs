using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS
{
    public class EUUPSZoneHelper : Helper
    {
        /// <summary>
        /// this function is to find eu ups price by zone 
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public List<EUUPSPrice> getEUUPSPriceByZone(EUUPSZone zone)
        {
            if (zone == null || string.IsNullOrEmpty(zone.ZoneCode) || string.IsNullOrEmpty(zone.Method))
                return new List<EUUPSPrice>();

            var priceList = (from c in context.EUUPSPrices
                             where c.Method == zone.Method && c.ZoneCode == zone.ZoneCode
                             select c).ToList();
            return priceList;
        }

        /// <summary>
        /// this function is to find eu ups zone list
        /// </summary>
        /// <returns></returns>
        public List<EUUPSZone> getEUUPSPriceList()
        {
            List<EUUPSZone> _euUPSZone = new List<EUUPSZone>();

            var priceList = (from c in context.EUUPSZones
                             select c).ToList();
            return priceList;
        }

        /// <summary>
        /// this function is to find one eu ups zone list by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EUUPSZone getEUUPSZoneByID(int id)
        {
            EUUPSZone _euUPSZone = null;
            _euUPSZone = (from c in context.EUUPSZones
                          where c.ID == id
                          select c).FirstOrDefault();
            if (_euUPSZone != null)
                _euUPSZone.helper = this;
            return _euUPSZone;
        }

        /// <summary>
        /// 以ZoneCode  Method  countryName为条件  查询zone
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="method"></param>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public List<EUUPSZone> getEUUPSZoneListByField(string zoneCode, string method,string countryName)
        {
            if (string.IsNullOrEmpty(zoneCode) || string.IsNullOrEmpty(method))
                return null;
            if (zoneCode == "ALL" && method == "ALL"&&countryName=="ALL")//查找所有数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method == "ALL"&&countryName=="ALL")//查找zoneCode的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.ZoneCode == zoneCode
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode == "ALL" && method != "ALL"&&countryName=="ALL")//查找method的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.Method == method
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode == "ALL" && method == "ALL" && countryName != "ALL")//查找country的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.CountryName == countryName
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method != "ALL" && countryName == "ALL")//查找zoneCode和method的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.Method == method && c.ZoneCode == zoneCode
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method == "ALL" && countryName != "ALL")//查找zoneCode和country的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.ZoneCode == zoneCode && c.CountryName == countryName
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode == "ALL" && method != "ALL" && countryName != "ALL")//查找method和countryName的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.Method == method && c.CountryName == countryName
                                   select c).ToList();
                return _euUPSPrice;
            }
            else if (zoneCode != "ALL" && method != "ALL" && countryName != "ALL")//查找zoneCode method countryName的数据
            {
                var _euUPSPrice = (from c in context.EUUPSZones.OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ThenBy
                                       (p => p.CountryName).ThenBy(p => p.PostcodeFrom).ThenBy(p => p.PostcodeEnd)
                                   where c.Method == method && c.ZoneCode == zoneCode && c.CountryName == countryName
                                   select c).ToList();
                return _euUPSPrice;
            }
            else
                return new List<EUUPSZone>();
        }

        //保存有关EUUPSZone的更改
        public int save(EUUPSZone _euUPSZone)
        {
            if (_euUPSZone == null && _euUPSZone.validate() == false)
                return 1;
            EUUPSZone _euUPSPrice_exist = getEUUPSZoneByID(_euUPSZone.ID);
            try
            {
                if (_euUPSPrice_exist == null)
                {
                    context.EUUPSZones.AddObject(_euUPSZone);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context.EUUPSZones.ApplyCurrentValues(_euUPSZone);
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

        //删除指定的EUUPSZone
        public int delete(EUUPSZone _euUPSZone)
        {

            if (_euUPSZone == null || _euUPSZone.validate() == false) return 1;
            try
            {
                context.DeleteObject(_euUPSZone);
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
