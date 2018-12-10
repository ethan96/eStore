using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public class ProductCategoryItem
    {
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
