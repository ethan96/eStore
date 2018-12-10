using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;
using eStore.POCOS.DAL;
using System.Text.RegularExpressions;

namespace eStore.BusinessModules.TaxService
{
    /// <summary>
    /// for Europe,
    /// ship to is NL TAX rate=19%
    /// bill to is NL if ship to is EC TAX rate=19% else rate=0
    /// and aeu tax include freigth
    /// </summary>
   public class EURate:TaxCalculator
    {
       protected override void calculateTax(POCOS.Cart cart, Store store)
        {
            int _taxRateDecimal = 2;        // Tax rate decimal
            _includedFreight = true;        //aeu tax include freigth
            try
            {
                string VATCountryCode = cart.BillToContact.countryCodeX.ToUpper();
                // AEU shipping warehouse is in Nehterland
                if (string.IsNullOrWhiteSpace(VATNumber) == false)
                {
                    try
                    {
                        Regex regexObj = new Regex(@"(\w{2})(\d+)");
                        Match matchResult = regexObj.Match(VATNumber);
                        if (matchResult.Success)
                        {
                            VATCountryCode = matchResult.Groups[1].Value;
                        }
                    }
                    catch (ArgumentException)
                    {

                    }
                }
                if (cart.storeX.ShipFromAddress.Country == "NL")
                {
                    //ship to is NL TAX rate=19%
                    if (cart.ShipToContact.countryCodeX.ToUpper() == "NL")
                    {
                        this.Rate = 21m;
                    }
                    //bill to is NL if ship to is EC TAX rate=19% else rate=0
                    else if (VATCountryCode == "NL")
                    {
                        CountryHelper _countryHelper = new CountryHelper();
                        this.Rate = _countryHelper.isECCountry(cart.ShipToContact.countryCodeX.ToUpper()) ? 19m : 0;
                    }
                    else
                    {
                        this.Rate = 0m;
                    }
                    this.Status = true;
                }
                else
                {
                    this.Status = false;
                    throw new Exception(" AEU Store ShipFromAddress.Country is not from Netherland");
                }

                //_includedFreight = (_order.cartX.ShipToContact.Country == "??") ? true : false;
                if (_includedFreight == true)
                {
                    // To calculate tax amount within freight
                    this.Amount = Math.Round((this.Freight + cart.TotalAmount-CartDiscount) * this.Rate * 0.01m, _taxRateDecimal);
                }
                else
                {
                    this.Amount = Math.Round((cart.TotalAmount - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal);
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed to get AEU tax rate", cart.UserID, "", cart.StoreID, ex);
            }
        }
    }
}
