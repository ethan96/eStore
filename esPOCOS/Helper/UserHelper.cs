using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
using System.Data.Entity;
using System.Data.EntityClient;
namespace eStore.POCOS.DAL
{

    public partial class UserHelper : Helper
    {
        #region Business Read
        public User getUserbyID(string email)
        {

            if (String.IsNullOrEmpty(email)) return null;

            try
            {

                var _user = (from usr in context.Users.Include("Contacts")
                             where (usr.UserID == email)
                             select usr).FirstOrDefault();

                if (_user!=null)
                _user.helper = this;

                return _user;

            }
            catch (Exception ex)
            {

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public User getSimpleUserbyID(string email)
        {

            if (String.IsNullOrEmpty(email)) return null;

            try
            {

                var _user = (from usr in context.Users
                             where (usr.UserID == email)
                             select usr).FirstOrDefault();
                return _user;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }

        public List<string> getInvitees(string email)
        {
            try
            {
                List<string> rlt = context.ExecuteStoreQuery<string>("SELECT r.emailaddress FROM membership.dbo. MemberReferralFieldResult r inner join membership.dbo.MemberReferralFrom f  on r.MemberReferralFromID = f.MemberReferralFromID   inner join membership.dbo.MemberReferralField fd on r.MemberReferralFieldID = fd.MemberReferralFieldID where f.Emailaddress=@email"
                    , (new System.Data.SqlClient.SqlParameter("@email",email))).ToList();
                return rlt;
            }
            catch (Exception)
            {

                return new List<string>();
            }
           
        }

        public bool isInvitee(string email)
        {
            try
            {
                var rlt = context.ExecuteStoreQuery<string>("SELECT f.Emailaddress FROM membership.dbo. MemberReferralFieldResult r inner join membership.dbo.MemberReferralFrom f  on r.MemberReferralFromID = f.MemberReferralFromID   inner join membership.dbo.MemberReferralField fd on r.MemberReferralFieldID = fd.MemberReferralFieldID where r.emailaddress=@email"
                    , (new System.Data.SqlClient.SqlParameter("@email", email)));
                return rlt.Any();
            }
            catch (Exception)
            {

                return false;
            }
           
        }

        public void getMemberActiveType(Member _member)
        {
            try
            {
                _member.ActivityType = "None";
                _member.ActivityDesc = "None";
                var rlt = context.ExecuteStoreQuery<string>("SELECT top 1 ACTIVITY_TYPE+'|'+ACTIVITY_DESC3 FROM membership.dbo.MEMBER_LOG r where r.Email_addr=@email order by DATE_CREATED asc"
                    , (new System.Data.SqlClient.SqlParameter("@email", _member.EMAIL_ADDR))).ToList();
                if (rlt != null && rlt.Count > 0)
                {
                    string[] arrMember = rlt[0].Split('|');
                    if (arrMember.Length>=2)
                    {
                        _member.ActivityType = arrMember[0];
                        _member.ActivityDesc = arrMember[1];
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        ~UserHelper()
        {
            context.Dispose();

        }
        //查询register 的open count
        public int[] getRegisteredOpenCount(DMF dmf, DateTime startdate, DateTime enddate, string Company = null, string email = null,
                                    bool isAdvantech = false, string rbu = "")
        {
            List<string> countries = dmf.getCountryCoverage(false, rbu);

            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);
            int[] openClose = new int[] { 0, 0 };
            var recordeCount = (from u in context.Members
                                let t = context.TrackingLogs.Where(p => p.TrackingNo == u.EMAIL_ADDR).OrderByDescending(x => x.LogId).FirstOrDefault()
                                where countries.Contains(u.COUNTRY)
                                && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? u.Insert_date >= startdate.Date && u.Insert_date <= enddate : true)
                                && (!string.IsNullOrEmpty(Company) ? u.COMPANY_NAME.ToUpper().Contains(Company.ToUpper()) : true)
                                && (!string.IsNullOrEmpty(email) ? u.EMAIL_ADDR.ToUpper().Contains(email.ToUpper()) : true)
                                && (isAdvantech ? true : !u.EMAIL_ADDR.ToUpper().Contains("@ADVANTECH."))
                                select t.FollowUpStatus).ToList();
            openClose[1] = (from p in recordeCount
                            where
                                !string.IsNullOrEmpty(p) && !eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(p)
                            select p).Count();
            openClose[0] = recordeCount.Count - openClose[1];
            return openClose;
        }

        /// <summary>
        /// For OM, get member registration information. Not all are eStore Users.
        /// </summary>
        /// <param name="dmf"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public List<Member> getRegisteredUsers(DMF dmf, DateTime startdate, DateTime enddate, string Company = null, string email = null,
                                    bool isAdvantech = false, string rbu = "", string FollowUpStatus = "", bool isMemberLog = false)
        {
            List<Member> memberList = new List<Member>();
            List<string> countries = dmf.getCountryCoverage(false, rbu);
            List<string> bbsource = new List<string>() { "abb_estore", "B+B" };

            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);
            //导出excel就把 trackinglog 一起拿出来.
            if (isMemberLog)
            {
                var memberTrackingList = (from u in context.Members
                              let t = context.TrackingLogs.Where(p => p.TrackingNo == u.EMAIL_ADDR).OrderByDescending(x => x.LogId).FirstOrDefault()
                              where countries.Contains(u.COUNTRY)
                              && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? u.Insert_date >= startdate.Date && u.Insert_date <= enddate : true)
                              && (!string.IsNullOrEmpty(Company) ? u.COMPANY_NAME.ToUpper().Contains(Company.ToUpper()) : true)
                              && (!string.IsNullOrEmpty(email) ? u.EMAIL_ADDR.ToUpper().Contains(email.ToUpper()) : true)
                              && (isAdvantech ? true : !u.EMAIL_ADDR.ToUpper().Contains("@ADVANTECH."))
                              && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                                 FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                                 : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                              ))
                              orderby u.Insert_date descending
                              select new
                              {
                                  EMAIL_ADDR = u.EMAIL_ADDR,
                                  SOURCE = u.SOURCE,
                                  COMPANY_NAME = u.COMPANY_NAME,
                                  COUNTRY = u.COUNTRY,
                                  CITY = u.CITY,
                                  STATE = u.STATE,
                                  IN_PRODUCT = u.IN_PRODUCT,
                                  DATE_LAST_CHANGED = u.DATE_LAST_CHANGED,
                                  currentFollowupStatus = (t == null) ? "N/A" : t.FollowUpStatus,
                                  currentFollowUpAssignee = (t == null) ? "N/A" : t.AssignTo,
                                  currentFollowupComment = (t == null) ? "N/A" : t.FollowUpComments,
                                  lastFollowupUpdateBy = (t == null) ? "N/A" : t.LastUpdateBy,
                                  FIRST_NAME = u.FIRST_NAME,
                                  LAST_NAME = u.LAST_NAME,
                                  TEL_AREACODE = u.TEL_AREACODE,
                                  TEL_NO = u.TEL_NO,
                                  TEL_EXT = u.TEL_EXT,
                                  MOBILE = u.TEL_EXT,
                                  ActivityType=u.RegPurpose ,
                                  ActivityDesc=u.referrer,
                                  IN_PRODUCT_VALUES = u.IN_PRODUCT_VALUES,
                                  Insert_date = u.Insert_date
                              }).ToList();
                foreach (var item in memberTrackingList)
                {
                    Member m = new Member();
                    m.EMAIL_ADDR = item.EMAIL_ADDR;
                    m.SOURCE = item.SOURCE;
                    m.COMPANY_NAME = item.COMPANY_NAME;
                    m.COUNTRY = item.COUNTRY;
                    m.STATE = item.STATE;
                    m.IN_PRODUCT = item.IN_PRODUCT;
                    m.DATE_LAST_CHANGED = item.DATE_LAST_CHANGED;
                    m.currentFollowupStatus = item.currentFollowupStatus;
                    m.currentFollowUpAssignee = item.currentFollowUpAssignee;
                    m.currentFollowupComment = item.currentFollowupComment;
                    m.lastFollowupUpdateBy = item.lastFollowupUpdateBy;
                    m.FIRST_NAME = item.FIRST_NAME;
                    m.LAST_NAME = item.LAST_NAME;
                    m.TEL_AREACODE = item.TEL_AREACODE;
                    m.TEL_NO = item.TEL_NO;
                    m.TEL_EXT = item.TEL_EXT;
                    m.IN_PRODUCT_VALUES = item.IN_PRODUCT_VALUES;
                    m.Insert_date = item.Insert_date;
                    memberList.Add(m);
                }
            }
            else
            {
                memberList = (from u in context.Members
                              let t = context.TrackingLogs.Where(p => p.TrackingNo == u.EMAIL_ADDR).OrderByDescending(x => x.LogId).FirstOrDefault()
                              where countries.Contains(u.COUNTRY)
                              && ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? u.Insert_date >= startdate.Date && u.Insert_date <= enddate : true)
                              && (!string.IsNullOrEmpty(Company) ? u.COMPANY_NAME.ToUpper().Contains(Company.ToUpper()) : true)
                              && (!string.IsNullOrEmpty(email) ? u.EMAIL_ADDR.ToUpper().Contains(email.ToUpper()) : true)
                              && (isAdvantech ? true : !u.EMAIL_ADDR.ToUpper().Contains("@ADVANTECH."))
                              && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                                 FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                                 : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                              ))
                              && ((dmf.StoreID == "AUS" || dmf.StoreID == "ALA") ? !bbsource.Contains(u.SOURCE) : true)
                              orderby u.Insert_date descending
                              select u).ToList();
            }

            ////导出excel的时候,一次性把member_log查出来.
            //if (isMemberLog && memberList != null && memberList.Count > 0)
            //{
            //    string logSql = "select t.EMAIL_ADDR,t.ACTIVITY_TYPE,t.ACTIVITY_DESC3 from ( " +
            //                        "SELECT ml.EMAIL_ADDR ,ml.ACTIVITY_TYPE, ml.ACTIVITY_DESC3, " +
            //                        "ROW_NUMBER() over(PARTITION by  ml.EMAIL_ADDR  order by ml.DATE_CREATED ) row " +
            //                        "FROM membership.dbo.MEMBER m INNER JOIN " +
            //                        "membership.dbo.MEMBER_LOG ml ON m.EMAIL_ADDR = ml.EMAIL_ADDR " +
            //                        "where 1 = 1";
            //    if (!isAdvantech)
            //        logSql += " and m.EMAIL_ADDR NOT LIKE '%@ADVANTECH.%'";

            //    string countrySql = "";
            //    foreach (var countryItem in countries)
            //    {
            //        if (!string.IsNullOrEmpty(countrySql))
            //            countrySql += ",";
            //        countrySql += "'" + countryItem + "'";
            //    }
            //    logSql += " and m.COUNTRY IN (" + countrySql + ")";

            //    if (string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email))
            //        logSql += " and m.Insert_date BETWEEN '" + startdate + "' AND '" + enddate  + "'";
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(Company))
            //            logSql += " and m.COMPANY_NAME LIKE '%"+ Company +"%'";
            //        if (!string.IsNullOrEmpty(email))
            //            logSql += " and m.EMAIL_ADDR like '%"+ email +"%'";
            //    }
            //    logSql += " ) t where row=1 ";

            //    List<MemberLog> memberLogList = context.ExecuteStoreQuery<MemberLog>(logSql).ToList();
            //    foreach (var memberItem in memberList)
            //    {
            //        MemberLog memberLogItem = memberLogList.FirstOrDefault(p => p.EMAIL_ADDR == memberItem.EMAIL_ADDR);
            //        if (memberLogItem != null)
            //        {
            //            memberItem.ActivityType = string.IsNullOrEmpty(memberLogItem.ACTIVITY_TYPE) ? "None" : memberLogItem.ACTIVITY_TYPE;
            //            memberItem.ActivityDesc = string.IsNullOrEmpty(memberLogItem.ACTIVITY_DESC3) ? "None" : memberLogItem.ACTIVITY_DESC3;
            //        }
            //        else
            //        {
            //            memberItem.ActivityType = "None";
            //            memberItem.ActivityDesc = "None";
            //        }
            //    }
            //}

            return memberList;
        }

        /// <summary>
        /// for om, get user register information all estore users
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="Company"></param>
        /// <param name="email"></param>
        /// <param name="isAdvantech"></param>
        /// <param name="rbu"></param>
        /// <param name="FollowUpStatus"></param>
        /// <returns></returns>
        public List<User> getRegisteredUsers_eStore(DateTime startdate, DateTime enddate, string Company = null, string email = null,
                                    bool isAdvantech = false, string FollowUpStatus = "")
        {
            //Add 24 hours to include the orders in the enddate
            enddate = enddate.Date.AddHours(24);
            //导出excel就把 trackinglog 一起拿出来.
            var memberTrackingList = (from u in context.Users
                                      let t = context.TrackingLogs.Where(p => p.TrackingNo == u.UserID).OrderByDescending(x => x.LogId).FirstOrDefault()
                                      where ((string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(email)) ? u.CreatedDate >= startdate.Date && u.CreatedDate <= enddate : true)
                                      && (!string.IsNullOrEmpty(Company) ? u.CompanyName.ToUpper().Contains(Company.ToUpper()) : true)
                                      && (!string.IsNullOrEmpty(email) ? u.UserID.ToUpper().Contains(email.ToUpper()) : true)
                                      && (isAdvantech ? true : !u.UserID.ToUpper().Contains("@ADVANTECH."))
                                      && (string.IsNullOrEmpty(FollowUpStatus) ? true : (
                                         FollowUpStatus.ToUpper() == "OPEN" ? (t.FollowUpStatus == null || eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                                         : (t.FollowUpStatus != null && !eStore.POCOS.PocoX.FollowUpable.MemberFollowUpStatues.Contains(t.FollowUpStatus))
                                      ))
                                      orderby u.CreatedDate descending
                                      select u).ToList();
            return memberTrackingList;
        }


        public Member getRegisteredUserByID(string email)
        {
            Member _user;
            if (String.IsNullOrEmpty(email)) return null;
        
                _user = (from u in context.Members
                         where u.EMAIL_ADDR.Contains(email)
                         select u).FirstOrDefault();

            if (_user != null)
                return _user;
            else
                return null;
        }

        /// <summary>
        /// Return all OM users
        /// </summary>
        /// <returns></returns>
        public List<User> getOMUsers() {
            var _omusers = from u in context.Users
                           where u.AdminRoles.Any()
                           select u;

            return _omusers.ToList();
        }

        public List<string> getInternalUserHint(string keywords, int Maxreturn)
        {
            var _omusers = from u in context.Users
                           where u.UserID.ToLower().Contains(keywords)
                           && u.UserID.ToLower().Contains("@advantech")
                           select u.UserID;
            return _omusers.Take(Maxreturn).ToList();
        }


        #endregion

        #region Creat Update Delete
        public int save(User _user)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_user == null || _user.validate() == false) return 1;
            //Try to retrieve object from DB

            User _exist_user = getUserbyID(_user.UserID);
            try
            {
                if (_exist_user == null)  //object not exist 
                {
                    //Insert                  
                    context.Users.AddObject(_user);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update            
                    if (_user.helper != null && _user.helper.context != null)
                        context = _user.helper.context;

                    //if (_user.AdminRoles != null) { 
                    //    foreach(AdminRole r in _user.AdminRoles){
                    //        eStore3Entities6 c = r.helper.context;
                    //        c.ObjectStateManager.ChangeObjectState(r, System.Data.EntityState.Unchanged);
                    //    }                
                    
                    //}

                    context.Users.ApplyCurrentValues(_user);
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

        /// <summary>
        /// Add Userrole by store procedure, to avoid many to many relationship table problem. 
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        
        public int addUserRole(string userid, int roleid) {

            int ret = 0;

            try
            {
                  ret = context.sp_InsertUserRole(userid, roleid).FirstOrDefault().GetValueOrDefault();
                  return ret;

            }catch(Exception ex){

                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }

        
        }

        public int delete(User _user)
        {

            if (_user == null || _user.validate() == false) return 1;

            try
            {


                context.DeleteObject(_user);
                context.SaveChanges();
                return 0;

            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int deleteContact(Contact _contact)
        {

            if (_contact == null) return 1;

            try
            {
                var _c = (from c in context.Contacts
                          where c.ContactID == _contact.ContactID
                          select c).FirstOrDefault();

                if (_c != null)
                {
                    context.Contacts.DeleteObject(_c);
                    context.SaveChanges();
                    return 0;
                }
                else
                {

                    return -1;
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
            return typeof(UserHelper).ToString();
        }
        #endregion
    }
}