using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    abstract class PaymentSolution : IPaymentSolutionProvider
    {
        /// <summary>
        /// The return value of this method indicates whether this payment solution provider support direct API call.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean supportDirectAccess() { return false; }

        /// <summary>
        /// This method is for transaction made through direct API call
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo">This parameter needs to contain CreditCard informatin, plus charge amount</param>
        /// <param name="simulation">This Boolean value is an flag specifying coming payment transaction is for real or for simulation purpose</param>
        /// <returns></returns>
        public virtual Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            throw new NotImplementedException("This method is not supported by " + this.GetType().ToString());
        }

        /// <summary>
        /// This method provides more transaction type choices
        /// </summary>
        /// <param name="order"></param>
        /// <param name="paymentInfo">This parameter needs to contain CreditCard informatin, plus charge amount</param>
        /// <param name="simulation">This Boolean value is an flag specifying coming payment transaction is for real or for simulation purpose</param>
        /// <returns></returns>
        public virtual Payment makePayment(Order order, Payment paymentInfo, Payment.TransactionType transactionType, Boolean simulation = false)
        {
            throw new NotImplementedException("This method is not supported by " + this.GetType().ToString());
        }

        /// <summary>
        /// getIndirectPaymentRequestForm shall be invoked in paired with processIndirectPaymentResponse.
        /// Application call getIndirectPaymentRequestForm to embedded it in returning HTML page for client
        /// to submit transaction request to Payment solution provider.  Once payment solution provider complete
        /// payment transaction, it will redirect user back to eStore with transaction status. processIndirectPaymentResponse
        /// will take care of the status validation and create a payment result if it's a successful payment transaction
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual IDictionary<String, String> getIndirectPaymentRequestForm(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            throw new NotImplementedException("This method is not supported by " + this.GetType().ToString());
        }
        public virtual string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        { throw new NotImplementedException("This method is not supported by " + this.GetType().ToString()); }

        public virtual Payment processIndirectPaymentResponse(NameValueCollection response, Order order, Boolean simulation = false)
        {
            throw new NotImplementedException("This method is not supported by " + this.GetType().ToString());
        }

        protected virtual int timeout
        {
            get { return 60; }
        }

        protected virtual String logPath
        {
            get
            {
                //the following file path is only for temporary purpose.  It requires further refinement
                StringBuilder filePath = new StringBuilder();
                //filePath.Append(AppDomain.CurrentDomain.BaseDirectory);
                filePath.Append(ConfigurationManager.AppSettings.Get("Log_Path"));
                String fullPath = Path.GetFullPath(filePath.ToString());

                return fullPath;
            }
        }

        protected virtual String configPath
        {
            get
            {
                //the following file path is only for temporary purpose.  It requires further refinement
                StringBuilder filePath = new StringBuilder();
                //filePath.Append(AppDomain.CurrentDomain.BaseDirectory);
                filePath.Append(ConfigurationManager.AppSettings.Get("Config_Path"));
                String fullPath = Path.GetFullPath(filePath.ToString());

                return fullPath;
            }
        }

        protected Dictionary<String, String> convertToDictionary(NameValueCollection items)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();

            try
            {
                foreach (String key in items.AllKeys)
                {
                    if (!String.IsNullOrEmpty(key))
                        dictionary.Add(key, items[key]);
                }
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        protected String getValue(Dictionary<String, String> dictionary, String key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return "";
        }
        
    }
}
