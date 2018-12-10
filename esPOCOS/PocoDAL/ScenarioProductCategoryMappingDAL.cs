using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    /// <summary>
    /// This class is to provide a way to create bundle item
    /// </summary>
    public partial class ScenarioProductCategoryMapping
    {
        public ScenarioProductCategoryMappingHelper helper;
        public int delete()
        {
            if (helper == null)
                helper = new ScenarioProductCategoryMappingHelper();
            return helper.delete(this);
        }
    }
}
