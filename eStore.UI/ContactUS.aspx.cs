using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI
{
    public partial class ContactUS1 : Presentation.eStoreBaseControls.eStoreBasePage 
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
         
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            POCOS.PolicyCategory category = eStore.Presentation.eStoreContext.Current.Store.getPolicyCategoryByUrl("~/ContactUs.aspx");
            if (category != null)
            {
                YouAreHere1.Navigator = category;
                PolicyCategories1.PolicyCategory = category;
                if (!this.isExistsPageMeta)
                    this.isExistsPageMeta = setPageMeta(
                 $"{category.Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{category.Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");

            }
        }
    }
}