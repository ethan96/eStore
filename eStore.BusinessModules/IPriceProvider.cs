using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
    interface IPriceProvider
    {
        /// <summary>
        /// This function shall return provider class name
        /// </summary>
        /// <returns>provider implementation class name</returns>
        String getProviderId();

        /// <summary>
        /// This method is to retrieve product price from the price provider
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>An instance of eStore.BusinessModules.Price</returns>
        Decimal getPrice(String productId);
    }
}
