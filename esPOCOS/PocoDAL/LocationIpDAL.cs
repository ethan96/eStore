using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class LocationIp
    {
        private LocationIpHelper _helper = null;

        public LocationIpHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new LocationIpHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new LocationIpHelper();
            return _helper.delete(this);
        }
    }
}
