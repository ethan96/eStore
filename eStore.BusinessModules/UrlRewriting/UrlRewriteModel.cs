
namespace eStore.BusinessModules.UrlRewrite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Configuration;
    using eStore.POCOS;
    using POCOS.DAL;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UrlRewriteModel
    {
        private static RewriteConfigHandler _config;
        public static RewriteConfigHandler config
        {
            get
            {
                if (_config == null)
                    _config = (RewriteConfigHandler)ConfigurationManager.GetSection("UrlRewriteRule");
                return _config;
            }
        }
        protected static bool EnableUrlRewriteRule
        {
            get
            {
                if (config == null)
                    return false;
                return config.Enabled();
            }
        }
        public static string getMappingUrl(object eStorePocoEntity, MiniSite minisite = null)
        {
            string mappingURL = null;
            string OriginalULR = null;

            if (eStorePocoEntity is POCOS.Product_Ctos)
            {
                Product_Ctos ctos = (Product_Ctos)eStorePocoEntity;
                OriginalULR = string.Format("~/Product/System.aspx?ProductID={0}", ctos.SProductID);
                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl("system", ctos.SEOPath1(minisite), ctos.SEOPath2(minisite), ctos.name, ctos.SProductID);
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;
            }
            else if (eStorePocoEntity is POCOS.Product_Bundle)
            {
                Product_Bundle ctos = (Product_Bundle)eStorePocoEntity;
                OriginalULR = string.Format("~/Product/Bundle.aspx?ProductID={0}", ctos.SProductID);
                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl("bundle", ctos.SEOPath1(minisite), ctos.SEOPath2(minisite), ctos.SProductID);
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;
            }
            else if (eStorePocoEntity is POCOS.Product)
            {
                POCOS.Product product = (POCOS.Product)eStorePocoEntity;
                OriginalULR = string.Format("~/Product/Product.aspx?ProductID={0}", System.Web.HttpUtility.UrlEncode(product.SProductID));

                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl(product.productType== Product.PRODUCTTYPE.APPMARKETPLACE ? "app": "model", product.SEOPath1(minisite), product.SEOPath2(minisite), product.SProductID);
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;
            }
            else if (eStorePocoEntity is POCOS.PStoreProduct)
            {
                POCOS.PStoreProduct product = (POCOS.PStoreProduct)eStorePocoEntity;
                OriginalULR = string.Format("~/CertifiedPeripherals/Product.aspx?ProductID={0}", System.Web.HttpUtility.UrlEncode(product.SProductID));

                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl("peripherals", product.productDescX, product.name, product.SProductID);
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;

            }
            else if (eStorePocoEntity is POCOS.Menu)
            {
                POCOS.Menu menu = (POCOS.Menu)eStorePocoEntity;
                mappingURL = getMappingUrl(menu);
            }
            else if (eStorePocoEntity is POCOS.ProductCategory)
            {
                POCOS.ProductCategory productCategory = (POCOS.ProductCategory)eStorePocoEntity;
                mappingURL = getMappingUrl(productCategory, minisite);
            }
            else if (eStorePocoEntity is POCOS.SolutionStoreTab)
            {
                POCOS.SolutionStoreTab solutionStoreTab = (POCOS.SolutionStoreTab)eStorePocoEntity;
                mappingURL = getMappingUrl(solutionStoreTab);
            }
            else if (eStorePocoEntity is POCOS.PStoreProductCategory)
            {
                POCOS.PStoreProductCategory productCategory = (POCOS.PStoreProductCategory)eStorePocoEntity;
                mappingURL = getMappingUrl(productCategory);

            }
            else if (eStorePocoEntity is POCOS.KitThemeType)
            {
                POCOS.KitThemeType themetype = (POCOS.KitThemeType)eStorePocoEntity;
                if (!string.IsNullOrEmpty(themetype.HyperLink))
                    return themetype.HyperLink;
                OriginalULR = string.Format("~/Kit/Theme.aspx?tid={0}", System.Web.HttpUtility.UrlEncode(themetype.Id.ToString()));
                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl("theme", themetype.KitTheme.Title, themetype.Title, themetype.Id.ToString());
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;
            }
            else if (eStorePocoEntity is POCOS.WidgetPage)
            {
                POCOS.WidgetPage widgetPage = (POCOS.WidgetPage)eStorePocoEntity;
                mappingURL = getMappingUrl(widgetPage);
            }
            return mappingURL;
        }

        public static string getMappingUrl(POCOS.Menu menu)
        {
            string mappingURL = null;
            string OriginalULR = null;
            if (menu.MenuType == null)
                mappingURL = "~/";
            else
                switch (menu.menuTypeX)
                {

                    case Menu.DataSource.CTOSCategory:
                    case Menu.DataSource.MinisiteCategory:
                    case Menu.DataSource.ApplicationCategory:
                    case Menu.DataSource.StandardCategory:
                        {

                            if (menu.productCategory != null)
                            {
                                mappingURL = getMappingUrl(menu.productCategory);
                            }
                            else
                            {
                                mappingURL = config.CreateMappedUrl("productsinrootcategory", menu.MenuName, menu.MenuName, menu.CategoryPath);
                            }
                            break;
                        }
                    case Menu.DataSource.CustomURL:
                        {
                            if (string.IsNullOrEmpty(menu.URL))
                                mappingURL = "~/";
                            else if (menu.URL[0] == '/')
                            {
                                mappingURL = menu.URL.Insert(0, "~");
                            }
                            else
                                mappingURL = menu.URL;
                            break;
                        }
                    case Menu.DataSource.WidgetPage:
                        {
                            if (string.IsNullOrEmpty(menu.CategoryPath))
                                mappingURL = "~/";
                            else
                            {
                                //OriginalULR = String.Format("~/Widget.aspx?WidgetID={0}", menu.CategoryPath.Trim());
                                //if (EnableUrlRewriteRule)
                                //{
                                //    if (menu.DisplayTypeX == Menu.RenderStyle.Solution)
                                //        mappingURL = config.CreateMappedUrl("solution", menu.MenuName, menu.MenuName, menu.CategoryPath);
                                //    else
                                //        mappingURL = config.CreateMappedUrl("widget", menu.MenuName, menu.MenuName, menu.CategoryPath);
                                //    if (string.IsNullOrEmpty(mappingURL))
                                //        mappingURL = OriginalULR;
                                //}
                                //else
                                //    mappingURL = OriginalULR;

                                //Alex 20171020: change menu widgetpage url format
                                OriginalULR = OriginalULR = String.Format("~/Widget.aspx?WidgetID={0}", menu.CategoryPath.Trim());
                                mappingURL = OriginalULR;
                                if (EnableUrlRewriteRule)
                                {
                                    int widgetPageId;
                                    bool isWidgetPageId = Int32.TryParse(menu.CategoryPath.Trim(), out widgetPageId);
                                    if (isWidgetPageId)
                                    {                                       
                                        WidgetPageHelper _help = new WidgetPageHelper();
                                        POCOS.WidgetPage widgetPage = _help.getWidgetPageByID(widgetPageId);
                                        if (widgetPage != null && !string.IsNullOrEmpty(widgetPage.SEOName1) && !string.IsNullOrEmpty(widgetPage.SEOName2))
                                        {
                                            if (menu.DisplayTypeX == Menu.RenderStyle.Solution)
                                                mappingURL = config.CreateMappedUrl("solution", widgetPage.SEOName1, widgetPage.SEOName2, widgetPage.WidgetPageID.ToString());
                                            else
                                                mappingURL = config.CreateMappedUrl("widget", widgetPage.SEOName1, widgetPage.SEOName2, widgetPage.WidgetPageID.ToString());

                                            if (string.IsNullOrEmpty(mappingURL))
                                                mappingURL = OriginalULR;
                                        }
                                    }

                                }
                                return mappingURL;

                            }
                            break;
                        }
                    case Menu.DataSource.PolicyCategoty:
                        {
                            if (menu.policyCategoty != null)
                            {
                                mappingURL = getMappingUrl(menu.policyCategoty);
                            }
                            break;
                        }
                    case Menu.DataSource.None:
                    default:
                        {
                            mappingURL = "~/";
                            break;
                        }

                }

            return mappingURL;
        }
        public static string getMappingUrl(POCOS.PolicyCategory policyCategory, MiniSite minisite = null)
        {
            string mappingURL = null;
            string OriginalULR = "~/Policy/Policy.aspx?pid=" + policyCategory.Id;
            if (!string.IsNullOrEmpty(policyCategory.CustomerURL))
            {

                if (policyCategory.CustomerURL[0] == '/')
                {
                    mappingURL = policyCategory.CustomerURL.Insert(0, "~");
                }
                else
                    mappingURL = policyCategory.CustomerURL;
            }
            else
            {
                mappingURL = config.CreateMappedUrl("article", policyCategory.Name, policyCategory.Id.ToString());
            }

            return mappingURL;
        }
        public static string getMappingUrl(POCOS.ProductCategory productCategory, MiniSite minisite = null)
        {
            string mappingURL = null;
            string OriginalULR = null;
            if (productCategory.DisplayTypeX == ProductCategory.RenderStyle.CustomURL && string.IsNullOrEmpty(productCategory.CustomURL) == false)
            {
                if (productCategory.CustomURL[0] == '/')
                {
                    mappingURL = productCategory.CustomURL.Insert(0, "~");
                }
                else
                    mappingURL = productCategory.CustomURL;
            }
            else if (minisite != null && (minisite.MiniSiteType == MiniSite.SiteType.IotMart || minisite.MiniSiteType == MiniSite.SiteType.UShop))
            {
                OriginalULR = string.Format("~/Product/IotModel.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                if (EnableUrlRewriteRule)
                    if (productCategory.parentCategoryX != null)
                    {
                        mappingURL = config.CreateMappedUrl("Iotpcm", productCategory.parentCategoryX.Keywords, productCategory.parentCategoryX.LocalCategoryName, productCategory.parentCategoryX.CategoryPath) + "#" + productCategory.CategoryPath;
                    }
                    else
                        mappingURL = config.CreateMappedUrl("Iotpcm", productCategory.Keywords, productCategory.LocalCategoryName, productCategory.CategoryPath);
                if (string.IsNullOrEmpty(mappingURL))
                    mappingURL = OriginalULR;
            }
            else if (productCategory.DisplayTypeX == ProductCategory.RenderStyle.ProductListWithModel)
            {
                mappingURL = string.Format("~/Product/ProductListWithModel.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                if (EnableUrlRewriteRule)
                {
                    mappingURL = config.CreateMappedUrl("pcm", productCategory.Keywords, productCategory.LocalCategoryName, String.IsNullOrEmpty(productCategory.CategoryPathSEO) ? productCategory.CategoryPath : productCategory.CategoryPathSEO);
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                else
                    mappingURL = OriginalULR;
            }
            else if (productCategory.DisplayTypeX == ProductCategory.RenderStyle.Promotions)
            {
                OriginalULR = string.Format("~/Product/ProductCategoryNew.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                if (EnableUrlRewriteRule)
                    mappingURL = config.CreateMappedUrl("categoryhomepage", productCategory.LocalCategoryName, String.IsNullOrEmpty(productCategory.CategoryPathSEO) ? productCategory.CategoryPath : productCategory.CategoryPathSEO);
                if (string.IsNullOrEmpty(mappingURL))
                    mappingURL = OriginalULR;
            }

            else
            {
                if (productCategory.dynamicCategoryType == ProductCategory.Category_Type.Application)
                {
                    if (productCategory.DisplayTypeX == ProductCategory.RenderStyle.Tabs)
                    {
                        OriginalULR = string.Format("~/Product/applicationsintabs.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                        if (EnableUrlRewriteRule)
                        {
                            mappingURL = config.CreateMappedUrl("appt", productCategory.Keywords, productCategory.LocalCategoryName, productCategory.CategoryPath);
                            if (string.IsNullOrEmpty(mappingURL))
                                mappingURL = OriginalULR;
                        }
                        else
                            mappingURL = OriginalULR;

                    }
                    else if (productCategory.parentCategoryX == null)
                    {

                        OriginalULR = string.Format("~/Product/applications.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                        if (EnableUrlRewriteRule)
                        {
                            mappingURL = config.CreateMappedUrl("app", productCategory.Keywords, productCategory.LocalCategoryName, productCategory.CategoryPath);
                            if (string.IsNullOrEmpty(mappingURL))
                                mappingURL = OriginalULR;
                        }
                        else
                            mappingURL = OriginalULR;
                    }
                    else if (productCategory.parentCategoryX != null && productCategory.parentCategoryX.parentCategoryX == null)
                    {
                        OriginalULR = string.Format("~/Product/applications.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.parentCategoryX.CategoryPath));
                        if (EnableUrlRewriteRule)
                        {
                            mappingURL = config.CreateMappedUrl("app", productCategory.Keywords, productCategory.LocalCategoryName, productCategory.parentCategoryX.CategoryPath);

                            if (string.IsNullOrEmpty(mappingURL))
                                mappingURL = OriginalULR;
                            else
                                mappingURL += ("#" + productCategory.CategoryID.ToString());
                        }
                        else
                            mappingURL = OriginalULR;
                    }

                    else
                    {
                        OriginalULR = string.Format("~/Product/SubCategory.aspx?category={0}", System.Web.HttpUtility.UrlEncode(productCategory.CategoryPath));
                        if (EnableUrlRewriteRule)
                        {
                            mappingURL = config.CreateMappedUrl("cs", productCategory.Keywords, productCategory.LocalCategoryName, productCategory.CategoryPath);
                            if (string.IsNullOrEmpty(mappingURL))
                                mappingURL = OriginalULR;
                        }
                        else
                            mappingURL = OriginalULR;
                    }
                }
                else
                {
                    OriginalULR = string.Format("~/Product/ProductCategoryV4.aspx?category={0}", System.Web.HttpUtility.UrlEncode(String.IsNullOrEmpty(productCategory.CategoryPathSEO) ? productCategory.CategoryPath : productCategory.CategoryPathSEO));
                    if (EnableUrlRewriteRule)
                    {
                        if (productCategory.parentCategoryX == null)
                        {
                            mappingURL = config.CreateMappedUrl("productsinrootcategorynew", productCategory.SEOPath1, String.IsNullOrEmpty(productCategory.CategoryPathSEO) ? productCategory.CategoryPath : productCategory.CategoryPathSEO, "");
                        }
                        else
                            mappingURL = config.CreateMappedUrl("productsinrootcategory", productCategory.SEOPath1, productCategory.SEOPath2, String.IsNullOrEmpty(productCategory.CategoryPathSEO) ? productCategory.CategoryPath : productCategory.CategoryPathSEO);
                        if (string.IsNullOrEmpty(mappingURL))
                            mappingURL = OriginalULR;
                    }
                    else
                        mappingURL = OriginalULR;
                }
            }

            return mappingURL;
        }

        private static Dictionary<int, POCOS.ProductCategory> getHierarchy(POCOS.ProductCategory category)
        {
            Dictionary<int, ProductCategory> categoryhierarchy = new Dictionary<int, ProductCategory>();
            int level = 0;
            if (category != null)
            {
                var parent = category;
                do
                {
                    categoryhierarchy.Add(level, parent);
                    level++;
                    parent = parent.parentCategoryX;
                }
                while (parent != null && level < 10);
            }
            return categoryhierarchy;
        }

        public static string getMappingUrl(POCOS.PStoreProductCategory productCategory)
        {
            string mappingURL = null;
            string OriginalULR = null;


            OriginalULR = string.Format("~/CertifiedPeripherals/ProductLine.aspx?category={0}", productCategory.Id);
            if (EnableUrlRewriteRule)
            {
                mappingURL = config.CreateMappedUrl("ppl", productCategory.Name, productCategory.DisplayName, productCategory.Id.ToString());
                if (string.IsNullOrEmpty(mappingURL))
                    mappingURL = OriginalULR;
            }
            else
                mappingURL = OriginalULR;

            return mappingURL;
        }

        public static string getMappingUrl(POCOS.SolutionStoreTab solutionStoreTab)
        {
            string mappingURL = null;
            if (solutionStoreTab != null)
                mappingURL = string.Format("~/SolutionStore.aspx?id={0}", System.Web.HttpUtility.UrlEncode(solutionStoreTab.CategoryPath));
            else
            {
                mappingURL = "~/";
            }

            return mappingURL;
        }

        public static string getMappingUrl(POCOS.WidgetPage widgetPage)
        {
            string OriginalULR = OriginalULR = String.Format("~/Widget.aspx?WidgetID={0}", widgetPage.WidgetPageID.ToString());
            string mappingURL = OriginalULR;
                           
            if (EnableUrlRewriteRule)
            {
                if (!string.IsNullOrEmpty(widgetPage.SEOName1) && !string.IsNullOrEmpty(widgetPage.SEOName2))
                {
                    mappingURL = config.CreateMappedUrl("widget", widgetPage.SEOName1, widgetPage.SEOName2, widgetPage.WidgetPageID.ToString());
                    if (string.IsNullOrEmpty(mappingURL))
                        mappingURL = OriginalULR;
                }
                           
            }
           
            return mappingURL;
        }
    }
}
