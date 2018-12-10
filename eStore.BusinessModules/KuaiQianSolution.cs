using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;
using eStore.POCOS;
using eStore.Utilities;

namespace eStore.BusinessModules
{
    class KuaiQianSolution : PaymentSolution
    {
        private string StoreId;
        private bool _testing = false;
        private enum Type { eStore, IotMart, UShop }
        private Type _siteType = Type.eStore;
        private Type SiteType
        {
            get { return _siteType; }
            set 
            {
                switch (value)
                {
                    case Type.UShop:
                        config.merchantAcctId = "1002448591001";
                        config.signerPfxPath = configPath + @"\ACN\ACNcert\99bill-rsa-ushop.pfx";
                        config.signerPfxPassword = "andyushop8177";
                        config.signerGfxPath = configPath + @"\ACN\ACNcert\99bill.cert.rsa.20340630-ushop.cer";
                        config.signerfxGPassword = "andyushop8177";
                        break;
                    case Type.eStore:
                    case Type.IotMart:
                    default:
                        config.merchantAcctId = "1002290702701";
                        config.signerPfxPath = configPath + @"\ACN\ACNcert\99bill-rsa.pfx";
                        config.signerPfxPassword = "Andy@8177";
                        config.signerGfxPath = configPath + @"\ACN\ACNcert\99bill.cert.rsa.20140728.cer";
                        config.signerfxGPassword = "Andy@8177";
                        break;
                }
                _siteType = value;
            }
        }
        

        private KQConfiguration _config;
        protected KQConfiguration config
        {
            get 
            {
                if (_config == null)
                {
                    _config = new KQConfiguration() 
                    {
                        actionURL = "https://www.99bill.com/gateway/recvMerchantInfoAction.htm",
                        //人民币网关账号，该账号为11位人民币网关商户编号+01,该参数必填。
                        merchantAcctId = "1002290702701", //ushop:1002448591001 , estore: 1002290702701
                        //编码方式，1代表 UTF-8, 2 代表 GBK, 3代表 GB2312 默认为1,该参数必填。
                        inputCharset = "1",
                        //接收支付结果的页面地址，该参数一般置为空即可。
                        pageUrl = "",
                        //服务器接收支付结果的后台地址，该参数务必填写，不能为空。
                        bgUrl = "http://buy.advantech.com.cn/completeKuaiQianPayment.aspx",
                        //网关版本，固定值：v2.0,该参数必填。
                        version = "v2.0",
                        //语言种类，1代表中文显示，2代表英文显示。默认为1,该参数必填。
                        language = "1",
                        //签名类型,该值为4，代表PKI加密方式,该参数必填。
                        signType = "4",
                        //支付人姓名,可以为空。
                        payerName = "",
                        //支付人联系类型，1 代表电子邮件方式；2 代表手机联系方式。可以为空。
                        payerContactType = "1",
                        //支付人联系方式，与payerContactType设置对应，payerContactType为1，则填写邮箱地址；payerContactType为2，则填写手机号码。可以为空。
                        payerContact = "",
                        //商户订单号，以下采用时间来定义订单号，商户可以根据自己订单号的定义规则来定义该值，不能为空。
                        orderId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        //订单金额，金额以“分”为单位，商户测试以1分测试即可，切勿以大金额测试。该参数必填。
                        orderAmount = "1",
                        //订单提交时间，格式：yyyyMMddHHmmss，如：20071117020101，不能为空。
                        orderTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        //商品名称，可以为空。
                        productName = "",
                        //商品数量，可以为空。
                        productNum = "",
                        //商品代码，可以为空。
                        productId = "",
                        //商品描述，可以为空。
                        productDesc = "",
                        //扩展字段1，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。
                        ext1 = "",
                        //扩展自段2，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。
                        ext2 = "",
                        //支付方式，一般为00，代表所有的支付方式。如果是银行直连商户，该值为10，必填。
                        payType = "10",
                        //银行代码，如果payType为00，该值可以为空；如果payType为10，该值必须填写，具体请参考银行列表。
                        bankId = "CCB",
                        //同一订单禁止重复提交标志，实物购物车填1，虚拟产品用0。1代表只能提交一次，0代表在支付不成功情况下可以再提交。可为空。
                        redoFlag = "",
                        //快钱合作伙伴的帐户号，即商户编号，可为空。
                        pid = "",
                        // signMsg 签名字符串 不可空，生成加密签名串
                        signMsg = "",
                        signerPfxPath = configPath + @"\ACN\ACNcert\99bill-rsa.pfx", //estore: \ACN\ACNcert\99bill-rsa.pfx  ushop:\ACN\ACNcert\99bill-rsa-ushop.pfx
                        signerPfxPassword = "Andy@8177", //estore:Andy@8177  ushop:andyushop8177
                        signerGfxPath = configPath + @"\ACN\ACNcert\99bill.cert.rsa.20140728.cer", // estore:\ACN\ACNcert\99bill.cert.rsa.20140728.cer  ushop:\ACN\ACNcert\99bill.cert.rsa.20340630-ushop.cer
                        signerfxGPassword = "Andy@8177" //estore:Andy@8177  ushop:andyushop8177
                    };
                    //if (_testing)
                    //{
                    //    _config.actionURL = "https://sandbox.99bill.com/gateway/recvMerchantInfoAction.htm";
                    //    _config.bgUrl = "http://219.233.173.50:8804/dongjian/rmb/c/receive.aspx";
                    //    _config.merchantAcctId = "1001213884201";
                    //    _config.signerPfxPath = configPath + @"\ACN\ACNcert\99bill-rsa-test.pfx";
                    //    _config.signerPfxPassword = "123456";
                    //    _config.signerGfxPath = configPath + @"\ACN\ACNcert\99bill.cert.rsa.20140728-test.cer";
                    //    _config.signerfxGPassword = "";
                    //}
                }
                return _config; 
            }
            set { _config = value; }
        }





