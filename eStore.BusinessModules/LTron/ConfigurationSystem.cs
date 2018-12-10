using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules.LTron
{
    public class ConfigurationSystem : Product
    {
        public List<Component> components { get; set; }

        public class Component {
            public string Name
            {
                get;
                set;
            }
            /// <summary>
            /// Radio,Select,Multiselect
            /// </summary>
            public string DisplayType
            {
                get;
                set;
            }

            public bool IsMainComponent { get; set; }

            public List<Option> options { get; set; }
        }
        public class Option {
            public string Name
            {
                get;
                set;
            }
            public bool Default { get; set; }
            public List<OptionDetail> optiondetails { get; set; }
        }

        public class OptionDetail
        {
            public string productcode { get; set; }
            public int Qty { get; set; }
        }
        
    }
}
