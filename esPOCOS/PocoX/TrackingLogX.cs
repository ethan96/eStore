using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;


namespace eStore.POCOS
{
    public partial class TrackingLog
    {
        public enum TrackType { ORDER, QUOTATION, LEADS, USERREQUEST, REQUESTDISCOUNT, CONTACTUS, NOT_SPECIFIED, CART, SERVICEREQUEST, CALLMENOW, GENERALINQUIRIES, TECHNICALSUPPORT, SALES }
        
        /// <summary>
        /// This is an override method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String formatedContent = "";
            try
            {
                formatedContent = String.Format("\r{0}\r{1} {2} - {3}\r{4}\r", LastUpdated, LastUpdateBy, Activity, FollowUpStatus, FollowUpComments);
            }
            catch (Exception)
            {
                //ignore and do nothing
            }
            return formatedContent;
        }

        public string historyResult()
        {
            String formatedContent = "";
            try
            {
                formatedContent = String.Format("{0}   {1} <br> {2} - {3}  {4}<br>", LastUpdated, LastUpdateBy, Activity, FollowUpStatus, FollowUpComments);
            }
            catch (Exception)
            {
                //ignore and do nothing
            }
            return formatedContent;
        }
    }
}
