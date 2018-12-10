using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    [Serializable]
    public partial class CartContentPreview : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public POCOS.Cart cart
        {
            set
            {
                if (value == null)
                    return;

                if (showATP)
                    value.prefetchCartItemATP();
                this.dlCartContent.DataSource = value.cartItemsX;
                this.dlCartContent.DataBind();
            }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void dlCartContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.CartItem cartitem = (POCOS.CartItem)e.Item.DataItem;  
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
                                             select Presentation.Product.ProductPrice.FormartPrice(bcd.AdjustedPrice, cartitem.currencySign)
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
                                         subtotal = cartitem.Qty * bci.Qty * bci.AdjustedPrice,
                                         ABCInd = bci.parts != null && bci.parts.Any() ? bci.parts.FirstOrDefault().Key.ABCInd : "",
                                         currencySign = cartitem.currencySign
                                     });
                    rp.DataBind();
                }
                if (cartitem.bundleX != null)
                {
                    eStore.Presentation.eStoreControls.Repeater rpBundleItem = (eStore.Presentation.eStoreControls.Repeater)e.Item.FindControl("rpBundleItem");

                    rpBundleItem.DataSource = cartitem.bundleX.BundleItems;
                    rpBundleItem.DataBind();
                }

                HtmlTableRow warrantyrow = (HtmlTableRow)e.Item.FindControl("warrrantyItem");
                
                if (cartitem.warrantyItemX != null)
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
        }
        protected void rpBundleItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
               

                BundleItem bi = e.Item.DataItem as BundleItem;
                if (bi.btosX != null)
                {
                    RepeaterItem riCartItem = ((Repeater)sender).Parent as RepeaterItem;
                    CartItem cartitem = riCartItem.DataItem as CartItem;
                     
                    eStore.Presentation.eStoreControls.Repeater rp = (eStore.Presentation.eStoreControls.Repeater)e.Item.FindControl("rpBTOSConfig");
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
                                         select Presentation.Product.ProductPrice.FormartPrice(bcd.AdjustedPrice, cartitem.currencySign)
                                         ).ToArray()
                                         ) : string.Empty
                                         )
                                          ,
                                         Qty = bci.BTOSConfigDetails != null && bci.BTOSConfigDetails.Count > 0 ? string.Join("<br>", (
                                         from bcd in bci.BTOSConfigDetails
                                         orderby bcd.SProductID
                                         select (bcd.Qty * bci.Qty * cartitem.Qty * bi.Qty).ToString()
                                         ).ToArray()
                                         ) : (bci.Qty * cartitem.Qty).ToString()
                                         ,
                                         BTOSConfigDetails = bci.BTOSConfigDetails.OrderBy(bcd => bcd.SProductID)
                                         ,
                                         subtotal = cartitem.Qty * bci.Qty * bci.AdjustedPrice * bi.Qty,
                                         ABCInd = bci.parts != null && bci.parts.Any() ? bci.parts.FirstOrDefault().Key.ABCInd : "",
                                         currencySign = cartitem.currencySign
                                     });
                    rp.DataBind();
                }
            }
        }
    }
}