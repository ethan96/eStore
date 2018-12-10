using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation.Product
{
    public class QuoteCartCompareManager
    {
        List<partXX> _lists = null;

        public QuoteCartCompareManager(POCOS.Part part,int qty = 1)
        {
            _lists = new List<partXX>();
            _lists.Add(new partXX(part, qty));
        }
        public QuoteCartCompareManager(List<POCOS.Part> list)
        {
            _lists = new List<partXX>();
            foreach (POCOS.Part p in list)
            {
                _lists.Add(new partXX(p));
            }
        }


        public void AddPart(POCOS.Part part, int qty)
        {
            _lists.Add(new partXX(part, qty));
        }


        public void AddToCart()
        {
            POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
            foreach (partXX p in _lists)
            {
                Add2Cart(cart, p);
            }
            cart.save();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            //    this.Response.Redirect("~/Cart/Cart.aspx");
        }

        public void AddToQuote()
        {
            POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
            if (quotation.statusX != POCOS.Quotation.QStatus.Open)
            {
                quotation = eStoreContext.Current.User.actingUser.openingQuote;
                Presentation.eStoreContext.Current.Quotation = quotation;
            }
            foreach (partXX p in _lists)
            {
                Add2Cart(quotation.cartX,p);
            }
            quotation.save();
            //this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        public void AddToCompare()
        {
            List<string> selectProductList = new List<string>();
            selectProductList = (from p in _lists
                                 select p._part.SProductID).ToList<string>();
            ProductCompareManagement.setProductIDs(selectProductList);
        }






        protected void Add2Cart(POCOS.Cart cart, partXX partxx)
        {
            POCOS.Product pr = (POCOS.Product)partxx._part;
            if (pr.notAvailable || pr.getListingPrice().value < 1)
                return;
            else if (pr is POCOS.Product_Ctos)
            {
                POCOS.Product_Ctos ctos = (eStore.POCOS.Product_Ctos)pr;
                POCOS.BTOSystem newbtos = ctos.getDefaultBTOS();
                ctos.updateBTOSPrice(newbtos);
                cart.addItem(ctos, 1, newbtos);
            }
            else
            {
                if(cart.compareOrderMOQ(partxx._part,partxx._qyt))
                    cart.addItem(pr, partxx._qyt);
                else
                    cart.addItem(pr, partxx._part.MininumnOrderQty.Value);
            }

            //cart.save();
            //查看是否需要添加周边产品
            // Function
        }

    }

    /// <summary>
    /// 储存Product 和 Qty
    /// </summary>
    public class partXX
    {
        public POCOS.Part _part { get; set; }
        public int _qyt { get; set; }
        public partXX(POCOS.Part p,int qty = 1)
        {
            _part = p;
            _qyt = qty;
        }
    }

}
