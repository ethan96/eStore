using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class SolutionStore :Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.LoadSolutionStore();
                base.ChildControlsCreated = true;
            }
        }

        private void LoadSolutionStore()
        {
            string SolutionStoreID = esUtilities.CommonHelper.QueryString("id");
            try
            {
                switch (SolutionStoreID.ToLower())
                {
                    case "productautomation":
                        Response.Redirect("~/Production-Automation/Production-Automation/dhtml-714.htm");
                        break;

                    default:
                        Control control = LoadControl(string.Format("~/Modules/SolutionStoreTemplate/{0}_Content_{1}.ascx", SolutionStoreID, Presentation.eStoreContext.Current.Store.storeID));
                        phSolutionStore.Controls.Add(control);
                        this.AddStyleSheet("/Styles/CB_01.css");
                        break;
                }
            }
            catch (Exception)
            {
               
  
            }
     
        }
    }
}