using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation.Product
{
    public class CTOS
    {
        private bool? _DisplayCTOSComponentThumbnail;
        private bool DisplayCTOSComponentThumbnail
        {
            get
            {
                if (_DisplayCTOSComponentThumbnail.HasValue == false)
                    _DisplayCTOSComponentThumbnail = Presentation.eStoreContext.Current.getBooleanSetting("DisplayCTOSComponentThumbnail", false);
                return _DisplayCTOSComponentThumbnail.GetValueOrDefault();
            }
        }
        public string composeBOMUI(POCOS.Product_Ctos ctos, BTOSystem btos, ref string navigator, bool showdetail = false)
        {
            StringBuilder SystemBuilder = new System.Text.StringBuilder();
          
          
            var categorygroup = (from com in ctos.components as List<CTOSBOM>
                                 group com by com.type into g
                                 select new
                                 {
                                     type = g.Key,
                                     components = g
                                 }).OrderBy(x => x.type).ToList();

            if (eStoreContext.Current.Store.storeID == "AUS")
            {
                List<CTOSBOM.COMPONENTTYPE> filter = null;
                filter = new List<CTOSBOM.COMPONENTTYPE>();
                filter.Add(CTOSBOM.COMPONENTTYPE.CATEGORY);
                filter.Add(CTOSBOM.COMPONENTTYPE.OPTION);
                filter.Add(CTOSBOM.COMPONENTTYPE.ADDONMODULES);
                filter.Add(CTOSBOM.COMPONENTTYPE.ACCESSORIES);
                categorygroup = categorygroup.Where(x => filter.Contains(x.type)).OrderBy(x => x.type).ToList();
            }

            if (categorygroup.Count() > 0)
            {
                StringBuilder sbnavigator = new StringBuilder();
                sbnavigator.Append(" <ul id=\"ConfigureSystemNavigator\" class=\"eStore_4Step_title\">");
                for (int i = 0; i < categorygroup.Count(); i++)
                {
                    string cgname = categorygroup[i].type.ToString(); ;
                    if (i == 0)
                    {
                        sbnavigator.AppendFormat("<li class=\"active\" ref=\"{0}\" style=\"margin-left: 0px; padding-left: 10px; background: none;\" ><span>{1}</span></li>"
                            , cgname, Presentation.eStoreContext.Current.Store.profile.getLocalizedValue("ConfigureSystemNavigator_" + cgname));
                    }
                    else
                    {
                        sbnavigator.AppendFormat("<li class=\"disable\" ref=\"{0}\" ><span>{1}</span></li>"
                          , cgname, Presentation.eStoreContext.Current.Store.profile.getLocalizedValue("ConfigureSystemNavigator_" + cgname));
                    }
                }
                sbnavigator.Append("</ul>");
                navigator = sbnavigator.ToString();
            }
            foreach (var cg in categorygroup)
            {
                StringBuilder CTOSBuilder = new System.Text.StringBuilder();
                foreach (CTOSBOM component in cg.components)
                {
                    if (!component.isExtentedFromProductCategory())
                        CTOSBuilder.Append(composeOptions(ctos, btos, component, showdetail));
                    else
                    {
                        bool valid = true;
                        StringBuilder compontHtml = new StringBuilder();
                        if (component.extendedCategories != null & component.extendedCategories.Count > 0)
                        {
                            int invalidtimes = 0;
                            foreach (ProductCategory child in component.extendedCategories)
                            {
                                bool childvalid = false;
                                string childHtml = composeExtendedComponent(component, ctos, btos, child, out childvalid, showdetail, true);
                                if (childvalid)
                                    compontHtml.AppendLine(childHtml);
                                else
                                    invalidtimes++;
                            }
                            if (invalidtimes == component.extendedCategories.Count)
                                valid = false;
                        }

                        if (valid)
                        {
                            CTOSBuilder.AppendLine(string.Format("<div class=\"module\" id=\"module-extended_header_{0}\"{1}>",
                            component.ComponentID
                             , (component.CTOSComponent == null || component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.None) ? string.Empty : "integritychecktype='" + component.CTOSComponent.integrityCheckTypeX.ToString() + "' "
                            ));
                            CTOSBuilder.AppendLine(String.Format("<div class=\"moduleheader extendedmoduleheader expoptionimg\" id=\"{0}\">{2}<span class=\"ctosCategory\">{1}</span></div>",
                            "extended_header" + component.ComponentID.ToString(),
                            component.name,
                            (!DisplayCTOSComponentThumbnail || component.CTOSComponent == null || string.IsNullOrEmpty(component.CTOSComponent.thumbnail)) ? string.Empty : string.Format("<div class=\"moduleThumbnail\"><img src=\"{0}\" alt=\"{1}\" /></div>", component.CTOSComponent.thumbnail, System.Web.HttpUtility.HtmlEncode( component.name)))
                            );

                            if (!string.IsNullOrEmpty(component.Message))
                            {
                                CTOSBuilder.AppendLine(string.Format("<p class=\"colorRed\">{0}</p>", component.Message));
                            }

                            CTOSBuilder.Append(compontHtml.ToString());
                            CTOSBuilder.AppendLine("</div>");
                        }
                    }
                }
                if (CTOSBuilder.Length > 0)
                {
                    SystemBuilder.Append(string.Format("<div id=\"{0}\">{1}</div>", cg.type.ToString(), CTOSBuilder.ToString()));
                }

            }
            return SystemBuilder.ToString();
        }

        public string composeExtendedComponent(CTOSBOM parentbom, POCOS.Product_Ctos ctos, BTOSystem btos, POCOS.ProductCategory category, out bool valid, bool showdetail = false, bool showcontent = false)
        {
            StringBuilder CTOSBuilder = new System.Text.StringBuilder();
            valid = true;
            if (category == null)
            {
                valid = false;
                return string.Empty;
            }

            CTOSBuilder.AppendLine(String.Format("<div class=\"extendedmodule{1}\" id=\"module-extended_{0}\">", category.CategoryID, showcontent ? string.Empty : " hiddenitem"));

            CTOSBuilder.AppendLine(String.Format("<div class=\"moduleheader extendedmoduleheader coloptionimg\" id=\"{0}\"><span class=\"ctosCategory\">{1}</span></div>",
                category.CategoryPath,
                category.LocalCategoryName
                ));

            if (category.childCategoriesX != null & category.childCategoriesX.Count > 0)
            {
                int invalidtimes = 0;
                foreach (ProductCategory child in category.childCategoriesX)
                {
                    bool childvalid = false;
                    string childHtml = composeExtendedComponent(parentbom, ctos, btos, child, out childvalid, showdetail);
                    if (childvalid)
                        CTOSBuilder.AppendLine(childHtml);
                    else
                        invalidtimes++;
                }
                if (invalidtimes == category.childCategoriesX.Count)
                    valid = false;
            }
            else if (category.ExtendedCategories.Any())
            {

                int invalidtimes = 0;
                foreach (ExtendedCategory ec in category.ExtendedCategories)
                {
                    bool childvalid = false;
                    string childHtml = createExtendedCategoryItems(parentbom, btos, category, out  childvalid, showdetail, ec);
                    if (childvalid)
                    {
                        CTOSBuilder.AppendLine(String.Format("<div class=\"extendedmodule{2}\" id=\"module-extended_{0}_{1}\">", category.CategoryID, ec.ID, " hiddenitem"));
                        CTOSBuilder.AppendLine(String.Format("<div class=\"moduleheader extendedmoduleheader coloptionimg\" id=\"{0}_{1}\"><span class=\"ctosCategory\">{2}</span></div>",
                           category.CategoryID,
                           ec.ID,
                            ec.Name
                            ));

                        CTOSBuilder.AppendLine(childHtml);
                        CTOSBuilder.AppendLine("</div>");
                    }
                    else
                        invalidtimes++;
                }
                if (invalidtimes == category.ExtendedCategories.Count)
                    valid = false;
            }
            else
            {
                CTOSBuilder.Append(createExtendedCategoryItems(parentbom, btos, category, out  valid, showdetail));
            }
            CTOSBuilder.AppendLine("</div>");
            return CTOSBuilder.ToString();
        }

        private string createExtendedCategoryItems(CTOSBOM parentbom, BTOSystem btos, POCOS.ProductCategory category, out bool valid, bool showdetail = false, ExtendedCategory extendedCategory = null)
        {
            StringBuilder CTOSBuilder = new StringBuilder();
            List<POCOS.BTOSConfig> configs = btos.BTOSConfigs.Where(x => x.OptionComponentID == category.CategoryID).ToList();
            List<POCOS.Product> qualifiedProducts = Presentation.Product.ProductResource.qualifyProductsByResource(category.productList, btos.parts);
            if (extendedCategory != null)
            {
                if (extendedCategory.productList.Any())
                {
                    qualifiedProducts = (from qp in qualifiedProducts
                                         from cp in extendedCategory.productList
                                         where qp.SProductID == cp.SProductID
                                         && qp.isOrderable()
                                         && qp.getListingPrice().value > 0
                                         select qp).ToList();
                }
                else
                {
                    valid = false;
                    return string.Empty;
                }
            }
            else if (qualifiedProducts != null &&qualifiedProducts.Any())
            {
                qualifiedProducts = (from qp in qualifiedProducts
                               
                                     where qp.isOrderable()
                                     && qp.getListingPrice().value > 0
                                     select qp).ToList();
            }
            if (qualifiedProducts == null || !qualifiedProducts.Any())
            {
                valid = false;
                return string.Empty;
            }
            else
                valid = true;
            foreach (POCOS.Product product in qualifiedProducts)
            {
                bool selected = configs.Any(x => x.BTOSConfigDetails.Any(d => d.SProductID == product.SProductID));
                StringBuilder sbparts = new StringBuilder();

                if (showdetail)
                {

                    sbparts.Append("<span class=\"colorRed\">[");
                    //foreach (POCOS.Part p in option.parts)
                    sbparts.Append(product.SProductID);
                    sbparts.Append(" ]</span>");
                }

                CTOSBuilder.AppendLine(String.Format("<div class=\"options hiddenitem\"><div  class=\"optionstext\"><input type=\"checkbox\" value=\"{0}\" {1}  name=\"extended_{2}\" addtion={3}/>",
                  product.SProductID
                    , (selected) ? "checked=\"checked\"" : ""
                    , category.CategoryID
                    , product.getListingPrice().value
                    ));


                string limitedSource = "";
                if (parentbom.Product_Ctos.isUseLimiteResource)
                    limitedSource = Presentation.Product.ProductResource.getJsonResourceSetting(product);
                int qty = 0;
                if (selected)
                    qty = configs.First(x => x.BTOSConfigDetails.Any(d => d.SProductID == product.SProductID)).Qty;
                CTOSBuilder.AppendLine(String.Format("<input type=\"text\" size=\"1\" id=\"{5}qty_{0}_{1}_{2}\" name=\"{5}qty_{0}_{1}_{2}\" class=\"qtytextbox\" value=\"{3}\" {4} >"
                    , parentbom.ID
                    , category.CategoryPath
                    , product.SProductID
                    , qty
                    , string.IsNullOrEmpty(limitedSource) ? "" : string.Format(" resource='{0}'", limitedSource)
                    , parentbom.type.ToString()
                    ));


                if (product.ProductResources.Any() && product.ProductResources.FirstOrDefault(x => x.ResourceType == "Datasheet") != null)
                {
                    string pdflink = product.ProductResources.FirstOrDefault(x => x.ResourceType == "Datasheet").ResourceURL;
                    CTOSBuilder.AppendLine(String.Format(""));
                    CTOSBuilder.AppendFormat("<a href=\"{0}\" Target=\"_blank\" title=\"\"><img src=\"{1}images/PDF_logo.jpg\" alt=\"download PDF\"/></a>"
                        , pdflink
                        , esUtilities.CommonHelper.GetStoreLocation()
                        );
                }

                CTOSBuilder.AppendLine(String.Format("<span class=\"optiondesc jTipProductDetail\" id=\"{0}\" name=\"{0}\">{1}</span> {2}</div><div  class=\"optionsprice\"> [ +<span class=\"addtionprice\" >{3}</span> ] </div></div>",
                    product.SProductID,
                product.productDescX,
                  sbparts.ToString(),
                 Presentation.Product.ProductPrice.FormartPriceWithDecimal(product.getListingPrice().value)));
            }
            return CTOSBuilder.ToString();
        }
        public string composeOptions(POCOS.Product_Ctos ctos, BTOSystem btos, POCOS.CTOSBOM component, bool showdetail = false)
        {

            StringBuilder CTOSBuilder = new System.Text.StringBuilder();

            if (component.Show)
            {
                Dictionary<int, decimal> warrantyitemsprice = new Dictionary<int, decimal>();
                if (component.isWarrantyType())
                {
                    POCOS.Price listprice = new Price(), markupprice = new Price();
                    ctos.getListingPrice(listprice, markupprice, btos);
                    if (!ctos.notAvailable && listprice.value > 0 && !ctos.checkIsBelowCost(btos))//if notAvailable or BelowCost, then do not show warranry list
                        warrantyitemsprice = ctos.getWarrantyItemsPrice(component, btos);
                    else
                        return string.Empty;
                }
                List<BTOSConfig> selectItems = ctos.getSelectedComponentOptions(btos, component);
                //if (selectItems.Count == 0)
                //    selectItems.Add(component.ChildComponents.FirstOrDefault());

                CTOSBuilder.AppendLine(String.Format("<div class=\"module\" id=\"module-{0}\"{1}>", component.ComponentID
                     , (component.CTOSComponent == null || component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.None) ? string.Empty : " integritychecktype='" + component.CTOSComponent.integrityCheckTypeX.ToString() + "' "   
                    ));

                string moduleThumbnail = string.Empty;
                if (DisplayCTOSComponentThumbnail && component.isMainComponent() && component.options.Count > 0)
                {
                    foreach (CTOSBOM option in component.options)
                    {
                        if (option.Defaults)
                        {
                            if (option.getUsePartList().Any())
                            {
                                moduleThumbnail = option.getUsePartList().First().thumbnailImageX;
                            }
                            break;
                        }
                    }
                }

                if (DisplayCTOSComponentThumbnail && string.IsNullOrEmpty(moduleThumbnail) && component.CTOSComponent != null && !string.IsNullOrEmpty(component.CTOSComponent.thumbnail))
                {
                    moduleThumbnail = component.CTOSComponent.thumbnail;
                }
                  
                CTOSBuilder.AppendLine(String.Format("<div class=\"moduleheader expoptionimg\">{3}<span class=\"ctosCategory\">{0}</span> : <span class=\"ctosSelectItem\">{1}</span>{2}</div>",
                                                                                    component.name,
                                                                                    (selectItems == null || selectItems.Count == 0
                                                                                    ? string.Empty
                                                                                    : string.Join(" | ", selectItems.Select(x => x.matchedOption.desc).ToArray())),
                                                                                    component.ComponentID == 2139 ? "<a class=\"eStoreHelper\" name=\"Product_Extended_warranty\" id=\"Product_Extended_warranty\">Extended Warranty Details </a>" : "",
                                                                                    (string.IsNullOrEmpty(moduleThumbnail) ? string.Empty : string.Format("<div class=\"moduleThumbnail\"><img src=\"{0}\" alt=\"{1}\" /></div>", moduleThumbnail,System.Web.HttpUtility.HtmlEncode( component.name)))
                                                                                    ));

                if (!string.IsNullOrEmpty(component.Message))
                {
                    CTOSBuilder.AppendLine(string.Format("<p class=\"colorRed\">{0}</p>", component.Message));
                }
                BTOSConfig selected = null;
                switch (component.InputType)
                {
                    case "Radio":
                        selected = selectItems.FirstOrDefault();
                        foreach (CTOSBOM option in component.options)
                        {
                            string limitedSource = "";
                            string isOSstyle = component.ComponentID == 21 ? "optionstextfull" : "optionstext";
                            var pic = option.priceX - ((selected == null || selected.matchedOption == null) ? 0 : selected.matchedOption.priceX);
                            if (option.Product_Ctos.isUseLimiteResource)
                                limitedSource = Presentation.Product.ProductResource.getJsonResourceSetting(option);
                            CTOSBuilder.AppendLine(String.Format("<div class=\"options\"><div  class=\"" + isOSstyle + "\"><input type=\"radio\" {0} value=\"{1}\" name=\"option_{2}\" addtion={3}{4}{5} />",
                                (selected != null && selected.matchedOption != null && selected.matchedOption.ID == option.ID) ? "checked=\"checked\"" : ""
                                , option.ID
                                , component.ID
                                , component.ComponentID == 21 || component.isWarrantyType() ? "0" :
                                    eStore.Utilities.Converter.CartPriceRound(Presentation.Product.ProductPrice.getSitePrice(pic).value, Presentation.eStoreContext.Current.Store.storeID).ToString()
                                , string.IsNullOrEmpty(limitedSource) ? "" : string.Format(" resource='{0}'", limitedSource)
                                ,((component.CTOSComponent ==null ||
                                    !(component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.Storage || component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.OS)
                                    || option.componentDetails.Any()) ? "" : " nonoption='true'")
                                    +(component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.SoftwareSubscription?$"{(option.name=="None"?"": " data-quoteonly='true'")} data-component='{option.ComponentID}'":"")
                                ));

                            CTOSBuilder.AppendLine(string.Format("<span class=\"optiondesc\" >{0}</span>"
                                , option.name));
                            if (showdetail && option.componentDetails != null && option.componentDetails.Count > 0)
                            {

                                CTOSBuilder.Append("<span class=\"colorRed\"> [");
                                //foreach (POCOS.Part p in option.parts)
                                CTOSBuilder.Append(string.Join(" | ", option.componentDetails.Select(x => x.SProductID + " X " + x.qty).ToArray()));
                                CTOSBuilder.Append(" ]</span>");
                            }
                            CTOSBuilder.Append("</div>");
                            if (component.ComponentID == 21)
                            { CTOSBuilder.Append("</div>"); }

                            else
                            {
                                decimal sum = 0m;
                                if (component.isWarrantyType())
                                {
                                    sum = warrantyitemsprice[option.ID] - ((selected == null || selected.matchedOption == null) ? 0 : warrantyitemsprice[selected.matchedOption.ID]);
                                }
                                else
                                {
                                    sum = option.priceX - ((selected == null || selected.matchedOption == null) ? 0 : selected.matchedOption.priceX);
                                }

                                string sumSign = "";
                                if (sum >= 0)
                                    sumSign = "+";
                                else if (sum < 0)
                                    sumSign = "-";
                                CTOSBuilder.AppendLine(string.Format("<div  class=\"optionsprice\"> [ <span class=\"priceSing\">{2}</span>{0}<span class=\"addtionprice\" >{1:n}</span> ]</div></div>"
                                    , Presentation.eStoreContext.Current.CurrentCurrency == null ? Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign : Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                                 , Presentation.Product.ProductPrice.getSitePrice(eStore.Utilities.Converter.CartPriceRound(sum, Presentation.eStoreContext.Current.Store.storeID)).valueWithoutCurrency
                                 , sumSign));
                            }


                        }



                        break;

                    case "Select":
                        if (component.options.Count > 0)
                        {
                            CTOSBuilder.AppendLine(String.Format("<div class=\"options\"><select name=\"option_{0}\" >", component.ID));
                            selected = selectItems.FirstOrDefault();
                            foreach (CTOSBOM option in component.options)
                            {
                                string limitedSource = "";
                                string optiondesc = "";
                                if (option.Product_Ctos.isUseLimiteResource)
                                    limitedSource = Presentation.Product.ProductResource.getJsonResourceSetting(option);
                                CTOSBuilder.AppendLine(String.Format("<option value=\"{0}\" addtion=\"{5}\" {1} optiondesc=\"$.optiondesc\" currency=\"{6}\">{2}{3}{4}",
                                    option.ID,
                                    (selected != null && selected.matchedOption != null && selected.matchedOption.ID == option.ID) ? "selected=\"selected\"" : "",
                                    option.name,
                                    string.IsNullOrEmpty(limitedSource) ? "" : string.Format(" resource='{0}'", limitedSource)
                                    , (component.CTOSComponent == null ||
                                    !(component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.Storage || component.CTOSComponent.integrityCheckTypeX == CTOSComponent.IntegrityCheckType.OS)
                                    || option.componentDetails.Any()) ? "" : " nonoption='true'"
                                    , component.ComponentID == 21 || component.isWarrantyType() ? option.priceX + "%" :
                                        eStore.Utilities.Converter.CartPriceRound(Presentation.Product.ProductPrice.getSitePrice(option.priceX).value, Presentation.eStoreContext.Current.Store.storeID).ToString()
                                    , Presentation.eStoreContext.Current.CurrentCurrency == null ? Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign : Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                                    ));
                                optiondesc += option.name;

                                if (showdetail && option.componentDetails != null && option.componentDetails.Count > 0)
                                {
                                    CTOSBuilder.Append(" [");
                                    CTOSBuilder.Append(string.Join(" | ", option.componentDetails.Select(x => x.SProductID + " x " + x.qty).ToArray()));
                                    CTOSBuilder.Append("] ");
                                    optiondesc += " [" + string.Join(" | ", option.componentDetails.Select(x => x.SProductID + " x " + x.qty).ToArray()) + "] ";
                                }


                                if (component.ComponentID == 21)
                                { CTOSBuilder.Append("</option>"); }
                                else if (component.isWarrantyType())
                                {
                                    CTOSBuilder.AppendLine(string.Format(" [ {0}% ]</option>"
                                    , option.priceX));
                                    optiondesc += string.Format(" [ {0}% ]</option>", option.priceX);
                                }
                                else
                                {
                                    decimal sum = option.priceX - ((selected == null || selected.matchedOption == null) ? 0 : selected.matchedOption.priceX);
                                    string sumSign = "";
                                    if (sum >= 0)
                                        sumSign = "+";
                                    else if (sum < 0)
                                        sumSign = "-";
                                    CTOSBuilder.AppendLine(string.Format(" [ <span class=\"priceSing\">{2}</span>{0}<span class=\"addtionprice\" >{1:n}</span> ]</option>"
                                     , Presentation.eStoreContext.Current.CurrentCurrency == null ? Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign : Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign
                                     , Presentation.Product.ProductPrice.getSitePrice(eStore.Utilities.Converter.CartPriceRound(sum, Presentation.eStoreContext.Current.Store.storeID)).valueWithoutCurrency
                                     , sumSign));
                                }
                                CTOSBuilder.Replace("$.optiondesc", optiondesc.Replace("\"","$.'"));
                            }

                            CTOSBuilder.AppendLine("</select></div>");
                        }
                        break;

                    case "Multiselect":
                        foreach (CTOSBOM option in component.options)
                        {
                            selected = selectItems.FirstOrDefault(x => x.matchedOption.ID == option.ID);
                            StringBuilder sbparts = new StringBuilder();

                            if (showdetail && option.componentDetails != null && option.componentDetails.Count > 0)
                            {

                                sbparts.Append("<span class=\"colorRed\">[");
                                //foreach (POCOS.Part p in option.parts)
                                sbparts.Append(string.Join(" | ", option.componentDetails.Select(x => x.SProductID + " X " + x.qty).ToArray()));
                                sbparts.Append(" ]</span>");
                            }

                            CTOSBuilder.AppendLine(String.Format("<div class=\"options\"><div  class=\"optionstextFull\"><input type=\"checkbox\" value=\"{0}\" {1}  name=\"option_{2}\" addtion=\"{3}\" />",
                                option.ID
                                , (selected != null) ? "checked=\"checked\"" : "", component.ID
                                , component.isWarrantyType() ? option.priceX + "%" :
                                    eStore.Utilities.Converter.CartPriceRound(Presentation.Product.ProductPrice.getSitePrice(option.priceX).value
                                        , Presentation.eStoreContext.Current.Store.storeID).ToString()
                                ));

                            if (String.IsNullOrEmpty(option.getPartList()))
                            {

                                CTOSBuilder.AppendLine(String.Format("<input type=\"text\" disabled=\"disabled\" size=\"1\" id=\"ItemQty_{0}_{1}\" name=\"ItemQty_{0}_{1}\" class=\"qtytextbox\" value=\"{2}\">", component.ID, option.ID, 0));
                            }
                            else
                            {
                                string limitedSource = "";
                                if (option.Product_Ctos.isUseLimiteResource)
                                    limitedSource = Presentation.Product.ProductResource.getJsonResourceSetting(option);
                                CTOSBuilder.AppendLine(String.Format("<input type=\"text\" size=\"1\" id=\"ItemQty_{0}_{1}\" name=\"ItemQty_{0}_{1}\" class=\"qtytextbox\" value=\"{2}\" {3} >"
                                    , component.ID
                                    , option.ID
                                    , (selected == null || selected.matchedOption == null) ? 0 : selected.Qty
                                    , string.IsNullOrEmpty(limitedSource) ? "" : string.Format(" resource='{0}'", limitedSource)
                                    ));
                            }
                            //updated for checkbox price style by Jack.Xu on 20110916
                            decimal sum = option.priceX - ((selected == null || selected.matchedOption == null) ? 0 : selected.matchedOption.priceX);
                            string sumSign = "";
                            if (sum >= 0)
                                sumSign = "+";
                            else if (sum < 0)
                                sumSign = "-";
                            CTOSBuilder.AppendLine(String.Format("<span class=\"optiondesc\" >{0}</span> {4} [ {2}{3}<span class=\"addtionprice\" >{1}</span> ] </div></div>",
                              option.name,
                              component.isWarrantyType() ? option.priceX + "%" :
                                    Presentation.Product.ProductPrice.getSitePrice(eStore.Utilities.Converter.CartPriceRound(sum, Presentation.eStoreContext.Current.Store.storeID)).valueWithoutCurrency,
                              sumSign,
                              Presentation.eStoreContext.Current.CurrentCurrency == null ? Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign : Presentation.eStoreContext.Current.CurrentCurrency.CurrencySign,
                              sbparts.ToString()
                            ));
                        }

                        break;

                    default:
                        break;
                }
                CTOSBuilder.AppendLine("</div>");
            }
            return CTOSBuilder.ToString();
        }

        public BTOSystem getBTOSbyParameters(POCOS.Product_Ctos ctos, string parameters)
        {


            string[] para = parameters.Split(';');

            Dictionary<string, Dictionary<string, int>> dictpara = new Dictionary<string, Dictionary<string, int>>();
            foreach (string parakeyvalue in para)
            {
                string[] keyvalue = parakeyvalue.Split(':');
                if (!dictpara.ContainsKey(keyvalue[0]))
                {
                    string[] keyProAndQty = keyvalue[1].Split(',');
                    Dictionary<string, int> dictpro = new Dictionary<string, int>();
                    foreach (var cc in keyProAndQty)
                    {
                        if (!string.IsNullOrWhiteSpace(cc))
                        {
                            int qty = 1;
                            string[] proANDqty = cc.Split('|');
                            if (!int.TryParse(proANDqty[1], out qty))
                                qty = 1;
                            dictpro.Add(proANDqty[0], qty);
                        }
                    }
                    dictpara.Add(keyvalue[0], dictpro);
                }
            }
            BTOSystem newbtos = ctos.getDefaultBTOS();
            newbtos.clear();
            CTOSBOM newoption;
            foreach (CTOSBOM component in ctos.components)
            {

                Dictionary<string, int> optionsid = dictpara[component.ComponentID.ToString()];
                if (optionsid != null)
                {
                    foreach (var optionid in optionsid)
                    {
                        if (!string.IsNullOrEmpty(optionid.Key))
                        {
                            newoption = new CTOSBOM();
                            newoption = ctos.findOption(component.ID, int.Parse(optionid.Key));
                            newbtos.addItem(component, newoption, optionid.Value);
                        }
                    }
                }

            }
            return newbtos;

        }

        /// <summary>
        /// get btos by user settings
        /// </summary>
        /// <param name="ctos"></param>
        /// <param name="para"></param>
        /// <param name="currentBTOS"></param>
        /// <returns></returns>
        public BTOSystem updateBTOS(Product_Ctos ctos, System.Collections.Specialized.NameValueCollection para, Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>> addons = null)
        {

            if (ctos != null)
            {
                BTOSystem currentBTOS = ctos.getDefaultBTOS();
                currentBTOS.clear();
                CTOSBOM newoption;
                foreach (CTOSBOM component in ctos.components)
                {
                    if (!component.isExtentedFromProductCategory())
                    {
                        string[] optionsid = para.GetValues("option_" + component.ID);
                        if (optionsid != null)
                        {
                            foreach (string optionid in optionsid)
                            {
                                newoption = new CTOSBOM();
                                newoption = ctos.findOption(component.ID, int.Parse(optionid));
                                int qty = 0;
                                string qtyStr = para.Get(string.Format("ItemQty_{0}_{1}", component.ID, optionid));
                                if (!int.TryParse(qtyStr, out qty))
                                    qty = 1;
                                if (addons != null && component.isAddon())
                                {
                                    add2Addons(ref addons, component.type, newoption, qty);
                                }
                                else
                                {
                                    BTOSConfig config = currentBTOS.addItem(component, newoption, qty);
                                    if (component.isAddon() && config != null && config.BTOSConfigDetails.Any())//extended warranty will be added at cart
                                    {
                                        foreach (var detail in config.BTOSConfigDetails)
                                        {
                                            detail.Warrantable = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string[] optionsid = para.Cast<string>().Where(x => x.StartsWith(component.type.ToString() + "qty_" + component.ID.ToString())).ToArray();
                        if (optionsid != null)
                        {
                            foreach (string optionid in optionsid)
                            {
                                try
                                {
                                    int qty = 0;
                                    string qtyStr = para.Get(optionid);
                                    if (string.IsNullOrEmpty(qtyStr) || qtyStr == "0")
                                        continue;
                                    if (!int.TryParse(qtyStr, out qty) || qty == 0)
                                        continue;
                                    int idspliter = optionid.LastIndexOf('_');
                                    int prefixLength = (component.type.ToString() + "qty_" + component.ID.ToString()).Length;
                                    if (idspliter > prefixLength + 1)
                                    {
                                        string categoryid = optionid.Substring(prefixLength + 1, idspliter - prefixLength - 1);
                                        string productid = optionid.Substring(idspliter + 1);
                                        POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(productid);
                                        POCOS.ProductCategory category = Presentation.eStoreContext.Current.Store.getProductCategory(categoryid);
                                        newoption = new CTOSBOM();
                                        if (addons != null && component.isAddon())
                                        {
                                            add2Addons(ref addons, component.type, product, qty);
                                        }
                                        else
                                        {
                                            BTOSConfig config = currentBTOS.addNoneCTOSItem(product, qty, 0, 1, category);
                                            if (component.isAddon() && config != null && config.BTOSConfigDetails.Any())
                                            {
                                                foreach (var detail in config.BTOSConfigDetails)
                                                {
                                                    detail.Warrantable = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {


                                }
                            }
                        }
                    }
                }
                return currentBTOS;
            }
            else
            {

                return null;
            }
        }

        private void add2Addons(ref Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>> addons, CTOSBOM.COMPONENTTYPE type, CTOSBOM option, int qty)
        {
            if (addons == null || option == null || qty == 0)
                return;
            if (option.componentDetails.Any())
            {
                if (option.componentDetails.Count == 1)
                {
                    CTOSBOMComponentDetail detail = option.componentDetails.First();
                    add2Addons(ref addons, type, detail.part, detail.qty * qty);
                }
                else
                {
                    Part _paddons = Presentation.eStoreContext.Current.Store.getPart("Addons");
                    if (_paddons is Product_Bundle)
                    {

                        Product_Bundle productBundle = ((Product_Bundle)_paddons).clone(option.ID);
                        productBundle.ProductDesc = option.desc;
                        productBundle.DisplayPartno = option.ParentComponent.desc;
                        productBundle.sourceTemplateID = option.ID;
                        foreach (CTOSBOMComponentDetail detail in option.componentDetails)
                        {
                            productBundle.addProductBundleItem(detail.part, detail.qty);
                        }
                        productBundle.initialize();

                        //update product bundle value
                        Bundle bundle = productBundle.bundle;
                        productBundle.priceSource = Part.PRICESOURCE.LOCAL;
                        productBundle.VendorSuggestedPrice = bundle.originalPrice;
                        productBundle.StorePrice = productBundle.VendorSuggestedPrice.GetValueOrDefault();

                        add2Addons(ref addons, type, productBundle, qty);
                    }
                }
            }

        }
        private void add2Addons(ref Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>> addons, CTOSBOM.COMPONENTTYPE type, Part part, int qty)
        {
            if (addons == null || part == null || qty == 0)
                return;
            Dictionary<Part, int> addon = null;
            if (addons.ContainsKey(type))
                addon = addons[type];
            else
            {
                addon = new Dictionary<Part, int>();
                addons.Add(type, addon);
            }
            if (addon.ContainsKey(part))
            {
                addon[part] += qty;
            }
            else
                addon.Add(part, qty);
        }
        /// <summary>
        /// get btos from added cartItems
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="item"></param>
        /// <param name="ctos"></param>
        /// <returns></returns>
        public BTOSystem reconfigFromCart(Cart cart, CartItem item, Product_Ctos ctos)
        {
            if (cart == null || item == null || ctos == null)
                return null;
            BTOSystem btos = item.btosX.clone();
            if (cart.cartItemsX.Any(x => x.RelatedItem == item.ItemNo))// if has RelatedItem
            {
                foreach (CartItem ci in cart.cartItemsX.Where(x => x.RelatedItem == item.ItemNo))
                {
                    foreach (CTOSBOM component in ctos.components)
                    {
                        if (component.type == ci.relatedTypeX)
                        {


                            if (!component.isExtentedFromProductCategory())
                            {
                                foreach (CTOSBOM option in component.options)
                                {
                                    if (ci.type == POCOS.Product.PRODUCTTYPE.BUNDLE)//if option has more than one details, will compose as bundle
                                    {
                                        int bicount = 0;
                                        foreach (BundleItem bi in ci.bundleX.BundleItems)
                                        {
                                            if (option.componentDetails.Any(x => x.SProductID == bi.ItemSProductID && x.qty == bi.Qty))
                                            {
                                                bicount++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        if (bicount == ci.bundleX.BundleItems.Count())
                                        {
                                            BTOSConfig config = btos.addItem(component, option, ci.Qty);
                                         
                                            if (component.isAddon() && config != null && config.BTOSConfigDetails.Any())
                                            {
                                                foreach (var detail in config.BTOSConfigDetails)
                                                {
                                                    detail.Warrantable = false;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (option.componentDetails.Any(x => x.SProductID == ci.SProductID && x.qty == ci.Qty))
                                        {
                                            BTOSConfig config = btos.addItem(component, option, ci.Qty);
                                            if (component.isAddon() && config != null && config.BTOSConfigDetails.Any())
                                            {
                                                foreach (var detail in config.BTOSConfigDetails)
                                                {
                                                    detail.Warrantable = false;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (ProductCategory pc in component.extendedCategories)
                                {
                                    POCOS.Product product = pc.productList.FirstOrDefault(x => x.SProductID == ci.SProductID);
                                    if (product != null)
                                    {
                                        BTOSConfig config = btos.addNoneCTOSItem(product, ci.Qty, 0, 1, pc);
                                        if (component.isAddon() && config != null && config.BTOSConfigDetails.Any())
                                        {
                                            foreach (var detail in config.BTOSConfigDetails)
                                            {
                                                detail.Warrantable = false;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                      
                        }
                    }
                }
            }
            return btos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="ctos"></param>
        /// <param name="para"></param>
        /// <param name="currentBTOS"></param>
        /// <returns></returns>
        public CartItem Add2Cart(Cart cart, Product_Ctos ctos, System.Collections.Specialized.NameValueCollection para)
        {
            if (cart == null || ctos == null)
                return null;
            Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>> addons = new Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>>();
            BTOSystem newbtos = updateBTOS(ctos, para, addons);
            ctos.updateBTOSPrice(newbtos);
            CartItem cartitem = cart.addItem(ctos, 1, newbtos);
            foreach (var addontype in addons)
            {
                foreach (var addon in addontype.Value)
                {
                    CartItem addonCartItem = cart.addItem(addon.Key, addon.Value);
                    if (addonCartItem != null)
                    {
                        addonCartItem.relatedTypeX = addontype.Key;
                        addonCartItem.RelatedItem = cartitem.ItemNo;
                    }
                }
            }
            return cartitem;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="item"></param>
        /// <param name="ctos"></param>
        /// <param name="para"></param>
        /// <param name="currentBTOS"></param>
        /// <returns></returns>
        public CartItem updateCart(Cart cart, CartItem item, Product_Ctos ctos, System.Collections.Specialized.NameValueCollection para)
        {
            if (cart == null || item == null || ctos == null)
                return null;

            Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>> addons = new Dictionary<CTOSBOM.COMPONENTTYPE, Dictionary<Part, int>>();
            BTOSystem newbtos = updateBTOS(ctos, para, addons);
            ctos.updateBTOSPrice(newbtos);
            item.BTOSystem = newbtos;
            cart.updateItem(item);
            var deleteItems = (from ci in cart.cartItemsX
                               where ci.RelatedItem.HasValue && ci.RelatedItem == item.ItemNo //find RelatedItem
                               && !(
                               addons.Any(x => x.Key == ci.relatedTypeX
                               && (ci.type == POCOS.Product.PRODUCTTYPE.BUNDLE ?
                               x.Value.Any(y => y.Key is POCOS.Product_Bundle && ((POCOS.Product_Bundle)y.Key).bundle.SourceTemplateID == ci.bundleX.SourceTemplateID)
                               : x.Value.Any(y => y.Key.SProductID == ci.SProductID))))
                               select ci);

            foreach (var di in deleteItems)
                cart.CartItems.Remove(di);

            //cart.cartItemsX.Where(x=>x.RelatedItem==item.ItemNo && x.relatedTypeX==

            //delete

            //new 

            //edit

            foreach (var addontype in addons)
            {
                foreach (var addon in addontype.Value)
                {
                    var existsItem = (from ci in cart.cartItemsX
                                      where ci.RelatedItem.HasValue && ci.RelatedItem == item.ItemNo //find RelatedItem
                                      && ci.relatedTypeX == addontype.Key
                                      && (ci.type == POCOS.Product.PRODUCTTYPE.BUNDLE && addon.Key is POCOS.Product_Bundle?
                                      ((POCOS.Product_Bundle)addon.Key).bundle.SourceTemplateID == ci.bundleX.SourceTemplateID
                                      : addon.Key.SProductID == ci.SProductID)
                                      select new
                                      {
                                          cartitem = ci
                                      ,
                                          addon = addon
                                      }).FirstOrDefault();
                    if (existsItem != null)
                    {
                        existsItem.cartitem.updateQty(existsItem.addon.Value);
                    }
                    else
                    {
                        CartItem addonCartItem = cart.addItem(addon.Key, addon.Value);
                        addonCartItem.relatedTypeX = addontype.Key;
                        addonCartItem.RelatedItem = item.ItemNo;
                    }
                }
            }

            return item;
        }
    }


}
