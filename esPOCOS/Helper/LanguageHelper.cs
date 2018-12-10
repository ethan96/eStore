using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.POCOS.DAL
{
    public class LanguageHelper : Helper
    {
        public Language getLangByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;
            return context.Languages.FirstOrDefault(c => c.Code.Equals(code,StringComparison.OrdinalIgnoreCase));
        }

        public Language getLangById(int id) 
        {
            return context.Languages.FirstOrDefault(c => c.Id == id);
        }

        public List<Language> getLanguageByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var languageList = (from l in context.Languages
                                  where l.Name.StartsWith(name)
                                  select l).ToList();

            return languageList;
        }

        public List<Language> getLanguageByStore(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
                return null;

            var languageList = (from s in context.StoreLanguages
                                join l in context.Languages on s.LanguageId equals l.Id
                                where s.StoreId == storeId 
                                select l).ToList();
            
            return languageList;
        }
    }
}
