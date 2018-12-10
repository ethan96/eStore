using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class BTOSConfigDetail
    {
        private POCOS.Part _part;
        public POCOS.Part partX
        {
            get
            {
                return _part;
            }
            set { 
                _part = value;
            }
        }
        public string descriptionX
        {
            get
            {
                if (partX != null)
                    return partX.productDescX;
                else
                    return this.Description;

            }
        }


    }
}
