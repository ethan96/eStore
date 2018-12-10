using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Collections.Specialized;
using eStore.Utilities;
using Uol.PagSeguro;
using System.Reflection;

namespace eStore.BusinessModules
{
    class PagSeguroSolution : PaymentSolution
    {
        AccountCredentials credentials = new AccountCredentials("rosane@advantech.com.br", "4341A4B0D10E441DAD9BBDA245C22F9C");

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

            try
            {
                PaymentRequest payment = new PaymentRequest();
                payment.Currency = Uol.PagSeguro.Currency.Brl;

                payment.Items.Add(new Item(order.OrderNo
                    , "eStore Order - " + order.OrderNo
                    , 1
                    , order.totalAmountX
                    , 100
                    , 0.00m));
                //payment.Items.Add(new Item("0002", "Notebook Rosa", 2, 2560, 750, 0));

                payment.Reference = order.OrderNo;
                //payment.Shipping = new Shipping();
                //payment.Shipping.ShippingType = ShippingType.Sedex;
                //payment.Shipping.Address = new Uol.PagSeguro.Address(order.cartX.ShipToContact.countryCodeX,
                //    order.cartX.ShipToContact.State,order.cartX.ShipToContact.City,"", order.cartX.ShipToContact.ZipCode, order.cartX.ShipToContact.Address1, "", "");
                //payment.Sender = new Sender(order.cartX.BillToContact.FirstName +" "+ order.cartX.BillToContact.LastName
                //    , order.cartX.BillToContact.UserID, new Phone(order.cartX.BillToContact.TelExt, order.cartX.BillToContact.TelNo));
                payment.RedirectUri = new Uri(esUtilities.CommonHelper.GetStoreLocation() + "completeIndirectPayment.aspx");
                Uri paymentRedirectUri = PaymentService.Register(credentials, payment);
                formItems.Add("actionURL", paymentRedirectUri.AbsoluteUri);
               
            }
            catch (PagSeguroServiceException exception)
            {
                eStoreLoger.Error("Exception at getIndirectPaymentRequestForm payment", "", "", "", exception);
            }
            return formItems;
        }
        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            try
            {
                string TRANSACTION_ID = response["TRANSACTION_ID"];
                Transaction transaction = TransactionSearchService.SearchByCode(credentials, TRANSACTION_ID);
                if (transaction != null)
                    return transaction.Reference;
                else
                    return string.Empty;
            }
            catch (Exception)
            {

                return string.Empty;
            }
           
              
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
            
            string TRANSACTION_ID = responseValues["TRANSACTION_ID"];
            Transaction transaction = TransactionSearchService.SearchByCode(credentials, TRANSACTION_ID);

            try
            {

//一	等待付款，买方发起的交易，但到目前为止还没有收到PayPal付款信息。
//2	分析：买方选择用信用卡支付和PayPal交易的风险分析。
//3	支付：交易是由买方支付，贝宝已经收到了确认的金融机构，负责处理。
//4	可供交易支付，并没有被退回，没有打开的任何争端达成其释放期结束。
//5	争议：内释放交易的买方，开了一个争端。
//6	返回，交易金额退还买方。
//7	取消：取消已敲定这笔交易。

                switch (transaction.TransactionStatus)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                        payment.statusX = Payment.PaymentStatus.NeedAttention;
                        break;
                    case 4:
                        if (transaction.Items[0].Amount == order.totalAmountX)
                            payment.statusX = Payment.PaymentStatus.Approved;
                        else
                            payment.statusX = Payment.PaymentStatus.FraudAlert;
                        break;
                    default:
                        payment.statusX = Payment.PaymentStatus.Declined;
                        break;
                }


