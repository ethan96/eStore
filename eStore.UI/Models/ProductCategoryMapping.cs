using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class ProductCategoryMapping
    {

        public ProductCategoryMapping()
        { }

        public ProductCategoryMapping(POCOS.ProductCategoryItem categoryItem)
        {
            this._sproductId = categoryItem.SproductId;
            this._seq = categoryItem.Seq;
        }

        private string _sproductId;

        public string SproductId
        {
            get { return _sproductId; }
            set { _sproductId = value; }
        }

        private int _seq;

        public int Seq
        {
            get { return _seq; }
            set { _seq = value; }
        }

    }
}