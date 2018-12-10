using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.Presentation;
using eStore.POCOS.DAL;
using System.Web.UI.HtmlControls;

namespace eStore.UI.Modules
{
    public partial class CartContent : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        //cart content中是否显示cart中产品的error message
        private bool _showCartMessage = false;
        public bool ShowCartMessage
        {
            get { return _showCartMessage; }
            set { _showCartMessage = value; }
        }

        public Object CartDataSource { get; set; }
        bool? _ShowATP = null;
        public Boolean showATP
        {
            get
            {
                if (_ShowATP == null)
                {
                    if (Presentation.eStoreContext.Current.User == null)
                        _ShowATP = false;
                    else
                        _ShowATP = Presentation.eStoreContext.Current.User.actingUser.hasRight(POCOS.User.ACToken.ATP);
                }
                return _ShowATP ?? false;
            }
        }
        private POCOS.Cart cart
        {
            get
            {
                if (CartDataSource is POCOS.Cart)
                {
                    CartSourceType = "Cart";
                    return (POCOS.Cart)CartDataSource;
                }
                else if (CartDataSource is POCOS.Quotation)
                {
                    CartSourceType = "Quotation";
                    return ((POCOS.Quotation)CartDataSource).cartX;
                }
                else if (CartDataSource is POCOS.Order)
                {
                    CartSourceType = "Order";
                    return ((POCOS.Order)CartDataSource).cartX;
                }
                else
                    return null;

            }
        }
        public string CartSourceType
        {
            get;
            set;
        }
        protected override void OnInit(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            if (!base.ChildControlsCreated)
            {
                base.CreateChildControls();
                this.showWarranty();
                base.ChildControlsCreated = true;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                applyPromotionCode();
                BindData(cart);

                var cartChangeMessage = eStore.Presentation.CartUtility.CheckAndMegerCartItem(cart, ShowCartMessage);
                if (!string.IsNullOrEmpty(cartChangeMessage))
                {
                    lblCartItemMessage.Visible = true;
                    lblCartItemMessage.Text = cartChangeMessage;
                    cart.cartItemChangedMessage.Clear();
                }
            }
            else
                lblCartItemMessage.Visible = false;
        }

        private void BindData(POCOS.Cart cart)
        {
            if (cart == null || cart.cartItemsX.Count == 0)
            {
                Response.Redirect("~/Cart/emptycart.aspx");
            }
            if (showATP)
            {
                cart.prefetchCartItemATP();
            }

            this.rpCartContent.DataSource = cart.cartItemsX;
            this.rpCartContent.DataBind();
        }

