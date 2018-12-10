namespace eStore.POCOS
{
    using DAL;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    public partial class StoreEDMInfo
    {
        private StoreEDMInfoHelper _helper = null;

        public StoreEDMInfoHelper helper
        {
            get
            {
                return _helper;
            }
            set
            {
                _helper = value;
            }
        }
    }
}
