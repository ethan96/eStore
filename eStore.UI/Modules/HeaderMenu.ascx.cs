using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using eStore.Presentation.UrlRewriting;

namespace eStore.UI.Modules
{
    public partial class HeaderMenu : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ICollection<POCOS.Menu> menus = Presentation.eStoreContext.Current.Store.getMenuItems(Presentation.eStoreContext.Current.MiniSite);
            StringBuilder sbMenu = new StringBuilder();
            int columncount = Presentation.eStoreContext.Current.getIntegerSetting("MenuColumns", 5);
            int rowcount = 15;


            btnSearch.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search);

            foreach (POCOS.Menu rootMenu in menus)
            {
                int count = rootMenu.subMenusX.Count + rootMenu.subMenusX.Sum(m => m.subMenusX.Count);
                int tmpcount = 0;
                if (count <= columncount+1)
                    rowcount = 1;
                else if (count <= 2 * columncount)
                    rowcount = 2;
                else
                    rowcount = (int)Math.Round((count / columncount) + 0.5);

                
                if (menus.First().Equals(rootMenu))
                    sbMenu.AppendFormat(" <li class=\"dir first\"><a href=\"{1}\" {2} class=\"{3}rootlink\">{0}</a><ul>"
                        , rootMenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage)
                        , ResolveUrl(MappingUrl.getMappingUrl(rootMenu)), 
                        rootMenu.Target != null && !string.IsNullOrEmpty(rootMenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty
                        , rootMenu.subMenusX.Count == 0 ? "withoutchilden " : ""
                        );
                else
                    sbMenu.AppendFormat(" <li class=\"dir\"><a href=\"{1}\" {2} class=\"{3}rootlink\">{0}</a><ul>"
                        , rootMenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage)
                        , ResolveUrl(MappingUrl.getMappingUrl(rootMenu))
                        , rootMenu.Target != null && !string.IsNullOrEmpty(rootMenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty
                        , rootMenu.subMenusX.Count==0?"withoutchilden ":""
                        );
                sbMenu.Append("<li><ul>");
                if (rootMenu.subMenusX.Count > 0)
                {
                    foreach (POCOS.Menu submenu in rootMenu.subMenusX)
                    {
                        if (rowcount - tmpcount < submenu.subMenusX.Count * 2 / 3.0 && tmpcount > 0)
                        {
                            tmpcount = submenu.subMenusX.Count + 1;
                            sbMenu.Append("</ul></li><li><ul>");
                        }
                        else
                        { tmpcount += submenu.subMenusX.Count + 1; }

                        sbMenu.AppendFormat(" <li class=\"submenu\"><a href=\"{1}\" {2}>{0}</a>", submenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(submenu)), submenu.Target != null && !string.IsNullOrEmpty(submenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                        if (submenu.subMenusX.Count > 0)
                        {
                            sbMenu.Append(" <ul>");
                            foreach (POCOS.Menu leafmenu in submenu.subMenusX)
                            {

                                sbMenu.AppendFormat(" <li><a href=\"{1}\" {2}>{0}</a></li>", leafmenu.getLocalName(eStore.Presentation.eStoreContext.Current.CurrentLanguage), ResolveUrl(MappingUrl.getMappingUrl(leafmenu)), leafmenu.Target != null && !string.IsNullOrEmpty(leafmenu.Target.ToString().ToLower()) ? "target=\"_blank\"" : string.Empty);
                            }
                            sbMenu.Append(" </ul>");
                        }
                        sbMenu.Append("</li>");
                    }
                }
                sbMenu.Append("</ul></li>");
                sbMenu.Append(" </ul></li>");
            }
            productCategoryMenu.Text = sbMenu.ToString();


        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(Presentation.eStoreContext.Current.SearchConfiguration.ResultPageUrl +"?skey=" + Request["storekeyworddispay"]);
        }


    }
}