using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// This class contain quantity availibility list including current and in production
    /// </summary>
    public class ATP
    {
        public ATP(DateTime earliestAvailableDate, int earliestAvailableQuantity)
        {
            availableDate = earliestAvailableDate;
            availableQty = earliestAvailableQuantity;
        }

        public DateTime availableDate
        {
            get;
            set;
        }

        public int availableQty
        {
            get;
            set;
        }

        public string Message
        {
            get;set;
        }
    }
}