                payment.CCPNREF = transaction.Code;
                payment.CCUser1 = transaction.Sender.Email;
                payment.CCResultCode = transaction.TransactionStatus.ToString();

//                1	Cartão de crédito: O comprador pagou pela transação com um cartão de crédito. Neste caso, o pagamento é processado imediatamente ou no máximo em algumas horas, dependendo da sua classificação de risco.
//2	Boleto: O comprador optou por pagar com um boleto bancário. Ele terá que imprimir o boleto e pagá-lo na rede bancária. Este tipo de pagamento é confirmado em geral de um a dois dias após o pagamento do boleto. O prazo de vencimento do boleto é de 3 dias.
//3	Débito online (TEF): O comprador optou por pagar com débito online de algum dos bancos com os quais o PagSeguro está integrado. O PagSeguro irá abrir uma nova janela com o Internet Banking do banco escolhido, onde o comprador irá efetuar o pagamento. Este tipo de pagamento é confirmado normalmente em algumas horas.
//4	Saldo PagSeguro: O comprador possuía saldo suficiente na sua conta PagSeguro e pagou integralmente pela transação usando seu saldo.
//5	Oi Paggo: o comprador paga a transação através de seu celular Oi. A confirmação do pagamento acontece em até duas horas.

//Código	Significado
//101	Cartão de crédito Visa.
//102	Cartão de crédito MasterCard.
//103	Cartão de crédito American Express.
//104	Cartão de crédito Diners.
//105	Cartão de crédito Hipercard.
//106	Cartão de crédito Aura.
//107	Cartão de crédito Elo.
//108	Cartão de crédito PLENOCard.
//109	Cartão de crédito PersonalCard.
//201	Boleto Bradesco. *
//202	Boleto Santander.
//301	Débito online Bradesco.
//302	Débito online Itaú.
//303	Débito online Unibanco. *
//304	Débito online Banco do Brasil.
//305	Débito online Banco Real. *
//306	Débito online Banrisul.
//307	Débito online HSBC.
//401	Saldo PagSeguro.
//501	Oi Paggo.
                payment.CardType = transaction.PaymentMethod.PaymentMethodCode.ToString() + "/" + transaction.PaymentMethod.PaymentMethodType.ToString();

                payment.responseValues = convertToDictionary(transaction); 
 



            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at CTCBRedirect payment", "", "", "", ex);
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

        private Dictionary<String, String> convertToDictionary(Transaction transaction)
        {
            Dictionary<String, String> dictionary = new Dictionary<string, string>();

            try
            {
                PropertyInfo[] props = transaction.GetType().GetProperties();

                    foreach (PropertyInfo prop in props.OrderBy(x => x.Name))
                    {

                        dictionary.Add(prop.Name, prop.GetValue(transaction, null).ToString());
                    }
                    dictionary.Add("Sender.Email", transaction.Sender.Email);
                    dictionary.Add("Sender.Name", transaction.Sender.Name);
                    dictionary.Add("Sender.Phone", transaction.Sender.Phone.AreaCode + "-" + transaction.Sender.Phone.Number);
                    dictionary.Add("PaymentMethod.PaymentMethodCode", transaction.PaymentMethod.PaymentMethodCode.ToString());
                    dictionary.Add("transaction.PaymentMethod.PaymentMethodType", transaction.PaymentMethod.PaymentMethodType.ToString());
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        /// <summary>
        /// This disctionary provides interface to get human readable description of specifying status code
        /// </summary>
        private static Dictionary<String, String> _statusTable = new Dictionary<string, string>()
        {
            {"0", "Authorized"} 
        };


        /// <summary>
        /// This method is to interpret payment return status and convert it to eStore standandard payment status
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="status"></param>
        protected void setPaymentStatus(Payment payment, String statusCode, String cvcCode, String acceptanceCode)
        {
            if (cvcCode != null && cvcCode.Equals("OK"))
            {
                if (_statusTable.ContainsKey(statusCode))
                {
                    payment.TransactionDesc = _statusTable[statusCode];
                    if (!String.IsNullOrEmpty(acceptanceCode) &&
                        (statusCode.Equals("0")))
                    {
                        payment.statusX = Payment.PaymentStatus.Approved;
                    }
                    else
                        payment.statusX = Payment.PaymentStatus.Declined;
                }
                else
                {
                    payment.TransactionDesc = "Rejected";
                    payment.statusX = Payment.PaymentStatus.Declined;
                }
            }
            else
            {
                payment.TransactionDesc = "CVC Failed";
                payment.statusX = Payment.PaymentStatus.Declined;
            }
        }

        private void setPaymentInfo(Payment payment, Order order, Dictionary<String, String> response)
        {
            try
            {
                payment.CCResultCode = getValue(response, "STATUS");
                payment.CCPNREF = getValue(response, "PNREF");
                payment.CCAuthCode = getValue(response, "PAYID");
                payment.CCRESPMSG = getValue(response, "NCERROR");
                payment.CCAVSAddr = "";
                payment.CCAVSZIP = "";
                payment.CCIAVS = getValue(response, "BRAND");
                payment.CCPREFPSMSG = "";
                payment.CCPOSTFPSMSG = getValue(response, "ACCEPTANCE");
                payment.CardType = getValue(response, "PM");
                payment.cardNo = getValue(response, "CARDNO");
                payment.CardHolderName = getValue(response, "CN");
                payment.CardExpiredDate = getValue(response, "ED");
                payment.CCUser1 = payment.CardHolderName;

                //convert namevaluecollection to dictionary
                payment.responseValues = response;

                payment.LastFourDigit = Convert.ToInt16(payment.cardNo.Substring(payment.cardNo.Length - 4));
            }
            catch (Exception)
            {
            }
        }

    }
}