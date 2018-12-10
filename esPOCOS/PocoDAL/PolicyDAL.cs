using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Policy
    {
        private PolicyHelper _helper = null;

        public PolicyHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new PolicyHelper();
            return _helper.delete(this);
        }
    }
}
