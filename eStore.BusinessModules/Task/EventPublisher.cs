using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.BusinessModules.SSO.Advantech;
using eStore.POCOS;
using eStore.POCOS.PocoX;
using Amib.Threading;
using eStore.Utilities;

namespace eStore.BusinessModules.Task
{
    public class EventPublisher : TaskBase
    {
#region Constructor
        public EventPublisher() { }

        public EventPublisher(String eventPage, POCOS.PocoX.FollowUpable followUpableobj, EventType specialAction = EventType.NewUserRequest)
        {
            try
            {
                AdvantechOnlineRequestV2Adapter adapter = new AdvantechOnlineRequestV2Adapter();
                Request = adapter.convert2OnlineRequest(followUpableobj);
                Request.website = eventPage;
                Request.company = Request.company ?? "N/A";
                if (specialAction == EventType.AbortCheckout && followUpableobj is POCOS.Order)
                {
                    Request.activityTypeEnum = EnumActivityType.eStoreAbortCheckOut;
                }

                followupableInstance = followUpableobj;
            }
            catch (Exception ex)
            {
                if (followupableInstance is Order)
                    eStore.Utilities.eStoreLoger.Error("Fails at converting eStore OrderConfirm event to UNICA event", "", "", "", ex);
                else if (followupableInstance is Quotation)
                    eStore.Utilities.eStoreLoger.Error("Fails at converting eStore QuotationConfirm event to UNICA event", "", "", "", ex);
                else if (followupableInstance is UserRequest)
                    eStore.Utilities.eStoreLoger.Error("Fails at converting eStore user request event to UNICA event", "", "", "", ex);
                else  //generic error message
                    eStore.Utilities.eStoreLoger.Error("Fails at converting eStore event to UNICA event", "", "", "", ex);
            }
        }        
        
        /// <summary>
        /// addto cart/build system
        /// </summary>
        /// <param name="user"></param>
        /// <param name="eventType"></param>
        /// <param name="ProductID"></param>
        public EventPublisher(String eventPage, POCOS.User user, string ProductID, EventType eventType)
        {
            if (user != null && user.isEmployee() == false) //filter out employee events
            {
                Request = new OnlineRequestV2();
                Request.firstName = user.mainContact.FirstName;
                Request.lastName = user.mainContact.LastName;
                Request.company = String.IsNullOrWhiteSpace(user.mainContact.AttCompanyName) ? "N/A" : user.mainContact.AttCompanyName;
                Request.email = user.UserID;
                Request.phone = user.mainContact.Mobile;
                Request.zipCode = user.mainContact.ZipCode;
                Request.address = user.mainContact.Address1;
                Request.city = user.mainContact.City;
                Request.state = user.mainContact.State;
                Request.country = user.mainContact.Country;
                if (user.MemberX != null && !string.IsNullOrEmpty(user.MemberX.COUNTRY)) //Use memberhsip country for instead
                    Request.country = user.MemberX.COUNTRY;
                Request.subject = "eStore  " + eventType.ToString();
                Request.product = ProductID;
                Request.requestType = 0;
                Request.requestTypeEnum = EnumRequestType.Activity;
                Request.description = "";
                Request.ownerEmail = "";
                Request.activityType = 0;//6 //WebInbound
                Request.website = eventPage;

                switch (eventType)
                {
                    case EventType.BuildSystem:
                        Request.activityTypeEnum = EnumActivityType.eStoreBuildSystem;
                        break;
                    case EventType.Add2Cart:
                        Request.activityTypeEnum = EnumActivityType.eStoreAddToCart;
                        break;
                    case EventType.PrintProductPage:
                        Request.activityTypeEnum = EnumActivityType.eStore_Print_Product_Page;
                        break;
                    case EventType.ClickTodayHighlightBanner:
                        Request.activityTypeEnum = EnumActivityType.eStore_Click_Today_Highlight_Banner;
                        break;
                    case EventType.ClickHomeBanner:
                        Request.activityTypeEnum = EnumActivityType.eStore_Click_Home_Banner;
                        break;
                    case EventType.BrowseExtenedSpec:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_Extened_Spec;
                        break;
                    case EventType.SeeConfiguredSystems:
                        Request.activityTypeEnum = EnumActivityType.eStore_See_Configured_Systems;
                        break;
                    case EventType.BrowseNews:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_News;
                        break;
                    case EventType.BrowseWhitePaper:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_White_Paper;
                        break;
                    case EventType.DownloadDatasheet:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Datasheet;
                        break;
                    case EventType.DownloadFiles:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Files;
                        break;
                    case EventType.DownloadDriver:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Driver;
                        break;
                    case EventType.DownloadManual:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Manual;
                        break;
                    case EventType.DownloadUtilities:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Utilities;
                        break;
                    case EventType.PrintComparisonTable:
                        Request.activityTypeEnum = EnumActivityType.eStore_Print_Comparison_Table;
                        break;
                    case EventType.AbortCheckOut:
                        Request.activityTypeEnum = EnumActivityType.eStore_Abort_Check_Out;
                        break;
                    case EventType.CallInbound:
                        Request.activityTypeEnum = EnumActivityType.eStore_Call_Inbound;
                        break;
                    case EventType.EmailInbound:
                        Request.activityTypeEnum = EnumActivityType.eStore_Email_Inbound;
                        break;
                    default:
                        break;
                }

                Request.activitySource = 4;
                Request.activitySourceEnum = EnumActivitySource.eStore;
                Request.userType = 4;
                Request.userTypeEnum = EnumUserType.eStore;
            }
        }

