using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ChangeCurrency : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public decimal defaultListPrice {
            get {
                decimal rlt = 0;
                decimal.TryParse(this.hlistprice.Value, out rlt);
                return rlt;

        }
            set {
                this.hlistprice.Value = value.ToString() ;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //BindData();
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindData();
            base.OnPreRender(e);
        }
        public void BindData()
        {
            if (defaultListPrice == 0)
            {
                this.Visible = false;
                return;
            }
            switch (Presentation.eStoreContext.Current.Store.profile.StoreCurrencies.Count)
            {
                case 0:
                 
                case 1:
                    this.Visible = false;
                    break;
                default:
                    this.rblCurrencies.DataSource = (from sc in Presentation.eStoreContext.Current.Store.profile.StoreCurrencies
                                                     select sc.Currency);
                    this.rblCurrencies.DataTextField = "CurrencyID";
                    this.rblCurrencies.DataValueField = "CurrencyID";
                    this.rblCurrencies.DataBind();
                    this.rblCurrencies.SelectedValue = Presentation.eStoreContext.Current.CurrentCurrency.CurrencyID;

                    if (this.defaultListPrice > 0)
                    {
                        decimal preexchagevalue = Presentation.eStoreContext.Current.Store.getCurrencyExchangeValue(defaultListPrice, Presentation.eStoreContext.Current.CurrentCurrency);
                        if (preexchagevalue > 0)
                        {
                            this.preExchange.Text = Presentation.Product.ProductPrice.FormartPriceWithoutDecimal(preexchagevalue, Presentation.eStoreContext.Current.CurrentCurrency);
                        }
                        else
                            this.preExchange.Text = string.Empty;
                    }
                    else
                        this.preExchange.Text = string.Empty;
                    break;
            }
        }

    }
}