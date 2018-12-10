using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class UserRoleHelper : Helper
    {
        //public UserRole getUserRoleById(int id)
        //{
        //    if (id == 0)
        //        return null;
        //    try
        //    {
        //        var _userRole = (from p in context.UserRoles
        //                           where p.Id == id
        //                           select p).FirstOrDefault();
        //        if (_userRole != null)
        //            _userRole.helper = this;
        //        return _userRole;
        //    }
        //    catch (Exception ex)
        //    {
        //        eStoreLoger.Fatal(ex.Message, "", "", "", ex);
        //        return null;
        //    }
        //}

        //根据条件查找  用户权限
        public List<UserRole> getUserRolebySearch(string email,string dmf,int roleId = 0)
        {
            try
            {
                List<UserRole> userRoleList = (from u in context.UserRoles
                                               where !string.IsNullOrEmpty(email) ? u.UserID.Contains(email) :
                                                     (roleId > 0 ? u.RoleId == roleId : u.AdminRole.DMFID == dmf)
                                               select u).ToList();

                if (userRoleList != null)
                    return userRoleList;
                else
                    return new List<UserRole>();
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return new List<UserRole>();

            }
        }


        public int delete(UserRole userRole)
        {
            if (userRole == null)
                return 1;
            try
            {
                UserRole existsRole = null;// getUserRoleById(userRole.Id);
                if (existsRole != null)
                {
                    context.UserRoles.DeleteObject(existsRole);
                    context.SaveChanges();
                    return 0;
                }
                return 1;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return 500;
            }
        }
    }
}
