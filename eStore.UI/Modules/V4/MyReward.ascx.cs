using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace eStore.UI.Modules.V4
{
    public partial class MyReward : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private List<POCOS.RewardLog> _alllog = null;
        public List<POCOS.RewardLog> alllog
        {
            get 
            {
                if(_alllog == null)
                    _alllog = Presentation.eStoreContext.Current.Store.getRewardLogByUserId(Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.MiniSite);
                return _alllog; 
            }
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (eStore.Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem"))
            {
                if (!IsPostBack)
                    bindCustomerRewardInfor();
            }
        }

        public void bindCustomerRewardInfor()
        {
            var activitypoint = eStore.Presentation.eStoreContext.Current.Store.getUserTotalPoint(eStore.Presentation.eStoreContext.Current.User.actingUser.UserID
                    , eStore.Presentation.eStoreContext.Current.MiniSite).FirstOrDefault();
            
            var allpoint = eStore.Presentation.eStoreContext.Current.Store.getUserAllPoint(eStore.Presentation.eStoreContext.Current.User.actingUser.UserID
                    , eStore.Presentation.eStoreContext.Current.MiniSite).FirstOrDefault();
            
            if (activitypoint.Key != null && allpoint.Key != null && activitypoint.Key.Id == allpoint.Key.Id)
            {

                ltAblePoints.Text = activitypoint.Value.ToString();

                List<POCOS.RewardLog> cousumelogList = alllog.Where(c => c.GiftNo != null && c.RewardPoint < 0).ToList();
                //Replace status text
                foreach (var rl in cousumelogList)
                {
                    string status = Presentation.eStoreContext.Current.getStringSetting(rl.TransactionTypeX);
                    if (!string.IsNullOrEmpty(status)) rl.TransactionTypeX = status;
                }

                rpAblePoints.DataSource = cousumelogList;
                rpAblePoints.DataBind();

                var orderls = (new APIControllers.AccountController()).GetAccountOrder(allpoint.Key.StartDate, allpoint.Key.EndDate,true).Orders;
                rpOrderList.DataSource = orderls;
                rpOrderList.DataBind();
                ltOrderDate.Text = string.Format("{0} ~ {1}", allpoint.Key.StartDate.Value.ToString("MM/dd/yyyy"), allpoint.Key.EndDate.Value.ToString("MM/dd/yyyy"));

                ltTotalAmountTop.Text = ltTotalOrderAmount.Text = orderls.Sum(c => c.subTotalX).ToString();
                ltTotalRewardAmount.Text = allpoint.Value.ToString();


                List<POCOS.RewardGiftItem> rewardGiftList = Presentation.eStoreContext.Current.Store.getAllPublishRewardGiftItem(Presentation.eStoreContext.Current.MiniSite).OrderBy(c=>c.ItemNo).ToList();
                if (rewardGiftList != null && rewardGiftList.Count > 0)
                    rptGiftItem.DataSource = rewardGiftList;
                rptGiftItem.DataBind();
            }
        }

        protected void rpAblePoints_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.RewardLog log = e.Item.DataItem as POCOS.RewardLog;
                if (log != null)
                {
                    if (log.RewardGiftItem != null)
                    {
                        Literal ltRewardName = e.Item.FindControl("ltRewardName") as Literal;
                        if(ltRewardName != null)
                            ltRewardName.Text = esUtilities.CommonHelper.SubStringByBytes(log.RewardGiftItem.Name, 48);
                    }

                    Literal ltType = e.Item.FindControl("ltType") as Literal;
                    Literal ltPoint = e.Item.FindControl("ltPoint") as Literal;
                    ltType.Text = log.TransactionTypeX;
                    ltPoint.Text = log.RewardPoint.ToString();
                    POCOS.RewardLog backlog = alllog.FirstOrDefault(c => !string.IsNullOrEmpty(c.OrderNo) && log.RewardID.ToString() == c.OrderNo);
                    if (backlog != null)
                    {
                        ltType.Text += string.Format("<br /><span class=\"colorRed\">{0}</span>", backlog.TransactionTypeX);
                        ltPoint.Text += string.Format("<br /><span class=\"colorRed\">+{0}</span>", backlog.RewardPoint.ToString());
                    }
                }
            }
        }

        protected void rptGiftItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.RewardGiftItem giftitem = e.Item.DataItem as POCOS.RewardGiftItem;
                if (giftitem != null)
                {
                    TextBox hdRedeemCount = e.Item.FindControl("hdRedeemCount") as TextBox;
                    hdRedeemCount.Attributes.Add("item", giftitem.StoreId + giftitem.ItemNo.ToString());
                    hdRedeemCount.Attributes.Add("cost", giftitem.Cost.Value.ToString());
                }
            }
        }

        protected void btsendRemeed_Click(object sender, EventArgs e)
        {
            var activitypoint = eStore.Presentation.eStoreContext.Current.Store.getUserTotalPoint(eStore.Presentation.eStoreContext.Current.User.actingUser.UserID
                    , eStore.Presentation.eStoreContext.Current.MiniSite).FirstOrDefault();
            List<GiftX> ls = new List<GiftX>();

            foreach (RepeaterItem ri in this.rptGiftItem.Items)
            {
                if (ri.ItemType == ListItemType.Item || ri.ItemType == ListItemType.AlternatingItem)
                {
                    TextBox hdRedeemCount = ri.FindControl("hdRedeemCount") as TextBox;
                    if (hdRedeemCount != null)
                    {
                        int cost = 0;
                        int.TryParse(hdRedeemCount.Attributes["cost"], out cost);
                        int count = 0;
                        int.TryParse(hdRedeemCount.Text.Trim(), out count);
                        int itemNo = 0;
                        int.TryParse(hdRedeemCount.Attributes["item"].Replace(activitypoint.Key.StoreId,""), out itemNo);
                        if (cost > 0 && count > 0)
                            ls.Add(new GiftX() { itemNo = itemNo, cost = cost, count = count });
                    }
                }
            }
            if (ls.Any())
            {
                var total = ls.Sum(c => c.cost * c.count);
                if ((decimal)total > activitypoint.Value)
                {
                    eStore.Presentation.eStoreContext.Current.AddStoreErrorCode("Error!");
                    return;
                }
                else
                {
                    foreach (var g in ls)
                    {
                        for (int i = 1; i <= g.count; i++)
                        {
                            POCOS.RewardLog currentRewardLog = new POCOS.RewardLog();
                            currentRewardLog.StoreId = Presentation.eStoreContext.Current.Store.storeID;
                            currentRewardLog.UserId = Presentation.eStoreContext.Current.User.UserID;
                            currentRewardLog.TransactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Redeemed;
                            currentRewardLog.GiftNo = g.itemNo;
                            currentRewardLog.RewardPoint = 0 - g.cost;
                            currentRewardLog.UpdateDate = DateTime.Now;
                            currentRewardLog.ActivityId = activitypoint.Key.Id;
                            currentRewardLog.UpdateBy = Presentation.eStoreContext.Current.User.UserID;
                            currentRewardLog.save();                                
                        }
                    }
                    bindCustomerRewardInfor();
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "fbShowSuccessDialog", "$(document).ready(function(){$('#showloyalty_thankU').click()});", true);
                }
            }
        }

        protected class GiftX
        {
            public int itemNo { get; set; }
            public decimal cost { get; set; }
            public int count { get; set; }
        }
    }
}