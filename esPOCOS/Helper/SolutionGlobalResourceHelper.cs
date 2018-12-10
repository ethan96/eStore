using System;
using System.Collections.Generic;
using System.Linq;
using eStore.Utilities;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class SolutionGlobalResourceHelper : Helper
    {
        internal int save(SolutionGlobalResource solutionGlobalResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (solutionGlobalResource == null || solutionGlobalResource.validate() == false) return 1;
            //Try to retrieve object from DB
            SolutionGlobalResource _exist_solutionGlobalResource = getSolutionGlobalResource(solutionGlobalResource.SolutionId, solutionGlobalResource.LanguageId);
            try
            {
                if (_exist_solutionGlobalResource == null)  //object not exist 
                {
                    //Insert
                    context.SolutionGlobalResources.AddObject(solutionGlobalResource);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.SolutionGlobalResources.ApplyCurrentValues(solutionGlobalResource);
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

        public SolutionGlobalResource getSolutionGlobalResource(int solutionId, int languageId)
        {
            SolutionGlobalResource solutionGlobalResource = context.SolutionGlobalResources.FirstOrDefault(s => s.SolutionId == solutionId && s.LanguageId == languageId);
            return solutionGlobalResource;
        }

        public int delete(SolutionGlobalResource solutionGlobalResource)
        {

            if (solutionGlobalResource == null || solutionGlobalResource.validate() == false) return 1;
            POCOS.SolutionGlobalResource sgr = (from c in context.SolutionGlobalResources
                                               where c.Id == solutionGlobalResource.Id
                                                select c).FirstOrDefault();
            try
            {
                context = solutionGlobalResource.helper.context;
                context.DeleteObject(sgr);
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
