using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using eStore.POCOS.PocoX;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class EDMCampaignScheduler
    {

        public EDMCampaignSchedulerHelper helper;

        public int save()
        {
            if (helper == null)
                helper = new EDMCampaignSchedulerHelper();

            return helper.save(this);
        }
        public int delete()
        {
            if (helper == null)
                helper = new EDMCampaignSchedulerHelper();

            return helper.delete(this);
        }
    }
}