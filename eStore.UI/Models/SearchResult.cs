using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class SearchResult
    {
        public string Keywords { get; set; }
        public int Count { get; set; }
        public string exactItem { get; set; }
        public string SearchTerms { get; set; }
        public List<SearchGroup> groups { get; set; }
        public List<SearchItem> items { get; set; }
        public List<Models.Category> categories { get; set; }

        public SearchResult()
        {
            categories = new List<Category>();
        }
    }
}