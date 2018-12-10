using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS.PocoX
{
    /// <summary>
    /// This class provide a structure instance for storing name/value pair content
    /// </summary>
    public class NameValuePair
    {
        public NameValuePair(String name, String value, int seq=0)
        {
            this.name = name;
            this.value = value;
            this.sequence = seq;
        }

        public String name { get; set; }
        public String value { get; set; }
        public int sequence { get; set; }
    }
}
