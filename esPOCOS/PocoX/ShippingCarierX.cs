using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

public partial class ShippingCarier { 
 
#region Extension Methods 
    /// <summary>
    ///  To get the service description by service code  
    /// </summary>
    /// <param name="serviceCode"></param>
    /// <param name="rateServiceName"></param>
    /// <returns>service name / description</returns>
    public string getServiceName(string serviceCode)
    {
        String serviceName = "";
        serviceName = (from rateService in RateServiceNames
                       where rateService.MessageCode.Equals(serviceCode)
                       select rateService.DefaultServiceName).FirstOrDefault();
        return String.IsNullOrEmpty(serviceName) ? "" : serviceName;
    }

        public RateServiceName GetRateServiceByName(string serviceCode)
        {
            return RateServiceNames.FirstOrDefault(c => c.MessageCode.Equals(serviceCode));
        }

        #endregion
    } 
 }