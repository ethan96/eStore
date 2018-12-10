using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class StoreErrorCodeHelper : Helper
    {
        #region Business
        public List<StoreErrorCode> getErrorCodebyStore(string storeid)
        {
       
                var _errorcode = (from error in context.StoreErrorCodes
                                  where error.StoreID == storeid
                                  select error).ToList();

                List<StoreErrorCode> errorcodes = null;
                if (_errorcode != null)
                    errorcodes = _errorcode.ToList<StoreErrorCode>();
                else
                    errorcodes = new List<StoreErrorCode>();

                foreach (StoreErrorCode t in errorcodes)
                    t.helper = this;
                return errorcodes;
 
        }

        public StoreErrorCode getErrorCodeByCode(string storeid, string errorcode)
        {
            var _errorcode = (from error in context.StoreErrorCodes
                              where error.StoreID == storeid && error.ErrorCode == errorcode
                              select error).FirstOrDefault();

            if (_errorcode != null)
                _errorcode.helper = this;
            return _errorcode;
        }
        #endregion 

        #region Creat Update Delete
        public int save(StoreErrorCode _errorcode)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_errorcode == null || _errorcode.validate() == false) return 1;
            //Try to retrieve object from DB
            StoreErrorCode _exist_errorcode = new StoreErrorCodeHelper().getErrorCodeByCode(_errorcode.StoreID, _errorcode.ErrorCode );
            try
            {
                if (_exist_errorcode == null)  //object not exist 
                {
                    context.StoreErrorCodes.AddObject(_errorcode);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update
                    context.StoreErrorCodes.ApplyCurrentValues(_errorcode);
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

        public int delete(StoreErrorCode _errorcode)
        {

            if (_errorcode == null || _errorcode.validate() == false) return 1;
            try
            {
                if (_errorcode.helper != null && _errorcode.helper.context != null)
                    context = _errorcode.helper.context;

                context.DeleteObject(_errorcode);
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
