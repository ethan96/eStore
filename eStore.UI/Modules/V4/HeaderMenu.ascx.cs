using eStore.Presentation.UrlRewriting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class HeaderMenu : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            POCOS.MiniSite minisite = eStore.Presentation.eStoreContext.Current.MiniSite;
            IList<eStore.POCOS.Menu> menus = eStore.Presentation.eStoreContext.Current.Store.getMenuItems(minisite);
            StringBuilder sbMenu = new StringBuilder();
            int columncount = 4;
            int rowcount = 15;
            

            sbMenu.Append("<div class=\"eStore_menuLink_linkBlock float-left\">");

            sbMenu.Append("<ol>");
            foreach (POCOS.Menu rootMenu in menus)
            {
                Dictionary<string, int> menuDic = new Dictionary<string, int>();
                int currColunm = 1;
                int count = rootMenu.subMenusX.Count + rootMenu.subMenusX.Sum(m => m.subMenusX.Count);
                int tmpcount = 0;
                if (count <= columncount )
                    rowcount = 1;
                else if (count <= 2 * columncount)
                    rowcount = 2;
                else
                    rowcount = (int)Math.Ceiling((count / (decimal)columncount));


                sbMenu.AppendLine("<li class=\"eStore_menuLink_link\">");
                sbMenu.AppendFormat("<a href=\"{0}\"  class=\"eStore_menuLink_linkTxt linkDown\"   {1} >{2}</a><span></span>"
                , ResolveUrl(MappingUrl.getMappingUrl(rootMenu))
                , rootMenu.Target != null && !string.IsNullOrEmpty(rootMenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty
                , rootMenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage)
                );

                if (rootMenu.subMenusX.Count > 0)
                {
                    sbMenu.Append("<div class=\"eStore_menuLink_linkList\">");
                    sbMenu.Append("<div class=\"eStore_menuLink_linkList_block\">");

                    foreach (POCOS.Menu submenu in rootMenu.subMenusX)
                    {
                        if (rowcount - tmpcount <= submenu.subMenusX.Count * 2 / 3.0 && tmpcount > 0)
                        {
                            menuDic.Add("{{" + currColunm + "}}", tmpcount);
                            sbMenu.Append("{{" + currColunm + "}}");
                            currColunm++;
                        }
                        StringBuilder sbTemp = new StringBuilder();
                        sbTemp.Append("<ol>");
                        if (currColunm > columncount)
                        { }
                        else if (rowcount - tmpcount <= submenu.subMenusX.Count * 2 / 3.0 && tmpcount > 0)
                        {
                            tmpcount = submenu.subMenusX.Count + 1;
                            sbTemp.Append("</ol></div><div class=\"eStore_menuLink_linkList_block\"><ol>");
                        }
                        else
                        { tmpcount += submenu.subMenusX.Count + 1; }

                        sbTemp.AppendFormat(" <li><a href=\"{1}\" {2}>{0}</a>", submenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(submenu)), submenu.Target != null && !string.IsNullOrEmpty(submenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                        if (submenu.subMenusX.Count > 0)
                        {

                            foreach (POCOS.Menu leafmenu in submenu.subMenusX)
                            {

                                sbTemp.AppendFormat(" <li><a href=\"{1}\" {2}>{0}</a></li>", leafmenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(leafmenu)), leafmenu.Target != null && !string.IsNullOrEmpty(leafmenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                            }

                        }
                        sbTemp.Append("</ol>");
                        if (currColunm <= columncount)
                            sbMenu.Append(sbTemp);
                        else
                        {
                            var item = menuDic.FirstOrDefault(c => c.Value == menuDic.Min(t => t.Value));
                            sbMenu.Replace(item.Key, sbTemp.ToString());
                        }
                    }
                    sbMenu.Append("</div>");
                    sbMenu.Append("</div>");
                    sbMenu.Append("</li>");
                }

                foreach (var item in menuDic)
                    sbMenu.Replace(item.Key, "");
            }
            sbMenu.Append("</ol>");
              
            sbMenu.Append("</div>");
            productCategoryMenu.Text = sbMenu.ToString();
            
        }

    }
}