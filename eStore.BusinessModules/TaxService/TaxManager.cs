using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.BusinessModules.TaxService
{
    public  class TaxManager
    {
        public TaxCalculator getTaxProvider(string taxProvider)
        {
            TaxCalculator provider = null;

            try
            {
                provider = (TaxCalculator)Activator.CreateInstance(Type.GetType("eStore.BusinessModules.TaxService." + taxProvider));
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("PaymentProvider " + taxProvider, "", "", "", ex);
            }

            return provider;
        }

    }
}
