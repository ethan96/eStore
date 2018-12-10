using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class UserShortCutHelper : Helper
    {
        #region Business Read        

        public UserShortCut getUserShortCutByID(int shortcutid)
        {
            try
            {
                var _shortcut = (from usc in context.UserShortCuts
                               where usc.ShortCutID == shortcutid
                                 select usc).FirstOrDefault();

                if (_shortcut != null)
                    _shortcut.helper = this;

                return _shortcut;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }        
    
               
    
        #endregion

        #region Creat Update Delete


        public int save(UserShortCut _usershortcut )
        { 
            //if parameter is null or validation is false, then return  -1 
            if (_usershortcut == null || _usershortcut.validate() == false) return 1;

            //Try to retrieve object from DB  
            UserShortCut _exist_shortCut = getUserShortCutByID(_usershortcut.ShortCutID);
 
            try
            {

                if (_exist_shortCut == null)
                {
                    context.UserShortCuts .AddObject(_usershortcut); //state=added.
                    context.SaveChanges();
                    return 0;
                }
                else //Update 
                {
                    context.UserShortCuts.ApplyCurrentValues(_usershortcut); //Even applycurrent value, cartmaster state still in unchanged.                 
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


        public int delete(UserShortCut _shortcut)
        {
            if (_shortcut == null || _shortcut.validate() == false) return 1;

             UserShortCut _exit_shortcut = getUserShortCutByID(_shortcut.ShortCutID);

            try
            {
                context.DeleteObject(_exit_shortcut);
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
            return typeof(WidgetPageHelper ).ToString();
        }
        #endregion
    }
}