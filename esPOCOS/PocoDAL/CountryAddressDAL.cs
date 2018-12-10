using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CountryAddress
    {
        private CountryAddressHelper _helper = null;

        public CountryAddressHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new CountryAddressHelper();
            return _helper.delete(this);
        }
    }
}
