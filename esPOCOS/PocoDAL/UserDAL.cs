using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{

     public partial class User
     {
         private UserHelper _helper = null;

         public UserHelper helper
         {
             get { return _helper; }
             set { _helper = value; }
         }

         public int save()
         {
             if (_helper == null)
                 _helper = new UserHelper();
             return _helper.save(this);
         }

         public int delete()
         {
             if (_helper == null)
                 _helper = new UserHelper();
             return _helper.delete(this);
         }

         public int deleteContact( Contact con)
         {
             if (_helper == null)
                 _helper = new UserHelper();
             return _helper.deleteContact(con);
         }

         /// <summary>
         /// Add Userrole by store procedure, to avoid many to many relationship table problem. 
         /// </summary>
         /// <param name="userid"></param>
         /// <param name="roleid"></param>
         /// <returns></returns>
         public int addUserRole(  int roleid) {

             if (_helper == null)
                 _helper = new UserHelper();
             return _helper.addUserRole(this.UserID ,roleid);
         }

     }
 }