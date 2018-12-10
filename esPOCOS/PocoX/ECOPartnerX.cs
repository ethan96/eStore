using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class ECOPartner
    {
        public enum StatusType { ApplyFor,Passing,Deleted }

        private string _IndustryX;
        public string IndustryX
        {
            get
            {
                if (_IndustryX == null)
                {
                    _IndustryX = "";
                    _IndustryX = string.Join(",",this.ECOPartnerIndustries.Select(c => c.ECOIndustry.IndustryName).ToArray());
                }
                return _IndustryX;
            }
        }

        public StatusType status
        {
            get
            {
                return (StatusType)Enum.Parse(typeof(StatusType), this.Status);
            }
            set { this.Status = value.ToString(); }
        }
    }
}
