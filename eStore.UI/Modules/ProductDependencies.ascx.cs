using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules
{
    public partial class ProductDependencies : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        public bool ShowATP;
        private List<Part> _partLs;
        public List<Part> partLs
        {
            get { return _partLs; }
            set { _partLs = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bindRepeartLs();
        }

        protected void rpDependencyProducts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                POCOS.Part _relatedProduct = e.Item.DataItem as POCOS.Part;
                TextBox txtQty = e.Item.FindControl("txtQty") as TextBox;
                txtQty.Attributes.Add("sproductid", _relatedProduct.SProductID);
                if (_relatedProduct.isOrderable() == false)
                    txtQty.Enabled = false;
            }
        }

        protected void bindRepeartLs()
        {
            lbDependencyMessage.Text = string.Empty;
            if (partLs != null)
            {
                this.Visible = partLs.Count > 0;
                if (_partLs.Where(p => p.isOrderable() == false).ToList().Count > 0)
                {
                    lbDependencyMessage.Text = Presentation.eStoreLocalization.Tanslation(Store.TranslationKey.eStore_This_product_can_not_be_added);
                    btnAddDependencyToCart.Visible = false;
                    btnAddDependencyToQuote.Visible = false;
                }
                rpDependencyProducts.DataSource = _partLs;
                rpDependencyProducts.DataBind();
            }
        }

        // this function is get all select dependency products 
        public Dictionary<Part, int> getSelectDependencyProducts()
        {
            Dictionary<Part, int> ls = new Dictionary<Part, int>();
            foreach (RepeaterItem item in rpDependencyProducts.Items)
            {
                if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
                {
                    TextBox txtQty = (TextBox)item.FindControl("txtQty");
                    HiddenField hSProductID = (HiddenField)item.FindControl("hSProductID");
                    if (!string.IsNullOrWhiteSpace(txtQty.Text))
                    {
                        int result;
                        Int32.TryParse(txtQty.Text, out result);
                        if (result > 0)
                        {
                            string relatedSProductID = hSProductID.Value.ToUpper();
                            var part = partLs.FirstOrDefault(c => c.SProductID.ToUpper().Equals(relatedSProductID));
                            if (part != null && part.isOrderable(true) && part.getListingPrice().value > 0) 
                                ls.Add(part, result);
                        }

                    }
                }
            }
            return ls;
        }

        public event EventHandler btnAddDependencyToCartClicked;
        public event EventHandler btnAddDependencyToQuoteClicked;

        private void btnAddDependencyToCartClick()
        {
            if (btnAddDependencyToCartClicked != null)
            {
                btnAddDependencyToCartClicked(this, EventArgs.Empty);
            }
        }

        private void btnAddDependencyToQuoteClick()
        {
            if (btnAddDependencyToQuoteClicked != null)
            {
                btnAddDependencyToQuoteClicked(this, EventArgs.Empty);
            }
        }

        protected void btnAddDependencyToCart_Click(object sender, EventArgs e)
        {
            btnAddDependencyToCartClick();
        }

        protected void btnAddDependencyToQuote_Click(object sender, EventArgs e)
        {
            btnAddDependencyToQuoteClick();
        }

    }
}