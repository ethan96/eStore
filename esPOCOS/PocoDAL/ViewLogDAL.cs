using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{

     public partial class ViewLog
     {
         private ViewLogHelper  _helper = null;

         public ViewLogHelper helper
         {
             get { return _helper; }
             set { _helper = value; }
         }

         public int save()
         {
             if (_helper == null)
                 _helper = new ViewLogHelper();
             return _helper.save(this);
         }

        

     }
 }