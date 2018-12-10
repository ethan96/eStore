using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;
using eStore.BusinessModules.USTax;
using eStore.Utilities;

namespace eStore.BusinessModules.TaxService
{
    public class BBTax : TaxCalculator
    {
        protected override void calculateTax(Cart cart, Store store)
        {
            if (!string.IsNullOrEmpty(base.ResellerID))
            {
                this.Rate = 0;
                this.Amount = 0;
                this.Status = true;
                return;
            }
            string _zipCode = cart.ShipToContact.ZipCode;
            string _state = cart.ShipToContact.State;
            string _county = cart.ShipToContact.Country;
            string _city = cart.ShipToContact.City;

            decimal _zipTax = 0;
            decimal _sinTax = 0;
            int _taxRateDecimal = 2;        // Tax rate decimal

            string state = null;
            string county = null;
            string city = null;
            bool _shippingAlone, _advanTax;
            _shippingAlone = false;
            _advanTax = true;
            if (_county.Equals("US") || _county.Equals("USA") || _county.Equals("United States", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    USTaxService _usTaxRate = new USTaxService();

                    var URL = store.profile.getStringSetting("BBTaxWebServiceURL");
                    if (string.IsNullOrEmpty(URL))
                        URL = "http://my.advantech.com/Services/BBorderAPI.asmx";

                    _usTaxRate.Url = URL;
                    _usTaxRate.Timeout = 100000;

                    if (!String.IsNullOrEmpty(_zipCode) && _zipCode.Length > 5)
                        _zipCode = _zipCode.Substring(0, 5);    //keep only 5 digits for SAP standard
                    if (!String.IsNullOrEmpty(_zipCode) && _zipCode.Length < 5)
                    {
                        _zipCode = _zipCode.PadLeft(5, '0');
                    }

                    if (_usTaxRate.getZIPInfo(_zipCode, ref state, ref county, ref city, ref _shippingAlone, ref _advanTax))
                    {
                        _state = state;
                        if (_advanTax)
                        {
                            if (_usTaxRate.getSalesTaxByZIP(_zipCode, ref _zipTax))
                            {
                                _sinTax = _zipTax * 100;
                                this.Status = true;
                            }
                            else
                            {
                                _sinTax = 0;
                                this.Status = false;
                                ErrorCode = "failed to get sales tax by zip code.";
                                throw new Exception("USTaxService is failed to get sales tax by zip code.");
                            }
                            this.Rate = _sinTax;
                        }
                        else
                        {
                            // Free duty
                            this.Rate = 0;
                            this.Status = true;
                        }
                    }
                    else
                    {
                        this.Status = false;
                        ErrorCode = "ZipCode is incorrect.";
                        throw new Exception("Cart.ShipToContact.ZipCode is incorrect");
                    }

                    _includedFreight = (cart.ShipToContact.State == "TX") ? true : false;
                    if (_includedFreight == true)
                    {
                        // To calculate tax amount within freight
                        this.Amount = Math.Round((this.Freight + cart.TotalAmount - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        this.Amount = Math.Round((cart.TotalAmount - CartDiscount) * this.Rate * 0.01m, _taxRateDecimal, MidpointRounding.AwayFromZero);
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Failed to get AUS tax rate", "", "", "", ex);
                }
            }
            else
            {
                this.Rate = 0;
                this.Amount = 0;
                this.Status = true;
            }
        }
    }
}
