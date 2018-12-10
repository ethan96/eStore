using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.Sync;
using eStore.POCOS;

namespace eStore.PIS.Helper 
{
    public class PISCategoryHelper
    {
        private List<GetPISFullCategory_Result> _allPisCategory;
        public List<GetPISFullCategory_Result> AllPisCategory
        {
            get 
            {
                if (_allPisCategory == null)
                {
                    PISEntities pc = new PISEntities();
                    try
                    {
                        _allPisCategory = IsBBeStore ? 
                            (from c in pc.GetPISFullBBCategory()
                                            select c).ToList() :
                            (from c in pc.GetPISFullCategory()
                                           select c).ToList();
                    }
                    catch (Exception ex)
                    {
                        _allPisCategory = new List<GetPISFullCategory_Result>();
                        throw ex;
                    }
                }
                return _allPisCategory; 
            }
            set { _allPisCategory = value; }
        }
        private bool isbbestore;
        public bool IsBBeStore
        {
            get
            {
                return this.isbbestore;
            }
            set
            {
                this.isbbestore = value;
            }
        }


        public static PISCategoryHelper Prof
        {
            get
            {
                PISCategoryHelper prof;
                var cacheHelper = CachePool.getInstance().getObject("cachePISCategoryHelper");
                if (cacheHelper == null)
                {
                    prof = new PISCategoryHelper();
                    CachePool.getInstance().cacheObject("cachePISCategoryHelper", prof);
                }
                else
                    prof = cacheHelper as PISCategoryHelper;
                return prof;
            }
        }

        private PISCategoryHelper()
        {

        }

        public List<GetPISFullCategory_Result> getAllMainCategory(bool isBB = false)
        {
            IsBBeStore = isBB;
            return AllPisCategory.Where(c => c.isMainCategory).ToList();
        }
    }
}
