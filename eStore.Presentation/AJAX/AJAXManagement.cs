using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.Presentation.AJAX
{
    public class AJAXManagement
    {
        public static IAJAX getAJAXFunction(AJAXFunctionType type)
        {

            IAJAX ajax;

            try
            {

                ajax = (IAJAX)Activator.CreateInstance(Type.GetType(string.Format("eStore.Presentation.AJAX.Function.{0}", type.ToString())));

            }
            catch (Exception ex)
            {
                eStoreLoger.Error("PaymentProvider " + type.ToString(), "", "", "", ex);
                ajax = null;
            }
            return ajax;
        }
    }
}
