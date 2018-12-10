using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using esUtilities;
using System.Web.UI.HtmlControls;

namespace eStore.Presentation.eStoreBaseControls
{
    public class eStoreBaseUserControl : System.Web.UI.UserControl
    {
        public eStoreBaseUserControl()
        {

        }

        public bool isMobile
        {
            get
            {
                if (this.Page is eStoreBaseCommonPage)
                    return ((eStoreBaseCommonPage)this.Page).isMobile;
                return false;
            }
        }
 
        protected virtual void BindScript(string ScriptsType,string ScriptsName,string Script)
        {
            if (ScriptsType.ToLower ()=="url")
            {
                if(!Script.ToLower().Contains("http"))
                    Script = CommonHelper.GetStoreLocation() + "Scripts/" + Script;
                Page.ClientScript.RegisterClientScriptInclude(ScriptsName, Script);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), ScriptsName, Script, true);
            }
        }
        public void alertMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
                BindScript("Script", "eStoreAlertMessage", "$(document).ready(function () { alert(\"" + message + "\"); });");
        }
        public virtual void setPageMeta(string title, string Description, string keyword)
        {
            if (this.Page is eStoreBaseCommonPage)
            {
                eStoreBaseCommonPage page = (eStoreBaseCommonPage)this.Page;
                page.isExistsPageMeta = page.setPageMeta(title, Description, keyword);
            }
        }

        protected virtual void AddStyleSheet(string link,string media=null)
        {
            HtmlLink csslink = new HtmlLink();
            csslink.Href = link;
            csslink.Attributes.Add("rel", "stylesheet");
            csslink.Attributes.Add("type", "text/css");
            if (!string.IsNullOrEmpty(media))
                csslink.Attributes.Add("media", media);
            try
            {
                if (this.Page.Header != null)
                    this.Page.Header.Controls.Add(csslink);
            }
            catch (Exception)
            {
             
            }

        }
        protected void popLoginDialog(object sender)
        {
            if (string.IsNullOrEmpty(Request["needlogin"]))
            {
                string purpose = "";
                if (sender is System.Web.UI.WebControls.Button)
                { purpose = (sender as System.Web.UI.WebControls.Button).Text; }
                else if (sender is System.Web.UI.WebControls.LinkButton)
                { purpose = (sender as System.Web.UI.WebControls.LinkButton).Text; }
                else if (sender is System.Web.UI.WebControls.HyperLink)
                { purpose = (sender as System.Web.UI.WebControls.HyperLink).Text; }

                if (Request.RawUrl.IndexOf("?") > 0)
                    Response.Redirect(Request.RawUrl + "&needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
                else
                    Response.Redirect(Request.RawUrl + "?needlogin=true&purpose=" + esUtilities.CommonHelper.RemoveHtmlTags(purpose));
            }
        }
        protected string GetLocaleResourceString(string ResourceName)
        {
            //Language language = NopContext.Current.WorkingLanguage;
            //return LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId);
            return ResourceName;
        }

        protected string GetLocaleResourceString(string ResourceName, params object[] args)
        {
            //Language language = NopContext.Current.WorkingLanguage;
            //return string.Format(
            //    LocalizationManager.GetLocaleResourceString(ResourceName, language.LanguageId),
            //    args);
            return ResourceName;
        }

        //如果url有errorPath 就移除  (全部url) 编码过的
        public string CurrentUrlEncodePath
        {
            get { return HttpUtility.UrlEncode(Presentation.eStoreContext.Current.ripErrorPath(Request.Url.OriginalString)); }
        }

        //如果url有errorPath  就移除  (排除域名意外的url)
        public string CurrentPagePath
        {
            get { return Presentation.eStoreContext.Current.CurrentPagePath;}
        }
    }
}
