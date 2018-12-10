using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public class CategoriesGlobalResourceHelper : Helper
    {
        public CategoriesGlobalResource getCategoryGlobalResource(int id)
        {
            if (id == 0)
                return null;
            var categoryResource = (from m in context.CategoriesGlobalResources
                                where m.Id == id
                                select m).FirstOrDefault();
            if (categoryResource != null)
                categoryResource.helper = this;
            return categoryResource;
        }

        public CategoriesGlobalResource getCategoryResourceByLanguageAndKey(string storeid, int categoryId, int languageId)
        {
            var _category = (from c in context.CategoriesGlobalResources
                             where c.StoreId == storeid && c.Categoryid == categoryId && c.LanguageId == languageId
                             select c).FirstOrDefault();
            if (_category != null)
                _category.helper = this;
            return _category;
        }

        internal int save(CategoriesGlobalResource categoryGlobalResource)
        {
            //if parameter is null or validation is false, then return  -1 
            if (categoryGlobalResource == null || categoryGlobalResource.validate() == false) return 1;
            //Try to retrieve object from DB

            CategoriesGlobalResource _exist = getCategoryGlobalResource(categoryGlobalResource.Id);
            try
            {
                if (_exist == null)  //object not exist 
                {
                    //Insert
                    context.CategoriesGlobalResources.AddObject(categoryGlobalResource);
                    context.SaveChanges();
                }
                else
                {
                    if (categoryGlobalResource.helper != null && categoryGlobalResource.helper.context != null)
                        context = categoryGlobalResource.helper.context;
                    context.CategoriesGlobalResources.ApplyCurrentValues(categoryGlobalResource);
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

        internal int delete(CategoriesGlobalResource categoryGlobalResource)
        {
            CategoriesGlobalResource _exit = getCategoryGlobalResource(categoryGlobalResource.Id);
            try
            {
                context.CategoriesGlobalResources.DeleteObject(_exit);
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
