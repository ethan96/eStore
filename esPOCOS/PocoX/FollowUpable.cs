using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS.PocoX
{
    /// <summary>
    /// This class is implemented for tracking purpose.  Any class wants to become trackable shall inherit this abstract class
    /// </summary>
    public abstract class FollowUpable
    {
        abstract protected String getInstanceID();
        abstract protected String getInstanceStoreID();
        abstract protected String getInstanceOwner();
        abstract protected TrackingLog.TrackType getTrackType();

        //Followup Status, should be the same as Sieble
        public enum FollowupStatus { NA, Validating, WaitingForPO_Approval, Won_PO_InputInSAP, Lost, Terminated, AssignToCP }

        private TrackingLog.TrackType _trackType = TrackingLog.TrackType.NOT_SPECIFIED;
        public TrackingLog.TrackType trackType
        {
            get 
            {
                if (_trackType == TrackingLog.TrackType.NOT_SPECIFIED)
                    _trackType = getTrackType();

                return _trackType; 
            }
        }

        private String _trackInstanceID = null;
        public String trackInstanceID
        {
            get 
            {
                if (_trackInstanceID == null)
                    _trackInstanceID = getInstanceID();
                return _trackInstanceID; 
            }
        }

        private String _trackInstanceStoreID = null;
        public String trackInstanceStoreID
        {
            get
            {
                if (_trackInstanceStoreID == null)
                    _trackInstanceStoreID = getInstanceStoreID();
                return _trackInstanceStoreID;
            }
        }

        private String _instanceOwnerID = null;
        public String instanceOwnerID
        {
            get 
            {
                if (_instanceOwnerID == null)
                    _instanceOwnerID = getInstanceOwner();
                return _instanceOwnerID; 
            }
        }

        /// <summary>
        /// FollowUp information is runtime info, when the followup info is changed, an activity log will be created and follow
        /// up status will be refreshed and retrieved from there.
        /// </summary>
        private String _currentFollowupStatus = null;
        public String currentFollowupStatus
        {
            set { _currentFollowupStatus = value; }
            get
            {
                if (_currentFollowupStatus == null) //get the last followup status from tracking logs
                    updateCurrentFollowupInfoFromLog();

                return _currentFollowupStatus;
            }
        }

        private String _currentFollowupComment = null;
        public String currentFollowupComment
        {
            set { _currentFollowupComment = value; }
            get
            {
                if (_currentFollowupComment == null) //get the last followup status from tracking logs
                    updateCurrentFollowupInfoFromLog();

                return _currentFollowupComment;
            }
        }

        private String _currentFollowupAssignee = null;
        public String currentFollowUpAssignee
        {
            set { _currentFollowupAssignee = value; }
            get
            {
                if (_currentFollowupAssignee == null)
                    updateCurrentFollowupInfoFromLog();

                return _currentFollowupAssignee;
            }
        }

        private Boolean _CPAssignee = false;
        public Boolean CPAssignee
        {
            get;
            set;
        }

        private DateTime _lastFollowupUpdateDate = DateTime.MinValue;
        public DateTime lastFollowUpUpdateDate
        {
            set { _lastFollowupUpdateDate = value; }
            get
            {
                if (_lastFollowupUpdateDate == null)
                    updateCurrentFollowupInfoFromLog();

                return _lastFollowupUpdateDate;
            }
        }

        private String _lastFollowupUpdateBy = null;
        public String lastFollowupUpdateBy
        {
            set { _lastFollowupUpdateBy = value; }
            get
            {
                if (_lastFollowupUpdateBy == null)
                    updateCurrentFollowupInfoFromLog();

                return _lastFollowupUpdateBy;
            }
        }

        private String _lastActivity = null;
        public String lastActivity
        {
            set { _lastActivity = value; }
            get
            {
                if (_lastActivity == null)
                    updateCurrentFollowupInfoFromLog();

                return _lastActivity;
            }
        }

        public List<TrackingLog> followupActivities
        {
            get
            {
                if (trackType == TrackingLog.TrackType.NOT_SPECIFIED ||
                    String.IsNullOrEmpty(trackInstanceID))
                    return null;
                else
                {
                    //retrieve following activity from DB
                    TrackingLogHelper helper = new TrackingLogHelper();
                    List<TrackingLog> logs = helper.getLogs(trackType, trackInstanceID);

                    return logs;
                }
            }
        }

        //Followup Status, [Quotation]
        private static String[] _availableStatusesSet = {"N/A", "New Lead", "Validating", "Quoting","Negotiating", 
                                                                                "Waiting for PO/Approval", "Won - PO input in SAP", "Lost", "Terminated"};
        public static List<String> availableFollowUpStatuses = new List<string>(_availableStatusesSet);

        //FollowUp Status [shopping cart/cart abandon]
        private static String[] _cartAbandonReasonFollowUpStatues = { "N/A", "Navigation Problem", "Just Browsing", "Long Availability","MOQ","Need more discovery", 
                                                                 "Price","Product Compatibility","Product Quality","Project Cancelled","Qualifying","Shipping Cost","Waiting for PO/Approval",
                                                                    "Completed Order","Submitted Quotation","Submitted Service Request","Order Processed","Proposing/Quoting","RFQ/RFI","Others"};
        public static List<String> CartAbandonReasonFollowUpStatues = new List<string>(_cartAbandonReasonFollowUpStatues);



        ////Followup Status, should be the same as Sieble
        //private static String[] _abandonReasonStatusesSet = {"Navigation Problem", "Just Browsing", "Waiting For PO/Approval", "Website Error", "Using another product", "Have Questions", "Others"};
        //public static List<String> AbandonReasonFollowUpStatuses = new List<string>(_abandonReasonStatusesSet);

        ////Followup Status, should be the same as Sieble
        //private static String[] _abandonCartReason = { "N/A","Ordered","Pricing","Shipping cost","Features","Waiting for PO","Tendering for Project","Others"};
        //public static List<String> AbandonCartReason = new List<string>(_abandonCartReason);
        ///-------------------------------------------------------------------------------------///
        //FollowUp Open Status [Membership]
        public static String[] MemberFollowUpStatues = { "N/A", "New Lead", "Validating", "Open", "open", "" };
        //FollowUp Open Status [Order, Quotation, Contact Us, Discount Service]
        public static String[] OrderFollowUpStatues = { "N/A", "New Lead", "Validating", "Quoting / Negotiating", "Quoting", "Negotiating", "Waiting for PO/Approval", "Open", "open", "" };
        //FollowUp Open Status [Cart Abandoned, User Shopping Cart]
        public static String[] ShippingCartFollowUpStatues = { "N/A", "Others", "Open", "open", "" };

        private void updateCurrentFollowupInfoFromLog()
        {
            List<TrackingLog> logs = followupActivities;
            if (logs != null && logs.Count() > 0)
            {
                _currentFollowupStatus = logs[0].FollowUpStatus;
                _currentFollowupComment = logs[0].FollowUpComments;
                _currentFollowupAssignee = logs[0].AssignTo;
                _CPAssignee = logs[0].AssignToCP.GetValueOrDefault();
                _lastFollowupUpdateBy = logs[0].LastUpdateBy;
                _lastFollowupUpdateDate = logs[0].LastUpdated.GetValueOrDefault();
                _lastActivity = logs[0].Activity;
            }
            else
            {
                //如果有属性调用后,trackinglog 没有数据,其他属性就没必要再来查了.
                _currentFollowupStatus = _currentFollowupComment = _currentFollowupAssignee = _lastFollowupUpdateBy = _lastActivity ="";
                _CPAssignee = false;
                _lastFollowupUpdateDate = DateTime.Now;
            }
        }

        /// <summary>
        /// This method is to keep follow-up activity of Member, UserRequest, Order and Quotation.  Other types are not
        /// supported at this moment and it returns false when it's not supported or the activity logging fails.
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="activity"></param>
        /// <param name="sentMailtoAssignee"></param>
        /// <returns></returns>
        public bool logFollowupActivity(string currentUser,string activity,bool sentMailtoAssignee=false,string currentStoreId = "AUS",string templateMail = "")
        {
            Boolean isSupported = true;
            if (this.trackType == TrackingLog.TrackType.NOT_SPECIFIED ||
                String.IsNullOrEmpty(this.trackInstanceID) ||
                String.IsNullOrEmpty(this.instanceOwnerID))
                isSupported = false;
            else
            {
                try
                {
                    TrackingLog log = new TrackingLog();
                    log.UserID = this.instanceOwnerID;
                    if (string.IsNullOrEmpty(this.trackInstanceStoreID))
                    {
                        this._trackInstanceStoreID = currentStoreId;
                    }
                    log.StoreID = this.trackInstanceStoreID;
                    log.TrackingNo = this.trackInstanceID;
                    log.TrackingType = this.trackType.ToString();
                    log.FollowUpStatus = this.currentFollowupStatus;
                    log.FollowUpComments = this.currentFollowupComment;
                    log.LastUpdateBy = currentUser;
                    log.LastUpdated = DateTime.Now;
                    log.AssignTo = this.currentFollowUpAssignee;
                    log.Activity = activity;
                    log.save();
                    //in US LDR process, this is replaced by UNICA lead assign and follow-up. This logic will be applied to non-US regions only 4/16/2013
                    if (sentMailtoAssignee && this.trackInstanceStoreID != "AUS")
                    {
                        sentChangingMail(currentUser, templateMail);
                    }
                }
                catch (Exception ex)
                {
                    eStore.Utilities.eStoreLoger.Error("logFollowupActivity failed", "", "", "", ex);
                    isSupported= false;
                }
            }
            return isSupported;
         
        }

        private void sentChangingMail(string currentUser, string templateMail = "")
        { 
          try
            {
                if (!string.IsNullOrEmpty(this.currentFollowUpAssignee)&& !string.IsNullOrEmpty(currentUser))
                {
                    string mailToAddress = this.currentFollowUpAssignee;
                    string myEmailAddress = currentUser;
                    string htmlContent = composeMailContent(templateMail);
                    esUtilities.EMail mail = new esUtilities.EMail(mailToAddress, myEmailAddress, "Advantech eStore", string.Format("{0} information({1})", this.trackType, this.trackInstanceStoreID),
                                                htmlContent, this.trackInstanceStoreID);
                    //mail.MailCCAddress = new List<string> { myEmailAddress };
                    var mailResutl = mail.sendMailNow();                   
                }
            }
            catch (Exception ex)
            {
                eStore.Utilities.eStoreLoger.Error("sentChangingMail failed", "", "", "", ex);
              
            }
        
        }

        /// <summary>
        /// for compose Mail Content, this function can be overwrited by sub class, so user/order etc. can have theirself special content
        /// </summary>
        /// <returns>return followup mail content</returns>
        protected virtual string composeMailContent(string templateMail = "")
        {
            if (!string.IsNullOrEmpty(templateMail))
                return templateMail;
            else
                return string.Format("The owner of <span style=\"color:#F00\">{0}</span>, {1}, has been assigned to you.  You can find more details in eStore OM.", this.trackType, this.trackInstanceID); 
        }
    }
}
