using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Policy
{
    public partial class Policy : Presentation.eStoreBaseControls.eStoreBasePage 
    {
        int pid = 0;
        protected string Name;
        protected string Html;

        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }
        }

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
                if (Request.QueryString["pid"] != null)
                {
                    int.TryParse(Request.QueryString["pid"], out pid);
                    POCOS.PolicyCategory category = eStore.Presentation.eStoreContext.Current.Store.getPolicyCategoryById(pid);
                    if (category == null)
                    {
                        Response.Redirect("~/Error404.aspx");
                    }
                    else
                    {
                        if (category.Policy == null)
                            Name = category.Name;
                        else if (eStore.Presentation.eStoreContext.Current.Store.profile.StoreLangID == eStore.Presentation.eStoreContext.Current.CurrentLanguage.Code)
                        {
                            Name = category.Name;
                            Html = category.Policy.HtmlContext;
                        }
                        else//翻译内容
                        {
                            POCOS.PolicyGlobalResource policyGlobal = eStore.Presentation.eStoreContext.Current.Store.getPolicyGlobalResource(eStore.Presentation.eStoreContext.Current.CurrentLanguage.Id, category.Policy.Id);
                            if (policyGlobal == null)
                            {
                                Name = category.Policy.Name;
                                Html = category.Policy.HtmlContext;
                            }
                            else
                            {
                                Name = policyGlobal.Name;
                                Html = policyGlobal.HtmlContext;
                            }
                        }
                        PolicyCategories1.PolicyCategory = category;
                        Dictionary<string, string> yourareherelist = new Dictionary<string, string>();
                        if (category.ParentCategory != null)
                            yourareherelist.Add(category.ParentCategory.Name, "#");
                        else 
                            yourareherelist.Add(category.Name, "#");
                        YouAreHere1.Navigator = category;

                        this.isExistsPageMeta = setPageMeta(
               $"{Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", $"{Name} - {Presentation.eStoreContext.Current.Store.profile.StoreName}", "");
                    }
                } 
            }   
        }

      
    }
}