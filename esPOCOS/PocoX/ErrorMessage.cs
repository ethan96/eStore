using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.PocoX
{
  public  class ErrorMessage
    {

      public ErrorMessage(string c, string msg)
      {
            columname = c;
            message = msg;
        }

        public string columname  {
            get;
            set;
        }

         
        public string message
        {
            get;
            set;
        }

    }
}
