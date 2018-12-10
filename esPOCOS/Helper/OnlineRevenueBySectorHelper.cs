using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class OnlineRevenueBySectorHelper : Helper
    {
        public List<getOnlineRevenueBySector_Result> GetOnlineRevenueBySector(string startDate, string endDate)
        {
            try
            {
                return context.getOnlineRevenueBySector(startDate, endDate).ToList();
            }
            catch
            {
                return new List<getOnlineRevenueBySector_Result>();
            }
        }
    }
}
