using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class OMMenu
    {
        public OMMenuHelper _omhelper = null;

        public OMMenuHelper Omhelper
        {
            get { return _omhelper; }
            set { _omhelper = value; }
        }
        public int save()
        {
            if (_omhelper == null)
                _omhelper = new OMMenuHelper();
            return _omhelper.save(this);
        }
        public int delete()
        {
            if (_omhelper == null)
                _omhelper = new OMMenuHelper();
            return _omhelper.delete(this);
        }
    }
}
