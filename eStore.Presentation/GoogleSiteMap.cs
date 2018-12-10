using eStore.POCOS;
using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace eStore.Presentation
{
    public class GoogleSiteMap
    {
        private String NameSpaceStr = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private XmlDocument siteMapdocument;
        private string storeurl;
        string Protocol;
        public string getSiteMaps(eStore.BusinessModules.Store store)
        {
            storeurl = store.profile.StoreURL;
            siteMapdocument = new XmlDocument();
            XmlDeclaration xmlDecl;
            XmlNode RootNode;
            xmlDecl = siteMapdocument.CreateXmlDeclaration("1.0", "utf-8", null);
            siteMapdocument.AppendChild(xmlDecl);
            RootNode = siteMapdocument.CreateElement(string.Empty, "urlset", NameSpaceStr);
            siteMapdocument.AppendChild(RootNode);
            //add home page
            Protocol = store.profile.getBooleanSetting("IsSecureEntireSite", false) ? "https" : "http";

            RootNode.AppendChild(getURLNode(Protocol+"://" + storeurl, "0.9"));
            //add menus
            foreach (POCOS.Menu menu in store.getMenuItems(null))
            {
                AddMenuURLNode(RootNode, menu);
            }


            //add standard category and products
            foreach (eStore.POCOS.ProductCategory pc in store.getTopLevelStandardProductCategories(null))
            {
                AddCategoryURLNode(RootNode, pc);
            }

            //add ctos category and products
            foreach (eStore.POCOS.ProductCategory pc in store.getTopLevelCTOSProductCategories(null))
            {
                AddCategoryURLNode(RootNode, pc);
            }

            WidgetPageHelper helper = new WidgetPageHelper();
            List<WidgetPage> widgetPages = null;
            widgetPages = helper.getWidgetsByStore(store.storeID );

            foreach (eStore.POCOS.WidgetPage widgetpage in widgetPages.Where(x=>x.Publish))
            {
                string url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(widgetpage);
                if (!string.IsNullOrEmpty(url))
                {
                    url = url.Replace("~", "");
                    RootNode.AppendChild(getURLNode(url, "0.8"));
                }
            }

            foreach (POCOS.Menu footMenu in store.getFooterLinks(null).Take(3).ToList())
            {
                foreach (POCOS.Menu leftRightItem in footMenu.subMenusX)
                {
                    AddFooterMenuURLNode(RootNode, leftRightItem);
                }
            }

            //add pstore url
            eStore.BusinessModules.PStore pstore = new eStore.BusinessModules.PStore(store.profile);
            List<POCOS.PStoreProductCategory> pstoreCategoryList = pstore.getTopLevelPStoreCategory();
            foreach (POCOS.PStoreProductCategory ppc in pstoreCategoryList)
            {
                //add pstore cateogry url
                string catetgoryUrl = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(ppc as Object);
                if (!string.IsNullOrEmpty(catetgoryUrl))
                {
                    catetgoryUrl = Protocol + "://" + storeurl + catetgoryUrl.Replace("~", "");
                    RootNode.AppendChild(getURLNode(catetgoryUrl, "0.8"));
                }
                //add pstore product url
                foreach (POCOS.PStoreProduct item in ppc.productList)
                {
                    string productUrl = eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(item);
                    if (!string.IsNullOrEmpty(productUrl))
                    {
                        productUrl = Protocol + "://" + storeurl + productUrl.Replace("~", "");
                        RootNode.AppendChild(getURLNode(productUrl, "0.8"));
                    }
                }
            }

            foreach (POCOS.Advertisement advItem in store.getAdvByStoreAndType(POCOS.Advertisement.AdvertisementType.Resources))
            {
                if (advItem != null && advItem.complexAdvertisementContent != null
                        && advItem.complexAdvertisementContent is POCOS.Advertisement.ResourcesContent)
                {
                    POCOS.Advertisement.ResourcesContent resource = (POCOS.Advertisement.ResourcesContent)advItem.complexAdvertisementContent;
                    List<int> cmsMapList = resource.getSiteMapContent();
                    //循环得到CMSPager
                    foreach (int cmsIndex in cmsMapList)
                    {
                        string url = string.Format(Protocol + "://" + storeurl + "/Cms/CMSpager.aspx?advid={0}&item={1}", resource.Advid, cmsIndex);
                        RootNode.AppendChild(getURLNode(url, "0.5"));

                        //获取CMS Detail
                        string cmsType = resource.CMSResourceTypes[cmsIndex];
                        string baa = resource.BusinessApArea;

                        eStore.POCOS.DAL.CMSType _cmsType;
                        if (string.IsNullOrWhiteSpace(cmsType) || !Enum.TryParse(cmsType.Trim().Replace(" ", "_").Replace("/", "Slash"), out _cmsType))
                            continue;

                        eStore.BusinessModules.CMSManager.DataModle modleDetail = store.getCms(_cmsType, baa);
                        if (modleDetail != null && modleDetail.DataSorce != null)
                        {
                            foreach (POCOS.CMS detailItem in modleDetail.DataSorce)
                            {
                                string detailUrl = string.Format(Protocol + "://" + storeurl + "/CMS/CmsDetail.aspx?CMSID={0}&CMSType={1}", detailItem.RECORD_ID, detailItem.cmsTypeX);
                                RootNode.AppendChild(getURLNode(detailUrl, "0.5"));
                            }
                        }
                    }
                }
            }

            //save
            if (System.IO.Directory.Exists(string.Format(@"C:\eStoreResources3C\Sitemap\{0}", store.profile.StoreID)) == false)
                System.IO.Directory.CreateDirectory(string.Format(@"C:\eStoreResources3C\Sitemap\{0}", store.profile.StoreID));

            siteMapdocument.Save(string.Format(@"C:\eStoreResources3C\Sitemap\{0}\SiteMap.xml", store.profile.StoreID));
            return siteMapdocument.InnerXml;
        }

        private void AddCategoryURLNode(XmlNode RootNode, eStore.POCOS.ProductCategory productcategory)
        {
            string url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(productcategory);
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Replace("~", "");
                RootNode.AppendChild(getURLNode(url, "0.8"));
            }
            
            if (productcategory.childCategoriesX.Count > 0)
            {
                foreach (eStore.POCOS.ProductCategory child in productcategory.childCategoriesX)
                {
                    AddCategoryURLNode(RootNode, child);
                }
            }
            else
            {
                foreach (eStore.POCOS.Product prod in productcategory.productList)
                {
                    url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(prod).Replace("~", "");
                    RootNode.AppendChild(getURLNode(url, "0.8"));
                }
            }
        }

        private void AddMenuURLNode(XmlNode RootNode, eStore.POCOS.Menu menu)
        {
            ///Product/AllProduct.aspx?type=standard 也写进去
            if (menu.ParentMenu == null)
            {
                if (menu.URL.Trim().Replace("/","").Length > 0)
                {
                    string url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(menu).Replace("~", "");
                    RootNode.AppendChild(getURLNode(url, "0.8"));
                }
            }
            if (menu.menuTypeX == POCOS.Menu.DataSource.CTOSCategory
                || menu.menuTypeX == POCOS.Menu.DataSource.StandardCategory
                || menu.menuTypeX == POCOS.Menu.DataSource.WidgetPage)
            {
                string url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(menu).Replace("~", "");
                RootNode.AppendChild(getURLNode(url, "0.8"));
            }
            if (menu.subMenusX.Count > 0)
            {
                foreach (eStore.POCOS.Menu child in menu.subMenusX)
                {
                    AddMenuURLNode(RootNode, child);
                }
            }
        }

        private void AddFooterMenuURLNode(XmlNode RootNode, eStore.POCOS.Menu menu)
        {
            if (!string.IsNullOrEmpty(menu.URL))
            {
                string url = string.Empty;
                if (menu.URL.ToLower().StartsWith("http://") || menu.URL.ToLower().StartsWith("https://"))
                    url = menu.URL;
                else if (menu.URL.StartsWith("javascript"))
                    return;
                else
                    url = Protocol + "://" + storeurl + eStore.Presentation.UrlRewriting.MappingUrl.getMappingUrl(menu).Replace("~", "");
                RootNode.AppendChild(getURLNode(url, "0.7"));
            }
            if (menu.subMenusX.Count > 0)
            {
                foreach (eStore.POCOS.Menu child in menu.subMenusX)
                {
                    AddFooterMenuURLNode(RootNode, child);
                }
            }
        }

        private XmlElement getURLNode(string url, string priority)
        {
            XmlElement urlNode = siteMapdocument.CreateElement(string.Empty, "url", NameSpaceStr);
            XmlElement xe_loc, xe_lastmod, xe_changefreq, xe_priority;

            xe_loc = siteMapdocument.CreateElement(string.Empty, "loc", NameSpaceStr);
            xe_loc.InnerText = url;
            urlNode.AppendChild(xe_loc);
            xe_lastmod = siteMapdocument.CreateElement(string.Empty, "lastmod", NameSpaceStr);
            xe_lastmod.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
            urlNode.AppendChild(xe_lastmod);
            xe_changefreq = siteMapdocument.CreateElement(string.Empty, "changefreq", NameSpaceStr);
            xe_changefreq.InnerText = "daily";
            urlNode.AppendChild(xe_changefreq);
            xe_priority = siteMapdocument.CreateElement(string.Empty, "priority", NameSpaceStr);
            xe_priority.InnerText = priority;
            urlNode.AppendChild(xe_priority);
            return urlNode;

        }
    }
}
