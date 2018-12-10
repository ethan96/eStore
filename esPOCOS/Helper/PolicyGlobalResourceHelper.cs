using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class PolicyGlobalResourceHelper:Helper
    {
        internal int save(PolicyGlobalResource policyGlobalResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (policyGlobalResource == null || policyGlobalResource.validate() == false) return 1;
            //Try to retrieve object from DB
            PolicyGlobalResource _exist_policyGlobalResource = getPolicyGlobalResource(policyGlobalResource.LanguageId, policyGlobalResource.PolicyId);
            try
            {
                if (_exist_policyGlobalResource == null)  //object not exist 
                {
                    //Insert
                    context.PolicyGlobalResources.AddObject(policyGlobalResource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PolicyGlobalResources.ApplyCurrentValues(policyGlobalResource);
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

        public int delete(PolicyGlobalResource _policyGlobalResource)
        {

            if (_policyGlobalResource == null || _policyGlobalResource.validate() == false) return 1;
            POCOS.PolicyGlobalResource policyGlobalResource = (from p in context.PolicyGlobalResources
                                   where p.Id == _policyGlobalResource.Id
                                   select p).FirstOrDefault();
            try
            {
                context.DeleteObject(policyGlobalResource);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public PolicyGlobalResource getPolicyGlobalResource(int languageId, int policyId)
        {
            return context.PolicyGlobalResources.FirstOrDefault(c => c.LanguageId == languageId && c.PolicyId == policyId);
        }
    }
}
