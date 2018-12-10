using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class PolicyCategories : System.Web.UI.UserControl
    {
        public POCOS.PolicyCategory PolicyCategory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        
        }
        protected override void OnPreRender(EventArgs e)
        {
            if (PolicyCategory == null)
            { this.Visible = false; }
            else if (PolicyCategory.ParentCategory == null)
            {
                lGroupName.Text = PolicyCategory.Name;
                this.Visible = true;
                rpPolicyCagtegories.DataSource = PolicyCategory.SubCategories.ToList();//绑定所有第一层子节点
                rpPolicyCagtegories.DataBind();
            }
            else
            {
                lGroupName.Text = PolicyCategory.ParentCategory.Name;
                this.Visible = true;
                rpPolicyCagtegories.DataSource = PolicyCategory.ParentCategory.SubCategories.ToList();//绑定所有第一层节点
                rpPolicyCagtegories.DataBind();
            }
        }
        protected void rpPolicyCagtegories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.PolicyCategory category = e.Item.DataItem as POCOS.PolicyCategory;
                POCOS.Language language = eStore.Presentation.eStoreContext.Current.CurrentLanguage;
                HyperLink hlPurl = e.Item.FindControl("hlPurl") as HyperLink;
                hlPurl.Attributes.Add("pid", category.Id.ToString());
                if (language.Code == eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID)
                    hlPurl.Text = category.Name;
                else
                    if (category.Policy == null)
                    { hlPurl.Text = category.Name; }
                    else
                    {
                        POCOS.PolicyGlobalResource pgr= eStore.Presentation.eStoreContext.Current.Store.getPolicyGlobalResource(language.Id, category.Policy.Id);
                        if (pgr == null)
                        { hlPurl.Text = category.Policy.Name; }
                        else
                            hlPurl.Text = pgr.Name;//翻译标题
                    }
                       
                hlPurl.NavigateUrl = Presentation.UrlRewriting.MappingUrl.getMappingUrl(category, eStore.Presentation.eStoreContext.Current.MiniSite);

                if (PolicyCategory.Id == category.Id)
                    hlPurl.CssClass = "on";

            }
        }
    }
}