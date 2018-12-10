using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Widget
    {

        private WidgetHelper _helper = null;

        public WidgetHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new WidgetHelper();
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new WidgetHelper();
            return _helper.delete(this);
        }

    }
}
