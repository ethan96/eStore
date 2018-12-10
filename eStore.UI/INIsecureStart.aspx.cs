using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INIpayNet;

namespace eStore.UI
{
    public partial class INIsecureStart : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "jquery11.2.min", "v4/jquery-1.11.2.min.js");
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current.Order != null && Presentation.eStoreContext.Current.Order.statusX == POCOS.Order.OStatus.Open)
                {
                    POCOS.StorePayment storePayment = Presentation.eStoreContext.Current.Store.getStorePayment(Presentation.eStoreContext.Current.Order.PaymentType);
                    POCOS.Payment paymentInfo = Presentation.eStoreContext.Current.Order.getLastOpenPayment();
                    if (paymentInfo == null)
                        paymentInfo = new POCOS.Payment();
                    paymentInfo.Amount = Presentation.eStoreContext.Current.Order.totalAmountX;
                    paymentInfo = StartChkFake(paymentInfo, Request.Params["ctl00$eStoreMainContent$cbsimulation"]!=null);
                    switch (Presentation.eStoreContext.Current.Order.PaymentType)
                    {
                        case "INIpaydbank":
                            this.gopaymethod.Value = "onlydbank";
                            break;
                        case "INIpayvbank":
                            this.gopaymethod.Value = "onlyvbank";
                            break;
                        case "INIpay":
                        default:
                            this.gopaymethod.Value = "onlycard";
                            break;
                    }
                    if (paymentInfo != null && paymentInfo.CCResultCode.Equals("00"))
                    {
                        IDictionary<String, String> paras = Presentation.eStoreContext.Current.Store.prepareIndirectPayment(Presentation.eStoreContext.Current.Order, storePayment, paymentInfo, Request.Params["ctl00$eStoreMainContent$cbsimulation"] != null);
                        ClientScriptManager csm = Page.ClientScript;
                        paras.Remove("actionURL");

                        foreach (var p in paras)
                        {
                            csm.RegisterHiddenField(p.Key, p.Value);
                        }
                        Presentation.eStoreContext.Current.Order.save();
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.Order.save();
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed", null, true);
                        return;
                    }
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Processing Payment Failed", null, true);
                    return;
                }
            }
        }
        private IINIpay getIINIpay(string type,bool simulation = false)
        {
            IINIpay INIpay = new IINIpay("50");
            INIpay.Initialize(type);
            string path = System.IO.Path.GetFullPath(System.Configuration.ConfigurationManager.AppSettings.Get("Config_Path") + "/AKR");
            INIpay.SetPath(path);			// INIpay installation path, need to chage to web.config.
            if (simulation)
            {
                INIpay.SetField("mid", "INIpayTest");
                INIpay.SetField("admin", "1111");
                INIpay.SetField("debug", "true");
            }
            else
            {
                if (Presentation.eStoreContext.Current.Order.PaymentType == "INIpayvbank")
                    INIpay.SetField("mid", "ESadvantec");
                else
                    INIpay.SetField("mid", "advantech2");

                INIpay.SetField("admin", "1111");
                INIpay.SetField("debug", "true");
            }
            return INIpay;
        }
        private POCOS.Payment StartChkFake(POCOS.Payment paymentInfo, bool simulation = false)
        {
            try
            {
                // 1. New IINIpay object 
                IINIpay INIpay = getIINIpay("chkfake",simulation);

             
                ////**************************************************************************************************
                //// * Admin is the kipaeseuwodeu variable. Should not be edited. Please use only the portion of 1111 to fix.
                //// * Kipaeseuwodeuneun Store Manager page (https: / / iniweb.inicis.com) is not the password. Please take note.
                //// * Kipaeseuwodeuneun consists only of the 4-digit numbers. This value is determined by issuing a keyfile.
                //// * To check the value of kipaeseuwodeu Keyfile issued in shops on the side, please refer to the readme.txt file.
                ////**************************************************************************************************
        
               
                    INIpay.SetField("price",((int)paymentInfo.Amount).ToString());	   // Price
               
                INIpay.SetField("nointerest", "no");		// Configuration setting interest
                INIpay.SetField("quotabase", "Select:Full Payment:2months:3months:6months");      //Installment duration
                INIpay.SetField("currency", "WON");	    // Currency
                //INIpay.SetField("debug", "true");			    // Log Mode("true" is set to log, leaving the details)
                // 5. Check the encryption process for transaction
                INIpay.StartAction();

                //6. Encryption result
                paymentInfo.CCResultCode = INIpay.GetResult("resultcode");		// Result code: '00' means success, '01' means failed.
                paymentInfo.CCPREFPSMSG = INIpay.GetResult("resultmsg");		// Result message
                paymentInfo.CCPNREF = INIpay.GetResult("rn");				// Encryption result
                paymentInfo.TransactionDesc = INIpay.GetResult("return_enc");		// Encription result
               if(simulation)
                   paymentInfo.Comment2 = "Simulation";	
                INIpay.Destory();
                INIpay = null;

         
                return paymentInfo;
            }
            catch (Exception)
            {

                return null;
            }


        }
    }
}