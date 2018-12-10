using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation
{
    /// <summary>
    /// fix cart view status and message
    /// </summary>
    public class CartUtility
    {
        public static string CheckAndMegerCartItem(POCOS.Cart cart, bool showCartMessage = true)
        {
            if (showCartMessage && cart != null)
            {
                if (cart.cartItemChangedMessage != null && cart.cartItemChangedMessage.Count > 0)
                {
                    string unavailableformat = eStore.Presentation.eStoreLocalization.Tanslation("cart_item_has_become_unavailable");
                    string pricechangedformat = eStore.Presentation.eStoreLocalization.Tanslation("cart_item_price_has_been_changed");
                       
                    System.Text.StringBuilder sbMessages = new System.Text.StringBuilder();
                        
                    sbMessages.Append("<ol  class=\"cartItemChangedMessage\">");

                    foreach (var message in cart.cartItemChangedMessage)
                    {
                        if (message is CartItemChangedMessage_Unavailable)
                        {
                            sbMessages.Append("<li>");
                            CartItemChangedMessage_Unavailable unavailablemsg = (CartItemChangedMessage_Unavailable)message;
                            sbMessages.AppendFormat(unavailableformat, unavailablemsg.Name);
                            sbMessages.Append("</li>");
                        }
                        else if (message is CartItemChangedMessage_PriceChanged)
                        {
                            sbMessages.Append("<li>");
                            CartItemChangedMessage_PriceChanged PriceChangedmsg = (CartItemChangedMessage_PriceChanged)message;
                            sbMessages.AppendFormat(pricechangedformat, PriceChangedmsg.Name
                            , Presentation.Product.ProductPrice.FormartPriceWithCurrency(PriceChangedmsg.OldPrice)
                            , Presentation.Product.ProductPrice.FormartPriceWithCurrency(PriceChangedmsg.NewPrice)
                                );
                            sbMessages.Append("</li>");
                        }
                    }
                    sbMessages.Append("</ol>");
                    sbMessages.AppendFormat("If you have questions, please click to Chat or call us at {0} to talk to a specialist.",
                                                            Presentation.eStoreContext.Current.CurrentAddress.Tel);
                      
                    return sbMessages.ToString();
                }
            }
            return "";
        }
    }
}
