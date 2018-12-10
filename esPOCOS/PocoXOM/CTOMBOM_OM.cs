using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    /// <summary>
    /// This file defines the extended methods and properties of CTOSBOM for OM
    /// </summary>
    public partial class CTOSBOM
    {
        /// <summary>
        /// This method will perform validation at option component.  It returns true if the validation passes.  Otherwise, it return
        /// false.  Along with validation result, it will also provide failure causes too.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public VALIDSTATE isValidOption(List<String> errors)
        {
            return validateOption(errors);
        }

        /// <summary>
        /// This method is to validate CTOSBOM category.  To be a valid BOM category
        /// 1. It has to have default option
        /// 2. Its default option has to be in sellable state in SAP
        /// 3. All of its default option parts need to have valid prices.
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public CTOSBOM.VALIDSTATE isValidCategory(List<String> errors)
        {
            CTOSBOM.VALIDSTATE isValid = CTOSBOM.VALIDSTATE.VALID;
            //find its default option
            if (this.isCategory() &&
                this.ChildComponents != null &&
                this.ChildComponents.Count > 0)
            {
                List<CTOSBOM> defaultOptions = (from item in ChildComponents
                                                where item.Defaults == true
                                                select item).ToList();

                if ((defaultOptions != null && defaultOptions.Count > 0) || (string.IsNullOrEmpty(this.InputType) == false && this.InputType.ToUpper() == "MULTISELECT"))
                {
                    foreach (CTOSBOM bom in defaultOptions)
                    {
                        if (bom.isValidOption(errors) == VALIDSTATE.INVALID)
                            isValid = VALIDSTATE.INVALID;
                    }
                }
                else
                {
                    isValid = VALIDSTATE.INVALID;
                    var errormessageStr = String.Format("Missing default option in category {0}", this.name);
                    errors.Add(errormessageStr);
                    if(!ErrorMessages.Contains(errormessageStr))
                        ErrorMessages.Add(errormessageStr);
                }
            }
            else
            {
                isValid = VALIDSTATE.INVALID;
                var errormessageStr = String.Format("There is no option in category {0}", this.name);
                errors.Add(errormessageStr);
                if(!ErrorMessages.Contains(errormessageStr))
                    ErrorMessages.Add(errormessageStr);
            }

            return isValid;
        }
    }
}
