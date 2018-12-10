using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ECOPartnerSpecialty
    {
        private ECOPartnerSpecialtyHelper _helper = null;

        public ECOPartnerSpecialtyHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new ECOPartnerSpecialtyHelper();
            return _helper.delete(this);
        }
    }
}
