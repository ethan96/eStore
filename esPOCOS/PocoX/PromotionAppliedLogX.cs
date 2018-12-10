using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace eStore.POCOS
{
    public partial class PromotionAppliedLog
    {
        public PromotionAppliedLog clone()
        {

            PromotionAppliedLog newLog = new PromotionAppliedLog();

            newLog.PromotionCode = this.PromotionCode;
            newLog.CampaignID = this.CampaignID;
            newLog.PromotionCode = this.PromotionCode;
            newLog.Status = this.Status;
            newLog.Type = this.Type;
            newLog.QuoteOrderNo = this.QuoteOrderNo;
            newLog.UserID = this.UserID;
            newLog.CartID = this.CartID;
            newLog.StoreID = this.StoreID;
            newLog.ModelNO = this.ModelNO;
            newLog.Qty = this.Qty;
            newLog.Discounts = this.Discounts;
            newLog.UpdatedDate = DateTime.Now;
            newLog.CreatedDate = this.CreatedDate;

            return newLog;
        }
    }
}
