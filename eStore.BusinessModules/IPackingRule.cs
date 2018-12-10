using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules
{
   public interface IPackingRule
    {
       List<POCOS.PackagingBox> packing(POCOS.Store store, POCOS.Cart cart, decimal dimensionalWeightBase);
    }
}
