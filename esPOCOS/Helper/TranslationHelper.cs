using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TranslationHelper : Helper
    {
        #region Business
        public Translation getTranslationByIdAndValue(string storeid, int tid, string keyValue)
        {
            var _tran = (from tran in context.Translations
                         where tran.StoreID == storeid && (tran.TranslateID == tid || tran.Value == keyValue)
                         select tran).FirstOrDefault();

            if (_tran != null)
                _tran.helper = this;
            return _tran;
        }

        public Translation getTranslationByKey(string storeid, string key)
        {
            var _tran = (from tran in context.Translations
                         where tran.StoreID == storeid && tran.Key  == key
                         select tran).FirstOrDefault();

            if (_tran != null)
                _tran.helper = this;
            return _tran;
        }
        public List<Translation> getTranslationByStore(string storeid)
        {
            var _tran = (from tran in context.Translations.Include("TranslationGlobalResources")
                         where tran.StoreID == storeid 
                         select tran).ToList ();

            List<Translation> translations = null;
            if (_tran != null)
                translations = _tran.ToList<Translation>();
            else
                translations = new List<Translation>();

            foreach (Translation t in translations)
                t.helper = this;

            return translations;
        }

        public List<Translation> getSeriesTranslation(string storeid, string prefix) {

            var _tran = (from tran in context.Translations
                         where tran.StoreID == storeid &&tran.Key.StartsWith(prefix)
                         select tran).ToList();

            List<Translation> translations = null;
            if (_tran != null)
                translations = _tran.ToList<Translation>();
            else
                translations = new List<Translation>();

            foreach (Translation t in translations)
                t.helper = this;

            return translations;
        }
        #endregion


        #region Creat Update Delete
        public int save(Translation _translation)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_translation == null || _translation.validate() == false) return 1;
            //Try to retrieve object from DB
            Translation _exist_translation = new TranslationHelper().getTranslationByKey(_translation.StoreID, _translation.Key ); ;
            try
            {
                if (_exist_translation == null)  //object not exist 
                {
                    context.Translations.AddObject(_translation);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update
                    context.Translations.ApplyCurrentValues(_translation);
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

        public int delete(Translation _translation)
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
            return typeof(TranslationHelper).ToString();
        }
        #endregion
    }
}
