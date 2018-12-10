using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;


namespace eStore.POCOS
{
    public partial class UserRequest : FollowUpable
    {
        public enum ReqType { ContactUs, RequestDiscount, CallMeNow, ServiceRequest, GeneralInquiries, TechnicalSupport, Sales,ECORequest, Subscription };

        public UserRequest()
        {
            CreatedDate = DateTime.Now;
            InsertDate = CreatedDate;
            LastUpdateTime = CreatedDate;
        }

        public UserRequest(Store store, ReqType type) : this()
        {
            StoreID = store.StoreID;
            RequestType = type.ToString();
        }

#region OM
        //The following methods are for FollowUp used in OM
        protected override String getInstanceID() { return this.ID.ToString(); }
        protected override String getInstanceStoreID() { return this.StoreID; }
        protected override String getInstanceOwner() { return this.Email; }
        protected override TrackingLog.TrackType getTrackType() 
        { 
            TrackingLog.TrackType type = TrackingLog.TrackType.NOT_SPECIFIED;

            if (RequestType.Equals(ReqType.ContactUs.ToString()) || RequestType.Equals(ReqType.GeneralInquiries.ToString()) || RequestType.Equals(ReqType.TechnicalSupport.ToString()) || RequestType.Equals(ReqType.Sales.ToString()))
                type = TrackingLog.TrackType.CONTACTUS;
            else if (RequestType.Equals(ReqType.RequestDiscount.ToString()))
                type = TrackingLog.TrackType.REQUESTDISCOUNT;
            else if (RequestType.Equals(ReqType.CallMeNow.ToString()))
                type = TrackingLog.TrackType.CALLMENOW;
            else
                type = TrackingLog.TrackType.NOT_SPECIFIED;

            return type;
        }
#endregion OM
        #region Primitive Properties
        public virtual Product ProductX
        {
            get;
            set;
        }

        #endregion
    }
}