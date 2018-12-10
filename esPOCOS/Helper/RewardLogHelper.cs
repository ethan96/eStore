using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class RewardLogHelper : Helper
    {
        #region Business Read
        //根据Id 获取RewardLog 对象
        public RewardLog getRewardLogById(int RewardID)
        {
            if (RewardID == 0)
                return null;
            var rewardLogItem = (from r in context.RewardLogs
                      where r.RewardID == RewardID
                      select r).FirstOrDefault();

            if (rewardLogItem != null)
                rewardLogItem.helper = this;
            return rewardLogItem;
        }
        //查询完成Order订单后, 用户得到的积分.  状态=0   需要审核
        public List<RewardLog> getRewardLogByRewardApproval(string storeId, string startdate = "", string enddate = "", string orderNo = "", string userId = "")
        {
            bool isStartDate = false; DateTime startDateTime = new DateTime();
            bool isEndDate = false; DateTime endDateTime = new DateTime();
            if (!string.IsNullOrEmpty(startdate))
                isStartDate = DateTime.TryParse(startdate, out startDateTime);
            if (!string.IsNullOrEmpty(enddate))
            {
                isEndDate = DateTime.TryParse(enddate, out endDateTime);
                if (isEndDate)
                    endDateTime = endDateTime.AddHours(24);
            }
            //Pending = 0
            int transactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Pending;
            int approvedType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Approved;

            //List<RewardLog> rewardLogList = new List<RewardLog>();

            //var rewardLogSource = (from r in context.RewardLogs
            //                      join o in context.Orders on r.OrderNo equals o.OrderNo
            //                       where (r.TransactionType.Value == transactionType || r.TransactionType.Value == approvedType)
            //                      && (!string.IsNullOrEmpty(orderNo) ? r.StoreId == storeId && o.StoreID == storeId : true)
            //                      && (isStartDate ? r.UpdateDate >= startDateTime : true)
            //                      && (isEndDate ? r.UpdateDate < endDateTime : true)
            //                      && (!string.IsNullOrEmpty(orderNo) ? r.OrderNo == orderNo : true)
            //                      && (!string.IsNullOrEmpty(userId) ? r.UserId.ToUpper().Contains(userId.ToUpper()) : true)
            //                      orderby r.RewardID descending
            //                      select new
            //                      {
            //                          RewardLogItem = r,
            //                          RewardLogOrder = o
            //                      }).ToList();
            ////把order放到log中
            //if (rewardLogSource != null && rewardLogSource.Count > 0)
            //{
            //    foreach (var item in rewardLogSource)
            //    {
            //        item.RewardLogItem.Order = item.RewardLogOrder;
            //        rewardLogList.Add(item.RewardLogItem);
            //    }
            //}

            #region 只查RewardLog 没查Order
            var rewardLogList = (from r in context.RewardLogs
                                 where (r.TransactionType.Value == transactionType || r.TransactionType.Value == approvedType)
                                  && r.StoreId == storeId
                                  && (isStartDate ? r.UpdateDate >= startDateTime : true)
                                  && (isEndDate ? r.UpdateDate <= endDateTime : true)
                                  && (!string.IsNullOrEmpty(orderNo) ? r.OrderNo == orderNo : true)
                                  && (!string.IsNullOrEmpty(userId) ? r.UserId.ToUpper().Contains(userId.ToUpper()) : true)
                                 orderby r.RewardID descending
                                 select r).ToList();
            #endregion
            
            if (rewardLogList != null)
            {
                foreach (RewardLog item in rewardLogList)
                {
                    item.helper = this;
                }
            }
            return rewardLogList;
        }

        //查询需要 兑换产品的数据   状态=-1.    用户申请兑换产品
        public List<RewardLog> getRewardLogByRewardRedeem(string storeId, string startdate = "", string enddate = "", string userId = "",string giftName="")
        {
            bool isStartDate = false; DateTime startDateTime = new DateTime();
            bool isEndDate = false; DateTime endDateTime = new DateTime();
            if (!string.IsNullOrEmpty(startdate))
                isStartDate = DateTime.TryParse(startdate, out startDateTime);
            if (!string.IsNullOrEmpty(enddate))
            {
                isEndDate = DateTime.TryParse(enddate, out endDateTime);
                if (isEndDate)
                    endDateTime = endDateTime.AddHours(24);
            }

            //Redeemed = -1
            int transactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Redeemed;

            List<RewardLog> rewardLogList = new List<RewardLog>();
            var rewardLogSource = (from r in context.RewardLogs
                                 join g in context.RewardGiftItems on r.GiftNo equals g.ItemNo
                                   where r.StoreId == g.StoreId && r.StoreId == storeId //&& r.TransactionType == transactionType
                                 && (isStartDate ? r.UpdateDate >= startDateTime : true)
                                 && (isEndDate ? r.UpdateDate < endDateTime : true)
                                 && (!string.IsNullOrEmpty(userId) ? r.UserId.ToUpper().Contains(userId.ToUpper()) : true)
                                 && (!string.IsNullOrEmpty(giftName) ? g.Name.Contains(giftName) : true)
                                 orderby r.RewardID descending
                                 select new
                                 {
                                     RewardLogItem = r,
                                     RewardLogGiftItem = g
                                 }).ToList();

            if (rewardLogSource != null && rewardLogSource.Count > 0)
            {
                foreach (var item in rewardLogSource)
                {
                    item.RewardLogItem.RewardGiftItem = item.RewardLogGiftItem;
                    rewardLogList.Add(item.RewardLogItem);
                }
            }

            if (rewardLogList != null)
            {
                foreach (RewardLog item in rewardLogList)
                {
                    item.helper = this;
                }
            }

            return rewardLogList;
        }

        //根绝用户和时间   获取所有的log
        public List<RewardLog> getAllRewardLogByCondition(string storeId, string startdate = "", string enddate = "", string userId = "")
        {
            bool isStartDate = false; DateTime startDateTime = new DateTime();
            bool isEndDate = false; DateTime endDateTime = new DateTime();
            if (!string.IsNullOrEmpty(startdate))
                isStartDate = DateTime.TryParse(startdate, out startDateTime);
            if (!string.IsNullOrEmpty(enddate))
            {
                isEndDate = DateTime.TryParse(enddate, out endDateTime);
                if (isEndDate)
                    endDateTime = endDateTime.AddHours(24);
            }

            var rewardLogList = (from r in context.RewardLogs
                                   where r.StoreId == storeId 
                                   && (isStartDate ? r.UpdateDate >= startDateTime : true)
                                   && (isEndDate ? r.UpdateDate < endDateTime : true)
                                   && (!string.IsNullOrEmpty(userId) ? r.UserId.ToUpper().Contains(userId.ToUpper()) : true)
                                   orderby r.RewardID descending
                                   select r).ToList();

            if (rewardLogList != null)
            {
                foreach (RewardLog item in rewardLogList)
                {
                    item.helper = this;
                }
            }

            return rewardLogList;
        }

        //查询具体用户的 log
        public List<RewardLog> getRewardLogByUserId(string storeId, string userId = "", int? activityId = null)
        {
            var rewardLogList = (from r in context.RewardLogs
                                 where r.StoreId == storeId
                                 && (!string.IsNullOrEmpty(userId) ? r.UserId.ToUpper() ==userId.ToUpper() : true)
                                 orderby r.RewardID descending
                                 select r).ToList();
            if (activityId != null)
                rewardLogList = rewardLogList.Where(c => c.ActivityId == activityId).ToList();

            if (rewardLogList != null)
            {
                foreach (RewardLog item in rewardLogList)
                {
                    item.helper = this;
                }
            }

            return rewardLogList;
        }

        //获取用户可用的积分
        public decimal getUserTotalPoint(string storeId, string userId, int activityid)
        {
            //Pending = 0
            int pendingType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Pending;
            //Rejected = -4
            int rejectedType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Rejected;
            decimal totalPoint = (from o in context.RewardLogs
                                  where o.StoreId == storeId && o.UserId == userId && o.ActivityId == activityid
                                 && o.TransactionType != pendingType && o.TransactionType != rejectedType 
                                 group o by o.UserId
                                     into g
                                     select new
                                     {
                                         totalPoints = g.Sum(p => p.RewardPoint)
                                     }).ToList().Sum(p => p.totalPoints.Value);
            return totalPoint;
        }

        //获取用户已获得的所有积分
        public decimal getUserAllPoint(string storeId, string userId, int activityid)
        {
            //Pending = 0
            int pendingType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Pending;
            //Rejected = -4
            int rejectedType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Rejected;
            //Credit Back = 2
            int creditbackType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Credit_Back;

            decimal totalPoint = (from o in context.RewardLogs
                                  where o.StoreId == storeId && o.UserId == userId && o.ActivityId == activityid
                                 && o.TransactionType != pendingType && o.TransactionType != rejectedType 
                                 && o.TransactionType != creditbackType && o.RewardPoint > 0
                                  group o by o.UserId
                                      into g
                                      select new
                                      {
                                          totalPoints = g.Sum(p => p.RewardPoint)
                                      }).ToList().Sum(p => p.totalPoints.Value);
            return totalPoint;
        }

        //获取用户一共消耗的积分
        public decimal getUserTotalConsumptionPoint(string storeId, string userId, int activityid)
        {
            //Pending = 0
            int pendingType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Pending;
            //Rejected = -4
            int rejectedType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Rejected;
            //ApprovedType = 1
            int ApprovedType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Approved;
            decimal totalPoint = (from o in context.RewardLogs
                                  where o.StoreId == storeId && o.UserId == userId && o.ActivityId == activityid
                                 && o.TransactionType != pendingType && o.TransactionType != rejectedType && o.TransactionType != ApprovedType
                                  group o by o.UserId
                                      into g
                                      select new
                                      {
                                          totalPoints = g.Sum(p => p.RewardPoint)
                                      }).ToList().Sum(p => p.totalPoints.Value);
            return totalPoint;
        }
        #endregion

        #region Creat Update Delete
        public int save(RewardLog _rewardLog)
        {
            if (_rewardLog == null) return 1;

            RewardLog _exists_rewardLog = getRewardLogById(_rewardLog.RewardID);
            try
            {
                if (_exists_rewardLog == null)
                {
                    context.RewardLogs.AddObject(_rewardLog);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_rewardLog.helper != null && _rewardLog.helper.context != null)
                        context = _rewardLog.helper.context;
                    context.RewardLogs.ApplyCurrentValues(_rewardLog);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(RewardLog _rewardLog)
        {
            if (_rewardLog == null) return 1;

            try
            {
                context.RewardLogs.DeleteObject(_rewardLog);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(RewardLogHelper).ToString();
        }
        #endregion
    }
}