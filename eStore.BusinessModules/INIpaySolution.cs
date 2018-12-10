using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.Utilities;
using esUtilities;
using INIpayNet;
using System.Configuration;
using System.Collections;
using System.IO;

namespace eStore.BusinessModules
{
    class INIpaySolution : PaymentSolution
    {
        private string _testing = ConfigurationManager.AppSettings.Get("TestingMode");

        /******************** Trial Credit Cart ********************/
        //private static string _trialPAN = "5432760201111234"; // Testing card number
        //private static string _trialCVV2 = "123";
        //private static string _trialExpiryDate = "201212";      //yyyyMM
        /*********************************************************/

        private string testingOrderDeptEmail = ConfigurationManager.AppSettings.Get("eStoreItEmailGroup");
        public string TestingOrderDeptEmail
        {
            get { return testingOrderDeptEmail; }
            set { testingOrderDeptEmail = value; }
        }

        public override Boolean supportDirectAccess()
        { return false; }

        public override Payment makePayment(Order order, Payment paymentInfo, Boolean simulation = false)
        {
            try
            {
                if (order == null)
                    throw new Exception("Order is null.");

                if (paymentInfo == null)
                    throw new Exception("PaymentInfo is null.");


            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal("Exception at INIPay payment. OrderNO is " + order.OrderNo, "", "", order.StoreID, ex);
            }

            return paymentInfo;
        }

