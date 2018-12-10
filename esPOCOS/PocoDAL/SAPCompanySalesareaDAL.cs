using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;


namespace eStore.POCOS
{

    public partial class SAPCompanySalesarea
    {
        private SAPCompanySalesareaHelper _helper = null;

        public SAPCompanySalesareaHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new SAPCompanySalesareaHelper();
            return _helper.save(this);
        }
    }
}
