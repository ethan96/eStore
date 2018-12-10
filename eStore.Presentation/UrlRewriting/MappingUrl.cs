using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using eStore.POCOS;

namespace eStore.Presentation.UrlRewriting
{
    /// <summary>
    /// 静态function不能重新，只能覆盖。
    /// </summary>
    public class MappingUrl : BusinessModules.UrlRewrite.UrlRewriteModel
    {
        public static string getMappingUrl(string linkType, string Keyword, string Name, string ID1, string ID2 = null, string ID3 = null, string storeID = null)
        {
            string mappingURL = null;
            Regex oReg = default(Regex);
            oReg = new Regex("\\W");

            if (string.IsNullOrEmpty(Name) & string.IsNullOrEmpty(Keyword))
            {
                Name = "eppro";
                Keyword = "products";
            }
            else
            {
                if (string.IsNullOrEmpty(Keyword))
                    Keyword = Name;
                if (string.IsNullOrEmpty(Name))
                    Name = "product";
            }

            Keyword = Keyword.Trim();
            Name = Name.Trim();

            Keyword = System.Web.HttpUtility.UrlEncode(oReg.Replace(Keyword.Replace("&", "and"), "-").Replace("--", "-"));
            Name = System.Web.HttpUtility.UrlEncode(oReg.Replace(Name.Replace("&", "and"), "-").Replace("--", "-"));
            linkType = linkType.ToLower();

            switch (linkType)
            {
                case "configcategory":
                    mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".cc.htm";

                    break;
                case "configsub":
                    mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".cs.htm";

                    break;
                case "category":
                    mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".cs.htm";

                    break;
                case "subcategory":
                    mappingURL = "~/" + Keyword + "/" + Name + "/configure-" + ID1 + ".htm";

                    break;
                case "configure":
                    mappingURL = "~/" + Keyword + "/" + Name + "/configure-" + ID1 + ".htm";

                    break;
                case "system":
                    mappingURL = "~/" + Keyword + "/" + Name + "/system-" + ID1 + ".htm";

                    break;
                case "productcategory":
                    mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".pc.htm";

                    break;
                case "productsub":
                    if (string.IsNullOrEmpty(ID2))
                    {
                        mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".ps.htm";
                    }
                    else
                    {
                        mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + "/" + ID2 + ".pss.htm";
                    }

                    break;
                case "matrix":
                    if (string.IsNullOrEmpty(ID2))
                    {
                        mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + ".mx.htm";
                    }
                    else
                    {
                        mappingURL = "~/" + Keyword + "/" + Name + "/" + ID1 + "/" + ID2 + ".mxs.htm";
                    }

                    break;
                case "model":
                    mappingURL = "~/" + Keyword + "/" + Name + "/model-" + ID1 + ".htm";
                    break;
                case "landing":
                    mappingURL = "~/seo/" + ID1 + ".htm";
                    break;
                case "landing2":
                    mappingURL = "~/resources/" + storeID + "/" + ID1 + ".htm";
                    break;
                case "ctoslanding":
                    mappingURL = "~/resources/" + storeID + "/landing/" + ID1 + "_landing.htm";
                    break;
                case "solutions":
                    mappingURL = "~/ConfigSystems/SolutionStoreContent.aspx?id=" + Name.Replace("-", "_");
                    break;
                default:
                    mappingURL = linkType;

                    break;
            }


            return System.Web.VirtualPathUtility.ToAbsolute(mappingURL);
        }

        public static string getMappingUrl(object eStorePocoEntity, MiniSite minisite = null)
        {
            if (eStorePocoEntity == null)
                return "";
            if (eStorePocoEntity is POCOS.Product)
            {
                POCOS.Product product = (POCOS.Product)eStorePocoEntity;
                return getMappingUrl(product, minisite);
            }
            else if (eStorePocoEntity is POCOS.Menu)
            {
                POCOS.Menu menu = (POCOS.Menu)eStorePocoEntity;
                return getMappingUrl(menu);
            }
            else if (eStorePocoEntity is POCOS.ProductCategory)
            {
                POCOS.ProductCategory productCategory = (POCOS.ProductCategory)eStorePocoEntity;
                return getMappingUrl(productCategory, minisite);
            }
            return BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(eStorePocoEntity, minisite);
        }

        public static string getMappingUrl(POCOS.Menu menu)
        {
            if (!string.IsNullOrEmpty(menu.StoreUrl))
                return menu.StoreUrl;
            return eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(menu);
        }

        public static string getMappingUrl(POCOS.Product product, MiniSite minisite = null)
        {

            if (!string.IsNullOrEmpty(product.StoreUrl))
                return product.StoreUrl;
            return eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(product, minisite);
        }

        public static string getMappingUrl(POCOS.ProductCategory productCategory, MiniSite minisite = null)
        {
            if (!string.IsNullOrEmpty(productCategory.StoreUrl))
                return productCategory.StoreUrl;
            return eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(productCategory, minisite);
        }
        public static string getMappingUrl(POCOS.PolicyCategory policyCategory, MiniSite minisite = null)
        {
            if (!string.IsNullOrEmpty(policyCategory.StoreUrl))
                return policyCategory.StoreUrl;
            return eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(policyCategory, minisite);
        }
        public static string getMappingUrl(POCOS.WidgetPage widgetPage, MiniSite minisite = null)
        {
            if (!string.IsNullOrEmpty(widgetPage.StoreURL))
                return widgetPage.StoreURL;
            return eStore.BusinessModules.UrlRewrite.UrlRewriteModel.getMappingUrl(widgetPage, minisite);
        }

    }
}
