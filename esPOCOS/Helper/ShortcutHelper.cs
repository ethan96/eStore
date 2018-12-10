using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ShortcutHelper : Helper
    {
        #region Business
        public Shortcut getShortcutByName(string storeId,string name)
        {
            var _tran = (from s in context.Shortcuts
                         where s.StoreID == storeId && s.Name.ToUpper() == name.ToUpper()
                         select s).FirstOrDefault();

            if (_tran != null)
                _tran.helper = this;
            return _tran;
        }

        public List<Shortcut> getShortcutList(string storeId, string name = "", string link = "")
        {
            var shortCutList = (from s in context.Shortcuts
                                where s.StoreID == storeId
                                && (!string.IsNullOrEmpty(name) ? s.Name == name : true)
                                && (!string.IsNullOrEmpty(link) ? s.Link == link : true)
                                orderby s.CreatedDate descending
                                select s).ToList();
            foreach (Shortcut item in shortCutList)
            {
                item.helper = this;
            }
            return shortCutList;
        }
        #endregion

        #region Creat Update Delete
        public int save(Shortcut _shortCut)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_shortCut == null || _shortCut.validate() == false) return 1;
            //Try to retrieve object from DB
            Shortcut _exist_Shortcut = getShortcutByName(_shortCut.StoreID,_shortCut.Name);
            try
            {
                if (_exist_Shortcut == null)  //object not exist 
                {
                    context.Shortcuts.AddObject(_shortCut);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update
                    if (_shortCut.helper != null && _shortCut.helper.context != null)
                        context = _shortCut.helper.context;
                    context.Shortcuts.ApplyCurrentValues(_shortCut);
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

        public int delete(Shortcut _shortCut)
        {

            if (_shortCut == null || _shortCut.validate() == false) return 1;
            try
            {
                if (_shortCut.helper != null && _shortCut.helper.context != null)
                    context = _shortCut.helper.context;

                context.DeleteObject(_shortCut);
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
            return typeof(ShortcutHelper).ToString();
        }
        #endregion
    }
}
