using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Product
{
    public partial class AllSolution : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool isMobileFriendly
        {
            get
            {
                return true;
            }
            set
            {
                base.isMobileFriendly = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                IList<POCOS.Solution> solutions = eStore.Presentation.eStoreContext.Current.Store.getAllSolution().Where(s => s.PublishStatus == true).ToList();
                this.rp_AllSolution.DataSource = solutions;
                this.rp_AllSolution.DataBind();
            }
            this.isExistsPageMeta = setPageMeta(
                $"{eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Show_All_Solutions)} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "", "");
        }
    }
}