using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.Presentation.Product;
using eStore.Presentation;

namespace eStore.UI.Modules
{
    public partial class ProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public IEnumerable<POCOS.Product> productList { get; set; }
        public string target = "";
        public bool isToeStore = false;

        private int _pagesize = 20;
        private string _ResultsLocationType = "Both";
        public string ResultsLocationType
        {
            get { return _ResultsLocationType; }
            set { _ResultsLocationType = value; }
        }

        public int pageSize
        {
            get { return _pagesize; }
            set { _pagesize = value; }
        }

        /// <summary>
        /// if false will sort by isOrderable , not PHASED_OUT, name desc 
        /// </summary>
        public bool KeepOriginalSequence { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            BindProductTable();
            // add below to avoid js error: caused by AjaxValidationSummary 
            this.QuantityDiscount.Visible = !string.IsNullOrEmpty(eStore.Presentation.eStoreContext.Current.getStringSetting("SOMSalesPhoneNumber"));
            GVProductList1.target = target;
            GVProductList1.isToeStore = isToeStore;
        }

        public void BindProductTable()
        {
            bindBaseInfor();
            bindFonts();
        }

        protected void bindBaseInfor()
        {
            if (productList != null && productList.Count() > 0)
            {
                bt_Compare.Visible = true;
                bt_AddtoCart.Visible = true;
                bt_AddtoQuote.Visible = true;
                GVProductList1.productList = productList.ToList();
                GVProductList1.pageSize = _pagesize;
                GVProductList1.KeepOriginalSequence = KeepOriginalSequence;
                GVProductList1.ResultsLocationType = _ResultsLocationType;
            }
            else
            {
                bt_Compare.Visible = false;
                bt_AddtoCart.Visible = false;
                bt_AddtoQuote.Visible = false;
            }
        }

        protected void bt_AddtoQuote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToQuote();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void bt_AddtoCart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getSlectProductNo();
            QuoteCartCompareManager qcc = new QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToCart();
            //if (Presentation.eStoreContext.Current.Store.storeID == "AEU")
            //    this.Response.Redirect("~/Cart/ChannelPartner.aspx");
            //else
            this.Response.Redirect("~/Cart/Cart.aspx");
        }

        protected void bt_Compare_Click(object sender, EventArgs e)
        {
            List<string> selectProductList = getStrSlectProductNo();
            ProductCompareManagement.setProductIDs(selectProductList);
            Response.Redirect("~/Compare.aspx");
        }


        /// <summary>
        /// get all select product
        /// </summary>
        /// <returns></returns>
        protected List<POCOS.Product> getSlectProductNo()
        {
            return GVProductList1.getSlectProductNo(); ;
        }

        protected List<string> getStrSlectProductNo()
        {
            return GVProductList1.getStrSlectProductNo();
        }
        protected void bindFonts()
        {
            bt_Compare.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare);
            bt_AddtoCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoCart);
            bt_AddtoQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_AddtoQuotation);
        }
        
    }
}