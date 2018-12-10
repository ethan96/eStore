using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS
{
    public partial class User
    {
        #region Extension Attributes

        public enum OMRole { SuperAdmin, Sales, CTOSEditor, PM, Marketing, SuperCBOMEditor, SalesAssistant, eCoverage };
        #endregion

        /// <summary>
        /// This method returns user authorization status at the specifying access (right).  Currently only very basic concept is 
        /// implemented.  This method requires later attention to complete it.  *******
        /// </summary>
        /// <param name="rightName"></param>
        /// <returns></returns>
        public Boolean hasOMRight(string accessToken, String dmf, POCOS.MiniSite minisite=null)
        {
            //current role
            if (isAuthenticated())
            {
                //if (isSuperAdmin(dmf))
                    //return true;
                
                List<AdminRole> storeRoleList = new List<AdminRole>();
                //如果minisite不为null,拿出对应minisite的权限
                if (minisite!=null)
                {   
                    storeRoleList = this.AdminRoles.Where(role => role.RoleName.StartsWith(minisite.SiteName) && role.DMFID.Equals(dmf)).ToList();
                    if (isSuperAdminOrIT(dmf))  /// 非好的寫法,但暫時強制superadmin/IT的user可以瀏覽minisite
                        return true;
                }
                else
                    //当前store的所有权限
                    storeRoleList = AdminRoles.Where(role => role.DMFID.Equals(dmf)).ToList();

                foreach (AdminRole role in storeRoleList)
                {
                    foreach (AdminAuth auth in role.AdminAuths)
                    {
                        if (auth.Name == accessToken)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method returns user authorization status at the specifying access (right).  Currently only very basic concept is 
        /// implemented.  This method requires later attention to complete it.  *******
        /// </summary>
        /// <param name="rightName"></param>
        /// <returns></returns>
        public Boolean hasOMAccess(string accessToken, String dmf)
        {
            //current role
            if (isAuthenticated())
            {
                if (isSuperAdminOrIT(dmf))
                    return true;
                AdminRole role = AdminRoles.FirstOrDefault(p=>p.DMFID.Equals(dmf) && p.RoleName.Equals(accessToken));
                return role != null;
            }

            return false;
        }

        /// <summary>
        /// Validate if current user is a super admin or IT of specified DMF
        /// </summary>
        /// <param name="dmf"></param>
        /// <returns></returns>
        public Boolean isSuperAdminOrIT(String dmf)
        {
            if (String.IsNullOrWhiteSpace(dmf))
                return false;

            foreach (AdminRole role in AdminRoles.Where(role => role.DMFID.Equals(dmf)))
            {
                if (!String.IsNullOrWhiteSpace(role.RoleName) && role.RoleName.ToUpper().Equals("SUPERADMIN"))
                    return true;
                if (!String.IsNullOrWhiteSpace(role.RoleName) && role.RoleName.ToUpper().Equals("IT ADMIN"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// This method returns user authorization status at the specifying Menu.
        /// First, check the menu's accesRight column, if no access Right, then check the adminauth table, 
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public Boolean hasMenuRight(POCOS.OMMenu menu, string dmf, POCOS.MiniSite minisite = null)
        {

            if (menu.AdminAuths.Count == 0 && (menu.AccessRight == "NONE" || menu.AccessRight == null))
                return true;

            if (this.hasOMRight(menu.AccessRight, dmf, minisite))
                return true;
            foreach (var auth in menu.AdminAuths)
            {
                if (this.hasOMRight(auth.Name, dmf, minisite))
                    return true;
            }
            return false;
        }
    }
}