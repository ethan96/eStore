namespace eStore.POCOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using eStore.POCOS.DAL;

    public partial class EDMCodeMapping
    {
        private EDMCodeMappingHelper _helper = null;
        public EDMCodeMappingHelper helper
        {
            get
            {
                return _helper;
            }
            set
            {
                _helper = value;
            }
        }

        public int save()
        {
            if (helper == null)
            {
                _helper = new EDMCodeMappingHelper();
            }
            return _helper.save(this);
        }

        public int delete()
        {
            if (_helper == null)
                _helper = new EDMCodeMappingHelper();
            return _helper.delete(this);
        }

    }
}
