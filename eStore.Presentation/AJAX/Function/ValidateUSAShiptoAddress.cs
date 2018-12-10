using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;
using eStore.BusinessModules;

namespace eStore.Presentation.AJAX.Function
{
    public class ValidateUSAShiptoAddress : IAJAX
    {
        public enum Type { Order, Quotation}
        public string ProcessRequest(HttpContext context)
        {
            string shippingmethods = AddressValidator.ValidatationProvider.UPS.ToString();
            if (!string.IsNullOrEmpty(context.Request["shippingmethod"]))
                shippingmethods = context.Request["shippingmethod"].ToString();

            Type t = Type.Order;
            if (!string.IsNullOrEmpty(context.Request["type"]) && context.Request["type"].ToString().ToUpper() == Type.Quotation.ToString().ToUpper())
                t = Type.Quotation;

            POCOS.CartContact cc = null;
            if (t == Type.Order)
                cc = eStoreContext.Current.Order.cartX.ShipToContact;
            else
                cc = eStoreContext.Current.Quotation.cartX.ShipToContact;

            if (cc == null)
                return JsonConvert.SerializeObject(new JObject() { new JProperty("result", 0), new JProperty("message", "No ship to data. Please check.") });

            var result = eStoreContext.Current.Store.isValidateUSAShiptoAddress(cc, shippingmethods, eStoreContext.Current.User);
            if (result.isValid == false)
            {
                if (result.TranslationKey == POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_POBox)
                {
                    //If address is PO box, then stop placing order/quotation.
                    return JsonConvert.SerializeObject(new JObject() { new JProperty("result", 0), new JProperty("message", eStoreLocalization.Tanslation(result.TranslationKey)) });
                }
                else
                    return JsonConvert.SerializeObject(new JObject() { new JProperty("result", 1), new JProperty("message", eStoreLocalization.Tanslation(POCOS.Store.TranslationKey.eStore_Valid_ShipToAddress_CCContact)) });
            }
            else
                return JsonConvert.SerializeObject(new JObject() { new JProperty("result", 2), new JProperty("message", "") });
        }
    }
}
