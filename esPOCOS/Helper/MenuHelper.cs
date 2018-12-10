using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class MenuHelper : Helper
    {

        #region Business Read
        public List<Menu> getMenusByStoreid(string storeid)
        {
            var _menus = (from menu in context.Menus.Include("SubMenus").Include("MiniSite")
                          where menu.StoreID == storeid && menu.ParentMenuID == null
                          orderby menu.Sequence, menu.MenuName
                          select menu);

            foreach (Menu m in _menus)
            {
                m.helper = this;
            }

            return _menus.ToList();

        }

        public List<Menu> getAllMenusByStoreid(string storeid)
        {
            var _menus = (from menu in context.Menus
                          where menu.StoreID == storeid && menu.Publish == true
                          orderby menu.Sequence, menu.MenuName
                          select menu);

            foreach (Menu m in _menus)
            {
                m.helper = this;
            }

            return _menus.ToList();

        }

        public List<Menu> getCachedMenus(string storeid)
        {
            string key = string.Format("{0}.CachedMenus", storeid);
            var cachedmenus = CachePool.getInstance().getObject(key);
            if (cachedmenus != null)
            { 
            return ( List<Menu> )cachedmenus;
            }
            else
            {
                var _menus = (from menu in context.Menus.Include("MiniSite").Include("MenuGlobalResources")
                              where menu.StoreID == storeid && menu.Publish == true
                              select menu
                                  ).ToList();
                CachePool.getInstance().cacheObject(key, _menus);
                return _menus;
            }
        }

        public List<Menu> getMenusByStoreid(string storeid, POCOS.Menu.MenuPosition position , Boolean isFromCache = true)
        {
            string sPosition = position.ToString();
            List<Menu> _menus = new List<Menu>();
            if (isFromCache)
            {
                _menus = (from menu in getCachedMenus(storeid)
                          where menu.ParentMenuID == null
                          && menu.Position.ToUpper().EndsWith(sPosition.ToUpper())
                          orderby menu.Sequence, menu.MenuName
                          select menu).ToList(); ;
            }
            else
            {
                _menus = (from menu in context.Menus.Include("SubMenus").Include("MiniSite").Include("MenuGlobalResources")
                          where menu.StoreID == storeid && menu.ParentMenuID == null
                          && menu.Position.ToUpper().EndsWith(sPosition.ToUpper())
                          orderby menu.Sequence, menu.MenuName
                          select menu).ToList();
                foreach (Menu m in _menus)
                {
                    m.helper = this;
                }
            }
            return _menus;

        }

        public Menu getMenusByid(String storeid, int id)
        {
            var _menu = (from menu in context.Menus
                         where menu.StoreID == storeid && menu.MenuID == id
                         select menu).FirstOrDefault();

            if (_menu!=null)
            _menu.helper = this;
            return _menu;
        }


        public List<Menu> getMenusByWidgetID( int widgetID)
        {
            string s = widgetID.ToString();
            var _menus = (from menu in context.Menus
                         where menu.MenuType == "WidgetPage" && menu.CategoryPath == s
                         select menu).ToList();

            foreach (Menu m in _menus)
            {
                m.helper = this;
            }
            return _menus.ToList();
        }


        #endregion

        #region Creat Update Delete
        public int save(Menu _menu)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_menu == null || _menu.validate() == false) return 1;
            //Try to retrieve object from DB

            Menu _exist_menu = new MenuHelper().getMenusByid(_menu.StoreID, _menu.MenuID);
            try
            {
                if (_exist_menu == null)  //object not exist 
                {
                    //Insert
                    context.Menus.AddObject(_menu);
                    context.SaveChanges();
                }
                else
                {
                    //Update
                    int count = _exist_menu.MenuGlobalResources.Count; // 解决delete MenuGlobalResource 的错误
                    context.Menus.ApplyCurrentValues(_menu);
                    context.SaveChanges();
                }
                saveGlobalResource(_menu, _exist_menu);
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        // 解决delete MenuGlobalResource 的错误
        private void saveGlobalResource(Menu _menu,Menu _odlMenu)
        {
            foreach (var c in _odlMenu.MenuGlobalResources)
            {
                if (_menu.MenuGlobalResources.FirstOrDefault(t => t.Id == c.Id) == null)
                    c.delete();
            }
        }

        public int delete(Menu _menu)
        {

            if (_menu == null || _menu.validate() == false) return 1;
          try
            {
                 _menu.helper.context.DeleteObject(_menu);
                _menu.helper.context.SaveChanges();
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
            return typeof(MenuHelper).ToString();
        }
        #endregion
    }
}