        public override IDictionary<string, string> getIndirectPaymentRequestForm(POCOS.Order order, POCOS.Payment paymentInfo, bool simulation = false)
        {
            Dictionary<String, String> formItems = new Dictionary<string, string>();
            try
            {
                StoreId = order.StoreID;
                _testing = simulation;
                putOrder2Config(order, paymentInfo);

                formItems.Add("actionURL", config.actionURL);
                formItems.Add("inputCharset", config.inputCharset);
                formItems.Add("pageUrl", config.pageUrl);
                formItems.Add("bgUrl", config.bgUrl);
                formItems.Add("version", config.version);
                formItems.Add("language", config.language);
                formItems.Add("signType", config.signType);
                formItems.Add("signMsg", config.signMsg);
                formItems.Add("merchantAcctId", config.merchantAcctId);
                formItems.Add("payerName", config.payerName);
                formItems.Add("payerContactType", config.payerContactType);
                formItems.Add("payerContact", config.payerContact);
                formItems.Add("orderId", config.orderId);
                formItems.Add("orderAmount", config.orderAmount);
                formItems.Add("orderTime", config.orderTime);
                formItems.Add("productName", config.productName);
                formItems.Add("productNum", config.productNum);
                formItems.Add("productId", config.productId);
                formItems.Add("productDesc", config.productDesc);
                formItems.Add("ext1", config.ext1);
                formItems.Add("ext2", config.ext2);
                formItems.Add("payType", config.payType);
                formItems.Add("bankId", config.bankId);
                formItems.Add("redoFlag", config.redoFlag);
                formItems.Add("pid", config.pid);
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at KuaiQianRedirect getIndirectPaymentRequestForm", "", "", "", ex);
            }
            
            return formItems;
        }

        public override string getIndirectPaymentOrderResponseNO(NameValueCollection response)
        {
            if (response != null && response["orderId"] != null)
                return (string)response["orderId"];
            else
                return string.Empty;
        }

