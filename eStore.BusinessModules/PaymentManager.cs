using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    public class PaymentManager
    {
        public IPaymentSolutionProvider getPaymentSolutionProvider(StorePayment providerInfo)
        {
            IPaymentSolutionProvider provider = null;

            try
            {
                provider = (IPaymentSolutionProvider) Activator.CreateInstance(Type.GetType(providerInfo.PaymentClass));
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("PaymentProvider " + providerInfo.PaymentClass, "", "", "", ex);
            }

            return provider;
        }
    }
}
