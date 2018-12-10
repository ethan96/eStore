using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class LanguagePackHelper : Helper { 
        #region Business Read

    public List<LanguagePack> getCachedLanguagePacks()
    { 
        CachePool cache = CachePool.getInstance();
        string key="PStoreLanguagePacks";
        List<LanguagePack> lps=(  List<LanguagePack>)cache.getObject(key);
        if(lps==null)
        {
            lock (context)
            {
                try
                {
                    lps = context.LanguagePacks.ToList();
                }
                catch (Exception)
                {

                    lps = new List<LanguagePack>();
                }
                cache.cacheObject(key, lps, CachePool.CacheOption.Hour12);
            }
        }
        return lps; 
    }
 

    public string Translate(string objectName, int objectId, int languageId, string fieldName, string originalFieldValue)
    {
        var result = string.Empty;
        var translate = getCachedLanguagePacks().Where(x => string.Compare(objectName.Trim(), x.ObjectName.Trim(), StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault();
        if (translate != null)
            result = translate.Value;
        else
            result = originalFieldValue;
        return result;
    }

        public LanguagePack get(int id)
        {
            return context.LanguagePacks.FirstOrDefault(x => x.Id == id);
        }
        public bool isExists(int id)
        {
            return context.LanguagePacks.Any(x => x.Id == id);
        }
        #endregion
        #region Creat Update Delete
        public int save(LanguagePack _languagepack)
        {
      
            //if parameter is null or validation is false, then return  -1 
            if (_languagepack == null || _languagepack.validate() == false) return 1;
            //Try to retrieve object from DB
             
            try
            {
                if (_languagepack.Id ==0 || !isExists(_languagepack.Id))  //object not exist 
                {
                    //Insert
                    context.LanguagePacks.AddObject( _languagepack);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.LanguagePacks.ApplyCurrentValues( _languagepack);
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
        public int delete(LanguagePack _languagepack)
        {
       
            if (_languagepack == null || _languagepack.validate() == false) return 1;
            try
            {
                context.DeleteObject(_languagepack);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                 eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion
	} 
 }