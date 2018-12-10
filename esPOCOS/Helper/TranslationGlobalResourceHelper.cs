using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TranslationGlobalResourceHelper : Helper
    {
        #region Business
        public TranslationGlobalResource getTranslationResourceByLanguageAndKey(string storeid, string key, int languageId)
        {
            var _tran = (from tran in context.TranslationGlobalResources
                         where tran.StoreId == storeid && tran.Key == key && tran.LanguageId == languageId 
                         select tran).FirstOrDefault();
            if (_tran != null)
                _tran.helper = this;
            return _tran;
        }

        public TranslationGlobalResource getTranslationResourceById(int id)
        {
            var _tran = (from tran in context.TranslationGlobalResources
                         where tran.Id == id
                         select tran).FirstOrDefault();

            if (_tran != null)
                _tran.helper = this;
            return _tran;
        }

        public List<LanguageResource> getTranslationResourceTemplate(string storeId, int languageId)
        {
            var translationList = (from t in context.Translations 
                               from l in context.Languages 
                               let tt = context.TranslationGlobalResources.Where(x => x.StoreId == storeId && x.LanguageId == languageId && x.Key == t.Key).FirstOrDefault()
                               where t.StoreID == storeId && l.Id == languageId 
                               select new LanguageResource
                               {
                                   DocId = t.TranslateID,
                                   StoreId = t.StoreID,
                                   DisplayKeyName = t.Key,
                                   DisplayKeyValue = t.Value,
                                   LanguageName = l.Name + "(" + l.Location + ")",
                                   LocalName = (tt == null ? "" : tt.LocalName)
                               }).Distinct().ToList();

            List<LanguageResource> tranList = new List<LanguageResource>();
            foreach (LanguageResource item in translationList)
            {
                LanguageResource tranItem = tranList.FirstOrDefault(p => p.DocId == item.DocId);
                if (tranItem == null)
                    tranList.Add(item);
            }
            return tranList;
        }
        #endregion


        #region Creat Update Delete
        public int save(TranslationGlobalResource _translation)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_translation == null || _translation.validate() == false) return 1;
            //Try to retrieve object from DB
            TranslationGlobalResource _exist_translation = new TranslationGlobalResourceHelper().getTranslationResourceById(_translation.Id);
            try
            {
                if (_exist_translation == null)  //object not exist 
                {
                    context.TranslationGlobalResources.AddObject(_translation);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update
                    if (_translation.helper != null && _translation.helper.context != null)
                        context = _translation.helper.context;
                    context.TranslationGlobalResources.ApplyCurrentValues(_translation);
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

        public int delete(TranslationGlobalResource _translation)
        {

            if (_translation == null || _translation.validate() == false) return 1;
            try
            {
                if (_translation.helper != null && _translation.helper.context != null)
                    context = _translation.helper.context;

                context.DeleteObject(_translation);
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

        #region Others

        private static string myclassname()
        {
            return typeof(TranslationGlobalResourceHelper).ToString();
        }
        #endregion
    }
}
