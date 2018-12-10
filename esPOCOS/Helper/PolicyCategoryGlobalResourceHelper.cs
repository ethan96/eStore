using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class PolicyCategoryGlobalResourceHelper : Helper
    {
        internal int save(PolicyCategoryGlobalResource policyCategoryGlobalResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (policyCategoryGlobalResource == null || policyCategoryGlobalResource.validate() == false) return 1;
            //Try to retrieve object from DB
            PolicyCategoryGlobalResource _exist_policyCategoryGlobalResource = getPolicyCategoryGlobalResource(policyCategoryGlobalResource.LanguageId, policyCategoryGlobalResource.PolicyCategoryId);
            try
            {
                if (_exist_policyCategoryGlobalResource == null)  //object not exist 
                {
                    //Insert
                    context.PolicyCategoryGlobalResources.AddObject(policyCategoryGlobalResource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PolicyCategoryGlobalResources.ApplyCurrentValues(policyCategoryGlobalResource);
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

        public PolicyCategoryGlobalResource getPolicyCategoryGlobalResource(int languageId, int policyCategoryId)
        {
            return context.PolicyCategoryGlobalResources.FirstOrDefault(c => c.LanguageId == languageId && c.PolicyCategoryId == policyCategoryId);
        }

        public int delete(PolicyCategoryGlobalResource _policyCategoryGlobalResource)
        {

            if (_policyCategoryGlobalResource == null || _policyCategoryGlobalResource.validate() == false) return 1;
            POCOS.PolicyCategoryGlobalResource policyCategoryGlobalResource = (from p in context.PolicyCategoryGlobalResources
                                                               where p.Id == _policyCategoryGlobalResource.Id
                                                               select p).FirstOrDefault();
            try
            {
                context.DeleteObject(policyCategoryGlobalResource);
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
