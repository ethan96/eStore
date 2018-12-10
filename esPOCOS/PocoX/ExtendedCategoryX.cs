
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{
    public partial class ExtendedCategory
    {
        private List<Product> _productList;
        public List<Product> productList
        {
            get {
                if (_productList == null)
                {
                    _productList = (new ExtendedCategoryHelper().getProducts(this));
                }
                return _productList;
            }
        }
    }
}
