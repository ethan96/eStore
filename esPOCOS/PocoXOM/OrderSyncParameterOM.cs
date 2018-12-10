using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.PocoX;

namespace eStore.POCOS.OM
{
    public class OrderSyncParameterOM
    {
        private List<NameValuePair> _options = new List<NameValuePair>();
        private List<NameValuePair> _sortedOptions = null;

        public OrderSyncParameterOM(OrderSyncParameter syncParam)
        {
            this.name = syncParam.Parameter;
            addOption(syncParam);
            this.seq = syncParam.ParameterSeq.GetValueOrDefault();
        }

        public String name { get; set; }

        /// <summary>
        /// This method is for adding option to this parameter
        /// </summary>
        /// <param name="value"></param>
        public void addOption(OrderSyncParameter syncParam)
        {
            NameValuePair pair = new NameValuePair(syncParam.ShowText, syncParam.Value, syncParam.ParameterSeq.GetValueOrDefault());
            _options.Add(pair);
        }

        public List<NameValuePair> options
        {
            get 
            {
                if (_sortedOptions == null)
                    _sortedOptions = _options.OrderBy(item => item.sequence).ToList();
                return _sortedOptions; 
            }
        }

        public int seq { get; set; }
    }
}
