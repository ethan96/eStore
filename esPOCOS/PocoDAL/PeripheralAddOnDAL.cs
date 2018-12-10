using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class PeripheralAddOn
    {
        private PeripheralAddOnHelper __helper = null;

        public PeripheralAddOnHelper helper
        {
            get { return __helper; }
            set { __helper = value; }
        }

        public int save()
        {
            if (__helper == null)
                __helper = new PeripheralAddOnHelper();
            return __helper.save(this);
        }

        public int delete()
        {
            if (__helper == null)
                __helper = new PeripheralAddOnHelper();
            return __helper.delete(this);
        }
    }
}
