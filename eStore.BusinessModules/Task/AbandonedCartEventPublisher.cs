using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace eStore.BusinessModules.Task
{
    public class AbandonedCartEventPublisher : TaskBase
    {
        //this value need to be restored from DB
        DateTime _lastScanTimeStart = DateTime.MinValue;
        DateTime _lastScanTimeEnd = DateTime.MinValue;
        private String _storeID;
        private String _storeURL;

        public AbandonedCartEventPublisher(String storeID, String storeURL) { _storeID = storeID; _storeURL = storeURL; }

        public override object execute(object obj)
        {
            //get EventManager
            EventManager eventManager = EventManager.getInstance(_storeID);
            //pick up inactive carts
            DateTime currentRunTime = DateTime.Now;
            if (_lastScanTimeStart == DateTime.MinValue)
                _lastScanTimeStart = currentRunTime.AddHours(-2);   //first round after system resume
            _lastScanTimeEnd = currentRunTime.AddHours(-1);

            POCOS.Store store = CachePool.getInstance().getStore(_storeID);
            if (store != null && store.getBooleanSetting("Abort_Check_Out", false) == false)
            {
                OrderHelper orderHelper = new OrderHelper();
                List<Order> orders = orderHelper.getAbandonedOrders(_storeID, _lastScanTimeStart, _lastScanTimeEnd);
                foreach (Order order in orders)
                {
                    //it's going to take a lot of effort at finding out this order's abandon page.  Hereby a general link will be used.
                    EventPublisher eventPublisher = new EventPublisher(_storeURL, order, EventType.AbortCheckout);
                    eventManager.PublishNewEvent(eventPublisher);
                }

                _lastScanTimeStart = _lastScanTimeEnd;  //shift end time to start time of next run
                NeedRerun = true;
                DeferDuration = 3600000;    //1 hour
                //DeferDuration = 10000;    //1 hour
            }
            return obj;
        }

    }
}
