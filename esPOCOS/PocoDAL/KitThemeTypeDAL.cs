using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class KitThemeType
    {
        private KitThemeTypeHelper _helper = null;

        public KitThemeTypeHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new KitThemeTypeHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new KitThemeTypeHelper();
            return _helper.delete(this);
        }
    }
}
