using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.eStoreBaseControls;
using esUtilities;
using eStore.Presentation;
using System.Web.Security;
using System.Security.Principal;

namespace eStore.UI.MasterPages
{
    public partial class Configurable : eStoreBaseMasterPage
    {

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                loadConfigurationControls();
                base.CreateChildControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            ServiceReference Service = eStoreScriptManager.Services.FirstOrDefault();
            if (Service != null)
                Service.Path = ResolveUrl("~/eStoreScripts.asmx");

            base.OnInit(e);
            EnsureChildControls();
        }
        void loadConfigurationControls()
        {
            string headerCtrl = Presentation.eStoreContext.Current.getStringSetting("HeaderCtrl");
            string sideCtrl = Presentation.eStoreContext.Current.getStringSetting("SideCtrl");
            string footerCtrl = Presentation.eStoreContext.Current.getStringSetting("FooterCtrl");
            AddCtrlToPlaceHolder("eStoreHeaderContent", headerCtrl);
            AddCtrlToPlaceHolder("eStoreConfigurableRightContent", sideCtrl);
            AddCtrlToPlaceHolder("eStoreFooterContent", footerCtrl);
        }

        void AddCtrlToPlaceHolder(string PlaceHolderName, string controls)
        {
            try
            {
                if (string.IsNullOrEmpty(PlaceHolderName) || string.IsNullOrEmpty(controls))
                    return;

                ContentPlaceHolder ph = FindControl(PlaceHolderName) as ContentPlaceHolder;
                if (ph != null && ph.HasControls() == false)
                {
                    foreach (string ctrl in controls.Split(';'))
                    {
                        Control uc = LoadControl(string.Format("~/Modules/{0}.ascx", ctrl));
                        if (uc != null)
                        {
                            ph.Controls.Add(uc);
                            if (ctrl == "ECO/ECOPartnerSearch")
                                uc.ID = "ECOPartnerSearch1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

    }
}