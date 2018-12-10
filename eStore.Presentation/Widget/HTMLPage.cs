using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace eStore.Presentation.Widget
{
    public class HTMLPage
    {
        public HTMLHeader header = new HTMLHeader();

        public HTMLBody body = new HTMLBody();
        public List<ServerControl> ServerControl { get; set; }
    }

    public class HTMLHeader
    {
        public String title = "";
        public Hashtable meta = new Hashtable();
        public String scripts = "";
        public String links = "";

        public String content = "";
    }

    public class HTMLBody
    {
        /// <summary>
        /// orig property keeps original body content including <body> tag
        /// </summary>
        public String orig = "";
        /// <summary>
        /// value property keeps the alternated content with real time widget values
        /// </summary>
        public String value = "";
    }
    public enum ServerControlType { SimpleMatrix, ProductList, ProductMatrix, ProductOperationButtons }
    public class ServerControl
    {
       public Guid ID { get; set; }
        public ServerControlType ServerControlType { get; set; }
        public string para { get; set; }
    }
}