        public override IDictionary<String, String> getIndirectPaymentRequestForm(Order order , Payment paymentInfo, Boolean simulation = false)
        {
            Dictionary<String, String> formItems = new Dictionary<string, string>();
            formItems.Add("actionURL", "/INIsecureStart.aspx");
            if (simulation)
            {
                formItems.Add("mid", "INIpayTest");
            }
            else
            {
                if (order.PaymentType == "INIpayvbank")
                    formItems.Add("mid", "IESadvante");
                else
                    formItems.Add("mid", "advantech2");
            }
       
            formItems.Add("nointerest", "no");
            formItems.Add("quotabase", "Select:Full Payment:2months:3months:6months");
            formItems.Add("price",((int)paymentInfo.Amount).ToString());
            formItems.Add("currency", "WON");
            formItems.Add("buyername", order.UserID);
            formItems.Add("goodname", order.OrderNo);
            //formItems.Add("acceptmethod", order);
            //formItems.Add("gopaymethod", order);
           // formItems.Add("ini_encfield", order);
           // formItems.Add("ini_certid", order);
           // formItems.Add("INIregno", order);
            formItems.Add("oid", order.OrderNo);
            formItems.Add("buyeremail", order.UserID);
            formItems.Add("ini_menuarea_url", "");
            formItems.Add("ini_logoimage_url", "");
            //formItems.Add("ini_bgskin_url", order);
           // formItems.Add("mall_noint", order);
            //formItems.Add("ini_onket_flag", order);
            //formItems.Add("ini_pin_flag", order);
            formItems.Add("buyertel", order.cartX.ShipToContact.TelNo);
           // formItems.Add("ini_escrow_dlv", order);
        //    formItems.Add("ansim_cardnumber", order);
            //formItems.Add("ansim_expy", order);
           // formItems.Add("ansim_expm", order);
        //    formItems.Add("ansim_quota", order);
           // formItems.Add("ini_onlycardcode", order);
            //formItems.Add("ESCROW_LOGO_URL", order);
            //formItems.Add("KVP_OACERT_INF", order);
            //formItems.Add("cardcode", order);
            //formItems.Add("cardquota", order);
            //formItems.Add("quotainterest", order);
            //formItems.Add("buyeremail", order);
            //formItems.Add("ispcardcode", order);
            //formItems.Add("kvp_card_prefix", order);

     return formItems;
        }
        public override string  getIndirectPaymentOrderResponseNO(System.Collections.Specialized.NameValueCollection response)
        {
            if (response != null && response["oid"] != null)
                return (string)response["oid"];
            else
                return string.Empty;
        }
        private IINIpay getIINIpay(string paymenttype,string type,bool simulation = false)
        {
            IINIpay INIpay = new IINIpay("50");
            INIpay.Initialize(type);
            string path = Path.GetFullPath(configPath + "/AKR");
            INIpay.SetPath(path);			// INIpay installation path, need to chage to web.config.
            if (simulation)
            {
                INIpay.SetField("mid", "INIpayTest");
                INIpay.SetField("admin", "1111");
                INIpay.SetField("debug", "true");
            }
            else
            {
                if (paymenttype == "INIpayvbank")
                {
                    INIpay.SetField("mid", "IESadvante");
                    INIpay.SetField("admin", "akr98705*");
                }
                else
                {
                    INIpay.SetField("mid", "advantech2");
                    INIpay.SetField("admin", "1111");
                }
                INIpay.SetField("debug", "true");	
            }
            return INIpay;
        }
        public override Payment processIndirectPaymentResponse(System.Collections.Specialized.NameValueCollection response, Order order, Boolean simulation = false)
        {
            Payment rlt = order.getLastOpenPayment();
            if (rlt == null)
            {
                rlt = new Payment();
                rlt.Amount = order.totalAmountX;
            }
            HashData();
            IINIpay INIpay = getIINIpay(order.PaymentType,"securepay", rlt.isSimulation);

            string strpaymethod = response["paymethod"];

            if (strpaymethod!=null && hstCode_PayMethod.ContainsKey(strpaymethod))
            {
                strpaymethod = hstCode_PayMethod[strpaymethod].ToString();
            }

            INIpay.SetPath(Path.GetFullPath(configPath + "/AKR"));
            INIpay.SetField("pgid", "INInet" + strpaymethod);				// PG ID (고정)
            INIpay.SetField("spgip", "203.238.3.10");						// 예비 PG IP (고정)
            INIpay.SetField("uid", response["uid"]);					// INIpay User ID(이니시스 내부변수 수정불가, 상점사용 user id 를 사용하지 마세요)
           						    // 상점아이디
            //INIpay.SetField("rn", rlt.CCPNREF);           // 결제 요청  페이지에서  세션에 저장 (또는 DB에 저장)한 것을 체크 하기 위해  결제 처리 페이지 에서 세팅)
            INIpay.SetField("price", ((int)rlt.Amount).ToString());

            INIpay.SetField("goodname", response["goodname"]);		// 상품명
            INIpay.SetField("currency", "WON");								// 화폐단위
            INIpay.SetField("buyername", response["buyername"]);		// 이용자 이름
            INIpay.SetField("buyertel", response["buyertel"]);		// 이용자 이동전화
            INIpay.SetField("buyeremail", response["buyeremail"]);	// 이용자 이메일
            INIpay.SetField("paymethod", response["paymethod"]);		// 지불방법
            INIpay.SetField("encrypted", response["encrypted"]);		// 암호문
            INIpay.SetField("sessionkey", response["sessionkey"]);	// 암호문
            INIpay.SetField("url", "http://www.buy.adavntech.co.kr");					// 홈페이지 주소
            INIpay.SetField("debug", "true");								// 로그모드(실서비스시에는 "false"로)
            INIpay.SetField("merchantreserved1", "예비1");	                // 예비필드1
            INIpay.SetField("merchantreserved2", "예비2");	                // 예비필드2  
            INIpay.SetField("merchantreserved3", "예비3");	                // 예비필드3

            INIpay.SetField("recvname", response["recvname"]);		 //수취인명
            INIpay.SetField("recvtel", response["recvtel"]);			 //수취인 전화번호
            INIpay.SetField("recvaddr", response["recvaddr"]);		 //수취인 주소
            INIpay.SetField("recvpostnum", response["recvpostnum"]);	 //수취인 우편번호
            INIpay.SetField("recvmsg", response["recvmsg"]);			 //수취인 전달 메세지

            INIpay.StartAction();


            rlt.CCResultCode = INIpay.GetResult("resultcode");		//결과코드 성공이면 '00' 실패 '01'
            rlt.CCPREFPSMSG = INIpay.GetResult("resultmsg");		//결과메세지 
            rlt.CCPNREF = INIpay.GetResult("rn");				// 암호화 결과값
            rlt.CCRESPMSG = INIpay.GetResult("return_enc");		// 암호화 결과값
            rlt.Comment1 = INIpay.GetResult("ini_certid");		// 암호화 결과값

            rlt.responseValues = convertToDictionary(response);

            INIpay.Destory();
            INIpay = null;


            if (rlt.CCResultCode.Equals("00"))
            {
                rlt.statusX = Payment.PaymentStatus.Approved;
            }
            else
                rlt.statusX = Payment.PaymentStatus.Declined;
            return rlt;
        }
        private Hashtable hstCode_PayMethod = new Hashtable();
        private void HashData()
        {
            hstCode_PayMethod["Card"] = "CARD";
            hstCode_PayMethod["VCard"] = "ISP_";
            hstCode_PayMethod["Account"] = "ACCT";
            hstCode_PayMethod["DirectBank"] = "DBNK";
            hstCode_PayMethod["INIcard"] = "INIC";
            hstCode_PayMethod["OCBPoint"] = "OCBP";
            hstCode_PayMethod["MDX"] = "MDX_";
            hstCode_PayMethod["HPP"] = "HPP_";
            hstCode_PayMethod["Nemo"] = "NEMO";
            hstCode_PayMethod["ArsBill"] = "ARSB";
            hstCode_PayMethod["PhoneBill"] = "PHNB";
            hstCode_PayMethod["Ars1588Bill"] = "1588";
            hstCode_PayMethod["VBank"] = "VBNK";
            hstCode_PayMethod["Culture"] = "CULT";
            hstCode_PayMethod["CMS"] = "CMS_";
            hstCode_PayMethod["AUTH"] = "AUTH";
        }

 
    
        private Dictionary<String, String> convertToDictionary(System.Collections.Specialized.NameValueCollection items)
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
