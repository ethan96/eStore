using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    /// <summary>
    /// This class content ComponentDetail of CTOSBOM option.  It's a runtime class and will have no entity related
    /// </summary>
    public class CTOSBOMComponentDetail
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="SProductID"></param>
        /// <param name="part"></param>
        /// <param name="qty"></param>
        public CTOSBOMComponentDetail(String storeID, String SProductID, Part part, int qty)
        {
            this.SProductID = SProductID;
            this.storeID = storeID;
            this.part = part;
            this.qty = qty;
        }

        public String SProductID
        {
            set;
            get;
        }

        public String storeID
        {
            set;
            get;
        }

        private Part _part = null;
        public Part part
        {
            set { _part = value; }
            get 
            {
                if (_part == null)
                    _part = new PartHelper().getPart(SProductID, storeID);

                return _part;
            }
        }

        public int qty
        {
            set;
            get;
        }
    }
}
