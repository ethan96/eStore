using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ReplicationCategoryProductsMapping
    {
        private ProductCategory _sourceCategory;
        public ProductCategory sourceCategory
        {
            get 
            {
                if (_sourceCategory == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _sourceCategory = helper.getProductCategory(this.CategoryIDFrom.Value, this.StoreIDFrom, true);
                }
                return _sourceCategory; 
            }
            set { _sourceCategory = value; }
        }

        private ProductCategory _targetCategory;
        public ProductCategory targetCategory
        {
            get 
            {
                if (_targetCategory == null)
                {
                    ProductCategoryHelper helper = new ProductCategoryHelper();
                    _targetCategory = helper.getProductCategory(this.CategoryIDTo.Value, this.StoreIDTo, true);
                }
                return _targetCategory; 
            }
            set { _targetCategory = value; }
        }
    }
}
