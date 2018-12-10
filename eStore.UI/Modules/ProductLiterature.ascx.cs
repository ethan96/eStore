using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class ProductLiterature : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private string ProductLiteratureString { get; set; }
        public List<POCOS.Part> LiteratureList = new List<POCOS.Part>();

        public void AddPart(POCOS.Part part)
        {
            LiteratureList.Add(part);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindScript("url", "lightbox", "jquery.lightbox-0.5.min.js");
            this.BindScript("Script", "lightBox", "$(function() {$(\".productLargeImages a\").lightBox({maxHeight: 600,maxWidth: 800}); });");
            this.AddStyleSheet("/Styles/jquery.lightbox-0.5.css");
            if (!IsPostBack)
            {
                //All Img Picture
                StringBuilder sbLargePicture = new StringBuilder();
                //Small Picture
                StringBuilder sbframe = new StringBuilder();

                //LightBox
                StringBuilder sbLightBox = new StringBuilder();
                //Description
                StringBuilder sbLiterature = new StringBuilder();
                if (LiteratureList.Count == 0)
                {
                    this.Visible = false;
                    return;
                }
                else if (LiteratureList.Count == 1)
                {
                    POCOS.Part part = LiteratureList[0];
                    sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", part.thumbnailImageX, part.SProductID);

                    sbLiterature.Append("<div><div class=\"resourceheader\">" + eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Resouces) + "</div><ul class=\"resourcelist\">");
                    bool has3DModel = false;
                    int largeimagecount = 0;
                    foreach (POCOS.ProductResource pr in part.ProductResources)
                    {
                        switch (pr.ResourceType)
                        {
                            case "LargeImages":
                                sbLightBox.AppendFormat("<li><a href=\"{0}\" title=\"{1}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\"  height=\"32px\"/></a></li>", pr.ResourceURL, part.name, part.name);
                                largeimagecount++;
                                break;
                            
                            case "LargeImage":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", pr.ResourceURL, "Large Image");
                                    break;
                            case "Utilities":
                            case "Datasheet":
                            case "Download":
                            case "Manual":
                            case "Driver":
                                sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", pr.ResourceURL, pr.ResourceName);
                                break;
                            case "3DModel":
                                has3DModel = true;
                                break;
                            default:
                                break;

                        }
                    }
                    if (has3DModel)
                        sbLiterature.AppendFormat("<li class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></li>",
                            eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                           part.SProductID);
                    if (part is POCOS.Product && part.specs != null && part.specs.Count>0)
                        sbLiterature.AppendFormat("<li class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProductSpecsDialog('{1}');\" title=\"\">{0}</a></li>",
                          "Extended Specifications",
                         part.SProductID);
                    sbLiterature.Append("</ul></div>");

                    if (largeimagecount >0)
                    {
                        sbLightBox.Insert(0, "<ul class=\"productLargeImages\"><li><img src=\"/images/btn_loop.jpg\"  width=\"32px\"  height=\"32px\"/></li>");
                        sbLightBox.Append("</ul>");

                    }
                    else
                    { sbLightBox.Clear(); }
                    ProductLiteratureString = string.Format("<div class=\"divLargePicture\">{0} </div>{1}{2}", sbLargePicture.ToString(), sbLightBox.ToString(), sbLiterature.ToString());
                }
                else
                {
                    int index = 0;
                    foreach (POCOS.Part part in LiteratureList)
                    {
                        if (index == 0)
                        {
                            sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\"  width=\"150px\" />", part.thumbnailImageX, part.name);
                            sbLiterature.AppendFormat("<div id=\"tab_{0}\" >", index);
                        }
                        else
                        {
                            sbLargePicture.AppendFormat("<img src=\"{0}\" alt=\"{1}\" class=\"ui-tabs-hide\"  width=\"150px\" />", part.thumbnailImageX, part.name);
                            sbLiterature.AppendFormat("<div id=\"tab_{0}\" class=\"ui-tabs-hide\">", index);

                        }

                        sbframe.AppendFormat(" <li><a href=\"#tab_{2}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", part.thumbnailImageX, part.name, index);
                        sbLiterature.Append("<ul class=\"resourcelist\">");
                        bool has3DModel = false;
                        foreach (POCOS.ProductResource pr in part.ProductResources)
                        {

                            switch (pr.ResourceType)
                            {
                                case "LargeImages":
                                    sbLightBox.AppendFormat("<li><a href=\"{0}\" title=\"{1}\"><img src=\"{0}\"   alt=\"{1}\" title=\"{1}\" width=\"32px\" /></a></li>", pr.ResourceURL, part.name, part.name);
                                    break;
                               
                                case "LargeImage":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", pr.ResourceURL, "Large Image");
                                    break;
                                case "Utilities":
                                case "Datasheet":
                                case "Download":
                                case "Manual":
                                case "Driver":
                                    sbLiterature.AppendFormat("<li><a href=\"{0}\" Target=\"_blank\" title=\"\">{1}</a></li>", pr.ResourceURL, pr.ResourceName);
                                    break;
                                case "3DModel":
                                    has3DModel = true;
                                    break;
                                default:
                                    break;

                            } 
                        }
                        if (has3DModel)
                            sbLiterature.AppendFormat("<li class=\"onecolumn\"><a href=\"#\"  onclick=\"return showProduct3DModelDialog('{1}');\" title=\"\">{0}</a></li>",
                                eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_DownLoad3DModel),
                               part.SProductID);

                        sbLiterature.Append("</ul></div>");
                        index++;
                    }
                    ProductLiteratureString = string.Format("<div class=\"divLargePicture\" id=\"images\">{0} </div><ul class=\"frameUl\">{1}</ul>{2}", sbLargePicture.ToString(), sbframe.ToString(), sbLiterature.ToString());
                }
                this.lLiterature.Text = ProductLiteratureString;
            }
        }
    }
}