        public override Payment processIndirectPaymentResponse(NameValueCollection responseValues, POCOS.Order order, Boolean simulation = false)
        {
            
            this.StoreId = order.StoreID;
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
            }
            try
            {
                _testing = payment.isSimulation;
                Dictionary<String, String> response = convertToDictionary(responseValues);
                payment.responseValues = response;
                //人民币网关账号，该账号为11位人民币网关商户编号+01,该值与提交时相同。
                string merchantAcctId = response["merchantAcctId"].ToString();
                //网关版本，固定值：v2.0,该值与提交时相同。
                string version = response["version"].ToString();
                //语言种类，1代表中文显示，2代表英文显示。默认为1,该值与提交时相同。
                string language = response["language"].ToString();
                //签名类型,该值为4，代表PKI加密方式,该值与提交时相同。
                string signType = response["signType"].ToString();
                //支付方式，一般为00，代表所有的支付方式。如果是银行直连商户，该值为10,该值与提交时相同。
                string payType = response["payType"].ToString();
                //银行代码，如果payType为00，该值为空；如果payType为10,该值与提交时相同。
                string bankId = response["bankId"].ToString();
                //商户订单号，,该值与提交时相同。
                string orderId = response["orderId"].ToString();
                //订单提交时间，格式：yyyyMMddHHmmss，如：20071117020101,该值与提交时相同。
                string orderTime = response["orderTime"].ToString();
                //订单金额，金额以“分”为单位，商户测试以1分测试即可，切勿以大金额测试,该值与支付时相同。
                string orderAmount = response["orderAmount"].ToString();
                // 快钱交易号，商户每一笔交易都会在快钱生成一个交易号。
                string dealId = response["dealId"].ToString();
                //银行交易号 ，快钱交易在银行支付时对应的交易号，如果不是通过银行卡支付，则为空
                string bankDealId = response["bankDealId"].ToString();
                //快钱交易时间，快钱对交易进行处理的时间,格式：yyyyMMddHHmmss，如：20071117020101
                string dealTime = response["dealTime"].ToString();
                //商户实际支付金额 以分为单位。比方10元，提交时金额应为1000。该金额代表商户快钱账户最终收到的金额。
                string payAmount = response["payAmount"].ToString();
                //费用，快钱收取商户的手续费，单位为分。
                string fee = response["fee"].ToString();
                //扩展字段1，该值与提交时相同。
                string ext1 = response["ext1"].ToString();
                //扩展字段2，该值与提交时相同。
                string ext2 = response["ext2"].ToString();
                //处理结果， 10支付成功，11 支付失败，00订单申请成功，01 订单申请失败
                string payResult = response["payResult"].ToString();
                //错误代码 ，请参照《人民币网关接口文档》最后部分的详细解释。
                string errCode = response["errCode"].ToString();
                //签名字符串 
                string signMsg = response["signMsg"].ToString();
                string signMsgVal = "";
                signMsgVal = appendParam(signMsgVal, "merchantAcctId", merchantAcctId);
                signMsgVal = appendParam(signMsgVal, "version", version);
                signMsgVal = appendParam(signMsgVal, "language", language);
                signMsgVal = appendParam(signMsgVal, "signType", signType);
                signMsgVal = appendParam(signMsgVal, "payType", payType);
                signMsgVal = appendParam(signMsgVal, "bankId", bankId);
                signMsgVal = appendParam(signMsgVal, "orderId", orderId);
                signMsgVal = appendParam(signMsgVal, "orderTime", orderTime);
                signMsgVal = appendParam(signMsgVal, "orderAmount", orderAmount);
                signMsgVal = appendParam(signMsgVal, "dealId", dealId);
                signMsgVal = appendParam(signMsgVal, "bankDealId", bankDealId);
                signMsgVal = appendParam(signMsgVal, "dealTime", dealTime);
                signMsgVal = appendParam(signMsgVal, "payAmount", payAmount);
                signMsgVal = appendParam(signMsgVal, "fee", fee);
                signMsgVal = appendParam(signMsgVal, "ext1", ext1);
                signMsgVal = appendParam(signMsgVal, "ext2", ext2);
                signMsgVal = appendParam(signMsgVal, "payResult", payResult);
                signMsgVal = appendParam(signMsgVal, "errCode", errCode);

                ///UTF-8编码  GB2312编码  用户可以根据自己网站的编码格式来选择加密的编码方式
                ///byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(signMsgVal);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
                byte[] SignatureByte = Convert.FromBase64String(signMsg);
                X509Certificate2 cert = new X509Certificate2(config.signerGfxPath, config.signerfxGPassword);
                RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PublicKey.Key;
                rsapri.ImportCspBlob(rsapri.ExportCspBlob(false));
                RSAPKCS1SignatureDeformatter f = new RSAPKCS1SignatureDeformatter(rsapri);
                byte[] result;
                f.SetHashAlgorithm("SHA1");
                SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
                result = sha.ComputeHash(bytes);

                if (f.VerifySignature(result, SignatureByte))
                {
                    //逻辑处理  写入数据库
                    if (payResult == "10")
                    {
                        payment.statusX = Payment.PaymentStatus.Approved;
                        //其他信息参数
                    }
                    else
                    {
                        payment.statusX = Payment.PaymentStatus.AuthenticationFailed;
                    }
                }
                else
                {
                    payment.statusX = Payment.PaymentStatus.Declined;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Exception at KuaiQianRedirect payment", "", "", "", ex);
                payment.statusX = Payment.PaymentStatus.Declined;
            }

            return payment;
        }
        

