using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules.SSO.Advantech;
using System.Text;
using eStore.POCOS;

namespace eStore.UI
{
    public partial class autoregNew : System.Web.UI.Page


    {
        /// <summary>
        /// Auto register for SAP shiping notice. 
        /// Check if the key is correct, if not, the link is invalid and not send by SAP program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        SSOUSER user;

        protected void Page_Load(object sender, EventArgs e)    
        {
            user = new SSOUSER();     
            string strKey = Request["k"];

           if (checkKey(strKey)){
               
               setSSOUser();
               string strSO, strPO, strTrackno;
               strSO = Request["SO"];
               strPO = Request["PO"];
               strTrackno = Request["t"];

               if (user != null && !string.IsNullOrEmpty(user.email_addr))
               {
                   POCOS.User eUser = Presentation.eStoreContext.Current.Store.getUser(user.email_addr.Trim());
                   if (eUser != null && !eUser.newUser)
                   {
                        Response.Redirect(string.Format("/?e={0}&callbackurl={1}", user.email_addr, string.IsNullOrEmpty(strSO) ? "" : "_autoOrder" + strSO));
                    }
                }

               //if register success, rediret to sso.
               if (Presentation.eStoreContext.Current.Store.registeredUsertoSSO(user))
               {
                   User estoreuser = Presentation.eStoreContext.Current.Store.login(user.email_addr, user.login_password, Presentation.eStoreContext.Current.getUserIP());

                   if(estoreuser !=null)
                       Response.Redirect(string.Format("/sso.aspx?tempid={0}&id={1}&callbackurl={2}", estoreuser.authKey, estoreuser.UserID
                                                      , string.IsNullOrEmpty(strSO) ? "" : "_autoOrder" + strSO));
                    else
                        Response.Redirect("/");
               }
            }
            
        }

        private void setSSOUser() {

            if (!string.IsNullOrEmpty(Request["e"]))
            {
                user.email_addr = Request["e"];
                user.erpid = Request["id"];
                user.country = Request["ctr"];
                user.state = Request["st"];
                user.primary_org_id = Request["oid"];
                user.first_name = user.email_addr.Substring(0, user.email_addr.IndexOf("@"));
                user.login_password = "1234"; //Default password                          
                user.canseeorder = true;
                user.source = "SAP_ship";
                user.siebel_status = "1";
                user.notify = "0";
            }
        
        }
        /// <summary>
        /// Check if the request is from SAP
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        private bool checkKey(string key)
        {
            bool flag = false;
            int total = 0;
            key = key.Trim();
            for (int i = 2; i <= 4; i += 2)
            {
                total = total + Convert.ToInt32(Right(key.Substring(0,i), 2));
            }

            if (total == Convert.ToInt32(Right(key, 2)))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;

        }

        /// <summary>
        /// Similiar Right function in VB
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private string Right(string s, int count)
        {
            string newString = String.Empty;
            if (s != null && count > 0)
            {
                int startIndex = s.Length - count;
                if (startIndex > 0)
                    newString = s.Substring(startIndex, count);
                else
                    newString = s;
            }
            return newString;
        }
    }
}