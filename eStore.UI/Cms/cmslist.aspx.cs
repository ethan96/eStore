using System;


namespace eStore.UI.Cms
{
    public partial class cmslist : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //提取到eStore.BusinessModules中
            string rbu = eStore.Presentation.eStoreContext.Current.getStringSetting("StoreRBU");
            string attribute = Request.QueryString["attr"] ?? "iotmart";
            rpcmsls.DataSource = eStore.BusinessModules.CMSManager.GetCmsFromApi(rbu, attribute, "news");
            rpcmsls.DataBind();
        }
    }
}