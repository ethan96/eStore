using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text.RegularExpressions;
using eStore.POCOS;
using System.Reflection;
using System.IO;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class LoadWidgets : System.Web.UI.UserControl
    {
        XmlDocument widgetConfig = new XmlDocument();
        Dictionary<string, object> objDict = new Dictionary<string, object>();
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Widgets.Controls.Clear();

            string widgetName;

            if (!string.IsNullOrEmpty(this.Request.QueryString["widget"]))
            {
                widgetName = this.Request.QueryString["widget"];

            }
            else
            {
                widgetName = "WidgetConfig";
            }
          
            string widgetConfigFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/WidgetSource/") + widgetName + ".xml";

            if (File.Exists(widgetConfigFilePath))
            {
                widgetConfig.Load(widgetConfigFilePath);

            }



            XmlNodeList widgetobjects = widgetConfig.SelectNodes("/Widget/Objects/Object");
            XmlNodeList widgetUserControls = widgetConfig.SelectNodes("/Widget/UserControls/UserControl");


            foreach (XmlNode ucConfig in widgetUserControls)
            {
                string objID = ucConfig.Attributes["id"].Value.ToString();
                string UCName = ucConfig.SelectSingleNode("./Name").InnerText;
                Object newuc = LoadControl(string.Format("~/Modules/{0}.ascx", UCName));
                Type newucType = newuc.GetType();
                XmlNodeList UCParas = ucConfig.SelectNodes("./Parameter");
                foreach (XmlNode ucp in UCParas)
                {
                    string pname = ucp.Attributes["Name"].Value;
                    string pvalue = ucp.Attributes["Value"].Value;
                    string ptype = ucp.Attributes["Type"].Value;
                    PropertyInfo pi = newucType.GetProperty(pname);
                    pi.SetValue(newuc, pvalue, null);
                }
                Literal UCStart = new Literal();
                UCStart.Text = String.Format("<%--{0} Start--%>", objID);
                Literal UCEnd = new Literal();
                UCEnd.Text = String.Format("<%--{0} End--%>", objID);
                this.Widgets.Controls.Add(UCStart);
                this.Widgets.Controls.Add((UserControl)newuc);
                this.Widgets.Controls.Add(UCEnd);
            }
            foreach (XmlNode objConfig in widgetobjects)
            {
                WidgetManager wm = new WidgetManager();

                string objID = objConfig.Attributes["id"].Value.ToString();
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
                objDict.Add(objID, newobj);


            }
        }



        protected override void Render(HtmlTextWriter writer)
        {

            StringWriter html = new StringWriter();
            HtmlTextWriter tw = new HtmlTextWriter(html);
            base.Render(tw);
            string outhtml = html.ToString();

            string widgetTemplate = widgetConfig.SelectSingleNode("/Widget/Template").InnerText;

            Regex objectreg = new Regex(@"\{eStoreObject:(.*?)\.(.*?)\}");
            MatchCollection mcobj = objectreg.Matches(widgetTemplate);
            foreach (Match mobj in mcobj)
            {
                string objID = mobj.Groups[1].Value;
                string objPropertyName = mobj.Groups[2].Value;
                Object tmpObj = this.objDict[objID];
                Type tmpObjType = tmpObj.GetType();

                widgetTemplate = Regex.Replace(widgetTemplate, mobj.Groups[0].Value, tmpObjType.GetProperty(objPropertyName).GetValue(tmpObj, null).ToString());
            }



            Regex ucreg = new Regex(@"{eStoreUserControl:(\w*?)}");
            MatchCollection mcuc = ucreg.Matches(widgetTemplate);

            foreach (Match match in mcuc)
            {
                string ucname = match.Groups[1].Value;

                widgetTemplate = Regex.Replace(widgetTemplate, "{eStoreUserControl:" + ucname + "}", Regex.Match(outhtml, @"<%--" + ucname + @" Start--%>([\s\S]*?)<%--" + ucname + @" End--%>").Groups[1].Value);

            }



            writer.Write(widgetTemplate);
        }
    }
}