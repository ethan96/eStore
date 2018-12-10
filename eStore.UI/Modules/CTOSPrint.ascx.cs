using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using System.Text;

namespace eStore.UI.Modules
{
    public partial class CTOSPrint : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Product product { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Product_Ctos _ctos = (POCOS.Product_Ctos)product;
            if (_ctos != null)
            {
                renderCTOS(_ctos, null);

                List<POCOS.Part> specSource = _ctos.specSources;
                foreach (POCOS.Part ss in specSource)
                {
                    imgProduct.ImageUrl = ss.thumbnailImageX;
                }
            }
            if (!IsPostBack)
            {
                bindFonts();
            }
        }
      

        private void renderCTOS(Product_Ctos CTOS, BTOSystem BTOS)
        {
            if (BTOS == null)
                BTOS = CTOS.getDefaultBTOS();
            Price _price = CTOS.updateBTOSPrice(BTOS);
            StringBuilder CTOSBuilder = new System.Text.StringBuilder();

            foreach (CTOSBOM component in CTOS.components)
            {
                if (component.Show)
                {

                    List<BTOSConfig> selectItems = CTOS.getSelectedComponentOptions(BTOS, component);
                    //if (selectItems.Count == 0)
                    //    selectItems.Add(component.ChildComponents.FirstOrDefault());

                    CTOSBuilder.AppendLine(String.Format("<div class=\"module\" id=\"module-{0}\">", component.ComponentID));


                    CTOSBuilder.AppendLine(String.Format("<div class=\"moduleheader expoptionimg\"><span class=\"ctosCategory\">{0}</span> : <span class=\"ctosSelectItem\">{1}</span></div>",
                                                                                        component.name,
                                                                                        selectItems == null || selectItems.Count == 0
                                                                                        ? string.Empty
                                                                                        : string.Join(" | ", selectItems.Select(x => x.matchedOption.desc).ToArray())));

                    if (!string.IsNullOrEmpty(component.Message))
                    {
                        CTOSBuilder.AppendLine(string.Format("<p class=\"colorRed\">{0}</p>", component.Message));
                    }

                    switch (component.InputType)
                    {
                        case "Radio":
                        case "Select":
                        case "Multiselect":
                            foreach (CTOSBOM option in component.options)
                            {
                                BTOSConfig selected = selectItems.FirstOrDefault(x => x.matchedOption.ID == option.ID);
                                CTOSBuilder.AppendLine(String.Format("<div class=\"options\">"));
                                CTOSBuilder.AppendLine(string.Format(selected != null && selected.matchedOption != null ?  "<span class=\"optiondesc\" ><b>{0}</b></span>" :  "<span class=\"optiondesc\" >{0}</span>"
                                    , option.name));
                              
                                if (component.ComponentID == 21)
                                { CTOSBuilder.Append("</div>"); }
                                else if (component.isWarrantyType())
                                {
                                    CTOSBuilder.AppendLine(string.Format(" [ {0}% ]</div>"
                                    , option.priceX));
                                }
                                else
                                {
                                    decimal sum = option.priceX - ((selected == null || selected.matchedOption == null) ? 0 : selected.matchedOption.priceX);
                                    string sumSign = "";
                                    if (sum >= 0)
                                        sumSign = "+";
                                    else if (sum < 0)
                                        sumSign = "-";
                                    CTOSBuilder.AppendLine(string.Format(" [ <span class=\"priceSing\">{2}</span>{0}<span class=\"addtionprice\" >{1:n0}</span> ]</div>"
                                     , Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign
                                     , Math.Abs(sum).ToString("n0")
                                     , sumSign));
                                }


                            }



                            break;
  
                        default:
                            break;
                    }
                }

                CTOSBuilder.AppendLine("</div>");
            }

            this.CTOSModules.Text = CTOSBuilder.ToString();

            this.lProductName.Text = CTOS.name;
            this.lShortDescription.Text = CTOS.productDescX;
            this.lProductFeature.Text = CTOS.productFeatures;


            this.lProductprice.Text = Presentation.Product.ProductPrice.getPrice(CTOS, Presentation.Product.PriceStyle.productpriceLarge);



            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "scriptName", "defaultprice=" + BTOS.Price.ToString() + ";", true);


            //for float div
            StringBuilder BTOSBuilder = new StringBuilder();
            BTOSBuilder.Append("<h3>Your System</h3><ul>");
            BTOSBuilder.AppendFormat(this.lProductprice.Text);

            foreach (BTOSConfig bc in BTOS.BTOSConfigs)
            {
                BTOSBuilder.AppendFormat("<li  id=\"btos-{0}\"><span class=\"btosCategory\">{1}</span> : <span class=\"btosSelectItem\">{2}</span></li>", bc.CategoryComponentID, bc.CategoryComponentDesc, bc.OptionComponentDesc);
            }
            BTOSBuilder.Append("</ul>");
            BTOSBuilder.AppendFormat(this.lProductprice.Text);


        }

        protected void bindFonts()
        {
            lbl_configItemsTitle.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Build_Your_System);
        }
    }
}