        /// <summary>
        /// 订单基本信息填入到快钱config中
        /// </summary>
        /// <param name="order"></param>
        private void putOrder2Config(POCOS.Order order, POCOS.Payment paymentInfo)
        {
            Store store = new Store(order.storeX);
            if (order.cartX.minisiteX != null && order.cartX.minisiteX.MiniSiteType == MiniSite.SiteType.UShop)
                this.SiteType = Type.UShop;
            else
                this.SiteType = Type.eStore;
            
            config.bgUrl = esUtilities.CommonHelper.GetStoreLocation(false) + "completeKuaiQianPayment.aspx";
            config.payerName = store.getCultureFullName(order.userX.FirstName, order.userX.LastName);
            config.payerContact = order.userX.UserID;
            config.orderId = order.OrderNo;
            config.productName = string.Join(";", order.cartX.cartItemsX.Select(c => c.SProductID).ToList());
            config.productNum = order.cartX.cartItemsX.Count.ToString();
            config.bankId = paymentInfo.CCAuthCode;
            if (_testing)
                config.orderAmount = "1";
            else
                config.orderAmount = (order.totalAmountX * 100).ToString("0");
            config.ext1 = order.userX.UserID;
            //config.productId = string.Join(";", order.cartX.cartItemsX.Select(c => c.SProductID).ToList());
            SignAndEncrypt();
        }

