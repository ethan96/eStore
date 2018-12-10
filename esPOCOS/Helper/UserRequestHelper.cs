using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Linq.Expressions;
namespace eStore.POCOS.DAL
{

    public partial class UserRequestHelper : Helper
    {
        #region Business Read

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>
        public List<UserRequest> getRequestDiscountByStore(string storeid)
        {
            Store store = new StoreHelper().getStorebyStoreid(storeid);

            var _rds = (from rd in context.UserRequests
                        from cr in store.Countries
                        where cr.CountryName == rd.Country
                        select rd);

            return _rds.ToList();
        }


        public UserRequest getRequestDiscountbyID(int id)
        {

            var _rds = (from rd in context.UserRequests
                        where rd.ID == id
                        select rd).FirstOrDefault();
            return _rds;
        }
        /// <summary>
        /// For OM, return all user request.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>

        public List<UserRequest> getUserRequest(DMF dmf, DateTime startdate, DateTime enddate, UserRequest.ReqType requesttype, string Company = null, string email = null)
        {
            enddate = enddate.Date.AddHours(24);
            List<string> countries = dmf.getCountryCoverage(true);
            string reqtype = requesttype.ToString();
            var _requests = from c in context.UserRequests
                                where countries.Contains(c.Country.ToUpper())
                                && c.CreatedDate >= startdate.Date && c.CreatedDate <= enddate
                                && (reqtype.ToUpper() == "RequestDiscount".ToUpper() ? c.RequestType.ToUpper() == reqtype.ToUpper() : (c.RequestType.ToUpper() != "RequestDiscount".ToUpper()))
                                && ((!String.IsNullOrEmpty(Company) && !String.IsNullOrEmpty(email)) ? c.Email.ToUpper().Contains(email.ToUpper()) && c.Company.Contains(Company.ToUpper()) : true)
                                && (!String.IsNullOrEmpty(Company) ? c.Company.ToUpper().Contains(Company.ToUpper()) : true)
                                && (!String.IsNullOrEmpty(email) ? c.Email.ToUpper().Contains(email.ToUpper()) : true)
                                orderby c.CreatedDate descending
                                select c;

                return _requests.ToList();

            
        }
        #endregion


        #region Create Update Delete
        public int save(UserRequest _reqDiscount)
        {

            //if parameter is null or validation is false, then return  -1 

            if (_reqDiscount == null || _reqDiscount.validate() == false) return 1;
            //Try to retrieve object from DB

            UserRequest _existRD = getRequestDiscountbyID(_reqDiscount.ID);
            try
            {
                if (_existRD == null)  //object not exist 
                {
                    //Insert            

                    context.UserRequests.AddObject(_reqDiscount);
                    context.SaveChanges();
                    return 0;

                }
                else
                {
                    //Update                  


                    context.UserRequests.ApplyCurrentValues(_reqDiscount);
                    context.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
                    return 0;

                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(UserRequest _RequestDiscount)
        {

            if (_RequestDiscount == null || _RequestDiscount.validate() == false) return 1;
            try
            {
                if (_RequestDiscount.helper != null && _RequestDiscount.helper.context != null)
                    context = _RequestDiscount.helper.context;

                context.DeleteObject(_RequestDiscount);
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
            return typeof(StoreHelper).ToString();
        }
        #endregion
    }


}