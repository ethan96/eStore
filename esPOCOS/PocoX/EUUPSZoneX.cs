using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class EUUPSZone
    {
        public enum ZoneType { Standard, Express }

        /// <summary>
        /// this method is mark zone type
        /// </summary>
        public ZoneType type
        {
            get
            {
                if (this.Method == "Express")
                    return ZoneType.Express;
                return ZoneType.Standard;
            }
        }

        /// <summary>
        /// ups Method display name
        /// </summary>
        public string MethodX
        {
            get
            {
                /*  Forwarder has been changed from UPS to TNT, but rate remain the same
                if (this.type == ZoneType.Express)
                    return "UPS Express";
                return "UPS Standard";
                 *************************/

                if (this.type == ZoneType.Express)
                    return "TNT Express";
                return "TNT Economy";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<EUUPSPrice> _EUUPSPriceX;
        public List<EUUPSPrice> EUUPSPriceX
        {
            get 
            {
                if (_EUUPSPriceX == null)
                {
                    EUUPSZoneHelper _helper = new EUUPSZoneHelper();
                    _EUUPSPriceX = _helper.getEUUPSPriceByZone(this).OrderBy(p => p.ZoneCode).ThenBy(p => p.Method).ToList();
                }
                return _EUUPSPriceX; 
            }
            set { _EUUPSPriceX = value; }
        }


    }
}
