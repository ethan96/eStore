using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.DAL
{
    public class RewardRecordHelper : Helper
    {
        public decimal GetCurrentPoint(string userid)
        {
            return (from record in context.RewardRecords
                    where record.UserID.Equals(userid, StringComparison.OrdinalIgnoreCase)
                    select record).Sum(p => p.RecordType == 1 ? p.TotalPoint.Value : p.RecordType == 2 ? p.TotalPoint.Value * -1 : 0m);
        }
    }
}
