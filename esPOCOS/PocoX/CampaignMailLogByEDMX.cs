namespace eStore.POCOS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using eStore.POCOS.DAL;

    public partial class CampaignMailLogByEDM
    {
            public enum MailLogStatus : int
            {
                MailLog = 0,
                MailCheck = 1,
                PromotionLog = 2
            };
    }
}
