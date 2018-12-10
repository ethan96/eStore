using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class StoreErrorCode
    {

        public void reSetTransGlobalResource(List<StoreErrorCodeGlobalResource> resLs)
        {
            if (resLs.Any())
            {
                foreach (var c in resLs)
                {
                    var obj = this.StoreErrorCodeGlobalResources.FirstOrDefault(t => t.LanguageId == c.LanguageId);
                    if (obj != null)
                        obj.copyfrom(c);
                    else
                        this.StoreErrorCodeGlobalResources.Add(c);
                }
            }
            foreach (var c in this.StoreErrorCodeGlobalResources.ToList())
            {
                var obj = resLs.FirstOrDefault(t => c.LanguageId == t.LanguageId);
                if (obj == null)
                    this.StoreErrorCodeGlobalResources.Remove(c);
            }
        }
    }
}
