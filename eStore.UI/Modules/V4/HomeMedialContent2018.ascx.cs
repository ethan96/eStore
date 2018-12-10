using eStore.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class HomeMedialContent2018 : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            ///show theme banner
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EableThemeBanner"))
            {
                var themebanners = eStore.Presentation.eStoreContext.Current.Store.GetKitThemeLs();
                rpThemeBanners.DataSource = themebanners;
                rpThemeBanners.DataBind();
                rpThemeBanners.Visible = themebanners.Any();
            }

            var catelist = eStoreContext.Current.Store.getTopLevelCategories();

            List<POCOS.ProductCategory> spc = catelist.Where(c=> new string[] { "StandardCategory", "CTOSCategory" }.Contains(c.CategoryType))
                .OrderBy(c=> c.Sequence).ThenBy(c=>c.localCategoryNameX).ToList();
            rpCategories.DataSource = spc;
            rpCategories.DataBind();

            List<POCOS.ProductCategory> cpc = catelist.Where(c=>c.CategoryType.Equals("ApplicationCategory")).OrderBy(c => c.Sequence).ThenBy(c => c.localCategoryNameX).ToList();
            rpApps.DataSource = cpc;
            rpApps.DataBind();

        }
    }
}