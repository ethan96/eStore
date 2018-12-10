using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
 namespace eStore.POCOS.DAL{ 

public partial class AdminAuthHelper : Helper { 

#region Business Read

    /// <summary>
    /// Get OM Module by ModuleID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AdminAuth  getAuthbyID(int id) {
        var _m = (from m in context.AdminAuths
                 where m.AuthID  == id
                 select m).FirstOrDefault();

        return _m;
    
    
    }

    /// <summary>
    /// Return all OM  Authorizations
    /// </summary>
    /// <returns></returns>

    public List<AdminAuth> getAuthorizations()
    {
        var _m = from m in context.AdminAuths
                  select m;

        if (_m != null)
        {
            foreach (AdminAuth m in _m)
                m.helper = this;

            return _m.ToList();
        }
        else
        {
            return new List<AdminAuth>();
            
        }

         


    }


#endregion 

 #region Creat Update Delete 

    public int save(AdminAuth auth) 

     {
          //if parameter is null or validation is false, then return  -1 
         if (auth == null || auth.validate() == false) return 1;
          //Try to retrieve object from DB
         AdminAuth _exist_module = getAuthbyID(auth.AuthID);
          try
          {
              if (_exist_module == null)  //object not exist 
              {
                  //Insert
                  context.AdminAuths.AddObject(auth);
                  context.SaveChanges();
                 return 0;
             }
             else  
  
             {
                 //Update
                 context.AdminAuths.ApplyCurrentValues(auth);
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

    public int delete(AdminAuth _adminmodule) 
{

    if (_adminmodule == null || _adminmodule.validate() == false) return 1;
  try    {

            context = _adminmodule.helper.context;
           context.DeleteObject(_adminmodule); 
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
            return typeof(AdminAuthHelper).ToString();
       } 
#endregion 
	} 
 }