        public void UpdateCartContent()
        {
            foreach (RepeaterItem item in rpCartContent.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    CartItem _ci;
                    Button hItemNo = (Button)item.FindControl("btDelete");
                    _ci = cart.getItem(int.Parse(hItemNo.CommandArgument));

                    if (_ci == null)
                        continue;
                    else
                    {
                        bool cartitemupdated = false;
                        TextBox txtQty = (TextBox)item.FindControl("txtQty");
                        if (showATP)
                        {
                            decimal unitprice = 0;
                            TextBox txtUnitPrice = (TextBox)item.FindControl("txtUnitPrice");
                            string cartuntiprice = eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(_ci.UnitPrice).value.ToString("f2");
                            if (decimal.TryParse(txtUnitPrice.Text.Trim(), out unitprice) && txtUnitPrice.Text.Trim() != cartuntiprice)
                            {
                                _ci.updateUnitPrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(unitprice, eStore.Presentation.eStoreContext.Current.CurrentCurrency));
                                if (_ci.type == POCOS.Product.PRODUCTTYPE.CTOS && _ci.BTOSystem != null)
                                    _ci.BTOSystem.updatePrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(unitprice, eStore.Presentation.eStoreContext.Current.CurrentCurrency));
                                else if (_ci.type == POCOS.Product.PRODUCTTYPE.BUNDLE && _ci.bundleX != null)
                                    _ci.bundleX.updatePrice(eStore.Presentation.Product.ProductPrice.changeToStorePrice(unitprice, eStore.Presentation.eStoreContext.Current.CurrentCurrency));
                                cartitemupdated = true;
                            }
                        }
                        int qty = 0;
                        if (!int.TryParse(txtQty.Text, out qty) || qty <= 0)
                            qty = 1;
                        if (_ci.Qty != qty)
                        {
                            _ci.updateQty(qty);
                            cartitemupdated = true;
                        }

                        _ci.PromotionMessage = string.Empty;
                        _ci.PromotionStrategy = null;
                        cart.updateItem(_ci);
                    }
                }
            }
            applyPromotionCode();
            reconcileDataSource();
            saveDataSource();
            Presentation.eStoreContext.Current.BusinessGroup = cart.businessGroup;
            BindData(cart);
        }

        private void saveDataSource()
        {
            if (CartDataSource is POCOS.Quotation)
                ((POCOS.Quotation)CartDataSource).save();
            else if ((CartDataSource is POCOS.Order))
                ((POCOS.Order)CartDataSource).save();
            else
                cart.save();
        }

        private void applyPromotionCode()
        {
            if (CartDataSource is POCOS.Quotation)
                ((POCOS.Quotation)CartDataSource).applyPromotionCode(Presentation.eStoreContext.Current.User.actingUser, string.Empty);
            else if ((CartDataSource is POCOS.Order))
                ((POCOS.Order)CartDataSource).applyPromotionCode(Presentation.eStoreContext.Current.User.actingUser, string.Empty);
            else
                cart.applyPromotionCode(string.Empty);
        }

        private void reconcileDataSource()
        {
            if (CartDataSource is POCOS.Quotation)
                ((POCOS.Quotation)CartDataSource).reconcile();
            else if ((CartDataSource is POCOS.Order))
                ((POCOS.Order)CartDataSource).reconcile();
            else
                cart.reconcile();
        }

        protected void rpCartContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.CartItem cartitem = (POCOS.CartItem)e.Item.DataItem;

                //Check if it is dependency product, hide it's delete button
                if (cartitem.RelatedType.HasValue && cartitem.RelatedType.Value == 1000) //1000 is dependency product
                {
                    Control c = e.Item.FindControl("btDelete");
                    if (c != null)
                        ((Button)c).Visible = false;
                }

                TextBox txtUnitPrice = e.Item.FindControl("txtUnitPrice") as TextBox;
                txtUnitPrice.Text = eStore.Presentation.Product.ProductPrice.fixExchangeCurrencyPrice(cartitem.UnitPrice).value.ToString("f2");

                if (cartitem.BTOSystem != null)
                {
                    eStore.Presentation.eStoreControls.Repeater rp = (eStore.Presentation.eStoreControls.Repeater)e.Item.FindControl("rpBTOSConfig");

                    rp.DataSource = (from bci in cartitem.BTOSystem.BTOSConfigsWithoutNoneItems
                                     select new
                                     {
                                         bci.ConfigID
                                         ,
                                         bci.isBuildin
                                         ,
                                         bci.atp
                                         ,
                                         bci.CategoryComponentDesc
                                         ,
                                         bci.OptionComponentDesc
                                          ,
                                         AdjustedPrice =
                                         (
                                            bci.BTOSConfigDetails != null && bci.BTOSConfigDetails.Count > 0 ? string.Join("<br>", (
                                            from bcd in bci.BTOSConfigDetails
                                            orderby bcd.SProductID
                                            select Presentation.Product.ProductPrice.FormartPrice(bcd.AdjustedPrice)
                                            ).ToArray()
                                            ) : string.Empty
                                         )
                                          ,
                                         Qty = bci.BTOSConfigDetails != null && bci.BTOSConfigDetails.Count > 0 ? string.Join("<br>", (
                                         from bcd in bci.BTOSConfigDetails
                                         orderby bcd.SProductID
                                         select (bcd.Qty * bci.Qty * cartitem.Qty).ToString()
                                         ).ToArray()
                                         ) : (bci.Qty * cartitem.Qty).ToString()
                                         ,
                                         BTOSConfigDetails = bci.BTOSConfigDetails.OrderBy(bcd => bcd.SProductID)
                                         ,
                                         subtotal = cartitem.Qty * bci.Qty * bci.AdjustedPrice
                                         ,
                                         isWarrantyPart = bci.isWarrantyConfig(),
                                         MOQ = bci.parts.Any() ? bci.parts.FirstOrDefault().Key.MininumnOrderQty : null,
                                         ABCInd = bci.parts != null && bci.parts.Any() ? bci.parts.FirstOrDefault().Key.ABCInd : ""
                                     });
                    rp.DataBind();
                }
                if (cartitem.bundleX != null)
                {
                    eStore.Presentation.eStoreControls.Repeater rpBundleItem = (eStore.Presentation.eStoreControls.Repeater)e.Item.FindControl("rpBundleItem");
                    
                    rpBundleItem.DataSource = cartitem.bundleX.BundleItems;
                    rpBundleItem.DataBind();
                }

                // 单买标准品加 Warranty
                #region
                HtmlTableRow warrantyrow = (HtmlTableRow)e.Item.FindControl("warrrantyItem");
                HyperLink hlAddWarranty = (HyperLink)e.Item.FindControl("hlAddWarranty");
                if (cartitem.hasWarrantyItem())
                {
                    hlAddWarranty.Visible = false;
                    if (cartitem.type == POCOS.Product.PRODUCTTYPE.STANDARD)
                    {
                        warrantyrow.Visible = true;
                        Literal lproductNameX = (Literal)warrantyrow.FindControl("lproductNameX");
                        lproductNameX.Text = cartitem.warrantyItemX.productNameX;
                        Literal lDescription = (Literal)warrantyrow.FindControl("lDescription");
                        lDescription.Text = cartitem.warrantyItemX.Description;
                        if (showATP)
                        {
                            Literal lavailableDate = (Literal)warrantyrow.FindControl("lavailableDate");
                            lavailableDate.Text = eStore.Presentation.eStoreLocalization.Date(cartitem.warrantyItemX.atp.availableDate);
                            Literal lavailableQty = (Literal)warrantyrow.FindControl("lavailableQty");
                            lavailableQty.Text = cartitem.warrantyItemX.atp.availableQty.ToString();
                            warrantyrow.Cells[3].Visible = true;
                            warrantyrow.Cells[4].Visible = true;
                            warrantyrow.Cells[5].Visible = true;
                        }
                        else
                        {
                            warrantyrow.Cells[3].Visible = false;
                            warrantyrow.Cells[4].Visible = false;
                            warrantyrow.Cells[5].Visible = false;
                        }
                        Literal lUnitPrice = (Literal)warrantyrow.FindControl("lUnitPrice");
                        lUnitPrice.Text = eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal(cartitem.warrantyItemX.UnitPrice);
                        Literal lQty = (Literal)warrantyrow.FindControl("lQty");
                        lQty.Text = cartitem.warrantyItemX.Qty.ToString();
                        Literal lAdjustedPrice = (Literal)warrantyrow.FindControl("lAdjustedPrice");
                        lAdjustedPrice.Text = eStore.Presentation.Product.ProductPrice.FormartPriceWithDecimal(cartitem.warrantyItemX.AdjustedPrice);
                    }
                    else
                    {
                        warrantyrow.Visible = false;
                    }
                }
                else
                {
                    if (
                    Presentation.eStoreContext.Current.Store.profile.ExtendedWaranties != null
                        && Presentation.eStoreContext.Current.Store.profile.ExtendedWaranties.Count > 0)
                    {
                        bool showhlAddWarranty = false;
                        switch (cartitem.type)
                        {
                            case POCOS.Product.PRODUCTTYPE.CTOS:
                              if(cartitem.partX is Product_Ctos && (cartitem.partX as Product_Ctos ).isSBCCTOS())
                                  showhlAddWarranty = true;
                                break;
                            case POCOS.Product.PRODUCTTYPE.BUNDLE:
                                showhlAddWarranty = true;
                                break;

                            case POCOS.Product.PRODUCTTYPE.STANDARD:
                            default:
                                if (cartitem.partX.isWarrantable())
                                    showhlAddWarranty = true;
                                break;
                        }

                        hlAddWarranty.Visible = showhlAddWarranty;
                        if (showhlAddWarranty)
                            hlAddWarranty.Attributes.Add("onclick", "showSelectWarrantyItemDialog('" + cartitem.ItemNo 
                                + "','" + cartitem.getWarrantableTotal() 
                                + "','" + cartitem.Qty + "')");
                    }
                    else
                    {
                        hlAddWarranty.Visible = false;
                    }
                    warrantyrow.Visible = false;
                }
                #endregion
            }
        }


        /// <summary>
        /// 是否可用TextBox
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool isAbleTextBoxOrNot(object obj)
        {
            bool isShow = false;
            if (obj != null)
                bool.TryParse(obj.ToString(), out isShow);
            return !isShow;
        }

        protected void rpBundleItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                BundleItem bi = e.Item.DataItem as BundleItem;

                if (bi != null && bi.part != null && bi.part.isWarrantyPart())
                {
                    Button btDeleteBundleItem = e.Item.FindControl("btDeleteBundleItem") as Button;
                    btDeleteBundleItem.Visible = true;
                }

                if (bi.btosX != null)
                {
                    RepeaterItem riCartItem = ((Repeater)sender).Parent as RepeaterItem;
                    CartItem cartitem = riCartItem.DataItem as CartItem;

                    eStore.Presentation.eStoreControls.Repeater rp;
                    rp = (eStore.Presentation.eStoreControls.Repeater)e.Item.FindControl("rpBTOSConfig");

                    rp.DataSource = (from bci in bi.btosX.BTOSConfigsWithoutNoneItems
                                     select new
                                     {
                                         bci.ConfigID
                                         ,
                                         bci.isBuildin
                                         ,
                                         bci.atp
                                         ,
                                         bci.CategoryComponentDesc
                                         ,
                                         bci.OptionComponentDesc
                                          ,
                                         AdjustedPrice = 
                                         (
                                          bci.BTOSConfigDetails != null && bci.BTOSConfigDetails.Count > 0 ? string.Join("<br>", (
                                         from bcd in bci.BTOSConfigDetails
                                         orderby bcd.SProductID
                                         select Presentation.Product.ProductPrice.FormartPrice(bcd.AdjustedPrice)
                                         ).ToArray()
                                         ) : string.Empty
                                         )
                                          ,
                                         Qty = bci.BTOSConfigDetails != null && bci.BTOSConfigDetails.Count > 0 ? string.Join("<br>", (
                                         from bcd in bci.BTOSConfigDetails
                                         orderby bcd.SProductID
                                         select (bcd.Qty * bci.Qty* cartitem.Qty * bi.Qty).ToString()
                                         ).ToArray()
                                         ) : (bci.Qty * cartitem.Qty).ToString()
                                         ,
                                         BTOSConfigDetails = bci.BTOSConfigDetails.OrderBy(bcd => bcd.SProductID)
                                         ,
                                         subtotal = bci.Qty * bci.AdjustedPrice * cartitem.Qty * bi.Qty
                                         ,
                                         isWarrantyPart = bci.isWarrantyConfig(),
                                         MOQ = bci.parts.Any() ? bci.parts.FirstOrDefault().Key.MininumnOrderQty : null,
                                         ABCInd = bci.parts != null && bci.parts.Any() ? bci.parts.FirstOrDefault().Key.ABCInd : ""
                                     });
                    rp.DataBind();
                }
            }
        }
        
        //是否考虑提出为控件？？
        protected void showWarranty()
        {
            var warrantyList = Presentation.eStoreContext.Current.Store.profile.ExtendedWaranties.ToList();
            if (warrantyList != null && warrantyList.Count > 0)
            {

                foreach (var wa in warrantyList)
                {
                    if (wa.partX == null)
                    {
                        //do not add the item if parts is not exists
                        if (string.IsNullOrEmpty(wa.PartNo))//None item
                        {
                            ListItem noneitem = new ListItem(string.Format(" {1}", wa.PartNo, wa.Description, wa.Rate), wa.PartNo);
                            noneitem.Attributes.Add("rate", "0");
                            rblWarranty.Items.Add(noneitem);
                        }
                    }
                    else
                    {  //change display format
                        ListItem warrantyItem = new ListItem(
                                string.Format(" {0} [{1}]"
                                 , wa.partX.VendorProductDesc
                                , string.Format(" <span class=\"priceSing\">{2}</span>{0}<span class=\"addtionprice\" ><img src=\"{1}\" /></span> "
                             , Presentation.eStoreContext.Current.Store.profile.defaultCurrency.CurrencySign
                             , ResolveUrl("~/images/priceprocessing.gif")
                             , "+"))
                            , wa.partX.SProductID);
                        warrantyItem.Attributes.Add("rate", wa.partX.getNetPrice(false).value.ToString());
                        rblWarranty.Items.Add(warrantyItem);
                    }
                }
                rblWarranty.SelectedIndex = 0;
            }
           
        }
        

        protected void btUpdateWarranty_Click(object sender, EventArgs e)
        {
            var cartItemNo = 0;
            if(int.TryParse(hfWarranyCartItem.Value,out cartItemNo))
            {
                Part warranty = null;
                if (rblWarranty.Items.Count > 0 && !string.IsNullOrEmpty(rblWarranty.SelectedValue))
                    warranty = Presentation.eStoreContext.Current.Store.getPart(rblWarranty.SelectedValue);
                if (warranty != null)
                {
                    CartItem ci = cart.getItem(cartItemNo);
                    ci.addWarranty(warranty);
                    reconcileDataSource();
                    saveDataSource();

                    //cart.updateItem(ci);
                    BindData(cart);
                }
            }
        }

        protected void rpCartContent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem" || e.CommandName == "DeleteItemWarranty")
            {
                Button btDelete = e.CommandSource as Button;
                CartItem _ci;
                _ci = cart.getItem(int.Parse(btDelete.CommandArgument));
                if (_ci == null)
                    return;

                //2017/02/23 test add to cart for ehance Ecommerce
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MeasureRemoveCart", GoogleGAHelper.MeasureRemoveCart(_ci).ToString(), true);


                //Delete dependency products
                int itemNo = int.Parse(btDelete.CommandArgument);
                List<CartItem> items = new List<CartItem>();
                foreach (var item in cart.cartItemsX)
                {
                    if (item.RelatedType.HasValue && item.RelatedType.Value == 1000) //Check if it is dependency product type.
                    {
                        if (item.RelatedItem.HasValue && item.RelatedItem.Value == itemNo) //Chekc if related item equals to item No., then delete it. 
                        {
                            CartItem c = cart.getItem(item.ItemNo);
                            if (c != null)
                                items.Add(c);
                        }
                    }
                }

                if (e.CommandName == "DeleteItem")
                {
                    cart.removeItem(_ci);
                    foreach (var item in items)
                        cart.removeItem(item);
                }
                if (e.CommandName == "DeleteItemWarranty")
                    _ci.removeWarranty();
                BindData(cart);
            }

        }

        protected void rpBundleItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteBundleItem")
            {
                Button btDelete = ((e.Item.NamingContainer as Repeater).NamingContainer as RepeaterItem).FindControl("btDelete") as Button;
                CartItem _ci;
                _ci = cart.getItem(int.Parse(btDelete.CommandArgument));

                Button btDeleteBundleItem = e.CommandSource as Button;
                int _bundleItemID = 0;
                if (int.TryParse(btDeleteBundleItem.CommandArgument, out _bundleItemID))
                    _ci.bundleX.removeItem(_bundleItemID);
                BindData(cart);
            }
        }
    }
}