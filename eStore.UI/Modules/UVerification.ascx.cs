using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;

namespace eStore.UI.Modules
{
    public partial class UVerification : eStoreBaseUserControl
    {
        private string _lableCss = "editorpanelplabel";
        public string lableCss
        {
            get { return _lableCss; }
            set { _lableCss = value; }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //注册用户不需要验证
            plVerfication.Visible = eStore.Presentation.eStoreContext.Current.User == null;
        }

        //验证号是否正确 
        public bool VerificationUser()
        {
            bool isValidate = true;
            //如果验证码显示,  需要验证是否正确.
            if (plVerfication.Visible)
            {
                if (eStore.Presentation.eStoreContext.Current.verificationCode.ToUpper().Equals(txtVerificationInput.Text.Trim().ToUpper()))
                {
                    imgVerificationtxtResult.ImageUrl = "~/App_Themes/Default/OK.jpg";
                    txtVerificationInput.Text = "";
                    isValidate = true;
                }
                else
                {
                    imgVerificationtxtResult.ImageUrl = "~/App_Themes/Default/Wrong.jpg";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "showVerificationtxtResult", "$(function() {$('#" + imgVerificationtxtResult.ClientID + "').removeClass('hiddenitem');});", true);
                    isValidate = false;
                }
            }
            return isValidate;
        }
    }
}