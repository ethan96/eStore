using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.POCOS
{
    public partial class Solution : TaggingEnabled<Solution>
    {
        #region Tagging
        public override object EntityKey
        {
            get
            {
                return this.Id;
            }

        }
        public override string EntityStoreId
        {
            get
            {
                return this.StoreID;
            }
        }

        #endregion
        public string BannerFileX
        {
            get
            {
                if (!string.IsNullOrEmpty(this.BannerFile))
                    return this.BannerFile;
                else
                    return this.Image;
            }
        }
    }
}
