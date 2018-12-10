using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Proxy
{
   public class ProductAvailability
    {

        public string PartNO
        {
            set;
            get;
        }

       public DateTime  RequestDate{
         set;
         get;
       }

       public int      RequestQty{
        set;
        get;

       }
       
        public string      Flag{
        set;get;
       }
       
        public string      Type{
           set;
           get;
       }
       
        public int      QtyATP{ 
           set;
         get;
       }
       
        public int     QtyFulfill{
            set;
            get;
        }

        public int QtyATB
        {
            set;
            get;
        
        }
        public string Message {
            get;set;
        }
    }
}
