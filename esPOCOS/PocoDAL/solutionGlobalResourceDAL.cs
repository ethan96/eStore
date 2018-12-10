using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class SolutionGlobalResource
    {
        private SolutionGlobalResourceHelper _helper = null;

        public SolutionGlobalResourceHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new SolutionGlobalResourceHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new SolutionGlobalResourceHelper();
            return _helper.delete(this);
        }
    }
}
