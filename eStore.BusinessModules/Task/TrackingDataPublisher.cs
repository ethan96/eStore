using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using eStore.Utilities;

namespace eStore.BusinessModules.Task
{

    public class TrackingDataPublisher : TaskBase
    {
        string trackingurl = string.Empty;
        public TrackingDataPublisher() { }
        public TrackingDataPublisher(string url)
        {
            trackingurl = url;
        }
        public override bool PreProcess()
        {
            bool status = base.PreProcess();
            if (string.IsNullOrEmpty(trackingurl))
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
            if (string.IsNullOrEmpty(trackingurl))
                return null;
            else
            {
                try
                {
                    using (System.Net.WebClient client = new System.Net.WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                        client.UploadString(new Uri(trackingurl), string.Empty);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Event Publish WebService Error: " + ex.StackTrace, String.IsNullOrEmpty(trackingurl) ? "" : trackingurl, "", "", ex);
                    OnFailed();
                    return null;
                }
            }
        }
    }
}
