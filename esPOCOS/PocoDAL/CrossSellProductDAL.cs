using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CrossSellProduct
    {
        private CrossSellProductHelper _helper = null;
        private SpecHelper _shelper = null;

        public CrossSellProductHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public SpecHelper shelper
        {
            get { return _shelper; }
            set { _shelper = value; }
        }


        public int save()
        {
            if (_helper == null)
                _helper = new CrossSellProductHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new CrossSellProductHelper();
            return _helper.delete(this);
        }
    }
}
