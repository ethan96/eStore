using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules.Task
{
    //操作Popular 推送视窗
    public class PopularProductReporter : TaskBase
    {
        private PopularModelLog popularLog = null;

        private Order order = null;

        private Quotation quotation = null;

        private PopularEventType popularType;

        public PopularProductReporter(Order _order, PopularModelLog _popularLog, PopularEventType _popularType)
        {
            this.order = _order;
            this.popularLog = _popularLog;
            this.popularType = _popularType;
        }

        public PopularProductReporter(Quotation _quotation, PopularModelLog _popularLog, PopularEventType _popularType)
        {
            this.quotation = _quotation;
            this.popularLog = _popularLog;
            this.popularType = _popularType;
        }

        public PopularProductReporter(PopularModelLog _popularLog, PopularEventType _popularType)
        {
            this.popularLog = _popularLog;
            this.popularType = _popularType;
        }



        public override bool PreProcess()
        {
            bool status = base.PreProcess();
            if (popularLog == null)
                status = false;
            else if (popularType == PopularEventType.Order && order == null)
                status = false;
            else if (popularType == PopularEventType.Quotation && quotation == null)
                status = false;
            return status;
        }

        public override object execute(object obj)
        {
            if (this.PreProcess())
            {
                try
                {
                    PopularModelLogHelper modelLogHelper = new PopularModelLogHelper();
                    if (popularType == PopularEventType.Order)
                    {
                        //排除ctos和bundle 
                        List<string> sproductList = order.cartX.cartItemsX.Where(p => string.IsNullOrEmpty(p.BTOConfigID) && !p.BundleID.HasValue).Select(p=>p.SProductID).ToList();
                        modelLogHelper.saveModelLogByOrderQuotation(popularLog, order.OrderNo, sproductList);
                    }
                    else if (popularType == PopularEventType.Quotation)
                    {
                        List<string> sproductList = quotation.cartX.cartItemsX.Where(p => string.IsNullOrEmpty(p.BTOConfigID) && !p.BundleID.HasValue).Select(p => p.SProductID).ToList();
                        modelLogHelper.saveModelLogByOrderQuotation(popularLog, quotation.QuotationNumber, sproductList);
                    }
                    else if (popularType == PopularEventType.UserLogin)
                        modelLogHelper.saveModelLogByLogin(popularLog);
                    else if (popularType == PopularEventType.Click)
                        modelLogHelper.saveModelLogByClick(popularLog);
                    else
                        modelLogHelper.saveModelLogByImpression(popularLog);


                    OnCompleted();
                    return true;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("Event Publish WebService Error: " + ex.StackTrace, popularLog != null ? popularLog.UserId : "", "", "", ex);
                    OnFailed();
                    return null;
                }
            }
            else
                return null;
        }
    }

    public enum PopularEventType
    {
        Order,
        Quotation,
        Impression,
        Click,
        UserLogin
    }
}
