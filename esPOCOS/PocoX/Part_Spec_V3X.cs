using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.POCOS
{
    public partial class Part_Spec_V3
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            Part_Spec_V3 spec = obj as Part_Spec_V3;

            return (spec.PART_NO.Equals(this.PART_NO, StringComparison.OrdinalIgnoreCase)
                        && spec.CATEGORY_ID == this.CATEGORY_ID);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        private Part _part = null;
        public Part part
        {
            get { return _part; }
            set { _part = value; }
        }
        

    }
}
