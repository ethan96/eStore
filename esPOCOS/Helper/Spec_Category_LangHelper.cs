using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class Spec_Category_LangHelper : Helper
    {
        internal int save(Spec_Category_Lang lang)
        {
            if (lang == null || lang.validate() == false) return 1;
            Spec_Category_Lang _exist = GetSpecCategoryLangByCategoryIDAndStoreID(lang.CATEGORY_ID, lang.StoreID);
            try
            {
                if (_exist == null)  //object not exist 
                {
                    //Insert
                    context.Spec_Category_Lang.AddObject(lang);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    _exist.Local_Displayname = lang.Local_Displayname;
                    context.Spec_Category_Lang.ApplyCurrentValues(_exist);
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

        public Spec_Category_Lang GetSpecCategoryLangByCategoryIDAndStoreID(int categoryID, string storeID)
        {
            var ls = (from c in context.Spec_Category_Lang
                      where c.CATEGORY_ID == categoryID && c.StoreID == storeID
                      orderby c.ID
                      select c).FirstOrDefault();
            return ls;
        }
    }
}