        /// <summary>
        /// 生产快钱config signMsg信息
        /// </summary>
        private void SignAndEncrypt()
        {
            string signMsgVal = "";
            signMsgVal = appendParam(signMsgVal, "inputCharset", config.inputCharset);
            signMsgVal = appendParam(signMsgVal, "pageUrl", config.pageUrl);
            signMsgVal = appendParam(signMsgVal, "bgUrl", config.bgUrl);
            signMsgVal = appendParam(signMsgVal, "version", config.version);
            signMsgVal = appendParam(signMsgVal, "language", config.language);
            signMsgVal = appendParam(signMsgVal, "signType", config.signType);
            signMsgVal = appendParam(signMsgVal, "merchantAcctId", config.merchantAcctId);
            signMsgVal = appendParam(signMsgVal, "payerName", config.payerName);
            signMsgVal = appendParam(signMsgVal, "payerContactType", config.payerContactType);
            signMsgVal = appendParam(signMsgVal, "payerContact", config.payerContact);
            signMsgVal = appendParam(signMsgVal, "orderId", config.orderId);
            signMsgVal = appendParam(signMsgVal, "orderAmount", config.orderAmount);
            signMsgVal = appendParam(signMsgVal, "orderTime", config.orderTime);
            signMsgVal = appendParam(signMsgVal, "productName", config.productName);
            signMsgVal = appendParam(signMsgVal, "productNum", config.productNum);
            signMsgVal = appendParam(signMsgVal, "productId", config.productId);
            signMsgVal = appendParam(signMsgVal, "productDesc", config.productDesc);
            signMsgVal = appendParam(signMsgVal, "ext1", config.ext1);
            signMsgVal = appendParam(signMsgVal, "ext2", config.ext2);
            signMsgVal = appendParam(signMsgVal, "payType", config.payType);
            signMsgVal = appendParam(signMsgVal, "bankId", config.bankId);
            signMsgVal = appendParam(signMsgVal, "redoFlag", config.redoFlag);
            signMsgVal = appendParam(signMsgVal, "pid", config.pid);

            ///PKI加密
            ///编码方式UTF-8 GB2312  用户可以根据自己系统的编码选择对应的加密方式
            ///byte[] OriginalByte=Encoding.GetEncoding("GB2312").GetBytes(OriginalString);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(signMsgVal);
            X509Certificate2 cert = new X509Certificate2(config.signerPfxPath, config.signerPfxPassword, X509KeyStorageFlags.MachineKeySet);
            RSACryptoServiceProvider rsapri = (RSACryptoServiceProvider)cert.PrivateKey;
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsapri);
            byte[] result;
            f.SetHashAlgorithm("SHA1");
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            result = sha.ComputeHash(bytes);
            config.signMsg = System.Convert.ToBase64String(f.CreateSignature(result)).ToString();
        }

