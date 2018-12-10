using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using eStore.Presentation;
using eStore.Presentation.eStoreBaseControls;
using System.Web.UI;

namespace eStore.UI.MasterPages
{
    public partial class TwoColumnIoT : eStoreBaseMasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            ServiceReference Service = eStoreScriptManager.Services.FirstOrDefault();
            if (Service != null)
                Service.Path = ResolveUrl("~/eStoreScripts.asmx");
            base.OnInit(e);
            AddStyleSheet(ResolveUrl("~/Styles/IotMart/contact-panel.css"));
            BindScript("url", "tabSlideOut", "jquery.tabSlideOut.v1.3.js");
            BindScript("url", "cookie", "jquery.cookie.js");
            BindScript("url", "ContactPanel", "ContactPanel.js");
            BindScript("url", "IoTApp", "IoTApp.js");
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