using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eStore.Presentation.AJAX
{
  public  interface  IAJAX
    {
      string ProcessRequest(HttpContext context);
    }
}
