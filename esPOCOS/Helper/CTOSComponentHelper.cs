using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class CTOSComponentHelper : Helper
    {

        #region Unit Test Methods
        private static int _nextAvailableId = 10000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentName"></param>
        /// <param name="componentType">0 : category and 1 : option</param>
        /// <returns></returns>
        public static CTOSComponent createDummyCTOSComponent(String componentName, String desc, int componentType)
        {
            CTOSComponent component = new CTOSComponent();
            component.ComponentID = nextAvailableId;
            component.ComponentName = componentName;
            component.ComponentDesc = desc;
            if (componentType == 0)
                component.ComponentType = "category";
            else
                component.ComponentType = "list";

            return component;
        }

        private static int nextAvailableId
        {
            get
            {
                return _nextAvailableId++;
            }
        }

        #endregion

        #region Business Read
        public CTOSComponent getbyID(int id,string storeid)
        {
            var _ctoscom = (from c in context.CTOSComponents
                            where c.ComponentID == id
                            && c.StoreID == storeid
                            select c).FirstOrDefault();
            if (_ctoscom != null)
                _ctoscom.helper = this;
            return _ctoscom;
        }

        /// <summary>
        /// For OM. get root components
        /// </summary>
        /// <returns></returns>

        public List<CTOSComponent> getRootCTOSComponent(string storeid)
        {
            var _rootcomponents = (from c in context.CTOSComponents
                                   where c.ComponentParentID == null && c.StoreID == storeid
                                   select c);

            if (_rootcomponents != null)
            {
                foreach (CTOSComponent cc in _rootcomponents)
                    cc.helper = this;
                return _rootcomponents.ToList();
            }
            else
            {
                return new List<CTOSComponent>();
            }

        }
        //根据sproductId 获取使用 它的CTOSComponent
        public List<CTOSComponent> getCTOSComponentByPartNo(string storeId, string sproductId)
        {
            var origcomponets = (from component in context.CTOSComponents
                                join componentDetail in context.CTOSComponentDetails on component.ComponentID equals componentDetail.ComponentID
                                    where component.StoreID == storeId && componentDetail.StoreID == storeId
                                    && componentDetail.SProductID == sproductId
                                    select component).ToList();

            if (origcomponets != null)
                return origcomponets.ToList();
            else
                return new List<CTOSComponent>();
        }

        /// <summary>
        /// get mapping table 
        /// </summary>
        /// <param name="storefrom"></param>
        /// <param name="storeto"></param>
        /// <param name="source"></param>
        /// <returns>target compent id = -1 is not find in target store</returns>
        public Dictionary<int, int> getNotInTargetStoreComponents(string storeto, Product_Ctos source)
        {
            Dictionary<int, int> mappings = new Dictionary<int, int>();
            var sourcecomids = source.CTOSBOMs.Select(c => c.ComponentID).ToList();

            var origcomponets = (from sourcemapp in context.CTOSComponentMappings
                                 where sourcemapp.StoreIDFrom == source.StoreID && sourcecomids.Contains(sourcemapp.ComponentIDFrom)
                                    && sourcemapp.StoreIDTo == storeto
                                 select sourcemapp).ToList();
            foreach (var m in origcomponets)
            {
                mappings.Add(m.ComponentIDFrom, m.ComponentIDTo);
                sourcecomids.Remove(m.ComponentIDFrom);
            }

            var componets = (from com in context.CTOSComponents
                             where com.StoreID == storeto && sourcecomids.Contains(com.ComponentID)
                             select com.ComponentID).ToList();

            foreach (var comid in componets)
            {
                mappings.Add(comid, comid);
                sourcecomids.Remove(comid);
            }

            foreach (var notincom in sourcecomids)
                mappings.Add(notincom, -1); //-1 is not find in target store

            return mappings;
        }

        /// <summary>
        /// 传递三个参数，如果scomid=-1，则获取ComponentID为mcomid的category的数据
        /// 如果scomid不是-1的话，责获取ComponentID为scomid的list数据
        /// </summary>
        /// <param name="fromStoreId"></param>
        /// <param name="mcomid"></param>
        /// <param name="scomid"></param>
        /// <returns></returns>
        public List<CTOSComponent> getFromStoreCTOSComponent(string fromStoreId, int mcomid)
        {
            var _components = (from c in context.CTOSComponents
                               where c.StoreID.ToLower() == fromStoreId.ToLower() && c.ComponentID == mcomid
                               select c).ToList();

            if (_components != null)
            {
                foreach (CTOSComponent cc in _components)
                    cc.helper = this;
                return _components.ToList();
            }
            else
            {
                return new List<CTOSComponent>();
            }
        }

        /// <summary>
        /// 传递三个参数，如果scomid=-1，则获取category的数据
        /// 如果scomid不是-1的话，责获取ComponentID为scomid的list数据
        /// </summary>
        /// <param name="fromStoreId"></param>
        /// <param name="mcomid"></param>
        /// <param name="scomid"></param>
        /// <returns></returns>
        public List<CTOSComponent> getToStoreIDCTOSComponent(string toStoreId, int scomid = -1)
        {
            var _components = (from c in context.CTOSComponents
                               where c.StoreID.ToLower() == toStoreId.ToLower() &&
                                    (scomid == -1 ? (1 == 1 && c.ComponentType == "category") :
                                    (1 == 1 && c.ComponentType == "list" && c.ComponentID == scomid))
                               select c);

            if (_components != null)
            {
                foreach (CTOSComponent cc in _components)
                    cc.helper = this;
                return _components.ToList();
            }
            else
            {
                return new List<CTOSComponent>();
            }
        }

        public List<CTOSComponent> getSimilarCTOSComponents(string storefrom, string storeto, string productid)
        {
            var origcomponets = (from origcom in context.CTOSComponents
                                 join odetail in context.CTOSComponentDetails on origcom equals odetail.CTOSComponent into orig
                                 from com in context.CTOSComponents.Include("CTOSComponentMappingsTo").Include("CTOSComponentDetails")
                                 join mapping in context.CTOSComponentMappings on com equals mapping.CTOSComponentTo into cms
                                 from cm in cms.DefaultIfEmpty()
                                 join ddetail in context.CTOSComponentDetails on com equals ddetail.CTOSComponent into dest
                                 from bom in context.CTOSBOMs
                                 from oc in orig.DefaultIfEmpty()
                                 from dc in dest.DefaultIfEmpty()
                                 where origcom.StoreID == bom.StoreID
                                 && origcom.ComponentID == bom.ComponentID
                                 && bom.SProductID == productid
                                 && bom.StoreID == storefrom
                                 && com.StoreID == storeto
                                 && (
                                    (cm != null && cm.ComponentIDFrom == origcom.ComponentID &&cm.StoreIDFrom == origcom.StoreID )
                                     ||
                                     (oc != null && dc != null && oc.SProductID == dc.SProductID && oc.Qty == dc.Qty)
                                     ||
                                     (oc == null && origcom.ComponentName == com.ComponentName
                                     )
                                 )
                                 select com
                                   );
                           

          
            if (origcomponets != null)
            {
                foreach (CTOSComponent cc in origcomponets)
                    cc.helper = this;
                return origcomponets.ToList();
            }
            else
            {
                return new List<CTOSComponent>();
            }
          

        }

        public CTOSComponent getMappingComponent(string storefrom, string storeto, int componentidto)
        {
            var origcomponets = (from origcom in context.CTOSComponents
                                 join odetail in context.CTOSComponentDetails on origcom equals odetail.CTOSComponent into orig
                                 from com in context.CTOSComponents.Include("CTOSComponentMappingsTo").Include("CTOSComponentDetails")
                                 join mapping in context.CTOSComponentMappings on com equals mapping.CTOSComponentTo into cms
                                 from cm in cms.DefaultIfEmpty()
                                 join ddetail in context.CTOSComponentDetails on com equals ddetail.CTOSComponent into dest
                                 
                                 from oc in orig.DefaultIfEmpty()
                                 from dc in dest.DefaultIfEmpty()
                                 where com.ComponentID == componentidto
                                 && com.StoreID == storeto
                                 && (
                                    (cm != null && cm.ComponentIDFrom == origcom.ComponentID && cm.StoreIDFrom == origcom.StoreID)
                                     ||
                                     (oc != null && dc != null && oc.SProductID == dc.SProductID && oc.Qty == dc.Qty)
                                     ||
                                     (oc == null && origcom.ComponentName == com.ComponentName
                                     )
                                 )
                                 select origcom
                                      ).FirstOrDefault();
            return origcomponets;
        }


        #endregion

        #region Creat Update Delete
        public int save(CTOSComponent _ctoscomponent)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_ctoscomponent == null || _ctoscomponent.validate() == false) return 1;
            //Try to retrieve object from DB
            CTOSComponent _exist_ctoscomponent = getbyID(_ctoscomponent.ComponentID, _ctoscomponent.StoreID);
            try
            {
                if (_exist_ctoscomponent == null)  //object not exist 
                {
                    //Insert
                    context.CTOSComponents.AddObject(_ctoscomponent);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.CTOSComponents.ApplyCurrentValues(_ctoscomponent);
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

        public int delete(CTOSComponent _ctoscomponent)
        {
            CTOSComponent _co = _ctoscomponent;

            if (_ctoscomponent == null || _ctoscomponent.validate() == false) return 1;
            try
            {
                if (_co.CTOSComponentMappingsFrom.Any())
                {
                    foreach (var c in _co.CTOSComponentMappingsFrom)
                        c.delete();
                }
                if (_co.CTOSComponentMappingsTo.Any())
                {
                    foreach (var c in _co.CTOSComponentMappingsTo)
                        c.delete();
                }

                _co = (from x in context.CTOSComponents
                       where x.ComponentID == _ctoscomponent.ComponentID
                            && x.StoreID == _ctoscomponent.StoreID
                       select x).FirstOrDefault();

                context.DeleteObject(_co);
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
            return typeof(CTOSComponentHelper).ToString();
        }
        #endregion
    }
}