using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using esUtilities;
namespace eStore.Presentation.eStoreBaseControls
{
    public class eStoreBaseMasterPage:System.Web.UI.MasterPage 
    {
        public string BaseUrl
        {
            get { return Request.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.GetDirectory(Request.FilePath); }
        }

        protected virtual void AddStyleSheet(string link, string media = null)
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
        protected virtual void BindScript(string ScriptsType, string ScriptsName, string Script)
        {
            if (ScriptsType.ToLower() == "url")
            {
                if (!Script.ToLower().Contains("http"))
                    Script = CommonHelper.GetStoreLocation() + "Scripts/" + Script;
                Page.ClientScript.RegisterClientScriptInclude(ScriptsName, Script);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), ScriptsName, Script, true);
            }
        }

        /* this JS script will hold eStore loading when corp web site is down.
                /// <summary>
                /// 是否引用Tranking log
                /// </summary>
                public bool IsUseAdvTrack
                {
                    get { return isUseAdvTrack; }
                    set { isUseAdvTrack = value; }
                }
                private bool isUseAdvTrack = true;

                protected override void Render(System.Web.UI.HtmlTextWriter writer)
                {
                    if (isUseAdvTrack)
                    {
                        System.Web.HttpCookie cookieUser = new System.Web.HttpCookie("eStore_Adv_Webtracking");
                        string username = "eStoreGuester";
                        if (Presentation.eStoreContext.Current.User != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.UserID))
                            username = Presentation.eStoreContext.Current.User.UserID;
                        cookieUser.Value = username;
                        cookieUser.Expires = DateTime.Now.AddDays(365);
                        Response.AppendCookie(cookieUser);

                
                        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("AdvTrack"))
                        {
                            string IncludeScript = @"http://advwebtracking.advantech.com/Track/AdvTrack.js";
                            this.Page.ClientScript.RegisterClientScriptInclude("AdvTrack", IncludeScript);
                        }
                        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("jquerycookies"))
                        {
                            string IncludeScript2 = @"http://advwebtracking.advantech.com/js/jquery.cookies.2.2.0.js";
                            this.Page.ClientScript.RegisterClientScriptInclude("jquerycookies", IncludeScript2);
                        }

                        if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("ForWWWAdvTrack"))
                        {
                            string IncludeScript2 = esUtilities.CommonHelper.GetStoreLocation() + @"Scripts/ForWWWAdvTrack.js";
                            this.Page.ClientScript.RegisterClientScriptInclude("ForWWWAdvTrack", IncludeScript2);
                        }
                    }

                    base.Render(writer);
                }

        */
    }

}