        //功能函数。将变量值不为空的参数组成字符串
        private string appendParam(string returnStr, string paramId, string paramValue)
        {
            if (returnStr != "")
            {
                if (paramValue != "")
                    returnStr += "&" + paramId + "=" + paramValue;
            }
            else
            {
                if (paramValue != "")
                    returnStr = paramId + "=" + paramValue;
            }
            return returnStr;
        }
        
    }

    public class KQConfiguration
    {
        private string _actionURL;
        /// <summary>
        /// test: https://sandbox.99bill.com/gateway/recvMerchantInfoAction.htm
        /// </summary>
        public string actionURL
        {
            get { return _actionURL; }
            set { _actionURL = value; }
        }
        

        private string _merchantAcctId;
        //人民币网关账号，该账号为11位人民币网关商户编号+01,该参数必填。
        public string merchantAcctId
        {
            get { return _merchantAcctId; }
            set { _merchantAcctId = value; }
        }

        private string _inputCharset;
        //编码方式，1代表 UTF-8; 2 代表 GBK; 3代表 GB2312 默认为1,该参数必填。
        public string inputCharset
        {
            get { return _inputCharset; }
            set { _inputCharset = value; }
        }

        private string _pageUrl;
        //接收支付结果的页面地址，该参数一般置为空即可。
        public string pageUrl
        {
            get { return _pageUrl; }
            set { _pageUrl = value; }
        }

        private string _bgUrl;
        //服务器接收支付结果的后台地址，该参数务必填写，不能为空。
        public string bgUrl
        {
            get { return _bgUrl; }
            set { _bgUrl = value; }
        }

        private string  _version;
        //网关版本，固定值：v2.0,该参数必填。
        public string  version
        {
            get { return _version; }
            set { _version = value; }
        }

        private string _language;
        //语言种类，1代表中文显示，2代表英文显示。默认为1,该参数必填。
        public string language
        {
            get { return _language; }
            set { _language = value; }
        }

        private string _signType;
        //签名类型,该值为4，代表PKI加密方式,该参数必填。
        public string signType
        {
            get { return _signType; }
            set { _signType = value; }
        }

        private string _payerName;
        //支付人姓名,可以为空。
        public string payerName
        {
            get { return _payerName; }
            set { _payerName = value; }
        }

        private string _payerContactType;
        //支付人联系类型，1 代表电子邮件方式；2 代表手机联系方式。可以为空。
        public string payerContactType
        {
            get { return _payerContactType; }
            set { _payerContactType = value; }
        }

        private string _payerContact;
        //支付人联系方式，与payerContactType设置对应，payerContactType为1，则填写邮箱地址；payerContactType为2，则填写手机号码。可以为空。
        public string payerContact
        {
            get { return _payerContact; }
            set { _payerContact = value; }
        }

        private string _orderId;
        //商户订单号，以下采用时间来定义订单号，商户可以根据自己订单号的定义规则来定义该值，不能为空。
        public string orderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        private string _orderAmount;
        //订单金额，金额以“分”为单位，商户测试以1分测试即可，切勿以大金额测试。该参数必填。
        public string orderAmount
        {
            get { return _orderAmount; }
            set { _orderAmount = value; }
        }

        private string _orderTime;
        //订单提交时间，格式：yyyyMMddHHmmss，如：20071117020101，不能为空。
        public string orderTime
        {
            get { return _orderTime; }
            set { _orderTime = value; }
        }

        private string _productName;
        //商品名称，可以为空。
        public string productName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        private string _productNum;
        //商品数量，可以为空。
        public string productNum
        {
            get { return _productNum; }
            set { _productNum = value; }
        }

        private string _productId;
        //商品代码，可以为空。
        public string productId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        private string _productDesc;
        //商品描述，可以为空。
        public string productDesc
        {
            get { return _productDesc; }
            set { _productDesc = value; }
        }

        private string _ext1;
        //扩展字段1，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。
        public string ext1
        {
            get { return _ext1; }
            set { _ext1 = value; }
        }

        private string _ext2;
        //扩展自段2，商户可以传递自己需要的参数，支付完快钱会原值返回，可以为空。
        public string ext2
        {
            get { return _ext2; }
            set { _ext2 = value; }
        }

        private string _payType;
        //支付方式，一般为00，代表所有的支付方式。如果是银行直连商户，该值为10，必填。
        public string payType
        {
            get { return _payType; }
            set { _payType = value; }
        }

        private string _bankId;
        //银行代码，如果payType为00，该值可以为空；如果payType为10，该值必须填写，具体请参考银行列表。
        public string bankId
        {
            get { return _bankId; }
            set { _bankId = value; }
        }

        private string _redoFlag;
        //同一订单禁止重复提交标志，实物购物车填1，虚拟产品用0。1代表只能提交一次，0代表在支付不成功情况下可以再提交。可为空。
        public string redoFlag
        {
            get { return _redoFlag; }
            set { _redoFlag = value; }
        }

        private string _pid;
        //快钱合作伙伴的帐户号，即商户编号，可为空。
        public string pid
        {
            get { return _pid; }
            set { _pid = value; }
        }

        private string _signMsg;
        // signMsg 签名字符串 不可空，生成加密签名串
        public string signMsg
        {
            get { return _signMsg; }
            set { _signMsg = value; }
        }

        private string _signerPfxPath;
        //证书存放路径(编译时)
        public string signerPfxPath
        {
            get { return _signerPfxPath; }
            set { _signerPfxPath = value; }
        }

        private string _signerPfxPassword;
        //证书密码(编译时)
        public string signerPfxPassword
        {
            get { return _signerPfxPassword; }
            set { _signerPfxPassword = value; }
        }

        private string _signerGfxPath;
        //证书存放路径(反编译时)
        public string signerGfxPath
        {
            get { return _signerGfxPath; }
            set { _signerGfxPath = value; }
        }

        private string _signerfxGPassword;
        //证书密码(反编译时)
        public string signerfxGPassword
        {
            get { return _signerfxGPassword; }
            set { _signerfxGPassword = value; }
        }

        
    }
}