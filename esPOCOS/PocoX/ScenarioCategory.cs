using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ScenarioCategory
    {
        private List<Product> _productList = null;

        public List<Product> productList
        {
            get
            {
                if (_productList == null)
                {
                     _productList=(from spcm in this.ScenarioProductCategoryMappings
                                   from p in this.ProductCategory.productList 
                                    orderby spcm.ProductCategroyMapping.Seq,p.name
                                    where p.SProductID==spcm.SProductID 
                                    select p).ToList();
                }
                return _productList;
            }
        }

    }
}
