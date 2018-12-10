using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ScenarioSpotMappingHelper : Helper
    {
        public eStore3Entities6 context;

        public ScenarioSpotMappingHelper()
        {
            context = new eStore3Entities6();
        }
        
        #region Business Read
        public ScenarioSpotMapping getScenarioSpotMappingByID(int id)
        {
            try
            {
                var _ScenarioSpotMappingSpot = (from b in context.ScenarioSpotMappings
                                where b.ScenarioSpotID == id
                                select b).FirstOrDefault();
                if (_ScenarioSpotMappingSpot != null)
                    _ScenarioSpotMappingSpot.helper = this;

                return _ScenarioSpotMappingSpot;
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
            ScenarioSpotMapping _scenarioSpotMappingItem = getScenarioSpotMappingByID(id);
            if (_scenarioSpotMappingItem == null || _scenarioSpotMappingItem.validate() == false) return 1;
            try
            {
                context = _scenarioSpotMappingItem.helper.context;
                //context.DeleteObject(_applicationSpotItem);
                context.ScenarioSpotMappings.DeleteObject(_scenarioSpotMappingItem);
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
            return typeof(ScenarioSpotMappingHelper).ToString();
        }
        #endregion
    }
}