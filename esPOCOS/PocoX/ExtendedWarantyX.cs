using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class ExtendedWaranty
    {
        private Part _partX = null;
        public Part partX
        {
            get 
            {
                PartHelper ph = new PartHelper();
                _partX = ph.getPart(this.PartNo, this.StoreID);
                return _partX; 
            }
            set { _partX = value; }
        }
    }
}
