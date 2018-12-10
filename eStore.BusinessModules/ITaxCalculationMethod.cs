using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.BusinessModules
{
    /// <summary>
    /// Different regions have different tax calculation methods.  ITaxCalculationMethod provides 
    /// the interface definition for TaxCalculationMethod provider to implement.  
    /// In eStore TaxCalculator it will create proper TaxCalculationMethod instances based on regional configuration.
    /// </summary>
    interface ITaxCalculationMethod
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="country"></param>
        /// <param name="state"></param>
        /// <param name="postalCode"></param>
        /// <param name="shoppingCart">A Cart instance</param>
        /// <param name="tax">tax calculation result will be returned through this variable</param>
        /// <returns>
        ///     An integer -- use positive number for returnign successful status
        ///                   user negative number as error indicator
        /// </returns>
        Int32 calculateTax(String country, String state, String postalCode, Cart shoppingCart, out Tax tax);

    }
}
