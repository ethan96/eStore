using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class ScenarioCategoryHelper : Helper
    {
        public eStore3Entities6 context;

        public ScenarioCategoryHelper()
        {
            context = new eStore3Entities6();
        }

        ~ScenarioCategoryHelper()
        {
            if (context != null)
                context.Dispose();
        }

        public int delete(ScenarioCategory scenarioCategory)
        {
            if (scenarioCategory == null || scenarioCategory.validate() == false) return 1;
            try
            {
                foreach (var item in scenarioCategory.ScenarioProductCategoryMappings)
                {
                    item.delete();
                }
                var _exitobj = getScenarioCategoryById(scenarioCategory.ScenarioCategoryID);
                if (_exitobj != null)
                {
                    context.ScenarioCategories.DeleteObject(_exitobj);
                    context.SaveChanges();
                }
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int save(ScenarioCategory scenarioCategory)
        {
            if (scenarioCategory == null || scenarioCategory.validate() == false) return 1;
            ScenarioCategory _exist = getScenarioCategoryById(scenarioCategory.ScenarioCategoryID);
            try
            {
                if (_exist == null)  //object not exist 
                {
                    //Insert
                    context.ScenarioCategories.AddObject(scenarioCategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    snycObj(ref _exist, scenarioCategory);
                    context.ScenarioCategories.ApplyCurrentValues(_exist);
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

        private void snycObj(ref ScenarioCategory oldObj,ScenarioCategory newObj)
        {
            oldObj.ScenarioCategoryName = newObj.ScenarioCategoryName;
            oldObj.Sequence = newObj.Sequence;
        }

        public ScenarioCategory getScenarioCategoryById(int scID)
        {
            var item = (from c in context.ScenarioCategories
                        where c.ScenarioCategoryID == scID
                        select c).FirstOrDefault();
            return item;
        }

        public List<ScenarioCategory> getScenarioCategoryByCategoryID(int categoryid, string storeid)
        {
            var ls = (from c in context.ScenarioCategories
                      where c.CategoryID.Equals(categoryid) && c.Storeid.Equals(storeid)
                      select c).ToList();
            foreach (var c in ls)
                c.helper = this;
            return ls;
        }

        public List<ScenarioCategory> getScenarioCategoryByCategoryID(ProductCategory category)
        {
            return getScenarioCategoryByCategoryID(category.CategoryID, category.Storeid);
        }

    }
}
