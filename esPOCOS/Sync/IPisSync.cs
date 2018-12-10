using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS.Sync
{
    interface IPisSync
    {
        PISEntities PISEntity { get; set; }
        List<spGetModelByPN_estore_Result> getModelByPartNo(string partNo);
        string getCategoryID(string productName);
    }
}
