using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class SpecialProductPrice
    {
        public enum RuleTypeWeight { None = 0, Equals = 1, StartsWith = 2, EndsWith = 3 }

        private RuleTypeWeight? _ruleType;
        public RuleTypeWeight ruleType
        {
            get 
            {
                if (_ruleType == null)
                    _ruleType = (RuleTypeWeight)Enum.Parse(typeof(RuleTypeWeight), this.RuleType);
                return _ruleType.Value;
            }
            set { _ruleType = value; }
        }
    }
}
