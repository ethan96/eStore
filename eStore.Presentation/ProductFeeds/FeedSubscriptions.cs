using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace eStore.Presentation.ProductFeeds
{
    public enum OutputType { csv, xml, json, }
    public class FeedSubscriptions
    {
        public virtual OutputType outputType { get { return OutputType.csv; } }

        public virtual System.IO.MemoryStream Subscribe()
        {
            throw new NotImplementedException();
        }

        protected string cleanup(string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", "");
        }
    }
}
