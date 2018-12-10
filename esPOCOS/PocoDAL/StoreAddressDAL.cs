using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS{

    public partial class StoreAddress
    {
        private StoreAddressHelper _helper = null;

        public StoreAddressHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

	} 
 }