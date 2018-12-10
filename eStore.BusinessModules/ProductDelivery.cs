using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

namespace eStore.BusinessModules
{
    public class ProductDelivery
    {
        private string _ico;

        public string Ico
        {
            get { return _ico; }
            set { _ico = value; }
        }

        private DateTime _endDeliveryTime;

        public DateTime EndDeliveryTime
        {
            get { return _endDeliveryTime; }
            set { _endDeliveryTime = value; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public ProductDelivery(POCOS.Product.PRODUCTMARKETINGSTATUS status, string storeid)
        {
            _endDeliveryTime = DateTime.Now.AddMonths(1);
            switch (status)
            {
                case POCOS.Product.PRODUCTMARKETINGSTATUS.TenDayFastDelivery:
                    _endDeliveryTime = new DateHelper().getAvailableDate(DateTime.Now, 10, storeid);
                    break;
                case POCOS.Product.PRODUCTMARKETINGSTATUS.ThreeDayFastDelivery:
                    _endDeliveryTime = new DateHelper().getAvailableDate(DateTime.Now, 3, storeid);
                    break;
                case POCOS.Product.PRODUCTMARKETINGSTATUS.TwoDaysFastDelivery:
                    _endDeliveryTime = new DateHelper().getAvailableDate(DateTime.Now, 2, storeid);
                    break;
                case POCOS.Product.PRODUCTMARKETINGSTATUS.TwoWeeksFastDelivery:
                    _endDeliveryTime = new DateHelper().getAvailableDate(DateTime.Now, 14, storeid);
                    break;
                default:
                    break;
            }
            _ico = "/images/" + status.ToString() + ".gif";
        }

    }
}
