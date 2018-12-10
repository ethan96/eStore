using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CampaignCode
    {
        public CampaignCodeHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new CampaignCodeHelper();

            return helper.save(this);
        }
    }
}
