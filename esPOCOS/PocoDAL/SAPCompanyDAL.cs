using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{

    public partial class SAPCompany
    {
        private SAPCompanyHelper _helper = null;

        public SAPCompanyHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new SAPCompanyHelper();
            return _helper.save(this);
        }
    }
}