        public EventPublisher(String eventPage, POCOS.User user, EventType eventType)
        {
            if (user != null && user.isEmployee() == false)
            {
                Request = new OnlineRequestV2();
                Request.firstName = user.mainContact.FirstName;
                Request.lastName = user.mainContact.LastName;
                Request.company = String.IsNullOrWhiteSpace(user.mainContact.AttCompanyName) ? "N/A" : user.mainContact.AttCompanyName;
                Request.email = user.UserID;
                Request.phone = user.mainContact.Mobile;
                Request.zipCode = user.mainContact.ZipCode;
                Request.address = user.mainContact.Address1;
                Request.city = user.mainContact.City;
                Request.state = user.mainContact.State;
                Request.country = user.mainContact.Country;
                if (user.MemberX != null && !string.IsNullOrEmpty(user.MemberX.COUNTRY)) //Use memberhsip country for instead
                    Request.country = user.MemberX.COUNTRY;
                Request.subject = "eStore  " + eventType.ToString();
                Request.product = string.Empty;
                Request.requestType = 0;
                Request.requestTypeEnum = EnumRequestType.Activity;
                Request.description = "";
                Request.ownerEmail = "";
                Request.activityType = 0;//6 //WebInbound
                Request.website = eventPage;

                switch (eventType)
                {
                    case EventType.BuildSystem:
                        Request.activityTypeEnum = EnumActivityType.eStoreBuildSystem;
                        break;
                    case EventType.Add2Cart:
                        Request.activityTypeEnum = EnumActivityType.eStoreAddToCart;
                        break;
                    case EventType.PrintProductPage:
                        Request.activityTypeEnum = EnumActivityType.eStore_Print_Product_Page;
                        break;
                    case EventType.ClickTodayHighlightBanner:
                        Request.activityTypeEnum = EnumActivityType.eStore_Click_Today_Highlight_Banner;
                        break;
                    case EventType.ClickHomeBanner:
                        Request.activityTypeEnum = EnumActivityType.eStore_Click_Home_Banner;
                        break;
                    case EventType.BrowseExtenedSpec:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_Extened_Spec;
                        break;
                    case EventType.SeeConfiguredSystems:
                        Request.activityTypeEnum = EnumActivityType.eStore_See_Configured_Systems;
                        break;
                    case EventType.BrowseNews:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_News;
                        break;
                    case EventType.BrowseWhitePaper:
                        Request.activityTypeEnum = EnumActivityType.eStore_Browse_White_Paper;
                        break;
                    case EventType.DownloadDatasheet:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Datasheet;
                        break;
                    case EventType.DownloadFiles:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Files;
                        break;
                    case EventType.DownloadDriver:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Driver;
                        break;
                    case EventType.DownloadManual:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Manual;
                        break;
                    case EventType.DownloadUtilities:
                        Request.activityTypeEnum = EnumActivityType.eStore_Download_Utilities;
                        break;
                    case EventType.PrintComparisonTable:
                        Request.activityTypeEnum = EnumActivityType.eStore_Print_Comparison_Table;
                        break;
                    case EventType.AbortCheckOut:
                        Request.activityTypeEnum = EnumActivityType.eStore_Abort_Check_Out;
                        break;
                    case EventType.CallInbound:
                        Request.activityTypeEnum = EnumActivityType.eStore_Call_Inbound;
                        break;
                    case EventType.EmailInbound:
                        Request.activityTypeEnum = EnumActivityType.eStore_Email_Inbound;
                        break;
                    default:
                        break;
                }

                Request.activitySource = 4;
                Request.activitySourceEnum = EnumActivitySource.eStore;
                Request.userType = 4;
                Request.userTypeEnum = EnumUserType.eStore;
            }
        }
        #endregion //Constructor

