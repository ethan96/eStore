using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class VProductMatrix
    {
        public int productcount { get; set; }
        public bool selected { get; set; }
        public bool ismatch;

        /// <summary>
        /// this is runtime property for epaps.
        /// </summary>
        public bool isProductSepc { get; set; }
    }
}