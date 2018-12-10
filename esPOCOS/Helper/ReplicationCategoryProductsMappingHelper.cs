using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class ReplicationCategoryProductsMappingHelper : Helper
    {
        public ReplicationCategoryProductsMapping getMap(ReplicationCategoryProductsMapping map)
        {
            return context.ReplicationCategoryProductsMappings.FirstOrDefault(c => c.StoreIDFrom == map.StoreIDFrom
                && c.StoreIDTo == map.StoreIDTo && c.CategoryIDTo == map.CategoryIDTo && c.CategoryIDFrom == map.CategoryIDFrom);
        }

        public ReplicationCategoryProductsMapping getMapById(int id)
        {
            return context.ReplicationCategoryProductsMappings.FirstOrDefault(c => c.ID == id);
        }


        public List<ReplicationCategoryProductsMapping> getMaps(string sourStore, int sourCateId, string tarStore)
        {
            return context.ReplicationCategoryProductsMappings.Where(c => c.StoreIDFrom == sourStore
                && c.StoreIDTo == tarStore && c.CategoryIDFrom == sourCateId).ToList();
        }

        internal int save(ReplicationCategoryProductsMapping peplicationCategoryProductsMapping)
        {
            if (peplicationCategoryProductsMapping == null || peplicationCategoryProductsMapping.validate() == false)
                return 1;
            try
            {
                var exitMap = getMap(peplicationCategoryProductsMapping);
                if (exitMap == null)
                {
                    context.ReplicationCategoryProductsMappings.AddObject(peplicationCategoryProductsMapping);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }  
        }

        internal int delete(ReplicationCategoryProductsMapping peplicationCategoryProductsMapping)
        {
            if (peplicationCategoryProductsMapping == null || peplicationCategoryProductsMapping.validate() == false) return 1;
            try
            {
                ReplicationCategoryProductsMapping rpm = getMapById(peplicationCategoryProductsMapping.ID);
                if (rpm != null)
                {
                    context.ReplicationCategoryProductsMappings.DeleteObject(rpm);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return -5000;
                
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public List<ReplicationCategoryProductsMapping> getMappingListByStoreTo(string storeid)
        { 
            var ls = (from c in context.ReplicationCategoryProductsMappings
                        where c.StoreIDTo == storeid
                          select c).ToList();
            return ls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromStoreid"></param>
        /// <param name="toStoreid"></param>
        /// <returns></returns>
        public List<ReplicationCategoryProductsMapping> getMappingListByFromStoreAndToStore(string fromStoreid, string toStoreid)
        {
            var ls = (from c in context.ReplicationCategoryProductsMappings
                      where c.StoreIDFrom == fromStoreid && c.StoreIDTo == toStoreid
                      select c).ToList();
            return ls;
        }
    }
}
