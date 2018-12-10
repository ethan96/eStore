using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class CTOSComponentMappingHelper : Helper
    {
        //get CTOSComponentMapping by componentMappingID
        public CTOSComponentMapping getComponentMappingByID(int componentMappingID)
        {
            if (string.IsNullOrEmpty(componentMappingID.ToString()))
                return null;
            CTOSComponentMapping _componentMapping = (from c in context.CTOSComponentMappings
                                                      where c.ID == componentMappingID
                                                      select c).FirstOrDefault();
            if (_componentMapping != null)
                _componentMapping.helper = this;
            return _componentMapping;
        }

        //get ComponentMappingList by storeIDFrom and storeIDTo
        public List<CTOSComponentMapping> getComponentMappingList(string storeIDFrom, string storeIDTo, int mscomid = -1, int scomid = -1)
        {
            List<CTOSComponentMapping> _componentMappingList = (from c in context.CTOSComponentMappings
                                                                where c.StoreIDFrom == storeIDFrom && c.StoreIDTo == storeIDTo
                                                                      && (mscomid == -1 ? 1 == 1 : c.ComponentIDFrom == mscomid) 
                                                                      && (scomid == -1 ? 1 == 1 : c.ComponentIDTo == scomid)
                                                                select c).ToList();
            if (_componentMappingList != null)
                return _componentMappingList;
            else
                return new List<CTOSComponentMapping>();
        }

        //get ComponentMapping by componentIDFrom, componentIDToTo,storeIDFrom,storeIDTo
        public CTOSComponentMapping getComponentMapping(int componentIDFrom, int componentIDToTo, string storeIDFrom, string storeIDTo)
        {
            CTOSComponentMapping _componentMapping = (from c in context.CTOSComponentMappings
                                                          where c.ComponentIDFrom == componentIDFrom && c.ComponentIDTo == componentIDToTo
                                                          && c.StoreIDFrom == storeIDFrom && c.StoreIDTo == storeIDTo
                                                          select c).FirstOrDefault();
            if (_componentMapping != null)
                return _componentMapping;
            else
                return new CTOSComponentMapping();
        }


        //保存有关CTOSComponentMapping的更改
        public int save(CTOSComponentMapping _componentMapping)
        {
            if (_componentMapping == null && _componentMapping.validate() == false)
                return 1;
            CTOSComponentMapping __componentMapping_exist = getComponentMappingByID(_componentMapping.ID);
            try
            {
                if (__componentMapping_exist == null)
                {
                    context.CTOSComponentMappings.AddObject(_componentMapping);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    context.CTOSComponentMappings.ApplyCurrentValues(_componentMapping);
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

        //删除指定的CTOSComponentMapping
        public int delete(CTOSComponentMapping _componentMapping)
        {
            if (_componentMapping == null || _componentMapping.validate() == false) return 1;
            try
            {
                CTOSComponentMapping c = getComponentMappingByID(_componentMapping.ID);
                if (c.CTOSComponentFrom.isCategory()) //删除from category 下面所有子category 匹配
                { 
                    foreach(var com in c.CTOSComponentFrom.CTOSComponent1)
                    {
                        var fromComs = getComponentMappingList(c.StoreIDFrom, c.StoreIDTo, com.ComponentID);
                        if (fromComs.Any())
                        {
                            foreach (var co in fromComs)
                                co.delete();
                        }
                    }
                }
                if (c.CTOSComponentTo.isCategory()) //删除to category 下面所有子category 匹配
                {
                    foreach (var tocom in c.CTOSComponentTo.CTOSComponent1)
                    {
                        var tocoms = getComponentMappingList(c.StoreIDFrom, c.StoreIDTo, -1, tocom.ComponentID);
                        if (tocoms.Any())
                        {
                            foreach (var toco in tocoms)
                                toco.delete();
                        }
                    }
                }
                if (c == null)
                    return -5000;
                context.DeleteObject(c);
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
