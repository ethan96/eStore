using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class MenuGlobalResource
    {
        public MenuGlobalResourceHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new MenuGlobalResourceHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new MenuGlobalResourceHelper();

            return helper.delete(this);
        }
    }
}
