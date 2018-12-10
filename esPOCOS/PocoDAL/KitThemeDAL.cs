using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class KitTheme
    {
        private KitThemeHelper _helper = null;

        public KitThemeHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new KitThemeHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new KitThemeHelper();
            return _helper.delete(this);
        }

    }
}
