using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{ 

    public partial class MBCPUMemorySpec
    {
        private MBCPUMemorySpecHelper _helper = null;
        public MBCPUMemorySpecHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }
        public int save()
        {
            if (_helper == null)
                _helper = new MBCPUMemorySpecHelper();
            return _helper.save(this);
        }
        public int delete()
        {
            if (_helper == null)
                _helper = new MBCPUMemorySpecHelper();
            return _helper.delete(this);
        }
    }
}
