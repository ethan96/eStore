using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.V4
{
    public partial class MyOrder : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public List<Models.Order> Orders 
        {
            set
            {
                if (value != null && value.Count > 0)
                {
                    this.rpMyOrder.Visible = true;
                    this.rpMyOrder.DataSource = value;
                    this.rpMyOrder.DataBind();
                }
                else
                {
                    this.rpMyOrder.Visible = false;
                }
            }
        }

        private bool? statusvisible = null;
        public bool StatusVisible
        {
            get
            {
                if (this.statusvisible.HasValue == false)
                    this.statusvisible = (Presentation.eStoreContext.Current.Store != null && Presentation.eStoreContext.Current.Store.profile.getBooleanSetting("Can_See_Order_Status") == true) ? true : false;
                return this.statusvisible.Value;
            }
        }

        private string reorder = string.Empty;
        public string ReOrder
        {
            get
            {
                if (string.IsNullOrEmpty(this.reorder))
                    this.reorder = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Order_ReOrder);
                return this.reorder;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddl_period.Items.Clear();
                ddl_period.Items.Add(new ListItem(Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.eStore_Select), "0"));
                ddl_period.Items.Add(new ListItem("1-3 month", "1"));
                ddl_period.Items.Add(new ListItem("4-6 month", "2"));
                ddl_period.Items.Add(new ListItem("6-12 month", "3"));
                ddl_period.Items.Add(new ListItem("12-24 month", "4"));
                ddl_period.SelectedIndex = 0;

                this.lb_searchOrder.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search);
                this.revOrder.Text = Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Invalid_Order_Number);
            }
        }

        protected void rpMyOrder_OnItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ReOrder")
            {
                POCOS.Order currentdorder = (from order in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                             where order.OrderNo == e.CommandArgument.ToString()
                                             select order).FirstOrDefault();

                //Go to SAP to get order
                //OrderDetail.ascx.cs has same code
                if (currentdorder == null)
                {
                    POCOS.Order saporder = new POCOS.Order();
                    saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                    saporder.OrderNo = e.CommandArgument.ToString();
                    saporder.User = Presentation.eStoreContext.Current.User.actingUser;
                    BusinessModules.SAPOrderTracking SAPOrderTracking = new BusinessModules.SAPOrderTracking(saporder, -360);
                    saporder = SAPOrderTracking.getStoreOrder();
                    if (saporder.UpdateBySAP)
                        currentdorder = saporder;
                }

                if (currentdorder != null)
                {
                    Presentation.eStoreContext.Current.User.actingUser.reOrder(currentdorder);
                    Presentation.eStoreContext.Current.UserShoppingCart = Presentation.eStoreContext.Current.User.actingUser.shoppingCart;
                    Response.Redirect("~/Cart/Cart.aspx");
                }
            }
        }

        protected void lb_searchOrder_Click(object sender, EventArgs e)
        {
            this.Orders = ((new APIControllers.AccountController())
                .GetAccountOrderBySearch(tb_orderNo.Text.Trim(), ddl_period.SelectedItem.Value)
                .Orders);
        }
    }
}