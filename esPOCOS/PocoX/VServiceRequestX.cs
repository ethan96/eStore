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
    public partial class VServiceRequest : FollowUpable
    {
        
    #region Extension Methods 

        public List<ServiceRequestTracking> getLogs() {
            ServiceRequestHelper srhelper = new ServiceRequestHelper();
            return srhelper.getServiceRequestLog(this.Activity_Id);
        
        }


    #endregion 
	
    
        protected override string getInstanceID()
        {
            return this.Activity_Id;
        }

        protected override string getInstanceStoreID()
        {
            return null;
        }

        protected override string getInstanceOwner()
        {
            return this.CREATED_BY;
        }

        protected override TrackingLog.TrackType getTrackType()
        {
            return TrackingLog.TrackType.SERVICEREQUEST;
        }
    } 
 }