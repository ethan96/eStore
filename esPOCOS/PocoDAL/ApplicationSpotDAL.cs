using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    /// <summary>
    /// This class is to provide a way to create bundle item
    /// </summary>
    public partial class ApplicationSpot
    {
        public ApplicationSpotHelper helper;
        public int delete(int id)
        {
            if (helper == null)
                helper = new ApplicationSpotHelper();
            return helper.delete(id);
        }
    }
}
