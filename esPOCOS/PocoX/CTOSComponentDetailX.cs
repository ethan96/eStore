using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class CTOSComponentDetail
    {
        private Part _part;

        public Part part
        {
            get 
            {
                if (_part == null)
                    _part = (new PartHelper()).getPart(this.SProductID, this.StoreID, true);
                return _part; 
            }

        }
        
    }
}
