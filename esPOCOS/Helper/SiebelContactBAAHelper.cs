using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class SiebelContactBAAHelper : Helper
    {
        public List<SiebelContactBAA> getAllStandardSiebelContactBAA()
        {
            List<SiebelContactBAA> siebelContactBAA = new List<SiebelContactBAA>();

            siebelContactBAA = context.SiebelContactBAAs.Where(scb => scb.IS_STANDARD == true).Distinct().OrderBy(sbc => sbc.VALUE).ToList();

            if (siebelContactBAA != null)
            {
                foreach (var item in siebelContactBAA)
                {
                    item.helper = this;
                }
            }
            return siebelContactBAA;
        }
    }
}
