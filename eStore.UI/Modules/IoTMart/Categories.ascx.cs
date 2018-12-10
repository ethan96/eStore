using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation;

namespace eStore.UI.Modules.IoTMart
{
    public partial class Categories : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hyAboutIot.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.AboutIoTMart);
            hyAboutIot.NavigateUrl = ResolveUrl(string.Format("~/Widget.aspx?WidgetName={0}", eStore.Presentation.eStoreContext.Current.getStringSetting("AboutSiteWidgetName")));
            bindRootCateogry();
        }
        protected void bindRootCateogry()
        {
            var rootCategorys = eStoreContext.Current.Store.getTopLeveluStoreCategories(eStoreContext.Current.MiniSite);
            rpRootCategory.DataSource = rootCategorys;
            rpRootCategory.DataBind();
        }
        protected string getMenuCss(object obj)
        {
            if (obj != null && obj is POCOS.ProductCategory)
            {
                POCOS.ProductCategory cate = obj as POCOS.ProductCategory;
                if (cate.childCategoriesX.Any())
                    return "class='iot-navBlock-linkTitle iot_iconList'";
                else
                    return "class='iot-navBlock-linkTitle'";
            }
            return "";
        }
    }
}