using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Factor : IComparable<Factor>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string TemplateName { get; set; }
        public bool LazyloadChildren { get; set; }
        public int Sequence { get; set; }

        public override bool Equals(Object obj)
        {
            Factor factorObj = obj as Factor;
            if (factorObj == null)
                return false;
            else
                return Id.Equals(factorObj.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public int CompareTo(Factor other)
        {
            if (other == null) return 1;
            return Name.CompareTo(other.Name);
        }
    }
}