        #region Task_Execution method
        public override bool PreProcess()
        {
            bool status = base.PreProcess();
            if (Request == null)
                status = false;

            return status;
        }


        /// <summary>
        /// This method will be executed to publish event
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object execute(object obj)
        {
            if (Request == null)
                return null;
            else
            {
                try
                {
                    /* simulator code
                    System.Threading.Thread.Sleep(30000);
                    return true;
                     * */

                    MembershipWebservice mws = new MembershipWebservice();
                    OnlineRequestV2 req = Request;
                    mws.AddOnlineRequestV2WithWS(ref req);
                    if (followupableInstance != null)
                    {
                        followupableInstance.currentFollowupStatus = "N/A";
                        followupableInstance.currentFollowUpAssignee = "N/A";
                        if (!string.IsNullOrEmpty(req.contactRowID) && !string.IsNullOrEmpty(req.requestRowID))
                            followupableInstance.currentFollowupComment = "Siebel Activity Succeeds -- Contact ID : " + req.contactRowID + ", Request ID : " + req.requestRowID
                                + ", Error Message : " + (string.IsNullOrEmpty(req.logMessage) ? "" : req.logMessage);
                        else
                            followupableInstance.currentFollowupComment = "Siebel Activity Fails -- Contact ID : " + (String.IsNullOrEmpty(req.contactRowID) ? "" : req.contactRowID)
                                + ", Request ID : " + (String.IsNullOrEmpty(req.requestRowID) ? "" : req.requestRowID) + ", Error Message : " +
                                (string.IsNullOrEmpty(req.logMessage) ? "" : req.logMessage);
                        followupableInstance.logFollowupActivity("System@eStore", req.subject + " Siebel Activity");
                    }
                    OnCompleted();

                    return true;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Event Publish WebService Error: " + ex.StackTrace, String.IsNullOrEmpty(Request.email) ? "" : Request.email, "", "", ex);                    
                    OnFailed();
                    return null;
                }
            }         
        }


#endregion //Task_Execution method

        public OnlineRequestV2 Request { get; set; }
        private POCOS.PocoX.FollowUpable followupableInstance = null;

    }

    public enum EventType
    {
        OrderConfirm,
        QuoteConfirm,
        NewUserRequest,
        Add2Cart,
        BuildSystem,
        AbortCheckout,
        PrintProductPage,
        ClickTodayHighlightBanner,
        ClickHomeBanner,
        BrowseExtenedSpec,
        SeeConfiguredSystems,
        BrowseNews,
        BrowseWhitePaper,
        DownloadDatasheet,
        DownloadFiles,
        DownloadDriver,
        DownloadManual,
        DownloadUtilities,
        PrintComparisonTable,
        AbortCheckOut,
        CallInbound,
        EmailInbound
    }

}
