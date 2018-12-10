using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class AdminRoleHelper : Helper { 
#region Business Read

    /// <summary>
    /// Get OM Module by ModuleID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AdminRole  getRolebyID(int roleid) {
        var _m = (from r in context.AdminRoles
                 where r.Roleid==roleid
                 select r).FirstOrDefault();

        if (_m != null)
            _m.helper = this;

        return _m;
    
    
    }

    /// <summary>
    /// get Roles by rolenames
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<AdminRole> getRolebyName(string rolenames)
    {
        try
        {
            var _m = (from r in context.AdminRoles
                      where rolenames.Contains(r.RoleName)
                      select r);

            if (_m != null)
            {
                foreach (AdminRole am in _m)
                    am.helper = this;

                return _m.ToList();
            }
            else
            {

                return new List<AdminRole>();
            }

        }
        catch (Exception ex) {
            eStoreLoger.Fatal(ex.Message, "", "", "", ex);
            return new List<AdminRole>();
        
        }


    }



    /// <summary>
    /// for OM use, retun all Adminroles.
    /// </summary>
    /// <returns></returns>

    public List<AdminRole> getAllAdminRoles() {

        var _ms = (from r in context.AdminRoles                 
                  select r);

        foreach (AdminRole r in _ms) {
            r.helper = this;
        }

        return _ms.ToList();
    
    }

    /// <summary>
    /// Return Roles under given DMF
    /// </summary>
    /// <returns></returns>

    public List<AdminRole> getRolesByDMF(DMF dmf)
    {
        var _roles = (from r in context.AdminRoles
                  where r.DMFID == dmf.DMFID
                  select r);

        if (_roles != null)
        {
            foreach (AdminRole role in _roles)
                role.helper = this;

            return _roles.ToList();
        }
        else
        {
            return new List<AdminRole>();
            
        }        


    }


#endregion 

 #region Creat Update Delete 

     public int save(AdminRole role) 

     {
          //if parameter is null or validation is false, then return  -1 
         if (role == null || role.validate() == false) return 1;
          //Try to retrieve object from DB
         AdminRole _exist_role = getRolebyID(role.Roleid);
          try
          {
              if (_exist_role == null)  //object not exist 
              {
                  //Insert
                  context.AdminRoles.AddObject(role);
                  context.SaveChanges();
                 return 0;
             }
             else  
  
             {
                 //Update
                 context.AdminRoles.ApplyCurrentValues(role);
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

 
 public int delete(AdminRole _adminrole) 
{

    if (_adminrole == null || _adminrole.validate() == false) return 1;
 
         try {
                _adminrole.Users.Clear();
                _adminrole.AdminAuths.Clear();
                context = _adminrole.helper.context;
                context.DeleteObject(_adminrole); 
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
            return typeof(AdminRoleHelper).ToString();
       } 
#endregion 
	} 
 }