using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

 namespace eStore.POCOS.DAL
{
    public partial class ExtendedWarantyHelper : Helper
    {
        public List<Part> getPartInforByWarrantyList(string storeID)
        {
            List<Part> ls = new List<Part>();
            ls = (from p in context.Parts
                    from w in context.ExtendedWaranties
                    where p.SProductID == w.PartNo && p.StoreID == w.StoreID && w.StoreID == storeID
                    select p).ToList();
            return ls;
        }
    }
}
