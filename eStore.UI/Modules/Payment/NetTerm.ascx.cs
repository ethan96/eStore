using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.Payment
{
    public partial class NetTerm : Presentation.eStoreBaseControls.eStoreBaseUserControl, Presentation.Payment.IPaymentMethodModule
    {
        public bool hasTermFile
        {
            get { return !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.NetTermID); }
        }
        public bool hasPoFile
        {
            get { return !string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PurchaseOrderFile); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current.User != null && hasTermFile)
                {
                    string downloadurlterm = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.NetTerm
                        , Presentation.eStoreContext.Current.User.actingUser.NetTermID);
                    if (!string.IsNullOrEmpty(downloadurlterm))
                    {
                        this.ldownloadterm.NavigateUrl = downloadurlterm;
                        this.pdownloadterm.Visible = true;
                    }
                }

                if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.Order.PurchaseNO))
                {
                    this.txtPONo.PONoText = Presentation.eStoreContext.Current.Order.PurchaseNO;
                    if (hasPoFile)
                    {
                        string downloadurlpo = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.PurchaseOrder
                              , Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate);
                        if (!string.IsNullOrEmpty(downloadurlpo))
                        {
                            this.ldownloadpo.NavigateUrl = downloadurlpo;
                            pdownloadpo.Visible = true;
                        }
                    }
                }
            }
        }

        public POCOS.Payment GetPaymentInfo()
        {
            POCOS.Payment paymentInfo = new POCOS.Payment();
            paymentInfo.PurchaseNO = this.txtPONo.PONoText;
            return paymentInfo;
        }

        public bool ValidateForm()
        {
            return true;
        }

        public string ClientValidataionFunction()
        {
            return "return validatePO();";
        }

        protected void tbnuploadterm_Click(object sender, EventArgs e)
        {
            string fileName = Presentation.eStoreFileManager.save(Presentation.eStoreFileManager.eStoreFileType.NetTerm, this.fuploadterm);
            if (string.IsNullOrEmpty(fileName))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Upload Net Term File Failed");
                ClientScriptManager csm = Page.ClientScript;
                csm.RegisterHiddenField("NetTermArrangement", "false");
                pdownloadterm.Visible = false;
            }
            else
            {
                Presentation.eStoreContext.Current.User.actingUser.NetTermID = fileName;
                Presentation.eStoreContext.Current.User.save();
                 
                string downloadURL = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.NetTerm, fileName);
                if (string.IsNullOrEmpty(downloadURL))
                {
                    pdownloadterm.Visible = false;
                }
                else
                {
                    ldownloadterm.NavigateUrl = downloadURL;
                    pdownloadterm.Visible = true;
                }
                ClientScriptManager csm = Page.ClientScript;
                csm.RegisterHiddenField("NetTermArrangement", "true");
                Presentation.eStoreContext.Current.AddStoreErrorCode("Upload Certificate Successful");
                this.BindScript("Script", "sPOcontent", "$(function() { netTermObj.selectPOcontent(false); });");
            }
        }

        protected void btnuploadpo_Click(object sender, EventArgs e)
        {
            string fileName = Presentation.eStoreFileManager.save(Presentation.eStoreFileManager.eStoreFileType.PurchaseOrder, this.fuploadpo);
            if (string.IsNullOrEmpty(fileName))
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Upload PO File Failed");
                pdownloadpo.Visible = false; ;
            }
            else
            {
                Presentation.eStoreContext.Current.Order.PurchaseOrderFile = fileName;
                string downloadURL = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.PurchaseOrder, fileName);
                if (string.IsNullOrEmpty(downloadURL))
                {
                    pdownloadpo.Visible = false;
                }
                else
                {
                    ldownloadpo.NavigateUrl = downloadURL;
                    pdownloadpo.Visible = true;
                }
                Presentation.eStoreContext.Current.AddStoreErrorCode("Upload Certificate Successful");
                this.BindScript("Script", "sPOcontent", "$(function() { netTermObj.selectTermcontent(false); });");
            } 
        }


        public bool PreLoad()
        {
            ClientScriptManager csm = Page.ClientScript;
            if (hasTermFile)
                csm.RegisterHiddenField("NetTermArrangement", "true");
            else
                csm.RegisterHiddenField("NetTermArrangement", "false");
            if (ViewState["IsPostBack"] == null)
            {
                csm.RegisterHiddenField("IsPostBack", "false");
                ViewState["IsPostBack"] = true;
            }
            else
            {
                csm.RegisterHiddenField("IsPostBack", "true");
            }
            return true;
        }
    }
}