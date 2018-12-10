using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Modules
{
    public partial class ResaleSetting_CNPJ : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        #region Property
        public string ResellerID
        {
            get {
                if (!string.IsNullOrEmpty(this.txtResaleSettingCNPJ.Text.Trim()))
                    return this.txtResaleSettingCNPJ.Text.Trim();
                else
                    return string.Empty;
            }
            set { this.txtResaleSettingCNPJ.Text = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Order不存在CNPJ/CPF 使用User的ResellerID 
                if (string.IsNullOrEmpty(ResellerID) && Presentation.eStoreContext.Current.User != null && !string.IsNullOrEmpty(Presentation.eStoreContext.Current.User.actingUser.ResellerID))
                    this.ResellerID = Presentation.eStoreContext.Current.User.actingUser.ResellerID;
            }
        }
    }
}