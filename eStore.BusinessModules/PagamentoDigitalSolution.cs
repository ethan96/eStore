using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web.Security;

namespace eStore.BusinessModules
{
    class PagamentoDigitalSolution : PaymentSolution
    {

        /// <summary>
        /// The return value of this method indicates whether this payment solution provider support direct API call.
        /// </summary>
        /// <returns></returns>
        public override Boolean supportDirectAccess() { return false; }

        /// <summary>
        /// getIndirectPaymentRequestForm shall be invoked in paired with processIndirectPaymentResponse.
        /// Application call getIndirectPaymentRequestForm to embedded it in returning HTML page for client
        /// to submit transaction request to Payment solution provider.  Once payment solution provider complete
        /// payment transaction, it will redirect user back to eStore with transaction status. processIndirectPaymentResponse
        /// will take care of the status validation and create a payment result if it's a successful payment transaction
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public override IDictionary<String, String> getIndirectPaymentRequestForm(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            Dictionary<String, String> formItems = new Dictionary<string, string>();


            
            //string EncryptedOder = this.SignAndEncrypt(order);
            formItems.Add("email_loja", "Rosane@advantech.com.br");

            formItems.Add("produto_codigo_1", order.OrderNo);
            formItems.Add("produto_descricao_1", "eStore Order - " + order.OrderNo);
            formItems.Add("produto_qtde_1", "1");
            formItems.Add("produto_valor_1", order.totalAmountX.ToString());

            formItems.Add("tipo_integracao", "PAD");
            //formItems.Add("frete", "");
            formItems.Add("url_retorno", esUtilities.CommonHelper.GetStoreLocation() + "completeIndirectPayment.aspx");
            formItems.Add("redirect", "true");
            formItems.Add("redirect_time", "10");
            formItems.Add("cliente_razao_social", order.storeX.StoreName);
            string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(string.Join("&",formItems.OrderBy(x=>x.Key).Select(x=>string.Format("{0}={1}",x.Key,x.Value).ToArray())), "MD5");
            formItems.Add("hash", hash);
            formItems.Add("actionURL", "https://www.pagamentodigital.com.br/checkout/pay/");
            return formItems;
        }
        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            if (response != null && response["d_transacao"] != null)
                return (string)response["d_transacao"];
            else
                return string.Empty;
        }
        public override Payment processIndirectPaymentResponse(NameValueCollection responseValues, Order order, Boolean simulation = false)
        {
            //retrieve pending payment from order
            Payment payment = order.getLastOpenPayment();
            if (payment == null)
            {
                payment = new Payment();

                try
                {
                    payment.Amount = order.totalAmountX; ;
                    payment.OrderNo = order.OrderNo;
                }
                catch (Exception)
                {

                }
                //payment = new Payment(strOrderNO, getValue(response, "CN"), cardType, getValue(response, "ED"), "", paymentAmount);
            }
            try
            {

                Dictionary<String, String> response = convertToDictionary(responseValues);

                //fill up payment values
                String strtx = getValue(response, "tx");

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.pagamentodigital.com.br/checkout/verify/");

                //Set values for the request back
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                string strRequest = null;
                strRequest = "transacao=" + getValue(response, "id_transacao") +
"&status=" + getValue(response, "status") +
"&cod_status=" + getValue(response, "cod_status") +
"&valor_original=" + getValue(response, "valor_original") +
"&valor_loja=" + getValue(response, "valor_loja") +
"&token=3FEDA467557399DB52E0C10A47136FF1";
                req.ContentLength = strRequest.Length;
                //Send the request to PayPal and get the response
                StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
                streamOut.Write(strRequest);
                streamOut.Close();
                StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                string sQuerystring = streamIn.ReadToEnd();
                streamIn.Close();


                if (!string.IsNullOrEmpty(sQuerystring) && sQuerystring.Equals("VERIFICADO"))
                {

                    switch (getValue(response, "cod_status"))
                    {
                        case "1":
                            payment.statusX = Payment.PaymentStatus.Approved;
                            break;
                        default:
                            payment.statusX = Payment.PaymentStatus.Declined;
                            break;
                    }
                    payment.responseValues = response;

                }
                else
                {
                    payment.statusX = Payment.PaymentStatus.Declined;
                }




            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at PagamentoDigitalSolution payment", "", "", "", ex);
                payment.statusX = Payment.PaymentStatus.Declined;
            }

            return payment;
        }


        private String getValue(Dictionary<String, String> dictionary, String key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return "";
        }

        private Dictionary<String, String> convertToDictionary(NameValueCollection items)
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



    }
}
