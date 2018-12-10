using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation
{
    public static class eStoreGlobalResource
    {

        public static string getLocalCategoryName(ProductCategory pc, Language lang = null)
        {
            var currLang = lang ?? eStoreContext.Current.CurrentLanguage;
            string localname = pc.localCategoryNameX;
            if (currLang != null && pc.CategoriesGlobalResources.Any(x => x.LanguageId == currLang.Id))
            {
                var cc = pc.CategoriesGlobalResources.FirstOrDefault(x => x.LanguageId == currLang.Id).LocalName;
                if (!string.IsNullOrEmpty(cc))
                    localname = cc;
            }
            return localname;
        }

        public static string getLocalCategoryDescription(ProductCategory pc, Language lang = null)
        {
            string localDec = pc.descriptionX;
            var currLang = lang ?? eStoreContext.Current.CurrentLanguage;
            if (currLang != null && pc.CategoriesGlobalResources.Any(x => x.LanguageId == currLang.Id))
            {
                var cc = pc.CategoriesGlobalResources.FirstOrDefault(x => x.LanguageId == currLang.Id).LocalDescription;
                if (!string.IsNullOrEmpty(cc))
                    localDec = cc;
            }
            return localDec;
        }

        public static string getLocalCategoryExtDescription(ProductCategory pc, Language lang = null)
        {
            string localDec = pc.extendedDescX;
            var currLang = lang ?? eStoreContext.Current.CurrentLanguage;
            if (currLang != null && pc.CategoriesGlobalResources.Any(x => x.LanguageId == currLang.Id))
            {
                var cc = pc.CategoriesGlobalResources.FirstOrDefault(x => x.LanguageId == currLang.Id).LocalExtDescription;
                if (!string.IsNullOrEmpty(cc))
                    localDec = cc;
            }
            return localDec;
        }
    }
}
