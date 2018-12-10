using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using System.Web.Services.Protocols;
using eStore.BusinessModules.USTax;
using eStore.Utilities;
using System.Text.RegularExpressions;

namespace eStore.BusinessModules
{
    /// <summary>
    /// Tax class is to provide convenience for containing tax related information
    /// </summary>
    public class Tax
    {

        #region Attributes
        private Cart _cart = null;
        private Decimal _freight = 0m;
        private string VatNumber=string.Empty;

        // Tax total amount
        private decimal _amount = 0;
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        // Tax rate
        private decimal _rate = 0;
        public decimal Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        // Calculate tax included freight
        private bool _includedFreight = false;
        public bool IncludedFreight
        {
            get { return _includedFreight; }
            set { _includedFreight = value; }
        }

        // Tax' status. If it is false, it means that tax is not ready.
        private bool _status;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string ErrorCode { get; set; }

        #endregion

        #region TaxMethods

        // Constructor
        public Tax(Order order)
        {
            //is channelPartner
            if (order.isReferredToChannelPartner())
            {
                _rate = 0;
                _amount = 0;
                _status = true;
                _includedFreight = false;
            }
            else
            {
                _cart = order.cartX;
                _freight = order.Freight.GetValueOrDefault()-order.FreightDiscount.GetValueOrDefault();
                VatNumber = order.VATNumbe;
                getTax();
            }
        }

        public Tax(Quotation quotation)
        {
            _cart = quotation.cartX;
            _freight = quotation.Freight.GetValueOrDefault()-quotation.FreightDiscount.GetValueOrDefault();
            getTax();
        }

        /// <summary>
        /// This function helps to get tax rate by different store.
        /// US has the different tax rate by each state. 
        /// EU used the VAT tax, if the store ships packages to NL and other not EC country then add 19% VAT.
        /// Other Country uses the fix tax rate.
        /// </summary>
        /// <returns></returns>
        private Tax getTax()
        {
            if (_cart.storeX.TaxProvider.ToLower() != "fixrate")
            {
                switch (_cart.StoreID)
                {
                    case "AUS":
                        getUSTaxRate();
                        break;

                    case "AEU":
                        getEUTaxRate();
                        break;

                    default:
                        break;
                }
            }
            else if (_cart.storeX.TaxProvider.ToLower() == "fixrate")
            {
                getFixTaxRate();
            }
            _amount = Utilities.Converter.CartPriceRound(_amount, _cart.StoreID);
            return this;
        }



