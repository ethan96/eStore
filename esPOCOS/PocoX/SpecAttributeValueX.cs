using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class SpecAttributeValue
    {

        public SpecAttributeValuelang findDefaultSpecAttributeValuelang()
        {
            SpecAttributeValuelang spvl = null;
            if (this != null && this.SpecAttributeValuelangs != null && this.SpecAttributeValuelangs.Count > 0)
            {
                spvl = (from c in this.SpecAttributeValuelangs
                       select c).FirstOrDefault();
            }
            return spvl;
        }


    }
}
