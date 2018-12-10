using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using System.CodeDom;
using System.Drawing;
using System.IO;
using AjaxControlToolkit;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using eStore.Presentation;

namespace eStore.UI.WidgetEditor
{
    [ParseChildren(true)]
    [PersistChildren(false)]
    [RequiredScript(typeof(OkCancelPopupButton))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    public class SelectorIcon : DesignModePopupImageButton
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

        #region [ Methods ]

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RelatedPopup = new SelectorPopups();
            (RelatedPopup as SelectorPopups).WidgetName = WidgetName;
            (RelatedPopup as SelectorPopups).WidgetConfigurationSource = WidgetConfigurationSource;
        }

        protected override void OnPreRender(EventArgs e)
        {
            RegisterButtonImages("ed_insertIcon");
            base.OnPreRender(e);
        }

        protected override string ClientControlType
        {
            get { return "Sys.Extended.UI.HTMLEditor.CustomToolbarButton.InsertIcon"; }
        }

        public override string ScriptPath
        {
            get { return "~/Scripts/InsertIcon.js"; }
        }

        public override string ToolTip
        {
            get { return "Insert Widget Target"; }
        }

        #endregion
    }

    [ParseChildren(true)]
    [RequiredScript(typeof(AjaxControlToolkit.HTMLEditor.Popups.AttachedTemplatePopup))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    internal class SelectorPopups : AjaxControlToolkit.HTMLEditor.Popups.AttachedTemplatePopup
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

        #region [ Methods ]

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        protected string LocalResolveUrl(string path)
        {
            string temp = base.ResolveUrl(path);
            Regex _Regex = new Regex(@"(\(S\([A-Za-z0-9_]+\)\)/)", RegexOptions.Compiled);
            temp = _Regex.Replace(temp, "");
            return temp;
        }

        protected override void CreateChildControls()
        {
            Table table;
            TableRow row = null;
            TableCell cell;

            string WidgetConfigurationSource = LocalResolveUrl(this.WidgetConfigurationSource);
            if (WidgetConfigurationSource.Length > 0)
            {
                string lastCh = WidgetConfigurationSource.Substring(WidgetConfigurationSource.Length - 1, 1);
                if (lastCh != "\\" && lastCh != "/") WidgetConfigurationSource += "/";
            }

            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(WidgetConfigurationSource)))
            {
                XmlDocument widgetConfig = new XmlDocument();
                string widgetConfigFilePath = System.Web.HttpContext.Current.Server.MapPath(WidgetConfigurationSource) + this.WidgetName + ".xml";

                if (File.Exists(widgetConfigFilePath))
                { widgetConfig.Load(widgetConfigFilePath); }
                else
                { return; }

                XmlNodeList widgetobjects = widgetConfig.SelectNodes("/Widget/Objects/Object");
                XmlNodeList widgetUserControls = widgetConfig.SelectNodes("/Widget/UserControls/UserControl");

                if (widgetUserControls.Count > 0)
                {
                    table = new Table();
                    table.Attributes.Add("border", "1px solid black");
                    row = new TableRow();
                    cell = new TableCell();
                    cell.ColumnSpan = widgetUserControls.Count;
                    cell.Text = "User Control";
                    row.Cells.Add(cell);
                    table.Rows.Add(row);

                    row = new TableRow();
                    foreach (XmlNode ucConfig in widgetUserControls)
                    {
                        cell = new TableCell();
                        string objID = ucConfig.Attributes["id"].Value.ToString();
                        //string UCName = ucConfig.SelectSingleNode("./Name").InnerText;

                        cell.Text = string.Format(@"eStoreUserControl:{0}", objID);
                        cell.Style.Add("cursor", "pointer");
                        cell.Attributes.Add("onmousedown", "insertWidget(\"" + string.Format(@"eStoreUserControl:{0}", objID) + "\")");
                        row.Cells.Add(cell);
                    }
                    table.Rows.Add(row);
                    Content.Add(table);
                }

                if (widgetobjects.Count > 0)
                {

                    table = new Table();
                    table.Attributes.Add("border", "1px solid black");
                   
                    row = new TableRow();
                    cell = new TableCell();
                    cell.ColumnSpan = widgetobjects.Count;
                    cell.Text = "Objects";

                    row.Cells.Add(cell);
                    table.Rows.Add(row);
                    TableRow ObjectName = new TableRow();
                    row = new TableRow();
                    foreach (XmlNode objConfig in widgetobjects)
                    {
                        WidgetManager wm = new WidgetManager();

                        string objID = objConfig.Attributes["id"].Value.ToString();
                        cell = new TableCell();
                        cell.Text = objID;
                        ObjectName.Cells.Add(cell);

                        string sInvokeMethod = objConfig.SelectSingleNode("./InvokeMethod").InnerText;
                        MethodInfo miInvokeMethod = wm.GetType().GetMethod(sInvokeMethod);


                        XmlNodeList objParas = objConfig.SelectNodes("./Parameter");
                        Object[] imPara = new Object[objParas.Count];

                        for (int i = 0; i < objParas.Count; i++)
                        {
                            string pname = objParas[i].Attributes["Name"].Value;
                            string pvalue = objParas[i].Attributes["Value"].Value;
                            string ptype = objParas[i].Attributes["Type"].Value;
                            imPara[i] = pvalue;
                        }

                        Object newobj = miInvokeMethod.Invoke(wm, imPara);

                        Type objType = newobj.GetType();
                        PropertyInfo[] objpi = objType.GetProperties();

                        cell = new TableCell();


                        foreach (PropertyInfo pi in objpi)
                        {
                            if (pi.PropertyType.FullName == "System.String")
                            {
                                Label lblproperty = new Label();
                                lblproperty.Text = string.Format("eStoreObject:{0}.{1}<br />", objID, pi.Name);
                                lblproperty.Attributes.Add("onmousedown", "insertWidget(\"" + string.Format("eStoreObject:{0}.{1}", objID, pi.Name) + "\")"); ;
                                lblproperty.Style.Add("cursor", "pointer");
                                cell.Controls.Add(lblproperty);
                            }
                        }
                        cell.Attributes.Add("vertical-align", "top");
                        row.Cells.Add(cell);
                        table.Rows.Add(ObjectName);
                        table.Rows.Add(row);

                        Content.Add(table);
                    }
                }

                base.CreateChildControls();
            }

        #endregion
        }
    }
}