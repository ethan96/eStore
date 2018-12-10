using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules.TaxService
{
   public abstract class TaxCalculator
    {
       public decimal Rate { get; set; }
       public decimal Amount { get; set; }
      public decimal CartDiscount { get; set; }
       public bool Status { get; set; }
       protected bool _includedFreight = false;
       public bool IncludedFreight
       {
           get { return _includedFreight; }
           set { _includedFreight = value; }
       }
       public decimal Freight { get; set; }
       public string ErrorCode { get; set; }
       public string VATNumber { get; set; }
       protected string ResellerID { get; set; }
       protected abstract void calculateTax(POCOS.Cart cart, Store store);

       public void calculateTax(POCOS.Order order, Store store)
       {
           ResellerID = order.ResellerID;
           calculateTax(order.cartX, store);
       }

       public void calculateTax(POCOS.Quotation quotation, Store store)
       {
           ResellerID = quotation.ResellerID;
           calculateTax(quotation.cartX, store);
       }
    }
}
