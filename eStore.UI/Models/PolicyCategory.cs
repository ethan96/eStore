using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class PolicyCategory
    {
        public PolicyCategory(POCOS.PolicyCategory pc)
        {
            if (pc.Policy == null)
                this.Html = "";
            else
                this.Html = pc.Policy.HtmlContext;
        }

        public string Html
        {
            get;
            set;
        }
    }
}