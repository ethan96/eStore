using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class OMMenuHelper : Helper
    {
        #region Business Read

        /// <summary>
        /// Get all OM menus
        /// </summary>
        /// <returns></returns>
        public List<OMMenu> getOMMenus()
        {

            try
            {
                var _m = from m in context.OMMenus
                         select m;
                return _m.ToList();

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// get on om menu by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OMMenu getOMMenuByID(int id)
        {
            try
            {
                var _m = (from m in context.OMMenus
                         where m.ID == id
                         select m).FirstOrDefault();
                if (_m != null)
                    _m._omhelper = this;
                return _m;
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// get id by ommenuName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OMMenu getOMMenuByName(string name)
        {
            try
            {
                var _m = from m in context.OMMenus
                         where m.Name == name
                         select m;
                return _m.FirstOrDefault();

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        /// <summary>
        /// get OM MENU by Virtual URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public OMMenu getOMMenuByActionURLandMinisite(string url,MiniSite miniSite)
        {
            try
            {
                var _m = from m in context.OMMenus
                       where (m.ActionURL == url && m.MiniSite == miniSite)
                       select m;
                return _m.FirstOrDefault();
            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }


        /// <summary>
        /// This method is to retrieve OM first level menu items
        /// </summary>
        /// <returns></returns>
        public List<OMMenu> getOMRootMenus()
        {

            try
            {
#if uStore
                var _m = from m in context.OMMenus.Include("SubMenus")
                         where m.ParentID == null && m.AccessRight.StartsWith("uStore.")==true
                         select m;
#else
                var _m = from m in context.OMMenus.Include("SubMenus")
                         where m.ParentID == null&& (m.AccessRight.StartsWith("uStore.")==false|| m.MiniSiteID !=null)
                         select m;
#endif
                if (_m != null && _m.Any())
                {
                    foreach (var m in _m)
                        m.Omhelper = this;
                }
                return _m.ToList();

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
            return typeof(OMMenuHelper).ToString();
        }
        #endregion



        #region Creat Update Delete
        public int save(OMMenu _ommenu)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_ommenu == null || _ommenu.validate() == false) return 1;
            //Try to retrieve object from DB

            OMMenu _exist_menu = new OMMenuHelper().getOMMenuByID(_ommenu.ID);
            try
            {
                if (_exist_menu == null)  //object not exist 
                {
                    //Insert
                    context.OMMenus.AddObject(_ommenu);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.OMMenus.ApplyCurrentValues(_ommenu);
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

        public int delete(OMMenu _ommenu)
        {

            if (_ommenu == null || _ommenu.validate() == false) return 1;
            try
            {
                _ommenu.Omhelper.context.DeleteObject(_ommenu);
                _ommenu.Omhelper.context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

    }
}