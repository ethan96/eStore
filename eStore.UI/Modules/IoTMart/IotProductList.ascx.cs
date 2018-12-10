using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.IoTMart
{
    public partial class IotProductList : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected string catePath = "";
        private List<POCOS.ProductCategory> _cateList = new List<POCOS.ProductCategory>();

        public POCOS.ProductCategory category
        {
            set
            {
                if (value.childCategoriesX.Any())
                    _cateList = value.childCategoriesX.ToList();
                else
                    _cateList.Add(value);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            BindScript("url", "compareStyle", "iot.compareStyle.js");
            rpCategories.DataSource = _cateList;
            rpCategories.DataBind();
            if (!IsPostBack)
            {
                bindFont();
            }
        }

        protected void rpCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                POCOS.ProductCategory pc = e.Item.DataItem as POCOS.ProductCategory;
                if (pc != null && pc.productList.Any())
                {
                    catePath = pc.CategoryPath;
                    Repeater rpProducts = e.Item.FindControl("rpProducts") as Repeater;
                    rpProducts.DataSource = pc.productList.OrderBy(c => c.CategorySeq).ThenBy(c => c.LastUpdated).ToList();
                    rpProducts.DataBind();

                    PaginationProductList ppList = e.Item.FindControl("ppList") as PaginationProductList;
                    ppList.category = pc;

                    Button bt_AddtoCart = e.Item.FindControl("bt_AddtoCart") as Button;
                    bt_AddtoCart.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_AddToCart);
                    Button bt_AddtoQuote = e.Item.FindControl("bt_AddtoQuote") as Button;
                    bt_AddtoQuote.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_AddToQuotation);

                }
            }
        }

        protected string getCompareStr(object product)
        {
            POCOS.Product pc = product as POCOS.Product;
            if (pc != null)
                return string.Format("<input type=\"checkbox\" class=\"iot-checkbox\" name=\"compare\" id='ck{0}_{1}' value=\"{1}\"><label for=\"ck{0}_{1}\"> {2}</label>"
                    , catePath, pc.SProductID, eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Product_Compare));
            else
                return "";
        }

        /// <summary>
        /// get all select product
        /// </summary>
        /// <returns></returns>
        public List<POCOS.Product> getListSlectProductNo()
        {
            List<POCOS.Product> ls = new List<POCOS.Product>();

            string requestproducts = Request["cbproduct"];
            if (!string.IsNullOrEmpty(requestproducts))
            {
                foreach (string pid in requestproducts.Split(','))
                {
                    POCOS.Product product = Presentation.eStoreContext.Current.Store.getProduct(pid);
                    if (product != null)
                        ls.Add(product);
                }
            }
            return ls;
        }

        protected void btCompare_Click(object sender, EventArgs e)
        {
            List<string> ls = new List<string>();
            string requestproducts = Request["compare"];
            if (!string.IsNullOrEmpty(requestproducts))
                ls = requestproducts.Split(',').ToList();
            if (!ls.Any())
            {
                eStore.Presentation.eStoreContext.Current.AddStoreErrorCode(eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_select_product_first));
                return;
            }
            eStore.Presentation.Product.ProductCompareManagement.setProductIDs(ls);
            Response.Redirect("~/Compare.aspx");
        }

        protected void bt_AddtoQuote_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getListSlectProductNo();
            eStore.Presentation.Product.QuoteCartCompareManager qcc = new eStore.Presentation.Product.QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToQuote();
            this.Response.Redirect("~/Quotation/Quote.aspx");
        }

        protected void bt_AddtoCart_Click(object sender, EventArgs e)
        {
            if (Presentation.eStoreContext.Current.User == null)
                popLoginDialog(sender);
            List<POCOS.Product> selectProductList = getListSlectProductNo();
            eStore.Presentation.Product.QuoteCartCompareManager qcc = new eStore.Presentation.Product.QuoteCartCompareManager(selectProductList.ToList<POCOS.Part>());
            qcc.AddToCart();
            this.Response.Redirect("~/Cart/Cart.aspx");
        }

        protected void bindFont()
        {
            btCompare.Text = eStore.Presentation.eStoreLocalization.Tanslation(eStore.POCOS.Store.TranslationKey.Iot_Compare);
        }
    }
}