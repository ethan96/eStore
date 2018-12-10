using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ApplicationSpotHelper : Helper
    {
        public eStore3Entities6 context;

        public ApplicationSpotHelper()
        {
            context = new eStore3Entities6();
        }
        
        #region Business Read
        public ApplicationSpot getApplicationSpotByID(int id)
        {
            try
            {
                var _ApplicationSpot = (from b in context.ApplicationSpots
                                where b.ApplicationSpotID == id
                                select b).FirstOrDefault();
                if (_ApplicationSpot != null)
                    _ApplicationSpot.helper = this;

                return _ApplicationSpot;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        #endregion

        #region Creat Update Delete
        public int delete(int id)
        {
            ApplicationSpot _applicationSpotItem = getApplicationSpotByID(id);
            if (_applicationSpotItem == null || _applicationSpotItem.validate() == false) return 1;
            try
            {
                context = _applicationSpotItem.helper.context;
                //context.DeleteObject(_applicationSpotItem);
                context.ApplicationSpots.DeleteObject(_applicationSpotItem);
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
            return typeof(ApplicationSpotHelper).ToString();
        }
        #endregion
    }
}