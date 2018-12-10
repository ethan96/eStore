using eStore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class AccessRightHelper : Helper 
    {
        public int bulkupdate(AdminRole role, List<AdminAuth> selectedAuthList)

        {
            //if parameter is null or validation is false, then return  -1 
            if (role == null || role.validate() == false || selectedAuthList == null ) return 1;
            //Try to retrieve object from DB


            AdminRole _exist_role = context.AdminRoles.FirstOrDefault(r => r.Roleid == role.Roleid);
            List<int> listID = selectedAuthList.Select(b => b.AuthID).ToList(); 
            List<AdminAuth> _exist_authList = context.AdminAuths.Where(a => listID.Contains(a.AuthID)).ToList();
            
            try
            {
                if (_exist_role != null && _exist_authList != null)
                {
                    //_exist_role.AdminAuths.Add(_exist_auth);
                    _exist_role.AdminAuths.Clear();
                    foreach (var auth in _exist_authList)
                        _exist_role.AdminAuths.Add(auth);
                    context.SaveChanges();
                    return 0;
                }
                return 1;
                //if (_exist_role == null)  //object not exist 
                //{
                //    //Insert
                //    context.AdminRoles.AddObject(role);
                //    context.SaveChanges();
                //    return 0;
                //}
                //else

                //{
                //    //Update
                //    context.AdminRoles.ApplyCurrentValues(role);
                //    context.SaveChanges();
                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