        /// <summary>
        /// Get United States tax rate
        /// </summary>
        private void getUSTaxRate()
        {
            string _zipCode = _cart.ShipToContact.ZipCode;
            string _state = _cart.ShipToContact.State;
            string _county = _cart.ShipToContact.Country;
            string _city = _cart.ShipToContact.City;

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
                    if (!String.IsNullOrEmpty(_zipCode) && _zipCode.Length > 5)
                        _zipCode = _zipCode.Substring(0, 5);    //keep only 5 digits for SAP standard

                    if (_usTaxRate.getZIPInfo(_zipCode, ref  state, ref  county, ref  city, ref  _shippingAlone, ref _advanTax))
                    {
                        _state = state;
                        if (_advanTax)
                        {
                            if (_usTaxRate.getSalesTaxByZIP(_zipCode, ref _zipTax))
                            {
                                _sinTax = _zipTax * 100;
                                _status = true;
                            }
                            else
                            {
                                _sinTax = 0;
                                _status = false;
                                ErrorCode = "failed to get sales tax by zip code.";
                                throw new Exception("USTaxService is failed to get sales tax by zip code.");
                            }
                            _rate = _sinTax;
                        }
                        else
                        {
                            // Free duty
                            _rate = 0;
                            _status = true;
                        }
                    }
                    else
                    {
                        _status = false;
                        ErrorCode = "ZipCode is incorrect.";
                        throw new Exception("Cart.ShipToContact.ZipCode is incorrect");
                    }

                    _includedFreight = (_cart.ShipToContact.State == "TX") ? true : false;
                    if (_includedFreight == true)
                    {
                        // To calculate tax amount within freight
                        _amount = Math.Round((_freight + _cart.TotalAmount) * _rate * 0.01m, _taxRateDecimal);
                    }
                    else
                    {
                        _amount = Math.Round(_cart.TotalAmount * _rate * 0.01m, _taxRateDecimal);
                    }
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Failed to get AUS tax rate", "", "", "", ex);
                }
            }
            else
            {
                _rate = 0;
                _amount = 0;
                _status = true;
            }
        }



        /// <summary>
        /// Get Europe tax rate
        /// </summary>
        private void getEUTaxRate()
        {
            int _taxRateDecimal = 2;        // Tax rate decimal
            _includedFreight = true;        //aeu tax include freigth
            try
            {
                string VATCountryCode = _cart.BillToContact.countryCodeX.ToUpper();
                // AEU shipping warehouse is in Nehterland
                try
                {
                    Regex regexObj = new Regex(@"(\w{2})(\d+)");
                    Match matchResult = regexObj.Match(VatNumber);
                    if (matchResult.Success)
                    {
                         VATCountryCode = matchResult.Groups[1].Value;
                    }
                }
                catch (ArgumentException)
                {
                
                }
                if (_cart.storeX.ShipFromAddress.Country == "NL")
                {
                    //ship to is NL TAX rate=19%
                    if (_cart.ShipToContact.countryCodeX.ToUpper() == "NL")
                    {
                        _rate = 19m;
                    }
                    //bill to is NL if ship to is EC TAX rate=19% else rate=0
                    else if (VATCountryCode == "NL")
                    {
                        CountryHelper _countryHelper = new CountryHelper();
                        _rate = _countryHelper.isECCountry(_cart.ShipToContact.countryCodeX.ToUpper()) ? 19m : 0;
                    }
                    else
                    {
                        _rate = 0m;
                    }
                    _status = true;
                }
                else
                {
                    _status = false;
                    throw new Exception(" AEU Store ShipFromAddress.Country is not from Netherland");
                }

                //_includedFreight = (_order.cartX.ShipToContact.Country == "??") ? true : false;
                if (_includedFreight == true)
                {
                    // To calculate tax amount within freight
                    _amount = Math.Round((_freight + _cart.TotalAmount) * _rate * 0.01m, _taxRateDecimal);
                }
                else
                {
                    _amount = Math.Round(_cart.TotalAmount * _rate * 0.01m, _taxRateDecimal);
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Error("Failed to get AEU tax rate", _cart.UserID, "", _cart.StoreID, ex);
            }
        }



        /// <summary>
        /// Get fix tax rate by Store
        /// </summary>
        private void getFixTaxRate()
        {
            try
            {
                TaxConfigHelper taxConfigHelper = new TaxConfigHelper();
                // Get fix tax rate by store id
                POCOS.TaxConfig taxCofing = taxConfigHelper.getFixTaxRateConfig(_cart.StoreID, _cart.ShipToContact.countryCodeX, _cart.ShipToContact.State);
                if (taxCofing != null)
                {

                    _rate = taxCofing.TaxRate.Value;
                    int _taxRateDecimal = 2;
                    if (!string.IsNullOrEmpty(taxCofing.IncludeFreight) && taxCofing.IncludeFreight == "Y")
                    {
                        _amount = (_rate != 0) ? Math.Round((_cart.TotalAmount + _freight) * _rate * 0.01m, _taxRateDecimal) : 0m;
                        _includedFreight = true;
                    }
                    else
                    {
                        _amount = (_rate != 0) ? Math.Round(_cart.TotalAmount * _rate * 0.01m, _taxRateDecimal) : 0m;
                        _includedFreight = false;
                    }
                    _status = true;

                }
                else
                {
                    _amount = 0;
                    _rate = 0;
                    _status = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Failed to get " + _cart.StoreID + " tax rate";
                this.ErrorCode = errorMsg;
                eStoreLoger.Error(errorMsg, _cart.UserID, "", _cart.StoreID, ex);
            }
        }


        #endregion
    }
}
