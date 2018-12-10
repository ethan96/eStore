using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class PolicyHelper : Helper
    {
        public int delete(Policy _policy)
        {

            if (_policy == null || _policy.validate() == false) return 1;
            POCOS.Policy policy = (from p in context.Policies
                                  where p.Id == _policy.Id
                                   select p).FirstOrDefault();
            try
            {
                foreach(var item in policy.PolicyGlobalResources.ToList())
                {
                    policy.PolicyGlobalResources.Remove(item);
                    item.delete();                    
                }
                foreach (var category in policy.PolicyCategories.ToList())
                {
                    category.delete();
                }


                context.DeleteObject(policy);
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
