using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class SimulateFreight
    {
        public string pn { get; set; }
        public string parent { get; set; }
        public int lineno { get; set; }
        public string displaypartno { get; set; }
        public string stockstatus { get; set; }
        public int qty { get; set; }

        public SimulateFreight()
        { }

        public SimulateFreight(POCOS.Part part, int qty, string parentID = "")
        {
            this.pn = part.SProductID;
            this.parent = parentID;
            this.stockstatus = part.StockStatus;
            this.qty = qty;
        }
    }
}