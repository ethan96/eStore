using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules.TaxService
{
    /// <summary>
    /// get tax rate by setting in TaxConfig
    /// </summary>
    public class FixRate : TaxCalculator
    {
        protected override void calculateTax(POCOS.Cart cart, Store store)
        {
            try
            {
                TaxConfigHelper taxConfigHelper = new TaxConfigHelper();
                // Get fix tax rate by store id
                POCOS.TaxConfig taxCofing = taxConfigHelper.getFixTaxRateConfig(cart.StoreID, cart.ShipToContact.countryCodeX, cart.ShipToContact.State);
                if (taxCofing != null)
                {

                    this.Rate = taxCofing.TaxRate.Value;
                    int _taxRateDecimal = 2;
                    if (!string.IsNullOrEmpty(taxCofing.IncludeFreight) && taxCofing.IncludeFreight == "Y")
                    {
                        this.Amount = (this.Rate != 0) ? Math.Round((cart.TotalAmount + this.Freight - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal) : 0m;
                        _includedFreight = true;
                    }
                    else
                    {
                        this.Amount = (this.Rate != 0) ? Math.Round((cart.TotalAmount - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal) : 0m;
                        _includedFreight = false;
                    }
                    this.Status = true;

                }
                else
                {
                    this.Amount = 0;
                    this.Rate = 0;
                    this.Status = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Failed to get " + cart.StoreID + " tax rate";
                this.ErrorCode = errorMsg;
                eStoreLoger.Error(errorMsg, cart.UserID, "", cart.StoreID, ex);
            }
        }

        public void GetcalculateTax(POCOS.Cart cart, Store store)
        {
            calculateTax(cart, store);
        }
    }
}
