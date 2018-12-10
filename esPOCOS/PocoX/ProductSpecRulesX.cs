using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
 public   class ProductSpecRules
    {

        public List<Product> _products{
            set;
            get;        
        }

        public List<VProductMatrix> _specrules
        {
            set;
            get;

        }

        public  ProductCategory  _productcategory
        {
            set;
            get;

        }
    }
}
