using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class ScenarioProductCategoryMappingHelper : Helper
    {
        public eStore3Entities6 context;

        public ScenarioProductCategoryMappingHelper()
        {
            context = new eStore3Entities6();
        }
        
        #region Business Read
        public ScenarioProductCategoryMapping getScenarioProductCategoryMappingByID(int id)
        {
            try
            {
                var _scenarioProductCategoryMapping = (from b in context.ScenarioProductCategoryMappings
                                where b.ScenarioProductCategoryMappingID == id
                                select b).FirstOrDefault();
                if (_scenarioProductCategoryMapping != null)
                    _scenarioProductCategoryMapping.helper = this;

                return _scenarioProductCategoryMapping;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
        #endregion

        #region Creat Update Delete
        public int delete(ScenarioProductCategoryMapping scm)
        {
            ScenarioProductCategoryMapping _scenarioProductCategoryMapping = getScenarioProductCategoryMappingByID(scm.ScenarioProductCategoryMappingID);
            if (_scenarioProductCategoryMapping == null || _scenarioProductCategoryMapping.validate() == false) return 1;
            try
            {
                context = _scenarioProductCategoryMapping.helper.context;
                //context.DeleteObject(_scenarioProductCategoryMapping);
                context.ScenarioProductCategoryMappings.DeleteObject(_scenarioProductCategoryMapping);
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
            return typeof(ScenarioProductCategoryMappingHelper).ToString();
        }
        #endregion
    }
}