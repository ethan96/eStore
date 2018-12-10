using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class GiftLog
    {
        public GiftActivity GiftActivityX
        {
            get 
            {
                if (this.GiftActivity != null)
                    return this.GiftActivity;
                else
                {
                    GiftActivityHelper helper = new GiftActivityHelper();
                    var obj = helper.getGiftActivityById(this.GiftId.Value);
                    this.GiftActivity = obj;
                    return obj;
                }
            }
        }
    }
}
