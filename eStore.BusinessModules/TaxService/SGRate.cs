using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules.TaxService
{
    public class SGRate : TaxCalculator
    {
        protected override void calculateTax(POCOS.Cart cart, Store store)
        {

            try
            {
                _includedFreight = true; // if store have tax will includ freight
                int _taxRateDecimal = 2;
                if (cart.ShipToContact.countryCodeX.ToUpper() == "SG")
                {
                    this.Rate = 7m;
                }
                else
                {
                    this.Rate = 0m;
                    //this.Message = "It will be subject to duty and tax (borne by receiver at custom)";
                }
                this.Status = true;

                if (_includedFreight == true)
                {
                    // To calculate tax amount within freight
                    this.Amount = Math.Round((this.Freight + cart.TotalAmount - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal);
                }
                else
                {
                    this.Amount = Math.Round((cart.TotalAmount- CartDiscount) * this.Rate * 0.01m, _taxRateDecimal);
                }
            }
            catch (Exception ex)
            {

            }



        }
    }
}
