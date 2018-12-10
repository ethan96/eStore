using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation.AJAX.Function
{
    class ExtentedComponentContent : IAJAX
    {
        public string ProcessRequest(System.Web.HttpContext context)
        {
            Presentation.Product.CTOS ctosMgr = new Presentation.Product.CTOS();
            POCOS.Product_Ctos ctos = (POCOS.Product_Ctos)Presentation.eStoreContext.Current.Store.getProduct(context.Request["productID"]);
            POCOS.BTOSystem btos = ctosMgr.getBTOSbyParameters(ctos, context.Request["para"]);
            POCOS.CTOSBOM component=ctos.CTOSBOMs.FirstOrDefault(x=>x.ComponentID.ToString()== context.Request["ComponentID"]);
            bool showdetail=false;   
            if (Presentation.eStoreContext.Current.User != null)
            {
                showdetail = Presentation.eStoreContext.Current.User.actingUser.hasRight(User.ACToken.ATP);
                 
            }
           
            return ctosMgr.composeOptions(ctos, btos, component, showdetail);
        }
    }
}