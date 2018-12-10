using eStore.Presentation.UrlRewriting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.UI.Modules.V4
{
    public partial class HeaderMenuV2 : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            POCOS.MiniSite minisite = eStore.Presentation.eStoreContext.Current.MiniSite;
            IList<eStore.POCOS.Menu> menus = eStore.Presentation.eStoreContext.Current.Store.getMenuItems(minisite);
            StringBuilder sbMenu = new StringBuilder();

            sbMenu.Append("<div class=\"eStore_menuLink_linkBlock float-left\">");

            sbMenu.Append("<ol>");
            foreach (POCOS.Menu rootMenu in menus)
            {
                Dictionary<string, int> menuDic = new Dictionary<string, int>();
                
                sbMenu.AppendLine("<li class=\"eStore_menuLink_link\">");
                sbMenu.AppendFormat("<a href=\"{0}\" class=\"eStore_menuLink_linkTxt\" {1}>{2} {3}</a><span></span>"
                , ResolveUrl(MappingUrl.getMappingUrl(rootMenu))
                , rootMenu.Target != null && !string.IsNullOrEmpty(rootMenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty
                , rootMenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage)
                , rootMenu.subMenusX.Count > 0 ? "<i class=\"fa fa-angle-down\"></i>" : ""
                );

                if (rootMenu.subMenusX.Count > 0)
                {
                    sbMenu.Append("<div class=\"eStore_menuLink_linkList\">");
                    foreach (POCOS.Menu submenu in rootMenu.subMenusX)
                    {
                        sbMenu.AppendFormat("<div class=\"eStore_menuLink_linkList_block{0}\">", (submenu.menuTypeX == POCOS.Menu.DataSource.BottomRight ? " menuBottomRight" : ""));
                        sbMenu.AppendFormat("<a href=\"{1}\" {2} class=\"l0\">{0} <i class=\"fa fa-angle-right\"></i></a>", submenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(submenu)), submenu.Target != null && !string.IsNullOrEmpty(submenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                        
                        if (submenu.subMenusX.Count > 0)
                        {
                            sbMenu.Append("<ol>");
                            foreach (POCOS.Menu leafmenu in submenu.subMenusX)
                            {
                                sbMenu.AppendFormat(" <li><a href=\"{1}\" {2}>{0}</a></li>", leafmenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(leafmenu)), leafmenu.Target != null && !string.IsNullOrEmpty(leafmenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                            }
                            sbMenu.Append("</ol>");
                        }                        
                        sbMenu.Append("</div>");
                    }
                    sbMenu.Append("</div>");
                }
                sbMenu.Append("</li>");
            }
            sbMenu.Append("</ol>");

            sbMenu.Append("</div>");
            productCategoryMenu.Text = sbMenu.ToString();
        }
    }
}