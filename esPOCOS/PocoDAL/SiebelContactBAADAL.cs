using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class SiebelContactBAA
    {
        private SiebelContactBAAHelper _helper = null;

        public SiebelContactBAAHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

    }
}
