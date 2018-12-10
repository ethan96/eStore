using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{
    public class PolicyCategoryHelper : Helper
    {

        internal int save(PolicyCategory policyCategory)
        {
            //if parameter is null or validation is false, then return  -1 
            if (policyCategory == null || policyCategory.validate() == false) return 1;
            //Try to retrieve object from DB
            PolicyCategory _exist_policycategory = getPolicyCategoryById(policyCategory.Id, policyCategory.StoreID);
            try
            {
                if (_exist_policycategory == null)  //object not exist 
                {
                    //Insert
                    context.PolicyCategories.AddObject(policyCategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.PolicyCategories.ApplyCurrentValues(policyCategory);
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

        public List<POCOS.PolicyCategory> getPolicyCategory(string storeid)
        {
            return context.PolicyCategories.Where(c=>c.StoreID == storeid).ToList();
        }        

        public PolicyCategory getPolicyCategoryById(int p,string storeid)
        {
            PolicyCategory pc = context.PolicyCategories.Include("Policy").FirstOrDefault(c => c.Id == p && c.StoreID == storeid);
            if (pc != null)
                pc.helper = this;
            return pc;
        }
        public PolicyCategory getPolicyCategoryByUrl(string url, string storeid)
        {
            PolicyCategory pc = context.PolicyCategories.Include("Policy").FirstOrDefault(c =>!string.IsNullOrEmpty(c.CustomerURL) &&  c.CustomerURL.Equals(url,StringComparison.OrdinalIgnoreCase)&& c.StoreID == storeid);
            if (pc != null)
                pc.helper = this;
            return pc;
        }
        public List<POCOS.PolicyCategory> getRootPolicyCategory(string storeid)
        {
            return context.PolicyCategories.Include("SubCategories").Where(c => c.ParentId == null && c.StoreID == storeid && c.Name != "Root").ToList();
        }

        public PolicyCategory getFirstPolicyCategoryByName(string name, string storeid)
        {
            PolicyCategory pc = context.PolicyCategories.Include("Policy").FirstOrDefault(c => c.Name == name && c.StoreID == storeid);
            if (pc != null)
                pc.helper = this;
            return pc;
        }

        public int delete(PolicyCategory _policyCategory)
        {

            if (_policyCategory == null || _policyCategory.validate() == false) return 1;
            POCOS.PolicyCategory policyCategory = (from p in context.PolicyCategories
                                                   where p.Id == _policyCategory.Id
                                                   select p).FirstOrDefault();
            try
            {
                if (policyCategory != null)
                {
                    foreach (var item in policyCategory.PolicyCategoryGlobalResources.ToList())
                    {
                        policyCategory.PolicyCategoryGlobalResources.Remove(item);
                        item.delete();
                    }

                    context.DeleteObject(policyCategory);
                    context.SaveChanges();
                    return 0;
                }
                else
                    return -5000;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
