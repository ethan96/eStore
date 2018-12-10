using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class StoreErrorCodeGlobalResourceHelper : Helper
    {
        #region Business
        public StoreErrorCodeGlobalResource getErrorCodeResourceById(int id)
        {
            var _errorCode = (from e in context.StoreErrorCodeGlobalResources
                              where e.Id == id
                              select e).FirstOrDefault();
            if (_errorCode != null)
                _errorCode.helper = this;
            return _errorCode;
        }

        public StoreErrorCodeGlobalResource getErrorCodeResourceByLanguageAndKey(string storeid, string errorCode, int languageId)
        {
            var _errorCode = (from e in context.StoreErrorCodeGlobalResources
                         where e.StoreId == storeid && e.ErrorCode == errorCode && e.LanguageId == languageId
                         select e).FirstOrDefault();
            if (_errorCode != null)
                _errorCode.helper = this;
            return _errorCode;
        }

        public List<LanguageResource> getErrorCodeResourceTemplate(string storeId, int languageId)
        {
            var errorCodeList = (from s in context.StoreErrorCodes 
                                 from l in context.Languages 
                                 let ss = context.StoreErrorCodeGlobalResources.Where(x => x.StoreId == storeId && x.LanguageId == languageId && x.ErrorCode == s.ErrorCode).FirstOrDefault() 
                                 where s.StoreID == storeId && l.Id == languageId
                                 select new LanguageResource
                                 {
                                     StoreId = s.StoreID,
                                     DisplayKeyName = s.ErrorCode,
                                     DisplayKeyValue = s.UserActionMessage,
                                     LanguageName = l.Name + "(" + l.Location + ")",
                                     LocalName = (ss == null ? "" : ss.LocalName)
                                 }).Distinct().ToList(); 

            List<LanguageResource> errorList = new List<LanguageResource>();
            foreach (LanguageResource item in errorCodeList)
            {
                LanguageResource errorItem = errorList.FirstOrDefault(p => p.DisplayKeyName == item.DisplayKeyName);
                if (errorItem == null)
                    errorList.Add(item);
            }
            return errorList;
        }
        #endregion


        #region Creat Update Delete
        public int save(StoreErrorCodeGlobalResource _storeErrorCode)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_storeErrorCode == null || _storeErrorCode.validate() == false) return 1;
            //Try to retrieve object from DB
            StoreErrorCodeGlobalResource _exist_ErrorCode = getErrorCodeResourceById(_storeErrorCode.Id);
            try
            {
                if (_exist_ErrorCode == null)  //object not exist 
                {
                    context.StoreErrorCodeGlobalResources.AddObject(_storeErrorCode);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update
                    if (_storeErrorCode.helper != null && _storeErrorCode.helper.context != null)
                        context = _storeErrorCode.helper.context;
                    context.StoreErrorCodeGlobalResources.ApplyCurrentValues(_storeErrorCode);
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

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(StoreErrorCodeGlobalResourceHelper).ToString();
        }
        #endregion
    }
}
