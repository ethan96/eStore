using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class MenuGlobalResourceHelper : Helper
    {
        public MenuGlobalResource getResource(int id)
        {
            var menuResource = (from m in context.MenuGlobalResources
                                where m.Id == id
                                select m).FirstOrDefault();

            return menuResource;
        }

        public MenuGlobalResource getResource(string storeid, int menuid, int lanid)
        {
            var resour = context.MenuGlobalResources.FirstOrDefault(c => c.LanguageId == lanid && c.StoreId == storeid && c.MenuId == menuid);
            return resour;
        }

        public MenuGlobalResource getResource(MenuGlobalResource source)
        {
            return getResource(source.Id);
        }

        public List<LanguageResource> getMenuResourceTemplate(string storeId, int languageId)
        {
            var menuList = (from m in context.Menus
                            from l in context.Languages
                            let mm = context.MenuGlobalResources.Where(x => x.StoreId == storeId && x.LanguageId == languageId && x.MenuId == m.MenuID).FirstOrDefault()
                            where m.StoreID == storeId && m.Publish == true && l.Id == languageId
                            select new LanguageResource
                            {
                                DocId = m.MenuID,
                                StoreId = m.StoreID,
                                DisplayKeyName = m.MenuName,
                                LanguageName = l.Name + "(" + l.Location + ")",
                                LocalName = (mm == null ? "" : mm.LocalName)
                            }).Distinct().ToList();

            List<LanguageResource> menuResourceList = new List<LanguageResource>();
            foreach (LanguageResource item in menuList)
            {
                LanguageResource menuItem = menuResourceList.FirstOrDefault(p => p.DocId == item.DocId);
                if (menuItem == null)
                    menuResourceList.Add(item);
            }
            return menuResourceList;
        }

        public MenuGlobalResource getMenuResourceByLanguageAndKey(string storeid, int menuId, int languageId)
        {
            var _menu = (from m in context.MenuGlobalResources 
                         where m.StoreId == storeid && m.MenuId == menuId && m.LanguageId == languageId
                         select m).FirstOrDefault();

            return _menu;
        } 

        internal int save(MenuGlobalResource menuGlobalResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (menuGlobalResource == null || menuGlobalResource.validate() == false) return 1;
            //Try to retrieve object from DB

            MenuGlobalResource _exist = getResource(menuGlobalResource);
            try
            {
                if (_exist == null)  //object not exist 
                {
                    //Insert
                    context.MenuGlobalResources.AddObject(menuGlobalResource);
                    context.SaveChanges();
                }
                else
                {
                    //Update
                    if (_exist.LocalName != menuGlobalResource.LocalName)
                    {
                        if (menuGlobalResource.helper != null && menuGlobalResource.helper.context != null)
                            context = menuGlobalResource.helper.context;
                        context.MenuGlobalResources.ApplyCurrentValues(menuGlobalResource);
                        context.SaveChanges();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        internal int delete(MenuGlobalResource menuGlobalResource)
        {
            MenuGlobalResource _exit = getResource(menuGlobalResource);
            try
            {
                context.MenuGlobalResources.DeleteObject(_exit);
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
