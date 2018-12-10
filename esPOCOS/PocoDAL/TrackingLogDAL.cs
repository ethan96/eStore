using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class TrackingLog
    {
        private TrackingLogHelper _helper = null;

        public TrackingLogHelper helper
         {
             get { return _helper; }
             set { _helper = value; }
         }

         public int save()
         {
             if (_helper == null)
                 _helper = new TrackingLogHelper();
             return _helper.save(this);
         }

        

     }
 }