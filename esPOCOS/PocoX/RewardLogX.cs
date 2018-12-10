using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.POCOS
{
    public partial class RewardLog
    {
        private Order _order;
        public Order Order
        {
            get {
                if (!string.IsNullOrEmpty(this.OrderNo) && _order == null)
                {
                    POCOS.DAL.OrderHelper helper = new DAL.OrderHelper();
                    _order = helper.getOrderbyOrderno(this.OrderNo);
                }
                return _order;
            }
            set { _order = value; }
        }

        private RewardGiftItem _rewardGiftItem;
        public RewardGiftItem RewardGiftItem
        {
            get {
                if (this.GiftNo.HasValue && this.GiftNo.Value > 0 && _rewardGiftItem == null)
                {
                    POCOS.DAL.RewardGiftItemHelper helper = new DAL.RewardGiftItemHelper();
                    _rewardGiftItem = helper.getRewardGiftItemById(this.GiftNo.Value);
                }
                return _rewardGiftItem;
            }
            set { _rewardGiftItem = value; }
        }

        private string _transactionTypeX;
        public string TransactionTypeX
        {
            get {
                if (string.IsNullOrEmpty(_transactionTypeX) && this.TransactionType != null)
                {
                    try
                    {
                        Enumerations.eStoreConfigure.RewardLogTransactionType enumTransactionType = (Enumerations.eStoreConfigure.RewardLogTransactionType)this.TransactionType;
                        _transactionTypeX = enumTransactionType.ToString().Replace("_", " ");
                    }
                    catch (Exception)
                    {
                        //活不到默认为  取消
                        _transactionTypeX = Enumerations.eStoreConfigure.RewardLogTransactionType.Cancel.ToString();
                    }
                }
                
                return _transactionTypeX; 
            }
            set { _transactionTypeX = value; }
        }

        public string SourceTemp { get; set; }
    }
}
