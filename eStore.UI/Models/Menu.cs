using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Menu
    {
        public Menu() { }
        public Menu(eStore.POCOS.Menu menu)
        {
            Id = menu.MenuID;
            Name = menu.getLocalName(Presentation.eStoreContext.Current.CurrentLanguage);
            Link =esUtilities.CommonHelper.ResolveUrl( Presentation.UrlRewriting.MappingUrl.getMappingUrl(menu));
            if (menu.subMenusX != null && menu.subMenusX.Any())
            {
                Children = menu.subMenusX.Select(x => new Menu(x)).ToList();
            }
        }
        public string Name { get; set; }
        public string Link { get; set; }
        public int Id { get; set; }
        public List<Menu> Children { get; set; }
    }
}