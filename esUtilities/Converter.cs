using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.Utilities
{
    public class Converter
    {
        /// <summary>
        /// When each store is loaded, store rounding unit shall be populated here
        /// </summary>
        private static Dictionary<String, Decimal> _roundingUnits = new Dictionary<string, decimal>();
        private static String _cartPriceRoundingUnitKey = "CartPriceRoundingUnit";

        /// <summary>
        /// This method shall be invoked to set store rounding unit whenever store is loaded.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="unit"></param>
        /// <param name="keyX">optional parameter, if it has value, the roudning unit will be saved with additonal key associated to</param>
        public static void setRoundingUnit(String storeId, String unit, String keyX = null)
        {
            Decimal roundingUnit = 1m;
            try
            {
                roundingUnit = Convert.ToDecimal(unit);
            }
            catch (Exception)
            {
                roundingUnit = 1m;
            }

            //create dictionaly index key
            String indexKey = storeId;
            if (!String.IsNullOrWhiteSpace(keyX))
                indexKey = String.Format("{0}-{1}", storeId, keyX);

            if (_roundingUnits.ContainsKey(indexKey))
                _roundingUnits[indexKey] = roundingUnit;
            else
                _roundingUnits.Add(indexKey, roundingUnit);
        }

        /// <summary>
        /// This method shall be invoked to set store rounding unit of price items in cart related content whenever store is loaded.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="unit"></param>
        public static void setCartPriceRoundingUnit(String storeId, String unit)
        {
            setRoundingUnit(storeId, unit, _cartPriceRoundingUnitKey);
        }

        /// <summary>
        /// This method is to retrieve rounding unit of a particular store.  If keyX is specified, it'll look forward
        /// particular rounding setting of the specified store.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="keyX"></param>
        /// <returns></returns>
        public static Decimal getRoundingUnit(String storeId, String keyX = null)
        {
            String indexKey = storeId;
            if (!String.IsNullOrWhiteSpace(keyX))
                indexKey = String.Format("{0}-{1}", storeId, keyX);

            return  _roundingUnits.ContainsKey(indexKey) ? _roundingUnits[indexKey] : 1m;
        }

        public static Decimal getCartPriceRoundingUnit(String storeID)
        {
            return getRoundingUnit(storeID, _cartPriceRoundingUnitKey);
        }

        /// <summary>
        /// The method is to round decimal number according to store setting
        /// </summary>
        /// <param name="target"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public static Decimal round(Decimal target, String storeId)
        {
            if (target == 0)
                return target;
            else
                return round(target, getRoundingUnit(storeId));
        }

        public static Decimal CartPriceRound(Decimal target, String storeId)
        {
            if (target == 0)
                return target;
            else
                return round(target, getCartPriceRoundingUnit(storeId));
        }

        // This method is to round decimal number according to specified rounding unit
        public static Decimal round(Decimal target, Decimal unit, Boolean roundUp = true)
        {
            Decimal quotient = 0m;
            if (roundUp)
                quotient = Math.Round((target / unit) + 0.499m);  //make sure it's rounded up
            else
                quotient = Math.Round((target / unit) + 0.001m); 

            if (quotient == 0m)
                quotient = 1m;   //make at least 1 unit
            return quotient * unit;
        }
    }
}
