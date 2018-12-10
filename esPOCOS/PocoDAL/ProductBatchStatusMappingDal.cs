using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ProductBatchStatusMapping
    {
        public ProductBatchStatusMappingHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new ProductBatchStatusMappingHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new ProductBatchStatusMappingHelper();

            return helper.delete(this);
        }
    }
}
