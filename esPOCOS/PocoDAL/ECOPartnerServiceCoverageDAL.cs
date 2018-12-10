using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ECOPartnerServiceCoverage
    {
        private ECOPartnerServiceCoverageHelper _helper = null;

        public ECOPartnerServiceCoverageHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new ECOPartnerServiceCoverageHelper();
            return _helper.delete(this);
        }
    }
}
