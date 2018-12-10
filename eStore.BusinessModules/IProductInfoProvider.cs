using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    interface IProductInfoProvider
    {
        /// <summary>
        /// This function shall return provider class name
        /// </summary>
        /// <returns>provider implementation class name</returns>
        String getProviderId();

        /// <summary>
        /// This method is to retrieve product auxilary information from the price provider
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>An instance of eStore.BusinessModules.ProductInfo</returns>
        ProductInfo getProductInfo(String productId);
    }
}
