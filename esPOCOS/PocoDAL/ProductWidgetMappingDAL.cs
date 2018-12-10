using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ProductWidgetMapping
    {
        public ProductWidgetMappingHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new ProductWidgetMappingHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new ProductWidgetMappingHelper();

            return helper.delete(this);
        }
    }
}
