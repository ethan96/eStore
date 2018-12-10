using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules.LTron
{
    [Serializable]
    public class ProductCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> SubCategories { get; set; }
    }
}
