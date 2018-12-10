using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class MarketingResourceHelper : Helper
    {
        public List<MarketingResourceResult> GetList(string rAWTXT, string nFKEY)
        {
            return context.spGetMarketingResource(rAWTXT, nFKEY).ToList();
        }
    }
}
