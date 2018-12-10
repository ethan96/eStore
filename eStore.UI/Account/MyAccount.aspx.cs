using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Account
{
    public partial class MyAccount : Presentation.eStoreBaseControls.eStoreBasePage
    {
        public override bool OverwriteMasterPageFile
        {
            get
            {
                return false;
            }

        }

        private string welcomename = string.Empty;
        public string WelcomeName
        {
            get
            {
                if (string.IsNullOrEmpty(this.welcomename))
                {
                    var user = eStore.Presentation.eStoreContext.Current.User;
                    if (user != null)
                    {
                        string name = eStore.Presentation.eStoreContext.Current.Store.storeID == "AKR" ? eStore.Presentation.eStoreContext.Current.Store.getCultureFullName(user.FirstName, user.LastName) : user.FirstName;
                        this.welcomename = string.Format("Hi! {0}", eStore.Presentation.eStoreContext.Current.Store.FormatHonorific(name, eStore.Presentation.eStoreContext.Current.getStringSetting("Honorific")));
                    }
                    else
                        this.welcomename = "Hi!";
                }
                return this.welcomename;
            }
            set
            {
                this.welcomename = value;
            }
        }

        private string Org2Store(string org)
        {
            Dictionary<string, string>  dicOrg2Store = new Dictionary<string, string>() { { "EU10", "AEU" } };
            return dicOrg2Store.ContainsKey(org) ? dicOrg2Store[org] : string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.bindFonts();

                //Get quotation from Equotation.
                //Use URL parameter eq to get quoteID , and also store parameter should has 
                if (!string.IsNullOrEmpty(Request["eq"]) && Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("CanGeteQuotationQuote") == true)
                {
                    this.MyQuote1.Account = ((new APIControllers.AccountController())
                    .GetAccountQuote());

                    POCOS.DAL.EquotationHelper helper = new POCOS.DAL.EquotationHelper();
                    POCOS.Equotation.Quote quote = helper.GetQuotationAllByQuoteID(Request["eq"].ToString());//Get quote now

                    //Quotation [Attenstion email] should equal with [User ID], and also quotation org should match store ID (ex. org EU10 = AEU)
                    if (quote != null && quote.AttentionEmail == Presentation.eStoreContext.Current.User.actingUser.UserID && this.Org2Store(quote.Org) == Presentation.eStoreContext.Current.Store.profile.StoreID)
                    {
                        this.MyQuote1.eQuote = quote;
                        this.setStatus(string.Empty, string.Empty, POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(),string.Empty, false, false, true,false);
                        return;
                        //有 quote 就走 MyQuote.ascx
                    }
                    
                }
                lb_MyReward.Visible = Presentation.eStoreContext.Current.getBooleanSetting("EnableRewardSystem", false);

                if (Presentation.eStoreContext.Current.getBooleanSetting("iAblePointClub", false) == true)
                {
                    string iAblePoint = Presentation.eStoreContext.Current.getStringSetting("iAblePointURL");
                    if (!string.IsNullOrEmpty(iAblePoint))
                        ltiAblePoint.Text = string.Format(iAblePoint, Presentation.eStoreContext.Current.User.UserID, Presentation.eStoreContext.Current.User.authKey);
                }
                
                if (!string.IsNullOrEmpty(Request["go"]) && Request["go"].ToString().ToLower().Equals("reward"))
                {
                    this.setStatus(string.Empty, string.Empty, string.Empty, POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(), false, false, false, true);
                    return;
                }

                this.setStatus(POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(), string.Empty, string.Empty,string.Empty, true, false, false,false);
                lb_MyReward.Visible = Presentation.eStoreContext.Current.Store.getAllPublishRewardGiftItem(Presentation.eStoreContext.Current.MiniSite).Any();
            }
        }

        protected void lb_MySetting_Click(object sender, EventArgs e)
        {
            this.AddressBook1.CurrentUser = Presentation.eStoreContext.Current.User.actingUser;

            this.setStatus(POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(), string.Empty, string.Empty,string.Empty, true, false, false,false);
        }

        protected void lb_MyOrder_Click(object sender, EventArgs e)
        {
            this.MyOrder1.Orders = ((new APIControllers.AccountController())
                    .GetAccountOrder(null,null,true)
                    .Orders);

            this.setStatus(string.Empty, POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(), string.Empty, string.Empty, false, true, false,false);
        }

        protected void lb_MyQuote_Click(object sender, EventArgs e)
        {
            this.MyQuote1.Account = ((new APIControllers.AccountController())
                    .GetAccountQuote());
            
            this.setStatus(string.Empty, string.Empty, POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(),string.Empty, false, false, true, false);
        }

        protected void lb_MyReward_Click(object sender, EventArgs e)
        {
            this.setStatus(string.Empty, string.Empty, string.Empty, POCOS.Enumerations.eStoreConfigure.MyAccountCSS.on.ToString(), false, false, false, true);
        }

        private void setStatus(string setCSS, string orderCSS, string quoteCSS, string rewardCSS, bool address, bool order, bool quote, bool reward)
        {
            this.lb_MySetting.CssClass = setCSS;
            this.lb_MyOrder.CssClass = orderCSS;
            this.lb_MyQuote.CssClass = quoteCSS;
            this.lb_MyReward.CssClass = rewardCSS;
            this.AddressBook1.Visible = address;
            this.MyOrder1.Visible = order;
            this.MyQuote1.Visible = quote;
            this.MyReward1.Visible = reward;
        }

        private void bindFonts()
        {
            this.lb_MyOrder.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Order);
            this.lb_MyQuote.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Quote);
            this.lb_MySetting.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Account);
            this.lb_MyReward.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_My_Reward);
        }
    }
}