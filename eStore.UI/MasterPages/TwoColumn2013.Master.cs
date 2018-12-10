using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;
using eStore.Presentation;
using System.Web.Security;
using System.Security.Principal;
using esUtilities;

namespace eStore.UI.MasterPages
{
    public partial class TwoColumn2013 : eStoreBaseMasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            ServiceReference Service = eStoreScriptManager.Services.FirstOrDefault();
            if (Service != null)
                Service.Path = ResolveUrl("~/eStoreScripts.asmx");
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
         

             
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
               
            }
            catch (Exception)
            {


            }
            base.OnPreRender(e);
        }
       
        
        protected void HeadLoginStatus_LoggedOut(object sender, EventArgs e)
        {
            if (eStoreContext.Current.User != null)
            {
                eStoreContext.Current.Store.logout(eStoreContext.Current.User);
                POCOS.Country selectedcountry = Presentation.eStoreContext.Current.CurrentCountry;
                HttpContext.Current.Session.Clear();
                Presentation.eStoreContext.Current.CurrentCountry = selectedcountry;
            }

        }

        protected void ddlEmployeeRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        FormsIdentity id =
                            (FormsIdentity)HttpContext.Current.User.Identity;
                        FormsAuthenticationTicket ticket = id.Ticket;


                        try
                        {
                            string[] roles = new string[1];
                            roles[0] = ((DropDownList)sender).SelectedItem.Text;
                            string[] loginInfo = ticket.UserData.Split(';');
                            string userdata = string.Format("{0};{1};{2};{3}", loginInfo[0], loginInfo[1], roles[0], loginInfo[3]);

                            //HttpContext.Current.Session.Abandon();
                            HttpContext.Current.Session.Clear();
                            FormsAuthenticationTicket t = new FormsAuthenticationTicket(1, ticket.Name,
                                DateTime.Now, DateTime.Now.AddMonths(3),
                                ticket.IsPersistent, userdata,
                                FormsAuthentication.FormsCookiePath);
                            string encTicket = FormsAuthentication.Encrypt(t);
                            HttpCookie c = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                            if (ticket.IsPersistent)
                            {
                                HttpContext.Current.Response.Cookies.Clear();
                                c.Expires = t.Expiration;
                            }
                            HttpContext.Current.Response.Cookies.Add(c);


                            FormsIdentity NewID = new FormsIdentity(t);
                            GenericPrincipal principal = new GenericPrincipal(NewID, roles);
                            POCOS.User.Role activeRole = (POCOS.User.Role)Enum.Parse(typeof(POCOS.User.Role), roles[0]);
                            eStoreContext.Current.User.switchRole(activeRole);
                            Response.Redirect("~/");
                        }
                        catch { }

                    }
                }
            }
        }

 
    }
}