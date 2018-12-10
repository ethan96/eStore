using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ScenarioCategory
    {
        public ScenarioCategoryHelper helper = null;
        public int save()
        {
            if (helper == null)
                helper = new ScenarioCategoryHelper();
            return helper.save(this); ;
        }
        public int delete()
        {
            if (helper == null)
                helper = new ScenarioCategoryHelper();
            return helper.delete(this);
        }
    }
}
