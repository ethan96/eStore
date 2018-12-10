using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.PIS.Helper;

namespace eStore.POCOS.Sync
{
    public partial class GetPISFullCategory_Result
    {
        public bool isMainCategory
        {
            get
            {
                return "ROOT".Equals((this.PARENT_CATEGORY_ID ?? "").ToUpper());
            }
        }

        private List<GetPISFullCategory_Result> _subCategorys;
        public List<GetPISFullCategory_Result> SubCategorys
        {
            get 
            {
                if (_subCategorys == null)
                {
                    _subCategorys = (from c in PISCategoryHelper.Prof.AllPisCategory
                                     where c.PARENT_CATEGORY_ID.Equals(this.category_id)
                                     select c).ToList();
                }
                return _subCategorys; 
            }
            set { _subCategorys = value; }
        }

        private GetPISFullCategory_Result _mainCategory;
        public GetPISFullCategory_Result MainCategory
        {
            get 
            {
                if (!isMainCategory)
                {
                    _mainCategory = (from c in PISCategoryHelper.Prof.AllPisCategory
                                    where c.category_id.Equals(this.PARENT_CATEGORY_ID)
                                    select c).FirstOrDefault();
                }
                return _mainCategory; 
            }
            set { _mainCategory = value; }
        }
    }
}
