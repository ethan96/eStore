using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class Part_Spec_V3
    {
        private Part_Spec_V3Helper _helper = null;

        public Part_Spec_V3Helper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new Part_Spec_V3Helper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new Part_Spec_V3Helper();
            return _helper.delete(this);
        }
    }
}
