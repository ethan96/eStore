using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.DAL
{
    public class SAPProductWarrantyHelper : Helper
    {
        public SAP_Product_Warranty getSAPProductWarranty(string partNo)
        {
            var _sapProductWarranty = (from s in context.SAP_Product_Warranty
                               where s.SProductID.ToUpper().Equals(partNo.ToUpper())
                               select s).FirstOrDefault();

            return _sapProductWarranty;
        }
    }
}
