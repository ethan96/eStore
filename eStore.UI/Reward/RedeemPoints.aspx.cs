using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Reward
{
    public partial class RedeemPoints : Presentation.eStoreBaseControls.eStoreBasePage
    {
        //用户总积分
        private decimal _userTotalPoint;
        public decimal UserTotalPoint
        {
            get { return _userTotalPoint; }
            set { _userTotalPoint = value; }
        }

        private string _redeemFont;
        public string RedeemFont
        {
            get {
                if (string.IsNullOrEmpty(_redeemFont))
                    _redeemFont = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Redeem_Item);
                return _redeemFont;
            }
            set { _redeemFont = value; }
        }
        
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current.User == null)
                    Response.Redirect("~/");

                BindRewardGiftItem();
            }

            ltUserRewPoints.Text = string.Format(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_User_RewardCondition)
                , Presentation.eStoreContext.Current.Store.getCultureGreetingName(Presentation.eStoreContext.Current.User.FirstName,Presentation.eStoreContext.Current.User.LastName), UserTotalPoint);
        }


        private void BindRewardGiftItem()
        {
            BindUserPoint();
            List<POCOS.RewardGiftItem> rewardGiftList = Presentation.eStoreContext.Current.Store.getAllPublishRewardGiftItem(Presentation.eStoreContext.Current.MiniSite);
            if (rewardGiftList != null && rewardGiftList.Count > 0)
                rptGiftItem.DataSource = rewardGiftList;
            rptGiftItem.DataBind();
        }

        private void BindUserPoint()
        {
            var con = Presentation.eStoreContext.Current.Store.getUserTotalPoint(Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.MiniSite);
            UserTotalPoint = (!con.Any() ? 0 : con.FirstOrDefault().Value);
        }

        protected void rptGiftItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.RewardGiftItem currentGiftItem = e.Item.DataItem as POCOS.RewardGiftItem;
                Image imgUrl = e.Item.FindControl("imgUrl") as Image;
                if (currentGiftItem.ImageUrl.StartsWith("http://"))
                    imgUrl.ImageUrl = currentGiftItem.ImageUrl;
                else
                    imgUrl.ImageUrl = "~/resource" + currentGiftItem.ImageUrl;

                Button btnRedeem = e.Item.FindControl("btnRedeem") as Button;
                btnRedeem.Text = RedeemFont;
                if (currentGiftItem.Cost > UserTotalPoint)
                {
                    btnRedeem.Enabled = false;
                    btnRedeem.Attributes.Add("style", "background-color: #D6D3CE;width: 111px;color:#848284;border:0;height: 20px;");
                }
                
            }
        }

        protected void btnRedeem_Click(object sender, EventArgs e)
        {
            Button btnRedeem = sender as Button;
            int itemNo = int.Parse(btnRedeem.CommandArgument);
            if (itemNo > 0)
            {
                POCOS.RewardGiftItem currentGiftItem = Presentation.eStoreContext.Current.Store.getRewardGiftItemById(itemNo);
                if (currentGiftItem != null)
                {
                    BindUserPoint();
                    if (UserTotalPoint >= currentGiftItem.Cost)
                    {
                        //消费积分 日志
                        POCOS.RewardLog currentRewardLog = new POCOS.RewardLog();
                        currentRewardLog.StoreId = Presentation.eStoreContext.Current.Store.storeID;
                        currentRewardLog.UserId = Presentation.eStoreContext.Current.User.UserID;
                        currentRewardLog.TransactionType = (int)POCOS.Enumerations.eStoreConfigure.RewardLogTransactionType.Redeemed;
                        currentRewardLog.GiftNo = currentGiftItem.ItemNo;
                        currentRewardLog.RewardPoint = 0 - currentGiftItem.Cost;
                        currentRewardLog.UpdateDate = DateTime.Now;
                        currentRewardLog.ActivityId = currentGiftItem.ActivityId;
                        currentRewardLog.UpdateBy = Presentation.eStoreContext.Current.User.UserID;
                        if (currentRewardLog.save() == 0)
                            Response.Redirect("~/Reward/CreditReward.aspx");
                        else
                            Presentation.eStoreContext.Current.AddStoreErrorCode("Failed.");
                    }
                    else
                    {
                        Presentation.eStoreContext.Current.AddStoreErrorCode("Enough points.");
                        BindRewardGiftItem();
                    }
                }
            }
            else
                Presentation.eStoreContext.Current.AddStoreErrorCode("Please refresh page.");
        }

    }
}