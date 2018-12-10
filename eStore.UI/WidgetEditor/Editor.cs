using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace eStore.UI.WidgetEditor
{
    public class Editor : AjaxControlToolkit.HTMLEditor.Editor
    {
        #region [ Properties ]

        [DefaultValue("WidgetConfig")]
        [Category("Appearance")]
        [Description("Widget Name")]
        public String WidgetName
        {
            get { return (String)(ViewState["WidgetName"] ?? "WidgetConfig"); }
            set { ViewState["WidgetName"] = value; }
        }

        [DefaultValue("~/App_Data/WidgetSource/")]
        [Category("Appearance")]
        [Description("Widget XML Source folder")]
        public string WidgetConfigurationSource
        {
            get { return (string)(ViewState["WidgetConfigurationSource"] ?? "~/App_Data/WidgetSource/"); }
            set { ViewState["WidgetConfigurationSource"] = value; }
        }

        #endregion
 
        protected override void FillTopToolbar()
        {
            Collection<AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption> options;
            AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption option;

            eStore.UI.WidgetEditor.SelectorIcon si = new eStore.UI.WidgetEditor.SelectorIcon();
            si.WidgetName = this.WidgetName;
            si.WidgetConfigurationSource = this.WidgetConfigurationSource;
            TopToolbar.Buttons.Add(si);


            eStore.UI.WidgetEditor.MutliSelector.UserControlSelector ucs = new eStore.UI.WidgetEditor.MutliSelector.UserControlSelector();
            ucs.Attributes.Add("onchange", "insertWidget(\"" + string.Format(@"eStoreUserControl:{0}", "ustest") + "\")");
            ucs.ToolTip = "select uc";
            TopToolbar.Buttons.Add(ucs);
            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control1";
            option.Value = "uc1";
            options.Add(option);

            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control2";
            option.Value = "uc2";
            options.Add(option);

            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control3";
            option.Value = "uc3";
            options.Add(option);

            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control4";
            option.Value = "uc4";
            options.Add(option);

            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control5";
            option.Value = "uc5";
            options.Add(option);

            options = ucs.Options;
            option = new AjaxControlToolkit.HTMLEditor.ToolbarButton.SelectOption();
            option.Text = "User Control6";
            option.Value = "uc6";
            options.Add(option);





           // base.FillTopToolbar();
            TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());

        }

        public override string ButtonImagesFolder
        {
            get
            {
                return "~/App_Images/HTMLEditor.customButtons/";
            }
        }
    }
}