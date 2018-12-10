using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Common;
using System.Data;
namespace eStore.POCOS.DAL
{

    public partial class ServiceRequestHelper : Helper
    {
        /// <summary>
        /// Return Service request from support portal under the given DMF.
        /// </summary>
        /// <param name="dmf"></param>
        /// <returns></returns>

        public List<VServiceRequest> getServiceRequest(DMF dmf, DateTime startdate, DateTime enddate, string activity_Id="")
        {
            try
            {
                List<string> countries = dmf.getCountryCoverage(true);  //including country abbreviation
                enddate = enddate.Date.AddHours(24);
                List<VServiceRequest> trackRequestList = new List<VServiceRequest>();

                var vsrList = (from vsr in context.VServiceRequests
                              let t = context.TrackingLogs.Where(p => p.TrackingNo == vsr.Activity_Id).OrderByDescending(x => x.LogId).FirstOrDefault()
                              where vsr.CREATED_DATE >= startdate && vsr.CREATED_DATE <= enddate && countries.Contains(vsr.country) && (!string.IsNullOrEmpty(activity_Id) ? vsr.Activity_Id == activity_Id : true)
                              select new
                              {
                                  Company = vsr.Company,
                                  country = vsr.country,
                                  City = vsr.City,
                                  State = vsr.State,
                                  COMMENT = vsr.COMMENT,
                                  CREATED_BY = vsr.CREATED_BY,
                                  Name = vsr.Name,
                                  Phone = vsr.Phone,
                                  CREATED_DATE = vsr.CREATED_DATE,
                                  Status = !string.IsNullOrEmpty(t.FollowUpStatus) ? t.FollowUpStatus
                                                     : !string.IsNullOrEmpty(vsr.ServiceRequestStatus) ? vsr.ServiceRequestStatus : "Open",
                                  FollowupAssignee = string.IsNullOrEmpty(t.AssignTo) ? "N/A" : t.AssignTo,
                                  FollowupComment = t.FollowUpComments == null ? "" : t.FollowUpComments,
                                  FollowupUpdateBy = t.LastUpdateBy == null ? "" :t.LastUpdateBy,
                                  Activity_Id = vsr.Activity_Id,
                                  Address = vsr.Address,
                                  DESCRIPTION = vsr.DESCRIPTION,
                                  Product = vsr.Product
                              }).ToList();
                if (vsrList.Count() > 0)
                {
                    foreach (var item in vsrList)
                    {
                        VServiceRequest vRequest = new VServiceRequest();
                        vRequest.Company = item.Company;
                        vRequest.country = item.country;
                        vRequest.City = item.City;
                        vRequest.State = item.State;
                        vRequest.COMMENT = item.COMMENT;
                        vRequest.CREATED_BY = item.CREATED_BY;
                        vRequest.Name = item.Name;
                        vRequest.Phone = item.Phone;
                        vRequest.CREATED_DATE = item.CREATED_DATE;
                        vRequest.Status = item.Status;
                        vRequest.currentFollowupComment = item.FollowupComment;
                        vRequest.currentFollowUpAssignee = item.FollowupAssignee;
                        vRequest.lastFollowupUpdateBy = item.FollowupUpdateBy;
                        vRequest.Activity_Id = item.Activity_Id;
                        vRequest.Address = item.Address;
                        vRequest.DESCRIPTION = item.DESCRIPTION;
                        vRequest.Product = item.Product;
                        trackRequestList.Add(vRequest);
                    }
                }
                return trackRequestList;
            }
            catch (Exception e) {
                eStoreLoger.Fatal(e.Message, "", "", dmf.StoreID, e);
                return null;
            }
        }

        /// <summary>
        /// Return a single Service Request
        /// </summary>
        /// <param name="dmf"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public VServiceRequest getServiceRequest(int sn_id, string activityID)
        {
            try
            {
                 
                var _sr = (from sr in context.VServiceRequests
                           where sr.SN_ID==sn_id && sr.Activity_Id == activityID
                           select sr).FirstOrDefault();


                return _sr;
            }
            catch (Exception e)
            {
                eStoreLoger.Fatal("getServiceRequest Failed", "", "","", e);
                return null;

            }

        }


        /// <summary>
        ///  This function is to get Support portal service request tracking logs. 
        /// </summary>
        /// <param name="vsr"></param>
        /// <returns></returns>
        public List<ServiceRequestTracking> getServiceRequestLog(string id)
        {
            try
            {                          
                var _sr = (from sr in context.ServiceRequestTrackings
                           where sr.ServiceRequestID  == id
                           orderby sr.UpdateDate descending
                           select sr);

                if (_sr != null)
                    return _sr.ToList();
                else
                    return new List<ServiceRequestTracking>();
            }
            catch (Exception e)
            {
                eStoreLoger.Fatal("GetServiceTrackingLogError", "", "","", e);
                return null;

            }

        }


        /// <summary>
        ///  This function is to get Support portal service request tracking logs. 
        /// </summary>
        /// <param name="vsr"></param>
        /// <returns></returns>
        public ServiceRequestTracking getServicetrackinglogbyid(int id)
        {
            try
            {
                var _sr = (from sr in context.ServiceRequestTrackings
                           where sr.ID  == id
                          select sr).FirstOrDefault();                
                return _sr;
                
            }
            catch (Exception e)
            {
                eStoreLoger.Fatal("GetServiceTrackingLogError", "", "", "", e);
                return null;

            }

        }




        #region CRUD

        public int save(ServiceRequestTracking _log)
        {
            //if parameter is null or validation is false, then return  -1 
            if (_log == null || _log.validate() == false) return 1;

            ServiceRequestTracking exist_log = getServicetrackinglogbyid(_log.ID);

            try{

                if (exist_log == null)
                {
                    //Insert                  
                    context.ServiceRequestTrackings.AddObject(_log);
                    context.SaveChanges();
                    return 0;
                }
                else {

                    //update
                    context.ServiceRequestTrackings.ApplyCurrentValues(_log);
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

   

        #endregion


        #region Others

        private static string myclassname()
        {
            return typeof(ServiceRequestHelper).ToString();
        }
        #endregion
    }
}