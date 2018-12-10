using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;
using eStore.POCOS.DAL;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class OrderbyPartNO : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMultipleReapder(new List<POCOS.Product>());
                bindFonts();
               
            }
        }

        #region Add to Quote
        private POCOS.Part getProduct(string productID)
        {
            string ProductId = productID.Trim();

            POCOS.Part part = null;
            if (string.IsNullOrEmpty(ProductId))
                return part;
            try
            {
                part = Presentation.eStoreContext.Current.Store.getPart(ProductId);
                if (part == null)
                    part = Presentation.eStoreContext.Current.Store.addVendorPartToPart(ProductId);
            }
            catch (Exception)
            {
            }
            //POCOS.Product _product = part as POCOS.Product;
            return part;
        }

        public string ProductErrorCode { get; set; }
        private bool hasvalidpart = false;
        private bool Add2Cart(POCOS.Cart cart, string productID,int qty)
        {
            POCOS.Part _product = getProduct(productID);
            if (_product != null && _product.isOrderable(true) && !_product .isOS()&&!_product.isTOrPParts() && _product.getListingPrice().value > 0)
            {
                cart.addItem(_product, qty);
                hasvalidpart = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(ProductErrorCode))
                    ProductErrorCode += ",";
                ProductErrorCode += productID;
                hasvalidpart = false;
            }
            return hasvalidpart;
        }

        /// <summary>
        /// get all products (add to hashtable)
        /// </summary>
        /// <returns></returns>
        protected System.Collections.Hashtable getMultipleProdut()
        {
            System.Collections.Hashtable hts = new System.Collections.Hashtable();
            foreach (RepeaterItem ri in rp_table_list.Items)
            {
                TextBox tb_PartNo = ri.FindControl("tbpartno") as TextBox;
                TextBox tb_Qty = ri.FindControl("tbqty") as TextBox;
                string partNo = "";
                int qty = 0;
                if (tb_PartNo != null && tb_PartNo.Text.Trim() != "")
                {
                    partNo = tb_PartNo.Text.Trim();
                    //skip if matched OrderByPartnoExcludeRules
                    if (Presentation.eStoreContext.Current.Store.isValidOrderbyPN(partNo) == false)
                        continue;
                    if (tb_Qty.Text.Trim() != "" && int.TryParse(tb_Qty.Text.Trim(), out qty) && qty > 0)
                    {
                        if (hts.ContainsKey(partNo))
                            hts[partNo] = (int)hts[partNo] + qty;
                        else
                            hts.Add(partNo, qty);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ProductErrorCode))
                            ProductErrorCode += ",";
                        ProductErrorCode += partNo;
                        tb_PartNo.Text = string.Empty;
                        tb_Qty.Text = string.Empty;
                    }
                }
            }
            return hts;
        }

        /// <summary>
        /// first create Multiple Part list
        /// </summary>
        /// <param name="list"></param>
        void BindMultipleReapder(List<POCOS.Product> list)
        {
            for (int i = (15 - list.Count); i > 0; i--)
            {
                POCOS.Product pro = new POCOS.Product();
                list.Add(pro);
            }
            rp_table_list.DataSource = list;
            rp_table_list.DataBind();
        }
        #endregion


        protected void bt_mAddtoCart_Click(object sender, EventArgs e)
        {
            POCOS.Cart cart = Presentation.eStoreContext.Current.UserShoppingCart;
            System.Collections.Hashtable hts = getMultipleProdut();
            foreach (System.Collections.DictionaryEntry objDE in hts)
            {
                if (objDE.Key.ToString().StartsWith("SOM"))
                {
                    Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.ScriptMessage_This_product_requires_custom_effort));
                    return;
                }
                bool result = Add2Cart(cart, objDE.Key.ToString(), Convert.ToInt16(objDE.Value));
                if (result == false)
                    break;
            }

            if (!string.IsNullOrEmpty(ProductErrorCode))
            {
                object[] args = new object[1];
                args[0] = ProductErrorCode;
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product {0} cannot be added to cart.", args);
                return;
            }

            getDependencyProduct(cart);

            if (hasvalidpart)
            {
                cart.save();
                if (Presentation.eStoreContext.Current.StoreErrorCode.Count > 0)
                    Server.Transfer("~/Cart/Cart.aspx");
                else
                    this.Response.Redirect("~/Cart/Cart.aspx");
            }
            else
            {
                object[] args = new object[1];
                args[0] = ProductErrorCode;
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product {0} cannot be added to cart.", args);
                return;
            }
          
        }

        protected void bt_mAddtoQuote_Click(object sender, EventArgs e)
        {
            POCOS.Quotation quotation = Presentation.eStoreContext.Current.Quotation;
            //if (quotation.statusX != POCOS.Quotation.QStatus.Open && quotation.statusX != POCOS.Quotation.QStatus.NeedTaxIDReview)
            if (quotation.isModifiable() == false)
            {
                quotation = eStoreContext.Current.User.actingUser.openingQuote;
                Presentation.eStoreContext.Current.Quotation = quotation;
            }
            System.Collections.Hashtable hts = getMultipleProdut();
            foreach (System.Collections.DictionaryEntry objDE in hts)
            {
                bool result = Add2Cart(quotation.cartX, objDE.Key.ToString(), Convert.ToInt16(objDE.Value));
                if (result == false)
                    break;
            }

            if (!string.IsNullOrEmpty(ProductErrorCode))
            {
                object[] args = new object[1];
                args[0] = ProductErrorCode;
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product {0} cannot be added to cart.", args);
                return;
            }

            getDependencyProduct(quotation.cartX);

            if (hasvalidpart)
            {
                quotation.save();
                if (Presentation.eStoreContext.Current.StoreErrorCode.Count > 0)
                    Server.Transfer("~/Quotation/Quote.aspx");
                else
                    this.Response.Redirect("~/Quotation/Quote.aspx");
            }
            else
            {
                object[] args = new object[1];
                args[0] = ProductErrorCode;
                Presentation.eStoreContext.Current.AddStoreErrorCode("Product {0} cannot be added to cart.", args);
                return;
            }
        }

        protected void bindFonts()
        {
            bt_mAddtoQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
            bt_mAddtoCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
        }

        /// <summary>
        /// this function is using to get dependency products
        /// and add this product to cart
        /// </summary>
        /// <returns></returns>
        protected void getDependencyProduct(POCOS.Cart _cart)
        {
            Dictionary<string, int> lis = new Dictionary<string, int>();
            string[] sproductids = Request.Form["DependencyID"] != null ? Request.Form["DependencyID"].Split(',') : new string[]{};
            string[] qtys = Request.Form["DependencyQty"] != null ? Request.Form["DependencyQty"].Split(',') : new string[]{};
            if (sproductids.Length == qtys.Length)
            {
                for (int i = 0; i < sproductids.Length; i++)
                {
                    if (!string.IsNullOrEmpty(qtys[i]))
                    {
                        int qty = 0;
                        if (int.TryParse(qtys[i], out qty) && qty != 0)
                        {
                            bool result = Add2Cart(_cart, sproductids[i], qty);
                            if (result == false)
                                break;
                        }
                            
                    }
                }
            }
        }
 
    }
}