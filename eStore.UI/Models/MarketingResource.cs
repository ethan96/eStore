using eStore.POCOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class MarketingResource
	{
        public string Title { get; set; }
        public string Url { get; set; }
        public string ShortDesc { get; set; }

        public MarketingResource()
		{ }

        public MarketingResource(MarketingResourceResult marketing)
        {
            this.Title = marketing.Title;
            this.Url = marketing.ResponseUri;
            this.ShortDesc = marketing.Meta_Description;
        }
    }

    public class MarketingResourcePage
    {
        public List<MarketingResource> MarketingResources { get; set; }
        public int Count { get; set; }
        public object Groups { get; set; }

        public MarketingResourcePage()
        {
            MarketingResources = new List<MarketingResource>();
        }
    }
}