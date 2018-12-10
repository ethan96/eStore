using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.BusinessModules;

namespace eStore.UI.Cart
{
    public partial class myorders : Presentation.eStoreBaseControls.eStoreBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDefaultOrder();
                bindFonts();
            }

        }

        public void BindDefaultOrder()
        {
            if (Presentation.eStoreContext.Current.User != null 
                && Presentation.eStoreContext.Current.User.ordersX != null)
                this.gvmyorders.DataSource = (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                              orderby o.statusX, o.ConfirmedDate descending
                                              select o).ToList();

            this.gvmyorders.DataBind();
        }

        protected void bt_searchOrder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_orderNo.Text.Trim()) && Presentation.eStoreContext.Current.User != null
                && Presentation.eStoreContext.Current.User != null
                && Presentation.eStoreContext.Current.User.ordersX != null)
            {
              List<POCOS.Order> match= (from o in Presentation.eStoreContext.Current.User.actingUser.ordersX
                                              where o.OrderNo.Contains(tb_orderNo.Text.Trim())
                                              orderby o.statusX, o.ConfirmedDate descending
                                              select o).ToList();

                  if (match==null||match.Count == 0)
                {
                    try
                    {
                        POCOS.Order saporder = new POCOS.Order();
                        saporder.StoreID = Presentation.eStoreContext.Current.Store.storeID;
                        saporder.OrderNo = tb_orderNo.Text.Trim();
                        saporder.User = Presentation.eStoreContext.Current.User.actingUser;

                        SAPOrderTracking SAPOrderTracking = new SAPOrderTracking(saporder);
                        saporder = SAPOrderTracking.getStoreOrder();
                        if (saporder.UpdateBySAP)
                        {
                            match = new List<POCOS.Order>();
                            match.Add(saporder);
                        }
                        
                    }
                    catch (Exception)
                    {

                    }
                }
                 this.gvmyorders.DataSource=match;
                this.gvmyorders.DataBind();
            }
            else
            {
                BindDefaultOrder();
            }
        }

        protected void bindFonts()
        {
            bt_searchOrder.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Search);
        }
    }
}