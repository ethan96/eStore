using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Reward
{
    public partial class CreditReward : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Presentation.eStoreContext.Current.User == null)
                    Response.Redirect("~/");
                BindReward();
            }
        }

        private void BindReward()
        {
            List<POCOS.RewardLog> rewardlogList = Presentation.eStoreContext.Current.Store.getRewardLogByUserId(Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.MiniSite);
            if (rewardlogList != null && rewardlogList.Count > 0)
            {
                //待 审批的积分
                POCOS.RewardLog totalRewardLog = new POCOS.RewardLog();
                totalRewardLog.UpdateDate = DateTime.Now;
                totalRewardLog.OrderNo = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Pending_Amount);
                totalRewardLog.TransactionType = -100;
                totalRewardLog.RewardPoint = rewardlogList.Where(p=>p.TransactionType == 0).Sum(p=>p.RewardPoint);
                rewardlogList.Add(totalRewardLog);
                //可使用的积分
                totalRewardLog = new POCOS.RewardLog();
                totalRewardLog.UpdateDate = DateTime.Now;
                totalRewardLog.OrderNo = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Reward_Available_Amount);
                totalRewardLog.TransactionType = -200;
                totalRewardLog.RewardPoint = rewardlogList.Where(p => p.TransactionType != 0 && p.TransactionType != -4 && p.TransactionType != -100).Sum(p => p.RewardPoint);
                rewardlogList.Add(totalRewardLog);
                gvReward.DataSource = rewardlogList;
            }
            gvReward.DataBind();
        }

        protected void gvReward_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator)
            {
                POCOS.RewardLog rewardLogItem = e.Row.DataItem as POCOS.RewardLog;
                HyperLink hyItemNo = e.Row.FindControl("hyItemNo") as HyperLink;

                if (rewardLogItem.TransactionType == -100 || rewardLogItem.TransactionType == -200)
                {
                    Label lblUpdateDate = e.Row.FindControl("lblUpdateDate") as Label;
                    lblUpdateDate.Text = rewardLogItem.OrderNo;
                    hyItemNo.Text = "";

                    if (rewardLogItem.TransactionType == -200)
                        e.Row.ForeColor = System.Drawing.Color.Red;
                    //amount 靠右显示
                    e.Row.Cells[0].CssClass = "right";
                    return;
                }
                
                if (rewardLogItem.Order != null)
                {
                    hyItemNo.Text = rewardLogItem.OrderNo;
                    hyItemNo.NavigateUrl = "~/Account/orderdetail.aspx?orderid=" + rewardLogItem.OrderNo;
                }
                else if (rewardLogItem.RewardGiftItem != null)
                {
                    hyItemNo.Text = esUtilities.CommonHelper.SubStringByBytes(rewardLogItem.RewardGiftItem.Name, 48);
                    hyItemNo.NavigateUrl = rewardLogItem.RewardGiftItem.ProductUrl;
                }
                if (rewardLogItem.RewardPoint < 0)
                {
                    Label lblPoint = e.Row.FindControl("lblPoint") as Label;
                    lblPoint.ForeColor = System.Drawing.Color.Red;
                }
                //状态信息
                Label lblStatus = e.Row.FindControl("lblStatus") as Label;
                lblStatus.Text = rewardLogItem.TransactionTypeX;
            }
        }
    }
}