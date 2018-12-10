using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CampaignMailLogByEDM
    {
        private CampaignMailLogByEDM_Helper _helper = null;

        public CampaignMailLogByEDM_Helper helper
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
                _helper = new CampaignMailLogByEDM_Helper();
            }
            return _helper.save(this);
        }
    }
}
