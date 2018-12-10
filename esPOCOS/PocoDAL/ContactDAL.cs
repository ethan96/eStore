using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{

     public partial class Contact
     {
         private ContactHelper _helper = null;

         public ContactHelper helper
         {
             get { return _helper; }
             set { _helper = value; }
         }

         public int save()
         {
             if (_helper == null)
                 _helper = new ContactHelper();
             return _helper.save(this);
         }

         public int delete()
         {
             if (_helper == null)
                 _helper = new ContactHelper();
             return _helper.delete(this);
         }
     }

 }