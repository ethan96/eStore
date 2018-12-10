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
    public partial class BundleItem
    {
        public BundelItemHelper helper;
        public int delete(int bundleItemId)
        {
            if (helper == null)
                helper = new BundelItemHelper();
            return helper.delete(bundleItemId);
        }
    }
}
