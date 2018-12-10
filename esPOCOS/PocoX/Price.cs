using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// If price currency is not specified, the caller shall assue it's in regular Store currency
    /// </summary>
    public class Price
    {
        public Price(decimal value, Currency currency)
        {
            this.value = value;
            this.currency = currency;
        }

        public Price()
        {
            // TODO: Complete member initialization
        }

        /// <summary>
        /// price figure
        /// </summary>
        public decimal value
        {
            get;
            set;
        }

        public string valueWithoutCurrency
        {
            get;
            set;
        }

        public string valueWithCurrency
        {
            get;
            set;
        }

        /// <summary>
        /// price currency
        /// </summary>
        public Currency currency
        {
            get;
            set;
        }

        public void exchangeValue(Currency targetCurrency)
        {
            if (targetCurrency == null || currency == null)
                return;
            value = (value * currency.ToUSDRate.GetValueOrDefault()) / targetCurrency.ToUSDRate.GetValueOrDefault();
            currency = targetCurrency;
        }
    }
}
