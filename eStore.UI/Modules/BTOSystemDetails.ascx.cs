using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class BTOSystemDetails : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private POCOS.BTOSystem _BTOSystem;

        private string type { get { return Request["source"] ?? string.Empty; } }
        private int ItemNO
        {
            get
            {
                if (string.IsNullOrEmpty(Request["ItemNO"])) { return -1; }
                else
                {
                    int rlt;
                    if (int.TryParse(Request["ItemNO"], out rlt))
                        return rlt;
                    else
                        return -1;
                }
            }
        }
        public POCOS.BTOSystem BTOSystem
        {
            get
            {
                if (_BTOSystem == null)
                {
                    if (!string.IsNullOrEmpty(type) && ItemNO > -1)
                    {
                        POCOS.Cart cart = null;
                        if (type.ToLower() == "cart")
                        {
                            cart = Presentation.eStoreContext.Current.UserShoppingCart;
                        }
                        else if (type.ToLower() == "quotation")
                        {
                            cart = Presentation.eStoreContext.Current.Quotation.cartX;
                        }
                        if (cart != null)
                        {
                            POCOS.CartItem ci = cart.getItem(ItemNO);
                            if (ci != null && ci.BTOSystem != null)
                                _BTOSystem = ci.BTOSystem;
                        }
                    }

                }
                return _BTOSystem;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null || Presentation.eStoreContext.Current.User.actingRole != POCOS.User.Role.Employee)
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("You don't have enough permissions to access this page", null, true);
                return;
            }
            base.OnInit(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
                bindFonts();
            }
        }

        private void BindData()
        {
            if (BTOSystem != null && BTOSystem.BTOSConfigs != null)
            {
                this.rpBTOSConfig.DataSource = BTOSystem.BTOSConfigsWithoutNoneItems;
                this.rpBTOSConfig.DataBind();
            }
        }
        protected void btnAddAddtionalProduct_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtAddtionalProduct.Text) && !string.IsNullOrEmpty(this.txtAddtionalProductQty.Text))
            {
                POCOS.Part part = Presentation.eStoreContext.Current.Store.getPart(this.txtAddtionalProduct.Text);
                int qty;
                if (part != null && int.TryParse(this.txtAddtionalProductQty.Text, out qty))
                {
                    BTOSystem.addNoneCTOSItem(part, 1, 0m, qty);
                    BTOSystem.reconcile();
                    if (type.ToLower() == "cart")
                    {
                        Presentation.eStoreContext.Current.UserShoppingCart.reconcile();
                        Presentation.eStoreContext.Current.UserShoppingCart.save();
                    }
                    else if (type.ToLower() == "quotation")
                    {
                        Presentation.eStoreContext.Current.Quotation.cartX.reconcile();
                        Presentation.eStoreContext.Current.Quotation.save();
                    }
                    BindData();
                }
                else
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode("Can't find Product");
                }
            }
            else
            {
                Presentation.eStoreContext.Current.AddStoreErrorCode("Can't find Product");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem sbcitem in rpBTOSConfig.Items)
            {
                if (sbcitem.ItemType == ListItemType.AlternatingItem || sbcitem.ItemType == ListItemType.Item)
                {
                    LinkButton btnRemoveComponent = (LinkButton)sbcitem.FindControl("btnRemoveComponent");


                    eStore.Presentation.eStoreControls.Repeater rpBTOSConfigDetails = (eStore.Presentation.eStoreControls.Repeater)sbcitem.FindControl("rpBTOSConfigDetails");

                    POCOS.BTOSConfig config = BTOSystem.BTOSConfigs.FirstOrDefault(con => con.ConfigID.ToString() == btnRemoveComponent.CommandArgument.ToString());
                    if (config != null)
                    {
                        foreach (RepeaterItem detail in rpBTOSConfigDetails.Items)
                        {
                            if (detail.ItemType == ListItemType.Item || detail.ItemType == ListItemType.AlternatingItem)//update detail price and qty
                            {
                                int detailqty;
                                decimal detailprice;
                                TextBox txtdetailQty = (TextBox)detail.FindControl("txtQty");
                                TextBox txtdetailprice = (TextBox)detail.FindControl("txtUnitPrice");

                                LinkButton btnBTOSConfigDetail = (LinkButton)detail.FindControl("btnBTOSConfigDetail");
                                POCOS.BTOSConfigDetail _BTOSConfigDetail = config.BTOSConfigDetails.FirstOrDefault(d => d.ID.ToString() == btnBTOSConfigDetail.CommandArgument.ToString());
                                if (_BTOSConfigDetail != null && config.Qty > 0)
                                {
                                    if (int.TryParse(txtdetailQty.Text, out detailqty) && decimal.TryParse(txtdetailprice.Text, out detailprice)
                                        && detailprice > 0
                                        && detailqty > 0
                                        && detailqty >= config.Qty && detailqty % config.Qty == 0)
                                    {
                                        _BTOSConfigDetail.AdjustedPrice = eStore.Presentation.Product.ProductPrice.changeToStorePrice(detailprice,Presentation.eStoreContext.Current.CurrentCurrency);
                                        _BTOSConfigDetail.Qty = detailqty / config.Qty;
                                    }
                                    else
                                    {

                                        object[] args;
                                        args = new object[1];
                                        args[0] = _BTOSConfigDetail.SProductID;
                                        Presentation.eStoreContext.Current.AddStoreErrorCode("Invalid price or wrong ordering quantity at BTOS component, {0}!", args);
                                        BindData();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            BTOSystem.reconcile();

            if (type.ToLower() == "cart")
            {
                Presentation.eStoreContext.Current.UserShoppingCart.save();
                Presentation.eStoreContext.Current.UserShoppingCart.reconcile();
                Response.Redirect("~/Cart/Cart.aspx");
            }
            else if (type.ToLower() == "quotation")
            {
                Presentation.eStoreContext.Current.Quotation.save();
                Presentation.eStoreContext.Current.Quotation.cartX.reconcile();
                Response.Redirect("~/Quotation/quote.aspx");
            }
            else
            {
                Response.Redirect("~/");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (type.ToLower() == "cart")
            {

                Response.Redirect("~/Cart/Cart.aspx");
            }
            else if (type.ToLower() == "quotation")
            {
                Response.Redirect("~/Quotation/quote.aspx");
            }
            else
            {
                Response.Redirect("~/");
            }
        }
        protected void rpBTOSConfig_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                BTOSystem.removeNoneCTOSItem(int.Parse(e.CommandArgument.ToString()));
                BindData();
            }
        }

        protected void rpBTOSConfigDetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                LinkButton btnRemoveComponent =(LinkButton) e.Item.Parent.Parent.FindControl("btnRemoveComponent");

                POCOS.BTOSConfig config = BTOSystem.BTOSConfigs.FirstOrDefault(con => con.ConfigID.ToString() == btnRemoveComponent.CommandArgument.ToString());
                if (config != null)
                {
                    POCOS.BTOSConfigDetail configdetail = config.BTOSConfigDetails.FirstOrDefault(d => d.ID.ToString() == e.CommandArgument.ToString());
                    if (configdetail != null)
                    {
                        config.BTOSConfigDetails.Remove(configdetail);

                        if (config.BTOSConfigDetails.Count == 0)
                        {
                            BTOSystem.BTOSConfigs.Remove(config);
                        }
                    }
                }
            }


            BTOSystem.removeNoneCTOSItem(int.Parse(e.CommandArgument.ToString()));
            BindData();

        }

        protected void rpBTOSConfigDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.BTOSConfigDetail btdetail = e.Item.DataItem as POCOS.BTOSConfigDetail;
                if (btdetail.MainProduct ?? false)
                {
                    LinkButton btnBTOSConfigDetail = e.Item.FindControl("btnBTOSConfigDetail") as LinkButton;
                    btnBTOSConfigDetail.Visible = false;
                }
                TextBox txtQty = (TextBox)e.Item.FindControl("txtQty");
                txtQty.Text =  (btdetail.Qty * btdetail.BTOSConfig.Qty).ToString("n0");

                if (btdetail.partX == null && btdetail.BTOSConfig.isWarrantyConfig()) //没有Part 并且为Warranty
                {
                    txtQty.Enabled = false;
                    TextBox txtUnitPrice = e.Item.FindControl("txtUnitPrice") as TextBox;
                    txtUnitPrice.Enabled = false;
                }
            }
        }

        protected void bindFonts()
        {
            btnAddAddtionalProduct.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Add_Component);
            btnSave.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Save);
            txtAddtionalProduct.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_PartNumber);
            txtAddtionalProductQty.ToolTip = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Table_Qty);
            btnCancel.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Cart_Cancel);
        }

        
        
    }
}