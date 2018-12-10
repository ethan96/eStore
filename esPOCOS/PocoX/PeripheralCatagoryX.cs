using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class PeripheralCatagory
    {
        public enum IntegrityCheckType { None, StandardStorage, StandardOS }

        /// <summary>
        /// 
        /// </summary>
        public IntegrityCheckType integrityCheckTypeX
        {
            get
            {
                if (string.IsNullOrEmpty(CategoryClassification))
                    return IntegrityCheckType.None;
                var _type = IntegrityCheckType.None;
                Enum.TryParse(CategoryClassification, out _type);
                return _type;
            }
        }


    }
}
