using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class VKitThemeData
    {
        public string Type { get; set; }
        public string ShowType { get; set; }
        public List<BusinessModules.AdvantechCmsModel> Data { get; set; }
        public string Title { get; set; }
        public POCOS.KitThemeMapping Mapping { get; set; }
    }
}