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
    public partial class ServiceRequestTracking
    {

        private ServiceRequestHelper _helper = null;

        public ServiceRequestHelper helper
      {
          get { return _helper; }
          set { _helper = value; }
      }

      public int save() 
        {
            if (_helper == null)
                _helper = new ServiceRequestHelper();
 		  return _helper.save(this);
        }
  
       
	} 
	 
 }