using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.BusinessModules.Reward
{
    public class RewardPointFromUnica : RewardPointBase, IRewardPoint
    {
        private Dictionary<string, eStore.BusinessModules.tw.com.advantech.unica.AkrPoint> UserPointTemp =
            new Dictionary<string, tw.com.advantech.unica.AkrPoint>();
        private eStore.BusinessModules.tw.com.advantech.unica.AkrPoint[] _allUserAndPoint = null;
        protected eStore.BusinessModules.tw.com.advantech.unica.AkrPoint[] allUserAndPoint
        {
            get
            {
                if (_allUserAndPoint == null)
                {
                    try
                    {
                        _allUserAndPoint = new tw.com.advantech.unica.AkrPoint[]{};
                        eStore.BusinessModules.tw.com.advantech.unica.ExtApi api = new tw.com.advantech.unica.ExtApi();
                        _allUserAndPoint = api.GetAKRLoyaltyList("eStoreProduction","");
                    }
                    catch (Exception ex)
                    {
                        eStoreLoger.Error("get User Point from unica error", "", "", "", ex);
                    }

                }
                return _allUserAndPoint;
            }
        }

        private eStore.BusinessModules.tw.com.advantech.unica.AkrPoint getUserPoint(string userid)
        {
            if (!UserPointTemp.Keys.Contains(userid))
            {
                try
                {
                    var _userAndPoint = new tw.com.advantech.unica.AkrPoint[] { };
                    eStore.BusinessModules.tw.com.advantech.unica.ExtApi api = new tw.com.advantech.unica.ExtApi();
                    _userAndPoint = api.GetAKRLoyaltyList("eStoreProduction", userid);
                    foreach (var c in _userAndPoint)
                        UserPointTemp.Add(c.Email, c);
                    return UserPointTemp.FirstOrDefault(c => c.Key.Equals(userid, StringComparison.OrdinalIgnoreCase)).Value;;
                }
                catch (Exception ex)
                {
                    eStoreLoger.Error("get User Point from unica error", "", "", "", ex);
                }
            }
            return new tw.com.advantech.unica.AkrPoint { Email = userid, Points = -1};
        }


        public override Dictionary<POCOS.RewardActivity, decimal> getUserAllPoint(string userId, POCOS.MiniSite minisite = null)
        {
            var item = getUserPoint(userId);
            if (rewardActivities != null && rewardActivities.Any())
            {
                var items = new Dictionary<POCOS.RewardActivity, decimal>();
                items.Add(rewardActivities.FirstOrDefault(), item == null ? 0 : (decimal)item.Points);
                return items;
            }
            return new Dictionary<POCOS.RewardActivity, decimal>();
        }

        public override Dictionary<POCOS.RewardActivity, decimal> getUserTotalPoint(string userId, POCOS.MiniSite minisite = null)
        {
            Dictionary<eStore.POCOS.RewardActivity, decimal> ls = new Dictionary<eStore.POCOS.RewardActivity, decimal>();
            if (rewardActivities != null && rewardActivities.Any())
            {
                var pointitem = getUserPoint(userId);
                var item = rewardActivities.FirstOrDefault();
                ls.Add(item, (pointitem == null ? 0 : (decimal)pointitem.Points) + (new RewardLogHelper()).getUserTotalConsumptionPoint(store.StoreID, userId, item.Id));
            }
            return ls;
        }


        public override List<POCOS.RewardLog> getRewardLogByRewardApproval(string startdate = "", string enddate = "", string orderNo = "", string userId = "")
        {
            var ls = (from v in allUserAndPoint
                      where string.IsNullOrEmpty(userId) || v.Email.Equals(userId,StringComparison.OrdinalIgnoreCase)
                select new POCOS.RewardLog
                {
                    UpdateDate = DateTime.Now,
                    UserId = v.Email,
                    RewardPoint = (decimal)v.Points,
                    TransactionType = 1,
                    StoreId = this.store.StoreID,
                    SourceTemp = "Unica"
                }).ToList();
            return ls;
        }

        public override List<POCOS.RewardLog> getAllRewardLogByCondition(string startdate = "", string enddate = "", string userId = "")
        {
            var lsDBLog = base.getAllRewardLogByCondition(startdate, enddate, userId);
            var lsUnicaLog = this.getRewardLogByRewardApproval(startdate, enddate, "", userId);
            lsDBLog.AddRange(lsUnicaLog);
            return lsDBLog.OrderByDescending(c => c.RewardID).ThenBy(c => c.UserId).ToList();
        }
    }
}
