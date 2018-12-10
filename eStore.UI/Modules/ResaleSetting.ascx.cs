using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Modules
{
    public partial class ResaleSetting : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {   
        public  EventHandler  btnRecalculateTax_Click;
        private string resellerType = "Order";
        public string ResellerType
        {
            get { return resellerType; }
            set { resellerType = value; }
        }
        public string ResellerID
        {
            get {
                if (!string.IsNullOrEmpty(this.resellerid.Text) && this.resellerid.Text != this.resellerid.ToolTip)
                    return this.resellerid.Text;
                else
                    return string.Empty;
            }
            set { this.resellerid.Text = value; }
        }
        public bool EnableReseller {
            get {
                return chkResale.Checked;
            }
            set {

                chkResale.Checked = value;
            }
        }
        public void  DisplayOnly()
        {
            this.Visible = true;
            this.btnRecalculateTax.Visible = false;
            this.filecertificate.Visible = false;
            this.resellerid.Enabled = false;
            chkResale.Enabled = false;
            this.resellerCert.Attributes.Remove("class");

        }
        public void DisplayAndEnableModify()
        {
            this.Visible = true;
            this.btnRecalculateTax.Visible = true;
            this.filecertificate.Visible = true;
            this.resellerid.Enabled = true;
            chkResale.Enabled = true;
            this.resellerCert.Attributes.Remove("class");

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Order不存在ResellerID 使用User的ResellerID 
                if (string.IsNullOrEmpty(ResellerID) && Presentation.eStoreContext.Current.User != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.ResellerID))
                {
                    this.ResellerID = Presentation.eStoreContext.Current.User.actingUser.ResellerID;
                    if (!string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate))
                    {
                        string downloadurl = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.Reseller
                            , Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate);
                        if (!string.IsNullOrEmpty(downloadurl))
                        {
                            this.ldownload.NavigateUrl = downloadurl;
                            pdownloadreseller.Visible = true;
                        }
                    }
                }
            }
            bindFonts();
            if (EnableReseller)
                resellerCert.Visible = true;
            if (btnRecalculateTax_Click != null)
            {
                this.btnRecalculateTax.Click += btnRecalculateTax_Click;
            }
        }

        public bool setResellerCertificate() 
        {
            bool rlt = false ;
            if (this.filecertificate.HasFile)
            {
                string fileExt = System.IO.Path.GetExtension(this.filecertificate.FileName).ToLower();
                if (!Presentation.eStoreFileManager.isDocTypeSupported(Presentation.eStoreFileManager.eStoreFileExtDocType.ImageAndDocument, fileExt))
                {
                    this.resellerCert.Attributes.Remove("class");
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Not a supported file type!");                    
                    return rlt;
                }
                if (!Presentation.eStoreFileManager.isContentLengthSupported(filecertificate))
                {
                    this.resellerCert.Attributes.Remove("class");
                    Presentation.eStoreContext.Current.AddStoreErrorCode("File size more than 4 M!");
                    return rlt;
                }

                string fileName = Presentation.eStoreFileManager.save(Presentation.eStoreFileManager.eStoreFileType.Reseller, this.filecertificate);
                if (string.IsNullOrEmpty(fileName))
                {
                    this.resellerCert.Attributes.Remove("class");
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Upload Certificate Failed");
                    pdownloadreseller.Visible = false;
                }
                else
                {
                    ldownload.NavigateUrl = Presentation.eStoreFileManager.downloadURL(Presentation.eStoreFileManager.eStoreFileType.Reseller, fileName);
                    pdownloadreseller.Visible = true;
                    //set order ResellerCertificate
                    if (ResellerType == "Order")
                        Presentation.eStoreContext.Current.Order.ResellerCertificate = fileName;
                    else
                    {
                        Presentation.eStoreContext.Current.Quotation.ResellerCertificate = fileName;
                    }
                    //set user ResellerCertificate
                    Presentation.eStoreContext.Current.User.actingUser.ResellerCertificate = fileName;
                    Presentation.eStoreContext.Current.User.actingUser.save();
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Upload Certificate Successful");
                }
            }
            if (!string.IsNullOrEmpty(this.ResellerID))
            {
                try
                {
                    if (String.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.ResellerID) ||
                        !(Presentation.eStoreContext.Current.User.actingUser.ResellerID.Equals(this.ResellerID)))
                    {
                        Presentation.eStoreContext.Current.User.actingUser.ResellerID = this.ResellerID;
                        Presentation.eStoreContext.Current.User.actingUser.save();
                        //Presentation.eStoreContext.Current.User.updateSSOContact(POCOS.User.SSO_Update_Type.ResellerID);
                        Presentation.eStoreContext.Current.Store.syncStoreUserToSSOUserProfile(Presentation.eStoreContext.Current.User.actingUser, eStore.BusinessModules.Store.SSO_Update_Type.ResellerID);
                    }
                    rlt = true;
                }
                catch (Exception ex)
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Set Reseller Certificate Error, please try again");
                    eStore.Utilities.eStoreLoger.Error("setResellerCertificate error. ResellerID:" + this.ResellerID, "", "", "", ex);
                }

            }
            else
            {
                this.resellerCert.Attributes.Remove("class");
                Presentation.eStoreContext.Current.AddStoreErrorCode("Please Enter ResellerId!");
            }
            return rlt;
        }

        protected void bindFonts()
        {
            chkResale.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Resale);
        }
      
    }
}