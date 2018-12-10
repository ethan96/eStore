using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class VContextTodaysHighlight
    {
        public IEnumerable<Models.TodaysHighlight> TodaysHighlights { get; set; }
        public string BtnMore { get; set; }
        public string Url { get; set; }
    }
}