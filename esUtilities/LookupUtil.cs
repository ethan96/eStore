using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using esUtilities.USTaxService;


namespace eStore.Utilities
{
    public class LookupUtil
    {
        /// <summary>
        /// This method is an expensive method call since it retrieve value via web service call. The best practice of using it is
        /// to catch its result and reuse it for later need.  Call this method repeatly may drop down the transaction performance.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public static String findState(String zipCode)
        {
            try
            {
                String state = "", county = "", city = "";
                Boolean shippingAlone = false, advanTax = false;
                USTaxService taxService = new USTaxService();
                if (taxService.getZIPInfo(zipCode, ref state, ref county, ref city, ref shippingAlone, ref advanTax))
                {
                    return state;
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                eStoreLoger.Warn("Exception at state lookup", zipCode, "", "", ex);
                return "";
            }
        }
    }
}
