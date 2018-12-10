using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace eStore.Presentation
{
    public class StructuredDataMarkup
    {

        //Add HomePage Structured data
        public void GenerateHomePageStruturedData(String PageName, System.Web.UI.Page page)
        {
            StringBuilder sbStructuredData = new StringBuilder();
            try
            {
                switch (PageName)
                {
                    case "HomePage":

                        sbStructuredData.Append("{\"@context\": \"http://schema.org\",");
                        sbStructuredData.Append("\"@type\": \"WebSite\",");
                        sbStructuredData.AppendFormat("\"name\" : \"{0}\",", HttpUtility.JavaScriptStringEncode(Presentation.eStoreContext.Current.Store.profile.Title));
                        sbStructuredData.AppendFormat("\"url\" : \"{0}\",", HttpUtility.JavaScriptStringEncode(esUtilities.CommonHelper.GetStoreLocation()));
                        sbStructuredData.Append("\"potentialAction\": {");
                        sbStructuredData.Append("\"@type\": \"SearchAction\",");
                        sbStructuredData.AppendFormat("\"target\": \"{0}Search.aspx?", esUtilities.CommonHelper.GetStoreLocation());
                        sbStructuredData.Append("skey={search_term_string}&src=mc_google\", ");
                        sbStructuredData.Append("\"query-input\": \"required name=search_term_string\"}"); //讓google搜尋到時，產生一個網站內部的serach bar 與搜尋結果上
                       string SocialProfileLinks=Presentation.eStoreContext.Current.getStringSetting("SocialProfileLinks");
                       if (!string.IsNullOrEmpty(SocialProfileLinks))
                       {
                           string SocialProfileLinksMore = Presentation.eStoreContext.Current.getStringSetting("SocialProfileLinksMore");
                           string linkstring = string.Join("\",\"", (SocialProfileLinks + SocialProfileLinksMore).Split(';'));
                           sbStructuredData.AppendFormat(",\"sameAs\" : [\"{0}\"]", linkstring);
                       }
                       sbStructuredData.Append("}");
                    
                        break;
                    default:
                        break;
                }

                registerStructuredData(sbStructuredData.ToString(), page);
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add ProductStruturedData Failed", "", "", "", ex);
            }
        }

        //Add Proudct Page Structured data
        public void GenerateProductStruturedData(POCOS.Product product, System.Web.UI.Page page)
        {

            try
            {
                StringBuilder sbStructuredData = new StringBuilder();
                sbStructuredData.Append("{\"@context\": \"http://schema.org\",");
                sbStructuredData.Append("\"@type\": \"Product\",");
                sbStructuredData.AppendFormat("\"name\" : \"{0}\",", HttpUtility.JavaScriptStringEncode(product.name));
                sbStructuredData.AppendFormat("\"description\": \"{0}\",",  HttpUtility.JavaScriptStringEncode(product.productDescX));
                sbStructuredData.AppendFormat("\"image\": \"{0}\",", product.thumbnailImageX);
                sbStructuredData.AppendFormat("\"url\": \"{0}\"", Presentation.UrlRewriting.MappingUrl.getMappingUrl(product).Replace("~/", esUtilities.CommonHelper.GetStoreLocation()));
                string model = "";
                string brand = "";
                string sku = "";
                if (product is POCOS.Product_Ctos)
                {
                    sku = product.name;
                    brand = "Advantech";
                }
                else
                {
                    if (product.isMainStream())
                    {
                        model = product.ModelNo ;
                        sku = product.SProductID;
                        brand = "Advantech";
                    }
                    else if (product.isEPAPS())
                    {
                        if (product.specs != null)
                        {
                            sku = product.specs.Where(x => x.AttrName == "Manufacturer Part Number").Select(x => x.AttrValueName).FirstOrDefault();
                            brand = product.specs.Where(x => x.AttrName == "Manufacturer").Select(x => x.AttrValueName).FirstOrDefault();
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(model))
                {
                    sbStructuredData.AppendFormat(" ,\"model\":  \"{0}\"", model);
                }

                if (!string.IsNullOrWhiteSpace(sku))
                {
                    sbStructuredData.AppendFormat(" ,\"sku\":  \"{0}\"", sku);
                }
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    sbStructuredData.Append(",\"brand\": { \"@type\": \"Thing\", \"name\": \"");
                    sbStructuredData.Append(brand);
                    sbStructuredData.Append("\" }");
                }
             

                decimal price = product.getListingPrice().value;
                if (product.isOrderable()&& product.ShowPrice&& price > 0)
                {
                    sbStructuredData.Append(",\"offers\": {");
                    sbStructuredData.Append("\"@type\": \"Offer\",");
                    sbStructuredData.AppendFormat("\"price\": \"{0}\",", product.getListingPrice().value);
                    sbStructuredData.AppendFormat("\"priceCurrency\": \"{0}\",", Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID);
                    sbStructuredData.Append("\"itemCondition\": \"http://schema.org/NewCondition\", \"availability\": \"http://schema.org/InStock\", \"seller\": { \"@type\": \"Organization\", \"name\": \"");
                    sbStructuredData.Append("Advantech eStore");
                    sbStructuredData.Append("\" }");
                    sbStructuredData.Append("}");
                }

                sbStructuredData.Append("}");

                registerStructuredData(sbStructuredData.ToString(), page);
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add ProductStruturedData Failed", "", "", "", ex);
            }


        }

        //Alex:改用HttpUtility.JavaScriptStringEncode來處理特殊字元
        //private string escapeDoubleQuotes(string input)
        //{
            //string result = "";
            //if (input.Contains("\""))
            //{
                //result = input.Replace("\"", "\\\"");
            //}
            //else
            //{ result = input; }
            //return result;
        //}
        private void registerStructuredData(string StructuredData, System.Web.UI.Page page)
        {
            if (!string.IsNullOrEmpty(StructuredData))
            {
                //Generate JS Code in HTML
                HtmlGenericControl con = new HtmlGenericControl();
                con.TagName = "script";
                con.Attributes.Add("type", "application/ld+json");
                con.InnerHtml = StructuredData.ToString();
                page.Header.Controls.Add(con);
            }
        }

        //Add ProudctCategory Page Structured data
        public void GenerateProudctCategoryStruturedData(POCOS.ProductCategory productCategory, System.Web.UI.Page page)
        {
            try
            {
                StringBuilder sbStructuredData = new StringBuilder();
                sbStructuredData.Append("{\"@context\": \"http://schema.org\",");
                sbStructuredData.Append("\"@type\": \"WebPage\",");
                sbStructuredData.AppendFormat("\"name\": \"{0}\",", HttpUtility.JavaScriptStringEncode(productCategory.localCategoryNameX));
                sbStructuredData.AppendFormat("\"description\": \"{0}\",", HttpUtility.JavaScriptStringEncode(eStore.Presentation.eStoreGlobalResource.getLocalCategoryExtDescription(productCategory)));
                sbStructuredData.AppendFormat("\"url\": \"{0}\"", Presentation.UrlRewriting.MappingUrl.getMappingUrl(productCategory).Replace("~/", esUtilities.CommonHelper.GetStoreLocation()));
                sbStructuredData.Append("}");
                registerStructuredData(sbStructuredData.ToString(), page);
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add ProductStruturedData Failed", "", "", "", ex);
            }
        }

        //Add BreadcrumbList Structured data
        public void GenerateBreadcrumbStruturedData(object obj, System.Web.UI.Page page)
        {
            string key = string.Empty;
            try
            {     
                Dictionary<POCOS.ProductCategory, List<POCOS.ProductCategory>> parentCategoryGroup = new Dictionary<POCOS.ProductCategory, List<POCOS.ProductCategory>>();
                if (obj is POCOS.Product)
                {
                    POCOS.Product product = obj as POCOS.Product;
                    if (product.productCategories != null && product.productCategories.Any(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite))
                    {
                        foreach (var productCategory in product.productCategories.Where(x => x.MiniSite == Presentation.eStoreContext.Current.MiniSite))
                        {

                            List<POCOS.ProductCategory> ancestralCategories = new List<POCOS.ProductCategory>();
                            CreatAncestralCategories(productCategory, ref  ancestralCategories);

                            parentCategoryGroup.Add(productCategory, ancestralCategories);
                            key = product.SProductID;
                        }
                    }
                }
                else if (obj is POCOS.ProductCategory)
                {

                    POCOS.ProductCategory productCategory = obj as POCOS.ProductCategory;
                    List<POCOS.ProductCategory> ancestralCategories = new List<POCOS.ProductCategory>();
                    CreatAncestralCategories(productCategory, ref  ancestralCategories);

                    parentCategoryGroup.Add(productCategory, ancestralCategories);
                    key = productCategory.localCategoryNameX;                  
                }

                foreach (POCOS.ProductCategory productCategory in parentCategoryGroup.Keys)
                {
                    StringBuilder sbStructuredData = new StringBuilder();
                    sbStructuredData.Append("{\"@context\": \"http://schema.org\",");
                    sbStructuredData.Append("\"@type\": \"BreadcrumbList\",");
                    sbStructuredData.Append("\"itemListElement\":[");

                    List<POCOS.ProductCategory> ancestralCategories = parentCategoryGroup[productCategory];
                    for (int i = 0; i < ancestralCategories.Count; i++)
                    {
                        sbStructuredData.Append("{\"@type\": \"ListItem\",");
                        sbStructuredData.AppendFormat("\"position\": {0},", i + 1);
                        sbStructuredData.Append("\"item\":{");
                        sbStructuredData.AppendFormat("\"@id\": \"{0}\",", Presentation.UrlRewriting.MappingUrl.getMappingUrl(ancestralCategories[ancestralCategories.Count - i - 1]).Replace("~/", esUtilities.CommonHelper.GetStoreLocation()));
                        sbStructuredData.AppendFormat("\"name\": \"{0}\"", HttpUtility.JavaScriptStringEncode(ancestralCategories[ancestralCategories.Count - i - 1].localCategoryNameX));
                        sbStructuredData.Append((i == (ancestralCategories.Count - 1)) ? "}}" : "}},");
                    }

                    sbStructuredData.Append("]}");
                    registerStructuredData(sbStructuredData.ToString(), page);
                }
                
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add GenerateBreadcrumbStruturedData Failed" + key, "", "", "", ex);
            }
        }

        void CreatAncestralCategories(POCOS.ProductCategory category, ref List<POCOS.ProductCategory> ancestralCategories)
        {
            ancestralCategories.Add(category);
            //youAreHereList.Add(string.Format(" {0}"
            //          , esUtilities.CommonHelper.RemoveHtmlTags(eStore.Presentation.eStoreGlobalResource.getLocalCategoryName(category), true)));
            if (category.parentCategoryX != null)
            {
                CreatAncestralCategories(category.parentCategoryX, ref ancestralCategories);
            }
        }


        private void registerLPSections(string[] LPSections, System.Web.UI.Page page)
        {
            if (LPSections != null && LPSections.Any())
            {
                //Generate JS Code in HTML
                HtmlGenericControl con = new HtmlGenericControl();
                con.TagName = "script";
                con.Attributes.Add("type", "text/javascript");
                con.InnerHtml = "lpTag.section = [\"" + (string.Join("\",\"", LPSections)) + "\"]; ";
                page.Header.Controls.Add(con);
            }
        }
        public void GenerateLPSections(POCOS.ProductCategory productCategory, System.Web.UI.Page page)
        {
            try
            {
                if (!Presentation.eStoreContext.Current.getBooleanSetting("EnableLiveEngage", false))
                    return;


                string[] adsettings = Presentation.eStoreContext.Current.Store.getLiveEngageSections(productCategory);
                if (adsettings.Any())
                {
                    string[] LPSections = new string[adsettings.Length+2 ];
                    LPSections[0] = "estore";
                    LPSections[1] = productCategory.Storeid;
  
                    Array.Copy(adsettings,0, LPSections, adsettings.Length, adsettings.Length);
                    registerLPSections(LPSections, page);
                }
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add GenerateLPSections for ProductCategory Failed", "", "", "", ex);
            }
        }

        public void GenerateLPSections(POCOS.Product product, System.Web.UI.Page page)
        {
            try
            {
                if (!Presentation.eStoreContext.Current.getBooleanSetting("EnableLiveEngage", false))
                    return;

                

                string[] adsettings = Presentation.eStoreContext.Current.Store.getLiveEngageSections(product);
                if (adsettings.Any())
                {
                    string[] LPSections = new string[adsettings.Length + 2];
                    LPSections[0] = "estore";
                    LPSections[1] = product.StoreID;
                    Array.Copy(adsettings, 0, LPSections, adsettings.Length, adsettings.Length);

                    registerLPSections(LPSections, page);
                }

              
            }
            catch (Exception ex)
            {
                Utilities.eStoreLoger.Error("Add GenerateLPSections for ProductCategory Failed", "", "", "", ex);
            }
        }
    }
}

