using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{

    public partial class RewardGiftItemHelper : Helper
    {
        #region Business Read
        //根据Id 获取RewardLog 对象
        public RewardGiftItem getRewardGiftItemById(int itemNo)
        {
            if (itemNo == 0)
                return null;
            var rewardLogItem = (from r in context.RewardGiftItems
                                 where r.ItemNo == itemNo
                                select r).FirstOrDefault();

            if (rewardLogItem != null)
                rewardLogItem.helper = this;
            return rewardLogItem;
        }

        public List<RewardGiftItem> getAllRewardGiftItem(string storeId, string startdate = "", string enddate = "", string title = "", string publishStatus = "")
        {
            bool isStartDate = false; DateTime startDateTime = new DateTime();
            bool isEndDate = false; DateTime endDateTime = new DateTime();
            bool isPublish = false; bool status = true;
            if (!string.IsNullOrEmpty(startdate))
                isStartDate = DateTime.TryParse(startdate, out startDateTime);
            if (!string.IsNullOrEmpty(enddate))
            {
                isEndDate = DateTime.TryParse(enddate, out endDateTime);
                if (isEndDate)
                    endDateTime =  endDateTime.AddHours(24);
            }
            if (!string.IsNullOrEmpty(publishStatus))
                isPublish = bool.TryParse(publishStatus, out status);

            var rewardGiftItemList = (from g in context.RewardGiftItems
                      where g.StoreId == storeId 
                      && (isStartDate ? g.UpdateDate >= startDateTime : true) 
                      && (isEndDate ? g.UpdateDate < endDateTime : true) 
                      && (!string.IsNullOrEmpty(title) ? g.Name.ToUpper().Contains(title) : true) 
                      && (isPublish ? g.PublishStatus == status : true)
                      orderby g.ItemNo descending
                      select g).ToList();

            if (rewardGiftItemList != null)
            {
                foreach (var item in rewardGiftItemList)
                {
                    item.helper = this;
                }
            }
            
            return rewardGiftItemList;
        }

        //获取所有 publish的兑换产品
        public List<RewardGiftItem> getAllPublishRewardGiftItem(string storeId, int activityid = 0)
        {

            var rewardGiftItemList = (from g in context.RewardGiftItems
                                      where g.StoreId == storeId 
                                      && g.PublishStatus == true
                                      orderby g.ItemNo descending
                                      select g).ToList();

            if (activityid != 0)
                rewardGiftItemList = rewardGiftItemList.Where(c => c.ActivityId == activityid).ToList();

            if (rewardGiftItemList != null)
            {
                foreach (var item in rewardGiftItemList)
                {
                    item.helper = this;
                }
            }

            return rewardGiftItemList;
        }
        
        #endregion

        #region Creat Update Delete
        public int save(RewardGiftItem _rewardGiftItem)
        {
            if (_rewardGiftItem == null) return 1;

            RewardGiftItem _exists_rewardGiftItem = getRewardGiftItemById(_rewardGiftItem.ItemNo);
            try
            {
                if (_exists_rewardGiftItem == null)
                {
                    context.RewardGiftItems.AddObject(_rewardGiftItem);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    if (_rewardGiftItem.helper != null && _rewardGiftItem.helper.context != null)
                        context = _rewardGiftItem.helper.context;
                    context.RewardGiftItems.ApplyCurrentValues(_rewardGiftItem);
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

        public int delete(RewardGiftItem _rewardGiftItem)
        {
            if (_rewardGiftItem == null) return 1;

            try
            {
                if (_rewardGiftItem.helper != null && _rewardGiftItem.helper.context != null)
                    context = _rewardGiftItem.helper.context;
                context.RewardGiftItems.DeleteObject(_rewardGiftItem);
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
            return typeof(RewardGiftItemHelper).ToString();
        }
        #endregion
    }
}