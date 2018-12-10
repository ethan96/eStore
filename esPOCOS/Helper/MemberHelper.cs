using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class MemberHelper : Helper
    {

        #region Search
        //get user by email
        public Member getMemberUserById(string email)
        {
            if (String.IsNullOrEmpty(email)) return null;
            try
            {
                var _member = (from user in context.Members
                             where (user.EMAIL_ADDR == email || user.EMAIL_ADDR2 == email)
                             select user).FirstOrDefault();

                return _member;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return null;
            }
        }
                
        #endregion

        #region Others
        private static string myclassname()
        {
            return typeof(UserActivityHelper).ToString();
        }
        #endregion

        internal int save(Member member)
        {
            //if parameter is null or validation is false, then return  -1 
            if (member == null || member.validate() == false) return 1;
            //Try to retrieve object from DB
            Member _exist_module = getMemberUserById(member.EMAIL_ADDR);
            try
            {
                if (_exist_module == null)  //object not exist 
                {
                    //Insert
                    context.Members.AddObject(member);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.Members.ApplyCurrentValues(member);
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
    }
}