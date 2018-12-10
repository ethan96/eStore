using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class VCategory
    {
        public int ProductCount { get; set; }
        public IEnumerable<Models.Product> Products { get; set; }
        public string ProductHtml { get; set; }
    }
}