using eStore.POCOS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class Spec_Category_Lang
    {
        private Spec_Category_LangHelper _helper = null;

        public Spec_Category_LangHelper helper
        {
            get { return _helper; }
            set { _helper = value; }
        }

        public int save()
        {
            if (_helper == null)
                _helper = new Spec_Category_LangHelper();
            return _helper.save(this);
        }
    }
}
